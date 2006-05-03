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
