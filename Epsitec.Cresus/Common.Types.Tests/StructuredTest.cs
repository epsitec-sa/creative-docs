using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TestFixture] public class StructuredTest
	{
		[Test]
		public void CheckStructuredRecordType()
		{
			StructuredRecordType record = new StructuredRecordType ();
			StructuredRecordType subRec1 = new StructuredRecordType ();
			StructuredRecordType subRec2 = new StructuredRecordType ();

			record.Fields.Add ("Number1", new DecimalType ());
			record.Fields.Add ("Text1", new StringType ());
			record.Fields.Add ("Number2", new DecimalType (new DecimalRange (0.0M, 999.9M, 0.1M)));
			
			record.Fields.Add ("Personne", subRec1);

			subRec1.Fields.Add ("Nom", new StringType ());
			subRec1.Fields.Add ("Prénom", new StringType ());
			subRec1.Fields.Add ("Adresse", subRec2);

			subRec2.Fields.Add ("NPA", new DecimalType (1M, 99999M, 1M));
			subRec2.Fields.Add ("Ville", new StringType ());

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
	}
}
