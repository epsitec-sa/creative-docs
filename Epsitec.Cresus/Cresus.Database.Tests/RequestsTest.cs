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
				
				System.Data.DataTable table = RequestsTest.CreateSampleTable ();
				
				Requests.Group            group = new Requests.Group ();
				Requests.InsertStaticData req_1 = new Requests.InsertStaticData (table.Rows[0]);
				Requests.InsertStaticData req_2 = new Requests.InsertStaticData (table.Rows[1]);
				
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
				
				Assertion.AssertEquals ("DemoTable", req_1.TableName);
				Assertion.AssertEquals ("DemoTable", req_2.TableName);
				
				Assertion.AssertEquals (1L, req_1.ColumnValues[0]);
				Assertion.AssertEquals (2L, req_2.ColumnValues[0]);
				
				Assertion.AssertEquals ("Pierre Arnaud", req_1.ColumnValues[1]);
				Assertion.AssertEquals ("Jérôme André",  req_2.ColumnValues[1]);
				
				Assertion.AssertEquals (1972, req_1.ColumnValues[2]);
				Assertion.AssertEquals (1994, req_2.ColumnValues[2]);
			}
		}
		
		public static System.Data.DataTable CreateSampleTable()
		{
			System.Data.DataSet   set   = new System.Data.DataSet ();
			System.Data.DataTable table = new System.Data.DataTable ("DemoTable");
			
			set.Tables.Add (table);
			
			System.Data.DataColumn col_1 = new System.Data.DataColumn ("ID", typeof (long));
			System.Data.DataColumn col_2 = new System.Data.DataColumn ("Name", typeof (string));
			System.Data.DataColumn col_3 = new System.Data.DataColumn ("Birth Year", typeof (int));
			
			table.Columns.Add (col_1);
			table.Columns.Add (col_2);
			table.Columns.Add (col_3);
			
			table.Rows.Add (new object[] { 1L, "Pierre Arnaud", 1972 });
			table.Rows.Add (new object[] { 2L, "Jérôme André",  1994 });
			
			return table;
		}
	}
}
