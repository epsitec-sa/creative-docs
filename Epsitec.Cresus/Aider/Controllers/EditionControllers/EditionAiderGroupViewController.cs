//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupViewController : EditionViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupEntity> wall)
		{
			if (this.Entity.GroupDef.IsNotNull ())
			{
				this.AddBrickWithDefinition (wall);
			}
			else
			{
				this.AddBrickWithoutDefinition (wall);
			}
		}

		private void AddBrickWithDefinition(BrickWall<AiderGroupEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.GroupDef)
						.ReadOnly ()
					.Field (x => x.Name)
						.ReadOnly ()
				.End ();
		}

		private void AddBrickWithoutDefinition(BrickWall<AiderGroupEntity> wall)
		{
#if ENABLE_GROUPS
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
				.End ();
#else
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
						.ReadOnly ()
				.End ();
#endif
		}

	}
}
