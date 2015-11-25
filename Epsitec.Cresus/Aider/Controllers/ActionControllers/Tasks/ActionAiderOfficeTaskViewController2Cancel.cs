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
	[ControllerSubType (2)]
	public sealed class ActionAiderOfficeTaskViewController2Cancel : ActionViewController<AiderOfficeTaskEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Annuler");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var task    = this.Entity;
			var process = task.Process;
			
			switch (task.Kind)
			{
				case Enumerations.OfficeTaskKind.EnterNewAddress:
				// Redo all processed tasks in the same office
				var toRedo = process.Tasks.Where (t => t.Kind == Enumerations.OfficeTaskKind.CheckParticipation && t.IsDone == true && t.Office == task.Office);
				foreach (var pTask in toRedo)
				{
					AiderPersonsProcess.DoRemoveParticipationTask (this.BusinessContext, pTask);
				}
				break;
			}
			this.BusinessContext.DeleteEntity (task);
			AiderPersonsProcess.Next (this.BusinessContext, process);
			
		}
	}
}
