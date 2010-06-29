using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Exceptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Runtime.Serialization.Formatters.Binary;
using Epsitec.Cresus.Services;


namespace Epsitec.Cresus.Requests
{

	
	/*
	 * The following code comes directly from the old test and has not been checked or tested. It
	 * is very likely to fail when run. If I have time to clean it once, I'll do it and I'll erase
	 * this comment. Marc
	 */


	[TestClass]
	public class UnitTestRequest
	{


#if false


		[TestMethod]
		public void Check01Group()
		{
			Requests.RequestCollection group = new Requests.RequestCollection ();

			Assert.AreEqual (0, group.Count);
			Assert.AreEqual ("RequestCollection", group.GetType ().Name);

			group.AddRange (null);
			group.AddRange (new Epsitec.Cresus.Requests.AbstractRequest[] { });

			Assert.AreEqual (0, group.Count);

			Requests.AbstractRequest req = new Requests.RequestCollection ();

			group.Add (req);

			Assert.AreEqual (1, group.Count);
			Assert.AreEqual (req, group[0]);
		}

		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void Check02GroupEx()
		{
			Requests.RequestCollection group = new Requests.RequestCollection ();

			Assert.AreEqual (0, group.Count);

			group.Add (null);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void Check03GroupEx()
		{
			Requests.RequestCollection group = new Requests.RequestCollection ();

			Assert.AreEqual (0, group.Count);

			Requests.AbstractRequest req = group[0];
		}


		[TestMethod]
		public void Check04Types()
		{
			Requests.AbstractRequest req1 = new Requests.RequestCollection ();
			Requests.AbstractRequest req2 = new Requests.InsertStaticDataRequest ();
			Requests.AbstractRequest req3 = new Requests.UpdateStaticDataRequest ();
			Requests.AbstractRequest req4 = new Requests.UpdateDynamicDataRequest ();

			Assert.AreEqual ("RequestCollection", req1.GetType ().Name);
			Assert.AreEqual ("InsertStaticDataRequest", req2.GetType ().Name);
			Assert.AreEqual ("UpdateStaticDataRequest", req3.GetType ().Name);
			Assert.AreEqual ("UpdateDynamicDataRequest", req4.GetType ().Name);
		}


		[TestMethod]
		public void Check05Serialization()
		{
			try
			{
				System.IO.File.Delete ("test-requests.bin");
			}
			catch
			{
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test-requests.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();

				System.Data.DataTable table = UnitTestRequest.CreateSampleTable ();

				Requests.RequestCollection group = new Requests.RequestCollection ();

				Requests.InsertStaticDataRequest req1 = new Requests.InsertStaticDataRequest (table.Rows[0]);
				Requests.InsertStaticDataRequest req2 = new Requests.InsertStaticDataRequest (table.Rows[1]);

				table.Rows[0].BeginEdit ();
				table.Rows[0][1] = "Pierre Arnaud-Bühlmann";
				table.Rows[0].EndEdit ();

				Requests.UpdateStaticDataRequest req3 = new Requests.UpdateStaticDataRequest (table.Rows[0], Requests.UpdateMode.Changed);

				group.Add (req1);
				group.Add (req2);
				group.Add (req3);

				Assert.AreEqual (2, req3.ColumnNames.Count);
				Assert.AreEqual ("ID", req3.ColumnNames[0]);
				Assert.AreEqual ("Name", req3.ColumnNames[1]);

				formatter.Serialize (stream, group);
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test-requests.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				Requests.RequestCollection group = formatter.Deserialize (stream) as Requests.RequestCollection;

				Assert.AreEqual (3, group.Count);

				Assert.AreEqual ("InsertStaticDataRequest", group[0].GetType ().Name);
				Assert.AreEqual ("InsertStaticDataRequest", group[1].GetType ().Name);
				Assert.AreEqual ("UpdateStaticDataRequest", group[2].GetType ().Name);

				Requests.InsertStaticDataRequest req1 = group[0] as Requests.InsertStaticDataRequest;
				Requests.InsertStaticDataRequest req2 = group[1] as Requests.InsertStaticDataRequest;
				Requests.UpdateStaticDataRequest req3 = group[2] as Requests.UpdateStaticDataRequest;

				Assert.AreEqual ("DemoTable", req1.TableName);
				Assert.AreEqual ("DemoTable", req2.TableName);

				Assert.AreEqual (1L, req1.ColumnValues[0]);
				Assert.AreEqual (2L, req2.ColumnValues[0]);

				Assert.AreEqual ("Pierre Arnaud", req1.ColumnValues[1]);
				Assert.AreEqual ("Jérôme André", req2.ColumnValues[1]);

				Assert.AreEqual (1972, req1.ColumnValues[2]);
				Assert.AreEqual (1994, req2.ColumnValues[2]);

				Assert.AreEqual (2, req3.ColumnNames.Count);
				Assert.AreEqual ("ID", req3.ColumnNames[0]);
				Assert.AreEqual ("Name", req3.ColumnNames[1]);
				Assert.AreEqual (1L, req3.ColumnValues[0]);
				Assert.AreEqual ("Pierre Arnaud-Bühlmann", req3.ColumnValues[1]);
			}
		}


		[TestMethod]
		public void Check06UpdateStaticData()
		{
			System.Data.DataTable table = UnitTestRequest.CreateSampleTable ();

			Requests.UpdateStaticDataRequest req1 = new Requests.UpdateStaticDataRequest (table.Rows[0], Requests.UpdateMode.Changed);

			table.Rows[0].BeginEdit ();
			table.Rows[0][1] = "Pierre Arnaud-Bühlmann";
			table.Rows[0].EndEdit ();

			Requests.UpdateStaticDataRequest req2 = new Requests.UpdateStaticDataRequest (table.Rows[0], Requests.UpdateMode.Changed);
			Requests.UpdateStaticDataRequest req3 = new Requests.UpdateStaticDataRequest (table.Rows[1], Requests.UpdateMode.Full);

			Assert.IsTrue (req1.ContainsData == false);
			Assert.IsTrue (req2.ContainsData == true);
			Assert.IsTrue (req3.ContainsData == true);

			Assert.AreEqual (2, req2.ColumnNames.Count);
			Assert.AreEqual ("ID", req2.ColumnNames[0]);
			Assert.AreEqual ("Name", req2.ColumnNames[1]);
			Assert.AreEqual (1L, req2.ColumnValues[0]);
			Assert.AreEqual ("Pierre Arnaud-Bühlmann", req2.ColumnValues[1]);
			Assert.AreEqual ("Pierre Arnaud", req2.ColumnOriginalValues[1]);

			Assert.AreEqual (3, req3.ColumnNames.Count);
			Assert.AreEqual ("ID", req3.ColumnNames[0]);
			Assert.AreEqual ("Name", req3.ColumnNames[1]);
			Assert.AreEqual ("Birth Year", req3.ColumnNames[2]);
			Assert.AreEqual (2L, req3.ColumnValues[0]);
			Assert.AreEqual ("Jérôme André", req3.ColumnValues[1]);
			Assert.AreEqual (1994, req3.ColumnValues[2]);
		}


		[TestMethod]
		public void Check07ExecutionQueue()
		{
			using (DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", true))
			{
				Assert.IsNotNull (infrastructure);

				Requests.ExecutionQueue queue = new Requests.ExecutionQueue (infrastructure, null);

				System.Data.DataRow[] rows;

				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					System.Data.DataTable table = UnitTestRequest.CreateSampleTable ();

					Requests.RequestCollection group = new Requests.RequestCollection ();

					Requests.InsertStaticDataRequest req1 = new Requests.InsertStaticDataRequest (table.Rows[0]);
					Requests.InsertStaticDataRequest req2 = new Requests.InsertStaticDataRequest (table.Rows[1]);

					table.Rows[0].BeginEdit ();
					table.Rows[0][1] = "Pierre Arnaud-Bühlmann";
					table.Rows[0].EndEdit ();

					Requests.UpdateStaticDataRequest req3 = new Requests.UpdateStaticDataRequest (table.Rows[0], Requests.UpdateMode.Changed);

					group.Add (req1);
					group.Add (req2);
					group.Add (req3);

					rows = queue.GetRows ();

					int n = rows.Length;

					queue.Enqueue (transaction, group);

					Assert.AreEqual (n, rows.Length);

					rows = queue.GetRows ();

					System.Data.DataRow row = rows[n];

					Assert.AreEqual (n+1, rows.Length);
					Assert.AreEqual (DbIdClass.Temporary, DbId.GetClass (DbKey.GetRowId (row)));
					Assert.AreEqual (Requests.ExecutionState.Pending, queue.GetRequestExecutionState (row));

					queue.PersistToBase (transaction);

					Assert.AreEqual (DbIdClass.Standard, DbId.GetClass (DbKey.GetRowId (row)));

					transaction.Commit ();
				}

				queue.Dispose ();

				queue = new Requests.ExecutionQueue (infrastructure, null);
				rows  = queue.GetRows ();

				foreach (System.Data.DataRow row in rows)
				{
					System.Diagnostics.Debug.WriteLine ("Row " + row[0] + " contains " + ((byte[]) row[Tags.ColumnReqData]).Length + " bytes, state = " + queue.GetRequestExecutionState (row));
				}

				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					queue.ClearQueue ();
					queue.PersistToBase (transaction);
					transaction.Commit ();
				}
			}
		}


		[TestMethod]
		public void Check08ExecutionEngine()
		{
			DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", false);
			Requests.ExecutionEngine engine = new Epsitec.Cresus.Requests.ExecutionEngine (infrastructure);

			Assert.AreEqual (0L, engine.CurrentLogId.Value);
			Assert.AreEqual (null, engine.CurrentTransaction);

			infrastructure.Logger.CreateTemporaryEntry (null);

			DbTypeDef db_type_name = infrastructure.ResolveDbType ("Customer Name");
			DbTypeDef db_type_date = infrastructure.ResolveDbType ("Birth Date");

			if (db_type_name == null)
			{
				db_type_name = new DbTypeDef ("Customer Name", DbSimpleType.String, null, 80, false, DbNullability.Yes);
				infrastructure.RegisterNewDbType (db_type_name);
			}

			if (db_type_date == null)
			{
				db_type_date = new DbTypeDef ("Birth Date", DbSimpleType.Date, null, 0, false, DbNullability.Yes);
				infrastructure.RegisterNewDbType (db_type_date);
			}

			DbTable db_table = infrastructure.CreateDbTable ("Simple Exec Table Test", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges);

			DbColumn db_col_1 = new DbColumn ("Name", db_type_name, DbColumnClass.Data, DbElementCat.ManagedUserData);
			DbColumn db_col_2 = new DbColumn ("Birth Date", db_type_date, DbColumnClass.Data, DbElementCat.ManagedUserData);

			db_table.Columns.AddRange (db_col_1, db_col_2);

			infrastructure.RegisterNewDbTable (db_table);

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

			table.Rows.Add (new object[] { DbId.CreateId (1, 1000).Value, 0, "Pierre Arnaud", new System.DateTime (1972, 2, 11) });

			factory.GenerateRequests (table);

			DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);

			engine.Execute (transaction, factory.CreateRequestCollection ());

			table.AcceptChanges ();
			table.Rows[0][col_3.ColumnName] = "Pierre Arnaud-Roost";
			table.Rows[0][col_4.ColumnName] = new System.DateTime (1940, 5, 20);

			factory.Clear ();
			factory.GenerateRequests (table);

			engine.Execute (transaction, factory.CreateRequestCollection ());

			transaction.Commit ();

			infrastructure.UnregisterDbTable (db_table);
		}


		[TestMethod]
		public void Check09Orchestrator()
		{
			DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", false);
			Requests.Orchestrator orchestrator = new Requests.Orchestrator (infrastructure);

			infrastructure.Logger.CreateTemporaryEntry (null);

			DbTypeDef db_type_name = infrastructure.ResolveDbType ("Customer Name");
			DbTypeDef db_type_date = infrastructure.ResolveDbType ("Birth Date");

			Assert.IsNotNull (db_type_name);
			Assert.IsNotNull (db_type_date);

			DbTable db_table = infrastructure.CreateDbTable ("Simple Exec Table Test", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges);

			DbColumn db_col_1 = new DbColumn ("Name", db_type_name, DbColumnClass.Data, DbElementCat.ManagedUserData);
			DbColumn db_col_2 = new DbColumn ("Birth Date", db_type_date, DbColumnClass.Data, DbElementCat.ManagedUserData);

			db_table.Columns.AddRange (new DbColumn[] { db_col_1, db_col_2 });

			infrastructure.RegisterNewDbTable (db_table);

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
			Requests.ExecutionQueue  queue   = orchestrator.ExecutionQueue;

			table.Rows.Add (new object[] { DbId.CreateId (1, 1000).Value, 0, "Pierre Arnaud", new System.DateTime (1972, 2, 11) });

			factory.GenerateRequests (table);

			queue.Enqueue (null, factory.CreateRequestCollection ());

			table.AcceptChanges ();
			table.Rows[0][col_3.ColumnName] = "Pierre Arnaud-Roost";
			table.Rows[0][col_4.ColumnName] = new System.DateTime (1940, 5, 20);

			factory.Clear ();
			factory.GenerateRequests (table);

			queue.Enqueue (null, factory.CreateRequestCollection ());

			UnitTestRequest.WaitUntilQueueFullyProcessed (queue);
			orchestrator.Dispose ();

			infrastructure.UnregisterDbTable (db_table);
		}


		private static void WaitUntilQueueFullyProcessed(Requests.ExecutionQueue queue)
		{
			int counter = queue.QueueChangeCounter;

			while (queue.AtomicCheck (
				q =>
				{
					counter = q.QueueChangeCounter;
					return q.HasPending;
				}))
			{
				System.Diagnostics.Debug.WriteLine (queue.HasPending ? "Queue has pending requests" : queue.HasConflicting ? "Queue has conflicting requests" : "Queue ready ?");
				queue.WaitForQueueChange (q => q.QueueChangeCounter == counter);
			}

			System.Diagnostics.Debug.WriteLine ("Queue has been fully processed");

		}


		[TestMethod]
		public void Check10ExecutionQueueDump()
		{
			using (DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", true))
			{
				Assert.IsNotNull (infrastructure);

				Requests.ExecutionQueue queue = new Requests.ExecutionQueue (infrastructure, null);
				System.Data.DataRow[] rows;

				queue = new Requests.ExecutionQueue (infrastructure, null);
				rows  = queue.GetRows ();

				foreach (System.Data.DataRow row in rows)
				{
					System.Diagnostics.Debug.WriteLine ("Row " + row[0] + " contains " + ((byte[]) row[Tags.ColumnReqData]).Length + " bytes, state = " + queue.GetRequestExecutionState (row));
				}
			}
		}


		[TestMethod]
		public void Check11ExecutionQueueClearQueue()
		{
			using (DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", true))
			{
				Assert.IsNotNull (infrastructure);

				Requests.ExecutionQueue queue = new Requests.ExecutionQueue (infrastructure, null);
				System.Data.DataRow[] rows;

				queue = new Requests.ExecutionQueue (infrastructure, null);
				rows  = queue.GetRows ();

				foreach (System.Data.DataRow row in rows)
				{
					System.Diagnostics.Debug.WriteLine ("Row " + row[0] + " contains " + ((byte[]) row[Tags.ColumnReqData]).Length + " bytes, state = " + queue.GetRequestExecutionState (row));
				}

				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					queue.ClearQueue ();
					queue.PersistToBase (transaction);
					transaction.Commit ();
				}
			}
		}


#endif


#if false


		[TestMethod]
		public void Check12ServiceServer()
		{
			DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", false);

			infrastructure.LocalSettings.IsServer = true;

			EngineHost host = new EngineHost (12345);
			Engine engine = new Engine (infrastructure, System.Guid.Empty);

			host.AddEngine (engine);

			infrastructure.LocalSettings.IsServer = false;

			infrastructure.Logger.CreateTemporaryEntry (null);
			UnitTestRequest.CreateTestTable (infrastructure, UnitTestRequest.TestTableName);

			System.Diagnostics.Debug.WriteLine ("Server ready. Running for 5 seconds.");

			System.Threading.Thread.Sleep (1000 * 5);

			UnitTestRequest.DeleteTestTable (infrastructure, UnitTestRequest.TestTableName);

			System.Diagnostics.Debug.WriteLine ("Aborting server...");
			Common.Support.Globals.SignalAbort ();
		}

#endif

#if false


		[TestMethod]
		public void Check12ServiceServer_Create()
		{
			//	Le service doit tourner (installer & lancer avec "installutil Cresus.Server.exe" depuis
			//	le dossier "App.Server/bin/Debug"). Ou alors, utiliser l'installateur X.Setup.Server.

			using (DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", false))
			{
				infrastructure.Logger.CreateTemporaryEntry (null);
				UnitTestRequest.CreateTestTable (infrastructure, UnitTestRequest.TestTableName);
			}
		}


		[TestMethod]
		public void Check13ConnectionClient()
		{
			Remoting.IRemoteServiceManager manager = ClientEngine.GetRemoteServiceManager ("localhost", 1234);
			Remoting.IConnectionService service = manager.GetConnectionService (System.Guid.Empty);
			Remoting.ClientIdentity client = new Remoting.ClientIdentity ("NUnit Test Client", 1000);

			Assert.IsNotNull (service);

			System.Guid[] serviceIds;

			Remoting.ClientIdentity.DefineDefaultClientId (1);

			service.CheckConnectivity (client);
			serviceIds = service.QueryAvailableServices (client);

			System.Diagnostics.Debug.WriteLine ("Found " + serviceIds.Length + " services:");
			foreach (var id in serviceIds)
			{
				System.Diagnostics.Debug.WriteLine ("-- " + id);
			}
		}


		[TestMethod]
		public void Check14OperatorClientWaiting()
		{
			Remoting.IRemoteServiceManager manager = ClientEngine.GetRemoteServiceManager ("localhost", 1234);
			Remoting.IOperatorService service = manager.GetOperatorService (System.Guid.Empty);
			Remoting.ProgressInformation operation;

			Assert.IsNotNull (service);

			System.Diagnostics.Debug.WriteLine ("Starting asynchronous request.");

			operation = service.CreateRoamingClient ("test");

			Remoting.ClientIdentity client;
			byte[]                  data;

			service.GetRoamingClientData (operation.OperationId, out client, out data);

			if ((data != null) &&
                (data.Length > 0))
			{
				System.Diagnostics.Debug.WriteLine ("Returned data, " + data.Length + " bytes for client ID " + client.Id + ".");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("No data returned.");
			}
		}


		[TestMethod]
		public void Check15OperatorClientPolling()
		{
			Remoting.IRemoteServiceManager manager = ClientEngine.GetRemoteServiceManager ("localhost", 1234);
			Remoting.IOperatorService service = manager.GetOperatorService (System.Guid.Empty);
			Remoting.ProgressInformation operation;
			Remoting.ProgressInformation progress;

			Assert.IsNotNull (service);

			operation = service.CreateRoamingClient ("test");

			for (int i = 0; i < 10; i++)
			{
				System.Threading.Thread.Sleep (100);
				Remoting.ProgressState status = operation.ProgressState;

				System.Diagnostics.Debug.WriteLine ("Operation: " + status + " after " + (int) operation.RunningDuration.TotalMilliseconds + " ms.");

				if (status == Remoting.ProgressState.Succeeded)
				{
					Remoting.ClientIdentity client;
					byte[]                  data;

					service.GetRoamingClientData (operation.OperationId, out client, out data);

					if ((data != null) &&
                        (data.Length > 0))
					{
						System.Diagnostics.Debug.WriteLine ("Returned data, " + data.Length + " bytes for client ID " + client.Id + ".");
					}
					else
					{
						System.Diagnostics.Debug.WriteLine ("No data returned.");
					}

					return;
				}
			}

			System.Diagnostics.Debug.WriteLine ("Cancelling...");
			progress = manager.CancelOperationAsync (operation.OperationId);

			for (int i = 0; i < 100; i++)
			{
				System.Threading.Thread.Sleep (10);

				Remoting.ProgressState status = progress.ProgressState;

				System.Diagnostics.Debug.WriteLine ("Cancellation: " + status + " after " + (int) progress.RunningDuration.TotalMilliseconds + " ms.");
				if (status == Remoting.ProgressState.Succeeded)
					break;
			}
		}

		// DEAD CODE?
		[TestMethod]
		public void Check16RequestExecutionClient()
		{
			DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", false);
			System.Data.DataTable table = UnitTestRequest.GetDataTableFromTable (infrastructure, UnitTestRequest.TestTableName);

			Remoting.ClientIdentity.DefineDefaultClientId (1);
			Remoting.ClientIdentity client = new Remoting.ClientIdentity ("NUnit Test Client", 1000);

			Remoting.IRequestExecutionService service = Engine.GetRemoteRequestExecutionService ("localhost", 1234);

			Assert.IsNotNull (service);

			DataLayer.RequestFactory factory = new DataLayer.RequestFactory ();

			System.Data.DataRow data_row;

			data_row = DbRichCommand.CreateRow (table);

			data_row.BeginEdit ();
			data_row[0] = DbId.CreateId (1, 1000).Value;
			data_row[3] = "Pierre Arnaud";
			data_row[4] = new System.DateTime (1972, 2, 11);
			data_row.EndEdit ();

			table.Rows.Add (data_row);

			factory.GenerateRequests (table);

			byte[] serialized_1 = Requests.AbstractRequest.SerializeToMemory (factory.CreateRequestCollection () );

			table.AcceptChanges ();
			table.Rows[0][3] = "Pierre Arnaud-Roost";
			table.Rows[0][4] = new System.DateTime (1940, 5, 20);

			factory.Clear ();
			factory.GenerateRequests (table);

			byte[] serialized_2 = Requests.AbstractRequest.SerializeToMemory (factory.CreateRequestCollection ());

			service.EnqueueRequest (client, new Remoting.SerializedRequest[] { new Remoting.SerializedRequest (DbId.CreateId (100, 1000).Value, serialized_1),
                /**/														   new Remoting.SerializedRequest (DbId.CreateId (101, 1000).Value, serialized_2) });

			Remoting.RequestState[] states;

			int change_id = -1;

			for (; ; )
			{
				states = service.QueryRequestStates (client);
				System.Diagnostics.Debug.WriteLine ("1/ Got " + states.Length + " states back from server (change ID=" + change_id + ") :");

				for (int i = 0; i < states.Length; i++)
				{
					System.Diagnostics.Debug.WriteLine ("-- " + states[i].RequestId + ", state = " + (Requests.ExecutionState) states[i].State);
				}

				if (states[0].State != (int) Requests.ExecutionState.Pending)
				{
					break;
				}
			}

			states = service.QueryRequestStates (client);
			System.Diagnostics.Debug.WriteLine ("2/ Got " + states.Length + " states back from server (change ID=" + change_id + ") :");

			for (int i = 0; i < states.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine ("-- " + states[i].RequestId + ", state = " + (Requests.ExecutionState) states[i].State);
			}

			service.RemoveRequestStates (client, new Remoting.RequestState[] { states[0] });
			states = service.QueryRequestStates (client);
			System.Diagnostics.Debug.WriteLine ("3/ After clearing first state, got " + states.Length + " states back from server (change ID=" + change_id + ") :");

			for (int i = 0; i < states.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine ("-- " + states[i].RequestId + ", state = " + (Requests.ExecutionState) states[i].State);
			}

			states = service.QueryRequestStates (client);
			System.Diagnostics.Debug.WriteLine ("4/ Got " + states.Length + " states back from server (change ID=" + change_id + ") :");

			for (int i = 0; i < states.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine ("-- " + states[i].RequestId + ", state = " + (Requests.ExecutionState) states[i].State);
			}
		}


		[TestMethod]
		public void Check17OperatorClientLoop()
		{
			for (int i = 0; i < 1000; i++)
			{
				try
				{
					this.Check14OperatorClientWaiting ();
				}
				catch (System.Exception ex)
				{
					System.Console.WriteLine ("At step {0}, {1}", i, ex.Message);
					System.Console.WriteLine (ex.ToString ());
				}
			}
		}

		[TestMethod]
		public void Check18RoamingClientTool()
		{
			Remoting.IRemoteServiceManager manager = ClientEngine.GetRemoteServiceManager ("localhost", 1234);
			Remoting.IOperatorService service = manager.GetOperatorService (System.Guid.Empty);
			Remoting.ProgressInformation operation;

			Assert.IsNotNull (service);

			System.Diagnostics.Debug.WriteLine ("Starting asynchronous request.");

			operation = service.CreateRoamingClient ("test");

			RoamingClientTool.CreateDatabase (service, operation.OperationId, "roaming");
		}


		// DEAD CODE?
		[TestMethod]
		public void Check19ReplicationAndSynchronization()
		{
			Remoting.IRequestExecutionService service = Engine.GetRemoteRequestExecutionService ("localhost", 1234);

			Assert.IsNotNull (service);

			using (DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("roaming", false))
			{
				Remoting.ClientIdentity client = new Remoting.ClientIdentity ("NUnit Test Client", infrastructure.LocalSettings.ClientId);
				Requests.Orchestrator orchestrator = new Requests.Orchestrator (infrastructure);

				orchestrator.DefineRemotingService (service, client);

				DbTable       db_table = infrastructure.ResolveDbTable (UnitTestRequest.TestTableName);
				DbRichCommand command  = DbRichCommand.CreateFromTable (infrastructure, null, db_table, DbSelectRevision.LiveActive);

				System.Data.DataTable    table   = command.DataSet.Tables[0];
				DataLayer.RequestFactory factory = new DataLayer.RequestFactory ();

				System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} rows in table {1}.", table.Rows.Count, table.TableName));

				System.Data.DataRow data_row = command.CreateRow (UnitTestRequest.TestTableName);

				data_row.BeginEdit ();
				data_row[3] = "Albert Einstein";
				data_row[4] = new System.DateTime (1904, 5, 14);
				data_row.EndEdit ();

				data_row = command.CreateRow (UnitTestRequest.TestTableName);

				data_row.BeginEdit ();
				data_row[3] = "Max Planck";
				data_row[4] = new System.DateTime (1858, 4, 23);
				data_row.EndEdit ();

				data_row = command.CreateRow (UnitTestRequest.TestTableName);

				data_row.BeginEdit ();
				data_row[3] = "Niels Bohr";
				data_row[4] = new System.DateTime (1885, 10, 7);
				data_row.EndEdit ();

				data_row = table.Rows[0];

				data_row.BeginEdit ();
				data_row[3] = "Toto";
				data_row.EndEdit ();

				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					command.UpdateRealIds (transaction);
					transaction.Commit ();
				}

				factory.GenerateRequests (table);

				System.Diagnostics.Debug.WriteLine ("Enqueue request.");

				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					lock (orchestrator.ExecutionQueue)
					{
						orchestrator.ExecutionQueue.Enqueue (transaction, factory.CreateRequestCollection ());
						transaction.Commit ();
					}
				}

				System.Diagnostics.Debug.WriteLine ("Waiting for requests to be executed.");

				System.Threading.Thread.Sleep (30000);

				orchestrator.Dispose ();
			}
		}


		[TestMethod]
		public void Check21RealReplication()
		{
			using (DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("roaming", false))
			{
				Remoting.IRemoteServiceManager manager = ClientEngine.GetRemoteServiceManager ("localhost", 1234);
				Remoting.ClientIdentity        client  = new Remoting.ClientIdentity ("NUnit Test Client", infrastructure.LocalSettings.ClientId);
				Remoting.IReplicationService   service = manager.GetReplicationService (System.Guid.Empty);

				Assert.IsNotNull (service);

				byte[] buffer;

				DbId last_sync_id = infrastructure.LocalSettings.SyncLogId;

				DbId from_id = DbId.CreateId (last_sync_id.LocalId + 1, last_sync_id.ClientId);
				DbId to_id   = DbId.CreateId (DbId.LocalRange - 1, last_sync_id.ClientId);

				Remoting.ProgressInformation operation;
				System.Diagnostics.Debug.WriteLine (string.Format ("Asking server for replication data (starting at {0}).", from_id));
				operation = service.AcceptReplication (client, from_id, to_id);
				System.Diagnostics.Debug.WriteLine ("Waiting...");
				buffer = service.GetReplicationData (operation.OperationId);
				System.Diagnostics.Debug.WriteLine ("Server reply received.");

				System.Diagnostics.Debug.WriteLine (string.Format ("Replication produced {0} byte(s) of data.", (buffer == null ? 0 : buffer.Length)));

				if (buffer != null)
				{
					Replication.ClientReplicationEngine engine = new Replication.ClientReplicationEngine (infrastructure, service);

					engine.ApplyChanges (infrastructure.DefaultDbAbstraction, operation.OperationId);

					manager.WaitForProgress (operation.OperationId, 100, System.TimeSpan.FromSeconds (10.0));
				}

				System.Diagnostics.Debug.WriteLine ("Done.");
			}
		}



#endif


		[TestMethod]
		public void Check99ServiceServer_Kill()
		{
			using (DbInfrastructure infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", false))
			{
				UnitTestRequest.DeleteTestTable (infrastructure, UnitTestRequest.TestTableName);
			}
		}


		private static readonly string TestTableName = "ST";


		private static void CreateTestTable(DbInfrastructure infrastructure, string name)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Creating test table {0} in database {1}", name, infrastructure.Access.Database));

			DbTypeDef db_type_name = infrastructure.ResolveDbType ("Customer Name");
			DbTypeDef db_type_date = infrastructure.ResolveDbType ("Birth Date");

			if (db_type_name == null)
			{
				db_type_name = new DbTypeDef ("Customer Name", DbSimpleType.String, null, 80, false, DbNullability.No);
				infrastructure.RegisterNewDbType (db_type_name);
			}

			if (db_type_date == null)
			{
				db_type_date = new DbTypeDef ("Birth Date", DbSimpleType.Date, null, 0, false, DbNullability.Yes);
				infrastructure.RegisterNewDbType (db_type_date);
			}

			Assert.IsNotNull (db_type_name);
			Assert.IsNotNull (db_type_date);

			DbTable db_table = infrastructure.CreateDbTable (name, DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges);

			DbColumn db_col_1 = new DbColumn ("Name", db_type_name, DbColumnClass.Data, DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges);
			DbColumn db_col_2 = new DbColumn ("Birth Date", db_type_date, DbColumnClass.Data, DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges);

			db_table.Columns.AddRange (new DbColumn[] { db_col_1, db_col_2 });

			infrastructure.RegisterNewDbTable (db_table);
		}


		private static System.Data.DataTable GetDataTableFromTable(DbInfrastructure infrastructure, string name)
		{
			DbTable db_table = infrastructure.ResolveDbTable (name);

			DbColumn db_col_id   = db_table.Columns[0];
			DbColumn db_col_stat = db_table.Columns[1];
			DbColumn db_col_log  = db_table.Columns[2];
			DbColumn db_col_1    = db_table.Columns[3];
			DbColumn db_col_2    = db_table.Columns[4];

			System.Data.DataSet   set   = new System.Data.DataSet ();
			System.Data.DataTable table = new System.Data.DataTable (db_table.Name);

			set.Tables.Add (table);

			System.Data.DataColumn col_1 = new System.Data.DataColumn (db_col_id.Name, typeof (long));
			System.Data.DataColumn col_2 = new System.Data.DataColumn (db_col_stat.Name, typeof (int));
			System.Data.DataColumn col_3 = new System.Data.DataColumn (db_col_log.Name, typeof (long));
			System.Data.DataColumn col_4 = new System.Data.DataColumn (db_col_1.Name, typeof (string));
			System.Data.DataColumn col_5 = new System.Data.DataColumn (db_col_2.Name, typeof (System.DateTime));

			col_1.Unique = true;

			table.Columns.Add (col_1);
			table.Columns.Add (col_2);
			table.Columns.Add (col_3);
			table.Columns.Add (col_4);
			table.Columns.Add (col_5);

			return table;
		}


		private static void DeleteTestTable(DbInfrastructure infrastructure, string name)
		{
			DbTable db_table = infrastructure.ResolveDbTable (name);
			infrastructure.UnregisterDbTable (db_table);
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
			table.Rows.Add (new object[] { 2L, "Jérôme André", 1994 });

			table.AcceptChanges ();

			return table;
		}


	}


}
