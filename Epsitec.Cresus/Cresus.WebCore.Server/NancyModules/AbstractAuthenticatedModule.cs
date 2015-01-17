//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using System.Linq;
using Nancy;

using System.Collections.Generic;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// This is the base class for all modules that requires the user to be logged in. It is thus
	/// the base class of all modules except the LoginModule. It adds checks to ensure that the
	/// user is properly logged in.
	/// In addition, it provides methods that can be used to access the services provided by the
	/// CoreServer, like BusinessContexts and WorkerApps. All modules that need to access these
	/// two services must use the call to the appropriate Execute(...) method, in order to ensure
	/// that the resources are set up and cleaned properly. Note that all calls to these methods
	/// might be blocking if the resources are not immediately available.
	/// </summary>
	public abstract class AbstractAuthenticatedModule : AbstractCoreModule
	{
		protected AbstractAuthenticatedModule(CoreServer coreServer)
			: base (coreServer)
		{
			LoginModule.CheckIsLoggedIn (this);
		}

		protected AbstractAuthenticatedModule(CoreServer coreServer, string modulePath)
			: base (coreServer, modulePath)
		{
			LoginModule.CheckIsLoggedIn (this);
		}

		protected Response Execute(System.Func<BusinessContext, Response> function)
		{
			return this.Execute ((pool, username, sessionId) => pool.Execute (username, sessionId, function));
		}

		protected Response Execute(System.Func<WorkerApp, Response> function)
		{
			return this.Execute ((pool, username, sessionId) => pool.Execute (username, sessionId, function));
		}

		protected void Enqueue(CoreJob task, System.Action<BusinessContext> action)
		{
			task.Enqueue ();

			this.CoreServer.CoreWorkerQueue.Enqueue (task.Id, task.Username, task.SessionId, action);
		}

		protected CoreJob GetJob(string jobId)
		{
			return this.CoreServer.Jobs[jobId];
		}

		protected IEnumerable<CoreJob> GetJobs()
		{
			return this.CoreServer.Jobs.Select (j => j.Value);
		}

		protected bool RemoveJob(string jobId)
		{
			var job = this.GetJob (jobId);
			if (job.Status == CoreJobStatus.Waiting)
			{
				this.CancelJob (job);
				return true;
			}

			if(job.Status != CoreJobStatus.Running) 
			{
				CoreJob removedJob;
				return this.CoreServer.Jobs.TryRemove (jobId, out removedJob);
			}
			return false;
		}

		protected void CancelJob(CoreJob job)
		{
			job.Cancel ();			
			this.CoreServer.CoreWorkerQueue.Cancel (job.Id);
			this.RemoveJob (job.Id);
		}

		private Response Execute(System.Func<CoreWorkerPool, string, string, Response> function)
		{
			try
			{
				var userName   = LoginModule.GetUserName (this);
				var sessionId  = LoginModule.GetSessionId (this);
				var workerPool = this.CoreServer.CoreWorkerPool;

				return function (workerPool, userName, sessionId);
			}
			catch (WorkerThreadException e)
			{
				var businessRuleException = e.InnerException as BusinessRuleException;

				if (businessRuleException != null)
				{
					return AbstractAuthenticatedModule.CreateResponse (businessRuleException);
				}
				
				throw;
			}
		}

		/// <summary>
		/// Creates a unique job ID, even if called within a very tiny interval of time.
		/// Every ID generated is greater than the previous one, which ensures that sorting
		/// will produce coherent results.
		/// </summary>
		/// <returns>The unique job ID.</returns>
		private string CreateJobId()
		{
			while (true)
			{
				var oldId = AbstractAuthenticatedModule.lastJobId;
				var jobId = System.Math.Max (System.DateTime.Now.Ticks, oldId+1);

				if (System.Threading.Interlocked.CompareExchange (ref AbstractAuthenticatedModule.lastJobId, jobId, oldId) == oldId)
				{
					return string.Format ("JOB-{0:X16}", jobId);
				}
			}
		}

		private static Response CreateResponse(BusinessRuleException businessRuleException)
		{
			var errors = new Dictionary<string, object> ()
			{
				{ "businesserror", businessRuleException.Message }
			};

			return CoreResponse.Failure (errors);
		}

		public Response CreateJob(BusinessContext businessContext, string title,bool enableCancelation, out CoreJob job)
		{
			var entityBag = EntityBagManager.GetCurrentEntityBagManager ();
			var statusBar = StatusBarManager.GetCurrentStatusBarManager ();
			var userName   = LoginModule.GetUserName (this);
			var sessionId  = LoginModule.GetSessionId (this);
			job = new CoreJob (userName, sessionId, this.CreateJobId (), title, entityBag, statusBar, enableCancelation);
			this.CoreServer.Jobs.TryAdd (job.Id, job);
			return new Response ()
			{
				StatusCode = HttpStatusCode.Accepted
			};
		}


		private static long						lastJobId;
	}
}
