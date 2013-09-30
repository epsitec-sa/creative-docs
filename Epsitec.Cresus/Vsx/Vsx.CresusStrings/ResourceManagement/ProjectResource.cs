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
		public static ProjectResource Load(SolutionResource solution, IProject project, CancellationToken cancellationToken = default(CancellationToken))
		{
			var projectModules = new List<ResourceModule> ();
			var projectResource = new ProjectResource (projectModules, solution, project);
			var modules = ProjectResource.LoadModules (project.FilePath, projectResource, cancellationToken);
			if (modules.Any ())
			{
				projectModules.AddRange (modules);
				return projectResource;
			}
			return null;
		}

		public static ProjectResource Load(string projectFilePath, CancellationToken cancellationToken = default(CancellationToken))
		{
			var projectModules = new List<ResourceModule> ();
			var projectResource = new ProjectResource (projectModules, default (SolutionResource), default (IProject));
			var modules = ProjectResource.LoadModules (projectFilePath, projectResource, cancellationToken);
			if (modules.Any ())
			{
				projectModules.AddRange (modules);
				return projectResource;
			}
			return null;
		}


		internal ProjectResource(IEnumerable<ResourceModule> modules, SolutionResource solution, IProject project)
		{
			this.solution = solution;
			this.project = project;
			this.modules = modules;
		}

		public SolutionResource					Solution
		{
			get
			{
				return this.solution;
			}
		}
		public IProject							Project
		{
			get
			{
				return this.project;
			}
		}


		public IEnumerable<string> TouchedFilePathes()
		{
			return this.SelectMany (m => m.TouchedFilePathes ());
		}


		#region Object Overrides
		public override string ToString()
		{
			return System.IO.Path.GetFullPath(this.project.FilePath);
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


		#region Helpers
		
		private static IEnumerable<ResourceModule> LoadModules(string projectFilePath, ProjectResource project, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			var directory = Path.GetDirectoryName (projectFilePath) + @"\Resources\";
			if (Directory.Exists (directory))
			{
				var fileNames = Directory.EnumerateFiles (directory, "module.info", SearchOption.AllDirectories);
				return fileNames
					.Select (fn => ResourceModule.Load (fn, project, cancellationToken))
					.Where (m => m != null)
					.Do (_ => cancellationToken.ThrowIfCancellationRequested ())
					.ToList ();
			}
			return Enumerable.Empty<ResourceModule> ();
		}
		
		#endregion


		private readonly SolutionResource solution;
		private readonly IProject project;
		private readonly IEnumerable<ResourceModule> modules;
	}
}
