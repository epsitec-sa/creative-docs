using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace Vsx.CresusDesignerTest
{
	[TestClass]
	public class RoslynTest
	{
		[TestMethod]
		public void SampleCodeSyntaxTest()
		{
			SyntaxTree tree = SyntaxTree.ParseText (RoslynTest.SampleCode1);

			var root = tree.GetRoot ();
		}

		[TestMethod]
		public void SampleFileSemanticTest()
		{
			var text = File.ReadAllText (RoslynTest.SampleFilePath);
			double totalMilliseconds = 0;
			for (int i = 0; i < 100; ++i)
			{
				var stopwatch = Stopwatch.StartNew ();
				var syntaxTree = SyntaxTree.ParseText (text);
				var compilation = Compilation.Create ("CresusStrings")
					.AddReferences (MetadataReference.CreateAssemblyReference ("mscorlib"))
					.AddSyntaxTrees (syntaxTree);
				var semanticModel = compilation.GetSemanticModel (syntaxTree);
				var duration = stopwatch.Elapsed;
				totalMilliseconds += duration.TotalMilliseconds;
				Trace.WriteLine (duration.TotalMilliseconds);
			}
			Trace.WriteLine (string.Format ("Mean parsing time : {0} [ms]", totalMilliseconds / 100));

		}
		[TestMethod]
		public void SelfContainedSemanticDocument()
		{
			var syntaxTree = SyntaxTree.ParseText (File.ReadAllText (RoslynTest.MultiFilePath1));
			var compilation = Compilation.Create ("CresusStrings")
				.AddReferences (MetadataReference.CreateAssemblyReference ("mscorlib"))
				//.AddReferences (MetadataReference.CreateAssemblyReference ("System"))
				//.AddReferences (new MetadataFileReference (@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll"))
				.AddSyntaxTrees (syntaxTree);

			var semanticModel = compilation.GetSemanticModel (syntaxTree);
			var walker = new ExpressionWalker ()
			{
				SemanticModel = semanticModel
			};

			walker.Visit (syntaxTree.GetRoot ());
			Trace.WriteLine (walker.Results);
		}

		[TestMethod]
		public void IncompleteSemanticDocument()
		{
			var syntaxTree = SyntaxTree.ParseText (File.ReadAllText (RoslynTest.MultiFilePath2));
			var compilation = Compilation.Create ("CresusStrings")
				.AddReferences (MetadataReference.CreateAssemblyReference ("mscorlib"))
				.AddSyntaxTrees (syntaxTree);
			var semanticModel = compilation.GetSemanticModel (syntaxTree);
			var walker = new ExpressionWalker ()
			{
				SemanticModel = semanticModel
			};

			walker.Visit (syntaxTree.GetRoot ());
			Trace.WriteLine (walker.Results);
		}

		[TestMethod]
		public void MultiDocumentSemantic()
		{
			var syntaxTree1 = SyntaxTree.ParseText (File.ReadAllText (RoslynTest.MultiFilePath1));
			var syntaxTree2 = SyntaxTree.ParseText (File.ReadAllText (RoslynTest.MultiFilePath2));
			var compilation = Compilation.Create ("CresusStrings")
				.AddReferences (MetadataReference.CreateAssemblyReference ("mscorlib"))
				.AddSyntaxTrees (syntaxTree1, syntaxTree2);

			var semanticModel = compilation.GetSemanticModel (syntaxTree2);
			var walker = new ExpressionWalker ()
			{
				SemanticModel = semanticModel
			};

			walker.Visit (syntaxTree2.GetRoot ());
			Trace.WriteLine (walker.Results);
		}

		public class ExpressionWalker : SyntaxWalker
		{
			public SemanticModel SemanticModel
			{
				get;
				set;
			}
			public StringBuilder Results
			{
				get;
				private set;
			}

			public ExpressionWalker()
			{
				this.Results = new StringBuilder ();
			}

			public override void Visit(SyntaxNode node)
			{
				if (node is ExpressionSyntax)
				{
					TypeSymbol type = this.SemanticModel.GetTypeInfo ((ExpressionSyntax) node).Type;
					if (type != null)
					{
						this.Results.AppendLine ();
						this.Results.Append (node.GetType ().Name);
						this.Results.Append (" ");
						this.Results.Append (node.ToString ());
						this.Results.Append (" has type ");
						this.Results.Append (type.ToDisplayString ());
					}
				}

				base.Visit (node);
			}
		}

		private const string MultiFilePath1 = @"..\..\..\Vsx.CresusStrings.Sample1\SomeClass.cs";
		private const string MultiFilePath2 = @"..\..\..\Vsx.CresusStrings.Sample1\SomeClassInOtherDocument.cs";

		private const string SampleFilePath = @"..\..\WorkspaceController.cs";
		private const string SampleCode1 =
@"
using System;
using System.Collections;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;
using Epsitec.Cresus.Graph.Widgets;
 
namespace HelloWorld
{
	class SampleClass
	{
		public void Foo()
		{
			var message1 = new StaticText ()
			{
				Parent = messageFrame,
				Dock = DockStyle.Top,
				Text = string.Format (Res/* TEST */
                    .Strings.
                    Message
                    .FreePiccoloBecauseOfCompta.
                    ToString (), date.ToShortDateString ()),
				PreferredHeight = 36,
				Margins = new Margins (4, 4, 0, 0),
			};
		}
	}
}";

	}
}
