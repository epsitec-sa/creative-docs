//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class CaptionCacheTest
	{
		[SetUp]
		public void SetUp()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			
			this.manager_en = new ResourceManager (@"S:\Epsitec.Cresus\Common.Support.Tests");
			this.manager_en.DefineDefaultModuleName ("Test");
			this.manager_en.ActivePrefix = "file";
			this.manager_en.ActiveCulture = Epsitec.Common.Support.Resources.FindSpecificCultureInfo ("en");

			this.manager_fr = new ResourceManager (@"S:\Epsitec.Cresus\Common.Support.Tests");
			this.manager_fr.DefineDefaultModuleName ("Test");
			this.manager_fr.ActivePrefix = "file";
			this.manager_fr.ActiveCulture = Epsitec.Common.Support.Resources.FindSpecificCultureInfo ("fr");
		}

		[Test]
		public void CheckEmptyCaptions()
		{
			Assert.IsNull (CaptionCache.Instance.GetCaption (this.manager_en, -1));
			Assert.IsNull (CaptionCache.Instance.GetCaption (this.manager_en, Druid.Empty));

			Assert.IsNull (CaptionCache.Instance.GetCaption (this.manager_en, "$02"));
			Assert.IsNull (CaptionCache.Instance.GetCaption (this.manager_en, new Druid (0, 2)));
		}

		[Test]
		public void CheckFillCache()
		{
			Druid idA = Druid.Parse ("[4002]");
			Druid idQ = Druid.Parse ("[4003]");

			Caption captionA_en;
			Caption captionQ_en;
			Caption captionA_fr;
			Caption captionQ_fr;

			captionA_en = CaptionCache.Instance.GetCaption (this.manager_en, idA);
			captionQ_en = CaptionCache.Instance.GetCaption (this.manager_en, idQ);
			
			Assert.AreEqual ("Pattern angle expressed in degrees.", captionA_en.Description);
			Assert.AreEqual ("Quality coefficient.", captionQ_en.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA_en.SortedLabels, 0));
			Assert.AreEqual ("Pattern angle", Collection.Extract (captionA_en.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ_en.SortedLabels, 0));

			captionA_fr = CaptionCache.Instance.GetCaption (this.manager_fr, idA);
			captionQ_fr = CaptionCache.Instance.GetCaption (this.manager_fr, idQ);

			Assert.AreEqual ("Angle de rotation de la trame, exprimé en degrés.", captionA_fr.Description);
			Assert.AreEqual ("Coefficient de Qualité.", captionQ_fr.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA_fr.SortedLabels, 0));
			Assert.AreEqual ("Angle de la trame", Collection.Extract (captionA_fr.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ_fr.SortedLabels, 0));

			Assert.AreEqual (captionA_en, CaptionCache.Instance.GetCaption (this.manager_en, idA));
			Assert.AreEqual (captionA_en, CaptionCache.Instance.GetCaption (this.manager_en, idA.ToLong ()));
			Assert.AreEqual (captionQ_en, CaptionCache.Instance.GetCaption (this.manager_en, idQ));
			Assert.AreEqual (captionQ_en, CaptionCache.Instance.GetCaption (this.manager_en, idQ.ToLong ()));

			Assert.AreEqual (captionA_fr, CaptionCache.Instance.GetCaption (this.manager_fr, idA));
			Assert.AreEqual (captionA_fr, CaptionCache.Instance.GetCaption (this.manager_fr, idA.ToLong ()));
			Assert.AreEqual (captionA_fr, CaptionCache.Instance.GetCaption (this.manager_fr, "[4002]"));
			Assert.AreEqual (captionQ_fr, CaptionCache.Instance.GetCaption (this.manager_fr, idQ));
			Assert.AreEqual (captionQ_fr, CaptionCache.Instance.GetCaption (this.manager_fr, idQ.ToLong ()));
			Assert.AreEqual (captionQ_fr, CaptionCache.Instance.GetCaption (this.manager_fr, "[4003]"));

			Assert.AreEqual (4, CaptionCache.Instance.DebugGetLiveCaptionsCount ());

			System.GC.Collect ();
			CaptionCache.Instance.TrimCache ();

			Assert.AreEqual (4, CaptionCache.Instance.DebugGetLiveCaptionsCount ());

			Assert.AreEqual ("A", Collection.Extract (captionA_en.SortedLabels, 0));
			Assert.AreEqual ("Q", Collection.Extract (captionQ_en.SortedLabels, 0));
			
			captionA_en = null;
			captionQ_en = null;

			System.GC.Collect ();
			CaptionCache.Instance.TrimCache ();

			Assert.AreEqual (2, CaptionCache.Instance.DebugGetLiveCaptionsCount ());
		}

		[Test]
		public void CheckProperties()
		{
			Assert.AreEqual (MyItem.TextProperty, MyItemX.TextProperty);
			
			Caption caption;
			
			caption = CaptionCache.Instance.GetPropertyCaption (this.manager_en, MyItem.TextProperty);

			Assert.IsNotNull (caption);
			Assert.AreEqual ("Text Property", caption.Description);
			Assert.AreEqual ("[4004]", caption.Id.ToResourceId ());

			caption = CaptionCache.Instance.GetTypeCaption (this.manager_en, MyItem.EnumProperty);

			Assert.IsNotNull (caption);
			Assert.AreEqual ("One of Three", caption.Description);

			INamedType type1 = MyItem.EnumProperty.DefaultMetadata.NamedType;
			INamedType type2 = MyItem.EnumProperty.GetMetadata (typeof (MyItem)).NamedType;

			Assert.AreEqual ("One of Three", CaptionCache.Instance.GetTypeCaption (this.manager_en, MyItem.EnumProperty).Description);
			Assert.AreEqual ("One of Three", CaptionCache.Instance.GetTypeCaption (this.manager_en, type2).Description);
			Assert.AreEqual ("One of Three", CaptionCache.Instance.GetTypeCaption (this.manager_en, type1).Description);

			caption = CaptionCache.Instance.GetPropertyCaption (this.manager_en, MyItemX.TextProperty);

			Assert.AreEqual ("Text Property", caption.Description);
			Assert.AreEqual ("[4004]", caption.Id.ToResourceId ());

			caption = CaptionCache.Instance.GetPropertyCaption (this.manager_en, typeof (MyItemX), MyItemX.TextProperty);

			Assert.AreEqual ("Borrowed Text", caption.Description);
			Assert.AreEqual ("[400A]", caption.Id.ToResourceId ());
			
			INamedType type3 = MyItem.EnumProperty.GetMetadata (typeof (MyItemX)).NamedType;

			Assert.AreEqual (type1, type2);
			Assert.AreNotEqual (type1, type3);
		}


		private class MyItem : DependencyObject
		{
			public MyItem()
			{
			}

			static MyItem()
			{
				DependencyPropertyMetadata metadataText = MyItem.TextProperty.GetMetadata (typeof (MyItem));
				DependencyPropertyMetadata metadataEnum = MyItem.EnumProperty.GetMetadata (typeof (MyItem));

				EnumType enumType = new EnumType (typeof (MyEnum));

				enumType.DefineCaptionId (new Druid ("[4005]"));
				enumType[MyEnum.None].DefineCaptionId (new Druid ("[4006]"));
				enumType[MyEnum.First].DefineCaptionId (new Druid ("[4007]"));
				enumType[MyEnum.Second].DefineCaptionId (new Druid ("[4008]"));
				enumType[MyEnum.Third].DefineCaptionId (new Druid ("[4009]"));

				metadataText.DefineCaptionId (new Druid ("[4004]"));
				metadataEnum.DefineNamedType (enumType);
			}


			public static readonly DependencyProperty TextProperty = DependencyProperty.Register ("Text", typeof (string), typeof (MyItem), new DependencyPropertyMetadata ());
			public static readonly DependencyProperty EnumProperty = DependencyProperty.Register ("Enum", typeof (MyEnum), typeof (MyItem), new DependencyPropertyMetadata (MyEnum.None));
		}

		private class MyItemX : DependencyObject
		{
			public MyItemX()
			{
			}

			static MyItemX()
			{
				DependencyPropertyMetadata metadataText = new DependencyPropertyMetadata ();
				DependencyPropertyMetadata metadataEnum = new DependencyPropertyMetadata (MyEnum.First);

				EnumType enumType = new EnumType (typeof (MyEnum));

				enumType.DefineCaptionId (new Druid ("[4005]"));
				enumType[MyEnum.First].DefineCaptionId (new Druid ("[4007]"));
				enumType[MyEnum.Second].DefineCaptionId (new Druid ("[4008]"));
				enumType[MyEnum.Third].DefineCaptionId (new Druid ("[4009]"));

				metadataText.DefineCaptionId (new Druid ("[400A]"));
				metadataEnum.DefineNamedType (enumType);

				MyItemX.TextProperty.OverrideMetadata (typeof (MyItemX), metadataText);
				MyItemX.EnumProperty.OverrideMetadata (typeof (MyItemX), metadataEnum);
			}


			public static readonly DependencyProperty TextProperty = MyItem.TextProperty.AddOwner (typeof (MyItemX));
			public static readonly DependencyProperty EnumProperty = MyItem.EnumProperty.AddOwner (typeof (MyItemX));
		}

		private enum MyEnum
		{
			None,
			First,
			Second,
			Third
		}
		
		ResourceManager manager_en;
		ResourceManager manager_fr;
	}
}
