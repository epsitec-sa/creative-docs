//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (13)]
	public sealed class ActionAiderGroupViewController13DeleteOfficeManagement : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer la gestion");
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

			var office = businessContext.GetByExample (example).FirstOrDefault ();
			if (office.IsNull ())
			{
				Logic.BusinessRuleException ("Ce groupe n'est pas associé a une gestion");
			}
			else
			{
				office.Delete (this.BusinessContext);
			}
		}
	}
}

