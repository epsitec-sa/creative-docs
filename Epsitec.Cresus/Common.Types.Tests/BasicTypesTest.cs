using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture] public class BasicTypesTest
	{
		[Test] public void CheckEnumType()
		{
			EnumType et = new EnumType (typeof (MyEnum));
			
			Assertion.AssertEquals (5, et.Values.Length);
			
			Assertion.AssertEquals ("Extra",  et.Values[0].Name);
			Assertion.AssertEquals ("First",  et.Values[1].Name);
			Assertion.AssertEquals ("None",   et.Values[2].Name);
			Assertion.AssertEquals ("Second", et.Values[3].Name);
			Assertion.AssertEquals ("Third",  et.Values[4].Name);
			
			Assertion.AssertEquals (-1, et["None"]  .Rank);
			Assertion.AssertEquals ( 1, et["First"] .Rank);
			Assertion.AssertEquals ( 2, et["Second"].Rank);
			Assertion.AssertEquals ( 3, et["Third"] .Rank);
			Assertion.AssertEquals (99, et["Extra"] .Rank);
			
			Assertion.AssertEquals ("None"  , et[-1].Name);
			Assertion.AssertEquals ("First" , et[ 1].Name);
			Assertion.AssertEquals ("Second", et[ 2].Name);
			Assertion.AssertEquals ("Third" , et[ 3].Name);
			Assertion.AssertEquals ("Extra" , et[99].Name);
		}
		
		private enum MyEnum
		{
			None	= -1,
			First	=  1,
			Second	=  2,
			Third	=  3,
			Extra	= 99
		}
	}
}
