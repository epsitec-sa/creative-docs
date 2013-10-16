﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		}


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

		public IEnumerable<AbstractDataProperty> GetObjectSyntheticProperties(Guid objectGuid, Timestamp timestamp)
		{
			var obj = this.mandat.GetObject (objectGuid);

			if (obj != null)
			{
				foreach (var field in DataAccessor.ObjectFields)
				{
					var p = obj.GetSyntheticProperty (timestamp, (int) field);
					if (p != null)
					{
						yield return p;
					}
				}
			}
		}


		public FieldType GetFieldType(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.Level:
				case ObjectField.FréquenceAmortissement:
					return FieldType.Int;

				case ObjectField.Valeur1:
				case ObjectField.Valeur2:
				case ObjectField.Valeur3:
				case ObjectField.ValeurRésiduelle:
					return FieldType.Amount;

				case ObjectField.TauxAmortissement:
					return FieldType.Rate;

				default:
					return FieldType.String;
			}
		}


		private static IEnumerable<ObjectField> ObjectFields
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
				yield return ObjectField.TauxAmortissement;
				yield return ObjectField.TypeAmortissement;
				yield return ObjectField.FréquenceAmortissement;
				yield return ObjectField.ValeurRésiduelle;
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
		#endregion


		private readonly DataMandat mandat;
	}
}
