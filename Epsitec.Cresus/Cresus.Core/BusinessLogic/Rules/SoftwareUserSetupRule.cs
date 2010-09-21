//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class SoftwareUserSetupRule : GenericBusinessRule<SoftwareUserEntity>
	{
		protected override void Apply(SoftwareUserEntity user)
		{
			user.Code = System.Guid.NewGuid ().ToString ("N");
			user.LoginName = "";
			user.BeginDate = System.DateTime.Now;
			user.AuthenticationMethod = Business.UserManagement.UserAuthenticationMethod.None;
			user.Disabled = false;
		}
	}
}
