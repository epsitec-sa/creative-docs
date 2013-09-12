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
		private readonly EnvDTE.TextEditorEvents editorEvents;
	}
}
