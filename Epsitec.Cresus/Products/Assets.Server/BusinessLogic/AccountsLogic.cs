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
		public static string GetNumber(DataAccessor accessor, Guid guid)
		{
			//	Retourne le numéro d'un compte.
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

		//-public static string GetSummary(DataAccessor accessor, Guid guid)
		//-{
		//-	//	Retourne le résumé d'un compte, du genre:
		//-	//	"1000 Caisse"
		//-	var obj = accessor.GetObject (BaseType.Accounts, guid);
		//-	return AccountsLogic.GetSummary (obj);
		//-}

		//-public static string GetSummary(DataObject obj)
		//-{
		//-	//	Retourne le résumé d'un compte, du genre:
		//-	//	"1000 Caisse"
		//-	if (obj == null)
		//-	{
		//-		return null;
		//-	}
		//-	else
		//-	{
		//-		var n = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
		//-		var t = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		//-
		//-		return string.Join (" ", n, t);
		//-	}
		//-}

		//-public static string GetNumber(DataAccessor accessor, Guid guid)
		//-{
		//-	//	Retourne le numéro d'un compte, du genre:
		//-	//	"1000"
		//-	if (guid == AccountsLogic.MultiGuid)
		//-	{
		//-		return "...";
		//-	}
		//-
		//-	var obj = accessor.GetObject (BaseType.Accounts, guid);
		//-	if (obj == null)
		//-	{
		//-		return null;
		//-	}
		//-	else
		//-	{
		//-		return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
		//-	}
		//-}

		//-public static Guid MultiGuid
		//-{
		//-	//	Retourne le Guid spécial indiquant une écriture multiple "...".
		//-	get
		//-	{
		//-		return Guid.NewGuid (1);
		//-	}
		//-}


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
