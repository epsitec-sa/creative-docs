using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTableTest
	{
		[Test] public void CheckCreateSqlTable()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
			DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
			DbTable db_table = new DbTable ("Test");
			
			db_table.PrimaryKey = new DbColumn[] { column_a, column_b };
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b, column_c, column_d, column_e });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			Assertion.AssertNotNull (sql_table);
			Assertion.AssertEquals (db_table.Name, sql_table.Name);
			Assertion.AssertEquals (db_table.Columns.Count, sql_table.Columns.Count);
			Assertion.AssertEquals (db_table.PrimaryKey.Length, sql_table.PrimaryKey.Length);
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
			
			db_table.PrimaryKey = new DbColumn[] { column_a, column_b };
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
			
			db_table.PrimaryKey = new DbColumn[] { column_a, column_b };
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
			
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b1, column_b2 });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			//	exception: colonne b à double
		}
	}
}
