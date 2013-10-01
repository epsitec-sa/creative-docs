using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.VisualStudio
{
	[Flags]
	public enum DteChangeTypes
	{
		SolutionOpened			= 0x0001,
		SolutionRenamed			= 0x0002,
		SolutionBeforeClosing	= 0x0004,
		SolutionAfterClosing	= 0x0008,
		ProjectAdded			= 0x0010,
		ProjectRemoved			= 0x0020,
		ProjectRenamed			= 0x0040,
		DocumentOpened			= 0x0080,
		DocumentClosing			= 0x0100,
		DocumentSaved			= 0x0200,
		All						= 0x02FF,
	}

	public class DteNotification
	{
		public DteNotification(DteChangeTypes changeType)
		{
			this.ChangeType = changeType;
		}

		public DteNotification(DteChangeTypes changeType, string oldName)
		{
			this.ChangeType = changeType;
			this.OldName = oldName;
		}

		public DteNotification(DteChangeTypes changeType, EnvDTE.Project project)
		{
			this.ChangeType = changeType;
			this.Project = project;
		}

		public DteNotification(DteChangeTypes changeType, EnvDTE.Project project, string oldName)
		{
			this.ChangeType = changeType;
			this.Project = project;
			this.OldName = oldName;
		}

		public DteNotification(DteChangeTypes changeType, EnvDTE.Document document)
		{
			this.ChangeType = changeType;
			this.Document = document;
		}

		public DteChangeTypes ChangeType
		{
			get;
			private set;
		}

		public EnvDTE.Project Project
		{
			get;
			private set;
		}

		public EnvDTE.Document Document
		{
			get;
			private set;
		}

		public string OldName
		{
			get;
			private set;
		}

		public override string ToString()
		{
			switch (this.ChangeType)
			{
				case DteChangeTypes.SolutionOpened:
				case DteChangeTypes.SolutionBeforeClosing:
				case DteChangeTypes.SolutionAfterClosing:
					return string.Format ("{0}", this.ChangeType);
				case DteChangeTypes.SolutionRenamed:
					return string.Format ("{0} OldName = {1}", this.ChangeType, this.OldName);
				case DteChangeTypes.ProjectAdded:
				case DteChangeTypes.ProjectRemoved:
					return string.Format ("{0} Project = {1}", this.ChangeType, this.Project.FullName);
				case DteChangeTypes.ProjectRenamed:
					return string.Format ("{0} Project = {1} OldName = {1}", this.ChangeType, this.Project.FullName, this.OldName);
				case DteChangeTypes.DocumentOpened:
				case DteChangeTypes.DocumentClosing:
					return string.Format ("{0} Document = {1}", this.ChangeType, this.Document.FullName);
				default:
					return base.ToString ();
			}
		}
	}
}
