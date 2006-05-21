//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	[TestFixture] public class DataSourceCollectionTest
	{
		[Test]
		public void CheckCreation()
		{
			DataSourceCollection collection = new DataSourceCollection ();
			
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
			Assert.AreEqual (source1.ObjectType, structuredType.GetFieldTypeObject ("A"));

			foreach (string name in collection.GetFieldNames ())
			{
				System.Console.Out.WriteLine ("Name: {0}", name);

				IStructuredData data = collection.GetDataSource (name);
				object          type = TypeRosetta.GetTypeObjectFromValue (data);
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

				foreach (string subPath in tree.GetFieldNames ())
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
		public void CheckPanelSerializationContext()
		{
			Panel panel = new UI.Panel ();
			DataSourceCollection collection = new DataSourceCollection ();

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

			panel.FillSerializationContext (context);

			Assert.AreEqual (1, context.ExternalMap.TagCount);
			Assert.AreEqual ("DataSource", Collection.ToArray<string> (context.ExternalMap.RecordedTags)[0]);
			Assert.AreEqual (collection, context.ExternalMap.GetValue ("DataSource"));

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
		public void CheckPanelDataSourceBinding()
		{
			Panel panel = new UI.Panel ();
			DataSourceCollection collection = new DataSourceCollection ();
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.AddField ("Label", new StringType ());
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
		public void CheckGetFieldTypeObject()
		{
			DataSourceCollection collection = new DataSourceCollection ();

			Widgets.Visual source1 = new Widgets.Visual ();
			MySimpleDataSource source2 = new MySimpleDataSource ();

			source2.SetValue ("Name", "Petrus");
			source2.SetValue ("BirthDateYear", 1972);

			collection.AddDataSource ("B", source2);
			collection.AddDataSource ("A", source1);

			IStructuredType type = collection as IStructuredType;

			Assert.IsNotNull (type);
			
			Assert.AreEqual (DependencyObjectType.FromSystemType (typeof (Widgets.Visual)), StructuredTree.GetFieldType (type, "A"));
			Assert.AreEqual (typeof (DynamicStructuredType), StructuredTree.GetFieldType (type, "B").GetType ());
			Assert.AreEqual (Widgets.Visual.NameProperty, StructuredTree.GetFieldType (type, "A.Name"));
			Assert.AreEqual (typeof (string), StructuredTree.GetFieldType (type, "B.Name"));
			Assert.AreEqual (typeof (int), StructuredTree.GetFieldType (type, "B.BirthDateYear"));

			Assert.AreEqual (typeof (Widgets.Visual), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetFieldType (type, "A")));
			Assert.AreEqual (null, Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetFieldType (type, "B")));
			Assert.AreEqual (typeof (string), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetFieldType (type, "A.Name")));
			Assert.AreEqual (typeof (string), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetFieldType (type, "B.Name")));
			Assert.AreEqual (typeof (int), Types.TypeRosetta.GetSystemTypeFromTypeObject (StructuredTree.GetFieldType (type, "B.BirthDateYear")));
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckSetValueEx1()
		{
			DataSourceCollection collection = new DataSourceCollection ();

			Widgets.Visual source1 = new Widgets.Visual ();

			collection.AddDataSource ("A", source1);
			StructuredTree.SetValue (collection, "A", source1);
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckSetValueEx2()
		{
			DataSourceCollection collection = new DataSourceCollection ();

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

			public string[] GetValueNames()
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

			public void SetValue(string name, object value)
			{
				this.data[name] = value;
			}

			#endregion

			Dictionary<string, object> data = new Dictionary<string, object> ();
		}
	}
}
