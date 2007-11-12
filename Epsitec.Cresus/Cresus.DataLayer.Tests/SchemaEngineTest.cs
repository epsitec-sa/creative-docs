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
		[TestFixtureSetUp]
		public void Setup()
		{
			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\FICHE.FIREBIRD");
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\FICHE.FIREBIRD");
				}
				catch
				{
				}
			}

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess ("FICHE"));
			}

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

			Assert.AreNotEqual (table1, table3);
			Assert.AreEqual (table1.Name, table3.Name);
			Assert.AreEqual (table1.Columns.Count, table3.Columns.Count);
		}

		[Test]
		public void Check10LoadTableSchema()
		{
			DataContext context = new DataContext (this.infrastructure);

			DbTable table = context.SchemaEngine.FindTableDefinition (this.articleEntityId);

			Assert.AreEqual (0, context.RichCommand.DataSet.Tables.Count);

			context.LoadTableSchema (table);

			Assert.AreEqual (1, context.RichCommand.DataSet.Tables.Count);

			DumpDataSet (context.RichCommand.DataSet);
		}

		[Test]
		public void Check11SaveEntity()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check11SaveEntity");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			
			AbstractEntity entity = context.EntityContext.CreateEntity (this.articleEntityId);

			Assert.AreEqual (6, context.CountManagedEntities ());

			entity.SetField<string> ("[63091]", null, "VI-M3-10");
			entity.SetField<string> ("[630A1]", null, "Vis M3 10mm, inox");

			context.PersistEntity (entity);
			context.SaveChanges ();

			int count = 0;

			System.Diagnostics.Debug.WriteLine ("Adding articles");

			foreach (KeyValuePair<string, string> item in SchemaEngineTest.GetItems ())
			{
				entity = context.EntityContext.CreateEntity (this.articleEntityId);
				entity.SetField<string> ("[63091]", null, item.Key);
				entity.SetField<string> ("[630A1]", null, item.Value);
				context.PersistEntity (entity);
				count++;
			}

			System.Diagnostics.Debug.WriteLine ("Saving");

			context.SaveChanges ();

			System.Diagnostics.Debug.WriteLine ("Saved " + count + " entities");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
		}

		private static IEnumerable<KeyValuePair<string, string>> GetItems()
		{
			string[] materials = new string[] { "Inox", "Cuivre", "Galvanisé", "Teflon", "POM", "Acier" };
			string[] categories = new string[] { "Vis", "Ecrou", "Rondelle", "Boulon" };
			string[] sizes = new string[] { "M3", "M4", "M5", "M6", "M8", "M10", "M12", "M14", "M16", "M20" };
			string[] lengths = new string[] { "10mm", "12mm", "15mm", "20mm", "25mm", "30mm", "40mm", "50mm" };

			foreach (string mat in materials)
			{
				foreach (string cat in categories)
				{
					foreach (string size in sizes)
					{
						foreach (string len in lengths)
						{
							string itemKey = string.Concat (cat.Substring (0, 1), mat.Substring (0, 1), "-", size, "-", len.Substring (0, 2));
							string itemValue = string.Concat (cat, " ", size, " ", len, ", ", mat);

							yield return new KeyValuePair<string, string> (itemKey, itemValue);
						}
					}
				}
			}
		}

		#region Helper Methods

		private static void DumpDataSet(System.Data.DataSet dataSet)
		{
			foreach (System.Data.DataTable table in dataSet.Tables)
			{
				System.Console.Out.WriteLine ("--------------------------------------");
				System.Console.Out.WriteLine ("Table {0}, {1} columns, {2} rows", table.TableName, table.Columns.Count, table.Rows.Count);
				
				foreach (System.Data.DataColumn column in table.Columns)
				{
					System.Console.Out.WriteLine ("- Column {0} : {1}{2}", column.ColumnName, column.DataType.Name, column.Unique ? ", unique" : "");
				}
				System.Console.Out.WriteLine ("--------------------------------------");
				foreach (System.Data.DataRow row in table.Rows)
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder();
					foreach (object o in row.ItemArray)
					{
						if (buffer.Length > 0)
						{
							buffer.Append (", ");
						}
						buffer.Append (o == null ? "<null>" : o.ToString ());
					}
					System.Console.Out.WriteLine (buffer);
				}
				System.Console.Out.WriteLine ();
			}
		}

		#endregion

		private DbInfrastructure infrastructure;
		private Druid articleEntityId = Druid.Parse ("[630Q]");
		private Druid adresseEntityId = Druid.Parse ("[63081]");
	}
}
