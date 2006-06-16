using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.SerializationTest.MyItem))]
[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.SerializationTest.MySimpleObject))]

namespace Epsitec.Common.Types
{
	public delegate object MyAllocator();
	
	[TestFixture] public class SerializationTest
	{
		[SetUp]
		public void Initialise()
		{
			Serialization.DependencyClassManager.Setup ();
		}

		[Test]
		public void CheckAllocationSpeed()
		{
			System.Type type = typeof (MySimpleObject);
			int steps = 1000*100;

			System.Reflection.Emit.DynamicMethod dm = new System.Reflection.Emit.DynamicMethod ("MyCtor", type, System.Type.EmptyTypes, typeof (SerializationTest).Module, true);
			System.Reflection.Emit.ILGenerator ilgen = dm.GetILGenerator ();
			
			ilgen.Emit (System.Reflection.Emit.OpCodes.Nop);
			ilgen.Emit (System.Reflection.Emit.OpCodes.Newobj, type.GetConstructor (System.Type.EmptyTypes));
			ilgen.Emit (System.Reflection.Emit.OpCodes.Ret);

			MyAllocator allocator = (MyAllocator) dm.CreateDelegate (typeof (MyAllocator));

			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();

			stopwatch.Start ();

			for (int i = 0; i < steps; i++)
			{
				//	allocator: 0.45 μs
				allocator ();
			}

			stopwatch.Stop ();
			System.Console.Out.WriteLine ("DynamicMethod + ILGen: {0} μs, {1} executions.", 1.0M * stopwatch.ElapsedMilliseconds / (steps / 1000), steps);

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < steps; i++)
			{
				//	Activator: 17.2 μs (38 x plus lent que activator)
				System.Activator.CreateInstance (type);
			}

			stopwatch.Stop ();
			System.Console.Out.WriteLine ("Activator.CreateInstance: {0} μs, {1} executions.", 1.0M * stopwatch.ElapsedMilliseconds / (steps / 1000), steps);

			DependencyObjectType depType = DependencyObjectType.FromSystemType (type);
			depType.CreateEmptyObject ();

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < steps; i++)
			{
				//	Même méthode que l'allocator ci-dessus...
				depType.CreateEmptyObject ();
			}

			stopwatch.Stop ();
			System.Console.Out.WriteLine ("CreateEmptyObject: {0} μs, {1} executions.", 1.0M * stopwatch.ElapsedMilliseconds / (steps / 1000), steps);
		}

		[Test]
		public void CheckAttributes()
		{
			List<string> names = new List<string> ();

			foreach (System.Type type in DependencyClassAttribute.GetRegisteredTypes (this.GetType ().Assembly))
			{
				names.Add (type.FullName);
			}
			
			string[] array = new string[] { "Epsitec.Common.Types.SerializationTest+MyItem", "Epsitec.Common.Types.SerializationTest+MySimpleObject" };

			Assert.IsTrue (Collection.ContainsAll (names, array));
		}

		[Test]
		public void CheckDependencyClassManager()
		{
			DependencyObjectType t1 = Serialization.DependencyClassManager.Current.FindObjectType ("Epsitec.Common.Types.SerializationTest+MyItem");
			DependencyObjectType t2 = Serialization.DependencyClassManager.Current.FindObjectType ("Epsitec.Common.Types.SerializationTest+MySimpleObject");

			Assert.AreEqual (typeof (MyItem), t1.SystemType);
			Assert.AreEqual (typeof (MySimpleObject), t2.SystemType);
		}

		[Test]
		public void CheckMapId()
		{
			Serialization.Generic.MapId<DependencyObject> map = new Serialization.Generic.MapId<DependencyObject> ();

			Assert.AreEqual (0, map.GetNullId ());
			Assert.AreEqual (0, map.GetId (null));
			Assert.IsNull (map.GetValue (0));
			Assert.AreEqual (1, Collection.Count (map.RecordedValues));
			Assert.IsTrue (map.IsValueDefined (null));

			map.Record (null);
			
			Assert.AreEqual (1, Collection.Count (map.RecordedValues));

			MyItem a = new MyItem ();
			MyItem b = new MyItem ();
			MyItem c = new MyItem ();

			MySimpleObject s1 = new MySimpleObject ();
			MySimpleObject s2 = new MySimpleObject ();
			
			map.Record (a);

			Assert.AreEqual (1, map.GetId (a));
			Assert.AreEqual (a, map.GetValue (1));
			Assert.AreEqual (2, Collection.Count (map.RecordedValues));
			Assert.IsTrue (map.IsValueDefined (a));
			Assert.IsTrue (map.IsIdDefined (1));
			Assert.IsFalse (map.IsValueDefined (b));
			Assert.IsFalse (map.IsIdDefined (2));
			Assert.AreEqual (2, Collection.Count (map.RecordedTypes));
			Assert.AreEqual (typeof (MyItem), Collection.ToList (map.RecordedTypes)[1]);
			Assert.AreEqual (typeof (MyItem), map.GetType (1));
			Assert.AreEqual (-1, map.GetId (b));

			map.Record (b);
			map.Record (b);

			Assert.AreEqual (2, map.GetId (b));
			Assert.AreEqual (b, map.GetValue (2));
			Assert.AreEqual (3, Collection.Count (map.RecordedValues));

			map.Record (s1);
			map.Record (c);
			map.Record (s2);

			Assert.AreEqual (3, map.GetId (s1));
			Assert.AreEqual (4, map.GetId (c));
			Assert.AreEqual (5, map.GetId (s2));

			Assert.AreEqual (3, map.TypeCount);
			Assert.AreEqual (3, Collection.Count (map.RecordedTypes));
			Assert.AreEqual (typeof (MyItem), Collection.ToList (map.RecordedTypes)[1]);
			Assert.AreEqual (typeof (MySimpleObject), Collection.ToList (map.RecordedTypes)[2]);
			Assert.IsNull (map.GetType (0));
			Assert.AreEqual (typeof (MyItem), map.GetType (1));
			Assert.AreEqual (typeof (MySimpleObject), map.GetType (2));

			Assert.AreEqual (6, map.ValueCount);
			Assert.AreEqual (3, Collection.Count (map.GetValues (typeof (MyItem))));
			Assert.AreEqual (2, Collection.Count (map.GetValues (typeof (MySimpleObject))));

			Assert.AreEqual (a, Collection.ToList (map.GetValues (typeof (MyItem)))[0]);
			Assert.AreEqual (b, Collection.ToList (map.GetValues (typeof (MyItem)))[1]);
			Assert.AreEqual (c, Collection.ToList (map.GetValues (typeof (MyItem)))[2]);
			
			Assert.AreEqual (s1, Collection.ToList (map.GetValues (typeof (MySimpleObject)))[0]);
			Assert.AreEqual (s2, Collection.ToList (map.GetValues (typeof (MySimpleObject)))[1]);
		}

		[Test]
		[ExpectedException (typeof (System.Collections.Generic.KeyNotFoundException))]
		public void CheckMapIdEx1()
		{
			Serialization.Generic.MapId<DependencyObject> map = new Serialization.Generic.MapId<DependencyObject> ();
			
			map.GetValue (1);
		}

		[Test]
		public void CheckMapTag()
		{
			Serialization.Generic.MapTag<DependencyObject> map = new Serialization.Generic.MapTag<DependencyObject> ();

			Assert.AreEqual (0, map.TagCount);
			Assert.AreEqual (0, map.ValueCount);

			MyItem a = new MyItem ();
			MyItem b = new MyItem ();

			a.Name = "a";
			b.Name = "b";

			map.Record ("a", a);
			map.Record ("b", b);

			Assert.AreEqual (2, map.TagCount);
			Assert.AreEqual (2, map.ValueCount);

			Assert.AreEqual (a, map.GetValue ("a"));
			Assert.AreEqual (b, map.GetValue ("b"));
			Assert.IsNull (map.GetValue ("c"));
			Assert.IsTrue (map.IsTagDefined ("a"));
			Assert.IsTrue (map.IsTagDefined ("b"));
			Assert.IsFalse (map.IsTagDefined ("c"));
			Assert.AreEqual ("a", map.GetTag (a));
			Assert.AreEqual ("b", map.GetTag (b));
			Assert.AreEqual (a, Collection.ToList (map.RecordedValues)[0]);
			Assert.AreEqual (b, Collection.ToList (map.RecordedValues)[1]);
			Assert.AreEqual ("a", Collection.ToList (map.RecordedTags)[0]);
			Assert.AreEqual ("b", Collection.ToList (map.RecordedTags)[1]);

			Assert.AreEqual (0, Collection.Count (map.GetUsedValues ()));
			Assert.AreEqual (2, Collection.Count (map.GetUnusedValues ()));
			Assert.AreEqual (0, map.GetValueUseCount (a));
			Assert.AreEqual (0, map.GetValueUseCount (b));
			Assert.AreEqual (a, Collection.ToList (map.GetUnusedValues ())[0]);
			Assert.AreEqual (b, Collection.ToList (map.GetUnusedValues ())[1]);

			map.IncrementUseValue (a);

			Assert.AreEqual (1, Collection.Count (map.GetUsedValues ()));
			Assert.AreEqual (1, Collection.Count (map.GetUnusedValues ()));
			Assert.AreEqual (1, map.GetValueUseCount (a));
			Assert.AreEqual (0, map.GetValueUseCount (b));
			Assert.AreEqual (a, Collection.ToList (map.GetUsedValues ())[0]);
			Assert.AreEqual (b, Collection.ToList (map.GetUnusedValues ())[0]);
			
			map.IncrementUseValue (a);
			map.IncrementUseValue (a);
			map.IncrementUseValue (a);

			Assert.AreEqual (1, Collection.Count (map.GetUsedValues ()));
			Assert.AreEqual (1, Collection.Count (map.GetUnusedValues ()));
			Assert.AreEqual (4, map.GetValueUseCount (a));
			Assert.AreEqual (0, map.GetValueUseCount (b));
			Assert.AreEqual (a, Collection.ToList (map.GetUsedValues ())[0]);
			Assert.AreEqual (b, Collection.ToList (map.GetUnusedValues ())[0]);

			map.ClearUseCount ();
			
			Assert.AreEqual (0, Collection.Count (map.GetUsedValues ()));
			Assert.AreEqual (2, Collection.Count (map.GetUnusedValues ()));
			Assert.AreEqual (0, map.GetValueUseCount (a));
			Assert.AreEqual (0, map.GetValueUseCount (b));
			Assert.AreEqual (a, Collection.ToList (map.GetUnusedValues ())[0]);
			Assert.AreEqual (b, Collection.ToList (map.GetUnusedValues ())[1]);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckMapTagEx1()
		{
			Serialization.Generic.MapTag<DependencyObject> map = new Serialization.Generic.MapTag<DependencyObject> ();

			MyItem a = new MyItem ();
			MyItem b = new MyItem ();

			map.Record ("x", a);
			map.Record ("x", b);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckMapTagEx2()
		{
			Serialization.Generic.MapTag<DependencyObject> map = new Serialization.Generic.MapTag<DependencyObject> ();

			MyItem x = new MyItem ();
			
			map.Record ("a", x);
			map.Record ("b", x);
		}

		[Test]
		public void CheckMarkupExtension()
		{
			string[] args;

			args = Serialization.MarkupExtension.Explode ("{abc}");

			Assert.AreEqual (1, args.Length);
			Assert.AreEqual ("abc", args[0]);

			args = Serialization.MarkupExtension.Explode ("{abc   }");

			Assert.AreEqual (1, args.Length);
			Assert.AreEqual ("abc", args[0]);

			args = Serialization.MarkupExtension.Explode ("{   abc}");

			Assert.AreEqual (1, args.Length);
			Assert.AreEqual ("abc", args[0]);

			args = Serialization.MarkupExtension.Explode ("{abc,def}");

			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("def", args[1]);

			args = Serialization.MarkupExtension.Explode ("{abc, def}");

			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("def", args[1]);

			args = Serialization.MarkupExtension.Explode ("{abc ,def}");

			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("def", args[1]);

			args = Serialization.MarkupExtension.Explode ("{  abc  ,  def  }");

			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("def", args[1]);

			args = Serialization.MarkupExtension.Explode ("{abc,}");

			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("", args[1]);

			args = Serialization.MarkupExtension.Explode ("{abc  ,  }");

			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("", args[1]);

			args = Serialization.MarkupExtension.Explode ("{,abc}");

			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("", args[0]);
			Assert.AreEqual ("abc", args[1]);

			args = Serialization.MarkupExtension.Explode ("{abc,,def}");

			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("", args[1]);
			Assert.AreEqual ("def", args[2]);

			args = Serialization.MarkupExtension.Explode ("{abc , , def}");

			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("", args[1]);
			Assert.AreEqual ("def", args[2]);

			args = Serialization.MarkupExtension.Explode ("{abc,,}");

			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("", args[1]);
			Assert.AreEqual ("", args[2]);

			args = Serialization.MarkupExtension.Explode ("{abc , ,  }");

			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("abc", args[0]);
			Assert.AreEqual ("", args[1]);
			Assert.AreEqual ("", args[2]);

			args = Serialization.MarkupExtension.Explode ("{,,}");

			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("", args[0]);
			Assert.AreEqual ("", args[1]);
			Assert.AreEqual ("", args[2]);

			args = Serialization.MarkupExtension.Explode ("{ , , }");

			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("", args[0]);
			Assert.AreEqual ("", args[1]);
			Assert.AreEqual ("", args[2]);

			args = Serialization.MarkupExtension.Explode ("{   }");

			Assert.AreEqual (0, args.Length);

			args = Serialization.MarkupExtension.Explode ("{}");

			Assert.AreEqual (0, args.Length);
			
			args = Serialization.MarkupExtension.Explode ("{ {x}, {}, x={a{b}c} }");

			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("{x}", args[0]);
			Assert.AreEqual ("{}", args[1]);
			Assert.AreEqual ("x={a{b}c}", args[2]);

		}

		[Test]
		public void CheckVisitSerializableNodes()
		{
			MyItem a = new MyItem ();
			MyItem b = new MyItem ();
			MyItem q = new MyItem ();
			MyItem c1 = new MyItem ();
			MyItem c2 = new MyItem ();

			a.AddChild (b);
			a.AddChild (q);
			b.AddChild (c1);
			b.AddChild (c2);

			a.Name = "a";
			b.Name = "b";
			q.Name = "q";
			c1.Name = "c1";
			c2.Name = "c2";

			a.Value = "A";
			b.Value = "B";
			q.Value = "Q";
			c1.Value = "C1";
			c2.Value = "C2";
			
			//	a --+--> b --+--> c1
			//	    |        +--> c2
			//	    +--> q

			Serialization.Context context = new Serialization.Context ();
			Serialization.Generic.MapId<DependencyObject> objectMap = context.ObjectMap;
			
			Serialization.GraphVisitor.VisitSerializableNodes (a, context);

			List<DependencyObject> objects = Collection.ToList (objectMap.RecordedValues);
			List<System.Type> types = Collection.ToList (objectMap.RecordedTypes);

			Assert.AreEqual (6, objectMap.ValueCount);
			Assert.AreEqual (2, objectMap.TypeCount);
			Assert.AreEqual (1, context.UnknownMap.ValueCount);
			
			Assert.AreEqual (6, objects.Count);
			Assert.IsNull (objects[0]);
			Assert.AreEqual (a, objects[1]);
			Assert.AreEqual (b, objects[2]);
			Assert.AreEqual (c1, objects[3]);
			Assert.AreEqual (c2, objects[4]);
			Assert.AreEqual (q, objects[5]);
			Assert.AreEqual (q, objectMap.GetValue (5));

			Assert.AreEqual (2, types.Count);
			Assert.IsNull (types[0]);
			Assert.AreEqual (typeof (MyItem), types[1]);
			Assert.AreEqual (typeof (MyItem), objectMap.GetType (1));

			object xxx = new object ();
			
			Binding binding = new Binding ();
			binding.Source = xxx;
			binding.Path = "Abc";

			c1.SetBinding (MyItem.FriendProperty, binding);

			context = new Serialization.Context ();

			Serialization.GraphVisitor.VisitSerializableNodes (a, context);

			Assert.AreEqual (2, context.UnknownMap.ValueCount);
			Assert.AreEqual (xxx, context.UnknownMap.GetValue (1));
		}

		[Test]
		public void CheckSerializeToXml()
		{
			MyItem ext;
			MyItem root = this.CreateSampleTree (out ext);

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);
			
			xmlWriter.Indentation = 2;
			xmlWriter.IndentChar = ' ';
			xmlWriter.Formatting = System.Xml.Formatting.Indented;
			xmlWriter.WriteStartDocument (true);
			xmlWriter.WriteStartElement ("root");

			Serialization.Context context = new Serialization.SerializerContext (new Serialization.IO.XmlWriter (xmlWriter));

			context.ExternalMap.Record ("ext", ext);

			MyItem b = DependencyObjectTree.FindChild (root, "b") as MyItem;
			MyItem m = new MyItem ();

			m.Name = "m";
			m.Value = "M";
			m.Price = 12.60M;
			m.Friend = ext;

			Assert.IsNotNull (b);
			
			Storage.Serialize (root, context);
			Storage.Serialize (b, context);
			Storage.Serialize (m, context);

			xmlWriter.WriteEndElement ();
			xmlWriter.WriteEndDocument ();
			xmlWriter.Flush ();
			xmlWriter.Close ();

			Assert.AreEqual ("_2.DataContext", context.GetPropertyName (DataObject.DataContextProperty));
			Assert.AreEqual (DataObject.DataContextProperty, context.GetProperty (root, "_2.DataContext"));

			System.Console.Out.WriteLine (buffer.ToString ());
		}

		[Test]
		public void CheckDeserializeFromXml()
		{
			MyItem ext;
			MyItem root = this.CreateSampleTree (out ext);

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			xmlWriter.Indentation = 2;
			xmlWriter.IndentChar = ' ';
			xmlWriter.Formatting = System.Xml.Formatting.Indented;
			xmlWriter.WriteStartDocument (true);
			xmlWriter.WriteStartElement ("root");

			Serialization.Context context = new Serialization.SerializerContext (new Serialization.IO.XmlWriter (xmlWriter));

			context.ExternalMap.Record ("ext", ext);

			MyItem b = DependencyObjectTree.FindChild (root, "b") as MyItem;
			MyItem m = new MyItem ();

			m.Name = "m";
			m.Value = "M";
			m.Price = 12.60M;
			m.Friend = ext;

			Storage.Serialize (root, context);
			Storage.Serialize (b, context);
			Storage.Serialize (m, context);

			xmlWriter.WriteEndElement ();
			xmlWriter.WriteEndDocument ();
			xmlWriter.Flush ();
			xmlWriter.Close ();

			System.IO.StringReader stringReader = new System.IO.StringReader (buffer.ToString ());
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			while (xmlReader.Read ())
			{
				if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
					(xmlReader.LocalName == "root"))
				{
					break;
				}
			}

			context = new Serialization.DeserializerContext (new Serialization.IO.XmlReader (xmlReader));

			context.ExternalMap.Record ("ext", ext);

			MyItem readRoot = Storage.Deserialize (context) as MyItem;

			Assert.AreEqual (3, context.ObjectMap.TypeCount);
			Assert.AreEqual (typeof (MyItem), context.ObjectMap.GetType (1));
			Assert.AreEqual (typeof (DataObject), context.ObjectMap.GetType (2));

			Assert.AreEqual (7, context.ObjectMap.ValueCount);

			root = context.ObjectMap.GetValue (1) as MyItem;
			b    = DependencyObjectTree.FindChild (root, "b") as MyItem;

			Assert.AreEqual ("a", root.Name);
			Assert.AreEqual ("A", root.Value);
			Assert.AreEqual (3, root.Children.Count);
			Assert.AreEqual ("b", root.Children[0].Name);
			Assert.AreEqual ("q", root.Children[1].Name);
			Assert.AreEqual ("r", root.Children[2].Name);

			Assert.AreEqual (4, root.Children[2].Labels.Count);
			Assert.AreEqual ("First", root.Children[2].Labels[0]);
			Assert.AreEqual ("Second", root.Children[2].Labels[1]);
			Assert.AreEqual (@"Third & last -- }, {;/<\'"" ", root.Children[2].Labels[2]);
			Assert.IsNull (root.Children[2].Labels[3]);

			Assert.AreEqual ("c1", root.Children[0].Children[0].Name);
			Assert.AreEqual ("c2", root.Children[0].Children[1].Name);

			Assert.AreEqual (10, root.Children[0].Children[1].SomeStruct.Value);
			Assert.AreEqual ("km", root.Children[0].Children[1].SomeStruct.Unit);
			
			Assert.AreEqual (3, context.ObjectMap.GetId (b));
			Assert.AreEqual (3899.20M, root.Friend.Price);
			
			MyItem readB = Storage.Deserialize (context) as MyItem;

			Assert.AreEqual (b, readB);
			
			MyItem readM = Storage.Deserialize (context) as MyItem;

			Assert.AreEqual ("m", readM.Name);
			Assert.AreEqual (ext, root.Friend.Friend);
		}
		
		private MyItem CreateSampleTree(out MyItem ext)
		{
			ext = new MyItem ();
			ext.Name = "ext";
			ext.Value = "EXT";

			MyItem a = new MyItem ();
			MyItem b = new MyItem ();
			MyItem q = new MyItem ();
			MyItem r = new MyItem ();
			MyItem c1 = new MyItem ();
			MyItem c2 = new MyItem ();

			a.AddChild (b);
			a.AddChild (q);
			a.AddChild (r);
			b.AddChild (c1);
			b.AddChild (c2);

			a.Name = "a";
			b.Name = "b";
			q.Name = "q";
			r.Name = "r";
			c1.Name = "c1";
			c2.Name = "c2";

			c2.SomeStruct = new SomeStruct (10, "km");
			
			a.Value = "A";
			b.Value = "B";
			q.Value = "Q{<\"&>}";
			r.Value = "R";
			c1.Value = "C1";
			c2.Value = "C2";

			//	c2       q
			//	:        :
			//	friend   friend
			//	:        :
			//	a --+--> b --+--> c1
			//	    |        +--> c2 ...friend... ext
			//	    +--> q ...friend... b
			//		+--> r

			a.Friend = c2;
			q.Friend = b;
			b.Friend = q;

			r.Labels.Add ("First");
			r.Labels.Add ("Second");
			r.Labels.Add (@"Third & last -- }, {;/<\'"" ");
			r.Labels.Add (null);

			c1.Price = 125.95M;
			c2.Price = 3899.20M;

			c2.Friend = ext;

			Binding bindingQ = new Binding ();
			Binding bindingR = new Binding ();
			Binding dataContext = new Binding ();
			
			bindingQ.Source = a;
			bindingQ.Path = "Friend.Price"; //("Children[0].Children[1]");
			bindingQ.Mode = BindingMode.OneWay;

			bindingR.Mode = BindingMode.OneWay;

			dataContext.Source = c1;
			
			DataObject.SetDataContext (a, dataContext);

			Assert.AreEqual (dataContext, DataObject.GetDataContext (a));
			Assert.AreEqual (dataContext, DataObject.GetDataContext (b));
			Assert.AreEqual (dataContext, DataObject.GetDataContext (r));

			q.SetBinding (MyItem.PriceProperty, bindingQ);
			r.SetBinding (MyItem.FriendProperty, bindingR);

			Assert.AreEqual (c2.Price, q.Price);
			Assert.AreEqual (c1, r.Friend);
			
			return a;
		}
		
		#region Class EventHandlerSupport
		private class EventHandlerSupport
		{
			public string Log
			{
				get
				{
					return this.buffer.ToString ();
				}
			}

			public void RecordEvent(object sender, DependencyPropertyChangedEventArgs e)
			{
				this.buffer.Append (e.PropertyName);
				this.buffer.Append (":");
				this.buffer.Append (e.OldValue);
				this.buffer.Append (",");
				this.buffer.Append (e.NewValue);
				this.buffer.Append (".");
			}
			public void RecordEventAndName(object sender, DependencyPropertyChangedEventArgs e)
			{
				this.buffer.Append (DependencyObjectTree.GetName (sender as DependencyObject));
				this.buffer.Append ("-");
				this.buffer.Append (e.PropertyName);
				this.buffer.Append (":");
				this.buffer.Append (e.OldValue);
				this.buffer.Append (",");
				this.buffer.Append (e.NewValue);
				this.buffer.Append (".");
			}
			public void Clear()
			{
				this.buffer.Length = 0;
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
		}
		#endregion

		#region MyItem Class

		public class MyItem : DependencyObject
		{
			public MyItem()
			{
			}
			
			public string						Name
			{
				get
				{
					return this.GetValue (MyItem.NameProperty) as string;
				}
				set
				{
					this.SetValue (MyItem.NameProperty, value);
				}
			}
			public MyItem						Parent
			{
				get
				{
					return this.parent;
				}
			}
			public ChildrenCollection			Children
			{
				get
				{
					return this.children;
				}
			}
			public bool							HasChildren
			{
				get
				{
					return this.children == null ? false : (this.children.Count > 0);
				}
			}
			public string						Value
			{
				get
				{
					return this.GetValue (MyItem.ValueProperty) as string;
				}
				set
				{
					this.SetValue (MyItem.ValueProperty, value);
				}
			}
			public string						Cascade
			{
				get
				{
					return this.GetValue (MyItem.CascadeProperty) as string;
				}
				set
				{
					this.SetValue (MyItem.CascadeProperty, value);
				}
			}
			public MyItem						Friend
			{
				get
				{
					return this.GetValue (MyItem.FriendProperty) as MyItem;
				}
				set
				{
					this.SetValue (MyItem.FriendProperty, value);
				}
			}
			public decimal						Price
			{
				get
				{
					return (decimal) this.GetValue (MyItem.PriceProperty);
				}
				set
				{
					this.SetValue (MyItem.PriceProperty, value);
				}
			}
			public SomeStruct					SomeStruct
			{
				get
				{
					return (SomeStruct) this.GetValue (MyItem.SomeStructProperty);
				}
				set
				{
					this.SetValue (MyItem.SomeStructProperty, value);
				}
			}
			public IList<string>				Labels
			{
				get
				{
					if (this.labels == null)
					{
						this.labels = new List<string> ();
					}
					return this.labels;
				}
			}
			
			public void AddChild(MyItem item)
			{
				DependencyObjectTreeSnapshot snapshot = DependencyObjectTree.CreateInheritedPropertyTreeSnapshot (item);
				
				if (this.children == null)
				{
					this.children = new ChildrenCollection ();
				}
				if (item.parent != null)
				{
					item.parent.children.Remove (item);
				}
				this.children.Add (item);

				item.parent = this;
				
				snapshot.AddNewInheritedProperties (item);
				snapshot.InvalidateDifferentProperties ();
			}
			
			public static object GetParentValue(DependencyObject o)
			{
				MyItem tt = o as MyItem;
				return tt.Parent;
			}
			public static object GetChildrenValue(DependencyObject o)
			{
				MyItem tt = o as MyItem;
				if (tt.children == null)
				{
					tt.children = new ChildrenCollection ();
				}
				return tt.children;
			}
			public static object GetHasChildrenValue(DependencyObject o)
			{
				MyItem tt = o as MyItem;
				return tt.HasChildren;
			}
			public static object GetLabelsValue(DependencyObject o)
			{
				MyItem tt = o as MyItem;
				return tt.Labels;
			}

			public static DependencyProperty NameProperty = DependencyObjectTree.NameProperty.AddOwner (typeof (MyItem));
			public static DependencyProperty ParentProperty = DependencyObjectTree.ParentProperty.AddOwner (typeof (MyItem), new DependencyPropertyMetadata (MyItem.GetParentValue));
			public static DependencyProperty ChildrenProperty = DependencyObjectTree.ChildrenProperty.AddOwner (typeof (MyItem), new DependencyPropertyMetadata (MyItem.GetChildrenValue).MakeReadOnlySerializable ());
			public static DependencyProperty HasChildrenProperty = DependencyObjectTree.HasChildrenProperty.AddOwner (typeof (MyItem), new DependencyPropertyMetadata (MyItem.GetHasChildrenValue));
			public static DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (string), typeof (MyItem));
			public static DependencyProperty CascadeProperty = DependencyProperty.Register ("Cascade", typeof (string), typeof (MyItem), new DependencyPropertyMetadataWithInheritance (UndefinedValue.Instance));
			public static DependencyProperty FriendProperty = DependencyProperty.Register ("Friend", typeof (MyItem), typeof (MyItem));
			public static DependencyProperty PriceProperty = DependencyProperty.Register ("Price", typeof (decimal), typeof (MyItem));
			public static DependencyProperty SomeStructProperty = DependencyProperty.Register ("SomeStruct", typeof (SomeStruct), typeof (MyItem));
			public static DependencyProperty LabelsProperty = DependencyProperty.RegisterReadOnly ("Labels", typeof (IList<string>), typeof (MyItem), new DependencyPropertyMetadata (MyItem.GetLabelsValue).MakeReadOnlySerializable ());

			MyItem parent;
			ChildrenCollection children;
			List<string> labels;
		}
		
		#endregion

		[SerializationConverter (typeof (SomeStruct.SerializationConverter))]
		public struct SomeStruct
		{
			public SomeStruct(int value, string unit)
			{
				this.value = value;
				this.unit = unit;
			}

			public int Value
			{
				get
				{
					return this.value;
				}
			}

			public string Unit
			{
				get
				{
					return this.unit;
				}
			}

			#region SerializationConverter Class

			public class SerializationConverter : ISerializationConverter
			{
				#region ISerializationConverter Members

				public string ConvertToString(object value, IContextResolver context)
				{
					SomeStruct s = (SomeStruct) value;
					return string.Format ("{0};{1}", s.value, s.unit);
				}

				public object ConvertFromString(string value, IContextResolver context)
				{
					string[] args = value.Split (';');
					
					return new SomeStruct (int.Parse (args[0]), args[1]);
				}

				#endregion
			}

			#endregion
			
			private int value;
			private string unit;
		}

		#region ChildrenCollection Class
		public class ChildrenCollection : DependencyObjectList<MyItem>
		{
		}
		#endregion

		#region MySimpleObject Class

		internal class MySimpleObject : DependencyObject
		{
			public MySimpleObject()
			{
			}
		}

		#endregion
	}
}
