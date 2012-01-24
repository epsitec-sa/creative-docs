using Epsitec.Aider.Data;

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
				
				// HACK Here we need to call this, because there is somewhere something that
				// registers a callback with a reference to the business context that we
				// have disposed. If we don't execute that callback, the pointer stays there
				// and the garbage collector can't reclaim the memory and we have a memory
				// leak. I think that this is a hack because that callback is related to user
				// interface stuff and we should be able to get a business context without being
				// related to a user interface.
				Action<BusinessContext> businessContextCleaner = b => Application.ExecuteAsyncCallbacks ();

				EChDataImporter.Import (businessContextCreator, businessContextCleaner, eChReportedPersons);

				Services.ShutDown ();
			}
		}


	}


}
