using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	[TestClass]
	public class DTETest
	{
		[TestMethod]
		public void TestMethod1()
		{
		}

		private void xxx(ISolution solution, CancellationToken cancellationToken)
		{
			var dte = new DTE ();
			var app = dte.Application;
			if (!solution.Documents (cancellationToken).Any ())
			{
				var dteSolution = app.Solution;

				var dteDocs = app.Documents;
				var dteDocuments = dteDocs.OfType<EnvDTE.Document> ();
				foreach (var dteDocument in dteDocuments)
				{
					var dteProjectItem = dteDocument.ProjectItem;
					var dteProject = dteProjectItem.ContainingProject;
					var projectId = ProjectId.CreateNewId (solution.Id, dteProject.Name);

					//var documentId = new DocumentId ();
					//var documentInfo = new DocumentInfo ();
					//var solution = this.Solution.AddDocument (DocumentInfo.WithFilePath (dteDocument.FullName));
				}

				var dteProjs = app.ActiveSolutionProjects;
				var dteProjects = (dteProjs as object[]).OfType<EnvDTE.Project> ();
				foreach (var dteProject in dteProjects)
				{
				}
			}
			dte.SolutionOpened += this.HandleDTESolutionOpened;
			dte.DocumentOpened += this.HandleDTEDocumentOpened;
		}

		private void HandleDTESolutionOpened()
		{
		}

		private void HandleDTEDocumentOpened(EnvDTE.Document document)
		{
		}
	}
}
