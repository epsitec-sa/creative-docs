using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceField
	{
		public ResourceField(ResourceItem item, ResourceBundle bundle = null, ResourceModule module = null, ProjectResource project = null, SolutionResource solution = null)
		{
			this.Item = item;
			this.Bundle = bundle;
			this.Module = module;
			this.Project = project;
			this.Solution = solution;
		}

		public ResourceItem Item
		{
			get;
			private set;
		}

		public ResourceBundle Bundle
		{
			get;
			private set;
		}

		public ResourceModule Module
		{
			get;
			private set;
		}

		public ProjectResource Project
		{
			get;
			private set;
		}

		public SolutionResource Solution
		{
			get;
			private set;
		}

		public string Namespace
		{
			get
			{
				return this.Module.Info.ResourceNamespace;
			}
		}

		public string SymbolName
		{
			get
			{
				return this.Item.SymbolName;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.Bundle.Culture;
			}
		}

		#region Object Overrides

		public override string ToString()
		{
			return string.Format ("{0}[{1}] = {2}", this.SymbolName, this.Culture.Parent.DisplayName, this.Item.Value);
		}

		#endregion
	}
}
