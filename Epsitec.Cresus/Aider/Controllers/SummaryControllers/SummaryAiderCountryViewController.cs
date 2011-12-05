using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{


	public sealed class SummaryAiderCountryViewController : SummaryViewController<AiderCountryEntity>
	{


		protected override void CreateBricks(BrickWall<AiderCountryEntity> wall)
		{
			wall.AddBrick (x => x);
		}


	}


}
