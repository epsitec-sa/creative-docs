using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	
	
	public sealed class SummaryAiderAddressViewController : SummaryViewController<AiderAddressEntity>
	{


		protected override void CreateBricks(BrickWall<AiderAddressEntity> wall)
		{
			wall.AddBrick (x => x);
		}


	}


}
