using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class RequestFactoryTest
	{
		[Test] public void Check01NewAndDispose()
		{
			RequestFactory factory = new RequestFactory ();
			
			factory.Dispose ();
		}
		
		[Test] public void Check02GenerateRequests()
		{
			RequestFactory factory = new RequestFactory ();
			
			System.Data.DataTable table = new System.Data.DataTable ("Test Table");
			
			System.Data.DataColumn col_a = new System.Data.DataColumn ("Column A", typeof (long)); col_a.Unique = true; col_a.AllowDBNull = false;
			System.Data.DataColumn col_b = new System.Data.DataColumn ("Column B", typeof (string)); col_b.AllowDBNull = false;
			System.Data.DataColumn col_c = new System.Data.DataColumn ("Column C", typeof (decimal)); col_c.AllowDBNull = true;
			
			table.Columns.Add (col_a);
			table.Columns.Add (col_b);
			table.Columns.Add (col_c);
			
			table.Rows.Add (new object[] { 1L, "Item X", 10.50M });
			table.Rows.Add (new object[] { 2L, "Item Y", System.DBNull.Value });
			
			factory.GenerateRequests (table);
			
			Assertion.AssertEquals (2, factory.PendingRequests.Count);
			
			Requests.AbstractRequest req_1 = factory.PendingRequests[0] as Requests.AbstractRequest;
			Requests.AbstractRequest req_2 = factory.PendingRequests[1] as Requests.AbstractRequest;
			
			Assertion.AssertEquals (Requests.RequestType.InsertStaticData, req_1.RequestType);
			Assertion.AssertEquals (Requests.RequestType.InsertStaticData, req_2.RequestType);
			
			table.AcceptChanges ();
			
			factory.Clear ();
			
			Assertion.AssertEquals (0, factory.PendingRequests.Count);
			
			factory.GenerateRequests (table);
			
			Assertion.AssertEquals (0, factory.PendingRequests.Count);
			
			table.Rows[1].BeginEdit ();
			table.Rows[1][col_c] = 25M;
			table.Rows[1].EndEdit ();
			
			factory.Clear ();
			factory.GenerateRequests (table);
			
			Assertion.AssertEquals (1, factory.PendingRequests.Count);
			
			Requests.AbstractRequest req_3 = factory.PendingRequests[0] as Requests.AbstractRequest;
			
			Assertion.AssertEquals (Requests.RequestType.UpdateStaticData, req_3.RequestType);
			
			Requests.UpdateStaticData update;
			update = req_3 as Requests.UpdateStaticData;
			
			Assertion.AssertEquals (table.TableName, update.TableName);
			Assertion.AssertEquals (2, update.ColumnNames.Length);
			Assertion.AssertEquals (col_a.ColumnName, update.ColumnNames[0]);
			Assertion.AssertEquals (col_c.ColumnName, update.ColumnNames[1]);
			Assertion.AssertEquals (2L, update.ColumnValues[0]);
			Assertion.AssertEquals (2L, update.ColumnOriginalValues[0]);
			Assertion.AssertEquals (System.DBNull.Value, update.ColumnOriginalValues[1]);
			Assertion.AssertEquals (25M, update.ColumnValues[1]);
			
			factory.Dispose ();
		}
	}
}
