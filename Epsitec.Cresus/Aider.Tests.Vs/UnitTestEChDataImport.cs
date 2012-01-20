using Epsitec.Aider.Data;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.IO;

using System.Linq;
using System.Collections.Generic;


namespace Aider.Tests.Vs
{


	[TestClass]
	public sealed class UnitTestEChDataImport
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

				var inputFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (inputFile);
				Func<BusinessContext> businessContextCreator = () => new BusinessContext (app.Data);
				EChDataImporter.Import (businessContextCreator, eChReportedPersons);

				Services.ShutDown ();
			}
		}


	}


}
