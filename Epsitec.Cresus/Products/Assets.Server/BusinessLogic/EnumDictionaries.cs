//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Cresus.Assets.Data;

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

		public static string GetAmortizationTypeName(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetAmortizationTypeName ((AmortizationType) type.Value);
			}

			return null;
		}

		public static string GetProrataTypeName(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetProrataTypeName ((ProrataType) type.Value);
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

		public static string GetAmortizationTypeName(AmortizationType type)
		{
			switch (type)
			{
				case AmortizationType.Linear:
					return Res.Strings.Enum.AmortizationType.Linear.ToString ();

				case AmortizationType.Degressive:
					return Res.Strings.Enum.AmortizationType.Degressive.ToString ();

				default:
					return null;
			}
		}

		public static string GetProrataTypeName(ProrataType type)
		{
			switch (type)
			{
				case ProrataType.None:
					return Res.Strings.Enum.ProrataType.None.ToString ();

				case ProrataType.Prorata365:
					return Res.Strings.Enum.ProrataType.Prorata365.ToString ();

				case ProrataType.Prorata360:
					return Res.Strings.Enum.ProrataType.Prorata360.ToString ();

				case ProrataType.Prorata12:
					return Res.Strings.Enum.ProrataType.Prorata12.ToString ();

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
				default:
					return null;
			}
			return EnumKeyValues.GetEnumKeyValue (category).Values.First ().ToString ();
		}

		public static string GetAccountTypeName(AccountType type)
		{
			switch (type)
			{
				default:
					return null;
			}
			return EnumKeyValues.GetEnumKeyValue (type).Values.First ().ToString ();
		}

		public static string GetEntryScenarioName(EntryScenario type)
		{
			switch (type)
			{
				default:
					return null;
			}
			return EnumKeyValues.GetEnumKeyValue (type).Values.First ().ToString ();
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


		public static Dictionary<int, string> DictAmortizationTypes
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				foreach (var type in EnumDictionaries.EnumAmortizationTypes)
				{
					var text = EnumDictionaries.GetAmortizationTypeName (type);
					dict.Add ((int) type, text);
				}

				return dict;
			}
		}

		private static IEnumerable<AmortizationType> EnumAmortizationTypes
		{
			get
			{
				yield return AmortizationType.Linear;
				yield return AmortizationType.Degressive;
			}
		}


		public static Dictionary<int, string> DictProrataTypes
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				foreach (var type in EnumDictionaries.EnumProrataTypes)
				{
					var text = EnumDictionaries.GetProrataTypeName (type);
					dict.Add ((int) type, text);
				}

				return dict;
			}
		}

		private static IEnumerable<ProrataType> EnumProrataTypes
		{
			get
			{
				yield return ProrataType.None;
				yield return ProrataType.Prorata365;
				yield return ProrataType.Prorata360;
				yield return ProrataType.Prorata12;
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
				yield return EntryScenario.Purchase;
				yield return EntryScenario.Sale;
				yield return EntryScenario.AmortizationAuto;
				yield return EntryScenario.AmortizationExtra;
				yield return EntryScenario.Increase;
				yield return EntryScenario.Decrease;
			}
		}
	}
}
