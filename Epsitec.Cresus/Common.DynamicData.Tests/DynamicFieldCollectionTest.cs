//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.DynamicData
{
	[TestFixture]
	public class PictogramTest
	{
		[SetUp] public void Initialize()
		{
		}
		
		
		[Test] public void Check01FindDynamicField()
		{
			DynamicFieldCollection dfc   = new DynamicFieldCollection ();
			System.Data.DataTable  table = this.CreateTable ();
			
			IDynamicField f00 = new DynamicFieldOne (table.Rows[0], table.Columns["C0"]);
			IDynamicField f01 = new DynamicFieldOne (table.Rows[0], table.Columns["C1"]);
			IDynamicField f02 = new DynamicFieldOne (table.Rows[0], table.Columns["C2"]);
			IDynamicField f22 = new DynamicFieldOne (table.Rows[2], table.Columns["C2"]);
			
			dfc.Add (f00);
			dfc.Add (f01);
			dfc.Add (f02);
			dfc.Add (f22);
			
			Assert.AreEqual (f00, dfc.FindDynamicField (table, 0, 0));
			Assert.AreEqual (f01, dfc.FindDynamicField (table, 0, 1));
			Assert.AreEqual (f02, dfc.FindDynamicField (table, 0, 2));
			Assert.AreEqual (f22, dfc.FindDynamicField (table, 2, 2));
		}
		
		[Test] public void Check02FindDynamicCells()
		{
			DynamicFieldCollection dfc   = new DynamicFieldCollection ();
			System.Data.DataTable  table = this.CreateTable ();
			
			IDynamicField f00 = new DynamicFieldOne (table.Rows[0], table.Columns["C0"]);
			IDynamicField f01 = new DynamicFieldOne (table.Rows[0], table.Columns["C1"]);
			IDynamicField f02 = new DynamicFieldOne (table.Rows[0], table.Columns["C2"]);
			IDynamicField f22 = new DynamicFieldOne (table.Rows[2], table.Columns["C2"]);
			
			dfc.Add (f00);
			dfc.Add (f01);
			dfc.Add (f02);
			dfc.Add (f22);
			
			CellIndex[] cells = dfc.FindDynamicCells (table);
			
			Assert.AreEqual (4, cells.Length);
			Assert.AreEqual (0, cells[0].Row);
			Assert.AreEqual (0, cells[1].Row);
			Assert.AreEqual (0, cells[2].Row);
			Assert.AreEqual (2, cells[3].Row);
			Assert.AreEqual (0, cells[0].Column);
			Assert.AreEqual (1, cells[1].Column);
			Assert.AreEqual (2, cells[2].Column);
			Assert.AreEqual (2, cells[3].Column);
		}
		
		[Test] public void Check03FindDynamicCells()
		{
			DynamicFieldCollection dfc   = new DynamicFieldCollection ();
			System.Data.DataTable  table = this.CreateTable ();
			
			IDynamicField f00 = new DynamicFieldOne (table.Rows[0], table.Columns["C0"]);
			IDynamicField f01 = new DynamicFieldAllInColumn (table.Columns["C1"]);
			IDynamicField f22 = new DynamicFieldOne (table.Rows[2], table.Columns["C2"]);
			
			dfc.Add (f00);		//	0:0
			dfc.Add (f01);		//	*:1 -> 0:1, 1:1, 2:1, 3:1
			dfc.Add (f22);		//	2:2
			
			CellIndex[] cells = dfc.FindDynamicCells (table);
			
			Assert.AreEqual (6, cells.Length);
			Assert.AreEqual (0, cells[0].Row);
			Assert.AreEqual (0, cells[1].Row);
			Assert.AreEqual (1, cells[2].Row);
			Assert.AreEqual (2, cells[3].Row);
			Assert.AreEqual (2, cells[4].Row);
			Assert.AreEqual (3, cells[5].Row);
			
			Assert.AreEqual (0, cells[0].Column);
			Assert.AreEqual (1, cells[1].Column);
			Assert.AreEqual (1, cells[2].Column);
			Assert.AreEqual (1, cells[3].Column);
			Assert.AreEqual (2, cells[4].Column);
			Assert.AreEqual (1, cells[5].Column);
		}
		
		[Test] public void Check04Changed()
		{
			this.changed_counter = 0;
			
			DynamicFieldCollection dfc = new DynamicFieldCollection ();
			
			dfc.Changed += new Support.EventHandler (this.ChangedEventHandler);
			dfc.Clear ();
			
			Assert.AreEqual (0, this.changed_counter);
			
			IDynamicField field = new DynamicFieldAllInColumn (null);
			
			dfc.Add (field);
			
			Assert.AreEqual (1, this.changed_counter);
			
			dfc.Clear ();
			
			Assert.AreEqual (2, this.changed_counter);
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void Check05AddEx()
		{
			DynamicFieldCollection dfc = new DynamicFieldCollection ();
			
			IDynamicField field = new DynamicFieldAllInColumn (null);
			
			dfc.Add (field);
			dfc.Add (field);
		}
		
		
		private void ChangedEventHandler(object sender)
		{
			this.changed_counter++;
		}
		
		private System.Data.DataTable CreateTable()
		{
			System.Data.DataTable table = new System.Data.DataTable ("T1");
			
			System.Data.DataColumn col0 = new System.Data.DataColumn ("C0", typeof (int));
			System.Data.DataColumn col1 = new System.Data.DataColumn ("C1", typeof (int));
			System.Data.DataColumn col2 = new System.Data.DataColumn ("C2", typeof (int));
			
			table.Columns.Add (col0);
			table.Columns.Add (col1);
			table.Columns.Add (col2);
			
			table.Rows.Add (new object[] { 0, 100, 200 });
			table.Rows.Add (new object[] { 1, 101, 201 });
			table.Rows.Add (new object[] { 2, 102, 202 });
			table.Rows.Add (new object[] { 3, 103, 203 });
			
			return table;
		}

		
		#region Support classes / various IDynamicField implementations
		public class DynamicFieldOne : IDynamicField
		{
			public DynamicFieldOne(System.Data.DataRow row, System.Data.DataColumn column)
			{
				this.row = row;
				this.column = column;
			}
			
			
			#region IDynamicField Members
			bool IDynamicField.Match(System.Data.DataRow row, System.Data.DataColumn column)
			{
				return (this.row == row) && (this.column == column);
			}
			
			FieldMatchResult IDynamicField.Match(System.Data.DataColumn column)
			{
				if (this.column == column)
				{
					return FieldMatchResult.One;
				}
				
				return FieldMatchResult.Zero;
			}
			
			FieldMatchResult IDynamicField.Match(System.Data.DataRow row)
			{
				if (this.row == row)
				{
					return FieldMatchResult.One;
				}
				
				return FieldMatchResult.Zero;
			}
			#endregion
			
			System.Data.DataRow				row;
			System.Data.DataColumn			column;
		}
		
		public class DynamicFieldAllInColumn : IDynamicField
		{
			public DynamicFieldAllInColumn(System.Data.DataColumn column)
			{
				this.column = column;
			}
			
			
			#region IDynamicField Members
			bool IDynamicField.Match(System.Data.DataRow row, System.Data.DataColumn column)
			{
				return (this.column == column);
			}
			
			FieldMatchResult IDynamicField.Match(System.Data.DataColumn column)
			{
				if (this.column == column)
				{
					return FieldMatchResult.All;
				}
				
				return FieldMatchResult.Zero;
			}
			
			FieldMatchResult IDynamicField.Match(System.Data.DataRow row)
			{
				return FieldMatchResult.One;
			}
			#endregion
			
			System.Data.DataColumn			column;
		}
		#endregion
		
		private int								changed_counter;
	}
}
