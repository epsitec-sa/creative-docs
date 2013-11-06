//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

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
	[ControllerSubType (1)]
	public sealed class ActionAiderMailingViewController1AddContactToExclusionsOnDrop : TemplateActionViewController<AiderMailingEntity, AiderContactEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter un contact aux exclusions");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			this.Entity.ExludeContact (this.BusinessContext, this.AdditionalEntity);
		}
	}
}
