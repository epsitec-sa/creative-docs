﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public partial class DataAccessor
	{
		public DataAccessor()
		{
			this.editionAccessor = new EditionAccessor (this);
		}

		public DataMandat Mandat
		{
			get
			{
				return this.mandat;
			}
			set
			{
				this.mandat = value;

				//	Recalcule tout.
				foreach (var obj in this.mandat.GetData (BaseType.Assets))
				{
					Amortizations.UpdateAmounts (this, obj);
				}
			}
		}

		public GlobalSettings GlobalSettings
		{
			get
			{
				return this.mandat.GlobalSettings;
			}
		}

		public static int Simulation;


		public System.DateTime StartDate
		{
			get
			{
				return this.mandat.StartDate;
			}
		}

		public EditionAccessor EditionAccessor
		{
			get
			{
				return this.editionAccessor;
			}
		}

		public AbstractNodeGetter<GuidNode> GetNodeGetter(BaseType baseType)
		{
			//	Retourne un moyen standardisé d'accès en lecture aux données d'une base.
			return new GuidNodeGetter (this.mandat, baseType);
		}


		#region Objects
		public DataObject GetObject(BaseType baseType, Guid objectGuid)
		{
			if (baseType == BaseType.UserFields)
			{
				return this.GlobalSettings.GetTempDataObject (objectGuid);
			}
			else
			{
				return this.mandat.GetData (baseType)[objectGuid];
			}
		}

		public Guid CreateObject(BaseType baseType, System.DateTime date, string name, Guid parent)
		{
			var obj = new DataObject ();
			this.mandat.GetData (baseType).Add (obj);

			var timestamp = new Timestamp (date, 0);
			var e = new DataEvent (timestamp, EventType.Input);
			obj.AddEvent (e);

			//	Ajoute la date de l'opération.
			this.AddDateOperation (e);

			//	Ajoute l'objet parent.
			if (!parent.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, parent));
			}

			//	Ajoute le nom de l'objet.
			var field = this.GetMainStringField (baseType);
			e.AddProperty (new DataStringProperty (field, name));

			//	Ajoute la valeur comptable.
			if (baseType == BaseType.Assets)
			{
				this.AddMainValue (obj, timestamp, e);
			}

			return obj.Guid;
		}

		public void ChangeAssetEventTimestamp(DataObject obj, DataEvent e, Timestamp timestamp)
		{
			//	Change la date d'un événement, sans aucun grade-fou "métier". Par exemple, on
			//	peut déplacer l'événement d'entrée après le premier amortissement !
			//	La modification de la date nécessite de créer une copie de l'événement, dont
			//	on ne changera que la date.
			obj.RemoveEvent (e);

			var newEvent = new DataEvent (timestamp, e.Type);
			newEvent.SetProperties (e);
			obj.AddEvent (newEvent);
		}

		public DataEvent CreateAssetEvent(DataObject obj, System.DateTime date, EventType type)
		{
			if (obj != null)
			{
				if (type == EventType.Locked)
				{
					//	Il ne peut y avoir qu'un seul verrou par objet.
					AssetCalculator.RemoveLockedEvent (obj);
				}

				var position = obj.GetNewPosition (date);
				var ts = new Timestamp (date, position);
				var e = new DataEvent (ts, type);
				obj.AddEvent (e);

				this.AddDateOperation (e);
				this.AddMainValue (obj, ts, e);

				Amortizations.UpdateAmounts (this, obj);
				return e;
			}

			return null;
		}

		private void AddDateOperation(DataEvent e)
		{
			//	Ajoute la date du jour comme date valeur.
			var p = new DataDateProperty (ObjectField.OneShotDateOperation, Timestamp.Now.Date);
			e.AddProperty (p);
		}

		private void AddMainValue(DataObject obj, Timestamp timestamp, DataEvent e)
		{
			//	La valeur comptable est créée une bonne fois pour toutes.
			//	On l'initialise avec les paramètres d'amortissement.
			if (e.Type == EventType.Modification ||
				e.Type == EventType.Locked)
			{
				//	Pour ces seuls types, il n'y a pas et n'y aura jamais de valeur comptable.
				return;
			}

			var amortizationType = AmortizationType.Unknown;
			var entryScenario    = EntryScenario.None;

			switch (e.Type)
			{
				case EventType.Input:
					amortizationType = AmortizationType.Unknown;  // montant fixe
					entryScenario    = EntryScenario.Purchase;
					break;

				case EventType.Revaluation:
				case EventType.Revalorization:
				case EventType.MainValue:
					amortizationType = AmortizationType.Unknown;  // montant fixe
					entryScenario    = EntryScenario.Revaluation;
					break;

				case EventType.AmortizationAuto:
				case EventType.AmortizationPreview:
					amortizationType = AmortizationType.Linear;
					entryScenario    = EntryScenario.AmortizationAuto;
					break;

				case EventType.AmortizationExtra:
					amortizationType = AmortizationType.Linear;
					entryScenario    = EntryScenario.AmortizationExtra;
					break;

				case EventType.Output:
					amortizationType = AmortizationType.Unknown;  // montant fixe
					entryScenario    = EntryScenario.Sale;
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown EventType {0}", e.Type.ToString ()));
			}

			var aa = Amortizations.InitialiseAmortizedAmount (obj, e, timestamp, amortizationType, entryScenario);
			Amortizations.SetAmortizedAmount (e, aa);
		}


		public void RemoveObject(BaseType baseType, Guid objectGuid)
		{
			var list = this.mandat.GetData (baseType);
			var obj = this.GetObject (baseType, objectGuid);

			if (obj != null)
			{
				list.Remove (obj);
			}
		}

		public void RemoveObject(BaseType baseType, DataObject obj)
		{
			var list = this.mandat.GetData (baseType);
			list.Remove (obj);
		}

		public void RemoveObjectEvent(DataObject obj, Timestamp? timestamp)
		{
			if (obj != null && timestamp.HasValue)
			{
				var e = obj.GetEvent (timestamp.Value);
				if (e != null)
				{
					this.RemoveObjectEvent (obj, e);
				}
			}
		}

		public void RemoveObjectEvent(DataObject obj, DataEvent e)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			if (p != null)
			{
				var aa = Entries.RemoveEntry(this, p.Value);
				Amortizations.SetAmortizedAmount (e, aa);
			}

			obj.RemoveEvent (e);
			Amortizations.UpdateAmounts (this, obj);
		}


		public void CopyObject(DataObject obj, DataObject model)
		{
			//	Copie dans 'obj' toutes les propriétés de 'model' que 'obj' n'a pas déjà.
			//	Ne fonctionne que pour les objets sans timeline (donc tous sauf les Assets),
			//	c'est-à-dire les objets qui n'ont qu'un seul événement.
			System.Diagnostics.Debug.Assert (  obj.EventsCount == 1);
			System.Diagnostics.Debug.Assert (model.EventsCount == 1);

			var dstEvent =   obj.GetEvent (0);  // événement unique de l'objet
			var srcEvent = model.GetEvent (0);  // événement unique de l'objet

			dstEvent.SetUndefinedProperties (srcEvent);
		}
		#endregion


		public static bool IsOneShotField(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.OneShotNumber:
				case ObjectField.OneShotDateEvent:
				case ObjectField.OneShotDateOperation:
				case ObjectField.OneShotComment:
				case ObjectField.OneShotDocuments:
					return true;

				default:
					return false;
			}
		}


		public ObjectField GetMainStringField(BaseType baseType)
		{
			if (baseType == BaseType.Assets ||
				baseType == BaseType.Persons)
			{
				return this.GlobalSettings.GetMainStringField (baseType);
			}
			else
			{
				return ObjectField.Name;
			}
		}


		public static IEnumerable<ObjectField> GroupGuidRatioFields
		{
			get
			{
				for (int i=0; i<=ObjectField.GroupGuidRatioLast-ObjectField.GroupGuidRatioFirst; i++)
				{
					yield return ObjectField.GroupGuidRatioFirst+i;
				}
			}
		}

		public IEnumerable<ObjectField> ValueFields
		{
			get
			{
				yield return ObjectField.MainValue;

				foreach (var field in this.GlobalSettings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.ComputedAmount)
					.Select (x => x.Field))
				{
					yield return field;
				}
			}
		}


		public string GetFieldName(ObjectField objectField)
		{
			if (objectField >= ObjectField.UserFieldFirst &&
				objectField <= ObjectField.UserFieldLast)
			{
				return this.GlobalSettings.GetUserFieldName (objectField);
			}

			return DataDescriptions.GetObjectFieldDescription (objectField);
		}

		public FieldType GetFieldType(ObjectField objectField)
		{
			if (objectField >= ObjectField.UserFieldFirst &&
				objectField <= ObjectField.UserFieldLast)
			{
				return this.GlobalSettings.GetUserFieldType (objectField);
			}

			if (objectField >= ObjectField.GroupGuidRatioFirst &&
				objectField <= ObjectField.GroupGuidRatioLast)
			{
				return FieldType.GuidRatio;
			}

			switch (objectField)
			{
				case ObjectField.MainValue:
					return FieldType.AmortizedAmount;

				case ObjectField.AmortizationRate:
				case ObjectField.ResidualValue:
				case ObjectField.Round:
				case ObjectField.EntryAmount:
					return FieldType.Decimal;

				case ObjectField.AmortizationType:
				case ObjectField.Periodicity:
				case ObjectField.Prorata:
					return FieldType.Int;

				case ObjectField.OneShotDateOperation:
				case ObjectField.OneShotDateEvent:
				case ObjectField.EntryDate:
					return FieldType.Date;

				case ObjectField.GroupParent:
					return FieldType.GuidGroup;

				case ObjectField.Account1:
				case ObjectField.Account2:
				case ObjectField.Account3:
				case ObjectField.Account4:
				case ObjectField.Account5:
				case ObjectField.Account6:
				case ObjectField.Account7:
				case ObjectField.Account8:
				case ObjectField.EntryDebitAccount:
				case ObjectField.EntryCreditAccount:
					return FieldType.GuidAccount;

				default:
					return FieldType.String;
			}
		}


		private readonly EditionAccessor		editionAccessor;
		private DataMandat						mandat;
	}
}
