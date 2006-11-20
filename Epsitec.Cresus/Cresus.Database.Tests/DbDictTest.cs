using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbDictTest
	{
		[TestFixtureSetUp] public void Setup()
		{
			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\FICHE.FIREBIRD");
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);
				
				try
				{
					System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\FICHE.FIREBIRD");
				}
				catch
				{
				}
			}

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess ("fiche"));
			}
			
			this.infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", true);
		}
		
		[TestFixtureTearDown] public void TearDown()
		{
			this.infrastructure.Dispose ();
			this.infrastructure = null;
		}
		
		[Test] public void Check01CreateDict()
		{
			Assert.IsNotNull (infrastructure);

			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				DbDict.CreateTable (infrastructure, transaction, "TestDict");
				transaction.Commit ();
			}
		}
		
		[Test] public void Check02Attach()
		{
			this.table = this.infrastructure.ResolveDbTable ("TestDict");
			
			Assert.IsNotNull (this.table);
			
			this.dict = new DbDict ();
			this.dict.Attach (this.infrastructure, this.table);
		}
		
		[Test] public void Check03RestoreFromBase()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				this.dict.LoadFromBase (transaction);
				
				Assert.AreEqual (0, this.dict.Count);
				transaction.Commit ();
			}
		}

		[Test] public void Check04SerializeToBaseEmpty()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.dict.PersistToBase (transaction);
				transaction.Commit ();
			}
		}

		[Test] public void Check05SerializeToBaseData1()
		{
			this.dict.Add ("Key A", "Value A");
			this.dict.Add ("Key B", "Value B");
			this.dict.Add ("Key C", "Value C");
			
			Assert.AreEqual (3, this.dict.Count);
			
			this.dict["Key B"] = "Value B, update 1";
			
			Assert.AreEqual (3, this.dict.Count);
			
			this.dict.Remove ("Key C");
			
			Assert.AreEqual (2, this.dict.Count);
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.dict.PersistToBase (transaction);
				transaction.Commit ();
			}
		}

		[Test] public void Check06SerializeToBaseData2()
		{
			this.dict.Add ("Key D", "Value D");
			
			Assert.AreEqual (3, this.dict.Count);
			
			this.dict["Key B"] = "Value B, update 2";
			this.dict.Remove ("Key A");
			
			Assert.AreEqual (2, this.dict.Count);
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.dict.PersistToBase (transaction);
				transaction.Commit ();
			}
		}

		[Test] public void Check07RestoreFromBase()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				this.dict.LoadFromBase (transaction);
				transaction.Commit ();
				
				Assert.AreEqual (2, this.dict.Count);
			}
		}

		[Test] public void Check90Detach()
		{
			this.dict.Detach ();
		}
		
		[Test] public void Check99RemoveTable()
		{
			this.infrastructure.UnregisterDbTable (this.table);
			this.table = null;
		}

		private DbInfrastructure				infrastructure;
		private DbDict							dict;
		private DbTable							table;
	}
}
