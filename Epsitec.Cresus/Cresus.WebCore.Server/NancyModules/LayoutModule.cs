using Epsitec.Cresus.Core.Business;

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
			var propertyAccessors = this.CoreServer.PropertyAccessorCache;
			var autoCreators = this.CoreServer.AutoCreatorCache;

			var panelBuilder = new PanelBuilder (businessContext, propertyAccessors, autoCreators);
			
			string rawEntityId = parameters.id;
			string rawControllerMode = parameters.mode;
			string rawControllerSubTypeId = parameters.controllerSubTypeId;

			var entity = Tools.ResolveEntity (businessContext, rawEntityId);
			var controllerMode = Tools.ParseViewControllerMode (rawControllerMode);
			var controllerSubTypeId = Tools.ParseControllerSubTypeId (rawControllerSubTypeId);

			var panels = panelBuilder.Build (entity, controllerMode, controllerSubTypeId);

			return CoreResponse.AsSuccess (panels);
		}
		

	}


}
