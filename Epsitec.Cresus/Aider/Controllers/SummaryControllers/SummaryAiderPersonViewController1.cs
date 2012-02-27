//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
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

			wall.AddBrick (x => x.Households)
				.Attribute (BrickMode.DefaultToCreationOrEditionSubView)
				.Template ()
					.Title ("Coordonnées du ménage")
					.Text (x => x.GetAddressSummary ())
				.End ();

			wall.AddBrick (x => x.Parish)
				.Icon ("Data.AiderGroup.Parish")
				.Title (x => x.GetCompactSummary ())
				.Text (this.Entity.GetParishDescription ());
	//			.Attribute (BrickMode.DefaultToSummarySubView);
//				.Attribute (BrickMode.SpecialController3);

#if false
			wall.AddBrick (x => x.AdditionalAddress1)
				.Attribute (BrickMode.AutoCreateNullEntity)
				.Title (this.GetAdditionalAddressTitle (this.Entity.AdditionalAddress1));

			wall.AddBrick (x => x.AdditionalAddress2)
				.Attribute (BrickMode.AutoCreateNullEntity)
				.Title (this.GetAdditionalAddressTitle (this.Entity.AdditionalAddress2));

			wall.AddBrick (x => x.AdditionalAddress3)
				.Attribute (BrickMode.AutoCreateNullEntity)
				.Title (this.GetAdditionalAddressTitle (this.Entity.AdditionalAddress3));

			wall.AddBrick (x => x.AdditionalAddress4)
				.Attribute (BrickMode.AutoCreateNullEntity)
				.Title (this.GetAdditionalAddressTitle (this.Entity.AdditionalAddress4));
#endif

			wall.AddBrick (x => x.AdditionalAddresses)
//				.Attribute (BrickMode.AutoGroup)
				.Template ()
					.Title (x => TextFormatter.FormatText (x.Type))
				.End ();
		}


		private FormattedText GetAdditionalAddressTitle(AiderAddressEntity address)
		{
			return TextFormatter.FormatText ("Coordonées supplémentaires", "(~", address.Type, "~)");
		}
	}
}

