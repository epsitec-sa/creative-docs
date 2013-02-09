//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderPersonViewController : EditionViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			if (this.Entity.IsGovernmentDefined ())
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
			wall.AddBrick ()
				.Icon (this.Entity.GetIconName ("Data"))
				.Input ()
					.HorizontalGroup ()
						.Title ("Prénoms et nom")
						.Field (x => x.eCH_Person.PersonFirstNames)
						.Field (x => x.eCH_Person.PersonOfficialName)
					.End ()
					.Field (x => x.CallName)
					.Field (x => x.OriginalName)
					.HorizontalGroup ()
						.Title ("Dates de naissance et de décès")
						.Field (x => x.eCH_Person.PersonDateOfBirth)
						.Field (x => x.eCH_Person.PersonDateOfDeath)
					.End ()
					.Field (x => x.eCH_Person.PersonSex)
					.Field (x => x.MrMrs)
					.Field (x => x.Title)
					.Field (x => x.eCH_Person.AdultMaritalStatus)
					.Field (x => x.Confession)
					.Field (x => x.Profession)
					.Field (x => x.Language)
					.Field (x => x.eCH_Person.Nationality)
					.Field (x => x.eCH_Person.Origins)
				.End ();
		}

		private void CreateBricksForGovernmentAiderPerson(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Icon (this.Entity.GetIconName ("Data"))
				.Input ()
					.HorizontalGroup ()
						.Title ("Prénoms et nom selon le contrôle des habitants")
						.Field (x => x.eCH_Person.PersonFirstNames).ReadOnly ()
						.Field (x => x.eCH_Person.PersonOfficialName).ReadOnly ()
					.End ()
					.Field (x => x.CallName)
					.Field (x => x.OriginalName)
					.HorizontalGroup ()
						.Title ("Dates de naissance et de décès")
						.Field (x => x.eCH_Person.PersonDateOfBirth).ReadOnly ()
						.Field (x => x.eCH_Person.PersonDateOfDeath).ReadOnly ()
					.End ()
					.Field (x => x.eCH_Person.PersonSex).ReadOnly ()
					.Field (x => x.MrMrs)
					.Field (x => x.Title)
					.Field (x => x.eCH_Person.AdultMaritalStatus).ReadOnly ()
					.Field (x => x.Confession)
					.Field (x => x.Profession)
					.Field (x => x.Language)
					.Field (x => x.eCH_Person.Nationality).ReadOnly ()
					.Field (x => x.eCH_Person.Origins).ReadOnly ()
				.End ();
		}
	}
}
