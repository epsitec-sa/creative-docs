using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class SqlTableTest
	{
		[Test] public void CheckNewTable()
		{
			SqlTable sql_table = new SqlTable ("Test");
			
			Assert.AreEqual ("Test", sql_table.Name);
		}
	}
}
