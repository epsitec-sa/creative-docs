using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.DataLayer.Context;
using Nancy;


namespace Epsitec.Cresus.Core.Server.Modules
{
	public class LayoutModule : CoreModule
	{
		public LayoutModule()
			: base ("/layout")
		{
			Get["/{id}"] = parameters =>
			{
				var coreSession = MainModule.GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var entityKey = EntityKey.Parse (parameters.id);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				var s = PanelBuilder.BuildController (entity, Controllers.ViewControllerMode.Summary, coreSession);

				return Response.AsJson (s);
			};
		}
	}
}
