//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Tests.Types.CaptionTest.Stuff))]

namespace Epsitec.Common.Tests.Types
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
			Assert.AreEqual (Druid.Empty, target.GetBindingExpression (Stuff.NameProperty).GetSourceCaptionId ());
			Assert.AreEqual ("Default.String", target.GetBindingExpression (Stuff.NameProperty).GetSourceNamedType ().Name);
			Assert.AreEqual ("StringType", target.GetBindingExpression (Stuff.NameProperty).GetSourceNamedType ().GetType ().Name);

			Assert.AreEqual ("Text", target.GetBindingExpression (Stuff.TextProperty).GetSourceName ());
			Assert.AreEqual (Druid.FromLong (0x0000400000000004L), target.GetBindingExpression (Stuff.TextProperty).GetSourceCaptionId ());
			Assert.AreEqual ("Default.String", target.GetBindingExpression (Stuff.TextProperty).GetSourceNamedType ().Name);

			Assert.AreEqual ("Enum", target.GetBindingExpression (Stuff.EnumProperty).GetSourceName ());
			Assert.AreEqual (Druid.Empty, target.GetBindingExpression (Stuff.EnumProperty).GetSourceCaptionId ());
			Assert.IsNotNull (target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ());
			Assert.AreEqual ("EnumType", target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ().GetType ().Name);
			Assert.AreEqual ("OneOfThree", target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ().Name);
			Assert.AreEqual (Druid.FromLong (0x0000400000000005L), target.GetBindingExpression (Stuff.EnumProperty).GetSourceNamedType ().CaptionId);
		}

		[Test]
		public void CheckChanged()
		{
			Caption caption1 = new Caption ();
			Caption caption2 = new Caption ();

			int count = 0;

			caption2.Changed += delegate(object sender) { count++; };

			caption1.Name = "Abc";
			caption1.Labels.Add ("Short");
			caption1.Labels.Add ("Long label");
			caption1.Description = "Description for Abc";

			caption2.Name = "Xyz";

			Assert.AreEqual (1, count);

			caption2.Labels.Add ("1");
			caption2.Labels.Add ("2");

			Assert.AreEqual (3, count);

			Caption.CopyDefinedProperties (caption1, caption2);
			
			Assert.AreEqual (4, count);
		}

		[Test]
		public void CheckEnumCreation1()
		{
			Caption caption = new Caption ();

			caption.DefineId (Druid.Parse ("[4005]"));
			caption.Name = "TestEnum";
			caption.Description = "Enumeration used for tests only";

			EnumType type = new EnumType (typeof (NotAnEnum), caption);

			type.MakeEditable ();

			Caption captionV1 = new Caption ();
			Caption captionV2 = new Caption ();
			Caption captionV3 = new Caption ();

			captionV1.DefineId (Druid.Parse ("[4007]"));
			captionV1.Name = "Value1";
			captionV1.Description = "First value";

			captionV2.DefineId (Druid.Parse ("[4008]"));
			captionV2.Name = "Value2";
			captionV2.Description = "Middle value";

			captionV3.DefineId (Druid.Parse ("[4009]"));
			captionV3.Name = "Value3";
			captionV3.Description = "Last value";

			type.EnumValues.Add (new EnumValue (1, captionV1));
			type.EnumValues.Add (new EnumValue (2, captionV2));
			type.EnumValues.Add (new EnumValue (3, captionV3));

			System.Console.Out.WriteLine ("XML: {0}", caption.SerializeToString ());
		}

		[Test]
		public void CheckEnumCreation2()
		{
			Caption caption = new Caption ();

			caption.DefineId (Druid.Parse ("[4005]"));
			caption.Name = "TestEnum";
			caption.Description = "Enumeration used for tests only";

			EnumType type = new EnumType (typeof (NotAnEnum), caption);

			string xml = type.Caption.SerializeToString ();

			System.Console.Out.WriteLine ("XML empty: {0}", xml);
			
			caption = new Caption ();
			caption.DeserializeFromString (xml);

			type = TypeRosetta.CreateTypeObject (caption) as EnumType;

			type.MakeEditable ();

			Caption captionV1 = new Caption ();
			Caption captionV2 = new Caption ();
			Caption captionV3 = new Caption ();

			captionV1.DefineId (Druid.Parse ("[4007]"));
			captionV1.Name = "Value1";
			captionV1.Description = "First value";

			captionV2.DefineId (Druid.Parse ("[4008]"));
			captionV2.Name = "Value2";
			captionV2.Description = "Middle value";

			captionV3.DefineId (Druid.Parse ("[4009]"));
			captionV3.Name = "Value3";
			captionV3.Description = "Last value";

			type.EnumValues.Add (new EnumValue (1, captionV1));
			type.EnumValues.Add (new EnumValue (2, captionV2));
			type.EnumValues.Add (new EnumValue (3, captionV3));

			xml = type.Caption.SerializeToString ();

			System.Console.Out.WriteLine ("XML filled: {0}", xml);

			caption = new Caption ();
			caption.DeserializeFromString (xml);

			type = TypeRosetta.CreateTypeObject (caption) as EnumType;

			Assert.AreEqual (3, Collection.Count (type.Values));
		}

		[Test]
		public void CheckEnumWithCaptions()
		{
			EnumType enumType = CaptionTest.GetMyEnumEnumType ();

			Assert.AreEqual ("[4005]", enumType.CaptionId.ToString ());

			System.Console.Out.WriteLine ("Type name: {0}", enumType.Name);
			System.Console.Out.WriteLine ("Type description: {0}", enumType.Caption.Description);

			foreach (EnumValue value in enumType.Values)
			{
				System.Console.Out.WriteLine ("- {0}, rank={1}, {2}", value.Name, value.Rank, value.Caption.Description);
			}

			string serial = enumType.Caption.SerializeToString ();

			System.Console.Out.WriteLine ("XML: {0}", serial);

			Caption caption = new Caption ();
			caption.DeserializeFromString (serial);

			EnumType eT = TypeRosetta.CreateTypeObject (caption) as EnumType;

			System.Console.Out.WriteLine ("Type name: {0}", eT.Name);
			System.Console.Out.WriteLine ("Type description: {0}", eT.Caption.Description);

			foreach (EnumValue value in eT.Values)
			{
				System.Console.Out.WriteLine ("- {0}, rank={1}, {2}", value.Name, value.Rank, value.Caption.Description);
			}
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
			Assert.IsFalse (c.ContainsLocalValue (Caption.DescriptionProperty));

			a.Description = "xyz";
			a.Name = "A";
			b.Labels.Add ("1");
			b.Labels.Add ("2");

			c = Caption.Merge (a, b);

			Assert.AreEqual ("A", c.Name);
			Assert.AreEqual (2, c.Labels.Count);
			Assert.AreEqual ("1", Collection.Extract (c.Labels, 0));
			Assert.AreEqual ("2", Collection.Extract (c.Labels, 1));
			Assert.AreEqual (a.Description, c.Description);

			b.Name = "B";
			b.Description = "abc";

			a.Labels.Add ("x");
			c = Caption.Merge (a, b);

			Assert.AreEqual ("B", c.Name);
			Assert.AreEqual (2, c.Labels.Count);
			Assert.AreEqual ("1", Collection.Extract (c.Labels, 0));
			Assert.AreEqual ("2", Collection.Extract (c.Labels, 1));
			Assert.AreEqual (b.Description, c.Description);

			b.Name = null;
			b.Labels.Clear ();
			
			c = Caption.Merge (a, b);

			Assert.AreEqual ("A", c.Name);
			Assert.AreEqual (1, c.Labels.Count);
			Assert.AreEqual ("x", Collection.Extract (c.Labels, 0));
			Assert.AreEqual (b.Description, c.Description);
		}

		[Test]
		public void CheckSerialization()
		{
			Caption caption = new Caption ();

			caption.Name = "FrameAngle";
			caption.Labels.Add ("A");
			caption.Labels.Add ("Angle");
			caption.Labels.Add ("Angle de la trame");
			caption.Description = "Angle de rotation de la trame, exprimé en degrés.";

			string xml = caption.SerializeToString ();

			System.Console.Out.WriteLine ("Caption as XML: {0}", xml);

			caption = new Caption ();
			caption.DeserializeFromString (xml);

			Assert.AreEqual ("FrameAngle", caption.Name);
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

				metadataText.DefineCaptionId (Druid.FromLong (0x0000400000000004L));
				metadataEnum.DefineNamedType (CaptionTest.GetMyEnumEnumType ());
			}

			public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached ("Name", typeof (string), typeof (Stuff));

			public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached ("Text", typeof (string), typeof (Stuff), new DependencyPropertyMetadata ());
			public static readonly DependencyProperty EnumProperty = DependencyProperty.RegisterAttached ("Enum", typeof (MyEnum), typeof (Stuff), new DependencyPropertyMetadata (MyEnum.None));
		}

		private static EnumType GetMyEnumEnumType()
		{
			Caption caption = Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.FromLong (0x0000400000000005L));
			
			EnumType enumType = new EnumType (typeof (MyEnum), caption);

			Assert.AreEqual (4, enumType.EnumValues.Count);

			if (enumType.EnumValues[0].CaptionId.IsEmpty)
			{
				enumType[MyEnum.None].DefineCaption (Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.FromLong (0x0000400000000006L)));
				enumType[MyEnum.First].DefineCaption (Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.FromLong (0x0000400000000007L)));
				enumType[MyEnum.Second].DefineCaption (Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.FromLong (0x0000400000000008L)));
				enumType[MyEnum.Third].DefineCaption (Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.FromLong (0x0000400000000009L)));
			}

			return enumType;
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
