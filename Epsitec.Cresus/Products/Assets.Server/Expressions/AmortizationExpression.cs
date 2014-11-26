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
		public static string DefaultExpression
		{
			get
			{
				return AmortizationExpression.Format (AmortizationExpression.defaultLines);
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

		private static string[] defaultLines =
		{
			"var rate = Rate * PeriodicityFactor * ProrataFactor;",
			"var amortization = BaseAmount * rate;",
			"",
			"value = value - amortization;",
			"value = Round (value);",
			"value = Residual (value);",
			"value = Override (value);",
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
			"	public static object Evaluate(AmortizedAmount amount)",
			"	{",
			"		var calculator = new InternalCalculator(amount);",
			"		return calculator.Evaluate();",
			"	}",
			"}",
			"",
			"public class InternalCalculator : AbstractCalculator",
			"{",
			"	public InternalCalculator(AmortizedAmount amount)",
			"		: base (amount)",
			"	{",
			"	}",
			"",
			"	public override object Evaluate()",
			"	{",
			"		decimal value = this.InitialAmount;",
			"",
			"//----------------------------------------------",
			"$",
			"//----------------------------------------------",
			"",
			"		return value;",
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


		public decimal? Evaluate(AmortizedAmount amount)
		{
			if (this.compiledAssembly != null)
			{
				System.Type calculator = this.compiledAssembly.GetType ("Calculator");
				MethodInfo evaluate = calculator.GetMethod ("Evaluate");

				object[] parameters = { amount };

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
