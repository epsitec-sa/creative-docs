using Nancy;
using Nancy.Responses;

using System.Collections.Generic;

using System.Globalization;


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
			// NOTE: This method will return a generic error to the client.

			var errors = new Dictionary<string, object> ();
			
			return CoreResponse.Error (errors);
		}


		public static Response Error(ErrorCode errorCode)
		{
			// NOTE: This method will return the custom error code and the client should behave
			// accordingly.

			var errors = new Dictionary<string, string> ()
			{
				{ "code", ((int) errorCode).ToString (CultureInfo.InvariantCulture) }
			};

			return CoreResponse.Error (errors);
		}


		public static Response Error(string title, string message)
		{
			// NOTE: This method will return a title and a message that the client will use to
			// display a dialog box.

			var errors = new Dictionary<string, string> ()
			{
				{ "title", title },
				{ "message", message },
			};

			return CoreResponse.Error (errors);
		}


		public static Response Error(object errors)
		{
			var parent = new Dictionary<string, object> ();
			
			parent["success"] = false;
			parent["errors"] = errors;

			var response = new JsonResponse (parent, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.BadRequest,
			};

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

			return new JsonResponse (parent, new DefaultJsonSerializer ());
		}


		public enum ErrorCode
		{


			SessionTimeout = 0,


		}


	}


}
