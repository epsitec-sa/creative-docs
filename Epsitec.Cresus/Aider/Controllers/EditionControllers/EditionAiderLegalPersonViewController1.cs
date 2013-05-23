using Epsitec.Aider.Entities;
using Epsitec.Common.Support;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	public sealed class EditionAiderLegalPersonViewController1 : EditionViewController<AiderLegalPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderLegalPersonEntity> wall)
		{
            var favorites = AiderTownEntity.GetTownFavorites(this.BusinessContext);

			wall.AddBrick ()
				.Title (Resources.Text ("Adresse de la personne morale"))
				.Icon ("Data.AiderAddress")
				.Input ()
					.Field (x => x.Address.Town)
                        .WithFavorites(favorites)
					.Field (x => x.Address.AddressLine1)
					.Field (x => x.Address.StreetHouseNumberAndComplement)
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
	}
}

