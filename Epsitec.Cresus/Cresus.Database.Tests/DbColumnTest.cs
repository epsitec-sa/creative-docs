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
			DbColumn column_x = DbColumn.CreateColumn ("<col null='1' index='1' unique='0' cat='1'/>");
			
			Assert.AreEqual ("A", column_a.Name);
			Assert.AreEqual (DbSimpleType.Decimal, column_a.SimpleType);
			Assert.AreEqual (true, column_a.IsNullAllowed);
			Assert.AreEqual (DbElementCat.Unknown, column_a.Category);
			
			Assert.AreEqual ("B", column_b.Name);
			Assert.AreEqual (DbSimpleType.Guid, column_b.SimpleType);
			
			Assert.AreEqual ("C", column_c.Name);
			Assert.AreEqual (DbSimpleType.String, column_c.SimpleType);
			Assert.AreEqual (100, column_c.Length);
			Assert.AreEqual (true, column_c.IsFixedLength);
			
			Assert.IsNotNull (column_x);
			Assert.AreEqual (true,  column_x.IsNullAllowed);
			Assert.AreEqual (true,  column_x.IsIndexed);
			Assert.AreEqual (false, column_x.IsUnique);
			Assert.AreEqual (DbElementCat.Internal, column_x.Category);
			
			column_a.DefineCategory (DbElementCat.Internal);
			
			Assert.AreEqual (DbElementCat.Internal, column_a.Category);
			
			column_a.DefineCategory (DbElementCat.Internal);
		}
		
		[Test] public void CheckNewDbColumnByteArray()
		{
			DbColumn column = new DbColumn ("A", DbSimpleType.ByteArray);
			
			Assert.AreEqual ("A", column.Name);
			Assert.AreEqual (DbSimpleType.ByteArray, column.SimpleType);
		}
		
		[Test] public void CheckNewDbColumnForeignKey()
		{
			DbColumn column_1 = DbColumn.CreateRefColumn ("A", "ParentTable", DbColumnClass.RefSimpleId, new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForId)), Nullable.Yes);
			DbColumn column_2 = DbColumn.CreateRefColumn ("B", "ParentTable", DbColumnClass.RefTupleRevision, new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForRevision)), Nullable.Yes);
			
			Assert.AreEqual ("A", column_1.Name);
			Assert.AreEqual ("ParentTable", column_1.ParentTableName);
			Assert.AreEqual ("CR_ID", column_1.ParentColumnName);
			
			Assert.AreEqual ("B", column_2.Name);
			Assert.AreEqual ("ParentTable", column_2.ParentTableName);
			Assert.AreEqual ("CR_REV", column_2.ParentColumnName);
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
			
			System.Console.Out.WriteLine ("XML: " + DbColumn.SerializeToXml (column, true));
		}
		
		[Test] public void CheckTypes()
		{
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbSimpleType.String, 100, true);
			DbColumn column_c = new DbColumn ("C", DbSimpleType.Time);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.ByteArray);
			
			Assert.IsTrue (column_a.Type.GetType () == typeof (DbTypeNum));
			Assert.IsTrue (column_b.Type.GetType () == typeof (DbTypeString));
			Assert.IsTrue (column_c.Type.GetType () == typeof (DbType));
			Assert.IsTrue (column_d.Type.GetType () == typeof (DbTypeByteArray));
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
			
			Assert.AreEqual (DbRawType.SmallDecimal, sql_a.Type);
			Assert.AreEqual ("U_A", sql_a.Name);
			Assert.AreEqual (true, sql_a.IsNullAllowed);
			
			Assert.IsTrue (DbRawType.Guid != sql_b.Type);
			Assert.AreEqual ("U_B", sql_b.Name);
			Assert.AreEqual (false, sql_b.IsNullAllowed);
			Assert.IsTrue (sql_b.HasRawConverter);
			Assert.AreEqual (sql_b.RawConverter.InternalType, sql_b.Type);
			Assert.AreEqual (sql_b.RawConverter.ExternalType, TypeConverter.MapToRawType (column_b.SimpleType, column_b.NumDef));
			
			Assert.AreEqual (DbRawType.String, sql_c.Type);
			Assert.AreEqual ("U_C", sql_c.Name);
			Assert.AreEqual (true, sql_c.IsFixedLength);
			Assert.AreEqual (false, sql_c.IsNullAllowed);
			Assert.AreEqual (false, sql_c.HasRawConverter);

			Assert.AreEqual (DbRawType.ByteArray, sql_d.Type);
			Assert.AreEqual ("U_D", sql_d.Name);
			Assert.AreEqual (false, sql_d.IsNullAllowed);
			Assert.AreEqual (false, sql_d.HasRawConverter);

			System.Console.Out.WriteLine ("Column {0} raw type is {1}, length={2}, fixed={3}.", sql_b.Name, sql_b.Type, sql_b.Length, sql_b.IsFixedLength);
			System.Console.Out.WriteLine ("Raw Converter between type {0} and {1}.", sql_b.RawConverter.InternalType, sql_b.RawConverter.ExternalType);
		}
	}
}
