//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;
using System.Collections.Generic;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (10)]
	public sealed class ActionAiderEventViewController10AddMinister : ActionViewController<AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Ajouter un ministre";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderPersonEntity> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderEventEntity, SimpleBrick<AiderEventEntity>> form)
		{
			var favoriteMinisters = AiderOfficeManagementEntity.GetOfficeMinisters (this.BusinessContext, this.Entity.Office);
			form
				.Title ("Choisir le ministre")
				.Field<AiderPersonEntity> ()
					.Title ("Ministre")
					.WithFavorites (favoriteMinisters)
				.End ()
			.End ();
		}

		private void Execute(AiderPersonEntity minister)
		{
			if (minister.IsNull ())
			{
				throw new BusinessRuleException ("Il faut choisir un ministre");
			}

			AiderEventParticipantEntity.Create (this.BusinessContext, this.Entity, minister, Enumerations.EventParticipantRole.Minister);
		}
	}
}
