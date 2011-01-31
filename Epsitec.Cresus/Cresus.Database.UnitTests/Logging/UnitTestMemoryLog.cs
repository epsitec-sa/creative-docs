using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database.UnitTests.Logging
{


	[TestClass]
	public sealed class UnitTestMemoryLog
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new MemoryLog (0)
			);
		}


		[TestMethod]
		public void AddEntryGetEntryGetNbEntriesTest()
		{
			int size = 50;

			List<Query> list = new List<Query>();
			MemoryLog log = new MemoryLog (size);

			for (int i = 0; i < 3 * size; i++)
			{
				Query query = this.GetSampleQuery ();

				log.AddEntry (query);

				if (list.Count >= size)
				{
					list.RemoveAt (0);
				}
				list.Add (query);

				Assert.AreEqual (list.Count, log.GetNbEntries ());

				for (int j = 0; j < list.Count; j++)
				{
					Assert.AreEqual (list[j], log.GetEntry (j));
				}
			}
		}


		[TestMethod]
		public void ClearTest()
		{
			int size = 10;

			MemoryLog log = new MemoryLog (size);

			for (int i = 0; i < size; i++)
			{
				while (log.GetNbEntries () < i)
				{
					log.AddEntry (this.GetSampleQuery ());
				}

				Assert.AreEqual (i, log.GetNbEntries ());

				log.Clear ();

				Assert.AreEqual (0, log.GetNbEntries ());
			}
		}


		private Query GetSampleQuery()
		{
			string sourceCode = "";
			List<Parameter> parameters = new List<Parameter> ();
			Result result = null;
			System.DateTime startTime = System.DateTime.Now;
			System.TimeSpan duration = System.TimeSpan.FromTicks (0);

			return new Query (sourceCode, parameters, result, startTime, duration);
		}


	}


}
