using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Threading;


namespace Epsitec.Cresus.DataLayer.UnitTests
{
	
	
	[TestClass]
	public class UnitTestConcurrency
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		private void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				dataContext.CreateSchema<NaturalPersonEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
			}

			DatabaseHelper.DisconnectFromDatabase ();
		}

		
		[TestMethod]
		public void ConcurrencySequenceAllTest()
		{
			int nbThreads = 2;
			int nbInsertions = 250;

			this.InsertData (nbThreads, nbInsertions, l => this.ThreadActionSequence(l));
			this.CheckData (nbThreads, nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		public void ConcurrencyMixedWriteSequenceReadTest()
		{
			int nbThreads = 2;
			int nbInsertions = 250;

			this.InsertData (nbThreads, nbInsertions, l => this.ThreadActionMixed (l));
			this.CheckData (nbThreads, nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		public void ConcurrencySequenceWriteMixedReadTest()
		{
			int nbThreads = 2;
			int nbInsertions = 250;

			this.InsertData (nbThreads, nbInsertions, l => this.ThreadActionSequence (l));
			this.CheckData (nbThreads, nbInsertions, l => this.ThreadActionMixed (l));
		}


		[TestMethod]
		public void ConcurrencyMixedAllTest()
		{
			int nbThreads = 2;
			int nbInsertions = 250;

			this.InsertData (nbThreads, nbInsertions, l => this.ThreadActionMixed (l));
			this.CheckData (nbThreads, nbInsertions, l => this.ThreadActionMixed (l));
		}


		private void InsertData(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadInsertData (iCopy, nbInsertions)));
			}

			threadFunction (threads);
		}


		private void CheckData(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadCheckData (iCopy, nbInsertions)));
			}

			threads.Add (new Thread (() => this.CheckWholeData (nbThreads, nbInsertions)));

			threadFunction (threads);
		}


		private void ThreadActionSequence(List<Thread> threads)
		{
			foreach (Thread thread in threads)
			{
				thread.Start ();
				thread.Join ();
			}
		}


		private void ThreadActionMixed(List<Thread> threads)
		{
			foreach (Thread thread in threads)
			{
				thread.Start ();
			}

			foreach (Thread thread in threads)
			{
				thread.Join ();
			}
		}


		private void ThreadInsertData(int threadNumber, int nbInsertions)
		{
			int startIndex = threadNumber * nbInsertions;
			
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					this.ThreadInsertionLoop (dataContext, startIndex, nbInsertions);
				}
			}
		}


		private void ThreadInsertionLoop(DataContext dataContext, int startIndex, int nbInsertions)
		{
			for (int i = startIndex; i < startIndex + nbInsertions; i++)
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

				person.Firstname = "FirstName" + i;

				try
				{
					dataContext.SaveChanges ();
				}
				catch (System.Exception e)
				{
					Assert.Fail ("Exception has been thrown: " + e.Message + "\n\n" + e.StackTrace);
				}
			}
		}
		

		private void ThreadCheckData(int threadNumber, int nbInsertions)
		{
			int startIndex = threadNumber * nbInsertions;
			
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					this.ThreadCheckLoop (nbInsertions, startIndex, dataContext);
				}
			}
		}


		private void ThreadCheckLoop(int nbInsertions, int startIndex, DataContext dataContext)
		{
			for (int i = startIndex; i < startIndex + nbInsertions; i++)
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Firstname = "FirstName" + i,
				};

				List<NaturalPersonEntity> persons = new List<NaturalPersonEntity> ();

				try
				{
					persons.AddRange (dataContext.GetByExample (example));
				}
				catch (System.Exception e)
				{
					Assert.Fail ("Exception has been thrown: " + e.Message + "\n\n" + e.StackTrace);
				}

				Assert.IsTrue (persons.Count == 1);
			}
		}


		private void CheckWholeData(int nbThreads, int nbInsertions)
		{
			int nbTotalInsertions = nbThreads * nbInsertions;

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					List<NaturalPersonEntity> persons = new List<NaturalPersonEntity> ();
					
					try
					{
						persons.AddRange (dataContext.GetByExample (new NaturalPersonEntity ()));
					}
					catch (System.Exception e)
					{
						Assert.Fail ("Exception has been thrown: " + e.Message + "\n\n" + e.StackTrace);
					}

					Assert.IsTrue (persons.Count == nbTotalInsertions);
				}
			}
		}
                    

	}


}
