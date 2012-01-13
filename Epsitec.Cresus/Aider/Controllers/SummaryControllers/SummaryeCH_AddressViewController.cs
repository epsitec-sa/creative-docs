using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{


	public sealed class SummaryeCH_AddressViewController : SummaryViewController<eCH_AddressEntity>
	{


		protected override void CreateBricks(BrickWall<eCH_AddressEntity> wall)
		{
			wall.AddBrick (x => x);
		}


	}


}
