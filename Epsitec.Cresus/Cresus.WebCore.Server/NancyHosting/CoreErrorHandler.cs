using Epsitec.Common.IO;
using Epsitec.Common.Support.Extensions;

using Nancy;
using Nancy.ErrorHandling;

using System;

using System.Diagnostics;


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
			
			var exception = (Exception) context.Items["ERROR_TRACE"];

			var message = "Uncaught exception while processing nancy request:\n"
				+ exception.GetFullText ();

			Logger.LogToConsole (message);
		}


		public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext nancyContext)
		{
			return statusCode == HttpStatusCode.InternalServerError;
		}


	}


}
