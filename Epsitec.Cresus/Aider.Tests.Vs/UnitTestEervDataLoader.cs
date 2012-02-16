using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epsitec.Aider.Data.Eerv;
using System.IO;
using System.Linq;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestEervDataLoader
	{


		[TestMethod]
		public void Test()
		{
			var personsFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Personnes.csv");
			var groupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Groupe definition.csv");
			var groupFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Groupes.csv");
			var activityFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Activites.csv");

			var houseHolds = EervDataLoader.LoadEervHouseHolds (personsFile).ToDictionary (h => h.Id);
			var persons = EervDataLoader.LoadEervPersons (personsFile).ToList ();
			var groupDefinitions = EervDataLoader.LoadEervGroupDefinitions (groupDefinitionFile).ToList ();
			var groups = EervDataLoader.LoadEervGroups (groupFile).ToList ();
			var activities = EervDataLoader.LoadEervActivities (activityFile).ToList ();
		}


	}


}
