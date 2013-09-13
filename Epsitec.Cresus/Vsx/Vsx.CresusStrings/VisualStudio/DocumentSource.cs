using System;
using System.Collections.Generic;
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

		public string Id
		{
			get
			{
				return this.dteDocument.FullName.ToLower ();
			}
		}

		public ITextBuffer TextBuffer
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
						this.textBuffer.Changed -= this.HandleTextBufferChanged;
					}
					this.textBuffer = value;
					if (this.textBuffer != null)
					{
						this.textBuffer.Changed += this.HandleTextBufferChanged;
					}
				}
			}
		}

		public async Task<IDocument> DocumentAsync()
		{
			return this.Solution.GetDocument (await this.DocumentIdAsync());
		}

		public async Task<CommonSyntaxNode> SyntaxRootAsync()
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			return await this.syntaxRootTask.ConfigureAwait(false);
		}

		public async Task<ISemanticModel> SemanticModelAsync()
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			return await this.semanticModelTask.ConfigureAwait (false);
		}

		public async Task<ResourceSymbolInfo> GetResourceSymbolInfoAsync(SnapshotPoint point, ResourceSymbolMapperSource resourceProvider)
		{
			var applicableToSpan = point.GetNullTrackingSpan ();
			var syntaxRoot = await this.SyntaxRootAsync ().ConfigureAwait (false);
			var token = syntaxRoot.FindToken (point);
			var node = DocumentSource.GetLeftSyntaxNode (token, point);
			if (node != null)
			{
				applicableToSpan = point.GetTextTrackingSpan (token.Span);
				var symbolName = node.RemoveTrivias ().ToString ();
				symbolName = Regex.Replace (symbolName, @"^global::", string.Empty);
				var resourceSymbolMapper = await resourceProvider.SymbolMapperAsync ();
				var resources = resourceSymbolMapper.FindPartial (symbolName).ToList ();
				return new ResourceSymbolInfo (resources, symbolName, node, token, applicableToSpan);
			}
			return null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.textBuffer != null)
			{
				this.textBuffer.Changed -= this.HandleTextBufferChanged;
			}

			this.CancelDocumentId ();
			this.CancelSyntaxAndSemantic ();
			this.ctsDocumentId.Dispose ();
			this.ctsSyntaxAndSemantic.Dispose ();
		}

		#endregion

		private Task<DocumentId> CreateDocumentIdTask(EnvDTE.Document dteDocument)
		{
			var cancellationToken = this.ctsDocumentId.Token;
			return Task.Run (() =>
			{
				using (new TimeTrace ("CreateDocumentIdTask"))
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

		//private static Task<CommonSyntaxNode> CreateSyntaxRootTask(Task<IDocument> documentTask, CancellationToken cancellationToken)
		//{
		//	return documentTask.ContinueWith (t =>
		//	{
		//		using (new TimeTrace ("CreateSyntaxRootTask"))
		//		{
		//			return t.Result.GetSyntaxRoot (cancellationToken);
		//		}
		//	},
		//		cancellationToken,
		//		TaskContinuationOptions.None,
		//		TaskScheduler.Default);
		//}

		//private static Task<ISemanticModel> CreateSemanticModelTask(Task<IDocument> documentTask, CancellationToken cancellationToken)
		//{
		//	return documentTask.ContinueWith (t =>
		//	{
		//		using (new TimeTrace ("CreateSemanticModelTask"))
		//		{
		//			return t.Result.GetSemanticModel (cancellationToken);
		//		}
		//	},
		//		cancellationToken,
		//		TaskContinuationOptions.None,
		//		TaskScheduler.Default);
		//}

		private Task<CommonSyntaxNode> CreateSyntaxRootTask()
		{
			return Task.Run (() =>
			{
				using (new TimeTrace ("CreateSyntaxRootTask"))
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
				using (new TimeTrace ("CreateSemanticModelTask"))
				{
					var documentId = this.documentIdTask.Result;
					var document = this.Solution.GetDocument (documentId);
					return document.GetSemanticModel (this.ctsSyntaxAndSemantic.Token);
				}
			}, this.ctsSyntaxAndSemantic.Token);
		}


		private ISolution Solution
		{
			get
			{
				return this.solutionProvider.Solution;
			}
		}

		public async Task<DocumentId> DocumentIdAsync()
		{
			this.ctsDocumentId.Token.ThrowIfCancellationRequested ();
			return await this.documentIdTask.ConfigureAwait (false);
		}


		private void StartDocumentId(EnvDTE.Document dteDocument)
		{
			this.ctsDocumentId.Dispose ();
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
		}

		private void StartSyntaxAndSemantic()
		{
			this.ctsSyntaxAndSemantic.Dispose ();
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
		}


		private void HandleTextBufferChanged(object sender, TextContentChangedEventArgs e)
		{
			var task = this.UpdateActiveDocumentAsync (e.Changes.ToRoslynTextChanges ());
		}

		private async Task<ISolution> UpdateActiveDocumentAsync(IEnumerable<Roslyn.Compilers.TextChange> changes)
		{
			var document = await this.DocumentAsync ();
			var text = document.GetText ().WithChanges (changes);
			var solution = this.solutionProvider.UpdateSolution (document.Id, text);
			this.RestartSyntaxAndSemantic ();
			return solution;
		}

		//public async Task<object> GetQuickInfoContent(ResourceController resourceProvider, int timeout, SnapshotPoint point, out ITrackingSpan applicableToSpan)
		//{
		//	applicableToSpan = point.GetNullTrackingSpan ();
		//	// get project resources
		//	try
		//	{
		//		var resourceSymbolMapper = await resourceProvider.SymbolMapperAsync ().WithTimeout (timeout);
		//		try
		//		{
		//			var document = await this.DocumentAsync ().WithTimeout (timeout);
		//			try
		//			{
		//				var syntaxRoot = await this.SyntaxRootAsync ().WithTimeout (timeout);
		//				var token = syntaxRoot.FindToken (point);
		//				var node = DocumentController.GetLeftSyntaxNode (token, point);
		//				if (node != null)
		//				{
		//					// syntax and resources available
		//					applicableToSpan = point.GetTextTrackingSpan (token.Span);
		//					var symbolName = node.RemoveTrivias ().ToString ();
		//					symbolName = Regex.Replace (symbolName, @"^global::", string.Empty);
		//					var resources = resourceSymbolMapper.FindPartial (symbolName).ToList ();
		//					if (resources.Count > 0)
		//					{
		//						// TODO: check for multiple namespaces
		//						// if (HasSingleNamespace(resources))
		//						// {
		//						if (resources.Count == 1)
		//						{
		//							return new MultiCultureResourceItemView (resources.First ());
		//						}
		//						else
		//						{
		//							return new MultiCultureResourceItemCollectionView (resources)
		//							{
		//								MaxHeight = 600
		//							};
		//						}
		//						// }
		//						// else
		//						// {

		//						//var semanticTask = activeDocumentController.SemanticModelAsync ();
		//						//if (semanticTask.Wait (QuickInfoSource.Timeout (100), activeDocumentController.RoslynCancellationToken))
		//						//{
		//						//	content = this.GetQiSemanticContent (node, semanticTask.Result);
		//						//}
		//						//else
		//						//{
		//						//	QuickInfoSource.SetQiPending ("Semantic", qiContent);
		//						//}

		//						// }
		//					}
		//				}
		//				return null;
		//			}
		//			catch (TimeoutException)
		//			{
		//				return DocumentController.GetPendingMessage ("Syntax");
		//			}
		//		}
		//		catch (TimeoutException)
		//		{
		//			return DocumentController.GetPendingMessage ("Document");
		//		}
		//	}
		//	catch (TimeoutException)
		//	{
		//		return DocumentController.GetPendingMessage ("Resources");
		//	}
		//}

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

		//private static string GetPendingMessage(string subject)
		//{
		//	return string.Format ("({0} cache is still being constructed. Please try again in a few seconds...)", subject);
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
