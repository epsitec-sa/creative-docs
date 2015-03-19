//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP
using System.Linq;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (5)]
	public sealed class ActionAiderUserViewController5UnsetOffice : ActionViewController<AiderUserEntity>
	{

		public override FormattedText GetTitle()
		{
			return "Supprimer gestionnaire AIDER";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var user = this.Entity;
			var currentOffice = user.Office;

			if (currentOffice.IsNotNull ())
			{
				AiderOfficeManagementEntity.LeaveOfficeManagement (this.BusinessContext, currentOffice, this.Entity, true);
			}				
		}
	}
}
