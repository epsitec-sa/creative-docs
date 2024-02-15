//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Common.Support;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	public sealed class EditionAiderLegalPersonViewController1Address : EditionViewController<AiderLegalPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderLegalPersonEntity> wall)
		{
            var user = AiderUserManager.Current.AuthenticatedUser;
            var favorites = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, user);

			wall.AddBrick ()
				.Title (Resources.Text ("Adresse de base"))
				.Icon ("Data.AiderAddress")
				.Input ()
					.Field (x => x.Address.Town).WithFavorites (favorites)
					.Field (x => x.Address.AddressLine1)
					.Field (x => x.Address.StreetHouseNumberAndComplement)
					.Field (x => x.Address.PostBox)
				.End ()
				.Input ()
					.Field (x => x.Address.Phone1)
					.Field (x => x.Address.Phone2)
					.Field (x => x.Address.Mobile)
					.Field (x => x.Address.Fax)
					.Field (x => x.Address.Email)
					.Field (x => x.Address.Web)
				.End ()
				.Input ()
					.Field (x => x.Address.Comment.Text)
                    .IfTrue (user.CanAccessComment ())
                .End ();
		}
	}
}