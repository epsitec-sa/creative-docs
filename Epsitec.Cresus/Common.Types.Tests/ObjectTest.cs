using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Common.Types
{
	[TestFixture] public class ObjectTest
	{
		[SetUp]
		public void Initialise()
		{
		}


		[Test]
		public void CheckAttachedProperties()
		{
			//	Test coup� en morceaux, car NUnit et son analyse du code provoque
			//	l'ex�cution du .cctor de la classe Test1 avant que le ObjectType
			//	relatif � Test1 ne soit instanci�.

			TestAttachedProperties.TestA ();
			TestAttachedProperties.TestB ();
			TestAttachedProperties.TestC ();
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
//		[Ignore ("Too slow")]
		public void CheckObjectCreationPerformance()
		{
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

			Assert.IsTrue (ObjectX.AProperty.IsValidType (oa));		//	ObjectA --> Object
			Assert.IsTrue (ObjectX.AProperty.IsValidType (ob));		//	ObjectB --> ObjectA --> Object
			Assert.IsFalse (ObjectX.AProperty.IsValidType (ox));	//	ObjectX --> Object, pas ObjectA

			Assert.IsFalse (ObjectX.BProperty.IsValidType (oa));	//	ObjectA --> Object, pas ObjectB
			Assert.IsTrue (ObjectX.BProperty.IsValidType (ob));		//	ObjectB --> ObjectA --> Object
			Assert.IsFalse (ObjectX.BProperty.IsValidType (ox));	//	ObjectX --> Object, pas ObjectB

			Assert.IsTrue (ObjectX.AProperty.IsOwnedBy (typeof (ObjectX)));
			Assert.IsFalse (ObjectX.AProperty.IsReferencedBy (typeof (ObjectY)));

			ObjectType.FromSystemType (typeof (ObjectY));
			
			Assert.IsTrue (ObjectX.AProperty.IsReferencedBy (typeof (ObjectY)));
		}

		[Test]
		public void CheckPassiveClass()
		{
			Assert.AreEqual ("", ObjectTest.registered);
			DerivedPassiveClass.Hello ();
			Assert.AreEqual ("DerivedPassiveClass, PassiveClass", ObjectTest.registered);
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

		private static class TestAttachedProperties
		{
			public static void TestA()
			{
				//	Aucune analyse de la classe Test1 n'a encore eu lieu; il y
				//	a donc 0 propri�t�s attach�es connues.
				
				Assert.AreEqual (0, Property.GetAllAttachedProperties ().Count);
				Test2 t2 = new Test2 ();
				Assert.AreEqual (0, Property.GetAllAttachedProperties ().Count);
			}
			public static void TestB()
			{
				Test2 t2 = new Test2 ();
				Assert.AreEqual (0, Property.GetAllAttachedProperties ().Count);

				//	Les types sont cr��s � la demande s'ils ne sont pas encore
				//	connus; c'est le cas de ot1 :

				Types.ObjectType ot1 = Types.ObjectType.FromSystemType (typeof (Test1));
				Types.ObjectType ot2 = Types.ObjectType.FromSystemType (typeof (Test2));

				Assert.IsNotNull (ot1);
				Assert.IsNotNull (ot2);

				//	L'analyse de la classe Test1 a eu lieu; il y a donc 1
				//	propri�t� attach�e; celle de Test1 !

				Assert.AreEqual (1, Property.GetAllAttachedProperties ().Count);
				Assert.AreEqual ("Attached", Property.GetAllAttachedProperties ()[0].Name);
			}
			public static void TestC()
			{
				//	V�rifie que l'utilisation d'une propri�t� attach�e produit
				//	aussi des �v�nements, comme si c'�tait une propri�t� normale
				//	de l'objet Test2 :
				
				Test2 t2 = new Test2 ();
				
				ObjectTest.log = "";
				Test1.SetAttached (t2, "Hello");
				Assert.AreEqual ("", ObjectTest.log);

				t2.AddEventHandler (Test1.AttachedProperty, ObjectTest.HandleTest1AttachedChanged);

				Test1.SetAttached (t2, "Good bye");
				Assert.AreEqual ("Property 'Attached' changed from 'Hello' to 'Good bye'", ObjectTest.log);
			}
		}

		private static string log = null;

		private static void HandleTest1AttachedChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			ObjectTest.log = string.Format ("Property '{2}' changed from '{0}' to '{1}'", e.OldValue, e.NewValue, e.PropertyName);
		}
		
		private void PrivateCreateObjects(int runs)
		{
			MyObject[] array = new MyObject[runs];
			long before = System.GC.GetTotalMemory (true);

			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();

			stopwatch.Reset ();
			stopwatch.Start ();
			
			for (int i = 0; i < runs; i++)
			{
				array[i] = new MyObject ();
			}

			stopwatch.Stop ();
			
			System.Console.WriteLine ("Creating objects: {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs);
			System.Console.WriteLine ("Allocated {0} bytes/object.", (System.GC.GetTotalMemory (true) - before) / runs);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();
			
			for (int i = 0; i < runs; i++)
			{
				array[i].Xyz = 10;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Setting int Xyz : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs);
			System.Console.Out.Flush ();
			
			string text = null;
			int    xyz  = 0;

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				text = array[i].Name;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Reading default string Name ({1}) : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, text);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				array[i].Name = "Test";
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Setting string Name : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				xyz = array[i].Xyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Reading local int Xyz ({1}) : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				text = array[i].Name;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Reading local string Name ({1}) : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, text);
			System.Console.Out.Flush ();
			
			MyObject.OnFooChangedCallCount = 0;

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				array[i].Foo = "0";
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Setting string Foo, initial : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs);
			System.Console.Out.Flush ();
			
			Assert.AreEqual (MyObject.OnFooChangedCallCount, 1*runs);

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				array[i].Foo = "1";
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Setting string Foo, changed : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs);
			System.Console.Out.Flush ();
			
			Assert.AreEqual (MyObject.OnFooChangedCallCount, 2*runs);

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				array[i].Foo = "1";
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Setting string Foo, unchanged : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs);
			System.Console.Out.Flush ();
			
			Assert.AreEqual (MyObject.OnFooChangedCallCount, 2*runs);
		}

		#region MyObject Class
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
		#endregion

		#region ObjectA, ObjectB and ObjectX Classes
		private class ObjectA : Types.Object
		{
		}
		private class ObjectB : ObjectA
		{
		}
		private class ObjectX : Types.Object
		{
			public static Property AProperty = Property.Register ("A", typeof (ObjectA), typeof (ObjectX));
			public static Property BProperty = Property.Register ("B", typeof (ObjectB), typeof (ObjectX));
		}
		private class ObjectY : ObjectX
		{
			public static Property AProperty = Property.Register ("A", typeof (ObjectA), typeof (ObjectX));
			public static Property BProperty = Property.Register ("B", typeof (ObjectB), typeof (ObjectX));
		}
		#endregion

		#region PassiveClass and DerivedPassiveClass Classes
		private class PassiveClass
		{
			static PassiveClass()
			{
				ObjectTest.Register ("PassiveClass");
			}
		}
		private class DerivedPassiveClass : PassiveClass
		{
			static DerivedPassiveClass()
			{
				ObjectTest.Register ("DerivedPassiveClass");
			}
			
			public static void Hello()
			{
				new DerivedPassiveClass ();
			}
		}
		#endregion

		#region Test1 and Test2 Classes
		public class Test1 : Types.Object
		{
			public Test1()
			{
			}

			public static void SetAttached(Object o, string value)
			{
				o.SetValue (Test1.AttachedProperty, value);
			}
			public static string GetAttached(Object o)
			{
				return o.GetValue (Test1.AttachedProperty) as string;
			}

			public static Property AttachedProperty = Property.RegisterAttached ("Attached", typeof (string), typeof (Test1));
			public static Property StandardProperty = Property.Register ("Standard", typeof (string), typeof (Test1));
		}
		public class Test2 : Types.Object
		{
			public Test2()
			{
			}

			public static Property StandardProperty = Property.Register ("Standard", typeof (string), typeof (Test2));
		}
		#endregion
		
		private static void Register(string name)
		{
			if (ObjectTest.registered.Length > 0)
			{
				ObjectTest.registered = string.Concat (ObjectTest.registered, ", ", name);
			}
			else
			{
				ObjectTest.registered = name;
			}
		}
		
		private static string registered = "";
	}
}
