//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types.Collections.Concurrent;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// The <c>CoreWorkerQueue</c> manages the asynchronous execution of work items,
	/// sequentially dispatched in a single worker thread.
	/// </summary>
	public sealed class CoreWorkerQueue : System.IDisposable
	{
		public CoreWorkerQueue(CultureInfo uiCulture)
		{
			this.worker         = new CoreWorker ("CoreWorkerQueue", uiCulture);
			this.workItems      = new ConcurrentQueue<WorkItem> ();
			this.wakeUpEvent    = new System.Threading.ManualResetEvent (false);
			this.executerThread = new System.Threading.Thread (this.ThreadMainLoop);
			
			this.executerThread.Name = "CoreWorkerQueue.MainLoop";
			this.executerThread.Start ();
		}

		
		public void Enqueue(string workItemId, string username, string sessionId, System.Action<BusinessContext> action)
		{
			this.workItems.Enqueue (new WorkItemBusinessContext (workItemId, username, sessionId, action));
			this.wakeUpEvent.Set ();
		}

		public void Enqueue(string workItemId, string username, string sessionId, System.Action<WorkerApp> action)
		{
			this.workItems.Enqueue (new WorkItemWorkerApp (workItemId, username, sessionId, action));
			this.wakeUpEvent.Set ();
		}

		public void Cancel(string workItemId)
		{
			this.workItems.Where (x => x.WorkItemId == workItemId).ForEach (x => x.Cancel ());
			this.wakeUpEvent.Set ();
		}

		public void DebugDumpQueue()
		{
			System.Diagnostics.Trace.WriteLine (string.Join ("\r\n", this.workItems.Where (x => !x.Cancelled).Select (x => x.ToString ())));
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.killRequest = true;
			this.wakeUpEvent.Set ();
			this.executerThread.Join ();
			this.worker.Dispose ();
			this.wakeUpEvent.Dispose ();
		}

		#endregion

		
		private void ThreadMainLoop()
		{
			while (this.killRequest == false)
			{
				this.wakeUpEvent.WaitOne ();
				this.wakeUpEvent.Reset ();

				while ((this.killRequest == false)
					&& (this.ExecuteWorkItem ()))
				{
					/* loop */
				}
			}
		}

		private bool ExecuteWorkItem()
		{
			WorkItem item;

			if (this.workItems.TryDequeue (out item))
			{
				if (!item.Cancelled)
				{
					try
					{
						item.Execute (this.worker);
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Trace.WriteLine ("CoreWorkerQueue: execution of work item crashed.");
						System.Diagnostics.Trace.WriteLine (ex.Message);
						System.Diagnostics.Trace.WriteLine (ex.StackTrace);
					}
				}

				return true;
			}

			return false;
		}


		#region WorkItem Classes

		private class WorkItemBusinessContext : WorkItem
		{
			public WorkItemBusinessContext(string workItemId, string username, string sessionId, System.Action<BusinessContext> action)
				: base (workItemId, username, sessionId)
			{
				this.action = action;
			}

			public override void Execute(CoreWorker worker)
			{
				worker.Execute (userName, sessionId, action);
			}

			private readonly System.Action<BusinessContext> action;
		}
		
		private class WorkItemWorkerApp : WorkItem
		{
			public WorkItemWorkerApp(string workItemId, string username, string sessionId, System.Action<WorkerApp> action)
				: base (workItemId, username, sessionId)
			{
				this.action = action;
			}

			public override void Execute(CoreWorker worker)
			{
				worker.Execute (userName, sessionId, action);
			}

			private readonly System.Action<WorkerApp> action;
		}

		#endregion


		private readonly CoreWorker					worker;
		private readonly ConcurrentQueue<WorkItem>	workItems;
		private readonly System.Threading.Thread	executerThread;
		private readonly System.Threading.ManualResetEvent wakeUpEvent;
		private volatile bool						killRequest;
	}
}
