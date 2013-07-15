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

                default :
                    wall.AddBrick();
                    break;
            }
              
        }
	}
}
