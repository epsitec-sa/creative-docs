using System.Collections.Generic;
using Nancy;

namespace Epsitec.Cresus.Core.Server.AdditionalResponses
{
	public static class ResponseExtensions
	{
		public static Response AsErrorExtJsForm(this IResponseFormatter formatter, Dictionary<string, object> dic)
		{
			return ExtJsForm.Error (dic);
		}

		public static Response AsSuccessExtJsForm(this IResponseFormatter formatter)
		{
			return ExtJsForm.Success ();
		}
	}
}
