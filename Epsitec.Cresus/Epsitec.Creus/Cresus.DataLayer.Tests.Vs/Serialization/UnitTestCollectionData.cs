using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Serialization
{


	[TestClass]
	public sealed class UnitTestCollectionData
	{


		[TestMethod]
		public void CollectionDataConstructorTest()
		{
			CollectionData collectionData = new CollectionData ();
		}


		[TestMethod]
		public void ItemTest()
		{
			CollectionData collectionData = new CollectionData ();

			Dictionary<Druid, List<DbKey>> keys = new Dictionary<Druid, List<DbKey>> ()
			{
				{ Druid.FromLong(1), new List<DbKey> () { new DbKey(new DbId(1)), new DbKey(new DbId(2)), new DbKey(new DbId(3)),} },
				{ Druid.FromLong(2), new List<DbKey> () { new DbKey(new DbId(4)), new DbKey(new DbId(5)), new DbKey(new DbId(6)),} },
				{ Druid.FromLong(3), new List<DbKey> () { new DbKey(new DbId(7)), new DbKey(new DbId(8)), new DbKey(new DbId(9)),} },
			};

			foreach (Druid druid in keys.Keys)
			{
				foreach (DbKey key in keys[druid])
				{
					collectionData[druid].Add (key);
				}
			}

			foreach (Druid druid in keys.Keys)
			{
				List<DbKey> keys1 = keys[druid];
				List<DbKey> keys2 = collectionData[druid];

				CollectionAssert.AreEquivalent (keys1, keys2);
			}

			CollectionAssert.AreEquivalent (new List<DbKey> (), collectionData[Druid.FromLong (9999)]);
		}


	}


}
