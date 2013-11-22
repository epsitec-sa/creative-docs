﻿//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			
			wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title ("Modifier")
				.Text (x => x.GetRecipientsOverview ())
				.WithSpecialController (typeof (EditionAiderMailingViewController1Mailing));

			wall.AddBrick ()
					.Icon ("Data.AiderGroup.People")
					.Title (p => p.GetRecipientsTitleSummary ())
					.Text (p => p.GetRecipientsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderMailingViewController0RecipientsContact))
					.EnableActionMenu<ActionAiderMailingViewController3KeepUpdated> ();

			wall.AddBrick ()
					.Icon ("Data.AiderGroup.People")
					.Title (p => p.GetExclusionsTitleSummary ())
					.Text (p => p.GetExclusionsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderMailingViewController1ExcludedContact));

			wall.AddBrick (p => p.RecipientGroups)
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.EnableActionMenu<ActionAiderMailingViewController5AddGroup> ()
					.EnableActionMenu<ActionAiderMailingViewController6RemoveGroup> ()
					.EnableActionButton<ActionAiderMailingViewController5AddGroup> ()
					.EnableActionButton<ActionAiderMailingViewController6RemoveGroup> ()
					.Template ()
						.Title ("Groupes")
						.Text (x => x.GetCompactSummary ())
					.End ();

			wall.AddBrick (p => p.RecipientGroupExtractions)
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.EnableActionMenu<ActionAiderMailingViewController11AddGroupExtraction> ()
					.EnableActionMenu<ActionAiderMailingViewController12RemoveGroupExtraction> ()
					.EnableActionButton<ActionAiderMailingViewController11AddGroupExtraction> ()
					.EnableActionButton<ActionAiderMailingViewController12RemoveGroupExtraction> ()
					.Template ()
						.Title ("Groupes transversaux")
						.Text (x => x.GetCompactSummary ())
					.End ();

			wall.AddBrick (p => p.RecipientHouseholds)
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.EnableActionMenu<ActionAiderMailingViewController7AddHousehold> ()
					.EnableActionMenu<ActionAiderMailingViewController8RemoveHousehold> ()
					.EnableActionButton<ActionAiderMailingViewController7AddHousehold> ()
					.EnableActionButton<ActionAiderMailingViewController8RemoveHousehold> ()
					.Template ()
						.Title ("Ménages")
						.Text (x => x.GetCompactSummary ())
					.End ();

			wall.AddBrick (p => p.RecipientContacts)
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.EnableActionMenu<ActionAiderMailingViewController9AddContact> ()
					.EnableActionMenu<ActionAiderMailingViewController4RemoveContact> ()
					.EnableActionButton<ActionAiderMailingViewController9AddContact> ()
					.EnableActionButton<ActionAiderMailingViewController4RemoveContact> ()
					.Template ()
						.Title ("Contacts")
						.Text (x => x.GetCompactSummary ())
					.End ();
		}
	}
}
