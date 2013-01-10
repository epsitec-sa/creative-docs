//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			SwissPost.Initialize ();

#if false
			Tests.TestPostMatch.TestMatchStreet ();		
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

#if false
			var eervGroupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Main\Groupe definition.xlsx");
			var groupDefinitions = EervMainDataLoader.LoadEervGroupDefinitions (eervGroupDefinitionFile).ToList ();

			var functionArray = groupDefinitions.Where (x => x.GroupClassification == Enumerations.GroupClassification.Function && x.GroupLevel == 1).ToArray ();
			var topLevel = groupDefinitions.Where (x => x.GroupLevel == 0).ToArray ();
#endif

			CoreProgram.Main (args);
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