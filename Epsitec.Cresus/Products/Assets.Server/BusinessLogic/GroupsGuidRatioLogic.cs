//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class GroupsGuidRatioLogic
	{
		public static IEnumerable<ObjectField> GetSortedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs, triés par ordre alphabétique des noms
			//	complets des groupes.
			return GroupsGuidRatioLogic.GetUsedFields (accessor)
				.OrderBy (x => GroupsGuidRatioLogic.GetName (accessor, x));
		}

		private static string GetName(DataAccessor accessor, ObjectField field)
		{
			//	Retourne le nom complet d'un groupe, en vue du tri.
			var gr = accessor.EditionAccessor.GetFieldGuidRatio (field);
			return GroupsLogic.GetFullName (accessor, gr.Guid);
		}

		private static IEnumerable<ObjectField> GetUsedFields(DataAccessor accessor)
		{
			//	Retourne la liste des champs utilisé par l'objet en édition, non triée.
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
