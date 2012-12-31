using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers.SpecialFieldControllers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public sealed class SpecialFieldModule : AbstractAuthenticatedModule
	{


		public SpecialFieldModule(CoreServer coreServer)
			: base (coreServer, "/specialField")
		{
			Get["/{controllerId}/{entityId}/{methodName}"] =
				p => this.Execute (b => this.Call (b, p));
		}


		public Response Call(BusinessContext businessContext, dynamic parameters)
		{
			string controllerId = parameters.controllerId;
			var controllerType = this.CoreServer.Caches.TypeCache.GetItem (controllerId);

			string entityId = parameters.entityId;
			var entity = EntityIO.ResolveEntity (businessContext, entityId);

			string methodName = parameters.methodName;

			var controller = SpecialFieldController.Create (controllerType, businessContext, entity);
			var method = controller.GetWebMethod (methodName);

			var queryParameters = (DynamicDictionary) Request.Query;
			var arguments = from parameter in method.GetParameters ()
							let name = parameter.Name
							let type = parameter.ParameterType
							let rawValue = (DynamicDictionaryValue) queryParameters[name]
							select FieldIO.ConvertFromClient (businessContext, rawValue, type);

			var result = method.Invoke (controller, arguments.ToArray ());
			var content = new Dictionary<string, object> ()
			{
				{ "data", FieldIO.ConvertToClientRecursive (businessContext, result) },
			};

			return CoreResponse.Success (content);
		}


	}


}
