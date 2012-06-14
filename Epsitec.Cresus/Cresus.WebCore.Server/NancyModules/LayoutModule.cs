using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.UserInterface;

using Nancy;


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
			Get["/{mode}/{controllerSubTypeId}/{id}"] = p => this.ExecuteWithCoreSession (cs => this.GetLayout (cs, p));
		}


		private Response GetLayout(CoreSession coreSession, dynamic parameters)
		{
			var context = coreSession.GetBusinessContext ();

			var entity = Tools.ResolveEntity (context, (string) parameters.id);

			ViewControllerMode mode = Tools.ParseViewControllerMode (parameters.mode);
			int? controllerSubTypeId = Tools.ParseControllerSubTypeId (parameters.controllerSubTypeId);

			var s = PanelBuilder.BuildController (entity, mode, controllerSubTypeId, coreSession);

			return CoreResponse.AsSuccess (s);
		}
		

	}


}
