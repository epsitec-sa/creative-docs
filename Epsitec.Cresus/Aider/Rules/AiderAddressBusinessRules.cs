//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Data.Platform;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderAddressBusinessRules : GenericBusinessRule<AiderAddressEntity>
	{
		public override void ApplyUpdateRule(AiderAddressEntity address)
		{
			AiderAddressBusinessRules.UpdateAddressLine1 (address);
			AiderAddressBusinessRules.UpdatePostBox (address);
			AiderAddressBusinessRules.UpdateHouserNumber (address);
			AiderAddressBusinessRules.UpdateWeb (address);
		}

		public override void ApplyValidateRule(AiderAddressEntity address)
		{
			AiderAddressBusinessRules.ValidateSwissPostAddress (address);
			AiderAddressBusinessRules.ValidateWeb (address);
			AiderAddressBusinessRules.ValidateEmail (address);
		}

		private static void ValidateEmail(AiderAddressEntity address)
		{
			var email = address.Email;

			if ((string.IsNullOrWhiteSpace (email)) ||
				(UriBuilder.IsValidMailTo (UriBuilder.FixScheme (email))))
			{
				return;
			}

			Logic.BusinessRuleException (address, Resources.Text ("L'adresse e-mail n'est pas valide."));
		}

		private static void ValidateWeb(AiderAddressEntity address)
		{
			var uri = UriBuilder.FixScheme (address.Web);

			if ((uri == null) ||
				(UriBuilder.IsValidFullyQualifiedDomainNameUrl (uri)))
			{
				return;
			}
			
			Logic.BusinessRuleException (address, Resources.Text ("L'adresse web n'est pas valide."));
		}

		private static void UpdateWeb(AiderAddressEntity address)
		{
			var uri = address.Web;
			var web = UriBuilder.FixScheme (uri);

			if (web != uri)
			{
				address.Web = web;
			}
		}

		private static void UpdateAddressLine1(AiderAddressEntity address)
		{
			if (string.IsNullOrEmpty (address.AddressLine1))
			{
				return;
			}

			string prefix = AiderAddressBusinessRules.GetPostBoxPrefix (address.AddressLine1);

			//	If the user used the address line 1 as the post box identification, move
			//	the value to the proper field.

			if (prefix != null)
			{
				address.PostBox = address.AddressLine1;
				address.AddressLine1 = "";
			}
		}

		private static void UpdatePostBox(AiderAddressEntity address)
		{
			var postBox = address.PostBox;

			if (string.IsNullOrEmpty (postBox))
			{
				return;
			}

			string prefix = AiderAddressBusinessRules.GetPostBoxPrefix (postBox);

			if (prefix != null)
			{
				var tuple = postBox.SplitAfter (x => !char.IsDigit (x));
				address.PostBox = string.Format ("{0} {1}", prefix, tuple.Item2);
			}
		}

		private static string GetPostBoxPrefix(string postBox)
		{
			string upper  = postBox.ToUpperInvariant ();
			string prefix = null;

			if ((upper.StartsWith ("CASE")) ||
				(upper.StartsWith ("CP")) ||
				(upper.StartsWith ("C.")))
			{
				prefix = "Case postale";
			}
			else if ((upper.StartsWith ("POSTFACH")) ||
					 (upper.StartsWith ("PF")))
			{
				prefix = "Postfach";
			}
			
			return prefix;
		}

		private static void UpdateHouserNumber(AiderAddressEntity address)
		{
			var complement = address.HouseNumberComplement;

			if (string.IsNullOrEmpty (complement))
			{
				return;
			}

			if (complement.Length == 1)
			{
				complement = complement.ToUpperInvariant ();
			}
			else
			{
				complement = complement.ToLowerInvariant ();
			}

			address.HouseNumberComplement = complement;
		}

		private static void ValidateSwissPostAddress(AiderAddressEntity address)
		{
			if ((address.Town.IsNotNull ()) &&
				(address.Town.SwissZipCode.HasValue))
			{
				var street  = address.Street;
				var zipCode = address.Town.SwissZipCode.Value;
				var postBox = address.PostBox;

				if (string.IsNullOrEmpty (street))
				{
					if (string.IsNullOrEmpty (postBox))
					{
						Logic.BusinessRuleException (address, Resources.Text ("Le nom de rue est obligatoire."));
					}
					
					return;
				}

				var repo = SwissPostStreetRepository.Current;

				if (repo.IsStreetKnown (zipCode, street) == false)
				{
					//	We were not able to match the street based on the given ZIP code. But the
					//	ZIP code might be the one associated with the post box, or it might be a
					//	special ZIP code referring to an administration or a big company.
					//
					//	Examples:
					//
					//	Martin Schwarz, rue de la Plaine 13, Case postale, 1401 Yverdon-les-Bains
					//	Service de la population, Division Etrangers, Av. de Beaulieu 19, 1014 Lausanne

					var info = SwissPostZipCodeFoldingRepository.Resolve (zipCode);
					
					bool isBusinessAddress = info.ZipCodeType == SwissPostZipType.Company || (string.IsNullOrEmpty (postBox) == false);

					if (isBusinessAddress && repo.IsStreetKnownRelaxed (zipCode, street))
					{
						return;
					}
					
					//	The ZIP and street are not defined in MAT[CH]street

					Logic.BusinessRuleException (address, Resources.Text ("Le nom de la rue n'a pas �t� trouv� pour cette localit�."));
				}
			}
		}
	}
}
