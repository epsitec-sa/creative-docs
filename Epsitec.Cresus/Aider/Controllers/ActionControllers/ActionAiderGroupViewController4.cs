using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (4)]
	public sealed class ActionAiderGroupViewController4 : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Exporter les membres vers un groupe";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity, Date?, FormattedText> (this.Execute);
		}

		private void Execute(AiderGroupEntity target, Date? startDate, FormattedText comment)
		{
			if (target.IsNull ())
			{
				var message = "Aucun groupe séléctionné.";

				throw new BusinessRuleException (message);
			}

			if (!target.CanHaveMembers ())
			{
				var message = "Ce groupe ne peut pas avoir de membres.";

				throw new BusinessRuleException (message);
			}

			if (!target.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			target.ImportMembers (this.BusinessContext, this.Entity, startDate, comment);
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<AiderGroupEntity> ()
					.WithSpecialField<AiderGroupSpecialField<AiderGroupEntity>> ()
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
