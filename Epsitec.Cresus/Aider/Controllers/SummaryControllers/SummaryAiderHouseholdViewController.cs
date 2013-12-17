//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;

using Epsitec.Common.Support;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Business.UserManagement;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Types;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderHouseholdViewController : SummaryViewController<AiderHouseholdEntity>
	{
		protected override void CreateBricks(BrickWall<AiderHouseholdEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			var household = this.Entity;
			
			if (user.LoginName == "epsitec")
			{
				//	Process the degenerated cases when we click on them: no name for the
				//	household (usually this means that there is no member). This should
				//	never happen, but as of dec. 17 2013, the database contained 219
				//	of these broken households.

				if (string.IsNullOrEmpty (household.DisplayName))
				{
					var example1 = new AiderHouseholdEntity
					{
						DisplayName = ""
					};

					var example2 = new AiderHouseholdEntity
					{
						DisplayName = ","
					};

					var emptyItems1 = this.BusinessContext.GetByExample (example1);
					var emptyItems2 = this.BusinessContext.GetByExample (example2);

					var emptyItems = emptyItems1.Concat (emptyItems2).ToList ();

					foreach (var item in emptyItems)
					{
						AiderHouseholdEntity.Delete (this.BusinessContext, item);
					}

					this.BusinessContext.SaveChanges (Cresus.Core.Business.LockingPolicy.KeepLock);

					return;
				}
			}

			var address = household.Address;
			var html = address.GetHtmlForLocationWebServices ();

			wall.AddBrick()
				.EnableActionMenu<ActionAiderHouseholdViewController0NewHouseholdMember> ()
				.EnableActionMenu<ActionAiderHouseholdViewController1AddHouseholdMember> ()
				.EnableActionMenu<ActionAiderHouseholdViewController6AddToBag> ()
				.EnableActionOnDrop<ActionAiderHouseholdViewController4AddHouseholdMemberOnDrop> ();

			wall.AddBrick ()
				.Title (new FormattedText (Resources.Text ("Adresse") + html))
				.Text (address.GetSummary ())
				.Icon ("Data.AiderAddress")
				.WithSpecialController (typeof (EditionAiderHouseholdViewController1Address));

			wall.AddBrick (h => h.Members)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				.EnableActionMenu<ActionAiderHouseholdViewController2RemoveMemberFromHousehold> ()
				.EnableActionMenu<ActionAiderHouseholdViewController3ChangeHeadOfHousehold> ()
				.EnableActionMenu<ActionAiderHouseholdViewController5CleanHousehold> ()
				.EnableActionMenu<ActionAiderHouseholdViewController7AddHouseholdMembersToBag> ()
				.Template ()
					.Icon ("Data.AiderPersons")
					.Title ("Membres du ménage")
					.Text (p => p.GetCompactSummary (household))
				.End ();

			var person    = household.Members.FirstOrDefault ();

			if (this.HasUserPowerLevel (UserPowerLevel.Administrator))
			{
				var head = household.Contacts.FirstOrDefault (c => (c.HouseholdRole == HouseholdRole.Head) && (c.Person.IsGovernmentDefined));

				if (head != null)
				{
					var list = head.Person.eCH_Person.ReportedPersons.SelectMany (x => x.Members).Distinct ();

					foreach (var item in list)
					{
						wall.AddBrick ()
							.Attribute (BrickMode.DefaultToSummarySubView)
							.Icon ("Data.AiderPersons")
							.Title ("Données ECh")
							.Text (item.GetSummary ());
					}
				}		
			}

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
