//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	internal sealed class EditionAiderContactViewController1 : EditionViewController<AiderContactEntity>
	{
		protected override void CreateBricks(BrickWall<AiderContactEntity> wall)
		{
			var bricks = wall.AddBrick ();

			bricks = this.GetHeader (bricks);

			bricks = bricks
				.Input ()
				.Field (x => x.Address.Town)
				.Field (x => x.Address.AddressLine1)
				.Field (x => x.Address.StreetUserFriendly)
				.Field (x => x.Address.HouseNumberAndComplement)
				.Field (x => x.Address.PostBox)
			.End ()
			.Input ()
				.Field (x => x.Address.Phone1)
				.Field (x => x.Address.Phone2)
				.Field (x => x.Address.Mobile)
				.Field (x => x.Address.Fax)
				.Field (x => x.Address.Email)
				.Field (x => x.Address.Web)
			.End ()
			.Input ()
				.Field (x => x.Address.Comment.Text)
			.End ();
		}

		private SimpleBrick<AiderContactEntity> GetHeader(SimpleBrick<AiderContactEntity> bricks)
		{
			switch (this.Entity.ContactType)
			{
				case ContactType.Legal:
					return bricks
						.Title (Resources.Text ("Adresse de contact")) // TODO
						.Icon ("Data.AiderAddress");

				case ContactType.PersonAddress:
					return bricks
						.Title (Resources.Text ("Adresse alternative"))
						.Icon ("Data.AiderAddress")
						.Input ()
							.Field (x => x.AddressType)
						.End ();

				case ContactType.PersonHousehold:
					return bricks
						.Title (Resources.Text ("Adresse du ménage"))
						.Icon ("Data.AiderAddress");

				default:
					return bricks;
			}
		}
	}
}
