//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DataAccessor
	{
		public DataAccessor(DataMandat mandat)
		{
			this.mandat = mandat;

			this.editionObjectGuid = Guid.Empty;

			//	Recalcule tout.
			foreach (var obj in this.mandat.Objects)
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


		public int ObjectsCount
		{
			get
			{
				return this.mandat.Objects.Count;
			}
		}

		public IEnumerable<Guid> GetObjectGuids(int start = 0, int count = int.MaxValue)
		{
			count = System.Math.Min (count, this.ObjectsCount);

			for (int i=start; i<start+count; i++)
			{
				yield return this.mandat.Objects[i].Guid;
			}
		}

		public DataObject GetObject(Guid objectGuid)
		{
			return this.mandat.Objects[objectGuid];
		}

		public Timestamp CreateObject(int row, Guid modelGuid)
		{
			var timestamp = new Timestamp (this.mandat.StartDate, 0);

			var o = new DataObject ();
			mandat.Objects.Insert (row, o);

			var e = new DataEvent (timestamp, EventType.Entrée);
			o.AddEvent (e);

			//	On met le même niveau que l'objet modèle.
			var objectModel = this.GetObject (modelGuid);

			var i = ObjectCalculator.GetObjectPropertyInt (objectModel, null, ObjectField.Level);
			if (i.HasValue)
			{
				e.AddProperty (new DataIntProperty ((int) ObjectField.Level, i.Value));
			}

			//	On met le même numéro que l'objet modèle.
			var n = ObjectCalculator.GetObjectPropertyString (objectModel, null, ObjectField.Numéro);
			if (!string.IsNullOrEmpty (n))
			{
				e.AddProperty (new DataStringProperty ((int) ObjectField.Numéro, n));
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


		#region Edition manager
		public void StartObjectEdition(Guid objectGuid, Timestamp? timestamp)
		{
			//	Marque le début de l'édition de l'événement d'un objet.
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
			this.editionTimestamp = timestamp;
		}

		public void SetObjectField(ObjectField field, string value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				if (value == null)
				{
					e.RemoveProperty ((int) field);
				}
				else
				{
					var newProperty = new DataStringProperty ((int) field, value);
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
					var newProperty = new DataDecimalProperty ((int) field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty ((int) field);
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
					var newProperty = new DataComputedAmountProperty ((int) field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty ((int) field);
				}

				var obj = this.GetObject (this.editionObjectGuid);
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
					var newProperty = new DataIntProperty ((int) field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty ((int) field);
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
					var newProperty = new DataDateProperty ((int) field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty ((int) field);
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
				var obj = this.mandat.Objects[this.editionObjectGuid];

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

		private Guid							editionObjectGuid;
		private Timestamp?						editionTimestamp;
	}
}
