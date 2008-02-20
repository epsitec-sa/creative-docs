//	Copyright © 2007-2008, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	internal class CallbackQueue : System.IDisposable
	{
		public CallbackQueue()
		{
			this.callbacks = callbacks = new LinkedList<Callback> ();
			this.waitHandle = new System.Threading.AutoResetEvent (false);
			this.thread = new System.Threading.Thread (this.WorkerThread);
			this.thread.Start ();
		}


		public void Queue(Callback callback)
		{
			lock (this.exclusion)
			{
				this.callbacks.AddLast (callback);
			}
			
			this.waitHandle.Set ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.thread != null)
			{
				this.exitRequested = true;
				this.waitHandle.Set ();

				Platform.Dispatcher.Join (this.thread);

				this.thread = null;
			}
		}

		#endregion

		private void WorkerThread()
		{
			try
			{
				this.waitHandle.WaitOne ();

				while (!this.exitRequested)
				{
					Callback work = null;
					
					lock (this.exclusion)
					{
						if (this.callbacks.Count > 0)
						{
							work = this.callbacks.First.Value;
							this.callbacks.RemoveFirst ();
						}
					}

					if (work == null)
					{
						this.waitHandle.WaitOne ();
					}
					else
					{
						work ();
					}
				}
			}
			catch (System.Threading.ThreadInterruptedException)
			{
			}
		}

		private readonly object exclusion = new object ();
		private LinkedList<Callback> callbacks;
		private System.Threading.Thread thread;
		private System.Threading.AutoResetEvent waitHandle;
		private bool exitRequested;
	}
}
