using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SqlColumnTest
	{
		[Test] public void CheckNewSqlColumn()
		{
			SqlColumn sql_col_a = new SqlColumn ("A", DbRawType.Int32);
			SqlColumn sql_col_b = new SqlColumn ("B", DbRawType.Int64, DbNullability.Yes);
			SqlColumn sql_col_c = new SqlColumn ("C", DbRawType.String, 100, false, DbNullability.Undefined);
			SqlColumn sql_col_d = new SqlColumn ("D", DbRawType.ByteArray, DbNullability.Yes);
			
			Assert.AreEqual ("A",				sql_col_a.Name);
			Assert.AreEqual (DbRawType.Int32,	sql_col_a.Type);
			Assert.AreEqual (false,				sql_col_a.IsNullable);
			Assert.AreEqual (1,					sql_col_a.Length);
			Assert.AreEqual (true,				sql_col_a.IsFixedLength);
			
			Assert.AreEqual ("B",				sql_col_b.Name);
			Assert.AreEqual (DbRawType.Int64,	sql_col_b.Type);
			Assert.AreEqual (true,				sql_col_b.IsNullable);
			Assert.AreEqual (1,					sql_col_b.Length);
			Assert.AreEqual (true,				sql_col_b.IsFixedLength);
			
			Assert.AreEqual ("C",				sql_col_c.Name);
			Assert.AreEqual (DbRawType.String,	sql_col_c.Type);
			Assert.AreEqual (false,				sql_col_c.IsNullable);
			Assert.AreEqual (100,				sql_col_c.Length);
			Assert.AreEqual (false,				sql_col_c.IsFixedLength);
			
			Assert.AreEqual ("D",				sql_col_d.Name);
			Assert.AreEqual (DbRawType.ByteArray,sql_col_d.Type);
			Assert.AreEqual (true,				sql_col_d.IsNullable);
			Assert.AreEqual (1,					sql_col_d.Length);
			Assert.AreEqual (true,				sql_col_d.IsFixedLength);
		}

		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))] public void CheckEx1()
		{
			SqlColumn sql_col = new SqlColumn ();
			sql_col.SetType (DbRawType.Int32, 2, true, DbCharacterEncoding.Unicode);
		}

		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))] public void CheckEx2()
		{
			SqlColumn sql_col = new SqlColumn ();
			sql_col.SetType (DbRawType.Int32, 1, false, DbCharacterEncoding.Unicode);
		}

		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))] public void CheckEx3()
		{
			SqlColumn sql_col = new SqlColumn ();
			sql_col.SetType (DbRawType.String, 0, false, DbCharacterEncoding.Unicode);
		}
	}
}
