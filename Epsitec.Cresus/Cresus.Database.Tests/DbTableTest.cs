using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTableTest
	{
		[Test] public void CheckNewDbTable()
		{
			DbTable table = new DbTable ("Test");
			
			Assertion.AssertNotNull (table);
			Assertion.AssertEquals ("Test", table.Name);
			Assertion.AssertEquals (DbElementCat.Unknown, table.Category);
			Assertion.AssertEquals (false, table.HasPrimaryKeys);
			Assertion.AssertEquals (0, table.PrimaryKeys.Count);
			Assertion.AssertEquals (0, table.Columns.Count);
			
			table.DefineCategory (DbElementCat.Internal);
			
			Assertion.AssertEquals (DbElementCat.Internal, table.Category);
			
			table.DefineCategory (DbElementCat.Internal);
		}
		
		[Test] public void CheckSerialiseToXml()
		{
			DbTable table = new DbTable ("Test");
			DbColumn column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			
			column.IsIndexed = true;
			column.DefineCategory (DbElementCat.UserDataManaged);
			
			table.PrimaryKeys.Add (column);
			table.Columns.Add (column);
			
			string xml = DbTable.SerialiseToXml (table, true);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			
			DbTable test = DbTable.NewTable (xml);
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
			DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
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
			
			Assertion.AssertNotNull (sql_table);
			Assertion.AssertEquals ("U_TEST__1__9", sql_table.Name);
			Assertion.AssertEquals (db_table.Columns.Count, sql_table.Columns.Count);
			Assertion.AssertEquals (db_table.PrimaryKeys.Count, sql_table.PrimaryKey.Length);
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
			
			//	exception: colonne b à double
		}
	}
}
