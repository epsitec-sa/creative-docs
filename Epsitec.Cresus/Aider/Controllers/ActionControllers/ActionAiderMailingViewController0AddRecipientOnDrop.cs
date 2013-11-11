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
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderMailingViewController0AddRecipientOnDrop : AbstractTemplateActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter aux destinataires");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			if (this.AdditionalEntity is AiderContactEntity)
			{
				this.Entity.AddContact (this.BusinessContext, (AiderContactEntity) this.AdditionalEntity);
			}

			if (this.AdditionalEntity is AiderGroupEntity)
			{
				this.Entity.AddGroup (this.BusinessContext, (AiderGroupEntity) this.AdditionalEntity);
			}

			if (this.AdditionalEntity is AiderHouseholdEntity)
			{
				this.Entity.AddHousehold (this.BusinessContext, (AiderHouseholdEntity) this.AdditionalEntity);
			}
			
		}
	}
}
