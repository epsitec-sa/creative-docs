//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (0)]
	public sealed class EditionAiderPersonViewController0DeceaseDateController : EditionViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			if (this.Entity.IsGovernmentDefined)
			{
				this.CreateBricksForGovernmentAiderPerson (wall);
			}
			else
			{
				this.CreateBricksForUserAiderPerson (wall);
			}
		}

		private void CreateBricksForUserAiderPerson(BrickWall<AiderPersonEntity> wall)
		{
			var favorites = AiderCountryEntity.GetCountryFavorites (this.BusinessContext);

			wall.AddBrick ()
				.Icon (this.Entity.GetIconName ("Data"))
				.Input ()
					.HorizontalGroup ()
						.Title ("Dates de naissance et de décès")
						.Field (x => x.eCH_Person.PersonDateOfBirth)
						.Field (x => x.eCH_Person.PersonDateOfDeath)
					.End ()
				.End ();
		}

		private void CreateBricksForGovernmentAiderPerson(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Icon (this.Entity.GetIconName ("Data"))
				.Input ()				
					.HorizontalGroup ()
						.Title ("Dates de naissance et de décès")
						.Field (x => x.eCH_Person.PersonDateOfBirth).ReadOnly ()
						.Field (x => x.eCH_Person.PersonDateOfDeath)
					.End ()
				.End ();
		}
	}
}
