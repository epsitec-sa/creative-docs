using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.ActionControllers
{


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

			AiderGroupParticipantEntity.StartParticipation (this.BusinessContext, this.Entity, group, startDate, comment);
		}


		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title (this.GetTitle())
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
