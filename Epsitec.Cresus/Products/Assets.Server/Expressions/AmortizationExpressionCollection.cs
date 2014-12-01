//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	/// <summary>
	/// Gestion de la collection des expression d'amortissements prédéfinies.
	/// </summary>
	public static class AmortizationExpressionCollection
	{
		public static string GetExpression(AmortizationExpressionType type)
		{
			//	Retourne l'expression correspondant à un type donné.
			return AmortizationExpressionCollection.Items
				.Where (x => x.Type == type)
				.Select (x => x.Expression)
				.FirstOrDefault ();
		}

		public static IEnumerable<AmortizationExpressionItem> Items
		{
			//	Enumère toutes les expressions prédéfinies.
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

		
		//	Code C# des expressions d'amortissements prédéfinies. Les propriétés et méthodes
		//	accessibles sont définies dans AbstractCalculator. Comme ces expressions sont
		//	modifiables par l'utilisateur, elles sont simplifiées à l'extrême, quitte à
		//	déroger à certains règles d'écriture. Par exemple, "this." est ici systématiquement
		//	omis. De même, l'opérateur "-=" n'est pas utilisé.
		//	Ces expressions sont injectées dans un code C# plus complexe, qui est défini
		//	dans AmortizationExpression.skeletonLines. Le tout est ensuite compilé à la
		//	volée avec Roselyn (librairies Microsoft.CodeAnalysis).

		private static string[] rateLinearLines =
		{
			"Trace (\"Linear Rate\");",
			"",
			"if (PeriodCount == 1 ||",
			"	PeriodRank % PeriodCount != PeriodCount-1)",
			"{",
			"	var rate = Rate * PeriodicityFactor * ProrataFactor;",
			"	var amortization = BaseAmount * rate;",
			"",
			"	value = value - amortization;",
			"	value = Round (value);",
			"	value = Residual (value);",
			"}",
			"else",
			"{",
			"	//	If last Period -> adjust.",
			"	var rate = Rate * ProrataFactor;",
			"	var amortization = BaseAmount * rate;",
			"",
			"	value = StartYearAmount - amortization;",
			"	value = Round (value);",
			"	value = Residual (value);",
			"}",
			"",
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
			"	rate = 1 / n;",
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
			"	ResidualAmount != 0 &&",
			"	InitialAmount != 0)",
			"{",
			"	var x = ResidualAmount / InitialAmount;",
			"	var y = 1 / n;",
			"	rate = 1 - Pow (x, y);",
			"}",
			"",
			"var amortization = InitialAmount * rate;",
			"value = value - amortization;",
			"value = Round (value);",
			"value = Residual (value);",
			"value = Override (value);",
		};
	}
}
