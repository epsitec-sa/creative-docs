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
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (12)]
	public sealed class ActionAiderMailingViewController12AddGroupExtraction : ActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter un groupe transversal");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupExtractionEntity> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> form)
		{
			form
				.Title ("Ajouter un groupe transversal")
				.Field<AiderGroupExtractionEntity> ()
					.Title ("Groupe transversal")
				.End ()
			.End ();
		}

		private void Execute(AiderGroupExtractionEntity groupExtraction)
		{
			this.Entity.AddGroupExtraction (this.BusinessContext, groupExtraction);
		}
	}
}

