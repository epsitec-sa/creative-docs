//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class DataBrokerTest
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			this.infrastructure = new DbInfrastructure ();
			this.infrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("FICHE"));
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			this.infrastructure.Dispose ();
			this.infrastructure = null;
		}

		[Test]
		public void Check01DataTableBroker()
		{
			System.Diagnostics.Debug.WriteLine ("Broker-1");
			StructuredType type = Epsitec.Common.UI.Res.Types.Record.Address;
			DbRichCommand command;

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = Adapter.FindTableDefinition (transaction, type);
				DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
				command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table, condition);
				transaction.Commit ();
			}

			Assert.IsNotNull (command);

			System.Data.DataTable dataTable = command.DataSet.Tables["Record.Address"];

			Assert.IsNotNull (dataTable);

			DataTableBroker broker = new DataTableBroker (type, dataTable);
			int total = 0;

			foreach (DataBrokerRecord record in broker.Records)
			{
				System.Console.Out.WriteLine ("{0} {1}, {2}, {3} {4}",
					/**/					  record.GetValue ("FirstName"), record.GetValue ("LastName"),
					/**/					  record.GetValue ("Address1"),
					/**/					  record.GetValue ("Zip"), record.GetValue ("City"));

				total++;
			}

			System.Console.Out.WriteLine ("Total: {0} records", total);
			System.Console.Out.WriteLine ("-----------------------------------------------");
			System.Diagnostics.Debug.WriteLine ("Broker-1 done");
		}

		[Test]
		public void Check02DataTableBroker()
		{
			System.Diagnostics.Debug.WriteLine ("Broker-2");
			StructuredType type = Epsitec.Common.UI.Res.Types.Record.Address;
			DbRichCommand command;

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = Adapter.FindTableDefinition (transaction, type);
				DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
				condition.AddCondition (table.Columns["Company"], DbCompare.Equal, "Epsitec SA");
				command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table, condition);
				transaction.Commit ();
			}

			Assert.IsNotNull (command);

			System.Data.DataTable dataTable = command.DataSet.Tables["Record.Address"];

			Assert.IsNotNull (dataTable);

			DataTableBroker broker = new DataTableBroker (type, dataTable);
			int total = 0;

			foreach (DataBrokerRecord record in broker.Records)
			{
				System.Console.Out.WriteLine ("{0} {1}, {2}, {3} {4}",
					/**/					  record.GetValue ("FirstName"), record.GetValue ("LastName"),
					/**/					  record.GetValue ("Address1"),
					/**/					  record.GetValue ("Zip"), record.GetValue ("City"));
				total++;
			}

			System.Console.Out.WriteLine ("Total: {0} records", total);
			System.Console.Out.WriteLine ("-----------------------------------------------");
			System.Diagnostics.Debug.WriteLine ("Broker-2 done");
		}

		private DbInfrastructure infrastructure;
	}
}
