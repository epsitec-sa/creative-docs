using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderHouseholdViewController : SummaryViewController<AiderHouseholdEntity>
	{
		protected override void CreateBricks(BrickWall<AiderHouseholdEntity> wall)
		{
			wall.AddBrick ();

			wall.AddBrick (h => h.Address);

			wall.AddBrick ()
				.EnableAction (0)
				.EnableAction (1)
				.Title (h => h.GetMembersTitle ())
				.Text (h => h.GetMembersSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.SpecialController1);

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}