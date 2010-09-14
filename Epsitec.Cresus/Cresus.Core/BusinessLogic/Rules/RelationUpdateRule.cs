//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
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
	
	[BusinessRule (RuleType=RuleType.Bind)]
	internal class RelationBindRule : GenericBusinessRule<RelationEntity>
	{
		protected override void Apply(RelationEntity relation)
		{
			Logic.Current.BusinessContext.Register (relation.Person);
		}
	}
	
	[BusinessRule (RuleType=RuleType.Update)]
	internal class NaturalPersonUpdateRule : GenericBusinessRule<NaturalPersonEntity>
	{
		protected override void Apply(NaturalPersonEntity person)
		{
			person.Contacts.ForEach (x => x.NaturalPerson = person);
		}
	}
	
	[BusinessRule (RuleType=RuleType.Update)]
	internal class LegalPersonUpdateRule : GenericBusinessRule<LegalPersonEntity>
	{
		protected override void Apply(LegalPersonEntity person)
		{
			person.Contacts.ForEach (x => x.LegalPerson = person);
		}
	}
}
