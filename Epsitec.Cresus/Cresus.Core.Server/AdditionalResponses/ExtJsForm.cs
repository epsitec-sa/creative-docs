using System.Collections.Generic;
using Nancy;
using Nancy.Responses;

namespace Epsitec.Cresus.Core.Server.AdditionalResponses
{
	internal class ExtJsForm
	{
		public static Response Error()
		{
			var errors = new Dictionary<string, object> ();

			return Error (errors);
		}

		public static Response Error(Dictionary<string, object> errors)
		{
			var parent = new Dictionary<string, object> ();
			parent["success"] = false;
			parent["errors"] = errors;

			var response = new JsonResponse (parent);
			response.StatusCode = HttpStatusCode.BadRequest;

			return response;
		}

		public static Response Success()
		{
			var parent = new Dictionary<string, object> ();
			parent["success"] = true;

			return new JsonResponse (parent);
		}
	}
}
