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


		public int ObjectsCount
		{
			get
			{
				return this.mandat.Objects.Count;
			}
		}

		public Guid GetObjectGuid(int index)
		{
			if (index >= 0 && index < this.mandat.Objects.Count)
			{
				return this.mandat.Objects[index].Guid;
			}
			else
			{
				return Guid.Empty;
			}
		}

		public List<AbstractDataProperty> GetObjectProperties(Guid guid, Timestamp timestamp)
		{
			var properties = new List<AbstractDataProperty> ();

			var obj = this.mandat.GetObject (guid);

			if (obj != null)
			{
				foreach (var field in DataAccessor.ObjectFields)
				{
					var p = obj.GetSyntheticProperty (timestamp, (int) field);
					if (p != null)
					{
						properties.Add (p);
					}
				}
			}

			return properties;
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
		public static string GetStringProperty(List<AbstractDataProperty> properties, int id)
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

		public static int? GetIntProperty(List<AbstractDataProperty> properties, int id)
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

		public static decimal? GetDecimalProperty(List<AbstractDataProperty> properties, int id)
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
