//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule (RuleType.Update)]
	internal class InvoiceDocumentUpdateRule : GenericBusinessRule<InvoiceDocumentEntity>
	{
		protected override void Apply(InvoiceDocumentEntity entity)
		{
			var dataContext = Logic.Current.DataContext;

			Epsitec.Cresus.Core.Helpers.InvoiceDocumentHelper.UpdatePrices (entity, dataContext);
		}
	}
}
