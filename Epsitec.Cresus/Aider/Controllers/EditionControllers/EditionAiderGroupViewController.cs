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
#if ENABLE_GROUPS
			wall.AddBrick ()
				.Input ()
					.HorizontalGroup ()
						.Title ("Dates de début et de fin")
						.Field (x => x.StartDate)
						.Field (x => x.EndDate)
					.End ()
					.Field (x => x.Path).ReadOnly ()
					.Field (x => x.GroupLevel).ReadOnly ()
					.Field (x => x.GroupDef)
					.Field (x => x.Name)
				.End ();
#else
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
#endif
		}
	}
}
