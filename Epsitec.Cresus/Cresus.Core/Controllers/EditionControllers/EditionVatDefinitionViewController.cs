//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionVatDefinitionViewController : EditionViewController<VatDefinitionEntity>
	{
		protected override void CreateBricks(BrickWall<VatDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.BeginDate).Width (150)
				  .Field (x => x.EndDate).Width (150)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.VatCode)
				  .Field (x => x.Rate)
				.End ()
				;
		}
	}
}
