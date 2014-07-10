//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AccountsLogic
	{
		public static string GetSummary(DataAccessor accessor, BaseType baseType, string number)
		{
			//	Retourne le résumé (par exemple "1000 Caisse") d'après le seul numéro.
			//	Le résumé dépend de la base (BaseType.Accounts+n), qui dépend elle-même
			//	de la période du plan comptable.
			if (!string.IsNullOrEmpty (number))
			{
				var obj = AccountsLogic.GetAccount (accessor, baseType, number);

				if (obj != null)
				{
					var name = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
					return string.Join (" ", number, name);
				}
			}

			return null;
		}

		public static DataObject GetAccount(DataAccessor accessor, BaseType baseType, string number)
		{
			if (baseType != BaseType.Unknown)
			{
				var data = accessor.Mandat.GetData (baseType);

				foreach (var obj in data)
				{
					var n = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
					if (n == number)
					{
						return obj;
					}
				}
			}

			return null;
		}

		public static string GetNumber(DataAccessor accessor, BaseType baseType, Guid guid)
		{
			//	Retourne le numéro d'un compte.
			var obj = accessor.GetObject (baseType, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
			}
		}


		public static bool Compare(GuidList<DataObject> accounts1, GuidList<DataObject> accounts2)
		{
			//	Compare deux plans comptables.
			if (accounts1.Count == accounts2.Count)
			{
				for (int i=0; i<accounts1.Count; i++)
				{
					var account1 = accounts1[i];
					var account2 = accounts2[i];

					if (!AccountsLogic.Compare (account1, account2))
					{
						return false;
					}

				}

				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool Compare(DataObject account1, DataObject account2)
		{
			//	Compare deux comptes.
			var number1   = ObjectProperties.GetObjectPropertyString (account1, null, ObjectField.Number, inputValue: true);
			var name1     = ObjectProperties.GetObjectPropertyString (account1, null, ObjectField.Name);
			var category1 = ObjectProperties.GetObjectPropertyInt    (account1, null, ObjectField.AccountCategory);
			var type1     = ObjectProperties.GetObjectPropertyInt    (account1, null, ObjectField.AccountType);

			var number2   = ObjectProperties.GetObjectPropertyString (account2, null, ObjectField.Number, inputValue: true);
			var name2     = ObjectProperties.GetObjectPropertyString (account2, null, ObjectField.Name);
			var category2 = ObjectProperties.GetObjectPropertyInt    (account2, null, ObjectField.AccountCategory);
			var type2     = ObjectProperties.GetObjectPropertyInt    (account2, null, ObjectField.AccountType);

			return number1   == number2
				&& name1     == name2
				&& category1 == category2
				&& type1     == type2;
		}
	}
}
