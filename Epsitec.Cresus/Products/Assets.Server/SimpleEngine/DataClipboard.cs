//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataClipboard
	{
		public DataClipboard()
		{
			this.objects = new Dictionary<BaseType, Data> ();
		}


		#region Objects
		public bool HasObject(BaseType baseType)
		{
			//	Indique s'il existe un objet dans le clipboard.
			return this.objects.ContainsKey (baseType);
		}

		public void CopyObject(DataAccessor accessor, BaseType baseType, DataObject obj, Timestamp? timestamp = null)
		{
			//	Copie un objet dans le clipboard.
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
				objCopy = this.CopyBaseObject (accessor, baseType, obj);
			}

			this.objects[baseType] = new Data (accessor.Mandat.Guid, objCopy);
		}

		private DataObject CopyAssetObject(DataAccessor accessor, DataObject obj, Timestamp? timestamp)
		{
			var e = obj.GetEvent (0);

			var objCopy = new DataObject ();
			var eventCopy = new DataEvent (e.Timestamp, e.Type);
			objCopy.AddEvent (eventCopy);

			foreach (var field in accessor.AssetValueFields)
			{
				var p = ObjectProperties.GetObjectSyntheticProperty (obj, timestamp, field);

				if (p != null)
				{
					eventCopy.AddProperty (p);
				}
			}

			return objCopy;
		}

		private DataObject CopyBaseObject(DataAccessor accessor, BaseType baseType, DataObject obj)
		{
			var e = obj.GetEvent (0);

			//	On conserve une copie de l'objet.
			var objCopy = new DataObject ();
			var eventCopy = new DataEvent (e);  // copie l'événement et toutes ses propriétés
			objCopy.AddEvent (eventCopy);

			return objCopy;
		}

		public DataObject PasteObject(DataAccessor accessor, BaseType baseType, System.DateTime? date = null)
		{
			//	Colle l'objet contenu dans le clipboard.
			if (!this.objects.ContainsKey (baseType))
			{
				return null;
			}

			var data = this.objects[baseType];

			if (accessor.Mandat.Guid != data.Guid)  // colle dans un autre mandat ?
			{
				return null;
			}

			var field = accessor.GetMainStringField (baseType);
			var name = ObjectProperties.GetObjectPropertyString(data.Object, null, field);
			name = DataClipboard.GetCopyName (name);

			if (!date.HasValue)
			{
				date = accessor.Mandat.StartDate;
			}

			var guid = accessor.CreateObject (baseType, date.Value, name, Guid.Empty);
			var objPaste = accessor.GetObject (baseType, guid);
			var eventPaste = objPaste.GetEvent (0);
			eventPaste.SetUndefinedProperties (data.Object.GetEvent (0));

			return objPaste;
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
			var eventCopy = new DataEvent (e.Timestamp, e.Type);
			eventCopy.SetProperties (e);
			
			this.dataEvent = eventCopy;
		}

		public DataEvent PasteEvent(DataAccessor accessor, DataObject obj, System.DateTime date)
		{
			//	Colle l'événement du clipboard à l'objet donné.
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


		private static string GetCopyName(string name)
		{
			//	A partir de "Toto", retourne "Copie de Toto".
			if (string.IsNullOrEmpty (name))
			{
				return "Copie";
			}
			else
			{
				return string.Format ("Copie de {0}", name);
			}
		}


		private struct Data
		{
			public Data(Guid guid, DataObject obj)
			{
				this.Guid   = guid;
				this.Object = obj;
			}

			public readonly Guid					Guid;
			public readonly DataObject				Object;
		}


		private readonly Dictionary<BaseType, Data>	objects;

		private Guid								eventGuidMandat;
		private DataEvent							dataEvent;
	}
}
