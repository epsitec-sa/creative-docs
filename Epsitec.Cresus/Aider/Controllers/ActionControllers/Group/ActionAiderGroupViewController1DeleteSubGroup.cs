//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderGroupViewController1DeleteSubGroup : TemplateActionViewController<AiderGroupEntity, AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Supprimer le sous groupe sélectionné";
		}

		public FormattedText GetText()
		{
			var format = "Voulez-vous vraiment supprimer \"{0}\" et tous ses sous groupes?";

			return string.Format (format, this.AdditionalEntity.Name);
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		
		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Text (this.GetText ())
			.End ();
		}

		
		private void Execute()
		{
			var group = this.Entity;
			var subgroup = this.AdditionalEntity;

			if (!subgroup.CanBeEdited ())
			{
				var message = "Ce groupe ne peut pas être détruit.";

				throw new BusinessRuleException (message);
			}

			if (!subgroup.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			group.DeleteSubgroup (this.BusinessContext, subgroup);
		}
	}
}
