using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTableTest
	{
		[Test] public void CheckNewDbTable()
		{
			DbTable table = new DbTable ("Test");
			
			Assert.IsNotNull (table);
			Assert.AreEqual ("Test", table.Name);
			Assert.AreEqual (DbElementCat.Unknown, table.Category);
			Assert.AreEqual (false, table.HasPrimaryKey);
			Assert.AreEqual (0, table.PrimaryKeys.Count);
			Assert.AreEqual (0, table.Columns.Count);
			
			table.DefineCategory (DbElementCat.Internal);
			
			Assert.AreEqual (DbElementCat.Internal, table.Category);
			
			table.DefineCategory (DbElementCat.Internal);
		}
		
		[Test] public void CheckSerializeToXml()
		{
			DbTable table = new DbTable ("Test");
			DbColumn column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			
			column.IsIndexed = true;
			column.DefineCategory (DbElementCat.UserDataManaged);
			
			table.DefineCategory (DbElementCat.UserDataManaged);
			table.DefineRevisionMode (DbRevisionMode.Enabled);
			
			table.PrimaryKeys.Add (column);
			table.Columns.Add (column);
			
			Assertion.AssertEquals (table, column.Table);
			
			string xml = DbTable.SerializeToXml (table, true);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			
			DbTable test = DbTable.CreateTable (xml);
			
			Assertion.AssertEquals (test, test.Columns["A"].Table);
			Assertion.AssertEquals (DbElementCat.UserDataManaged, test.Category);
			Assertion.AssertEquals (DbRevisionMode.Enabled, test.RevisionMode);
			Assertion.AssertEquals ("Test", test.Name);
			Assertion.AssertEquals (DbElementCat.UserDataManaged, test.Columns["A"].Category);
		}
		
		[Test] public void CheckForeignKeys()
		{
			DbTable table = new DbTable ("Test");
			
			DbColumn column_1 = DbColumn.CreateRefColumn ("A", "ParentTable", DbColumnClass.RefId, new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForId)), Nullable.Yes);
			DbColumn column_2 = new DbColumn ("X", new DbTypeNum (DbNumDef.FromRawType (DbRawType.SmallDecimal)), Nullable.Yes, DbColumnClass.Data, DbElementCat.UserDataManaged);
			DbColumn column_3 = DbColumn.CreateRefColumn ("Z", "Customer", DbColumnClass.RefId, new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForId)), Nullable.Yes);
			
			table.Columns.Add (column_1);
			table.Columns.Add (column_2);
			table.Columns.Add (column_3);
			
			DbForeignKey[] fk = table.ForeignKeys;
			
			Assertion.AssertEquals (2, fk.Length);
			Assertion.AssertEquals (1, fk[0].Columns.Length);
			Assertion.AssertEquals (1, fk[1].Columns.Length);
			Assertion.AssertEquals (column_1, fk[0].Columns[0]);
			Assertion.AssertEquals (column_3, fk[1].Columns[0]);
			
			table = new DbTable ("Test");
			
			table.Columns.Add (column_2);
			table.Columns.Add (column_1);
			
			fk = table.ForeignKeys;
			
			Assertion.AssertEquals (1, fk.Length);
			Assertion.AssertEquals (1, fk[0].Columns.Length);
			Assertion.AssertEquals (column_1, fk[0].Columns[0]);
			
			Assertion.AssertEquals (table, table.Columns[column_1.Name].Table);
			Assertion.AssertEquals (table, table.Columns[column_2.Name].Table);
			
			table.Columns.Remove (column_2);
			
			Assertion.AssertNull (table.Columns[column_2.Name]);
			Assertion.AssertNull (column_2.Table);
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckNewDbTableEx1()
		{
			DbTable table = new DbTable ("Test");
			
			table.DefineCategory (DbElementCat.Internal);
			table.DefineCategory (DbElementCat.UserDataExternal);
		}
		
		
		[Test] public void CheckCreateSqlTable()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
			DbColumn column_e = new DbColumn ("e/x", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
			DbTable db_table = new DbTable ("Test (1)");
			
			db_table.DefineInternalKey (new DbKey (9));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			column_c.DefineCategory (DbElementCat.UserDataManaged);
			column_d.DefineCategory (DbElementCat.UserDataManaged);
			column_e.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.PrimaryKeys.AddRange (new DbColumn[] { column_a, column_b });
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b, column_c, column_d, column_e });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			Assert.IsNotNull (sql_table);
			Assert.AreEqual ("U_TEST__1__9", sql_table.Name);
			Assert.AreEqual (db_table.Columns.Count, sql_table.Columns.Count);
			Assert.AreEqual (db_table.PrimaryKeys.Count, sql_table.PrimaryKey.Length);
			Assert.AreEqual ("U_A", sql_table.Columns[0].Name);
			Assert.AreEqual ("U_E_X", sql_table.Columns[4].Name);
		}
		
		[Test] [ExpectedException (typeof (DbSyntaxException))] public void CheckCreateSqlTableEx1()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
			DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
			DbTable db_table = new DbTable ("Test");
			
			db_table.DefineInternalKey (new DbKey (0));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			column_c.DefineCategory (DbElementCat.UserDataManaged);
			column_d.DefineCategory (DbElementCat.UserDataManaged);
			column_e.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.PrimaryKeys.AddRange (new DbColumn[] { column_a, column_b });
			db_table.Columns.AddRange (new DbColumn[] { column_b, column_c, column_d, column_e });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			//	exception: colonnes a et b ne font pas partie de la table
		}
		
		[Test] [ExpectedException (typeof (DbSyntaxException))] public void CheckCreateSqlTableEx2()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32), Nullable.Yes);
			DbColumn column_b = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
			DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
			DbTable db_table = new DbTable ("Test");
			
			db_table.DefineInternalKey (new DbKey (0));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			column_c.DefineCategory (DbElementCat.UserDataManaged);
			column_d.DefineCategory (DbElementCat.UserDataManaged);
			column_e.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.PrimaryKeys.AddRange (new DbColumn[] { column_a, column_b });
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b, column_c, column_d, column_e });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			//	exception: colonne A est nullable
		}
		
		[Test] [ExpectedException (typeof (DbSyntaxException))] public void CheckCreateSqlTableEx3()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a  = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b1 = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_b2 = new DbColumn ("B", DbSimpleType.String, 50, false, Nullable.Yes);
			
			DbTable db_table = new DbTable ("Test");
			
			db_table.DefineInternalKey (new DbKey (0));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b1.DefineCategory (DbElementCat.UserDataManaged);
			column_b2.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b1, column_b2 });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			//	exception: colonne b � double
		}
	}
}
