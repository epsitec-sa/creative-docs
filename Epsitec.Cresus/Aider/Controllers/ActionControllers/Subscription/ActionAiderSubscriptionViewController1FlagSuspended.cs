//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderSubscriptionViewController1FlagSuspended : ActionViewController<AiderSubscriptionEntity>
	{
		public override FormattedText GetTitle()
		{
			switch (this.Entity.SubscriptionFlag)
			{
				case Enumerations.SubscriptionFlag.VerificationRequired:
				case Enumerations.SubscriptionFlag.None:
					return Resources.Text ("Marquer comme suspendu");
				case Enumerations.SubscriptionFlag.Suspended:
					return Resources.Text ("Marquer comme valide");
				default:
					throw new System.NotImplementedException ();
			}
		}

		protected override bool NeedsInteraction
		{
			get
			{
				return false;
			}
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			switch (this.Entity.SubscriptionFlag)
			{
				case Enumerations.SubscriptionFlag.VerificationRequired:
				case Enumerations.SubscriptionFlag.None:
					this.Entity.SubscriptionFlag = Enumerations.SubscriptionFlag.Suspended;
					break;
				case Enumerations.SubscriptionFlag.Suspended:
					this.Entity.SubscriptionFlag = Enumerations.SubscriptionFlag.None;
					break;
				default:
					throw new System.NotImplementedException ();
			}
		}
	}
}
