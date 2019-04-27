//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderEmployeeJobViewController01RemoveJob : ActionViewController<AiderEmployeeJobEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer ce poste");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

        private void Execute()
		{
            var employeeJobEntity = this.Entity;
            var context = this.BusinessContext;

            employeeJobEntity.Delete (context);
		}
	}
}
