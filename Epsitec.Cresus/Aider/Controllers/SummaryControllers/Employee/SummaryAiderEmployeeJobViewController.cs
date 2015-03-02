//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderEmployeeJobViewController : SummaryViewController<AiderEmployeeJobEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEmployeeJobEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			var canEditEmploye = user.CanEditEmployee () || user.CanEditReferee ();

			wall.AddBrick (x => x.Employee)
				.Icon ("Data.AiderUser")
				.Attribute (BrickMode.DefaultToSummarySubView);


			wall.AddBrick ()
				.Attribute (BrickMode.DefaultToNoSubView).IfFalse (user.CanEditEmployee ())
				.EnableActionButton<ActionAiderEmployeeJobViewController01RemoveJob> ();

			wall.AddBrick (x => x.Office)
				.Icon ("Base.AiderGoup.Parish")
				.Title ("Gestion associée")
				.Text (p => p.GetCompactSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView);
		}
	}
}

