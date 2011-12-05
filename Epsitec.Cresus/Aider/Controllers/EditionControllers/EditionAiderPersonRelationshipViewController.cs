using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	
	
	public sealed class EditionAiderPersonRelationshipViewController : EditionViewController<AiderPersonRelationshipEntity>
	{


		protected override void CreateBricks(BrickWall<AiderPersonRelationshipEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Person1)
					.Field (x => x.Person2)
					.Field (x => x.Type)
					.Field (x => x.StartDate)
					.Field (x => x.EndDate)
					.Field (x => x.Comment)
				.End ();
		}


	}


}
