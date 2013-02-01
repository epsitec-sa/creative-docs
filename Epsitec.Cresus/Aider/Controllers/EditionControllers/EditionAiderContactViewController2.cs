//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (2)]
	internal sealed class EditionAiderContactViewController2 : EditionViewController<AiderContactEntity>
	{
		protected override void CreateBricks(BrickWall<AiderContactEntity> wall)
		{
			wall.AddBrick ()
				.Title (Resources.Text ("Détails du ménage"))
				.Icon ("Data.AiderAddress")
				.Input ()
					.Field (x => x.HouseholdRole)
					.Field (x => x.Household.HouseholdMrMrs)
					.Field (x => x.Household.HouseholdName)
				.End ();
		}
	}
}
