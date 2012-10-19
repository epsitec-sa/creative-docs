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
	/// 
	/// There are two kind of requests for now:
	/// 
	/// 1) Url: /scope/list/
	///    Method: GET
	///    Response format: content: {
	///      scopes: [{
	///         id: ID_OF_SCOPE,
	///         name: NAME_OF_SCOPE,
	///      }],
	///      activeId: ID_OF_ACTIVE_SCOPE
	///    }
	///    This request returns the list of scopes available for the current user.
	///    
	/// 2) Url /scope/set/
	///    Method: POST
	///    Post parameters: scopeId
	///    This request sets the scope to the one given by the id passed with the post parameter.
	/// </summary>
	public sealed class ScopeModule : AbstractAuthenticatedModule
	{


		public ScopeModule(CoreServer coreServer)
			: base (coreServer, "/scope")
		{
			Get["/list/"] = p => this.Execute (wa => this.GetScopeList (wa));
			Post["/set/"] = p => this.Execute (wa => this.SetScope (wa));
		}


		private Response GetScopeList(WorkerApp workerApp)
		{
			var session = workerApp.UserManager.ActiveSession;
			
			var scopes = session.GetAvailableUserScopes ()
				.Select (s => this.GetScopeData (s))
				.ToList ();

			var activeScopeId = session.GetActiveUserScope().Id;

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
