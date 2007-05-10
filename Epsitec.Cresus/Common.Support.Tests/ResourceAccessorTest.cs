using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourceAccessorTest
	{
		[SetUp]
		public void Initialize()
		{
			this.manager = new ResourceManager (typeof (ResourceAccessorTest));
			this.manager.DefineDefaultModuleName ("Test");
		}

		[Test]
		public void CheckStringAccessor()
		{
			ResourceAccessors.StringResourceAccessor accessor = new ResourceAccessors.StringResourceAccessor ();
			
			Assert.IsFalse (accessor.ContainsChanges);

			accessor.Load (this.manager);

			Assert.AreEqual (8, accessor.Collection.Count);
			
			Assert.AreEqual (Druid.Parse ("[4002]"), accessor.Collection[Druid.Parse ("[4002]")].Id);
			Assert.AreEqual ("Text A", accessor.Collection[Druid.Parse ("[4002]")].GetCultureData ("00").GetValue (Res.Fields.ResourceString.Text));

			Assert.AreEqual (Druid.Parse ("[4006]"), accessor.Collection[Druid.Parse ("[4006]")].Id);
			Assert.AreEqual ("Text1", accessor.Collection[Druid.Parse ("[4006]")].Name);
			Assert.AreEqual ("Hello, world", accessor.Collection["Text1"].GetCultureData ("00").GetValue (Res.Fields.ResourceString.Text));

			Types.StructuredData data1 = accessor.Collection["Text1"].GetCultureData ("fr");
			Types.StructuredData data2 = accessor.Collection["Text1"].GetCultureData ("fr");

			Assert.AreSame (data1, data2);
			Assert.AreEqual ("Bonjour", data1.GetValue (Res.Fields.ResourceString.Text));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["Text1"].GetCultureData ("de");
			data2 = accessor.Collection["Text1"].GetCultureData ("de");

			Assert.IsNotNull (data1);
			Assert.AreSame (data1, data2);
			Assert.AreEqual (Types.UndefinedValue.Instance, data1.GetValue (Res.Fields.ResourceString.Text));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["Text1"].GetCultureData ("fr");
			data1.SetValue (Res.Fields.ResourceString.Text, "Bonjour tout le monde");
			data2.SetValue (Res.Fields.ResourceString.Text, "Hallo, Welt");

			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("Bonjour tout le monde", this.manager.GetText (Druid.Parse ("[4006]"), ResourceLevel.Localized, Resources.FindCultureInfo ("fr")));
			Assert.AreEqual ("Hallo, Welt", this.manager.GetText (Druid.Parse ("[4006]"), ResourceLevel.Localized, Resources.FindCultureInfo ("de")));

			CultureMap map = accessor.CreateItem ();

			Assert.IsNotNull (map);
			Assert.AreEqual (Druid.Parse ("[4008]"), map.Id);
			Assert.IsNull (accessor.Collection[map.Id]);

			accessor.Collection.Add (map);
			Assert.IsTrue (accessor.ContainsChanges);
			
			map.Name = "NewItem";
			map.GetCultureData ("00").SetValue (Res.Fields.ResourceString.Text, "New value");
			map.GetCultureData ("fr").SetValue (Res.Fields.ResourceString.Text, "Nouvelle valeur");
			
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("New value", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Default));
			Assert.AreEqual ("Nouvelle valeur", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")));

			map.GetCultureData ("fr").SetValue (Res.Fields.ResourceString.Text, Types.UndefinedValue.Instance);
			
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("New value", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Default));
			Assert.AreEqual ("New value", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")));
			
			accessor.Collection.Remove (map);
			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.IsNull (this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Default));
			Assert.IsNull (this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Localized, Resources.FindCultureInfo ("fr")));
		}


		[Test]
		public void CheckCaptionAccessor()
		{
			ResourceAccessors.CaptionResourceAccessor accessor = new ResourceAccessors.CaptionResourceAccessor ();

			Assert.IsFalse (accessor.ContainsChanges);

			accessor.Load (this.manager);

			Assert.AreEqual (2, accessor.Collection.Count);

			Assert.AreEqual (Druid.Parse ("[4002]"), accessor.Collection[Druid.Parse ("[4002]")].Id);
			Assert.AreEqual ("PatternAngle", accessor.Collection[Druid.Parse ("[4002]")].Name);
			Assert.AreEqual ("Pattern angle expressed in degrees.", accessor.Collection[Druid.Parse ("[4002]")].GetCultureData ("00").GetValue (Res.Fields.ResourceCaption.Description));

#if false
			Types.StructuredData data1 = accessor.Collection["Text1"].GetCultureData ("fr");
			Types.StructuredData data2 = accessor.Collection["Text1"].GetCultureData ("fr");

			Assert.AreSame (data1, data2);
			Assert.AreEqual ("Bonjour", data1.GetValue (Res.Fields.ResourceString.Text));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["Text1"].GetCultureData ("de");
			data2 = accessor.Collection["Text1"].GetCultureData ("de");

			Assert.IsNotNull (data1);
			Assert.AreSame (data1, data2);
			Assert.AreEqual (Types.UndefinedValue.Instance, data1.GetValue (Res.Fields.ResourceString.Text));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["Text1"].GetCultureData ("fr");
			data1.SetValue (Res.Fields.ResourceString.Text, "Bonjour tout le monde");
			data2.SetValue (Res.Fields.ResourceString.Text, "Hallo, Welt");

			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("Bonjour tout le monde", this.manager.GetText (Druid.Parse ("[4006]"), ResourceLevel.Localized, Resources.FindCultureInfo ("fr")));
			Assert.AreEqual ("Hallo, Welt", this.manager.GetText (Druid.Parse ("[4006]"), ResourceLevel.Localized, Resources.FindCultureInfo ("de")));

			CultureMap map = accessor.CreateItem ();

			Assert.IsNotNull (map);
			Assert.AreEqual (Druid.Parse ("[4008]"), map.Id);
			Assert.IsNull (accessor.Collection[map.Id]);

			accessor.Collection.Add (map);
			Assert.IsTrue (accessor.ContainsChanges);

			map.Name = "NewItem";
			map.GetCultureData ("00").SetValue (Res.Fields.ResourceString.Text, "New value");
			map.GetCultureData ("fr").SetValue (Res.Fields.ResourceString.Text, "Nouvelle valeur");

			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("New value", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Default));
			Assert.AreEqual ("Nouvelle valeur", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")));

			map.GetCultureData ("fr").SetValue (Res.Fields.ResourceString.Text, Types.UndefinedValue.Instance);

			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("New value", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Default));
			Assert.AreEqual ("New value", this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")));

			accessor.Collection.Remove (map);
			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.IsNull (this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Default));
			Assert.IsNull (this.manager.GetText (Druid.Parse ("[4008]"), ResourceLevel.Localized, Resources.FindCultureInfo ("fr")));
#endif
		}

		ResourceManager manager;
	}
}
