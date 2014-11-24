//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Epsitec.Common.Widgets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	/// <summary>
	/// Gère une expression d'amortissement, qui est immédiatement compilée.
	/// Cette classe est immutable.
	/// </summary>
	public class AmortizationExpression
	{
		public AmortizationExpression(string expression)
		{
			expression = TextLayout.ConvertToSimpleText (expression);

			if (!expression.StartsWith ("public static class Calculator"))
			{
				expression =
					("public static class Calculator\n" +
					"{\n" +
					"    public static object Evaluate(" +
							"decimal? forcedAmount, decimal baseAmount, decimal initialAmount, " +
							"decimal residualAmount, decimal roundAmount, " +
							"decimal rate, decimal periodicityFactor, " +
							"decimal prorataNumerator, decimal prorataDenominator, " +
							"decimal yearCount, int yearRank)\n" +
					"    {\n" + 
					"        $;\n" +
					"    }\n" +
					"}").Replace ("$", expression);
			}

			var tree = SyntaxFactory.ParseSyntaxTree (expression);

			var compilation = CSharpCompilation.Create ("calc.dll",
				options: new CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary),
				syntaxTrees: new[] { tree },
				references: new[] { MetadataReference.CreateFromAssembly (typeof (object).Assembly) });

			using (var stream = new MemoryStream ())
			{
				var compileResult = compilation.Emit (stream);
				if (compileResult.Success)
				{
					this.compiledAssembly = Assembly.Load (stream.GetBuffer ());
				}
				else
				{
					this.error = string.Format ("Failed to compile:\n{0}", string.Join ("\n", compileResult.Diagnostics.Select (x => x.ToString ())));
				}
			}
		}


		#region Compilation samples
		public static class Calculator_RateLinear
		{
			public static object Evaluate(
				decimal? forcedAmount, decimal baseAmount, decimal initialAmount,
				decimal residualAmount, decimal roundAmount,
				decimal rate, decimal periodicityFactor,
				decimal prorataNumerator, decimal prorataDenominator,
				decimal yearCount, int yearRank)
			{
				if (forcedAmount.HasValue)
				{
					return forcedAmount.Value;
				}
				else
				{
					rate *= periodicityFactor;

					if (prorataDenominator != 0)
					{
						rate *= prorataNumerator / prorataDenominator;
					}

					var amortization = baseAmount * rate;
					var value = initialAmount - amortization;

					if (roundAmount > 0)
					{
						if (value < 0)
						{
							value -= roundAmount/2;
						}
						else
						{
							value += roundAmount/2;
						}

						value -= (value % roundAmount);
					}

					return value = System.Math.Max (value, residualAmount);
				}
			}
		}

		public static class Calculator_RateDegressive
		{
			public static object Evaluate(
				decimal? forcedAmount, decimal baseAmount, decimal initialAmount,
				decimal residualAmount, decimal roundAmount,
				decimal rate, decimal periodicityFactor,
				decimal prorataNumerator, decimal prorataDenominator,
				decimal yearCount, int yearRank)
			{
				if (forcedAmount.HasValue)
				{
					return forcedAmount.Value;
				}
				else
				{
					rate *= periodicityFactor;

					if (prorataDenominator != 0)
					{
						rate *= prorataNumerator / prorataDenominator;
					}

					var amortization = initialAmount * rate;
					var value = initialAmount - amortization;

					if (roundAmount > 0)
					{
						if (value < 0)
						{
							value -= roundAmount/2;
						}
						else
						{
							value += roundAmount/2;
						}

						value -= (value % roundAmount);
					}

					return value = System.Math.Max (value, residualAmount);
				}
			}
		}

		public static class Calculator_YearsLinear
		{
			public static object Evaluate(
				decimal? forcedAmount, decimal baseAmount, decimal initialAmount,
				decimal residualAmount, decimal roundAmount,
				decimal rate, decimal periodicityFactor,
				decimal prorataNumerator, decimal prorataDenominator,
				decimal yearCount, int yearRank)
			{
				if (forcedAmount.HasValue)
				{
					return forcedAmount.Value;
				}
				else
				{
					rate = 1.0m;
					decimal n = yearCount - yearRank;  // nb d'années restantes

					if (n > 0)
					{
						rate = 1.0m / n;
					}

					var amortization = initialAmount * rate;
					var value = initialAmount - amortization;

					if (roundAmount > 0)
					{
						if (value < 0)
						{
							value -= roundAmount/2;
						}
						else
						{
							value += roundAmount/2;
						}

						value -= (value % roundAmount);
					}

					return value = System.Math.Max (value, residualAmount);
				}
			}
		}

		public static class Calculator_YearsDegressive
		{
			public static object Evaluate(
				decimal? forcedAmount, decimal baseAmount, decimal initialAmount,
				decimal residualAmount, decimal roundAmount,
				decimal rate, decimal periodicityFactor,
				decimal prorataNumerator, decimal prorataDenominator,
				decimal yearCount, int yearRank)
			{
				if (forcedAmount.HasValue)
				{
					return forcedAmount.Value;
				}
				else
				{
					rate = 1.0m;
					decimal n = yearCount - yearRank;  // nb d'années restantes

					if (n > 0)
					{
						if (residualAmount == 0 || initialAmount == 0)
						{
							rate = 1.0m;
						}
						else
						{
							var x = residualAmount / initialAmount;
							var y = 1.0m / n;
							rate = 1.0m - (decimal) System.Math.Pow ((double) x, (double) y);
						}
					}

					var amortization = initialAmount * rate;
					var value = initialAmount - amortization;

					if (roundAmount > 0)
					{
						if (value < 0)
						{
							value -= roundAmount/2;
						}
						else
						{
							value += roundAmount/2;
						}

						value -= (value % roundAmount);
					}

					return value = System.Math.Max (value, residualAmount);
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


		public decimal? Evaluate(decimal? forcedAmount, decimal baseAmount, decimal initialAmount,
			decimal residualAmount, decimal roundAmount,
			decimal rate, decimal periodicityFactor,
			decimal prorataNumerator, decimal prorataDenominator,
			decimal yearCount, int yearRank)
		{
			if (this.compiledAssembly != null)
			{
				System.Type calculator = this.compiledAssembly.GetType ("Calculator");
				MethodInfo evaluate = calculator.GetMethod ("Evaluate");

				object[] parameters =
				{
					forcedAmount, baseAmount, initialAmount,
					residualAmount, roundAmount,
					rate, periodicityFactor,
					prorataNumerator, prorataDenominator,
					yearCount, yearRank
				};

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
