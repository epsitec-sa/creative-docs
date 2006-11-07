using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbSqlStandardTest
	{
		[Test] public void CheckValidateName()
		{
			Assert.AreEqual (true,  DbSqlStandard.ValidateName (@"ABC_123"));
			Assert.AreEqual (false, DbSqlStandard.ValidateName (@"ABC-123"));
			Assert.AreEqual (false, DbSqlStandard.ValidateName (@"123_ABC"));
			Assert.AreEqual (false, DbSqlStandard.ValidateName (@"ABC:ABC"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateName (@"""ABC-123"""));
			Assert.AreEqual (true,  DbSqlStandard.ValidateName (@"""123_ABC"""));
			Assert.AreEqual (true,  DbSqlStandard.ValidateName (@"""ABC:ABC"""));
			Assert.AreEqual (false, DbSqlStandard.ValidateName (@"""ABC""ABC"""));
		}
		
		[Test] public void CheckValidateString()
		{
			Assert.AreEqual (true,  DbSqlStandard.ValidateString (@"'ABC_123'"));
			Assert.AreEqual (false, DbSqlStandard.ValidateString (@"ABC-123"));
			Assert.AreEqual (false, DbSqlStandard.ValidateString (@"'123'ABC'"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateString (@"'123''ABC'"));
		}
		
		[Test] public void CheckValidateNumber()
		{
			Assert.AreEqual (true,  DbSqlStandard.ValidateNumber (@"123"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateNumber (@"-123"));
			Assert.AreEqual (false, DbSqlStandard.ValidateNumber (@"123_123"));
			Assert.AreEqual (false, DbSqlStandard.ValidateNumber (@"A123"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateNumber (@"123.456"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateNumber (@"-123.4"));
			Assert.AreEqual (false, DbSqlStandard.ValidateNumber (@"12.34.56"));
			Assert.AreEqual (false, DbSqlStandard.ValidateNumber (@"12E3"));
		}
		
		[Test] public void CheckValidateQualifiedName()
		{
			Assert.AreEqual (false, DbSqlStandard.ValidateQualifiedName (@"ABC_123"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateQualifiedName (@"AB.C123"));
			Assert.AreEqual (false, DbSqlStandard.ValidateQualifiedName (@"123.ABC"));
			Assert.AreEqual (false, DbSqlStandard.ValidateQualifiedName (@"AB.AB.AB"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateQualifiedName (@"""ABC.123"".XYZ"));
			Assert.AreEqual (true,  DbSqlStandard.ValidateQualifiedName (@"ABC.""123.ABC"""));
			Assert.AreEqual (true,  DbSqlStandard.ValidateQualifiedName (@"""AB.AB.AB"".AB"));
			Assert.AreEqual (false, DbSqlStandard.ValidateQualifiedName (@"""ABC""ABC"".XY"));
		}
		
		[Test] public void CheckConcatNames()
		{
			string	name;
			name = DbSqlStandard.ConcatNames("ABC", "XYZ");
			Assert.AreEqual ("ABCXYZ", name);
			name = DbSqlStandard.ConcatNames("ABC", @"""XYZ""");
			Assert.AreEqual (@"""ABCXYZ""", name);
			name = DbSqlStandard.ConcatNames(@"""ABC""", "XYZ");
			Assert.AreEqual (@"""ABCXYZ""", name);
		}
		
		[Test] public void CheckConcatStrings()
		{
			string name = DbSqlStandard.ConcatStrings("'ABC'", "'123'");
			Assert.AreEqual ("'ABC123'", name);
		}
		
		[Test] public void CheckQualifyName()
		{
			string name = DbSqlStandard.QualifyName ("ABC" , "XYZ");
			Assert.AreEqual ("ABC.XYZ", name);

			Assert.AreEqual (@"""ABC"".XYZ", DbSqlStandard.QualifyName (@"""ABC""", "XYZ"));
			Assert.AreEqual (@"ABC.""XYZ""", DbSqlStandard.QualifyName ("ABC", @"""XYZ"""));
		}
		
		[Test] public void CheckSplitQualifiedName()
		{
			string	qualifier, name;
			DbSqlStandard.SplitQualifiedName ("ABC.X12", out qualifier, out name);
			Assert.AreEqual ("ABC", qualifier);
			Assert.AreEqual ("X12", name);

			DbSqlStandard.SplitQualifiedName ("\"ABC.XY\".Y12", out qualifier, out name);
			Assert.AreEqual ("\"ABC.XY\"", qualifier);
			Assert.AreEqual ("Y12", name);
		}
		
#if false
		[Test] public void CheckCreateSqlTableName()
		{
			string nam1 = "test-x";
			string nam2 = "CR_TEST";
			string std1 = DbSqlStandard.MakeSqlTableName (nam1, DbElementCat.UserDataManaged, new DbKey (123));
			string std2 = DbSqlStandard.MakeSqlTableName (nam2, DbElementCat.Internal, null);
			
			Assert.AreEqual ("U_TEST_X_123", std1);
			Assert.AreEqual ("CR_TEST",      std2);
		}
#endif
		
		[Test] [ExpectedException (typeof (Exceptions.GenericException))] public void CheckCreateSqlTableNameEx1()
		{
			string name = "test-x";
			string std  = DbSqlStandard.MakeSqlTableName (name, DbElementCat.Internal, new DbKey (123));
			
			Assert.AreEqual ("U_TEST_X_123", std);
		}
	}
}
