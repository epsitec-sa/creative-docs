//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionAccountingOperationViewController : EditionViewController<AccountingOperationEntity>
	{
		protected override void CreateBricks(BrickWall<AccountingOperationEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.StandardVatCode)
				  .Field (x => x.ReducedVatCode)
				  .Field (x => x.SpecialVatCode)
				.End ()
				;
		}
	}
}
