//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using Nancy;

namespace Epsitec.Cresus.Core.Server.AdditionalResponses
{
	/// <summary>
	/// Extensions to be able to easily call our custom responses.
	/// </summary>
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
