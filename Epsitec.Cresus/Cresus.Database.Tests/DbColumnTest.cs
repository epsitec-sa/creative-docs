using NUnit.Framework; 

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbColumnTest
	{
		[Test] public void CheckNewDbColumn()
		{
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			DbColumn column_b = new DbColumn ("B", DbSimpleType.Guid);
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 100, true);
			DbColumn column_x = DbColumn.CreateColumn ("<col null='Y' idx='Y' uniq='N' cat='1'/>");
			
			Assertion.AssertEquals ("A", column_a.Name);
			Assertion.AssertEquals (DbSimpleType.Decimal, column_a.SimpleType);
			Assertion.AssertEquals (true, column_a.IsNullAllowed);
			Assertion.AssertEquals (DbElementCat.Unknown, column_a.Category);
			
			Assertion.AssertEquals ("B", column_b.Name);
			Assertion.AssertEquals (DbSimpleType.Guid, column_b.SimpleType);
			
			Assertion.AssertEquals ("C", column_c.Name);
			Assertion.AssertEquals (DbSimpleType.String, column_c.SimpleType);
			Assertion.AssertEquals (100, column_c.Length);
			Assertion.AssertEquals (true, column_c.IsFixedLength);
			
			Assertion.AssertNotNull (column_x);
			Assertion.AssertEquals (true,  column_x.IsNullAllowed);
			Assertion.AssertEquals (true,  column_x.IsIndexed);
			Assertion.AssertEquals (false, column_x.IsUnique);
			Assertion.AssertEquals (DbElementCat.Internal, column_x.Category);
			Assertion.AssertEquals (DbRevisionMode.Unknown, column_x.RevisionMode);
			
			column_a.DefineCategory (DbElementCat.Internal);
			
			Assertion.AssertEquals (DbElementCat.Internal, column_a.Category);
			
			column_a.DefineCategory (DbElementCat.Internal);
		}
		
		[Test] public void CheckNewDbColumnByteArray()
		{
			DbColumn column = new DbColumn ("A", DbSimpleType.ByteArray);
			
			Assertion.AssertEquals ("A", column.Name);
			Assertion.AssertEquals (DbSimpleType.ByteArray, column.SimpleType);
		}
		
		[Test] public void CheckNewDbColumnForeignKey()
		{
			DbColumn column_1 = DbColumn.CreateRefColumn ("A", "ParentTable", DbColumnClass.RefId, new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForId)), Nullable.Yes);
			
			Assertion.AssertEquals ("A", column_1.Name);
			Assertion.AssertEquals ("ParentTable", column_1.ParentTableName);
			Assertion.AssertEquals ("CR_ID", column_1.ParentColumnName);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckNewDbColumnForeignKeyEx()
		{
			DbColumn column = DbColumn.CreateRefColumn ("A", "ParentTable", DbColumnClass.Data, new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForId)), Nullable.Yes);
			
			System.Console.Out.WriteLine (column.ParentColumnName);
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckNewDbColumnEx1()
		{
			DbColumn column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			
			column.DefineCategory (DbElementCat.Internal);
			column.DefineCategory (DbElementCat.UserDataManaged);
		}
		
		[Test] public void CheckSerializeToXml()
		{
			DbColumn column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			
			column.IsIndexed = true;
			column.DefineCategory (DbElementCat.UserDataManaged);
			column.DefineRevisionMode (DbRevisionMode.Disabled);
			
			System.Console.Out.WriteLine ("XML/1: " + DbColumn.SerializeToXml (column, true));
			
			column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.No);
			
			column.IsIndexed = false;
			column.DefineCategory (DbElementCat.UserDataManaged);
			column.DefineRevisionMode (DbRevisionMode.Enabled);
			
			System.Console.Out.WriteLine ("XML/2: " + DbColumn.SerializeToXml (column, true));
		}
		
		[Test] public void CheckTypes()
		{
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbSimpleType.String, 100, true);
			DbColumn column_c = new DbColumn ("C", DbSimpleType.Time);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.ByteArray);
			
			Assertion.Assert (column_a.Type.GetType () == typeof (DbTypeNum));
			Assertion.Assert (column_b.Type.GetType () == typeof (DbTypeString));
			Assertion.Assert (column_c.Type.GetType () == typeof (DbType));
			Assertion.Assert (column_d.Type.GetType () == typeof (DbTypeByteArray));
		}
		
		[Test] public void CheckCreateSqlColumn()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			DbColumn column_b = new DbColumn ("B", DbSimpleType.Guid);
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 100, true);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.ByteArray);
			
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			column_c.DefineCategory (DbElementCat.UserDataManaged);
			column_d.DefineCategory (DbElementCat.UserDataManaged);
			
			SqlColumn sql_a = column_a.CreateSqlColumn (type_converter);
			SqlColumn sql_b = column_b.CreateSqlColumn (type_converter);
			SqlColumn sql_c = column_c.CreateSqlColumn (type_converter);
			SqlColumn sql_d = column_d.CreateSqlColumn (type_converter);
			
			Assertion.AssertEquals (DbRawType.SmallDecimal, sql_a.Type);
			Assertion.AssertEquals ("U_A", sql_a.Name);
			Assertion.AssertEquals (true, sql_a.IsNullAllowed);
			
			Assertion.Assert (DbRawType.Guid != sql_b.Type);
			Assertion.AssertEquals ("U_B", sql_b.Name);
			Assertion.AssertEquals (false, sql_b.IsNullAllowed);
			Assertion.Assert (sql_b.HasRawConverter);
			Assertion.AssertEquals (sql_b.RawConverter.InternalType, sql_b.Type);
			Assertion.AssertEquals (sql_b.RawConverter.ExternalType, TypeConverter.MapToRawType (column_b.SimpleType, column_b.NumDef));
			
			Assertion.AssertEquals (DbRawType.String, sql_c.Type);
			Assertion.AssertEquals ("U_C", sql_c.Name);
			Assertion.AssertEquals (true, sql_c.IsFixedLength);
			Assertion.AssertEquals (false, sql_c.IsNullAllowed);
			Assertion.AssertEquals (false, sql_c.HasRawConverter);

			Assertion.AssertEquals (DbRawType.ByteArray, sql_d.Type);
			Assertion.AssertEquals ("U_D", sql_d.Name);
			Assertion.AssertEquals (false, sql_d.IsNullAllowed);
			Assertion.AssertEquals (false, sql_d.HasRawConverter);

			System.Console.Out.WriteLine ("Column {0} raw type is {1}, length={2}, fixed={3}.", sql_b.Name, sql_b.Type, sql_b.Length, sql_b.IsFixedLength);
			System.Console.Out.WriteLine ("Raw Converter between type {0} and {1}.", sql_b.RawConverter.InternalType, sql_b.RawConverter.ExternalType);
			
			object guid_object = new System.Guid (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
			
			Assertion.Assert (guid_object is System.Guid);
			
			sql_b.ConvertToInternalType (ref guid_object);
			
			System.Console.Out.WriteLine ("Converted GUID object to string {0}.", guid_object);
			
			Assertion.Assert (guid_object is string);
			
			sql_b.ConvertFromInternalType (ref guid_object);
			
			Assertion.Assert (guid_object is System.Guid);
			
			System.Console.Out.WriteLine ("Converted string back to GUID {0}.", guid_object);
		}
	}
}
