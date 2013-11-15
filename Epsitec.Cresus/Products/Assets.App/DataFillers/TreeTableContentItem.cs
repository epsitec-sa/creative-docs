//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class TreeTableContentItem
	{
		public TreeTableContentItem()
		{
			this.columns = new List<TreeTableColumnItem> ();
		}

		public List<TreeTableColumnItem> Columns
		{
			get
			{
				return this.columns;
			}
		}

		private readonly List<TreeTableColumnItem> columns;
	}
}
