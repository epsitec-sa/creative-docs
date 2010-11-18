using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests.Serialization
{


	[TestClass]
	public sealed class UnitTestValueData
	{
		

		[TestMethod]
		public void ValueDataConstructorTest()
		{
			ValueData valueData = new ValueData ();
		}


		[TestMethod]
		public void ItemTest()
		{
			ValueData valueData = new ValueData ();

			Dictionary<Druid, object> values = new Dictionary<Druid, object> ()
			{
				{ Druid.FromLong(1), 1},
				{ Druid.FromLong(2), 2.0},
				{ Druid.FromLong(3), "trois"},
				{ Druid.FromLong(4), System.Tuple.Create (1, 2, 3, 4) },
				{ Druid.FromLong(5), null },
			};

			foreach (Druid druid in values.Keys)
			{
				valueData[druid] = values[druid];
			}

			foreach (Druid druid in values.Keys)
			{
				object value1 = values[druid];
				object value2 = valueData[druid];

				Assert.AreEqual (value1, value2);
			}

			for (int i = 6; i < 10; i++)
			{
				object value = valueData[Druid.FromLong (i)];

				Assert.IsNull (value);
			}
		}


		[TestMethod]
		public void ContainsTest()
		{
			ValueData valueData = new ValueData ();

			Dictionary<Druid, object> values = new Dictionary<Druid, object> ()
			{
				{ Druid.FromLong(1), 1},
				{ Druid.FromLong(2), 2.0},
				{ Druid.FromLong(3), "trois"},
				{ Druid.FromLong(4), System.Tuple.Create (1, 2, 3, 4) },
				{ Druid.FromLong(5), null },
			};

			foreach (Druid druid in values.Keys)
			{
				valueData[druid] = values[druid];
			}

			for (int i = 1; i < 6; i++)
			{
				Assert.IsTrue (valueData.ContainsValue (Druid.FromLong (i)));
			}

			for (int i = 6; i < 10; i++)
			{
				Assert.IsFalse (valueData.ContainsValue (Druid.FromLong (i)));
			}
		}


	}


}
