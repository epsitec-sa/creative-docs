using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

// Properties / Debug / Command Line Arguments
//		/rootsuffix Exp "..\..\..\..\App.CresusGraph\App.CresusGraph.sln"
//		/rootsuffix Exp "..\..\..\Vsx.CresusStrings.Sample1\Vsx.CresusStrings.Sample1.sln"

namespace Epsitec.Cresus.Strings
{
	internal class QuickInfoSource : IQuickInfoSource
	{
		public QuickInfoSource(QuickInfoSourceProvider provider, ITextBuffer subjectBuffer, ISolution solution)
		{
			this.provider = provider;
			this.subjectBuffer = subjectBuffer;
			this.subjectBuffer.Changed += this.HandleSubjectBufferChanged;
			this.cts = new CancellationTokenSource ();
			this.solutionResourceTask = Task.Run (() => new SolutionResource (solution, this.cts.Token), this.cts.Token);
			this.Initialize (solution, this.cts.Token);
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				Trace.WriteLine ("### DISPOSE");
				this.isDisposed = true;
				this.subjectBuffer.Changed -= this.HandleSubjectBufferChanged;
				this.cts.Cancel ();
				this.documentTask.ForgetSafely ();
				this.syntaxRootTask.ForgetSafely ();
				this.semanticModelTask.ForgetSafely ();
				this.solutionResourceTask.ForgetSafely ();
			}
		}

		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
		{
			applicableToSpan = null;

			// Map the trigger point down to our buffer.
			SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint (this.subjectBuffer.CurrentSnapshot);
			if (subjectTriggerPoint.HasValue)
			{
				var point = subjectTriggerPoint.Value;

				var cts = new CancellationTokenSource ();

				if (this.documentTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
				{
					QuickInfoSource.AddQiContent (qiContent, this.GetQiPathsContent());

					var syntaxTask = this.GetQiSyntaxDataAsync (point, cts.Token);
					if (syntaxTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
					{
						var syntaxData = syntaxTask.Result;
						if (syntaxData != null)
						{
							var token = syntaxData.Item1;
							var node = syntaxData.Item2;
							if (node != null)
							{
								QuickInfoSource.AddQiContent (qiContent, syntaxData.Item3);

								// get project resources
								if (this.solutionResourceTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
								{
									var document = this.documentTask.Result;
									var project = document.Project;
									var solutionResource = this.solutionResourceTask.Result;
									var projectResource = solutionResource[project.Id];
									QuickInfoSource.AddQiContent (qiContent, string.Format("RES: {0}", projectResource));
								}
								else
								{
									cts.Cancel ();
									applicableToSpan = QuickInfoSource.SetQiPending (syntaxTask, "Resources", qiContent, point);
								}

								var semanticTask = this.GetQiSemanticDataAsync (node, cts.Token);
								if (semanticTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
								{
									QuickInfoSource.AddQiContent (qiContent, semanticTask.Result);
									applicableToSpan = QuickInfoSource.GetTrackingSpan (point, token);
									Trace.WriteLine ("DONE: " + applicableToSpan.ToString ());
								}
								else
								{
									cts.Cancel ();
									applicableToSpan = QuickInfoSource.SetQiPending (semanticTask, "Semantic", qiContent, point);
								}
							}
						}
					}
					else
					{
						cts.Cancel ();
						applicableToSpan = QuickInfoSource.SetQiPending (syntaxTask, "Syntax", qiContent, point);
					}
				}
				else
				{
					cts.Cancel ();
					applicableToSpan = QuickInfoSource.SetQiPending (this.documentTask, "Document", qiContent, point);
				}
			}
		}

		
		private static ITrackingSpan SetQiPending(Task task, string subject, IList<object> qiContent, SnapshotPoint point)
		{
			task.ForgetSafely ();
			QuickInfoSource.AddQiContent (qiContent, string.Format("({0} cache is still being constructed. Please try again in a few seconds...)", subject));
			var applicableToSpan = QuickInfoSource.GetTrackingSpan (point);
			Trace.WriteLine ("CANCELING: " + applicableToSpan.ToString ());
			return applicableToSpan;
		}

		private static SyntaxNode FilterSyntax(CommonSyntaxToken token)
		{
			return QuickInfoSource.FilterFieldOrPropertyAccessSyntax (token);
		}

		private static SyntaxNode FilterAnySyntax(CommonSyntaxToken token)
		{
			if (token == default(CommonSyntaxToken))
			{
				return null;
			}
			return token.Parent as SyntaxNode;
		}

		private static SyntaxNode FilterFieldOrPropertyAccessSyntax(CommonSyntaxToken token)
		{
			if (token != default (CommonSyntaxToken))
			{
				var identifier = token.Parent as IdentifierNameSyntax;
				if (identifier != null && !identifier.IsInvocation ())
				{
					var memberAccess = identifier.Parent as MemberAccessExpressionSyntax;
					if (memberAccess != null && !memberAccess.IsInvocation ())
					{
						if (memberAccess.Span.End > token.Span.End)
						{
							return identifier;
						}
						return memberAccess;
					}
				}

			}
			return null;
		}

		private static int Timeout(int milliseconds)
		{
			//return Debugger.IsAttached ? -1 : milliseconds;
			return milliseconds;
		}

		private static ISemanticModel GetSemanticModel(IDocument document, CancellationToken cancellationToken)
		{
			using (new TimeTrace ("GetSemanticModel"))
			{
				return document.GetSemanticModel (cancellationToken);
			}
		}

		private static CommonSyntaxNode GetSyntaxRoot(IDocument document, CancellationToken cancellationToken)
		{
			using (new TimeTrace ("GetSyntaxRoot"))
			{
				return document.GetSyntaxRoot (cancellationToken);
			}
		}

		private static IDocument GetActiveDocument(ISolution solution, CancellationToken cancellationToken)
		{
			using (new TimeTrace ("GetActiveDocument"))
			{
				return solution.ActiveDocument (cancellationToken);
			}
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
			var span = new Span (snapshotPoint.Position, 1);
			return currentSnapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
		}

		private static ITrackingSpan GetTrackingSpan(SnapshotPoint snapshotPoint, CommonSyntaxToken token)
		{
			ITextSnapshot currentSnapshot = snapshotPoint.Snapshot;
			var span = Span.FromBounds (token.Span.Start, token.Span.End);
			return currentSnapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
		}


		private void Initialize(ISolution solution, CancellationToken cancellationToken = default(CancellationToken))
		{
			this.Initialize(Task.Run (() => QuickInfoSource.GetActiveDocument (solution, cancellationToken), cancellationToken));
		}

		private void Initialize(Task<IDocument> documentTask, CancellationToken cancellationToken = default(CancellationToken))
		{
			this.documentTask = documentTask;
			this.syntaxRootTask = this.documentTask.ContinueWith (t => QuickInfoSource.GetSyntaxRoot (t.Result, cancellationToken), cancellationToken);
			this.semanticModelTask = this.documentTask.ContinueWith (t => QuickInfoSource.GetSemanticModel (t.Result, cancellationToken), cancellationToken);
		}

		private IEnumerable<string> GetQiPathsContent()
		{
			var document = this.documentTask.Result;
			var project = document.Project;
			yield return string.Format ("DOC : {0}", document.FilePath);
			yield return string.Format ("PRJ : {0}", document.Project.FilePath);
			yield return string.Format ("SLN : {0}", document.Project.Solution.FilePath);
		}

		private async Task<Tuple<CommonSyntaxToken, SyntaxNode, IEnumerable<string>>> GetQiSyntaxDataAsync(int position, CancellationToken cancellationToken = default(CancellationToken))
		{
			var token = await this.GetSyntaxTokenAsync (position, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested ();
			var node = QuickInfoSource.FilterSyntax (token);
			if (node != null)
			{
				cancellationToken.ThrowIfCancellationRequested ();
				return Tuple.Create (token, node, this.GetQiSyntaxContent (node));
			}
			return Tuple.Create (token, default(SyntaxNode), Enumerable.Empty<string>());
		}

		private async Task<IEnumerable<string>> GetQiSemanticDataAsync(SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested ();
			var semanticModel = await this.semanticModelTask.ConfigureAwait(false);

			cancellationToken.ThrowIfCancellationRequested ();
			return this.GetQiSemanticContent (node, semanticModel);
		}

		private async Task<CommonSyntaxToken> GetSyntaxTokenAsync(int position, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested ();
			var syntaxRoot = await this.syntaxRootTask.ConfigureAwait (false);

			cancellationToken.ThrowIfCancellationRequested ();
			return syntaxRoot.FindToken (position);
		}

		private IEnumerable<string> GetQiSyntaxContent(SyntaxNode node)
		{
			var displayNode = node.RemoveTrivias ();
			yield return string.Format ("SYNTAX : {0}", displayNode.ToString ());
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

		private void HandleSubjectBufferChanged(object sender, TextContentChangedEventArgs e)
		{
			this.ApplyTextChangesAsync (e.Changes.ToRoslynTextChanges ()).ForgetSafely ();
		}

		private async Task ApplyTextChangesAsync(IEnumerable<TextChange> changes, CancellationToken cancellationToken = default(CancellationToken))
		{
			var document = await this.documentTask;
			this.cts.Cancel ();

			var text = document.GetText ().WithChanges (changes);
			var solution = document.Project.Solution.UpdateDocument (document.Id, text);
			document = solution.GetProject (document.Project.Id).GetDocument (document.Id);

			this.cts = new CancellationTokenSource ();
			this.Initialize(Task.FromResult (document), this.cts.Token);
		}

		
		private readonly QuickInfoSourceProvider provider;
		private readonly ITextBuffer subjectBuffer;
		private bool isDisposed;
		private CancellationTokenSource cts;
		private Task<IDocument> documentTask;
		private Task<CommonSyntaxNode> syntaxRootTask;
		private Task<ISemanticModel> semanticModelTask;
		private Task<SolutionResource> solutionResourceTask;
	}
}
