//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class HostedTest
	{
		[Test]
		public void CheckHostedIntList()
		{
			ListHost<int> hostInt = new ListHost<int> ();

			hostInt.SetExpectedInsertions (1, 2, 3, 4, 5, 6);
			hostInt.SetExpectedRemovals (2, 1, 5, 6);

			hostInt.Items.Add (1);
			hostInt.Items.Insert (0, 2);
			hostInt.Items.Add (3);
			hostInt.Items.RemoveAt (0);
			hostInt.Items.RemoveAt (0);
			hostInt.Items.AddRange (new int[] { 4, 5 });

			Assert.AreEqual (3, hostInt.Items[0]);
			Assert.AreEqual (4, hostInt.Items[1]);
			Assert.AreEqual (5, hostInt.Items[2]);

			hostInt.Items[2] = 6;
			hostInt.Items[2] = 6;

			Assert.AreEqual (2, hostInt.Items.IndexOf (6));

			hostInt.Items.Remove (6);

			Assert.AreEqual (2, hostInt.Items.Count);
		}

		[Test]
		public void CheckHostedStringList()
		{
			ListHost<string> hostString = new ListHost<string> ();

			hostString.SetExpectedInsertions ("1", "2", "3", "4", "5", "6");
			hostString.SetExpectedRemovals ("2", "1", "5", "6");

			hostString.Items.Add ("1");
			hostString.Items.Insert (0, "2");
			hostString.Items.Add ("3");
			hostString.Items.RemoveAt (0);
			hostString.Items.RemoveAt (0);
			hostString.Items.AddRange (new string[] { "4", "5" });

			Assert.AreEqual ("3", hostString.Items[0]);
			Assert.AreEqual ("4", hostString.Items[1]);
			Assert.AreEqual ("5", hostString.Items[2]);

			hostString.Items[2] = "6";
			hostString.Items[2] = "6";

			Assert.AreEqual (2, hostString.Items.IndexOf ("6"));

			hostString.Items.Remove ("6");

			Assert.AreEqual (2, hostString.Items.Count);
		}

		[Test]
		public void CheckHostedIntDict()
		{
			DictHost<string, int> hostInt = new DictHost<string, int> ();

			hostInt.SetExpectedInsertions (new KeyValuePair<string, int> ("A", 1), new KeyValuePair<string, int> ("B", 2), new KeyValuePair<string, int> ("C", 3), new KeyValuePair<string, int> ("D", 4), new KeyValuePair<string, int> ("E", 5), new KeyValuePair<string, int> ("E", 6));
			hostInt.SetExpectedRemovals (new KeyValuePair<string, int> ("B", 2), new KeyValuePair<string, int> ("A", 1), new KeyValuePair<string, int> ("E", 5), new KeyValuePair<string, int> ("E", 6));

			hostInt.Items.Add ("A", 1);
			hostInt.Items["B"] = 2;
			hostInt.Items.Add ("C", 3);
			hostInt.Items.Remove ("B");
			hostInt.Items.Remove (new KeyValuePair<string, int> ("A", 1));
			hostInt.Items["D"] = 4;
			hostInt.Items["E"] = 5;
			
			Assert.AreEqual (3, hostInt.Items["C"]);
			Assert.AreEqual (4, hostInt.Items["D"]);
			Assert.AreEqual (5, hostInt.Items["E"]);
			Assert.IsTrue (hostInt.Items.ContainsKey ("E"));
			Assert.IsTrue (hostInt.Items.ContainsValue (5));

			hostInt.Items["E"] = 6;
			hostInt.Items["E"] = 6;

			hostInt.Items.Remove ("E");

			Assert.AreEqual (2, hostInt.Items.Count);
		}

		private class ListHost<T> : IListHost<T>
		{
			public ListHost()
			{
				this.list = new Collections.HostedList<T> (this);
			}

			public Collections.HostedList<T> Items
			{
				get
				{
					return this.list;
				}
			}

			public void SetExpectedInsertions(params T[] items)
			{
				this.SetExpectedInsertions ((IEnumerable<T>) (items));
			}

			public void SetExpectedRemovals(params T[] items)
			{
				this.SetExpectedRemovals ((IEnumerable<T>) (items));
			}

			public void SetExpectedInsertions(IEnumerable<T> collection)
			{
				this.expectedInsertions.Clear ();

				foreach (T item in collection)
				{
					this.expectedInsertions.Enqueue (item);
				}
			}
			
			public void SetExpectedRemovals(IEnumerable<T> collection)
			{
				this.expectedRemovals.Clear ();

				foreach (T item in collection)
				{
					this.expectedRemovals.Enqueue (item);
				}
			}
			
			#region IListHost<T> Members

			public void NotifyListInsertion(T item)
			{
				T expect = this.expectedInsertions.Dequeue ();
				Assert.AreEqual (expect, item);
			}

			public void NotifyListRemoval(T item)
			{
				T expect = this.expectedRemovals.Dequeue ();
				Assert.AreEqual (expect, item);
			}

			#endregion

			private Collections.HostedList<T> list;
			private Queue<T> expectedInsertions = new Queue<T> ();
			private Queue<T> expectedRemovals = new Queue<T> ();
		}

		private class DictHost<K, V> : IDictionaryHost<K, V>
		{
			public DictHost()
			{
				this.dict = new Collections.HostedDictionary<K, V> (this);
			}

			public void SetExpectedInsertions(params KeyValuePair<K, V>[] pairs)
			{
				foreach (KeyValuePair<K, V> pair in pairs)
				{
					this.expectedInsertions.Enqueue (pair);
				}
			}

			public void SetExpectedRemovals(params KeyValuePair<K, V>[] pairs)
			{
				foreach (KeyValuePair<K, V> pair in pairs)
				{
					this.expectedRemovals.Enqueue (pair);
				}
			}

			#region IDictionaryHost<K,V> Members

			public Collections.HostedDictionary<K, V> Items
			{
				get
				{
					return this.dict;
				}
			}

			public void NotifyDictionaryInsertion(K key, V value)
			{
				KeyValuePair<K, V> expect = this.expectedInsertions.Dequeue ();
				Assert.AreEqual (expect.Key, key);
				Assert.AreEqual (expect.Value, value);
			}

			public void NotifyDictionaryRemoval(K key, V value)
			{
				KeyValuePair<K, V> expect = this.expectedRemovals.Dequeue ();
				Assert.AreEqual (expect.Key, key);
				Assert.AreEqual (expect.Value, value);
			}

			#endregion

			private Collections.HostedDictionary<K, V> dict;
			private Queue<KeyValuePair<K, V>> expectedInsertions = new Queue<KeyValuePair<K, V>> ();
			private Queue<KeyValuePair<K, V>> expectedRemovals = new Queue<KeyValuePair<K, V>> ();
		}
	}
}
