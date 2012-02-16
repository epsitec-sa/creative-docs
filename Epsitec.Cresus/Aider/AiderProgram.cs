

using Epsitec.Cresus.Core;
using Epsitec.Aider.Data;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.Ech;
using System.Collections.Generic;






namespace Epsitec.Aider
{


	public static class AiderProgram
	{

		public static void Main(string[] args)
		{
			var repo = new ParishAddressRepository ();
			var name = repo.FindParishName ("1400 Yverdon-les-Bains", SwissPostStreet.NormalizeStreetName ("Fontenay, ch. du"), 6);
						
			var inputFile = new System.IO.FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
			var persons = EChDataLoader.Load (inputFile);

			var unresolved = new List<string> ();

			foreach (var person in persons)
			{
				var streetName  = person.Address.Street;
				int houseNumber = SwissPostStreet.NormalizeHouseNumber (person.Address.HouseNumber);

				var key = person.Address.SwissZipCode + " " + person.Address.Town;
				
				name = repo.FindParishName (key, SwissPostStreet.NormalizeStreetName (streetName), houseNumber);

				if (name == null)
				{
					unresolved.Add (string.Format ("{0}\t{1}\t{2}\t{3}", person.Address.SwissZipCode, person.Address.Town, person.Address.Street, person.Address.HouseNumber));
				}
			}

			int count = unresolved.Count;

			System.IO.File.WriteAllLines ("unresolved addresses.txt", unresolved, System.Text.Encoding.Default);

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
