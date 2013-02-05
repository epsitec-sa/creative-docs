//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupViewController : EditionViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupEntity> wall)
		{
			// Some fields have been made readonly because this functionnality is not yet fully
			// implemented and we dont' want the users to mess up this data.

			wall.AddBrick ()
				.Input ()
					.HorizontalGroup ()
						.Title ("Dates de début et de fin")
						.Field (x => x.StartDate).ReadOnly ()
						.Field (x => x.EndDate).ReadOnly ()
					.End ()
					.Field (x => x.Path).ReadOnly ()
					.Field (x => x.GroupLevel).ReadOnly ()
					.Field (x => x.GroupDef).ReadOnly ()
					.Field (x => x.Name).ReadOnly ()
				.End ();
		}
	}
}
