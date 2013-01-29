//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

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
			AiderAddressBusinessRules.UpdateHouserNumber (address);
		}

		public override void ApplyValidateRule(AiderAddressEntity address)
		{
			AiderAddressBusinessRules.ValidateSwissPostAddress (address);
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
					//	The ZIP and street are not defined in MAT[CH]street

					Logic.BusinessRuleException (address, Resources.Text ("Le nom de la rue n'a pas été trouvé pour cette localité."));
				}
			}
		}
	}
}
