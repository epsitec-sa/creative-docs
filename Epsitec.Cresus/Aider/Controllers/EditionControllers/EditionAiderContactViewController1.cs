//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	internal sealed class EditionAiderContactViewController1 : EditionViewController<AiderContactEntity>
	{
		protected override void CreateBricks(BrickWall<AiderContactEntity> wall)
		{
			var contact = this.Entity;

			if (contact.Household.IsNull ())
			{
				wall.AddBrick ()
					.Title (Resources.Text ("Adresse alternative"))
					.Icon ("Data.AiderAddress")
					.Input ()
						.Field (x => x.AddressType)
					.End ()
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
					.End ();
			}
			else
			{
				wall.AddBrick ()
					.Title (Resources.Text ("Adresse du ménage"))
					.Icon ("Data.AiderAddress")
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
					.End ();
			}
		}
	}
}