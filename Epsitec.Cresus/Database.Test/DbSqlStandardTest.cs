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
		
		[Test] [Ignore ("Not implemented")] public void CheckValidateString()
		{
			//	TODO: � faire
		}
		
		[Test] [Ignore ("Not implemented")] public void CheckValidateNumber()
		{
			//	TODO: � faire
		}
		
		[Test] [Ignore ("Not implemented")] public void CheckValidateQualifiedName()
		{
			//	TODO: � faire
		}
		
		[Test] [Ignore ("Not implemented")] public void CheckConcatNames()
		{
			//	TODO: � faire
		}
		
		[Test] [Ignore ("Not implemented")] public void CheckConcatStrings()
		{
			//	TODO: � faire
		}
		
		[Test] [Ignore ("Not implemented")] public void CheckQualifyName()
		{
			//	TODO: � faire
		}
		
		[Test] [Ignore ("Not implemented")] public void CheckSplitQualifiedName()
		{
			//	TODO: � faire
		}
	}
}
