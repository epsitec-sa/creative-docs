using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Collections.Generic;

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
			//	l'ex�cution du .cctor de la classe Test1 avant que le DependencyObjectType
			//	relatif � Test1 ne soit instanci�.

			TestAttachedProperties.TestA ();
			TestAttachedProperties.TestB ();
			TestAttachedProperties.TestC ();
		}
		
		[Test]
		public void CheckAttachedPropertiesEventNotification()
		{
			//	V�rifie que les �v�nements sont g�n�r�s aussi pour des propri�t�s attach�es,
			//	pas seulement pour des propri�t�s normales.
			
			Test2 t2 = new Test2 ();

			EventHandlerSupport handler = new EventHandlerSupport ();

			t2.SetValue (Test2.StandardProperty, "x");
			Test1.SetAttached (t2, "a");
			
			t2.AddEventHandler (Test2.StandardProperty, handler.RecordEvent);
			t2.AddEventHandler (Test1.AttachedProperty, handler.RecordEvent);

			//	Modifie la propri�t� normale de "x" en "y"
			
			t2.SetValue (Test2.StandardProperty, "y");
			Assert.AreEqual ("Standard:x,y.", handler.Log);
			
			handler.Clear ();
			
			//	Modifie la propri�t� attach�e de "a" en "b"
			
			Test1.SetAttached (t2, "b");
			Assert.AreEqual ("Attached:a,b.", handler.Log);
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
				bindingName.Path = "Name";

				myTarget.SetBinding (MyObject.FooProperty, bindingName);
			}

			using (bindingXyz.DeferChanges ())
			{
				bindingXyz.Mode = BindingMode.TwoWay;
				bindingXyz.Source = mySource;
				bindingXyz.Path = "Sibling.Xyz";

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
			bindingContext.Path = "Sibling";

			//	myTarget --> bindingContext { source=mySource1, path=Sibling }

			DataObject.SetDataContext (myTarget, bindingContext);

			bindingXyz.Mode = BindingMode.TwoWay;
			bindingXyz.Path = "Xyz";

			myTarget.SetBinding (MyObject.XyzProperty, bindingXyz);

			//	myTarget -+-> binding { source=*, path=Xyz } sur propri�t� Xyz
			//	          +-> bindingContext { source=mySource1, path=Sibling }
			//
			//	Donc myTarget.Xyz sera d�fini selon le contenu obtenu par
			//	la source mySource1, via un path concat�n� de Sibling.Xyz,
			//	soit :
			//
			//		mySource1 --Sibling--> myData1 --Xyz--> 999

			Assert.AreEqual (999, myTarget.Xyz);

			//		mySource1 --Sibling--> myData1 --Xyz--> 777

			myData1.Xyz = 777;
			Assert.AreEqual (777, myTarget.Xyz);

			//		mySource2 --Sibling--> myData2 --Xyz--> 888

			bindingContext.Source = mySource2;
			Assert.AreEqual (888, myTarget.Xyz);

			//		mySource1 --Sibling--> myData1 --Xyz--> 777

			bindingContext.Source = mySource1;
			Assert.AreEqual (777, myTarget.Xyz);

			//		mySource1 --Sibling--> myData2 --Xyz--> 888

			mySource1.Sibling = myData2;
			Assert.AreEqual (888, myTarget.Xyz);

			//		mySource1 --Sibling--> myData2 --Xyz--> 555 <==== myTarget --Xyz

			Assert.AreEqual (888, myData2.Xyz);
			Assert.AreEqual (888, myTarget.Xyz);
			myTarget.Xyz = 555;
			Assert.AreEqual (555, myData2.Xyz);
			Assert.AreEqual (555, myTarget.Xyz);

			//		mySource1 --Sibling--> myData2 --Xyz--> 333 ====> myTarget --Xyz

			myData2.Xyz = 333;
			Assert.AreEqual (333, myData2.Xyz);
			Assert.AreEqual (333, myTarget.Xyz);
		}

		[Test]
		public void CheckBinding3()
		{
			ResourceBoundData test = new ResourceBoundData ();
			
			Binding binding1 = new Binding (BindingMode.OneTime, test, "Abc");
			Binding binding2 = new Binding (BindingMode.OneTime, test, "Xyz");

			MyObject myData1 = new MyObject ();
			MyObject myData2 = new MyObject ();

			myData1.SetBinding (MyObject.NameProperty, binding1);
			myData1.SetBinding (MyObject.FooProperty, binding2);
			myData2.SetBinding (MyObject.NameProperty, binding1);
			myData2.SetBinding (MyObject.FooProperty, binding2);

			Assert.AreEqual ("[Abc]", myData1.Name);
			Assert.AreEqual ("[Xyz]", myData1.Foo);
			Assert.AreEqual ("[Abc]", myData2.Name);
			Assert.AreEqual ("[Xyz]", myData2.Foo);

			test.Suffix = "1";

			binding1.UpdateTargets (BindingUpdateMode.Reset);
			binding2.UpdateTargets (BindingUpdateMode.Default);

			Assert.AreEqual ("[Abc1]", myData1.Name);
			Assert.AreEqual ("[Xyz]", myData1.Foo);
			Assert.AreEqual ("[Abc1]", myData2.Name);
			Assert.AreEqual ("[Xyz]", myData2.Foo);
		}

		[Test]
		public void CheckBinding4()
		{
			ResourceBoundData test = new ResourceBoundData ();

			Binding binding1 = new Binding (BindingMode.OneTime, test, "Abc");
			Binding binding2 = new Binding (BindingMode.OneTime, test, "Xyz");

			MyObject myData1 = new MyObject ();
			MyObject myData2 = new MyObject ();
			MyObject myData3 = new MyObject ();

			myData1.SetBinding (MyObject.NameProperty, binding1);
			myData1.SetBinding (MyObject.FooProperty, binding2);
			myData2.SetBinding (MyObject.NameProperty, binding1);
			myData2.SetBinding (MyObject.FooProperty, binding2);
			myData3.SetBinding (MyObject.NameProperty, binding1);
			myData3.SetBinding (MyObject.FooProperty, binding2);

			Assert.AreEqual ("[Abc]", myData1.Name);
			Assert.AreEqual ("[Xyz]", myData1.Foo);
			Assert.AreEqual ("[Abc]", myData2.Name);
			Assert.AreEqual ("[Xyz]", myData2.Foo);
			Assert.AreEqual ("[Abc]", myData3.Name);
			Assert.AreEqual ("[Xyz]", myData3.Foo);

			test.Suffix = "1";

			binding1.UpdateTargets (BindingUpdateMode.Reset);
			binding2.UpdateTargets (BindingUpdateMode.Default);

			Assert.AreEqual ("[Abc1]", myData1.Name);
			Assert.AreEqual ("[Xyz]", myData1.Foo);
			Assert.AreEqual ("[Abc1]", myData2.Name);
			Assert.AreEqual ("[Xyz]", myData2.Foo);
			Assert.AreEqual ("[Abc1]", myData3.Name);
			Assert.AreEqual ("[Xyz]", myData3.Foo);

			myData3.ClearBinding (MyObject.NameProperty);
			myData3.ClearBinding (MyObject.FooProperty);

			test.Suffix = "2";

			binding1.UpdateTargets (BindingUpdateMode.Reset);

			Assert.AreEqual ("[Abc2]", myData1.Name);
			Assert.AreEqual ("[Abc2]", myData2.Name);
			Assert.AreEqual ("[Abc1]", myData3.Name);

			myData2.ClearBinding (MyObject.NameProperty);

			test.Suffix = "3";

			binding1.UpdateTargets (BindingUpdateMode.Reset);

			Assert.AreEqual ("[Abc3]", myData1.Name);
			Assert.AreEqual ("[Abc2]", myData2.Name);
			Assert.AreEqual ("[Abc1]", myData3.Name);

			myData1.ClearBinding (MyObject.NameProperty);

			test.Suffix = "4";

			binding1.UpdateTargets (BindingUpdateMode.Reset);

			Assert.AreEqual ("[Abc3]", myData1.Name);
			Assert.AreEqual ("[Abc2]", myData2.Name);
			Assert.AreEqual ("[Abc1]", myData3.Name);
		}

		[Test]
		public void CheckBindingWithConverter()
		{
			MyObject myData1 = new MyObject ();
			MyObject myData2 = new MyObject ();
			MyObject myData3 = new MyObject ();
			MyObject myData4 = new MyObject ();

			myData1.Xyz = 1;
			myData2.Foo = "2";

			Binding binding1 = new Binding (BindingMode.TwoWay, myData1, "Xyz");
			Binding binding2 = new Binding (BindingMode.TwoWay, myData2, "Foo");

			binding1.Converter = new Converters.AutomaticValueConverter ();
			binding2.Converter = new Converters.AutomaticValueConverter ();

			myData3.SetBinding (MyObject.FooProperty, binding1);	//	Data1.Xyz --> Data3.Foo
			myData4.SetBinding (MyObject.XyzProperty, binding2);	//	Data2.Foo --> Data4.Xyz

			Assert.AreEqual ("1", myData3.Foo);				//	r�sultat de la conversion de Data1.Xyz
			Assert.AreEqual (2, myData4.Xyz);				//	r�sultat de la conversion de Data2.Foo

			myData1.Xyz = 10;
			myData2.Foo = "20";

			Assert.AreEqual ("10", myData3.Foo);			//	r�sultat de la conversion de Data1.Xyz
			Assert.AreEqual (20, myData4.Xyz);				//	r�sultat de la conversion de Data2.Foo

			myData3.Foo = "-1";
			myData4.Xyz = -2;

			Assert.AreEqual (-1, myData1.Xyz);				//	r�sultat de la conversion de Data3.Foo
			Assert.AreEqual ("-2", myData2.Foo);			//	r�sultat de la conversion de Data4.Xyz
			
			myData1.ClearValueBase (MyObject.XyzProperty);
			myData2.ClearValueBase (MyObject.FooProperty);

			Assert.AreEqual ("-1", myData3.Foo);			//	inchang� car Data1.Xyz invalide
			Assert.AreEqual (-2, myData4.Xyz);				//	inchang� car Data2.Foo invalide
		}

		private class ResourceBoundData : IResourceBoundSource
		{
			public string Suffix
			{
				get
				{
					return this.suffix;
				}
				set
				{
					this.suffix = value;
				}
			}
			
			#region IResourceBoundSource Members

			public object GetValue(string id)
			{
				return "[" + id + this.suffix + "]";
			}

			#endregion
			
			private string suffix = "";
		}

		[Test]
		public void CheckPropertyInheritance()
		{
			MyObject o1 = new MyObject ();
			MyObject o2 = new MyObject ();
			MyObject o3 = new MyObject ();

			o3.Parent = o2;
			o2.Parent = o1;

			o1.Cascade = "O1";

			Assert.AreEqual ("O1", o1.Cascade);
			Assert.AreEqual ("O1", o2.Cascade);
			Assert.AreEqual ("O1", o3.Cascade);

			o2.Parent = null;

			Assert.AreEqual (null, o3.Cascade);

			o1.Cascade = "X";
			
			Assert.AreEqual ("X", o1.Cascade);
			Assert.AreEqual (null, o2.Cascade);
			Assert.AreEqual (null, o3.Cascade);

			o2.Parent = o1;

			Assert.AreEqual ("X", o1.Cascade);
			Assert.AreEqual ("X", o2.Cascade);
			Assert.AreEqual ("X", o3.Cascade);
		}

		[Test]
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
			Assert.IsTrue (typeof (MyObject).IsSubclassOf (typeof (DependencyObject)));
			Assert.IsTrue (typeof (MyObject).IsAssignableFrom (typeof (MyObject)));
			Assert.IsTrue (typeof (DependencyObject).IsAssignableFrom (typeof (MyObject)));

			Assert.IsFalse (MyObject.FooProperty.IsPropertyTypeDerivedFromDependencyObject);
			Assert.IsTrue (MyObject.SiblingProperty.IsPropertyTypeDerivedFromDependencyObject);
			
			Assert.IsTrue (MyObject.ReadOnlyProperty.IsReadOnly);
			Assert.IsTrue (MyObject.XyzProperty.IsReadWrite);
			Assert.IsFalse (MyObject.XyzProperty.IsReadOnly);
			Assert.IsFalse (MyObject.ReadOnlyProperty.IsReadWrite);
			
			Assert.AreEqual ("DependencyObject", DependencyObjectType.FromSystemType (typeof (Types.DependencyObject)).Name);
			Assert.AreEqual ("MyObject", DependencyObjectType.FromSystemType (typeof (MyObject)).Name);
			Assert.AreEqual ("DependencyObject", DependencyObjectType.FromSystemType (typeof (MyObject)).BaseType.Name);
			
			MyObject mo = new MyObject ();
			
			Assert.AreEqual ("MyObject", mo.ObjectType.Name);
			Assert.AreSame (DependencyObjectType.FromSystemType (typeof (MyObject)), mo.ObjectType);
			Assert.AreSame (typeof (MyObject), mo.ObjectType.SystemType);
			
			ObjectA oa = new ObjectA ();
			ObjectB ob = new ObjectB ();
			ObjectX ox = new ObjectX ();
			
			Assert.AreEqual ("ObjectA", oa.ObjectType.Name);
			Assert.AreEqual ("ObjectB", ob.ObjectType.Name);
			Assert.AreEqual ("ObjectX", ox.ObjectType.Name);
			Assert.AreEqual ("DependencyObject", oa.ObjectType.BaseType.Name);
			Assert.AreEqual ("ObjectA", ob.ObjectType.BaseType.Name);
			Assert.AreEqual ("DependencyObject", ox.ObjectType.BaseType.Name);
			
			Assert.AreEqual (oa.ObjectType.BaseType.Name, DependencyObjectType.FromSystemType (typeof (DependencyObject)).Name);
			Assert.AreSame (oa.ObjectType.BaseType, DependencyObjectType.FromSystemType (typeof (DependencyObject)));
			
			Assert.IsTrue (oa.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (DependencyObject))));
			Assert.IsTrue (ob.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (DependencyObject))));
			Assert.IsTrue (ox.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (DependencyObject))));
			Assert.IsTrue (ob.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (ObjectA))));
			Assert.IsFalse (oa.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (ObjectA))));
			Assert.IsFalse (oa.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (ObjectB))));
			Assert.IsFalse (oa.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (ObjectX))));
			Assert.IsFalse (ob.ObjectType.IsSubclassOf (DependencyObjectType.FromSystemType (typeof (ObjectX))));
			
			Assert.IsTrue (oa.ObjectType.IsObjectInstanceOfType (oa));
			Assert.IsTrue (oa.ObjectType.IsObjectInstanceOfType (ob));
			Assert.IsFalse (ob.ObjectType.IsObjectInstanceOfType (oa));
			Assert.IsFalse (oa.ObjectType.IsObjectInstanceOfType (ox));

			Assert.IsTrue (ObjectX.AProperty.IsValidType (oa));		//	ObjectA --> DependencyObject
			Assert.IsTrue (ObjectX.AProperty.IsValidType (ob));		//	ObjectB --> ObjectA --> DependencyObject
			Assert.IsFalse (ObjectX.AProperty.IsValidType (ox));	//	ObjectX --> DependencyObject, pas ObjectA

			Assert.IsFalse (ObjectX.BProperty.IsValidType (oa));	//	ObjectA --> DependencyObject, pas ObjectB
			Assert.IsTrue (ObjectX.BProperty.IsValidType (ob));		//	ObjectB --> ObjectA --> DependencyObject
			Assert.IsFalse (ObjectX.BProperty.IsValidType (ox));	//	ObjectX --> DependencyObject, pas ObjectB

			Assert.IsTrue (ObjectX.AProperty.IsOwnedBy (typeof (ObjectX)));
			Assert.IsFalse (ObjectX.AProperty.IsReferencedBy (typeof (ObjectY)));

			DependencyObjectType.FromSystemType (typeof (ObjectY));
			
			Assert.IsTrue (ObjectX.AProperty.IsReferencedBy (typeof (ObjectY)));
		}
		[Test]
		[ExpectedException (typeof (Exceptions.WrongBaseTypeException))]
		public void CheckObjectTypeEx1()
		{
			DependencyObjectType.FromSystemType (typeof (ObjectTest));
		}

		[Test]
		public void CheckPassiveClass()
		{
			Assert.AreEqual ("", ObjectTest.registered);
			DerivedPassiveClass.Hello ();
			Assert.AreEqual ("DerivedPassiveClass, PassiveClass", ObjectTest.registered);
		}

		[Test]
		public void CheckProperties()
		{
			TreeTest t = new TreeTest ();

			t.Name = "Name";
			t.Value = "Value";

			List<DependencyProperty> properties = new List<DependencyProperty> ();

			foreach (LocalValueEntry entry in t.LocalValueEntries)
			{
				properties.Add (entry.Property);
			}

			Assert.AreEqual (5, properties.Count);
			Assert.AreEqual ("Name", properties[0].Name);
			Assert.AreEqual ("Value", properties[1].Name);
			Assert.AreEqual ("Children", properties[2].Name);
			Assert.AreEqual ("HasChildren", properties[3].Name);
			Assert.AreEqual ("Parent", properties[4].Name);
		}

		[Test]
		[ExpectedException (typeof (System.TypeInitializationException))]
		public void CheckPropertiesEx1()
		{
			DependencyProperty p = Test2b.StandardProperty;
		}

		[Test]
		public void CheckPropertiesEx2()
		{
			DependencyProperty p;
			
			p = Test1b.AttachedProperty;
			p = Test1c.AttachedProperty;
		}

		[Test]
		[ExpectedException (typeof (System.TypeInitializationException))]
		public void CheckPropertiesEx3()
		{
			DependencyProperty p = Test3a.InvalidProperty;
		}

		[Test]
		[ExpectedException (typeof (System.TypeInitializationException))]
		public void CheckPropertiesEx4()
		{
			DependencyProperty p = Test3b.InvalidProperty;
		}

		[Test]
		[ExpectedException (typeof (System.TypeInitializationException))]
		public void CheckPropertiesEx5()
		{
			DependencyProperty p = Test3c.InvalidProperty;
		}

		[Test]
		public void CheckPropertyPath()
		{
			DependencyPropertyPath pp1 = new DependencyPropertyPath ();
			DependencyPropertyPath pp2 = new DependencyPropertyPath ("abc");
			DependencyPropertyPath pp3 = new DependencyPropertyPath ("{0}.{1}", MyObject.FooProperty, MyObject.NameProperty);
			DependencyPropertyPath pp4 = new DependencyPropertyPath ("Bar.{0}", MyObject.XyzProperty);
			DependencyPropertyPath pp5 = new DependencyPropertyPath ("{*}", MyObject.FooProperty, MyObject.NameProperty);

			Assert.AreEqual (null, pp1.GetFullPath ());
			Assert.AreEqual ("abc", pp2.GetFullPath ());
			Assert.AreEqual ("Foo.Name", pp3.GetFullPath ());
			Assert.AreEqual ("Bar.Xyz", pp4.GetFullPath ());
			Assert.AreEqual ("Foo.Name", pp5.GetFullPath ());
			Assert.IsTrue (pp2.Elements.IsNull);
			Assert.AreEqual (0, pp2.Elements.Length);
			Assert.AreEqual ("Foo.Name.Bar.Xyz", DependencyPropertyPath.Combine (pp3, pp4).GetFullPath ());
			Assert.AreEqual ("Bar.Xyz", DependencyPropertyPath.Combine (pp1, pp4).GetFullPath ());
			Assert.AreEqual ("abc", DependencyPropertyPath.Combine (pp2, pp1).GetFullPath ());
			Assert.AreEqual (null, DependencyPropertyPath.Combine (pp1, pp1).GetFullPath ());
		}

		[Test]
		public void CheckTree()
		{
			Assert.IsTrue (DependencyObjectTree.ChildrenProperty.IsReadOnly);
			Assert.IsTrue (TreeTest.ChildrenProperty.IsReadOnly);
			Assert.IsTrue (TreeTest.ChildrenProperty.GetMetadata (typeof (TreeTest)).CanSerializeReadOnly);
			Assert.IsFalse (TreeTest.ChildrenProperty.DefaultMetadata.CanSerializeReadOnly);
			
			TreeTest a = new TreeTest ();
			TreeTest b = new TreeTest ();
			TreeTest q = new TreeTest ();
			TreeTest c1 = new TreeTest ();
			TreeTest c2 = new TreeTest ();
			TreeTest anon1 = new TreeTest ();
			TreeTest y = new TreeTest ();

			a.AddChild (b);
			a.AddChild (q);
			b.AddChild (c1);
			b.AddChild (c2);

			a.Name = "a";
			b.Name = "b";
			q.Name = "q";
			c1.Name = "c1";
			c2.Name = "c2";

			y.Name = "y";

			a.Value = "A";
			b.Value = "B";
			q.Value = "Q";
			c1.Value = "C1";
			c2.Value = "C2";
			
			//	a --+--> b --+--> c1
			//	    |        +--> c2
			//	    +--> q

			Assert.IsTrue (a.HasChildren);
			Assert.IsTrue (b.HasChildren);
			Assert.IsFalse (c1.HasChildren);
			Assert.IsFalse (c2.HasChildren);
			Assert.AreEqual ("a", a.Name);
			Assert.AreEqual ("b", b.Name);
			Assert.AreEqual (b.Children[0].Name, "c1");
			Assert.AreEqual (b.Children[1].Name, "c2");

			Assert.AreEqual (b, DependencyObjectTree.GetParent (c1));
			Assert.AreEqual (b, DependencyObjectTree.GetParent (c2));
			Assert.AreEqual (a, DependencyObjectTree.GetParent (b));
			Assert.AreEqual (a, DependencyObjectTree.GetParent (q));

			DependencyObjectTreeSnapshot snapshot = DependencyObjectTree.CreatePropertyTreeSnapshot (a, TreeTest.ValueProperty);

			c1.Value = "C1-X";
			c2.Value = "C2-X";

			DependencyObjectTreeSnapshot.ChangeRecord[] changes = snapshot.GetChanges ();

			Assert.AreEqual (2, changes.Length);
			Assert.AreEqual (c1, changes[0].Object);
			Assert.AreEqual (TreeTest.ValueProperty, changes[0].Property);
			Assert.AreEqual ("C1", changes[0].OldValue);
			Assert.AreEqual ("C1-X", changes[0].NewValue);
			Assert.AreEqual (c2, changes[1].Object);
			Assert.AreEqual (TreeTest.ValueProperty, changes[1].Property);
			Assert.AreEqual ("C2", changes[1].OldValue);
			Assert.AreEqual ("C2-X", changes[1].NewValue);

			Assert.AreEqual (a, DependencyObjectTree.FindFirst (a, "a"));
			Assert.AreEqual (b, DependencyObjectTree.FindFirst (a, "b"));
			Assert.AreEqual (c1, DependencyObjectTree.FindFirst (a, "c1"));
			Assert.AreEqual (c2, DependencyObjectTree.FindFirst (a, "c2"));
			Assert.IsNull (DependencyObjectTree.FindFirst (a, "x"));
			Assert.IsNull (DependencyObjectTree.FindFirst (a, "y"));
			
			DependencyObject[] search = DependencyObjectTree.FindAll (a, "*");

			Assert.AreEqual (5, search.Length);
			Assert.AreEqual ("a", (search[0] as TreeTest).Name);
			Assert.AreEqual ("b", (search[1] as TreeTest).Name);
			Assert.AreEqual ("q", (search[2] as TreeTest).Name);
			Assert.AreEqual ("c1", (search[3] as TreeTest).Name);
			Assert.AreEqual ("c2", (search[4] as TreeTest).Name);
			
			search = DependencyObjectTree.FindAll (a, new System.Text.RegularExpressions.Regex (@"c.?.*", System.Text.RegularExpressions.RegexOptions.Singleline));
			
			Assert.AreEqual (2, search.Length);
			Assert.AreEqual ("c1", (search[0] as TreeTest).Name);
			Assert.AreEqual ("c2", (search[1] as TreeTest).Name);

			Assert.AreEqual (b, DependencyObjectTree.FindChild (a, "b"));
			Assert.AreEqual (q, DependencyObjectTree.FindChild (a, "q"));
			Assert.AreEqual (c1, DependencyObjectTree.FindChild (a, "b", "c1"));
			Assert.AreEqual (c2, DependencyObjectTree.FindChild (a, "b", "c2"));
			Assert.IsNull (DependencyObjectTree.FindChild (a, "c2"));
			Assert.IsNull (DependencyObjectTree.FindChild (a, "x"));
			Assert.IsNull (DependencyObjectTree.FindChild (a, "a", "b", "c1", "x"));

			q.AddChild (anon1);
			anon1.AddChild (y);
			
			Assert.AreEqual (y, DependencyObjectTree.FindFirst (a, "y"));
			Assert.AreEqual (y, DependencyObjectTree.FindChild (a, "q", "y"));
			
			search = DependencyObjectTree.FindAll (a, new System.Text.RegularExpressions.Regex (@"c1|y", System.Text.RegularExpressions.RegexOptions.Singleline));
			
			Assert.AreEqual (2, search.Length);
			Assert.AreEqual ("c1", (search[0] as TreeTest).Name);
			Assert.AreEqual ("y", (search[1] as TreeTest).Name);
			
			search = DependencyObjectTree.FindAll (a, "*");

			Assert.AreEqual (7, search.Length);
			Assert.AreEqual ("a", (search[0] as TreeTest).Name);
			Assert.AreEqual ("b", (search[1] as TreeTest).Name);
			Assert.AreEqual ("q", (search[2] as TreeTest).Name);
			Assert.AreEqual ("c1", (search[3] as TreeTest).Name);
			Assert.AreEqual ("c2", (search[4] as TreeTest).Name);
			Assert.AreEqual (null, (search[5] as TreeTest).Name);
			Assert.AreEqual ("y", (search[6] as TreeTest).Name);
		}
		
		[Test]
		public void CheckTreeWithPropertyInheritance()
		{
			TreeTest a = new TreeTest ();
			TreeTest b = new TreeTest ();
			TreeTest c1 = new TreeTest ();
			TreeTest c2 = new TreeTest ();
			TreeTest c3 = new TreeTest ();

			a.AddChild (b);
			b.AddChild (c1);
			b.AddChild (c2);
			b.AddChild (c3);
			
			a.Cascade = "A";
			c2.Cascade = "C2";

			Assert.AreEqual ("A", c1.Cascade);
			Assert.AreEqual ("C2", c2.Cascade);
			Assert.AreEqual ("A", c3.Cascade);

			a.Name = "a";
			b.Name = "b";
			c1.Name = "c1";
			c2.Name = "c2";
			c3.Name = "c3";
			
			EventHandlerSupport handler = new EventHandlerSupport ();

			a.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);
			b.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);
			c1.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);
			c2.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);
			c3.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);

			//	La modification d'une propri�t� h�rit�e qui est d�finie localement
			//	ne provoque qu'une notification locale (et �vtl. des enfants, mais
			//	il n'y en a pas dans ce cas) :
			
			c2.Cascade = "C";

			Assert.AreEqual ("c2-Cascade:C2,C.", handler.Log);
			handler.Clear ();

			//	La modification d'une propri�t� � la racine va �tre r�percut�e �
			//	travers tout l'arbre; on poursuit par une red�finition (en fait,
			//	on d�finit une valeur locale) :
			
			a.Cascade = "a";
			c3.Cascade = "c";

			Assert.AreEqual ("a-Cascade:A,a.b-Cascade:A,a.c1-Cascade:A,a.c3-Cascade:A,a.c3-Cascade:a,c.", handler.Log);
			handler.Clear ();

			//	Supprime une valeur locale pour v�rifier que l'on reprend bien 
			//	la valeur h�rit�e :
			
			c3.ClearValueBase (TreeTest.CascadeProperty);

			Assert.AreEqual ("c3-Cascade:c,a.", handler.Log);
			handler.Clear ();

			//	Modifie l'h�ritage � un niveau interm�diaire de l'arbre, puis
			//	restaure l'arbre dans l'�tat initial.
			
			b.Cascade = "b";
			
			Assert.AreEqual ("b-Cascade:a,b.c1-Cascade:a,b.c3-Cascade:a,b.", handler.Log);
			handler.Clear ();

			b.ClearValueBase (TreeTest.CascadeProperty);

			Assert.AreEqual ("b-Cascade:b,a.c1-Cascade:b,a.c3-Cascade:b,a.", handler.Log);
			handler.Clear ();

			TreeTest x = new TreeTest ();
			TreeTest y = new TreeTest ();
			TreeTest z = new TreeTest ();

			x.Name = "x";
			y.Name = "y";
			z.Name = "z";

			x.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);
			y.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);
			z.AddEventHandler (TreeTest.CascadeProperty, handler.RecordEventAndName);
			
			a.AddChild (x);
			System.Console.Out.WriteLine ("a.AddChild(x) : {0}", handler.Log);
			Assert.AreEqual ("a", a.Cascade);
			Assert.AreEqual ("a", x.Cascade);
			handler.Clear ();
			
			a.AddChild (y);
			System.Console.Out.WriteLine ("a.AddChild(y) : {0}", handler.Log);
			Assert.AreEqual ("a", a.Cascade);
			Assert.AreEqual ("a", y.Cascade);
			handler.Clear ();
			
			//	Change le parent de 'b'. L'arbre devient :
			//
			//	a-->x-->b-->c1/c2/c3
			//	a-->y

			x.AddChild (b);
			System.Console.Out.WriteLine ("x.AddChild(b) : {0}", handler.Log);
			handler.Clear ();

			Assert.AreEqual ("a", y.Cascade);
			y.Cascade = "y";
			Assert.AreEqual ("y", y.Cascade);

			Assert.AreEqual ("y-Cascade:a,y.", handler.Log);
			handler.Clear ();
			
			//	Change le parent de 'b' : a-->y-->b-->c1/c2/c3. Comme 'y' d�finit
			//	sa propre valeur h�rit�e, cela affecte b, c1 et c3.

			//	L'arbre devient :
			//
			//	a-->x
			//	a-->y-->b-->c1/c2/c3
			//	z
			
			y.AddChild (b);

			Assert.AreEqual ("b-Cascade:a,y.c1-Cascade:a,y.c3-Cascade:a,y.", handler.Log);
			handler.Clear ();

			//	L'arbre devient :
			//
			//	a-->x
			//	a-->y
			//	z-->b-->c1/c2/c3
			
			z.AddChild (b);
			Assert.AreEqual ("b-Cascade:y,<UndefinedValue>.c1-Cascade:y,<UndefinedValue>.c3-Cascade:y,<UndefinedValue>.", handler.Log);
			handler.Clear ();
			Assert.AreEqual (null, b.Cascade);
			Assert.AreEqual (null, c1.Cascade);
			Assert.AreEqual ("C", c2.Cascade);
			Assert.AreEqual (null, c3.Cascade);
			
			//	L'arbre devient :
			//
			//	a-->x-->b-->c1/c2/c3
			//	a-->y
			
			x.AddChild (b);
			Assert.AreEqual ("b-Cascade:<UndefinedValue>,a.c1-Cascade:<UndefinedValue>,a.c3-Cascade:<UndefinedValue>,a.", handler.Log);
			handler.Clear ();
		}

		[Test]
		[ExpectedException (typeof (System.TypeInitializationException))]
		public void CheckWrongParentClass()
		{
			MyObject obj = new MyObject ();

			try
			{
				TestWrongParentClass.Test (obj);
			}
			catch (System.Exception ex)
			{
				Assert.AreEqual (typeof (Exceptions.WrongBaseTypeException), ex.InnerException.GetType ());
				throw;
			}
		}
		
		private static class TestWrongParentClass
		{
			public static void Test(DependencyObject obj)
			{
				obj.SetValue (WrongParentClass.FooProperty, "x");
			}
		}
		
		private static class TestAttachedProperties
		{
			public static void TestA()
			{
				//	Aucune analyse de la classe Test1 n'a encore eu lieu; il y
				//	a donc 0 propri�t�s attach�es connues.

				int n = DependencyProperty.GetAllAttachedProperties ().Count;
				Test2 t2 = new Test2 ();
				Assert.AreEqual (n, DependencyProperty.GetAllAttachedProperties ().Count);
			}
			public static void TestB()
			{
				Test2 t2 = new Test2 ();
				int n = DependencyProperty.GetAllAttachedProperties ().Count;
				
				//	Les types sont cr��s � la demande s'ils ne sont pas encore
				//	connus; c'est le cas de ot1 :

				Types.DependencyObjectType ot1 = Types.DependencyObjectType.FromSystemType (typeof (Test1));
				Types.DependencyObjectType ot2 = Types.DependencyObjectType.FromSystemType (typeof (Test2));

				Assert.IsNotNull (ot1);
				Assert.IsNotNull (ot2);

				//	L'analyse de la classe Test1 a eu lieu; il y a donc 1
				//	propri�t� attach�e; celle de Test1 !

				Assert.AreEqual (n+1, DependencyProperty.GetAllAttachedProperties ().Count);
				Assert.AreEqual ("Attached", DependencyProperty.GetAllAttachedProperties ()[n].Name);
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
				Assert.AreEqual ("DependencyProperty 'Attached' changed from 'Hello' to 'Good bye'", ObjectTest.log);
			}
		}

		private static string log = null;

		private static void HandleTest1AttachedChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			ObjectTest.log = string.Format ("DependencyProperty '{2}' changed from '{0}' to '{1}'", e.OldValue, e.NewValue, e.PropertyName);
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

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < runs; i++)
			{
				array[i].NativeXyz = 10;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Setting int Xyz [native] : {0:0.00} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs);
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

			//	-------- lecture ----------

			stopwatch.Reset ();
			stopwatch.Start ();
			
			xyz = 0;

			for (int i = 0; i < runs; i++)
			{
				xyz += array[i].Xyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Reading local int Xyz ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			xyz = 0;
			
			for (int i = 0; i < runs; i++)
			{
				xyz += array[i].NativeXyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Reading local int Xyz [native] ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			MyObject myObj = array[0];
			xyz = 0;

			for (int i = 0; i < runs; i++)
			{
				xyz = myObj.Xyz;
				myObj.Xyz = xyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Inc. local int Xyz [same] ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			myObj = array[0];
			xyz = 0;

			for (int i = 0; i < runs; i++)
			{
				xyz = myObj.NativeXyz;
				myObj.NativeXyz = xyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Inc. local int Xyz [native/same] ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			myObj = array[0];
			xyz = 0;

			for (int i = 0; i < runs; i++)
			{
				xyz = myObj.Xyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Get local int Xyz [same] ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			myObj = array[0];
			xyz = 0;

			for (int i = 0; i < runs; i++)
			{
				xyz = myObj.NativeXyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Get local int Xyz [native/same] ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			myObj = array[0];
			xyz = 0;

			for (int i = 0; i < runs; i++)
			{
				myObj.Xyz = xyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Set local int Xyz [same] ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			myObj = array[0];
			xyz = 0;

			for (int i = 0; i < runs; i++)
			{
				myObj.NativeXyz = xyz;
			}
			stopwatch.Stop ();
			System.Console.WriteLine ("Set local int Xyz [native/same] ({1}) : {0:0.000} us.", stopwatch.ElapsedMilliseconds * 1000.0 / runs, xyz);
			System.Console.Out.Flush ();

			//	-------- mesur� inc. 560ns, get 130ns, set 430ns sur PC 3GHz, Pentium D (dual core)
			
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
		private class MyObject : Types.DependencyObject
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
			public int				NativeXyz
			{
				get
				{
					return this.nativeXyz;
				}
				set
				{
					this.nativeXyz = value;
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
			public string			Cascade
			{
				get
				{
					return this.GetValue (MyObject.CascadeProperty) as string;
				}
				set
				{
					this.SetValue (MyObject.CascadeProperty, value);
				}
			}
			public MyObject			Parent
			{
				get
				{
					return this.GetValue (MyObject.ParentProperty) as MyObject;
				}
				set
				{
					if (value != this.Parent)
					{
						if (this.Parent != null)
						{
							this.Parent.Children.Remove (this);
							this.InheritedPropertyCache.ClearAllValues (this);
						}
						
						this.SetValue (MyObject.ParentProperty, value);

						if (this.Parent != null)
						{
							this.Parent.Children.Add (this);
							this.InheritedPropertyCache.InheritValuesFromParent (this, this.Parent);
						}
					}
				}
			}
			public MyObjectChildren	Children
			{
				get
				{
					if (this.children == null)
					{
						this.children = new MyObjectChildren ();
					}

					return this.children;
				}
			}
			public bool				HasChildren
			{
				get
				{
					return this.children == null ? false : (this.children.Count > 0);
				}
			}
			
			public static int		OnFooChangedCallCount = 0;

			public static object GetValueChildren(DependencyObject o)
			{
				MyObject tt = o as MyObject;
				return tt.Children;
			}
			public static object GetValueHasChildren(DependencyObject o)
			{
				MyObject tt = o as MyObject;
				return tt.HasChildren;
			}
			
			public static DependencyProperty XyzProperty	= DependencyProperty.Register ("Xyz", typeof (int), typeof (MyObject));
			public static DependencyProperty NameProperty	= DependencyProperty.Register ("Name", typeof (string), typeof (MyObject), new DependencyPropertyMetadata ("[default]"));
			public static DependencyProperty FooProperty	= DependencyProperty.Register ("Foo", typeof (string), typeof (MyObject), new DependencyPropertyMetadata ("[default]", new PropertyInvalidatedCallback (MyObject.NotifyOnFooChanged)));
			public static DependencyProperty SiblingProperty = DependencyProperty.Register ("Sibling", typeof (MyObject), typeof (MyObject));
			public static DependencyProperty CascadeProperty = DependencyProperty.Register ("Cascade", typeof (string), typeof (MyObject), new DependencyPropertyMetadataWithInheritance ());
			public static DependencyProperty ParentProperty = DependencyObjectTree.ParentProperty.AddOwner (typeof (MyObject));
			public static DependencyProperty ChildrenProperty = DependencyObjectTree.ChildrenProperty.AddOwner (typeof (MyObject), new DependencyPropertyMetadata (MyObject.GetValueChildren).MakeReadOnlySerializable ());
			public static DependencyProperty HasChildrenProperty = DependencyObjectTree.HasChildrenProperty.AddOwner (typeof (MyObject), new DependencyPropertyMetadata (MyObject.GetValueHasChildren));
			public static DependencyProperty ReadOnlyProperty = DependencyProperty.RegisterReadOnly ("ReadOnly", typeof (string), typeof (MyObject));
			
			protected virtual void OnFooChanged()
			{
				MyObject.OnFooChangedCallCount++;
			}

			private static void NotifyOnFooChanged(DependencyObject o, object old_value, object new_value)
			{
				MyObject m = o as MyObject;
				m.OnFooChanged ();
			}
			
			private MyObjectChildren	children;
			private int					nativeXyz;
		}
		#endregion

		#region ObjectA, ObjectB and ObjectX Classes
		private class ObjectA : Types.DependencyObject
		{
		}
		private class ObjectB : ObjectA
		{
		}
		private class ObjectX : Types.DependencyObject
		{
			public static DependencyProperty AProperty = DependencyProperty.Register ("A", typeof (ObjectA), typeof (ObjectX));
			public static DependencyProperty BProperty = DependencyProperty.Register ("B", typeof (ObjectB), typeof (ObjectX));
		}
		private class ObjectY : ObjectX
		{
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

		#region Test1, Test2... and Test3... Classes
		public class Test1 : Types.DependencyObject
		{
			public Test1()
			{
			}

			public static void SetAttached(DependencyObject o, string value)
			{
				o.SetValue (Test1.AttachedProperty, value);
			}
			public static string GetAttached(DependencyObject o)
			{
				return o.GetValue (Test1.AttachedProperty) as string;
			}

			public static DependencyProperty AttachedProperty = DependencyProperty.RegisterAttached ("Attached", typeof (string), typeof (Test1));
			public static DependencyProperty StandardProperty = DependencyProperty.Register ("Standard", typeof (string), typeof (Test1));
		}
		public class Test1b : Test1
		{
			public Test1b()
			{
			}

			//	Ne provoque pas d'exception � l'initialisation, car "Attached", bien que
			//	h�rit� de Test1, n'est pas incompatible (c'est une propri�t� attach�e).

			public static new DependencyProperty AttachedProperty = DependencyProperty.RegisterAttached ("Attached", typeof (string), typeof (Test1b));
		}
		public class Test1c : Test1
		{
			public Test1c()
			{
			}

			//	Ne provoque pas d'exception � l'initialisation, car "Attached", bien que
			//	h�rit� de Test1, n'est pas incompatible (dans Test1, c'est une propri�t�
			//	attach�e).

			public static new DependencyProperty AttachedProperty = DependencyProperty.Register ("Attached", typeof (string), typeof (Test1c));
			public static new DependencyProperty StandardProperty = DependencyProperty.RegisterAttached ("Standard", typeof (string), typeof (Test1c));
		}
		public class Test2 : Types.DependencyObject
		{
			public Test2()
			{
			}

			public static DependencyProperty StandardProperty = DependencyProperty.Register ("Standard", typeof (string), typeof (Test2));
		}
		public class Test2b : Test2
		{
			public Test2b()
			{
			}
			
			//	Provoque une exception � l'initialisation, car "Standard" est h�rit�
			//	de Test2.
			
			public static new DependencyProperty StandardProperty = DependencyProperty.Register ("Standard", typeof (string), typeof (Test2b));
		}
		public class Test3a : DependencyObject
		{
			public static DependencyProperty InvalidProperty = DependencyProperty.Register (null, typeof (string), typeof (Test3a));
		}
		public class Test3b : DependencyObject
		{
			public static DependencyProperty InvalidProperty = DependencyProperty.Register ("X$z", typeof (string), typeof (Test3b));
		}
		public class Test3c : DependencyObject
		{
			public static DependencyProperty InvalidProperty = DependencyProperty.Register ("_Invalid", typeof (string), typeof (Test3c));
		}
		#endregion

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

		#region TreeTestChildren Class
		class TreeTestChildren : DependencyObjectList<TreeTest>
		{
		}
		#endregion

		#region MyObjectChildren Class
		class MyObjectChildren : DependencyObjectList<MyObject>
		{
		}
		#endregion

		#region TreeTest Class

		class TreeTest : DependencyObject
		{
			public string						Name
			{
				get
				{
					return this.GetValue (TreeTest.NameProperty) as string;
				}
				set
				{
					this.SetValue (TreeTest.NameProperty, value);
				}
			}
			public TreeTest						Parent
			{
				get
				{
					return this.parent;
				}
			}
			public TreeTestChildren				Children
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
					return this.GetValue (TreeTest.ValueProperty) as string;
				}
				set
				{
					this.SetValue (TreeTest.ValueProperty, value);
				}
			}
			public string						Cascade
			{
				get
				{
					return this.GetValue (TreeTest.CascadeProperty) as string;
				}
				set
				{
					this.SetValue (TreeTest.CascadeProperty, value);
				}
			}
			
			public void AddChild(TreeTest item)
			{
				if (this.children == null)
				{
					this.children = new TreeTestChildren ();
				}
				if (item.parent != null)
				{
					item.parent.children.Remove (item);
					item.InheritedPropertyCache.ClearAllValues (item);
				}
				this.children.Add (item);

				item.parent = this;
				item.InheritedPropertyCache.InheritValuesFromParent (item, item.parent);
				item.InheritedPropertyCache.NotifyChanges (item);
			}
			
			public static object GetValueParent(DependencyObject o)
			{
				TreeTest tt = o as TreeTest;
				return tt.Parent;
			}
			public static object GetValueChildren(DependencyObject o)
			{
				TreeTest tt = o as TreeTest;
				if (tt.children == null)
				{
					tt.children = new TreeTestChildren ();
				}
				return tt.children;
			}
			public static object GetValueHasChildren(DependencyObject o)
			{
				TreeTest tt = o as TreeTest;
				return tt.HasChildren;
			}

			public static DependencyProperty NameProperty = DependencyObjectTree.NameProperty.AddOwner (typeof (TreeTest));
			public static DependencyProperty ParentProperty = DependencyObjectTree.ParentProperty.AddOwner (typeof (TreeTest), new DependencyPropertyMetadata (TreeTest.GetValueParent));
			public static DependencyProperty ChildrenProperty = DependencyObjectTree.ChildrenProperty.AddOwner (typeof (TreeTest), new DependencyPropertyMetadata (TreeTest.GetValueChildren).MakeReadOnlySerializable ());
			public static DependencyProperty HasChildrenProperty = DependencyObjectTree.HasChildrenProperty.AddOwner (typeof (TreeTest), new DependencyPropertyMetadata (TreeTest.GetValueHasChildren));
			public static DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (string), typeof (TreeTest));
			public static DependencyProperty CascadeProperty = DependencyProperty.Register ("Cascade", typeof (string), typeof (TreeTest), new DependencyPropertyMetadataWithInheritance (UndefinedValue.Instance));

			TreeTest parent;
			TreeTestChildren children;
		}
		
		#endregion

		#region WrongParentClass Class

		class WrongParentClass
		{
			public WrongParentClass()
			{
			}

			public static readonly DependencyProperty FooProperty = DependencyProperty.RegisterAttached ("Foo", typeof (string), typeof (WrongParentClass));
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
