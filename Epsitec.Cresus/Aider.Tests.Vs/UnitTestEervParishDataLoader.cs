using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.IO;

using System.Linq;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestEervParishDataLoader
	{


		[TestMethod]
		public void LoadEervHouseholdsTest()
		{
			var households = EervParishDataLoader.LoadEervHouseholds (this.PersonsFile).ToList ();

			Assert.AreEqual (4594, households.Count);

			var h1 = new EervHousehold ("4030000000", new EervAddress (null, "Route de Genève", 24, null, "1131", "Tolochenaz"), new EervCoordinates (null, "801 71 37", null, null, null), null);
			this.CheckForEquality (h1, households[0]);

			var h2 = new EervHousehold ("4030010010", new EervAddress (null, "Avenue des Reneveyres", 14, "B", "1110", "Morges"), new EervCoordinates ("801 89 10", null, null, null, null), null);
			this.CheckForEquality (h2, households[7]);

			var h3 = new EervHousehold ("4030044894", new EervAddress (null, "Chemin du Pré", 3, null, "1110", "Morges"), new EervCoordinates ("802 20 26", null, null, "802 63 39", null), null);
			this.CheckForEquality (h3, households[1235]);

			var h4 = new EervHousehold ("4030042257", new EervAddress ("Bureau d'ingénieurs SIA", "Rue des Charpentiers", 36, null, "1110", "Morges"), new EervCoordinates ("021 802 32 55", "021 804 75 40", null, null, null), null);
			this.CheckForEquality (h4, households[1988]);
		}


		[TestMethod]
		public void LoadEervPersonsTest()
		{
			var persons = EervParishDataLoader.LoadEervPersons (this.PersonsFile).ToList ();

			Assert.AreEqual (7255, persons.Count);

			var p1 = Tuple.Create (new EervPerson ("4030000000", "Jacques-André", "Henry", null, null, null, "Monsieur", PersonSex.Male, PersonMaritalStatus.Married, null, null, PersonConfession.Evangelic, null, null, null, null, null, null, null, null, null, null, null, new EervCoordinates (null, null, null, null, "jajah@bluewin.ch")), Tuple.Create ("4030000000", (int?) 1));
			this.CheckForEquality (p1, persons[0]);

			var p2 = Tuple.Create (new EervPerson ("4030015525", "Anne-Marie", "Aberghouss", "Combremont", new Date (1943, 7, 25), null, "Madame", PersonSex.Female, PersonMaritalStatus.Divorced, "Grandcour", null, PersonConfession.Protestant, null, "Combremont Gérard", "Cusin Lucie", null, null, null, null, null, null, null, null, new EervCoordinates (null, null, null, null, null)), Tuple.Create ("4030010002", (int?) 1));
			this.CheckForEquality (p2, persons[9]);

			var p3 = Tuple.Create (new EervPerson ("4030044955", "Sabrina Françoise", "Bezençon", null, new Date (1988, 12, 10), null, "Mademoiselle", PersonSex.Female, PersonMaritalStatus.Single, "Eclagnens", "Employée atelier protégé", PersonConfession.Protestant, "Ne désire plus recevoir le soutien financier", "Bezençon Jean-Claude", "Cevey Patricia José", null, null, null, null, null, null, null, null, new EervCoordinates (null, null, null, null, null)), Tuple.Create ("4030044414", (int?) 1));
			this.CheckForEquality (p3, persons[5425]);

			var p4 = Tuple.Create (new EervPerson ("4030027188", "Ghislaine", "Mounoud", "Hiehle", new Date (1929, 7, 10), new Date (2003, 8, 23), "Madame", PersonSex.Female, PersonMaritalStatus.None, "Les Thioleyres / VD", null, PersonConfession.Protestant, null, "Hiehle René Arthur", "Gabriel Mathilde Maria Elisa", null, null, null, null, null, null, null, null, new EervCoordinates (null, null, null, null, null)), Tuple.Create ("4030014392", (int?) 2));
			this.CheckForEquality (p4, persons[4132]);

			var p5 = Tuple.Create (new EervPerson ("4030045031", "Mélya Orange", "Bulundwe", null, new Date (2006, 9, 20), null, "Mademoiselle", PersonSex.Female, PersonMaritalStatus.Single, "Avenches / Donatyre", null, PersonConfession.Protestant, null, "Bulundwe Mathieu", "Bulundwe Jacques Charlène", null, null, null, "Eglise d'Echichens", new Date (2007, 8, 4), null, null, null, new EervCoordinates (null, null, null, null, "mathieu.bulundway@sanitas.com")), Tuple.Create ("4030044476", (int?) 4));
			this.CheckForEquality (p5, persons[5456]);

			var p6 = Tuple.Create (new EervPerson ("4030001274", "Nicolas", "Cruchon", null, new Date (1994, 5, 17), null, "Monsieur", PersonSex.Male, PersonMaritalStatus.Single, "Bercher", null, PersonConfession.Protestant, null, "Cruchon Alexandre", "Bindelli Nadia", "Morges VD", "Echichens", new Date (1994, 9, 4), null, null, "Temple de Morges", new Date (2010, 3, 28), 1, new EervCoordinates (null, null, "079/397 78 78", null, "cavedusignal@bluewin.ch")), Tuple.Create ("4030011487", (int?) 4));
			this.CheckForEquality (p6, persons[677]);
		}


		[TestMethod]
		public void LoadEervLegalPersonsTest()
		{
			var legalPersons = EervParishDataLoader.LoadEervLegalPersons (this.PersonsFile).ToList ();

			Assert.AreEqual (34, legalPersons.Count);

			var p1 = new EervLegalPerson ("4030043016", "Services sociaux Couvaloup", new EervAddress (null, "Rue de Couvaloup", 10, null, "1110", "Morges"), new EervCoordinates (null, null, null, null, null))
			{
				ContactPerson = new EervPerson ("4030043016", "Jacques", "Baudat", null, null, null, "Monsieur", PersonSex.Male, PersonMaritalStatus.None, null, null, PersonConfession.Protestant, null, null, null, null, null, null, null, null, null, null, null, new EervCoordinates (null, null, null, null, null))
			};
			this.CheckForEquality (p1, legalPersons[0]);

			var p2 = new EervLegalPerson ("4030043037", "Paroisse catholique de Morges", new EervAddress (null, "Route du Rond-Point", 2, null, "1110", "Morges"), new EervCoordinates ("801 24 35", null, null, "803 14 94", null))
			{
				ContactPerson = new EervPerson ("4030043037", null, "Secrétariat", null, null, null, null, PersonSex.Unknown, PersonMaritalStatus.None, null, null, PersonConfession.Unknown, null, null, null, null, null, null, null, null, null, null, null, new EervCoordinates (null, null, null, null, null))
			};
			this.CheckForEquality (p2, legalPersons[6]);
		}


		[TestMethod]
		public void LoadEervGroupsTest()
		{
			var groups = EervParishDataLoader.LoadEervGroups (this.GroupFile).ToList ();

			Assert.AreEqual (171, groups.Count);

			var g1 = new EervGroup ("4030000008", "Office de Taizé");
			g1.GroupDefinitionIds.Add ("0403110100");
			g1.GroupDefinitionIds.Add ("0403060100");
			this.CheckForEquality (g1, groups[0]);

			var g2 = new EervGroup ("4030000103", "Groupes d'adultes");
			g2.GroupDefinitionIds.Add ("0403111700");
			this.CheckForEquality (g2, groups[16]);
		}


		[TestMethod]
		public void LoadEervActivitiesTest()
		{
			var activities = EervParishDataLoader.LoadEervActivities (this.ActivityFile).ToList ();

			Assert.AreEqual (7313, activities.Count);

			var a1 = Tuple.Create (new EervActivity (new Date (2004, 8, 20), null, null), "4030010057", "4030000147");
			this.CheckForEquality (a1, activities[0]);

			var a2 = Tuple.Create (new EervActivity (new Date (2005, 8, 22), new Date (2006, 8, 21), null), "4030042696", "4030000130");
			this.CheckForEquality (a2, activities[2318]);
		}


		private void CheckForEquality(Tuple<EervActivity, string, string> expected, Tuple<EervActivity, string, string> actual)
		{
			Assert.AreEqual (expected.Item1.StartDate, actual.Item1.StartDate);
			Assert.AreEqual (expected.Item1.EndDate, actual.Item1.EndDate);
			Assert.AreEqual (expected.Item1.Remarks, actual.Item1.Remarks);
			Assert.AreEqual (expected.Item2, actual.Item2);
			Assert.AreEqual (expected.Item3, actual.Item3);
		}


		private void CheckForEquality(EervGroup expected, EervGroup actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Name, actual.Name);
			Assert.AreEqual (expected.GroupDefinitionIds.Count, actual.GroupDefinitionIds.Count);

			for (int i = 0; i < expected.GroupDefinitionIds.Count; i++)
			{
				Assert.AreEqual (expected.GroupDefinitionIds[i], actual.GroupDefinitionIds[i]);
			}
		}


		private void CheckForEquality(EervHousehold expected, EervHousehold actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Remarks, actual.Remarks);
			this.CheckForEquality (expected.Address, actual.Address);
			this.CheckForEquality (expected.Coordinates, actual.Coordinates);
		}


		private void CheckForEquality(EervCoordinates expected, EervCoordinates actual)
		{
			Assert.AreEqual (expected.FaxNumber, actual.FaxNumber);
			Assert.AreEqual (expected.PrivatePhoneNumber, actual.PrivatePhoneNumber);
			Assert.AreEqual (expected.ProfessionalPhoneNumber, actual.ProfessionalPhoneNumber);
			Assert.AreEqual (expected.EmailAddress, actual.EmailAddress);
			Assert.AreEqual (expected.MobilePhoneNumber, actual.MobilePhoneNumber);
		}


		private void CheckForEquality(EervAddress expected, EervAddress actual)
		{
			Assert.AreEqual (expected.FirstAddressLine, actual.FirstAddressLine);
			Assert.AreEqual (expected.StreetName, actual.StreetName);
			Assert.AreEqual (expected.HouseNumber, actual.HouseNumber);
			Assert.AreEqual (expected.HouseNumberComplement, actual.HouseNumberComplement);
			Assert.AreEqual (expected.ZipCode, actual.ZipCode);
			Assert.AreEqual (expected.Town, actual.Town);
		}


		private void CheckForEquality(Tuple<EervPerson, Tuple<string, int?>> expected, Tuple<EervPerson, Tuple<string, int?>> actual)
		{
			this.CheckForEquality (expected.Item1, actual.Item1);
			Assert.AreEqual (expected.Item2.Item1, actual.Item2.Item1);
			Assert.AreEqual (expected.Item2.Item2, actual.Item2.Item2);
		}


		private void CheckForEquality(EervPerson expected, EervPerson actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Firstname, actual.Firstname);
			Assert.AreEqual (expected.Lastname, actual.Lastname);
			Assert.AreEqual (expected.OriginalName, actual.OriginalName);
			Assert.AreEqual (expected.DateOfBirth, actual.DateOfBirth);
			Assert.AreEqual (expected.DateOfDeath, actual.DateOfDeath);
			Assert.AreEqual (expected.Honorific, actual.Honorific);
			Assert.AreEqual (expected.Sex, actual.Sex);
			Assert.AreEqual (expected.MaritalStatus, actual.MaritalStatus);
			Assert.AreEqual (expected.Origins, actual.Origins);
			Assert.AreEqual (expected.Profession, actual.Profession);
			Assert.AreEqual (expected.Confession, actual.Confession);
			Assert.AreEqual (expected.Remarks, actual.Remarks);
			Assert.AreEqual (expected.Father, actual.Father);
			Assert.AreEqual (expected.Mother, actual.Mother);
			Assert.AreEqual (expected.PlaceOfBirth, actual.PlaceOfBirth);
			Assert.AreEqual (expected.PlaceOfBaptism, actual.PlaceOfBaptism);
			Assert.AreEqual (expected.DateOfBaptism, actual.DateOfBaptism);
			Assert.AreEqual (expected.PlaceOfChildBenediction, actual.PlaceOfChildBenediction);
			Assert.AreEqual (expected.DateOfChildBenediction, actual.DateOfChildBenediction);
			Assert.AreEqual (expected.PlaceOfCatechismBenediction, actual.PlaceOfCatechismBenediction);
			Assert.AreEqual (expected.DateOfCatechismBenediction, actual.DateOfCatechismBenediction);
			Assert.AreEqual (expected.SchoolYearOffset, actual.SchoolYearOffset);
			this.CheckForEquality (expected.Coordinates, actual.Coordinates);
		}


		private void CheckForEquality(EervLegalPerson expected, EervLegalPerson actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Name, actual.Name);

			this.CheckForEquality (expected.Address, actual.Address);
			this.CheckForEquality (expected.Coordinates, actual.Coordinates);
			this.CheckForEquality (expected.ContactPerson, actual.ContactPerson);
		}


		private readonly FileInfo PersonsFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Personnes.xlsx");
		private readonly FileInfo GroupFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Groupes.xlsx");
		private readonly FileInfo ActivityFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Activites.xlsx");


	}


}
