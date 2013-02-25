//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderGroupViewController : SummaryViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupEntity> wall)
		{
			if (this.Entity.CanBeEdited ())
			{
				wall.AddBrick ()
					.EnableAction (2);
			}
			else
			{
				wall.AddBrick ();
			}

			if (this.Entity.CanHaveSubgroups ())
			{
				var bricks = wall.AddBrick (x => x.Subgroups)
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton);

				if (this.Entity.CanSubgroupsBeEdited ())
				{
					bricks = bricks
						.EnableAction (0)
						.EnableAction (1);
				}

				bricks = bricks
					.Template ()
						.Title ("Sous groupes")
					.End ();
			}

			if (this.Entity.CanHaveMembers ())
			{
				wall.AddBrick ()
					.Icon ("Data.AiderGroup.People")
					.Title (p => p.GetParticipantsTitle ())
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.Attribute (BrickMode.SpecialController0);
			}

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
