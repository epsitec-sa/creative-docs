//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author:  Samuel LOUP, Maintainer: Samuel LOUP

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
	[ControllerSubType (6)]
	public sealed class ActionAiderEventViewController6RemoveParticipant : TemplateActionViewController<AiderEventEntity, AiderEventParticipantEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Enlever ce participant");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			this.Entity.RemoveParticipant (this.BusinessContext, this.AdditionalEntity);
		}
	}
}

