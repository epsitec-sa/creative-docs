using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

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
		public void EntityDataConstructorTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);
		}


		[TestMethod]
		public void EntityDataConstructorArgumentCheck()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new EntityData (entityKey, leafEntityId, loadedEntityId, null, referenceData, collectionData)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, null, collectionData)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, null)
			);
		}


		[TestMethod]
		public void CollectionDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (collectionData, entityData.CollectionData);
		}


		[TestMethod]
		public void EntityKeyTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreEqual (entityKey, entityData.EntityKey);
		}


		[TestMethod]
		public void LeafEntityIdTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreEqual (leafEntityId, entityData.LeafEntityId);
		}


		[TestMethod]
		public void LoadedEntityIdTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreEqual (loadedEntityId, entityData.LoadedEntityId);
		}


		[TestMethod]
		public void ReferenceDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (referenceData, entityData.ReferenceData);
		}


		[TestMethod]
		public void ValueDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (2);
			DbKey rowKey = new DbKey (new DbId (3));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData valueData = new ValueData ();
			ReferenceData referenceData = new ReferenceData ();
			CollectionData collectionData = new CollectionData ();

			EntityData entityData = new EntityData (entityKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (valueData, entityData.ValueData);
		}


	}


}
