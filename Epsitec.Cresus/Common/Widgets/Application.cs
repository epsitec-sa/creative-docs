//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Linq;
using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>Application</c> class offers basic, application related, services.
	/// </summary>
	public abstract class Application : DependencyObject
	{
		protected Application()
		{
			this.applicationThread = System.Threading.Thread.CurrentThread;
			this.applicationStartStatus = Platform.AppSupport.CreateSemaphore (this.ApplicationIdentifier);
			this.commandDispatcher = CommandDispatcher.DefaultDispatcher;
			this.commandContext = new CommandContext ();
			this.resourceManager = Support.Resources.DefaultManager;
			this.resourceManagerPool = this.resourceManager.Pool;
		}


		/// <summary>
		/// Gets the application window.
		/// </summary>
		/// <value>The application window.</value>
		public Window							Window
		{
			get
			{
				return this.window;
			}
			protected set
			{
				System.Diagnostics.Debug.Assert (this.window == null);
				
				this.window = value;

				this.CommandDispatcher.RegisterController (this);
				
				this.SetupApplicationWindow (this.window);
			}
		}

		/// <summary>
		/// Gets the application command dispatcher.
		/// </summary>
		/// <value>The application command dispatcher.</value>
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				return this.commandDispatcher;
			}
		}

		/// <summary>
		/// Gets the application command context.
		/// </summary>
		/// <value>The application command context.</value>
		public CommandContext					CommandContext
		{
			get
			{
				return this.commandContext;
			}
		}

		/// <summary>
		/// Gets the application resource manager.
		/// </summary>
		/// <value>The application resource manager.</value>
		public Support.ResourceManager			ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		/// <summary>
		/// Gets the application resource manager pool.
		/// </summary>
		/// <value>The application resource manager pool.</value>
		public Support.ResourceManagerPool		ResourceManagerPool
		{
			get
			{
				return this.resourceManagerPool;
			}
		}

		/// <summary>
		/// Gets the main application thread.
		/// </summary>
		/// <value>
		/// The main application thread.
		/// </value>
		public System.Threading.Thread			ApplicationThread
		{
			get
			{
				return this.applicationThread;
			}
		}

		/// <summary>
		/// Gets the global application thread (i.e. the one which first accessed the
		/// <see cref="Application"/> class). This is the main UI thread for the process.
		/// </summary>
		public static System.Threading.Thread	MainUIThread
		{
			get
			{
				return Application.thread;
			}
		}
		
		public static bool						IsRunningOnMainUIThread
		{
			get
			{
				return Application.MainUIThread == System.Threading.Thread.CurrentThread;
			}
		}


		public static bool						DisableAsyncCallbackExecution
		{
			get
			{
				return Application.disableAsyncCallbackExecution;
			}
			set
			{
				Application.disableAsyncCallbackExecution = value;
			}
		}

		public abstract string					ShortWindowTitle
		{
			get;
		}

		public abstract string					ApplicationIdentifier
		{
			get;
		}

		public ApplicationStartStatus			ApplicationStartStatus
		{
			get
			{
				return this.applicationStartStatus;
			}
		}


		public static bool						IsExecutingAsyncCallbacks
		{
			get
			{
				return Application.executingAsyncCallbacks;
			}
		}

		public static bool						HasPendingAsyncCallbacks
		{
			get
			{
				return Application.pendingCallbacks.Count > 0;
			}
		}

		public static void SetWaitCursor()
		{
			System.Threading.Interlocked.Increment (ref Application.waitCursorCount);
			Platform.Window.UseWaitCursor = true;
		}

		public static void ClearWaitCursor()
		{
			if (System.Threading.Interlocked.Decrement (ref Application.waitCursorCount) < 0)
			{
				throw new System.InvalidOperationException ("WaitCursor count < 0");
			}
			
			Platform.Window.UseWaitCursor = (Application.waitCursorCount > 0);
		}


		public void SetEnable(Command command, bool enable)
		{
			if (this.commandContext != null)
			{
				this.commandContext.GetCommandState (command).Enable = enable;
			}
		}

		public void SetActiveState(Command command, ActiveState activeState)
		{
			if (this.commandContext != null)
			{
				this.commandContext.GetCommandState (command).ActiveState = activeState;
			}
		}

		public static string GetCommandLineProgram()
		{
			var commandLine = System.Environment.CommandLine;
			var splitMode   = System.StringSplitOptions.RemoveEmptyEntries;

			return Support.Utilities.StringToTokens (commandLine, ' ', splitMode).FirstOrDefault ();
		}

		public static IList<string> GetCommandLineArguments()
		{
			var commandLine = System.Environment.CommandLine;
			var splitMode   = System.StringSplitOptions.RemoveEmptyEntries;
			
			return new List<string> (Support.Utilities.StringToTokens (commandLine, ' ', splitMode).Skip (1));
		}


		
		public void RunMessageLoop()
		{
			System.Windows.Forms.Application.Run (this.Window.PlatformWindow);
		}

		public void PumpMessageLoop()
		{
			System.Windows.Forms.Application.DoEvents ();
		}

		public void DispatchCommandLineCommands()
		{
			this.DispatchCommandLineCommands (null);
		}

		public void DispatchCommandLineCommands(System.Action cleanupAction)
		{
			if (this.Window.DispatchCommandLineCommands ())
			{
				try
				{
					if (cleanupAction != null)
					{
						cleanupAction ();
					}
				}
				finally
				{
					System.Environment.Exit (0);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

		[Support.Command (ApplicationCommands.Id.Quit)]
		protected virtual void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Application: Quit executed");
			e.Executed = true;
			Window.Quit ();
		}

		private void SetupApplicationWindow(Window window)
		{
			CommandDispatcher.SetDispatcher (window, this.CommandDispatcher);
			CommandContext.SetContext (window, this.CommandContext);
			Application.SetApplication (window, this);
			
			Support.ResourceManager.SetResourceManager (window, this.ResourceManager);

			window.Root.WindowType = WindowType.Document;
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;
			window.Name = "Application";
			window.PreventAutoClose = true;
			window.PreventAutoQuit = false;
		}
		
		static Application()
		{
			Application.thread = System.Threading.Thread.CurrentThread;
		}

		public static void QueueTasklets(string name, params TaskletJob[] jobs)
		{
			var tasklet = Tasklet.QueueBatch (name, jobs);

			if (tasklet.ContainsPendingJobs)
			{
				Application.QueueAsyncCallback (tasklet.ExecuteAllJobs);
			}
		}


		
		public static bool HasQueuedAsyncCallback(Support.SimpleCallback callback)
		{
			lock (Application.queueExclusion)
			{
				if ((Application.pendingCallbacks.Contains (callback)) ||
					(Application.runningCallbacks.Contains (callback)))
				{
					return true;
				}
			}

			return false;
		}

		public static void RemoveQueuedAsyncCallback(Support.SimpleCallback callback)
		{
			lock (Application.queueExclusion)
			{
				if (Application.pendingCallbacks.Contains (callback))
				{
					int n = Application.pendingCallbacks.Count;

					for (int i = 0; i < n; i++)
					{
						Support.SimpleCallback item = Application.pendingCallbacks.Dequeue ();
						
						if (item != callback)
						{
							Application.pendingCallbacks.Enqueue (item);
						}
					}
				}
				if (Application.runningCallbacks.Contains (callback))
				{
					int n = Application.runningCallbacks.Count;

					for (int i = 0; i < n; i++)
					{
						Support.SimpleCallback item = Application.runningCallbacks.Dequeue ();

						if (item != callback)
						{
							Application.runningCallbacks.Enqueue (item);
						}
					}
				}
			}
		}

		public static void QueueAsyncCallback(Support.SimpleCallback callback)
		{
			lock (Application.queueExclusion)
			{
				//	Reorder the queue if the callback is already in the queue; otherwise
				//	add it to the pending queue and make sure the main thread executes it
				//	soon.

				if (Application.pendingCallbacks.Count > 50)
				{
					System.Diagnostics.Debug.WriteLine ("Probable performance issue: more than 50 pending callbacks queued !");
				}
				
				if (Application.pendingCallbacks.Contains (callback))
				{
					Application.pendingCallbacks.Requeue (callback);
				}
				else if (Application.runningCallbacks.Contains (callback))
				{
					Application.runningCallbacks.Requeue (callback);
				}
				else
				{
					Application.pendingCallbacks.Enqueue (callback);

					if (Application.thread != System.Threading.Thread.CurrentThread)
					{
						Platform.Window.SendAwakeEvent ();
					}
					else
					{
						Platform.Window.SendAwakeEvent ();
					}
				}
			}
		}

		public static void ExecuteAsyncCallbacks()
		{
			if ((Application.executingAsyncCallbacks) ||
				(Application.disableAsyncCallbackExecution))
			{
				return;
			}

			System.Diagnostics.Debug.Assert (Application.thread == System.Threading.Thread.CurrentThread);
			System.Diagnostics.Debug.Assert (Application.runningCallbacks.Count == 0);

			if (Application.pendingCallbacks.Count > 0)
			{
				try
				{
					Application.executingAsyncCallbacks = true;
					
					lock (Application.queueExclusion)
					{
						Application.runningCallbacks = Application.pendingCallbacks;
						Application.pendingCallbacks = new Queue<Support.SimpleCallback> ();
					}

//-					System.Diagnostics.Trace.WriteLine ("Executing async callbacks, started.");

					while (Application.runningCallbacks.Count > 0)
					{
						Support.SimpleCallback callback = Application.runningCallbacks.Dequeue ();
						callback ();
					}

//-					System.Diagnostics.Trace.WriteLine ("Executing async callbacks, done.");
				}
				finally
				{
					if (Application.runningCallbacks.Count > 0)
					{
						System.Diagnostics.Debug.WriteLine ("Running callbacks not executed: " + Application.runningCallbacks.Count, "ExecuteAsyncCallbacks");

						lock (Application.queueExclusion)
						{
							Queue<Support.SimpleCallback> queue = new Queue<Support.SimpleCallback> ();

							while (Application.runningCallbacks.Count > 0)
							{
								queue.Enqueue (Application.runningCallbacks.Dequeue ());
							}
							while (Application.pendingCallbacks.Count > 0)
							{
								queue.Enqueue (Application.pendingCallbacks.Dequeue ());
							}

							Application.pendingCallbacks = queue;
						}
					}

					System.Diagnostics.Debug.Assert (Application.runningCallbacks.Count == 0);
					
					Application.executingAsyncCallbacks = false;
				}
			}
		}

		public static void Invoke(Support.SimpleCallback callback)
		{
			Drawing.Platform.Dispatcher.Invoke (callback);
		}

		public static void SetApplication(DependencyObject obj, Application application)
		{
			obj.SetValue (Application.ApplicationProperty, application);
		}

		public static Application GetApplication(DependencyObject obj)
		{
			return (Application) obj.GetValue (Application.ApplicationProperty);
		}

		public static readonly DependencyProperty ApplicationProperty = DependencyProperty.RegisterAttached ("Application", typeof (Application), typeof (Application));

		private static readonly System.Threading.Thread thread;
		private static readonly object queueExclusion = new object ();
		private static Queue<Support.SimpleCallback> pendingCallbacks = new Queue<Support.SimpleCallback> ();
		private static Queue<Support.SimpleCallback> runningCallbacks = new Queue<Support.SimpleCallback> ();
		private static bool executingAsyncCallbacks;
		private static bool disableAsyncCallbackExecution;
		private static int waitCursorCount;

		private readonly System.Threading.Thread applicationThread;
		private readonly CommandDispatcher commandDispatcher;
		private readonly CommandContext commandContext;
		private readonly Support.ResourceManager resourceManager;
		private readonly Support.ResourceManagerPool resourceManagerPool;
		private readonly ApplicationStartStatus applicationStartStatus;
		
		private Window window;
	}
}
