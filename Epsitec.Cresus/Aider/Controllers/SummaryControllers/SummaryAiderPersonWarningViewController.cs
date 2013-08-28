//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				case WarningType.EChPersonMissing:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController0SetVisibility> ();
					break;
				
				case WarningType.EChProcessDeparture:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController0SetVisibility> ();
					break;
				
				case WarningType.EChProcessArrival:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController1DiscardWarning> ();
					break;
				
				case WarningType.EChPersonDataChanged:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController1DiscardWarning> ();
					break;

				case WarningType.EChHouseholdAdded:
                    this.AddDefaultBrick(wall)
                        .EnableAction<ActionAiderPersonWarningViewController3ProcessNewHousehold> ();
                    break;

                case WarningType.EChHouseholdChanged:
                    this.AddDefaultBrick(wall)
                        .EnableAction<ActionAiderPersonWarningViewController1DiscardWarning> ();
                    break;

				case WarningType.EChAddressChanged:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController2Relocate> ();
					break;
				
				case WarningType.ParishArrival:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController2Relocate> ();
					break;
				
				case WarningType.ParishDeparture:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController1DiscardWarning> ();
					break;
				
				default:
					this.AddDefaultBrick (wall);
					break;
			}
		}

		private SimpleBrick<AiderPersonWarningEntity> AddDefaultBrick(BrickWall<AiderPersonWarningEntity> wall)
		{
			return wall.AddBrick ()
				.Title (x => x.WarningType);
		}
	}
}
