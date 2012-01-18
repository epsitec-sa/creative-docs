//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Ce contrôleur gère le tableau pour les données de la comptabilité.
	/// </summary>
	public class ArrayController
	{
		public ArrayController()
		{
		}


		public StringArray CreateUI(FrameBox parent, System.Action updateCellContent, System.Action columnsWidthChanged, System.Action selectedRowChanged)
		{
			//	Crée l'en-tête en dessus du tableau.
			this.headerController = new HeaderController ();
			this.headerController.CreateUI (parent);

			//	Crée le tableau.
			this.array = new StringArray
			{
				Parent            = parent,
				LineHeight        = 18,
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
				if (!this.ignoreChanged)
				{
					selectedRowChanged ();
				}
			};

			return this.array;
		}


		public void UpdateColumnsHeader(IEnumerable<FormattedText> descriptions, IEnumerable<double> relativeWidths, IEnumerable<ContentAlignment> alignments)
		{
			int columnCount = relativeWidths.Count ();
			System.Diagnostics.Debug.Assert (columnCount == descriptions.Count ());
			System.Diagnostics.Debug.Assert (columnCount == alignments.Count ());

			this.headerController.UpdateColumns (descriptions);

			this.array.Columns = columnCount;

			for (int column = 0; column < columnCount; column++)
			{
				this.array.SetColumnsRelativeWidth (column, relativeWidths.ElementAt (column));
				this.array.SetColumnBreakMode      (column, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				this.array.SetColumnAlignment      (column, alignments.ElementAt (column));
			}
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

		public void SetSearchLocator(int row, int column)
		{
			this.array.SetSearchLocator (row, column);
		}

		public void UpdateArrayContent(int rowCount, System.Func<int, int, FormattedText> getCellText, System.Func<int, bool> getBottomSeparator)
		{
			//	Met à jour le contenu du tableau.
			this.array.TotalRows = rowCount;

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
						string text = getCellText (row, column).ToString ();
						var color = Color.Empty;

						if (!string.IsNullOrEmpty (text) && text.StartsWith (StringArray.SpecialContentSearchingTarget))
						{
							text = text.Substring (StringArray.SpecialContentSearchingTarget.Length);
							color = SearchResult.BackOutsideSearch;  // jaune pâle
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

		public void HiliteHeaderColumn(int column)
		{
			//	Met en évidence une en-tête de colonne à choix.
			this.headerController.HiliteColumn (column);
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


		public bool IgnoreChanged
		{
			get
			{
				return this.ignoreChanged;
			}
			set
			{
				this.ignoreChanged = value;
			}
		}


		private HeaderController				headerController;
		private StringArray						array;
		private bool							ignoreChanged;
	}
}
