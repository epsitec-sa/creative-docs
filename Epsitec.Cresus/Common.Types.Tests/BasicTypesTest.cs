using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture] public class BasicTypesTest
	{
		[Test] public void CheckEnumType()
		{
			EnumType et = new EnumType (typeof (MyEnum));
			
			Assert.AreEqual (5, et.Values.Length);
			
			Assert.AreEqual ("None",   et.Values[0].Name);
			Assert.AreEqual ("First",  et.Values[1].Name);
			Assert.AreEqual ("Second", et.Values[2].Name);
			Assert.AreEqual ("Third",  et.Values[3].Name);
			Assert.AreEqual ("Extra",  et.Values[4].Name);
			
			Assert.AreEqual (-1, et["None"]  .Rank);
			Assert.AreEqual ( 1, et["First"] .Rank);
			Assert.AreEqual ( 2, et["Second"].Rank);
			Assert.AreEqual ( 3, et["Third"] .Rank);
			Assert.AreEqual (99, et["Extra"] .Rank);
			
			Assert.AreEqual ("None"  , et[-1].Name);
			Assert.AreEqual ("First" , et[ 1].Name);
			Assert.AreEqual ("Second", et[ 2].Name);
			Assert.AreEqual ("Third" , et[ 3].Name);
			Assert.AreEqual ("Extra" , et[99].Name);
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
