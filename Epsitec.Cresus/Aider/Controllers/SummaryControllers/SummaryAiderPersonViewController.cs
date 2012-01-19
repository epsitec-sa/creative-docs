using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderPersonViewController : SummaryViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ();

			//	Just to check that this is possible :
			wall.AddBrick ()
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.SpecialController0)
				.Title ("Informations confidentielles")
				.Text ("Pour en savoir plus...");

			if (this.Entity.IsGovernmentDefined ())
			{
				wall.AddBrick (x => x.eCH_Person.Address);
			}

			wall.AddBrick (x => x.AdditionalAddress1)
				.Title ("Adresse supplémentaire 1");
			wall.AddBrick (x => x.AdditionalAddress2)
				.Title ("Adresse supplémentaire 2");

			wall.AddBrick (x => x.Household);
			wall.AddBrick (x => x.Comment);
		}
	}

	[ControllerSubType (0)]
	public sealed class SummaryAiderPersonViewController0 : SummaryViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Title ("Contrôleur spécifique zéro")
				.Text ("Contenu spécifique<br/>Et bla bla bla...<br/>Juste pour prouver que c'est possible !");
		}
	}
}
