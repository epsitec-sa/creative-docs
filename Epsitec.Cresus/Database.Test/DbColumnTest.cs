using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbColumnTest
	{
		[SetUp] public void LoadAssemblies()
		{
			DbFactory.Initialise ();
		}
		
		[Test] public void CheckCreateSqlColumn()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ();
			DbColumn column_b = new DbColumn ();
			
			column_a.Name = "A";
			column_a.SetType (DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal));
			column_a.IsNullAllowed = true;
			
			column_b.Name = "B";
			column_b.SetType (DbSimpleType.Guid);
			
			SqlColumn sql_a = column_a.CreateSqlColumn (type_converter);
			SqlColumn sql_b = column_b.CreateSqlColumn (type_converter);
			
			Assertion.AssertEquals (DbRawType.SmallDecimal, sql_a.Type);
			Assertion.AssertEquals ("A", sql_a.Name);
			Assertion.AssertEquals (true, sql_a.IsNullAllowed);
			
			Assertion.Assert (DbRawType.Guid != sql_b.Type);
			Assertion.AssertEquals ("B", sql_b.Name);
			Assertion.AssertEquals (false, sql_b.IsNullAllowed);
			Assertion.Assert (sql_b.HasRawConverter);
			Assertion.AssertEquals (sql_b.RawConverter.InternalType, sql_b.Type);
			Assertion.AssertEquals (sql_b.RawConverter.ExternalType, TypeConverter.MapToRawType (column_b.SimpleType, column_b.NumDef));
			
			System.Console.Out.WriteLine ("Column {0} raw type is {1}, length={2}, fixed={3}.", sql_b.Name, sql_b.Type, sql_b.Length, sql_b.IsFixedLength);
			System.Console.Out.WriteLine ("Raw Converter between type {0} and {1}.", sql_b.RawConverter.InternalType, sql_b.RawConverter.ExternalType);
		}
	}
}
