//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class ArticleLinesController
	{
		public ArticleLinesController(DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;
		}


		public void CreateUI(Widget parent)
		{
			var tile = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				DrawFullFrame = true,
			};

			this.table = new CellTable
			{
				Parent = tile,
				StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.Header,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
				DefHeight = ArticleLinesController.lineHeight,
				Margins = new Margins (2),
				Dock = DockStyle.Fill,
			};
		}

		public void UpdateUI(System.Func<int, ColumnType, FormattedText> getCellContent, int? sel = null)
		{
			this.getCellContent = getCellContent;

			int rows = this.businessDocumentEntity.Lines.Count;
			int columns = this.ColumnTypes.Count ();

			this.table.SetArraySize (columns, rows);

			int column = 0;
			foreach (var columnType in this.ColumnTypes)
			{
				this.table.SetWidthColumn (column, this.GetColumnWidth (columnType));
				this.table.SetHeaderTextH (column, this.GetColumnName (columnType));

				column++;
			}

			if (sel == null)
			{
				sel = this.table.SelectedRow;
			}

			for (int row=0; row<rows; row++)
			{
				this.TableFillRow (row);
				this.TableUpdateRow (row);
			}

			sel = System.Math.Min (sel.Value, rows-1);
			if (sel != -1)
			{
				this.table.SelectRow (sel.Value, true);
			}
		}

		private void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			int column = 0;
			foreach (var columnType in this.ColumnTypes)
			{
				if (columnType == ColumnType.Group)
				{
					// TODO: ...
				}
				else
				{
					if (this.table[column, row].IsEmpty)
					{
						var text = new StaticText
						{
							PreferredHeight = ArticleLinesController.lineHeight,
							ContentAlignment = this.GetRowColumnContentAlignment (row, columnType),
							Dock = DockStyle.Fill,
							Margins = new Margins (4, 4, 0, 0),
						};

						this.table[column, row].Insert (text);
					}
				}

				column++;
			}
		}

		private void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			int column = 0;
			foreach (var columnType in this.ColumnTypes)
			{
				if (this.table[column, row].Children.Count != 0)
				{
					var text = this.table[column, row].Children[0] as StaticText;

					if (text != null)
					{
						text.FormattedText = this.getCellContent (row, columnType);
					}
				}

				column++;
			}

			this.table.SelectRow (row, false);
		}


		private IEnumerable<ColumnType> ColumnTypes
		{
			//	Retourne les colonnes visibles dans la table, de gauche à droite.
			get
			{
				yield return ColumnType.Group;
				yield return ColumnType.Id;
				yield return ColumnType.Description;
				yield return ColumnType.Quantity;
				yield return ColumnType.Unit;
				yield return ColumnType.Type;
				yield return ColumnType.Price;
				yield return ColumnType.Total;
			}
		}

		private int GetColumnWidth(ColumnType columnType)
		{
			switch (columnType)
			{
				case ColumnType.Group:
					return 30;

				case ColumnType.Quantity:
					return 40;

				case ColumnType.Unit:
					return 50;

				case ColumnType.Type:
					return 70;

				case ColumnType.Id:
					return 50;

				case ColumnType.Description:
					return 180;

				case ColumnType.Price:
				case ColumnType.Total:
					return 70;

				default:
					return 100;
			}
		}

		private string GetColumnName(ColumnType columnType)
		{
			switch (columnType)
			{
				case ColumnType.Quantity:
					return "Nb";

				case ColumnType.Unit:
					return "Unité";

				case ColumnType.Type:
					return "Type";

				case ColumnType.Id:
					return "N°";

				case ColumnType.Description:
					return "Désignation";

				case ColumnType.Price:
					return "Prix";

				case ColumnType.Total:
					return "Total";

				default:
					return null;
			}
		}

		private ContentAlignment GetRowColumnContentAlignment(int row, ColumnType columnType)
		{
			if (columnType == ColumnType.Quantity ||
				columnType == ColumnType.Price)
			{
				return ContentAlignment.MiddleRight;
			}

			return ContentAlignment.MiddleLeft;
		}

	
		private static readonly double lineHeight = 17;

		private readonly DocumentMetadataEntity documentMetadataEntity;
		private readonly BusinessDocumentEntity businessDocumentEntity;

		private CellTable										table;
		private System.Func<int, ColumnType, FormattedText>		getCellContent;
	}
}
