using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// This modules handles all the requests related to scope management
	/// </summary>
	public sealed class ScopeModule : AbstractAuthenticatedModule
	{


		public ScopeModule(CoreServer coreServer)
			: base (coreServer, "/scope")
		{
			// Gets the list of scopes available for the current user, as well as the active scope
			// id.
			Get["/list/"] = p =>
				this.Execute (wa => this.GetScopeList (wa));

			// Sets the scope for the current user.
			// POST arguments:
			// - scopeId: The id of the scope that must be the user's current scope.
			Post["/set/"] = p =>
				this.Execute (wa => this.SetScope (wa));
		}


		private Response GetScopeList(WorkerApp workerApp)
		{
			var session = workerApp.UserManager.ActiveSession;

			var scopes = session.GetAvailableUserScopes ()
				.Select (s => this.GetScopeData (s))
				.ToList ();

			var activeScope = session.GetActiveUserScope ();

			var activeScopeId = activeScope != null
				? activeScope.Id
				: null;

			var content = new Dictionary<string, object> ()
			{
				{ "scopes", scopes },
				{ "activeId", activeScopeId }, 
			};

			return CoreResponse.Success (content);
		}


		private Dictionary<string, object> GetScopeData(UserScope scope)
		{
			return new Dictionary<string, object> ()
			{
				{ "id", scope.Id },
				{ "name", scope.Name },
			};
		}


		private Response SetScope(WorkerApp workerApp)
		{
			string scopeId = Request.Form.scopeId;

			var session = workerApp.UserManager.ActiveSession;
			session.SetActiveUserScope (scopeId);

			return CoreResponse.Success ();
		}


	}


}
