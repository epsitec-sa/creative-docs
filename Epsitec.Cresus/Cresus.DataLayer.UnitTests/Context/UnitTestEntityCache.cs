﻿using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;



namespace Epsitec.Cresus.DataLayer.UnitTests.Context
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
			EntityCache entityCache = new EntityCache ();
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

			EntityCache entityCache = new EntityCache ();

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
		public void AddTestArgumentCheck()
		{
			EntityCache entityCache = new EntityCache ();

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

			EntityCache entityCache = new EntityCache ();

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

			EntityCache entityCache = new EntityCache ();

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


		[TestMethod]
		public void DefineRowKeyArgumentCheck()
		{
			EntityCache entityCache = new EntityCache ();
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

			EntityCache entityCache = new EntityCache ();

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

			EntityCache entityCache = new EntityCache ();

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

			EntityCache entityCache = new EntityCache ();

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


		[TestMethod]
		public void GetEntityKeyArgumentCheck()
		{
			EntityCache entityCache = new EntityCache ();

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

			EntityCache entityCache = new EntityCache ();

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


		[TestMethod]
		public void RemoveTestArgumentCheck()
		{
			EntityCache entityCache = new EntityCache ();

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => entityCache.Remove (null)
			);
		}


	}


}
