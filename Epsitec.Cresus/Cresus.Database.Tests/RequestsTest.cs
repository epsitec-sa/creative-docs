using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class RequestsTest
	{
		[TestFixtureSetUp] public void Setup()
		{
		}
		
		[TestFixtureTearDown] public void TearDown()
		{
		}
		
		
		[Test] public void Check01Group()
		{
			Requests.Group group = new Requests.Group ();
			
			Assertion.AssertEquals (0, group.Count);
			
			group.AddRange (null);
			group.AddRange (new object[] { });
			
			Assertion.AssertEquals (0, group.Count);
			
			Requests.Base req = new Requests.Group ();
			
			group.Add (req);
			
			Assertion.AssertEquals (1, group.Count);
			Assertion.AssertEquals (req, group[0]);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentNullException))] public void Check02GroupEx()
		{
			Requests.Group group = new Requests.Group ();
			
			Assertion.AssertEquals (0, group.Count);
			
			group.Add (null);
		}
		
		[Test] [ExpectedException (typeof (System.IndexOutOfRangeException))] public void Check03GroupEx()
		{
			Requests.Group group = new Requests.Group ();
			
			Assertion.AssertEquals (0, group.Count);
			
			Requests.Base req = group[0];
		}
	}
}
