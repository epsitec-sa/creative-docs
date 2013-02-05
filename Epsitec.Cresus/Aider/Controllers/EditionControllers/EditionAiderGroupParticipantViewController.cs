//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
						.Title ("Date d'entrée et de sortie")
						.Field (x => x.StartDate)
							.ReadOnly ()
						.Field (x => x.EndDate)
							.ReadOnly ()
					.End ()
					.Field (x => x.Group)
						.ReadOnly ()
					.Field (x => x.Person)
						.ReadOnly ()
					// This field has been made readonly because this functionnality is not yet
					// fully implemented and we dont' want the users to mess up this data.
					.Field (x => x.Comment.Text)
						.ReadOnly ()
				.End ();
		}
	}
}
