//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class StructuredTest
	{
		[Test]
		public void CheckStructuredData()
		{
			StructuredData data = new StructuredData ();

			Assert.AreEqual (0, Collection.Count (data.GetValueNames ()));
			Assert.AreEqual (0, Collection.Count (data.StructuredType.GetFieldIds ()));

			data.SetValue ("A", 10);
			data.SetValue ("B", 20);

			Assert.AreEqual (2, Collection.Count (data.GetValueNames ()));
			Assert.AreEqual (2, Collection.Count (data.StructuredType.GetFieldIds ()));

			Assert.AreEqual ("A", Collection.Extract (data.StructuredType.GetFieldIds (), 0));
			Assert.AreEqual ("B", Collection.Extract (data.StructuredType.GetFieldIds (), 1));

			Assert.AreEqual (typeof (int), data.StructuredType.GetField ("A").Type.SystemType);
			Assert.AreEqual (10, data.GetValue ("A"));
			Assert.AreEqual (20, data.GetValue ("B"));
			Assert.AreEqual (UndefinedValue.Instance, data.GetValue ("X"));
			
			data.SetValue ("A", UndefinedValue.Instance);

			Assert.AreEqual (1, Collection.Count (data.GetValueNames ()));
			Assert.AreEqual (1, Collection.Count (data.StructuredType.GetFieldIds ()));

			Assert.AreEqual ("B", Collection.Extract (data.StructuredType.GetFieldIds (), 0));

		}

		[Test]
		public void CheckStructuredDataWithType()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("A", new IntegerType (0, 100));
			type.Fields.Add ("B", new IntegerType (0, 100));

			Assert.AreEqual (2, Collection.Count (data.GetValueNames ()));
			Assert.AreEqual (2, Collection.Count (data.StructuredType.GetFieldIds ()));

			Assert.AreEqual ("A", Collection.Extract (data.StructuredType.GetFieldIds (), 0));
			Assert.AreEqual ("B", Collection.Extract (data.StructuredType.GetFieldIds (), 1));

			Assert.AreEqual (UndefinedValue.Instance, data.GetValue ("A"));
			Assert.AreEqual (UndefinedValue.Instance, data.GetValue ("B"));

			Assert.AreEqual (-1, data.InternalGetValueCount ());
			
			data.SetValue ("A", 10);
			data.SetValue ("B", 20);

			Assert.AreEqual (2, data.InternalGetValueCount ());
			Assert.AreEqual (typeof (IntegerType), data.StructuredType.GetField ("A").Type.GetType ());
			Assert.AreEqual (10, data.GetValue ("A"));
			Assert.AreEqual (20, data.GetValue ("B"));
			
			this.buffer.Length = 0;

			data.AttachListener ("A", this.HandleDataPropertyChanged);
			data.SetValue ("A", 15);
			data.SetValue ("A", UndefinedValue.Instance);
			Assert.AreEqual (2, data.InternalGetValueCount ());
			data.DetachListener ("A", this.HandleDataPropertyChanged);
			Assert.AreEqual (1, data.InternalGetValueCount ());
			data.SetValue ("A", 10);
			Assert.AreEqual (2, data.InternalGetValueCount ());
			
			Assert.AreEqual ("[A:10->15][A:15-><UndefinedValue>]", this.buffer.ToString ());

			data.SetValue ("A", UndefinedValue.Instance);
			data.SetValue ("B", UndefinedValue.Instance);
			Assert.AreEqual (0, data.InternalGetValueCount ());
		}

		private System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

		private void HandleDataPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.buffer.Append ("[");
			this.buffer.Append (e.PropertyName);
			this.buffer.Append (":");
			this.buffer.Append (e.OldValue);
			this.buffer.Append ("->");
			this.buffer.Append (e.NewValue);
			this.buffer.Append ("]");
		}

		[Test]
		[ExpectedException (typeof (System.Collections.Generic.KeyNotFoundException))]
		public void CheckStructuredDataWithTypeEx1()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("A", IntegerType.Default);
			type.Fields.Add ("B", IntegerType.Default);

			data.SetValue ("X", 100);
		}

		[Test]
		[ExpectedException (typeof (System.Collections.Generic.KeyNotFoundException))]
		public void CheckStructuredDataWithTypeEx2()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("A", IntegerType.Default);
			type.Fields.Add ("B", IntegerType.Default);

			data.GetValue ("X");
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckStructuredDataWithTypeEx3()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("A", new IntegerType (0, 100));

			data.SetValue ("A", 200);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckStructuredDataWithTypeEx4()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("A", new IntegerType (0, 100));

			data.SetValue ("A", "-");
		}

		[Test]
		public void CheckStructuredTreeIsPathValid()
		{
			StructuredType record = new StructuredType ();

			StructuredTest.Fill (record);

			Assert.IsTrue (StructuredTree.IsPathValid (record, "Number1"));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Number2"));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Text1"));
			Assert.IsFalse (StructuredTree.IsPathValid (record, "Text2"));
			Assert.IsFalse (StructuredTree.IsPathValid (record, null));
			Assert.IsFalse (StructuredTree.IsPathValid (record, ""));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Personne"));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Personne.Nom"));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Personne.Prénom"));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Personne.Adresse"));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Personne.Adresse.NPA"));
			Assert.IsTrue (StructuredTree.IsPathValid (record, "Personne.Adresse.Ville"));
		}

		[Test]
		public void CheckStructuredTreeGetField()
		{
			StructuredType record = new StructuredType ();

			StructuredTest.Fill (record);

			Assert.IsTrue (StructuredTree.GetField (record, "Number1").Type is DecimalType);
			Assert.IsTrue (StructuredTree.GetField (record, "Text1").Type is StringType);

			Assert.IsTrue (StructuredTree.GetField (record, "Personne").Type is StructuredType);
			Assert.IsTrue (StructuredTree.GetField (record, "Personne.Adresse").Type is StructuredType);
			Assert.IsTrue (StructuredTree.GetField (record, "Personne.Adresse.NPA").Type is IntegerType);
		}

		[Test]
		public void CheckStructuredTreeMisc()
		{
			Assert.AreEqual ("a*b*c", string.Join ("*", StructuredTree.SplitPath ("a.b.c")));
			Assert.AreEqual ("a.b.c.d", StructuredTree.CreatePath ("a", "b", "c.d"));
			Assert.AreEqual ("", StructuredTree.CreatePath ());
			Assert.AreEqual ("", StructuredTree.CreatePath (""));
			Assert.AreEqual (0, StructuredTree.SplitPath (null).Length);
			Assert.AreEqual (0, StructuredTree.SplitPath ("").Length);
			Assert.AreEqual ("a.b.c.d", StructuredTree.GetSubPath ("a.b.c.d", 0));
			Assert.AreEqual ("b.c.d", StructuredTree.GetSubPath ("a.b.c.d", 1));
			Assert.AreEqual ("c.d", StructuredTree.GetSubPath ("a.b.c.d", 2));
			Assert.AreEqual ("", StructuredTree.GetSubPath ("a.b.c.d", 10));
			Assert.AreEqual ("a", StructuredTree.GetRootName ("a.b.c.d"));
			Assert.AreEqual ("abc", StructuredTree.GetRootName ("abc"));
			Assert.AreEqual ("", StructuredTree.GetRootName (""));
			Assert.IsNull (StructuredTree.GetRootName (null));

			string leafPath;
			string leafName;

			leafPath = StructuredTree.GetLeafPath ("a.b.c.d", out leafName);

			Assert.AreEqual ("a.b.c", leafPath);
			Assert.AreEqual ("d", leafName);

			leafPath = StructuredTree.GetLeafPath ("a", out leafName);

			Assert.IsNull (leafPath);
			Assert.AreEqual ("a", leafName);

			leafPath = StructuredTree.GetLeafPath ("", out leafName);

			Assert.IsNull (leafPath);
			Assert.IsNull (leafName);
		}

		[Test]
		public void CheckStructuredTree()
		{
			StructuredType type = new StructuredType ();
			StructuredData record = new StructuredData (null);

			Assert.AreEqual (0, Collection.Count (type.GetFieldIds ()));
			
			StructuredTest.Fill (type);

			record = new StructuredData (type);

			Assert.AreEqual ("Number1/Number2/Personne/Text1", string.Join ("/", Collection.ToSortedArray (type.GetFieldIds ())));
			Assert.AreEqual ("Personne.Adresse/Personne.Nom/Personne.Prénom", string.Join ("/", Collection.ToSortedArray (StructuredTree.GetFieldPaths (type, "Personne"))));
			Assert.AreEqual ("Personne.Adresse.NPA/Personne.Adresse.Ville", string.Join ("/", Collection.ToSortedArray (StructuredTree.GetFieldPaths (type, "Personne.Adresse"))));

			Assert.AreEqual (0, Collection.Count (StructuredTree.GetFieldPaths (type, "Number1")));
			Assert.AreEqual (0, Collection.Count (StructuredTree.GetFieldPaths (type, "Personne.Adresse.Ville")));
			Assert.AreEqual (0, Collection.Count (StructuredTree.GetFieldPaths (type, "X")));

			Assert.AreEqual ("Number1/Number2/Personne/Text1", string.Join ("/", Collection.ToSortedArray (record.StructuredType.GetFieldIds ())));
			Assert.AreEqual ("Personne.Adresse/Personne.Nom/Personne.Prénom", string.Join ("/", Collection.ToSortedArray (StructuredTree.GetFieldPaths (record.StructuredType, "Personne"))));
			Assert.AreEqual ("Personne.Adresse.NPA/Personne.Adresse.Ville", string.Join ("/", Collection.ToSortedArray (StructuredTree.GetFieldPaths (record.StructuredType, "Personne.Adresse"))));

			Assert.AreEqual (0, Collection.Count (StructuredTree.GetFieldPaths (record.StructuredType, "Number1")));
			Assert.AreEqual (0, Collection.Count (StructuredTree.GetFieldPaths (record.StructuredType, "Personne.Adresse.Ville")));
			Assert.AreEqual (0, Collection.Count (StructuredTree.GetFieldPaths (record.StructuredType, "X")));

			Assert.AreEqual (typeof (DecimalType), record.StructuredType.GetField ("Number1").Type.GetType ());
			Assert.AreEqual (typeof (StringType), record.StructuredType.GetField ("Text1").Type.GetType ());
			Assert.AreEqual (typeof (StructuredType), record.StructuredType.GetField ("Personne").Type.GetType ());

			Assert.IsTrue (StructuredTree.IsPathValid (type, "Number1"));
			Assert.IsTrue (StructuredTree.IsPathValid (type, "Personne.Nom"));
			Assert.IsTrue (StructuredTree.IsPathValid (type, "Personne.Adresse.Ville"));
			Assert.IsFalse (StructuredTree.IsPathValid (type, "Personne.Adresse.Pays"));
			Assert.IsFalse (StructuredTree.IsPathValid (type, "Client.Adresse.Pays"));
		}

		[Test]
		public void CheckStructuredTypeSerialization()
		{
			StructuredType type = new StructuredType ();

			type.Caption.Name = "TestStruct";

			type.Fields.Add ("Name", StringType.Default);
			type.Fields.Add ("Age", IntegerType.Default);

			Assert.IsNull (type.SystemType);

			string xml = type.Caption.SerializeToString ();

			System.Console.Out.WriteLine ("StructuredType: {0}", xml);

			Caption caption = new Caption ();
			caption.DeserializeFromString (xml);

			StructuredType restoredType = TypeRosetta.CreateTypeObject (caption) as StructuredType;

			Assert.AreEqual (type.Fields.Count, restoredType.Fields.Count);
			Assert.AreEqual (type.GetField ("Name").Type.GetType ().Name, restoredType.GetField ("Name").Type.GetType ().Name);
			Assert.AreEqual (type.GetField ("Age").Type.GetType ().Name, restoredType.GetField ("Age").Type.GetType ().Name);
		}

		private static void Fill(StructuredType record)
		{
			StructuredType subRec1 = new StructuredType ();
			StructuredType subRec2 = new StructuredType ();

			record.Fields.Add ("Number1", new DecimalType ());
			record.Fields.Add ("Text1", new StringType ());
			record.Fields.Add ("Number2", new DecimalType (new DecimalRange (0.0M, 999.9M, 0.1M)));

			record.Fields.Add ("Personne", subRec1);

			subRec1.Fields.Add ("Nom", new StringType ());
			subRec1.Fields.Add ("Prénom", new StringType ());
			subRec1.Fields.Add ("Adresse", subRec2);

			subRec2.Fields.Add ("NPA", new IntegerType (1, 999999));
			subRec2.Fields.Add ("Ville", new StringType ());
		}
	}
}
