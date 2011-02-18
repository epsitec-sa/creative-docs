//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			TypeRosettaTest.CreateTypeObject (StringType.NativeDefault);
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
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Default.Boolean"), "Boolean type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Default.Decimal"), "Decimal type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Default.Double"), "Double type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Default.Integer"), "Integer type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Default.LongInteger"), "LongInteger type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Default.String"), "String type not found");
			Assert.IsNotNull (TypeRosetta.GetTypeObject ("Default.Void"), "Void type not found");

			Assert.AreEqual (typeof (bool),		TypeRosetta.GetTypeObject ("Default.Boolean").SystemType);
			Assert.AreEqual (typeof (decimal),	TypeRosetta.GetTypeObject ("Default.Decimal").SystemType);
			Assert.AreEqual (typeof (double),	TypeRosetta.GetTypeObject ("Default.Double").SystemType);
			Assert.AreEqual (typeof (int),		TypeRosetta.GetTypeObject ("Default.Integer").SystemType);
			Assert.AreEqual (typeof (long),		TypeRosetta.GetTypeObject ("Default.LongInteger").SystemType);
			Assert.AreEqual (typeof (string),	TypeRosetta.GetTypeObject ("Default.String").SystemType);
			Assert.AreEqual (typeof (void),		TypeRosetta.GetTypeObject ("Default.Void").SystemType);

			Assert.AreEqual (typeof (bool),		TypeRosetta.GetTypeObject (Druid.Parse ("[1003]")).SystemType);
			Assert.AreEqual (typeof (decimal),	TypeRosetta.GetTypeObject (Druid.Parse ("[1004]")).SystemType);
			Assert.AreEqual (typeof (double),	TypeRosetta.GetTypeObject (Druid.Parse ("[1005]")).SystemType);
			Assert.AreEqual (typeof (int),		TypeRosetta.GetTypeObject (Druid.Parse ("[1006]")).SystemType);
			Assert.AreEqual (typeof (long),		TypeRosetta.GetTypeObject (Druid.Parse ("[1007]")).SystemType);
			Assert.AreEqual (typeof (string),	TypeRosetta.GetTypeObject (Druid.Parse ("[1008]")).SystemType);
			Assert.AreEqual (typeof (void),		TypeRosetta.GetTypeObject (Druid.Parse ("[1009]")).SystemType);

			AbstractType t1 = TypeRosetta.GetTypeObject ("Default.Boolean");
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
		public void CheckIsNullable()
		{
			Assert.IsFalse (TypeRosetta.IsNullable (typeof (int)));
			Assert.IsTrue (TypeRosetta.IsNullable (typeof (string)));
			Assert.IsTrue (TypeRosetta.IsNullable (typeof (int?)));
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
			Assert.AreEqual ("Default.Double", type.Name);
			Assert.AreEqual (typeof (double), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (new IntegerType (0, 100));

			Assert.IsNotNull (type);
			Assert.IsTrue (type is IntegerType);
			Assert.AreEqual ("Integer", type.Name);
			Assert.AreEqual (typeof (int), type.SystemType);

			type = TypeRosetta.GetNamedTypeFromTypeObject (DependencyObjectTree.NameProperty);

			Assert.IsNotNull (type);
			Assert.AreEqual ("Default.String", type.Name);
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

			objType1.Fields.Add ("X", IntegerType.Default);
			objType1.Fields.Add ("Y", IntegerType.Default);
			
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

			objType1.Fields.Add ("X", IntegerType.Default);
			objType1.Fields.Add ("Y", IntegerType.Default);

			TypeRosetta.SetTypeObject (obj1, objType1);

			IStructuredType t1 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (obj1));
			IStructuredType t2 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (obj2));
			IStructuredType t3 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (record));
			IStructuredType t4 = TypeRosetta.GetStructuredTypeFromTypeObject (TypeRosetta.GetTypeObjectFromValue (data));

			Assert.AreEqual (2, Collection.Count (t1.GetFieldIds ()));
			Assert.AreEqual (1, Collection.Count (t2.GetFieldIds ()));
			Assert.AreEqual (2, Collection.Count (t3.GetFieldIds ()));
			Assert.AreEqual (1, Collection.Count (t4.GetFieldIds ()));

			Assert.AreEqual ("X", Collection.Extract (t1.GetFieldIds (), 0));
			Assert.AreEqual ("Y", Collection.Extract (t1.GetFieldIds (), 1));
			Assert.AreEqual ("Foo", Collection.Extract (t2.GetFieldIds (), 0));
			Assert.AreEqual ("Self", Collection.Extract (t4.GetFieldIds (), 0));

			INamedType t1x = t1.GetField ("X").Type;
			INamedType t2foo = t2.GetField ("Foo").Type;
			INamedType t4self = t4.GetField ("Self").Type;

			Assert.AreEqual (typeof (IntegerType), t1x.GetType ());
			Assert.AreEqual (MyObject.FooProperty.PropertyType, t2foo.SystemType);
			Assert.AreEqual (typeof (DynamicStructuredType), t4self.GetType ());

			Assert.AreEqual ("Default.Integer", TypeRosetta.GetNamedTypeFromTypeObject (t1x).Name);
			Assert.AreEqual ("Default.String", TypeRosetta.GetNamedTypeFromTypeObject (t2foo).Name);
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

			Assert.IsTrue (TypeRosetta.IsValidValue (10, new StructuredTypeField ("X", IntegerType.Default)));
		}

		[Test]
		public void CheckVerifyValueValidityForCollectionTypes()
		{
			Assert.IsTrue (TypeRosetta.IsValidValueForCollectionOfType (new List<int> (), IntegerType.Default));
			Assert.IsTrue (TypeRosetta.IsValidValueForCollectionOfType (new List<int> (), typeof (int)));
			Assert.IsFalse (TypeRosetta.IsValidValueForCollectionOfType (new List<int> (), StringType.NativeDefault));
			Assert.IsFalse (TypeRosetta.IsValidValueForCollectionOfType (new List<int> (), typeof (string)));
			Assert.IsTrue (TypeRosetta.IsValidValueForCollectionOfType (new List<string> (), StringType.NativeDefault));
			Assert.IsFalse (TypeRosetta.IsValidValueForCollectionOfType (new List<string> (), IntegerType.Default));

			Assert.IsTrue (TypeRosetta.IsValidValueForCollectionOfType (new List<A> (), typeof (A)));
			Assert.IsTrue (TypeRosetta.IsValidValueForCollectionOfType (new List<B> (), typeof (A)));
			Assert.IsFalse (TypeRosetta.IsValidValueForCollectionOfType (new List<A> (), typeof (B)));
			Assert.IsTrue (TypeRosetta.IsValidValueForCollectionOfType (new List<B> (), typeof (B)));

			StructuredType typeA = new StructuredType ();
			StructuredType typeB = new StructuredType ();

			typeA.Fields.Add (new StructuredTypeField ("Text", StringType.NativeDefault));
			typeB.Fields.Add (new StructuredTypeField ("Number", IntegerType.Default));

			StructuredData dataA = new StructuredData (typeA);
			StructuredData dataB = new StructuredData (typeB);

			Assert.IsTrue (TypeRosetta.IsValidValue (10, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None)));
			Assert.IsFalse (TypeRosetta.IsValidValue (new List<StructuredData> (), new StructuredTypeField ("X", typeA, Support.Druid.Empty, 0, FieldRelation.None)));
			Assert.IsTrue (TypeRosetta.IsValidValue (new List<StructuredData> (), new StructuredTypeField ("X", typeA, Support.Druid.Empty, 0, FieldRelation.Collection)));

			List<StructuredData> listA = new List<StructuredData> ();
			List<StructuredData> listB = new List<StructuredData> ();

			listA.Add (dataA);
			listB.Add (dataB);

			Assert.IsTrue (TypeRosetta.IsValidValue (listA, new StructuredTypeField ("X", typeA, Support.Druid.Empty, 0, FieldRelation.Collection)));
			Assert.IsFalse (TypeRosetta.IsValidValue (listB, new StructuredTypeField ("X", typeA, Support.Druid.Empty, 0, FieldRelation.Collection)));
		}

		[Test]
		public void CheckVerifyValueValidityWithNullable()
		{
			Assert.IsTrue (StringType.NativeDefault.IsNullable);
			Assert.IsFalse (IntegerType.Default.IsNullable);

			Assert.IsTrue (TypeRosetta.IsValidValue ("Abc", new StructuredTypeField ("X", StringType.NativeDefault, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)));
			Assert.IsTrue (TypeRosetta.IsValidValue (null, new StructuredTypeField ("X", StringType.NativeDefault, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)));
			Assert.IsTrue (TypeRosetta.IsValidValue ("Abc", new StructuredTypeField ("X", StringType.NativeDefault, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.Nullable, null)));
			Assert.IsTrue (TypeRosetta.IsValidValue (null, new StructuredTypeField ("X", StringType.NativeDefault, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.Nullable, null)));

			int? numNull = null;
			int? num1234 = 1234;
			
			Assert.IsTrue (TypeRosetta.IsValidValue (123, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)));
			Assert.IsFalse (TypeRosetta.IsValidValue (null, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)));
			Assert.IsTrue (TypeRosetta.IsValidValue (123, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.Nullable, null)));
			Assert.IsTrue (TypeRosetta.IsValidValue (null, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.Nullable, null)));
			
			Assert.IsTrue (TypeRosetta.IsValidValue (num1234, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)));
			Assert.IsFalse (TypeRosetta.IsValidValue (numNull, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)));
			Assert.IsTrue (TypeRosetta.IsValidValue (num1234, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.Nullable, null)));
			Assert.IsTrue (TypeRosetta.IsValidValue (numNull, new StructuredTypeField ("X", IntegerType.Default, Support.Druid.Empty, 0, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.Nullable, null)));
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

			public void AttachListener(string id, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public void DetachListener(string id, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public void SetValue(string id, object value)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public IEnumerable<string> GetValueIds()
			{
				yield return "Self";
			}

			public object GetValue(string id)
			{
				if (id == "Self")
				{
					return this;
				}
				
				throw new System.ArgumentException ();
			}

			public void SetValue(string id, object value, ValueStoreSetMode mode)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			#endregion
		}

		#endregion
	}
}
