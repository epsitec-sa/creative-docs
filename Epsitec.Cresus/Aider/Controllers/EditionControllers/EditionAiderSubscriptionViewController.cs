using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

using Epsitec.Aider.Entities;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	class EditionAiderSubscriptionViewController : EditionViewController<AiderSubscriptionEntity>
	{
		protected override void CreateBricks(BrickWall<AiderSubscriptionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Id)
						.ReadOnly ()
					.Field (x => x.Count)
					.Field (x => x.RegionalEdition)
				.End ();
		}
	}
}
