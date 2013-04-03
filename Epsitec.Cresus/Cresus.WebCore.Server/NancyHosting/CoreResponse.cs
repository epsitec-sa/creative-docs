using Nancy;
using Nancy.Responses;

using System.Collections.Generic;

using System.IO;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{
	
	
	/// <summary>
	/// Responses that are sent to the ExtJS client.
	/// Every response should be one of them, because ExtJS or the global JS
	/// use the "success" field to know what to do with the data.
	/// </summary>
	internal static class CoreResponse
	{


		public static Response InternalServerError()
		{
			var jsonData = new Dictionary<string, object> ();

			return new JsonResponse (jsonData, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.InternalServerError,
			};
		}
		
		
		public static Response Failure()
		{
			var content = new Dictionary<string, object> ();

			return CoreResponse.Failure (content);
		}


		public static Response Failure(string title, string message)
		{
			var content = new Dictionary<string, object> ()
			{
				{ "title", title },
				{ "message", message },
			};

			return CoreResponse.Failure (content);
		}


		public static Response Failure(Dictionary<string, object> content)
		{
			return CoreResponse.CreateJsonResponse (false, content);
		}


		public static Response Success()
		{
			var content = new Dictionary<string, object> ();
			
			return CoreResponse.Success (content);
		}


		public static Response Success(Dictionary<string, object> content)
		{
			return CoreResponse.CreateJsonResponse (true, content);
		}


		private static Response CreateJsonResponse(bool success, object content)
		{
			var jsonData = new Dictionary<string, object> ()
			{
				{ "success", success },
				{ "content", content },
			};

			return CoreResponse.CreateJsonResponse (jsonData, HttpStatusCode.OK);
		}


		public static Response FormSuccess()
		{
			return CoreResponse.CreateFormResponse (true, null);
		}


		public static Response FormFailure(Dictionary<string, object> errors)
		{
			return CoreResponse.CreateFormResponse (false, errors);
		}


		private static Response CreateFormResponse(bool success, object errors)
		{
			// NOTE ExtJs uses the 'success' and 'errors' properties of the returned object in order
			// to know if the form submit is a success or not and which fields are invalid. This is
			// why we need a dedicated method for this.

			var jsonData = new Dictionary<string, object> ()
			{
				{ "success", success },
			};

			if (errors != null)
			{
				jsonData["errors"] = errors;
			}

			return CoreResponse.CreateJsonResponse (jsonData, HttpStatusCode.OK);
		}


		private static Response CreateJsonResponse(Dictionary<string, object> jsonData, HttpStatusCode code)
		{
			return new JsonResponse (jsonData, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.OK,
			};
		}


		public static Response CreateStreamResponse(Stream stream, string filename)
		{
			var contentType = "application/force-download";

			return new StreamResponse (() => stream, contentType)
			{
				Headers =
				{
					{ "Content-Disposition", "attachment; filename=\"" + filename + "\"" }
				}
			};
		}


	}


}
