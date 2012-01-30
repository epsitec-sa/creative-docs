using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using System.Linq;


namespace Epsitec.Aider.Controllers.SummaryControllers
{


	public sealed class SummaryAiderPersonViewController : SummaryViewController<AiderPersonEntity>
	{


		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Text (x => x.GetCoordinatesSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.SpecialController1);

			wall.AddBrick ()
				.Title (this.GetRelatedPersonsTitle (this.Entity))
				.Text (x => x.GetRelatedPersonsSummary (5))
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.SpecialController2);

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}


		private string GetRelatedPersonsTitle(AiderPersonEntity person)
		{
			return "Personnes liées (" + person.GetRelatedPersons ().Count () + ")";
		}


	}


}
