using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceMapper : ResourceVisitor
	{
		public CompositeDictionary				Map
		{
			get
			{
				return this.map;
			}
		}
		public IEnumerable<IKey>				DuplicateKeys
		{
			get
			{
				return this.duplicateKeys;
			}
		}
		public IEnumerable<ResourceItemError>	ResourceItemErrors
		{
			get
			{
				return this.resourceItemErrors;
			}
		}
		public SolutionResource					SolutionResource
		{
			get
			{
				return this.solution;
			}
		}

		protected ProjectResource				CurrentProjectResource
		{
			get
			{
				return this.project;
			}
		}
		protected ResourceModule				CurrentResourceModule
		{
			get
			{
				return this.module;
			}
		}
		protected ResourceBundle				CurrentResourceBundle
		{
			get
			{
				return this.bundle;
			}
		}

		protected virtual ICompositeKey CreateAccessKey(ResourceItem item)
		{
			return Key.Create (													// Example
				this.CurrentResourceBundle.Culture,								// fr-CH
				this.CurrentResourceModule.Info.ResourceNamespace.Split ('.'),	// .Epsitec.Cresus
				"Res",															// .Res
				this.CurrentResourceBundle.Name,								// .Strings
				item.Name.Split ('.'));											// .Application.Name
		}

		#region ResourceVisitor Overrides

		public override ResourceNode VisitItem(ResourceItem item)
		{
			item = base.VisitItem (item) as ResourceItem;
			if (item is ResourceItemError)
			{
				this.resourceItemErrors.Add (item as ResourceItemError);
			}

			var key = this.CreateAccessKey (item);
			if (this.map.ContainsKey (key))
			{
				this.duplicateKeys.Add (key);
			}
			else
			{
				this.map[key] = item;
			}

			return item;
		}

		public override ResourceNode VisitBundle(ResourceBundle bundle)
		{
			this.bundle = bundle;
			return base.VisitBundle (bundle);
		}

		public override ResourceNode VisitModule(ResourceModule module)
		{
			this.module = module;
			return base.VisitModule (module);
		}

		public override ResourceNode VisitProject(ProjectResource project)
		{
			this.project = project;
			return base.VisitProject (project);
		}

		public override ResourceNode VisitSolution(SolutionResource solution)
		{
			this.solution = solution;
			return base.VisitSolution (solution);
		}

		#endregion

		private readonly CompositeDictionary map = new CompositeDictionary ();
		private readonly HashSet<IKey> duplicateKeys = new HashSet<IKey> ();
		private readonly List<ResourceItemError> resourceItemErrors = new List<ResourceItemError> ();

		private SolutionResource solution;
		private ProjectResource project;
		private ResourceModule module;
		private ResourceBundle bundle;
	}
}
