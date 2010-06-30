using Epsitec.Cresus.Database;

using Epsitec.Cresus.Remoting;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epsitec.Cresus.Services;


namespace Epsitec.Cresus.Replication
{


	/*
	 * The following code comes directly from the old test and has not been checked or tested. It
	 * is very likely to fail when run. If I have time to clean it once, I'll do it and I'll erase
	 * this comment. Marc
	 */


	[TestClass]
	public class UnitTestReplication
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			this.infrastructure = UnitTestDbInfrastructure.GetInfrastructureFromBase ("fiche", true);

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.infrastructure.Logger.CreatePermanentEntry (transaction);
				
				DbTypeDef type1 = new DbTypeDef ("TypeFixed" + System.DateTime.Now.Ticks, DbSimpleType.Null, new DbNumDef () { MaxValue = 1000 }, 1, true, DbNullability.Undefined);
				DbTypeDef type2 = new DbTypeDef ("TypeNum" + System.DateTime.Now.Ticks, DbSimpleType.Null, new DbNumDef (5, 1), 1, true, DbNullability.Undefined);

				this.infrastructure.RegisterNewDbType (transaction, type1);
				this.infrastructure.RegisterNewDbType (transaction, type2);

				transaction.Commit ();
			}
		}


		[ClassCleanup]
		public void CleanUp()
		{
			this.infrastructure.Dispose ();
		}


		[TestMethod]
		public void Check01DataExtractorExtractData()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DataExtractor extractpr = new DataExtractor (this.infrastructure, transaction);
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableTypeDef);

				System.Data.DataTable data = extractpr.ExtractDataUsingLogIds (table, DbId.CreateId (2, 1), DbId.CreateId (999999999, 1));

				foreach (System.Data.DataRow row in data.Rows)
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					
					bool first = true;

					foreach (object x in row.ItemArray)
					{
						if (first)
						{
							first = false;
						}
						else
						{
							buffer.Append (", ");
						}

						buffer.Append (x == System.DBNull.Value ? "<DBNull>" : x.ToString ());
					}

					System.Diagnostics.Debug.WriteLine (buffer.ToString ());
				}
			}
		}


#if false


		[TestMethod]
		public void Check02DataCruncherPackColumnToNativeArray()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DataExtractor cruncher = new DataExtractor (this.infrastructure, transaction);
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableTypeDef);

				System.Data.DataTable data = cruncher.ExtractDataUsingLogIds (table, DbId.CreateId (2, 1), DbId.CreateId (999999999, 1));

				for (int i = 0; i < data.Columns.Count; i++)
				{
					System.Array array;
					bool[] null_array;
					int    null_count;

					array = PackedTableData.PackColumnToNativeArray (data, i, out null_array, out null_count);

					System.Console.WriteLine ("Column {0}: type {1}, {2} rows, {3} are null.", i, array.GetType (), array.Length, null_count);
				}
			}
		}


		[TestMethod]
		public void Check03DataCruncherUnpackColumnFromNativeArray()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DataExtractor cruncher = new DataExtractor (this.infrastructure, transaction);
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableTypeDef);

				System.Data.DataTable data = cruncher.ExtractDataUsingLogIds (table, DbId.CreateId (2, 1), DbId.CreateId (999999999, 1));

				object[][] store = new object[data.Rows.Count][];

				for (int i = 0; i < data.Columns.Count; i++)
				{
					System.Array array;
					bool[] null_array;
					int    null_count;

					array = PackedTableData.PackColumnToNativeArray (data, i, out null_array, out null_count);
					PackedTableData.UnpackColumnFromNativeArray (store, i, data.Columns.Count, array, (null_count == 0) ? null : null_array);
				}

				for (int r = 0; r < data.Rows.Count; r++)
				{
					for (int i = 0; i < data.Columns.Count; i++)
					{
						object a = data.Rows[r][i];
						object b = store[r][i];

						Assert.AreEqual (a, b, string.Format ("Row {0}, Column {1} mismatch: {2}, {3}", r, i, a, b));
					}
				}
			}
		}


		[TestMethod]
		public void Check05PackedTableData()
		{
			System.Data.DataTable table_a = new System.Data.DataTable ("A");

			table_a.Columns.Add ("C0", typeof (int));
			table_a.Columns.Add ("C1", typeof (string));
			table_a.Columns.Add ("C2", typeof (decimal));
			table_a.Columns.Add ("C3", typeof (short));

			table_a.Columns["C0"].AllowDBNull = false;
			table_a.Columns["C1"].AllowDBNull = true;
			table_a.Columns["C2"].AllowDBNull = true;
			table_a.Columns["C3"].AllowDBNull = true;

			table_a.Rows.Add (new object[] { 0, "Abc", 10.0M, System.DBNull.Value });
			table_a.Rows.Add (new object[] { 1, "Def", 25.0M, System.DBNull.Value });
			table_a.Rows.Add (new object[] { 2, null, 30.0M, System.DBNull.Value });
			table_a.Rows.Add (new object[] { 3, "Xyz", System.DBNull.Value, System.DBNull.Value });

			System.Array a_0, a_1, a_2, a_3;
			bool[] n_0, n_1, n_2, n_3;

			int count;

			a_0 = PackedTableData.PackColumnToNativeArray (table_a, 0, out n_0, out count);

			Assert.AreEqual (0, count);
			Assert.AreEqual (4, n_0.Length);
			Assert.AreEqual (false, n_0[0]);
			Assert.AreEqual (false, n_0[1]);
			Assert.AreEqual (false, n_0[2]);
			Assert.AreEqual (false, n_0[3]);

			a_1 = PackedTableData.PackColumnToNativeArray (table_a, 1, out n_1, out count);

			Assert.AreEqual (1, count);
			Assert.AreEqual (4, n_1.Length);
			Assert.AreEqual (false, n_1[0]);
			Assert.AreEqual (false, n_1[1]);
			Assert.AreEqual (true, n_1[2]);
			Assert.AreEqual (false, n_1[3]);

			a_2 = PackedTableData.PackColumnToNativeArray (table_a, 2, out n_2, out count);

			Assert.AreEqual (1, count);
			Assert.AreEqual (4, n_2.Length);
			Assert.AreEqual (false, n_2[0]);
			Assert.AreEqual (false, n_2[1]);
			Assert.AreEqual (false, n_2[2]);
			Assert.AreEqual (true, n_2[3]);

			a_3 = PackedTableData.PackColumnToNativeArray (table_a, 3, out n_3, out count);

			Assert.AreEqual (4, count);
			Assert.AreEqual (4, n_3.Length);
			Assert.AreEqual (true, n_3[0]);
			Assert.AreEqual (true, n_3[1]);
			Assert.AreEqual (true, n_3[2]);
			Assert.AreEqual (true, n_3[3]);

			table_a.Rows.Add (new object[] { 4, "G", 10.0M, System.DBNull.Value });
			table_a.Rows.Add (new object[] { 5, "H", 25.0M, System.DBNull.Value });
			table_a.Rows.Add (new object[] { 6, "I", 30.0M, System.DBNull.Value });
			table_a.Rows.Add (new object[] { 7, System.DBNull.Value, 30.0M, System.DBNull.Value });
			table_a.Rows.Add (new object[] { 8, "J", System.DBNull.Value, System.DBNull.Value });

			DbTable db_table = new DbTable ("A");

			db_table.DefineKey (new DbKey ());

			PackedTableData packed = PackedTableData.CreateFromTable (db_table, table_a);

			Assert.IsFalse (packed.HasNullValues (0));
			Assert.IsTrue (packed.HasNullValues (1));
			Assert.IsTrue (packed.HasNullValues (2));
			Assert.IsTrue (packed.HasNullValues (3));

			Assert.IsTrue (packed.HasNonNullValues (0));
			Assert.IsTrue (packed.HasNonNullValues (1));
			Assert.IsTrue (packed.HasNonNullValues (2));
			Assert.IsFalse (packed.HasNonNullValues (3));

			System.Data.DataTable table_b = table_a.Clone ();

			Assert.AreEqual (0, table_b.Rows.Count);
			Assert.AreEqual (4, table_b.Columns.Count);

			packed.FillTable (table_b);

			Assert.AreEqual (9, table_b.Rows.Count);

			for (int i = 0; i < table_a.Rows.Count; i++)
			{
				for (int j = 0; j < table_a.Columns.Count; j++)
				{
					Assert.AreEqual (table_a.Rows[i][j], table_b.Rows[i][j], string.Format ("Mismatch for row {0}, column {1}", i, j));
				}
			}

			byte[] packed_bytes = Common.IO.Serialization.SerializeAndCompressToMemory (packed, Common.IO.Compressor.DeflateCompact);

			System.Console.WriteLine ("Packed table has {0} bytes.", packed_bytes.Length);

			packed = Common.IO.Serialization.DeserializeAndDecompressFromMemory (packed_bytes) as PackedTableData;

			table_b = table_a.Clone ();

			Assert.AreEqual (0, table_b.Rows.Count);
			Assert.AreEqual (4, table_b.Columns.Count);

			packed.FillTable (table_b);

			Assert.AreEqual (9, table_b.Rows.Count);

			for (int i = 0; i < table_a.Rows.Count; i++)
			{
				for (int j = 0; j < table_a.Columns.Count; j++)
				{
					Assert.AreEqual (table_a.Rows[i][j], table_b.Rows[i][j], string.Format ("Mismatch for row {0}, column {1}", i, j));
				}
			}
		}


		[TestMethod]
		public void Check06ServerEngine()
		{
			IReplicationService service = Engine.GetRemoteReplicationService ("localhost", 1234);

			Assert.IsNotNull (service);

			byte[] buffer;

			DbId from_id = DbId.CreateId (1, 1);
			DbId to_id   = DbId.CreateId (999999, 1);

			System.Diagnostics.Debug.WriteLine ("Asking server for replication data.");
			ProgressInformation info = service.AcceptReplication (new ClientIdentity ("test", 1000), from_id, to_id);

			System.Diagnostics.Debug.WriteLine ("Waiting...");
			buffer = service.GetReplicationData (info.OperationId);

			System.Diagnostics.Debug.WriteLine ("Server reply received.");

			System.Console.WriteLine ("Replication produced {0} bytes of data.", (buffer == null ? 0 : buffer.Length));

			if (buffer != null)
			{
				ReplicationData data = Common.IO.Serialization.DeserializeAndDecompressFromMemory (buffer) as ReplicationData;

				foreach (PackedTableData packed_table in data.PackedTableData)
				{
					System.Console.WriteLine ("Table {0} contains {1} changed rows:", packed_table.Name, packed_table.RowCount);

					object[][] values = packed_table.GetAllValues ();

					foreach (object[] row in values)
					{
						System.Text.StringBuilder message = new System.Text.StringBuilder ();

						for (int i = 0; i < row.Length; i++)
						{
							if (i > 0)
							{
								message.Append ("; ");
							}
							message.Append (row[i]);
						}

						System.Console.WriteLine ("  {0}", message.ToString ());
					}
				}
			}
		}


		[TestMethod]
		public void Check07ClientEngine()
		{
			IReplicationService service = Engine.GetRemoteReplicationService ("localhost", 1234);

			Assert.IsNotNull (service);

			byte[] buffer;

			DbId from_id = DbId.CreateId (1, 1);
			DbId to_id   = DbId.CreateId (999999, 1);

			System.Diagnostics.Debug.WriteLine ("Asking server for replication data.");
			ProgressInformation info = service.AcceptReplication (new ClientIdentity ("test", 1000), from_id, to_id);

			System.Diagnostics.Debug.WriteLine ("Waiting...");
			buffer = service.GetReplicationData (info.OperationId);

			System.Diagnostics.Debug.WriteLine ("Server reply received.");

			System.Console.WriteLine ("Replication produced {0} bytes of data.", (buffer == null ? 0 : buffer.Length));

			if (buffer != null)
			{
				for (int i = 0; i < 3; i++)
				{
					try
					{
						System.IO.File.Delete (@"C:\Program Files\firebird15\Data\Epsitec\REPLITEST.FIREBIRD");
					}
					catch (System.IO.IOException ex)
					{
						System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 2 {1}seconds...", ex.ToString (), (i == 0) ? "" : "more ");
						System.Threading.Thread.Sleep (2000);
					}
				}

				using (DbInfrastructure infrastructure = new DbInfrastructure ())
				{
					DbAccess db_access = DbInfrastructure.CreateDatabaseAccess ("replitest");

					infrastructure.CreateDatabase (db_access);
					ClientReplicationEngine client = new ClientReplicationEngine (infrastructure, service);
					client.ApplyChanges (infrastructure.DefaultDbAbstraction, info.OperationId);
				}
			}
		}


		[TestMethod]
		public void Check08ClientEnginePull()
		{
			IReplicationService service = Engine.GetRemoteReplicationService ("localhost", 1234);

			Assert.IsNotNull (service);

			byte[] buffer;

			DbId from_id = DbId.CreateId (1, 1);
			DbId to_id   = DbId.CreateId (1, 1);

			PullReplicationChunk[] args = new PullReplicationChunk[1];
			args[0] = new PullReplicationChunk (1000000000010, new long[] { 1000000000004, 1000000000005, 1000000000006 });

			System.Diagnostics.Debug.WriteLine ("Asking server for replication data.");
			ProgressInformation info = service.PullReplication (new ClientIdentity ("test", 1000), from_id, to_id, args);

			System.Diagnostics.Debug.WriteLine ("Waiting...");
			buffer = service.GetReplicationData (info.OperationId);

			System.Diagnostics.Debug.WriteLine ("Server reply received.");

			System.Console.WriteLine ("Replication produced {0} bytes of data.", (buffer == null ? 0 : buffer.Length));

			if (buffer != null)
			{
				for (int i = 0; i < 3; i++)
				{
					try
					{
						System.IO.File.Delete (@"C:\Program Files\firebird15\Data\Epsitec\REPLITEST.FIREBIRD");
					}
					catch (System.IO.IOException ex)
					{
						System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 2 {1}seconds...", ex.ToString (), (i == 0) ? "" : "more ");
						System.Threading.Thread.Sleep (2000);
					}
				}

				using (DbInfrastructure infrastructure = new DbInfrastructure ())
				{
					DbAccess db_access = DbInfrastructure.CreateDatabaseAccess ("replitest");

					infrastructure.CreateDatabase (db_access);
					ClientReplicationEngine client = new ClientReplicationEngine (infrastructure, service);
					client.ApplyChanges (infrastructure.DefaultDbAbstraction, info.OperationId);
				}
			}
		}

#endif



		private DbInfrastructure infrastructure;


	}


}
