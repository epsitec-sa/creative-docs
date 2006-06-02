using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Comparers
{
	/// <summary>
	///	Compare deux bundles d'après leurs rangs.
	/// </summary>
	public class BundleRank : IComparer<ResourceBundle>
	{
		public int Compare(ResourceBundle obj1, ResourceBundle obj2)
		{
			int rank1 = obj1.Rank;
			int rank2 = obj2.Rank;

			return rank1.CompareTo(rank2);
		}
	}
}
