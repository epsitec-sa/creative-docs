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
		public static AccountCategory GetCategories(DataAccessor accessor, BaseType baseType)
		{
			//	Retourne toutes les catégories existantes dans un plan comptable.
			var accounts = accessor.Mandat.GetData (baseType);

			AccountCategory categories = AccountCategory.Unknown;

			foreach (var account in accounts)
			{
				var category = (AccountCategory) ObjectProperties.GetObjectPropertyInt (account, null, ObjectField.AccountCategory);
				var type     = (AccountType)     ObjectProperties.GetObjectPropertyInt (account, null, ObjectField.AccountType);

				if (type == AccountType.Normal)
				{
					categories |= category;
				}
			}

			return categories;
		}

		public static string GetExplanation(DataAccessor accessor, System.DateTime date, string number,
			out bool hasError, out bool gotoVisible)
		{
			//	Retourne le texte explicatif d'un compte. Exemples:
			//	"1000 Caisse"
			//	"1111 — Inconnu dans le plan comptable"
			string explanationsValue;

			if (string.IsNullOrEmpty (number))  // aucun compte ?
			{
				explanationsValue = null;
				hasError = false;
				gotoVisible = false;
			}
			else  // compte présent ?
			{
				//	Cherche le plan comptable correspondant à la date.
				var baseType = accessor.Mandat.GetAccountsBase (date);

				if (baseType.AccountsDateRange.IsEmpty)  // pas de plan comptable ?
				{
					explanationsValue = AccountsLogic.AddError (number, Res.Strings.AccountsLogic.InvalidDate.ToString ());
					hasError = true;
					gotoVisible = false;
				}
				else  // plan comptable trouvé ?
				{
					//	Cherche le résumé du compte (numéro et titre).
					var summary = AccountsLogic.GetSummary (accessor, baseType, number);

					if (string.IsNullOrEmpty (summary))  // compte inexistant ?
					{
						explanationsValue = AccountsLogic.AddError (number, Res.Strings.AccountsLogic.AccountDoesNotExist.ToString ());
						hasError = true;
						gotoVisible = false;
					}
					else
					{
						explanationsValue = summary;  // par exemple "1000 Caisse"
						hasError = false;
						gotoVisible = true;
					}
				}
			}

			return explanationsValue;
		}

		private static string AddError(string text, string error)
		{
			//	Retourne un texte explicatif composé du numéro du compte et de l'erreur.
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				if (string.IsNullOrEmpty (error))
				{
					return text;
				}
				else
				{
					return UniversalLogic.NiceJoin (text, error);
				}
			}
		}


		public static string GetSummary(DataAccessor accessor, BaseType baseType, string number)
		{
			//	Retourne le résumé (par exemple "1000 Caisse") d'après le seul numéro.
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


		public static bool Compare(GuidDictionary<DataObject> accounts1, GuidDictionary<DataObject> accounts2)
		{
			//	Compare deux plans comptables.
			var a1 = accounts1.ToArray ();
			var a2 = accounts2.ToArray ();

			if (accounts1.Count == accounts2.Count)
			{
				for (int i=0; i<accounts1.Count; i++)
				{
					var account1 = a1[i];
					var account2 = a2[i];

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
