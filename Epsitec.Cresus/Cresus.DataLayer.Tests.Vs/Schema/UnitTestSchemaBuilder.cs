using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
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
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

				new SchemaBuilder (entityTypeEngine, dbInfrastructure);
			}
		}


		[TestMethod]
		public void SchemaBuilderConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new SchemaBuilder (null, dbInfrastructure)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new SchemaBuilder (entityTypeEngine, null)
				);
			}
		}


		[TestMethod]
		public void RegisterAndCheckSchema1()
		{
			List<Druid> entityIds = new List<Druid> ()
			{
				Druid.Parse ("[J1A81]")
			};

			this.CheckSchema (entityIds, false);
			this.RegisterSchema (entityIds);
			this.CheckSchema (entityIds, true);
		}


		[TestMethod]
		public void RegisterAndCheckSchema2()
		{
			List<Druid> entityIds = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]"),
				Druid.Parse ("[J1AA1]"),
				Druid.Parse ("[J1AV]"),
				Druid.Parse ("[J1A41]"),
				Druid.Parse ("[J1AJ1]"),
				Druid.Parse ("[J1AB1]"),
				Druid.Parse ("[J1AE1]"),
				Druid.Parse ("[J1A11]"),
				Druid.Parse ("[J1AN]"),
				Druid.Parse ("[J1AQ]"),
				Druid.Parse ("[J1AT]"),
				Druid.Parse ("[J1AJ]"),
				Druid.Parse ("[J1AG]"),
				Druid.Parse ("[J1AE]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A4]"),
			};

			this.CheckSchema (entityIds, false);
			this.RegisterSchema (entityIds);
			this.CheckSchema (entityIds, true);
		}


		[TestMethod]
		public void RegisterAndCheckSchema5()
		{
			List<Druid> entityIds1 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A4]"),
			};

			List<Druid> entityIds2 = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]"),
				Druid.Parse ("[J1AA1]"),
				Druid.Parse ("[J1AV]"),
				Druid.Parse ("[J1A41]"),
				Druid.Parse ("[J1AJ1]"),
				Druid.Parse ("[J1AB1]"),
				Druid.Parse ("[J1AE1]"),
				Druid.Parse ("[J1A11]"),
				Druid.Parse ("[J1AN]"),
				Druid.Parse ("[J1AQ]"),
				Druid.Parse ("[J1AT]"),
				Druid.Parse ("[J1AJ]"),
				Druid.Parse ("[J1AG]"),
				Druid.Parse ("[J1AE]"),
			};

			this.CheckSchema (entityIds1, false);
			this.CheckSchema (entityIds2, false);

			this.RegisterSchema (entityIds1);

			this.CheckSchema (entityIds1, true);
			this.CheckSchema (entityIds2, false);

			this.RegisterSchema (entityIds2);

			this.CheckSchema (entityIds1, true);
			this.CheckSchema (entityIds2, true);
		}


		[TestMethod]
		public void UpdateAndCheckSchema1()
		{
			List<Druid> entityIds1 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A4]"),
			};

			List<Druid> entityIds2 = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]"),
				Druid.Parse ("[J1AA1]"),
				Druid.Parse ("[J1AV]"),
				Druid.Parse ("[J1A41]"),
				Druid.Parse ("[J1AJ1]"),
				Druid.Parse ("[J1AB1]"),
				Druid.Parse ("[J1AE1]"),
				Druid.Parse ("[J1A11]"),
				Druid.Parse ("[J1AN]"),
				Druid.Parse ("[J1AQ]"),
				Druid.Parse ("[J1AT]"),
				Druid.Parse ("[J1AJ]"),
				Druid.Parse ("[J1AG]"),
				Druid.Parse ("[J1AE]"),
			};

			this.CheckSchema (entityIds1, false);
			this.CheckSchema (entityIds2, false);

			this.UpdateSchema (entityIds1);

			this.CheckSchema (entityIds1, true);
			this.CheckSchema (entityIds2, false);

			this.UpdateSchema (entityIds2.Concat (entityIds1).ToList ());

			this.CheckSchema (entityIds1, true);
			this.CheckSchema (entityIds2, true);
		}


		[TestMethod]
		public void UpdateAndCheckSchema2()
		{
			List<Druid> entityIds1 = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]"),
				Druid.Parse ("[J1AA1]"),
				Druid.Parse ("[J1AV]"),
				Druid.Parse ("[J1A41]"),
				Druid.Parse ("[J1AJ1]"),
				Druid.Parse ("[J1AB1]"),
				Druid.Parse ("[J1AE1]"),
				Druid.Parse ("[J1A11]"),
				Druid.Parse ("[J1AN]"),
				Druid.Parse ("[J1AQ]"),
				Druid.Parse ("[J1AT]"),
				Druid.Parse ("[J1AJ]"),
				Druid.Parse ("[J1AG]"),
				Druid.Parse ("[J1AE]"),
			};

			List<Druid> entityIds2 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A4]"),
			};

			this.CheckSchema (entityIds1, false);
			this.CheckSchema (entityIds2, false);

			this.UpdateSchema (entityIds1.Concat (entityIds2).ToList ());

			this.CheckSchema (entityIds1, true);
			this.CheckSchema (entityIds2, true);

			this.UpdateSchema (entityIds2);

			this.CheckSchema (entityIds1, false);
			this.CheckSchema (entityIds2, true);
		}


		[TestMethod]
		public void CreateSchemaArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				
				var builder = new SchemaBuilder (entityTypeEngine, dbInfrastructure);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => builder.RegisterSchema (null)
				);
			}
		}


		[TestMethod]
		public void UpdateSchemaArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				
				var builder = new SchemaBuilder (entityTypeEngine, dbInfrastructure);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => builder.UpdateSchema (null)
				);
			}
		}


		[TestMethod]
		public void CheckSchemaArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				
				var builder = new SchemaBuilder (entityTypeEngine, dbInfrastructure);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => builder.CheckSchema (null)
				);
			}
		}


		private void RegisterSchema(List<Druid> entityIdsToRegister)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				
				var builder = new SchemaBuilder (entityTypeEngine, dbInfrastructure);

				builder.RegisterSchema (entityIdsToRegister);
			}
		}


		private void UpdateSchema(List<Druid> entityIdsToRegister)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				
				var builder = new SchemaBuilder (entityTypeEngine, dbInfrastructure);

				builder.UpdateSchema (entityIdsToRegister);
			}
		}


		private void CheckSchema(List<Druid> entityIdsToCheck, bool isRegistered)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				
				var builder = new SchemaBuilder (entityTypeEngine, dbInfrastructure);

				Assert.AreEqual (isRegistered, builder.CheckSchema (entityIdsToCheck));
			}
		}


	}


}
