using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;


namespace Epsitec.Aider.Controllers.EditionControllers
{


	public sealed class EditionAiderTownViewController : EditionViewController<AiderTownEntity>
	{


		protected override void CreateBricks(BrickWall<AiderTownEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.SwissZipCode)
					.Field (x => x.Name)
					.Field (x => x.Country)
				.End ();
		}


	}


}
