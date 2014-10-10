//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class GroupsGuidRatioLogic
	{
		public static Guid GetPercentErrorGroupGuid(DataAccessor accessor, DataEvent e)
		{
			//	Vérifie si toutes les propriétés group-ratio d'un événement d'un objet
			//	d'immobilisation ont bien une somme de 100%.
			//	Si ce n'est pas le cas, retourne le Guid du groupe erroné.
			var dict = new Dictionary<Guid, decimal> ();

			foreach (var property in e.Properties
				.Where (x => x is DataGuidRatioProperty)
				.OfType<DataGuidRatioProperty> ())
			{
				if (property.Value.Ratio.HasValue)
				{
					var guid = GroupsGuidRatioLogic.GetPercentRoot (accessor, property.Value.Guid);

					if (!dict.ContainsKey (guid))
					{
						dict.Add (guid, 0.0m);
					}

					dict[guid] += property.Value.Ratio.GetValueOrDefault ();
				}
			}

			foreach (var pair in dict)
			{
				if (pair.Value != 1.0m)
				{
					return pair.Key;
				}
			}

			return Guid.Empty;
		}

		private static Guid GetPercentRoot(DataAccessor accessor, Guid guid)
		{
			//	Retourne le groupe à la racine pour la vérification des pourcents.
			//	C'est toujours le groupe de niveau 1. Par exemple, avec:
			//	Groupes > Catégories MCH2 > Immeubles > Ecoles
			//	Le parent est Catégories MCH2.
			var lastGuid = Guid.Empty;

			while (true)
			{
				var group = accessor.GetObject (BaseType.Groups, guid);
				var parentGuid = ObjectProperties.GetObjectPropertyGuid (group, null, ObjectField.GroupParent);

				if (parentGuid.IsEmpty)
				{
					return lastGuid;
				}

				lastGuid = guid;
				guid = parentGuid;
			}
		}


		public static IEnumerable<ObjectField> GetSortedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs, triés par ordre alphabétique des noms
			//	complets des groupes.
			return GroupsGuidRatioLogic.GetUsedFields (accessor)
				.OrderBy (x => GroupsGuidRatioLogic.GetSortingValue (accessor, x));
		}

		private static string GetSortingValue(DataAccessor accessor, ObjectField field)
		{
			//	Retourne le numéro d'un groupe, en vue du tri.
			var gr = accessor.EditionAccessor.GetFieldGuidRatio (field);
			return GroupsLogic.GetSortingValue (accessor, gr.Guid);
		}

		private static IEnumerable<ObjectField> GetUsedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs utilisés par l'objet en édition, non triée.
			foreach (var field in DataAccessor.GroupGuidRatioFields)
			{
				var gr = accessor.EditionAccessor.GetFieldGuidRatio (field);
				if (!gr.IsEmpty)
				{
					yield return field;
				}
			}
		}
	}
}
