using NUnit.Framework;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class ReplicationTest
	{
		[TestFixtureSetUp] public void Setup()
		{
			this.infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", true);
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.infrastructure.Logger.CreatePermanentEntry (transaction);
				DbType type_1 = this.infrastructure.CreateDbType ("TypeFixed"+System.DateTime.Now.Ticks.ToString (), 1000, true);
				DbType type_2 = this.infrastructure.CreateDbType ("TypeNum"+System.DateTime.Now.Ticks.ToString (), new DbNumDef (5, 1));
				this.infrastructure.RegisterNewDbType (transaction, type_1);
				this.infrastructure.RegisterNewDbType (transaction, type_2);
				transaction.Commit ();
			}
		}
		
		[TestFixtureTearDown] public void TearDown()
		{
			this.infrastructure.Dispose ();
		}
		
		private DbInfrastructure infrastructure;
		
		[Test] public void Check01DataCruncherExtractData()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				Replication.DataCruncher cruncher = new Replication.DataCruncher (this.infrastructure, transaction);
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableTypeDef);
				
				System.Data.DataTable data = cruncher.ExtractData (table, DbId.CreateId (1, 1));
				
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
						buffer.Append (x.ToString ());
					}
					
					System.Console.WriteLine (buffer.ToString ());
				}
			}
		}
	}
}
