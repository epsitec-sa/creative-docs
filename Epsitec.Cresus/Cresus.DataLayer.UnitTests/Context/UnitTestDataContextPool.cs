using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Context
{


	[TestClass]
	public sealed class UnitTestDataContextPool
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void DataContextPoolConstructorTest()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();
		}


		[TestMethod]
		public void AddTest()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			List<DataContext> dataContexts = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			foreach (DataContext dataContext in dataContexts)
			{
				Assert.IsTrue (dataContextPool.Add (dataContext));
			}

			foreach (DataContext dataContext in dataContexts)
			{
				Assert.IsTrue (dataContextPool.Contains (dataContext));
			}

			foreach (DataContext dataContext in dataContexts)
			{
				Assert.IsFalse (dataContextPool.Add (dataContext));
			}

			foreach (DataContext dataContext in dataContexts)
			{
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void AddTestArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DataContextPool_Accessor ().Add (null)
			);
		}


		[TestMethod]
		public void ContainsTest()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			List<DataContext> dataContexts1 = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			List<DataContext> dataContexts2 = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			foreach (DataContext dataContext in dataContexts1)
			{
				dataContextPool.Add (dataContext);
			}

			foreach (DataContext dataContext in dataContexts1)
			{
				Assert.IsTrue (dataContextPool.Contains (dataContext));
			}

			foreach (DataContext dataContext in dataContexts2)
			{
				Assert.IsFalse (dataContextPool.Contains (dataContext));
			}

			foreach (DataContext dataContext in dataContexts1)
			{
				dataContext.Dispose ();
			}

			foreach (DataContext dataContext in dataContexts2)
			{
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void ContainsTestArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DataContextPool_Accessor ().Contains (null)
			);
		}


		[TestMethod]
		public void GetEnumeratorTest1()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			List<DataContext> dataContexts1 = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			foreach (DataContext dataContext in dataContexts1)
			{
				dataContextPool.Add (dataContext);
			}

			List<DataContext> dataContexts2 = new List<DataContext> ();

			foreach (DataContext dataContext in dataContextPool)
			{
				dataContexts2.Add (dataContext);
			}

			Assert.IsTrue (dataContexts1.Except (dataContexts2).Count () == 0);
			Assert.IsTrue (dataContexts2.Except (dataContexts1).Count () == 0);

			foreach (DataContext dataContext in dataContexts1)
			{
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetEnumeratorTest2()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			List<DataContext> dataContexts1 = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			foreach (DataContext dataContext in dataContexts1)
			{
				dataContextPool.Add (dataContext);
			}

			List<DataContext> dataContexts2 = new List<DataContext> ();

			foreach (DataContext dataContext in (dataContextPool as IEnumerable))
			{
				dataContexts2.Add (dataContext);
			}

			Assert.IsTrue (dataContexts1.Except (dataContexts2).Count () == 0);
			Assert.IsTrue (dataContexts2.Except (dataContexts1).Count () == 0);

			foreach (DataContext dataContext in dataContexts1)
			{
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void FindDataContextTest2()
		{
			DataContext dataContext = new DataContextPool_Accessor ().FindDataContext (null);

			Assert.IsNull (dataContext);
		}


		[TestMethod]
		public void FindDataContextTest1()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			List<DataContext> dataContexts = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			DataContext dataContext11 = new DataContext (DatabaseHelper.DbInfrastructure);

			foreach (DataContext dataContext in dataContexts)
			{
				dataContextPool.Add (dataContext);
			}

			NaturalPersonEntity person1 = dataContexts[1].ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
			NaturalPersonEntity person2 = dataContexts[2].ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
			NaturalPersonEntity person3 = dataContext11.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

			Assert.AreSame (dataContexts[1], dataContextPool.FindDataContext (person1));
			Assert.AreSame (dataContexts[2], dataContextPool.FindDataContext (person2));
			Assert.IsNull (dataContextPool.FindDataContext (person3));

			foreach (DataContext dataContext in dataContexts)
			{
				dataContext.Dispose ();
			}

			dataContext11.Dispose ();
		}

		
		[TestMethod]
		public void FindEntityKeyTest()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			List<DataContext> dataContexts = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			DataContext dataContext11 = new DataContext (DatabaseHelper.DbInfrastructure);

			foreach (DataContext dataContext in dataContexts)
			{
				dataContextPool.Add (dataContext);
			}

			DbKey dbKey1 = new DbKey (new DbId (1));
			DbKey dbKey2 = new DbKey (new DbId (2));
			DbKey dbKey3 = new DbKey (new DbId (3));
			
			NaturalPersonEntity person1 = dataContexts[1].ResolveEntity<NaturalPersonEntity> (dbKey1);
			NaturalPersonEntity person2 = dataContexts[2].ResolveEntity<NaturalPersonEntity> (dbKey2);
			NaturalPersonEntity person3 = dataContext11.ResolveEntity<NaturalPersonEntity> (dbKey3);

			Assert.AreEqual (new EntityKey (person1, dbKey1), dataContextPool.FindEntityKey (person1));
			Assert.AreEqual (new EntityKey (person2, dbKey2), dataContextPool.FindEntityKey (person2));
			Assert.IsNull (dataContextPool.FindEntityKey (person3));

			foreach (DataContext dataContext in dataContexts)
			{
				dataContext.Dispose ();
			}

			dataContext11.Dispose ();
		}

		
		[TestMethod]
		public void RemoveTest()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			List<DataContext> dataContexts = new List<DataContext> ()
			{
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
				new DataContext (DatabaseHelper.DbInfrastructure),
			};

			foreach (DataContext dataContext in dataContexts)
			{
				dataContextPool.Add (dataContext);
			}

			foreach (DataContext dataContext in dataContexts)
			{
				Assert.IsTrue (dataContextPool.Contains (dataContext));
			}

			foreach (DataContext dataContext in dataContexts)
			{
				Assert.IsTrue (dataContextPool.Remove (dataContext));
			}

			foreach (DataContext dataContext in dataContexts)
			{
				Assert.IsFalse (dataContextPool.Contains (dataContext));
			}

			foreach (DataContext dataContext in dataContexts)
			{
				Assert.IsFalse (dataContextPool.Remove (dataContext));
			}

			foreach (DataContext dataContext in dataContexts)
			{
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void RemoveTestArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DataContextPool_Accessor ().Remove (null)

			);
		}

		
		[TestMethod]
		public void InstanceTest()
		{
			DataContextPool dataContextPool1 = DataContextPool.Instance;
			
			for (int i = 0; i < 10; i++)
			{
				DataContextPool dataContextPool2 = DataContextPool.Instance;

				Assert.AreSame (dataContextPool1, dataContextPool2);
			}
		}


		[TestMethod]
		public void AreEqualDatabaseInstancesTest1()
		{
			DataContextPool_Accessor dataContextPool = new DataContextPool_Accessor ();

			DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure);
			DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure);
			DataContext dataContext3 = new DataContext (DatabaseHelper.DbInfrastructure);

			dataContextPool.Add (dataContext1);
			dataContextPool.Add (dataContext2);

			DbKey dbKey1 = new DbKey (new DbId (1));
			DbKey dbKey2 = new DbKey (new DbId (2));

			NaturalPersonEntity person11 = dataContext1.ResolveEntity<NaturalPersonEntity> (dbKey1);
			NaturalPersonEntity person12 = dataContext1.ResolveEntity<NaturalPersonEntity> (dbKey2);
			NaturalPersonEntity person21 = dataContext2.ResolveEntity<NaturalPersonEntity> (dbKey1);
			NaturalPersonEntity person22 = dataContext2.ResolveEntity<NaturalPersonEntity> (dbKey2);
			NaturalPersonEntity person31 = dataContext3.ResolveEntity<NaturalPersonEntity> (dbKey1);
			NaturalPersonEntity person32 = dataContext3.ResolveEntity<NaturalPersonEntity> (dbKey2);

			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person11, person11));
			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person12, person12));

			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person21, person21));
			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person22, person22));

			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person11, person21));
			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person12, person22));

			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person11, person12));
			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person21, person22));

			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person11, person22));
			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person12, person21));

			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person21, person12));
			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person22, person11));

			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (null, null));

			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (null, person11));
			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person11, null));

			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person31, person31));
			Assert.IsTrue (dataContextPool.AreEqualDatabaseInstances (person32, person32));

			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person31, person32));
			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person11, person31));
			Assert.IsFalse (dataContextPool.AreEqualDatabaseInstances (person12, person32));
			
			dataContextPool.Remove (dataContext1);
			dataContextPool.Remove (dataContext2);

			dataContext1.Dispose ();
			dataContext2.Dispose ();
			dataContext3.Dispose ();
		}


	}


}
