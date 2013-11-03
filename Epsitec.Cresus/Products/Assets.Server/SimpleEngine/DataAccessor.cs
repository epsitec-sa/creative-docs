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


		#region Objects
		public int GetObjectsCount(BaseType baseType)
		{
			return this.mandat.GetData (baseType).Count;
		}

		public IEnumerable<Guid> GetObjectGuids(BaseType baseType, int start = 0, int count = int.MaxValue)
		{
			count = System.Math.Min (count, this.GetObjectsCount (baseType));

			for (int i=start; i<start+count; i++)
			{
				yield return this.mandat.GetData (baseType)[i].Guid;
			}
		}

		public DataObject GetObject(BaseType baseType, Guid objectGuid)
		{
			return this.mandat.GetData (baseType)[objectGuid];
		}

		public Timestamp CreateObject(BaseType baseType, int row, Guid modelGuid)
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

			return timestamp;
		}

		public DataEvent CreateObjectEvent(DataObject obj, System.DateTime date, EventType type)
		{
			if (obj != null)
			{
				var position = obj.GetNewPosition (date);
				var ts = new Timestamp (date, position);
				var e = new DataEvent (ts, type);

				obj.AddEvent (e);
				ObjectCalculator.UpdateComputedAmounts (obj);
				return e;
			}

			return null;
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


		public static bool IsSingletonField(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.EvNuméro:
				case ObjectField.EvCommentaire:
				case ObjectField.EvDocuments:
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
