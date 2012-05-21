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
			SqlColumn sqlColumnA = new SqlColumn ("A", DbRawType.Int32);
			SqlColumn sqlColumnB = new SqlColumn ("B", DbRawType.Int64, DbNullability.Yes);
			SqlColumn sqlColumnC = new SqlColumn ("C", DbRawType.String, 100, false, DbNullability.Undefined);
			SqlColumn sqlColumnD = new SqlColumn ("D", DbRawType.ByteArray, DbNullability.Yes);
			
			Assert.AreEqual ("A", sqlColumnA.Name);
			Assert.AreEqual (DbRawType.Int32,	sqlColumnA.Type);
			Assert.AreEqual (false, sqlColumnA.IsNullable);
			Assert.AreEqual (1, sqlColumnA.Length);
			Assert.AreEqual (true, sqlColumnA.IsFixedLength);
			
			Assert.AreEqual ("B", sqlColumnB.Name);
			Assert.AreEqual (DbRawType.Int64, sqlColumnB.Type);
			Assert.AreEqual (true, sqlColumnB.IsNullable);
			Assert.AreEqual (1, sqlColumnB.Length);
			Assert.AreEqual (true, sqlColumnB.IsFixedLength);
			
			Assert.AreEqual ("C", sqlColumnC.Name);
			Assert.AreEqual (DbRawType.String, sqlColumnC.Type);
			Assert.AreEqual (false, sqlColumnC.IsNullable);
			Assert.AreEqual (100, sqlColumnC.Length);
			Assert.AreEqual (false, sqlColumnC.IsFixedLength);
			
			Assert.AreEqual ("D", sqlColumnD.Name);
			Assert.AreEqual (DbRawType.ByteArray, sqlColumnD.Type);
			Assert.AreEqual (true, sqlColumnD.IsNullable);
			Assert.AreEqual (1, sqlColumnD.Length);
			Assert.AreEqual (true, sqlColumnD.IsFixedLength);
		}


		[TestMethod]
		public void SetTypeArgumetCheck()
		{
			SqlColumn sqlColumn = new SqlColumn ();
			
			ExceptionAssert.Throw<System.ArgumentOutOfRangeException>
			(
				() => sqlColumn.SetType (DbRawType.Int32, 2, true, DbCharacterEncoding.Unicode)
			);

			ExceptionAssert.Throw<System.ArgumentOutOfRangeException>
			(
				() => sqlColumn.SetType (DbRawType.Int32, 1, false, DbCharacterEncoding.Unicode)
			);

			ExceptionAssert.Throw<System.ArgumentOutOfRangeException>
			(
				() => sqlColumn.SetType (DbRawType.String, 0, false, DbCharacterEncoding.Unicode)
			);
		}


	}


}
