using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Exceptions;

using Epsitec.Cresus.DataLayer;

using Epsitec.Cresus.Requests;

using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestRequestFactory
	{

		[TestMethod]
		public void Check01NewAndDispose()
		{
			using (RequestFactory factory = new RequestFactory ())
			{
			}
		}


		[TestMethod]
		public void Check02GenerateRequests()
		{
			RequestFactory factory = new RequestFactory ();

			using (System.Data.DataTable table = new System.Data.DataTable ("Test Table"))
			{
				System.Data.DataColumn colId = new System.Data.DataColumn (Tags.ColumnId, typeof (long))
				{
					AllowDBNull = false,
					Unique = true
				};

				System.Data.DataColumn colStat = new System.Data.DataColumn (Tags.ColumnStatus, typeof (short))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colA = new System.Data.DataColumn ("Column A", typeof (long))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colB = new System.Data.DataColumn ("Column B", typeof (string))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colC = new System.Data.DataColumn ("Column C", typeof (decimal))
				{
					AllowDBNull = true
				};

				table.Columns.Add (colId);
				table.Columns.Add (colStat);
				table.Columns.Add (colA);
				table.Columns.Add (colB);
				table.Columns.Add (colC);

				table.Rows.Add (new object[] { DbId.CreateId (1, 1).Value, 0, 1L, "Item X", 10.50M });
				table.Rows.Add (new object[] { DbId.CreateId (2, 1).Value, 0, 2L, "Item Y", System.DBNull.Value });
				
				factory.GenerateRequests (table);
				
				Assert.AreEqual (2, factory.PendingRequests.Count ());
				
				AbstractRequest req1 = factory.PendingRequests.ElementAt (0) as Requests.AbstractRequest;
				AbstractRequest req2 = factory.PendingRequests.ElementAt (1) as Requests.AbstractRequest;
				
				Assert.AreEqual (req1.GetType (), typeof (InsertStaticDataRequest));
				Assert.AreEqual (req2.GetType (), typeof (InsertStaticDataRequest));
				
				table.AcceptChanges ();
				
				factory.Clear ();
				
				Assert.AreEqual (0, factory.PendingRequests.Count ());
				
				factory.GenerateRequests (table);
				
				Assert.AreEqual (0, factory.PendingRequests.Count ());
				
				table.Rows[1].BeginEdit ();
				table.Rows[1][colC] = 25M;
				table.Rows[1].EndEdit ();
				
				factory.Clear ();
				
				factory.GenerateRequests (table);
				
				Assert.AreEqual (1, factory.PendingRequests.Count ());
				
				Requests.AbstractRequest req3 = factory.PendingRequests.ElementAt (0) as Requests.AbstractRequest;
				
				Assert.AreEqual (req3.GetType (), typeof (UpdateStaticDataRequest));
				
				UpdateStaticDataRequest update;
				
				update = req3 as UpdateStaticDataRequest;
				
				Assert.AreEqual (table.TableName, update.TableName);
				Assert.AreEqual (2, update.ColumnNames.Count);
				Assert.AreEqual (colId.ColumnName, update.ColumnNames[0]);
				Assert.AreEqual (colC.ColumnName, update.ColumnNames[1]);
				Assert.AreEqual (1000000000002L, update.ColumnValues[0]);
				Assert.AreEqual (1000000000002L, update.ColumnOriginalValues[0]);
				Assert.AreEqual (System.DBNull.Value, update.ColumnOriginalValues[1]);
				Assert.AreEqual (25M, update.ColumnValues[1]);
			}

			factory.Dispose ();
		}


		[TestMethod]
		[ExpectedException (typeof (InvalidIdException))]
		public void Check03GenerateRequestsEx()
		{
			RequestFactory factory = new RequestFactory ();

			using (System.Data.DataTable table = new System.Data.DataTable ("Test Table"))
			{

				System.Data.DataColumn colId = new System.Data.DataColumn (Tags.ColumnId, typeof (long))
				{
					AllowDBNull = false,
					Unique = true
				};

				System.Data.DataColumn colStat = new System.Data.DataColumn (Tags.ColumnStatus, typeof (short))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colA = new System.Data.DataColumn ("Column A", typeof (long))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colB = new System.Data.DataColumn ("Column B", typeof (string))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colC = new System.Data.DataColumn ("Column C", typeof (decimal))
				{
					AllowDBNull = true
				};

				table.Columns.Add (colId);
				table.Columns.Add (colStat);
				table.Columns.Add (colA);
				table.Columns.Add (colB);
				table.Columns.Add (colC);

				table.Rows.Add (new object[] { DbId.CreateId (1, 1).Value, 0, 1L, "Item X", 10.50M });
				table.Rows.Add (new object[] { DbId.CreateTempId (555).Value, 0, 2L, "Item Y", System.DBNull.Value });

				factory.GenerateRequests (table);
			}

			factory.Dispose ();
		}


		[TestMethod]
		[ExpectedException (typeof (InvalidIdException))]
		public void Check04GenerateRequestsEx()
		{
			RequestFactory factory = new RequestFactory ();

			using (System.Data.DataTable table = new System.Data.DataTable ("Test Table"))
			{
				System.Data.DataColumn colId = new System.Data.DataColumn (Tags.ColumnId, typeof (long))
				{
					AllowDBNull = false,
					Unique = true
				};

				System.Data.DataColumn colStat = new System.Data.DataColumn (Tags.ColumnStatus, typeof (short))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colA = new System.Data.DataColumn ("Column A", typeof (long))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colB = new System.Data.DataColumn ("Column B", typeof (string))
				{
					AllowDBNull = false
				};

				System.Data.DataColumn colC = new System.Data.DataColumn ("Column C", typeof (decimal))
				{
					AllowDBNull = true
				};

				table.Columns.Add (colId);
				table.Columns.Add (colStat);
				table.Columns.Add (colA);
				table.Columns.Add (colB);
				table.Columns.Add (colC);

				table.Rows.Add (new object[] { DbId.CreateId (1, 1).Value, 0, 1L, "Item X", 10.50M });
				table.Rows.Add (new object[] { 0L, 0, 2L, "Item Y", System.DBNull.Value });

				factory.GenerateRequests (table);
			}

			factory.Dispose ();
		}


	}


}
