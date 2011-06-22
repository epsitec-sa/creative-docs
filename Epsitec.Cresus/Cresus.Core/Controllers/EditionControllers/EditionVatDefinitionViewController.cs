//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionVatDefinitionViewController : EditionViewController<VatDefinitionEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<VatDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Title ("Date de début")
				  .Field (x => x.BeginDate)
				  .Title ("Date de fin")
				  .Field (x => x.EndDate)
				.End ()
				.Separator ()
				.Input ()
				  .Title ("Nom")
				  .Field (x => x.Name)
				  .Title ("Description")
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Title ("Code TVA")
				  .Field (x => x.VatCode)
				  .Title ("Taux appliqué")
				  .Field (x => x.Rate)
				.End ()
				;
		}
	}
}
