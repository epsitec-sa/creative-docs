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
			Assertion.AssertEquals (Requests.RequestType.Group, group.RequestType);
			
			group.AddRange (null);
			group.AddRange (new object[] { });
			
			Assertion.AssertEquals (0, group.Count);
			
			Requests.AbstractRequest req = new Requests.Group ();
			
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
			
			Requests.AbstractRequest req = group[0];
		}
		
		[Test] public void Check04Types()
		{
			Requests.AbstractRequest req1 = new Requests.Group ();
			Requests.AbstractRequest req2 = new Requests.InsertStaticData ();
			Requests.AbstractRequest req3 = new Requests.UpdateStaticData ();
			Requests.AbstractRequest req4 = new Requests.UpdateDynamicData ();
			
			Assertion.AssertEquals (Requests.RequestType.Group, req1.RequestType);
			Assertion.AssertEquals (Requests.RequestType.InsertStaticData, req2.RequestType);
			Assertion.AssertEquals (Requests.RequestType.UpdateStaticData, req3.RequestType);
			Assertion.AssertEquals (Requests.RequestType.UpdateDynamicData, req4.RequestType);
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
				
				Requests.Group group = new Requests.Group ();
				
				Requests.InsertStaticData req_1 = new Requests.InsertStaticData (table.Rows[0]);
				Requests.InsertStaticData req_2 = new Requests.InsertStaticData (table.Rows[1]);
				
				table.Rows[0].BeginEdit ();
				table.Rows[0][1] = "Pierre Arnaud-Bühlmann";
				table.Rows[0].EndEdit ();
				
				Requests.UpdateStaticData req_3 = new Requests.UpdateStaticData (table.Rows[0], Requests.UpdateMode.Changed);
				
				group.Add (req_1);
				group.Add (req_2);
				group.Add (req_3);
				
				Assertion.AssertEquals (2, req_3.ColumnNames.Length);
				Assertion.AssertEquals ("ID", req_3.ColumnNames[0]);
				Assertion.AssertEquals ("Name", req_3.ColumnNames[1]);
				
				formatter.Serialize (stream, group);
			}
			
			using (System.IO.Stream stream = System.IO.File.Open ("test-requests.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				Requests.Group group = formatter.Deserialize (stream) as Requests.Group;
				
				Assertion.AssertEquals (3, group.Count);
				
				Assertion.AssertEquals (Requests.RequestType.InsertStaticData, group[0].RequestType);
				Assertion.AssertEquals (Requests.RequestType.InsertStaticData, group[1].RequestType);
				Assertion.AssertEquals (Requests.RequestType.UpdateStaticData, group[2].RequestType);
				
				Requests.InsertStaticData req_1 = group[0] as Requests.InsertStaticData;
				Requests.InsertStaticData req_2 = group[1] as Requests.InsertStaticData;
				Requests.UpdateStaticData req_3 = group[2] as Requests.UpdateStaticData;
				
				Assertion.AssertEquals ("DemoTable", req_1.TableName);
				Assertion.AssertEquals ("DemoTable", req_2.TableName);
				
				Assertion.AssertEquals (1L, req_1.ColumnValues[0]);
				Assertion.AssertEquals (2L, req_2.ColumnValues[0]);
				
				Assertion.AssertEquals ("Pierre Arnaud", req_1.ColumnValues[1]);
				Assertion.AssertEquals ("Jérôme André",  req_2.ColumnValues[1]);
				
				Assertion.AssertEquals (1972, req_1.ColumnValues[2]);
				Assertion.AssertEquals (1994, req_2.ColumnValues[2]);
				
				Assertion.AssertEquals (2, req_3.ColumnNames.Length);
				Assertion.AssertEquals ("ID", req_3.ColumnNames[0]);
				Assertion.AssertEquals ("Name", req_3.ColumnNames[1]);
				Assertion.AssertEquals (1L, req_3.ColumnValues[0]);
				Assertion.AssertEquals ("Pierre Arnaud-Bühlmann", req_3.ColumnValues[1]);
			}
		}
		
		[Test] public void Check06UpdateStaticData()
		{
			System.Data.DataTable table = RequestsTest.CreateSampleTable ();
			
			Requests.UpdateStaticData req_1 = new Requests.UpdateStaticData (table.Rows[0], Requests.UpdateMode.Changed);
			
			table.Rows[0].BeginEdit ();
			table.Rows[0][1] = "Pierre Arnaud-Bühlmann";
			table.Rows[0].EndEdit ();
			
			Requests.UpdateStaticData req_2 = new Requests.UpdateStaticData (table.Rows[0], Requests.UpdateMode.Changed);
			Requests.UpdateStaticData req_3 = new Requests.UpdateStaticData (table.Rows[1], Requests.UpdateMode.Full);
			
			Assertion.Assert (req_1.ContainsData == false);
			Assertion.Assert (req_2.ContainsData == true);
			Assertion.Assert (req_3.ContainsData == true);
			
			Assertion.AssertEquals (2, req_2.ColumnNames.Length);
			Assertion.AssertEquals ("ID", req_2.ColumnNames[0]);
			Assertion.AssertEquals ("Name", req_2.ColumnNames[1]);
			Assertion.AssertEquals (1L, req_2.ColumnValues[0]);
			Assertion.AssertEquals ("Pierre Arnaud-Bühlmann", req_2.ColumnValues[1]);
			Assertion.AssertEquals ("Pierre Arnaud", req_2.ColumnOriginalValues[1]);
			
			Assertion.AssertEquals (3, req_3.ColumnNames.Length);
			Assertion.AssertEquals ("ID", req_3.ColumnNames[0]);
			Assertion.AssertEquals ("Name", req_3.ColumnNames[1]);
			Assertion.AssertEquals ("Birth Year", req_3.ColumnNames[2]);
			Assertion.AssertEquals (2L, req_3.ColumnValues[0]);
			Assertion.AssertEquals ("Jérôme André", req_3.ColumnValues[1]);
			Assertion.AssertEquals (1994, req_3.ColumnValues[2]);
		}
		
		[Test] public void Check07ExecutionQueue()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", true))
			{
				Assert.IsNotNull (infrastructure);
				
				Requests.ExecutionQueue queue = new Requests.ExecutionQueue (infrastructure);
				
				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					System.Data.DataTable table = RequestsTest.CreateSampleTable ();
					
					Requests.Group group = new Requests.Group ();
					
					Requests.InsertStaticData req_1 = new Requests.InsertStaticData (table.Rows[0]);
					Requests.InsertStaticData req_2 = new Requests.InsertStaticData (table.Rows[1]);
					
					table.Rows[0].BeginEdit ();
					table.Rows[0][1] = "Pierre Arnaud-Bühlmann";
					table.Rows[0].EndEdit ();
					
					Requests.UpdateStaticData req_3 = new Requests.UpdateStaticData (table.Rows[0], Requests.UpdateMode.Changed);
					
					group.Add (req_1);
					group.Add (req_2);
					group.Add (req_3);
					
					int n = queue.Rows.Count;
					
					System.Data.DataRow row_1 = queue.AddRequest (group);
					System.Data.DataRow row_2 = queue.Rows[n];
					
					Assertion.AssertEquals (row_1, row_2);
					Assertion.AssertEquals (n+1, queue.Rows.Count);
					Assertion.AssertEquals (DbIdClass.Temporary, DbId.AnalyzeClass ((long) row_1[Tags.ColumnId]));
					Assertion.AssertEquals (Requests.ExecutionState.Pending, queue.GetRequestExecutionState (row_1));
					
					queue.SerializeToBase (transaction);
					
					Assertion.AssertEquals (DbIdClass.Standard, DbId.AnalyzeClass ((long) row_1[Tags.ColumnId]));
					
					transaction.Commit ();
				}
				
				queue.Detach ();
				
				queue = new Requests.ExecutionQueue (infrastructure);
				
				System.Data.DataRowCollection rows = queue.Rows;
				
				foreach (System.Data.DataRow row in rows)
				{
					System.Diagnostics.Debug.WriteLine ("Row " + row[0] + " contains " + ((byte[])row[Tags.ColumnReqData]).Length + " bytes.");
				}
			}
		}
		
		[Test] public void Check08ExecutionEngine()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			Requests.ExecutionEngine engine = new Epsitec.Cresus.Requests.ExecutionEngine (infrastructure);
			
			Assertion.AssertEquals (0L, engine.CurrentLogId.Value);
			Assertion.AssertEquals (null, engine.CurrentTransaction);
			
			infrastructure.Logger.CreateTemporaryEntry (null);
			
			DbType db_type_name = infrastructure.ResolveDbType (null, "Customer Name");
			DbType db_type_date = infrastructure.ResolveDbType (null, "Birth Date");
			
			if (db_type_name == null)
			{
				db_type_name = infrastructure.CreateDbType ("Customer Name", 80, false);
				infrastructure.RegisterNewDbType (null, db_type_name);
			}
			
			if (db_type_date == null)
			{
				db_type_date = infrastructure.CreateDbTypeDateTime ("Birth Date");
				infrastructure.RegisterNewDbType (null, db_type_date);
			}
			
			DbTable db_table = infrastructure.CreateDbTable ("Simple Exec Table Test", DbElementCat.UserDataManaged, DbRevisionMode.Disabled);
			
			DbColumn db_col_1 = DbColumn.CreateUserDataColumn ("Name",       db_type_name, Nullable.No);
			DbColumn db_col_2 = DbColumn.CreateUserDataColumn ("Birth Date", db_type_date, Nullable.Yes);
			
			db_table.Columns.AddRange (new DbColumn[] { db_col_1, db_col_2 });
			
			infrastructure.RegisterNewDbTable (null, db_table);
			
			DbColumn db_col_id   = db_table.Columns[0];
			DbColumn db_col_stat = db_table.Columns[1];
			
			System.Data.DataSet   set   = new System.Data.DataSet ();
			System.Data.DataTable table = new System.Data.DataTable (db_table.Name);
			
			set.Tables.Add (table);
			
			System.Data.DataColumn col_1 = new System.Data.DataColumn (db_col_id.Name, typeof (long));
			System.Data.DataColumn col_2 = new System.Data.DataColumn (db_col_stat.Name, typeof (int));
			System.Data.DataColumn col_3 = new System.Data.DataColumn (db_col_1.Name, typeof (string));
			System.Data.DataColumn col_4 = new System.Data.DataColumn (db_col_2.Name, typeof (System.DateTime));
			
			col_1.Unique = true;
			
			table.Columns.Add (col_1);
			table.Columns.Add (col_2);
			table.Columns.Add (col_3);
			table.Columns.Add (col_4);
			
			DataLayer.RequestFactory factory = new DataLayer.RequestFactory ();
			
			table.Rows.Add (new object[] { 1L, 0, "Pierre Arnaud", new System.DateTime (1972, 2, 11) });
			
			factory.GenerateRequests (table);
			
			DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);
			
			engine.Execute (transaction, factory.CreateGroup ());
			
			table.AcceptChanges ();
			table.Rows[0][col_3.ColumnName] = "Pierre Arnaud-Roost";
			table.Rows[0][col_4.ColumnName] = new System.DateTime (1940, 5, 20);
			
			factory.Clear ();
			factory.GenerateRequests (table);
			
			engine.Execute (transaction, factory.CreateGroup ());
			
			transaction.Commit ();
			
			infrastructure.UnregisterDbTable (null, db_table);
		}
		
		
		
		public static System.Data.DataTable CreateSampleTable()
		{
			System.Data.DataSet   set   = new System.Data.DataSet ();
			System.Data.DataTable table = new System.Data.DataTable ("DemoTable");
			
			set.Tables.Add (table);
			
			System.Data.DataColumn col_1 = new System.Data.DataColumn ("ID", typeof (long));
			System.Data.DataColumn col_2 = new System.Data.DataColumn ("Name", typeof (string));
			System.Data.DataColumn col_3 = new System.Data.DataColumn ("Birth Year", typeof (int));
			
			col_1.Unique = true;
			
			table.Columns.Add (col_1);
			table.Columns.Add (col_2);
			table.Columns.Add (col_3);
			
			table.Rows.Add (new object[] { 1L, "Pierre Arnaud", 1972 });
			table.Rows.Add (new object[] { 2L, "Jérôme André",  1994 });
			
			table.AcceptChanges ();
			
			return table;
		}
	}
}
