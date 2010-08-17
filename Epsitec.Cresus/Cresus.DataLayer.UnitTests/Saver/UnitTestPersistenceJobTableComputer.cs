using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver
{


	[TestClass]
	public class UnitTestPersistenceJobTableComputer
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void ConstructorTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				new PersistenceJobTableComputer (dataContext);
			}
		}


		[TestMethod]
		public void ConstructorTestArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new PersistenceJobTableComputer (null)
			);
		}


		[TestMethod]
		public void GetAffectedTablesArgumentCheck()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => computer.GetAffectedTables ((DeletePersistenceJob) null)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => computer.GetAffectedTables ((ValuePersistenceJob) null)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
				  () => computer.GetAffectedTables ((ReferencePersistenceJob) null)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => computer.GetAffectedTables ((CollectionPersistenceJob) null)
				);
			}
		}


		[TestMethod]
		public void GetAffectedTablesDeletePersistenceJob()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				DeletePersistenceJob job = new DeletePersistenceJob (entity);

				List<DbTable> tables1 = new List<DbTable> ()
                {
                    dataContext.SchemaEngine.GetEntityTableDefinition (Druid.Parse ("[L0AM]")),
                    dataContext.SchemaEngine.GetEntityTableDefinition (Druid.Parse ("[L0AN]")),
                    dataContext.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[L0AM]"), Druid.Parse ("[L0AS]")),
                    dataContext.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[L0AM]"), Druid.Parse ("[L0AD1]")),
                    dataContext.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[L0AN]"), Druid.Parse ("[L0AU]")),
                    dataContext.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[L0AN]"), Druid.Parse ("[L0A11]")),
                    dataContext.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0A71]")),
                };

				List<DbTable> tables2 = computer.GetAffectedTables (job).ToList ();

				CollectionAssert.IsSubsetOf (tables1, tables2);
				CollectionAssert.IsSubsetOf (tables2, tables1);
			}
		}


		[TestMethod]
		public void GetAffectedTablesValuePersistenceJob()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ()
            {
                { Druid.Parse ("[L0AV]"), "coucou" },
            };

			Druid localEntityId = Druid.Parse ("[L0AN]");

			PersistenceJobType jobType = PersistenceJobType.Insert;

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				ValuePersistenceJob job = new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, false, jobType);

				DbTable table = dataContext.SchemaEngine.GetEntityTableDefinition (localEntityId);
				List<DbTable> tables = computer.GetAffectedTables (job).ToList ();

				Assert.IsTrue (tables.Count == 1);
				Assert.AreSame (table, tables.First ());
			}
		}


		[TestMethod]
		public void GetAffectedTablesReferencePersistenceJob()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			PersonTitleEntity target = new PersonTitleEntity ();

			Druid localEntityId = Druid.Parse ("[L0AN]");
			Druid fieldId = Druid.Parse ("[L0AU]");

			PersistenceJobType jobType = PersistenceJobType.Insert;

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				ReferencePersistenceJob job = new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);

				DbTable table = dataContext.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
				List<DbTable> tables = computer.GetAffectedTables (job).ToList ();

				Assert.IsTrue (tables.Count == 1);
				Assert.AreSame (table, tables.First ());
			}
		}


		[TestMethod]
		public void GetAffectedTablesCollectionPersistenceJob()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();

			var targets = new List<AbstractEntity> ()
            {
                new AbstractContactEntity (),
                new AbstractContactEntity (),
            };

			Druid localEntityId = Druid.Parse ("[L0AM]");
			Druid fieldId = Druid.Parse ("[L0AD]");

			PersistenceJobType jobType = PersistenceJobType.Insert;

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				CollectionPersistenceJob job = new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType);

				DbTable table = dataContext.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
				List<DbTable> tables = computer.GetAffectedTables (job).ToList ();

				Assert.IsTrue (tables.Count == 1);
				Assert.AreSame (table, tables.First ());
			}
		}


	}


}
