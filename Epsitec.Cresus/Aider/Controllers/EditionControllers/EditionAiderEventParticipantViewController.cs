//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderEventParticipantViewController : EditionViewController<AiderEventParticipantEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventParticipantEntity> wall)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;
			var favorites = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);
            this.Entity.UpdateActDataFromModel ();
			if (this.Entity.IsExternal == false)
			{
				wall.AddBrick ()
					.Input ()
						.Field (x => x.Role)
						.Field (x => x.Person)
					.End ()
					.Input ()
						.Field (x => x.Person.MainContact.Address.Town)
							.WithFavorites (favorites)
  					.End ()
                    .Input ()
                        .Field (x => x.FirstName)
                        .Field (x => x.LastName)
                        .Field (x => x.Confession)
  					    .Field (x => x.Sex)
                    .End ();
            }
			else
			{
				wall.AddBrick ()
					.Input ()
						.Field (x => x.Role)
						.Field (x => x.FirstName)
						.Field (x => x.LastName)
						.Field (x => x.BirthDate)
						.Field (x => x.Town)
						.Field (x => x.ParishName)
						.Field (x => x.Confession)
						.Field (x => x.Sex)
					.End ();
			}
		}
	}
}

