//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (17)]
	public sealed class ActionAiderMailingViewController17ToggleHouseholdGrouping : TemplateActionViewController<AiderMailingEntity, AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			if(this.Entity.IsGroupedByHousehold)
			{
				return Resources.FormattedText ("Enlever le regroupement par ménage");
				
			}
			else
			{
				return Resources.FormattedText ("Regrouper par ménage");
			}
			
		}

		public override bool ExecuteInQueue
		{
			get
			{
				return true;
			}
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			this.Entity.IsGroupedByHousehold = !this.Entity.IsGroupedByHousehold;
			this.Entity.UpdateMailingParticipants (this.BusinessContext);
		}
	}
}
