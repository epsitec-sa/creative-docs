using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Exceptions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestThreadSafety
	{


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
			long nbCalls = 0;

			Func<int, int, bool> isCache = (i, j) => true;

			Action<DataContext> initialization = (d) =>
			{
				d.GetByExample (new NaturalPersonEntity ());
				d.GetByExample (new LegalPersonEntity ());
				d.GetByExample (new UriContactEntity ());
				d.GetByExample (new TelecomContactEntity ());
				d.GetByExample (new MailContactEntity ());
				d.GetByExample (new PersonTitleEntity ());
			};

			Action<int, List<DataContext>> action = (i, dataContexts) =>
			{
				DataContext d = dataContexts.Single ();

				this.ExecuteRandomNonMutatingOperation (d);

				Interlocked.Increment (ref nbCalls);
			};

			this.BuildContextAndExecute (1, 1, isCache, initialization, action);

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("NON MUTATING OPERATIONS TEST");
			Console.WriteLine ("NB CALLS: " + nbCalls);
		}


		[TestMethod]
		public void MutatingOperationsTest()
		{
			long nbSingleCalls = 0;
			long nbDoubleCalls = 0;

			Func<int, int, bool> isCache = (i, j) => true;

			Action<DataContext> initialization = (d) =>
			{
				d.GetByExample (new PersonTitleEntity ());
			};

			Action<int, List<DataContext>> action = (i, dataContexts) =>
			{
				switch (this.Dice.Next (0, 2))
				{
					case 0:
						{
							DataContext d = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationSingle (d);

							Interlocked.Increment (ref nbSingleCalls);

							break;
						}
					case 1:
						{
							DataContext d1 = dataContexts.GetRandomElement ();
							DataContext d2 = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationDouble (d1, d2);

							Interlocked.Increment (ref nbDoubleCalls);

							break;
						}
				}
			};

			this.BuildContextAndExecute (1, 2, isCache, initialization, action);

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("MUTATING OPERATIONS TEST");
			Console.WriteLine ("NB CALLS SINGLE: " + nbSingleCalls);
			Console.WriteLine ("NB CALLS DOUBLE: " + nbDoubleCalls);
		}


		[TestMethod]
		public void MixedOperationsTest1()
		{
			long nbNonMutatingCalls = 0;
			long nbMutatingSingleCalls = 0;
			long nbMutatingDoubleCalls = 0;

			Func<int, int, bool> isCache = (i, j) => true;

			Action<DataContext> initialization = (d) =>
			{
				d.GetByExample (new PersonTitleEntity ());
			};

			Action<int, List<DataContext>> action = (i, dataContexts) =>
			{
				switch (this.Dice.Next (0, 3))
				{
					case 0:
						{
							DataContext d = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationSingle (d);

							Interlocked.Increment (ref nbMutatingSingleCalls);

							break;
						}
					case 1:
						{
							DataContext d1 = dataContexts.GetRandomElement ();
							DataContext d2 = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationDouble (d1, d2);

							Interlocked.Increment (ref nbMutatingDoubleCalls);

							break;
						}
					case 2:
						{
							DataContext d = dataContexts.GetRandomElement ();

							this.ExecuteRandomNonMutatingOperation (d);

							Interlocked.Increment (ref nbNonMutatingCalls);

							break;
						}
				}
			};

			this.BuildContextAndExecute (1, 2, isCache, initialization, action);

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("MIXED OPERATIONS TEST 1");
			Console.WriteLine ("NB CALLS NON MUTATING: " + nbNonMutatingCalls);
			Console.WriteLine ("NB CALLS MUTATING SINGLE: " + nbMutatingSingleCalls);
			Console.WriteLine ("NB CALLS MUTATING DOUBLE: " + nbMutatingDoubleCalls);
		}


		[TestMethod]
		public void MixedOperationsTest2()
		{
			long nbNonMutatingCalls = 0;
			long nbMutatingSingleCalls = 0;
			long nbMutatingDoubleCalls = 0;

			Func<int, int, bool> isCache = (i, j) => true;

			Action<DataContext> initialization = (d) =>
			{
				d.GetByExample (new PersonTitleEntity ());
			};

			Action<int, List<DataContext>> action = (i, dataContexts) =>
			{
				switch (this.Dice.Next (0, 3))
				{
					case 0:
						{
							DataContext d = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationSingle (d);

							Interlocked.Increment (ref nbMutatingSingleCalls);

							break;
						}
					case 1:
						{
							DataContext d1 = dataContexts.GetRandomElement ();
							DataContext d2 = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationDouble (d1, d2);

							Interlocked.Increment (ref nbMutatingDoubleCalls);

							break;
						}
					case 2:
						{
							DataContext d = dataContexts.GetRandomElement ();

							this.ExecuteRandomNonMutatingOperation (d);

							Interlocked.Increment (ref nbNonMutatingCalls);

							break;
						}
				}
			};

			this.BuildContextAndExecute (1, 10, isCache, initialization, action);

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("MIXED OPERATIONS TEST 2");
			Console.WriteLine ("NB CALLS NON MUTATING: " + nbNonMutatingCalls);
			Console.WriteLine ("NB CALLS MUTATING SINGLE: " + nbMutatingSingleCalls);
			Console.WriteLine ("NB CALLS MUTATING DOUBLE: " + nbMutatingDoubleCalls);
		}


		[TestMethod]
		public void MixedOperationsTest3()
		{
			long nbNonMutatingCalls = 0;
			long nbMutatingSingleCalls = 0;
			long nbMutatingDoubleCalls = 0;

			Func<int, int, bool> isCache = (i, j) => true;

			Action<DataContext> initialization = (d) =>
			{
				d.GetByExample (new PersonTitleEntity ());
			};

			Action<int, List<DataContext>> action = (i, dataContexts) =>
			{
				switch (this.Dice.Next (0, 3))
				{
					case 0:
						{
							DataContext d = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationSingle (d);

							Interlocked.Increment (ref nbMutatingSingleCalls);

							break;
						}
					case 1:
						{
							DataContext d1 = dataContexts.GetRandomElement ();
							DataContext d2 = dataContexts.GetRandomElement ();

							this.ExecuteRandomMutatingOperationDouble (d1, d2);

							Interlocked.Increment (ref nbMutatingDoubleCalls);

							break;
						}
					case 2:
						{
							DataContext d = dataContexts.GetRandomElement ();

							this.ExecuteRandomNonMutatingOperation (d);

							Interlocked.Increment (ref nbNonMutatingCalls);

							break;
						}
				}
			};

			this.BuildContextAndExecute (10, 5, isCache, initialization, action);

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("MIXED OPERATIONS TEST 3");
			Console.WriteLine ("NB CALLS NON MUTATING: " + nbNonMutatingCalls);
			Console.WriteLine ("NB CALLS MUTATING SINGLE: " + nbMutatingSingleCalls);
			Console.WriteLine ("NB CALLS MUTATING DOUBLE: " + nbMutatingDoubleCalls);
		}


		[TestMethod]
		public void MixedOperationsTest4()
		{
			long nbNonMutatingPrivateCalls = 0;
			long nbNonMutatingCommonCalls = 0;
			long nbMutatingPrivateCalls = 0;
			long nbMutatingCommonCalls = 0;
			long nbMutatingBothCalls = 0;

			Func<int, int, bool> isCache = (i, j) => i == this.nbThreads;

			Action<DataContext> initialization = (d) =>
			{
				d.GetByExample (new PersonTitleEntity ());
			};

			Action<int, List<DataContext>> action = (i, dataContexts) =>
			{
				int lbPrivate = i * 5;
				int ubPrivate = (i + 1) * 5;

				int lbCommon = this.nbThreads * 5;
				int ubCommon = (this.nbThreads + 1) * 5;

				switch (this.Dice.Next (0, 5))
				{
					case 0:
						{
							DataContext d = dataContexts[this.Dice.Next (lbPrivate, ubPrivate)];

							this.ExecuteRandomMutatingOperationSingle (d);

							Interlocked.Increment (ref nbMutatingPrivateCalls);

							break;
						}
					case 1:
						{
							DataContext d = dataContexts[this.Dice.Next (lbCommon, ubCommon)];

							this.ExecuteRandomMutatingOperationSingle (d);

							Interlocked.Increment (ref nbMutatingCommonCalls);

							break;
						}
					case 2:
						{
							DataContext d = dataContexts[this.Dice.Next (lbPrivate, ubPrivate)];

							this.ExecuteRandomNonMutatingOperation (d);

							Interlocked.Increment (ref nbNonMutatingPrivateCalls);

							break;
						}
					case 3:
						{
							DataContext d = dataContexts[this.Dice.Next (lbCommon, ubCommon)];

							this.ExecuteRandomNonMutatingOperation (d);

							Interlocked.Increment (ref nbNonMutatingCommonCalls);

							break;
						}
					case 4:
						{
							DataContext d1 = dataContexts[this.Dice.Next (lbCommon, ubCommon)];
							DataContext d2 = dataContexts[this.Dice.Next (lbPrivate, ubPrivate)];

							this.ExecuteRandomMutatingOperationDouble (d1, d2);

							Interlocked.Increment (ref nbMutatingBothCalls);

							break;
						}
				}
			};

			this.BuildContextAndExecute (this.nbThreads + 1, 5, isCache, initialization, action);

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("MIXED OPERATIONS TEST 4");
			Console.WriteLine ("NB CALLS NON MUTATING PRIVATE: " + nbNonMutatingPrivateCalls);
			Console.WriteLine ("NB CALLS NON MUTATING COMMON: " + nbNonMutatingCommonCalls);
			Console.WriteLine ("NB CALLS MUTATING PRIVATE: " + nbMutatingPrivateCalls);
			Console.WriteLine ("NB CALLS MUTATING COMMON: " + nbMutatingCommonCalls);
			Console.WriteLine ("NB CALLS MUTATING DOUBLE: " + nbMutatingBothCalls);
		}


		[TestMethod]
		public void AbstractEntityThreadSafetyTest()
		{
			long nbSimpleCalls = 0;
			long nbCollectionCalls = 0;

			Func<int, int, bool> isCache = (i, j) => true;

			Action<DataContext> initialization = (d) =>
			{
				d.GetByExample (new AbstractPersonEntity ());
				d.GetByExample (new AbstractContactEntity ());
				d.GetByExample (new PersonTitleEntity ());
			};

			Action<int, List<DataContext>> action = (i, dataContexts) =>
			{
				var entity = dataContexts.Single ().GetEntities ().GetRandomElement ();

				var fieldId = entity
					.GetStructuredTypeProvider ()
					.GetStructuredType ()
					.GetFieldIds ()
					.Shuffle ()
					.First ();

				var field = entity
					.GetStructuredTypeProvider ()
					.GetStructuredType ()
					.GetField (fieldId);

				switch (field.Relation)
				{
					case FieldRelation.None:
					case FieldRelation.Reference:

						var target = entity.GetField<object> (fieldId);

						Interlocked.Increment (ref nbSimpleCalls);

						break;

					case FieldRelation.Collection:

						var targets = entity.GetFieldCollection<AbstractEntity> (fieldId);

						this.ExecuteRandomOperationOnIList (targets);

						Interlocked.Increment (ref nbCollectionCalls);

						break;
				}
			};

			this.BuildContextAndExecute (1, 1, isCache, initialization, action);

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("ABSTRACT ENTITY THREAD SAFETY TEST");
			Console.WriteLine ("NB CALLS SIMPLE: " + nbSimpleCalls);
			Console.WriteLine ("NB CALLS COLLECTION: " + nbCollectionCalls);
		}


		private void BuildContextAndExecute(int nbDataInfrastructures, int nbDataContextsPerDataInfrastructure, System.Func<int, int, bool> isCache, System.Action<DataContext> initialization, System.Action<int, List<DataContext>> action)
		{
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			var dataInfrastructures = new List<DataInfrastructure> ();
			var dataContexts = new List<DataContext> ();

			try
			{
				for (int i = 0; i < nbDataInfrastructures; i++)
				{
					var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (entityEngine);

					dataInfrastructures.Add (dataInfrastructure);

					for (int j = 0; j < nbDataContextsPerDataInfrastructure; j++)
					{
						bool cache = isCache (i, j);

						var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, readOnly: cache);

						initialization (dataContext);

						dataContexts.Add (dataContext);
					}
				}

				List<Thread> threads = new List<Thread> ();

				DateTime startTime = DateTime.Now;

				for (int i = 0; i < this.nbThreads; i++)
				{
					int iCaptured = i;

					Thread thread = new Thread
					(
						() =>
						{
							while (DateTime.Now - startTime < this.duration)
							{
								action (iCaptured, dataContexts);
							}
						}
					);

					threads.Add (thread);
				}

				foreach (Thread thread in threads.Shuffle ())
				{
					thread.Start ();
				}

				foreach (Thread thread in threads.Shuffle ())
				{
					thread.Join ();
				}
			}
			finally
			{
				foreach (var dataContext in dataContexts)
				{
					dataContext.Dispose ();
				}

				foreach (var dataInfrastructure in dataInfrastructures)
				{
					dataInfrastructure.Dispose ();
				}
			}
		}


		private void ExecuteRandomNonMutatingOperation(DataContext dataContext)
		{
			switch (this.Dice.Next (0, 14))
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
					throw new NotImplementedException ();
			}
		}


		private void ExecuteRandomMutatingOperationSingle(DataContext dataContext1)
		{
			switch (this.Dice.Next (0, 14))
			{
				case 0:
					dataContext1.GetPersistedEntity ("db:" + Druid.Parse ("[J1AB1]") + ":" + new DbKey (new DbId (1000000000 + this.Dice.Next (1, 200))).Id);
					break;

				case 1:
					dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000 + this.Dice.Next (1, 100))));
					break;

				case 2:
					dataContext1.ResolveEntity (Druid.Parse ("[J1AA1]"), new DbKey (new DbId (1000000000 + this.Dice.Next (1, 600))));
					break;

				case 3:
					dataContext1.ResolveEntity (new EntityKey (Druid.Parse ("[J1A9]"), new DbKey (new DbId (1000000000 + this.Dice.Next (1, 75)))));
					break;

				case 4:
					dataContext1.Reload ();
					break;

				case 5:
					dataContext1.GetByExample
					(
						new NaturalPersonEntity ()
						{
							Lastname = "firstname" + this.Dice.Next (0, 100)
						}
					);
					break;

				case 6:
					dataContext1.GetByRequest<LegalPersonEntity>
					(
						new Request ()
						{
							RootEntity = new LegalPersonEntity ()
							{
								Name = "name" + this.Dice.Next (0, 100)
							}
						}
					);
					break;

				case 7:
					{
						var entity = dataContext1.GetEntities ().GetRandomElement ();

						Druid entityTypeId = entity.GetEntityStructuredTypeId ();
						var fields = dataContext1.DataInfrastructure.EntityEngine.EntityTypeEngine.GetReferenceFields (entityTypeId);

						if (fields.Any ())
						{
							var field = fields.GetRandomElement ();

							entity.GetField<AbstractEntity> (field.Id);
						}
						break;
					}

				case 8:
					{
						var entity = dataContext1.GetEntities ().GetRandomElement ();

						Druid entityTypeId = entity.GetEntityStructuredTypeId ();
						var fields = dataContext1.DataInfrastructure.EntityEngine.EntityTypeEngine.GetCollectionFields (entityTypeId);

						if (fields.Any ())
						{
							var field = fields.GetRandomElement ();

							var collection = entity.GetFieldCollection<AbstractEntity> (field.Id);

							this.ExecuteRandomOperationOnIList (collection);
						}
						break;
					}

				case 9:
					{
						var entity = dataContext1.GetEntities ().GetRandomElement ();

						Druid entityTypeId = entity.GetEntityStructuredTypeId ();
						var fields = dataContext1.DataInfrastructure.EntityEngine.EntityTypeEngine.GetCollectionFields (entityTypeId);

						if (fields.Any ())
						{
							var field = fields.GetRandomElement ();

							var collection = entity.InternalGetFieldCollection (field.Id);

							this.ExecuteRandomOperationOnIList (collection);
						}
						break;
					}

				case 10:
					{
						var entity = dataContext1.GetEntities ().GetRandomElement ();

						using (entity.DefineOriginalValues ())
						{
							Thread.Sleep (10);
						}
						break;
					}

				case 11:
					{
						var entity = dataContext1.GetEntities ().GetRandomElement ();

						using (entity.DisableEvents ())
						{
							Thread.Sleep (10);
						}
						break;
					}

				case 12:
					{
						var entity = dataContext1.GetEntities ().GetRandomElement ();

						using (entity.DisableReadOnlyChecks ())
						{
							Thread.Sleep (10);
						}
						break;
					}

				case 13:
					{
						var entity = dataContext1.GetEntities ().GetRandomElement ();

						using (entity.UseSilentUpdates ())
						{
							Thread.Sleep (10);
						}
						break;
					}

				default:
					throw new NotImplementedException ();
			}
		}


		private void ExecuteRandomMutatingOperationDouble(DataContext sender, DataContext receiver)
		{
			switch (this.Dice.Next (0, 2))
			{
				case 0:
					DataContext.CopyEntity (sender, sender.GetEntities ().GetRandomElement (), receiver);
					break;

				case 1:
					receiver.GetLocalEntity (sender.GetEntities ().GetRandomElement ());
					break;

				default:
					throw new NotImplementedException ();
			}
		}


		private void ExecuteRandomOperationOnIList(IList<AbstractEntity> collection)
		{
			switch (this.Dice.Next (0, 7))
			{
				case 0:
					Assert.IsNotNull (collection.Count);
					break;

				case 1:
					collection.Contains (null);
					break;

				case 2:
					collection.GetEnumerator ();
					break;

				case 3:
					collection.IndexOf (new NaturalPersonEntity ());
					break;

				case 4:
					Assert.IsNotNull (collection.IsReadOnly);
					break;

				case 5:
					collection.CopyTo (new AbstractEntity[1000], 0);
					break;

				case 6:
					if (collection.Any ())
					{
						collection.GetRandomElement ();
					}
					break;

				default:
					throw new NotImplementedException ();
			}
		}


		private void ExecuteRandomOperationOnIList(IList collection)
		{
			switch (this.Dice.Next (0, 10))
			{
				case 0:
					Assert.IsNotNull (collection.Count);
					break;

				case 1:
					collection.Contains (null);
					break;

				case 2:
					collection.GetEnumerator ();
					break;

				case 3:
					collection.IndexOf (new NaturalPersonEntity ());
					break;

				case 4:
					Assert.IsNotNull (collection.IsReadOnly);
					break;

				case 5:
					collection.CopyTo (new AbstractEntity[1000], 0);
					break;

				case 6:
					Assert.IsNotNull (collection.IsFixedSize);
					break;

				case 7:
					if (collection.Count > 0)
					{
						Assert.AreNotEqual (new object (), collection[0]);
					}
					break;

				case 8:
					Assert.IsNotNull (collection.IsSynchronized);
					break;

				case 9:
					Assert.AreNotEqual (new object (), collection.SyncRoot);
					break;

				default:
					throw new NotImplementedException ();
			}
		}


		private readonly int nbThreads = 100;


		private readonly TimeSpan duration = TimeSpan.FromSeconds (60);


		private Random Dice
		{
			get
			{
				if (this.dice == null)
				{
					this.dice = new Random (Thread.CurrentThread.ManagedThreadId);
				}

				return this.dice;
			}
		}


		[ThreadStatic]
		private Random dice;


	}


}