using Nancy;
using Nancy.Responses;

using System.Collections.Generic;

using System.Globalization;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{
	
	
	/// <summary>
	/// Responses that are sent to the ExtJS client.
	/// Every response should be one of them, because ExtJS or the global JS
	/// use the "success" field to know what to do with the data.
	/// </summary>
	internal static class CoreResponse
	{


		public static Response AsError()
		{
			// NOTE: This method will return a generic error to the client.

			var errors = new Dictionary<string, object> ();

			return CoreResponse.AsError (errors);
		}


		public static Response AsError(ErrorCode errorCode)
		{
			// NOTE: This method will return the custom error code and the client should behave
			// accordingly.

			var errors = new Dictionary<string, string> ()
			{
				{ "code", ((int) errorCode).ToString (CultureInfo.InvariantCulture) }
			};

			return CoreResponse.AsError (errors);
		}


		public static Response AsError(string title, string message)
		{
			// NOTE: This method will return a title and a message that the client will use to
			// display a dialog box.

			var errors = new Dictionary<string, string> ()
			{
				{ "title", title },
				{ "message", message },
			};

			return CoreResponse.AsError (errors);
		}


		public static Response AsError(object errors)
		{
			var response = CoreResponse.GetResponse (false, errors);

			return CoreResponse.AsJson (response, HttpStatusCode.BadRequest);
		}


		public static Response AsSuccess()
		{
			var content = new Dictionary<string, object> ();
			
			return CoreResponse.AsSuccess (content);
		}


		public static Response AsSuccess(object content)
		{
			var response = CoreResponse.GetResponse (true, content);

			return CoreResponse.AsJson (response, HttpStatusCode.OK);
		}


		public static Response AsJson(object content)
		{
			return CoreResponse.AsJson (content, HttpStatusCode.OK);
		}


		public static Response AsJson(object content, HttpStatusCode httpStatusCode)
		{
			Dumper.Instance.Dump (content);

			return new JsonResponse (content, new DefaultJsonSerializer ())
			{
				StatusCode = httpStatusCode,
			};
		}


		private static Dictionary<string, object> GetResponse(bool success, object content)
		{
			var parent = new Dictionary<string, object> ();

			var key = success
				? "content"
				: "errors";
			
			parent["success"] = success;
			parent[key] = content;

			return parent;
		}


		public enum ErrorCode
		{


			SessionTimeout = 0,


		}


	}


}
