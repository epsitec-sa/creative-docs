using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
			this.DisposeSymbolMapper ();
		}

		#endregion

		private static IObservable<FileSystemNotification> CreateResourceFileEvents(SolutionResource solution)
		{
			using (new TimeTrace ())
			{
				var folders = new HashSet<string> ();
				foreach (var folderPath in solution.TouchedFolderPathes ().Select (path => path.ToLower (CultureInfo.InvariantCulture)))
				{
					folders.Add (Path.GetDirectoryName (folderPath));
				}
				var monitors = folders
					.SelectMany (folder => ResourceSymbolMapperSource.FileMonitors(folder))
					.Select(m => m.Watch());

				return Observable.Merge (monitors);
			}
		}

		private static IEnumerable<FileMonitor> FileMonitors(string folder)
		{
			yield return new FileMonitor (folder, "module.info", true);
			yield return new FileMonitor (folder, "*.resource", true);
		}


		private ISolution Solution
		{
			get
			{
				return this.solutionProvider.Solution;
			}
		}


		private void DisposeSymbolMapper()
		{
			lock (this.syncMapper)
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

		private void RestartSymbolMapper()
		{
			using (new TimeTrace ())
			{
				this.DisposeSymbolMapper ();
				this.SymbolMapperAsync (CancellationToken.None).ConfigureAwait (false);
			}
		}

		private CancellationTokenSource EnsureCts()
		{
			lock (this.syncMapper)
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
			lock (this.syncMapper)
			{
				if (this.symbolMapperTask == null || this.symbolMapperTask.Status == TaskStatus.Canceled || this.symbolMapperTask.Status == TaskStatus.Faulted)
				{
					var cancellationToken = this.EnsureCts ().Token;

					this.symbolMapperTask = Task.Run (() =>
					{
						using (new TimeTrace ())
						{
							var solutionResource = new SolutionResource (this.Solution, cancellationToken);
							lock (this.syncMapper)
							{
								this.subscription = ResourceSymbolMapperSource.CreateResourceFileEvents (solutionResource)
									.Throttle (TimeSpan.FromMilliseconds (50))
									.Subscribe (_ => this.RestartSymbolMapper ());
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


		private readonly object syncMapper = new object ();

		private readonly ISolutionProvider solutionProvider;
		private CancellationTokenSource cts;
		private Task<ResourceSymbolMapper> symbolMapperTask;
		private IDisposable subscription;
	}
}
