//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

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
			wall.AddBrick ()
				.EnableAction (4)
				.EnableAction (5)
//				.EnableAction (6)
				.Icon (this.Entity.GetIconName ("Data"))
				.Title (x => TextFormatter.FormatText (x.GetCompactSummary ()))
				.Text (x => x.GetPersonalDataSummary ())
//				.Title (x => TextFormatter.FormatText (x.CallName, x.eCH_Person.PersonOfficialName, "(~", x.OriginalName, "~)"))
//				.Text (x => TextFormatter.FormatText (TextFormatter.FormatText (x.Parish.Group.Name).ApplyBold(), "", "\n", TextFormatter.Command.IfElseEmpty, x.Household1.Address.GetPostalAddress ()))
//				.Attribute (BrickMode.DefaultToSummarySubView)
//				.Attribute (BrickMode.SpecialController1)
			;

			wall.AddBrick (x => x.Parish.Group)
				.Icon ("Data.AiderGroup.Parish")
				.Title ("Paroisse")
				.Attribute (BrickMode.DefaultToSummarySubView);

			wall.AddBrick ()
				.Icon ("Data.AiderGroup.People")
				.Title (p => p.GetGroupTitle ())
				.Text (p => p.GetGroupText ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.SpecialController1);

			//if (this.Entity.Warnings.Any () || true)
			//{
			//    wall.AddBrick (x => x.Warnings)
			//        .Title ("Attention")
			//        .Icon ("manifest:Epsitec.Aider.Images.Data.AiderWarning.icon")
			//        .Template ()
			//        .End ();
			//}

			//wall.AddBrick (x => x.Relationships)
			//    .Attribute (BrickMode.DefaultToSummarySubView)
			//    .Attribute (BrickMode.SpecialController2);

			var households = this.Entity.Households;
			var contacts   = this.Entity.Contacts;

			if (households.Any ())
			{
				wall.AddBrick (x => x.Households)
					.Title (households.Count > 1 ? Resources.Text ("Ménages") : Resources.Text ("Ménage"))
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.EnableAction (7)
					.Template ()
					.End ()
					.Attribute (BrickMode.DefaultToSummarySubView);
			}

			if (contacts.Any (x => x.ContactType == Enumerations.ContactType.PersonAddress))
			{
				wall.AddBrick (x => x.AdditionalAddresses)
					.Title ("Adresses alternatives")
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.EnableAction (6)
					.Template ()
						.Text (x => TextFormatter.FormatText (TextFormatter.FormatText (x.AddressType).ApplyBold (), "\n", x.Address.GetSummary ()))
					.End ()
					.Attribute (BrickMode.SpecialController1)
					;
			}

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
