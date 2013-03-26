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
	[ControllerSubType (5)]
	public sealed class ActionAiderGroupViewController5 : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Fusionner avec un groupe";
		}

		private string GetText()
		{
			return "Cette action va supprimer ce groupe et rajouter tous ses membres au groupe sélectionné.";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity, bool> (this.Execute);
		}

		private void Execute(AiderGroupEntity other, bool confirm)
		{
			if (!confirm)
			{
				var message = "Vous devez indiquer que vous voulez détruire ce groupe.";

				throw new BusinessRuleException (message);
			}

			if (other.IsNull ())
			{
				var message = "Aucun groupe séléctionné.";

				throw new BusinessRuleException (message);
			}

			if (!other.CanHaveMembers ())
			{
				var message = "Ce groupe ne peut pas avoir de membres.";

				throw new BusinessRuleException (message);
			}

			if (this.Entity == other)
			{
				var message = "Un groupe ne peut pas être fusionné avec lui-même";

				throw new BusinessRuleException (message);
			}

			if (!this.Entity.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			if (!other.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			this.Entity.Merge(this.BusinessContext, other);
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Text (this.GetText ())
				.Field<AiderGroupEntity> ()
					.WithSpecialField<AiderGroupSpecialField<AiderGroupEntity>> ()
					.Title ("Groupe")
				.End ()
				.Field<bool> ()
					.Title ("Je veux détruire ce groupe")
					.InitialValue (false)
				.End ()
			.End ();
		}
	}
}
