//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class SchemaEngineTest
	{
		[Test]
		public void Check01CreateTableDefinition()
		{
			SchemaEngine engine = new SchemaEngine (this.infrastructure);
			DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);
			engine.CreateTableDefinition (this.articleEntityId);
			transaction.Rollback ();
			transaction.Dispose ();
		}

		[Test]
		public void Check02CreateTwiceAndFindTableDefinition()
		{
			SchemaEngine engine = new SchemaEngine (this.infrastructure);

			Assert.IsNull (engine.FindTableDefinition (this.articleEntityId));
			DbTable table1 = engine.CreateTableDefinition (this.articleEntityId);
			DbTable table2 = engine.CreateTableDefinition (this.articleEntityId);

			Assert.AreEqual (table1, table2);

			engine = new SchemaEngine (this.infrastructure);
			DbTable table3 = engine.FindTableDefinition (this.articleEntityId);
			DbTable table4 = engine.FindTableDefinition (this.articleVisserieEntityId);

			Assert.AreNotEqual (table1, table3);
			Assert.AreEqual (table1.Name, table3.Name);
			Assert.AreEqual (table1.Columns.Count, table3.Columns.Count);
			Assert.IsNull (table4);

			table4 = engine.CreateTableDefinition (this.articleVisserieEntityId);
		}

		[Test]
		public void Check03LoadTableSchema()
		{
			DataContext context = new DataContext (this.infrastructure);

			DbTable table = context.SchemaEngine.FindTableDefinition (this.articleEntityId);

			Assert.AreEqual (0, context.RichCommand.DataSet.Tables.Count);

			context.LoadTableSchema (table);

			Assert.AreEqual (6, context.RichCommand.DataSet.Tables.Count);

			TestSupport.Database.DumpDataSet (context.RichCommand.DataSet);

			context.Dispose ();
		}

		private readonly DbInfrastructure infrastructure = TestSupport.Database.NewInfrastructure ();

		private readonly Druid articleEntityId = Druid.Parse ("[630Q]");
		private readonly Druid articleVisserieEntityId = Druid.Parse ("[631]");
	}
}
