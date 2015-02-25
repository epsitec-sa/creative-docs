//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Aider.Override;
using Epsitec.Aider.Controllers.SetControllers;
using Epsitec.Aider.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderEventPlaceViewController : SummaryViewController<AiderEventPlaceEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventPlaceEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			if(user.CanViewOfficeDetails () || user.Office.OfficeName == this.Entity.OfficeOwner.OfficeName)
			{
				wall.AddBrick ()
					.Icon ("Base.AiderPlace")
					.Title ("Lieu de célébration")
					.Text (x => x.GetSummary ())
					.Attribute (BrickMode.DefaultToCreationOrEditionSubView);
			}
			else
			{
				wall.AddBrick ()
					.Icon ("Base.AiderPlace")
					.Title ("Lieu de célébration")
					.Text (x => x.GetSummary ())
					.Attribute (BrickMode.DefaultToNoSubView);
			}
		}
	}
}