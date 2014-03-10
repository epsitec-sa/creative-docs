//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderOfficeSettingsViewController : EditionViewController<AiderOfficeSettingsEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeSettingsEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
				.End ()
				.Input ()
					.Field (x => x.OfficialContact)
					.Field (x => x.OfficeAddress)
					.Field (x => x.PPFrankingTown)
				.End ()
				.Input ()
					.Field (x => x.OfficeLogoImagePath)
				.End ();
		}
	}
}
