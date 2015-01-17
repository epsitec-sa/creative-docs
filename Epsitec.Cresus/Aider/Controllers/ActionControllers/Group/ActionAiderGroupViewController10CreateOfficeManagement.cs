//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (10)]
	public sealed class ActionAiderGroupViewController10CreateOfficeManagement : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Créer une gestion");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var businessContext = this.BusinessContext;
			var group = this.Entity;

			var example = new AiderOfficeManagementEntity ()
			{
				ParishGroup = group
			};

			if (businessContext.GetByExample (example).Count > 0)
			{
				Logic.BusinessRuleException ("Ce groupe est déjà associé à une gestion.");
			}
			else
			{
				AiderOfficeManagementEntity.Create (businessContext, group.Name, group);
			}
		}
	}
}

