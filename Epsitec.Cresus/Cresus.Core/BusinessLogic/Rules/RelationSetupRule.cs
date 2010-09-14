//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType=RuleType.Setup)]
	internal class RelationSetupRule : GenericBusinessRule<RelationEntity>
	{
		protected override void Apply(RelationEntity relation)
		{
			var pool = Logic.Current.BusinessContext.Data.RefIdGeneratorPool;
			var generator = pool.GetGenerator<RelationEntity> ();
			var nextId    = generator.GetNextId ();

			relation.IdA = string.Format ("{0:000000}", nextId);
			
			relation.FirstContactDate = Date.Today;
			relation.TaxMode = Business.Finance.TaxMode.LiableForVat;
			relation.DefaultBillingMode =	Business.Finance.BillingMode.IncludingTax;
			relation.DefaultCurrencyCode = Business.Finance.CurrencyCode.Chf;
		}
	}

	[BusinessRule (RuleType=RuleType.Update)]
	internal class RelationUpdateRule : GenericBusinessRule<RelationEntity>
	{
		protected override void Apply(RelationEntity relation)
		{
			var oldAddress = relation.DefaultAddress;
			var newAddress = relation.Person.Contacts.Select (x => x as Entities.MailContactEntity).Where (x => x != null).Select (x => x.Address).FirstOrDefault ();

			if (oldAddress.RefDiffers (newAddress))
			{
				relation.DefaultAddress = newAddress;
			}
		}
	}
}
