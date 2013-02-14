using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
{


	[TestClass]
	public sealed class UnitTestServiceSchemaBuilder
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void GetServiceTableNames()
		{
			var expectedServiceTableNames = new List<string> ()
			{
				ConnectionManager.TableFactory.TableName,
				EntityDeletionLog.TableFactory.TableName,
				EntityModificationLog.TableFactory.TableName,
				InfoManager.TableFactory.TableName,
				LockManager.TableFactory.TableName,
				UidManager.TableFactory.TableName,
			};

			var actualServiceTableNames = ServiceSchemaBuilder.GetServiceTableNames ().ToList ();

			expectedServiceTableNames = expectedServiceTableNames.OrderBy (s => s).ToList ();
			actualServiceTableNames = actualServiceTableNames.OrderBy (s => s).ToList ();

			CollectionAssert.AreEqual (expectedServiceTableNames, actualServiceTableNames);
		}


		[TestMethod]
		public void GetServiceTables()
		{
			var expectedServiceTables = new List<DbTable> ()
			{
				ConnectionManager.TableFactory.BuildTable(),
				EntityDeletionLog.TableFactory.BuildTable(),
				InfoManager.TableFactory.BuildTable(),
				LockManager.TableFactory.BuildTable(),
				EntityModificationLog.TableFactory.BuildTable(),
				UidManager.TableFactory.BuildTable(),
			};

			var actualServiceTables = ServiceSchemaBuilder.BuildServiceTables ().ToList ();

			expectedServiceTables = expectedServiceTables.OrderBy (t => t.Name).ToList ();
			actualServiceTables = actualServiceTables.OrderBy (t => t.Name).ToList ();

			Assert.AreEqual (expectedServiceTables.Count, actualServiceTables.Count);

			for (int i = 0; i < expectedServiceTables.Count; i++)
			{
				DbSchemaChecker.AreDbTablesEqual (expectedServiceTables[i], actualServiceTables[i]);
			}
		}


	}


}
