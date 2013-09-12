using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Roslyn.Services;

namespace Epsitec.Controllers
{
	public class ResourceController : IDisposable
	{
		public ResourceController(WorkspaceController parent)
		{
			this.parent = parent;
			var task = this.StartAsync ();
		}

		public CancellationToken CancellationToken
		{
			get
			{
				return this.cts.Token;
			}
		}

		public async Task<ResourceSymbolMapper> SymbolMapperAsync()
		{
			return await this.mapperTask;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Cancel ();
		}

		#endregion

		private static async Task<ResourceSymbolMapper> LoadAsync(ISolution solution, CancellationToken cancellationToken)
		{
			return await Task.Run (() =>
			{
				using (new TimeTrace ("LoadResourcesAsync"))
				{
					var solutionResource = new SolutionResource (solution, cancellationToken);
					var mapper = new ResourceSymbolMapper ();
					mapper.VisitSolution (solutionResource);
					return mapper;
				}
			}, cancellationToken).ConfigureAwait(false);
		}


		private async Task<ResourceSymbolMapper> StartAsync()
		{
			this.cts = new CancellationTokenSource ();
			return await (this.mapperTask = ResourceController.LoadAsync (this.parent.Solution, this.cts.Token));
		}

		private async Task<ResourceSymbolMapper> RestartAsync()
		{
			this.Cancel ();
			return await this.StartAsync ();
		}

		private void Cancel()
		{
			this.cts.Cancel ();
			this.mapperTask.ForgetSafely ();
		}


		private readonly WorkspaceController parent;
		private CancellationTokenSource cts;
		private Task<ResourceSymbolMapper> mapperTask;
	}
}
