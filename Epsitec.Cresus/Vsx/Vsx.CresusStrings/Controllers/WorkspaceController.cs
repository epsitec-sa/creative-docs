using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace Epsitec.Controllers
{
	[Export(typeof(WorkspaceController))]
	public class WorkspaceController : IDisposable
	{
		public WorkspaceController()
		{
			using (new TimeTrace ("WorkspaceController"))
			{
				this.workspace = Workspace.PrimaryWorkspace;
				this.solution = Workspace.PrimaryWorkspace.CurrentSolution;
				if (!string.IsNullOrEmpty (this.solution.FilePath))
				{
					this.workspace = Workspace.LoadSolution (this.solution.FilePath, enableFileTracking: true);
					this.solution = this.workspace.CurrentSolution;
				}
				this.resourceController = new ResourceController (this);
				this.workspace.WorkspaceChanged += this.OnWorkspaceChanged;
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

				this.activeDocumentController = new DocumentController (this, this.dte.Application.ActiveDocument);
				this.documentControllers[this.activeDocumentController.Id] = this.activeDocumentController;

				this.dte.WindowActivated += this.OnDTEWindowActivated;
			}
		}

		public ISolution Solution
		{
			get
			{
				return this.solution;
			}
		}

		public ResourceController ResourceController
		{
			get
			{
				return this.resourceController;
			}
		}

		public DocumentController ActiveDocumentController
		{
			get
			{
				return this.activeDocumentController;
			}
		}


		public async Task<ISolution> UpdateActiveDocumentAsync(IEnumerable<Roslyn.Compilers.TextChange> changes)
		{
			var document = await this.activeDocumentController.DocumentAsync ();
			var text = document.GetText ().WithChanges (changes);
			this.solution = this.Solution.UpdateDocument (document.Id, text);
			this.activeDocumentController.OnTextChanged (text);
			return this.solution;
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.resourceController.Dispose ();
			foreach (var documentController in this.documentControllers.Values)
			{
				documentController.Dispose ();
			}

			this.workspace.WorkspaceChanged -= this.OnWorkspaceChanged;
			this.dte.SolutionOpened -= this.OnDTESolutionOpened;
			this.dte.DocumentOpened -= this.OnDTEDocumentOpened;
			this.dte.WindowActivated -= this.OnDTEWindowActivated;
		}

		#endregion


		private void OnWorkspaceChanged(object sender, WorkspaceEventArgs e)
		{
		}

		private void OnDTESolutionOpened()
		{
		}
		
		private void OnDTEDocumentOpened(EnvDTE.Document Document)
		{
		}

		private void OnDTEWindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
		{
			var dteActiveDocument = GotFocus.Document;
			if (dteActiveDocument != null)
			{
				var id = dteActiveDocument.FullName.ToLower ();
				this.activeDocumentController = this.documentControllers.GetOrAdd (id, _ => new DocumentController (this, dteActiveDocument));
			}
		}


		private readonly ResourceController resourceController;
		private readonly Dictionary<string, DocumentController> documentControllers = new Dictionary<string, DocumentController> ();

		private DocumentController activeDocumentController;

		// Visual Studio DOM
		private Epsitec.VisualStudio.DTE dte;

		// Roslyn DOM
		private IWorkspace workspace;
		private ISolution solution;
	

	}
}
