using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public class UnitTestReferenceData
	{


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ReferenceDataConstructorTest()
		{
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ItemTest1()
		{
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();

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
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ItemTest2()
		{
			ReferenceData_Accessor referenceData = new ReferenceData_Accessor ();

			referenceData[Druid.FromLong (1)] = null;
		}


	}


}
