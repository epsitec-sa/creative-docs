//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AccountsLogic
	{
		public static string GetShortName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'un groupe, du genre:
			//	"Immobilisations financières"
			var obj = accessor.GetObject (BaseType.Accounts, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
			}
		}


		public static string GetFullName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom complet d'un groupe, du genre:
			//	"Actifs > Actifs immobilisés > Immobilisations financières"
			var list = new List<string> ();

			while (!guid.IsEmpty)
			{
				var obj = accessor.GetObject (BaseType.Accounts, guid);
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

			//-return string.Join (" ˃ ", list);  // 02C3
			//-return string.Join (" → ", list);  // 2192
			return string.Join ("  ►  ", list);  // 25BA
		}
	}
}
