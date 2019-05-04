//	Copyright © 2013-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (9)]
	public sealed class ActionAiderEventViewController9PreviewReport : ActionViewController<AiderEventEntity>
	{
		public override bool IsEnabled
		{
			get
			{
				return this.Entity.State == Enumerations.EventState.ToValidate;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.Text ("Prévisualiser l'acte");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create(this.Execute);
		}

		private void Execute()
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

            if (user.CanValidateEvents () || user.IsAdmin ())
			{
				// Trigger validation rules
				this.Entity.State     = Enumerations.EventState.ToValidate;
				this.BusinessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.None);
				var act        = AiderEventOfficeReportEntity.CreatePreview (this.BusinessContext, this.Entity);
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
				act.ProcessorUrl		= act.GetProcessorUrl (this.BusinessContext, "eventofficereport");
				this.Entity.Report      = act;
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
			}
			else
			{
				Logic.BusinessRuleException ("Vous n'avez pas le droit de valider un acte");
			}
		}
	}
}