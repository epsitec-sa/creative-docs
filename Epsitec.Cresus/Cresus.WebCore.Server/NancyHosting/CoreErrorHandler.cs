using Nancy;
using Nancy.ErrorHandling;

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
			context.Response = CoreResponse.Error ();
			context.Response.StatusCode = statusCode;

			Debug.WriteLine ("Exception not caught !");
		}


		public bool HandlesStatusCode(HttpStatusCode statusCode)
		{
			return statusCode == HttpStatusCode.InternalServerError;
		}


	}


}
