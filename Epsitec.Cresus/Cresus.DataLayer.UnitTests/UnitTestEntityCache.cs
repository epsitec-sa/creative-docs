using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass ()]
	public class UnitTestEntityCache
	{


		private TestContext testContextInstance;

		
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}

		
		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void EntityCacheConstructorTest()
		{
			EntityCache_Accessor entityCache = new EntityCache_Accessor ();
		}


		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void AddTest1()
		{
			List<AbstractEntity> entities = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			foreach (AbstractEntity entity in entities)
			{
				entityCache.Add (entity);
			}

			foreach (AbstractEntity entity in entities)
			{
				Assert.IsTrue (entityCache.ContainsEntity (entity));
			}
		}


		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void AddTest2()
		{
			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			entityCache.Add (null);
		}

		
		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ContainsEntityTest()
		{
			List<AbstractEntity> entities1 = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			List<AbstractEntity> entities2 = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			foreach (AbstractEntity entity in entities1)
			{
				entityCache.Add (entity);
			}

			foreach (AbstractEntity entity in entities1)
			{
				Assert.IsTrue (entityCache.ContainsEntity (entity));
			}

			foreach (AbstractEntity entity in entities2)
			{
				Assert.IsFalse (entityCache.ContainsEntity (entity));
			}
		}

		
		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void DefineRowKeyTest1()
		{
			List<AbstractEntity> entities = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			for (int i = 0; i < entities.Count; i++)
			{
				AbstractEntity entity = entities[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));
				EntityKey entityKey = new EntityKey (entity, dbKey);

				entityCache.Add (entity);
				entityCache.DefineRowKey (entity, dbKey);
				
				Assert.IsTrue (entityCache.ContainsEntity (entity));
				Assert.AreEqual (entityKey, entityCache.GetEntityKey (entity));
			}
		}


		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void DefineRowKeyTest2()
		{
			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			entityCache.DefineRowKey (null, new DbKey (new DbId (1)));

		}


		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void DefineRowKeyTest3()
		{
			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			entityCache.DefineRowKey (new NaturalPersonEntity (), new DbKey (new DbId (0)));
		}


		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void DefineRowKeyTest4()
		{
			NaturalPersonEntity entity = EntityContext.Current.CreateEntity<NaturalPersonEntity> ();
			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			entityCache.DefineRowKey (entity, new DbKey (new DbId (1)));
		}

		
		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetEntitiesTest()
		{
			List<AbstractEntity> entities1 = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			foreach (AbstractEntity entity in entities1)
			{
				entityCache.Add (entity);
			}

			List<AbstractEntity> entities2 = entityCache.GetEntities ().ToList ();

			Assert.IsTrue (entities1.Except (entities2).Count () == 0);
			Assert.IsTrue (entities2.Except (entities1).Count () == 0);
		}

		
		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetEntityTest()
		{
			List<AbstractEntity> entities = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			for (int i = 0; i < entities.Count; i++)
			{
				AbstractEntity entity = entities[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));

				entityCache.Add (entity);
				entityCache.DefineRowKey (entity, dbKey);
			}

			for (int i = 0; i < entities.Count; i++)
			{
				AbstractEntity entity = entities[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));
				EntityKey entityKey = new EntityKey (entity, dbKey);

				Assert.AreSame (entity, entityCache.GetEntity (entityKey));
			}

			Assert.IsNull (entityCache.GetEntity (new EntityKey ()));
			Assert.IsNull (entityCache.GetEntity (new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)))));
		}

		
		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetEntityKeyTest1()
		{
			List<AbstractEntity> entities1 = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			List<AbstractEntity> entities2 = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			for (int i = 0; i < entities1.Count; i++)
			{
				AbstractEntity entity = entities1[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));

				entityCache.Add (entity);
				entityCache.DefineRowKey (entity, dbKey);
			}

			for (int i = 0; i < entities1.Count; i++)
			{
				AbstractEntity entity = entities1[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));
				EntityKey entityKey = new EntityKey (entity, dbKey);

				Assert.AreEqual (entityKey, entityCache.GetEntityKey (entity));
			}

			foreach (AbstractEntity entity in entities2)
			{
				Assert.IsNull (entityCache.GetEntityKey (entity));
			}
		}


		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetEntityKeyTest2()
		{
			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			entityCache.GetEntityKey (null);
		}

		
		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void RemoveTest1()
		{
			List<AbstractEntity> entities = new List<AbstractEntity> ()
			{
				EntityContext.Current.CreateEntity<NaturalPersonEntity> (),
				EntityContext.Current.CreateEntity<LegalPersonEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<UriContactEntity> (),
				EntityContext.Current.CreateEntity<AbstractContactEntity> (),
				EntityContext.Current.CreateEntity<CountryEntity> (),
				EntityContext.Current.CreateEntity<LocationEntity> (),
			};

			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			foreach (AbstractEntity entity in entities)
            {
            	entityCache.Add(entity);
            }

			foreach (AbstractEntity entity in entities)
			{
				Assert.IsTrue (entityCache.ContainsEntity (entity));
			}

			foreach (AbstractEntity entity in entities)
			{
				entityCache.Remove (entity);
			}

			foreach (AbstractEntity entity in entities)
			{
				Assert.IsFalse (entityCache.ContainsEntity (entity));
			}

			for (int i = 0; i < entities.Count; i++)
			{
				AbstractEntity entity = entities[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));

				entityCache.Add (entity);
				entityCache.DefineRowKey (entity, dbKey);
			}

			for (int i = 0; i < entities.Count; i++)
			{
				AbstractEntity entity = entities[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));
				EntityKey entityKey = new EntityKey (entity, dbKey);

				Assert.IsTrue (entityCache.ContainsEntity (entity));
				Assert.IsNotNull (entityCache.GetEntityKey (entity));
				Assert.IsNotNull (entityCache.GetEntity (entityKey));
			}

			foreach (AbstractEntity entity in entities)
			{
				entityCache.Remove (entity);
			}

			for (int i = 0; i < entities.Count; i++)
			{
				AbstractEntity entity = entities[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));
				EntityKey entityKey = new EntityKey (entity, dbKey);

				Assert.IsFalse (entityCache.ContainsEntity (entity));
				Assert.IsNull (entityCache.GetEntityKey (entity));
				Assert.IsNull (entityCache.GetEntity (entityKey));
			}
		}


		[TestMethod ()]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void RemoveTest2()
		{
			EntityCache_Accessor entityCache = new EntityCache_Accessor ();

			entityCache.Remove (null);
		}


	}


}
