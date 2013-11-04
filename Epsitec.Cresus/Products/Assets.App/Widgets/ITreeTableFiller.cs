//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	interface ITreeTableFiller
	{
		void SetColumns(TreeTableColumnDescription[] descriptions, int dockToLeftCount);

		void SetColumnCells(int rank, TreeTableCellTree[] cells);
		void SetColumnCells(int rank, TreeTableCellString[] cells);
		void SetColumnCells(int rank, TreeTableCellDecimal[] cells);
		void SetColumnCells(int rank, TreeTableCellComputedAmount[] cells);
		void SetColumnCells(int rank, TreeTableCellDate[] cells);
		void SetColumnCells(int rank, TreeTableCellInt[] cells);
		void SetColumnCells(int rank, TreeTableCellGlyph[] cells);
	}
}
