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
	[ControllerSubType (7)]
	public sealed class ActionAiderEventViewController7AddParticipant : ActionViewController<AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Ajouter un participant";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderPersonEntity, Enumerations.EventParticipantRole> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderEventEntity, SimpleBrick<AiderEventEntity>> form)
		{
			form
				.Title ("Choisir la personne")
				.Field<AiderPersonEntity> ()
					.Title ("Personne")
				.End ()
				.Field<Enumerations.EventParticipantRole> ()
					.Title ("Rôle")
				.End ()
			.End ();
		}

		private void Execute(AiderPersonEntity person, Enumerations.EventParticipantRole role)
		{
			if (person.IsNull ())
			{
				throw new BusinessRuleException ("Il faut choisir une personne");
			}

			AiderEventParticipantEntity.Create (this.BusinessContext, this.Entity, person, role);
		}
	}
}
