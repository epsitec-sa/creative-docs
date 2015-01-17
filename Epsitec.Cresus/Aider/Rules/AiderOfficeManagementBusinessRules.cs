//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Helpers;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderOfficeManagementBusinessRules : GenericBusinessRule<AiderOfficeManagementEntity>
	{
		public override void ApplyUpdateRule(AiderOfficeManagementEntity office)
		{
			office.RefreshOfficeShortName ();
		}
	}
}

