using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

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
        public void TestInitialize()
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
		[Ignore]
        public void ConcurrencySequenceAllTest()
        {
            this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence(l));
            this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
        }


		[TestMethod]
		[Ignore]
        public void ConcurrencyMixedWriteSequenceReadTest()
        {
            this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
            this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
        }


		[TestMethod]
		[Ignore]
        public void ConcurrencySequenceWriteMixedReadTest()
        {
            this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
            this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
        }


		[TestMethod]
		[Ignore]
        public void ConcurrencyMixedAllTest()
        {
            this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
            this.CheckData (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
        }


		[TestMethod]
		[Ignore]
        public void ConflictingValuesUpdatesSequence()
        {
            this.ConflictingValueUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
        }


		[TestMethod]
		[Ignore]
        public void ConflictingValueUpdatesMixed()
        {
            this.ConflictingValueUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
        }


		[TestMethod]
		[Ignore]
        public void ConflictingReferenceUpdatesSequence()
        {
            this.ConflictingReferenceUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
        }


		[TestMethod]
		[Ignore]
        public void ConflictingReferenceUpdatesMixed()
        {
            this.ConflictingReferenceUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
		}


		[TestMethod]
		[Ignore]
		public void ConflictingCollectionUpdatesSequence()
		{
			this.ConflictingCollectionUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		[Ignore]
		public void ConflictingCollectionUpdatesMixed()
		{
			this.ConflictingCollectionUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
		}


		[TestMethod]
		[Ignore]
		public void ConflictingSequence()
		{
			this.Conflicting (this.nbThreads, this.nbInsertions / 5, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		[Ignore]
		public void ConflictingMixed()
		{
			this.Conflicting (this.nbThreads, this.nbInsertions / 5, l => this.ThreadActionMixed (l));
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


        private void ConflictingValueUpdates(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
        {
            this.errorMessages = new List<string> ();

            this.ConflictingValueUpdateSetup ();

            List<Thread> threads = new List<Thread> ();

            for (int i = 0; i < nbThreads; i++)
            {
                int iCopy = i;

                threads.Add (new Thread (() => this.ThreadConflictingValueUpdates (iCopy, nbInsertions)));
            }

            threadFunction (threads);

            this.FinalizeTest ();
        }



        private void ConflictingValueUpdateSetup()
        {
            DatabaseHelper.ConnectToDatabase ();

            using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
            {
                NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

                person.Firstname = "name";

                dataContext.SaveChanges ();
			}

			DatabaseHelper.DisconnectFromDatabase ();
        }


		private void ThreadConflictingValueUpdates(int threadNumber, int nbInsertions)
		{
			int startIndex = threadNumber * nbInsertions;

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					this.ThreadConflictingValueUpdatesLoop (nbInsertions, startIndex, dataContext);
				}
			}
		}


        private void ThreadConflictingValueUpdatesLoop(int nbInsertions, int startIndex, DataContext dataContext)
        {
            try
            {
                NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

                for (int i = startIndex; i < startIndex + nbInsertions; i++)
                {
                    naturalPerson.Firstname = "name" + i;

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


		private void ConflictingReferenceUpdates(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.errorMessages = new List<string> ();

			this.ConflictingReferenceUpdateSetup ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadConflictingReferenceUpdates (iCopy, nbInsertions)));
			}

			threadFunction (threads);

			this.CheckConflictingReferenceUpdates ();

			this.FinalizeTest ();
		}



		private void ConflictingReferenceUpdateSetup()
		{
			DatabaseHelper.ConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity gender1 = dataContext.CreateEntity<PersonGenderEntity> ();
				PersonGenderEntity gender2 = dataContext.CreateEntity<PersonGenderEntity> ();

				dataContext.SaveChanges ();
			}

			DatabaseHelper.DisconnectFromDatabase ();
		}


		private void ThreadConflictingReferenceUpdates(int threadNumber, int nbInsertions)
		{
			int startIndex = threadNumber * nbInsertions;

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					this.ThreadConflictingReferenceUpdatesLoop (nbInsertions, startIndex, dataContext);
				}
			}
		}


		private void ThreadConflictingReferenceUpdatesLoop(int nbInsertions, int startIndex, DataContext dataContext)
		{
			try
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				List<PersonGenderEntity> genders = new List<PersonGenderEntity> ()
				{
					dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2))),
				};

				System.Random dice = new System.Random ();

				for (int i = startIndex; i < startIndex + nbInsertions; i++)
				{
					int diceNext = dice.Next (0, 2);
					person.Gender = genders[diceNext];

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


		private void CheckConflictingReferenceUpdates()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					try
					{
						using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
						{
							SqlSelect query = new SqlSelect ();

							query.Fields.Add (new SqlField ("CR_ID", "CR_ID"));

							query.Tables.Add (new SqlField ("X_L0A11_L0AN", "X_L0A11_L0AN"));

							System.Data.DataTable data = dbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

							Assert.IsTrue (data.Rows.Count == 1);
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


		private void ConflictingCollectionUpdates(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.errorMessages = new List<string> ();

			int nbTotalContacts = 10;
			int nbContactsToUse = 5;

			this.ConflictingCollectionUpdateSetup (nbTotalContacts);

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadConflictingCollectionUpdates (iCopy, nbInsertions, nbTotalContacts, nbContactsToUse)));
			}

			threadFunction (threads);

			this.CheckConflictingCollectionUpdates (nbContactsToUse);

			this.FinalizeTest ();
		}



		private void ConflictingCollectionUpdateSetup(int nbContacts)
		{
			DatabaseHelper.ConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

				for (int i = 0; i < nbContacts; i++)
				{
					UriContactEntity contact = dataContext.CreateEntity<UriContactEntity> ();	
				}
				
				dataContext.SaveChanges ();
			}

			DatabaseHelper.DisconnectFromDatabase ();
		}


		private void ThreadConflictingCollectionUpdates(int threadNumber, int nbInsertions, int nbContacts, int nbContactsToUse)
		{
			int startIndex = threadNumber * nbInsertions;

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					this.ThreadConflictingCollectionUpdatesLoop (nbInsertions, startIndex, dataContext, nbContacts, nbContactsToUse);
				}
			}
		}


		private void ThreadConflictingCollectionUpdatesLoop(int nbInsertions, int startIndex, DataContext dataContext, int nbTotalContacts, int nbContactsToUse)
		{
			try
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				List<UriContactEntity> contacts = new List<UriContactEntity> ();

				for (int i = 0; i < nbTotalContacts; i++)
                {
					contacts.Add (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (i + 1))));
                }

				System.Random dice = new System.Random ();

				for (int i = startIndex; i < startIndex + nbInsertions; i++)
				{
					List<UriContactEntity> contactsCopy = new List<UriContactEntity> (contacts);
					
					person.Contacts.Clear ();

					for (int j = 0; j < nbContactsToUse; j++)
					{
						person.Contacts.Add (contactsCopy[dice.Next (0, contactsCopy.Count)]);
					}
					
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


		private void CheckConflictingCollectionUpdates(int nbContactsToUse)
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					try
					{
						NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						Assert.IsTrue (person.Contacts.Count == nbContactsToUse);
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


		private void Conflicting(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.errorMessages = new List<string> ();

			this.ConflictingSetup ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadConflicting (iCopy, nbInsertions)));
			}

			threadFunction (threads);

			this.FinalizeTest ();
		}



		private void ConflictingSetup()
		{
			DatabaseHelper.ConnectToDatabase ();

			DatabaseCreator1.PopulateDatabase (DatabaseSize.Small);

			DatabaseHelper.DisconnectFromDatabase ();
		}


		private void ThreadConflicting(int threadNumber, int nbInsertions)
		{
			int startIndex = threadNumber * nbInsertions;

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				dbInfrastructure.AttachToDatabase (access);

				using (DataContext dataContext = new DataContext (dbInfrastructure))
				{
					this.ThreadConflictingLoop (nbInsertions, startIndex, dataContext);
				}
			}
		}


		private void ThreadConflictingLoop(int nbInsertions, int startIndex, DataContext dataContext)
		{
			try
			{
				System.Random dice = new System.Random ();

				for (int i = startIndex; i < startIndex + nbInsertions; i++)
				{
					switch (dice.Next(0, 6))
					{
						case 0:
							this.ThreadConflictingLoopCase0 (dataContext, dice);
							break;

						case 1:
							this.ThreadConflictingLoopCase1 (dataContext, dice);
							break;

						case 2:
							this.ThreadConflictingLoopCase2 (dataContext, dice);
							break;

						case 3:
							this.ThreadConflictingLoopCase3 (dataContext, dice);
							break;

						case 4:
							this.ThreadConflictingLoopCase4 (dataContext, dice);
							break;

						default:
							this.ThreadConflictingLoopCaseDefault (dataContext, dice);
							break;
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


		private void ThreadConflictingLoopCase0(DataContext dataContext, System.Random dice)
		{
			List<AddressEntity> addresses = new List<AddressEntity> ();

			for (int i = 0; i < 10; i++)
			{
				addresses.Add (dataContext.ResolveEntity<AddressEntity> (new DbKey (new DbId (dice.Next (1, 151)))));
			}

			for (int i = 0; i < addresses.Count; i++)
			{
				addresses[i].Location.Name = dice.Next ().ToString ();
				addresses[i].PostBox.Number = dice.Next ().ToString ();
				addresses[i].Street.StreetName = dice.Next ().ToString ();

				addresses[i].Location = addresses[dice.Next (0, addresses.Count)].Location;
				addresses[i].PostBox = addresses[dice.Next (0, addresses.Count)].PostBox;
				addresses[i].Street = addresses[dice.Next (0, addresses.Count)].Street;

				addresses[i].Location.Name = dice.Next ().ToString ();
				addresses[i].PostBox.Number = dice.Next ().ToString ();
				addresses[i].Street.StreetName = dice.Next ().ToString ();
			}

			dataContext.SaveChanges ();
		}


		private void ThreadConflictingLoopCase1(DataContext dataContext, System.Random dice)
		{
			List<NaturalPersonEntity> persons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < 10; i++)
			{
				persons.Add (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (dice.Next (1, 101)))));
			}

			foreach (NaturalPersonEntity person in persons)
			{
				person.Title.Name = dice.Next ().ToString ();
				person.Gender.Code = dice.Next ().ToString ();
				person.PreferredLanguage.Name = dice.Next ().ToString ();
				
				IList<AbstractContactEntity> contacts1 = person.Contacts;
				IList<AbstractContactEntity> contacts2 = this.Shuffle (contacts1);
				
				person.Contacts.Clear ();
				
				foreach (AbstractContactEntity contact in contacts2)
				{
					person.Contacts.Add (contact);
				}
			}

			dataContext.SaveChanges ();
		}


		private void ThreadConflictingLoopCase2(DataContext dataContext, System.Random dice)
		{
			List<AbstractContactEntity> contacts = new List<AbstractContactEntity> ();

			for (int i = 0; i < 25; i++)
			{
				contacts.Add (dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (dice.Next (1, 601)))));
			}

			for (int i = 0; i < contacts.Count; i++)
			{
				int j = dice.Next (0, contacts.Count);

				NaturalPersonEntity personi = contacts[i].NaturalPerson;
				NaturalPersonEntity personj = contacts[j].NaturalPerson;

				contacts[i].NaturalPerson = personj;
				contacts[j].NaturalPerson = personi;

				personi.Contacts.Remove (contacts[i]);
				personj.Contacts.Remove (contacts[j]);

				personi.Contacts.Add (contacts[j]);
				personj.Contacts.Add (contacts[i]);
			}

			dataContext.SaveChanges ();
		}


		private void ThreadConflictingLoopCase3(DataContext dataContext, System.Random dice)
		{
			List<AbstractContactEntity> contacts = new List<AbstractContactEntity> ();
			List<UriSchemeEntity> uriSchemes = new List<UriSchemeEntity> ();
			List<TelecomTypeEntity> telecomTypes = new List<TelecomTypeEntity> ();

			for (int i = 0; i < 25; i++)
			{
				contacts.Add (dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (dice.Next (1, 601)))));
			}

			for (int i = 0; i < 5; i++)
			{
				uriSchemes.Add (dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (dice.Next (1, 6)))));
			}

			for (int i = 0; i < 5; i++)
			{
				telecomTypes.Add (dataContext.ResolveEntity<TelecomTypeEntity> (new DbKey (new DbId (dice.Next (1, 6)))));
			}

			foreach (UriContactEntity contact in contacts.OfType<UriContactEntity> ())
			{
				contact.Uri = dice.Next ().ToString ();
				contact.UriScheme = uriSchemes[dice.Next (0, uriSchemes.Count)];
			}

			foreach (TelecomContactEntity contact in contacts.OfType<TelecomContactEntity> ())
			{
				contact.Number = dice.Next ().ToString ();
				contact.TelecomType = telecomTypes[dice.Next (0, telecomTypes.Count)];
			}

			dataContext.SaveChanges ();
		}


		private void ThreadConflictingLoopCase4(DataContext dataContext, System.Random dice)
		{
			List<NaturalPersonEntity> persons = new List<NaturalPersonEntity> ();
			List<PersonGenderEntity> genders = new List<PersonGenderEntity> ();
			List<PersonTitleEntity> titles = new List<PersonTitleEntity> ();

			for (int i = 0; i < 10; i++)
			{
				persons.Add (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (dice.Next (1, 100)))));
			}

			for (int i = 0; i < 2; i++)
			{
				genders.Add (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (dice.Next (1, 3)))));
			}

			for (int i = 0; i < 5; i++)
			{
				titles.Add (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (dice.Next (1, 16)))));
			}

			foreach (NaturalPersonEntity person in persons)
			{
				person.Gender = genders[dice.Next (0, genders.Count)];
				person.Title = titles[dice.Next (0, titles.Count)];

				person.Firstname = dice.Next ().ToString ();
				person.Lastname = dice.Next ().ToString ();
			}

			dataContext.SaveChanges ();
		}


		private void ThreadConflictingLoopCaseDefault(DataContext dataContext, System.Random dice)
		{
			List<MailContactEntity> mailContacts = new List<MailContactEntity> ();

			for (int i = 0; i < 10; i++)
			{
				mailContacts.Add (dataContext.ResolveEntity<MailContactEntity> (new DbKey (new DbId (dice.Next (401, 601)))));
			}

			for (int i = 0; i < mailContacts.Count; i++)
			{
				mailContacts[i].Address = mailContacts[dice.Next (0, mailContacts.Count)].Address;
				mailContacts[i].Comments.Add (mailContacts[dice.Next (0, mailContacts.Count)].Comments.First ());
				mailContacts[i].Roles.Add (mailContacts[dice.Next (0, mailContacts.Count)].Roles.First ());
				mailContacts[i].Complement = dice.Next ().ToString ();
			}

			dataContext.SaveChanges ();
		}

				
		private void FinalizeTest()
        {
            if (errorMessages.Count > 0)
            {
                string message = "\n\n" + string.Join ("\n\n", this.errorMessages);

                Assert.Fail (message);
            }
        }


		private List<T> Shuffle<T>(IList<T> list)
		{
			List<T> copy = new List<T> (list);
			List<T> shuffled = new List<T> ();

			System.Random dice = new System.Random();

			while (copy.Any ())
			{
				int index = dice.Next (0, copy.Count);

				shuffled.Add (copy[index]);
				copy.RemoveAt (index);
			}

			return shuffled;
		}


        private List<string> errorMessages;


        private readonly int nbThreads = 5;


        private readonly int nbInsertions = 250;


    }


}
