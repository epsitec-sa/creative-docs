using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.VisualStudio
{
	[Export(typeof(DTE))]
	public class DTE
	{
		public DTE()
		{
			this.application = DTE2Provider.GetDTE2 (System.Diagnostics.Process.GetCurrentProcess ().Id);
			this.solutionEvents = this.application.Events.SolutionEvents;
			this.documentEvents = this.application.Events.DocumentEvents;
			this.windowEvents = this.application.Events.WindowEvents;
		}

		public event EnvDTE._dispSolutionEvents_OpenedEventHandler SolutionOpened
		{
			add
			{
				this.solutionEvents.Opened += value;
			}
			remove
			{
				this.solutionEvents.Opened -= value;
			}
		}

		public event EnvDTE._dispSolutionEvents_RenamedEventHandler SolutionRenamed
		{
			add
			{
				this.solutionEvents.Renamed += value;
			}
			remove
			{
				this.solutionEvents.Renamed -= value;
			}
		}

		public event EnvDTE._dispSolutionEvents_QueryCloseSolutionEventHandler QueryCloseSolution
		{
			add
			{
				this.solutionEvents.QueryCloseSolution += value;
			}
			remove
			{
				this.solutionEvents.QueryCloseSolution -= value;
			}
		}

		public event EnvDTE._dispSolutionEvents_AfterClosingEventHandler AfterClosingSolution
		{
			add
			{
				this.solutionEvents.AfterClosing += value;
			}
			remove
			{
				this.solutionEvents.AfterClosing -= value;
			}
		}

		public event EnvDTE._dispSolutionEvents_BeforeClosingEventHandler BeforeClosingSolution
		{
			add
			{
				this.solutionEvents.BeforeClosing += value;
			}
			remove
			{
				this.solutionEvents.BeforeClosing -= value;
			}
		}

		public event EnvDTE._dispSolutionEvents_ProjectAddedEventHandler ProjectAdded
		{
			add
			{
				this.solutionEvents.ProjectAdded += value;
			}
			remove
			{
				this.solutionEvents.ProjectAdded -= value;
			}
		}

		public event EnvDTE._dispSolutionEvents_ProjectRemovedEventHandler ProjectRemoved
		{
			add
			{
				this.solutionEvents.ProjectRemoved += value;
			}
			remove
			{
				this.solutionEvents.ProjectRemoved -= value;
			}
		}

		public event EnvDTE._dispSolutionEvents_ProjectRenamedEventHandler ProjectRenamed
		{
			add
			{
				this.solutionEvents.ProjectRenamed += value;
			}
			remove
			{
				this.solutionEvents.ProjectRenamed -= value;
			}
		}

		public event EnvDTE._dispDocumentEvents_DocumentClosingEventHandler DocumentClosing
		{
			add
			{
				this.documentEvents.DocumentClosing += value;
			}
			remove
			{
				this.documentEvents.DocumentClosing -= value;
			}
		}

		public event EnvDTE._dispDocumentEvents_DocumentOpeningEventHandler DocumentOpening
		{
			add
			{
				this.documentEvents.DocumentOpening += value;
			}
			remove
			{
				this.documentEvents.DocumentOpening -= value;
			}
		}

		public event EnvDTE._dispDocumentEvents_DocumentOpenedEventHandler DocumentOpened
		{
			add
			{
				this.documentEvents.DocumentOpened += value;
			}
			remove
			{
				this.documentEvents.DocumentOpened -= value;
			}
		}

		public event EnvDTE._dispDocumentEvents_DocumentSavedEventHandler DocumentSaved
		{
			add
			{
				this.documentEvents.DocumentSaved += value;
			}
			remove
			{
				this.documentEvents.DocumentSaved -= value;
			}
		}

		public event EnvDTE._dispWindowEvents_WindowActivatedEventHandler WindowActivated
		{
			add
			{
				this.windowEvents.WindowActivated += value;
			}
			remove
			{
				this.windowEvents.WindowActivated -= value;
			}
		}

		public EnvDTE80.DTE2 Application
		{
			get
			{
				return this.application;
			}
		}

		private readonly EnvDTE80.DTE2 application;
		private readonly EnvDTE.SolutionEvents solutionEvents;
		private readonly EnvDTE.DocumentEvents documentEvents;
		private readonly EnvDTE.WindowEvents windowEvents;
	}
}
