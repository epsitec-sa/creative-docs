using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Core.Server.CoreServer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.Core.Server.Tests.Vs.CoreServer
{


	[TestClass]
	public sealed class CoreSessionManagerTest
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void SimpleTest()
		{
			var manager = new CoreSessionManager (3, TimeSpan.FromSeconds (5));

			var session = manager.CreateSession ();

			Assert.AreEqual (session, manager.GetSession (session.Id));

			manager.DeleteSession (session.Id);

			Assert.IsNull (manager.GetSession (session.Id));
		}


		[TestMethod]
		public void BigTest()
		{
			var manager = new CoreSessionManager (3, TimeSpan.FromSeconds (5));

			var sessionIds = new List<string> ();
			var sessions = new Dictionary<string, SafeCoreSession> ();

			var dice = new Random ();

			var actions = new Action[]
			{
				() =>
				{
					var session = manager.CreateSession ();

					Assert.IsFalse (sessions.ContainsKey (session.Id));
					Assert.IsFalse (sessions.ContainsValue (session));

					sessions[session.Id] = session;
					sessionIds.Add (session.Id);
				},
				
				() =>
				{
					if (sessionIds.Count > 1)
					{
						var sessionIdIndex = dice.Next (sessionIds.Count);
						var sessionId = sessionIds[sessionIdIndex];
						var session = sessions[sessionId];
						
						Assert.AreEqual (session, manager.GetSession (sessionId));
					}
				},
				
				() =>
				{
					if (sessionIds.Count > 2)
					{
						var sessionIdIndex = dice.Next (sessionIds.Count);
						var sessionId = sessionIds[sessionIdIndex];

						Assert.IsNotNull (manager.GetSession (sessionId));

						Assert.IsTrue (manager.DeleteSession (sessionId));

						Assert.IsNull (manager.GetSession (sessionId));

						Assert.IsFalse (manager.DeleteSession (sessionId));

						sessions.Remove (sessionId);
						sessionIds.RemoveAt (sessionIdIndex);
					}
				},
			};

			RandomExecutor.ExecuteRandomly (15, actions);
		}


		[TestMethod]
		public void CleanupTest()
		{
			var manager = new CoreSessionManager (2, TimeSpan.FromSeconds (3));

			var sessions = Enumerable.Range (0, 4)
				.Select (_ => manager.CreateSession ())
				.ToList ();

			foreach (var session in sessions)
			{
				Assert.IsNotNull (manager.GetSession (session.Id));

				Thread.Sleep (100);
			}

			Thread.Sleep (TimeSpan.FromSeconds (3));

			foreach (var session in sessions.Take (3))
			{
				Assert.IsNotNull (manager.GetSession (session.Id));
			}

			manager.CleanUpSessions ();

			Assert.IsNull (manager.GetSession (sessions[0].Id));
			Assert.IsNotNull (manager.GetSession (sessions[1].Id));
			Assert.IsNotNull (manager.GetSession (sessions[2].Id));
			Assert.IsNull (manager.GetSession (sessions[3].Id));
		}


	}


}
