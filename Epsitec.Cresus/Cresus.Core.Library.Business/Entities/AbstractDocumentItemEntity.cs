//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Controllers.TabIds;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AbstractDocumentItemEntity : System.IComparable<AbstractDocumentItemEntity>
	{
		public AbstractDocumentItemEntity()
		{
			this.sortRankCache = -1;
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

		internal int LineRankCache
		{
			get
			{
				return this.lineRankCache;
			}
			set
			{
				this.lineRankCache = value;
				this.sortRankCache = -1;
			}
		}

		internal long SortRankCache
		{
			get
			{
				if (this.sortRankCache < 0)
				{
					this.UpdateSortRankCache ();
				}

				return this.sortRankCache;
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

		partial void OnGroupIndexChanging(int oldValue, int newValue)
		{
			this.sortRankCache = -1;
		}

		private void UpdateSortRankCache()
		{
			System.Diagnostics.Debug.Assert (this.GroupLevel < 5);

			int index = this.GroupIndex;

			if (index == 0)
			{
				index = 99;
			}

			//	Convert 00       into 99000000
			//	Convert 01       into 01000000
			//	Convert 01 03    into 03010000
			//	Convert 01 01 02 into 02010100

			int sortableIndex = (index/1       % 100) * 1000000
							  + (index/100     % 100) * 10000
							  + (index/10000   % 100) * 100
							  + (index/1000000 % 100) * 1;

			this.sortRankCache = this.lineRankCache + (10000L * sortableIndex);
		}

		#region IComparable<AbstractDocumentItemEntity> Members

		public int CompareTo(AbstractDocumentItemEntity other)
		{
			long diff = this.SortRankCache - other.SortRankCache;

			if (diff < 0)
			{
				return -1;			
			}
			else if (diff > 0)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		#endregion

		private int lineRankCache;
		private long sortRankCache;
	}
}