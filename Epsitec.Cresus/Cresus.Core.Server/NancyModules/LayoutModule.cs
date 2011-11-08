//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.Core.Server.CoreServer;
using Epsitec.Cresus.Core.Server.NancyHosting;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.Core.Server.NancyModules
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
			Get["/{mode}/{id}"] = parameters => this.ExecuteWithCoreSession(coreSession => 
			{
				var context = coreSession.GetBusinessContext ();

				var entityKey = EntityKey.Parse (parameters.id);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				ViewControllerMode mode = LayoutModule.GetMode (parameters.mode);

				var s = PanelBuilder.BuildController (entity, mode, coreSession);

				return Response.AsCoreSuccess (s);
			});
		}


		private static ViewControllerMode GetMode(string mode)
		{
			return (ViewControllerMode) System.Enum.Parse (typeof (ViewControllerMode), mode, true);
		}


	}


}
