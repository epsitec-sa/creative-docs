using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Layout;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public class LayoutModule : AbstractAuthenticatedModule
	{


		/// <summary>
		/// Call the <see cref="PanelBuilder"/> to create the ExtJS interface.
		/// It is called to show the summary of the edition interface.
		/// </summary>
		public LayoutModule(CoreServer coreServer)
			: base (coreServer, "/layout")
		{
			Get["/{viewMode}/{viewId}/{entityId}"] = p => this.Execute (b => this.GetLayout (b, p));
		}


		private Response GetLayout(BusinessContext businessContext, dynamic parameters)
		{
			var propertyAccessors = this.CoreServer.PropertyAccessorCache;
			var autoCreators = this.CoreServer.AutoCreatorCache;

			var entity = Tools.ResolveEntity (businessContext, (string) parameters.entityId);
			var viewMode = Tools.ParseViewMode ((string) parameters.viewMode);
			var viewId = Tools.ParseViewId ((string) parameters.viewId);

			var layoutBuilder = new LayoutBuilder (businessContext, propertyAccessors, autoCreators);
			var layout = layoutBuilder.Build (entity, viewMode, viewId);

			return CoreResponse.Success (layout);
		}
		

	}


}
