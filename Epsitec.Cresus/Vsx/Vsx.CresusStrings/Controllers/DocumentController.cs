using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace Epsitec.Controllers
{
	public class DocumentController : IDisposable
	{
		public DocumentController(WorkspaceController parent, EnvDTE.Document dteDocument)
		{
			this.parent = parent;
			this.dteDocument = dteDocument;
			var task1 = this.StartActiveDocumentIdAsync (dteDocument);
			var task2 = this.StartSyntaxAndSemanticAsync ();
		}

		public CancellationToken CancellationToken
		{
			get
			{
				return this.ctsId.Token;
			}
		}

		public CancellationToken SyntaxAndSemanticCancellationToken
		{
			get
			{
				return this.ctsSyntaxAndSemantic.Token;
			}
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
			set
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
			this.ctsId.Token.ThrowIfCancellationRequested ();
			return this.parent.Solution.GetDocument (await this.idTask);
		}

		public async Task<CommonSyntaxNode> SyntaxRootAsync()
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			return await this.syntaxRootTask;
		}

		public async Task<ISemanticModel> SemanticModelAsync()
		{
			this.ctsSyntaxAndSemantic.Token.ThrowIfCancellationRequested ();
			return await this.semanticModelTask;
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
		}

		#endregion

	
		private static async Task<DocumentId> GetIdAsync(ISolution solution, EnvDTE.Document dteDocument, CancellationToken cancellationToken)
		{
			return await Task.Run (() =>
			{
				using (new TimeTrace ("GetDocumentIdAsync"))
				{
					cancellationToken.ThrowIfCancellationRequested ();
					var dteActiveDocumentPath = dteDocument.FullName;
					for (int retryCount = 0; retryCount < 3; ++retryCount)
					{
						try
						{
							var document = solution.Documents (cancellationToken)
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
			}, cancellationToken).ConfigureAwait(false);
		}

		private static async Task<CommonSyntaxNode> GetSyntaxRootAsync(IDocument document, CancellationToken cancellationToken)
		{
			return await Task.Run (() =>
			{
				using (new TimeTrace ("GetSyntaxRoot"))
				{
					return document.GetSyntaxRoot (cancellationToken);
				}
			}, cancellationToken).ConfigureAwait (false);
		}

		private static async Task<ISemanticModel> GetSemanticModelAsync(IDocument document, CancellationToken cancellationToken)
		{
			return await Task.Run (() =>
			{
				using (new TimeTrace ("GetSemanticModel"))
				{
					return document.GetSemanticModel (cancellationToken);
				}
			}, cancellationToken).ConfigureAwait (false);
		}


		private async Task<DocumentId> StartActiveDocumentIdAsync(EnvDTE.Document dteActiveDocument)
		{
			this.ctsId = new CancellationTokenSource ();
			return await (this.idTask = DocumentController.GetIdAsync (this.parent.Solution, dteActiveDocument, this.ctsId.Token));
		}

		private async Task<DocumentId> RestartActiveDocumentIdAsync(EnvDTE.Document dteActiveDocument)
		{
			this.CancelDocumentId ();
			return await this.StartActiveDocumentIdAsync (dteActiveDocument);
		}
		
		private void CancelDocumentId()
		{
			this.ctsId.Cancel ();
			this.idTask.ForgetSafely ();
		}

		private async Task StartSyntaxAndSemanticAsync()
		{
			this.ctsSyntaxAndSemantic = new CancellationTokenSource ();
			var document = await this.DocumentAsync ();
			this.syntaxRootTask = DocumentController.GetSyntaxRootAsync (document, this.ctsSyntaxAndSemantic.Token);
			this.semanticModelTask = DocumentController.GetSemanticModelAsync (document, this.ctsSyntaxAndSemantic.Token);
		}

		private async Task RestartSyntaxAndSemanticAsync()
		{
			this.CancelSyntaxAndSemantic ();
			await this.StartSyntaxAndSemanticAsync ();
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
			var solution = this.parent.UpdateSolution (document.Id, text);
			await this.RestartSyntaxAndSemanticAsync ();
			return solution;
		}


		private readonly WorkspaceController parent;
		private readonly EnvDTE.Document dteDocument;

		private CancellationTokenSource ctsId;
		private Task<DocumentId> idTask;

		private CancellationTokenSource ctsSyntaxAndSemantic;
		private Task<CommonSyntaxNode> syntaxRootTask;
		private Task<ISemanticModel> semanticModelTask;

		private ITextBuffer textBuffer;
	}
}
