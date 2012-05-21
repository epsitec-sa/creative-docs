using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{
	
	
	/// <summary>
	/// Extensions to be able to easily call our custom responses.
	/// </summary>
	public static class CoreResponseExtensions
	{


		public static Response AsCoreError(this IResponseFormatter formatter)
		{
			return CoreResponse.Error ();
		}


		public static Response AsCoreError(this IResponseFormatter formatter, object dictionary)
		{
			return CoreResponse.Error (dictionary);
		}


		public static Response AsCoreSuccess(this IResponseFormatter formatter)
		{
			return CoreResponse.Success ();
		}


		public static Response AsCoreSuccess(this IResponseFormatter formatter, object dictionary)
		{
			return CoreResponse.Success (dictionary);
		}


		public static Response AsCoreBoolean(this IResponseFormatter formatter, bool success)
		{
			return success ? CoreResponse.Success () : CoreResponse.Error ();
		}


	}

}
