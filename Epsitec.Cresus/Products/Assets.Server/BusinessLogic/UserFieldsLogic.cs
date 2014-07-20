//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class UserFieldsLogic
	{
		public static string GetFieldName(DataAccessor accessor, BaseType baseType, ObjectField field)
		{
			//	Retourne le nom d'un champ, y compris s'il s'agit d'un champ défini
			//	par l'utilisateur.
			if (field >= ObjectField.UserFieldFirst &&
				field <= ObjectField.UserFieldLast)
			{
				return accessor.Mandat.GlobalSettings.GetUserFields (baseType)
					.Where (x => x.Field == field)
					.Select (x => x.Name)
					.FirstOrDefault ();
			}
			else
			{
				return DataDescriptions.GetObjectFieldDescription (field);
			}
		}

		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			var obj = accessor.GetObject (BaseType.UserFields, guid);
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		}
	}
}
