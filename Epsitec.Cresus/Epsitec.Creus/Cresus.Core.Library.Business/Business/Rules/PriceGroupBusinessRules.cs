//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class PriceGroupBusinessRules : GenericBusinessRule<PriceGroupEntity>
	{
		public override void ApplyValidateRule(PriceGroupEntity priceGroup)
		{
			var currentRoundingModes = priceGroup.DefaultRoundingModes;
			var sorterRoundingModes  = currentRoundingModes.OrderBy (x => x).ToArray ();

			if (Comparer.EqualRefs (currentRoundingModes, sorterRoundingModes) == false)
			{
				priceGroup.DefaultRoundingModes.Clear ();
				priceGroup.DefaultRoundingModes.AddRange (sorterRoundingModes);
			}
		}
	}
}
