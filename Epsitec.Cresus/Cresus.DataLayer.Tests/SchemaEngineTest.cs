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
			DbTable table4 = engine.FindTableDefinition (this.articleVisserieEntityId);

			Assert.AreNotEqual (table1, table3);
			Assert.AreEqual (table1.Name, table3.Name);
			Assert.AreEqual (table1.Columns.Count, table3.Columns.Count);
			Assert.IsNull (table4);

			table4 = engine.CreateTableDefinition (this.articleVisserieEntityId);
		}

		[Test]
		public void Check10LoadTableSchema()
		{
			DataContext context = new DataContext (this.infrastructure);

			DbTable table = context.SchemaEngine.FindTableDefinition (this.articleEntityId);

			Assert.AreEqual (0, context.RichCommand.DataSet.Tables.Count);

			context.LoadTableSchema (table);

			Assert.AreEqual (5, context.RichCommand.DataSet.Tables.Count);

			DumpDataSet (context.RichCommand.DataSet);

			context.Dispose ();
		}

		[Test]
		public void Check11SaveEntity()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check11SaveEntity");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			
			AbstractEntity entity = context.EntityContext.CreateEntity (this.articleEntityId);
			AbstractEntity prixVente = entity.GetField<AbstractEntity> ("[630B1]");				//	Article.PrixVente

			Assert.AreEqual (6, context.CountManagedEntities ());
			Assert.AreEqual (this.prixEntityId, prixVente.GetEntityStructuredTypeId ());

			entity.SetField<string> ("[63091]", "VI-M3-10");									//	Article.Numéro
			entity.SetField<string> ("[630A1]", "Vis M3 10mm, inox");							//	Article.Désignation

			prixVente.SetField<decimal> ("[630H]", 0.05M);										//	Prix.HT pour Article.PrixVente

			context.SerializeEntity (entity);
			context.SerializeEntity (prixVente);
			context.SaveChanges ();

			int count = 0;

			System.Diagnostics.Debug.WriteLine ("Adding articles");
			List<AbstractEntity> entities = new List<AbstractEntity> ();

			foreach (AbstractEntity item in this.GetItems (context.EntityContext))
			{
				entities.Add (item);
				context.SerializeEntity (item);
				count++;
			}

			System.Diagnostics.Debug.WriteLine ("Saving");

			context.SaveChanges ();

			System.Diagnostics.Debug.WriteLine ("Saved " + count + " entities");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			context.Dispose ();

			Assert.AreEqual (1920, entities.Count);
			Assert.AreEqual (1000000000001L, context.GetEntityDataMapping (entity).RowKey.Id.Value);
			Assert.AreEqual (1000000000002L, context.GetEntityDataMapping (entities[0]).RowKey.Id.Value);
		}

		[Test]
		public void Check12LoadEntity()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check12LoadEntity");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			
			DbTable table1 = context.SchemaEngine.FindTableDefinition (this.articleEntityId);
			DbTable table2 = context.SchemaEngine.FindTableDefinition (this.articleVisserieEntityId);
			DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);

			System.Diagnostics.Debug.WriteLine ("Loading data from database");

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				context.RichCommand.ImportTable (transaction, table1, condition);
				context.RichCommand.ImportTable (transaction, table2, condition);
				transaction.Commit ();
			}

			System.Diagnostics.Debug.WriteLine ("Done.");

			DbKey key1 = new DbKey (new DbId (1000000000001L));
			DbKey key2 = new DbKey (new DbId (1000000000002L));

			AbstractEntity entity1 = context.DeserializeEntity (key1, this.articleEntityId);
			AbstractEntity entity2 = context.DeserializeEntity (key2, this.articleEntityId);

			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			Assert.AreEqual (this.articleEntityId, entity1.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity1.GetField<string> ("[63091]"));

			Assert.AreEqual (this.articleVisserieEntityId, entity2.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity2.GetField<string> ("[63091]"));
			Assert.AreEqual ("M3", entity2.GetField<string> ("[6312]"));
			
			context.Dispose ();
		}

		[Test]
		public void Check13LoadEntitySingle()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check13LoadEntitySingle");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			DbTable table1 = context.SchemaEngine.FindTableDefinition (this.articleEntityId);
			DbTable table2 = context.SchemaEngine.FindTableDefinition (this.articleVisserieEntityId);
			DbSelectCondition condition1 = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
			DbSelectCondition condition2 = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
			DbSelectCondition condition3 = new DbSelectCondition (this.infrastructure.Converter, DbSelectRevision.LiveActive);
			condition1.AddCondition (table1.Columns[Tags.ColumnId], DbCompare.Equal, 1000000000002L);
			condition2.AddCondition (table2.Columns[Tags.ColumnId], DbCompare.Equal, 1000000000002L);
			condition3.AddCondition (table1.Columns[Tags.ColumnId], DbCompare.Equal, 1000000000001L);

			System.Diagnostics.Debug.WriteLine ("Loading data from database");

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				context.RichCommand.ImportTable (transaction, table1, condition1);
				context.RichCommand.ImportTable (transaction, table2, condition2);
				context.RichCommand.ImportTable (transaction, table1, condition3);
				transaction.Commit ();
			}

			System.Diagnostics.Debug.WriteLine ("Done.");

			DbKey key1 = new DbKey (new DbId (1000000000001L));
			DbKey key2 = new DbKey (new DbId (1000000000002L));

			AbstractEntity entity1 = context.DeserializeEntity (key1, this.articleEntityId);
			AbstractEntity entity2 = context.DeserializeEntity (key2, this.articleEntityId);

			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			Assert.AreEqual (this.articleEntityId, entity1.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity1.GetField<string> ("[63091]"));

			Assert.AreEqual (this.articleVisserieEntityId, entity2.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity2.GetField<string> ("[63091]"));
			Assert.AreEqual ("M3", entity2.GetField<string> ("[6312]"));

			Assert.AreEqual (entity1, context.ResolveEntity (key1, this.articleEntityId));
			Assert.AreEqual (entity2, context.ResolveEntity (key2, this.articleEntityId));
			Assert.AreEqual (entity2, context.ResolveEntity (key2, this.articleVisserieEntityId));

			context.Dispose ();
		}

		[Test]
		public void Check14LoadAndUpdateEntitySingle()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check14LoadAndUpdateEntitySingle");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			System.Diagnostics.Debug.WriteLine ("Implicit loading of entities");

			DbKey key1 = new DbKey (new DbId (1000000000001L));
			DbKey key2 = new DbKey (new DbId (1000000000002L));

			AbstractEntity entity1 = context.DeserializeEntity (key1, this.articleEntityId);
			AbstractEntity entity2 = context.DeserializeEntity (key2, this.articleEntityId);

			System.Diagnostics.Debug.WriteLine ("Done.");

			Assert.AreEqual (this.articleEntityId, entity1.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity1.GetField<string> ("[63091]"));

			Assert.AreEqual (this.articleVisserieEntityId, entity2.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity2.GetField<string> ("[63091]"));
			Assert.AreEqual ("M3", entity2.GetField<string> ("[6312]"));

			entity1.SetField<string> ("[630A1]", "Vis M3 10mm (acier inox)");
			entity2.SetField<string> ("[630A1]", "Vis M3 10mm (acier inox)");
			entity2.SetField<string> ("[6312]", "M3.0");

			System.Diagnostics.Debug.WriteLine ("Saving");

			context.SaveChanges ();

			System.Diagnostics.Debug.WriteLine ("Done.");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			
			context.Dispose ();
		}

		private IEnumerable<AbstractEntity> GetItems(EntityContext context)
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

							AbstractEntity entity = context.CreateEntity (this.articleVisserieEntityId);

							entity.SetField<string> ("[63091]", itemKey);
							entity.SetField<string> ("[630A1]", itemValue);
							entity.SetField<int> ("[6311]", int.Parse (len.Substring (0, 2)));
							entity.SetField<string> ("[6312]", size);

							yield return entity;
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
		private readonly Druid articleEntityId = Druid.Parse ("[630Q]");
		private readonly Druid articleVisserieEntityId = Druid.Parse ("[631]");
		private readonly Druid adresseEntityId = Druid.Parse ("[63081]");
		private readonly Druid prixEntityId = Druid.Parse ("[6308]");
	}
}
