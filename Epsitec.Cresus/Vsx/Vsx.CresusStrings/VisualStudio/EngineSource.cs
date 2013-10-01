using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Cresus.Strings;
using Epsitec.IO;
using Epsitec.Tools;
using Microsoft.VisualStudio.Text;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	[Export (typeof (EngineSource))]
	public class EngineSource : IDisposable
	{
		public EngineSource()
		{
			this.cts = new CancellationTokenSource ();
			this.EngineAsync (CancellationToken.None).ConfigureAwait (false);
		}

		[Import]
		public Epsitec.VisualStudio.DTE			DTE
		{
			get
			{
				return this.dte;
			}
			set
			{
				this.dte = value;
				this.dte.WindowActivated += this.OnDTEWindowActivated;
			}
		}

		[Import]
		public CresusDesigner					CresusDesigner
		{
			get;
			set;
		}

		public Engine							Engine
		{
			get
			{
				var cts = new CancellationTokenSource (Config.MaxAsyncDelay);
				try
				{
					var engineTask = this.EngineAsync (cts.Token);
					engineTask.Wait (cts.Token);
					return engineTask.Result;
				}
				catch (OperationCanceledException)
				{
					return null;
				}
			}
		}

		public ITextBuffer					ActiveTextBuffer
		{
			set
			{
				var engine = this.Engine;
				if (engine != null)
				{
					engine.SetActiveDocument (this.dte.Application.ActiveDocument, value);
				}
			}
		}

		public async Task<Engine>				EngineAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			return await this.EnsureEngineTask ();
		}

		public async Task<ITextBuffer>			ActiveTextBufferAsync(ITextBuffer textBuffer, CancellationToken cancellationToken)
		{
			var engine = await this.EngineAsync (cancellationToken).ConfigureAwait(false);
			engine.SetActiveDocument (this.dte.Application.ActiveDocument, textBuffer);
			return textBuffer;
		}

		public async Task<ResourceSymbolInfo> GetResourceSymbolInfoAsync(SnapshotPoint point, CancellationToken cancellationToken)
		{
			var engine = await this.EngineAsync (cancellationToken).ConfigureAwait (false);
			return await engine.GetResourceSymbolInfoAsync (point, cancellationToken).ConfigureAwait(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.CresusDesigner.Dispose ();
			this.dte.WindowActivated -= this.OnDTEWindowActivated;
			this.DisposeEngine ();
		}

		#endregion

		
		private void OnDTEWindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
		{
			var engine = this.Engine;
			if (engine != null)
			{
				engine.SetActiveDocument (this.dte.Application.ActiveDocument);
			}
		}


		private void DisposeEngine()
		{
			lock (this.syncEngine)
			{
				this.cts.Cancel ();
				var subscription = Interlocked.Exchange (ref this.subscription, null);
				if (subscription != null)
				{
					subscription.Dispose ();
				}
				Interlocked.Exchange (ref this.engineTask, null).DisposeResult ().ForgetSafely ();
			}
		}

		private void RestartEngine()
		{
			using (new TimeTrace ())
			{
				this.DisposeEngine ();
				this.EngineAsync (CancellationToken.None).ConfigureAwait (false);
			}
		}

		private CancellationTokenSource EnsureCts()
		{
			lock (this.syncEngine)
			{
				if (this.cts.IsCancellationRequested)
				{
					Interlocked.Exchange (ref this.cts, new CancellationTokenSource ());
				}
				return this.cts;
			}
		}

		private static IEnumerable<FileMonitor> CreateFileMonitors(ISolution solution)
		{
			yield return new FileMonitor (Path.GetDirectoryName (solution.FilePath), "*.sln");
			foreach (var project in solution.Projects)
			{
				yield return new FileMonitor (Path.GetDirectoryName (project.FilePath), "*.csproj");
			}
		}

		private static IObservable<FileSystemNotification> CreateFileEvents(ISolution solution)
		{
			var monitors = EngineSource.CreateFileMonitors (solution)
				.Select (m => m.Watch ());
			return Observable.Merge (monitors);
		}

		private static IObservable<DteNotification> CreateDomEvents(EnvDTE80.DTE2 dte)
		{

			return dte.Watch (
				DteChangeTypes.SolutionOpened |
				DteChangeTypes.SolutionRenamed |
				DteChangeTypes.ProjectAdded |
				DteChangeTypes.ProjectRemoved |
				DteChangeTypes.ProjectRenamed |
				DteChangeTypes.SolutionOpened);
		}

		private static IObservable<Unit> CreateEvents(EnvDTE80.DTE2 dte, ISolution solution)
		{
			using (new TimeTrace ())
			{
				var fileEvents = EngineSource.CreateFileEvents (solution).Select (n => Unit.Default);
				var domEvents = EngineSource.CreateDomEvents (dte).Select (n => Unit.Default);
				return Observable.Merge (fileEvents, domEvents);
			}
		}

		private Task<Engine> EnsureEngineTask()
		{
			lock (this.syncEngine)
			{
				if (this.engineTask == null || this.engineTask.Status == TaskStatus.Canceled || this.engineTask.Status == TaskStatus.Faulted)
				{
					var cancellationToken = this.EnsureCts ().Token;

					this.engineTask = Task.Run (() =>
					{
						using (new TimeTrace ())
						{
							var engine = Engine.Create ();
							if (engine != null)
							{
								lock (this.syncEngine)
								{
									this.subscription = EngineSource.CreateFileEvents (engine.Solution)
										.Throttle (TimeSpan.FromMilliseconds (250))
										.Subscribe (_ => this.RestartEngine ());
								}
							}
							return engine;
						}
					}, cancellationToken);
				}
				return this.engineTask;
			}
		}


		private readonly object syncEngine = new object ();

		// Visual Studio DOM
		private Epsitec.VisualStudio.DTE dte;

		private CancellationTokenSource cts;
		private Task<Engine> engineTask;
		private IDisposable subscription;
	}
}
