//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Server.NancyComponents;

namespace Epsitec.Cresus.Core.Server
{
	public sealed class CoreServerProgram
	{
		public CoreServerProgram()
		{
			IconsBuilder.BuildIcons (@"S:\Epsitec\experimental-js\webcore\");

			RunNancy ();

			//	TODO: wait until the server shuts down...
		}

		private static void RunNancy()
		{
			CoreServerProgram.RunSelf ();
			//CoreServerProgram.RunWcf ();
			//System.Threading.Thread.Sleep (60*1000);
		}

		private static void RunSelf()
		{
			var coreHost = new CoreHost (BaseUri);
			coreHost.Start ();

			System.Console.WriteLine ("Nancy now listening - navigate to {0}", BaseUri);

			//coreHost.Stop ();
			coreHost.Join ();
		}

		// Does not work
		//private static void RunWcf()
		//{
		//    using (CreateAndOpenWebServiceHost ())
		//    {
		//        System.Console.WriteLine ("Service is now running on: {0}", BaseUri);
		//    }

		////    CreateAndOpenWebServiceHost ();
		////    System.Console.WriteLine ("Service is now running on: {0}", BaseUri);
		//}

		//private static WebServiceHost CreateAndOpenWebServiceHost()
		//{
		//    var host = new WebServiceHost (new NancyWcfGenericService (), BaseUri);

		//    host.AddServiceEndpoint (typeof (NancyWcfGenericService), new WebHttpBinding (), "");
		//    host.Open ();

		//    return host;
		//}

		private static readonly System.Uri BaseUri = new System.Uri ("http://localhost:12345/");
	}
}
