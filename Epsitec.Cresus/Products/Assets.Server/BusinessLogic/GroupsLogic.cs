//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class GroupsLogic
	{
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
				return AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Name);
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
			//	"Immobilisations > Bâtiments > Usines > Etrangères"
			var list = new List<string> ();

			while (!guid.IsEmpty)
			{
				var obj = accessor.GetObject (BaseType.Groups, guid);
				if (obj == null)
				{
					break;
				}

				list.Insert (0, AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Name));
				guid = AssetCalculator.GetObjectPropertyGuid (obj, null, ObjectField.GroupParent);
			}

			if (list.Count > 1)
			{
				list.RemoveAt (0);  // supprime le premier nom "Groupes"
			}

			//-return string.Join (" ˃ ", list);  // 02C3
			//-return string.Join (" → ", list);  // 2192
			return string.Join ("  ►  ", list);  // 25BA
		}
	}
}
