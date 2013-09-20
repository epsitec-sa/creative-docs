//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Core.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Epsitec.Cresus.Assets.Core.Tests
{
	[TestClass]
	public class InfiniteCollectionTest
	{
		[TestMethod]
		public void Check_GetAccess_ImplementsAsyncRetrieval()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());

			Assert.IsNull (collection[0]);
			Assert.IsNull (collection[1]);
			Assert.IsNull (collection[-2]);

			Thread.Sleep (150);

			Assert.AreEqual ("0", collection[0]);
			Assert.AreEqual ("1", collection[1]);
			Assert.AreEqual ("-2", collection[-2]);
		}

		[TestMethod]
		public void Check_ClearValue_Sync()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());
			var list = new List<string> ();

			collection.CollectionChanged += (o, e) => list.Add (e.ToString ());

			collection[0] = "A";

			Assert.AreEqual ("A", collection[0]);

			Assert.IsTrue (collection.ClearValue (0));
			Assert.IsFalse (collection.ClearValue (0));
			Assert.IsFalse (collection.ClearValue (1));

			Assert.AreEqual (2, list.Count);
			Assert.AreEqual ("Add at 0", list[0]);
			Assert.AreEqual ("Remove at 0", list[1]);

			Assert.IsNull (collection[0]);
		}

		[TestMethod]
		public void Check_SetAccess_OverridesAsyncRetrieval()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());
			var list = new List<string> ();

			collection.CollectionChanged += (o, e) => list.Add (e.ToString ());


			Assert.IsNull (collection[0]);
			Assert.IsNull (collection[2]);

			collection[0] = "A";
			collection[1] = "B";

			Assert.AreEqual ("A", collection[0]);
			Assert.AreEqual ("B", collection[1]);
			Assert.AreEqual (2, list.Count);
			Assert.AreEqual ("Add at 0", list[0]);
			Assert.AreEqual ("Add at 1", list[1]);

			Thread.Sleep (150);

			Assert.AreEqual ("A", collection[0]);
			Assert.AreEqual ("B", collection[1]);
			Assert.AreEqual ("2", collection[2]);
			Assert.AreEqual (3, list.Count);
			Assert.AreEqual ("Add at 0", list[0]);
			Assert.AreEqual ("Add at 1", list[1]);
			Assert.AreEqual ("Add at 2", list[2]);

			collection[2] = "C";

			Assert.AreEqual (4, list.Count);
			Assert.AreEqual ("Replace at 2", list[3]);
		}

		[TestMethod]
		public void Check_Clear_CancelsAsyncRetrieval()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());

			Assert.IsNull (collection[2]);

			Thread.Sleep (50);

			Assert.IsNull (collection[2]);

			collection.Clear ();

			//	The retrieval of collection[2] has been canceled. We won't get any result
			//	back.

			Thread.Sleep (100);

			Assert.IsNull (collection[2]);
		}

		[TestMethod]
		[ExpectedException (typeof (System.Exception), AllowDerivedTypes=false)]
		public void Check_Access_ImplementsAsyncException()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());

			Assert.IsNull (collection[100]);

			Thread.Sleep (50);

			Assert.IsNull (collection[100]);

			Thread.Sleep (100);

			//	The retrieval of collection[100] has now produced an error. If we
			//	try to retrieve the value, we will get an exception.

			var data = collection[100];

			Assert.Fail ("Access to collection should have thrown an exception");
		}

		[TestMethod]
		public void Check_SetAccess_OverridesAsyncException()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());

			Assert.IsNull (collection[100]);

			collection[100] = "X";

			Thread.Sleep (150);

			//	The async retrieval of collection[100] has now produced an error;
			//	the exception will be dropped silently.

			Assert.AreEqual ("X", collection[100]);
		}

		[TestMethod]
		public void Check_ClearValue_OverridesAsyncException()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());

			Assert.IsNull (collection[100]);

			collection.ClearValue (100);

			Thread.Sleep (150);

			//	The async retrieval of collection[100] has now produced an error;
			//	the exception will not be dropped, as the collection item was cleared.

			Assert.IsNull (collection[100]);
			
			collection.Clear ();

			Thread.Sleep (150);
		}

		[TestMethod]
		public void Check_Access_RaisesCollectionChangedEvents()
		{
			var collection = new InfiniteCollection<string> (new AsyncEnumerable ());
			var list = new List<string> ();

			collection.CollectionChanged += (o, e) => list.Add (e.ToString ());

			Assert.IsNull (collection[0]);
			Thread.Sleep (50);
			Assert.IsNull (collection[1]);

			Thread.Sleep (150);

			Assert.IsNull (collection[2]);

			collection.Clear ();

			Thread.Sleep (150);

			Assert.IsNull (collection[100]);

			Thread.Sleep (150);

			Assert.AreEqual (3, list.Count);
			Assert.AreEqual ("Add at 0", list[0]);
			Assert.AreEqual ("Add at 1", list[1]);
			Assert.AreEqual ("Reset", list[2]);
		}


		#region AsyncEnumerable Class

		private class AsyncEnumerable : IAsyncEnumerable<string>
		{
			#region IAsyncValueProvider<string> Members

			public IAsyncEnumerator<string> GetAsyncEnumerator(int index, int count, CancellationToken token)
			{
				return new AsyncEnumerator (index, count, token);
			}

			#endregion
		}

		#endregion

		#region AsyncEnumerator Class

		private class AsyncEnumerator : IAsyncEnumerator<string>
		{
			public AsyncEnumerator(int start, int count, CancellationToken token)
			{
				this.token = token;
				this.start = start;
				this.count = count;
				this.step  = count < 0 ? -1 : 1;
				this.Reset ();
			}

			#region IAsyncEnumerator<string> Members

			public string Current
			{
				get
				{
					return this.value;
				}
			}

			public async Task<bool> MoveNext()
			{
				if (this.more == 0)
				{
					return false;
				}

				this.token.ThrowIfCancellationRequested ();

				await TaskEx.Delay (100, this.token);

				if (this.index == 100)
				{
					throw new System.InvalidOperationException ("Invalid index");
				}

				this.value = this.index.ToString ();

				System.Diagnostics.Debug.WriteLine ("Produced " + this.value);
				
				this.index += this.step;
				this.more  -= this.step;

				return true;
			}

			public void Reset()
			{
				this.index = this.start;
				this.more  = this.count;
				this.value = null;
			}

			#endregion

			private readonly CancellationToken	token;
			private readonly int				start;
			private readonly int				count;
			private readonly int				step;
			
			private int							index;
			private int							more;
			private string						value;
		}

		#endregion
	}
}
