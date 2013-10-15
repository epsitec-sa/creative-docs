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
			}
		}


		#region Easy access
		public static PropertyState GetPropertyState(IEnumerable<AbstractDataProperty> properties, int id)
		{
			var p = properties.Where (x => x.Id == id).FirstOrDefault ();

			if (p == null)
			{
				return PropertyState.Undefined;
			}
			else
			{
				return p.State;
			}
		}

		public static string GetStringProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataStringProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static int? GetIntProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataIntProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static decimal? GetDecimalProperty(IEnumerable<AbstractDataProperty> properties, int id)
		{
			var p = properties.Where (x => x.Id == id).FirstOrDefault () as DataDecimalProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}
		#endregion


		private readonly DataMandat mandat;
	}
}
