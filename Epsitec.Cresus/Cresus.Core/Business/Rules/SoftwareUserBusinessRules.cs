//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class SoftwareUserBusinessRules : GenericBusinessRule<SoftwareUserEntity>
	{
		public override void ApplySetupRule(SoftwareUserEntity user)
		{
			user.Code = ItemCodeGenerator.NewCode ();
			user.DisplayName = new FormattedText ("");
			user.LoginName = "";
			user.BeginDate = System.DateTime.Now;
			user.AuthenticationMethod = Business.UserManagement.UserAuthenticationMethod.None;
			user.Disabled = false;
		}
	}
}