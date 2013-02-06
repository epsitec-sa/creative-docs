using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;



namespace Epsitec.Cresus.DataLayer.Tests.Vs.Serialization
{


	[TestClass]
	public sealed class UnitTestEntitySerializationManager
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void EntitySerializationManagerConstructorTest()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				new EntitySerializationManager (dataContext);
			}
		}


		[TestMethod]
		public void EntitySerializationManagerConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new EntitySerializationManager (null)
			);
		}


		[TestMethod]
		public void SerializeArgumentCheck()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
				   () => new EntitySerializationManager (dataContext).Serialize (null, 0)
				);
			}
		}


		[TestMethod]
		public void DeserializeArgumentCheck()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
				   () => new EntitySerializationManager (dataContext).Deserialize (null)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
				   () => new EntitySerializationManager (dataContext).Deserialize (null, new EntityData (new DbKey (new DbId (1)), Druid.FromLong (1), Druid.FromLong (1), 0, new ValueData (), new ReferenceData (), new CollectionData ()))
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntitySerializationManager (dataContext).Deserialize (new NaturalPersonEntity (), null)
				);
			}
		}


		[TestMethod]
		public void SerializeAndDeserialize()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				EntitySerializationManager serializer = new EntitySerializationManager (dataContext);

				foreach (AbstractEntity entity in this.GetSampleEntities (dataContext))
				{
					EntityData serializedEntity = serializer.Serialize (entity, 0);
					AbstractEntity deserializedEntity = serializer.Deserialize (serializedEntity);

					this.CheckEntitiesAreSimilar (dataContext, entity, deserializedEntity);
				}
			}
		}


		[TestMethod]
		public void Deserialize()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));

				ValueData valueData = new ValueData ();
				valueData[Druid.Parse ("[J1AL1]")] = "Albert";
				valueData[Druid.Parse ("[J1AO1]")] = new Date (1995, 1, 1);

				ReferenceData referenceData = new ReferenceData ();
				referenceData[Druid.Parse ("[J1AD1]")] = new DbKey (new DbId (1000000002));
				referenceData[Druid.Parse ("[J1AK1]")] = new DbKey (new DbId (1000000001));

				CollectionData collectionData = new CollectionData ();
				collectionData[Druid.Parse ("[J1AC1]")].Add (new DbKey (new DbId (1000000002)));
				collectionData[Druid.Parse ("[J1AC1]")].Add (new DbKey (new DbId (1000000001)));

				int logSequenceId = 4;
				EntityData data = new EntityData (new DbKey (new DbId (1000000001)), Druid.Parse ("[J1AJ1]"), Druid.Parse ("[J1AJ1]"), logSequenceId, valueData, referenceData, collectionData);

				new EntitySerializationManager (dataContext).Deserialize (alfred, data);

				Assert.AreEqual ("Albert", alfred.Firstname);
				Assert.AreEqual ("Dupond", alfred.Lastname);
				Assert.AreEqual (new Date (1995, 1, 1), alfred.BirthDate);
				Assert.AreEqual (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002))), alfred.PreferredLanguage);
				Assert.AreEqual (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001))), alfred.Title);
				Assert.IsNull (alfred.Gender);
				Assert.AreEqual (2, alfred.Contacts.Count);
				Assert.AreEqual (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))), alfred.Contacts[0]);
				Assert.AreEqual (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))), alfred.Contacts[1]);
			}
		}


		private void CheckEntitiesAreSimilar(DataContext dataContext, AbstractEntity entity1, AbstractEntity entity2)
		{
			Assert.AreEqual (entity1.GetEntityStructuredTypeId (), entity2.GetEntityStructuredTypeId ());

			this.CheckValueFields (dataContext, entity1, entity2);
			this.CheckReferenceFields (dataContext, entity1, entity2);
			this.CheckCollectionFields (dataContext, entity1, entity2);
		}


		private void CheckValueFields(DataContext dataContext, AbstractEntity entity1, AbstractEntity entity2)
		{
			Druid leafEntityId = entity1.GetEntityStructuredTypeId();

			var fieldIds = from field in dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetValueFields (leafEntityId)
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				object value1 = entity1.GetField<object> (fieldId.ToResourceId ());
				object value2 = entity2.GetField<object> (fieldId.ToResourceId ());

				Assert.AreEqual (value1, value2);
			}
		}


		private void CheckReferenceFields(DataContext dataContext,  AbstractEntity entity1, AbstractEntity entity2)
		{
			Druid leafEntityId = entity1.GetEntityStructuredTypeId ();

			var fieldIds = from field in dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetReferenceFields (leafEntityId)
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				AbstractEntity value1 = entity1.GetField<AbstractEntity> (fieldId.ToResourceId ());
				AbstractEntity value2 = entity2.GetField<AbstractEntity> (fieldId.ToResourceId ());

				if (value1 == null || !dataContext.IsPersistent (value1))
				{
					Assert.IsNull (value2);
				}
				else
				{
					Assert.AreEqual (value1, value2);
				}
			}
		}


		private void CheckCollectionFields(DataContext dataContext, AbstractEntity entity1, AbstractEntity entity2)
		{
			Druid leafEntityId = entity1.GetEntityStructuredTypeId ();

			var fieldIds = from field in dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetCollectionFields (leafEntityId)
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				IList<AbstractEntity> values1 = entity1.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ());
				IList<AbstractEntity> values2 = entity2.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ());

				Assert.IsTrue
				(
					values1
					.Where (t => t!= null)
					.Where (t => dataContext.IsPersistent (t))
					.SequenceEqual (values2)
				);

			}
		}


		private IEnumerable<AbstractEntity> GetSampleEntities(DataContext dataContext)
		{
			yield return dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
			yield return dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));
			yield return dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));

			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));
			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000003)));
			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));

			yield return dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
			yield return dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002)));

			yield return dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001)));
			yield return dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000002)));

			yield return dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)));
			yield return dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002)));

			yield return dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));
		}


	}


}
