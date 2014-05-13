//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	public sealed class EditionAiderEmployeeViewController : EditionViewController<AiderEmployeeEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEmployeeEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.EmployeeType)
					.Field (x => x.EmployeeActivity)
					.Field (x => x.Description)
					.Field (x => x.Navs13)
				.End ();
		}
	}
}
