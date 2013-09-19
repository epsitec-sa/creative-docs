using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	public class ResourceSymbolMapperSource : IDisposable
	{
		public ResourceSymbolMapperSource(ISolutionProvider solutionProvider)
		{
			this.solutionProvider = solutionProvider;
			this.Start ();
		}

		public async Task<ResourceSymbolMapper> SymbolMapperAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			return await this.symbolMapperTask.ConfigureAwait(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Cancel ();
			this.cts.Dispose ();
		}

		#endregion

		private ISolution Solution
		{
			get
			{
				return this.solutionProvider.Solution;
			}
		}

		private Task<ResourceSymbolMapper> CreateSymbolMapperTask()
		{
			return Task.Run (() =>
			{
				using (new TimeTrace ())
				{
					var solutionResource = new SolutionResource (this.Solution, this.cts.Token);
					var mapper = new ResourceSymbolMapper ();
					mapper.VisitSolution (solutionResource);
					return mapper;
				}
			}, this.cts.Token);
		}

		private void Start()
		{
			this.cts = new CancellationTokenSource ();
			this.symbolMapperTask = this.CreateSymbolMapperTask ();
		}

		private void Restart()
		{
			this.Cancel ();
			this.Start ();
		}

		private void Cancel()
		{
			this.cts.Cancel ();
			this.symbolMapperTask.ForgetSafely ();
			this.cts.Dispose ();
		}


		private readonly ISolutionProvider solutionProvider;
		private CancellationTokenSource cts;
		private Task<ResourceSymbolMapper> symbolMapperTask;
	}
}
