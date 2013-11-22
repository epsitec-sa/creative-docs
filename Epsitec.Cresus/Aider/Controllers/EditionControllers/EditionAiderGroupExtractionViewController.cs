//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupExtractionViewController : EditionViewController<AiderGroupExtractionEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupExtractionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
					.Field (x => x.SearchGroup)
						.WithSpecialField<AiderGroupSpecialField<AiderGroupExtractionEntity>> ()
					.Field (x => x.Match)
					.Field (x => x.SearchPath)
						.ReadOnly ().IfFalse (this.HasUserPowerLevel (UserPowerLevel.Administrator))
				.End ();
		}
	}
}

