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

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère le tableau pour les données de la comptabilité.
	/// </summary>
	public class ArrayController<Entity>
		where Entity : class
	{
		public ArrayController()
		{
		}


		public StringArray CreateUI(FrameBox parent, IEnumerable<FormattedText> descriptions, IEnumerable<double> relativeWidths, IEnumerable<ContentAlignment> alignments,
									System.Action updateCellContent, System.Action columnsWidthChanged, System.Action selectedRowChanged)
		{
			int columnCount = relativeWidths.Count ();
			System.Diagnostics.Debug.Assert (columnCount == descriptions.Count ());
			System.Diagnostics.Debug.Assert (columnCount == alignments.Count ());

			//	Crée l'en-tête en dessus du tableau.
			this.headerController = new HeaderController ();
			this.headerController.CreateUI (parent, descriptions);

			//	Crée le tableau.
			this.array = new StringArray
			{
				Parent            = parent,
				Columns           = columnCount,
				LineHeight        = 18,
				Dock              = DockStyle.Fill,
				TabNavigationMode = TabNavigationMode.ActivateOnTab,
				Margins           = new Margins (0, 0, 0, 1),
			};

			int column = 0;
			foreach (var relativeWidth in relativeWidths)
			{
				this.array.SetColumnsRelativeWidth (column, relativeWidth);
				this.array.SetColumnBreakMode (column, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
				column++;
			}

			column = 0;
			alignments.ToList ().ForEach (alignment => this.array.SetColumnAlignment (column++, alignment));

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

		public void SetColumnAlignment(int column, ContentAlignment alignment)
		{
			this.array.SetColumnAlignment (column, alignment);
		}

		public void UpdateArrayContent(int rowCount, System.Func<int, int, FormattedText> getCellText)
		{
			//	Met à jour le contenu du tableau.
			int columnCount = this.array.Columns;

			this.array.TotalRows = rowCount;

			int first = this.array.FirstVisibleRow;
			for (int line=0; line<this.array.LineCount; line++)
			{
				int row = first+line;

				if (row < rowCount)
				{
					for (int column = 0; column < columnCount; column++)
					{
						this.array.SetLineState (column, row, StringList.CellState.Normal);
						this.array.SetLineString (column, row, getCellText (row, column).ToString ());
					}
				}
				else
				{
					for (int column = 0; column < columnCount; column++)
					{
						this.array.SetLineState (column, row, StringList.CellState.Disabled);
						this.array.SetLineString (column, row, "");
					}
				}
			}
		}

		public void HiliteHeaderColumn(int column)
		{
			//	Met en évidence une en-tête de colonne à choix.
			this.headerController.HiliteColumn (column);
		}

		public void ShowSelection()
		{
			//	Montre la sélection dans le tableau.
			this.array.ShowSelectedRow ();
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

		public double GetColumnsAbsoluteWidth(int column)
		{
			//	Retourne la largeur absolue d'une colonne.
			return this.array.GetColumnsAbsoluteWidth (column);
		}


		public Entity SelectedEntity
		{
			get
			{
				return this.selectedEntity;
			}
			set
			{
				this.selectedEntity = value;
			}
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
		private Entity							selectedEntity;
		private bool							ignoreChanged;
	}
}
