//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPaymentDetailViewController : EditionViewController<PaymentDetailEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<PaymentDetailEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.PaymentType)
				  .Field (x => x.PaymentMode)
				  .Field (x => x.PaymentData)
				  .Field (x => x.Amount).Width (100)
				  .Field (x => x.Currency)
				  .Field (x => x.Date).Width (150)
				.End ()
				;
		}
	}
}
