using NUnit.Framework;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

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
			Assertion.AssertEquals (Requests.Type.Group, group.Type);
			
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
		
		[Test] public void Check04Types()
		{
			Requests.Base req1 = new Requests.Group ();
			Requests.Base req2 = new Requests.InsertStaticData ();
			Requests.Base req3 = new Requests.UpdateStaticData ();
			Requests.Base req4 = new Requests.UpdateDynamicData ();
			
			Assertion.AssertEquals (Requests.Type.Group, req1.Type);
			Assertion.AssertEquals (Requests.Type.InsertStaticData, req2.Type);
			Assertion.AssertEquals (Requests.Type.UpdateStaticData, req3.Type);
			Assertion.AssertEquals (Requests.Type.UpdateDynamicData, req4.Type);
		}
		
		[Test] public void Check05Serialization()
		{
			try
			{
				System.IO.File.Delete ("test-requests.bin");
			}
			catch {}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test-requests.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				
				Requests.Group            group = new Requests.Group ();
				Requests.InsertStaticData req_1 = new Requests.InsertStaticData ();
				Requests.InsertStaticData req_2 = new Requests.InsertStaticData ();
				
				req_1.DefineTableName ("Table Abc");
				req_2.DefineTableName ("Table Xyz");
				
				group.Add (req_1);
				group.Add (req_2);
				
				formatter.Serialize (stream, group);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test-requests.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				Requests.Group group = formatter.Deserialize (stream) as Requests.Group;
				
				Assertion.AssertEquals (2, group.Count);
				
				Assertion.AssertEquals (Requests.Type.InsertStaticData, group[0].Type);
				Assertion.AssertEquals (Requests.Type.InsertStaticData, group[1].Type);
				
				Requests.InsertStaticData req_1 = group[0] as Requests.InsertStaticData;
				Requests.InsertStaticData req_2 = group[1] as Requests.InsertStaticData;
				
				Assertion.AssertEquals ("Table Abc", req_1.TableName);
				Assertion.AssertEquals ("Table Xyz", req_2.TableName);
			}
			
			
		}
	}
}
