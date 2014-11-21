//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
			if (!expression.StartsWith ("public static class Calculator"))
			{
				expression =
					("public static class Calculator\n" +
					"{\n" +
					"    public static object Evaluate(decimal value, decimal rate, int count)\n" +
					"    {\n" + 
					"        return $;\n" +
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


		public string Error
		{
			get
			{
				return this.error;
			}
		}


		public decimal? Evaluate(decimal value, decimal rate, int count)
		{
			if (this.compiledAssembly != null)
			{
				System.Type calculator = this.compiledAssembly.GetType ("Calculator");
				MethodInfo evaluate = calculator.GetMethod ("Evaluate");

				object[] parameters = { value, rate, count };

				object answer = evaluate.Invoke (null, parameters);

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
			}

			return null;
		}


		private readonly Assembly				compiledAssembly;
		private readonly string					error;
	}
}
