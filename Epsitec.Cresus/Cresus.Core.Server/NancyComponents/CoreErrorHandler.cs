using Nancy.ErrorHandling;
using Nancy;
using Nancy.Extensions;
using Epsitec.Cresus.Core.Server.AdditionalResponses;

namespace Epsitec.Cresus.Core.Server.NancyComponents
{
	public class CoreErrorHandler : IErrorHandler
	{
		public void Handle(HttpStatusCode statusCode, NancyContext context)
		{
			context.Response = ExtJsForm.Error();
			context.Response.StatusCode = statusCode;
		}

		public bool HandlesStatusCode(HttpStatusCode statusCode)
		{
			return statusCode == HttpStatusCode.InternalServerError;
		}
	}
}
