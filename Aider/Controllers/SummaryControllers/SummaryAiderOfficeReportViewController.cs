//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.EditionControllers
{
    public sealed class SummaryAiderOfficeReportViewController : SummaryViewController<AiderOfficeReportEntity>
    {
        protected override void CreateBricks(BrickWall<AiderOfficeReportEntity> wall)
        {
            if (this.Entity is AiderEventOfficeReportEntity report)
            {
                if (report.Event.IsNull ())
                {
                    wall.AddBrick ()
                        .Title ("Acte supprimé")
                        .Text ("(aucune information)")
                        .Attribute (BrickMode.DefaultToNoSubView);
                }
                else
                {
                    wall.AddBrick (x => ((AiderEventOfficeReportEntity)x).Event)
                        .Title (report.Event.GetActTitle ())
                        .Attribute (BrickMode.DefaultToSummarySubView);
                }
            }
        }
    }
}
