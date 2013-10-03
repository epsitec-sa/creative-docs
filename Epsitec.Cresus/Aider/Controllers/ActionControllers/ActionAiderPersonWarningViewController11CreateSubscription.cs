//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (11)]
	public sealed class ActionAiderPersonWarningViewController11CreateSubscription : ActionAiderPersonWarningViewControllerPassive
	{
		public override FormattedText GetTitle()
		{
			return TextFormatter.FormatText ("Crée l'abonnement");
		}

		protected override void Execute()
		{
			var warning   = this.Entity;
			var person    = warning.Person;
			var household = person.Households.FirstOrDefault ();

			AiderSubscriptionEntity.Create (this.BusinessContext, household);

			this.ClearWarningAndRefreshCaches ();
		}
	}
}

