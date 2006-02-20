using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Common.Types
{
	[TestFixture] public class ObjectTest
	{
		[System.Runtime.InteropServices.DllImport("Kernel32.dll")] private static extern System.IntPtr LoadLibrary(string fullpath);
		
		[SetUp] public void Initialise()
		{
			ObjectTest.LoadLibrary (@"s:\Epsitec.Cresus\External\AntiGrain.Win32.dll");
		}


		[Test]
		public void CheckPropertyPath()
		{
			PropertyPath pp1 = new PropertyPath ();
			PropertyPath pp2 = new PropertyPath ("abc");
			PropertyPath pp3 = new PropertyPath ("{0}.{1}", MyObject.FooProperty, MyObject.NameProperty);
			PropertyPath pp4 = new PropertyPath ("Bar.{0}", MyObject.XyzProperty);

			Assert.AreEqual (null, pp1.GetFullPath ());
			Assert.AreEqual ("abc", pp2.GetFullPath ());
			Assert.AreEqual ("Foo.Name", pp3.GetFullPath ());
			Assert.AreEqual ("Bar.Xyz", pp4.GetFullPath ());
			Assert.IsTrue (pp2.Elements.IsNull);
			Assert.AreEqual (0, pp2.Elements.Length);
			Assert.AreEqual ("Foo.Name.Bar.Xyz", PropertyPath.Combine (pp3, pp4).GetFullPath ());
			Assert.AreEqual ("Bar.Xyz", PropertyPath.Combine (pp1, pp4).GetFullPath ());
			Assert.AreEqual ("abc", PropertyPath.Combine (pp2, pp1).GetFullPath ());
			Assert.AreEqual (null, PropertyPath.Combine (pp1, pp1).GetFullPath ());
		}

		[Test]
		public void CheckBinding1()
		{
			Binding bindingName = new Binding ();
			Binding bindingXyz = new Binding ();

			MyObject mySource = new MyObject ();
			MyObject myData = new MyObject ();
			MyObject myTarget = new MyObject ();

			mySource.SetValue (MyObject.NameProperty, "Jean Dupont");
			mySource.SetValue (MyObject.XyzProperty, 123);
			mySource.SetValue (MyObject.SiblingProperty, myData);

			myData.SetValue (MyObject.NameProperty, "John Doe");
			myData.SetValue (MyObject.XyzProperty, 999);

			using (bindingName.DeferChanges ())
			{
				bindingName.Mode = BindingMode.TwoWay;
				bindingName.Source = mySource;
				bindingName.Path = new PropertyPath ("Name");

				myTarget.SetBinding (MyObject.FooProperty, bindingName);
			}

			using (bindingXyz.DeferChanges ())
			{
				bindingXyz.Mode = BindingMode.TwoWay;
				bindingXyz.Source = mySource;
				bindingXyz.Path = new PropertyPath ("Sibling.Xyz");

				myTarget.SetBinding (MyObject.XyzProperty, bindingXyz);
			}

			Assert.AreEqual ("Jean Dupont", myTarget.Foo);
			Assert.AreEqual (999, myTarget.Xyz);

			mySource.Name = "Jeanne Dupont";
			myData.Xyz = 888;

			Assert.AreEqual ("Jeanne Dupont", myTarget.Foo);
			Assert.AreEqual (888, myTarget.Xyz);
		}

		[Test]
		public void CheckBinding2()
		{
			Binding bindingXyz = new Binding ();
			Binding bindingContext = new Binding ();

			MyObject mySource1 = new MyObject ();
			MyObject mySource2 = new MyObject ();
			MyObject myData1 = new MyObject ();
			MyObject myData2 = new MyObject ();
			MyObject myTarget = new MyObject ();

			mySource1.Xyz  = 1;
			mySource1.Sibling = myData1;
			
			mySource2.Xyz  = 2;
			mySource2.Sibling = myData2;

			myData1.Xyz = 999;
			myData2.Xyz = 888;

			bindingContext.Source = mySource1;
			bindingContext.Path = new PropertyPath ("Sibling");

			DataObject.SetDataContext (myTarget, bindingContext);

			bindingXyz.Mode = BindingMode.TwoWay;
			bindingXyz.Path = new PropertyPath ("Xyz");

			myTarget.SetBinding (MyObject.XyzProperty, bindingXyz);
			Assert.AreEqual (999, myTarget.Xyz);

			myData1.Xyz = 777;
			Assert.AreEqual (777, myTarget.Xyz);

			bindingContext.Source = mySource2;
			Assert.AreEqual (888, myTarget.Xyz);

			bindingContext.Source = mySource1;
			Assert.AreEqual (777, myTarget.Xyz);

			mySource1.Sibling = myData2;
			Assert.AreEqual (888, myTarget.Xyz);
		}

		[Test]
		[Ignore ("Too slow")]
		public void CheckObjectCreation()
		{
			AntiGrain.Interface.Initialise ();
			
			System.Console.WriteLine ("Performance test of AbstractWidget Properties");
			System.Console.WriteLine ("--------------------------------------100'000");
			System.Console.Out.Flush ();
			this.PrivateCreateObjects (100000);
			System.Console.WriteLine ("------------------------------------1'000'000");
			System.Console.Out.Flush ();
			this.PrivateCreateObjects (1000000);
			System.Console.WriteLine ("-----------------------------------------done");
			System.Console.Out.Flush ();
		}

		[Test]
		[Ignore ("Too slow")]
		public void CheckObjectType()
		{
			Assert.AreEqual ("Object", ObjectType.FromSystemType (typeof (Types.Object)).Name);
			Assert.AreEqual ("MyObject", ObjectType.FromSystemType (typeof (MyObject)).Name);
			Assert.AreEqual ("Object", ObjectType.FromSystemType (typeof (MyObject)).BaseType.Name);
			
			Assert.IsNull (ObjectType.FromSystemType (typeof (ObjectTest)));
			
			MyObject mo = new MyObject ();
			
			Assert.AreEqual ("MyObject", mo.ObjectType.Name);
			Assert.AreSame (ObjectType.FromSystemType (typeof (MyObject)), mo.ObjectType);
			Assert.AreSame (typeof (MyObject), mo.ObjectType.SystemType);
			
			ObjectA oa = new ObjectA ();
			ObjectB ob = new ObjectB ();
			ObjectX ox = new ObjectX ();
			
			Assert.AreEqual ("ObjectA", oa.ObjectType.Name);
			Assert.AreEqual ("ObjectB", ob.ObjectType.Name);
			Assert.AreEqual ("ObjectX", ox.ObjectType.Name);
			Assert.AreEqual ("Object", oa.ObjectType.BaseType.Name);
			Assert.AreEqual ("ObjectA", ob.ObjectType.BaseType.Name);
			Assert.AreEqual ("Object", ox.ObjectType.BaseType.Name);
			
			Assert.AreEqual (oa.ObjectType.BaseType.Name, ObjectType.FromSystemType (typeof (Object)).Name);
			Assert.AreSame (oa.ObjectType.BaseType, ObjectType.FromSystemType (typeof (Object)));
			
			Assert.IsTrue (oa.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (Object))));
			Assert.IsTrue (ob.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (Object))));
			Assert.IsTrue (ox.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (Object))));
			Assert.IsTrue (ob.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (ObjectA))));
			Assert.IsFalse (oa.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (ObjectA))));
			Assert.IsFalse (oa.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (ObjectB))));
			Assert.IsFalse (oa.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (ObjectX))));
			Assert.IsFalse (ob.ObjectType.IsSubclassOf (ObjectType.FromSystemType (typeof (ObjectX))));
			
			Assert.IsTrue (oa.ObjectType.IsObjectInstanceOfType (oa));
			Assert.IsTrue (oa.ObjectType.IsObjectInstanceOfType (ob));
			Assert.IsFalse (ob.ObjectType.IsObjectInstanceOfType (oa));
			Assert.IsFalse (oa.ObjectType.IsObjectInstanceOfType (ox));
		}
		
		
		
		private void PrivateCreateObjects(int runs)
		{
			MyObject[] array = new MyObject[runs];
			long before = System.GC.GetTotalMemory (true);
			
			for (int i = 0; i < 1000; i++)
			{
				long cc = Epsitec.Common.Drawing.Agg.Library.Cycles;
			}
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c0 = Epsitec.Common.Drawing.Agg.Library.Cycles - c2;
			
			System.Console.Out.WriteLine ("Zero work: {0:0.0} ns", c0 / cycles_per_ns);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				array[i] = new MyObject ();
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Creating objects: {0:0.0} ns.", c2 / runs / cycles_per_ns);
			System.Console.WriteLine ("Allocated {0} bytes/object.", (System.GC.GetTotalMemory (true) - before) / runs);
			System.Console.Out.Flush ();
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				array[i].Xyz = 10;
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Setting int Xyz : {0:0.0} ns.", c2 / runs / cycles_per_ns);
			System.Console.Out.Flush ();
			
			string text = null;
			int    xyz  = 0;
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				text = array[i].Name;
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Reading default string Name ({1}) : {0:0.0} ns.", c2 / runs / cycles_per_ns, text);
			System.Console.Out.Flush ();
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				array[i].Name = "Test";
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Setting string Name : {0:0.0} ns.", c2 / runs / cycles_per_ns);
			System.Console.Out.Flush ();
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				xyz = array[i].Xyz;
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Reading local int Xyz ({1}) : {0:0.0} ns.", c2 / runs / cycles_per_ns, xyz);
			System.Console.Out.Flush ();
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				text = array[i].Name;
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Reading local string Name ({1}) : {0:0.0} ns.", c2 / runs / cycles_per_ns, text);
			System.Console.Out.Flush ();
			
			MyObject.OnFooChangedCallCount = 0;
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				array[i].Foo = "0";
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Setting string Foo, initial : {0:0.0} ns.", c2 / runs / cycles_per_ns);
			System.Console.Out.Flush ();
			
			Assert.AreEqual (MyObject.OnFooChangedCallCount, 1*runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				array[i].Foo = "1";
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Setting string Foo, changed : {0:0.0} ns.", c2 / runs / cycles_per_ns);
			System.Console.Out.Flush ();
			
			Assert.AreEqual (MyObject.OnFooChangedCallCount, 2*runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				array[i].Foo = "1";
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			System.Console.WriteLine ("Setting string Foo, unchanged : {0:0.0} ns.", c2 / runs / cycles_per_ns);
			System.Console.Out.Flush ();
			
			Assert.AreEqual (MyObject.OnFooChangedCallCount, 2*runs);
		}
		
		
		private class MyObject : Types.Object
		{
			public MyObject()
			{
			}
			
			
			public int				Xyz
			{
				get
				{
					return (int) this.GetValue (MyObject.XyzProperty);
				}
				set
				{
					this.SetValue (MyObject.XyzProperty, value);
				}
			}
			public string			Name
			{
				get
				{
					return (string) this.GetValue (MyObject.NameProperty);
				}
				set
				{
					this.SetValue (MyObject.NameProperty, value);
				}
			}
			public string			Foo
			{
				get
				{
					return (string) this.GetValue (MyObject.FooProperty);
				}
				set
				{
					this.SetValue (MyObject.FooProperty, value);
				}
			}
			public MyObject			Sibling
			{
				get
				{
					return this.GetValue (MyObject.SiblingProperty) as MyObject;
				}
				set
				{
					this.SetValue (MyObject.SiblingProperty, value);
				}
			}
			
			public static int		OnFooChangedCallCount = 0;
			
			public static Property XyzProperty	= Property.Register ("Xyz", typeof (int), typeof (MyObject));
			public static Property NameProperty	= Property.Register ("Name", typeof (string), typeof (MyObject), new PropertyMetadata ("[default]"));
			public static Property FooProperty	= Property.Register ("Foo", typeof (string), typeof (MyObject), new PropertyMetadata ("[default]", new PropertyInvalidatedCallback (MyObject.NotifyOnFooChanged)));
			public static Property SiblingProperty = Property.Register ("Sibling", typeof (MyObject), typeof (MyObject));
			
			protected virtual void OnFooChanged()
			{
				MyObject.OnFooChangedCallCount++;
			}
			
			
			private static void NotifyOnFooChanged(Object o, object old_value, object new_value)
			{
				MyObject m = o as MyObject;
				m.OnFooChanged ();
			}
		}
		
		private class ObjectA : Types.Object
		{
		}
		
		private class ObjectB : ObjectA
		{
		}
		
		private class ObjectX : Types.Object
		{
		}
		
		private const double cycles_per_ns = 1.7;
	}
}
