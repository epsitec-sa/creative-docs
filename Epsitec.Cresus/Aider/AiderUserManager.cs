//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider
{
	public sealed class AiderUserManager : UserManager
	{
		public AiderUserManager(CoreData data)
			: base (data)
		{
		}

		protected override void SetAuthenticatedUser(SoftwareUserEntity user)
		{
			base.SetAuthenticatedUser (user);
		}

		public override void SetActiveSessionId(string sessionId)
		{
			base.SetActiveSessionId (sessionId);
		}
	}
}
