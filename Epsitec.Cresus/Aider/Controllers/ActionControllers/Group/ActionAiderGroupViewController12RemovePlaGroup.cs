//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (12)]
	public sealed class ActionAiderGroupViewController12RemovePlaGroup : TemplateActionViewController<AiderGroupEntity, AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Retirer un groupe territorial";
		}

		public FormattedText GetText()
		{
			var format = "Voulez-vous vraiment retirer \"{0}\" ?";

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
			var pla    = this.Entity;
			var parish = this.AdditionalEntity;

			pla.RemovePlaParishGroup (this.BusinessContext, parish);
		}
	}
}

