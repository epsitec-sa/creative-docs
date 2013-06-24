//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Diagnostics;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderAddressViewController : EditionViewController<AiderAddressEntity>
	{
		protected override void CreateBricks(BrickWall<AiderAddressEntity> wall)
		{
			// We print a warning in the console because this controller should not be called. The
			// problem is that if we edit an address with it, the business rules of the entities
			// related to the address won't be triggered and that might lead to inconsitencies in
			// their data.
			Debug.WriteLine ("Warning: EditionAiderAddressViewController has been called.");

			var currentUser = UserManager.Current.AuthenticatedUser;
			var favorites = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);

			wall.AddBrick ()
				.Input ()
					.Field (x => x.Town)
						.WithFavorites (favorites)
					.Field (x => x.AddressLine1)
					.Field (x => x.StreetHouseNumberAndComplement)
					.Field (x => x.PostBox)
				.End ()
				.Input ()
					.Field (x => x.Phone1)
					.Field (x => x.Phone2)
					.Field (x => x.Mobile)
					.Field (x => x.Fax)
					.Field (x => x.Email)
					.Field (x => x.Web)
				.End ()
				.Input ()
					.Field (x => x.Comment.Text)
				.End ();
		}
	}
}
