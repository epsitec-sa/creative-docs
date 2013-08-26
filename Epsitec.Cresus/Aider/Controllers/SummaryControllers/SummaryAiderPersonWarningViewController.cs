//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderPersonWarningViewController : SummaryViewController<AiderPersonWarningEntity>
	{
        protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
        {

            switch (this.Entity.WarningType)
            {
                case WarningType.MissingECh: 
                    wall.AddBrick()
                        .EnableAction<ActionAiderPersonWarningViewController0SetVisibility>();
                    break;
                case WarningType.DepartureProcessNeeded:
                    wall.AddBrick()
                        .EnableAction<ActionAiderPersonWarningViewController0SetVisibility>();
                    break;
                case WarningType.ArrivalProcessNeeded:
                    wall.AddBrick()
                        .EnableAction<ActionAiderPersonWarningViewController1DiscardWarning>();
                    break;
                case WarningType.DataChangedECh:
                    wall.AddBrick()
                        .EnableAction<ActionAiderPersonWarningViewController1DiscardWarning>();
                    break;
                case WarningType.AddressChange:
                    wall.AddBrick()
                        .EnableAction<ActionAiderPersonWarningViewController2Relocate>();
                    break;
                case WarningType.ParishArrival:
                    wall.AddBrick()
                        .EnableAction<ActionAiderPersonWarningViewController2Relocate>();
                    break;
                case WarningType.ParishDeparture:
                    wall.AddBrick()
                        .EnableAction<ActionAiderPersonWarningViewController1DiscardWarning>();
                    break;
                default :
                    wall.AddBrick();
                    break;
            }
              
        }
	}
}
