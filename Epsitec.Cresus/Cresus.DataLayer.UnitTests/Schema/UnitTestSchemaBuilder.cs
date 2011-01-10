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
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void SchemaBuilderConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				new SchemaBuilder (dbInfrastructure);
			}
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
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
		}


		[TestMethod]
		public void RegisterAndCheckSchema2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
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
		}


		[TestMethod]
		public void RegisterAndCheckSchema3()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
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
		}


		[TestMethod]
		public void RegisterSchema1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
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
					Assert.IsNull (dbInfrastructure.ResolveDbTable (id));
				}

				SchemaBuilder schemaBuilder = new SchemaBuilder (dbInfrastructure);

				schemaBuilder.RegisterSchema (entityId);

				foreach (Druid id in entityIds)
				{
					Assert.IsNotNull (dbInfrastructure.ResolveDbTable (id));
				}

				foreach (Druid id in entityIds)
				{
					Assert.IsTrue (schemaBuilder.CheckSchema (id));
				}
			}
		}


		[TestMethod]
		public void CreateSchemaArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var builder = new SchemaBuilder (dbInfrastructure);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => builder.RegisterSchema (Druid.Empty)
				);
			}
		}


		[TestMethod]
		public void CheckSchemaArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var builder = new SchemaBuilder (dbInfrastructure);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => builder.CheckSchema (Druid.Empty)
				);
			}
		}


	}


}
