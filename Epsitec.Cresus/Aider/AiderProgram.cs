//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.Ech;

using Epsitec.Cresus.Core;
using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Aider
{
	public static class AiderProgram
	{
		public static void Main(string[] args)
		{
#if true
			var streets = SwissPostStreetRepository.Current;

			var repo = ParishAddressRepository.Current;
			var name = repo.FindParishName ("1400 Yverdon-les-Bains", SwissPostStreet.NormalizeStreetName ("Fontenay, ch. du"), 6);
						
			var inputFile = new System.IO.FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
			var persons = EChDataLoader.Load (inputFile);

			var unresolved = new List<string> ();
			var unresolvedCompact = new HashSet<string> ();
			var fixStreets = new HashSet<string> ();

			foreach (var person in persons)
			{
				var streetName  = person.Address.Street;
				int houseNumber = SwissPostStreet.NormalizeHouseNumber (person.Address.HouseNumber);

				var key = person.Address.SwissZipCode + " " + person.Address.Town;

				string normalizedStreetName = SwissPostStreet.NormalizeStreetName (streetName);

				if (normalizedStreetName.Length == 0)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Error: no street for {0} {1}, {2}", person.Adult1.FirstNames, person.Adult1.OfficialName, key));
				}

				name = repo.FindParishName (key, normalizedStreetName, houseNumber);

				if (name == null)
				{
					var nameOnly  = person.Address.Street.Split (',')[0];
					var rootName1 = nameOnly.Split (new char[] { ' ', '\'' }).LastOrDefault ();

					var tokens = rootName1.Split ('-');

					var rootName2 = tokens.Last ();
					var rootName3 = tokens.First ();
					var rootName4 = string.Join ("-", tokens.Reverse ().Take (2).Reverse ());
					var rootName5 = string.Join ("-", tokens.Reverse ().Take (3).Reverse ());
					var rootName6 = rootName1.Replace ("Saint", "St");
					var rootName7 = string.Join (" ", nameOnly.Split (new char[] { ' ', '\'' }).Reverse ().Take (2).Reverse ());

					int zip = int.Parse (person.Address.SwissZipCode);
					
					var streetFound = streets.FindStreets (zip, rootName1).FirstOrDefault ()
								   ?? streets.FindStreets (zip, rootName2).FirstOrDefault ()
								   ?? streets.FindStreets (zip, rootName3).FirstOrDefault ()
								   ?? streets.FindStreets (zip, rootName4).FirstOrDefault ()
								   ?? streets.FindStreets (zip, rootName5).FirstOrDefault ()
								   ?? streets.FindStreets (zip, rootName6).FirstOrDefault ()
								   ?? streets.FindStreets (zip, rootName7).FirstOrDefault ();

					if (streetFound == null)
					{
						System.Diagnostics.Debug.WriteLine ("Unknown street: {0} {1}", person.Address.SwissZipCode, rootName1);
						fixStreets.Add (person.Address.SwissZipCode + "\t" + person.Address.Street);
					}
					else
					{
						name = repo.FindParishName (key, streetFound.NormalizedStreetName, houseNumber);

						if (name != null)
						{
						}
					}

					if (name == null)
					{
						unresolved.Add (string.Format ("{0}\t{1}\t{2}\t{3}\t{4}", person.Address.SwissZipCode, person.Address.Town, person.Address.Street, person.Address.HouseNumber, person.Adult1.OfficialName.ToUpper () + " " + person.Adult1.FirstNames));
						unresolvedCompact.Add (string.Format ("{0}\t{1}\t{2}", person.Address.SwissZipCode, person.Address.Town, person.Address.Street));
					}
				}
			}

			int count = unresolved.Count;

			System.IO.File.WriteAllLines ("unresolved addresses.txt", unresolved.OrderBy (x => x), System.Text.Encoding.Default);
			System.IO.File.WriteAllLines ("unresolved addresses (compact).txt", unresolvedCompact.OrderBy (x => x), System.Text.Encoding.Default);
			System.IO.File.WriteAllLines ("unresolved eCH addresses.txt", EChAddressFixesRepository.Current.GetFailures ().OrderBy (x => x), System.Text.Encoding.Default);
			
			return;
#endif
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