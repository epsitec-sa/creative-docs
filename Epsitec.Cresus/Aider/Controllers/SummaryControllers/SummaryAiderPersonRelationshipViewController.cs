using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	
	
	public sealed class SummaryAiderPersonRelationshipViewController : SummaryViewController<AiderPersonRelationshipEntity>
	{


		protected override void CreateBricks(BrickWall<AiderPersonRelationshipEntity> wall)
		{
			wall.AddBrick (x => x);
		}


	}


}
