//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture] public class CaptionTest
	{
		[Test]
		public void CheckCaptionLabels()
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
		public void CheckSerialization()
		{
			Caption caption = new Caption ();

			caption.Labels.Add ("A");
			caption.Labels.Add ("Angle");
			caption.Labels.Add ("Angle de la trame");
			caption.Description = "Angle de rotation de la trame, exprimé en degrés.";

			string xml = caption.ToPartialXml ();
			
			System.Console.Out.WriteLine ("Caption as XML: {0}", xml);

			caption = Caption.CreateFromPartialXml (xml);

			Assert.AreEqual ("A", Collection.Extract (caption.SortedLabels, 0));
			Assert.AreEqual ("Angle", Collection.Extract (caption.SortedLabels, 1));
			Assert.AreEqual ("Angle de rotation de la trame, exprimé en degrés.", caption.Description);
		}
	}
}
