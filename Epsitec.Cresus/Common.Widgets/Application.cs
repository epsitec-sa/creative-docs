//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>Application</c> class offers basic, application related, services.
	/// </summary>
	public class Application : System.IDisposable
	{
		protected Application()
		{
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
			}
		}
		
		public void RunMessageLoop()
		{
			System.Windows.Forms.Application.Run (this.Window.PlatformWindow);
		}

		public void PumpMessageLoop()
		{
			System.Windows.Forms.Application.DoEvents ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}
		
		static Application()
		{
			Application.thread = System.Threading.Thread.CurrentThread;
		}
		
		private static System.Threading.Thread thread;

		public static void QueueAsyncCallback(Support.SimpleCallback callback)
		{
			lock (Application.queueExclusion)
			{
				System.Diagnostics.Debug.Assert (Application.thread == System.Threading.Thread.CurrentThread);
				
				if ((Application.pendingCallbacks.Contains (callback)) ||
					(Application.runningCallbacks.Contains (callback)))
				{
					//	Do nothing. The callback is already in the queue.
				}
				else
				{
					Application.pendingCallbacks.Enqueue (callback);
				}
			}
		}

		public static void ExecuteAsyncCallbacks()
		{
			System.Diagnostics.Debug.Assert (Application.thread == System.Threading.Thread.CurrentThread);
			System.Diagnostics.Debug.Assert (Application.runningCallbacks.Count == 0);

			if (Application.pendingCallbacks.Count > 0)
			{
				lock (Application.queueExclusion)
				{
					Application.runningCallbacks = Application.pendingCallbacks;
					Application.pendingCallbacks = new Queue<Support.SimpleCallback> ();
				}

				while (Application.runningCallbacks.Count > 0)
				{
					Support.SimpleCallback callback = Application.runningCallbacks.Dequeue ();
					callback ();
				}
			}
		}

		private static object queueExclusion = new object ();
		private static Queue<Support.SimpleCallback> pendingCallbacks = new Queue<Support.SimpleCallback> ();
		private static Queue<Support.SimpleCallback> runningCallbacks = new Queue<Support.SimpleCallback> ();

		private Window window;
	}
}
