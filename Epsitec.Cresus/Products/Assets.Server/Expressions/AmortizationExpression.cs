//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
		public AmortizationExpression(string taggedArguments, string taggedExpression)
		{
			if (string.IsNullOrEmpty (taggedExpression))
			{
				this.error = "Empty expression";  // anglais, ne pas traduire
				return;
			}

			var expression = AmortizationExpression.Skeleton
				.Replace ("$args$", AmortizationExpression.ConvertToSimpleText (taggedArguments))
				.Replace ("$exp$",  AmortizationExpression.ConvertToSimpleText (taggedExpression));

			var tree = SyntaxFactory.ParseSyntaxTree (expression);

			var compilation = CSharpCompilation.Create ("calc.dll",
				options: new CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary),
				syntaxTrees: new[] { tree },
				references: new[]
				{
					MetadataReference.CreateFromAssembly (typeof (object).Assembly),
					MetadataReference.CreateFromAssembly (typeof (AbstractCalculator).Assembly),
					MetadataReference.CreateFromAssembly (typeof (AmortizationDetails).Assembly),
					MetadataReference.CreateFromAssembly (typeof (DateRange).Assembly)
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


		public string Error
		{
			get
			{
				return this.error;
			}
		}

		public AbstractCalculator.Result Evaluate(AmortizationDetails details)
		{
			if (this.compiledAssembly != null)
			{
				System.Type calculator = this.compiledAssembly.GetType ("Calculator");
				MethodInfo evaluate = calculator.GetMethod ("Evaluate");

				object[] parameters = { details };

				return (AbstractCalculator.Result) evaluate.Invoke (null, parameters);
			}

			return AbstractCalculator.Result.Empty;
		}


		#region Expression
		public static string GetDebugExpression(string arguments, string inside)
		{
			//	Retourne le code C# complet avec les lignes numérotées.
			var a = AmortizationExpression.ConvertToSimpleText (arguments);
			var b = AmortizationExpression.ConvertToSimpleText (inside);

			var exp = AmortizationExpression.Skeleton.Replace ("$args$", a).Replace ("$exp$", b);
			var lines = exp.Split ('\n');

			var list = new List<string> ();
			int index = 1;  // numérote les lignes à partir de 1 pour concorder avec les messages d'erreur

			foreach (var line in lines)
			{
				string s = string.Format ("{0}:\t{1}",
					TypeConverters.IntToString (index++),
					line.Replace ("\t", "    "));  // indentation classique avec 4 espaces

				list.Add (s);
			}

			return AmortizationExpression.ConvertToTaggedText (string.Join ("\n", list));
		}

		private static string ConvertToSimpleText(string expression)
		{
			return TextLayout.ConvertToSimpleText (expression);
		}

		public static string ConvertToTaggedText(string expression)
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

		private static string[] skeletonLines =
		{
			"//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland",
			"//	Author: Daniel ROUX, Maintainer: Daniel ROUX",
			"",
			"using Epsitec.Cresus.Assets.Server.BusinessLogic;",
			"using Epsitec.Cresus.Assets.Server.Expression;",
			"",
			"public static class Calculator",
			"{",
			"	public static AbstractCalculator.Result Evaluate(AmortizationDetails details)",
			"	{",
			"		var calculator = new InternalCalculator(details);",
			"		var value = calculator.Evaluate();",
			"		var trace = calculator.GetTraces();",
			"		return new AbstractCalculator.Result (value, trace);",
			"	}",
			"",
			"	private class InternalCalculator : AbstractCalculator",
			"	{",
			"		public InternalCalculator(AmortizationDetails details)",
			"			: base (details)",
			"		{",
			"		}",
			"",
			"		public override decimal Evaluate()",
			"		{",
			"			decimal value = this.InitialAmount;",
			"",
			"//-------------------------------------------------------------------------",
			"$args$",
			"//-------------------------------------------------------------------------",
			"$exp$",
			"//-------------------------------------------------------------------------",
			"",
			"			return value;",
			"		}",
			"	}",
			"}",
		};
		#endregion


		public static string GetArgumentCode(ArgumentType type, bool nullable,
			string variable, string def, string desc)
		{
			//	Retourne une ligne de code C# permettant de déclarer la variable
			//	correspondant à l'argument.
			//	Par exemple:
			//	"decimal Rate = 0.1m; // Taux d'amortissement"
			//	"string Name = "coucou"; // Message"
			//	"System.DateTime Date = new System.DateTime (2014, 12, 31); // Début"

			var builder = new System.Text.StringBuilder ();

			builder.Append (AmortizationExpression.GetArgumentType (type));

			if (nullable && type != ArgumentType.String)
			{
				builder.Append ("?");
			}

			builder.Append (" ");
			builder.Append (variable);

			if (!string.IsNullOrEmpty (def))
			{
				builder.Append (" = ");

				switch (type)
				{
					case ArgumentType.Decimal:
					case ArgumentType.Amount:
					case ArgumentType.Rate:
						if (def.Last () == '%')
						{
							var n = TypeConverters.ParseRate (def);
							def = TypeConverters.DecimalToString (n);
						}
						else
						{
							var n = TypeConverters.ParseAmount (def);
							def = TypeConverters.DecimalToString (n);
						}
						builder.Append (def);

						if (def.Last () != 'm')
						{
							builder.Append ("m");
						}
						break;

					case ArgumentType.Date:
						var date = TypeConverters.ParseDate (def, System.DateTime.Now, System.DateTime.MinValue, System.DateTime.MaxValue).GetValueOrDefault ();
						builder.Append ("new System.DateTime (");
						builder.Append (date.Year);
						builder.Append (", ");
						builder.Append (date.Month);
						builder.Append (", ");
						builder.Append (date.Day);
						builder.Append (")");
						break;

					case ArgumentType.String:
						builder.Append ("\"");
						builder.Append (def.Replace ("\"", "\\\""));
						builder.Append ("\"");
						break;

					default:
						builder.Append (def);
						break;
				}
			}

			builder.Append (";");

			if (!string.IsNullOrEmpty (desc))
			{
				builder.Append (" // ");
				builder.Append (desc);
			}

			return builder.ToString ();
		}

		public static string GetArgumentType(ArgumentType type)
		{
			//	Retourne le type exact tel qu'il s'écrit en C#.
			switch (type)
			{
				case ArgumentType.String:
					return "string";

				case ArgumentType.Int:
					return "int";

				case ArgumentType.Decimal:
				case ArgumentType.Amount:
				case ArgumentType.Rate:
					return "decimal";

				case ArgumentType.Date:
					return "System.DateTime";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid ArgumentType {0}", type));
			}
		}


		private readonly Assembly				compiledAssembly;
		private readonly string					error;
	}
}
