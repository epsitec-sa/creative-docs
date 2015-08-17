//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using System.Linq;
using Epsitec.Aider.Controllers.SetControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (1)]
	public sealed class SummaryAiderEventViewController1Person : SummaryViewController<AiderEventEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventEntity> wall)
		{
			wall.AddBrick ()
					.Icon ("Data.AiderEvent")
					.Title (x => "Acte N° " + x.Report.EventNumberByYearAndRegistry)
					.Text (x => x.GetActSummary ())
					.Attribute (BrickMode.DefaultToNoSubView);

			if (this.Entity.Report.IsNotNull ())
			{
				wall.AddBrick (x => x.Report)
				.Attribute (BrickMode.DefaultToNoSubView);
			}


			wall.AddBrick ()
				.Title ("Participants")
				.Text (p => p.GetParticipantsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderEventViewController0Participants));
		}
	}
}
