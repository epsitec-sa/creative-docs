//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using System.Collections.Generic;
using Nancy;
using Nancy.Responses;

namespace Epsitec.Cresus.Core.Server.NancyHosting
{
	/// <summary>
	/// Responses that are sent to the ExtJS client.
	/// Every response should be one of them, because ExtJS or the global JS
	/// use the "success" field to know what to do with the data.
	/// </summary>
	internal class CoreResponse
	{
		public static Response Error()
		{
			var errors = new Dictionary<string, object> ();
			return CoreResponse.Error (errors);
		}

		public static Response Error(object errors)
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
			var content = new Dictionary<string, object> ();
			return CoreResponse.Success (content);
		}

		public static Response Success(object content)
		{
			var parent = new Dictionary<string, object> ();
			parent["success"] = true;
			parent["content"] = content;

			return new JsonResponse (parent);
		}
	}
}
