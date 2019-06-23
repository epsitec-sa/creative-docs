//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (8)]
	public sealed class ActionAiderOfficeManagementViewController8DeleteEvent : TemplateActionViewController<AiderOfficeManagementEntity, AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer l'acte");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
            //  Ce code n'est disponible que pour des actes en cours de préparation
            //  ou en attente de validation...

            var act = this.AdditionalEntity;

            act.DeleteMutableEntity (this.BusinessContext);
		}
	}
}
