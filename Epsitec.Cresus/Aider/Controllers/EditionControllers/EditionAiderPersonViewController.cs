using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;


namespace Epsitec.Aider.Controllers.EditionControllers
{


	public sealed class EditionAiderPersonViewController : EditionViewController<AiderPersonEntity>
	{


		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.DisplayName)
					.Field (x => x.CallName)
					.Field (x => x.OriginalName)
					.Field (x => x.Title)
					.Field (x => x.MrMrs)
					.Field (x => x.Profession)
					.Field (x => x.Confession)
					.Field (x => x.CodeId)
					.Field (x => x.ValidationState)
					.Field (x => x.Household)
					.Field (x => x.AdditionalAddress1)
					.Field (x => x.AdditionalAddress2)
					.Field (x => x.Language)
					.Field (x => x.Comment)
				.End ();
		}


	}


}
