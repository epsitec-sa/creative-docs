//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderEventPlaceViewController : EditionViewController<AiderEventPlaceEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventPlaceEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
					.Field (x => x.OfficeOwner)
					.Field (x => x.Shared)
				.End ();
		}
	}
}
