//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

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
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			
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
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
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
				condition.AddCondition (table.Columns["Company"], DbCompare.Like, "Epsitec%");
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


		[Test]
		public void Check03DataTableBroker()
		{
			System.Diagnostics.Debug.WriteLine ("Broker-3");
			StructuredType type = Epsitec.Common.UI.Res.Types.Record.Address;
			DbRichCommand command;

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = Adapter.FindTableDefinition (transaction, type);
				DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
				condition.AddCondition (table.Columns["Zip"], DbCompare.LessThan, 1200);
				condition.AddCondition (table.Columns["Address1"], DbCompare.Like, "Case%");
				condition.Combiner = DbCompareCombiner.Or;
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
			System.Diagnostics.Debug.WriteLine ("Broker-3 done");
		}

		[Test]
		public void CheckInteractiveTable()
		{
			StructuredType type = Epsitec.Common.UI.Res.Types.Record.Address;
			DbRichCommand command;

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = Adapter.FindTableDefinition (transaction, type);
				DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
				command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table, condition);
				transaction.Commit ();
			}

			DataTableBroker broker = new DataTableBroker (type, command.DataSet.Tables["Record.Address"]);
			
			Window window = new Window ();

			window.Text = "CheckInteractiveTable";
			window.ClientSize = new Size (480, 400);
			window.Root.Padding = new Margins (4, 4, 4, 4);

			ItemTable itemTable = new ItemTable ();
			
			itemTable.Dock = DockStyle.Fill;
			itemTable.DefineDefaultColumns (type, 80.0);
			itemTable.Items = new CollectionView (broker);

			window.Root.Children.Add (itemTable);

			itemTable.ItemPanel.Show (itemTable.ItemPanel.GetItemView (0));

			window.Show ();

			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckInteractiveTableCompact()
		{
			ResourceManager manager = new ResourceManager ();
			StructuredType type = Epsitec.Common.UI.Res.Types.Record.Address;
			DbRichCommand command;

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = Adapter.FindTableDefinition (transaction, type);
				DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
				command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table, condition);
				transaction.Commit ();
			}

			DataTableBroker broker = new DataTableBroker (type, command.DataSet.Tables["Record.Address"]);

			Window window = new Window ();

			window.Text = "CheckInteractiveTableCompact";
			window.ClientSize = new Size (480, 400);
			window.Root.Padding = new Margins (4, 4, 4, 4);

			StructuredType record = new StructuredType ();
			StructuredData source = new StructuredData (record);

			record.Fields.Add (new StructuredTypeField ("Friends", type, Druid.Empty, -1, Relation.Collection));

			source.SetValue ("Friends", broker);

			TablePlaceholder placeholder = new TablePlaceholder ();
			PanelPlaceholder panel = new PanelPlaceholder ();

			window.Root.Children.Add (placeholder);
			window.Root.Children.Add (panel);
			
			ResourceManager.SetResourceManager (window.Root, manager);
			
			ItemTableColumn column = new ItemTableColumn (null, Epsitec.Common.Widgets.Layouts.GridLength.Auto);

			column.CaptionId  = Epsitec.Common.UI.Res.Captions.Address1.Id;
			column.TemplateId = Druid.Parse ("[KF]");

			placeholder.Columns.Add (column);
			
			placeholder.Dock = DockStyle.Left;
			placeholder.PreferredWidth = 200;
			placeholder.SourceTypeId = type.CaptionId;
			placeholder.SetBinding (placeholder.GetValueProperty (), new Binding (BindingMode.OneWay, "Friends"));

			panel.Dock = DockStyle.Fill;
			panel.SetBinding (panel.GetValueProperty (), new Binding (BindingMode.TwoWay, "Friends"));
			panel.PanelId = Druid.Parse ("[KF03]");

			Assert.IsNull (placeholder.Value);
			
			DataObject.SetDataContext (window.Root, new Binding (source));
			
			Assert.AreEqual (broker, ((ICollectionView) placeholder.Value).SourceCollection);

//-			Assert.IsNotNull (placeholder.Source);
//-			Assert.IsNotNull (placeholder.CollectionView);

			window.Show ();

			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckInteractiveTablePanel()
		{
			ResourceManager manager = new ResourceManager ();
			StructuredType type = Epsitec.Common.UI.Res.Types.Record.Address;
			DbRichCommand command;

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = Adapter.FindTableDefinition (transaction, type);
				DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
				command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table, condition);
				transaction.Commit ();
			}

			DataTableBroker broker = new DataTableBroker (type, command.DataSet.Tables["Record.Address"]);

			Window window = new Window ();

			window.Text = "CheckInteractiveTablePanel";
			window.ClientSize = new Size (475, 300);
			
			StructuredData source = new StructuredData (Epsitec.Common.UI.Res.Types.Record.Staff);
			source.SetValue ("Employees", broker);
			
			Epsitec.Common.UI.Panel panel = Epsitec.Common.UI.Panel.CreatePanel (Druid.Parse ("[KF04]"), manager);

			DataSource dataSource = new DataSource ();
			
			dataSource.AddDataSource ("*", source);
			panel.DataSource = dataSource;
			panel.Dock = DockStyle.Fill;
			
			window.Root.Children.Add (panel);

			window.Show ();

			Window.RunInTestEnvironment (window);
		}

		private DbInfrastructure infrastructure;
	}
}
