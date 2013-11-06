//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;


using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Library;

using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderMailingViewController : SummaryViewController<AiderMailingEntity>
	{
		protected override void CreateBricks(BrickWall<AiderMailingEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title (x => TextFormatter.FormatText (x.GetCompactSummary ()))
				.Text (x => x.GetSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderMailingViewController1Recipients));
	
			wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title ("Actions disponibles")
				.Text ("Certaines actions sont uniquement disponible à l'aide de l'arche")
				.EnableActionButton<ActionAiderMailingViewController3KeepUpdated> ()
				.EnableActionOnDrop<ActionAiderMailingViewController0AddContactToRecipientsOnDrop> ()
				.EnableActionOnDrop<ActionAiderMailingViewController2AddGroupToRecipientsOnDrop> ()
				.EnableActionOnDrop<ActionAiderMailingViewController1AddContactToExclusionsOnDrop> ();
		}
	}
}
