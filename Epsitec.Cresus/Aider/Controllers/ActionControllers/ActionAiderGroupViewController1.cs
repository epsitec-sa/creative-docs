using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderGroupViewController1 : TemplateActionViewController<AiderGroupEntity, AiderGroupEntity>
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

		public override bool RequiresAdditionalEntity()
		{
			return true;
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var group = this.Entity;
			var subgroup = this.AdditionalEntity;

			if (subgroup.GroupDef.IsNotNull ())
			{
				var message = "Ce groupe ne peut pas être détruit puisqu'il correspond à une définition";

				throw new BusinessRuleException (message);
			}

			group.DeleteSubgroup (this.BusinessContext, subgroup);
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Text (this.GetText ())
			.End ();
		}
	}
}
