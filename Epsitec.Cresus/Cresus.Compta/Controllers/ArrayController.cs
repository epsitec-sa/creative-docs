//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le tableau pour les données de la comptabilité.
	/// </summary>
	public class ArrayController
	{
		public ArrayController(AbstractController controller)
		{
			this.controller = controller;
			this.columnMappers = this.controller.ColumnMappers;
			this.ignoreChanges = new SafeCounter ();
		}


		public StringArray CreateUI(FrameBox parent, System.Action updateCellContent, System.Action columnsWidthChanged, System.Action selectedRowChanged, System.Action<Point> rightClick)
		{
			this.rightClick = rightClick;

			//	Crée l'en-tête en dessus du tableau.
			this.headerController = new HeaderController (this.controller);
			this.headerController.CreateUI (parent);

			//	Crée le tableau.
			this.array = new StringArray
			{
				Parent            = parent,
				Dock              = DockStyle.Fill,
				TabNavigationMode = TabNavigationMode.ActivateOnTab,
				Margins           = new Margins (0, 0, 0, 1),
			};

			this.array.UpdateCellContent += delegate
			{
				updateCellContent ();
			};

			this.array.ColumnsWidthChanged += delegate
			{
				this.headerController.UpdateGeometry (this.array);  // adapte l'en-tête
				columnsWidthChanged ();
			};

			this.array.SelectedRowChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.controller.IgnoreChanges.IsZero)
				{
					selectedRowChanged ();
					this.UpdateCommands ();
				}
			};

			this.array.SelectionClicked += new EventHandler<MessageEventArgs> (this.HandleArraySelectionClicked);

			this.array.FirstVisibleRowChanged += delegate
			{
				this.UpdateCommands ();
			};

			return this.array;
		}

		private void HandleArraySelectionClicked(object sender, MessageEventArgs e)
		{
			if (e.Message.IsRightButton)
			{
				this.rightClick (e.Point);
			}
		}


		public void UpdateColumnsHeader(int lineHeight)
		{
			this.headerController.UpdateColumns ();

			this.columnMappersShowed = this.columnMappers.Where (x => x.Show && x.RelativeWidth != 0).ToList ();
			this.array.Columns = this.columnMappersShowed.Count ();
			int column = 0;

			foreach (var mapper in this.columnMappersShowed)
			{
				this.array.SetColumnsRelativeWidth (column, mapper.RelativeWidth);
				this.array.SetColumnBreakMode      (column, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				this.array.SetColumnAlignment      (column, mapper.Alignment);

				column++;
			}

			this.array.LineHeight = lineHeight;
		}


		public bool GetColumnGeometry(ColumnType columnType, out double left, out double width)
		{
			//	Retourne la géométrie d'une colonne. Retourne false si la colonne n'est pas visible.
			int index = this.GetColumnIndex (columnType);

			if (index != -1 && this.columnMappers[index].Show)
			{
				left = 0;
				for (int column = 0; column < index; column++)
				{
					left += this.GetColumnsAbsoluteWidth (column);
				}

				width = this.GetColumnsAbsoluteWidth (index);

				return true;
			}
			else
			{
				left = 0;
				width = 0;

				return false;
			}
		}


		public void MoveSelection(int direction)
		{
			int firstRow, countRow, sel;
			this.GetHilitedRows (out firstRow, out countRow);

			if (direction == 0)
			{
				this.array.ShowRow (firstRow, countRow);
			}
			else
			{
				if (direction < 0)
				{
					sel = firstRow+direction;
				}
				else
				{
					sel = firstRow+countRow;
				}

				if (sel >= 0 && sel < this.array.TotalRows)
				{
					this.SelectedRow = sel;
				}
			}
		}

		private void UpdateCommands()
		{
			int firstRow, countRow;
			this.GetHilitedRows (out firstRow, out countRow);

			this.controller.SetCommandEnable (Res.Commands.Select.Up,   firstRow != -1 && firstRow > 0);
			this.controller.SetCommandEnable (Res.Commands.Select.Down, firstRow != -1 && firstRow+countRow < this.array.TotalRows);

			this.controller.SetCommandEnable (Res.Commands.Select.Home, !this.array.IsShowedRow (firstRow, countRow));

			this.controller.SetCommandEnable (Res.Commands.Edit.Up,   this.controller.DataAccessor.IsMoveEditionLineEnable (-1));
			this.controller.SetCommandEnable (Res.Commands.Edit.Down, this.controller.DataAccessor.IsMoveEditionLineEnable (1));
		}


		public bool Enable
		{
			get
			{
				return this.array.Enable;
			}
			set
			{
				this.array.Enable = value;
			}
		}

		public void SetSearchLocator(int row, ColumnType columnType)
		{
			this.array.SetSearchLocator (row, this.GetColumnIndex (columnType));
		}

		public void UpdateArrayContent(int rowCount, System.Func<int, ColumnType, FormattedText> getCellText, System.Func<int, bool> getBottomSeparator)
		{
			//	Met à jour le contenu du tableau.
			this.array.TotalRows = rowCount;

			this.AdjustRows ();
			this.array.AdjustFirstVisibleRow ();

			int first       = this.array.FirstVisibleRow;
			int columnCount = this.array.Columns;

			for (int line=0; line<this.array.LineCount; line++)
			{
				int row = first+line;
				bool sep = getBottomSeparator (row);

				if (row < rowCount)
				{
					for (int column = 0; column < columnCount; column++)
					{
						string text = getCellText (row, this.columnMappersShowed[column].Column).ToString ();
						var color = Color.Empty;

						if (!string.IsNullOrEmpty (text) && text.StartsWith (StringArray.SpecialContentSearchTarget))
						{
							text = text.Substring (StringArray.SpecialContentSearchTarget.Length);
							color = UIBuilder.BackOutsideSearchColor;  // jaune pâle
						}

						this.array.SetLineState (column, row, StringList.CellState.Normal);
						this.array.SetLineString (column, row, text);
						this.array.SetLineColor (column, row, color);
						this.array.SetLineBottomSeparator (column, row, sep);
					}
				}
				else
				{
					for (int column = 0; column < columnCount; column++)
					{
						this.array.SetLineState (column, row, StringList.CellState.Disabled);
						this.array.SetLineString (column, row, "");
						this.array.SetLineBottomSeparator (column, row, false);
					}
				}
			}
		}

		private void AdjustRows()
		{
			int sel = this.SelectedRow;

			if (sel != -1)
			{
				int adjustedSel = System.Math.Min (sel, this.array.TotalRows-1);

				if (adjustedSel != sel)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.SelectedRow = sel;
					}
				}
			}

			int firstRow, countRow;
			this.GetHilitedRows (out firstRow, out countRow);

			if (firstRow != -1)
			{
				int lastRow = firstRow+countRow;

				int adjustedFirstRow = System.Math.Min (firstRow, this.array.TotalRows-1);
				int adjustedLastRow  = System.Math.Min (lastRow, this.array.TotalRows-1);
				int adjustedCountRow = System.Math.Max (lastRow-firstRow, 1);

				if (adjustedFirstRow != firstRow || adjustedCountRow != countRow)
				{
					this.SetHilitedRows (adjustedFirstRow, adjustedCountRow);
				}
			}
		}

		public void HiliteHeaderColumn(ColumnType columnType)
		{
			//	Met en évidence une en-tête de colonne à choix.
			this.headerController.HiliteColumn (columnType);
		}

		public void SetHilitedRows(int firstRow, int countRow)
		{
			//	Met en évidence un groupe de lignes.
			this.array.SetHilitedRows (firstRow, countRow);
		}

		public void GetHilitedRows(out int firstRow, out int countRow)
		{
			this.array.GetHilitedRows (out firstRow, out countRow);
		}

		public Color ColorSelection
		{
			//	Couleur utilisée pour les lignes sélectionnées..
			get
			{
				return this.array.ColorSelection;
			}
			set
			{
				this.array.ColorSelection = value;
			}
		}

		public void ShowRow(int firstRow, int countRow)
		{
			//	Montre une ligne dans le tableau.
			this.array.ShowRow (firstRow, countRow);
		}

		public void SetSelectedRow(int row, int column)
		{
			this.array.SetSelectedRow (row, column);
		}

		public int SelectedRow
		{
			//	Ligne sélectionnée.
			get
			{
				return this.array.SelectedRow;
			}
			set
			{
				this.array.SelectedRow = value;
			}
		}

		public int SelectedColumn
		{
			//	Colonne sélectionnée.
			get
			{
				return this.array.SelectedColumn;
			}
		}

		public int InsertionPointRow
		{
			//	Point d'insertion.
			get
			{
				return this.array.InsertionPointRow;
			}
			set
			{
				this.array.InsertionPointRow = value;
			}
		}

		public double GetColumnsAbsoluteWidth(int column)
		{
			//	Retourne la largeur absolue d'une colonne.
			return this.array.GetColumnsAbsoluteWidth (column);
		}


		private int GetColumnIndex(ColumnType columnType)
		{
			return this.columnMappersShowed.FindIndex (x => x.Column == columnType);
		}


		private readonly AbstractController		controller;
		private readonly List<ColumnMapper>		columnMappers;
		private readonly SafeCounter			ignoreChanges;

		private System.Action<Point>			rightClick;
		private List<ColumnMapper>				columnMappersShowed;
		private HeaderController				headerController;
		private StringArray						array;
	}
}
