//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.CaptionTest.Stuff))]

namespace Epsitec.Common.Types
{
	[TestFixture] public class CaptionTest
	{
		[Test]
		public void CheckBindingCaptionExtraction()
		{
			Stuff target = new Stuff ();
			Stuff source = new Stuff ();

			Binding bindingName = new Binding (BindingMode.TwoWay, source, "Name");
			Binding bindingText = new Binding (BindingMode.TwoWay, source, "Text");
			Binding bindingEnum = new Binding (BindingMode.TwoWay, source, "Enum");

			target.SetBinding (Stuff.NameProperty, bindingName);
			target.SetBinding (Stuff.TextProperty, bindingText);
			target.SetBinding (Stuff.EnumProperty, bindingEnum);

			Assert.AreEqual ("Name", target.GetBindingExpression (Stuff.NameProperty).GetSourceName ());
			Assert.AreEqual (-1, target.GetBindingExpression (Stuff.NameProperty).GetSourceCaptionId ());
			Assert.AreEqual ("String", target.GetBindingExpression (Stuff.NameProperty).GetSourceNamedType ().Name);
			Assert.AreEqual ("StringType", target.GetBindingExpression (Stuff.NameProperty).GetSourceNamedType ().GetType ().Name);

			Assert.AreEqual ("Text", target.GetBindingExpression (Stuff.TextProperty).GetSourceName ());
			Assert.AreEqual (0x0000400000000004L, target.GetBindingExpression (Stuff.TextProperty).GetSourceCaptionId ());
			Assert.AreEqual ("String", target.GetBindingExpression (Stuff.TextProperty).GetSourceNamedType ().Name);

			Assert.AreEqual ("Enum", target.GetBindingExpression (Stuff.EnumProperty).GetSourceName ());
			Assert.AreEqual (-1, target.GetBindingExpression (Stuff.EnumProperty).GetSourceCaptionId ());
			Assert.IsNotNull (target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ());
			Assert.AreEqual ("EnumType", target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ().GetType ().Name);
			Assert.AreEqual ("Enumeration MyEnum", target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ().Name);
			Assert.AreEqual (0x0000400000000005L, target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ().CaptionId);
		}
		
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

		[Test]
		public void CheckSerializationWithAttachedProperties()
		{
			Caption caption = new Caption ();

			caption.Labels.Add ("M");
			caption.Labels.Add ("Mystery");
			caption.SetValue (Stuff.NameProperty, "MysteryName");
			caption.SetValue (Stuff.EnumProperty, MyEnum.First);

			string xml = caption.SerializeToString ();

			System.Console.Out.WriteLine ("Caption as XML: {0}", xml);

			caption = new Caption ();
			caption.DeserializeFromString (xml);

			Assert.AreEqual ("M", Collection.Extract (caption.SortedLabels, 0));
			Assert.AreEqual ("Mystery", Collection.Extract (caption.SortedLabels, 1));
			Assert.AreEqual ("MysteryName", caption.GetValue (Stuff.NameProperty));
			Assert.AreEqual (MyEnum.First, caption.GetValue (Stuff.EnumProperty));
		}

		internal class Stuff : DependencyObject
		{
			static Stuff()
			{
				DependencyPropertyMetadata metadataText = Stuff.TextProperty.GetMetadata (typeof (Stuff));
				DependencyPropertyMetadata metadataEnum = Stuff.EnumProperty.GetMetadata (typeof (Stuff));

				EnumType enumType = new EnumType (typeof (MyEnum));

				enumType.DefineCaptionId (0x0000400000000005L);
				enumType[MyEnum.None].DefineCaptionId (0x0000400000000006L);
				enumType[MyEnum.First].DefineCaptionId (0x0000400000000007L);
				enumType[MyEnum.Second].DefineCaptionId (0x0000400000000008L);
				enumType[MyEnum.Third].DefineCaptionId (0x0000400000000009L);

				metadataText.DefineCaptionId (0x0000400000000004L);
				metadataEnum.DefineNamedType (enumType);
			}

			public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached ("Name", typeof (string), typeof (Stuff));

			public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached ("Text", typeof (string), typeof (Stuff), new DependencyPropertyMetadata ());
			public static readonly DependencyProperty EnumProperty = DependencyProperty.RegisterAttached ("Enum", typeof (MyEnum), typeof (Stuff), new DependencyPropertyMetadata (MyEnum.None));
		}
		
		private enum MyEnum
		{
			None,
			First,
			Second,
			Third
		}
	}
}
