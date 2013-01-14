using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.ActionControllers
{


	[ControllerSubType (0)]
	public sealed class ActionAiderGroupViewController0 : ActionViewController<AiderGroupEntity>
	{


		public override FormattedText GetTitle()
		{
			return "Créer un sous groupe";
		}


		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity, string> (this.Execute);
		}


		private void Execute(AiderGroupEntity group, string name)
		{
			group.CreateSubGroup (this.BusinessContext, name);
		}


		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title ("Créer un sous groupe")
				.Field<string> ()
					.Title ("Nom du sous groupe")
				.End ()
			.End ();
		}
	}


}
