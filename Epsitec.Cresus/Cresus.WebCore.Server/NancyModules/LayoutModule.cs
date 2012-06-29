using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.UserInterface;

using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public class LayoutModule : AbstractBusinessContextModule
	{


		/// <summary>
		/// Call the <see cref="PanelBuilder"/> to create the ExtJS interface.
		/// It is called to show the summary of the edition interface.
		/// </summary>
		public LayoutModule(CoreServer coreServer)
			: base (coreServer, "/layout")
		{
			Get["/{mode}/{controllerSubTypeId}/{id}"] = p => this.Execute (b => this.GetLayout (b, p));
		}


		private Response GetLayout(BusinessContext businessContext, dynamic parameters)
		{
			var entity = Tools.ResolveEntity (businessContext, (string) parameters.id);

			ViewControllerMode mode = Tools.ParseViewControllerMode (parameters.mode);
			int? controllerSubTypeId = Tools.ParseControllerSubTypeId (parameters.controllerSubTypeId);

			var s = PanelBuilder.BuildController (entity, mode, controllerSubTypeId, businessContext, this.CoreServer.PropertyAccessorCache, this.CoreServer.AutoCreatorCache);

			return CoreResponse.AsSuccess (s);
		}
		

	}


}
