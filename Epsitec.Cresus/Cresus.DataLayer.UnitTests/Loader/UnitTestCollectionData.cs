using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public class UnitTestCollectionData
	{


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CollectionDataConstructorTest()
		{
			CollectionData_Accessor target = new CollectionData_Accessor ();
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ItemTest()
		{
			CollectionData_Accessor target = new CollectionData_Accessor ();

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
					target[druid].Add (key);
				}
			}

			foreach (Druid druid in keys.Keys)
			{
				List<DbKey> keys1 = keys[druid];
				List<DbKey> keys2 = target[druid];

				CollectionAssert.AreEquivalent (keys1, keys2);
			}

			CollectionAssert.AreEquivalent (new List<DbKey> (), target[Druid.FromLong (9999)]);
		}


	}


}
