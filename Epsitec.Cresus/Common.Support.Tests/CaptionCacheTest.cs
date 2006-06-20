//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class CaptionCacheTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void CheckFillCache()
		{
			ResourceManager manager_en;
			ResourceManager manager_fr;

			manager_en = new ResourceManager (@"S:\Epsitec.Cresus\Common.Support.Tests");
			manager_en.DefineDefaultModuleName ("Test");
			manager_en.ActivePrefix = "file";
			manager_en.ActiveCulture = Resources.FindSpecificCultureInfo ("en");

			manager_fr = new ResourceManager (@"S:\Epsitec.Cresus\Common.Support.Tests");
			manager_fr.DefineDefaultModuleName ("Test");
			manager_fr.ActivePrefix = "file";
			manager_fr.ActiveCulture = Resources.FindSpecificCultureInfo ("fr");

			Druid idA = Druid.Parse ("[4002]");
			Druid idQ = Druid.Parse ("[4003]");

			List<Caption> scrap = new List<Caption> ();

			Caption captionA_en;
			Caption captionQ_en;
			Caption captionA_fr;
			Caption captionQ_fr;

			captionA_en = CaptionCache.Instance.GetCaption (manager_en, idA);
			captionQ_en = CaptionCache.Instance.GetCaption (manager_en, idQ);
			
			Assert.AreEqual ("Pattern angle expressed in degrees.", captionA_en.Description);
			Assert.AreEqual ("Quality coefficient.", captionQ_en.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA_en.SortedLabels, 0));
			Assert.AreEqual ("Pattern angle", Collection.Extract (captionA_en.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ_en.SortedLabels, 0));

			captionA_fr = CaptionCache.Instance.GetCaption (manager_fr, idA);
			captionQ_fr = CaptionCache.Instance.GetCaption (manager_fr, idQ);

			Assert.AreEqual ("Angle de rotation de la trame, exprimé en degrés.", captionA_fr.Description);
			Assert.AreEqual ("Coefficient de Qualité.", captionQ_fr.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA_fr.SortedLabels, 0));
			Assert.AreEqual ("Angle de la trame", Collection.Extract (captionA_fr.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ_fr.SortedLabels, 0));

			Assert.AreEqual (captionA_en, CaptionCache.Instance.GetCaption (manager_en, idA));
			Assert.AreEqual (captionA_en, CaptionCache.Instance.GetCaption (manager_en, idA.ToLong ()));
			Assert.AreEqual (captionQ_en, CaptionCache.Instance.GetCaption (manager_en, idQ));
			Assert.AreEqual (captionQ_en, CaptionCache.Instance.GetCaption (manager_en, idQ.ToLong ()));

			Assert.AreEqual (captionA_fr, CaptionCache.Instance.GetCaption (manager_fr, idA));
			Assert.AreEqual (captionA_fr, CaptionCache.Instance.GetCaption (manager_fr, idA.ToLong ()));
			Assert.AreEqual (captionQ_fr, CaptionCache.Instance.GetCaption (manager_fr, idQ));
			Assert.AreEqual (captionQ_fr, CaptionCache.Instance.GetCaption (manager_fr, idQ.ToLong ()));

			Assert.AreEqual (4, CaptionCache.Instance.DebugCountLiveCaptions ());

			System.GC.Collect ();
			CaptionCache.Instance.TrimCache ();

			Assert.AreEqual (4, CaptionCache.Instance.DebugCountLiveCaptions ());

			Assert.AreEqual ("A", Collection.Extract (captionA_en.SortedLabels, 0));
			Assert.AreEqual ("Q", Collection.Extract (captionQ_en.SortedLabels, 0));
			
			captionA_en = null;
			captionQ_en = null;

			System.GC.Collect ();
			CaptionCache.Instance.TrimCache ();

			Assert.AreEqual (2, CaptionCache.Instance.DebugCountLiveCaptions ());
		}
	}
}
