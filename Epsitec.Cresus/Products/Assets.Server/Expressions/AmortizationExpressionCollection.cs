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
					AmortizationExpressionType.None,
					Res.Strings.Enum.AmortizationExpressionType.None.ToString (),
					null);

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
			"var start = BaseDate;",
			"",
			"if (!Prorata)",
			"{",
			"    start = new System.DateTime (start.Year, 1, 1);",
			"}",
			"",
			"var m = Months (start, EndDate);",
			"value = BaseAmount * (1-(Rate*m/12));",
			"value = Round (value, RoundAmount);",
			"value = Residual (value, ResidualAmount);",
		};

		private static string[] rateDegressiveLines =
		{
			"var start = BaseDate;",
			"",
			"if (!Prorata)",
			"{",
			"    start = new System.DateTime (start.Year, 1, 1);",
			"}",
			"",
			"var m = Months (start, EndDate);",
			"value = BaseAmount * Pow (1-Rate, m/12);",
			"value = Round (value, RoundAmount);",
			"value = Residual (value, ResidualAmount);",
		};

		private static string[] yearsLinearLines =
		{
			"var start = BaseDate;",
			"",
			"if (!Prorata)",
			"{",
			"    start = new System.DateTime (start.Year, 1, 1);",
			"}",
			"",
			"var m = Months (start, EndDate);",
			"var rate = (m/12) / BaseYearCount;",
			"",
			"value = BaseAmount * (1-rate);",
			"value = Round (value, RoundAmount);",
			"value = Residual (value, ResidualAmount);",
		};

		private static string[] yearsDegressiveLines =
		{
			"var start = BaseDate;",
			"",
			"if (!Prorata)",
			"{",
			"    start = new System.DateTime (start.Year, 1, 1);",
			"}",
			"",
			"var m = Months (start, EndDate);",
			"var x = Pow (ResidualAmount/BaseAmount, 1/BaseYearCount);",
			"var rate = Pow (x, m/12);",
			"",
			"value = BaseAmount * rate;",
			"value = Round (value, RoundAmount);",
			"value = Residual (value, ResidualAmount);",
		};
	}
}
