//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataAccessor
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
				foreach (var obj in this.mandat.GetData (BaseType.Objects))
				{
					ObjectCalculator.UpdateComputedAmounts (obj);
				}
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


		#region NodesGetter
		public GuidNodesGetter GetNodesGetter(BaseType baseType)
		{
			//	Retourne un moyen standardisé d'accès en lecture aux données d'une base.
			return new GuidNodesGetter (this.mandat, baseType);
		}

		public class GuidNodesGetter : AbstractNodesGetter<GuidNode>
		{
			public GuidNodesGetter(DataMandat mandat, BaseType baseType)
			{
				this.mandat   = mandat;
				this.baseType = baseType;
			}

			public override int Count
			{
				get
				{
					return this.Data.Count;
				}
			}

			public override GuidNode this[int index]
			{
				get
				{
					var obj = this.Data[index];
					return new GuidNode (obj.Guid);
				}
			}

			private GuidList<DataObject> Data
			{
				get
				{
					return this.mandat.GetData (this.baseType);
				}
			}

			private readonly DataMandat			mandat;
			private readonly BaseType			baseType;
		}
		#endregion


		#region Objects
		public DataObject GetObject(BaseType baseType, Guid objectGuid)
		{
			return this.mandat.GetData (baseType)[objectGuid];
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

			e.AddProperty (new DataStringProperty (ObjectField.Name, name));
		
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
				var p = new DataDateProperty (ObjectField.OneShotDateOpération, Timestamp.Now.Date);
				e.AddProperty (p);

				obj.AddEvent (e);
				ObjectCalculator.UpdateComputedAmounts (obj);
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
					ObjectCalculator.UpdateComputedAmounts (obj);
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
				case ObjectField.OneShotNuméro:
				case ObjectField.OneShotDateOpération:
				case ObjectField.OneShotCommentaire:
				case ObjectField.OneShotDocuments:
					return true;

				default:
					return false;
			}
		}

		public static IEnumerable<ObjectField> GroupGuidRatioFields
		{
			get
			{
				yield return ObjectField.GroupGuidRatio+0;
				yield return ObjectField.GroupGuidRatio+1;
				yield return ObjectField.GroupGuidRatio+2;
				yield return ObjectField.GroupGuidRatio+3;
				yield return ObjectField.GroupGuidRatio+4;
				yield return ObjectField.GroupGuidRatio+5;
				yield return ObjectField.GroupGuidRatio+6;
				yield return ObjectField.GroupGuidRatio+7;
				yield return ObjectField.GroupGuidRatio+8;
				yield return ObjectField.GroupGuidRatio+9;
			}
		}

		public static IEnumerable<ObjectField> ValueFields
		{
			get
			{
				yield return ObjectField.MainValue;
				yield return ObjectField.Value1;
				yield return ObjectField.Value2;
				yield return ObjectField.Value3;
				yield return ObjectField.Value4;
				yield return ObjectField.Value5;
				yield return ObjectField.Value6;
				yield return ObjectField.Value7;
				yield return ObjectField.Value8;
				yield return ObjectField.Value9;
				yield return ObjectField.Value10;
			}
		}

		public static FieldType GetFieldType(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.MainValue:
				case ObjectField.Value1:
				case ObjectField.Value2:
				case ObjectField.Value3:
				case ObjectField.Value4:
				case ObjectField.Value5:
				case ObjectField.Value6:
				case ObjectField.Value7:
				case ObjectField.Value8:
				case ObjectField.Value9:
				case ObjectField.Value10:
					return FieldType.ComputedAmount;

				case ObjectField.AmortizationRate:
				case ObjectField.ResidualValue:
				case ObjectField.Round:
					return FieldType.Decimal;

				case ObjectField.AmortizationType:
				case ObjectField.Periodicity:
				case ObjectField.Prorata:
					return FieldType.Int;

				case ObjectField.OneShotDateOpération:
					return FieldType.Date;

				case ObjectField.GroupParent:
					return FieldType.GuidGroup;

				case ObjectField.Person1:
				case ObjectField.Person2:
				case ObjectField.Person3:
				case ObjectField.Person4:
				case ObjectField.Person5:
					return FieldType.GuidPerson;

				case ObjectField.GroupGuidRatio+0:
				case ObjectField.GroupGuidRatio+1:
				case ObjectField.GroupGuidRatio+2:
				case ObjectField.GroupGuidRatio+3:
				case ObjectField.GroupGuidRatio+4:
				case ObjectField.GroupGuidRatio+5:
				case ObjectField.GroupGuidRatio+6:
				case ObjectField.GroupGuidRatio+7:
				case ObjectField.GroupGuidRatio+8:
				case ObjectField.GroupGuidRatio+9:
					return FieldType.GuidRatio;

				default:
					return FieldType.String;
			}
		}


		private readonly EditionAccessor		editionAccessor;
		private DataMandat						mandat;
	}
}
