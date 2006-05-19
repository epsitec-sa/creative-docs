//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class StructuredTest
	{
		[Test]
		public void CheckStructuredRecordTypeIsPathValid()
		{
			StructuredRecordType record = new StructuredRecordType ();

			StructuredTest.Fill (record);

			Assert.IsTrue (record.IsPathValid ("Number1"));
			Assert.IsTrue (record.IsPathValid ("Number2"));
			Assert.IsTrue (record.IsPathValid ("Text1"));
			Assert.IsFalse (record.IsPathValid ("Text2"));
			Assert.IsFalse (record.IsPathValid (null));
			Assert.IsFalse (record.IsPathValid (""));
			Assert.IsTrue (record.IsPathValid ("Personne"));
			Assert.IsTrue (record.IsPathValid ("Personne.Nom"));
			Assert.IsTrue (record.IsPathValid ("Personne.Prénom"));
			Assert.IsTrue (record.IsPathValid ("Personne.Adresse"));
			Assert.IsTrue (record.IsPathValid ("Personne.Adresse.NPA"));
			Assert.IsTrue (record.IsPathValid ("Personne.Adresse.Ville"));
		}

		[Test]
		public void CheckStructuredRecordTypeGetFieldType()
		{
			StructuredRecordType record = new StructuredRecordType ();

			StructuredTest.Fill (record);

			Assert.IsTrue (record.GetFieldTypeFromPath ("Number1") is DecimalType);
			Assert.IsTrue (record.GetFieldTypeFromPath ("Text1") is StringType);
			
			Assert.IsTrue (record.GetFieldTypeFromPath ("Personne") is StructuredRecordType);
			Assert.IsTrue (record.GetFieldTypeFromPath ("Personne.Adresse") is StructuredRecordType);
			Assert.IsTrue (record.GetFieldTypeFromPath ("Personne.Adresse.NPA") is IntegerType);
		}

		[Test]
		public void CheckStructuredTree()
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
		}

		[Test]
		public void CheckStructuredRecordTree()
		{
			StructuredRecordType type = new StructuredRecordType ();
			StructuredRecord record = new StructuredRecord (null);

			Assert.AreEqual (0, type.GetFieldNames ().Length);
			Assert.AreEqual (0, record.GetFieldNames ().Length);

			StructuredTest.Fill (type);

			record = new StructuredRecord (type);

			Assert.AreEqual ("Number1/Number2/Personne/Text1", string.Join ("/", type.GetFieldNames ()));
			Assert.AreEqual ("Personne.Adresse/Personne.Nom/Personne.Prénom", string.Join ("/", type.GetFieldPaths ("Personne")));
			Assert.AreEqual ("Personne.Adresse.NPA/Personne.Adresse.Ville", string.Join ("/", type.GetFieldPaths ("Personne.Adresse")));

			Assert.IsNull (type.GetFieldPaths ("Number1"));
			Assert.IsNull (type.GetFieldPaths ("Personne.Adresse.Ville"));
			Assert.IsNull (type.GetFieldPaths ("X"));

			Assert.AreEqual ("Number1/Number2/Personne/Text1", string.Join ("/", record.GetFieldNames ()));
			Assert.AreEqual ("Personne.Adresse/Personne.Nom/Personne.Prénom", string.Join ("/", record.GetFieldPaths ("Personne")));
			Assert.AreEqual ("Personne.Adresse.NPA/Personne.Adresse.Ville", string.Join ("/", record.GetFieldPaths ("Personne.Adresse")));

			Assert.IsNull (record.GetFieldPaths ("Number1"));
			Assert.IsNull (record.GetFieldPaths ("Personne.Adresse.Ville"));
			Assert.IsNull (record.GetFieldPaths ("X"));

			Assert.AreEqual (typeof (DecimalType), record.GetFieldTypeObject ("Number1").GetType ());
			Assert.AreEqual (typeof (StringType), record.GetFieldTypeObject ("Text1").GetType ());
			Assert.AreEqual (typeof (StructuredRecordType), record.GetFieldTypeObject ("Personne").GetType ());
		}

		private static void Fill(StructuredRecordType record)
		{
			StructuredRecordType subRec1 = new StructuredRecordType ();
			StructuredRecordType subRec2 = new StructuredRecordType ();

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
