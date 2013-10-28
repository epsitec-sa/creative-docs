//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	/// <summary>
	/// Cette couche est sensée pouvoir accéder à travers le réseau au mandat.
	/// On ne retourne jamais un DataObject complet (avec tous ses événements),
	/// cela serait un trop gros volume de données, mais juste le Guid.
	/// </summary>
	public class DataAccessor
	{
		public DataAccessor(DataMandat mandat)
		{
			this.mandat = mandat;

			this.editionObjectGuid = Guid.Empty;

			//	Recalcule tout.
			foreach (var obj in this.mandat.Objects)
			{
				this.UpdateComputedAmount (obj.Guid);
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

		public Guid GetObjectGuid(int objectIndex)
		{
			if (objectIndex >= 0 && objectIndex < this.mandat.Objects.Count)
			{
				return this.mandat.Objects[objectIndex].Guid;
			}
			else
			{
				return Guid.Empty;
			}
		}

		public int GetObjectEventsCount(Guid objectGuid)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				return obj.Events.Count;
			}

			return 0;
		}

		public Guid GetObjectEventGuid(Guid objectGuid, int eventIndex)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				if (eventIndex >= 0 && eventIndex < obj.Events.Count)
				{
					return obj.Events[eventIndex].Guid;
				}
			}

			return Guid.Empty;
		}

		public Timestamp? GetObjectEventTimestamp(Guid objectGuid, Guid eventGuid)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				var e = obj.Events.Where (x => x.Guid == eventGuid).FirstOrDefault ();

				if (e != null)
				{
					return e.Timestamp;
				}
			}

			return null;
		}

		public Timestamp? GetObjectEventTimestamp(Guid objectGuid, int eventIndex)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				if (eventIndex >= 0 && eventIndex < obj.Events.Count)
				{
					return obj.Events[eventIndex].Timestamp;
				}
			}

			return null;
		}

		public EventType? GetObjectEventType(Guid objectGuid, int eventIndex)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				if (eventIndex >= 0 && eventIndex < obj.Events.Count)
				{
					return obj.Events[eventIndex].Type;
				}
			}

			return null;
		}

		public EventType? GetObjectEventType(Guid objectGuid, Timestamp timestamp)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				var e = obj.Events.Where (x => x.Timestamp == timestamp).FirstOrDefault ();
				if (e != null)
				{
					return e.Type;
				}
			}

			return null;
		}

		public void RemoveAmortissementsAuto(Guid objectGuid)
		{
			//	Supprime tous les événements d'amortissement automatique d'un objet.
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				while (true)
				{
					var e = obj.Events.Where (x => x.Type == EventType.AmortissementAuto).FirstOrDefault ();

					if (e == null)
					{
						break;
					}

					this.RemoveEventeventGuid (objectGuid, e.Guid);
				}
			}
		}

		public void RemoveEventeventGuid(Guid objectGuid, Guid eventGuid)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				int i = obj.Events.FindIndex (x => x.Guid == eventGuid);
				if (i != -1)
				{
					obj.Events.RemoveAt (i);
				}
			}
		}

		public bool HasObjectEvent(Guid objectGuid, Timestamp timestamp)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				return obj.GetEvent (timestamp) != null;
			}

			return false;
		}

		public IEnumerable<AbstractDataProperty> GetObjectSingleProperties(Guid objectGuid, Timestamp timestamp)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				foreach (var field in DataAccessor.ObjectFields)
				{
					var p = obj.GetSingleProperty (timestamp, (int) field);
					if (p != null)
					{
						yield return p;
					}
				}
			}
		}

		public IEnumerable<AbstractDataProperty> GetObjectSyntheticProperties(Guid objectGuid, Timestamp? timestamp)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				if (!timestamp.HasValue)
				{
					timestamp = new Timestamp (System.DateTime.MaxValue, 0);
				}

				foreach (var field in DataAccessor.ObjectFields)
				{
					var p = obj.GetSyntheticProperty (timestamp.Value, (int) field);
					if (p != null)
					{
						yield return p;
					}
				}
			}
		}

		public Timestamp CreateObject(int row, Guid modelGuid)
		{
			var timestamp = new Timestamp (this.mandat.StartDate, 0);

			var o = new DataObject (0);
			mandat.Objects.Insert (row, o);

			var e = new DataEvent (1, timestamp, EventType.Entrée);
			o.AddEvent (e);

			var properties = this.GetObjectSyntheticProperties (modelGuid, null);

			//	On met le même niveau que l'objet modèle.
			var i = DataAccessor.GetIntProperty (properties, (int) ObjectField.Level);
			if (i.HasValue)
			{
				e.Properties.Add (new DataIntProperty ((int) ObjectField.Level, i.Value));
			}

			//	On met le même numéro que l'objet modèle.
			var n = DataAccessor.GetStringProperty (properties, (int) ObjectField.Numéro);
			if (!string.IsNullOrEmpty (n))
			{
				e.Properties.Add (new DataStringProperty ((int) ObjectField.Numéro, n));
			}

			return timestamp;
		}

		public Timestamp? CreateObjectEvent(Guid objectGuid, System.DateTime date, EventType type)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				var position = obj.GetNewPosition (date);
				var ts = new Timestamp (date, position);
				var e = new DataEvent (0, ts, type);

				obj.AddEvent (e);
				this.UpdateComputedAmount (objectGuid);
				return ts;
			}

			return null;
		}

		public void AddObjectEventProperty(Guid objectGuid, Timestamp timestamp, AbstractDataProperty property)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				var e = obj.Events.Where (x => x.Timestamp == timestamp).FirstOrDefault ();

				if (e != null)
				{
					e.Properties.Add (property);
				}
			}
		}


		public void UpdateComputedAmount(Guid objectGuid)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				for (int i=0; i<3; i++)  // Valeur1..3
				{
					decimal? last = null;

					foreach (var e in obj.Events)
					{
						var current = DataAccessor.GetComputedAmount (e, i);

						if (current.HasValue)
						{
							if (last.HasValue == false)
							{
								last = current.Value.FinalAmount;
								current = new ComputedAmount (last);
								DataAccessor.SetComputedAmount (e, i, current);
							}
							else
							{
								current = new ComputedAmount (last.Value, current.Value);
								last = current.Value.FinalAmount;
								DataAccessor.SetComputedAmount (e, i, current);
							}
						}
					}
				}
			}
		}

		private static ComputedAmount? GetComputedAmount(DataEvent e, int rank)
		{
			int id = DataAccessor.RankToField (rank);

			if (id == -1)
			{
				return null;
			}
			else
			{
				return DataAccessor.GetComputedAmountProperty (e.Properties, id);
			}
		}

		private static void SetComputedAmount(DataEvent e, int rank, ComputedAmount? value)
		{
			int id = DataAccessor.RankToField (rank);

			if (id != -1)
			{
				var currentProperty = e.Properties.Where (x => x.Id == id).FirstOrDefault ();
				if (currentProperty != null)
				{
					e.Properties.Remove (currentProperty);
				}

				if (value.HasValue)
				{
					var newProperty = new DataComputedAmountProperty (id, value.Value);
					e.Properties.Add (newProperty);
				}
			}
		}

		private static int RankToField(int rank)
		{
			switch (rank)
			{
				case 0:
					return (int) ObjectField.Valeur1;

				case 1:
					return (int) ObjectField.Valeur2;

				case 2:
					return (int) ObjectField.Valeur3;

				default:
					return -1;
			}
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
				var currentProperty = e.Properties.Where (x => x.Id == (int) field).FirstOrDefault ();
				if (currentProperty != null)
				{
					e.Properties.Remove (currentProperty);
				}

				if (value != null)
				{
					var newProperty = new DataStringProperty ((int) field, value);
					e.Properties.Add (newProperty);
				}
			}
		}

		public void SetObjectField(ObjectField field, decimal? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				var currentProperty = e.Properties.Where (x => x.Id == (int) field).FirstOrDefault ();
				if (currentProperty != null)
				{
					e.Properties.Remove (currentProperty);
				}

				if (value.HasValue)
				{
					var newProperty = new DataDecimalProperty ((int) field, value.Value);
					e.Properties.Add (newProperty);
				}
			}
		}

		public void SetObjectField(ObjectField field, ComputedAmount? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				var currentProperty = e.Properties.Where (x => x.Id == (int) field).FirstOrDefault ();
				if (currentProperty != null)
				{
					e.Properties.Remove (currentProperty);
				}

				if (value.HasValue)
				{
					var newProperty = new DataComputedAmountProperty ((int) field, value.Value);
					e.Properties.Add (newProperty);
				}

				this.UpdateComputedAmount (this.editionObjectGuid);
			}
		}

		public void SetObjectField(ObjectField field, int? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				var currentProperty = e.Properties.Where (x => x.Id == (int) field).FirstOrDefault ();
				if (currentProperty != null)
				{
					e.Properties.Remove (currentProperty);
				}

				if (value.HasValue)
				{
					var newProperty = new DataIntProperty ((int) field, value.Value);
					e.Properties.Add (newProperty);
				}
			}
		}

		public void SetObjectField(ObjectField field, System.DateTime? value)
		{
			var e = this.EditionEvent;
			if (e != null)
			{
				var currentProperty = e.Properties.Where (x => x.Id == (int) field).FirstOrDefault ();
				if (currentProperty != null)
				{
					e.Properties.Remove (currentProperty);
				}

				if (value.HasValue)
				{
					var newProperty = new DataDateProperty ((int) field, value.Value);
					e.Properties.Add (newProperty);
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
				var obj = this.mandat.GetObject (this.editionObjectGuid);

				if (obj != null)
				{
					return obj.Events.Where (x => x.Timestamp == this.editionTimestamp).FirstOrDefault ();
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


		public static IEnumerable<ObjectField> ObjectFields
		{
			get
			{
				yield return ObjectField.Level;
				yield return ObjectField.Numéro;
				yield return ObjectField.Nom;
				yield return ObjectField.Description;
				yield return ObjectField.Valeur1;
				yield return ObjectField.Valeur2;
				yield return ObjectField.Valeur3;
				yield return ObjectField.Responsable;
				yield return ObjectField.Couleur;
				yield return ObjectField.NuméroSérie;

				yield return ObjectField.NomCatégorie;
				yield return ObjectField.DateAmortissement1;
				yield return ObjectField.DateAmortissement2;
				yield return ObjectField.TauxAmortissement;
				yield return ObjectField.TypeAmortissement;
				yield return ObjectField.FréquenceAmortissement;
				yield return ObjectField.ValeurRésiduelle;

				yield return ObjectField.Compte1;
				yield return ObjectField.Compte2;
				yield return ObjectField.Compte3;
				yield return ObjectField.Compte4;
				yield return ObjectField.Compte5;
				yield return ObjectField.Compte6;
				yield return ObjectField.Compte7;
				yield return ObjectField.Compte8;
			}
		}


		#region Easy access
		public static PropertyState GetPropertyState(IEnumerable<AbstractDataProperty> properties, int id)
		{
			if (properties != null)
			{
				var p = properties.Where (x => x.Id == id).FirstOrDefault ();

				if (p != null)
				{
					return p.State;
				}
			}

			return PropertyState.Undefined;
		}

		public static string GetStringProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			if (properties != null)
			{
				var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataStringProperty;

				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		public static int? GetIntProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			if (properties != null)
			{
				var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataIntProperty;

				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		public static ComputedAmount? GetComputedAmountProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			if (properties != null)
			{
				var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataComputedAmountProperty;

				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		public static decimal? GetDecimalProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			if (properties != null)
			{
				var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataDecimalProperty;

				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		public static System.DateTime? GetDateProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			if (properties != null)
			{
				var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataDateProperty;

				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}
		#endregion


		private readonly DataMandat				mandat;

		private Guid							editionObjectGuid;
		private Timestamp?						editionTimestamp;
	}
}
