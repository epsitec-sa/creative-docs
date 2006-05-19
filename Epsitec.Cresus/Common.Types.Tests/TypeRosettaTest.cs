//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class TypeRosettaTest
	{
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
			Assert.AreEqual ("System.Double", type.Name);
			Assert.AreEqual (typeof (double), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (new IntegerType (0, 100));

			Assert.IsNotNull (type);
			Assert.IsTrue (type is IntegerType);
			Assert.AreEqual ("Integer", type.Name);
			Assert.AreEqual (typeof (int), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (DependencyObjectTree.NameProperty);

			Assert.IsNotNull (type);
			Assert.AreEqual ("System.String", type.Name);
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
			StructuredRecord record = new StructuredRecord (objType1);

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
			StructuredRecord record = new StructuredRecord (objType1);

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

			object t1x = t1.GetFieldTypeObject ("X");
			object t2foo = t2.GetFieldTypeObject ("Foo");
			object t4self = t4.GetFieldTypeObject ("Self");

			Assert.AreEqual (typeof (IntegerType), t1x.GetType ());
			Assert.AreEqual (MyObject.FooProperty, t2foo);
			Assert.AreEqual (typeof (DynamicStructuredType), t4self.GetType ());
			
			Assert.AreEqual ("Integer", TypeRosetta.GetNamedTypeFromTypeObject (t1x).Name);
			Assert.AreEqual ("System.String", TypeRosetta.GetNamedTypeFromTypeObject (t2foo).Name);
			Assert.AreEqual ("Dynamic", TypeRosetta.GetNamedTypeFromTypeObject (t4self).Name);
		}

		private class MyObject : DependencyObject
		{
			public MyObject()
			{
			}

			public static readonly DependencyProperty FooProperty = DependencyProperty.Register ("Foo", typeof (string), typeof (MyObject));
		}

		private class MyData : IStructuredData
		{
			#region IStructuredData Members

			public void AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public void DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
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

			public bool HasImmutableRoots
			{
				get
				{
					return true;
				}
			}

			#endregion
		}
	}
}
