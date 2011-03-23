using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Schema;



namespace Epsitec.Cresus.DataLayer.Tests.Vs.Context
{


	[TestClass]
	public sealed class UnitTestEntityCache
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void EntityCacheConstructorTest()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);
		}


		[TestMethod]
		public void AddTest()
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

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			foreach (AbstractEntity entity in entities)
			{
				entityCache.Add (entity);
			}

			foreach (AbstractEntity entity in entities)
			{
				Assert.IsTrue (entityCache.ContainsEntity (entity));
			}
		}


		[TestMethod]
		public void AddArgumentCheck()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => entityCache.Add (null)
			);
		}

		
		[TestMethod]
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

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

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

		
		[TestMethod]
		public void DefineRowKeyTest()
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

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			for (int i = 0; i < entities.Count; i++)
			{
				AbstractEntity entity = entities[i];
				DbKey dbKey = new DbKey (new DbId (i + 1));
				EntityKey entityKey = new EntityKey (entity, dbKey).GetNormalizedEntityKey (entityTypeEngine);

				entityCache.Add (entity);
				entityCache.DefineRowKey (entity, dbKey);
				
				Assert.IsTrue (entityCache.ContainsEntity (entity));
				Assert.AreEqual (entityKey, entityCache.GetEntityKey (entity));
			}
		}


		[TestMethod]
		public void DefineRowKeyArgumentCheck()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);
			NaturalPersonEntity entity = EntityContext.Current.CreateEntity<NaturalPersonEntity> ();

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => entityCache.DefineRowKey (null, new DbKey (new DbId (1)))
			);
		
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => entityCache.DefineRowKey (entity, new DbKey (new DbId (1)))
			);

			entityCache.Add (entity);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => entityCache.DefineRowKey (entity, DbKey.Empty)
			);
		}

		
		[TestMethod]
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

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			foreach (AbstractEntity entity in entities1)
			{
				entityCache.Add (entity);
			}

			List<AbstractEntity> entities2 = entityCache.GetEntities ().ToList ();

			Assert.IsTrue (entities1.Except (entities2).Count () == 0);
			Assert.IsTrue (entities2.Except (entities1).Count () == 0);
		}

		
		[TestMethod]
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

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

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
		}

		
		[TestMethod]
		public void GetEntityKeyTest()
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

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

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
				EntityKey entityKey = new EntityKey (entity, dbKey).GetNormalizedEntityKey (entityTypeEngine);

				Assert.AreEqual (entityKey, entityCache.GetEntityKey (entity));
			}

			foreach (AbstractEntity entity in entities2)
			{
				Assert.IsNull (entityCache.GetEntityKey (entity));
			}
		}


		[TestMethod]
		public void GetEntityKeyArgumentCheck()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => entityCache.GetEntityKey (null)
			);
		}

		
		[TestMethod]
		public void RemoveTest()
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

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			foreach (AbstractEntity entity in entities)
			{
				entityCache.Add (entity);
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


		[TestMethod]
		public void RemoveArgumentCheck()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => entityCache.Remove (null)
			);
		}


		[TestMethod]
		public void DefineAndGetLogIdTest1()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			List<Druid> samples = Enumerable.Range (1, 10).Select (i => Druid.FromLong (i)).ToList ();

			for (int i = 0; i < 5; i++)
			{
				foreach (Druid sample in samples)
				{
					entityCache.DefineLogId (sample, sample.ToLong () + i);
				}

				foreach (Druid sample in samples)
				{
					Assert.AreEqual (sample.ToLong () + i, entityCache.GetLogId (sample));
				}
			}
		}


		[TestMethod]
		public void DefineLogIdArgumentCheck2()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			List<AbstractEntity> samples = new List<AbstractEntity> ()
			{
				new NaturalPersonEntity (),
				new LegalPersonEntity (),
				new UriContactEntity (),
				new TelecomContactEntity (),
				new MailContactEntity (),
				new LanguageEntity (),
			};

			foreach (AbstractEntity sample in samples)
			{
				entityCache.Add (sample);
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < samples.Count; j++)
				{
					entityCache.DefineLogId (samples[j], j + i);
				}

				for (int j = 0; j < samples.Count; j++)
				{
					Assert.AreEqual (j + i, entityCache.GetLogId (samples[j]));
				}
			}
		}


		[TestMethod]
		public void DefineAndGetLogIdTest2()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			entityCache.Add (new NaturalPersonEntity ());

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => entityCache.DefineLogId ((AbstractEntity) null, 0)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => entityCache.DefineLogId (new NaturalPersonEntity (), 0)
			);
		}


		[TestMethod]
		public void GetMinimumLogId()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			Assert.IsNull (entityCache.GetMinimumLogId ());

			for (int i = 0; i < 10; i++)
			{
				entityCache.DefineLogId (Druid.FromLong (i + 1), i);

				Assert.AreEqual (0, entityCache.GetMinimumLogId ());
			}
		}


		[TestMethod]
		public void GetEntityTypeIdsTest()
		{
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			EntityCache entityCache = new EntityCache (entityTypeEngine);

			List<Druid> samples = Enumerable.Range (1, 10).Select (i => Druid.FromLong (i)).ToList ();

			foreach (Druid sample in samples)
			{
				entityCache.DefineLogId (sample, 1);
			}

			Assert.IsTrue (samples.SetEquals (entityCache.GetEntityTypeIds ()));
		}


	}


}
