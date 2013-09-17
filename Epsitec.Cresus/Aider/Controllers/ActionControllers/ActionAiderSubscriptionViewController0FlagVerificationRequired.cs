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
	[ControllerSubType (0)]
	public sealed class ActionAiderSubscriptionViewController0FlagVerificationRequired : ActionViewController<AiderSubscriptionEntity>
	{
		public override FormattedText GetTitle()
		{
			switch (this.Entity.SusbscriptionFlag)
			{
				case Enumerations.SubscriptionFlag.None:
					return Resources.Text ("Marquer pour vérification");
				case Enumerations.SubscriptionFlag.VerificationRequired:
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
			switch (this.Entity.SusbscriptionFlag)
			{
				case Enumerations.SubscriptionFlag.None:
					this.Entity.SusbscriptionFlag = Enumerations.SubscriptionFlag.VerificationRequired;
					break;
				case Enumerations.SubscriptionFlag.VerificationRequired:
					this.Entity.SusbscriptionFlag = Enumerations.SubscriptionFlag.None;
					break;
				default:
					throw new System.NotImplementedException ();
			}
		}
	}
}

