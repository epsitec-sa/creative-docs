using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Tools;
using Epsitec.VisualStudio;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	[Export(typeof(Engine))]
	public class Engine : ISolutionProvider, IDisposable
	{
		public Engine()
		{
			using (new TimeTrace ())
			{
				this.workspace = Workspace.PrimaryWorkspace;
				this.solution = Workspace.PrimaryWorkspace.CurrentSolution;
				if (!string.IsNullOrEmpty (this.solution.FilePath))
				{
					this.workspace = Workspace.LoadSolution (this.solution.FilePath, enableFileTracking: true);
					this.solution = this.workspace.CurrentSolution;
				}
				this.documentSourceManager = new DocumentSourceManager (this);
				this.resourceProvider = new ResourceSymbolMapperSource (this);
			}
		}

		[Import]
		public CresusDesigner					CresusDesigner
		{
			get;
			set;
		}

		public DocumentSource					ActiveDocumentSource
		{
			get
			{
				return this.documentSourceManager.ActiveDocumentSource;
			}
		}

		internal void SetActiveDocument(EnvDTE.Document document, ITextBuffer textBuffer = null)
		{
			this.documentSourceManager.SetActiveDocument (document, textBuffer);
		}

		public async Task<ResourceSymbolInfo> GetResourceSymbolInfoAsync(SnapshotPoint point, CancellationToken cancellationToken)
		{
			return await this.ActiveDocumentSource.GetResourceSymbolInfoAsync (point, this.resourceProvider, cancellationToken);
		}


		#region ISolutionProvider Members

		public ISolution Solution
		{
			get
			{
				return this.solution;
			}
		}

		public ISolution UpdateSolution(DocumentId documentId, IText text)
		{
			return this.solution = this.solution.UpdateDocument (documentId, text);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.CresusDesigner.Dispose ();
			this.resourceProvider.Dispose ();
			this.documentSourceManager.Dispose ();
		}

		#endregion

		private readonly ResourceSymbolMapperSource resourceProvider;
		private readonly DocumentSourceManager documentSourceManager;

		// Roslyn DOM
		private IWorkspace workspace;
		private ISolution solution;
	}
}
