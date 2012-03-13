using Epsitec.Aider.Data;
using Epsitec.Aider.Data.Ech;
using Epsitec.Aider.Data.Eerv;

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
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
			CoreData.ForceDatabaseCreationRequest = true;
			
			var lines = CoreContext.ReadCoreContextSettingsFile ().ToList ();
			CoreContext.ParseOptionalSettingsFile (lines);
			CoreContext.StartAsInteractive ();
			Services.Initialize ();

			using (var app = new CoreApplication ())
			{
				app.SetupApplication ();

				Func<BusinessContext> businessContextCreator = () => new BusinessContext (app.Data);
				Action<BusinessContext> businessContextCleaner = b => Application.ExecuteAsyncCallbacks ();

				var eChDataFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (eChDataFile);
				EChDataImporter.Import (businessContextCreator, businessContextCleaner, eChReportedPersons);
				GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);

				var parishRepository = ParishAddressRepository.Current;
				EervMainDataImporter.Import (businessContextCreator, businessContextCleaner, parishRepository);
				GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);

				Services.ShutDown ();
			}
		}


	}


}
