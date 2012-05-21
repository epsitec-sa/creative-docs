using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
{


	[TestClass]
	public sealed class UnitTestServiceSchemaEngine
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetEmptyTestDatabase ();
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new ServiceSchemaEngine (null, this.GetServiceTableNames ())
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new ServiceSchemaEngine (dbInfrastructure, null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new ServiceSchemaEngine (dbInfrastructure, new List<string>() { null })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new ServiceSchemaEngine (dbInfrastructure, new List<string> () { "" })
				);
			}
		}


		[TestMethod]
		public void ConstructorInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new ServiceSchemaEngine (dbInfrastructure, new List<string> () { "fgfdgfsd" })
				);
			}
		}


		[TestMethod]
		public void GetServiceTableArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ServiceSchemaEngine engine = new ServiceSchemaEngine (dbInfrastructure, this.GetServiceTableNames ());

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => engine.GetServiceTable (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => engine.GetServiceTable ("")
				);
			}
		}


		[TestMethod]
		public void GetServiceTableTest1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ServiceSchemaEngine engine = new ServiceSchemaEngine (dbInfrastructure, this.GetServiceTableNames ());

				foreach (DbTable expectedTable in this.GetServiceTables ())
				{
					var actualTable = engine.GetServiceTable (expectedTable.Name);

					Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (expectedTable, actualTable));
				}
			}
		}


		[TestMethod]
		public void GetServiceTableTest2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ServiceSchemaEngine engine = new ServiceSchemaEngine (dbInfrastructure, this.GetServiceTableNames ().Skip (3));

				foreach (DbTable expectedTable in this.GetServiceTables ().Skip (3))
				{
					var actualTable = engine.GetServiceTable (expectedTable.Name);

					Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (expectedTable, actualTable));
				}
			}
		}


		[TestMethod]
		public void GetServiceTableInvalidBehaviorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ServiceSchemaEngine engine = new ServiceSchemaEngine (dbInfrastructure, this.GetServiceTableNames ());

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => engine.GetServiceTable ("543543")
				);
			}
		}


		private IEnumerable<string> GetServiceTableNames()
		{
			yield return ConnectionManager.TableFactory.TableName;
			yield return EntityDeletionLog.TableFactory.TableName;
			yield return InfoManager.TableFactory.TableName;
			yield return LockManager.TableFactory.TableName;
			yield return EntityModificationLog.TableFactory.TableName;
			yield return UidManager.TableFactory.TableName;
		}


		private IEnumerable<DbTable> GetServiceTables()
		{
			yield return ConnectionManager.TableFactory.BuildTable ();
			yield return EntityDeletionLog.TableFactory.BuildTable ();
			yield return InfoManager.TableFactory.BuildTable ();
			yield return LockManager.TableFactory.BuildTable ();
			yield return EntityModificationLog.TableFactory.BuildTable ();
			yield return UidManager.TableFactory.BuildTable ();
		}


	}


}
