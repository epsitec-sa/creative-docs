using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.DataLayer.Context;
using Nancy;
using Epsitec.Cresus.Core.Controllers;
using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Server.Modules
{
	public class LayoutModule : CoreModule
	{
		public LayoutModule()
			: base ("/layout")
		{
			Get["/{mode}/{id}"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var entityKey = EntityKey.Parse (parameters.id);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				ViewControllerMode mode = LayoutModule.GetMode (parameters.mode);

				var s = PanelBuilder.BuildController (entity, mode, coreSession);

				return Response.AsJson (s);
			};
		}

		private static ViewControllerMode GetMode(string mode)
		{
			return (ViewControllerMode) System.Enum.Parse (typeof (ViewControllerMode), mode, true);
		}
	}
}
