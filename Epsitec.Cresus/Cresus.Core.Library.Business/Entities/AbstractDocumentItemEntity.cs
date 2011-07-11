//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Controllers.TabIds;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AbstractDocumentItemEntity
	{
		public AbstractDocumentItemEntity()
		{
		}

		public virtual DocumentItemTabId		TabId
		{
			get
			{
				return DocumentItemTabId.None;
			}
		}

		/// <summary>
		/// Gets the group level based on the <see cref="GroupIndex"/>. The level will be zero
		/// if the index is zero, index <c>nn</c> will map to level <c>1</c>, <c>nnnn</c> to
		/// level <c>2</c>, etc.
		/// </summary>
		public int								GroupLevel
		{
			get
			{
				return AbstractDocumentItemEntity.GetGroupLevel (this.GroupIndex);
			}
		}


		public virtual void Process(IDocumentPriceCalculator priceCalculator)
		{
		}
		
		public static int GetGroupLevel(int index)
		{
			if (index == 0)
			{
				return 0;
			}
			else
			{
				//	Log10 (0001..0099) --> 0..1.995635 --> trunc/2 = 0
				//	Log10 (0100..9999) --> 2..3.999957 --> trunc/2 = 1
				//	etc.

				return 1 + (int) System.Math.Truncate (System.Math.Log10 (index) / 2);
			}
		}
	}
}