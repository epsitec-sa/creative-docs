//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
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


		#region Static GroupIndex operations
		public static bool GroupCompare(int groupIndex1, int groupIndex2, int levelCount)
		{
			//	Compare deux GroupIndex jusqu'à une profondeur donnée.
			//	groupIndex1 = 665544, groupIndex2 = 775544, levelCount = 1 -> true
			//	groupIndex1 = 665544, groupIndex2 = 775544, levelCount = 2 -> true
			//	groupIndex1 = 665544, groupIndex2 = 775544, levelCount = 3 -> false
			//	groupIndex1 = 665544, groupIndex2 = 775544, levelCount = 4 -> false
			for (int i = 0; i < levelCount; i++)
			{
				int n1 = AbstractDocumentItemEntity.GroupExtract (groupIndex1, i);
				int n2 = AbstractDocumentItemEntity.GroupExtract (groupIndex2, i);

				if (n1 != n2)
				{
					return false;
				}
			}

			return true;
		}

		public static int GroupReplace(int groupIndex, int level, int rank)
		{
			//	Remplace une paire de digits d'un niveau quelconque.
			//	groupIndex = 665544, level = 0, rank = 88 ->   665588
			//	groupIndex = 665544, level = 1, rank = 88 ->   668844
			//	groupIndex = 665544, level = 2, rank = 88 ->   885544
			//	groupIndex = 665544, level = 3, rank = 88 -> 88665544
			System.Diagnostics.Debug.Assert (groupIndex >= 0);
			System.Diagnostics.Debug.Assert (groupIndex <= 99999999);

			System.Diagnostics.Debug.Assert (level >= 0);
			System.Diagnostics.Debug.Assert (level < AbstractDocumentItemEntity.maxGroupingDepth);

			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank <= 99);

			int result = 0;
			int f = 1;

			for (int i = 0; i < AbstractDocumentItemEntity.maxGroupingDepth; i++)
			{
				if (i == level)
				{
					result += f * rank;
				}
				else
				{
					result += f * AbstractDocumentItemEntity.GroupExtract (groupIndex, i);
				}

				f *= 100;
			}

			System.Diagnostics.Debug.Assert (result >= 0);
			System.Diagnostics.Debug.Assert (result <= 99999999);

			return result;
		}

		public static int GroupExtract(int groupIndex, int level)
		{
			//	Extrait une paire de digits.
			//	Retourne 0 si le niveau n'existe pas.
			//	groupIndex = 665544, level = 0 -> 44
			//	groupIndex = 665544, level = 1 -> 55
			//	groupIndex = 665544, level = 2 -> 66
			//	groupIndex = 665544, level = 3 ->  0
			//	groupIndex = 665544, level = 4 ->  0
			System.Diagnostics.Debug.Assert (groupIndex >= 0);
			System.Diagnostics.Debug.Assert (groupIndex <= 99999999);

			System.Diagnostics.Debug.Assert (level >= 0);

			if (level >= AbstractDocumentItemEntity.maxGroupingDepth)
			{
				return 0;
			}
			else
			{
				int f = (int) System.Math.Pow (100, level);  // f = 1, 100, 10000 ou 1000000
				return (groupIndex/f) % 100;
			}
		}

		/// <summary>
		/// Gets the group level based on an index. For instance <c>01</c> is of level <c>1</c>,
		/// <c>0101</c> of level <c>2</c>, etc.
		/// </summary>
		/// <param name="groupIndex">The group index.</param>
		/// <returns>The group level.</returns>
		public static int GetGroupLevel(int groupIndex)
		{
			//	Retourne le niveau, compris entre 0 et 4.
			//	       0 -> 0
			//	       4 -> 1
			//	      44 -> 1
			//	     544 -> 2
			//	    5544 -> 2
			//	   65544 -> 3
			//	  665544 -> 3
			//	 8665544 -> 4
			//	88665544 -> 4
			if (groupIndex < 0 || groupIndex > 99999999)
			{
				throw new System.ArgumentOutOfRangeException ("index", "The index must lie between 0 and 99999999)");
			}
			else if (groupIndex == 0)
			{
				return 0;
			}
			else
			{
				//	Log10 (0001..0099) --> 0..1.995635 --> trunc/2 = 0
				//	Log10 (0100..9999) --> 2..3.999957 --> trunc/2 = 1
				//	etc.

				return 1 + (int) System.Math.Truncate (System.Math.Log10 (groupIndex) / 2);
			}
		}
		#endregion


		public static readonly int maxGroupingDepth = 4;
	}
}