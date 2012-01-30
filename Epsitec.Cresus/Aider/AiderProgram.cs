

using Epsitec.Cresus.Core;






namespace Epsitec.Aider
{


	public static class AiderProgram
	{

		public static void Main(string[] args)
		{

#if false
			new eCH_Importer ().ParseAll ();
#endif

#if false
			CoreData.ForceDatabaseCreationRequest = true;

			var lines = CoreContext.ReadCoreContextSettingsFile ().ToList ();
			CoreContext.ParseOptionalSettingsFile (lines);
			CoreContext.StartAsInteractive ();
			Services.Initialize ();

			using (var app = new CoreApplication ())
			{
				app.SetupApplication ();

				var inputFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (inputFile).Take (200).ToList ();

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
#endif

#if true
			CoreProgram.Main (args);
#endif
		}


	}


}
