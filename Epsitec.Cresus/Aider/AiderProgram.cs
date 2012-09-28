//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Tools;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Epsitec.Data.Platform;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Aider
{
	public static class AiderProgram
	{
		public static void Main(string[] args)
		{
#if false
			AiderProgram.TestMatchStreet ();		
			return;
#endif

#if false
			AiderProgram.TestEChImporter ();
			return;
#endif

#if false
			AiderProgram.TestDatabaseCreation ();
			return;
#endif


			var eervGroupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Main\Groupe definition.xlsx");
			var groupDefinitions = EervMainDataLoader.LoadEervGroupDefinitions (eervGroupDefinitionFile).ToList ();

			var functionArray = groupDefinitions.Where (x => x.GroupClassification == Enumerations.GroupClassification.Function && x.GroupLevel == 1).ToArray ();
			var topLevel = groupDefinitions.Where (x => x.GroupLevel == 0).ToArray ();


			CoreProgram.Main (args);
		}

		private static void TestMatchStreet()
		{
			var streets = SwissPostStreetRepository.Current;
			var zips    = SwissPostZipRepository.Current;

			var repo = ParishAddressRepository.Current;
			var name = repo.FindParishName (1400, "Yverdon-les-Bains", SwissPostStreet.NormalizeStreetName ("Fontenay, ch. du"), 6);

			var inputFile = new System.IO.FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
			var persons = EChDataLoader.Load (inputFile);

			var unresolved = new List<string> ();
			var unresolvedCompact = new HashSet<string> ();

			foreach (var person in persons)
			{
				var zip = person.Address.SwissZipCode;
				var all = SwissPostZipRepository.Current.FindZips (zip, person.Address.Town);
				var odd = all.Where (x => x.Canton != "VD").ToArray ();
				var streetName  = person.Address.Street;
				int houseNumber = SwissPostStreet.NormalizeHouseNumber (person.Address.HouseNumber);

				if ((all.Any () == false) ||
					(odd.Length > 0))
				{
					System.Diagnostics.Debug.WriteLine ("Error: not in VD, {0} {1}, {2} {3}, {4}", person.Adult1.FirstNames, person.Adult1.OfficialName, person.Address.SwissZipCode, person.Address.Town, person.Address.Street);
				}

				string normalizedStreetName = SwissPostStreet.NormalizeStreetName (streetName);

				if (normalizedStreetName.Length == 0)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Error: no street for {0} {1}, {2} {3}", person.Adult1.FirstNames, person.Adult1.OfficialName, person.Address.SwissZipCode, person.Address.Town));
				}
				if (houseNumber == 0)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Error: no street number for {0} {1}, {2} {3}, {4}", person.Adult1.FirstNames, person.Adult1.OfficialName, person.Address.SwissZipCode, person.Address.Town, person.Address.Street));
				}

				name = repo.FindParishName (person.Address.SwissZipCode, person.Address.Town, normalizedStreetName, houseNumber);

				if (name == null)
				{
					unresolved.Add (string.Format ("{0}\t{1}\t{2}\t{3}\t{4}", person.Address.SwissZipCode, person.Address.Town, person.Address.Street, person.Address.HouseNumber, person.Adult1.OfficialName.ToUpper () + " " + person.Adult1.FirstNames));
					unresolvedCompact.Add (string.Format ("{0}\t{1}\t{2}", person.Address.SwissZipCode, person.Address.Town, person.Address.Street));
				}
			}

			int count = unresolved.Count;

			System.IO.File.WriteAllLines ("unresolved addresses.txt", unresolved.OrderBy (x => x), System.Text.Encoding.Default);
			System.IO.File.WriteAllLines ("unresolved addresses (compact).txt", unresolvedCompact.OrderBy (x => x), System.Text.Encoding.Default);
		}

		private static void TestEChImporter()
		{
			new eCH_Importer ().ParseAll ();
		}

		private static void TestDatabaseCreation()
		{
			var hack = new Epsitec.Data.Platform.Entities.MatchStreetEntity ();

			CoreData.ForceDatabaseCreationRequest = true;

			var lines = CoreContext.ReadCoreContextSettingsFile ().ToList ();
			CoreContext.ParseOptionalSettingsFile (lines);
			CoreContext.StartAsInteractive ();
			Services.Initialize ();

			using (var app = new CoreApplication ())
			{
				app.SetupApplication ();

				var coreDataManager = new CoreDataManager (app.Data);

				var eChDataFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2012-04-18.xml");
				var eChReportedPersons = EChDataLoader.Load (eChDataFile, 1000);
				EChDataImporter.Import (coreDataManager, eChReportedPersons);
				
				var eervGroupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Main\Groupe definition.xlsx");
				var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);
				var parishRepository = ParishAddressRepository.Current;

				EervMainDataImporter.Import (coreDataManager, eervMainData, parishRepository);

#if false
				var eervFileGroups = new List<Tuple<FileInfo, FileInfo, FileInfo, FileInfo, FileInfo>> ()
				{
					Tuple.Create
					(
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Personnes.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Activites.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Groupes.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\SuperGroupes.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Ids.xlsx")
					),
					Tuple.Create
					(
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Région 9\Personnes.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Région 9\Activites.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Région 9\Groupes.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Région 9\SuperGroupes.xlsx"),
						new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Région 9\Ids.xlsx")
					),
				};

				foreach (var eervFileGroup in eervFileGroups)
				{
					var eervPersonsFile = eervFileGroup.Item1;
					var eervActivityFile = eervFileGroup.Item2;
					var eervGroupFile = eervFileGroup.Item3;
					var eervSuperGroupFile = eervFileGroup.Item4;
					var eervIdFile = eervFileGroup.Item5;

					var eervParishData = EervParishDataLoader.LoadEervParishData (eervPersonsFile, eervActivityFile, eervGroupFile, eervSuperGroupFile, eervIdFile).ToList ();

					foreach (var eervParishDatum in eervParishData)
					{
						EervParishDataImporter.Import (coreDataManager, parishRepository, eervMainData, eervParishDatum);
					}
				}
#endif
				Services.ShutDown ();
			}
		}
	}
}