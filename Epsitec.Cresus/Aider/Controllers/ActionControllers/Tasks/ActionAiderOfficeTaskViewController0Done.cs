//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderOfficeTaskViewController0Done : ActionViewController<AiderOfficeTaskEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("C'est fait!");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			this.Entity.IsDone = true;
			this.Entity.Actor  = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);
			switch (this.Entity.Process.Type)
			{
				case Enumerations.OfficeProcessType.PersonsOutputProcess:
					AiderPersonsExitProcess.Next (this.BusinessContext, this.Entity.Process);
					break;
				case Enumerations.OfficeProcessType.PersonsParishChangeProcess:
					AiderParishChangeProcess.Next (this.BusinessContext, this.Entity.Process);
					break;
			}
		}
	}
}
