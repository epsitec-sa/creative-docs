//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class OptionValueBusinessRules : GenericBusinessRule<OptionValueEntity>
	{
		public override void ApplySetupRule(OptionValueEntity entity)
		{
			entity.Code = (string) ItemCodeGenerator.NewCode ();
			entity.Quantity = 1;
		}
	}
}
