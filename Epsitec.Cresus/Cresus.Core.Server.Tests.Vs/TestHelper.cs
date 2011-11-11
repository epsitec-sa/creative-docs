using Epsitec.Cresus.Core.Library;


namespace Epsitec.Cresus.Core.Server.Tests.Vs
{


	internal static class TestHelper
	{


		public static void Initialize()
		{
			if (!TestHelper.isInitialized)
			{
				CoreContext.ParseOptionalSettingsFile (CoreContext.ReadCoreContextSettingsFile ());
				CoreContext.StartAsServer ();

				TestHelper.isInitialized = true;
			}
		}


		private static bool isInitialized = false;



	}


}
