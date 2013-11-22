//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Controllers.SetControllers;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Library;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderMailingViewController : SummaryViewController<AiderMailingEntity>
	{
		protected override void CreateBricks(BrickWall<AiderMailingEntity> wall)
		{
			wall.AddBrick ()
//				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderMailingViewController1Recipients))
//				.EnableActionMenu<ActionAiderMailingViewController3UpdateRecipients> ()
				.EnableActionMenu<ActionAiderMailingViewController2Duplicate> ()
				.EnableActionMenu<ActionAiderMailingViewController10AddToBag> ()
//				.EnableActionButton<ActionAiderMailingViewController2Duplicate> ()
				.EnableActionButton<ActionAiderMailingViewController11AddRecipientFromBag> ()
				.EnableActionOnDrop<ActionAiderMailingViewController0AddRecipientOnDrop> ();

			wall.AddBrick ()
				.Icon ("Data.AiderGroup.People")
				.Title (p => p.GetRecipientsTitleSummary ())
				.Text (p => p.GetRecipientsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderMailingViewController0RecipientsContact))
				.EnableActionButton<ActionAiderMailingViewController3UpdateRecipients> ();
		}
	}
}
