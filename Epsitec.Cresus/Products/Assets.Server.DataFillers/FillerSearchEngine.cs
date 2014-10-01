//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public static class FillerSearchEngine<T>
		where T : struct
	{
		public static int Search(INodeGetter<T> nodeGetter, AbstractTreeTableFiller<T> dataFiller, int row,
			string pattern, int direction,
			SearchMode mode = SearchMode.IgnoreCase | SearchMode.IgnoreDiacritic | SearchMode.Fragment)
		{
			//	A partir d'une ligne sélectionnée, on cherche la prochaine ligne correspondant
			//	au motif de recherche, cycliquement.
			int count = nodeGetter.Count;

			for (int i=0; i<count; i++)
			{
				row += direction;

				if (row < 0)
				{
					row = count-1;
				}

				if (row > count-1)
				{
					row = 0;
				}

				var content = dataFiller.GetContent (row, 1, -1);
				var result = FillerSearchEngine<T>.Search (content, pattern, mode);

				if (!result.IsEmpty)
				{
					return row;
				}
			}

			return -1;  // pas trouvé
		}


		private static Result Search(TreeTableContentItem content, string pattern, SearchMode mode)
		{
			if (content.Columns.Any ())
			{
				var engine = new SearchEngine (pattern, mode);

				int rowCount = content.Columns[0].Cells.Count;

				for (int row=0; row<rowCount; row++)
				{
					for (int column=0; column<content.Columns.Count; column++)
					{
						var cell = content.Columns[column].Cells[row];

						if (FillerSearchEngine<T>.IsMatching (engine, cell))
						{
							return new Result (column, row);
						}
					}
				}
			}

			return Result.Empty;
		}


		private static bool IsMatching(SearchEngine engine, AbstractTreeTableCell cell)
		{
			if (cell is TreeTableCellString)
			{
				var c = cell as TreeTableCellString;
				return engine.IsMatching (c.Value);
			}
			else if (cell is TreeTableCellTree)
			{
				var c = cell as TreeTableCellTree;
				return engine.IsMatching (c.Value);
			}
			else if (cell is TreeTableCellDecimal)
			{
				var c = cell as TreeTableCellDecimal;
				return engine.IsMatching (c.Value);
			}
			else if (cell is TreeTableCellAmortizedAmount)
			{
				var c = cell as TreeTableCellAmortizedAmount;
				if (c.Value.HasValue)
				{
					return engine.IsMatching (c.Value.Value.FinalAmortizedAmount);
				}
			}
			else if (cell is TreeTableCellComputedAmount)
			{
				var c = cell as TreeTableCellComputedAmount;
				if (c.Value.HasValue)
				{
					return engine.IsMatching (c.Value.Value.FinalAmount);
				}
			}
			else if (cell is TreeTableCellInt)
			{
				var c = cell as TreeTableCellInt;
				return engine.IsMatching (c.Value);
			}
			else if (cell is TreeTableCellDate)
			{
				var c = cell as TreeTableCellDate;
				return engine.IsMatching (c.Value);
			}

			return false;
		}


		private struct Result
		{
			public Result(int column, int row)
			{
				this.Column = column;
				this.Row    = row;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Column == -1
						&& this.Row    == -1;
				}
			}

			public static Result Empty = new Result (-1, -1);

			public readonly int Column;
			public readonly int Row;
		}
	}
}
