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
			
			wall.AddBrick ()
				.Icon (Res.Commands.Base.ShowAiderMailing.Caption.Icon)
				.Title ("Modifier")
				.Text (x => x.GetSummary ())
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
		}
	}
}
