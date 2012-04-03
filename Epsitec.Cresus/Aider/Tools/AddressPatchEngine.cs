//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Aider.Data.ECh;


namespace Epsitec.Aider.Tools
{
	
	
	/// <summary>
	/// The <c>AddressPatchEngine</c> class is used to apply fixes to an address so that it complies
	/// with the Swiss Post MAT[CH]street database.
	/// </summary>
	internal sealed class AddressPatchEngine
	{


		public AddressPatchEngine()
		{
			this.failures = new HashSet<string> ();
		}


		public IEnumerable<string> GetFailures()
		{
			return this.failures;
		}


		private void LogFailure(string message)
		{
			this.failures.Add (message);
		}


		public FixStatus FixAddress(ref string firstAddressLine, ref string street, int? houseNumber, ref int zipCode, ref int zipCodeAddOn, ref int zipCodeId, ref string town, bool logFailures = true)
		{
			if (string.IsNullOrEmpty (street))
			{
				return FixStatus.Invalid;
			}

			var streets = SwissPostStreetRepository.Current.FindStreets (zipCode);
			var tokens  = SwissPostStreet.TokenizeStreetName (street).ToArray ();

			int n = tokens.Length;

			if (n == 0)
			{
				return FixStatus.Invalid;
			}

			//	The tokens are words in upper case, without any accented letters. For instance
			//	"CHEMIN"/"FONTENAY" or "AVENUE"/"QUATRE"/"MARRONNIERS".

			//	Sometimes, we need to try several permutations in order to find the proper street,
			//	as the root of the name used by MAT[CH] depends on subtle language-based heuristics.

			var shuffles = new string[][]
			{
				tokens,
				new string[] { tokens[0] },
				(n > 1) ? new string[] { tokens[n-1] } : null,
				(n > 1) ? new string[] { tokens[n-2], tokens[n-1] } : null,
				(n > 2) ? new string[] { tokens[n-3], tokens[n-2], tokens[n-1] } : null
			};

			List<SwissPostStreetInformation> matches = new List<SwissPostStreetInformation> ();

			foreach (var shuffle in shuffles)
			{
				if (shuffle != null)
				{
					matches.AddRange (streets.Where (x => x.MatchNameWithHeuristics (shuffle)));
				}
			}

			if (matches.Count > 0)
			{
				AddressPatchEngine.FixAddress (matches, ref street, houseNumber, ref zipCode, ref zipCodeAddOn, ref zipCodeId, ref town);

				return FixStatus.Applied;
			}

			//	Failed to match any of the attempts: if there is an additional address
			//	line, try that one too (in case the address was stored in the wrong field
			//	by the eCH software).

			if (string.IsNullOrEmpty (firstAddressLine))
			{
				return this.Fail (logFailures, zipCode, street);
			}

			string newFirstAddressLine = null;
			string newStreet = firstAddressLine;

			var status = this.FixAddress (ref newFirstAddressLine, ref newStreet, houseNumber, ref zipCode, ref zipCodeAddOn, ref zipCodeId, ref town, false);

			if (status == FixStatus.Invalid)
			{
				return this.Fail (logFailures, zipCode, street);
			}

			//	Yep, the additional address line was in fact the street name. Use
			//	it instead:

			firstAddressLine = null;
			street = newStreet;

			return FixStatus.Applied;
		}


		private FixStatus Fail(bool logFailures, int zipCode, string street)
		{
			if (logFailures)
			{
				this.LogFailure (string.Format ("{0:0000} {1}", zipCode, street));
			}

			return FixStatus.Invalid;
		}


		public static void FixAddress(IEnumerable<SwissPostStreetInformation> hits, ref string street, int? houseNumber, ref int zipCode, ref int zipCodeAddOn, ref int zipCodeId, ref string town)
		{
			int house = houseNumber ?? 0;
			var info  = hits.FirstOrDefault (x => x.MatchHouseNumber (house));

			if (info == null)
			{
				//	Cannot apply patch -- unknown street or house number
				return;
			}

			var zip = SwissPostZipRepository.Current.FindZips (info.ZipCode, info.ZipComplement).FirstOrDefault ();

			if (zip == null)
			{
				//	Cannot apply patch -- unknown zip
				return;
			}

			street = info.StreetName;
			town   = zip.LongName;

			zipCode      = zip.ZipCode;
			zipCodeAddOn = zip.ZipComplement;
			zipCodeId    = zip.OnrpCode;
		}
		
		
		private readonly HashSet<string> failures;


		public static readonly AddressPatchEngine Current = new AddressPatchEngine ();


		#region FixStatus Enumeration


		public enum FixStatus
		{
			Invalid,
			Applied
		}


		#endregion
	}


}
