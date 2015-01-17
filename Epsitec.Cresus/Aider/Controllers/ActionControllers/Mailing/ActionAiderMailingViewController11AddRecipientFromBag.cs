//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (11)]
	public sealed class ActionAiderMailingViewController11AddRecipientFromBag : ActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Remplir avec le panier");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var context     = this.BusinessContext;
			var bagEntities = EntityBag.GetEntities (context.DataContext);

			foreach (var entity in bagEntities)
			{
				EntityBag.Process (entity as AiderContactEntity, x => this.Entity.AddContact (context, x));
				EntityBag.Process (entity as AiderPersonEntity, x => this.Entity.AddContact (context, x.MainContact));
				EntityBag.Process (entity as AiderGroupEntity, x => this.Entity.AddGroup (context, x));
				EntityBag.Process (entity as AiderGroupExtractionEntity, x => this.Entity.AddGroupExtraction (context, x));
				EntityBag.Process (entity as AiderHouseholdEntity, x => this.Entity.AddHousehold (context, x));
				EntityBag.Process (entity as AiderLegalPersonEntity, x => this.Entity.AddContact (context, x.GetMainContact ()));
			}
		}
	}
}
