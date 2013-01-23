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
					.Field (x => x.Type)
					.Field (x => x.AddressLine1)
					.Field (x => x.PostBox)
					.Field (x => x.StreetUserFriendly)
					.Field (x => x.HouseNumberAndComplement)
					.Field (x => x.Town)
					.Field (x => x.Phone1)
					.Field (x => x.Phone2)
					.Field (x => x.Mobile)
					.Field (x => x.Email)
					.Field (x => x.Web)
				.End ();

			// TODO Make this work. Now it crashes probably because the entity is null.
			//wall.AddBrick ()
			//	.Include (x => x.Comment);
		}


	}


}
