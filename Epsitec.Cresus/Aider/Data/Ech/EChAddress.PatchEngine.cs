//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Ech
{
	internal sealed partial class EChAddress
	{
		private static class PatchEngine
		{
			internal static EChAddress ApplyFix(EChAddress address)
			{
				if (address.CountryCode == "CH")
				{
					string zipCode      = address.SwissZipCode;
					string streetName   = address.Street;
					string addressLine1 = address.AddressLine1;

					if (PatchEngine.ApplyFix (ref zipCode, ref streetName, addressLine1))
					{
						return new EChAddress (addressLine1, streetName, address.HouseNumber, address.Town, zipCode, address.SwissZipCodeAddOn, address.SwissZipCodeId, address.CountryCode);
					}
				}

				return address;
			}

			/// <summary>
			/// Applies a fix to the address so that it is compatible with the MAT[CH] database
			/// provided by the Swiss Post.
			/// </summary>
			/// <param name="zipCode">The zip code.</param>
			/// <param name="streetName">Full name of the street.</param>
			/// <param name="addressLine1">The additional address line.</param>
			/// <returns></returns>
			private static bool ApplyFix(ref string zipCode, ref string streetName, string addressLine1 = null)
			{
				bool fixApplied = false;

				fixApplied |= EChAddressFixesRepository.Current.ApplyQuickFix (ref zipCode, ref streetName);
				fixApplied |= PatchEngine.ApplySwissPostFix (zipCode, ref streetName, addressLine1) == FixStatus.Applied;

				return fixApplied;
			}

			private static FixStatus ApplySwissPostFix(string zipCode, ref string streetName, string addressLine1, bool logFailures = true)
			{
				if (string.IsNullOrEmpty (streetName))
				{
					return FixStatus.Invalid;
				}

				int zip     = int.Parse (zipCode, System.Globalization.CultureInfo.InvariantCulture);
				var streets = SwissPostStreetRepository.Current.FindStreets (zip);
				var tokens  = SwissPostStreet.TokenizeStreetName (streetName).ToArray ();

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

				SwissPostStreetInformation match = null;

				for (int i = 0; match == null; i++)
				{
					if (i == 5)
					{
						//	Failed to match any of the 5 attempts: if there is an additional address
						//	line, try that one too (in case the address was stored in the wrong field
						//	by the eCH software).

						if (string.IsNullOrEmpty (addressLine1) == false)
						{
							var status = PatchEngine.ApplySwissPostFix (zipCode, ref addressLine1, null, false);

							if (status == FixStatus.Invalid)
							{
								if (logFailures)
								{
									EChAddressFixesRepository.Current.RegisterFailure (string.Concat (zipCode, " ", streetName));
								}

								return FixStatus.Invalid;
							}
							else
							{
								//	Yep, the additional address line was in fact the street name. Use
								//	it instead:

								streetName = addressLine1;

								return FixStatus.Applied;
							}
						}

						//	Failed to resolve the name, log this if desired.

						if (logFailures)
						{
							EChAddressFixesRepository.Current.RegisterFailure (string.Concat (zipCode, " ", streetName));
						}

						return FixStatus.Invalid;

					}

					var shuffle = shuffles[i];

					if (shuffle != null)
					{
						match = streets.Where (x => x.MatchNameWithHeuristics (shuffle)).FirstOrDefault ();
					}
				}

				string preferred = match.StreetName;

				if (preferred == streetName)
				{
					return FixStatus.Unchanged;
				}
				else
				{
					streetName = preferred;
					return FixStatus.Applied;
				}
			}

			#region FixStatus Enumeration

			private enum FixStatus
			{
				Unchanged,
				Invalid,
				Applied
			}

			#endregion
		}
	}
}
