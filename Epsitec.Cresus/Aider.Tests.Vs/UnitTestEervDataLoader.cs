using Epsitec.Aider.Data.Eerv;

using Epsitec.Common.Types;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using System.Linq;
using Epsitec.Aider.Enumerations;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestEervDataLoader
	{


		[TestMethod]
		public void LoadEervHouseholdsTest()
		{
			var households = EervDataLoader.LoadEervHouseholds (this.PersonsFile).ToList ();

			Assert.AreEqual (4628, households.Count);

			var h1 = new EervHousehold ("4030000000", "", "Route de Genève", 24, "", "1131", "Tolochenaz", "", "", "801 71 37", "");
			this.CheckForEquality (h1, households[0]);

			var h2 = new EervHousehold ("4030042793", "CP 393", "Chemin du Crêt", 2, "", "1110", "Morges", "", "", "804 22 11", "");
			this.CheckForEquality (h2, households[7]);

			var h3 = new EervHousehold ("4030042802", "", "Route du Rond-Point", 2, "", "1110", "Morges", "803 14 94", "801 24 35", "", "");
			this.CheckForEquality (h3, households[10]);

			var h4 = new EervHousehold ("4030010010", "", "Avenue des Reneveyres", 14, "B", "1110", "Morges", "", "801 89 10", "", "");
		}


		[TestMethod]
		public void LoadEervPersonsTest()
		{
			var persons = EervDataLoader.LoadEervPersons (this.PersonsFile).ToList ();

			Assert.AreEqual (7289, persons.Count);

			var p1 = new EervPerson ("4030000000", "Jacques-André", "", "Henry", "", "", null, null, "Monsieur", PersonSex.Male, PersonMaritalStatus.Married, "", "", PersonConfession.Evangelic, "jajah@bluewin.ch", "", "", "", "", "", "", null, "", null, "", null, null, "4030000000", 1);
			this.CheckForEquality (p1, persons[0]);

			var p2 = new EervPerson ("4030015525", "Anne-Marie", "", "Aberghouss", "Combremont", "", new Date (1943, 7, 25), null, "Madame", PersonSex.Female, PersonMaritalStatus.Divorced, "Grandcour", "", PersonConfession.Protestant, "", "", "", "Combremont Gérard", "Cusin Lucie", "", "", null, "", null, "", null, null, "4030010002", 1);
			this.CheckForEquality (p2, persons[16]);

			var p3 = new EervPerson ("4030043030", "David", "", "Richir", "", "Eglise Evangélique l’Oasis", null, null, "Monsieur", PersonSex.Male, PersonMaritalStatus.Married, "", "", PersonConfession.Evangelic, "david.richir@lafree.ch", "077 449 27 91", "", "", "", "", "", null, "", null, "", null, null, "4030042795", 1);
			this.CheckForEquality (p3, persons[9]);

			var p4 = new EervPerson ("4030044955", "Sabrina", "Françoise", "Bezençon", "", "", new Date (1988, 12, 10), null, "Mademoiselle", PersonSex.Female, PersonMaritalStatus.Single, "Eclagnens", "Employée atelier protégé", PersonConfession.Protestant, "", "", "Ne désire plus recevoir le soutien financier", "Bezençon Jean-Claude", "Cevey Patricia José", "", "", null, "", null, "", null, null, "4030044414", 1);
			this.CheckForEquality (p4, persons[5453]);

			var p5 = new EervPerson ("4030027188", "Ghislaine", "", "Mounoud", "Hiehle", "", new Date (1929, 7, 10), new Date (2003, 8, 23), "Madame", PersonSex.Female, PersonMaritalStatus.None, "Les Thioleyres / VD", "", PersonConfession.Protestant, "", "", "", "Hiehle René Arthur", "Gabriel Mathilde Maria Elisa", "", "", null, "", null, "", null, null, "4030014392", 2);
			this.CheckForEquality (p5, persons[4154]);

			var p6 = new EervPerson ("4030045031", "Mélya Orange", "", "Bulundwe", "", "", new Date (2006, 9, 20), null, "Mademoiselle", PersonSex.Female, PersonMaritalStatus.Single, "Avenches / Donatyre", "", PersonConfession.Protestant, "mathieu.bulundway@sanitas.com", "", "", "Bulundwe Mathieu", "Bulundwe Jacques Charlène", "", "", null, "Eglise d'Echichens", new Date (2007, 8, 4), "", null, null, "4030044476", 4);
			this.CheckForEquality (p6, persons[5486]);

			var p7 = new EervPerson ("4030001274", "Nicolas", "", "Cruchon", "", "", new Date (1994, 5, 17), null, "Monsieur", PersonSex.Male, PersonMaritalStatus.Single, "Bercher", "", PersonConfession.Protestant, "cavedusignal@bluewin.ch", "079/397 78 78", "", "Cruchon Alexandre", "Bindelli Nadia", "Morges VD", "Echichens", new Date (1994, 9, 4), "", null, "Temple de Morges", new Date (2010, 3, 28), 1, "4030011487", 4);
			this.CheckForEquality (p7, persons[685]);
		}


		[TestMethod]
		public void LoadEervGroupDefinitionsTest()
		{
			var groupDefinitions = EervDataLoader.LoadEervGroupDefinitions (this.GroupDefinitionFile).ToList ();

			Assert.AreEqual (568, groupDefinitions.Count);

			var g1 = new EervGroupDefinition ("0100000000", "Paramètres transversaux", null);
			this.CheckForEquality (g1, groupDefinitions[0]);

			var g2 = new EervGroupDefinition ("0102000000", "Personnel EERV", "0100000000");
			this.CheckForEquality (g2, groupDefinitions[90]);

			var g3 = new EervGroupDefinition ("0301010000", "Assemblée", "0301000000");
			this.CheckForEquality (g3, groupDefinitions[250]);

			var g4 = new EervGroupDefinition ("0303030100", "Catéchumènes 2007-09-11", "0303030000");
			this.CheckForEquality (g4, groupDefinitions[285]);

			var g5 = new EervGroupDefinition ("0604010203", "Délégués CER", "0604010200");
			this.CheckForEquality (g5, groupDefinitions[509]);
		}


		[TestMethod]
		public void LoadEervGroupsTest()
		{
			var groups = EervDataLoader.LoadEervGroups (this.GroupFile).ToList ();

			Assert.AreEqual (204, groups.Count);

			var g1 = new EervGroup ("4030000008", "0403110100", "Office de Taizé");
			this.CheckForEquality (g1, groups[0]);

			var g2 = new EervGroup ("4030000015", "0403110200", "Parkings des Jardins - Locataires");
			this.CheckForEquality (g2, groups[1]);
		}


		[TestMethod]
		public void LoadEervActivitiesTest()
		{
			var activities = EervDataLoader.LoadEervActivities (this.ActivityFile).ToList ();

			Assert.AreEqual (7313, activities.Count);

			var a1 = new EervActivity ("4030010057", "4030000147", new Date (2004, 8, 20), null, "");
			this.CheckForEquality (a1, activities[0]);

			var a2 = new EervActivity ("4030042696", "4030000130", new Date(2005, 8, 22), new Date (2006, 8, 21), "");
			this.CheckForEquality (a2, activities[2318]);
		}


		private void CheckForEquality(EervActivity expected, EervActivity actual)
		{
			Assert.AreEqual (expected.PersonId, actual.PersonId);
			Assert.AreEqual (expected.GroupId, actual.GroupId);
			Assert.AreEqual (expected.StartDate, actual.StartDate);
			Assert.AreEqual (expected.EndDate, actual.EndDate);
			Assert.AreEqual (expected.Remarks, actual.Remarks);
		}


		private void CheckForEquality(EervGroup expected, EervGroup actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.DefinitionId, actual.DefinitionId);
			Assert.AreEqual (expected.Name, actual.Name);
		}


		private void CheckForEquality(EervGroupDefinition expected, EervGroupDefinition actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Name, actual.Name);
			Assert.AreEqual (expected.ParentId, actual.ParentId);
		}


		private void CheckForEquality(EervHousehold expected, EervHousehold actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.FirstAddressLine, actual.FirstAddressLine);
			Assert.AreEqual (expected.StreetName, actual.StreetName);
			Assert.AreEqual (expected.HouseNumber, actual.HouseNumber);
			Assert.AreEqual (expected.HouseNumberComplement, actual.HouseNumberComplement);
			Assert.AreEqual (expected.ZipCode, actual.ZipCode);
			Assert.AreEqual (expected.City, actual.City);
			Assert.AreEqual (expected.FaxNumber, actual.FaxNumber);
			Assert.AreEqual (expected.PrivatePhoneNumber, actual.PrivatePhoneNumber);
			Assert.AreEqual (expected.ProfessionalPhoneNumber, actual.ProfessionalPhoneNumber);
			Assert.AreEqual (expected.Remarks, actual.Remarks);
		}


		private void CheckForEquality(EervPerson expected, EervPerson actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Firstname1, actual.Firstname1);
			Assert.AreEqual (expected.Firstname2, actual.Firstname2);
			Assert.AreEqual (expected.Lastname, actual.Lastname);
			Assert.AreEqual (expected.OriginalName, actual.OriginalName);
			Assert.AreEqual (expected.CorporateName, actual.CorporateName);
			Assert.AreEqual (expected.DateOfBirth, actual.DateOfBirth);
			Assert.AreEqual (expected.DateOfDeath, actual.DateOfDeath);
			Assert.AreEqual (expected.Honorific, actual.Honorific);
			Assert.AreEqual (expected.Sex, actual.Sex);
			Assert.AreEqual (expected.MaritalStatus, actual.MaritalStatus);
			Assert.AreEqual (expected.Origins, actual.Origins);
			Assert.AreEqual (expected.Profession, actual.Profession);
			Assert.AreEqual (expected.Confession, actual.Confession);
			Assert.AreEqual (expected.EmailAddress, actual.EmailAddress);
			Assert.AreEqual (expected.MobilPhoneNumber, actual.MobilPhoneNumber);
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
			Assert.AreEqual (expected.HouseholdId, actual.HouseholdId);
			Assert.AreEqual (expected.HouseholdRank, actual.HouseholdRank);
		}


		private readonly FileInfo PersonsFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Personnes.csv");
		private readonly FileInfo GroupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Groupe definition.csv");
		private readonly FileInfo GroupFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Groupes.csv");
		private readonly FileInfo ActivityFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Activites.csv");


	}


}
