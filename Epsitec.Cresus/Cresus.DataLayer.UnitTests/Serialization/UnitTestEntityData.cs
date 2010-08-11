using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Serialization
{


	[TestClass]
	public sealed class UnitTestEntityData
	{

		[TestMethod]
		public void EntityDataConstructorTest1()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, referenceData, collectionData);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void EntityDataConstructorTest2()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, null, referenceData, collectionData);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void EntityDataConstructorTest3()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, null, collectionData);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void EntityDataConstructorTest4()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, referenceData, null);
		}


		[TestMethod]
		public void CollectionDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (collectionData, entityData.CollectionData);
		}


		[TestMethod]
		public void EntityKeyTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreEqual (entityKey, entityData.EntityKey);
		}


		[TestMethod]
		public void LoadedEntityIdTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreEqual (loadedEntityId, entityData.LoadedEntityId);
		}


		[TestMethod]
		public void ReferenceDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (referenceData, entityData.ReferenceData);
		}


		[TestMethod]
		public void ValueDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (valueData, entityData.ValueData);
		}


	}


}
