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
			this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence(l));
			this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		public void ConcurrencyMixedWriteSequenceReadTest()
		{
			this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
			this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		public void ConcurrencySequenceWriteMixedReadTest()
		{
			this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
			this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
		}


		[TestMethod]
		public void ConcurrencyMixedAllTest()
		{
			this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
			this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
		}


		private void InsertData(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.errorMessages = new List<string> ();
			
			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadInsertData (iCopy, nbInsertions)));
			}

			threadFunction (threads);

			this.FinalizeTest ();
		}


		private void CheckData(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.errorMessages = new List<string> ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadCheckData (iCopy, nbInsertions)));
			}

			threads.Add (new Thread (() => this.CheckWholeData (nbThreads, nbInsertions)));

			threadFunction (threads);

			this.FinalizeTest ();
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
			try
			{
				for (int i = startIndex; i < startIndex + nbInsertions; i++)
				{
					NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

					person.Firstname = "FirstName" + i;

					dataContext.SaveChanges ();
				}
			}
			catch (System.Exception e)
			{
				lock (this.errorMessages)
				{
					this.errorMessages.Add (e.Message + "\n" + e.StackTrace);
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
			try
			{
				for (int i = startIndex; i < startIndex + nbInsertions; i++)
				{
					NaturalPersonEntity example = new NaturalPersonEntity ()
					{
						Firstname = "FirstName" + i,
					};

					List<NaturalPersonEntity> persons = new List<NaturalPersonEntity> ();
					
					persons.AddRange (dataContext.GetByExample (example));

					if (persons.Count == 0)
					{
						lock (this.errorMessages)
						{
							this.errorMessages.Add ("Insertion " + i + "not found");
						}
					}
					else if (persons.Count > 1)
					{
						lock (this.errorMessages)
						{
							this.errorMessages.Add ("Duplicate insertion " + i);
						}
					}
				}
			}
			catch (System.Exception e)
			{
				lock (this.errorMessages)
				{
					this.errorMessages.Add (e.Message + "\n" + e.StackTrace);
				}
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
					try
					{
						List<NaturalPersonEntity> persons = new List<NaturalPersonEntity> ();
											
						persons.AddRange (dataContext.GetByExample (new NaturalPersonEntity ()));

						if (persons.Count != nbTotalInsertions)
						{
							lock (this.errorMessages)
							{
								this.errorMessages.Add ("Total number of insertions is " + persons.Count + " but " + nbTotalInsertions + " is expected");
							}
						}
					}
					catch (System.Exception e)
					{
						lock (this.errorMessages)
						{
							this.errorMessages.Add (e.Message + "\n" + e.StackTrace);
						}
					}
				}
			}
		}


		private void FinalizeTest()
		{
			if (errorMessages.Count > 0)
			{
				string message = "\n\n" + string.Join ("\n\n", this.errorMessages);

				Assert.Fail (message);
			}
		}


		private List<string> errorMessages;


		private readonly int nbThreads = 2;


		private readonly int nbInsertions = 250;
	}


}
