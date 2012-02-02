using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.UserInterface;

using Epsitec.Cresus.DataLayer.Context;

using System;



namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public class LayoutModule : AbstractCoreSessionModule
	{


		/// <summary>
		/// Call the <see cref="PanelBuilder"/> to create the ExtJS interface.
		/// It is called to show the summary of the edition interface.
		/// </summary>
		public LayoutModule(ServerContext serverContext)
			: base (serverContext, "/layout")
		{
			Get["/{mode}/{controllerSubTypeId}/{id}"] = parameters => this.ExecuteWithCoreSession(coreSession => 
			{
				var context = coreSession.GetBusinessContext ();

				var entityKey = EntityKey.Parse (parameters.id);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				ViewControllerMode mode = Tools.ParseViewControllerMode (parameters.mode);
				int? controllerSubTypeId = Tools.ParseControllerSubTypeId (parameters.controllerSubTypeId);

				var s = PanelBuilder.BuildController (entity, mode, controllerSubTypeId, coreSession);

				return Response.AsCoreSuccess (s);
			});
		}
		

	}


}
