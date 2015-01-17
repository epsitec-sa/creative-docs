//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public abstract class AbstractExport<T> : System.IDisposable
		where T : struct
	{
		public virtual void Export(DataAccessor accessor, ExportInstructions instructions, AbstractExportProfile profile, AbstractTreeTableFiller<T> filler, ColumnsState columnsState)
		{
			this.accessor     = accessor;
			this.instructions = instructions;
			this.profile      = profile;
			this.filler       = filler;
			this.columnsState = columnsState;
		}

		public void Dispose()
		{
		}


		protected void FillArray(bool hasHeader)
		{
			//	Génère le contenu de this.array.
			var columnsState = this.columnsState.Columns.ToArray ();
			var columnDescriptions = this.filler.Columns;

			int rowOffset = hasHeader ? 1 : 0;
			this.columnCount = columnsState.Where (x => !x.Hide).Count ();
			this.rowCount = rowOffset + this.filler.Count;

			this.array = new string[this.columnCount, this.rowCount];
			this.levels = new int[this.rowCount];

			//	Génère la première ligne d'en-tête.
			if (hasHeader)
			{
				int c = 0;
				for (int abs=0; abs<columnsState.Length; abs++)
				{
					var mapped = this.columnsState.AbsoluteToMapped (abs);
					var columnState = columnsState[mapped];
					if (!columnState.Hide)
					{
						var description = columnDescriptions.Where (x => x.Field == columnState.Field).FirstOrDefault ();
						this.array[c++, 0] = description.Header;
						this.levels[0] = 0;
					}
				}
			}

			//	Génère tout le contenu.
			for (int row=0; row<this.rowCount-rowOffset; row++)
			{
				var contentItem = this.filler.GetContent (row, 1, -1);  // toutes les colonnes d'une ligne

				int c = 0;
				int level = 0;

				for (int abs=0; abs<columnsState.Length; abs++)
				{
					var mapped = this.columnsState.AbsoluteToMapped (abs);
					var columnState = columnsState[mapped];
					if (!columnState.Hide)
					{
						var description = columnDescriptions.Where (x => x.Field == columnState.Field).FirstOrDefault ();
						var columnItem = contentItem.Columns[mapped];
						var cell = columnItem.Cells.First ();
						this.array[c++, rowOffset+row] = this.ConvertToString (cell, description);

						if (cell is TreeTableCellTree)
						{
							level = System.Math.Max (level, (cell as TreeTableCellTree).Level);
						}
					}
				}

				this.levels[rowOffset+row] = level;
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
				return this.AmountToString (cell.Value.Value.FinalAmount);
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
				return this.AmountToString (cell.Value.Value.FinalAmount);
			}
			else
			{
				return null;
			}
		}

		private string BaseConvertToString(TreeTableCellDate cell, TreeTableColumnDescription description)
		{
			return this.DateToString (cell.Value);
		}

		private string BaseConvertToString(TreeTableCellDecimal cell, TreeTableColumnDescription description)
		{
			switch (description.Type)
			{
				case TreeTableColumnType.Rate:
					return this.RateToString (cell.Value);

				case TreeTableColumnType.Amount:
					return this.AmountToString (cell.Value);

				default:
					return this.DecimalToString (cell.Value);
			}
		}

		private string BaseConvertToString(TreeTableCellInt cell, TreeTableColumnDescription description)
		{
			return this.IntToString (cell.Value);
		}

		private string BaseConvertToString(TreeTableCellTree cell, TreeTableColumnDescription description)
		{
			return cell.Value;
		}
		#endregion


		#region Universal converters
		//	Si humanFormat == false, on utilise un format universel, qui ne met pas d'espace
		//	pour séparer les milliers par exemple.

		private string AmountToString(decimal? amount)
		{
			if (this.humanFormat)
			{
				return TypeConverters.AmountToString (amount);
			}
			else
			{
				if (amount.HasValue)
				{
					return amount.Value.ToString ("0.00");  // toujours 2 décimales
				}
				else
				{
					return null;
				}
			}
		}

		private string RateToString(decimal? rate)
		{
			if (this.humanFormat)
			{
				return TypeConverters.RateToString (rate);
			}
			else
			{
				if (rate.HasValue)
				{
					return rate.Value.ToString ("P");
				}
				else
				{
					return null;
				}
			}
		}

		private string DecimalToString(decimal? value)
		{
			if (this.humanFormat)
			{
				return TypeConverters.DecimalToString (value);
			}
			else
			{
				if (value.HasValue)
				{
					return value.Value.ToString (System.Globalization.CultureInfo.InvariantCulture);
				}
				else
				{
					return null;
				}
			}
		}

		private string IntToString(int? value)
		{
			if (this.humanFormat)
			{
				return TypeConverters.IntToString (value);
			}
			else
			{
				if (value.HasValue)
				{
					return value.Value.ToString (System.Globalization.CultureInfo.InvariantCulture);
				}
				else
				{
					return null;
				}
			}
		}

		private string DateToString(System.DateTime? date)
		{
			if (this.humanFormat)
			{
				return TypeConverters.DateToString (date);
			}
			else
			{
				if (date.HasValue)
				{
					return date.Value.ToString ("dd.MM.yyyy");
				}
				else
				{
					return null;
				}
			}
		}
		#endregion


		protected DataAccessor					accessor;
		protected ExportInstructions			instructions;
		protected AbstractExportProfile			profile;
		protected AbstractTreeTableFiller<T>	filler;
		protected ColumnsState					columnsState;
		protected string[,]						array;
		protected int[]							levels;
		protected int							rowCount;
		protected int							columnCount;
		protected bool							humanFormat;
	}
}