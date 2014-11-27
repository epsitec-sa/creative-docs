//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	/// <summary>
	/// Gère une expression d'amortissement, qui est immédiatement compilée.
	/// Cette classe est immutable.
	/// </summary>
	public class AmortizationExpression : System.IDisposable
	{
		public AmortizationExpression(string taggedExpression)
		{
			if (string.IsNullOrEmpty (taggedExpression))
			{
				this.error = "Empty expression";  // anglais, ne pas traduire
				return;
			}

			var expression = AmortizationExpression.Skeleton
				.Replace ("$", AmortizationExpression.ConvertToSimpleText (taggedExpression));

			var tree = SyntaxFactory.ParseSyntaxTree (expression);

			var compilation = CSharpCompilation.Create ("calc.dll",
				options: new CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary),
				syntaxTrees: new[] { tree },
				references: new[]
				{
					MetadataReference.CreateFromAssembly (typeof (object).Assembly),
					MetadataReference.CreateFromAssembly (typeof (AbstractCalculator).Assembly),
					MetadataReference.CreateFromAssembly (typeof (AmortizedAmount).Assembly)
				});

			using (var stream = new MemoryStream ())
			{
				var compileResult = compilation.Emit (stream);
				if (compileResult.Success)
				{
					this.compiledAssembly = Assembly.Load (stream.GetBuffer ());
				}
				else
				{
					// Anglais, ne pas traduire.
					this.error = string.Format ("Failed to compile:<br/>{0}", string.Join ("<br/>", compileResult.Diagnostics.Select (x => x.ToString ())));
				}
			}
		}

		public void Dispose()
		{
		}


		#region Expression
		public static string GetDebugExpression(string inside)
		{
			//	Retourne le code C# complet avec les lignes numérotées.
			var x = AmortizationExpression.ConvertToSimpleText (inside);
			var y = AmortizationExpression.Skeleton.Replace ("$", x);
			var z = y.Split ('\n');

			var list = new List<string> ();
			int index = 1;

			foreach (var line in z)
			{
				string s = string.Format ("{0}:\t{1}", TypeConverters.IntToString (index++), line);
				list.Add (s);
			}

			return AmortizationExpression.ConvertToTaggedText (string.Join ("\n", list));
		}

		public static string GetDefaultExpression(SampleType type)
		{
			switch (type)
			{
				case SampleType.RateLinear:
					return AmortizationExpression.Format (AmortizationExpression.defaultLinesRateLinear);

				case SampleType.RateDegressive:
					return AmortizationExpression.Format (AmortizationExpression.defaultLinesRateDegressive);

				case SampleType.YearsLinear:
					return AmortizationExpression.Format (AmortizationExpression.defaultLinesYearsLinear);

				case SampleType.YearsDegressive:
					return AmortizationExpression.Format (AmortizationExpression.defaultLinesYearsDegressive);

				default:
					return AmortizationExpression.Format (AmortizationExpression.defaultLinesNull);

			}
		}

		public static string Format(params string[] lines)
		{
			return AmortizationExpression.ConvertToTaggedText (string.Join ("\n", lines));
		}

		private static string ConvertToSimpleText(string expression)
		{
			return TextLayout.ConvertToSimpleText (expression);
		}

		private static string ConvertToTaggedText(string expression)
		{
			return TextLayout.ConvertToTaggedText (expression);
		}

		private static string Skeleton
		{
			get
			{
				return string.Join ("\n", AmortizationExpression.skeletonLines);
			}
		}

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

		private static string[] defaultLinesNull =
		{
			"value = 0;",
		};

		private static string[] skeletonLines =
		{
			"//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland",
			"//	Author: Daniel ROUX, Maintainer: Daniel ROUX",
			"",
			"using Epsitec.Cresus.Assets.Data;",
			"using Epsitec.Cresus.Assets.Server.Expression;",
			"",
			"public static class Calculator",
			"{",
			"	public static AbstractCalculator.Result Evaluate(AmortizedAmount amount)",
			"	{",
			"		var calculator = new InternalCalculator(amount);",
			"		var value = calculator.Evaluate();",
			"		var trace = calculator.GetTraces();",
			"		return new AbstractCalculator.Result (value, trace);",
			"	}",
			"",
			"	private class InternalCalculator : AbstractCalculator",
			"	{",
			"		public InternalCalculator(AmortizedAmount amount)",
			"			: base (amount)",
			"		{",
			"		}",
			"",
			"		public override decimal Evaluate()",
			"		{",
			"			decimal value = this.InitialAmount;",
			"",
			"//----------------------------------------------",
			"$",
			"//----------------------------------------------",
			"",
			"			return value;",
			"		}",
			"	}",
			"}",
		};
		#endregion


		public string Error
		{
			get
			{
				return this.error;
			}
		}


		public AbstractCalculator.Result Evaluate(AmortizedAmount amount)
		{
			if (this.compiledAssembly != null)
			{
				System.Type calculator = this.compiledAssembly.GetType ("Calculator");
				MethodInfo evaluate = calculator.GetMethod ("Evaluate");

				object[] parameters = { amount };

				return (AbstractCalculator.Result) evaluate.Invoke (null, parameters);
			}

			return AbstractCalculator.Result.Empty;
		}


		private readonly Assembly				compiledAssembly;
		private readonly string					error;
	}
}
