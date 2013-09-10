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
//		/rootsuffix Exp "..\..\..\..\App.CresusGraph\App.CresusGraph.sln"
//		/rootsuffix Exp "..\..\..\..\Epsitec.Cresus.sln"
//		/rootsuffix Exp "..\..\..\Vsx.CresusStrings.Sample1\Vsx.CresusStrings.Sample1.sln"
namespace Epsitec.Cresus.Strings
{
	internal class QuickInfoSource : IQuickInfoSource
	{
		public QuickInfoSource(QuickInfoSourceProvider provider, ITextBuffer subjectBuffer)
		{
			this.provider = provider;
			this.subjectBuffer = subjectBuffer;

			this.subjectBuffer.Changed += this.HandleSubjectBufferChanged;
			this.cts = new CancellationTokenSource ();

			this.documentIdTask = QuickInfoSource.GetActiveDocumentIdAsync (this.Solution, this.cts.Token);
			this.resourceSymbolMapperTask = QuickInfoSource.LoadResourcesAsync (this.Solution, this.cts.Token);

			this.Initialize (this.cts.Token);
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				Trace.WriteLine ("### DISPOSE");
				this.isDisposed = true;
				this.subjectBuffer.Changed -= this.HandleSubjectBufferChanged;
				this.cts.Cancel ();
				this.documentIdTask.ForgetSafely ();
				this.syntaxRootTask.ForgetSafely ();
				this.semanticModelTask.ForgetSafely ();
				this.resourceSymbolMapperTask.ForgetSafely ();
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
					var cts = new CancellationTokenSource ();

					if (this.documentIdTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
					{
						//QuickInfoSource.AddQiContent (qiContent, this.GetQiPathsContent());

						var tokenTask = this.GetSyntaxTokenAsync (point, cts.Token);
						if (tokenTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
						{
							var token = tokenTask.Result;
							var node = QuickInfoSource.LookAheadSyntaxNode (token, point, out applicableToSpan);
							if (node != null)
							{
								//QuickInfoSource.AddQiContent (qiContent, syntaxData.Item3);

								// get project resources
								if (this.resourceSymbolMapperTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
								{
									var symbol = node.RemoveTrivias ().ToString ();
									var resources = this.ResourceSymbolMapper.FindPartial (symbol);
									var content = new MultiCultureResourceItemCollectionView (resources)
									{
										MaxHeight = 600
									};
									qiContent.Add (content);

									//var context = SynchronizationContext.Current;
									//var element = content as System.Windows.FrameworkElement;
									//context.Post (e =>
									//	{
									//		var parent = element.Parent;
									//	}, element);

									//var message = string.Join("\n", QuickInfoSource.GetQiResourceView (memberAccessMap));
									//QuickInfoSource.AddQiContent (qiContent, message);
								}
								else
								{
									cts.Cancel ();
									QuickInfoSource.SetQiPending (tokenTask, "Resources", qiContent);
								}

								//var semanticTask = this.GetQiSemanticDataAsync (node, cts.Token);
								//if (semanticTask.Wait (QuickInfoSource.Timeout (100), this.cts.Token))
								//{
								//	//QuickInfoSource.AddQiContent (qiContent, semanticTask.Result);
								//	applicableToSpan = QuickInfoSource.GetTrackingSpan (point, node.Span);
								//	Trace.WriteLine ("DONE: " + applicableToSpan.ToString ());
								//}
								//else
								//{
								//	cts.Cancel ();
								//	QuickInfoSource.SetQiPending (semanticTask, "Semantic", qiContent);
								//}

							}
						}
						else
						{
							cts.Cancel ();
							QuickInfoSource.SetQiPending (tokenTask, "Syntax", qiContent);
						}
					}
					else
					{
						cts.Cancel ();
						QuickInfoSource.SetQiPending (this.documentIdTask, "Document", qiContent);
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

		private static ResourceSymbolMapper LoadResources(ISolution solution, CancellationToken cancellationToken)
		{
			var solutionResource = new SolutionResource (solution, cancellationToken);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitSolution (solutionResource);
			return mapper;
		}

		private static Task<ResourceSymbolMapper> LoadResourcesAsync(ISolution solution, CancellationToken cancellationToken)
		{
			return Task.Run (() => QuickInfoSource.LoadResources (solution, cancellationToken), cancellationToken);
		}

		private static void SetQiPending(Task task, string subject, IList<object> qiContent)
		{
			task.ForgetSafely ();
			QuickInfoSource.AddQiContent (qiContent, string.Format("({0} cache is still being constructed. Please try again in a few seconds...)", subject));
			Trace.WriteLine ("CANCELING");
		}

		private static void AddQiException(IList<object> qiContent, Exception e, string prefix = "Cresus Strings Extension Exception\n")
		{
			string message = string.Format ("{0}{1} : {2}", prefix, e.GetType ().Name, e.Message);
			QuickInfoSource.AddQiContent (qiContent, message);
		}

		//private static SyntaxNode FilterSyntaxToken(CommonSyntaxToken token)
		//{
		//	return QuickInfoSource.FilterFieldOrPropertyAccessSyntax (token);
		//}

		private static SyntaxNode FilterAnySyntax(CommonSyntaxToken token)
		{
			if (token == default(CommonSyntaxToken))
			{
				return null;
			}
			return token.Parent as SyntaxNode;
		}

		//private static SyntaxNode FilterFieldOrPropertyAccessSyntax(CommonSyntaxToken token)
		//{
		//	if (token != default (CommonSyntaxToken))
		//	{
		//		var identifier = token.Parent as IdentifierNameSyntax;
		//		if (identifier != null && !identifier.IsInvocation ())
		//		{
		//			var memberAccess = identifier.Parent as MemberAccessExpressionSyntax;
		//			if (memberAccess != null && !memberAccess.IsInvocation ())
		//			{
		//				if (memberAccess.Span.End > token.Span.End)
		//				{
		//					return identifier;
		//				}
		//				return memberAccess;
		//			}
		//		}

		//	}
		//	return null;
		//}

		private static SyntaxNode LookAroundSyntaxNode(CommonSyntaxToken token, SnapshotPoint point, out ITrackingSpan applicableToSpan)
		{
			applicableToSpan = QuickInfoSource.GetTrackingSpan (point);
			if (token != default (CommonSyntaxToken))
			{
				if (token.Parent.IsMemberAccess ())
				{
					var ancestors = token.Parent.AncestorsAndSelf ();
					var properties = ancestors
						.TakeWhile (a => a.IsPropertyOrField ());
					var node = properties.LastOrDefault ();
					if (node != null)
					{
						applicableToSpan = QuickInfoSource.GetTrackingSpan (point, node.Span);
					}
					return node as SyntaxNode;
				}
			}
			return null;
		}

		private static SyntaxNode LookAheadSyntaxNode(CommonSyntaxToken token, SnapshotPoint point, out ITrackingSpan applicableToSpan)
		{
			applicableToSpan = QuickInfoSource.GetTrackingSpan (point);
			if (token != default (CommonSyntaxToken))
			{
				if (token.Parent.IsMemberAccess ())
				{
					var ancestors = token.Parent.AncestorsAndSelf ();
					var properties = ancestors
						.TakeWhile (a => a.Span.End <= token.Span.End && a.IsPropertyOrField ());
					var node = properties.LastOrDefault ();
					if (node != null)
					{
						applicableToSpan = QuickInfoSource.GetTrackingSpan (point, token.Span);
					}
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

		private static DocumentId GetActiveDocumentId(ISolution solution, CancellationToken cancellationToken)
		{
			using (new TimeTrace ("GetActiveDocument"))
			{
				return solution.ActiveDocument (cancellationToken).Id;
			}
		}

		private static Task<DocumentId> GetActiveDocumentIdAsync(ISolution solution, CancellationToken cancellationToken)
		{
			return Task.Run (() => QuickInfoSource.GetActiveDocumentId (solution, cancellationToken), cancellationToken);
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

		//private static ITrackingSpan GetTrackingSpan(SnapshotPoint snapshotPoint, CommonSyntaxToken token)
		//{
		//	ITextSnapshot currentSnapshot = snapshotPoint.Snapshot;
		//	var span = Span.FromBounds (token.Span.Start, token.Span.End);
		//	return currentSnapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
		//}

		//private static ITrackingSpan GetTrackingSpan(SnapshotPoint snapshotPoint, SyntaxNode node)
		//{
		//	ITextSnapshot currentSnapshot = snapshotPoint.Snapshot;
		//	var span = Span.FromBounds (node.Span.Start, node.Span.End);
		//	return currentSnapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
		//}

		private static ITrackingSpan GetTrackingSpan(SnapshotPoint snapshotPoint, TextSpan textSpan)
		{
			ITextSnapshot currentSnapshot = snapshotPoint.Snapshot;
			var span = Span.FromBounds (textSpan.Start, textSpan.End);
			return currentSnapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
		}

		private ISolution Solution
		{
			get
			{
				return this.provider.Solution;
			}
			set
			{
				this.provider.Solution = value;
			}
		}

		private IProject Project
		{
			get
			{
				return this.Document.Project;
			}
		}

		private IDocument Document
		{
			get
			{
				return this.Solution.GetDocument (this.documentIdTask.Result);
			}
		}

		private ResourceSymbolMapper ResourceSymbolMapper
		{
			get
			{
				return this.resourceSymbolMapperTask.Result;
			}
		}

		private void Initialize(CancellationToken cancellationToken)
		{
			this.syntaxRootTask = this.documentIdTask.ContinueWith (t => QuickInfoSource.GetSyntaxRoot (this.Document, cancellationToken), cancellationToken);
			this.semanticModelTask = this.documentIdTask.ContinueWith (t => QuickInfoSource.GetSemanticModel (this.Document, cancellationToken), cancellationToken);
		}

		private IEnumerable<string> GetQiPathsContent()
		{
			yield return string.Format ("DOC : {0}", this.Document.FilePath);
			yield return string.Format ("PRJ : {0}", this.Project.FilePath);
			yield return string.Format ("SLN : {0}", this.Solution.FilePath);
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
			var documentId = await this.documentIdTask;
			this.cts.Cancel ();
			this.cts = new CancellationTokenSource ();

			var text = this.Document.GetText ().WithChanges (changes);
			this.Solution = this.Solution.UpdateDocument (documentId, text);

			this.Initialize (this.cts.Token);
		}
		
		private readonly QuickInfoSourceProvider provider;
		private readonly ITextBuffer subjectBuffer;

		private bool isDisposed;
		private CancellationTokenSource cts;

		private Task<DocumentId> documentIdTask;
		private Task<ResourceSymbolMapper> resourceSymbolMapperTask;

		private Task<CommonSyntaxNode> syntaxRootTask;
		private Task<ISemanticModel> semanticModelTask;
	}
}
