//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleAccountingDefinitionEntity
	{
		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.SellingBookAccount.GetEntityStatus ());
				a.Accumulate (this.SellingDiscountBookAccount.GetEntityStatus ());

				a.Accumulate (this.PurchaseBookAccount.GetEntityStatus ());
				a.Accumulate (this.PurchaseDiscountBookAccount.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
