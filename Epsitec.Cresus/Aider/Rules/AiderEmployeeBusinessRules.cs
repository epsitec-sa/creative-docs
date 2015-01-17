//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderEmployeeBusinessRules : GenericBusinessRule<AiderEmployeeEntity>
	{
		public override void ApplyValidateRule(AiderEmployeeEntity entity)
		{
			var navs13 = entity.Navs13;

			if (string.IsNullOrEmpty (navs13))
			{
				return;
			}

			var digits = navs13.Where (x => char.IsDigit (x)).Select (x => x - '0').ToArray ();

			if ((digits.Length != 13) ||
					(navs13.Length != 16) ||
					(navs13[3] != '.') || (navs13[8] != '.') || (navs13[13] != '.'))
			{
				Logic.BusinessRuleException ("Le numéro AVS doit avoir 13 chiffres et être formaté comme \"756.nnnn.nnnn.nn\".");
				return;
			}

			//	Algorithm: http://www.edstat.ch/Portals/1/Files/AHVN13/BFS_Chiffre_de_controle.pdf

			int firstSum  = digits[0]+digits[2]+digits[4]+digits[6]+digits[8]+digits[10];
			int secondSum = digits[1]+digits[3]+digits[5]+digits[7]+digits[9]+digits[11];
			int totalSum  = firstSum + 3 * secondSum;
			int lastDigit = (10 - (totalSum % 10)) % 10;

			if (digits[12] != lastDigit)
			{
				Logic.BusinessRuleException ("Le numéro AVS contient une erreur.");
				return;
			}
		}
	}
}

