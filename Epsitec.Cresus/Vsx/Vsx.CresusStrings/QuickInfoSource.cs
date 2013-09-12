using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.ViewModels;
using Epsitec.Cresus.Strings.Views;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

// Properties / Debug / Command Line Arguments
//		/rootsuffix Exp "..\..\..\..\App.CresusGraph\App.CresusGraph.sln"
//		/rootsuffix Exp "..\..\..\..\Epsitec.Cresus.sln"
//		/rootsuffix Exp "..\..\..\Vsx.CresusStrings.Sample1\Vsx.CresusStrings.Sample1.sln"
namespace Epsitec.Cresus.Strings
{
	internal class QuickInfoSource : IQuickInfoSource
	{
		public QuickInfoSource(QuickInfoSourceProvider provider, ITextBuffer subjectBuffer)
		{
			using (new TimeTrace ("QuickInfoSource"))
			{
				this.provider = provider;
				this.subjectBuffer = subjectBuffer;
			}
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				Trace.WriteLine ("### DISPOSE");
				this.isDisposed = true;
			}
		}

		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
		{
			applicableToSpan = null;

			try
			{
				// Map the trigger point down to our buffer.
				SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint (this.subjectBuffer.CurrentSnapshot);
				if (subjectTriggerPoint.HasValue)
				{
					var point = subjectTriggerPoint.Value;
					applicableToSpan = QuickInfoSource.GetTrackingSpan (point);

					// get project resources
					var resourceController = this.Workspace.ResourceController;
					var resourceSymbolMapperTask = resourceController.SymbolMapperAsync ();
					if (resourceSymbolMapperTask.Wait (QuickInfoSource.Timeout (100), resourceController.CancellationToken))
					{
						var activeDocumentController = this.Workspace.ActiveDocumentController;
						var activeDocumentTask = activeDocumentController.DocumentAsync ();
						if (activeDocumentTask.Wait (QuickInfoSource.Timeout (100), activeDocumentController.CancellationToken))
						{
							//QuickInfoSource.AddQiContent (qiContent, this.GetQiPathsContent());

							var syntaxRootTask = activeDocumentController.SyntaxRootAsync ();
							if (syntaxRootTask.Wait (QuickInfoSource.Timeout (100), activeDocumentController.SyntaxAndSemanticCancellationToken))
							{
								var token = syntaxRootTask.Result.FindToken (point);
								var node = QuickInfoSource.GetLeftSyntaxNode (token, point);
								if (node != null)
								{
									// syntax and resources available
									applicableToSpan = QuickInfoSource.GetTrackingSpan (point, token.Span);
									var symbolName = node.RemoveTrivias ().ToString ();
									symbolName = Regex.Replace (symbolName, @"^global::", string.Empty);
									var resources = resourceSymbolMapperTask.Result.FindPartial (symbolName).ToList();
									if (resources.Count > 0)
									{
										object content = null;

										// TODO: check for multiple namespaces
										// if (HasSingleNamespace(resources))
										// {
										if (resources.Count == 1)
										{
											content = new MultiCultureResourceItemView (resources.First ());
										}
										else
										{
											content = new MultiCultureResourceItemCollectionView (resources)
											{
												MaxHeight = 600
											};
										}
										// }
										// else
										// {

										//var semanticTask = activeDocumentController.SemanticModelAsync ();
										//if (semanticTask.Wait (QuickInfoSource.Timeout (100), activeDocumentController.RoslynCancellationToken))
										//{
										//	content = this.GetQiSemanticContent (node, semanticTask.Result);
										//}
										//else
										//{
										//	QuickInfoSource.SetQiPending ("Semantic", qiContent);
										//}

										// }
										if (content != null)
										{
											qiContent.Add (content);
										}
									}
								}
							}
							else
							{
								QuickInfoSource.SetQiPending ("Syntax", qiContent);
							}
						}
						else
						{
							QuickInfoSource.SetQiPending ("Document", qiContent);
						}
					}
					else
					{
						QuickInfoSource.SetQiPending ("Resources", qiContent);
					}
				}
			}
			catch (AggregateException e)
			{
				QuickInfoSource.AddQiException (qiContent, e);
				foreach (var ie in e.InnerExceptions)
				{
					QuickInfoSource.AddQiException (qiContent, ie, "  ");
				}
			}
			catch (Exception e)
			{
				QuickInfoSource.AddQiException (qiContent, e);
			}
		}

		private static void SetQiPending(string subject, IList<object> qiContent)
		{
			QuickInfoSource.AddQiContent (qiContent, string.Format("({0} cache is still being constructed. Please try again in a few seconds...)", subject));
			Trace.WriteLine ("CANCELING");
		}

		private static void AddQiException(IList<object> qiContent, Exception e, string prefix = "Cresus Strings Extension Exception\n")
		{
			string message = string.Format ("{0}{1} : {2}", prefix, e.GetType ().Name, e.Message);
			QuickInfoSource.AddQiContent (qiContent, message);
		}

		private static SyntaxNode FilterAnySyntax(CommonSyntaxToken token)
		{
			if (token == default(CommonSyntaxToken))
			{
				return null;
			}
			return token.Parent as SyntaxNode;
		}

		private static SyntaxNode GetFullSyntaxNode(CommonSyntaxToken token, SnapshotPoint point)
		{
			if (token != default (CommonSyntaxToken))
			{
				if (token.Parent.IsMemberAccess ())
				{
					var ancestors = token.Parent.AncestorsAndSelf ();
					var properties = ancestors
						.TakeWhile (a => a.IsPropertyOrField ());
					var node = properties.LastOrDefault ();
					return node as SyntaxNode;
				}
			}
			return null;
		}

		private static SyntaxNode GetLeftSyntaxNode(CommonSyntaxToken token, SnapshotPoint point)
		{
			if (token != default (CommonSyntaxToken))
			{
				if (token.Parent.IsMemberAccess ())
				{
					var ancestors = token.Parent.AncestorsAndSelf ();
					var properties = ancestors
						.TakeWhile (a => a.Span.End <= token.Span.End && a.IsPropertyOrField ());
					var node = properties.LastOrDefault ();
					return node as SyntaxNode;
				}
			}
			return null;
		}

		private static int Timeout(int milliseconds)
		{
			//return Debugger.IsAttached ? -1 : milliseconds;
			return milliseconds;
		}

		private static void AddQiContent(IList<object> qiContent, IEnumerable<string> content)
		{
			foreach (var item in content)
			{
				QuickInfoSource.AddQiContent (qiContent, item);
			}
		}

		private static void AddQiContent(IList<object> qiContent, string message)
		{
			Trace.WriteLine (message);
			qiContent.Add (message);
		}

		private static ITrackingSpan GetTrackingSpan(SnapshotPoint snapshotPoint)
		{
			ITextSnapshot currentSnapshot = snapshotPoint.Snapshot;
			var span = new Span (snapshotPoint.Position, 0);
			return currentSnapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeExclusive);
		}

		private static ITrackingSpan GetTrackingSpan(SnapshotPoint snapshotPoint, TextSpan textSpan)
		{
			ITextSnapshot currentSnapshot = snapshotPoint.Snapshot;
			var span = Span.FromBounds (textSpan.Start, textSpan.End);
			return currentSnapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
		}

		private Epsitec.Controllers.WorkspaceController Workspace
		{
			get
			{
				return this.provider.Workspace;
			}
		}

		private IEnumerable<string> GetQiPathsContent()
		{
			yield return string.Format ("DOC : {0}", this.Workspace.ActiveDocumentController.DocumentAsync ().Result.FilePath);
			yield return string.Format ("PRJ : {0}", this.Workspace.ActiveDocumentController.DocumentAsync ().Result.Project.FilePath);
			yield return string.Format ("SLN : {0}", this.Workspace.Solution.FilePath);
		}

		private IEnumerable<string> GetQiSemanticContent(SyntaxNode node, ISemanticModel semanticModel)
		{
			ISymbol symbol = null;

			var typeSymbol = semanticModel.GetTypeInfo (node).Type;
			if (typeSymbol != null && !typeof (ErrorTypeSymbol).IsAssignableFrom (typeSymbol.GetType ()))
			{
				yield return string.Format ("TYPE : {0}", typeSymbol.ToString ());
				symbol = typeSymbol;
			}
			var symbolSymbol = semanticModel.GetSymbolInfo (node).Symbol;
			if (symbolSymbol != null)
			{
				yield return string.Format ("SYM : {0}", symbolSymbol.ToString ());
				symbol = symbolSymbol;
			}

			var declaredSymbol = semanticModel.GetDeclaredSymbol (node);
			if (declaredSymbol != null)
			{
				yield return string.Format ("DECL : {0}", declaredSymbol.ToString ());
				symbol = declaredSymbol;
			}

			if (symbol != null)
			{
				var location = symbol.Locations.FirstOrDefault ();
				if (location != null && location.SourceTree != null)
				{
					var path = location.SourceTree.FilePath;
					yield return string.Format ("PATH : {0}", path);
				}
			}
		}

		private readonly QuickInfoSourceProvider provider;
		private readonly ITextBuffer subjectBuffer;

		private bool isDisposed;
	}
}
