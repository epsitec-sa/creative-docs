//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class RelationBusinessRules : GenericBusinessRule<RelationEntity>
	{
		public override void ApplyBindRule(RelationEntity relation)
		{
			Logic.Current.BusinessContext.Register (relation.Person);
		}
		
		public override void ApplySetupRule(RelationEntity relation)
		{
			var pool      = Logic.Current.Data.GetComponent<RefIdGeneratorPool> ();
			var generator = pool.GetGenerator<RelationEntity> ();
			var nextId    = generator.GetNextId ();

			relation.IdA = string.Format ("{0:000000}", nextId);

			relation.FirstContactDate = Date.Today;
			relation.TaxMode = Business.Finance.TaxMode.LiableForVat;
			relation.DefaultBillingMode =	Business.Finance.BillingMode.IncludingTax;
			relation.DefaultCurrencyCode = Business.Finance.CurrencyCode.Chf;
		}
		
		public override void ApplyUpdateRule(RelationEntity relation)
		{
			var oldAddress = relation.DefaultAddress;
			var newAddress = relation.Person.Contacts.Select (x => x as Entities.MailContactEntity).Where (x => x != null).Select (x => x.Address).FirstOrDefault ();

			if (oldAddress.RefDiffers (newAddress))
			{
				relation.DefaultAddress = newAddress;
			}

			relation.Affairs.ForEach (affair => affair.Relation = relation);
		}
	}
}
