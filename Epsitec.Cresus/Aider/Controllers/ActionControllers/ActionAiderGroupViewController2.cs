using Epsitec.Aider.Entities;

using Epsitec.Aider.Controllers.SpecialFieldControllers;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using Epsitec.Aider.Data;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (2)]
	public sealed class ActionAiderGroupViewController2 : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Déplacer le groupe";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity> (this.Execute);
		}

		private void Execute(AiderGroupEntity newParent)
		{
			var group = this.Entity;

			if (!group.CanBeEdited ())
			{
				var message = "Ce groupe ne peut pas être déplacé.";

				throw new BusinessRuleException (message);
			}

			if (newParent.IsNull ())
			{
				var message = "Aucun parent séléctionné.";

				throw new BusinessRuleException (message);
			}

			if (!newParent.CanHaveSubgroups ())
			{
				var message = "Ce groupe ne peut pas avoir de sous groupes.";

				throw new BusinessRuleException (message);
			}

			if (newParent == this.Entity)
			{
				var message = "Un groupe ne peut pas être déplacé dans lui-même";

				throw new BusinessRuleException (message);
			}

			if (newParent.IsChild (this.Entity))
			{
				var message = "Un groupe ne peut pas être déplacé dans un de ses sous groupes.";

				throw new BusinessRuleException (message);
			}

			if (newParent.GroupLevel + this.Entity.GetDepth () > AiderGroupIds.MaxGroupLevel)
			{
				var message = "Impossible de créer plus de " + (AiderGroupIds.MaxGroupLevel + 1) + " niveaux de groupes";

				throw new BusinessRuleException (message);
			}

			if (newParent == this.Entity.Parent)
			{
				return;
			}

			this.Entity.Move (newParent);
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<AiderGroupEntity> ()
					.Title ("Nouveau parent")
					.InitialValue (this.Entity.Parent)
					.WithSpecialField<AiderGroupSpecialField<AiderGroupEntity>> ()
				.End ()
			.End ();
		}
	}
}
