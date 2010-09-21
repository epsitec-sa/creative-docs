//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class SoftwareUserGroupSetupRule : GenericBusinessRule<SoftwareUserGroupEntity>
	{
		protected override void Apply(SoftwareUserGroupEntity group)
		{
			group.Code = System.Guid.NewGuid ().ToString ("N");
			group.UserPowerLevel = Business.UserManagement.UserPowerLevel.None;
		}
	}
}
