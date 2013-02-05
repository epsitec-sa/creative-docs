//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderLegalPersonBusinessRules : GenericBusinessRule<AiderLegalPersonEntity>
	{
		public override void ApplySetupRule(AiderLegalPersonEntity legal)
		{
			legal.Type          = LegalPersonType.Business;
			legal.Visibility    = PersonVisibilityStatus.Default;
			legal.RemovalReason = RemovalReason.None;
			legal.Language      = Language.French;
		}

		public override void ApplyUpdateRule(AiderLegalPersonEntity legal)
		{
			legal.Name = legal.Name.TrimSpacesAndDashes ();
		}
	}
}
