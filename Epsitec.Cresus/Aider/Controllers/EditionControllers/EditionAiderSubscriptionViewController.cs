//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Linq;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

using Epsitec.Aider.Entities;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	class EditionAiderSubscriptionViewController : EditionViewController<AiderSubscriptionEntity>
	{
		protected override void CreateBricks(BrickWall<AiderSubscriptionEntity> wall)
		{
			var favorites = AiderGroupEntity.FindRegionRootGroups (this.BusinessContext);

			//Remove PLA from list
			favorites     = favorites.Where (f => f.Path != "R012.").ToList ();

			wall.AddBrick ()
				.Input ()
					.Field (x => x.Id)
						.ReadOnly ()
					.Field (x => x.Count)
					.Field (x => x.RegionalEdition)
						.WithFavorites (favorites, favoritesOnly: true)
					.Field (x => x.SusbscriptionFlag)
					.Field (x => x.ParishGroupPathCache)
						.IfTrue (this.HasUserPowerLevel (Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator))
						.ReadOnly ()
				.End ();
		}
	}
}
