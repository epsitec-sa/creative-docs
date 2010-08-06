using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestEntityData
	{

		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
