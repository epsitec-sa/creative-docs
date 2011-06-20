//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			businessContext.Register (relation.Person);
		}
		
		public override void ApplySetupRule(RelationEntity relation)
		{
			relation.FirstContactDate    = Date.Today;
			relation.TaxMode             = Business.Finance.TaxMode.LiableForVat;
			relation.DefaultCurrencyCode = Business.Finance.CurrencyCode.Chf;
		}
		
		public override void ApplyUpdateRule(RelationEntity relation)
		{
			var oldAddress = relation.DefaultAddress;
			var newAddress = relation.Person.Contacts.OfType<MailContactEntity> ().Select (x => x.Address).FirstOrDefault ();

			if (oldAddress.RefDiffers (newAddress))
			{
				relation.DefaultAddress = newAddress;
			}
		}
	}
}