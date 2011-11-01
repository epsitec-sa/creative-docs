//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class LegalPersonBusinessRules : GenericBusinessRule<LegalPersonEntity>
	{
		public override void ApplyUpdateRule(LegalPersonEntity person)
		{
			person.Contacts.OfType<MailContactEntity> ().ForEach (x => x.ResetPersonAddress (person));
		}
	}
}
