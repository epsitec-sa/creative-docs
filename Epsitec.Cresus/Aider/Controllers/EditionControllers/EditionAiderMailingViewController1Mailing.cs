//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	public sealed class EditionAiderMailingViewController1Mailing : EditionViewController<AiderMailingEntity>
	{
		protected override void CreateBricks(BrickWall<AiderMailingEntity> wall)
		{
			wall.AddBrick ()
				.Title (this.Entity.GetCompactSummary())
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Input ()
					.Field (x => x.Name)
					.Field (x => x.Category)
					.Field (x => x.IsReady)
					.Field (x => x.Sharing)
					.Field (x => x.Description)
				.End ();
		}
	}
}
