using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Serialization
{


	[TestClass]
	public sealed class UnitTestReferenceData
	{


		[TestMethod]
		public void ReferenceDataConstructorTest()
		{
			ReferenceData referenceData = new ReferenceData ();
		}


		[TestMethod]
		public void ItemTest()
		{
			ReferenceData referenceData = new ReferenceData ();

			Dictionary<Druid, DbKey> keys = new Dictionary<Druid, DbKey> ()
			{
				{ Druid.FromLong(1), new DbKey (new DbId (1))},
				{ Druid.FromLong(2), new DbKey (new DbId (2))},
				{ Druid.FromLong(3), new DbKey (new DbId (3))},
				{ Druid.FromLong(4), new DbKey (new DbId (4))},
				{ Druid.FromLong(5), new DbKey (new DbId (5))},
			};

			foreach (Druid druid in keys.Keys)
			{
				referenceData[druid] = keys[druid];
			}

			foreach (Druid druid in keys.Keys)
			{
				DbKey key1  = keys[druid];
				DbKey? key2 = referenceData[druid];

				Assert.IsTrue (key2.HasValue);
				Assert.AreEqual (key1, key2.Value);
			}

			for (int i = 6; i < 10; i++)
			{
				DbKey? key = referenceData[Druid.FromLong (i)];

				Assert.IsFalse (key.HasValue);
			}
		}


		[TestMethod]
		public void ItemArgumentCheck()
		{
			ReferenceData referenceData = new ReferenceData ();

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => referenceData[Druid.FromLong (1)] = null
			);
		}


	}


}
