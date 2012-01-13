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
					.Field (x => x.Description)
					.Field (x => x.AddressLine1)
					.Field (x => x.PostBox)
					.Field (x => x.Street)
					.Field (x => x.HouseNumber)
					.Field (x => x.HouseNumberComplement)
					.Field (x => x.Town)
					.Field (x => x.Phone1)
					.Field (x => x.Phone2)
					.Field (x => x.Email)
				.End ();
		}
		

	}


}
