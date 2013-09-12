using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
			var task2 = this.StartRoslynAsync ();
		}

		public CancellationToken CancellationToken
		{
			get
			{
				return this.ctsId.Token;
			}
		}

		public CancellationToken RoslynCancellationToken
		{
			get
			{
				return this.ctsRoslyn.Token;
			}
		}

		public string Id
		{
			get
			{
				return this.dteDocument.FullName.ToLower ();
			}
		}

		public async Task<IDocument> DocumentAsync()
		{
			this.ctsId.Token.ThrowIfCancellationRequested ();
			return this.parent.Solution.GetDocument (await this.idTask);
		}

		public async Task<CommonSyntaxNode> SyntaxRootAsync()
		{
			this.ctsRoslyn.Token.ThrowIfCancellationRequested ();
			return await this.syntaxRootTask;
		}

		public async Task<ISemanticModel> SemanticModelAsync()
		{
			this.ctsRoslyn.Token.ThrowIfCancellationRequested ();
			return await this.semanticModelTask;
		}


		internal void OnTextChanged(IText text)
		{
			var task = this.RestartRoslynAsync ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.CancelDocumentId ();
			this.CancelRoslyn ();
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
			}, cancellationToken);
		}

		private static async Task<CommonSyntaxNode> GetSyntaxRootAsync(IDocument document, CancellationToken cancellationToken)
		{
			return await Task.Run (() =>
			{
				using (new TimeTrace ("GetSyntaxRoot"))
				{
					return document.GetSyntaxRoot (cancellationToken);
				}
			}, cancellationToken);
		}

		private static async Task<ISemanticModel> GetSemanticModelAsync(IDocument document, CancellationToken cancellationToken)
		{
			return await Task.Run (() =>
			{
				using (new TimeTrace ("GetSemanticModel"))
				{
					return document.GetSemanticModel (cancellationToken);
				}
			}, cancellationToken);
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

		private async Task StartRoslynAsync()
		{
			this.ctsRoslyn = new CancellationTokenSource ();
			var document = await this.DocumentAsync ();
			this.syntaxRootTask = DocumentController.GetSyntaxRootAsync (document, this.ctsRoslyn.Token);
			this.semanticModelTask = DocumentController.GetSemanticModelAsync (document, this.ctsRoslyn.Token);
		}

		private async Task RestartRoslynAsync()
		{
			this.CancelRoslyn ();
			await this.StartRoslynAsync ();
		}

		private void CancelRoslyn()
		{
			this.ctsRoslyn.Cancel ();
			this.syntaxRootTask.ForgetSafely ();
			this.semanticModelTask.ForgetSafely ();
		}


		private readonly WorkspaceController parent;
		private readonly EnvDTE.Document dteDocument;

		private CancellationTokenSource ctsId;
		private Task<DocumentId> idTask;

		private CancellationTokenSource ctsRoslyn;
		private Task<CommonSyntaxNode> syntaxRootTask;
		private Task<ISemanticModel> semanticModelTask;
	}
}
