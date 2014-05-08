//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupViewController : EditionViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupEntity> wall)
		{
			var group = this.Entity;
			var groupDef = group.GroupDef;

			if (groupDef.IsNull ())
			{
				this.AddBrickWithoutDefinition (wall);
			}
			else
			{
				this.AddBrickWithDefinition (wall);
			}
		}

		
		private void AddBrickWithDefinition(BrickWall<AiderGroupEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.GroupDef)
						.ReadOnly ()
					.Field (x => x.GroupDef.NameArticle)
						.IfTrue (this.HasUserPowerLevel (Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator))
						.ReadOnly ()
					.Field (x => x.Name)
						.ReadOnly (this.Entity.CanBeRenamedByCurrentUser () == false)
					.Field (x => x.Path)
						.IfTrue (this.HasUserPowerLevel (Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator))
						.ReadOnly ()
				.End ();
		}

		private void AddBrickWithoutDefinition(BrickWall<AiderGroupEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
					.Field (x => x.Path)
						.IfTrue (this.HasUserPowerLevel (Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator))
				.End ();
		}
	}
}
