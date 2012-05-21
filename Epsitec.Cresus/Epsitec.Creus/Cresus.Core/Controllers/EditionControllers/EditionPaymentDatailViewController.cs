//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPaymentDetailViewController : EditionViewController<PaymentDetailEntity>
	{
		protected override void CreateBricks(BrickWall<PaymentDetailEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.PaymentType)
				  .Field (x => x.PaymentCategory)
				  .Field (x => x.PaymentData)
				  .Field (x => x.Amount)
				  .Field (x => x.Currency)
				  .Field (x => x.Date)
				.End ()
				;
		}
	}
}
