//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule (RuleType.Update)]
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

			relation.Affairs.ForEach (affair => affair.Relation = relation);
		}
	}
}
