//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>LineTreeSorter</c> class is used to sort a collection of document lines.
	/// The real work is implemented in <see cref="LineTreeNode"/>.
	/// </summary>
	internal static class LineTreeSorter
	{
		public static void Sort(IEnumerable<AbstractDocumentItemEntity> lines, List<AbstractDocumentItemEntity> list)
		{
			LineTreeNode root = new LineTreeNode (0);

			foreach (var line in lines)
			{
				root.Insert (line);
			}

			list.AddRange (root.Lines);
		}
	}
}