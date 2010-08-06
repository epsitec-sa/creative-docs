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

			ValueData_Accessor valueData = new ValueData_Accessor ();
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, referenceData, collectionData);
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

			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, null, referenceData, collectionData);
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

			ValueData_Accessor valueData = new ValueData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, null, collectionData);
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

			ValueData_Accessor valueData = new ValueData_Accessor ();
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, referenceData, null);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CollectionDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData_Accessor valueData = new ValueData_Accessor ();
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (collectionData.Target, entityData.CollectionData.Target);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void EntityKeyTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData_Accessor valueData = new ValueData_Accessor ();
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, referenceData, collectionData);

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

			ValueData_Accessor valueData = new ValueData_Accessor ();
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, referenceData, collectionData);

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

			ValueData_Accessor valueData = new ValueData_Accessor ();
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (referenceData.Target, entityData.ReferenceData.Target);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ValueDataTest()
		{
			Druid leafEntityId = Druid.FromLong (1);
			Druid loadedEntityId = Druid.FromLong (1);
			DbKey rowKey = new DbKey (new DbId (1));

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			ValueData_Accessor valueData = new ValueData_Accessor ();
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
			CollectionData_Accessor collectionData = new CollectionData_Accessor ();

			EntityData_Accessor entityData = new EntityData_Accessor (entityKey, loadedEntityId, valueData, referenceData, collectionData);

			Assert.AreSame (valueData.Target, entityData.ValueData.Target);
		}


	}


}
