using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Tests.Types
{
	[TestFixture] public class ComparerTest
	{
		[Test]
		public void CheckCompareBoxed()
		{
			object a = 1;
			object b = 1;
			
			Assert.IsTrue (a != b);
			Assert.IsTrue (a.Equals (b));
			
			a = "hello";
			b = "hello";
			
			Assert.IsTrue (a == b);
			Assert.IsTrue (a.Equals (b));
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("hell");
			buffer.Append ("o");
			
			b = buffer.ToString ();
			
			Assert.IsFalse (a == b);
			Assert.IsTrue ((string)a == (string)b);
			Assert.IsTrue (a.Equals (b));
		}
		
		[Test]
		public void CheckArrayComparers()
		{
			int size = 1000*1000;
			int loops = 10;
			
			int[] x1 = new int[size];
			int[] x2 = new int[size];

			for (int i = 0; i < size; i++)
			{
				x1[i] = i;
				x2[i] = i;
			}

			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();
			
			stopwatch.Start ();
			Comparer.Equal (x1, x2);
			Comparer.EqualValues (x1, x2);
			stopwatch.Stop ();

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < loops; i++)
			{
				Comparer.Equal (x1, x2);
			}

			stopwatch.Stop ();
			System.Console.Out.WriteLine ("Native int == comparison: {0}ms, {1} loops x {2} elements", stopwatch.ElapsedMilliseconds, loops, size);

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < loops; i++)
			{
				Comparer.EqualValues (x1, x2);
			}

			stopwatch.Stop ();
			System.Console.Out.WriteLine ("IEquatable<T> comparison: {0}ms, {1} loops x {2} elements", stopwatch.ElapsedMilliseconds, loops, size);
		}

		[Test]
		public void CheckEqual()
		{
			object a = 1;
			object b = 1;
			
			Assert.IsTrue (Comparer.Equal (a, b));
			
			a = "hello";
			b = "hello";
			
			Assert.IsTrue (Comparer.Equal (a, b));
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("hell");
			buffer.Append ("o");
			
			b = buffer.ToString ();
			
			Assert.IsTrue (Comparer.Equal (a, b));
			
			a = new string[] { "a", "b" };
			b = new string[] { "a", "b" };
			
			Assert.IsFalse (System.Object.Equals (a, b));
			Assert.IsTrue (Comparer.Equal (a, b));
			
			a = new string[2,2] { { "1", "2" }, { "3", "4" } };
			b = new string[2,2] { { "1", "2" }, { "3", "4" } };
			
			Assert.IsFalse (System.Object.Equals (a, b));
			Assert.IsTrue (Comparer.Equal (a, b));
			
			a = new string[2,2] { { "1", "2" }, { "3", "4" } };
			b = new string[2,3] { { "1", "2", "X" }, { "3", "4", "Y" } };
			
			Assert.IsFalse (Comparer.Equal (a, b));

			System.Collections.ArrayList la = new System.Collections.ArrayList ();
			System.Collections.ArrayList lb = new System.Collections.ArrayList ();

			la.Add ("a");
			la.Add ("b");
			lb.Add ("a");

			Assert.IsFalse (Comparer.Equal (la, lb));
			Assert.IsTrue (Comparer.Equal (la, la));

			lb.Add ("b");

			Assert.IsTrue (Comparer.Equal (la, lb));
			
			a = new string[] { "a", "b" };
			b = new string[] { "a", "b" };

			la.Add (a);
			lb.Add (b);
			
			Assert.IsTrue (Comparer.Equal (la, lb));
		}
	}
}
