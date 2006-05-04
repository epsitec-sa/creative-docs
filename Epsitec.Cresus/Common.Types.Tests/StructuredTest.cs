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

			Assert.IsTrue (record.GetFieldType ("Number1") is DecimalType);
			Assert.IsTrue (record.GetFieldType ("Text1") is StringType);
			
			Assert.IsTrue (record.GetFieldType ("Personne") is StructuredRecordType);
			Assert.IsTrue (record.GetFieldType ("Personne.Adresse") is StructuredRecordType);
			Assert.IsTrue (record.GetFieldType ("Personne.Adresse.NPA") is IntegerType);
		}

		[Test]
		public void CheckStructuredRecordPaths()
		{
			Assert.AreEqual ("a*b*c", string.Join ("*", StructuredRecordType.SplitPath ("a.b.c")));
			Assert.AreEqual ("a.b.c.d", StructuredRecordType.CreatePath ("a", "b", "c.d"));
			Assert.AreEqual ("", StructuredRecordType.CreatePath ());
			Assert.AreEqual ("", StructuredRecordType.CreatePath (""));
			Assert.AreEqual (0, StructuredRecordType.SplitPath (null).Length);
			Assert.AreEqual (0, StructuredRecordType.SplitPath ("").Length);
		}

		[Test]
		public void CheckStructuredRecordTree()
		{
			StructuredRecordType record = new StructuredRecordType ();
			StructuredTest.Fill (record);

			Assert.AreEqual ("Number1/Number2/Personne/Text1", string.Join ("/", record.GetFieldNames ()));
			Assert.AreEqual ("Personne.Adresse/Personne.Nom/Personne.Prénom", string.Join ("/", record.GetFieldPaths ("Personne")));
			Assert.AreEqual ("Personne.Adresse.NPA/Personne.Adresse.Ville", string.Join ("/", record.GetFieldPaths ("Personne.Adresse")));

			Assert.IsNull (record.GetFieldPaths ("Number1"));
			Assert.IsNull (record.GetFieldPaths ("Personne.Adresse.Ville"));
			Assert.IsNull (record.GetFieldPaths ("X"));
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
