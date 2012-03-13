//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	internal sealed partial class EChAddress
	{
		/// <summary>
		/// The <c>PatchEngine</c> class is used to apply fixes to an address, while it is being
		/// initialized, so that it complies with the Swiss Post MAT[CH]street database.
		/// </summary>
		private static class PatchEngine
		{
			/// <summary>
			/// Applies a fix to the address so that it is compatible with the MAT[CH] database
			/// provided by the Swiss Post.
			/// </summary>
			/// <param name="address">The address.</param>
			public static void ApplyFix(EChAddress address)
			{
				var zipCode = address.swissZipCode;
				var street  = address.street;

				if (EChAddressFixesRepository.Current.ApplyQuickFix (ref zipCode, ref street))
				{
					var hits = SwissPostStreetRepository.Current.FindStreets (zipCode).Where (x => x.MatchName (street));
					address.Patch (hits);
				}

				PatchEngine.ApplySwissPostFix (address, zipCode, ref address.street, address.addressLine1);
			}

			private static FixStatus ApplySwissPostFix(EChAddress address, int zip, ref string streetName, string addressLine1, bool logFailures = true)
			{
				if (string.IsNullOrEmpty (streetName))
				{
					return FixStatus.Invalid;
				}

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

				List<SwissPostStreetInformation> matches = new List<SwissPostStreetInformation> ();

				for (int i = 0; true; i++)
				{
					if (i == 5)
					{
						if (matches.Count > 0)
						{
							break;
						}

						//	Failed to match any of the 5 attempts: if there is an additional address
						//	line, try that one too (in case the address was stored in the wrong field
						//	by the eCH software).

						if (string.IsNullOrEmpty (addressLine1) == false)
						{
							var status = PatchEngine.ApplySwissPostFix (address, zip, ref addressLine1, null, false);

							if (status == FixStatus.Invalid)
							{
								if (logFailures)
								{
									EChAddressFixesRepository.Current.RegisterFailure (string.Format ("{0:0000} {1}", zip, streetName));
								}

								return FixStatus.Invalid;
							}
							else
							{
								//	Yep, the additional address line was in fact the street name. Use
								//	it instead:

								return FixStatus.Applied;
							}
						}

						//	Failed to resolve the name, log this if desired.

						if (logFailures)
						{
							EChAddressFixesRepository.Current.RegisterFailure (string.Format ("{0:0000} {1}", zip, streetName));
						}

						return FixStatus.Invalid;

					}

					var shuffle = shuffles[i];

					if (shuffle != null)
					{
						matches.AddRange (streets.Where (x => x.MatchNameWithHeuristics (shuffle)));
					}
				}

				address.Patch (matches);

				return FixStatus.Applied;
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
