//	Copyright © 2015-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
    public sealed class SummaryAiderEmployeeJobViewController : SummaryViewController<AiderEmployeeJobEntity>
    {
        protected override void CreateBricks(BrickWall<AiderEmployeeJobEntity> wall)
        {
            var user = AiderUserManager.Current.AuthenticatedUser;
            var canEditEmployee = user.CanEditEmployee ();

            wall.AddBrick (x => x.Employee)
                .Icon ("Data.AiderUser")
                .Attribute (BrickMode.DefaultToSummarySubView);


            wall.AddBrick ()
                .Attribute (BrickMode.DefaultToNoSubView).IfFalse (canEditEmployee)
                .EnableActionMenu<ActionAiderEmployeeJobViewController01RemoveJob> ().IfTrue (canEditEmployee);

            wall.AddBrick (x => x.Office)
                .Icon ("Base.AiderGoup.Parish")
                .Title ("Gestion associée")
                .Text (p => p.GetCompactSummary ())
                .Attribute (BrickMode.DefaultToSummarySubView);
        }
    }
}
