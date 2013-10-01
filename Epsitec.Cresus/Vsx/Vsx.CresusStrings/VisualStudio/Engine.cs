using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
	public class Engine : ISolutionProvider, IDisposable
	{
		public static Engine Create()
		{
			using (new TimeTrace ())
			{
				var workspace = Workspace.PrimaryWorkspace;
				var solution = workspace.CurrentSolution;
				if (string.IsNullOrEmpty (solution.FilePath))
				{
					Trace.TraceWarning ("SOLUTION PATH IS EMPTY");
					return null;
				}
				else
				{
					workspace = Workspace.LoadSolution (solution.FilePath);
					// TEST ONLY
					//foreach (var project in this.solution.Projects.Take(1))
					//{
					//	foreach (var document in project.Documents)
					//	{
					//		Trace.WriteLine (document.FilePath);
					//	}
					//}
					return new Engine (workspace);
				}
			}
		}

		private Engine(IWorkspace workspace)
		{
			this.workspace = workspace;
			this.solution = workspace.CurrentSolution;
			this.documentSourceManager = new DocumentSourceManager (this);
			this.resourceProvider = new ResourceSymbolMapperSource (this);
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
