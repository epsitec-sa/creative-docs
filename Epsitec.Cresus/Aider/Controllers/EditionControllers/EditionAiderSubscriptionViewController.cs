//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	class EditionAiderSubscriptionViewController : EditionViewController<AiderSubscriptionEntity>
	{
		protected override void CreateBricks(BrickWall<AiderSubscriptionEntity> wall)
		{
			var favorites = AiderGroupEntity.FindRegionRootGroups (this.BusinessContext);

			wall.AddBrick ()
				.Input ()
					.Field (x => x.Id)
						.ReadOnly ()
					.Field (x => x.Count)
					.Field (x => x.RegionalEdition)
						.WithFavorites (favorites, favoritesOnly: true)
						
				.End ();
		}
	}
}
