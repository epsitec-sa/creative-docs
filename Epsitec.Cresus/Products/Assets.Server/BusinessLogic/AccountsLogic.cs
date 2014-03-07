//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AccountsLogic
	{
		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le résumé d'un compte, du genre:
			//	"1000 Caisse"
			var obj = accessor.GetObject (BaseType.Accounts, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				var n = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
				var t = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);

				return string.Join (" ", n, t);
			}
		}

		public static string GetNumber(DataAccessor accessor, Guid guid)
		{
			//	Retourne le numéro d'un compte, du genre:
			//	"1000"
			if (guid == AccountsLogic.MultiGuid)
			{
				return "...";
			}

			var obj = accessor.GetObject (BaseType.Accounts, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
			}
		}

		public static Guid MultiGuid
		{
			//	Retourne le Guid spécial indiquant une écriture multiple "...".
			get
			{
				return Guid.NewGuid (1);
			}
		}
	}
}
