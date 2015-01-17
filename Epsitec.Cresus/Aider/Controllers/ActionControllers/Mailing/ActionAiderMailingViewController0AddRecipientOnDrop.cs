//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.Override;

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
			EntityBag.Process (this.AdditionalEntity as AiderPersonEntity, x => this.Entity.AddContact (this.BusinessContext, x.MainContact));
			EntityBag.Process (this.AdditionalEntity as AiderContactEntity, x => this.Entity.AddContact (this.BusinessContext, x));
			EntityBag.Process (this.AdditionalEntity as AiderGroupEntity, x => this.Entity.AddGroup (this.BusinessContext, x));
			EntityBag.Process (this.AdditionalEntity as AiderGroupExtractionEntity, x => this.Entity.AddGroupExtraction (this.BusinessContext, x));
			EntityBag.Process (this.AdditionalEntity as AiderHouseholdEntity, x => this.Entity.AddHousehold (this.BusinessContext, x));
			EntityBag.Process (this.AdditionalEntity as AiderLegalPersonEntity, x => this.Entity.AddContact (this.BusinessContext, x.GetMainContact ()));
		}
	}
}
