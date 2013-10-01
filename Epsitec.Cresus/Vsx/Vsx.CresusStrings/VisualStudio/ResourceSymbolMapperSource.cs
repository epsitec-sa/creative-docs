using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.IO;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	public class ResourceSymbolMapperSource : IDisposable
	{
		public ResourceSymbolMapperSource(ISolutionProvider solutionProvider)
		{
			this.solutionProvider = solutionProvider;
			this.cts = new CancellationTokenSource ();
			this.SymbolMapperAsync (CancellationToken.None).ConfigureAwait (false);
		}


		public ResourceSymbolMapper SymbolMapper
		{
			get
			{
				return this.SymbolMapperAsync (CancellationToken.None).Result;
			}
		}

		public async Task<ResourceSymbolMapper> SymbolMapperAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			return await this.EnsureSymbolMapperTask ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			lock (this.syncRoot)
			{
				this.cts.Cancel ();
				var subscription = Interlocked.Exchange (ref this.subscription, null);
				if (subscription != null)
				{
					subscription.Dispose ();
				}
				Interlocked.Exchange (ref this.symbolMapperTask, null).DisposeResult ().ForgetSafely ();
			}
		}

		#endregion

		private static IObservable<FileSystemNotification> CreateResourceFileEvents(SolutionResource solution)
		{
			using (new TimeTrace ())
			{
				var files = new HashSet<string> ();
				var folders = new HashSet<string> ();
				foreach (var filePath in solution.TouchedFilePathes ().Select (path => path.ToLower ()))
				{
					files.Add (filePath);
					folders.Add (Path.GetDirectoryName (filePath));
				}
				return Observable.Merge (folders.Select (folder => new FileMonitor (folder).Watch ()))
					.Where (n => files.Contains (n.FullPath, StringComparer.OrdinalIgnoreCase));
			}
		}


		private ISolution Solution
		{
			get
			{
				return this.solutionProvider.Solution;
			}
		}


		private void Restart()
		{
			using (new TimeTrace ())
			{
				this.Dispose ();
				this.SymbolMapperAsync (CancellationToken.None).ConfigureAwait (false);
			}
		}

		private CancellationTokenSource EnsureCts()
		{
			lock (this.syncRoot)
			{
				if (this.cts.IsCancellationRequested)
				{
					Interlocked.Exchange (ref this.cts, new CancellationTokenSource ());
				}
				return this.cts;
			}
		}

		private Task<ResourceSymbolMapper> EnsureSymbolMapperTask()
		{
			lock (this.syncRoot)
			{
				if (this.symbolMapperTask == null || this.symbolMapperTask.Status == TaskStatus.Canceled || this.symbolMapperTask.Status == TaskStatus.Faulted)
				{
					var cancellationToken = this.EnsureCts ().Token;

					this.symbolMapperTask = Task.Run (() =>
					{
						using (new TimeTrace ())
						{
							var solutionResource = new SolutionResource (this.Solution, cancellationToken);
							lock (this.syncRoot)
							{
								this.subscription = ResourceSymbolMapperSource.CreateResourceFileEvents (solutionResource)
									.Throttle (TimeSpan.FromMilliseconds (50))
									.Subscribe (_ => this.Restart ());
							}
							var mapper = new ResourceSymbolMapper ();
							mapper.VisitSolution (solutionResource);
							return mapper;
						}
					}, cancellationToken);
				}
				return this.symbolMapperTask;
			}
		}


		private readonly object syncRoot = new object ();

		private readonly ISolutionProvider solutionProvider;
		private CancellationTokenSource cts;
		private Task<ResourceSymbolMapper> symbolMapperTask;
		private IDisposable subscription;
	}
}
