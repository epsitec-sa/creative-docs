//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.ECh;

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Tests
{
	internal static class TestPostMatch
	{
		public static void TestMatchStreet()
		{
			var streets = SwissPostStreetRepository.Current;
			var zips    = SwissPostZipRepository.Current;

			var repo = ParishAddressRepository.Current;
			var name = repo.FindParishName (1400, "Yverdon-les-Bains", SwissPostStreet.NormalizeStreetName ("Fontenay, ch. du"), 6);

			var inputFile = new System.IO.FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2012-12-07.xml");
			var persons = EChDataLoader.Load (inputFile);

			var unresolved = new List<string> ();
			var unresolvedCompact = new HashSet<string> ();
			var fixZipCode = new List<string> ();
			int personCount = 0;

			foreach (var person in persons)
			{
				personCount++;

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

				var zipCode = person.Address.SwissZipCode;

				name = repo.FindParishName (zipCode, person.Address.Town, normalizedStreetName, houseNumber);
				
				if (name == null)
				{
					if ((zipCode >= 1000) &&
						(zipCode <= 1012))
					{
						for (int z = 1000; z <= 1012; z++)
						{
							name = repo.FindParishName (z, person.Address.Town, normalizedStreetName, houseNumber);
							
							if (name != null)
							{
								fixZipCode.Add (string.Format ("{0}\t{1}\t{2}", zipCode, person.Address.Street, z));
								break;
							}
						}
					}
				}

				if (name == null)
				{
					unresolved.Add (string.Format ("{0}\t{1}\t{2}\t{3}\t{4}", person.Address.SwissZipCode, person.Address.Town, person.Address.Street, person.Address.HouseNumber, person.Adult1.OfficialName.ToUpper () + " " + person.Adult1.FirstNames));
					unresolvedCompact.Add (string.Format ("{0}\t{1}\t{2}", person.Address.SwissZipCode, person.Address.Town, person.Address.Street));
				}

				if (personCount % 1000 == 0)
				{
					System.Diagnostics.Debug.WriteLine ("{0} personnes analysées", personCount);
				}
			}
			
			int count = unresolved.Count;
			
			System.IO.File.WriteAllLines ("unresolved addresses.txt", unresolved.OrderBy (x => x), System.Text.Encoding.Default);
			System.IO.File.WriteAllLines ("unresolved addresses (compact).txt", unresolvedCompact.OrderBy (x => x).Distinct (), System.Text.Encoding.Default);
			System.IO.File.WriteAllLines ("incorrect zip codes.txt", fixZipCode.OrderBy (x => x).Distinct (), System.Text.Encoding.Default);
		}
	}
}
