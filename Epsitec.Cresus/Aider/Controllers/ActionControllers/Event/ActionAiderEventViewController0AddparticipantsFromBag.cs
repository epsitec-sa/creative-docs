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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderEventViewController0AddParticipantsFromBag : ActionViewController<AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Reprendre depuis le panier";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var contacts = new List<AiderContactEntity> ();
			var bagEntities = EntityBag.GetEntities (this.BusinessContext.DataContext);

			foreach (var entity in bagEntities)
			{
				EntityBag.Process (entity as AiderContactEntity, x => contacts.Add (x));
			}

			foreach (var contact in contacts)
			{
				AiderEventParticipantEntity.Create (this.BusinessContext, this.Entity, contact.Person, Enumerations.EventParticipantRole.None);
			}
		}
	}
}
