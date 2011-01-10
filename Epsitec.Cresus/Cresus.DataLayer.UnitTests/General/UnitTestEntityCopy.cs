using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestEntityCopy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void CopyEntityArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity entity1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001000000001)));
					NaturalPersonEntity entity2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001000000001)));
					NaturalPersonEntity entity3 = dataContext1.CreateEntity<NaturalPersonEntity> ();
					
					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => DataContext.CopyEntity (null, entity1, dataContext2)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => DataContext.CopyEntity (dataContext1, entity1, null)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => DataContext.CopyEntity (dataContext1, (NaturalPersonEntity) null, dataContext2)
					);
					
					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => DataContext.CopyEntity (dataContext1, entity2, dataContext2)
					);
					
					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => DataContext.CopyEntity (dataContext1, entity3, dataContext2)
					);
				}
			}
		}


		[TestMethod]
		public void CopyToSelfTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001000000001)));

				NaturalPersonEntity copy = DataContext.CopyEntity (dataContext, entity, dataContext);

				Assert.AreSame (entity, copy);
			}
		}


		[TestMethod]
		public void SimlpeCopyTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					foreach (AbstractEntity entity in this.GetSampleEntities (dataContext1))
					{
						AbstractEntity copy = DataContext.CopyEntity (dataContext1, entity, dataContext2);

						this.CheckEntitiesAreSimilar (dataContext1, entity, dataContext2, copy);
					}
				}
			}
		}


		private void CheckEntitiesAreSimilar(DataContext dataContext1, AbstractEntity entity1, DataContext dataContext2, AbstractEntity entity2)
		{
			Assert.AreEqual (entity1.GetEntityStructuredTypeId (), entity2.GetEntityStructuredTypeId ());

			this.CheckValueFields (dataContext1, entity1, dataContext2, entity2);
			this.CheckReferenceFields (dataContext1, entity1, dataContext2, entity2);
			this.CheckCollectionFields (dataContext1, entity1, dataContext2, entity2);
		}


		private void CheckValueFields(DataContext dataContext1, AbstractEntity entity1, DataContext dataContext2, AbstractEntity entity2)
		{
			Druid leafEntityId = entity1.GetEntityStructuredTypeId ();

			var fieldIds = from field in dataContext1.EntityContext.GetEntityFieldDefinitions (leafEntityId)
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


		private void CheckReferenceFields(DataContext dataContext1, AbstractEntity entity1, DataContext dataContext2, AbstractEntity entity2)
		{
			Druid leafEntityId = entity1.GetEntityStructuredTypeId ();

			var fieldIds = from field in dataContext1.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Reference
						   where field.Source == FieldSource.Value
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				AbstractEntity value1 = entity1.GetField<AbstractEntity> (fieldId.ToResourceId ());
				AbstractEntity value2 = entity2.GetField<AbstractEntity> (fieldId.ToResourceId ());

				if (value1 == null || !dataContext1.IsPersistent (value1))
				{
					Assert.IsNull (value2);
				}
				else
				{
					EntityKey? key1 = dataContext1.GetNormalizedEntityKey (entity1);
					EntityKey? key2 = dataContext2.GetNormalizedEntityKey (entity2);

					Assert.AreEqual (key1, key2);
				}
			}
		}


		private void CheckCollectionFields(DataContext dataContext1, AbstractEntity entity1, DataContext dataContext2, AbstractEntity entity2)
		{
			Druid leafEntityId = entity1.GetEntityStructuredTypeId ();

			var fieldIds = from field in dataContext1.EntityContext.GetEntityFieldDefinitions (leafEntityId)
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
					.Where (t => dataContext1.IsPersistent (t))
					.Select (t => dataContext1.GetNormalizedEntityKey (t))
					.SequenceEqual
					(
						values2
					.Select (t => dataContext2.GetNormalizedEntityKey (t))
					)
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
