using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


    [TestClass]
    public sealed class UnitTestUidGenerator
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
		public void CreateUidGeneratorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				UidManager manager = new UidManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				
				string name = "test";
				List<UidSlot> slots = new List<UidSlot> ()
                {
                    new UidSlot (0, 9),
                    new UidSlot (20, 29),
                    new UidSlot (10, 19),
                };
				
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new UidGenerator (null, name, slots)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new UidGenerator (manager, null, slots)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new UidGenerator (manager, "", slots)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new UidGenerator (manager, name, null)
				);

				List<UidSlot> badSlots = new List<UidSlot> ();

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new UidGenerator (manager, name, badSlots)
				);

				badSlots = new List<UidSlot> ()
                {
                    new UidSlot (0, 5),
                    new UidSlot (5, 10),
                };

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new UidGenerator (manager, name, badSlots)
				);

				badSlots = new List<UidSlot> ()
                {
                    new UidSlot (3, 5),
                    new UidSlot (1, 2),
                };

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new UidGenerator (manager, name, badSlots)
				);
			}
		}


		[TestMethod]
		public void ConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				UidManager manager = new UidManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				string name = "myCounter";

				List<UidSlot> slots = new List<UidSlot> ()
                {
                    new UidSlot (0, 9),
                    new UidSlot (15, 15),
                    new UidSlot (19, 34),
                };

				UidGenerator generator = new UidGenerator (manager, name, slots);

				Assert.AreEqual (name, generator.Name);
				CollectionAssert.AreEqual (slots, generator.Slots);				
			}
		}

		[TestMethod]
		public void GetNextUidTest1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				UidManager manager = new UidManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				string name = "myCounter";

				List<UidSlot> slots = new List<UidSlot> ()
                {
                    new UidSlot (0, 9),
                };

				manager.CreateUidGenerator (name, slots);

				UidGenerator generator = manager.GetUidGenerator (name);

				for (int i = 0; i <= 9; i++)
				{
					Assert.AreEqual (i, generator.GetNextUid ());
				}

				ExceptionAssert.Throw<System.InvalidOperationException> (() => generator.GetNextUid ());
			}
		}


		[TestMethod]
		public void GetNextUidTest2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				UidManager manager = new UidManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				string name = "myCounter";

				List<UidSlot> slots = new List<UidSlot> ()
                {
                    new UidSlot (0, 9),
                    new UidSlot (15, 15),
                    new UidSlot (19, 34),
                };

				manager.CreateUidGenerator (name, slots);

				UidGenerator generator = manager.GetUidGenerator (name);

				for (int i = 0; i <= 9; i++)
				{
					Assert.AreEqual (i, generator.GetNextUid ());
				}

				for (int i = 15; i <= 15; i++)
				{
					Assert.AreEqual (i, generator.GetNextUid ());
				}

				for (int i = 19; i <= 34; i++)
				{
					Assert.AreEqual (i, generator.GetNextUid ());
				}

				ExceptionAssert.Throw<System.InvalidOperationException> (() => generator.GetNextUid ());
			}
		}


	}


}
