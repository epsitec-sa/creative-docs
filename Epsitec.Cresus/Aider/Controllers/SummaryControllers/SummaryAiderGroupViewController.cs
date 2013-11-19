//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SetControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderGroupViewController : SummaryViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupEntity> wall)
		{
			if (this.Entity.CanBeEdited ())
			{
				var bricks = wall.AddBrick ()
					.EnableActionMenu<ActionAiderGroupViewController6AddToBag> ()
					.EnableActionMenu<ActionAiderGroupViewController2MoveGroup> ();

				if (this.Entity.Subgroups.Count == 0)
				{
					bricks = bricks
						.EnableActionMenu<ActionAiderGroupViewController5MergeGroup> ();
				}
			}
			else
			{
				wall.AddBrick ()
					.EnableActionMenu<ActionAiderGroupViewController6AddToBag> ();
			}

			if (this.Entity.CanHaveSubgroups ())
			{
				var bricks = wall.AddBrick (x => x.Subgroups)
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton);

				if (this.Entity.CanSubgroupsBeEdited () && this.Entity.CanBeEditedByCurrentUser ())
				{
					bricks = bricks
						.EnableActionMenu<ActionAiderGroupViewController0CreateSubGroup> ()
						.EnableActionMenu<ActionAiderGroupViewController1DeleteSubGroup> ();
				}

				bricks = bricks
					.Template ()
						.Title ("Sous groupes")
						.Text (x => x.GetCompactSummary ())
					.End ();
			}

			if (this.Entity.CanHaveMembers ())
			{
				var bricks = wall.AddBrick ()
					.Icon ("Data.AiderGroup.People")
					.Title (p => p.GetParticipantsTitle ())
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant));

				if (this.Entity.CanBeEditedByCurrentUser ())
				{
					bricks = bricks
						.EnableActionMenu<ActionAiderGroupViewController3ImportGroupMembers> ()
						.EnableActionMenu<ActionAiderGroupViewController4ExportGroupMembers> ();
				}
			}

			if (this.Entity.GroupDef.Function.IsNotNull ())
			{
				wall.AddBrick ()
					.Icon ("Data.AiderGroup.People")
					.Title (p => p.GetGroupAndSubGroupParticipantTitle ())
					.Text (p => p.GetGroupAndSubGroupParticipantSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderGroupViewController1Contact));
			}

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
					
		}
	}
}
