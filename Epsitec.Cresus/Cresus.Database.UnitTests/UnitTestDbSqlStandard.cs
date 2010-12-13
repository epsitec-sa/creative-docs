using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Exceptions;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestDbSqlStandard
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void ValidateNameTest()
		{
			Assert.IsTrue (DbSqlStandard.ValidateName (@"ABC_123"));
			Assert.IsFalse (DbSqlStandard.ValidateName (@"ABC-123"));
			Assert.IsFalse (DbSqlStandard.ValidateName (@"123_ABC"));
			Assert.IsFalse (DbSqlStandard.ValidateName (@"ABC:ABC"));
			Assert.IsTrue (DbSqlStandard.ValidateName (@"""ABC-123"""));
			Assert.IsTrue (DbSqlStandard.ValidateName (@"""123_ABC"""));
			Assert.IsTrue (DbSqlStandard.ValidateName (@"""ABC:ABC"""));
			Assert.IsFalse ( DbSqlStandard.ValidateName (@"""ABC""ABC"""));
		}


		[TestMethod]
		public void ValidateStringTest()
		{
			Assert.IsTrue (DbSqlStandard.ValidateString (@"'ABC_123'"));
			Assert.IsFalse (DbSqlStandard.ValidateString (@"ABC-123"));
			Assert.IsFalse (DbSqlStandard.ValidateString (@"'123'ABC'"));
			Assert.IsTrue (DbSqlStandard.ValidateString (@"'123''ABC'"));
		}


		[TestMethod]
		public void ValidateNumberTest()
		{
			Assert.IsTrue (DbSqlStandard.ValidateNumber (@"123"));
			Assert.IsTrue (DbSqlStandard.ValidateNumber (@"-123"));
			Assert.IsFalse (DbSqlStandard.ValidateNumber (@"123_123"));
			Assert.IsFalse (DbSqlStandard.ValidateNumber (@"A123"));
			Assert.IsTrue (DbSqlStandard.ValidateNumber (@"123.456"));
			Assert.IsTrue (DbSqlStandard.ValidateNumber (@"-123.4"));
			Assert.IsFalse (DbSqlStandard.ValidateNumber (@"12.34.56"));
			Assert.IsFalse (DbSqlStandard.ValidateNumber (@"12E3"));
		}


		[TestMethod]
		public void ValidateQualifiedNameTest()
		{
			Assert.IsFalse (DbSqlStandard.ValidateQualifiedName (@"ABC_123"));
			Assert.IsTrue (DbSqlStandard.ValidateQualifiedName (@"AB.C123"));
			Assert.IsFalse (DbSqlStandard.ValidateQualifiedName (@"123.ABC"));
			Assert.IsFalse (DbSqlStandard.ValidateQualifiedName (@"AB.AB.AB"));
			Assert.IsTrue (DbSqlStandard.ValidateQualifiedName (@"""ABC.123"".XYZ"));
			Assert.IsTrue (DbSqlStandard.ValidateQualifiedName (@"ABC.""123.ABC"""));
			Assert.IsTrue (DbSqlStandard.ValidateQualifiedName (@"""AB.AB.AB"".AB"));
			Assert.IsFalse (DbSqlStandard.ValidateQualifiedName (@"""ABC""ABC"".XY"));
		}


		[TestMethod]
		public void ConcatNamesTest()
		{
			string	name;

			name = DbSqlStandard.ConcatNames ("ABC", "XYZ");
			Assert.AreEqual ("ABCXYZ", name);
			
			name = DbSqlStandard.ConcatNames ("ABC", @"""XYZ""");
			Assert.AreEqual (@"""ABCXYZ""", name);
			
			name = DbSqlStandard.ConcatNames (@"""ABC""", "XYZ");
			Assert.AreEqual (@"""ABCXYZ""", name);
		}


		[TestMethod]
		public void ConcatStringsTest()
		{
			string name;

			name = DbSqlStandard.ConcatStrings ("'ABC'", "'123'");
			Assert.AreEqual ("'ABC123'", name);
		}


		[TestMethod]
		public void QualifyNameTest()
		{
			string name;

			name = DbSqlStandard.QualifyName ("ABC", "XYZ");
			Assert.AreEqual ("ABC.XYZ", name);

			name = DbSqlStandard.QualifyName (@"""ABC""", "XYZ");
			Assert.AreEqual (@"""ABC"".XYZ", name);

			name = DbSqlStandard.QualifyName ("ABC", @"""XYZ""");
			Assert.AreEqual (@"ABC.""XYZ""", name);
		}


		[TestMethod]
		public void SplitQualifiedNameTest()
		{
			string qualifier;
			string name;

			DbSqlStandard.SplitQualifiedName ("ABC.X12", out qualifier, out name);
			Assert.AreEqual ("ABC", qualifier);
			Assert.AreEqual ("X12", name);

			DbSqlStandard.SplitQualifiedName ("\"ABC.XY\".Y12", out qualifier, out name);
			Assert.AreEqual ("\"ABC.XY\"", qualifier);
			Assert.AreEqual ("Y12", name);
		}


		[TestMethod]
		public void CreateSqlTableNameException()
		{
			string name = "test-x";

			ExceptionAssert.Throw<GenericException>
			(
				() => DbSqlStandard.MakeSqlTableName (name, true, DbElementCat.Internal, new DbKey (123))
			);
		}


		[TestMethod]
		public void CreateSqlTableNameTest()
		{
			string name1 = "test-x";
			string name2 = "CR_TEST";

			string result1 = DbSqlStandard.MakeSqlTableName (name1, true, DbElementCat.ManagedUserData, new DbKey (123));
			string result2 = DbSqlStandard.MakeSqlTableName (name2, true, DbElementCat.Internal, DbKey.Empty);
			string result3 = DbSqlStandard.MakeSqlTableName (name1, false, DbElementCat.ManagedUserData, new DbKey (123));

			Assert.AreEqual ("MUD_TEST_X$123", result1);
			Assert.AreEqual ("CR_TEST", result2);
			Assert.AreEqual ("MUD_TEST_X", result3);
		}


	}


}
