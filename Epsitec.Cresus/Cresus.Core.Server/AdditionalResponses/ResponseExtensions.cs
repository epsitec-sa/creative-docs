using System.Collections.Generic;
using Nancy;

namespace Epsitec.Cresus.Core.Server.AdditionalResponses
{
	public static class ResponseExtensions
	{
		public static Response AsCoreError(this IResponseFormatter formatter)
		{
			return CoreResponse.Error ();
		}

		public static Response AsCoreError(this IResponseFormatter formatter, object dic)
		{
			return CoreResponse.Error (dic);
		}

		public static Response AsCoreSuccess(this IResponseFormatter formatter)
		{
			return CoreResponse.Success ();
		}

		public static Response AsCoreSuccess(this IResponseFormatter formatter, object dic)
		{
			return CoreResponse.Success (dic);
		}

		public static Response AsCoreBoolean(this IResponseFormatter formatter, bool success)
		{
			return success ? CoreResponse.Success () : CoreResponse.Error ();
		}
	}
}
