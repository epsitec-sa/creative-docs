//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using Nancy;
using Nancy.ErrorHandling;

namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{
	/// <summary>
	/// The <c>CoreErrorHandler</c> class logs all unhandled exceptions thrown during
	/// the Nancy processing of a request.
	/// </summary>
	public class CoreErrorHandler : IStatusCodeHandler
	{
		public void Handle(HttpStatusCode statusCode, NancyContext context)
		{
			context.Response = CoreResponse.InternalServerError ();

			string error;

			object exception;
			object trace;

			if (context.Items.TryGetValue ("ERROR_EXCEPTION", out exception))
			{
				var ex = exception as System.Exception;
				error = ex.GetFullText ();
			}
			else if (context.Items.TryGetValue ("ERROR_TRACE", out trace))
			{
				error = (string) trace;
			}
			else
			{
				error = "Details are not available";
			}

			Tools.LogError ("Uncaught exception while processing nancy request: " + error);
		}


		public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext nancyContext)
		{
			return statusCode == HttpStatusCode.InternalServerError;
		}
	}
}
