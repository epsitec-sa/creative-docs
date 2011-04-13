using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestThreadSafety
	{


		// TODO Add tests with proxy resolution when AbstractEntity and EntityContext are thread
		// safe.

		// TODO Add tests with events.


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DbInfrastructureHelper.ResetTestDatabase ();
			DatabaseCreator1.PopulateDatabase (DatabaseSize.Small);
		}


		[TestMethod]
		public void NonMutatingOperationsTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: true))
			{
				dataContext.GetByExample (new NaturalPersonEntity ());
				dataContext.GetByExample (new LegalPersonEntity ());
				dataContext.GetByExample (new UriContactEntity ());
				dataContext.GetByExample (new TelecomContactEntity ());
				dataContext.GetByExample (new MailContactEntity ());

				List<Thread> threads = new List<Thread> ();
				System.DateTime time = System.DateTime.Now;

				for (int i = 0; i < 100; i++)
				{
					Thread thread = new Thread
					(
						() =>
						{
							System.Random dice = new System.Random (Thread.CurrentThread.ManagedThreadId);
						
							while (System.DateTime.Now - time < System.TimeSpan.FromSeconds (15))
							{
								this.ExecuteRandomNonMutatingOperation (dataContext, dice);
							}
						}
					);

					threads.Add (thread);
				}

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}
		}


		[TestMethod]
		public void MutatingOperationsTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: true))
			{
				dataContext1.GetByExample (new RegionEntity ());
				dataContext2.GetByExample (new RegionEntity ());

				List<Thread> threads = new List<Thread> ();
				System.DateTime time = System.DateTime.Now;

				for (int i = 0; i < 100; i++)
				{
					Thread thread = new Thread
					(
						() =>
						{
							System.Random dice = new System.Random (Thread.CurrentThread.ManagedThreadId);

							while (System.DateTime.Now - time < System.TimeSpan.FromSeconds (15))
							{
								this.ExecuteRandomMutatingOperation (dataContext1, dataContext2, dice);
							}
						}
					);

					threads.Add (thread);
				}

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}
		}


		[TestMethod]
		public void MixedOperationsTest1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: true))
			{
				dataContext1.GetByExample (new RegionEntity ());
				dataContext2.GetByExample (new RegionEntity ());

				List<Thread> threads = new List<Thread> ();
				System.DateTime time = System.DateTime.Now;

				for (int i = 0; i < 100; i++)
				{
					Thread thread = new Thread
					(
						() =>
						{
							System.Random dice = new System.Random (Thread.CurrentThread.ManagedThreadId);

							while (System.DateTime.Now - time < System.TimeSpan.FromSeconds (15))
							{
								if (dice.NextDouble () <= 0.5)
								{
									this.ExecuteRandomNonMutatingOperation (dataContext1, dice);
								}
								else
								{
									this.ExecuteRandomMutatingOperation (dataContext1, dataContext2, dice);
								}
							}
						}
					);

					threads.Add (thread);
				}

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}
		}


		[TestMethod]
		public void MixedOperationsTest2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < 10; i++)
				{
					var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: true);

					dataContexts.Add (dataContext);
				}

				foreach (var dataContext in dataContexts)
				{
					dataContext.GetByExample (new RegionEntity ());
				}

				List<Thread> threads = new List<Thread> ();
				System.DateTime time = System.DateTime.Now;

				for (int i = 0; i < 100; i++)
				{
					Thread thread = new Thread
					(
						() =>
						{
							System.Random dice = new System.Random (Thread.CurrentThread.ManagedThreadId);

							while (System.DateTime.Now - time < System.TimeSpan.FromSeconds (15))
							{
								if (dice.NextDouble () <= 0.5)
								{
									DataContext dataContext = dataContexts.GetRandomElement ();

									this.ExecuteRandomNonMutatingOperation (dataContext, dice);
								}
								else
								{
									DataContext dataContext1 = dataContexts.GetRandomElement ();
									DataContext dataContext2 = dataContexts.GetRandomElement ();

									this.ExecuteRandomMutatingOperation (dataContext1, dataContext2, dice);
								}
							}
						}
					);

					threads.Add (thread);
				}

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}

				foreach (var dataContext in dataContexts)
				{
					dataContext.Dispose ();
				}
			}
		}


		[TestMethod]
		public void MixedOperationsTest3()
		{
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			List<DataInfrastructure> dataInfrastructures = new List<DataInfrastructure> ();
			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < 10; i++)
			{
				var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (entityEngine);

				dataInfrastructures.Add (dataInfrastructure);

				for (int j = 0; j < 5; j++)
				{
					var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: true);

					dataContexts.Add (dataContext);
				}
			}
			
			foreach (var dataContext in dataContexts)
			{
				dataContext.GetByExample (new RegionEntity ());
			}

			List<Thread> threads = new List<Thread> ();
			System.DateTime time = System.DateTime.Now;

			for (int i = 0; i < 100; i++)
			{
				Thread thread = new Thread
				(
					() =>
					{
						System.Random dice = new System.Random (Thread.CurrentThread.ManagedThreadId);

						while (System.DateTime.Now - time < System.TimeSpan.FromSeconds (15))
						{
							if (dice.NextDouble () <= 0.5)
							{
								DataContext dataContext = dataContexts.GetRandomElement ();

								this.ExecuteRandomNonMutatingOperation (dataContext, dice);
							}
							else
							{
								DataContext dataContext1 = dataContexts.GetRandomElement ();
								DataContext dataContext2 = dataContexts.GetRandomElement ();

								this.ExecuteRandomMutatingOperation (dataContext1, dataContext2, dice);
							}
						}
					}
				);

				threads.Add (thread);
			}

			foreach (var thread in threads)
			{
				thread.Start ();
			}

			foreach (var thread in threads)
			{
				thread.Join ();
			}

			foreach (var dataContext in dataContexts)
			{
				dataContext.Dispose ();
			}

			foreach (var dataInfrastructure in dataInfrastructures)
			{
				dataInfrastructure.Dispose ();
			}
		}


		private void ExecuteRandomNonMutatingOperation(DataContext dataContext, System.Random dice)
		{
			switch (dice.Next (0, 14))
			{
				case 0:
					bool readOnly = dataContext.IsReadOnly;
					break;

				case 1:
					string name = dataContext.Name;
					break;

				case 2:
					bool nullVirtualization = dataContext.EnableNullVirtualization;
					break;

				case 3:
					long id = dataContext.UniqueId;
					break;

				case 4:
					DataContextPool dataContextPool = dataContext.DataContextPool;
					break;

				case 5:
					dataContext.Contains (dataContext.GetEntities ().GetRandomElement ());
					break;

				case 6:
					dataContext.GetNormalizedEntityKey (dataContext.GetEntities ().GetRandomElement ());
					break;

				case 7:
					dataContext.ContainsChanges ();
					break;

				case 8:
					dataContext.IsRegisteredAsEmptyEntity (dataContext.GetEntities ().GetRandomElement ());
					break;

				case 9:
					dataContext.IsDeleted (dataContext.GetEntities ().GetRandomElement ());
					break;

				case 10:
					dataContext.GetEntities ();
					break;

				case 11:
					dataContext.IsForeignEntity (dataContext.GetEntities ().GetRandomElement ());
					break;

				case 12:
					dataContext.GetPersistedId (dataContext.GetEntities ().GetRandomElement ());
					break;

				case 13:
					dataContext.IsPersistent (dataContext.GetEntities ().GetRandomElement ());
					break;

				default:
					throw new System.NotImplementedException ();
			}
		}


		private void ExecuteRandomMutatingOperation(DataContext dataContext1, DataContext dataContext2, System.Random dice)
		{
			switch (dice.Next (0, 11))
			{
				case 0:
					dataContext1.GetPersistedEntity ("db:" + Druid.Parse ("[J1AB1]") + ":" + new DbKey (new DbId (1000000000 + dice.Next (1, 200))).Id);
					break;

				case 1:
					dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000 + dice.Next (1, 100))));
					break;

				case 2:
					dataContext1.ResolveEntity (Druid.Parse ("[J1AA1]"), new DbKey (new DbId (1000000000 + dice.Next (1, 600))));
					break;

				case 3:
					dataContext1.ResolveEntity (new EntityKey (Druid.Parse ("[J1A9]"), new DbKey (new DbId (1000000000 + dice.Next (1, 75)))));
					break;

				case 4:
					dataContext1.Reload ();
					break;

				case 5:
					DataContext.CopyEntity (dataContext1, dataContext1.GetEntities ().GetRandomElement (), dataContext2);
					break;

				case 6:
					DataContext.CopyEntity (dataContext2, dataContext2.GetEntities ().GetRandomElement (), dataContext1);
					break;

				case 7:
					dataContext1.GetLocalEntity (dataContext2.GetEntities ().GetRandomElement ());
					break;

				case 8:
					dataContext2.GetLocalEntity (dataContext1.GetEntities ().GetRandomElement ());
					break;

				case 9:
					dataContext1.GetByExample
					(
						new NaturalPersonEntity ()
						{
							Lastname = "firstname" + dice.Next (0, 100)
						}
					);
					break;

				case 10:
					dataContext1.GetByRequest<LegalPersonEntity>
					(
						new Request ()
						{
							RootEntity = new LegalPersonEntity ()
							{
								Name = "name" + dice.Next (0, 100)
							}
						}
					);
					break;

				default:
					throw new System.NotImplementedException ();
			}
		}                                                        


	}


}
