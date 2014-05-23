//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public abstract class AbstractExport<T>
		where T : struct
	{
		public virtual void Export(ExportInstructions instructions, AbstractExportProfile profile, AbstractTreeTableFiller<T> filler)
		{
			this.instructions = instructions;
			this.profile      = profile;
			this.filler       = filler;
		}


		protected void FillArray(bool hasHeader)
		{
			//	Génère le contenu de this.array.
			var columnDescriptions = this.filler.Columns;
			int rowOffset = hasHeader ? 1 : 0;

			this.columnCount = columnDescriptions.Count ();
			this.rowCount = rowOffset + this.filler.Count;

			this.array = new string[columnCount, this.rowCount];

			//	Génère la première ligne d'en-tête.
			if (hasHeader)
			{
				for (int column=0; column<columnCount; column++)
				{
					var description = columnDescriptions[column];
					this.array[column, 0] = description.Header;
				}
			}

			//	Génère tout le contenu.
			for (int row=0; row<this.rowCount-rowOffset; row++)
			{
				var contentItem = this.filler.GetContent (row, 1, -1);  // toutes les colonnes d'une ligne

				for (int column=0; column<columnCount; column++)
				{
					var columnItem = contentItem.Columns[column];
					var cell = columnItem.Cells.First ();
					var description = columnDescriptions[column];
					this.array[column, rowOffset+row] = this.ConvertToString (cell, description);
				}
			}
		}


		#region String converter
		private string ConvertToString(AbstractTreeTableCell cell, TreeTableColumnDescription description)
		{
			if (cell is TreeTableCellAmortizedAmount)
			{
				return this.BaseConvertToString (cell as TreeTableCellAmortizedAmount, description);
			}
			else if (cell is TreeTableCellComputedAmount)
			{
				return this.BaseConvertToString (cell as TreeTableCellComputedAmount, description);
			}
			else if (cell is TreeTableCellDate)
			{
				return this.BaseConvertToString (cell as TreeTableCellDate, description);
			}
			else if (cell is TreeTableCellDecimal)
			{
				return this.BaseConvertToString (cell as TreeTableCellDecimal, description);
			}
			else if (cell is TreeTableCellInt)
			{
				return this.BaseConvertToString (cell as TreeTableCellInt, description);
			}
			else if (cell is TreeTableCellString)
			{
				return (cell as TreeTableCellString).Value;
			}
			else if (cell is TreeTableCellTree)
			{
				return this.BaseConvertToString (cell as TreeTableCellTree, description);
			}
			else
			{
				return null;
			}
		}

		private string BaseConvertToString(TreeTableCellAmortizedAmount cell, TreeTableColumnDescription description)
		{
			if (cell.Value.HasValue)
			{
				return TypeConverters.AmountToString (cell.Value.Value.FinalAmortizedAmount);
			}
			else
			{
				return null;
			}
		}

		private string BaseConvertToString(TreeTableCellComputedAmount cell, TreeTableColumnDescription description)
		{
			if (cell.Value.HasValue)
			{
				return TypeConverters.AmountToString (cell.Value.Value.FinalAmount);
			}
			else
			{
				return null;
			}
		}

		private string BaseConvertToString(TreeTableCellDate cell, TreeTableColumnDescription description)
		{
			return TypeConverters.DateToString (cell.Value);
		}

		private string BaseConvertToString(TreeTableCellDecimal cell, TreeTableColumnDescription description)
		{
			switch (description.Type)
			{
				case TreeTableColumnType.Rate:
					return TypeConverters.RateToString (cell.Value);

				case TreeTableColumnType.Amount:
					return TypeConverters.AmountToString (cell.Value);

				default:
					return TypeConverters.DecimalToString (cell.Value);
			}
		}

		private string BaseConvertToString(TreeTableCellInt cell, TreeTableColumnDescription description)
		{
			return TypeConverters.IntToString (cell.Value);
		}

		private string BaseConvertToString(TreeTableCellTree cell, TreeTableColumnDescription description)
		{
			return cell.Value;
		}
		#endregion


		protected ExportInstructions			instructions;
		protected AbstractExportProfile			profile;
		protected AbstractTreeTableFiller<T>	filler;
		protected string[,]						array;
		protected int							rowCount;
		protected int							columnCount;
	}
}