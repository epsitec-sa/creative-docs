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
		public Epsitec.VisualStudio.DTE			DTE
		{
			get
			{
				return this.dte;
			}
			set
			{
				this.dte = value;
				this.dte.WindowActivated += this.OnDTEWindowActivated;
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

		internal ITextBuffer					ActiveTextBuffer
		{
			set
			{
				this.documentSourceManager.SetActiveDocument (this.dte.Application.ActiveDocument, value);
			}
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
			this.dte.WindowActivated -= this.OnDTEWindowActivated;
		}

		#endregion

		private void OnDTEWindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
		{
			//var dteActiveDocument = GotFocus.Document;
			//if (dteActiveDocument != null)
			//{
			this.documentSourceManager.SetActiveDocument (this.dte.Application.ActiveDocument);
			//}
		}

		private readonly ResourceSymbolMapperSource resourceProvider;
		private readonly DocumentSourceManager documentSourceManager;

		// Visual Studio DOM
		private Epsitec.VisualStudio.DTE dte;

		// Roslyn DOM
		private IWorkspace workspace;
		private ISolution solution;
	}
}
