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
	public sealed class EditionAiderEmployeeJobViewController : EditionViewController<AiderEmployeeJobEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEmployeeJobEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			wall.AddBrick ()
				.Input ()
					.Field (x => x.Office)
					.Field (x => x.Description)
					.Field (x => x.EmployeeJobFunction).ReadOnly ().IfFalse (user.CanEditEmployee ())
					.Field (x => x.Employer).ReadOnly ().IfFalse (user.CanEditEmployee ())
				.End ();
		}
	}
}

