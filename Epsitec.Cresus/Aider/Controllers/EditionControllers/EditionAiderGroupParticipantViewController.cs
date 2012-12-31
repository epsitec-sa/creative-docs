//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupParticipantViewController : EditionViewController<AiderGroupParticipantEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupParticipantEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.HorizontalGroup ()
						.Title ("Date de début et de fin")
						.Field (x => x.StartDate)
						.Field (x => x.EndDate)
					.End ()
					.Field (x => x.Group)
						.WithSpecialField<AiderGroupSpecialField<AiderGroupParticipantEntity>> ()
					.Field (x => x.Person)
				.End ();
		}
	}
}
