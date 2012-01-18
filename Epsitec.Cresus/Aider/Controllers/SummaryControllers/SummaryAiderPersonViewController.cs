using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;


namespace Epsitec.Aider.Controllers.SummaryControllers
{


	public sealed class SummaryAiderPersonViewController : SummaryViewController<AiderPersonEntity>
	{


		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick (x => x);
			wall.AddBrick (x => x)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Text ("Pour en savoir plus...");	//	Just to check that this is possible

			if (this.Entity.IsGovernmentDefined ())
			{
				wall.AddBrick (x => x.eCH_Person.Address);
			}

			wall.AddBrick (x => x.AdditionalAddress1)
				.Title ("Adresse supplémentaire 1");
			wall.AddBrick (x => x.AdditionalAddress2)
				.Title ("Adresse supplémentaire 2");

			wall.AddBrick (x => x.Household);
		}


	}


}
