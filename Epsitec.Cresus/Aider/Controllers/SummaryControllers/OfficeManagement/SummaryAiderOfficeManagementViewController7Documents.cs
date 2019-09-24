//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
    [ControllerSubType (7)]
    public sealed class SummaryAiderOfficeManagementViewController7Documents : SummaryViewController<AiderOfficeManagementEntity>
    {
        protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
        {
            foreach (var type in AiderOfficeManagementEntity.GetEventTypes ())
            {
                wall.AddBrick ()
                    .Icon ("Data.ArticleAccountingDefinition")
                    .Title (p => p.GetDocumentTitleSummary (type))
                    .Text (p => p.GetDocumentsSummary (type))
                    .Attribute (BrickMode.DefaultToSummarySubView)
                    .WithSpecialController (typeof (SummaryAiderOfficeManagementViewController8Documents), arg: type.ToString ());
            }
        }
    }
}
