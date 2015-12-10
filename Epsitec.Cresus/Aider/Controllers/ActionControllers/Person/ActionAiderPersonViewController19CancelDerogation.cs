//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.Reporting;
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (19)]
	public sealed class ActionAiderPersonViewController19CancelDerogation : ActionViewController<AiderPersonEntity>
	{
		public override bool IsEnabled
		{
			get
			{
				return this.Entity.HasDerogation;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.Text ("Annuler la dérogation");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{		
			var person = this.Entity;
			var user   = AiderUserManager.Current.AuthenticatedUser;

			if (!user.IsAdmin ())
			{
				throw new BusinessRuleException ("Vos droits ne vous permettent pas d'annuler une dérogation");
			}

			var oldParishGroup = AiderGroupEntity.FindGroups (this.BusinessContext, this.Entity.GeoParishGroupPathCache).Single ();
			AiderDerogations.RemoveDerogation (this.BusinessContext, this.Entity, oldParishGroup, enableWarning: false);
		}
	}
}
