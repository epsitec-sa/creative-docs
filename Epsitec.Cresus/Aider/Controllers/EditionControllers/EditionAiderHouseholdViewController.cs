using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderHouseholdViewController : EditionViewController<AiderHouseholdEntity>
	{
		protected override void CreateBricks(BrickWall<AiderHouseholdEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.HouseholdMrMrs)
					.Field (x => x.Head1)
					.Field (x => x.Head2)
				.End ();
		}
	}
}
