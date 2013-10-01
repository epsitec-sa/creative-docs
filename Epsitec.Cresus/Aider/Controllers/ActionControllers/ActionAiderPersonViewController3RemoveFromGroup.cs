//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (3)]
	public sealed class ActionAiderPersonViewController3RemoveFromGroup : TemplateActionViewController<AiderPersonEntity, AiderGroupParticipantEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Retirer du groupe";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity, Date, FormattedText> (this.Execute);
		}

		
		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<AiderGroupEntity> ()
					.Title ("Groupe")
					.InitialValue (this.AdditionalEntity.Group)
					.ReadOnly ()
				.End ()
				.Field<Date> ()
					.Title ("Date de sortie du groupe")
					.InitialValue (this.AdditionalEntity.EndDate ?? Date.Today)
				.End ()
				.Field<FormattedText> ()
					.Title ("Commentaire")
					.Multiline ()
					.InitialValue (this.AdditionalEntity.Comment.Text)
				.End ()
			.End ();
		}

		
		private void Execute(AiderGroupEntity group, Date endDate, FormattedText comment)
		{
			if (endDate < this.AdditionalEntity.StartDate)
			{
				var message = "La date de sortie doit être postérieure à la date d'entrée";

				throw new BusinessRuleException (message);
			}

			if (!group.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			AiderGroupParticipantEntity.StopParticipation (this.AdditionalEntity, endDate, comment);
		}
	}
}
