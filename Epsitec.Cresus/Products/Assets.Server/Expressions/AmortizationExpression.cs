//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Epsitec.Common.Widgets;
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

			var expression = AmortizationExpression.ConvertToSimpleText (taggedExpression);

			var tree = SyntaxFactory.ParseSyntaxTree (expression);

			var compilation = CSharpCompilation.Create ("calc.dll",
				options: new CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary),
				syntaxTrees: new[] { tree },
				references: new[]
				{
					MetadataReference.CreateFromAssembly (typeof (object).Assembly),
					MetadataReference.CreateFromAssembly (typeof (Data).Assembly)
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
		public static string DefaultExpression
		{
			get
			{
				return AmortizationExpression.GetExpression ("return 0;");
			}
		}

		public static string GetExpression(params string[] insideLines)
		{
			var merged = new List<string> ();

			foreach (var skeletonLine in AmortizationExpression.SkeletonLines)
			{
				if (skeletonLine.Contains ("$"))
				{
					foreach (var insideLine in insideLines)
					{
						merged.Add (skeletonLine.Replace ("$", insideLine));
					}
				}
				else
				{
					merged.Add (skeletonLine);
				}
			}

			return AmortizationExpression.JoinExpression (merged.ToArray ());
		}

		private static string JoinExpression(params string[] lines)
		{
			return AmortizationExpression.ConvertToTaggedText (string.Join ("\n", lines))
				.Replace ("<tab/>", "    ");
		}

		private static string ConvertToSimpleText(string expression)
		{
			return TextLayout.ConvertToSimpleText (expression);
		}

		private static string ConvertToTaggedText(string expression)
		{
			return TextLayout.ConvertToTaggedText (expression);
		}

		private static string[] SkeletonLines =
		{
			"using Epsitec.Cresus.Assets.Server.Expression;", 
			"", 
			"public static class Calculator", 
			"{", 
			"	public static object Evaluate(Data data)", 
			"	{", 
			"		$",  // partie centrale remplacée par du code
			"	}", 
			"}", 
		};
		#endregion


		#region Standard calculators
		public static class Calculator_RateLinear
		{
			public static object Evaluate(Data data)
			{
				if (data.ForcedAmount.HasValue)
				{
					return data.ForcedAmount.Value;
				}
				else
				{
					var rate = data.Rate * data.PeriodicityFactor;

					if (data.ProrataDenominator != 0)
					{
						rate *= data.ProrataNumerator / data.ProrataDenominator;
					}

					var amortization = data.BaseAmount * rate;
					var value = data.InitialAmount - amortization;
					value = data.Round (value);
					return data.Residual (value);
				}
			}
		}

		public static class Calculator_RateDegressive
		{
			public static object Evaluate(Data data)
			{
				if (data.ForcedAmount.HasValue)
				{
					return data.ForcedAmount.Value;
				}
				else
				{
					var rate = data.Rate * data.PeriodicityFactor;

					if (data.ProrataDenominator != 0)
					{
						rate *= data.ProrataNumerator / data.ProrataDenominator;
					}

					var amortization = data.InitialAmount * rate;
					var value = data.InitialAmount - amortization;
					value = data.Round (value);
					return data.Residual (value);
				}
			}
		}

		public static class Calculator_YearsLinear
		{
			public static object Evaluate(Data data)
			{
				if (data.ForcedAmount.HasValue)
				{
					return data.ForcedAmount.Value;
				}
				else
				{
					var rate = 1.0m;
					decimal n = data.YearCount - data.YearRank;  // nb d'années restantes

					if (n > 0)
					{
						rate = 1.0m / n;
					}

					var amortization = data.InitialAmount * rate;
					var value = data.InitialAmount - amortization;
					value = data.Round (value);
					return data.Residual (value);
				}
			}
		}

		public static class Calculator_YearsDegressive
		{
			public static object Evaluate(Data data)
			{
				if (data.ForcedAmount.HasValue)
				{
					return data.ForcedAmount.Value;
				}
				else
				{
					var rate = 1.0m;
					decimal n = data.YearCount - data.YearRank;  // nb d'années restantes

					if (n > 0 && data.ResidualAmount != 0 && data.InitialAmount != 0)
					{
						var x = data.ResidualAmount / data.InitialAmount;
						var y = 1.0m / n;
						rate = 1.0m - (decimal) System.Math.Pow ((double) x, (double) y);
					}

					var amortization = data.InitialAmount * rate;
					var value = data.InitialAmount - amortization;
					value = data.Round (value);
					return data.Residual (value);
				}
			}
		}
		#endregion


		public string Error
		{
			get
			{
				return this.error;
			}
		}


		public decimal? Evaluate(Data data)
		{
			if (this.compiledAssembly != null)
			{
				System.Type calculator = this.compiledAssembly.GetType ("Calculator");
				MethodInfo evaluate = calculator.GetMethod ("Evaluate");

				object[] parameters = { data };

				object answer = evaluate.Invoke (null, parameters);
				return AmortizationExpression.CastResult (answer);
			}

			return null;
		}

		public static decimal? CastResult(object answer)
		{
			if (answer is decimal)
			{
				return (decimal) answer;
			}
			else if (answer is double)
			{
				var a = (double) answer;
				return (decimal) a;
			}
			else if (answer is int)
			{
				var a = (int) answer;
				return (decimal) a;
			}
			else
			{
				return null;
			}
		}


		private readonly Assembly				compiledAssembly;
		private readonly string					error;
	}
}
