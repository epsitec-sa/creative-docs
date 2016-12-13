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
	[ControllerSubType (20)]
	public sealed class ActionAiderOfficeTaskViewController20RemoveAllParticipations : ActionViewController<AiderOfficeTaskEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer toutes les participations");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			var tasksToRemove = this
				.Entity
				.Process
				.Tasks
				.Where (
					t => t.Kind == Enumerations.OfficeTaskKind.CheckParticipation &&
					t.IsDone == false && 
					user.CanDoTaskInOffice (t.Office)
			);
			foreach (var task in tasksToRemove)
			{
				AiderPersonsProcess.DoRemoveParticipationTask (this.BusinessContext, task);
			}
		}
	}
}
