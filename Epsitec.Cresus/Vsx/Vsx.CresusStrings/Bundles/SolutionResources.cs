using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Services;

namespace Epsitec.Cresus.Strings.Bundles
{

	[Export (typeof (SolutionResources))]
	public class SolutionResources
	{
		public SolutionResources()
		{
		}

		internal ProjectResources Load(IProject project)
		{
			return this.bundles.GetOrAdd (project.Id, _ => ProjectResources.Load (project.FilePath));
		}

		private readonly ConcurrentDictionary<ProjectId, ProjectResources> bundles = new ConcurrentDictionary<ProjectId, ProjectResources> ();
	}
}
