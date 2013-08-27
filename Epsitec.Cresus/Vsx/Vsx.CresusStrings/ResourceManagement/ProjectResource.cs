using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Roslyn.Services;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ProjectResource : ResourceNode, IEnumerable<ResourceModule>
	{
		public static ProjectResource Load(IProject project, CancellationToken cancellationToken)
		{
			var modules = ProjectResource.LoadModules (project.FilePath, cancellationToken);
			if (modules.Any ())
			{
				return new ProjectResource (project, modules);
			}
			return null;
		}

		public static ProjectResource Load(string projectFilePath, CancellationToken cancellationToken)
		{
			var modules = ProjectResource.LoadModules (projectFilePath, cancellationToken);
			if (modules.Any ())
			{
				return new ProjectResource (default(IProject), modules);
			}
			return null;
		}

		public ProjectResource(ProjectResource resource, IEnumerable<ResourceModule> modules)
			: this(resource.project, modules)
		{
		}

		public ProjectResource(IProject project, IEnumerable<ResourceModule> modules)
		{
			this.project = project;
			this.modules = modules.ToList ();
		}

		public IProject Project
		{
			get
			{
				return this.project;
			}
		}

		#region Object Overrides
		public override string ToString()
		{
			return System.IO.Path.GetFullPath(project.FilePath);
		}
		#endregion

		#region ResourceNode Overrides

		public override ResourceNode Accept(ResourceVisitor visitor)
		{
			return visitor.VisitProject (this);
		}

		#endregion

		#region IEnumerable<ResourceModule> Members

		public IEnumerator<ResourceModule> GetEnumerator()
		{
			return this.modules.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.modules.GetEnumerator ();
		}

		#endregion

		private static IEnumerable<ResourceModule> LoadModules(string projectFilePath, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			var directory = Path.GetDirectoryName (projectFilePath) + @"\Resources\";
			if (Directory.Exists (directory))
			{
				var fileNames = Directory.EnumerateFiles (directory, "module.info", SearchOption.AllDirectories);
				return fileNames
					.Select (fn => ResourceModule.Load (fn, cancellationToken))
					.Do (_ => cancellationToken.ThrowIfCancellationRequested ())
					.ToList();
			}
			return Enumerable.Empty<ResourceModule> ();
		}

		private readonly IProject project;
		private readonly List<ResourceModule> modules;
	}
}
