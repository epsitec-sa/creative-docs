//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

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
	internal class AiderTownBusinessRules : GenericBusinessRule<AiderTownEntity>
	{
		public override void ApplyUpdateRule(AiderTownEntity town)
		{
			if (town.Country.IsoCode == "CH")
			{
				var zipCode  = Epsitec.Common.Types.InvariantConverter.ParseInt (town.ZipCode);
				var zipMatch = SwissPostZipRepository.Current.FindZips (zipCode, town.Name).FirstOrDefault ();

				if (zipMatch != null)
				{
					town.SwissZipCode    = zipCode;
					town.SwissZipCodeId  = zipMatch.OnrpCode;
					town.SwissCantonCode = zipMatch.Canton;
					town.Name            = zipMatch.LongName;
					town.SwissZipType    = zipMatch.ZipType;
				}
				else
				{
					Logic.BusinessRuleException (town, Resources.Text ("Le numéro postal n'est pas connu par La Poste suisse."));
				}
			}
			else
			{
				town.SwissZipCode    = null;
				town.SwissZipCodeId  = null;
				town.SwissZipType    = null;
				town.SwissCantonCode = null;
			}
		}

		public override void ApplyValidateRule(AiderTownEntity town)
		{
			if (town.Country.IsoCode == "CH")
			{
				var zip = town.ZipCode;

				if (zip.IsInteger () == false)
				{
					Logic.BusinessRuleException (town, Resources.Text ("Un numéro postal en Suisse est composé uniquement de chiffres."));
					return;
				}

				var zipNum = Epsitec.Common.Types.InvariantConverter.ParseInt (zip);

				if ((zipNum < 1000) ||
					(zipNum > 9999))
				{
					Logic.BusinessRuleException (town, Resources.Text ("Un numéro postal en Suisse est compris entre 1000 et 9999."));
					return;
				}
			}

			if (town.Name.Length < 1)
			{
				Logic.BusinessRuleException (town, Resources.Text ("Le nom de la ville ne peut pas être vide."));
				return;
			}
		}
	}
}
