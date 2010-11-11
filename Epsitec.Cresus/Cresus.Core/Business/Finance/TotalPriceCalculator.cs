//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class TotalPriceCalculator
	{
		public TotalPriceCalculator(BusinessDocumentEntity document, TotalDocumentItemEntity totalItem)
		{
			this.document  = document;
			this.totalItem = totalItem;
		}

		
		public int GroupIndex
		{
			get
			{
				return this.totalItem.GroupIndex;
			}
		}

		private readonly BusinessDocumentEntity		document;
		private readonly TotalDocumentItemEntity	totalItem;
	}
}
