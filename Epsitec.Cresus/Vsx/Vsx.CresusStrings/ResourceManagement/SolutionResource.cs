using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Roslyn.Services;

namespace Epsitec.Cresus.ResourceManagement
{
	public class SolutionResource : ResourceNode, IEnumerable<ProjectResource>
	{
		public SolutionResource(ISolution solution, CancellationToken cancellationToken = default(CancellationToken))
			: this (solution.Projects, cancellationToken)
		{
		}

		public SolutionResource(SolutionResource solutionResource, IEnumerable<ProjectResource> projectResources)
			: this(solutionResource.solution, projectResources)
		{
		}

		public SolutionResource(ISolution solution, IEnumerable<ProjectResource> projectResources)
		{
			this.solution = solution;
			this.projectResourceMap = projectResources.ToDictionary (r => r.Project.Id);
		}

		public ISolution Solution
		{
			get
			{
				return this.solution;
			}
		}

		public ProjectResource this[ProjectId id]
		{
			get
			{
				return this.projectResourceMap[id];
			}
		}

		public IEnumerable<string> TouchedFilePathes()
		{
			return this.SelectMany (p => p.TouchedFilePathes ());
		}

		public IEnumerable<string> TouchedFolderPathes()
		{
			return this.SelectMany (p => p.TouchedFolderPathes ());
		}


		#region ResourceNode Overrides

		public override ResourceNode Accept(ResourceVisitor visitor)
		{
			return visitor.VisitSolution(this);
		}
		
		#endregion

		#region IEnumerable<ProjectResource> Members

		public IEnumerator<ProjectResource> GetEnumerator()
		{
			return this.projectResourceMap.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private SolutionResource(IEnumerable<IProject> projects, CancellationToken cancellationToken = default(CancellationToken))
		{
			this.projectResourceMap = projects
				.Select (p => new
					{
						Key = p.Id,
						Resource = ProjectResource.Load (this, p, cancellationToken)
					})
				.Where(a => a.Resource != null)
				.Do (_ => cancellationToken.ThrowIfCancellationRequested ())
				.ToDictionary (a => a.Key, a => a.Resource);
		}

		private readonly ISolution solution;
		private readonly Dictionary<ProjectId, ProjectResource> projectResourceMap;
	}
}
