using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceVisitor
	{
		public virtual ResourceNode VisitItem(ResourceItem item)
		{
			return item;
		}

		public virtual ResourceNode VisitBundle(ResourceBundle bundle)
		{
			Dictionary<string, ResourceItem> newTable = null;
			foreach (var kv in bundle)
			{
				var item = kv.Value;
				var newItem = (ResourceItem) item.Accept (this);
				if (item != newItem)
				{
					if (newTable == null)
					{
						newTable = new Dictionary<string, ResourceItem>();
					}
					newTable[kv.Key] = newItem;
				}
			}
			if (newTable != null)
			{
				foreach (var kv in bundle)
				{
					if (!newTable.ContainsKey (kv.Key))
					{
						newTable[kv.Key] = kv.Value;
					}
				}
				bundle = new ResourceBundle (bundle, newTable);
			}
			return bundle;
		}

		public virtual ResourceNode VisitModule(ResourceModule module)
		{
			ResourceBundle[] newBundles = null;
			var bundles = module.ToArray();
			for (int i = 0; i < bundles.Length; ++i)
			{
				var bundle = bundles[i];
				var newBundle = (ResourceBundle) bundle.Accept (this);
				if (bundle != newBundle)
				{
					if (newBundles == null)
					{
						newBundles = new ResourceBundle[bundles.Length];
					}
					newBundles[i] = newBundle;
				}
			}
			if (newBundles != null)
			{
				for (int i = 0; i < newBundles.Length; ++i)
				{
					if (newBundles[i] == null)
					{
						newBundles[i] = bundles[i];
					}
				}
				module = new ResourceModule (module, newBundles);
			}
			return module;
		}

		public virtual ResourceNode VisitProject(ProjectResource project)
		{
			ResourceModule[] newModules = null;
			var modules = project.ToArray();
			for (int i = 0; i < modules.Length; ++i)
			{
				var module = modules[i];
				var newModule = (ResourceModule) module.Accept (this);
				if (module != newModule)
				{
					if (newModules == null)
					{
						newModules = new ResourceModule[modules.Length];
					}
					newModules[i] = newModule;
				}
			}
			if (newModules != null)
			{
				for (int i = 0; i < newModules.Length; ++i)
				{
					if (newModules[i] == null)
					{
						newModules[i] = modules[i];
					}
				}
				project = new ProjectResource (newModules, project.Solution, project.Project);
			}
			return project;
		}

		public virtual ResourceNode VisitSolution(SolutionResource solution)
		{
			ProjectResource[] newProjects = null;
			var projects = solution.ToArray();
			for (int i = 0; i < projects.Length; ++i)
			{
				var project = projects[i];
				var newProject = (ProjectResource) project.Accept (this);
				if (project != newProject)
				{
					if (newProjects == null)
					{
						newProjects = new ProjectResource[projects.Length];
					}
					newProjects[i] = newProject;
				}
			}
			if (newProjects != null)
			{
				for (int i = 0; i < newProjects.Length; ++i)
				{
					if (newProjects[i] == null)
					{
						newProjects[i] = projects[i];
					}
				}
				solution = new SolutionResource (solution, newProjects);
			}
			return solution;
		}
	}
}
