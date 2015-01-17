//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderMailingCategoryViewController : EditionViewController<AiderMailingCategoryEntity>
	{
		protected override void CreateBricks(BrickWall<AiderMailingCategoryEntity> wall)
		{
			wall.AddBrick ()
				.Title (this.Entity.GetCompactSummary())
				.Icon (Res.Commands.Base.ShowAiderMailingCategory.Caption.Icon)
				.Input ()
					.Field (x => x.Name)
					.Field (x => x.Group).WithSpecialField<AiderGroupSpecialField<AiderMailingCategoryEntity>> ()
				.End ();
		}
	}
}
