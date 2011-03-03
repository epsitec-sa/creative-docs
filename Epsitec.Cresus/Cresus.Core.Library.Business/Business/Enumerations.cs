//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Business
{
	public static class Enumerations
	{

		public static IEnumerable<EnumKeyValues<UnitOfMeasureCategory>> GetAllPossibleUnitOfMeasureCategories()
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

		public static IEnumerable<EnumKeyValues<Finance.VatCode>> GetInputVatCodes()
		{
			var filter = new HashSet<Finance.VatCode>
			{
				Finance.VatCode.Excluded,
				Finance.VatCode.ZeroRated,
				Finance.VatCode.StandardInputTaxOnMaterialOrServiceExpenses,
				Finance.VatCode.ReducedInputTaxOnMaterialOrServiceExpenses,
				Finance.VatCode.SpecialInputTaxOnMaterialOrServiceExpenses,
				Finance.VatCode.StandardInputTaxOnInvestementOrOperatingExpenses,
				Finance.VatCode.ReducedInputTaxOnInvestementOrOperatingExpenses,
				Finance.VatCode.SpecialInputTaxOnInvestementOrOperatingExpenses,
			};

			return EnumKeyValues.FromEnum<VatCode> ().Where (x => filter.Contains (x.Key));
		}

		public static IEnumerable<EnumKeyValues<Finance.VatCode>> GetOutputVatCodes()
		{
			var filter = new HashSet<Finance.VatCode>
			{
				Finance.VatCode.Excluded,
				Finance.VatCode.ZeroRated,
				Finance.VatCode.StandardTaxOnTurnover,
				Finance.VatCode.ReducedTaxOnTurnover,
				Finance.VatCode.SpecialTaxOnTurnover,
			};

			return EnumKeyValues.FromEnum<VatCode> ().Where (x => filter.Contains (x.Key));
		}

		public static IEnumerable<EnumKeyValues<ArticleQuantityType>> GetAllPossibleValueArticleQuantityType()
		{
			yield return EnumKeyValues.Create (ArticleQuantityType.Ordered,     "Commandé");
			yield return EnumKeyValues.Create (ArticleQuantityType.Billed,      "Livré");
			yield return EnumKeyValues.Create (ArticleQuantityType.Delayed,     "Suivra");
			yield return EnumKeyValues.Create (ArticleQuantityType.Information, "Information");
		}
	}
}
