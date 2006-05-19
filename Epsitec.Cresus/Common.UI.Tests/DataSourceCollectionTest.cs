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

			collection.SetValue ("A.Name", "Source1");
			collection.SetValue ("B.x", 1);
			collection.SetValue ("B.y", "foo");

			Assert.AreEqual ("Source1", collection.GetValue ("A.Name"));
			Assert.AreEqual (-1, collection.GetValue ("A.Index"));
			Assert.AreEqual (1, collection.GetValue ("B.x"));
			Assert.AreEqual ("foo", collection.GetValue ("B.y"));

			Assert.AreEqual (source1.ObjectType, collection.GetFieldTypeObject ("A"));

			foreach (string name in collection.GetFieldNames ())
			{
				System.Console.Out.WriteLine ("Name: {0}", name);

				IStructuredData data = collection.GetDataSource (name);
				IStructuredTree tree = data as IStructuredTree;

				Assert.IsNotNull (data);

				if (tree != null)
				{
					Assert.AreNotEqual ("B", name);
					
					System.Text.StringBuilder buffer1 = new System.Text.StringBuilder ();
					System.Text.StringBuilder buffer2 = new System.Text.StringBuilder ();

					foreach (string subPath in collection.GetFieldPaths (name))
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
				else
				{
					Assert.AreNotEqual ("A", name);
				}
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

			Assert.AreEqual (source1, collection.GetValue ("A"));
			Assert.AreEqual (source2, collection.GetValue ("B"));

			Types.Serialization.Context context = new Types.Serialization.Context ();

			panel.FillSerializationContext (context);

			Assert.AreEqual (1, context.ExternalMap.TagCount);
			Assert.AreEqual ("DataSource", Collection.ToArray<string> (context.ExternalMap.RecordedTags)[0]);
			Assert.AreEqual (collection, context.ExternalMap.GetValue ("DataSource"));

			Widgets.Button b1 = new Epsitec.Common.Widgets.Button ();
			Binding binding = new Binding (BindingMode.OneWay, null, "A.Name");
			
			b1.Dock = Widgets.DockStyle.Top;
			b1.SetBinding (Widgets.Visual.NameProperty, binding);

			panel.Children.Add (b1);

			Assert.AreEqual (source1.Name, b1.Name);

			source1.Name = "X";

			Assert.AreEqual ("X", source1.Name);
			Assert.AreEqual ("X", b1.Name);

			b1.Name = "Y";
			
			Assert.AreEqual ("X", source1.Name);
			Assert.AreEqual ("Y", b1.Name);
		}

		[Test]
		public void CheckGetValueType()
		{
			DataSourceCollection collection = new DataSourceCollection ();

			Widgets.Visual source1 = new Widgets.Visual ();
			MySimpleDataSource source2 = new MySimpleDataSource ();

			source2.SetValue ("Name", "Petrus");
			source2.SetValue ("BirthDateYear", 1972);

			collection.AddDataSource ("B", source2);
			collection.AddDataSource ("A", source1);
			
			Assert.AreEqual (DependencyObjectType.FromSystemType (typeof (Widgets.Visual)), collection.GetValueTypeObject ("A"));
			Assert.AreEqual (typeof (MySimpleDataSource), collection.GetValueTypeObject ("B"));
			Assert.AreEqual (Widgets.Visual.NameProperty, collection.GetValueTypeObject ("A.Name"));
			Assert.AreEqual (typeof (string), collection.GetValueTypeObject ("B.Name"));
			Assert.AreEqual (typeof (int), collection.GetValueTypeObject ("B.BirthDateYear"));

			Assert.AreEqual (typeof (Widgets.Visual), Types.TypeRosetta.GetSystemTypeFromTypeObject (collection.GetValueTypeObject ("A")));
			Assert.AreEqual (typeof (MySimpleDataSource), Types.TypeRosetta.GetSystemTypeFromTypeObject (collection.GetValueTypeObject ("B")));
			Assert.AreEqual (typeof (string), Types.TypeRosetta.GetSystemTypeFromTypeObject (collection.GetValueTypeObject ("A.Name")));
			Assert.AreEqual (typeof (string), Types.TypeRosetta.GetSystemTypeFromTypeObject (collection.GetValueTypeObject ("B.Name")));
			Assert.AreEqual (typeof (int), Types.TypeRosetta.GetSystemTypeFromTypeObject (collection.GetValueTypeObject ("B.BirthDateYear")));
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckSetValueEx1()
		{
			DataSourceCollection collection = new DataSourceCollection ();

			Widgets.Visual source1 = new Widgets.Visual ();

			collection.AddDataSource ("A", source1);
			collection.SetValue ("A", source1);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckSetValueEx2()
		{
			DataSourceCollection collection = new DataSourceCollection ();

			Widgets.Visual source1 = new Widgets.Visual ();

			collection.SetValue ("A", source1);
		}

		private class MySimpleDataSource : IStructuredData
		{
			public MySimpleDataSource()
			{
			}
			
			#region IStructuredData Members

			public void AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public void DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public object GetValue(string path)
			{
				return this.data[path];
			}

			public object GetValueTypeObject(string path)
			{
				return TypeRosetta.GetTypeObjectFromValue (this.data[path]);
			}

			public void SetValue(string path, object value)
			{
				this.data[path] = value;
			}

			public bool HasImmutableRoots
			{
				get
				{
					return true;
				}
			}

			#endregion

			Dictionary<string, object> data = new Dictionary<string, object> ();
		}
	}
}
