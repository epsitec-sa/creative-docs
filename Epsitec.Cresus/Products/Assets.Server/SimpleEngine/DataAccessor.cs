//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataAccessor
	{
		public DataAccessor(DataMandat mandat)
		{
			this.mandat = mandat;

			this.editionObjectGuid = Guid.Empty;

			//	Recalcule tout.
			foreach (var obj in this.mandat.GetData (BaseType.Objects))
			{
				ObjectCalculator.UpdateComputedAmounts (obj);
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

		public DataEvent CreateObject(BaseType baseType, int row, Guid modelGuid)
		{
			var timestamp = new Timestamp (this.mandat.StartDate, 0);

			var o = new DataObject ();
			mandat.GetData (baseType).Insert (row, o);

			var e = new DataEvent (timestamp, EventType.Entrée);
			o.AddEvent (e);

			//	On met le même niveau que l'objet modèle.
			var objectModel = this.GetObject (baseType, modelGuid);

			var i = ObjectCalculator.GetObjectPropertyInt (objectModel, null, ObjectField.Level);
			if (i.HasValue)
			{
				e.AddProperty (new DataIntProperty (ObjectField.Level, i.Value));
			}

			//	On met le même numéro que l'objet modèle.
			var n = ObjectCalculator.GetObjectPropertyString (objectModel, null, ObjectField.Numéro);
			if (!string.IsNullOrEmpty (n))
			{
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, n));
			}

			return e;
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
		#endregion


		#region Edition manager
		public void StartObjectEdition(BaseType baseType, Guid objectGuid, Timestamp? timestamp)
		{
			//	Marque le début de l'édition de l'événement d'un objet.
			this.editionBaseType = baseType;

			if (objectGuid.IsEmpty || timestamp == null)
			{
				return;
			}

			if (!this.editionObjectGuid.IsEmpty)  // déjà une édition en cours ?
			{
				if (objectGuid != this.editionObjectGuid ||
					timestamp  != this.editionTimestamp)  // sur un autre objet/événement ?
				{
					this.SaveObjectEdition ();
				}
			}

			this.editionObjectGuid = objectGuid;
			this.editionTimestamp  = timestamp;
		}

		public void SetObjectField(ObjectField field, string value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				if (value == null)
				{
					e.RemoveProperty (field);
				}
				else
				{
					var newProperty = new DataStringProperty (field, value);
					e.AddProperty (newProperty);
				}
			}
		}

		public void SetObjectField(ObjectField field, decimal? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataDecimalProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}
			}
		}

		public void SetObjectField(ObjectField field, ComputedAmount? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataComputedAmountProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}

				var obj = this.GetObject (BaseType.Objects, this.editionObjectGuid);
				ObjectCalculator.UpdateComputedAmounts (obj);
			}
		}

		public void SetObjectField(ObjectField field, int? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataIntProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}
			}
		}

		public void SetObjectField(ObjectField field, System.DateTime? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataDateProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}
			}
		}

		public void SaveObjectEdition()
		{
			//	Marque la fin de l'édition de l'événement d'un objet.
			this.CancelObjectEdition ();
		}

		private DataEvent EditionEvent
		{
			get
			{
				var obj = this.mandat.GetData (this.editionBaseType)[this.editionObjectGuid];

				if (obj != null && this.editionTimestamp.HasValue)
				{
					return obj.GetEvent (this.editionTimestamp.Value);
				}

				return null;
			}
		}

		public void CancelObjectEdition()
		{
			//	Marque la fin de l'édition de l'événement d'un objet.
			this.editionObjectGuid = Guid.Empty;
			this.editionTimestamp = null;
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

		public static FieldType GetFieldType(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.Level:
				case ObjectField.FréquenceAmortissement:
					return FieldType.Int;

				case ObjectField.Valeur1:
				case ObjectField.Valeur2:
				case ObjectField.Valeur3:
					return FieldType.ComputedAmount;

				case ObjectField.TauxAmortissement:
				case ObjectField.ValeurRésiduelle:
					return FieldType.Decimal;

				case ObjectField.OneShotDateOpération:
				case ObjectField.DateAmortissement1:
				case ObjectField.DateAmortissement2:
					return FieldType.Date;

				default:
					return FieldType.String;
			}
		}


		private readonly DataMandat				mandat;

		private BaseType						editionBaseType;
		private Guid							editionObjectGuid;
		private Timestamp?						editionTimestamp;
	}
}
