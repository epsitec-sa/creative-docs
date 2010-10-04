using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Serialization;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;



namespace Epsitec.Cresus.DataLayer.UnitTests.Serialization
{


	[TestClass]
	public sealed class UnitTestEntitySerializationManager
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			Assert.IsTrue (DatabaseHelper.DbInfrastructure.IsConnectionOpen);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void EntitySerializationManagerConstructorTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					new EntitySerializationManager (dataContext);
				}
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
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExceptionAssert.Throw<System.ArgumentNullException>
						(
					   () => new EntitySerializationManager (dataContext).Serialize (null)
						);
				}
			}
		}


		[TestMethod]
		public void DeserializeArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExceptionAssert.Throw<System.ArgumentNullException>
						(
					   () => new EntitySerializationManager (dataContext).Deserialize (null)
						);
				}
			}
		}


		[TestMethod]
		public void SimpleTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					EntitySerializationManager serializer = new EntitySerializationManager (dataContext);

					foreach (AbstractEntity entity in this.GetSampleEntities (dataContext))
					{
						EntityData serializedEntity = serializer.Serialize (entity);
						AbstractEntity deserializedEntity = serializer.Deserialize (serializedEntity);

						this.CheckEntitiesAreSimilar (dataContext, entity, deserializedEntity);
					}
				}
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

			var fieldIds = from field in dataContext.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.None
						   where field.Source == FieldSource.Value
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

			var fieldIds = from field in dataContext.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Reference
						   where field.Source == FieldSource.Value
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

			var fieldIds = from field in dataContext.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Collection
						   where field.Source == FieldSource.Value
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
			yield return dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
			yield return dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
			yield return dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));
			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2)));
			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (3)));
			yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4)));

			yield return dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));
			yield return dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (2)));

			yield return dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1)));
			yield return dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (2)));

			yield return dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));
			yield return dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));

			yield return dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1)));
		}


	}


}
