//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class BusinessSettingsBusinessRules : GenericBusinessRule<BusinessSettingsEntity>
	{
		public override void ApplySetupRule(BusinessSettingsEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			entity.Code = (string) ItemCodeGenerator.NewCode ();
			entity.Finance = businessContext.CreateEntity<FinanceSettingsEntity> ();
			entity.Tax = businessContext.CreateEntity<TaxSettingsEntity> ();
		}
	}
}
