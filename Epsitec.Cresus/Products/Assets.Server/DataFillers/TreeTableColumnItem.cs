//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableColumnItem
	{
		public TreeTableColumnItem()
		{
			this.cells = new List<AbstractTreeTableCell> ();
		}


		public IEnumerable<AbstractTreeTableCell> Cells
		{
			get
			{
				return this.cells;
			}
		}

		public void AddRow(AbstractTreeTableCell cell)
		{
			this.cells.Add (cell);
		}


		private readonly List<AbstractTreeTableCell> cells;
	}
}
