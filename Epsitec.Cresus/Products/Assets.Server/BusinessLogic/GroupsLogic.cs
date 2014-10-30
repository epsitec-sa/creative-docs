//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class GroupsLogic
	{
		public static IEnumerable<DataObject> GetSuggestedGroups(DataAccessor accessor)
		{
			//	Retourne la liste des groupes qui doivent être proposés lors de la création
			//	d'un objet.
			foreach (var group in accessor.Mandat.GetData (BaseType.Groups))
			{
				bool suggested = ObjectProperties.GetObjectPropertyInt (group, null, ObjectField.GroupSuggestedDuringCreation) == 1;

				if (suggested)
				{
					yield return group;
				}
			}
		}

		public static IEnumerable<Guid> GetAllChildrensGuid(DataAccessor accessor, Guid groupGuid)
		{
			//	Retourne la liste de tous les groupes fils, non triée.
			return accessor.Mandat.GetData (BaseType.Groups)
				.Where (x => GroupsLogic.IsChildren (accessor, groupGuid, x.Guid))
				.Select (x => x.Guid);
		}

		public static IEnumerable<Guid> GetChildrensGuids(DataAccessor accessor, Guid groupGuid)
		{
			//	Retourne la liste des groupes fils pour peupler un combo, triée par
			//	ordre alphabétique.
			return accessor.Mandat.GetData (BaseType.Groups)
				.Where (x => GroupsLogic.IsChildren (accessor, groupGuid, x.Guid) && GroupsLogic.IsFinal (accessor, x.Guid))
				.OrderBy (x => GroupsLogic.GetShortName (accessor, x.Guid))
				.Select (x => x.Guid);
		}

		private static bool IsChildren(DataAccessor accessor, Guid parentGuid, Guid groupGuid)
		{
			//	Indique si un groupe est un descendant d'un parent (fils, petit-fils, etc.).
			if (groupGuid == parentGuid)
			{
				return false;
			}

			while (true)
			{
				var group = accessor.GetObject (BaseType.Groups, groupGuid);
				groupGuid = ObjectProperties.GetObjectPropertyGuid (group, null, ObjectField.GroupParent);

				if (groupGuid.IsEmpty)
				{
					return false;
				}

				if (groupGuid == parentGuid)
				{
					return true;
				}
			}
		}

		private static bool IsFinal(DataAccessor accessor, Guid groupGuid)
		{
			//	Indique si un groupe est terminal (donc s'il n'y pas de descendants).
			//	Seuls ces groupes peuvent être choisis dans les combos.
			foreach (var group in accessor.Mandat.GetData (BaseType.Groups))
			{
				var parentGuid = ObjectProperties.GetObjectPropertyGuid (group, null, ObjectField.GroupParent);

				if (parentGuid == groupGuid)
				{
					return false;
				}
			}

			return true;
		}

	
		public static string GetDescription(string name, string number)
		{
			//	Retourne la description d'un groupe, contenant le nom et le numéro.
			return string.Format ("{0} ({1})", name, number);
		}

		public static string GetShortName(DataAccessor accessor, GuidRatio guidRatio)
		{
			//	Retourne le nom court d'un groupe-ratio, du genre:
			//	"Etrangères (75%)"
			var text = GroupsLogic.GetShortName (accessor, guidRatio.Guid);

			if (guidRatio.Ratio.HasValue)
			{
				text += " (";
				text += TypeConverters.RateToString (guidRatio.Ratio);
				text += ")";
			}

			return text;
		}

		public static string GetShortName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'un groupe, du genre:
			//	"Etrangères"
			var obj = accessor.GetObject (BaseType.Groups, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
			}
		}


		public static string GetFullName(DataAccessor accessor, GuidRatio guidRatio)
		{
			//	Retourne le nom complet d'un groupe-ratio, du genre:
			//	"Immobilisations > Bâtiments > Usines > Etrangères (75%)"
			var text = GroupsLogic.GetFullName (accessor, guidRatio.Guid);

			if (guidRatio.Ratio.HasValue)
			{
				text += " (";
				text += TypeConverters.RateToString (guidRatio.Ratio);
				text += ")";
			}

			return text;
		}

		public static string GetFullName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom complet d'un groupe, du genre:
			//	"Immobilisations > Bâtiments > Usines > Etrangères (100.40.20.3)"
			var number = GroupsLogic.GetFullNumber (accessor, guid);

			var list = new List<string> ();

			while (!guid.IsEmpty)
			{
				var obj = accessor.GetObject (BaseType.Groups, guid);
				if (obj == null)
				{
					break;
				}

				list.Insert (0, ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name));
				guid = ObjectProperties.GetObjectPropertyGuid (obj, null, ObjectField.GroupParent);
			}

			if (list.Count > 1)
			{
				list.RemoveAt (0);  // supprime le premier nom "Groupes"
			}

			var text = string.Join ("  ►  ", list);  // unicode 25BA

			if (!string.IsNullOrEmpty (number))
			{
				text = string.Concat (text, " (", number, ")");
			}

			return text;
		}

		public static string GetFullNumber(DataAccessor accessor, Guid guid)
		{
			//	Retourne le numéro complet d'un groupe, du genre:
			//	"100.20/a.400"
			return GroupsLogic.GetFullNumber (accessor, guid, ".");
		}

		public static string GetSortingValue(DataAccessor accessor, Guid guid)
		{
			//	Retourne le numéro d'un groupe en vue du tri, du genre:
			//	"100→20/a→400"
			//	On utilise comme séparateur un caractère qui ne sera à priori pas
			//	utilisé lors de la saisie par l'utilisateur.
			return GroupsLogic.GetFullNumber (accessor, guid, "→");
		}

		private static string GetFullNumber(DataAccessor accessor, Guid guid, string separator)
		{
			var list = new List<string> ();

			while (!guid.IsEmpty)
			{
				var obj = accessor.GetObject (BaseType.Groups, guid);
				if (obj == null)
				{
					break;
				}

				string s = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
				if (string.IsNullOrEmpty (s))
				{
					s = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
					s = string.Concat ("<i>", s, "</i>");
				}

				list.Insert (0, s);
				guid = ObjectProperties.GetObjectPropertyGuid (obj, null, ObjectField.GroupParent);
			}

			if (list.Any ())
			{
				list.RemoveAt (0);  // supprime le premier nom "Groupes"
			}

			return string.Join (separator, list);
		}
	}
}
