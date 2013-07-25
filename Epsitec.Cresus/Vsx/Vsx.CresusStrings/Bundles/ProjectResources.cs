using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Services;

namespace Epsitec.Cresus.Strings.Bundles
{
	public class ProjectResources : IEnumerable<ResourceModule>
	{
		public static ProjectResources Load(IProject project)
		{
			var modules = ProjectResources.LoadModules (project.FilePath);
			return new ProjectResources (project, modules);
		}

		public static ProjectResources Load(string projectFilePath)
		{
			var modules = ProjectResources.LoadModules (projectFilePath);
			return new ProjectResources (null, modules);
		}

		public IProject Project
		{
			get
			{
				return this.project;
			}
		}

		#region IEnumerable<ModuleBundles> Members

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

		private ProjectResources(IProject project, IEnumerable<ResourceModule> modules)
		{
			this.project = project;
			this.modules = modules.ToList();
		}

		private static IEnumerable<ResourceModule> LoadModules(string projectFilePath)
		{
			var directory = Path.GetDirectoryName (projectFilePath) + @"\Resources\";
			var fileNames = Directory.EnumerateFiles (directory, "module.info", SearchOption.AllDirectories);
			return fileNames.Select (fn => ResourceModule.Load (fn));
		}

		private readonly IProject project;
		private readonly List<ResourceModule> modules;
	}
}
