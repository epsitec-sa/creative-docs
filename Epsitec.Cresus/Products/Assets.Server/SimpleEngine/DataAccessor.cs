//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
					AssetCalculator.UpdateComputedAmounts (this, obj);
				}
			}
		}

		public Settings Settings
		{
			get
			{
				return this.mandat.Settings;
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

		public System.DateTime EndDate
		{
			get
			{
				return this.mandat.EndDate;
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
				return this.Settings.GetTempDataObject (objectGuid);
			}
			else
			{
				return this.mandat.GetData (baseType)[objectGuid];
			}
		}

		public Guid CreateObject(BaseType baseType, System.DateTime date, string name, Guid parent)
		{
			var obj = new DataObject ();
			mandat.GetData (baseType).Add (obj);

			var timestamp = new Timestamp (date, 0);
			var e = new DataEvent (timestamp, EventType.Input);
			obj.AddEvent (e);

			if (!parent.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, parent));
			}

			var field = this.GetMainStringField (baseType);
			e.AddProperty (new DataStringProperty (field, name));
		
			return obj.Guid;
		}

		public DataEvent CreateObjectEvent(DataObject obj, System.DateTime date, EventType type)
		{
			if (obj != null)
			{
				var position = obj.GetNewPosition (date);
				var ts = new Timestamp (date, position);
				var e = new DataEvent (ts, type);

				//	Ajoute la date du jour comme date valeur.
				var p = new DataDateProperty (ObjectField.OneShotDateOperation, Timestamp.Now.Date);
				e.AddProperty (p);

				obj.AddEvent (e);
				AssetCalculator.UpdateComputedAmounts (this, obj);
				return e;
			}

			return null;
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

		public void RemoveObjectEvent(DataObject obj, Timestamp? timestamp)
		{
			if (obj != null && timestamp.HasValue)
			{
				var e = obj.GetEvent (timestamp.Value);
				if (e != null)
				{
					obj.RemoveEvent (e);
					AssetCalculator.UpdateComputedAmounts (this, obj);
				}
			}
		}

		public void CopyObject(DataObject obj, DataObject model, Timestamp? timestamp)
		{
			//	Copie dans 'obj' toutes les propriétés de 'model' que 'obj' n'a pas déjà.
			if (!timestamp.HasValue)
			{
				timestamp = Timestamp.MaxValue;
			}

			var e = obj.GetEvent (obj.EventsCount-1);  // dernier événement

			foreach (ObjectField field in System.Enum.GetValues (typeof (ObjectField)))
			{
				var op = obj.GetSyntheticProperty (timestamp.Value, field);
				if (op == null)  // propriété pas encore connue ?
				{
					var p = model.GetSyntheticProperty (timestamp.Value, field);
					if (p != null)
					{
						e.AddProperty (p);
					}
				}
			}
		}
		#endregion


		public static bool IsOneShotField(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.OneShotNumber:
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
				return this.Settings.GetMainStringField (baseType);
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

				foreach (var field in this.Settings.GetUserFields (BaseType.Assets)
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
				return this.Settings.GetUserFieldName (objectField);
			}

			return DataDescriptions.GetObjectFieldDescription (objectField);
		}

		public FieldType GetFieldType(ObjectField objectField)
		{
			if (objectField >= ObjectField.UserFieldFirst &&
				objectField <= ObjectField.UserFieldLast)
			{
				return this.Settings.GetUserFieldType (objectField);
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
					return FieldType.Decimal;

				case ObjectField.AmortizationType:
				case ObjectField.Periodicity:
				case ObjectField.Prorata:
					return FieldType.Int;

				case ObjectField.OneShotDateOperation:
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
					return FieldType.GuidAccount;

				default:
					return FieldType.String;
			}
		}


		private readonly EditionAccessor		editionAccessor;
		private DataMandat						mandat;
	}
}
