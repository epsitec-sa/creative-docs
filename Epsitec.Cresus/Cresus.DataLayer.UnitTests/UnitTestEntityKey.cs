using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer
{
    
    
	[TestClass]
	public class UnitTestEntityKey
	{


		private TestContext testContextInstance;

		
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		
		[TestMethod]
		public void EntityKeyConstructorTest1()
		{
			foreach (var data in this.GetTestData ())
			{
				EntityKey target = this.Create (data);

				Assert.AreEqual (data.Item1, target.RowKey);
				Assert.AreEqual (data.Item2, target.EntityId);
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void EntityKeyConstructorTest2()
		{
			DbKey rowKey = DbKey.Empty;
			Druid entityId = Druid.FromLong (1);

			EntityKey entityKey = new EntityKey (rowKey, entityId);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void EntityKeyConstructorTest3()
		{
			DbKey rowKey = new DbKey (new DbId (1));
			Druid entityId = Druid.Empty;

			EntityKey entityKey = new EntityKey (rowKey, entityId);
		}

		
		[TestMethod ]
		public void EqualsTest()
		{
			foreach (var data1 in this.GetTestData ())
			{
				foreach (var data2 in this.GetTestData ())
				{
					bool areEqual = data1.Item1 == data2.Item1 && data1.Item2 == data2.Item2;

					EntityKey target1 = this.Create (data1);
					EntityKey target2 = this.Create (data2);

					Assert.AreEqual (areEqual, target1.Equals (target2));
					Assert.AreEqual (areEqual, target2.Equals (target1));
				}
			}

		}

	   
		[TestMethod]
		public void GetHashCodeTest()
		{
			foreach (var data in this.GetTestData ())
			{
				EntityKey target1 = this.Create (data);
				EntityKey target2 = this.Create (data);

				Assert.IsTrue (target1.GetHashCode () == target2.GetHashCode ());
			}
		}

		
		[TestMethod]
		public void op_EqualityTest()
		{
			foreach (var data1 in this.GetTestData ())
			{
				foreach (var data2 in this.GetTestData ())
				{
					bool areEqual = data1.Item1 == data2.Item1 && data1.Item2 == data2.Item2;

					EntityKey target1 = this.Create (data1);
					EntityKey target2 = this.Create (data2);

					Assert.AreEqual (areEqual, target1 == target2);
					Assert.AreEqual (areEqual, target2 == target1);
				}
			}
		}

		
		[TestMethod]
		public void op_InequalityTest()
		{
			foreach (var data1 in this.GetTestData ())
			{
				foreach (var data2 in this.GetTestData ())
				{
					bool areEqual = data1.Item1 == data2.Item1 && data1.Item2 == data2.Item2;

					EntityKey target1 = this.Create (data1);
					EntityKey target2 = this.Create (data2);

					Assert.AreNotEqual (areEqual, target1 != target2);
					Assert.AreNotEqual (areEqual, target2 != target1);
				}
			}
		}

		
		[TestMethod]
		public void EntityIdTest()
		{
			foreach (var data in this.GetTestData ())
			{
				EntityKey target = this.Create (data);

				Assert.AreEqual (data.Item2, target.EntityId);
			}
		}

		
		[TestMethod]
		public void IsEmptyTest()
		{
			foreach (var data in this.GetTestData ())
			{
				EntityKey target = this.Create (data);

				bool isEmpty = data.Item1.IsEmpty && data.Item2.IsEmpty;

				Assert.AreEqual (isEmpty, target.IsEmpty);
			}

			Assert.AreEqual (EntityKey.Empty, new EntityKey ());
			Assert.IsTrue (EntityKey.Empty.IsEmpty);
		}


		[TestMethod]
		public void RowKeyTest()
		{
			foreach (var data in this.GetTestData ())
			{
				EntityKey target = this.Create (data);

				Assert.AreEqual (data.Item1, target.RowKey);
			}
		}


		private IEnumerable<System.Tuple<DbKey, Druid>> GetTestData()
		{
			int count = 25;

			for (int i = 1; i <= count; i++)
			{
				for (int j = 1; j <= count; j++)
				{
					DbKey dbKey = new DbKey (new DbId (i));
					Druid druid = Druid.FromLong (j);

					yield return System.Tuple.Create (dbKey, druid);
				}
			}

			for (int i = 0; i < count * count; i++)
			{
				int number1 = 0;
				int number2 = 0;
				
				while (number1 == 0)
                {
                	number1 = this.dice.Next ();
                }

				while (number2 == 0)
                {
                	number2 = this.dice.Next ();
                }
				
				DbKey dbKey = new DbKey (new DbId (number1));
				Druid druid = Druid.FromLong (number2);

				yield return System.Tuple.Create (dbKey, druid);
			}
		}


		private EntityKey Create(System.Tuple<DbKey, Druid> data)
		{
			DbKey rowKey = data.Item1;
			Druid entityId = data.Item2;

			return new EntityKey (rowKey, entityId);
		}


		private System.Random dice = new System.Random ();


	}


}
