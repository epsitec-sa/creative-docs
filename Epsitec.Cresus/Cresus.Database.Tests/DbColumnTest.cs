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
			DbColumn column_x = DbColumn.NewColumn ("<col null='1' index='1' unique='0' cat='1'/>");
			
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
			
			column_a.DefineCategory (DbElementCat.Internal);
			
			Assertion.AssertEquals (DbElementCat.Internal, column_a.Category);
			
			column_a.DefineCategory (DbElementCat.Internal);
		}
		
		[Test] [Ignore ("Not implemented")] public void CheckNewDbColumnByteArray()
		{
			DbColumn column_d = new DbColumn ("D", DbSimpleType.ByteArray, 10, false);
			Assertion.AssertEquals ("D", column_d.Name);
			Assertion.AssertEquals (DbSimpleType.ByteArray, column_d.SimpleType);
			Assertion.AssertEquals (10, column_d.Length);
			Assertion.AssertEquals (false, column_d.IsFixedLength);
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckNewDbColumnEx1()
		{
			DbColumn column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			
			column.DefineCategory (DbElementCat.Internal);
			column.DefineCategory (DbElementCat.UserDataManaged);
		}
		
		[Test] public void CheckSerialiseToXml()
		{
			DbColumn column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			
			column.IsIndexed = true;
			column.DefineCategory (DbElementCat.UserDataManaged);
			
			System.Console.Out.WriteLine ("XML: " + DbColumn.SerialiseToXml (column, true));
		}
		
		[Test] public void CheckTypes()
		{
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbSimpleType.String, 100, true);
			DbColumn column_c = new DbColumn ("C", DbSimpleType.Time);
			
			Assertion.Assert (column_a.Type.GetType () == typeof (DbTypeNum));
			Assertion.Assert (column_b.Type.GetType () == typeof (DbTypeString));
			Assertion.Assert (column_c.Type.GetType () == typeof (DbType));
		}
		
		[Test] public void CheckCreateSqlColumn()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			DbColumn column_b = new DbColumn ("B", DbSimpleType.Guid);
			
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			
			SqlColumn sql_a = column_a.CreateSqlColumn (type_converter);
			SqlColumn sql_b = column_b.CreateSqlColumn (type_converter);
			
			Assertion.AssertEquals (DbRawType.SmallDecimal, sql_a.Type);
			Assertion.AssertEquals ("U_A", sql_a.Name);
			Assertion.AssertEquals (true, sql_a.IsNullAllowed);
			
			Assertion.Assert (DbRawType.Guid != sql_b.Type);
			Assertion.AssertEquals ("U_B", sql_b.Name);
			Assertion.AssertEquals (false, sql_b.IsNullAllowed);
			Assertion.Assert (sql_b.HasRawConverter);
			Assertion.AssertEquals (sql_b.RawConverter.InternalType, sql_b.Type);
			Assertion.AssertEquals (sql_b.RawConverter.ExternalType, TypeConverter.MapToRawType (column_b.SimpleType, column_b.NumDef));
			
			System.Console.Out.WriteLine ("Column {0} raw type is {1}, length={2}, fixed={3}.", sql_b.Name, sql_b.Type, sql_b.Length, sql_b.IsFixedLength);
			System.Console.Out.WriteLine ("Raw Converter between type {0} and {1}.", sql_b.RawConverter.InternalType, sql_b.RawConverter.ExternalType);
		}
	}
}
