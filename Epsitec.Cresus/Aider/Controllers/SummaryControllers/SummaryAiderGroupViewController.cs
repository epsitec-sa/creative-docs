//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SetControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderGroupViewController : SummaryViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupEntity> wall)
		{
			var group = this.Entity;

			bool canSubgroupsBeEdited					= group.CanSubgroupsBeEdited ();
			bool canGroupBeEditedByCurrentUser			= group.CanBeEditedByCurrentUser ();
			bool canGroupBeSeenByCurrentUser			= group.CanBeSeenByCurrentUser ();
			bool canSubgroupsBeEditedByCurrentUser		= canSubgroupsBeEdited && canGroupBeEditedByCurrentUser;

			if (canGroupBeSeenByCurrentUser)
			{
				wall.AddBrick ()
					.EnableActionMenu<ActionAiderGroupViewController6AddToBag> ()
					.EnableActionMenu<ActionAiderGroupViewController2MoveGroup> ().IfTrue (canGroupBeEditedByCurrentUser)
					.EnableActionMenu<ActionAiderGroupViewController5MergeGroup> ().IfTrue (canGroupBeEditedByCurrentUser && group.Subgroups.Count == 0)
					.EnableActionMenu<ActionAiderGroupViewController10CreateOfficeManagement> ().IfTrue (this.HasUserPowerLevel (UserPowerLevel.Administrator));

				wall.AddBrick (x => x.Subgroups)
					.IfTrue (group.CanHaveSubgroups ())
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.EnableActionMenu<ActionAiderGroupViewController0CreateSubGroup> ().IfTrue (canSubgroupsBeEditedByCurrentUser)
					.EnableActionMenu<ActionAiderGroupViewController1DeleteSubGroup> ().IfTrue (canSubgroupsBeEditedByCurrentUser)
					.Template ()
						.Text (x => x.GetCompactSummary ())
					.End ();

				wall.AddBrick ()
					.IfTrue (group.CanHaveMembers ())
					.Icon ("Data.AiderGroup.People")
					.Title ("Participants")
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant))
					.EnableActionMenu<ActionAiderGroupViewController3ImportGroupMembers> ().IfTrue (canGroupBeEditedByCurrentUser)
					.EnableActionMenu<ActionAiderGroupViewController4ExportGroupMembers> ().IfTrue (canGroupBeEditedByCurrentUser)
					.EnableActionMenu<ActionAiderGroupViewController8PurgeGroupMembers> ().IfTrue (canGroupBeEditedByCurrentUser)
					.EnableActionButton<ActionAiderGroupViewController7AddMembersFromBag> ().IfTrue (canGroupBeEditedByCurrentUser);

				wall.AddBrick (x => x.GroupDef)
					.IfTrue (this.HasUserPowerLevel (Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator));

				if (group.GroupDef.Function.IsNotNull ())
				{
					wall.AddBrick ()
						.IfTrue (group.GroupDef.Function.IsNotNull ())
						.Icon ("Data.AiderGroup.People")
						.Title ("Participants")
						.Text (p => p.GetGroupAndSubGroupParticipantSummary ())
						.Attribute (BrickMode.DefaultToSetSubView)
						.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant));
				}

				wall.AddBrick (x => x.Comment)
					.Attribute (BrickMode.AutoCreateNullEntity);
			}
			else
			{
				wall.AddBrick ()
						.Title ("Groupe hors d'accès")
						.Text ("Vos droits ne vous permettent pas de consulter ce groupe");
			}
		}
	}
}
