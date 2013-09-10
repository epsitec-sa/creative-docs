﻿//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;

using Epsitec.Common.Support;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderHouseholdViewController : SummaryViewController<AiderHouseholdEntity>
	{
		protected override void CreateBricks(BrickWall<AiderHouseholdEntity> wall)
		{
			wall.AddBrick ()
				.EnableActionMenu<ActionAiderHouseholdViewController0NewHouseholdMember> ()
				.EnableActionMenu<ActionAiderHouseholdViewController1AddHouseholdMember> ();

			wall.AddBrick ()
				.Title (Resources.Text ("Adresse"))
				.Text (this.Entity.Address.GetSummary ())
				.Icon ("Data.AiderAddress")
				.WithSpecialController (typeof (EditionAiderHouseholdViewController1Address));

			wall.AddBrick (h => h.Members)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				.EnableActionMenu<ActionAiderHouseholdViewController2RemoveMemberFromHousehold> ()
				.EnableActionMenu<ActionAiderHouseholdViewController3ChangeHeadOfHousehold> ()
				.Template ()
					.Icon ("Data.AiderPersons")
					.Title ("Membres du ménage")
					.Text (p => p.GetCompactSummary (this.Entity))
				.End ();

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
