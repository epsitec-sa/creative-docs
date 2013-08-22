using Epsitec.Aider.Tools;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Aider.Data.Common
{


	internal sealed class AddressChecker
	{


		public AddressChecker()
		{
			this.townCorrections = new Dictionary<Tuple<string, string>, Tuple<string, string>> ();
			this.streetCorrections = new Dictionary<Tuple<string, string, int?, int, string>, Tuple<string, string, int?, int, string>> ();
			this.townChecker = new TownChecker ();
			this.addressPatchEngine = new AddressPatchEngine ();
		}


		public void FixZipCodeAndTown(ref string zipCode, ref string town)
		{
			var result = this.townChecker.Validate (zipCode, town);
			var newZipCode = result.Item1;
			var newTown = result.Item2;

			if (newZipCode != zipCode || newTown != town)
			{
				var key = Tuple.Create (zipCode, town);
				var value = Tuple.Create (newZipCode, newTown);

				this.townCorrections[key] = value;
			}

			zipCode = newZipCode;
			town = newTown;
		}


		public void FixStreetName(ref string firstAddressLine, ref string streetName, int? houseNumber, ref string zipCode, ref string town, string postBox = null)
		{
			if (string.IsNullOrEmpty (town) ||  string.IsNullOrWhiteSpace (zipCode))
			{
				return;
			}

			var zipCodeInt = int.Parse (zipCode);
			var zipCodeAddOn = SwissPostZipRepository.Current.FindZips (zipCodeInt, town).First ().ZipCodeAddOn;
			var zipCodeId = 0;

			var saveFirstAddressLine = firstAddressLine;
			var saveStreetName = streetName;
			var saveZipCodeInt = zipCodeInt;
			var saveTown = town;

			this.addressPatchEngine.FixAddress (ref firstAddressLine, ref streetName, houseNumber, ref zipCodeInt, ref zipCodeAddOn, ref zipCodeId, ref town, logFailures: true, postBox: postBox);

			var diff = firstAddressLine != saveFirstAddressLine
				|| streetName  != saveStreetName
				|| zipCodeInt != saveZipCodeInt
				|| town != saveTown;

			if (diff)
			{
				var key = Tuple.Create
				(
					saveFirstAddressLine, saveStreetName, houseNumber, saveZipCodeInt, saveTown
				);

				var value = Tuple.Create
				(
					firstAddressLine, streetName, houseNumber, zipCodeInt, town
				);

				this.streetCorrections[key] = value;
			}

			zipCode = InvariantConverter.ToString (zipCodeInt);
		}


		public void DisplayWarnings()
		{
			Console.WriteLine ("=================================================================");
			Console.WriteLine ("Town corrections");
			foreach (var townCorrection in this.townCorrections)
			{
				var key = townCorrection.Key;
				var value = townCorrection.Value;

				var message = key.Item1 + "; " + key.Item2
						+ " => "
						+ value.Item1 + "; " + value.Item2;

				Console.WriteLine (message);
			}

			Console.WriteLine ("=================================================================");
			Console.WriteLine ("Street corrections");
			foreach (var streetCorrection in this.streetCorrections)
			{
				var key = streetCorrection.Key;
				var value = streetCorrection.Value;

				var message = StringUtils.Join ("; ", key.Item1, key.Item2, key.Item3, key.Item4, key.Item5)
						+ " => "
						+ StringUtils.Join ("; ", value.Item1, value.Item2, value.Item3, value.Item4, value.Item5);

				Console.WriteLine (message);
			}
			Console.WriteLine ("=================================================================");
			Console.WriteLine ("Street correction failures");
			foreach (var failure in this.addressPatchEngine.GetFailures ())
			{
				Console.WriteLine (failure);
			}
		}



		private readonly Dictionary<Tuple<string, string>, Tuple<string, string>> townCorrections;


		private readonly Dictionary<Tuple<string, string, int?, int, string>, Tuple<string, string, int?, int, string>> streetCorrections;


		private readonly TownChecker townChecker;


		private readonly AddressPatchEngine addressPatchEngine;


	}


}
