//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
