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
		public DocumentSource(ISolutionProvider solutionProvider, EnvDTE.Document dteDocument, ITextBuffer textBuffer)
		{
			this.solutionProvider = solutionProvider;
			this.dteDocument = dteDocument;
			this.textBuffer = textBuffer;
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

		public async Task<CommonSyntaxNode>		SyntaxRootAsync(CancellationToken cancellationToken)
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			cancellationToken.ThrowIfCancellationRequested ();
			return await this.syntaxRootTask.ConfigureAwait(false);
		}

		public async Task<ISemanticModel>		SemanticModelAsync(CancellationToken cancellationToken)
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			cancellationToken.ThrowIfCancellationRequested ();
			return await this.semanticModelTask.ConfigureAwait (false);
		}


		public async Task<ResourceSymbolInfo> GetResourceSymbolInfoAsync(SnapshotPoint point, ResourceSymbolMapperSource resourceProvider, CancellationToken cancellationToken)
		{
			if (this.textBuffer == null)
			{
				throw new InvalidOperationException ("TextBuffer not initialized");
			}
			var syntaxRoot = await this.SyntaxRootAsync (cancellationToken).ConfigureAwait (false);
			var token = syntaxRoot.FindToken (point);
			var node = DocumentSource.FindSymbolSyntaxNode (token, point);
			if (node != null)
			{
				var symbolName = node.RemoveTrivias ().ToString ();
				symbolName = Regex.Replace (symbolName, @"^global::", string.Empty);
				var resourceSymbolMapper = await resourceProvider.SymbolMapperAsync (cancellationToken);
				var resources = resourceSymbolMapper.FindPartial (symbolName, cancellationToken).ToList ();
				if (resources.Count > 0)
				{
					return new ResourceSymbolInfo (this.TextBuffer, token, node, symbolName, resources);
				}
			}
			return null;
		}

		/// <summary>
		/// Try to associate a text buffer with document (<see cref="DocumentSourceManager.ActiveDocument"/> and <see cref="DocumentSourceManager.ActiveTextBuffer"/> for more information
		/// </summary>
		/// <param name="pendingTextBuffer"></param>
		/// <returns>true if this document source takes ownership of the text buffer</returns>
		internal bool TrySetPendingTextBuffer(ITextBuffer pendingTextBuffer)
		{
			pendingTextBuffer.ThrowIfNull();

			if (this.textBuffer == pendingTextBuffer)
			{
				// already owned
				return true;
			}
			else if (this.textBuffer == null)
			{
				// still not initialized : takes ownership
				this.textBuffer = pendingTextBuffer;
				if (this.textBuffer != null)
				{
					this.textBuffer.Changed += this.OnTextBufferChanged;
				}
				return true;
			}
			else
			{
				// already initialized : declines ownership
				return false;
			}
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


		private static SyntaxNode FindSymbolSyntaxNode(CommonSyntaxToken token, SnapshotPoint point)
		{
			if (token != default (CommonSyntaxToken))
			{
				var node = token.Parent.AncestorsAndSelf ().SkipWhile (n => n is MemberAccessExpressionSyntax || n is IdentifierNameSyntax).FirstOrDefault ();
				if (node != null)
				{
					//var descendants = node.DescendantNodesAndSelf ().SkipWhile (n => !(n is MemberAccessExpressionSyntax));
					var x1 = DocumentSource.DescendantNodesAndSelf (node, token.Span).ToList ();
					var descendants = x1.SkipWhile (n => !(n is MemberAccessExpressionSyntax));
					node = descendants.FirstOrDefault ();
					if (node != null)
					{
						if (node.Parent is InvocationExpressionSyntax)
						{
							node = descendants.Skip (1).FirstOrDefault ();
						}
						return node as SyntaxNode;
					}
				}
			}
			return null;
		}

		private static IEnumerable<CommonSyntaxNode> DescendantNodesAndSelf(CommonSyntaxNode node, TextSpan span)
		{
			if (node != null)
			{
				yield return node;
				foreach (var n in DocumentSource.DescendantNodesAndSelf (DocumentSource.ChildNode (node, span), span))
				{
					yield return n;
				}
			}
		}

		private static CommonSyntaxNode ChildNode(CommonSyntaxNode node, TextSpan span)
		{
			return node.ChildNodes ().Where (n => n.Span.OverlapsWith (span) || n.Span.ContiguousWith (span)).FirstOrDefault();
		}

		private ISolution Solution
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
				using (new TimeTrace ())
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
				using (new TimeTrace ())
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
				using (new TimeTrace ())
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
			this.UpdateActiveDocumentAsync (e.Changes.ToRoslynTextChanges ()).ConfigureAwait(false);
		}

		//private IEnumerable<string> GetQiPathsContent()
		//{
		//	yield return string.Format ("DOC : {0}", this.DocumentAsync ().Result.FilePath);
		//	yield return string.Format ("PRJ : {0}", this.DocumentAsync ().Result.Project.FilePath);
		//	yield return string.Format ("SLN : {0}", this.Solution.FilePath);
		//}

		//private IEnumerable<string> GetQiSemanticContent(SyntaxNode node, ISemanticModel semanticModel)
		//{
		//	ISymbol symbol = null;

		//	var typeSymbol = semanticModel.GetTypeInfo (node).Type;
		//	if (typeSymbol != null && !typeof (ErrorTypeSymbol).IsAssignableFrom (typeSymbol.GetType ()))
		//	{
		//		yield return string.Format ("TYPE : {0}", typeSymbol.ToString ());
		//		symbol = typeSymbol;
		//	}
		//	var symbolSymbol = semanticModel.GetSymbolInfo (node).Symbol;
		//	if (symbolSymbol != null)
		//	{
		//		yield return string.Format ("SYM : {0}", symbolSymbol.ToString ());
		//		symbol = symbolSymbol;
		//	}

		//	var declaredSymbol = semanticModel.GetDeclaredSymbol (node);
		//	if (declaredSymbol != null)
		//	{
		//		yield return string.Format ("DECL : {0}", declaredSymbol.ToString ());
		//		symbol = declaredSymbol;
		//	}

		//	if (symbol != null)
		//	{
		//		var location = symbol.Locations.FirstOrDefault ();
		//		if (location != null && location.SourceTree != null)
		//		{
		//			var path = location.SourceTree.FilePath;
		//			yield return string.Format ("PATH : {0}", path);
		//		}
		//	}
		//}

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
