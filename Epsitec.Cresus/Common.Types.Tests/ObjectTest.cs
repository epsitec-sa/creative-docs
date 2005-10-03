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
		
		
		[Test] public void CheckObjectCreation()
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
		
		[Test] public void CheckObjectType()
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
			
			Assert.IsTrue (oa.ObjectType.IsInstanceOfType (oa));
			Assert.IsTrue (oa.ObjectType.IsInstanceOfType (ob));
			Assert.IsFalse (ob.ObjectType.IsInstanceOfType (oa));
			Assert.IsFalse (oa.ObjectType.IsInstanceOfType (ox));
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
			
			public static int		OnFooChangedCallCount = 0;
			
			public static Property XyzProperty	= Property.Register ("Xyz", typeof (int), typeof (MyObject));
			public static Property NameProperty	= Property.Register ("Name", typeof (string), typeof (MyObject), new PropertyMetadata ("[default]"));
			public static Property FooProperty	= Property.Register ("Foo", typeof (string), typeof (MyObject), new PropertyMetadata ("[default]", new PropertyInvalidatedCallback (MyObject.NotifyOnFooChanged)));
			
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
