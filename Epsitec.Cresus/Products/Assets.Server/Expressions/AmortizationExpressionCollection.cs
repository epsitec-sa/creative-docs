//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public static class AmortizationExpressionCollection
	{
		public static string GetExpression(AmortizationExpressionType type)
		{
			return AmortizationExpressionCollection.Items
				.Where (x => x.Type == type)
				.Select (x => x.Expression)
				.FirstOrDefault ();
		}

		public static IEnumerable<AmortizationExpressionItem> Items
		{
			get
			{
				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.RateLinear,
					Res.Strings.Enum.AmortizationExpressionType.RateLinear.ToString (),
					AmortizationExpressionCollection.rateLinearLines);

				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.RateDegressive,
					Res.Strings.Enum.AmortizationExpressionType.RateDegressive.ToString (),
					AmortizationExpressionCollection.rateDegressiveLines);

				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.YearsLinear,
					Res.Strings.Enum.AmortizationExpressionType.YearsLinear.ToString (),
					AmortizationExpressionCollection.yearsLinearLines);

				yield return new AmortizationExpressionItem (
					AmortizationExpressionType.YearsDegressive,
					Res.Strings.Enum.AmortizationExpressionType.YearsDegressive.ToString (),
					AmortizationExpressionCollection.yearsDegressiveLines);
			}
		}


		//	Voir (*)
		private static string[] zeroLines =
		{
			"value = 0;",
		};

		private static string[] rateLinearLines =
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

		private static string[] rateDegressiveLines =
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

		private static string[] yearsLinearLines =
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

		private static string[] yearsDegressiveLines =
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
		//	Voir (*)

		//	(*) Le code entre ces 2 bornes ne doit pas contenir de tabulateurs. Il faut
		//		les remplacer systématiquement par 4 espaces. En effet, lorsque l'utilisateur
		//		édite le code, il ne peut pas insérer de tabulateur, car la touche Tab
		//		passe au champ suivant !
	
	}
}
