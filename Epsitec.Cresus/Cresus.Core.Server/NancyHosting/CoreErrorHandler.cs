//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using Nancy;
using Nancy.ErrorHandling;

namespace Epsitec.Cresus.Core.Server.NancyHosting
{
	/// <summary>
	/// Handles the error that have not been caught before and return an error message to the user
	/// </summary>
	public class CoreErrorHandler : IErrorHandler
	{
		public void Handle(HttpStatusCode statusCode, NancyContext context)
		{
			context.Response = CoreResponse.Error();
			context.Response.StatusCode = statusCode;

			System.Diagnostics.Debug.WriteLine ("Exception not caught !");
		}

		public bool HandlesStatusCode(HttpStatusCode statusCode)
		{
			return statusCode == HttpStatusCode.InternalServerError;
		}
	}
}
