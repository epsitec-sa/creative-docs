//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	/// <summary>
	/// Algorithme général de recherche d'un pattern dans un DataFiller.
	/// </summary>
	public static class FillerSearchEngine<T>
		where T : struct
	{
		public static int Search(DataAccessor accessor, INodeGetter<T> nodeGetter, AbstractTreeTableFiller<T> dataFiller,
			SearchDefinition definition, int row, int direction)
		{
			//	A partir d'une ligne donnée, on cherche la prochaine ligne correspondant
			//	au motif de recherche, dans une direction à choix, cycliquement.
			//	Retourne la ligne trouvée, ou -1.
			System.Diagnostics.Debug.Assert (direction == 1 || direction == -1);

			var engine = new SearchEngine (definition);

			int count = nodeGetter.Count;
			for (int i=0; i<count; i++)
			{
				row += direction;

				if (row < 0)  // arrivé avant le début ?
				{
					row = count-1;  // va à la fin
				}

				if (row > count-1)  // arrivé après la fin ?
				{
					row = 0;  // va au début
				}

				var content = dataFiller.GetContent (row, 1, -1);  // demande juste une ligne
				var result = FillerSearchEngine<T>.Search (accessor, content, engine);

				if (!result.IsEmpty)  // trouvé ?
				{
					return row;
				}
			}

			return -1;  // pas trouvé
		}


		private static Result Search(DataAccessor accessor, TreeTableContentItem content, SearchEngine engine)
		{
			if (content.Columns.Any ())
			{
				int rowCount = content.Columns[0].Cells.Count;

				for (int row=0; row<rowCount; row++)
				{
					for (int column=0; column<content.Columns.Count; column++)
					{
						var cell = content.Columns[column].Cells[row];

						if (FillerSearchEngine<T>.IsMatching (accessor, engine, cell))
						{
							return new Result (column, row);  // retourne l'emplacement trouvé
						}
					}
				}
			}

			return Result.Empty;  // pas trouvé
		}


		private static bool IsMatching(DataAccessor accessor, SearchEngine engine, AbstractTreeTableCell cell)
		{
			//	Cherche si le contenu d'une cellule match avec le pattern, quel que soit
			//	le type de la cellule.
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
					return engine.IsMatching (c.Value.Value.FinalAmount);
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
