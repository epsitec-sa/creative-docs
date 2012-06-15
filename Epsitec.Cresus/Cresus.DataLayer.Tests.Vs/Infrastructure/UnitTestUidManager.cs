using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


    [TestClass]
    public class UnitTestUidManager
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
		public void ConstructorArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new UidManager (null, entityEngine.ServiceSchemaEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new UidManager (dbinfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void CreateUidGeneratorArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				string name = "test";
				List<UidSlot> slots = new List<UidSlot> ()
                {
                    new UidSlot (0, 9),
                    new UidSlot (20, 29),
                    new UidSlot (10, 19),
                };

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidGenerator (null, slots)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidGenerator ("", slots)
				);


				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => manager.CreateUidGenerator (name, null)
				);

				List<UidSlot> badSlots = new List<UidSlot> ();

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidGenerator (name, badSlots)
				);

				badSlots = new List<UidSlot> ()
                {
                    new UidSlot (0, 5),
                    new UidSlot (5, 10),
                };

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidGenerator (name, badSlots)
				);

				badSlots = new List<UidSlot> ()
                {
                    new UidSlot (3, 5),
                    new UidSlot (1, 2),
                };

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidGenerator (name, badSlots)
				);
			}
		}


		[TestMethod]
		public void DeleteUidGeneratorArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DeleteUidGenerator (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DeleteUidGenerator ("")
				);
			}
		}


		[TestMethod]
		public void DoesUisGeneratorExistsArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DoesUidGeneratorExist (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DoesUidGeneratorExist ("")
				);
			}
		}


		[TestMethod]
		public void GetUidGeneratorArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidGenerator (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidGenerator ("")
				);
			}
		}


		[TestMethod]
		public void GetNextUidArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetNextUid (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetNextUid ("")
				);
			}
		}


		[TestMethod]
		public void InvalidOperationTest()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.DeleteUidGenerator ("generator")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetUidGenerator ("generator")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetNextUid ("generator")
				);

				List<UidSlot> slots = new List<UidSlot> ()
                {
                    new UidSlot (0, 9),
                };

				manager.CreateUidGenerator ("generator", slots);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CreateUidGenerator ("generator", slots)
				);
			}
		}


		[TestMethod]
		public void CreateExistsDeleteUidGenerator()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				for (int i = 0; i < 10; i++)
				{
					List<UidSlot> slots = new List<UidSlot> ()
					{
						new UidSlot (0, 9),
						new UidSlot (10, 99),
					};

					manager.CreateUidGenerator ("myCounter" + i, slots);
					Assert.IsTrue (manager.DoesUidGeneratorExist ("myCounter" + i));
				}

				for (int i = 0; i < 10; i++)
				{
					Assert.IsTrue (manager.DoesUidGeneratorExist ("myCounter" + i));

					manager.DeleteUidGenerator ("myCounter" + i);
					Assert.IsFalse (manager.DoesUidGeneratorExist ("myCounter" + i));
				}
			}
		}


		[TestMethod]
		public void GetUidGenerator()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				for (int i = 0; i < 3; i++)
				{
					List<UidSlot> slots = new List<UidSlot> ();

					for (int j = 0; j < i + 1; j++)
					{
						slots.Add (new UidSlot (10 * (j + 1), 10 * (j + 2) - 1));
					}

					manager.CreateUidGenerator ("myCounter" + i, slots);
				}

				for (int i = 0; i < 3; i++)
				{
					UidGenerator generator = manager.GetUidGenerator ("myCounter" + i);

					Assert.AreEqual ("myCounter" + i, generator.Name);

					Assert.AreEqual (i + 1, generator.Slots.Count);

					for (int j = 0; j < i + 1; j++)
					{
						Assert.AreEqual (10 * (j + 1), generator.Slots[j].MinValue);
						Assert.AreEqual (10 * (j + 2) - 1, generator.Slots[j].MaxValue);
					}
				}
			}
		}


		[TestMethod]
		public void GetUidCounterNextValue()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				UidManager manager = new UidManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				for (int i = 0; i < 10; i++)
				{
					List<UidSlot> slots = new List<UidSlot> ()
					{
						new UidSlot (0, 9),
						new UidSlot (90, 99),
					};
					
					manager.CreateUidGenerator ("myCounter" + i, slots);
				}

				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j <= 9; j++)
					{
						Assert.AreEqual (j, manager.GetNextUid ("myCounter" + i));
					}

					for (int j = 90; j <= 99; j++)
					{
						Assert.AreEqual (j, manager.GetNextUid ("myCounter" + i));
					}

					Assert.AreEqual (null, manager.GetNextUid ("myCounter" + i));
				}
			}
		}


		[TestMethod]
		public void Concurrency()
		{
			int nbThreads = 100;

			var entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			var dbInfrastructures = Enumerable.Range (0, nbThreads)
				.Select (i => DbInfrastructureHelper.ConnectToTestDatabase ())
				.ToList ();

			List<UidGenerator> generators = new List<UidGenerator> ();
			Dictionary<UidGenerator, int> usedGenerators = new Dictionary<UidGenerator, int> ();

			try
			{
				System.DateTime time = System.DateTime.Now;

				var threads = dbInfrastructures.Select (d => new System.Threading.Thread (() =>
				{
					var dice = new System.Random (System.Threading.Thread.CurrentThread.ManagedThreadId);

					var uidManager = new UidManager (d, entityEngine.ServiceSchemaEngine);

					while (System.DateTime.Now - time <= System.TimeSpan.FromSeconds (15))
					{
						int count;

						lock (generators)
						{
							count = generators.Count;
						}

						if (count < nbThreads || dice.NextDouble () > 0.9)
						{
							var slots = Enumerable.Range (0, 5)
								.Select (i => new UidSlot (i * 3, (i + 1) * 3 - 1))
								.ToList ();

							var generator = uidManager.CreateUidGenerator (System.Guid.NewGuid ().ToString (), slots);

							lock (generators)
							{
								generators.Add (generator);
								usedGenerators[generator] = 0;
							}
						}

						UidGenerator generator1 = null;

						lock (generators)
						{
							if (generators.Count > 0)
							{
								generator1 = generators[dice.Next (0, generators.Count)];
							}
						}

						if (generator1 != null)
						{
							uidManager.DoesUidGeneratorExist (generator1.Name);
						}

						UidGenerator generator2 = null;

						lock (generators)
						{
							if (generators.Count > 0)
							{
								generator2 = generators[dice.Next (0, generators.Count)];
								usedGenerators[generator2] = usedGenerators[generator2] + 1;
							}
						}

						if (generator2 != null)
						{
							uidManager.GetUidGenerator (generator2.Name);

							lock (generators)
							{
								usedGenerators[generator2] = usedGenerators[generator2] - 1;
							}
						}

						UidGenerator generator3 = null;

						lock (generators)
						{
							if (generators.Count > 0)
							{
								generator3 = generators[dice.Next (0, generators.Count)];
								usedGenerators[generator3] = usedGenerators[generator3] + 1;
							}
						}

						if (generator3 != null)
						{
							uidManager.GetNextUid (generator3.Name);


							lock (generators)
							{
								usedGenerators[generator3] = usedGenerators[generator3] - 1;
							}
						}

						lock (generators)
						{
							count = generators.Count;
						}

						if (count > nbThreads && dice.NextDouble () > 0.95)
						{
							UidGenerator generator = null;

							lock (generators)
							{
								if (generators.Count > 0)
								{
									var possibleGenerators = generators
										.Where (g => usedGenerators[g] == 0)
										.ToList ();

									int index = dice.Next (0, possibleGenerators.Count);

									generator = possibleGenerators[index];

									generators.Remove (generator);
									usedGenerators.Remove (generator);
								}
							}

							if (generator != null)
							{
								uidManager.DeleteUidGenerator (generator.Name);
							}
						}
					}
				})).ToList ();

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}
			finally
			{
				foreach (var dbInfrastructure in dbInfrastructures)
				{
					dbInfrastructure.Dispose ();
				}
			}
		}


	}

}
