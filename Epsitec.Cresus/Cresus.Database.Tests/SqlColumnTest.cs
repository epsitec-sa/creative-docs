using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SqlColumnTest
	{
		[Test] public void CheckNewSqlColumn()
		{
			SqlColumn sql_col_a = new SqlColumn ("A", DbRawType.Int32);
			SqlColumn sql_col_b = new SqlColumn ("B", DbRawType.Int64, Nullable.Yes);
			SqlColumn sql_col_c = new SqlColumn ("C", DbRawType.String, 100, false, Nullable.Undefined);
			SqlColumn sql_col_d = new SqlColumn ("D", DbRawType.ByteArray, 50, true, Nullable.Yes);
			
			Assertion.AssertEquals ("A",				sql_col_a.Name);
			Assertion.AssertEquals (DbRawType.Int32,	sql_col_a.Type);
			Assertion.AssertEquals (false,				sql_col_a.IsNullAllowed);
			Assertion.AssertEquals (1,					sql_col_a.Length);
			Assertion.AssertEquals (true,				sql_col_a.IsFixedLength);
			
			Assertion.AssertEquals ("B",				sql_col_b.Name);
			Assertion.AssertEquals (DbRawType.Int64,	sql_col_b.Type);
			Assertion.AssertEquals (true,				sql_col_b.IsNullAllowed);
			Assertion.AssertEquals (1,					sql_col_b.Length);
			Assertion.AssertEquals (true,				sql_col_b.IsFixedLength);
			
			Assertion.AssertEquals ("C",				sql_col_c.Name);
			Assertion.AssertEquals (DbRawType.String,	sql_col_c.Type);
			Assertion.AssertEquals (false,				sql_col_c.IsNullAllowed);
			Assertion.AssertEquals (100,				sql_col_c.Length);
			Assertion.AssertEquals (false,				sql_col_c.IsFixedLength);
			
			Assertion.AssertEquals ("D",				sql_col_d.Name);
			Assertion.AssertEquals (DbRawType.ByteArray,sql_col_d.Type);
			Assertion.AssertEquals (true,				sql_col_d.IsNullAllowed);
			Assertion.AssertEquals (50,					sql_col_d.Length);
			Assertion.AssertEquals (true,				sql_col_d.IsFixedLength);
		}

		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))] public void CheckEx1()
		{
			SqlColumn sql_col = new SqlColumn ();
			sql_col.SetType (DbRawType.Int32, 2, true);
		}

		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))] public void CheckEx2()
		{
			SqlColumn sql_col = new SqlColumn ();
			sql_col.SetType (DbRawType.Int32, 1, false);
		}

		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))] public void CheckEx3()
		{
			SqlColumn sql_col = new SqlColumn ();
			sql_col.SetType (DbRawType.String, 0, false);
		}
	}
}
