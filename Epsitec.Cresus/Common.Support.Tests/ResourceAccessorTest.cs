using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourceAccessorTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();

			this.manager = new ResourceManager (typeof (ResourceAccessorTest));
			this.manager.DefineDefaultModuleName ("Test");
		}
		
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckBasicTypes()
		{
			Assert.IsNotNull (Types.DruidType.Default);

			Types.StructuredType typeStruct = Res.Types.ResourceStructuredType;
			Types.StructuredType typeField  = Res.Types.Field;
//-			Types.CollectionType typeFields = Res.Types.FieldCollection;
			
			Types.EnumType typeFieldRelation   = Types.Res.Types.FieldRelation;
			Types.EnumType typeFieldMembership = Types.Res.Types.FieldMembership;

			Assert.AreEqual (typeField, typeStruct.GetField (Res.Fields.ResourceStructuredType.Fields.ToString ()).Type);
			Assert.AreEqual (Types.FieldRelation.Collection, typeStruct.GetField (Res.Fields.ResourceStructuredType.Fields.ToString ()).Relation);
//-			Assert.AreEqual (typeField, typeFields.ItemType);
			Assert.AreEqual (typeof (Types.FieldRelation), typeFieldRelation.SystemType);
			Assert.AreEqual (typeof (Types.FieldMembership), typeFieldMembership.SystemType);
			Assert.AreEqual (typeFieldRelation.SystemType, typeField.GetField (Res.Fields.Field.Relation.ToString ()).Type.SystemType);
			Assert.AreEqual (typeFieldMembership.SystemType, typeField.GetField (Res.Fields.Field.Membership.ToString ()).Type.SystemType);
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

			Types.StructuredData data1 = accessor.Collection["PatternAngle"].GetCultureData ("fr");
			Types.StructuredData data2 = accessor.Collection["PatternAngle"].GetCultureData ("fr");

			Assert.AreSame (data1, data2);
			Assert.AreEqual ("Angle de rotation de la trame, exprimé en degrés.", data1.GetValue (Res.Fields.ResourceCaption.Description));
			Assert.AreEqual (3, (data1.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>).Count);
			Assert.AreEqual ("A", (data1.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>)[0]);
			Assert.AreEqual ("Angle", (data1.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>)[1]);
			Assert.AreEqual ("Angle de la trame", (data1.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>)[2]);
			Assert.IsTrue (data1.IsValueLocked (Res.Fields.ResourceCaption.Labels));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["PatternAngle"].GetCultureData ("de");
			data2 = accessor.Collection["PatternAngle"].GetCultureData ("de");

			Assert.IsNotNull (data1);
			Assert.AreSame (data1, data2);
			Assert.AreEqual (0, ((IList<string>)data1.GetValue (Res.Fields.ResourceCaption.Labels)).Count);
			Assert.AreEqual (Types.UndefinedValue.Instance, data1.GetValue (Res.Fields.ResourceCaption.Description));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["PatternAngle"].GetCultureData ("fr");
			data1.SetValue (Res.Fields.ResourceCaption.Description, "Angle de la hachure");
			data2.SetValue (Res.Fields.ResourceCaption.Description, "Schraffurwinkel");

			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("Angle de la hachure", this.manager.GetCaption (Druid.Parse ("[4002]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")).Description);
			Assert.AreEqual ("Schraffurwinkel", this.manager.GetCaption (Druid.Parse ("[4002]"), ResourceLevel.Merged, Resources.FindCultureInfo ("de")).Description);

			IList<string> labels = data1.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>;

			labels.RemoveAt (2);

			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			labels[0] = "A.";

			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			CultureMap map = accessor.CreateItem ();

			Assert.IsNotNull (map);
			Assert.AreEqual (Druid.Parse ("[400C]"), map.Id);
			Assert.IsNull (accessor.Collection[map.Id]);

			accessor.Collection.Add (map);
			Assert.IsTrue (accessor.ContainsChanges);

			map.Name = "NewItem";
			map.GetCultureData ("00").SetValue (Res.Fields.ResourceCaption.Description, "New value");
			map.GetCultureData ("fr").SetValue (Res.Fields.ResourceCaption.Description, "Nouvelle valeur");

			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("New value", this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Default).Description);
			Assert.AreEqual ("Nouvelle valeur", this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")).Description);
			Assert.AreEqual ("NewItem", this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Default).Name);
			Assert.AreEqual ("NewItem", this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")).Name);
			Assert.AreEqual ("Cap.NewItem", this.manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Druid.Parse ("[400C]")].Name);
			Assert.IsTrue (string.IsNullOrEmpty (this.manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Localized, Resources.FindCultureInfo ("fr"))[Druid.Parse ("[400C]")].Name));

			map.GetCultureData ("fr").SetValue (Res.Fields.ResourceCaption.Description, Types.UndefinedValue.Instance);

			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("New value", this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Default).Description);
			Assert.AreEqual ("New value", this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")).Description);

			accessor.Collection.Remove (map);
			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.IsNull (this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Default));
			Assert.IsNull (this.manager.GetCaption (Druid.Parse ("[400C]"), ResourceLevel.Merged, Resources.FindCultureInfo ("fr")));
		}

		[Test]
		public void CheckCommandAccessor()
		{
			ResourceAccessors.CommandResourceAccessor accessor = new ResourceAccessors.CommandResourceAccessor ();

			Assert.IsFalse (accessor.ContainsChanges);

			accessor.Load (this.manager);
			
			Assert.AreEqual (1, accessor.Collection.Count);
		}

		[Test]
		public void CheckStructuredTypeAccessor()
		{
			ResourceAccessors.StructuredTypeResourceAccessor accessor = new ResourceAccessors.StructuredTypeResourceAccessor ();

			Assert.IsFalse (accessor.ContainsChanges);

			accessor.Load (Res.Manager);

			Assert.AreEqual (16, accessor.Collection.Count);

			CultureMap map = accessor.Collection[Res.Types.ResourceStructuredType.CaptionId];

			Assert.AreEqual ("ResourceStructuredType", map.Name);
			Assert.AreEqual ("Typ.StructuredType.ResourceStructuredType", Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Res.Types.ResourceStructuredType.CaptionId].Name);
			Assert.AreEqual ("Fld.ResourceStructuredType.Fields", Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Res.Fields.ResourceStructuredType.Fields].Name);
			
			Types.StructuredData        data       = map.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			Types.StructuredTypeClass   typeClass  = (Types.StructuredTypeClass) data.GetValue (Res.Fields.ResourceStructuredType.Class);
			Druid                       baseTypeId = (Druid) data.GetValue (Res.Fields.ResourceStructuredType.BaseType);
			IList<Types.StructuredData> fields     = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<Types.StructuredData>;

			Assert.AreEqual (12, fields.Count);

			Assert.AreEqual (Res.Fields.ResourceBase.ModificationId, fields[0].GetValue (Res.Fields.Field.CaptionId));
			Assert.AreEqual (Res.Fields.ResourceBase.Comment,        fields[1].GetValue (Res.Fields.Field.CaptionId));

			map.Name = "ResourceEntityType";
			accessor.PersistChanges ();

			Assert.AreEqual ("ResourceEntityType", map.Name);
			Assert.AreEqual ("Typ.StructuredType.ResourceEntityType", Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Res.Types.ResourceStructuredType.CaptionId].Name);
			Assert.AreEqual ("Fld.ResourceEntityType.Fields", Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Res.Fields.ResourceStructuredType.Fields].Name);
			Assert.AreEqual ("ResourceEntityType.Fields", accessor.FieldAccessor.Collection[Res.Fields.ResourceStructuredType.Fields].ToString ());
			
			map.Name = "ResourceStructuredType";
			accessor.PersistChanges ();

			Assert.AreEqual ("ResourceStructuredType", map.Name);
			Assert.AreEqual ("Typ.StructuredType.ResourceStructuredType", Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Res.Types.ResourceStructuredType.CaptionId].Name);
			Assert.AreEqual ("Fld.ResourceStructuredType.Fields", Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Res.Fields.ResourceStructuredType.Fields].Name);
			Assert.AreEqual ("ResourceStructuredType.Fields", accessor.FieldAccessor.Collection[Res.Fields.ResourceStructuredType.Fields].ToString ());

			CultureMap fieldItem;

			fieldItem = accessor.FieldAccessor.Collection[Res.Fields.ResourceStructuredType.Fields];

			Assert.AreEqual ("Fields", fieldItem.Name);
			Assert.AreEqual ("ResourceStructuredType.Fields", fieldItem.ToString ());
			
			fieldItem = accessor.CreateFieldItem (map);

			fieldItem.Name = "X";

			accessor.FieldAccessor.Collection.Add (fieldItem);
			accessor.FieldAccessor.PersistChanges ();
			
			Assert.AreEqual ("Fld.ResourceStructuredType.X", Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[fieldItem.Id].Name);
			Assert.AreEqual ("ResourceStructuredType.X", accessor.FieldAccessor.Collection[fieldItem.Id].ToString ());

			accessor.FieldAccessor.Collection.Remove (fieldItem);
			accessor.FieldAccessor.PersistChanges ();
			
			Assert.IsTrue (Res.Manager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[fieldItem.Id].IsEmpty);
		}

		[Test]
		public void CheckAnyTypeAccessor()
		{
			ResourceAccessors.AnyTypeResourceAccessor accessor = new ResourceAccessors.AnyTypeResourceAccessor ();

			Assert.IsFalse (accessor.ContainsChanges);

			accessor.Load (Epsitec.Common.Types.Res.Manager);


			System.Console.Out.WriteLine ("{0} resources found", accessor.Collection.Count);

			foreach (CultureMap item in accessor.Collection)
			{
				Types.StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
				string[] ids = Types.Collection.ToArray (data.GetValueIds ());
				Types.TypeCode code = (Types.TypeCode) data.GetValue (Res.Fields.ResourceBaseType.TypeCode);

				System.Console.Out.WriteLine ("  {2}, {0} --> {1}", item.Name, item.Id, code);

				foreach (string id in ids)
				{
					System.Console.Out.WriteLine ("    {0}: {1}", id, data.GetValue (id));
				}
			}

			//	Check Integer
			
			CultureMap newItem = accessor.CreateItem ();
			Types.StructuredData newData = newItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			newItem.Name = "AnyTypeAccessorInteger1";

			newData.SetValue (Res.Fields.ResourceBaseType.TypeCode, Types.TypeCode.Integer);
			newData.SetValue (Res.Fields.ResourceNumericType.SmallStep, 1M);
			newData.SetValue (Res.Fields.ResourceNumericType.LargeStep, 10M);
			newData.SetValue (Res.Fields.ResourceNumericType.Range, new Types.DecimalRange (0, 999, 1));

			accessor.Collection.Add (newItem);
			accessor.PersistChanges ();

			Types.Caption caption = accessor.ResourceManager.GetCaption (newItem.Id, ResourceLevel.Default);
			Types.IntegerType intType = Types.TypeRosetta.CreateTypeObject (caption, false) as Types.IntegerType;

			Assert.IsNotNull (intType);
			Assert.AreEqual (1M, intType.SmallStep);
			Assert.AreEqual (10M, intType.LargeStep);
			Assert.AreEqual (999M, intType.Range.Maximum);
			
			//	Check DateTime
			
			newItem = accessor.CreateItem ();
			newData = newItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			newItem.Name = "AnyTypeAccessorDateTime1";

			newData.SetValue (Res.Fields.ResourceBaseType.TypeCode, Types.TypeCode.DateTime);
			newData.SetValue (Res.Fields.ResourceDateTimeType.Resolution, Types.TimeResolution.Minutes);
			newData.SetValue (Res.Fields.ResourceDateTimeType.MinimumDate, new Types.Date (2000, 06, 10));
			newData.SetValue (Res.Fields.ResourceDateTimeType.TimeStep, new System.TimeSpan (0, 15, 0));
			
			accessor.Collection.Add (newItem);
			accessor.PersistChanges ();

			caption = accessor.ResourceManager.GetCaption (newItem.Id, ResourceLevel.Default);
			Types.DateTimeType dtType = Types.TypeRosetta.CreateTypeObject (caption, false) as Types.DateTimeType;

			Assert.IsNotNull (dtType);
			Assert.AreEqual (Types.TimeResolution.Minutes, dtType.Resolution);
			Assert.AreEqual (2000, dtType.MinimumDate.Year);
			Assert.AreEqual (6, dtType.MinimumDate.Month);
			Assert.AreEqual (10, dtType.MinimumDate.Day);
			Assert.IsTrue (dtType.MaximumDate.IsNull);
			Assert.IsTrue (dtType.MinimumTime.IsNull);
			Assert.IsTrue (dtType.MaximumTime.IsNull);
			Assert.AreEqual (15, dtType.TimeStep.TotalMinutes);

			//	Check Other
			
			newItem = accessor.CreateItem ();
			newData = newItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			newItem.Name = "AnyTypeAccessorOther1";

			newData.SetValue (Res.Fields.ResourceBaseType.TypeCode, Types.TypeCode.Other);
			newData.SetValue (Res.Fields.ResourceOtherType.SystemType, typeof (char));

			accessor.Collection.Add (newItem);
			accessor.PersistChanges ();

			caption = accessor.ResourceManager.GetCaption (newItem.Id, ResourceLevel.Default);
			Types.OtherType otherType = Types.TypeRosetta.CreateTypeObject (caption, false) as Types.OtherType;

			Assert.IsNotNull (otherType);
			Assert.AreEqual (typeof (char).Name, otherType.SystemType.Name);

			//	Check String

			newItem = accessor.CreateItem ();
			newData = newItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			newItem.Name = "AnyTypeAccessorString1";

			newData.SetValue (Res.Fields.ResourceBaseType.TypeCode, Types.TypeCode.String);
			newData.SetValue (Res.Fields.ResourceStringType.UseMultilingualStorage, true);
			newData.SetValue (Res.Fields.ResourceStringType.MinimumLength, 1);

			accessor.Collection.Add (newItem);
			accessor.PersistChanges ();

			caption = accessor.ResourceManager.GetCaption (newItem.Id, ResourceLevel.Default);
			Types.StringType stringType = Types.TypeRosetta.CreateTypeObject (caption, false) as Types.StringType;

			Assert.IsNotNull (stringType);
			Assert.AreEqual (true, stringType.UseMultilingualStorage);
			Assert.AreEqual (1, stringType.MinimumLength);
			Assert.AreEqual (1000, stringType.MaximumLength);

			//	Check Collection

			newItem = accessor.CreateItem ();
			newData = newItem.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			newItem.Name = "AnyTypeAccessorCollection1";

			newData.SetValue (Res.Fields.ResourceBaseType.TypeCode, Types.TypeCode.Collection);
			newData.SetValue (Res.Fields.ResourceCollectionType.ItemType, Types.IntegerType.Default.CaptionId);

			accessor.Collection.Add (newItem);
			accessor.PersistChanges ();

			caption = accessor.ResourceManager.GetCaption (newItem.Id, ResourceLevel.Default);
			Types.CollectionType colType = Types.TypeRosetta.CreateTypeObject (caption, false) as Types.CollectionType;

			Assert.IsNotNull (colType);
			Assert.AreEqual (Types.IntegerType.Default.CaptionId, colType.ItemType.CaptionId);

			//	Check Enumeration

			CultureMap map = accessor.Collection[Types.Res.Types.BindingMode.CaptionId];

			Assert.AreEqual ("BindingMode", map.Name);
			Assert.AreEqual ("Typ.BindingMode", accessor.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Types.Res.Types.BindingMode.CaptionId].Name);
			Assert.AreEqual ("Val.BindingMode.None", accessor.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Types.Res.Values.BindingMode.None.Id].Name);

			Types.StructuredData enumData = map.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			IList<Types.StructuredData> enumValues = enumData.GetValue (Res.Fields.ResourceEnumType.Values) as IList<Types.StructuredData>;

			Assert.AreEqual (5, enumValues.Count);
			Assert.AreEqual (typeof (Types.BindingMode), enumData.GetValue (Res.Fields.ResourceEnumType.SystemType) as System.Type);

			Assert.AreEqual (Types.Res.Values.BindingMode.None.Id, enumValues[0].GetValue (Res.Fields.EnumValue.CaptionId));
			Assert.AreEqual (Types.Res.Values.BindingMode.TwoWay.Id, enumValues[4].GetValue (Res.Fields.EnumValue.CaptionId));

			map.Name = "Foo";
			accessor.PersistChanges ();

			Assert.AreEqual ("Foo", map.Name);
			Assert.AreEqual ("Typ.Foo", accessor.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Types.Res.Types.BindingMode.CaptionId].Name);
			Assert.AreEqual ("Val.Foo.None", accessor.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Types.Res.Values.BindingMode.None.Id].Name);
			Assert.AreEqual ("Foo.None", accessor.ValueAccessor.Collection[Types.Res.Values.BindingMode.None.Id].ToString ());

			map.Name = "BindingMode";
			accessor.PersistChanges ();

			Assert.AreEqual ("BindingMode", map.Name);
			Assert.AreEqual ("Typ.BindingMode", accessor.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Types.Res.Types.BindingMode.CaptionId].Name);
			Assert.AreEqual ("Val.BindingMode.None", accessor.ResourceManager.GetBundle (Resources.CaptionsBundleName, ResourceLevel.Default)[Types.Res.Values.BindingMode.None.Id].Name);
			Assert.AreEqual ("BindingMode.None", accessor.ValueAccessor.Collection[Types.Res.Values.BindingMode.None.Id].ToString ());
		}

		[Test]
		public void CheckMetadata()
		{
			ResourceAccessors.StringResourceAccessor  stringAccessor  = new ResourceAccessors.StringResourceAccessor ();
			ResourceAccessors.CaptionResourceAccessor captionAccessor = new ResourceAccessors.CaptionResourceAccessor ();
			ResourceAccessors.CommandResourceAccessor commandAccessor = new ResourceAccessors.CommandResourceAccessor ();
			ResourceAccessors.StructuredTypeResourceAccessor structAccessor = new ResourceAccessors.StructuredTypeResourceAccessor ();

			stringAccessor.Load (this.manager);
			captionAccessor.Load (this.manager);
			commandAccessor.Load (this.manager);
			structAccessor.Load (Res.Manager);

			System.Console.Out.WriteLine ("Strings:");
			this.DumpCultureMap (stringAccessor.Collection[0]);
			System.Console.Out.WriteLine ("Captions:");
			this.DumpCultureMap (captionAccessor.Collection[0]);
			System.Console.Out.WriteLine ("Commands:");
			this.DumpCultureMap (commandAccessor.Collection[0]);
			System.Console.Out.WriteLine ("Structured Types:");
			this.DumpCultureMap (structAccessor.Collection[0]);
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
			Assert.AreEqual (0, data1.GetValue (Res.Fields.ResourceBase.ModificationId));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["Text1"].GetCultureData ("de");
			data2 = accessor.Collection["Text1"].GetCultureData ("de");

			Assert.IsNotNull (data1);
			Assert.AreSame (data1, data2);
			Assert.AreEqual (Types.UndefinedValue.Instance, data1.GetValue (Res.Fields.ResourceString.Text));
			Assert.IsFalse (accessor.ContainsChanges);
			Assert.IsTrue (data1.IsEmpty);

			data1 = accessor.Collection["Text1"].GetCultureData ("fr");
			data1.SetValue (Res.Fields.ResourceString.Text, "Bonjour tout le monde");
			data2.SetValue (Res.Fields.ResourceString.Text, "Hallo, Welt");
			data2.SetValue (Res.Fields.ResourceBase.ModificationId, 1);

			Assert.IsTrue (accessor.ContainsChanges);
			Assert.AreEqual (1, accessor.PersistChanges ());
			Assert.IsFalse (accessor.ContainsChanges);

			Assert.AreEqual ("Bonjour tout le monde", this.manager.GetText (Druid.Parse ("[4006]"), ResourceLevel.Localized, Resources.FindCultureInfo ("fr")));
			Assert.AreEqual ("Hallo, Welt", this.manager.GetText (Druid.Parse ("[4006]"), ResourceLevel.Localized, Resources.FindCultureInfo ("de")));
			Assert.AreEqual (1, this.manager.GetBundle ("Strings", ResourceLevel.Localized, Resources.FindCultureInfo ("de"))[Druid.Parse ("[4006]")].ModificationId);

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
		public void CheckUI()
		{
			ResourceManager manager = new ResourceManager (typeof (ResourceAccessorTest));
			manager.DefineDefaultModuleName ("Common.Document");

			ResourceAccessors.StringResourceAccessor stringAccessor = new ResourceAccessors.StringResourceAccessor ();
			stringAccessor.Load (manager);

			IResourceAccessor accessor = stringAccessor;

			Types.StructuredType cultureMapType = new Types.StructuredType ();
			cultureMapType.Fields.Add ("Name", Types.StringType.Default);

			Types.CollectionView collectionView = new Types.CollectionView (accessor.Collection);

			Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Widgets.Window window = new Widgets.Window ();
			window.Text = "CheckUI";
			window.ClientSize = new Drawing.Size (300, 500);

			UI.ItemTable table = new UI.ItemTable (window.Root);
			table.Dock = Widgets.DockStyle.Fill;
			
			table.SourceType = cultureMapType;
			table.Items = collectionView;
			table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("Name"));
			table.HorizontalScrollMode = Epsitec.Common.UI.ItemTableScrollMode.None;
			table.VerticalScrollMode = Epsitec.Common.UI.ItemTableScrollMode.ItemBased;
			table.HeaderVisibility = false;
			table.FrameVisibility = false;
			table.ItemPanel.Layout = Epsitec.Common.UI.ItemPanelLayout.VerticalList;
			table.ItemPanel.ItemSelectionMode = Epsitec.Common.UI.ItemPanelSelectionMode.ExactlyOne;
			table.Margins = new Drawing.Margins (4, 1, 4, 2);

			table.SizeChanged += this.HandleTableSizeChanged;

			Widgets.TextFieldMulti field = new Epsitec.Common.Widgets.TextFieldMulti (window.Root);
			field.Dock = Widgets.DockStyle.Bottom;
			field.PreferredHeight = 60;
			field.Margins = new Drawing.Margins (4, 0, 2, 4);
			
			Widgets.HSplitter splitter = new Epsitec.Common.Widgets.HSplitter (window.Root);
			splitter.Dock = Widgets.DockStyle.Bottom;
			splitter.PreferredHeight = 8;

			//	Critère de tri : selon le nom (on n'a pas vraiment le choix, vu la définition
			//	de CultureMap)
			
			collectionView.SortDescriptions.Clear ();
			collectionView.SortDescriptions.Add (new Epsitec.Common.Types.SortDescription ("Name"));

			//	Filtre uniquement les items qui ont un "b" dans leur nom :

			collectionView.Filter +=
				delegate (object obj)
				{
					CultureMap item = obj as CultureMap;
					
					if (item.Name.Contains ("b"))
					{
						return true;
					}
					else
					{
						return false;
					}
				};

			table.ItemPanel.SelectionChanged +=
				delegate
				{
					CultureMap item = collectionView.CurrentItem as CultureMap;
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					string[] cultures = new string[] { "00", "fr", "en", "de" };
					
					foreach (string culture in cultures)
					{
						Types.StructuredData data = item.GetCultureData (culture);
						string text = data.GetValue (Res.Fields.ResourceString.Text) as string;
						if (text != null)
						{
							buffer.Append (culture);
							buffer.Append (": ");
							buffer.Append (Widgets.TextLayout.ConvertToTaggedText (text));
							buffer.Append ("<br/>");
						}
					}
					field.Text = buffer.ToString ();
				};

			window.Show ();
			Widgets.Window.RunInTestEnvironment (window);
		}

		void HandleTableSizeChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			UI.ItemTable table = (UI.ItemTable) sender;
			Drawing.Size size = (Drawing.Size) e.NewValue;

			double width = size.Width - table.GetPanelPadding ().Width;
			table.ColumnHeader.SetColumnWidth (0, width);

			table.ItemPanel.ItemViewDefaultSize = new Epsitec.Common.Drawing.Size (width, 20);
		}

		private void DumpCultureMap(CultureMap map)
		{
			foreach (string culture in map.GetDefinedCultures ())
			{
				Types.StructuredData  data = map.GetCultureData (culture);
				Types.IStructuredType type = data.StructuredType;

				this.DumpStructuredData ("", data, type);
			}
		}

		private void DumpStructuredData(string indent, Types.StructuredData data, Types.IStructuredType type)
		{
			if (data == null)
			{
				return;
			}

			foreach (string fieldId in type.GetFieldIds ())
			{
				Types.StructuredTypeField field = type.GetField (fieldId);
				Types.Caption caption = this.manager.GetCaption (field.CaptionId);

				System.Console.Out.WriteLine ("{4}{0} ({1}) : type = {2}, data = {3}, relation = {5}, {6}", fieldId, (caption == null) ? "<?>" : caption.Name, (field.Type == null) ? "<null>" : field.Type.Name, data.GetValue (fieldId), indent, field.Relation, field.Membership);

				if ((field.Type is Types.IStructuredType) &&
					(field.Relation != Types.FieldRelation.Collection))
				{
					this.DumpStructuredData ("  " + indent, data.GetValue (fieldId) as Types.StructuredData, field.Type as Types.IStructuredType);
				}
				else if (field.Type is Types.CollectionType)
				{
					Types.CollectionType collectionType = field.Type as Types.CollectionType;

					if (collectionType.ItemType is Types.IStructuredType)
					{
						System.Collections.IList collection = data.GetValue (fieldId) as System.Collections.IList;
						Types.StructuredData item0;
						
						if (collection.Count > 0)
						{
							item0 = collection[0] as Types.StructuredData;
						}
						else
						{
							item0 = new Types.StructuredData (collectionType.ItemType as Types.IStructuredType);
						}

						this.DumpStructuredData ("* " + indent, item0, collectionType.ItemType as Types.IStructuredType);
					}
				}
				else if (field.Relation == Types.FieldRelation.Collection)
				{
					Types.AbstractType collectionType = field.Type as Types.AbstractType;

					if (collectionType is Types.IStructuredType)
					{
						System.Collections.IList collection = data.GetValue (fieldId) as System.Collections.IList;
						Types.StructuredData item0;

						if (collection.Count > 0)
						{
							item0 = collection[0] as Types.StructuredData;
						}
						else
						{
							item0 = new Types.StructuredData (collectionType as Types.IStructuredType);
						}

						this.DumpStructuredData ("* " + indent, item0, collectionType as Types.IStructuredType);
					}
				}
			}
		}

		ResourceManager manager;
	}
}
