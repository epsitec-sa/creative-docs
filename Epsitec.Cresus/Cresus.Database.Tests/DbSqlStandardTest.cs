using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbSqlStandardTest
	{
		[Test] public void CheckValidateName()
		{
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateName (@"ABC_123"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateName (@"ABC-123"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateName (@"123_ABC"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateName (@"ABC:ABC"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateName (@"""ABC-123"""));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateName (@"""123_ABC"""));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateName (@"""ABC:ABC"""));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateName (@"""ABC""ABC"""));
		}
		
		[Test] public void CheckValidateString()
		{
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateString (@"'ABC_123'"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateString (@"ABC-123"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateString (@"'123'ABC'"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateString (@"'123''ABC'"));
		}
		
		[Test] public void CheckValidateNumber()
		{
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateNumber (@"123"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateNumber (@"-123"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateNumber (@"123_123"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateNumber (@"A123"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateNumber (@"123.456"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateNumber (@"-123.4"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateNumber (@"12.34.56"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateNumber (@"12E3"));
		}
		
		[Test] public void CheckValidateQualifiedName()
		{
			Assertion.AssertEquals (false, DbSqlStandard.ValidateQualifiedName (@"ABC_123"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateQualifiedName (@"AB.C123"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateQualifiedName (@"123.ABC"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateQualifiedName (@"AB.AB.AB"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateQualifiedName (@"""ABC.123"".XYZ"));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateQualifiedName (@"ABC.""123.ABC"""));
			Assertion.AssertEquals (true,  DbSqlStandard.ValidateQualifiedName (@"""AB.AB.AB"".AB"));
			Assertion.AssertEquals (false, DbSqlStandard.ValidateQualifiedName (@"""ABC""ABC"".XY"));
		}
		
		[Test] public void CheckConcatNames()
		{
			string	name;
			name = DbSqlStandard.ConcatNames("ABC", "XYZ");
			Assertion.AssertEquals ("ABCXYZ", name);
			name = DbSqlStandard.ConcatNames("ABC", @"""XYZ""");
			Assertion.AssertEquals (@"""ABCXYZ""", name);
			name = DbSqlStandard.ConcatNames(@"""ABC""", "XYZ");
			Assertion.AssertEquals (@"""ABCXYZ""", name);
		}
		
		[Test] public void CheckConcatStrings()
		{
			string name = DbSqlStandard.ConcatStrings("'ABC'", "'123'");
			Assertion.AssertEquals ("'ABC123'", name);
		}
		
		[Test] public void CheckQualifyName()
		{
			string name = DbSqlStandard.QualifyName ("ABC" , "XYZ");
			Assertion.AssertEquals ("ABC.XYZ", name);
		}
		
		[Test] public void CheckSplitQualifiedName()
		{
			string	qualifier, name;
			DbSqlStandard.SplitQualifiedName ("ABC.X12", out qualifier, out name);
			Assertion.AssertEquals ("ABC", qualifier);
			Assertion.AssertEquals ("X12", name);

			DbSqlStandard.SplitQualifiedName ("\"ABC.XY\".Y12", out qualifier, out name);
			Assertion.AssertEquals ("\"ABC.XY\"", qualifier);
			Assertion.AssertEquals ("Y12", name);
		}
		
		[Test] public void CheckCreateSqlTableName()
		{
			string nam1 = "test-x";
			string nam2 = "CR_TEST";
			string std1 = DbSqlStandard.CreateSqlTableName (nam1, DbElementCat.UserDataManaged, new DbKey (123));
			string std2 = DbSqlStandard.CreateSqlTableName (nam2, DbElementCat.Internal, null);
			
			Assertion.AssertEquals ("U_TEST_X_123", std1);
			Assertion.AssertEquals ("CR_TEST",      std2);
		}
		
		[Test] [ExpectedException (typeof (DbException))] public void CheckCreateSqlTableNameEx1()
		{
			string name = "test-x";
			string std  = DbSqlStandard.CreateSqlTableName (name, DbElementCat.Internal, new DbKey (123));
			
			Assertion.AssertEquals ("U_TEST_X_123", std);
		}
	}
}
