//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	[TestFixture]
	public class DataSourceTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
		}

		[Test]
		public void CheckCreation()
		{
			DataSource collection = new DataSource ();

			Widgets.Visual source1 = new Widgets.Visual ();
			MySimpleDataSource source2 = new MySimpleDataSource ();

			collection.AddDataSource ("A", source1);
			collection.AddDataSource ("B", source2);

			StructuredTree.SetValue (collection, "A.Name", "Source1");
			StructuredTree.SetValue (collection, "B.x", 1);
			StructuredTree.SetValue (collection, "B.y", "foo");

			Assert.AreEqual ("Source1", StructuredTree.GetValue (collection, "A.Name"));
			Assert.AreEqual (-1, StructuredTree.GetValue (collection, "A.Index"));
			Assert.AreEqual (1, StructuredTree.GetValue (collection, "B.x"));
			Assert.AreEqual ("foo", StructuredTree.GetValue (collection, "B.y"));

			IStructuredType structuredType = collection as IStructuredType;

			Assert.IsNotNull (structuredType);
			Assert.AreEqual (source1.ObjectType.SystemType, structuredType.GetField ("A").Type.SystemType);

			foreach (string name in collection.GetFieldIds ())
			{
				System.Console.Out.WriteLine ("Name: {0}", name);

				IStructuredData data = collection.GetDataSource (name);
				object type = TypeRosetta.GetTypeObjectFromValue (data);
				IStructuredType tree = TypeRosetta.GetStructuredTypeFromTypeObject (type);

				Assert.IsNotNull (data);
				Assert.IsNotNull (tree);

				System.Text.StringBuilder buffer1 = new System.Text.StringBuilder ();
				System.Text.StringBuilder buffer2 = new System.Text.StringBuilder ();

				foreach (string subPath in StructuredTree.GetFieldPaths (collection, name))
				{
					buffer1.Append (subPath);
					buffer1.Append (" ");

					System.Console.Out.WriteLine ("  {0}", subPath);
				}

				foreach (string subPath in tree.GetFieldIds ())
				{
					buffer2.Append (name);
					buffer2.Append (".");
					buffer2.Append (subPath);
					buffer2.Append (" ");
				}

				Assert.AreEqual (buffer1.ToString (), buffer2.ToString ());
			}
		}

		[Test]
		public void CheckGetValue()
		{
			StringType strType = new StringType ();
			DataSource source = new DataSource ();
			StructuredType recType = new StructuredType ();
			StructuredData rec = new StructuredData (recType);

			strType.DefineDefaultValue ("");
			strType.DefineSampleValue ("Abc");

			recType.Fields.Add ("A", strType);
			recType.Fields.Add ("B", strType);
			recType.Fields.Add ("R", recType);

			rec.SetValue ("A", "a");

			source.AddDataSource ("X", rec);

			IStructuredData data = source;

			Assert.AreEqual (rec, data.GetValue ("X"));
			Assert.AreEqual (UnknownValue.Value, data.GetValue ("Y"));

			Assert.AreEqual (rec, StructuredTree.GetValue (data, "X"));
			Assert.AreEqual ("a", StructuredTree.GetValue (data, "X.A"));
			Assert.AreEqual (UndefinedValue.Value, StructuredTree.GetValue (data, "X.R.R.R.A"));
			Assert.AreEqual (UndefinedValue.Value, StructuredTree.GetValue (data, "X.B"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (data, "X.C"));

			rec.UndefinedValueMode = UndefinedValueMode.Default;

			Assert.AreEqual ("", StructuredTree.GetValue (data, "X.B"));
			Assert.AreEqual ("", StructuredTree.GetValue (data, "X.R.R.R.A"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (data, "X.C"));

			rec.UndefinedValueMode = UndefinedValueMode.Sample;

			Assert.AreEqual ("Abc", StructuredTree.GetValue (data, "X.B"));
			Assert.AreEqual ("Abc", StructuredTree.GetValue (data, "X.R.R.R.A"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (data, "X.C"));
		}

		[Test]
		public void CheckPanelSerializationContext()
		{
			Support.ResourceManager manager = new Support.ResourceManager ();
			Panel panel = new UI.Panel ();
			DataSource collection = new DataSource ();

			panel.DataSource = collection;

			Widgets.Visual source1 = new Widgets.Visual ();
			Widgets.Visual source2 = new Widgets.Visual ();

			source1.Name = "Source1";
			source2.Name = "Source2";

			collection.AddDataSource ("B", source2);
			collection.AddDataSource ("A", source1);

			Assert.AreEqual (source1, StructuredTree.GetValue (collection, "A"));
			Assert.AreEqual (source2, StructuredTree.GetValue (collection, "B"));

			Types.Serialization.Context context = new Types.Serialization.Context ();

			UI.Panel.FillSerializationContext (context, collection, manager);

			Assert.AreEqual (2, context.ExternalMap.TagCount);
			Assert.AreEqual ("_DataSource", Collection.ToArray<string> (context.ExternalMap.RecordedTags)[0]);
			Assert.AreEqual ("_ResourceManager", Collection.ToArray<string> (context.ExternalMap.RecordedTags)[1]);
			Assert.AreEqual (collection, context.ExternalMap.GetValue ("_DataSource"));
			Assert.AreEqual (manager, context.ExternalMap.GetValue ("_ResourceManager"));

			Widgets.Button b1 = new Epsitec.Common.Widgets.Button ();
			Binding binding = new Binding (BindingMode.OneWay, null, "A.Name");

			b1.Dock = Widgets.DockStyle.Top;
			b1.SetBinding (Widgets.Visual.NameProperty, binding);

			Assert.AreEqual (DataSourceType.None, b1.GetBindingExpression (Widgets.Visual.NameProperty).DataSourceType);

			panel.Children.Add (b1);

			Assert.AreEqual (DataSourceType.PropertyObject, b1.GetBindingExpression (Widgets.Visual.NameProperty).DataSourceType);
			Assert.AreEqual (source1.Name, b1.Name);

			source1.Name = "X";

			Assert.AreEqual ("X", source1.Name);
			Assert.AreEqual ("X", b1.Name);

			b1.Name = "Y";

			Assert.AreEqual ("X", source1.Name);
			Assert.AreEqual ("Y", b1.Name);
		}

		[Test]
		public void CheckPanelDataSourceBinding1()
		{
			Panel panel = new UI.Panel ();
			DataSource collection = new DataSource ();
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("Label", Types.StringType.NativeDefault);
			data.SetValue ("Label", "Hello");

			panel.DataSource = collection;

			collection.AddDataSource ("A", data);

			Widgets.Button b1 = new Epsitec.Common.Widgets.Button ();

			panel.Children.Add (b1);

			b1.SetBinding (Widgets.Visual.NameProperty, new Binding (BindingMode.OneWay, null, "A.Label"));

			Assert.AreEqual (DataSourceType.StructuredData, b1.GetBindingExpression (Widgets.Visual.NameProperty).DataSourceType);
			Assert.AreEqual (typeof (StringType), b1.GetBindingExpression (Widgets.Visual.NameProperty).GetSourceTypeObject ().GetType ());
			Assert.AreEqual ("Hello", b1.Name);

			data.SetValue ("Label", "Good bye");

			Assert.AreEqual ("Good bye", b1.Name);
		}

		[Test]
		public void CheckPanelDataSourceBinding2()
		{
			Panel panel = new UI.Panel ();
			DataSource source = new DataSource ();
			
			StructuredType type1 = new StructuredType ();
			StructuredType type2 = new StructuredType ();
			
			StructuredData data1 = new StructuredData (type1);
			StructuredData data2 = new StructuredData (type2);

			type1.Fields.Add ("A", Types.StringType.NativeDefault);
			type1.Fields.Add ("R", type2);

			type2.Fields.Add ("B", Types.StringType.NativeDefault);
			
			data1.SetValue ("A", "a");
			data1.SetValue ("R", data2);

			data2.SetValue ("B", "b");

			panel.DataSource = source;

			source.AddDataSource ("*", data1);

			Widgets.Button b1 = new Epsitec.Common.Widgets.Button ();
			Widgets.Button b2 = new Epsitec.Common.Widgets.Button ();

			panel.Children.Add (b1);

			b1.SetBinding (Widgets.Visual.NameProperty, new Binding (BindingMode.OneWay, null, "*.A"));
			b2.SetBinding (Widgets.Visual.NameProperty, new Binding (BindingMode.OneWay, null, "*.B"));

			Assert.AreEqual ("a", b1.Name);
			Assert.AreEqual (null, b2.Name);

			PanelPlaceholder placeholder = new PanelPlaceholder ();
			Panel subPanel = new Panel ();

			subPanel.Children.Add (b2);

			placeholder.DefinePanel (subPanel);
			
			placeholder.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.OneWay, null, "*.R"));

			panel.Children.Add (placeholder);

			Assert.AreEqual ("a", b1.Name);
			Assert.AreEqual ("b", b2.Name);
		}

		[Test]
		public void CheckGetFieldTypeObject()
		{
			DataSource collection = new DataSource ();

			Widgets.Visual source1 = new Widgets.Visual ();
			MySimpleDataSource source2 = new MySimpleDataSource ();

			source2.SetValue ("Name", "Petrus");
			source2.SetValue ("BirthDateYear", 1972);

			collection.AddDataSource ("B", source2);
			collection.AddDataSource ("A", source1);

			IStructuredType type = collection as IStructuredType;

			Assert.IsNotNull (type);

			Assert.AreEqual (typeof (Widgets.Visual), StructuredTree.GetField (type, "A").Type.SystemType);
			Assert.AreEqual (typeof (DynamicStructuredType), StructuredTree.GetField (type, "B").Type.GetType ());
			Assert.AreEqual (typeof (StringType), StructuredTree.GetField (type, "A.Name").Type.GetType ());
			Assert.AreEqual (typeof (StringType), StructuredTree.GetField (type, "B.Name").Type.GetType ());
			Assert.AreEqual (typeof (IntegerType), StructuredTree.GetField (type, "B.BirthDateYear").Type.GetType ());

			Assert.AreEqual (typeof (Widgets.Visual), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetField (type, "A").Type));
			Assert.AreEqual (null, Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetField (type, "B").Type));
			Assert.AreEqual (typeof (string), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetField (type, "A.Name").Type));
			Assert.AreEqual (typeof (string), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetField (type, "B.Name").Type));
			Assert.AreEqual (typeof (int), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetField (type, "B.BirthDateYear").Type));
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckSetValueEx1()
		{
			DataSource collection = new DataSource ();

			Widgets.Visual source1 = new Widgets.Visual ();

			collection.AddDataSource ("A", source1);
			StructuredTree.SetValue (collection, "A", source1);
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckSetValueEx2()
		{
			DataSource collection = new DataSource ();

			Widgets.Visual source1 = new Widgets.Visual ();

			StructuredTree.SetValue (collection, "A", source1);
		}

		private class MySimpleDataSource : IStructuredData
		{
			public MySimpleDataSource()
			{
			}

			#region IStructuredData Members

			public void AttachListener(string name, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public void DetachListener(string name, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public void SetValue(string name, object value)
			{
				this.data[name] = value;
			}

			public IEnumerable<string> GetValueIds()
			{
				string[] names = new string[this.data.Keys.Count];
				this.data.Keys.CopyTo (names, 0);
				System.Array.Sort (names);
				return names;
			}

			public object GetValue(string name)
			{
				return this.data[name];
			}

			public void SetValue(string name, object value, ValueStoreSetMode mode)
			{
				this.data[name] = value;
			}

			#endregion

			Dictionary<string, object> data = new Dictionary<string, object> ();
		}
	}
}
