//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public override void ApplyValidateRule(AiderAddressEntity address)
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
						return;
//-						throw new BusinessRuleException (address, Resources.Text ("Le nom de rue est obligatoire."));
					}
					else
					{
						return;
					}
				}

				var repo = SwissPostStreetRepository.Current;

				if (repo.IsStreetKnown (zipCode, street))
				{
					//	OK, the ZIP and street are defined in MAT[CH]street
					
					return;
				}
				
//-				throw new BusinessRuleException (address, Resources.Text ("Le nom de la rue n'a pas �t� trouv� pour cette localit�."));
			}
		}
	}
}
