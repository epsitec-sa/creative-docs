using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;
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


		protected Response Execute(Func<BusinessContext, Response> function)
		{
			return this.Execute ((wp, n, id) => wp.Execute (n, id, function));
		}


		protected Response Execute(Func<WorkerApp, Response> function)
		{
			return this.Execute ((wp, n, id) => wp.Execute (n, id, function));
		}


		protected Response Execute(Func<WorkerApp, BusinessContext, Response> function)
		{
			return this.Execute (wa => wa.Execute (b => function (wa, b)));
		}

		private Response Execute(Func<CoreWorkerPool, string, string, Response> function)
		{
			try
			{
				var userName = LoginModule.GetUserName (this);
				var sessionId = LoginModule.GetSessionId (this);
				var workerPool = this.CoreServer.CoreWorkerPool;

				return function (workerPool, userName, sessionId);
			}
			catch (WorkerThreadException e)
			{
				var businessRuleException = e.InnerException as BusinessRuleException;

				if (businessRuleException != null)
				{
					var errors = new Dictionary<string, object> ()
					{
						{ "businesserror", businessRuleException.Message }
					};

					return CoreResponse.Failure (errors);
				}

				throw;
			}
		}

		protected void Enqueue(Action<BusinessContext> action,string jobId)
		{
			this.Enqueue ((wq, t, n, id) => wq.Enqueue (t, n, id, action),jobId);
		}

		protected void Enqueue(Action<WorkerApp> action,string jobId)
		{
			this.Enqueue ((wq, t, n, id) => wq.Enqueue (t, n, id, action),jobId);
		}

		protected Response Enqueue(Action<WorkerApp, BusinessContext> function,string jobId)
		{
			this.Enqueue (wa => wa.Enqueue (b => function (wa, b)),jobId);
			return null;
		}

		private void Enqueue(Action<CoreWorkerQueue, string, string, string> action,string jobId)
		{
			try
			{
				var userName = LoginModule.GetUserName (this);
				var sessionId = LoginModule.GetSessionId (this);
				var workerQueue = this.CoreServer.CoreWorkerQueue;
				action (workerQueue, jobId, userName, sessionId);
			}
			catch
			{
			}
		}
	}


}
