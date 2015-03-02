//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderEventViewController : EditionViewController<AiderEventEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventEntity> wall)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;
			var favorites = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);
			var favoritesPlaces = AiderOfficeManagementEntity.GetOfficeEventPlaces (this.BusinessContext, this.Entity.Office);

			wall.AddBrick ()
				.Input ()
					.Field (x => x.Type)
					.Field (x => x.State).ReadOnly ()
					.Field (x => x.Place)
						.WithFavorites (favoritesPlaces)
					.Field (x => x.Town)
						.Title ("Localité")
						.WithFavorites (favorites)
					.Field (x => x.Date)
					.Field (x => x.Office).ReadOnly ()
					.Field (x => x.Kind)
					.Field (x => x.Description)
				.End ();
		}
	}
}

