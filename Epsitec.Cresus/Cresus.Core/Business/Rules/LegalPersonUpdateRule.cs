//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule (RuleType.Update)]
	internal class LegalPersonUpdateRule : GenericBusinessRule<LegalPersonEntity>
	{
		protected override void Apply(LegalPersonEntity person)
		{
			person.Contacts.ForEach (x => x.LegalPerson = person);
		}
	}
}
