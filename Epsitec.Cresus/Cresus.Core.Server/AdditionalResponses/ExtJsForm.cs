using System.Collections.Generic;
using Nancy;
using Nancy.Responses;

namespace Epsitec.Cresus.Core.Server.AdditionalResponses
{
	internal class ExtJsForm
	{
		public static Response Error(Dictionary<string, object> errors)
		{
			var parent = new Dictionary<string, object> ();
			parent["success"] = false;
			parent["errors"] = errors;

			return new JsonResponse (parent);
		}

		public static Response Success()
		{
			var parent = new Dictionary<string, object> ();
			parent["success"] = true;

			return new JsonResponse (parent);
		}
	}
}
