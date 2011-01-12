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
			List<Druid> entityIds = new List<Druid> ()
			{
				Druid.Parse ("[L0A62]")
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
				Druid.Parse ("[L0A5]")
			};

			List<Druid> entityIdsToCheck = new List<Druid> ()
			{
				Druid.Parse ("[L0A5]"),
				Druid.Parse ("[L0A4]"),
				Druid.Parse ("[L0A1]"),
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
				Druid.Parse ("[L0AQ]")
			};

			List<Druid> entityIdsToCheck = new List<Druid> ()
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
			
			this.CheckSchema (entityIdsToCheck, false);
			this.RegisterSchema (entityIdsToRegister);
			this.CheckSchema (entityIdsToCheck, true);
		}


		[TestMethod]
		public void RegisterAndCheckSchema4()
		{
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

			this.CheckSchema (entityIds, false);
			this.RegisterSchema (entityIds);
			this.CheckSchema (entityIds, true);
		}


		[TestMethod]
		public void RegisterAndCheckSchema5()
		{
			List<Druid> entityIdsToRegister1 = new List<Druid> ()
			{
				Druid.Parse ("[L0A5]")
			};

			List<Druid> entityIdsToCheck1 = new List<Druid> ()
			{
				Druid.Parse ("[L0A5]"),
				Druid.Parse ("[L0A4]"),
				Druid.Parse ("[L0A1]"),
			};

			List<Druid> entityIdsToRegister2 = new List<Druid> ()
			{
				Druid.Parse ("[L0AQ]")
			};

			List<Druid> entityIdsToCheck2 = new List<Druid> ()
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
				Druid.Parse ("[L0A5]")
			};

			List<Druid> entityIdsToCheck1 = new List<Druid> ()
			{
				Druid.Parse ("[L0A5]"),
				Druid.Parse ("[L0A4]"),
				Druid.Parse ("[L0A1]"),
			};

			List<Druid> entityIdsToRegister2 = new List<Druid> ()
			{
				Druid.Parse ("[L0AQ]")
			};

			List<Druid> entityIdsToCheck2 = new List<Druid> ()
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
				Druid.Parse ("[L0AQ]")
			};

			List<Druid> entityIdsToCheck1 = new List<Druid> ()
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
			};

			List<Druid> entityIdsToRegister2 = new List<Druid> ()
			{
				Druid.Parse ("[L0A5]")
			};

			List<Druid> entityIdsToCheck2 = new List<Druid> ()
			{
				Druid.Parse ("[L0A5]"),
				Druid.Parse ("[L0A4]"),
				Druid.Parse ("[L0A1]"),
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
