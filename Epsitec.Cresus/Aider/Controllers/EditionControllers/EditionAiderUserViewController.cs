//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class EditionAiderUserViewController : EditionViewController<AiderUserEntity>
	{
		protected override void CreateBricks(BrickWall<AiderUserEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			var senderDefined = this.Entity.Office.IsNotNull ();
			var contactDefined = this.Entity.Contact.IsNotNull ();

			if (user.HasPowerLevel (UserPowerLevel.Administrator))
			{
				EditionAiderUserViewController.AddUserDataBrick (wall, contactDefined, senderDefined);
			}
			else
			{
				EditionAiderUserViewController.AddUserDataBrickReadonly (wall, senderDefined);
			}
		}

		private static void AddUserDataBrick(BrickWall<AiderUserEntity> wall, bool contactDefined, bool senderDefined)
		{
			wall.AddBrick ()
				.EnableActionMenu<ActionAiderUserViewController0SetPassword> ()
				.EnableActionMenu<ActionAiderUserViewController1SetAdministrator> ()
				.EnableActionMenu<ActionAiderUserViewController2SetOffice> ().IfTrue (contactDefined)
				.Title (Res.Strings.AiderUserDataTitle)
				.Input ()
					.Field (x => x.Contact)
					.Field (x => x.Parish)
						.WithSpecialField<AiderGroupSpecialField<AiderUserEntity>> ()
					.Field (x => x.Office).ReadOnly ()
					.Field (x => x.OfficeSender).IfTrue (senderDefined)
					.Field (x => x.LoginName)
					.Field (x => x.DisplayName)
					.Field (x => x.Email)
					.Field (x => x.Role)
					.Field (x => x.PowerLevel)
					.Field (x => x.Disabled)
					.Field (x => x.EnableGroupEditionCanton)
					.Field (x => x.EnableGroupEditionRegion)
					.Field (x => x.EnableGroupEditionParish)
				.End ();
		}

		private static void AddUserDataBrickReadonly(BrickWall<AiderUserEntity> wall, bool senderDefined)
		{
			wall.AddBrick ()
				.EnableActionMenu<ActionAiderUserViewController0SetPassword> ()
				.Title (Res.Strings.AiderUserDataTitle)
				.Input ()
					.Field (x => x.Contact)
					.Field (x => x.Parish).ReadOnly ()
					.Field (x => x.Office).ReadOnly ()
					.Field (x => x.OfficeSender).IfTrue (senderDefined)
					.Field (x => x.LoginName).ReadOnly ()
					.Field (x => x.DisplayName).ReadOnly ()
					.Field (x => x.Email)
					.Field (x => x.Role).ReadOnly ()
					.Field (x => x.PowerLevel).ReadOnly ()
					.Field (x => x.Disabled).ReadOnly ()
					.Field (x => x.EnableGroupEditionCanton).ReadOnly ()
					.Field (x => x.EnableGroupEditionRegion).ReadOnly ()
					.Field (x => x.EnableGroupEditionParish).ReadOnly ()
				.End ();
		}
	}
}
