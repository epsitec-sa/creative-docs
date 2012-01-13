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
				.Input ()
					.Field (x => x.eCH_Person.PersonFirstNames)
					.Field (x => x.eCH_Person.PersonOfficialName)
					.Field (x => x.OriginalName)
					.Field (x => x.eCH_Person.DateOfBirth)
					.Field (x => x.eCH_Person.PersonSex)
					.Field (x => x.Title)
					.Field (x => x.MrMrs)
					.Field (x => x.eCH_Person.AdultMaritalStatus)
					.Field (x => x.Confession)
					.Field (x => x.Profession)
					.Field (x => x.Language)
					.Field (x => x.eCH_Person.NationalityCountryCode)
					.Field (x => x.eCH_Person.Origins)
					.Field (x => x.Comment)
				.End ();
		}


		private void CreateBricksForGovernmentAiderPerson(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.eCH_Person.PersonFirstNames)
						.ReadOnly ()
					.Field (x => x.eCH_Person.PersonOfficialName)
						.ReadOnly ()
					.Field (x => x.OriginalName)
					.Field (x => x.eCH_Person.DateOfBirth)
						.ReadOnly ()
					.Field (x => x.eCH_Person.PersonSex)
						.ReadOnly ()
					.Field (x => x.Title)
					.Field (x => x.MrMrs)
					.Field (x => x.eCH_Person.AdultMaritalStatus)
						.ReadOnly ()
					.Field (x => x.Confession)
					.Field (x => x.Profession)
					.Field (x => x.Language)
					.Field (x => x.eCH_Person.NationalityCountryCode)
						.ReadOnly ()
					.Field (x => x.eCH_Person.Origins)
						.ReadOnly ()
					.Field (x => x.Comment)
				.End ();
		}


	}


}
