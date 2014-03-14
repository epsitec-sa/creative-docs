//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderOfficeManagementViewController : EditionViewController<AiderOfficeManagementEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.OfficeMainContact)
				.End ()
				.Input ()
					.Field (x => x.PostalTown)
				.End ();
		}
	}
}
