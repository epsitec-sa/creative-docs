//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Library;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	/// <summary>
	/// The <c>SummaryAiderPersonViewController</c> class implements the base summary view controller
	/// of a <see cref="AiderPersonEntity"/>. It displays a compact summary of the person, the list
	/// of the relations, the list of the groups to which the person belongs, and comments.
	/// </summary>
	public sealed class SummaryAiderPersonViewController : SummaryViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			bool showEmployeeTile   = this.Entity.Employee.IsNotNull ();
			bool showEmployeeAction = (showEmployeeTile == false) && user.CanEditEmployee ();
			bool canEditEmployee	= user.CanEditEmployee () || user.CanEditReferee ();
			bool canRemoveEmployee	= showEmployeeTile && user.CanEditEmployee ();

			wall.AddBrick ()
				.EnableActionMenu<ActionAiderPersonViewController4AddAlternateAddress> ()
				.EnableActionMenu<ActionAiderPersonViewController5AddHousehold> ()
				.EnableActionMenu<ActionAiderPersonViewController12Relocate> ()
				.EnableActionMenu<ActionAiderPersonViewController13DeleteContact> ()
				.EnableActionMenu<ActionAiderPersonViewController15DeleteEmployee> ().IfTrue (canRemoveEmployee)
				.EnableActionMenu<ActionAiderPersonViewController16RemoveJob> ().IfTrue (canRemoveEmployee)
				.EnableActionMenu<ActionAiderPersonViewController17AddReferee> ().IfTrue (canEditEmployee)
				.EnableActionButton<ActionAiderPersonViewController14DefineEmployee> ().IfTrue (showEmployeeAction)
				.EnableActionOnDrop<ActionAiderPersonViewController8FusionOnDrop> ()
				.Icon (this.Entity.GetIconName ("Data"))
				.Title (x => TextFormatter.FormatText (x.GetCompactSummary ()))
				.Text (x => x.GetPersonalDataSummary ());

			if (showEmployeeTile)
			{
				var type = canEditEmployee
						 ? typeof (SummaryAiderEmployeeViewController2WithoutContact)
						 : typeof (SummaryAiderEmployeeViewController3WithoutContactReadonly);

				wall.AddBrick (x => x.Employee)
					.Title ("Emploi")
					.Text (x => x.GetEmployeeSummary ())
					.Attribute (BrickMode.DefaultToSummarySubView)
					.WithSpecialController (type);
			}

			wall.AddBrick ()
				.IfTrue (this.HasUserPowerLevel (UserPowerLevel.Administrator))
				.Icon (this.Entity.GetIconName ("Data"))
				.Title (x => TextFormatter.FormatText ("Détails techniques"))
				.Text (x => TextFormatter.FormatText ("Visibilité: " + x.Visibility))
				.WithSpecialController (typeof (EditionAiderPersonViewController1Technical));

			wall.AddBrick ()
				.IfTrue (this.HasUserPowerLevel (UserPowerLevel.Administrator))
				.Icon (this.Entity.GetIconName ("Data"))
				.Title (x => TextFormatter.FormatText ("Détails techniques du RCH"))
				.Text (x => SummaryAiderPersonViewController.GetTechnicalSummary (x));

			this.CreateParishBrick (wall);

			wall.AddBrick ()
				.Icon ("Data.AiderGroup.People")
				.Title (p => p.GetGroupTitle ())
				.Text (p => p.GetGroupText ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderPersonViewController1Groups));

			if ((CoreContext.HasExperimentalFeature ("Warnings")) &&
				(this.Entity.Warnings.Any ()))
			{
				wall.AddBrick ()
					.Icon ("Data.AiderPersonWarning")
					.Title (p => p.GetWarningsTitle ())
					.Text (p => p.GetWarningsDescription ())
					.Attribute (BrickMode.DefaultToSummarySubView)
					.WithSpecialController (typeof (SummaryAiderPersonViewController2Warnings));
			}

			var households = this.Entity.Households;
			var contacts   = this.Entity.Contacts;

			this.CreateHouseholdBrick (wall, households);
			this.CreateContactsBrick (wall, contacts, user);

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}

		private void CreateHouseholdBrick(BrickWall<AiderPersonEntity> wall, System.Collections.Generic.IList<AiderHouseholdEntity> households)
		{
			if (households.Any ())
			{
				wall.AddBrick (x => x.Households)
					.Title (households.Count > 1 ? Resources.Text ("Ménages") : Resources.Text ("Ménage"))
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.EnableActionMenu<ActionAiderPersonViewController7RemoveHousehold> ().IfTrue (households.Count > 1)
					.Template ()
					.End ()
					.Attribute (BrickMode.DefaultToSummarySubView);
			}
		}
		
		private void CreateContactsBrick(BrickWall<AiderPersonEntity> wall, System.Collections.Generic.IList<AiderContactEntity> contacts, AiderUserEntity user)
		{
			if (contacts.Any (x => x.ContactType == Enumerations.ContactType.PersonAddress))
			{
				wall.AddBrick (x => x.AdditionalAddresses)
					.Title ("Adresses alternatives")
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.EnableActionMenu<ActionAiderPersonViewController6RemoveAlternateAddress> ()
					.Template ()
						.Text (x => TextFormatter.FormatText (TextFormatter.FormatText (x.AddressType).ApplyBold (), "\n", x.Address.GetSummary ()))
					.End ()
					.WithSpecialController (typeof (EditionAiderContactViewController1Address));

				if ((user.CanViewConfidentialAddress ()) &&
					(contacts.Any (x => x.AddressType == Enumerations.AddressType.Confidential)))
				{
					wall.AddBrick (x => x.ConfidentialAddresses)
						.Title ("Adresses confidentielles")
						.Attribute (BrickMode.HideAddButton)
						.Attribute (BrickMode.HideRemoveButton)
						.Attribute (BrickMode.AutoGroup)
						.EnableActionMenu<ActionAiderPersonViewController6RemoveAlternateAddress> ()
						.Template ()
							.Text (x => TextFormatter.FormatText (TextFormatter.FormatText (x.AddressType).ApplyBold (), "\n", x.Address.GetSummary ()))
						.End ()
						.WithSpecialController (typeof (EditionAiderContactViewController1Address));
				}
			}
		}

		private void CreateParishBrick(BrickWall<AiderPersonEntity> wall)
		{
			AiderGroupEntity geoParish = this.Entity.GetDerogationGeoParishGroup (this.BusinessContext);
			
			if (geoParish.IsNull ())
			{
				wall.AddBrick ()
					.EnableActionMenu<ActionAiderPersonViewController11Derogate> ()
					.Icon ("Data.AiderGroup.Parish")
					.Title ("Paroisse")
					.Text (p => p.ParishGroup.Name);
			}
			else
			{
				var buffer = new System.Text.StringBuilder ();

				if (geoParish.IsNoParish ())
				{
					//	No need to say where the person lives...
				}
				else
				{
					buffer.Append ("Habite la ");
					buffer.Append (geoParish.Name);
					buffer.Append ("\n");
				}

				buffer.Append ("Dérogation vers la ");
				buffer.Append (this.Entity.ParishGroup.Name);

				wall.AddBrick ()
					.EnableActionMenu<ActionAiderPersonViewController11Derogate> ()
					.Icon ("Data.AiderGroup.Parish")
					.Title ("Paroisse")
					.Text (TextFormatter.FormatText (buffer));
			}
		}
		
		private static FormattedText GetTechnicalSummary(AiderPersonEntity person)
		{
			return TextFormatter.FormatText (person.eCH_Person.GetSummary (), "~\n \n~",
				person.eCH_Person.ReportedPerson1.Address.GetSummary (), "~\n \n~",
				person.eCH_Person.ReportedPerson2.Address.GetSummary ());
		}
	}
}
