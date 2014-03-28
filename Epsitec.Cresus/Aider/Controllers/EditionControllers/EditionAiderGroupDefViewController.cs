//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
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
			bool isRoot = UserManager.Current.AuthenticatedUser.LoginName == "root";

			wall.AddBrick ()
				.EnableActionMenu<ActionAiderGroupDefViewController0CreateSubGroupDef> ()
				.EnableActionMenu<ActionAiderGroupDefViewController1AssignFunctions> ()
				.EnableActionMenu<ActionAiderGroupDefViewController2DeleteAll> ()
				//.IfTrue (this.Entity.SubgroupsAllowed && this.HasUserPowerLevel (UserPowerLevel.Administrator))
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
						.IfFalse (isRoot)
					.Field (x => x.MembersAllowed)
						.ReadOnly ()
						.IfFalse (isRoot)
					.Field (x => x.MembersReadOnly)
						.ReadOnly ()
						.IfFalse (isRoot)
					.Field (x => x.PathTemplate)
						.ReadOnly ()
					.Field (x => x.Classification)
						.ReadOnly ()
						.IfFalse (isRoot)
					.Field (x => x.Mutability)
						.ReadOnly ()
						.IfFalse (isRoot)
					.Field (x => x.Function)
						.WithSpecialField<AiderGroupDefSpecialField<AiderGroupDefEntity>>()
						.ReadOnly ()
						.IfFalse (isRoot)
				.End ();
		}
	}
}
