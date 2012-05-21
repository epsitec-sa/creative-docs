using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestRequestView
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void SimpleTest1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var keys = dataContext.GetByExample (new NaturalPersonEntity ())
					.Select (e => dataContext.GetNormalizedEntityKey (e).Value)
					.ToList ();

				var request = Request.Create (new NaturalPersonEntity ());

				using (var requestView = dataContext.GetRequestView (request))
				{
					Assert.AreEqual (3, requestView.GetCount ());

					for (int i = 0; i < 5; i++)
					{
						for (int j = 0; j < 5; j++)
						{
							var expectedKeys = keys.Skip (i).Take (j).ToList ();
							var actualKeys = requestView.GetKeys (i, j).ToArray ();

							CollectionAssert.AreEqual (expectedKeys, actualKeys);
							this.GetIndexTest (requestView, actualKeys, i);
						}
					}
				}
			}
		}


		[TestMethod]
		public void SimpleTest2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var keys = dataContext.GetByExample (new NaturalPersonEntity ())
					.OrderByDescending (p => p.Lastname)
					.Select (e => dataContext.GetNormalizedEntityKey (e).Value)
					.ToList ();

				var request = Request.Create (new NaturalPersonEntity ());

				request.SortClauses.Add
				(
					new SortClause
					(
						new PublicField (request.RootEntity, Druid.Parse ("[J1AM1]")),
						SortOrder.Descending
					)
				);

				using (var requestView = dataContext.GetRequestView (request))
				{
					Assert.AreEqual (3, requestView.GetCount ());

					for (int i = 0; i < 5; i++)
					{
						for (int j = 0; j < 5; j++)
						{
							var expectedKeys = keys.Skip (i).Take (j).ToList ();
							var actualKeys = requestView.GetKeys (i, j).ToArray ();

							CollectionAssert.AreEqual (expectedKeys, actualKeys);
							this.GetIndexTest (requestView, actualKeys, i);
						}
					}
				}
			}
		}


		[TestMethod]
		public void SimpleTest3()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				Request request = new Request ()
				{
					RootEntity = DatabaseCreator2.GetCorrectExample3 (),
				};

				using (var requestView = dataContext.GetRequestView (request))
				{
					var keys = requestView.GetKeys (0, 5);

					Assert.AreEqual (2, keys.Count);
					this.GetIndexTest (requestView, keys.ToArray (), 0);
				}
			}
		}


		public void GetIndexTest(RequestView requestView, EntityKey[] entityKeys, int nbSkip)
		{
			for (int i = 0; i < entityKeys.Length; i++)
			{
				var entityKey = entityKeys[i];
				var index = requestView.GetIndex (entityKey);

				Assert.AreEqual (i + nbSkip, index);
			}
		}


		[TestMethod]
		public void ConcurrencyTest()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var request = Request.Create (new NaturalPersonEntity ());

				using (var requestView = dataContext.GetRequestView (request))
				{
					var time = DateTime.Now;

					var t1 = new Thread (() =>
					{
						while (DateTime.Now < time.AddSeconds (10))
						{
							dataContext.GetByExample (new NaturalPersonEntity ());
						}
					});

					var t2 = new Thread (() =>
					{
						var dice = new Random ();

						while (DateTime.Now < time.AddSeconds (10))
						{
							switch (dice.Next (0, 2))
							{
								case 0:
									requestView.GetKeys (0, 3);
									break;

								case 1:
									requestView.GetCount ();
									break;
							}
						}
					});

					t1.Start ();
					t2.Start ();

					t1.Join ();
					t2.Join ();
				}
			}
		}


		[TestMethod]
		public void IsolationTest()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var request = Request.Create (new NaturalPersonEntity ());

				using (var requestView = dataContext.GetRequestView (request))
				{
					var expectedKeys = requestView.GetKeys (0, 5).ToArray ();
					var expectedCount = requestView.GetCount ();

					for (int i = 0; i < 100; i++)
					{
						if (i % 2 == 0)
						{
							if ((i / 2) % 2 == 0)
							{
								dataContext.CreateEntity<NaturalPersonEntity> ();

								dataContext.SaveChanges ();
							}
							else
							{
								var entities = dataContext.GetByExample (new NaturalPersonEntity ());
								var entity = entities.GetRandomElement ();
								dataContext.DeleteEntity (entity);

								dataContext.SaveChanges ();
							}
						}
						else
						{
							var actualKeys = requestView.GetKeys (0, 5).ToArray ();
							var actualCount = requestView.GetCount ();

							CollectionAssert.AreEqual (expectedKeys, actualKeys);
							Assert.AreEqual (expectedCount, actualCount);
						}
					}
				}
			}
		}


	}


}
