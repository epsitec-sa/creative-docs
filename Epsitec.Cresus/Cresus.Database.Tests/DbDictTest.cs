using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbDictTest
	{
		[Test] public void Check01CreateDict()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", true))
			{
				Assert.IsNotNull (infrastructure);
				
				DbDict.CreateTable (infrastructure, null, "TestDict");
			}
		}
	}
}
