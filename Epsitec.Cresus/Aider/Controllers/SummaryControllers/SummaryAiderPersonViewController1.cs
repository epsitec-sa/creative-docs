//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (1)]
	public sealed class SummaryAiderPersonViewController1 : SummaryViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.EnableAction (0)
				.EnableAction (1)
				.Icon (this.Entity.GetIconName ("Data"))
				.Text (x => x.GetPersonalDataSummary ());

			if (this.Entity.IsGovernmentDefined ())
			{
				// TODO Make those two tiles without edition tiles when they are clicked.

				wall.AddBrick (x => x.eCH_Person.Address1);

				if (this.Entity.eCH_Person.Address2.IsNotNull ())
				{
					wall.AddBrick (x => x.eCH_Person.Address2);
				}
			}

			if (this.Entity.Household1.IsNotNull ())
			{
				wall.AddBrick (x => x.Household1)
					.Title ("Ménage principal")
					.Text (x => x.Address.GetSummary ())
					.Attribute (BrickMode.DefaultToSummarySubView);
			}

			if (this.Entity.Household2.IsNotNull ())
			{
				wall.AddBrick (x => x.Household2)
					.Title ("Ménage secondaire")
					.Text (x => x.Address.GetSummary ())
					.Attribute (BrickMode.DefaultToSummarySubView);
			}

			wall.AddBrick (x => x.Parish)
				.Icon ("Data.AiderGroup.Parish")
				.Title (x => x.GetCompactSummary ())
				.Text (this.Entity.GetParishDescription ());

			wall.AddBrick (x => x.AdditionalAddresses)
				.AttributeIf (BrickMode.HideAddButton, this.Entity.AdditionalAddresses.Count > 3)
				.Title ("Coordonnées supplémentaires")
				.Template ()
					.Title (x => this.GetAdditionalAddressTitle (x))
				.End ();
		}

		private FormattedText GetAdditionalAddressTitle(AiderAddressEntity address)
		{
			var meaningfullType = address.Type != AddressType.Default
							   && address.Type != AddressType.None;

			return meaningfullType
				? TextFormatter.FormatText ("Coordonnées (", address.Type, ")")
				: TextFormatter.FormatText ("Coordonnées supplémentaires");
		}
	}
}

