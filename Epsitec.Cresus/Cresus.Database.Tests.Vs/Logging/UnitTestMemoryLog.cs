using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Tests.Vs.Logging
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
		public void GetEntryArgumentCheck()
		{
			MemoryLog log = new MemoryLog (10);
			MemoryLog_Accessor logAccessor = new MemoryLog_Accessor (new PrivateObject (log));

			while (log.GetNbEntries() < 5)
            {
				Query query = this.GetSampleQuery ();

				logAccessor.AddEntry (query);
            }

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => log.GetEntry (-1)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => log.GetEntry (6)
			);
		}


		[TestMethod]
		public void AddEntryGetEntryGetNbEntriesTest()
		{
			int size = 50;

			List<Query> list = new List<Query>();
			MemoryLog log = new MemoryLog (size);
			MemoryLog_Accessor logAccessor = new MemoryLog_Accessor (new PrivateObject (log));

			for (int i = 0; i < 3 * size; i++)
			{
				Query query = this.GetSampleQuery ();

				logAccessor.AddEntry (query);

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
		public void GetEntriesArgumentCheck()
		{
			int size = 10;
			
			List<Query> list = new List<Query> ();
			MemoryLog log = new MemoryLog (size);
			MemoryLog_Accessor logAccessor = new MemoryLog_Accessor (new PrivateObject (log));

			for (int i = 0; i < 3 * size; i++)
			{
				Query query = this.GetSampleQuery ();

				if (list.Count >= size)
				{
					list.RemoveAt (0);
				}
				list.Add (query);
				logAccessor.AddEntry (query);

				for (int j = 0; j < log.GetNbEntries (); j++)
				{
					for (int k = 0; k < log.GetNbEntries () - j; k++)
					{
						List<Query> queries1 = list.Skip (j).Take (k).ToList ();
						List<Query> queries2 = log.GetEntries (j, k).ToList ();

						CollectionAssert.AreEqual (queries1, queries2);
					}
				}
			}
		}


		[TestMethod]
		public void GetEntriesTest()
		{
			MemoryLog log = new MemoryLog (10);
			MemoryLog_Accessor logAccessor = new MemoryLog_Accessor (new PrivateObject (log));

			while (log.GetNbEntries () < 5)
			{
				Query query = this.GetSampleQuery ();

				logAccessor.AddEntry (query);
			}
		}
		

		[TestMethod]
		public void GetNextNumberTest()
		{
			int size = 10;

			MemoryLog log = new MemoryLog (size);
			MemoryLog_Accessor logAccessor = new MemoryLog_Accessor (new PrivateObject (log));
			
			for (int i = 0; i < 50; i++)
			{
				Assert.AreEqual (i, logAccessor.GetNextNumber ());
			}

			log.Clear ();

			for (int i = 0; i < 50; i++)
			{
				Assert.AreEqual (i, logAccessor.GetNextNumber ());
			}
		}


		[TestMethod]
		public void ClearTest()
		{
			int size = 10;

			MemoryLog log = new MemoryLog (size);
			MemoryLog_Accessor logAccessor = new MemoryLog_Accessor (new PrivateObject(log));
			for (int i = 0; i < size; i++)
			{
				while (log.GetNbEntries () < i)
				{
					logAccessor.AddEntry (this.GetSampleQuery ());
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

			return new Query (1, startTime, duration, sourceCode, parameters, result);
		}


	}


}
