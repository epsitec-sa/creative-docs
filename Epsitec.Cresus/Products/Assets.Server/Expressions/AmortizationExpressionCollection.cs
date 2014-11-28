//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public static class AmortizationExpressionCollection
	{
		public static IEnumerable<AmortizationExpressionItem> Items
		{
			get
			{
				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.RateLinear,
					Res.Strings.Enum.AmortizationExpressionType.RateLinear.ToString (),
					AmortizationExpressionCollection.defaultLinesRateLinear);

				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.RateDegressive,
					Res.Strings.Enum.AmortizationExpressionType.RateDegressive.ToString (),
					AmortizationExpressionCollection.defaultLinesRateDegressive);

				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.YearsLinear,
					Res.Strings.Enum.AmortizationExpressionType.YearsLinear.ToString (),
					AmortizationExpressionCollection.defaultLinesYearsLinear);

				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.YearsDegressive,
					Res.Strings.Enum.AmortizationExpressionType.YearsDegressive.ToString (),
					AmortizationExpressionCollection.defaultLinesYearsDegressive);
			}
		}

		public static IEnumerable<AmortizationExpressionType> Types
		{
			get
			{
				yield return AmortizationExpressionType.RateLinear;
				yield return AmortizationExpressionType.RateDegressive;
				yield return AmortizationExpressionType.YearsLinear;
				yield return AmortizationExpressionType.YearsDegressive;
			}
		}


		//	Voir (*)
		private static string[] defaultLinesRateLinear =
		{
			"Trace (\"Linear Rate\");",
			"",
			"var rate = Rate * PeriodicityFactor * ProrataFactor;",
			"var amortization = BaseAmount * rate;",
			"",
			"value = value - amortization;",
			"value = Round (value);",
			"value = Residual (value);",
			"value = Override (value);",
		};

		private static string[] defaultLinesRateDegressive =
		{
			"Trace (\"Degressive Rate\");",
			"",
			"var rate = Rate * PeriodicityFactor * ProrataFactor;",
			"var amortization = InitialAmount * rate;",
			"",
			"value = value - amortization;",
			"value = Round (value);",
			"value = Residual (value);",
			"value = Override (value);",
		};

		private static string[] defaultLinesYearsLinear =
		{
			"Trace (\"Linear Years\");",
			"",
			"decimal rate = 1;  // 100%",
			"decimal n = YearCount - YearRank;  // remaining years",
			"",
			"if (n > 0)",
			"{",
			"    rate = 1 / n;",
			"}",
			"",
			"var amortization = InitialAmount * rate;",
			"value = value - amortization;",
			"value = Round (value);",
			"value = Residual (value);",
			"value = Override (value);",
		};

		private static string[] defaultLinesYearsDegressive =
		{
			"Trace (\"Degressive Years\");",
			"",
			"decimal rate = 1;  // 100%",
			"decimal n = YearCount - YearRank;  // remaining years",
			"",
			"if (n > 0 &&",
			"    ResidualAmount != 0 &&",
			"    InitialAmount != 0)",
			"{",
			"    var x = ResidualAmount / InitialAmount;",
			"    var y = 1 / n;",
			"    rate = 1 - Pow (x, y);",
			"}",
			"",
			"var amortization = InitialAmount * rate;",
			"value = value - amortization;",
			"value = Round (value);",
			"value = Residual (value);",
			"value = Override (value);",
		};

		private static string[] defaultLinesZero =
		{
			"value = 0;",
		};
		//	Voir (*)

		//	(*) Le code entre ces 2 bornes ne doit pas contenir de tabulateurs. Il faut
		//		les remplacer systématiquement par 4 espaces. En effet, lorsque l'utilisateur
		//		édite le code, il ne peut pas insérer de tabulateur, car la touche Tab
		//		passe au champ suivant !
	
	}
}
