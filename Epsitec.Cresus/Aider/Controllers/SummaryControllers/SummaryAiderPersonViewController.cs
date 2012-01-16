using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{


	public sealed class SummaryAiderPersonViewController : SummaryViewController<AiderPersonEntity>
	{


		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick (x => x);

			if (this.Entity.IsGovernmentDefined ())
			{
				wall.AddBrick (x => x.eCH_Person.Address);
			}

			wall.AddBrick (x => x.AdditionalAddress1);
			// This lines crashes because of a bug in ActionItemGenerator
			//wall.AddBrick (x => x.AdditionalAddress2);

			wall.AddBrick (x => x.Household);
		}


	}


}
