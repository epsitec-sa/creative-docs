using Epsitec.Cresus.WebCore.Server.CoreServer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.WebCore.Server.Tests.Vs.CoreServer
{


	[TestClass]
	public sealed class ServerContextTest
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}
		
		
		[TestMethod]
		public void CoreSessionCleanupTest()
		{
			using (var serverContext = new ServerContext (3, TimeSpan.FromSeconds (10), TimeSpan.FromSeconds (10)))
			{
				var sessions = new List<SafeCoreSession> ();

				for (int i = 0; i < 5; i++)
				{
					sessions.Add (serverContext.CoreSessionManager.CreateSession ());

					foreach (var session in sessions)
					{
						serverContext.CoreSessionManager.GetSession (session.Id);
					}
				}
			
				Thread.Sleep (20000);

				foreach (var session in sessions)
				{
					Assert.IsNull (serverContext.CoreSessionManager.GetSession (session.Id));
				}
			}

		}


	}


}
