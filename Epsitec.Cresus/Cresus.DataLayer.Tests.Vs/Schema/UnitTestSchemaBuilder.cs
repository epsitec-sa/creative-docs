using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


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
			List<Druid> entityIdsToRegister = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]")
			};

			List<Druid> entityIdsToCheck = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A4]"),
			};

			this.CheckSchema (entityIdsToCheck, false);
			this.RegisterSchema (entityIdsToRegister);
			this.CheckSchema (entityIdsToCheck, true);
		}


		[TestMethod]
		public void RegisterAndCheckSchema3()
		{
			List<Druid> entityIdsToRegister = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]")
			};

			List<Druid> entityIdsToCheck = new List<Druid> ()
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
			
			this.CheckSchema (entityIdsToCheck, false);
			this.RegisterSchema (entityIdsToRegister);
			this.CheckSchema (entityIdsToCheck, true);
		}


		[TestMethod]
		public void RegisterAndCheckSchema4()
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
			List<Druid> entityIdsToRegister1 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]")
			};

			List<Druid> entityIdsToCheck1 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A4]"),
			};

			List<Druid> entityIdsToRegister2 = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]")
			};

			List<Druid> entityIdsToCheck2 = new List<Druid> ()
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

			this.CheckSchema (entityIdsToCheck1, false);
			this.CheckSchema (entityIdsToCheck2, false);

			this.RegisterSchema (entityIdsToRegister1);

			this.CheckSchema (entityIdsToCheck1, true);
			this.CheckSchema (entityIdsToCheck2, false);

			this.RegisterSchema (entityIdsToRegister2);
	
			this.CheckSchema (entityIdsToCheck1, true);
			this.CheckSchema (entityIdsToCheck2, true);
		}


		[TestMethod]
		public void UpdateAndCheckSchema1()
		{
			List<Druid> entityIdsToRegister1 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]")
			};

			List<Druid> entityIdsToCheck1 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A4]"),
			};

			List<Druid> entityIdsToRegister2 = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]")
			};

			List<Druid> entityIdsToCheck2 = new List<Druid> ()
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

			this.CheckSchema (entityIdsToCheck1, false);
			this.CheckSchema (entityIdsToCheck2, false);

			this.UpdateSchema (entityIdsToRegister1);

			this.CheckSchema (entityIdsToCheck1, true);
			this.CheckSchema (entityIdsToCheck2, false);

			this.UpdateSchema (entityIdsToRegister2);

			this.CheckSchema (entityIdsToCheck1, true);
			this.CheckSchema (entityIdsToCheck2, true);
		}


		[TestMethod]
		public void UpdateAndCheckSchema2()
		{
			List<Druid> entityIdsToRegister1 = new List<Druid> ()
			{
				Druid.Parse ("[J1AT1]")
			};

			List<Druid> entityIdsToCheck1 = new List<Druid> ()
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

			List<Druid> entityIdsToRegister2 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]")
			};

			List<Druid> entityIdsToCheck2 = new List<Druid> ()
			{
				Druid.Parse ("[J1A9]"),
				Druid.Parse ("[J1A6]"),
				Druid.Parse ("[J1A4]"),
			};

			this.CheckSchema (entityIdsToCheck1, false);
			this.CheckSchema (entityIdsToCheck2, false);

			this.UpdateSchema (entityIdsToRegister1);

			this.CheckSchema (entityIdsToCheck1, true);
			this.CheckSchema (entityIdsToCheck2, true);

			this.UpdateSchema (entityIdsToRegister2);

			this.CheckSchema (entityIdsToCheck1, false);
			this.CheckSchema (entityIdsToCheck2, true);
		}


		[TestMethod]
		public void CreateSchemaArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var builder = new SchemaBuilder (dbInfrastructure);

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
				var builder = new SchemaBuilder (dbInfrastructure);

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
				var builder = new SchemaBuilder (dbInfrastructure);

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
				SchemaBuilder builder = new SchemaBuilder (dbInfrastructure);

				builder.RegisterSchema (entityIdsToRegister);
			}
		}


		private void UpdateSchema(List<Druid> entityIdsToRegister)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				SchemaBuilder builder = new SchemaBuilder (dbInfrastructure);

				builder.UpdateSchema (entityIdsToRegister);
			}
		}


		private void CheckSchema(List<Druid> entityIdsToCheck, bool isRegistered)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				SchemaBuilder builder = new SchemaBuilder (dbInfrastructure);

				Assert.AreEqual (isRegistered, builder.CheckSchema (entityIdsToCheck));
			}
		}
            

	}


}
