//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct MandatStatistics
	{
		public MandatStatistics(int assetsCount, int eventsCount, int categoriesCount, int groupsCount, int personsCount, int reportsCount, int accountsCount)
		{
			this.AssetsCount	 = assetsCount;
			this.EventsCount	 = eventsCount;
			this.CategoriesCount = categoriesCount;
			this.GroupsCount	 = groupsCount;
			this.PersonsCount	 = personsCount;
			this.ReportsCount	 = reportsCount;
			this.AccountsCount	 = accountsCount;
		}

		public bool IsEmpty
		{
			get
			{
				return this.AssetsCount == -1;
			}
		}

		public string Summary
		{
			get
			{
				return string.Format ("{0} objets d'immobilisations<br/>{1} catégories d'immobilisations<br/>{2} groupes<br/>{3} contacts<br/>{4} rapports<br/>{5} plans comptables",
					this.AssetsCount, this.CategoriesCount, this.GroupsCount, this.PersonsCount, this.ReportsCount, this.AccountsCount);
			}
		}

		public static MandatStatistics Empty = new MandatStatistics (-1, -1, -1, -1, -1, -1, -1);

		public const int LinesCount = 6;

		public readonly int						AssetsCount;
		public readonly int						EventsCount;
		public readonly int						CategoriesCount;
		public readonly int						GroupsCount;
		public readonly int						PersonsCount;
		public readonly int						ReportsCount;
		public readonly int						AccountsCount;
	}
}
