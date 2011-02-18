using Epsitec.Common.IO;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;

using Epsitec.Cresus.PerformanceTests;
using Epsitec.Cresus.PerformanceTests.Entities;

using System.Linq;


namespace Epsitec.Cresus.PerformanceTests.Concurrency
{


	public sealed class ConcurrencyProgram
	{


		public static void Main(string[] args)
		{
			TestHelper.Initialize ();

			int programNumber = int.Parse (args[0]);
			bool createDatabase = bool.Parse (args[1]);
			string host = args[2];

			int indexNumber = programNumber * 1000;

			if (createDatabase)
			{
				ConcurrencyProgram.CreateDatabase (host);
			}

			ConcurrencyProgram.InsertData (host, indexNumber);
			ConcurrencyProgram.CheckData (host, indexNumber);

			Logger.LogToConsole ("Press <ENTER> to exit");
			System.Console.ReadLine ();
		}


		private static void CreateDatabase(string host)
		{
			Logger.LogToConsole ("Press <ENTER> to create the database");
			System.Console.ReadLine ();

			try
			{
				using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
				{
					DbAccess access = ConcurrencyProgram.GetAccess (host);
					dbInfrastructure.AttachToDatabase (access);

					dbInfrastructure.DropDatabase ();
				}

				System.Threading.Thread.Sleep (2500);
			}
			catch (System.Exception e)
			{
				Logger.LogToConsole ("Something bad happened while dropping the current database: " + e.StackTrace);
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.CreateDatabase (access);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
				{
					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						dataContext.CreateSchema<NaturalPersonEntity> ();
						dataContext.CreateSchema<UriContactEntity> ();
						dataContext.CreateSchema<MailContactEntity> ();
						dataContext.CreateSchema<TelecomContactEntity> ();
					}
				}
			}

			System.Console.WriteLine ("Database has been created");
		}


		private static void InsertData(string host, int indexNumber)
		{
			Logger.LogToConsole ("Press <ENTER> to insert the data");
			System.Console.ReadLine ();

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = ConcurrencyProgram.GetAccess (host);
				dbInfrastructure.AttachToDatabase (access);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
				{
					using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
					{
						for (int i = indexNumber; i < indexNumber + 1000; i++)
						{
							NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

							person.Firstname = "FirstName" + i;

							dataContext.SaveChanges ();

							Logger.LogToConsole ("Created person " + i);
						}
					}
				}
			}
		}


		private static void CheckData(string host, int indexNumber)
		{
			Logger.LogToConsole ("Data has been inserted");

			Logger.LogToConsole ("Press <ENTER> to check data");
			System.Console.ReadLine ();

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = ConcurrencyProgram.GetAccess (host);
				dbInfrastructure.AttachToDatabase (access);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					for (int i = indexNumber; i < indexNumber + 1000; i++)
					{
						NaturalPersonEntity example = new NaturalPersonEntity ()
						{
							Firstname = "FirstName" + i,
						};

						var persons = dataContext.GetByExample (example).ToList ();

						if (persons.Count != 1)
						{
							Logger.LogToConsole ("Person " + i + " not found");
						}
						else
						{
							Logger.LogToConsole ("Person " + i + " found");
						}
					}
				}
				}
			}
			Logger.LogToConsole ("Data check done");
		}


		private static DbAccess GetAccess(string host)
		{
			return new DbAccess ("Firebird", "CORETEST", host, "sysdba", "masterkey", false);
		}


	}


}
