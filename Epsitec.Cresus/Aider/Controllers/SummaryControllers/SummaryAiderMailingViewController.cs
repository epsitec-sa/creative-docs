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
				.WithSpecialController (typeof (SummaryAiderMailingViewController1Recipients))
				.EnableActionMenu<ActionAiderMailingViewController3KeepUpdated> ()
				.EnableActionMenu<ActionAiderMailingViewController2Duplicate> ()
				.EnableActionButton<ActionAiderMailingViewController2Duplicate> ()
				.EnableActionOnDrop<ActionAiderMailingViewController0AddRecipientOnDrop> ();
	
			/*wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title ("Ajouter un contact")
				.Text ("Uniquement disponible à l'aide de l'arche")
				.EnableActionOnDrop<ActionAiderMailingViewController0AddContactToRecipientsOnDrop> ();

			wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title ("Ajouter un groupe")
				.Text ("Uniquement disponible à l'aide de l'arche")
				.EnableActionOnDrop<ActionAiderMailingViewController2AddGroupToRecipientsOnDrop> ();

			wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title ("Ajouter un ménage")
				.Text ("Uniquement disponible à l'aide de l'arche")
				.EnableActionOnDrop<ActionAiderMailingViewController4AddHouseholdToRecipientsOnDrop> ();

			wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title ("Exclure un contact")
				.Text ("Uniquement disponible à l'aide de l'arche")
				.EnableActionOnDrop<ActionAiderMailingViewController1AddContactToExclusionsOnDrop> ();*/
		}
	}
}
