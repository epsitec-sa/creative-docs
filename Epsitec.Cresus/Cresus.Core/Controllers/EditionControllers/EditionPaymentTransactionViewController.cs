//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPaymentTransactionViewController : EditionViewController<Entities.PaymentTransactionEntity>
	{
		protected override void CreateBricks(BrickWall<PaymentTransactionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Text)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.PaymentDetail.PaymentType)
				  .Field (x => x.PaymentDetail.PaymentCategory)
				  .Field (x => x.PaymentDetail.Amount)
				  .Field (x => x.PaymentDetail.Currency)
				  .Field (x => x.PaymentDetail.Date)
				  .Field (x => x.PaymentDetail.PaymentData)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.InstalmentRank)
				  .Field (x => x.InstalmentName)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.TransactionId)
				  .Field (x => x.IsrReferenceNumber)
				  .Field (x => x.IsrDefinition)
				.End ()
				;
		}
	}
}
