using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.ActionControllers
{


	[ControllerSubType (3)]
	public sealed class ActionAiderPersonViewController3 : TemplateActionViewController<AiderPersonEntity, AiderGroupParticipantEntity>
	{


		public override FormattedText GetTitle()
		{
			return "Retirer du groupe";
		}


		public override bool RequiresAdditionalEntity()
		{
			return true;
		}


		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity, Date, FormattedText> (this.Execute);
		}


		private void Execute(AiderGroupEntity group, Date endDate, FormattedText comment)
		{
			AiderGroupParticipantEntity.StopParticipation (this.AdditionalEntity, endDate, comment);
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
	}


}
