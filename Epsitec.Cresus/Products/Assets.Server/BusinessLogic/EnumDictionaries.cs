//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.Expression;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class EnumDictionaries
	{
		public static string GetPeriodicityName(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetPeriodicityName ((Periodicity) type.Value);
			}

			return null;
		}

		public static string GetArgumentTypeName(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetArgumentTypeName ((ArgumentType) type.Value);
			}

			return null;
		}


		public static string GetPeriodicityName(Periodicity type)
		{
			switch (type)
			{
				case Periodicity.Annual:
					return Res.Strings.Enum.Periodicity.Annual.ToString ();

				case Periodicity.Semestrial:
					return Res.Strings.Enum.Periodicity.Semestrial.ToString ();

				case Periodicity.Trimestrial:
					return Res.Strings.Enum.Periodicity.Trimestrial.ToString ();

				case Periodicity.Mensual:
					return Res.Strings.Enum.Periodicity.Mensual.ToString ();

				default:
					return null;
			}
		}

		public static string GetFieldTypeName(FieldType type)
		{
			switch (type)
			{
				case FieldType.String:
					return Res.Strings.Enum.FieldType.String.ToString ();

				case FieldType.Decimal:
					return Res.Strings.Enum.FieldType.Decimal.ToString ();

				case FieldType.ComputedAmount:
					return Res.Strings.Enum.FieldType.ComputedAmount.ToString ();

				case FieldType.Int:
					return Res.Strings.Enum.FieldType.Int.ToString ();

				case FieldType.Date:
					return Res.Strings.Enum.FieldType.Date.ToString ();

				case FieldType.GuidPerson:
					return Res.Strings.Enum.FieldType.GuidPerson.ToString ();

				default:
					return null;
			}
		}

		public static string GetAccountCategoryName(AccountCategory category)
		{
			switch (category)
			{
				case AccountCategory.Actif:
					return Res.Strings.Enum.AccountCategory.Actif.ToString ();

				case AccountCategory.Passif:
					return Res.Strings.Enum.AccountCategory.Passif.ToString ();

				case AccountCategory.Charge:
					return Res.Strings.Enum.AccountCategory.Charge.ToString ();

				case AccountCategory.Produit:
					return Res.Strings.Enum.AccountCategory.Produit.ToString ();

				case AccountCategory.Exploitation:
					return Res.Strings.Enum.AccountCategory.Exploitation.ToString ();

				case AccountCategory.Revenu:
					return Res.Strings.Enum.AccountCategory.Revenu.ToString ();

				case AccountCategory.Depense:
					return Res.Strings.Enum.AccountCategory.Depense.ToString ();

				case AccountCategory.Recette:
					return Res.Strings.Enum.AccountCategory.Recette.ToString ();

				default:
					return null;
			}
		}

		public static string GetAccountTypeName(AccountType type)
		{
			switch (type)
			{
				case AccountType.Normal:
					return Res.Strings.Enum.AccountType.Normal.ToString ();

				case AccountType.Groupe:
					return Res.Strings.Enum.AccountType.Groupe.ToString ();

				case AccountType.TVA:
					return Res.Strings.Enum.AccountType.TVA.ToString ();

				default:
					return null;
			}
		}

		public static string GetArgumentTypeName(ArgumentType type)
		{
			//	Retourne le type sous la forme "nom commun (type C#)".
			string s;

			switch (type)
			{
				case ArgumentType.String:
					s = Res.Strings.Enum.ArgumentType.String.ToString ();
					break;

				case ArgumentType.Int:
					s = Res.Strings.Enum.ArgumentType.Int.ToString ();
					break;

				case ArgumentType.Bool:
					s = Res.Strings.Enum.ArgumentType.Bool.ToString ();
					break;

				case ArgumentType.Decimal:
					s = Res.Strings.Enum.ArgumentType.Decimal.ToString ();
					break;

				case ArgumentType.Amount:
					s = Res.Strings.Enum.ArgumentType.Amount.ToString ();
					break;

				case ArgumentType.Rate:
					s = Res.Strings.Enum.ArgumentType.Rate.ToString ();
					break;

				case ArgumentType.Years:
					s = Res.Strings.Enum.ArgumentType.Years.ToString ();
					break;

				case ArgumentType.Date:
					s = Res.Strings.Enum.ArgumentType.Date.ToString ();
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid ArgumentType {0}", type));
			}

			var t = AmortizationExpression.GetArgumentType (type);
			return string.Format ("{0} ({1})", s, t);
		}

		public static string GetEntryScenarioName(EntryScenario type)
		{
			switch (type)
			{
				case EntryScenario.None:
					return Res.Strings.Enum.EntryScenario.None.ToString ();

				case EntryScenario.PreInput:
					return Res.Strings.Enum.EntryScenario.PreInput.ToString ();

				case EntryScenario.Purchase:
					return Res.Strings.Enum.EntryScenario.Purchase.ToString ();

				case EntryScenario.Sale:
					return Res.Strings.Enum.EntryScenario.Sale.ToString ();

				case EntryScenario.AmortizationAuto:
					return Res.Strings.Enum.EntryScenario.AmortizationAuto.ToString ();

				case EntryScenario.AmortizationExtra:
					return Res.Strings.Enum.EntryScenario.AmortizationExtra.ToString ();

				case EntryScenario.Increase:
					return Res.Strings.Enum.EntryScenario.Increase.ToString ();

				case EntryScenario.Decrease:
					return Res.Strings.Enum.EntryScenario.Decrease.ToString ();

				case EntryScenario.Adjust:
					return Res.Strings.Enum.EntryScenario.Adjust.ToString ();

				default:
					return null;
			}
		}

		public static string GetEntryScenarioTitle(EntryScenario type)
		{
			switch (type)
			{
				case EntryScenario.PreInput:
					return Res.Strings.Enum.EntryScenario.Title.PreInput.ToString ();

				case EntryScenario.Purchase:
					return Res.Strings.Enum.EntryScenario.Title.Purchase.ToString ();

				case EntryScenario.Sale:
					return Res.Strings.Enum.EntryScenario.Title.Sale.ToString ();

				case EntryScenario.AmortizationAuto:
					return Res.Strings.Enum.EntryScenario.Title.AmortizationAuto.ToString ();

				case EntryScenario.AmortizationExtra:
					return Res.Strings.Enum.EntryScenario.Title.AmortizationExtra.ToString ();

				case EntryScenario.Increase:
					return Res.Strings.Enum.EntryScenario.Title.Increase.ToString ();

				case EntryScenario.Decrease:
					return Res.Strings.Enum.EntryScenario.Title.Decrease.ToString ();

				case EntryScenario.Adjust:
					return Res.Strings.Enum.EntryScenario.Title.Adjust.ToString ();

				default:
					return null;
			}
		}


		//	Ici, il est préférable de ne pas avoir de mécanisme automatique pour
		//	générer les dictionnaires à partir des enumérations C#. En effet, les
		//	énumérations peuvent évoluer au cours du temps, de nouvelles valeurs
		//	peuvent être introduites. Le dictionnaire est dans un ordre logique
		//	pour l'utilisateur, qui n'est pas forcément le même que celui de
		//	l'énumération C#.

		public static Dictionary<int, string> DictPeriodicities
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				foreach (var type in EnumDictionaries.EnumPeriodicities)
				{
					var text = EnumDictionaries.GetPeriodicityName (type);
					dict.Add ((int) type, text);
				}

				return dict;
			}
		}

		private static IEnumerable<Periodicity> EnumPeriodicities
		{
			get
			{
				yield return Periodicity.Annual;
				yield return Periodicity.Semestrial;
				yield return Periodicity.Trimestrial;
				yield return Periodicity.Mensual;
			}
		}


		public static Dictionary<int, string> DictArgumentTypes
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				foreach (var type in EnumDictionaries.EnumArgumentsTypes)
				{
					var text = EnumDictionaries.GetArgumentTypeName (type);
					dict.Add ((int) type, text);
				}

				return dict;
			}
		}

		private static IEnumerable<ArgumentType> EnumArgumentsTypes
		{
			get
			{
				yield return ArgumentType.Amount;
				yield return ArgumentType.Rate;
				yield return ArgumentType.Years;
				yield return ArgumentType.Decimal;
				yield return ArgumentType.Int;
				yield return ArgumentType.Bool;
				yield return ArgumentType.Date;
				yield return ArgumentType.String;
			}
		}


		public static Dictionary<int, string> GetDictFieldTypes(bool hasComplexTypes = true)
		{
			var dict = new Dictionary<int, string> ();

			foreach (var type in EnumDictionaries.GetEnumFieldTypes (hasComplexTypes))
			{
				var text = EnumDictionaries.GetFieldTypeName (type);
				dict.Add ((int) type, text);
			}

			return dict;
		}

		private static IEnumerable<FieldType> GetEnumFieldTypes(bool hasComplexTypes = true)
		{
			yield return FieldType.String;

			if (hasComplexTypes)
			{
				yield return FieldType.ComputedAmount;
			}

			yield return FieldType.Decimal;
			yield return FieldType.Int;
			yield return FieldType.Date;

			if (hasComplexTypes)
			{
				yield return FieldType.GuidPerson;
			}
		}


		public static Dictionary<int, string> DictAccountCategories
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				foreach (var cat in EnumDictionaries.EnumAccountCategories)
				{
					var text = EnumDictionaries.GetAccountCategoryName (cat);
					dict.Add ((int) cat, text);
				}

				return dict;
			}
		}

		private static IEnumerable<AccountCategory> EnumAccountCategories
		{
			get
			{
				yield return AccountCategory.Actif;
				yield return AccountCategory.Passif;
				yield return AccountCategory.Charge;
				yield return AccountCategory.Produit;
				yield return AccountCategory.Exploitation;
				yield return AccountCategory.Revenu;
				yield return AccountCategory.Depense;
				yield return AccountCategory.Recette;
			}
		}


		public static Dictionary<int, string> DictAccountTypes
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				foreach (var type in EnumDictionaries.EnumAccountTypes)
				{
					var text = EnumDictionaries.GetAccountTypeName (type);
					dict.Add ((int) type, text);
				}

				return dict;
			}
		}

		private static IEnumerable<AccountType> EnumAccountTypes
		{
			get
			{
				yield return AccountType.Normal;
				yield return AccountType.Groupe;
				yield return AccountType.TVA;
			}
		}


		public static IEnumerable<EntryScenario> EnumEntryScenarios
		{
			get
			{
				yield return EntryScenario.None;
				yield return EntryScenario.PreInput;
				yield return EntryScenario.Purchase;
				yield return EntryScenario.Sale;
				yield return EntryScenario.AmortizationAuto;
				yield return EntryScenario.AmortizationExtra;
				yield return EntryScenario.Increase;
				yield return EntryScenario.Decrease;
				yield return EntryScenario.Adjust;
			}
		}
	}
}
