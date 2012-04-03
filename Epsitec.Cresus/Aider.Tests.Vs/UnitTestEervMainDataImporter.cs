using Epsitec.Aider.Data;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Tools;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.IO;

using System.Linq;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestEervMainDataImporter
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

				var businessContextManager = new BusinessContextManager (app.Data);

				var eChDataFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (eChDataFile);
				EChDataImporter.Import (businessContextManager, eChReportedPersons);

				var parishRepository = ParishAddressRepository.Current;
				EervMainDataImporter.Import (businessContextManager, parishRepository);

				Services.ShutDown ();
			}
		}


	}


}
