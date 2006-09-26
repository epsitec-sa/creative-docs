//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Types
{
	[TestFixture] public class TypeRosettaTest
	{
		[Test]
		public void CheckCreateTypeObjects()
		{
			TypeRosettaTest.CreateTypeObject (BooleanType.Default);
			TypeRosettaTest.CreateTypeObject (DecimalType.Default);
			TypeRosettaTest.CreateTypeObject (DoubleType.Default);
			TypeRosettaTest.CreateTypeObject (IntegerType.Default);
			TypeRosettaTest.CreateTypeObject (LongIntegerType.Default);
			TypeRosettaTest.CreateTypeObject (StringType.Default);
			TypeRosettaTest.CreateTypeObject (VoidType.Default);
		}

		private static void CreateTypeObject(AbstractType type)
		{
			System.Console.Out.WriteLine ("Type '{0}':", type.Name);

			Caption caption = type.Caption;
			string text = caption.SerializeToString ();

			text = text.Replace ("&", "&amp;");
			text = text.Replace ("<", "&lt;");
			text = text.Replace (">", "&gt;");
			
			System.Console.Out.WriteLine ("  {0}", text);
		}
		
		[Test]
		public void CheckGetTypeObject()
		{
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Boolean"), "Boolean type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Decimal"), "Decimal type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Double"), "Double type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Integer"), "Integer type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("LongInteger"), "LongInteger type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("String"), "String type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Void"), "Void type not found");

			Assert.AreEqual (typeof (bool),		TypeRosetta.GetTypeObject ("Boolean").SystemType);
			Assert.AreEqual (typeof (decimal),	TypeRosetta.GetTypeObject ("Decimal").SystemType);
			Assert.AreEqual (typeof (double),	TypeRosetta.GetTypeObject ("Double").SystemType);
			Assert.AreEqual (typeof (int),		TypeRosetta.GetTypeObject ("Integer").SystemType);
			Assert.AreEqual (typeof (long),		TypeRosetta.GetTypeObject ("LongInteger").SystemType);
			Assert.AreEqual (typeof (string),	TypeRosetta.GetTypeObject ("String").SystemType);
			Assert.AreEqual (typeof (void),		TypeRosetta.GetTypeObject ("Void").SystemType);

			Assert.AreEqual (typeof (bool),		TypeRosetta.GetTypeObject (Druid.Parse ("[1003]")).SystemType);
			Assert.AreEqual (typeof (decimal),	TypeRosetta.GetTypeObject (Druid.Parse ("[1004]")).SystemType);
			Assert.AreEqual (typeof (double),	TypeRosetta.GetTypeObject (Druid.Parse ("[1005]")).SystemType);
			Assert.AreEqual (typeof (int),		TypeRosetta.GetTypeObject (Druid.Parse ("[1006]")).SystemType);
			Assert.AreEqual (typeof (long),		TypeRosetta.GetTypeObject (Druid.Parse ("[1007]")).SystemType);
			Assert.AreEqual (typeof (string),	TypeRosetta.GetTypeObject (Druid.Parse ("[1008]")).SystemType);
			Assert.AreEqual (typeof (void),		TypeRosetta.GetTypeObject (Druid.Parse ("[1009]")).SystemType);

			AbstractType t1 = TypeRosetta.GetTypeObject ("Boolean");
			AbstractType t2 = TypeRosetta.GetTypeObject (Druid.Parse ("[1003]"));
			AbstractType t3 = TypeRosetta.CreateTypeObject (Druid.Parse ("[1003]"));
			AbstractType t4 = TypeRosetta.CreateTypeObject (Druid.Parse ("[1003]"));

			Assert.AreEqual (t1, t2);
			Assert.AreNotEqual (t3, t4);

			Caption caption = Resources.DefaultManager.GetCaption (Druid.Parse ("[1003]"));

			AbstractType t5 = TypeRosetta.GetTypeObject (caption);
			AbstractType t6 = TypeRosetta.GetTypeObject (caption);
			AbstractType t7 = TypeRosetta.CreateTypeObject (caption);
			AbstractType t8 = TypeRosetta.CreateTypeObject (caption);

			Assert.AreEqual (t5, t6);
			Assert.AreNotEqual (t7, t8);
		}
		
		[Test]
		public void CheckObjectTypeToSytemType()
		{
			Assert.IsNull (TypeRosetta.GetSystemTypeFromTypeObject (null));
			Assert.IsNull (TypeRosetta.GetSystemTypeFromTypeObject (15.0));

			Assert.AreEqual (typeof (double), TypeRosetta.GetSystemTypeFromTypeObject (typeof (double)));
			
			Assert.AreEqual (typeof (decimal), TypeRosetta.GetSystemTypeFromTypeObject (new DecimalType (0, 10, 0.1M)));
			Assert.AreEqual (typeof (int), TypeRosetta.GetSystemTypeFromTypeObject (new IntegerType (0, 1000)));
			Assert.AreEqual (typeof (string), TypeRosetta.GetSystemTypeFromTypeObject (new StringType ()));
			Assert.AreEqual (typeof (BindingMode), TypeRosetta.GetSystemTypeFromTypeObject (new EnumType (typeof (BindingMode))));
			Assert.AreEqual (typeof (BindingUpdateMode), TypeRosetta.GetSystemTypeFromTypeObject (new EnumType (typeof (BindingUpdateMode))));

			Assert.AreEqual (typeof (string), TypeRosetta.GetSystemTypeFromTypeObject (DependencyObjectTree.NameProperty));

			Assert.AreEqual (typeof (DataObject), TypeRosetta.GetSystemTypeFromTypeObject (DependencyObjectType.FromSystemType (typeof (DataObject))));
		}

		[Test]
		public void CheckObjectTypeToNamedType()
		{
			INamedType type;

			type = TypeRosetta.GetNamedTypeFromTypeObject (typeof (double));

			Assert.IsNotNull (type);
			Assert.AreEqual ("Double", type.Name);
			Assert.AreEqual (typeof (double), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (new IntegerType (0, 100));

			Assert.IsNotNull (type);
			Assert.IsTrue (type is IntegerType);
			Assert.AreEqual ("Integer", type.Name);
			Assert.AreEqual (typeof (int), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (DependencyObjectTree.NameProperty);

			Assert.IsNotNull (type);
			Assert.AreEqual ("String", type.Name);
			Assert.AreEqual (typeof (string), type.SystemType);
		}
		
		[Test]
		[ExpectedException (typeof (Exceptions.InvalidTypeObjectException))]
		public void CheckObjectTypeToNamedTypeEx1()
		{
			INamedType type;

			type = TypeRosetta.GetNamedTypeFromTypeObject (15.0);
		}

		[Test]
		public void CheckValueToObjectType()
		{
			MyObject obj1 = new MyObject ();
			MyObject obj2 = new MyObject ();

			StructuredType objType1 = new StructuredType ();
			StructuredData record = new StructuredData (objType1);

			MyData data = new MyData ();

			objType1.Fields["X"] = new IntegerType ();
			objType1.Fields["Y"] = new IntegerType ();

			TypeRosetta.SetTypeObject (obj1, objType1);

			Assert.AreEqual (objType1, TypeRosetta.GetTypeObjectFromValue (obj1));
			Assert.AreEqual (DependencyObjectType.FromSystemType (typeof (MyObject)), TypeRosetta.GetTypeObjectFromValue (obj2));
			Assert.AreEqual (objType1, TypeRosetta.GetTypeObjectFromValue (record));
			Assert.AreEqual (typeof (DynamicStructuredType), TypeRosetta.GetTypeObjectFromValue (data).GetType ());
		}

		[Test]
		public void CheckObjectTypeToStructuredType()
		{
			MyObject obj1 = new MyObject ();
			MyObject obj2 = new MyObject ();

			StructuredType objType1 = new StructuredType ();
			StructuredData record = new StructuredData (objType1);

			MyData data = new MyData ();

			objType1.Fields["X"] = new IntegerType ();
			objType1.Fields["Y"] = new IntegerType ();

			TypeRosetta.SetTypeObject (obj1, objType1);

			IStructuredType t1 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (obj1));
			IStructuredType t2 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (obj2));
			IStructuredType t3 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (record));
			IStructuredType t4 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (data));

			Assert.AreEqual (2, t1.GetFieldNames ().Length);
			Assert.AreEqual (1, t2.GetFieldNames ().Length);
			Assert.AreEqual (2, t3.GetFieldNames ().Length);
			Assert.AreEqual (1, t4.GetFieldNames ().Length);

			Assert.AreEqual ("X", t1.GetFieldNames ()[0]);
			Assert.AreEqual ("Y", t1.GetFieldNames ()[1]);
			Assert.AreEqual ("Foo", t2.GetFieldNames ()[0]);
			Assert.AreEqual ("Self", t4.GetFieldNames ()[0]);

			INamedType t1x = t1.GetFieldType ("X");
			INamedType t2foo = t2.GetFieldType ("Foo");
			INamedType t4self = t4.GetFieldType ("Self");

			Assert.AreEqual (typeof (IntegerType), t1x.GetType ());
			Assert.AreEqual (MyObject.FooProperty.PropertyType, t2foo.SystemType);
			Assert.AreEqual (typeof (DynamicStructuredType), t4self.GetType ());
			
			Assert.AreEqual ("Integer", TypeRosetta.GetNamedTypeFromTypeObject (t1x).Name);
			Assert.AreEqual ("String", TypeRosetta.GetNamedTypeFromTypeObject (t2foo).Name);
			Assert.AreEqual ("DynamicStructure", TypeRosetta.GetNamedTypeFromTypeObject (t4self).Name);
			Assert.AreEqual ("System.Int16", TypeRosetta.GetNamedTypeFromTypeObject (typeof (short)).Name);
		}

		[Test]
		public void CheckVerifyValueValidity()
		{
			Assert.IsTrue (TypeRosetta.IsValidValue (10, typeof (int)));
			Assert.IsFalse (TypeRosetta.IsValidValue (10.5, typeof (int)));
			Assert.IsTrue (TypeRosetta.IsValidValue (10, new IntegerType ()));
			Assert.IsFalse (TypeRosetta.IsValidValue (10.5, new IntegerType ()));
			Assert.IsTrue (TypeRosetta.IsValidValue (10, new IntegerType (0, 10)));
			Assert.IsFalse (TypeRosetta.IsValidValue (11, new IntegerType (0, 10)));

			Assert.IsTrue (TypeRosetta.IsValidValue (new A (), typeof (A)));
			Assert.IsTrue (TypeRosetta.IsValidValue (new B (), typeof (A)));
			Assert.IsFalse (TypeRosetta.IsValidValue (new A (), typeof (B)));
			Assert.IsTrue (TypeRosetta.IsValidValue (new B (), typeof (B)));
		}

		#region A Class

		private class A
		{
			public A()
			{
			}
		}
		
		#endregion

		#region B Class

		private class B : A
		{
			public B()
			{
			}
		}

		#endregion

		#region MyObject Class

		private class MyObject : DependencyObject
		{
			public MyObject()
			{
			}

			public static readonly DependencyProperty FooProperty = DependencyProperty.Register ("Foo", typeof (string), typeof (MyObject));
		}

		#endregion

		#region MyData Class

		private class MyData : IStructuredData
		{
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
				return new string[] { "Self" };
			}

			public object GetValue(string name)
			{
				if (name == "Self")
				{
					return this;
				}
				
				throw new System.ArgumentException ();
			}

			public void SetValue(string name, object value)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			#endregion
		}

		#endregion
	}
}
