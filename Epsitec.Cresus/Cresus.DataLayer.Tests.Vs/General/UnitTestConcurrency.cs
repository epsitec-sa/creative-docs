using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestConcurrency
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetEmptyTestDatabase ();
		}


		[TestMethod]
		public void ConcurrencySequenceAllTest()
		{
			this.InsertData (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
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


		[TestMethod]
		public void ConflictingValuesUpdatesSequence()
		{
			this.ConflictingValueUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		public void ConflictingValuesUpdatesMixed()
		{
			this.ConflictingValueUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
		}


		[TestMethod]
		public void ConflictingReferenceUpdatesSequence()
		{
			this.ConflictingReferenceUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		public void ConflictingReferenceUpdatesMixed()
		{
			this.ConflictingReferenceUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
		}


		[TestMethod]
		public void ConflictingCollectionUpdatesSequence()
		{
			this.ConflictingCollectionUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
		public void ConflictingCollectionUpdatesMixed()
		{
			this.ConflictingCollectionUpdates (this.nbThreads, this.nbInsertions, l => this.ThreadActionMixed (l));
		}


		[TestMethod]
		public void ConflictingSequence()
		{
			this.Conflicting (this.nbThreads, this.nbInsertions / 5, l => this.ThreadActionSequence (l));
		}


		[TestMethod]
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
			this.handledExceptions = new List<System.Exception> ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadInsertData (iCopy, nbInsertions)));
			}

			threadFunction (threads);

			this.FinalizeTest ("handledExceptions", this.handledExceptions);
		}


		private void ThreadInsertData(int threadNumber, int nbInsertions)
		{
			try
			{
				int startIndex = threadNumber * nbInsertions;

				using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					this.ThreadInsertionLoop (dataContext, startIndex, nbInsertions);
				}
			}
			catch (System.Exception e)
			{
				lock (this.handledExceptions)
				{
					this.handledExceptions.Add (e);
				}
			}
		}


		private void ThreadInsertionLoop(DataContext dataContext, int startIndex, int nbInsertions)
		{
			for (int i = startIndex; i < startIndex + nbInsertions; i++)
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

				person.Firstname = "FirstName" + i;

				dataContext.SaveChanges ();
			}
		}


		private void CheckData(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.handledExceptions = new List<System.Exception> ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadCheckData (iCopy, nbInsertions)));
			}

			this.CheckWholeData (nbThreads, nbInsertions);

			threadFunction (threads);

			this.FinalizeTest ("handledExceptions", this.handledExceptions);
		}


		private void ThreadCheckData(int threadNumber, int nbInsertions)
		{
			try
			{
				int startIndex = threadNumber * nbInsertions;

				using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					this.ThreadCheckLoop (nbInsertions, startIndex, dataContext);
				}
			}
			catch (System.Exception e)
			{
				lock (this.handledExceptions)
				{
					this.handledExceptions.Add (e);
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

				persons.AddRange (dataContext.GetByExample (example));

				Assert.IsTrue (persons.Count == 1);
			}
		}


		private void CheckWholeData(int nbThreads, int nbInsertions)
		{
			int nbTotalInsertions = nbThreads * nbInsertions;

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				List<NaturalPersonEntity> persons = new List<NaturalPersonEntity> ();

				persons.AddRange (dataContext.GetByExample (new NaturalPersonEntity ()));

				Assert.IsTrue (persons.Count == nbTotalInsertions);
			}
		}


		private void ConflictingValueUpdates(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.handledExceptions = new List<System.Exception> ();

			this.ConflictingValueUpdateSetup ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadConflictingValueUpdates (iCopy, nbInsertions)));
			}

			threadFunction (threads);

			this.FinalizeTest ("handledExceptions", this.handledExceptions);
		}



		private void ConflictingValueUpdateSetup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

				person.Firstname = "name";

				dataContext.SaveChanges ();
			}
		}


		private void ThreadConflictingValueUpdates(int threadNumber, int nbInsertions)
		{
			try
			{
				int startIndex = threadNumber * nbInsertions;

				using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					this.ThreadConflictingValueUpdatesLoop (nbInsertions, startIndex, dataContext);
				}
			}
			catch (System.Exception e)
			{
				lock (this.handledExceptions)
				{
					this.handledExceptions.Add (e);
				}
			}
		}


		private void ThreadConflictingValueUpdatesLoop(int nbInsertions, int startIndex, DataContext dataContext)
		{
			NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

			for (int i = startIndex; i < startIndex + nbInsertions; i++)
			{
				naturalPerson.Firstname = "name" + i;

				dataContext.SaveChanges ();
			}
		}


		private void ConflictingReferenceUpdates(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.handledExceptions = new List<System.Exception> ();

			this.ConflictingReferenceUpdateSetup ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadConflictingReferenceUpdates (iCopy, nbInsertions)));
			}

			threadFunction (threads);

			this.FinalizeTest ("handledExceptions", this.handledExceptions);
		}



		private void ConflictingReferenceUpdateSetup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity gender1 = dataContext.CreateEntity<PersonGenderEntity> ();
				PersonGenderEntity gender2 = dataContext.CreateEntity<PersonGenderEntity> ();

				dataContext.SaveChanges ();
			}
		}


		private void ThreadConflictingReferenceUpdates(int threadNumber, int nbInsertions)
		{
			try
			{
				int startIndex = threadNumber * nbInsertions;

				using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					this.ThreadConflictingReferenceUpdatesLoop (nbInsertions, startIndex, dataContext);
				}
			}
			catch (System.Exception e)
			{
				lock (this.handledExceptions)
				{
					this.handledExceptions.Add (e);
				}
			}
		}


		private void ThreadConflictingReferenceUpdatesLoop(int nbInsertions, int startIndex, DataContext dataContext)
		{
			NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
			List<PersonGenderEntity> genders = new List<PersonGenderEntity> ()
            {
                dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001))),
                dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002))),
            };

			System.Random dice = new System.Random ();

			for (int i = startIndex; i < startIndex + nbInsertions; i++)
			{
				int diceNext = dice.Next (0, 2);
				person.Gender = genders[diceNext];

				dataContext.SaveChanges ();
			}
		}


		private void ConflictingCollectionUpdates(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.handledExceptions = new List<System.Exception> ();

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

			if (!this.handledExceptions.Any ())
			{
				this.CheckConflictingCollectionUpdates (nbContactsToUse);
			}

			this.FinalizeTest ("handledExceptions", this.handledExceptions);
		}



		private void ConflictingCollectionUpdateSetup(int nbContacts)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

				for (int i = 0; i < nbContacts; i++)
				{
					UriContactEntity contact = dataContext.CreateEntity<UriContactEntity> ();
				}

				dataContext.SaveChanges ();
			}
		}


		private void ThreadConflictingCollectionUpdates(int threadNumber, int nbInsertions, int nbContacts, int nbContactsToUse)
		{
			try
			{
				int startIndex = threadNumber * nbInsertions;

				using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					this.ThreadConflictingCollectionUpdatesLoop (nbInsertions, startIndex, dataContext, nbContacts, nbContactsToUse);
				}
			}
			catch (System.Exception e)
			{
				lock (this.handledExceptions)
				{
					this.handledExceptions.Add (e);
				}
			}
		}


		private void ThreadConflictingCollectionUpdatesLoop(int nbInsertions, int startIndex, DataContext dataContext, int nbTotalContacts, int nbContactsToUse)
		{
			NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
			List<UriContactEntity> contacts = new List<UriContactEntity> ();

			for (int i = 0; i < nbTotalContacts; i++)
			{
				contacts.Add (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000 + i + 1))));
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


		private void CheckConflictingCollectionUpdates(int nbContactsToUse)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.IsTrue (person.Contacts.Count == nbContactsToUse);
			}
		}


		private void Conflicting(int nbThreads, int nbInsertions, System.Action<List<Thread>> threadFunction)
		{
			this.handledExceptions = new List<System.Exception> ();

			this.ConflictingSetup ();

			List<Thread> threads = new List<Thread> ();

			for (int i = 0; i < nbThreads; i++)
			{
				int iCopy = i;

				threads.Add (new Thread (() => this.ThreadConflicting (iCopy, nbInsertions)));
			}

			threadFunction (threads);

			this.FinalizeTest ("handledExceptions", this.handledExceptions);
		}



		private void ConflictingSetup()
		{
			DatabaseCreator1.PopulateDatabase (DatabaseSize.Small);
		}


		private void ThreadConflicting(int threadNumber, int nbInsertions)
		{
			try
			{
				int startIndex = threadNumber * nbInsertions;

				using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					this.ThreadConflictingLoop (nbInsertions, startIndex, dataContext);
				}
			}
			catch (System.Exception e)
			{
				lock (this.handledExceptions)
				{
					this.handledExceptions.Add (e);
				}
			}
		}


		private void ThreadConflictingLoop(int nbInsertions, int startIndex, DataContext dataContext)
		{
			System.Random dice = new System.Random ();

			for (int i = startIndex; i < startIndex + nbInsertions; i++)
			{
				switch (dice.Next (0, 6))
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


		private void ThreadConflictingLoopCase0(DataContext dataContext, System.Random dice)
		{
			List<AddressEntity> addresses = new List<AddressEntity> ();

			for (int i = 0; i < 10; i++)
			{
				addresses.Add (dataContext.ResolveEntity<AddressEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 151)))));
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
				persons.Add (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 101)))));
			}

			foreach (NaturalPersonEntity person in persons)
			{
				person.Title.Name = dice.Next ().ToString ();
				person.Gender.Code = dice.Next ().ToString ();
				person.PreferredLanguage.Name = dice.Next ().ToString ();

				IList<AbstractContactEntity> contacts1 = person.Contacts;
				IList<AbstractContactEntity> contacts2 = contacts1.Shuffle ().ToList ();

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
				contacts.Add (dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 601)))));
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
				contacts.Add (dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 601)))));
			}

			for (int i = 0; i < 5; i++)
			{
				uriSchemes.Add (dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 6)))));
			}

			for (int i = 0; i < 5; i++)
			{
				telecomTypes.Add (dataContext.ResolveEntity<TelecomTypeEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 6)))));
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
				persons.Add (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 100)))));
			}

			for (int i = 0; i < 2; i++)
			{
				genders.Add (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 3)))));
			}

			for (int i = 0; i < 5; i++)
			{
				titles.Add (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 16)))));
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
				mailContacts.Add (dataContext.ResolveEntity<MailContactEntity> (new DbKey (new DbId (1000000000 + dice.Next (401, 601)))));
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


		private void FinalizeTest(string name, List<System.Exception> exceptions)
		{
			if (exceptions.Count > 0)
			{
				string message = name + ":\n\n" + string.Join ("\n\n", exceptions.Select (e => this.GetExceptionString (e)));

				Assert.Fail (message);
			}
		}


		private string GetExceptionString(System.Exception e)
		{
			return e.Message + "\n\n" + e.StackTrace;
		}


		private List<System.Exception> handledExceptions;


		private readonly int nbThreads = 5;


		private readonly int nbInsertions = 250;


	}


}
