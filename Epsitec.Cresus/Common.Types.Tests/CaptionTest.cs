//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture] public class CaptionTest
	{
		[Test]
		public void CheckLabels()
		{
			Caption caption = new Caption ();
			
			caption.Labels.Add ("abc123");
			caption.Labels.Add ("xyz");
			
			Assert.AreEqual (2, caption.Labels.Count);
			Assert.AreEqual (2, Collection.Count (caption.SortedLabels));
			Assert.AreEqual ("xyz", Collection.Extract (caption.SortedLabels, 0));
			Assert.AreEqual ("abc123", Collection.Extract (caption.SortedLabels, 1));
			
			caption.Labels.Add ("*");
			
			Assert.AreEqual (3, caption.Labels.Count);
			Assert.AreEqual (3, Collection.Count (caption.SortedLabels));
			Assert.AreEqual ("*", Collection.Extract (caption.SortedLabels, 0));
			Assert.AreEqual ("xyz", Collection.Extract (caption.SortedLabels, 1));
			Assert.AreEqual ("abc123", Collection.Extract (caption.SortedLabels, 2));

			caption.Labels.Clear ();
			
			Assert.AreEqual (0, caption.Labels.Count);
			Assert.AreEqual (0, Collection.Count (caption.SortedLabels));
		}

		[Test]
		public void CheckMerge()
		{
			Caption a = new Caption ();
			Caption b = new Caption ();

			Caption c;

			c = Caption.Merge (a, b);

			Assert.AreEqual (0, c.Labels.Count);
			Assert.IsFalse (c.ContainsLocalValue (Caption.DecriptionProperty));

			a.Description = "xyz";
			b.Labels.Add ("1");
			b.Labels.Add ("2");

			c = Caption.Merge (a, b);
			
			Assert.AreEqual (2, c.Labels.Count);
			Assert.AreEqual ("1", Collection.Extract (c.Labels, 0));
			Assert.AreEqual ("2", Collection.Extract (c.Labels, 1));
			Assert.AreEqual (a.Description, c.Description);

			b.Description = "abc";

			a.Labels.Add ("x");
			c = Caption.Merge (a, b);

			Assert.AreEqual (2, c.Labels.Count);
			Assert.AreEqual ("1", Collection.Extract (c.Labels, 0));
			Assert.AreEqual ("2", Collection.Extract (c.Labels, 1));
			Assert.AreEqual (b.Description, c.Description);

			b.Labels.Clear ();
			
			c = Caption.Merge (a, b);

			Assert.AreEqual (1, c.Labels.Count);
			Assert.AreEqual ("x", Collection.Extract (c.Labels, 0));
			Assert.AreEqual (b.Description, c.Description);
		}

		[Test]
		public void CheckSerialization()
		{
			Caption caption = new Caption ();

			caption.Labels.Add ("A");
			caption.Labels.Add ("Angle");
			caption.Labels.Add ("Angle de la trame");
			caption.Description = "Angle de rotation de la trame, exprimé en degrés.";

			string xml = caption.SerializeToString ();
			
			System.Console.Out.WriteLine ("Caption as XML: {0}", xml);

			caption = new Caption ();
			caption.DeserializeFromString (xml);
			
			Assert.AreEqual ("A", Collection.Extract (caption.SortedLabels, 0));
			Assert.AreEqual ("Angle", Collection.Extract (caption.SortedLabels, 1));
			Assert.AreEqual ("Angle de rotation de la trame, exprimé en degrés.", caption.Description);
		}
	}
}
