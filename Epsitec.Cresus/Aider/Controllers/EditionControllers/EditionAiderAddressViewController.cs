//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderAddressViewController : EditionViewController<AiderAddressEntity>
	{
		protected override void CreateBricks(BrickWall<AiderAddressEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Town)
					.Field (x => x.AddressLine1)
					.Field (x => x.StreetUserFriendly)
					.Field (x => x.HouseNumberAndComplement)
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
