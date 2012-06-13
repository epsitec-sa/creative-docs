using Epsitec.Common.IO;
using Epsitec.Common.Support.Extensions;

using Nancy;
using Nancy.ErrorHandling;

using System;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{
	
	
	/// <summary>
	/// Handles the error that have not been caught before and return an error message to the user
	/// </summary>
	public class CoreErrorHandler : IErrorHandler
	{


		public void Handle(HttpStatusCode statusCode, NancyContext context)
		{
			context.Response = CoreResponse.AsError ();
			context.Response.StatusCode = statusCode;

			string error;
			
			object exception;
			object trace;

			if (context.Items.TryGetValue ("ERROR_EXCEPTION", out exception))
			{
				error = ((Exception) exception).GetFullText ();
			}
			else if (context.Items.TryGetValue ("ERROR_TRACE", out trace))
			{
				error = (string) trace;
			}
			else
			{
				error = "Details are not available";
			}

			var message = "Uncaught exception while processing nancy request: " + error;

			Logger.LogToConsole (message);
		}


		public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext nancyContext)
		{
			return statusCode == HttpStatusCode.InternalServerError;
		}


	}


}
