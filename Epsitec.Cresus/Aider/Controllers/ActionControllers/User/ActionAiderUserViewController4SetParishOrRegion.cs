//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP
using System.Linq;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using Epsitec.Aider.Controllers.SpecialFieldControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (4)]
	public sealed class ActionAiderUserViewController4SetParishOrRegion : ActionViewController<AiderUserEntity>
	{

		public override FormattedText GetTitle()
		{
			return "Définir ou changer la paroisse ou région";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity> (this.Execute);
		}

		private void Execute(AiderGroupEntity selected)
		{
			this.Entity.SetParishOrRegion (this.BusinessContext, selected);
		}

		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> form)
		{
			var user = this.Entity;

			form
				.Title ("Définir ou changer la paroisse ou région")		
					.Field<AiderGroupEntity> ()
						.Title ("Choisir un groupe ou une région")
						.InitialValue (user.Parish)
						.WithSpecialField<AiderGroupSpecialField<AiderUserEntity>> ()
					.End ()
				.End ();
		}
	}
}
