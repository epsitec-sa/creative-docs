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

			Assert.AreEqual (0, data.GetValueNames ().Length);
			Assert.AreEqual (0, data.StructuredType.GetFieldNames ().Length);

			data.SetValue ("A", 10);
			data.SetValue ("B", 20);

			Assert.AreEqual (2, data.GetValueNames ().Length);
			Assert.AreEqual (2, data.StructuredType.GetFieldNames ().Length);

			Assert.AreEqual ("A", data.StructuredType.GetFieldNames ()[0]);
			Assert.AreEqual ("B", data.StructuredType.GetFieldNames ()[1]);

			Assert.AreEqual (typeof (int), data.StructuredType.GetFieldTypeObject ("A"));
			Assert.AreEqual (10, data.GetValue ("A"));
			Assert.AreEqual (20, data.GetValue ("B"));
			Assert.AreEqual (UndefinedValue.Instance, data.GetValue ("X"));
		}

		[Test]
		public void CheckStructuredDataWithType()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields["A"] = new IntegerType ();
			type.Fields["B"] = new IntegerType ();

			Assert.AreEqual (2, data.GetValueNames ().Length);
			Assert.AreEqual (2, data.StructuredType.GetFieldNames ().Length);

			Assert.AreEqual ("A", data.StructuredType.GetFieldNames ()[0]);
			Assert.AreEqual ("B", data.StructuredType.GetFieldNames ()[1]);

			Assert.AreEqual (UndefinedValue.Instance, data.GetValue ("A"));
			Assert.AreEqual (UndefinedValue.Instance, data.GetValue ("B"));
			
			data.SetValue ("A", 10);
			data.SetValue ("B", 20);

			Assert.AreEqual (typeof (IntegerType), data.StructuredType.GetFieldTypeObject ("A").GetType ());
			Assert.AreEqual (10, data.GetValue ("A"));
			Assert.AreEqual (20, data.GetValue ("B"));
		}

		[Test]
		[ExpectedException (typeof (System.Collections.Generic.KeyNotFoundException))]
		public void CheckStructuredDataWithTypeEx1()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields["A"] = new IntegerType ();
			type.Fields["B"] = new IntegerType ();

			data.SetValue ("X", 100);
		}

		[Test]
		[ExpectedException (typeof (System.Collections.Generic.KeyNotFoundException))]
		public void CheckStructuredDataWithTypeEx2()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields["A"] = new IntegerType ();
			type.Fields["B"] = new IntegerType ();

			data.GetValue ("X");
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
		public void CheckStructuredTreeGetFieldType()
		{
			StructuredType record = new StructuredType ();

			StructuredTest.Fill (record);

			Assert.IsTrue (StructuredTree.GetFieldType (record, "Number1") is DecimalType);
			Assert.IsTrue (StructuredTree.GetFieldType (record, "Text1") is StringType);

			Assert.IsTrue (StructuredTree.GetFieldType (record, "Personne") is StructuredType);
			Assert.IsTrue (StructuredTree.GetFieldType (record, "Personne.Adresse") is StructuredType);
			Assert.IsTrue (StructuredTree.GetFieldType (record, "Personne.Adresse.NPA") is IntegerType);
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

			Assert.AreEqual (0, type.GetFieldNames ().Length);
			
			StructuredTest.Fill (type);

			record = new StructuredData (type);

			Assert.AreEqual ("Number1/Number2/Personne/Text1", string.Join ("/", type.GetFieldNames ()));
			Assert.AreEqual ("Personne.Adresse/Personne.Nom/Personne.Prénom", string.Join ("/", StructuredTree.GetFieldPaths (type, "Personne")));
			Assert.AreEqual ("Personne.Adresse.NPA/Personne.Adresse.Ville", string.Join ("/", StructuredTree.GetFieldPaths (type, "Personne.Adresse")));

			Assert.IsNull (StructuredTree.GetFieldPaths (type, "Number1"));
			Assert.IsNull (StructuredTree.GetFieldPaths (type, "Personne.Adresse.Ville"));
			Assert.IsNull (StructuredTree.GetFieldPaths (type, "X"));

			Assert.AreEqual ("Number1/Number2/Personne/Text1", string.Join ("/", record.StructuredType.GetFieldNames ()));
			Assert.AreEqual ("Personne.Adresse/Personne.Nom/Personne.Prénom", string.Join ("/", StructuredTree.GetFieldPaths (record.StructuredType, "Personne")));
			Assert.AreEqual ("Personne.Adresse.NPA/Personne.Adresse.Ville", string.Join ("/", StructuredTree.GetFieldPaths (record.StructuredType, "Personne.Adresse")));

			Assert.IsNull (StructuredTree.GetFieldPaths (record.StructuredType, "Number1"));
			Assert.IsNull (StructuredTree.GetFieldPaths (record.StructuredType, "Personne.Adresse.Ville"));
			Assert.IsNull (StructuredTree.GetFieldPaths (record.StructuredType, "X"));

			Assert.AreEqual (typeof (DecimalType), record.StructuredType.GetFieldTypeObject ("Number1").GetType ());
			Assert.AreEqual (typeof (StringType), record.StructuredType.GetFieldTypeObject ("Text1").GetType ());
			Assert.AreEqual (typeof (StructuredType), record.StructuredType.GetFieldTypeObject ("Personne").GetType ());

			Assert.IsTrue (StructuredTree.IsPathValid (type, "Number1"));
			Assert.IsTrue (StructuredTree.IsPathValid (type, "Personne.Nom"));
			Assert.IsTrue (StructuredTree.IsPathValid (type, "Personne.Adresse.Ville"));
			Assert.IsFalse (StructuredTree.IsPathValid (type, "Personne.Adresse.Pays"));
			Assert.IsFalse (StructuredTree.IsPathValid (type, "Client.Adresse.Pays"));
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
