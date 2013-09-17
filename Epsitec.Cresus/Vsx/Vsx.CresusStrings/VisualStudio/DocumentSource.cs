using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.Views;
using Epsitec.VisualStudio;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	public class DocumentSource : IDisposable
	{
		public DocumentSource(ISolutionProvider solutionProvider, EnvDTE.Document dteDocument)
		{
			this.solutionProvider = solutionProvider;
			this.dteDocument = dteDocument;
			this.StartDocumentId (dteDocument);
			this.StartSyntaxAndSemantic ();
		}


		public string							Id
		{
			get
			{
				return this.dteDocument.FullName.ToLower ();
			}
		}

		public ITextBuffer						TextBuffer
		{
			get
			{
				return this.textBuffer;
			}
			internal set
			{
				if (this.textBuffer != value)
				{
					if (this.textBuffer != null)
					{
						this.textBuffer.Changed -= this.OnTextBufferChanged;
					}
					this.textBuffer = value;
					if (this.textBuffer != null)
					{
						this.textBuffer.Changed += this.OnTextBufferChanged;
					}
				}
			}
		}

		public async Task<IDocument>			DocumentAsync()
		{
			return this.Solution.GetDocument (await this.DocumentIdAsync());
		}

		public async Task<CommonSyntaxNode>		SyntaxRootAsync()
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			return await this.syntaxRootTask.ConfigureAwait(false);
		}

		public async Task<ISemanticModel>		SemanticModelAsync()
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			return await this.semanticModelTask.ConfigureAwait (false);
		}


		public async Task<ResourceSymbolInfo> GetResourceSymbolInfoAsync(SnapshotPoint point, ResourceSymbolMapperSource resourceProvider, bool leftExtent)
		{
			//var applicableToSpan = point.GetNullTrackingSpan ();
			var syntaxRoot = await this.SyntaxRootAsync ().ConfigureAwait (false);
			var token = syntaxRoot.FindToken (point);
			var node = leftExtent
				? DocumentSource.GetLeftSyntaxNode (token, point)
				: DocumentSource.GetFullSyntaxNode (token, point);
			if (node != null)
			{
				var symbolName = node.RemoveTrivias ().ToString ();
				symbolName = Regex.Replace (symbolName, @"^global::", string.Empty);
				var resourceSymbolMapper = await resourceProvider.SymbolMapperAsync ();
				var resources = resourceSymbolMapper.FindPartial (symbolName).ToList ();
				if (resources.Count > 0)
				{
					return new ResourceSymbolInfo (resources, symbolName, node, token);
				}
			}
			return null;
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.textBuffer != null)
			{
				this.textBuffer.Changed -= this.OnTextBufferChanged;
			}

			this.CancelDocumentId ();
			this.CancelSyntaxAndSemantic ();
			this.ctsDocumentId.Dispose ();
			this.ctsSyntaxAndSemantic.Dispose ();
		}

		#endregion


		private static SyntaxNode GetFullSyntaxNode(CommonSyntaxToken token, SnapshotPoint point)
		{
			if (token != default (CommonSyntaxToken))
			{
				var phase1 = token.Parent.AncestorsAndSelf ().SkipWhile (n => n is MemberAccessExpressionSyntax || n is IdentifierNameSyntax || n is AliasQualifiedNameSyntax);
				var node1 = phase1.FirstOrDefault ();
				if (node1 != null)
				{
					var phase2 = node1.DescendantNodesAndSelf ().SkipWhile (n => !(n is MemberAccessExpressionSyntax));
					var node2 = phase2.FirstOrDefault ();
					if (node2 != null)
					{
						if (node2.Parent is InvocationExpressionSyntax)
						{
							node2 = phase2.Skip (1).FirstOrDefault ();
						}
						if (node2.Span.OverlapsWith (token.Span) || node2.Span.ContiguousWith(token.Span))
						{
							return node2 as SyntaxNode;
						}
					}
				}
			}
			return null;
		}

		private static SyntaxNode GetFullSyntaxNode0(CommonSyntaxToken token, SnapshotPoint point)
		{
			SyntaxNode node = null;
			if (token != default (CommonSyntaxToken))
			{
				var startNode = token.Parent;
				if (startNode.IsMemberAccess ())
				{
					var ancestors = startNode.AncestorsAndSelf ();
					node = GetFullSyntaxNodeUpTheTree (ancestors);
					if (node == null)
					{
						node = DocumentSource.GetFullSyntaxNodeDownTheTree (startNode.DescendantNodes ());
					}
				}
				else
				{
					node = DocumentSource.GetFullSyntaxNodeDownTheTree (startNode.DescendantNodesAndSelf ());
				}
			}
			return node;
		}

		private static SyntaxNode GetFullSyntaxNodeUpTheTree(IEnumerable<CommonSyntaxNode> ancestors)
		{
			var properties = ancestors.TakeWhile (a => a.IsPropertyOrField ());
			return properties.LastOrDefault () as SyntaxNode;
		}

		private static SyntaxNode GetFullSyntaxNodeDownTheTree(IEnumerable<CommonSyntaxNode> descendants)
		{
			var properties = descendants.SkipWhile (a => a is InvocationExpressionSyntax || a.IsPropertyOrField ());
			var node = properties.FirstOrDefault ();
			if (node.IsInvocation ())
			{
				return null;
			}
			return node as SyntaxNode;
		}

		private static SyntaxNode GetLeftSyntaxNode(CommonSyntaxToken token, SnapshotPoint point)
		{
			if (token != default (CommonSyntaxToken))
			{
				var startNode = token.Parent;
				if (startNode.IsMemberAccess ())
				{
					var ancestors = startNode.AncestorsAndSelf ();
					var properties = ancestors.TakeWhile (a => a.Span.End <= token.Span.End && a.IsPropertyOrField ());
					var node = properties.LastOrDefault ();
					return node as SyntaxNode;
				}
				else
				{
					var descendants = startNode.DescendantNodesAndSelf ();
					var properties = descendants.SkipWhile (a => !(a.Span.End <= token.Span.End && a.IsPropertyOrField ()));
					var node = properties.FirstOrDefault ();
					return node as SyntaxNode;
				}
			}
			return null;
		}


		private ISolution						Solution
		{
			get
			{
				return this.solutionProvider.Solution;
			}
		}

		public async Task<DocumentId>			DocumentIdAsync()
		{
			this.ctsDocumentId.Token.ThrowIfCancellationRequested ();
			return await this.documentIdTask.ConfigureAwait (false);
		}


		private Task<DocumentId> CreateDocumentIdTask(EnvDTE.Document dteDocument)
		{
			var cancellationToken = this.ctsDocumentId.Token;
			return Task.Run (() =>
			{
				using (new TimeTrace ("DocumentSource.CreateDocumentIdTask"))
				{
					cancellationToken.ThrowIfCancellationRequested ();
					var dteActiveDocumentPath = dteDocument.FullName;
					for (int retryCount = 0; retryCount < 3; ++retryCount)
					{
						try
						{
							var document = this.Solution.Documents (cancellationToken)
								.Where (d => string.Compare (d.FilePath, dteActiveDocumentPath, true) == 0)
								.Do (_ => cancellationToken.ThrowIfCancellationRequested ())
								.Single ();

							return document.Id;
						}
						catch (InvalidOperationException)
						{
						}
					}
					return null;
				}
			}, cancellationToken);
		}

		private Task<CommonSyntaxNode> CreateSyntaxRootTask()
		{
			return Task.Run (() =>
			{
				using (new TimeTrace ("DocumentSource.CreateSyntaxRootTask"))
				{
					var documentId = this.documentIdTask.Result;
					var document = this.Solution.GetDocument (documentId);
					return document.GetSyntaxRoot (this.ctsSyntaxAndSemantic.Token);
				}
			}, this.ctsSyntaxAndSemantic.Token);
		}

		private Task<ISemanticModel> CreateSemanticModelTask()
		{
			return Task.Run (() =>
			{
				using (new TimeTrace ("DocumentSource.CreateSemanticModelTask"))
				{
					var documentId = this.documentIdTask.Result;
					var document = this.Solution.GetDocument (documentId);
					return document.GetSemanticModel (this.ctsSyntaxAndSemantic.Token);
				}
			}, this.ctsSyntaxAndSemantic.Token);
		}


		private void StartDocumentId(EnvDTE.Document dteDocument)
		{
			this.ctsDocumentId = new CancellationTokenSource ();
			this.documentIdTask = this.CreateDocumentIdTask (dteDocument);
		}

		private void RestartDocumentId(EnvDTE.Document dteActiveDocument)
		{
			this.CancelDocumentId ();
			this.StartDocumentId (dteActiveDocument);
		}
		
		private void CancelDocumentId()
		{
			this.ctsDocumentId.Cancel ();
			this.documentIdTask.ForgetSafely ();
			this.ctsDocumentId.Dispose ();
		}


		private void StartSyntaxAndSemantic()
		{
			this.ctsSyntaxAndSemantic = new CancellationTokenSource ();
			this.syntaxRootTask = this.CreateSyntaxRootTask ();
			this.semanticModelTask = this.CreateSemanticModelTask ();
		}

		private void RestartSyntaxAndSemantic()
		{
			this.CancelSyntaxAndSemantic ();
			this.StartSyntaxAndSemantic ();
		}

		private void CancelSyntaxAndSemantic()
		{
			this.ctsSyntaxAndSemantic.Cancel ();
			this.syntaxRootTask.ForgetSafely ();
			this.semanticModelTask.ForgetSafely ();
			this.ctsSyntaxAndSemantic.Dispose ();
		}


		private async Task<ISolution> UpdateActiveDocumentAsync(IEnumerable<Roslyn.Compilers.TextChange> changes)
		{
			var document = await this.DocumentAsync ();
			var text = document.GetText ().WithChanges (changes);
			var solution = this.solutionProvider.UpdateSolution (document.Id, text);
			this.RestartSyntaxAndSemantic ();
			return solution;
		}

		private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
		{
			var task = this.UpdateActiveDocumentAsync (e.Changes.ToRoslynTextChanges ());
		}


		private readonly ISolutionProvider solutionProvider;
		private readonly EnvDTE.Document dteDocument;

		private CancellationTokenSource ctsDocumentId;
		private Task<DocumentId> documentIdTask;

		private CancellationTokenSource ctsSyntaxAndSemantic;
		private Task<CommonSyntaxNode> syntaxRootTask;
		private Task<ISemanticModel> semanticModelTask;

		private ITextBuffer textBuffer;
	}
}
