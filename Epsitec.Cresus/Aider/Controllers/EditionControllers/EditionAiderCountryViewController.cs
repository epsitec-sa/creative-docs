using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;


namespace Epsitec.Aider.Controllers.EditionControllers
{


	public sealed class EditionAiderCountryViewController : EditionViewController<AiderCountryEntity>
	{


		protected override void CreateBricks(BrickWall<AiderCountryEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.IsoCode)
					.Field (x => x.Name)
				.End ();					
		}


	}


}
