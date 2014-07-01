//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (11)]
	public sealed class ActionAiderGroupViewController11AddPlaGroup : TemplateActionViewController<AiderGroupEntity, AiderGroupEntity>
	{
		public override bool RequiresAdditionalEntity
		{
			get
			{
				return false;
			}
		}


		public override FormattedText GetTitle()
		{
			return "Associer à une paroisse territoriale";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity> (this.Execute);
		}


		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<AiderGroupEntity> ()
					.WithSpecialField<AiderGroupSpecialField<AiderGroupEntity>> ()
					.Title ("Paroisse territoriale")
				.End ()
			.End ();
		}

		private void Execute(AiderGroupEntity parishGroup)
		{
			this.Entity.AddPlaParishGroup (this.BusinessContext, parishGroup);
		}
	}
}

