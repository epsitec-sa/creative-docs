using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class RequestFactoryTest
	{
		[Test]
		public void Check01NewAndDispose()
		{
			RequestFactory factory = new RequestFactory ();

			factory.Dispose ();
		}

		[Test]
		public void Check02GenerateRequests()
		{
			RequestFactory factory = new RequestFactory ();

			System.Data.DataTable table = new System.Data.DataTable ("Test Table");

			System.Data.DataColumn col_id   = new System.Data.DataColumn (Database.Tags.ColumnId, typeof (long));
			col_id.AllowDBNull = false;
			col_id.Unique = true;
			System.Data.DataColumn col_stat = new System.Data.DataColumn (Database.Tags.ColumnStatus, typeof (short));
			col_stat.AllowDBNull = false;
			System.Data.DataColumn col_a    = new System.Data.DataColumn ("Column A", typeof (long));
			col_a.AllowDBNull = false;
			System.Data.DataColumn col_b    = new System.Data.DataColumn ("Column B", typeof (string));
			col_b.AllowDBNull = false;
			System.Data.DataColumn col_c    = new System.Data.DataColumn ("Column C", typeof (decimal));
			col_c.AllowDBNull = true;

			table.Columns.Add (col_id);
			table.Columns.Add (col_stat);
			table.Columns.Add (col_a);
			table.Columns.Add (col_b);
			table.Columns.Add (col_c);

			table.Rows.Add (new object[] { Database.DbId.CreateId (1, 1).Value, 0, 1L, "Item X", 10.50M });
			table.Rows.Add (new object[] { Database.DbId.CreateId (2, 1).Value, 0, 2L, "Item Y", System.DBNull.Value });

			factory.GenerateRequests (table);

			Assert.AreEqual (2, factory.PendingRequests.Count);

			Requests.AbstractRequest req_1 = factory.PendingRequests[0] as Requests.AbstractRequest;
			Requests.AbstractRequest req_2 = factory.PendingRequests[1] as Requests.AbstractRequest;

			Assert.AreEqual (Requests.RequestType.InsertStaticData, req_1.RequestType);
			Assert.AreEqual (Requests.RequestType.InsertStaticData, req_2.RequestType);

			table.AcceptChanges ();

			factory.Clear ();

			Assert.AreEqual (0, factory.PendingRequests.Count);

			factory.GenerateRequests (table);

			Assert.AreEqual (0, factory.PendingRequests.Count);

			table.Rows[1].BeginEdit ();
			table.Rows[1][col_c] = 25M;
			table.Rows[1].EndEdit ();

			factory.Clear ();
			factory.GenerateRequests (table);

			Assert.AreEqual (1, factory.PendingRequests.Count);

			Requests.AbstractRequest req_3 = factory.PendingRequests[0] as Requests.AbstractRequest;

			Assert.AreEqual (Requests.RequestType.UpdateStaticData, req_3.RequestType);

			Requests.UpdateStaticData update;
			update = req_3 as Requests.UpdateStaticData;

			Assert.AreEqual (table.TableName, update.TableName);
			Assert.AreEqual (2, update.ColumnNames.Length);
			Assert.AreEqual (col_id.ColumnName, update.ColumnNames[0]);
			Assert.AreEqual (col_c.ColumnName, update.ColumnNames[1]);
			Assert.AreEqual (1000000000002L, update.ColumnValues[0]);
			Assert.AreEqual (1000000000002L, update.ColumnOriginalValues[0]);
			Assert.AreEqual (System.DBNull.Value, update.ColumnOriginalValues[1]);
			Assert.AreEqual (25M, update.ColumnValues[1]);

			factory.Dispose ();
		}

		[Test]
		[ExpectedException (typeof (Database.Exceptions.InvalidIdException))]
		public void Check03GenerateRequestsEx()
		{
			RequestFactory factory = new RequestFactory ();

			System.Data.DataTable table = new System.Data.DataTable ("Test Table");

			System.Data.DataColumn col_id   = new System.Data.DataColumn (Database.Tags.ColumnId, typeof (long));
			col_id.AllowDBNull = false;
			col_id.Unique = true;
			System.Data.DataColumn col_stat = new System.Data.DataColumn (Database.Tags.ColumnStatus, typeof (short));
			col_stat.AllowDBNull = false;
			System.Data.DataColumn col_a    = new System.Data.DataColumn ("Column A", typeof (long));
			col_a.AllowDBNull = false;
			System.Data.DataColumn col_b    = new System.Data.DataColumn ("Column B", typeof (string));
			col_b.AllowDBNull = false;
			System.Data.DataColumn col_c    = new System.Data.DataColumn ("Column C", typeof (decimal));
			col_c.AllowDBNull = true;

			table.Columns.Add (col_id);
			table.Columns.Add (col_stat);
			table.Columns.Add (col_a);
			table.Columns.Add (col_b);
			table.Columns.Add (col_c);

			table.Rows.Add (new object[] { Database.DbId.CreateId (1, 1).Value, 0, 1L, "Item X", 10.50M });
			table.Rows.Add (new object[] { Database.DbId.CreateTempId (555).Value, 0, 2L, "Item Y", System.DBNull.Value });

			factory.GenerateRequests (table);

			factory.Dispose ();
		}

		[Test]
		[ExpectedException (typeof (Database.Exceptions.InvalidIdException))]
		public void Check04GenerateRequestsEx()
		{
			RequestFactory factory = new RequestFactory ();

			System.Data.DataTable table = new System.Data.DataTable ("Test Table");

			System.Data.DataColumn col_id   = new System.Data.DataColumn (Database.Tags.ColumnId, typeof (long));
			col_id.AllowDBNull = false;
			col_id.Unique = true;
			System.Data.DataColumn col_stat = new System.Data.DataColumn (Database.Tags.ColumnStatus, typeof (short));
			col_stat.AllowDBNull = false;
			System.Data.DataColumn col_a    = new System.Data.DataColumn ("Column A", typeof (long));
			col_a.AllowDBNull = false;
			System.Data.DataColumn col_b    = new System.Data.DataColumn ("Column B", typeof (string));
			col_b.AllowDBNull = false;
			System.Data.DataColumn col_c    = new System.Data.DataColumn ("Column C", typeof (decimal));
			col_c.AllowDBNull = true;

			table.Columns.Add (col_id);
			table.Columns.Add (col_stat);
			table.Columns.Add (col_a);
			table.Columns.Add (col_b);
			table.Columns.Add (col_c);

			table.Rows.Add (new object[] { Database.DbId.CreateId (1, 1).Value, 0, 1L, "Item X", 10.50M });
			table.Rows.Add (new object[] { 0L, 0, 2L, "Item Y", System.DBNull.Value });

			factory.GenerateRequests (table);

			factory.Dispose ();
		}
	}
}
