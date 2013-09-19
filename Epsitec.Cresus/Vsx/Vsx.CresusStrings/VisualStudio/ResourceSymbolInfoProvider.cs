using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.VisualStudio;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	[Export(typeof(ResourceSymbolInfoProvider))]
	public class ResourceSymbolInfoProvider : ISolutionProvider, IDisposable
	{
		public ResourceSymbolInfoProvider()
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
				this.workspace.WorkspaceChanged += this.HandleWorkspaceChanged;
			}
		}

		[Import]
		public Epsitec.VisualStudio.DTE DTE
		{
			get
			{
				return this.dte;
			}
			set
			{
				this.dte = value;
				//var app = value.Application;
				//if (!this.Solution.Documents (this.cts.Token).Any ())
				//{
				//	var dteSolution = app.Solution;

				//	var dteDocs = app.Documents;
				//	var dteDocuments = dteDocs.OfType<EnvDTE.Document> ();
				//	foreach (var dteDocument in dteDocuments)
				//	{
				//		var dteProjectItem = dteDocument.ProjectItem;
				//		var dteProject = dteProjectItem.ContainingProject;
				//		var projectId = ProjectId.CreateNewId (this.Solution.Id, dteProject.Name);
				//		//var documentId = new DocumentId ();
				//		//var documentInfo = new DocumentInfo ();
				//		//var solution = this.Solution.AddDocument (DocumentInfo.WithFilePath (dteDocument.FullName));
				//	}

				//	var dteProjs = app.ActiveSolutionProjects;
				//	var dteProjects = (dteProjs as object[]).OfType<EnvDTE.Project> ();
				//	foreach (var dteProject in dteProjects)
				//	{
				//	}
				//}
				//this.dte.SolutionOpened += this.OnDTESolutionOpened;
				//this.dte.DocumentOpened += this.OnDTEDocumentOpened;

				this.ActiveDteDocument = this.dte.Application.ActiveDocument;
				this.dte.WindowActivated += this.HandleDTEWindowActivated;
			}
		}

		public DocumentSource ActiveDocumentSource
		{
			get
			{
				return this.documentSourceManager.ActiveDocumentSource;
			}
		}

		public async Task<ResourceSymbolInfo> GetResourceSymbolInfoAsync(SnapshotPoint point, CancellationToken cancellationToken)
		{
			return await this.ActiveDocumentSource.GetResourceSymbolInfoAsync (point, this.resourceProvider, cancellationToken).ConfigureAwait (false);
		}

		internal ITextBuffer ActiveTextBuffer
		{
			set
			{
				this.documentSourceManager.ActiveTextBuffer = value;
			}
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
			this.resourceProvider.Dispose ();
			this.documentSourceManager.Dispose ();

			this.workspace.WorkspaceChanged -= this.HandleWorkspaceChanged;
			this.dte.SolutionOpened -= this.HandleDTESolutionOpened;
			this.dte.DocumentOpened -= this.HandleDTEDocumentOpened;
			this.dte.WindowActivated -= this.HandleDTEWindowActivated;
		}

		#endregion


		private EnvDTE.Document ActiveDteDocument	
		{
			set
			{
				this.documentSourceManager.ActiveDteDocument = value;
			}
		}

		private void HandleWorkspaceChanged(object sender, WorkspaceEventArgs e)
		{
		}

		private void HandleDTESolutionOpened()
		{
		}
		
		private void HandleDTEDocumentOpened(EnvDTE.Document document)
		{
		}

		private void HandleDTEWindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
		{
			var focusedDocument = GotFocus.Document;
			if (focusedDocument != null)
			{
				this.documentSourceManager.ActiveDteDocument = focusedDocument;
			}
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
