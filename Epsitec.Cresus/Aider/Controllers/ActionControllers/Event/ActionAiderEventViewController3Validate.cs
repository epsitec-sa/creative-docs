//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	[ControllerSubType (3)]
	public sealed class ActionAiderEventViewController3Validate : ActionViewController<AiderEventEntity>
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
			return Resources.Text ("Valider d�finitivement l'acte");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create(this.Execute);
		}

		private void Execute()
		{
			this.Entity.State = Enumerations.EventState.Validated;
			this.BusinessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.None);

			var user = AiderUserManager.Current.AuthenticatedUser;
			if (user.CanValidateEvents () || user.IsAdmin ())
			{
				if (AiderEventOfficeReportEntity.GetByEvent (this.BusinessContext, this.Entity).IsNotNull ())
				{
					Logic.BusinessRuleException ("Un acte existe d�j� pour cet �v�nement");
				}

				var nextNumber = AiderEventEntity.FindNextNumber (this.BusinessContext, this.Entity.Type);
				AiderEventOfficeReportEntity.Create (this.BusinessContext, nextNumber, this.Entity);
			}
			else
			{
				Logic.BusinessRuleException ("Vous n'avez pas le droit de valider un acte");
			}
		}
	}
}