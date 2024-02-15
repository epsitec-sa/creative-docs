//	Copyright © 2013-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SpecializedSummaryAiderPersonWarningViewController_EChAddressChanged : SpecializedSummaryAiderPersonWarningViewController
	{
		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{
			wall.AddBrick ()
				.Title (x => x.WarningType)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderPersonWarningViewController1Details))
				.EnableActionButton<ActionAiderPersonWarningViewController9ProcessAddressChange> ()
				.EnableActionButton<ActionAiderPersonWarningViewController91ProcessAddressChange> ()
				.EnableActionButton<ActionAiderPersonWarningViewController100AddToBag> ();
		}
	}
}
