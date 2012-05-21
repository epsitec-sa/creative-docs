using Epsitec.Aider.Data;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Tools;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using System.Linq;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestEervParishDataImporter
	{


		[TestMethod]
		public void Test()
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

				var eChDataFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (eChDataFile);
				EChDataImporter.Import (coreDataManager, eChReportedPersons);

				var eervGroupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Main\Groupe definition.xlsx");
				var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);
				var parishRepository = ParishAddressRepository.Current;

				EervMainDataImporter.Import (coreDataManager, eervMainData, parishRepository);

				var eervPersonsFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Personnes.xlsx");
				var eervGroupFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Groupes.xlsx");
				var eervSuperGroupFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\SuperGroupes.xlsx");
				var eervActivityFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Activites.xlsx");
				var eervIdFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Morges\Ids.xlsx");
				var eervParishData = EervParishDataLoader.LoadEervParishData (eervPersonsFile, eervActivityFile, eervGroupFile, eervSuperGroupFile, eervIdFile).ToList ();

				foreach (var eervParishDatum in eervParishData)
				{
					EervParishDataImporter.Import (coreDataManager, parishRepository, eervMainData, eervParishDatum);
				}

				Services.ShutDown ();
			}
		}


	}


}
