using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class HostedTest
	{
		[Test]
		public void CheckHostedIntList()
		{
			Host<int> hostInt = new Host<int> ();

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

			Assert.AreEqual (2, hostInt.Items.IndexOf (6));

			hostInt.Items.Remove (6);

			Assert.AreEqual (2, hostInt.Items.Count);
		}

		[Test]
		public void CheckHostedStringList()
		{
			Host<string> hostString = new Host<string> ();

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

			Assert.AreEqual (2, hostString.Items.IndexOf ("6"));

			hostString.Items.Remove ("6");

			Assert.AreEqual (2, hostString.Items.Count);
		}

		private class Host<T> : IListHost<T>
		{
			public Host()
			{
				this.list = new HostedList<T> (this);
			}

			public HostedList<T> Items
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

			private HostedList<T> list;
			private Queue<T> expectedInsertions = new Queue<T> ();
			private Queue<T> expectedRemovals = new Queue<T> ();
		}
	}
}
