//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

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
				.EnableAction<ActionAiderPersonViewController4AddAlternateAddress> ()
				.EnableAction<ActionAiderPersonViewController5AddHousehold> ()
				.Icon (this.Entity.GetIconName ("Data"))
				.Title (x => TextFormatter.FormatText (x.GetCompactSummary ()))
				.Text (x => x.GetPersonalDataSummary ());

			wall.AddBrick (x => x.ParishGroup)
				.Icon ("Data.AiderGroup.Parish")
				.Title ("Paroisse")
				.Text (x => x.Name)
				.Attribute (BrickMode.DefaultToSummarySubView);

			wall.AddBrick ()
				.Icon ("Data.AiderGroup.People")
				.Title (p => p.GetGroupTitle ())
				.Text (p => p.GetGroupText ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderPersonViewController1Groups));

			wall.AddBrick ()
				.Icon("Data.AiderPersonWarning")
				.Title(p => p.GetWarningsTitle ())
				.Text(p => p.GetWarningsDescription ())
				.Attribute(BrickMode.DefaultToSummarySubView)
				.WithSpecialController(typeof (SummaryAiderPersonViewController2Warnings));

			var households = this.Entity.Households;
			var contacts   = this.Entity.Contacts;

			if (households.Any ())
			{
				wall.AddBrick (x => x.Households)
					.Title (households.Count > 1 ? Resources.Text ("Ménages") : Resources.Text ("Ménage"))
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.EnableAction<ActionAiderPersonViewController7RemoveHousehold> ()
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
					.EnableAction<ActionAiderPersonViewController6RemoveAlternateAddress> ()
					.Template ()
						.Text (x => TextFormatter.FormatText (TextFormatter.FormatText (x.AddressType).ApplyBold (), "\n", x.Address.GetSummary ()))
					.End ()
					.WithSpecialController (typeof (EditionAiderContactViewController1Address));
			}

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
