//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.Ech;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
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
//-			var test = new Epsitec.Data.Platform.Entities.MatchStreetEntity ();
#if false
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

				var inputData = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (inputData);
				var parishRepository = ParishAddressRepository.Current;

				Func<BusinessContext> businessContextCreator = () => new BusinessContext (app.Data);
				Action<BusinessContext> businessContextCleaner = b => Application.ExecuteAsyncCallbacks ();

				EChDataImporter.Import (businessContextCreator, businessContextCleaner, eChReportedPersons);
				EervDataImporter.Import (businessContextCreator, businessContextCleaner, parishRepository);

				Services.ShutDown ();
			}
#endif

#if true
			CoreProgram.Main (args);
#endif
		}
	}
}