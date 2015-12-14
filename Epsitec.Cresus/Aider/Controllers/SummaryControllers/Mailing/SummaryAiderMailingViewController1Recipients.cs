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
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Aider.Controllers.SetControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (1)]
	public sealed class SummaryAiderMailingViewController1Recipients : SummaryViewController<AiderMailingEntity>
	{
		protected override void CreateBricks(BrickWall<AiderMailingEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			if (Date.FromObject (this.Entity.LastUpdate.Value.AddMonths (1)) < Date.Today)
			{
				var userName = user.LoginName;
				var message  = new NotificationMessage ()
				{
					Title = "Mise à jour du mailing recommandée",
					Body  = "Ce mailing n'a pas été mis à jour depuis plus d'un mois"
				};

				NotificationManager.GetCurrentNotificationManager ().Notify (userName, message, When.Now);
			}

			wall.AddBrick ()
				.Icon ("Data.AiderMailing")
				.Title ("Détails du mailing")
				.Text (x => "Modifier " + x.GetCompactSummary ())
				.WithSpecialController (typeof (EditionAiderMailingViewController1Mailing));

			wall.AddBrick ()
				.Icon ("Data.AiderMailing")
				.Title ("Modifer les destinataires")
				.Text (x => x.GetRecipientsOverview ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.EnableActionButton<ActionAiderMailingViewController11AddRecipientFromBag> ()
				.EnableActionOnDrop<ActionAiderMailingViewController0AddRecipientOnDrop> ()
				.EnableActionButton<ActionAiderMailingViewController3UpdateMailing> ()
				.WithSpecialController (typeof (SummaryAiderMailingViewController2Build));

			wall.AddBrick ()
				.Icon ("Data.AiderGroup.People")
				.Title (p => p.GetRecipientsTitleSummary ())
				.Text (p => p.GetRecipientsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderMailingViewController0RecipientsContact));


			wall.AddBrick ()
				.Icon ("Data.AiderGroup.Exclusions")
				.Title (p => p.GetExclusionsTitleSummary ())
				.Text (p => p.GetExclusionsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.EnableActionMenu<ActionAiderMailingViewController26AddHouseholdExclusion> ()
				.WithSpecialController (typeof (SetAiderMailingViewController1ExcludedContact));
		}
	}
}
