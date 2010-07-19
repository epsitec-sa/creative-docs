//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public static class Enumerations
	{
		public static IEnumerable<EnumKeyValues<Finance.TaxMode>> GetAllPossibleItemsTaxModes()
		{
			yield return EnumKeyValues.Create (Finance.TaxMode.LiableForVat,    "Assujetti à la TVA");
			yield return EnumKeyValues.Create (Finance.TaxMode.NotLiableForVat, "Non-assujetti à la TVA");
			yield return EnumKeyValues.Create (Finance.TaxMode.ExemptFromVat,   "Exonéré");
		}

		public static IEnumerable<EnumKeyValues<Finance.CurrencyCode>> GetGetAllPossibleItemsDefaultCurrencyCodes()
		{
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Chf, "CHF", "Franc suisse");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Eur, "EUR", "Euro");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Usd, "USD", "Dollar américain");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Gbp, "GBP", "Livre anglaise");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Jpy, "JPY", "Yen japonais");
			yield return EnumKeyValues.Create (Finance.CurrencyCode.Cny, "CNY", "Yuan chinois");
		}

		public static IEnumerable<EnumKeyValues<UnitOfMeasureCategory>> GetGetAllPossibleItemsUnitOfMeasureCategory()
		{
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Unrelated, "Indépendant");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Unit,      "Unité");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Mass,      "Masse");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Length,    "Longueur");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Surface,   "Surface");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Volume,    "Volume");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Time,      "Temps");
			yield return EnumKeyValues.Create (UnitOfMeasureCategory.Energy,    "Energie");
		}
	}
}
