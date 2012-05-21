using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;


namespace Epsitec.Aider.Controllers.EditionControllers
{


	public sealed class EditioneCH_AddressViewController : EditionViewController<eCH_AddressEntity>
	{


		protected override void CreateBricks(BrickWall<eCH_AddressEntity> wall)
		{
			wall.AddBrick ()
					.Input ()
						.Field (x => x.AddressLine1)
							.ReadOnly ()
						.Field (x => x.Street)
							.ReadOnly ()
						.Field (x => x.HouseNumber)
							.ReadOnly ()
						.Field (x => x.Town)
							.ReadOnly ()
						.Field (x => x.SwissZipCode)
							.ReadOnly ()
						.Field (x => x.Country)
							.ReadOnly ()
					.End ();
		}


	}


}
