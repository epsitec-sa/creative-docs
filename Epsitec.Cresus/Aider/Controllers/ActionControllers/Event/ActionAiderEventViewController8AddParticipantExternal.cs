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
using Epsitec.Aider.Data.Job;
using Epsitec.Aider.Data.ECh;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Aider.Data.Common;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (8)]
	public sealed class ActionAiderEventViewController8AddParticipantExternal : ActionViewController<AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Ajouter une personne externe à AIDER";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string, string, Enumerations.PersonConfession, Enumerations.EventParticipantRole> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderEventEntity, SimpleBrick<AiderEventEntity>> form)
		{
			form
				.Title ("Information sur la personne")
				.Field<string> ()
					.Title ("Nom")
				.End ()
				.Field<string> ()
					.Title ("Prénom")
				.End ()
				.Field<string> ()
					.Title ("Localité")
				.End ()
				.Field<Enumerations.PersonConfession> ()
					.Title ("Confession")
					.InitialValue (Enumerations.PersonConfession.Protestant)
				.End ()
				.Field<Enumerations.EventParticipantRole> ()
					.Title ("Rôle")
				.End ()
			.End ();
		}

		private void Execute(
			string lastName,
			string firstName,
			string town,
			Enumerations.PersonConfession confession,
			Enumerations.EventParticipantRole role
		)
		{
			if(string.IsNullOrEmpty (lastName) || string.IsNullOrEmpty (firstName))
			{
				throw new BusinessRuleException ("un nom et un prénom et obligatoire");
			}

			if (string.IsNullOrEmpty (town))
			{
				throw new BusinessRuleException ("La localité est obligatoire");
			}

			if (role == Enumerations.EventParticipantRole.None)
			{
				throw new BusinessRuleException ("Un rôle est obligatoire");
			}

			AiderEventParticipantEntity.CreateForExternal (
				this.BusinessContext, 
				this.Entity, 
				firstName, 
				lastName,
				null,
				town, 
				null,
				confession,
				role
			);
		}
	}
}
