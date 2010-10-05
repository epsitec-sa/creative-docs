using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests.Schema
{


    [TestClass]
	public sealed class UnitTestSchemaBuilder
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();
		}


		[TestMethod]
		public void SchemaBuilderConstructorTest()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			new SchemaBuilder (dbInfrastructure);
		}


		[TestMethod]
		public void SchemaBuilderConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SchemaBuilder ((DbInfrastructure) null)
			);
		}


		[TestMethod]
		public void RegisterAndCheckSchema1()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			SchemaBuilder builder = new SchemaBuilder (dbInfrastructure);

			Druid entityId = Druid.Parse ("[L0A62]");
			
			Assert.IsFalse (builder.CheckSchema (entityId));

			using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				builder.RegisterSchema (entityId);

				transaction.Commit ();
			}

			Assert.IsTrue (builder.CheckSchema (entityId));
		}


		[TestMethod]
		public void RegisterAndCheckSchema2()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			SchemaBuilder builder = new SchemaBuilder (dbInfrastructure);

			Druid entityId = Druid.Parse ("[L0A5]");
			
			Assert.IsFalse (builder.CheckSchema (entityId));

			using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				builder.RegisterSchema (entityId);

				transaction.Commit ();
			}

			Assert.IsTrue (builder.CheckSchema (entityId));
		}


		[TestMethod]
		public void RegisterAndCheckSchema3()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			SchemaBuilder builder = new SchemaBuilder (dbInfrastructure);

			Druid entityId = Druid.Parse ("[L0AQ]");

			Assert.IsFalse (builder.CheckSchema (entityId));

			using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				builder.RegisterSchema (entityId);

				transaction.Commit ();
			}

			Assert.IsTrue (builder.CheckSchema (entityId));
		}


		[TestMethod]
		public void RegisterSchema1()
		{
			Druid entityId = Druid.Parse ("[L0AQ]");

			List<Druid> entityIds = new List<Druid> ()
			{
				Druid.Parse ("[L0AQ]"),
				Druid.Parse ("[L0AP]"),
				Druid.Parse ("[L0AE1]"),
				Druid.Parse ("[L0AQ1]"),
				Druid.Parse ("[L0AN]"),
				Druid.Parse ("[L0AM]"),
				Druid.Parse ("[L0AO]"),
				Druid.Parse ("[L0AL1]"),
				Druid.Parse ("[L0AT]"),
				Druid.Parse ("[L0AA1]"),
				Druid.Parse ("[L0A21]"),
				Druid.Parse ("[L0AD]"),
				Druid.Parse ("[L0AI]"),
				Druid.Parse ("[L0AF]"),
				Druid.Parse ("[L0A4]"),
				Druid.Parse ("[L0A5]"),
				Druid.Parse ("[L0A1]"),
			};

			foreach (Druid id in entityIds)
			{
				Assert.IsNull (DatabaseHelper.DbInfrastructure.ResolveDbTable (id));
			}

			SchemaBuilder schemaBuilder = new SchemaBuilder (DatabaseHelper.DbInfrastructure);

			schemaBuilder.RegisterSchema (entityId);

			foreach (Druid id in entityIds)
			{
				Assert.IsNotNull (DatabaseHelper.DbInfrastructure.ResolveDbTable (id));
			}

			foreach (Druid id in entityIds)
			{
				Assert.IsTrue (schemaBuilder.CheckSchema (id));
			}
		}


		[TestMethod]
		public void CreateSchemaArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			var builder = new SchemaBuilder (dbInfrastructure);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => builder.RegisterSchema (Druid.Empty)
			);
		}


		[TestMethod]
		public void CheckSchemaArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			var builder = new SchemaBuilder (dbInfrastructure);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => builder.CheckSchema (Druid.Empty)
			);
		}


	}


}
