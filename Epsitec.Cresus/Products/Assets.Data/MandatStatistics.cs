//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct MandatStatistics
	{
		public MandatStatistics(int assetCount, int eventCount, int categoryCount, int groupCount, int personCount, int reportCount, int accountCount)
		{
			this.AssetCount	   = assetCount;
			this.EventCount	   = eventCount;
			this.CategoryCount = categoryCount;
			this.GroupCount	   = groupCount;
			this.PersonCount   = personCount;
			this.ReportCount   = reportCount;
			this.AccountCount  = accountCount;
		}

		public bool IsEmpty
		{
			get
			{
				return this.AssetCount == -1;
			}
		}

		public string Summary
		{
			get
			{
				return string.Format ("{0} objets d'immobilisations<br/>{1} catégories d'immobilisations<br/>{2} groupes<br/>{3} contacts<br/>{4} rapports<br/>{5} plans comptables",
					this.AssetCount, this.CategoryCount, this.GroupCount, this.PersonCount, this.ReportCount, this.AccountCount);
			}
		}

		public static MandatStatistics Empty = new MandatStatistics (-1, -1, -1, -1, -1, -1, -1);

		public const int LinesCount = 6;

		public readonly int							AssetCount;
		public readonly int							EventCount;
		public readonly int							CategoryCount;
		public readonly int							GroupCount;
		public readonly int							PersonCount;
		public readonly int							ReportCount;
		public readonly int							AccountCount;
	}
}
