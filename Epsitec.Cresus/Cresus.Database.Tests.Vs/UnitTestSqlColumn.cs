//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Database.Tests.Vs
{


	[TestClass]
	public sealed class UnitTestSqlColumn
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void ConstructorTest()
		{
			var sqlColumnA = new SqlColumn ("A", DbRawType.Int32);
			var sqlColumnB = new SqlColumn ("B", DbRawType.Int64, true);
			var sqlColumnC = new SqlColumn ("C", DbRawType.String, false, 100, false, DbCharacterEncoding.Unicode, DbCollation.UnicodeCiAi);
			var sqlColumnD = new SqlColumn ("D", DbRawType.ByteArray, true);
			
			Assert.AreEqual ("A", sqlColumnA.Name);
			Assert.AreEqual (DbRawType.Int32,	sqlColumnA.Type);
			Assert.AreEqual (false, sqlColumnA.IsNullable);
			Assert.AreEqual (1, sqlColumnA.Length);
			Assert.AreEqual (true, sqlColumnA.IsFixedLength);
			Assert.AreEqual (null, sqlColumnA.Encoding);
			Assert.AreEqual (null, sqlColumnA.Collation);
			
			Assert.AreEqual ("B", sqlColumnB.Name);
			Assert.AreEqual (DbRawType.Int64, sqlColumnB.Type);
			Assert.AreEqual (true, sqlColumnB.IsNullable);
			Assert.AreEqual (1, sqlColumnB.Length);
			Assert.AreEqual (true, sqlColumnB.IsFixedLength);
			Assert.AreEqual (null, sqlColumnB.Encoding);
			Assert.AreEqual (null, sqlColumnB.Collation);
			
			Assert.AreEqual ("C", sqlColumnC.Name);
			Assert.AreEqual (DbRawType.String, sqlColumnC.Type);
			Assert.AreEqual (false, sqlColumnC.IsNullable);
			Assert.AreEqual (100, sqlColumnC.Length);
			Assert.AreEqual (false, sqlColumnC.IsFixedLength);
			Assert.AreEqual (DbCharacterEncoding.Unicode, sqlColumnC.Encoding);
			Assert.AreEqual (DbCollation.UnicodeCiAi, sqlColumnC.Collation);
			
			Assert.AreEqual ("D", sqlColumnD.Name);
			Assert.AreEqual (DbRawType.ByteArray, sqlColumnD.Type);
			Assert.AreEqual (true, sqlColumnD.IsNullable);
			Assert.AreEqual (1, sqlColumnD.Length);
			Assert.AreEqual (true, sqlColumnD.IsFixedLength);
			Assert.AreEqual (null, sqlColumnD.Encoding);
			Assert.AreEqual (null, sqlColumnD.Collation);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentOutOfRangeException>
			(
				() => new SqlColumn ("", DbRawType.Int32, false, 2, true)
			);

			ExceptionAssert.Throw<System.ArgumentOutOfRangeException>
			(
				() => new SqlColumn ("", DbRawType.Int32, false, 1, false)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new SqlColumn ("", DbRawType.String, false, 0, false)
			);
		}


	}


}
