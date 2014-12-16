//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataClipboard
	{
		public DataClipboard()
		{
			this.objects = new Dictionary<BaseType, ClipboardObject> ();
			this.userFields = new Dictionary<BaseType, ClipboardUserField> ();
		}


		#region Objects
		public bool HasObject(BaseType baseType)
		{
			//	Indique s'il existe un objet dans le clipboard.
			return this.objects.ContainsKey (baseType);
		}

		public string GetObjectSummary(BaseType baseType)
		{
			//	Retourne le résumé de l'objet copié dans le clipboard.
			ClipboardObject data;
			if (this.objects.TryGetValue (baseType, out data))
			{
				return data.Summary;
			}
			else
			{
				return null;
			}
		}

		public void CopyObject(DataAccessor accessor, BaseType baseType, DataObject obj, Timestamp? timestamp = null)
		{
			//	Copie un objet dans le clipboard. Pour un objet d'immobilisation, on indique le
			//	timestamp de l'état à copier.
			if (obj == null)
			{
				return;
			}

			DataObject objCopy;

			if (baseType == BaseType.Assets)
			{
				objCopy = this.CopyAssetObject (accessor, obj, timestamp);
			}
			else
			{
				objCopy = this.CopyBaseObject (obj);
			}

			var summary = this.GetObjectSummary (accessor, baseType, obj, timestamp);

			this.objects[baseType] = new ClipboardObject (accessor.Mandat.Guid, objCopy, summary);
		}

		private string GetObjectSummary(DataAccessor accessor, BaseType baseType, DataObject obj, Timestamp? timestamp)
		{
			switch (baseType.Kind)
			{
				case BaseTypeKind.Assets:
					return AssetsLogic.GetSummary (accessor, obj.Guid, timestamp);

				case BaseTypeKind.Persons:
					return PersonsLogic.GetSummary (accessor, obj.Guid);

				default:
					return ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.Name);
			}
		}

		private DataObject CopyAssetObject(DataAccessor accessor, DataObject obj, Timestamp? timestamp)
		{
			//	Copie un object d'immobilisation.
			System.Diagnostics.Debug.Assert (timestamp.HasValue);

			var objCopy = new DataObject (null);
			var eventCopy = new DataEvent (null, timestamp.Value, EventType.Input);
			objCopy.AddEvent (eventCopy);

			foreach (var field in accessor.AssetFields)
			{
				var p = ObjectProperties.GetObjectSyntheticProperty (obj, timestamp, field);

				if (p != null)
				{
					eventCopy.AddProperty (p);
				}
			}

			return objCopy;
		}

		private DataObject CopyBaseObject(DataObject obj)
		{
			//	Copie un objet sans timeline, c'est-à-dire tous les objets sauf ceux d'immobilisation.
			var e = obj.GetInputEvent ();

			//	On conserve une copie de l'objet.
			var objCopy = new DataObject (null);
			var eventCopy = new DataEvent (null, e);  // copie l'événement et toutes ses propriétés
			objCopy.AddEvent (eventCopy);

			return objCopy;
		}

		public DataObject PasteObject(DataAccessor accessor, BaseType baseType, System.DateTime? inputDate = null)
		{
			//	Colle l'objet contenu dans le clipboard. Pour un object d'immobilisation, on
			//	indique sa date d'entrée.
			if (!this.objects.ContainsKey (baseType))  // clipboard vide ?
			{
				return null;
			}

			var data = this.objects[baseType];

			if (accessor.Mandat.Guid != data.MandatGuid)  // colle dans un autre mandat ?
			{
				return null;
			}

			var field = accessor.GetMainStringField (baseType);
			var name = ObjectProperties.GetObjectPropertyString(data.Object, null, field);
			name = DataClipboard.GetCopyName (name, accessor.GlobalSettings.CopyNameStrategy);

			if (!inputDate.HasValue)
			{
				inputDate = accessor.Mandat.StartDate;
			}

			//	On insère l'objet collé.
			var guid = accessor.CreateObject (baseType, inputDate.Value, name, Guid.Empty);
			var objPaste = accessor.GetObject (baseType, guid);
			var eventPaste = objPaste.GetInputEvent ();
			eventPaste.SetUndefinedProperties (data.Object.GetInputEvent ());

			if (baseType == BaseType.Assets)
			{
				this.SetMainValue (accessor, data.Object.GetInputEvent (), objPaste, inputDate.Value);
			}

			return objPaste;
		}

		private void SetMainValue(DataAccessor accessor, DataEvent modelEvent, DataObject objPaste, System.DateTime inputDate)
		{
			//	Copie la valeur comptable. Elle ne doit pas du tout être copiée telle qu'elle.
			//	Par exemple, il peut s'agit d'un amortissement dans la source, qui sera une
			//	valeur fixe (achat) dans la destination.
			var modelProperty = modelEvent.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

			if (modelProperty != null)
			{
				var eventPaste = objPaste.GetInputEvent ();

				var init = modelProperty.Value.FinalAmount;
				var aa = new AmortizedAmount (init, init, null, null, EntryScenario.Purchase, Guid.Empty, 0);

				aa = Entries.CreateEntry (accessor, objPaste, eventPaste, aa);  // génère ou met à jour les écritures
				Amortizations.SetAmortizedAmount (eventPaste, aa);
			}
		}
		#endregion


		#region Events
		public bool								HasEvent
		{
			//	Indique s'il existe un événement dans le clipboard.
			get
			{
				return this.dataEvent != null;
			}
		}

		public Timestamp?						EventTimestamp
		{
			//	Retourne le timestamp de l'événement dans le clipboard.
			get
			{
				if (this.dataEvent == null)
				{
					return null;
				}
				else
				{
					return this.dataEvent.Timestamp;
				}
			}
		}

		public EventType						EventType
		{
			//	Retourne le type de l'événement dans le clipboard.
			get
			{
				if (this.dataEvent == null)
				{
					return EventType.Unknown;
				}
				else
				{
					return this.dataEvent.Type;
				}
			}
		}

		public void CopyEvent(DataAccessor accessor, DataEvent e)
		{
			//	Copie un événement dans le clipboard.
			this.eventGuidMandat = accessor.Mandat.Guid;

			//	On conserve une copie de l'événement.
			var eventCopy = new DataEvent (null, e.Timestamp, e.Type);
			eventCopy.SetProperties (e);
			
			this.dataEvent = eventCopy;
		}

		public DataEvent PasteEvent(DataAccessor accessor, DataObject obj, System.DateTime date)
		{
			//	Colle l'événement du clipboard à l'objet donné.
			if (this.dataEvent == null)  // clipboard vide ?
			{
				return null;
			}

			if (accessor.Mandat.Guid != this.eventGuidMandat)  // colle dans un autre mandat ?
			{
				return null;
			}

			var e = accessor.CreateAssetEvent (obj, date, this.dataEvent.Type);
			e.SetUndefinedProperties (this.dataEvent);

			Amortizations.UpdateAmounts (accessor, obj);

			return e;
		}
		#endregion


		#region UserFields
		public bool HasUserField(BaseType baseType)
		{
			//	Indique s'il existe un UserField dans le clipboard.
			return this.userFields.ContainsKey (baseType);
		}

		public void CopyUserField(DataAccessor accessor, BaseType baseType, UserField userField)
		{
			//	Copie un UserField dans le clipboard.
			var copy = new UserField (userField);
			var data = new ClipboardUserField (accessor.Mandat.Guid, copy);

			this.userFields[baseType] = data;
		}

		public UserField PasteUserField(DataAccessor accessor, BaseType baseType, int order)
		{
			//	Colle le UserField contenu dans le clipboard.
			if (!this.userFields.ContainsKey (baseType))  // clipboard vide ?
			{
				return UserField.Empty;
			}

			var data = this.userFields[baseType];

			if (accessor.Mandat.Guid != data.MandatGuid)  // colle dans un autre mandat ?
			{
				return UserField.Empty;
			}

			//	On insère le UserField Collé.
			var field = accessor.UserFieldsAccessor.GetNewUserField ();
			var name = DataClipboard.GetCopyName (data.UserField.Name, accessor.GlobalSettings.CopyNameStrategy);
			var userField = new UserField (data.UserField, field, name);
			accessor.UserFieldsAccessor.AddUserField (baseType, userField);
			accessor.UserFieldsAccessor.ChangeOrder (baseType, userField, order);
			accessor.WarningsDirty = true;

			return userField;
		}
		#endregion


		public static string GetCopyName(string name, CopyNameStrategy strategy)
		{
			//	A partir de "Toto", retourne "Copie de Toto" (par exemple).
			if (string.IsNullOrEmpty (name))
			{
				return Res.Strings.DataClipboard.Copy.ToString ();
			}
			else
			{
				switch (strategy)
				{
					case CopyNameStrategy.NameDashCopy:
						return string.Format (Res.Strings.DataClipboard.NameDashCopy.ToString (), name);

					case CopyNameStrategy.NameBracketCopy:
						return string.Format (Res.Strings.DataClipboard.NameBracketCopy.ToString (), name);

					default:
						return string.Format (Res.Strings.DataClipboard.CopyOfName.ToString (), name);
				}
			}
		}


		private struct ClipboardObject
		{
			public ClipboardObject(Guid mandatGuid, DataObject obj, string summary)
			{
				this.MandatGuid = mandatGuid;
				this.Object     = obj;
				this.Summary    = summary;
			}

			public readonly Guid					MandatGuid;
			public readonly DataObject				Object;
			public readonly string					Summary;
		}

		private struct ClipboardUserField
		{
			public ClipboardUserField(Guid mandatGuid, UserField userField)
			{
				this.MandatGuid = mandatGuid;
				this.UserField  = userField;
			}

			public readonly Guid					MandatGuid;
			public readonly UserField				UserField;
		}


		private readonly Dictionary<BaseType, ClipboardObject>		objects;
		private readonly Dictionary<BaseType, ClipboardUserField>	userFields;

		private Guid								eventGuidMandat;
		private DataEvent							dataEvent;
	}
}
