namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	internal static class Constants
	{


		public static readonly string KeyForNullValue = "null";


		// NOTE
		// Here we can't have an empty string because it does not render propery with the ExtJs
		// client. It would render only as a thin line because it would have no content. We are
		// unfortunately forced to display something in order. I chose to display the same text as
		// the text that is usally used for unknown values for enumeration in Cresus.Core.
		public static readonly string TextForNullValue = "—";


	}


}