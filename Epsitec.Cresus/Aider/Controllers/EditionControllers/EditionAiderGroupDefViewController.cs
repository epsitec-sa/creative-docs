//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupDefViewController : EditionViewController<AiderGroupDefEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupDefEntity> wall)
		{
			wall.AddBrick ()
				.EnableActionMenu<ActionAiderGroupDefViewController0CreateSubGroupDef> ().IfTrue (this.HasUserPowerLevel			(UserPowerLevel.Administrator) && this.Entity.SubgroupsAllowed)
				.Input ()
					.Field (x => x.Name)
						.ReadOnly ()
						.IfFalse (this.HasUserPowerLevel (UserPowerLevel.Administrator))
					.Field (x => x.Number)
						.ReadOnly ()
					.Field (x => x.Level)
						.ReadOnly ()
					.Field (x => x.SubgroupsAllowed)
						.ReadOnly ()
					.Field (x => x.MembersAllowed)
						.ReadOnly ()
					.Field (x => x.PathTemplate)
						.ReadOnly ()
					.Field (x => x.Classification)
						.ReadOnly ()
					.Field (x => x.Mutability)
						.ReadOnly ()
					.Field (x => x.Function)
						.ReadOnly ()
				.End ();
		}
	}
}
