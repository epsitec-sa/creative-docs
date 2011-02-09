using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
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

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void ConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				DeletePersistenceJob job = new DeletePersistenceJob (entity);

				List<DbTable> tables1 = new List<DbTable> ()
				{
					dataInfrastructure.SchemaEngine.GetEntityTableDefinition (Druid.Parse ("[J1AB1]")),
					dataInfrastructure.SchemaEngine.GetEntityTableDefinition (Druid.Parse ("[J1AJ1]")),
					dataInfrastructure.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AC1]")),
					dataInfrastructure.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AD1]")),
					dataInfrastructure.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[J1AJ1]"), Druid.Parse ("[J1AK1]")),
					dataInfrastructure.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[J1AJ1]"), Druid.Parse ("[J1AN1]")),
					dataInfrastructure.SchemaEngine.GetRelationTableDefinition (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AR1]")),
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
                { Druid.Parse ("[J1AL1]"), "coucou" },
            };

			Druid localEntityId = Druid.Parse ("[J1AJ1]");

			PersistenceJobType jobType = PersistenceJobType.Insert;

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				ValuePersistenceJob job = new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, false, jobType);

				DbTable table = dataInfrastructure.SchemaEngine.GetEntityTableDefinition (localEntityId);
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

			Druid localEntityId = Druid.Parse ("[J1AJ1]");
			Druid fieldId = Druid.Parse ("[J1AK1]");

			PersistenceJobType jobType = PersistenceJobType.Insert;

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				ReferencePersistenceJob job = new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);

				DbTable table = dataInfrastructure.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
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

			Druid localEntityId = Druid.Parse ("[J1AB1]");
			Druid fieldId = Druid.Parse ("[J1AJ]");

			PersistenceJobType jobType = PersistenceJobType.Insert;

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobTableComputer computer = new PersistenceJobTableComputer (dataContext);

				CollectionPersistenceJob job = new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType);

				DbTable table = dataInfrastructure.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
				List<DbTable> tables = computer.GetAffectedTables (job).ToList ();

				Assert.IsTrue (tables.Count == 1);
				Assert.AreSame (table, tables.First ());
			}
		}


	}


}
