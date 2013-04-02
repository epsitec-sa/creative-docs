//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

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
	//	@PA: faut-il déplacer ce contrôleur pour qu'il manipule des contacts?

	[ControllerSubType (2)]
	public sealed class ActionAiderPersonViewController2 : TemplateActionViewController<AiderPersonEntity, AiderGroupParticipantEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Ajouter à un groupe";
		}

		public override bool RequiresAdditionalEntity()
		{
			return false;
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity, Date, FormattedText> (this.Execute);
		}

		private void Execute(AiderGroupEntity group, Date startDate, FormattedText comment)
		{
			if (group.IsNull ())
			{
				throw new BusinessRuleException ("Aucun groupe sélectionné");
			}

			if (!group.CanHaveMembers ())
			{
				throw new BusinessRuleException ("Ce groupe ne peut pas avoir de membres");
			}

			if (this.Entity.IsMemberOf (group))
			{
				throw new BusinessRuleException ("Cette personne est déjà membre de ce groupe");
			}

			if (!group.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			var participationData = new ParticipationData
			{
				Person = this.Entity,
				//	@PA: contact ?
			};

			AiderGroupParticipantEntity.StartParticipation (this.BusinessContext, group, participationData, startDate, comment);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<AiderGroupEntity> ()
					.WithSpecialField<AiderGroupSpecialField<AiderPersonEntity>> ()
					.Title ("Groupe")
				.End ()
				.Field<Date> ()
					.Title ("Date d'entrée dans le groupe")
					.InitialValue (Date.Today)
				.End ()
				.Field<FormattedText> ()
					.Title ("Commentaire")
					.Multiline ()
				.End ()
			.End ();
		}
	}
}
