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
	/// <summary>
	/// Liste de lignes d'articles (AbstractDocumentItemEntity).
	/// </summary>
	public class ArticleLinesController
	{
		public ArticleLinesController(DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;

			this.showAllColumns = true;
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
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine | CellArrayStyles.SelectMulti,
				DefHeight = ArticleLinesController.lineHeight,
				Margins = new Margins (2),
				Dock = DockStyle.Fill,
			};
		}

		public void UpdateUI(int lineCount, System.Func<int, ColumnType, FormattedText> getCellContent, int? sel = null)
		{
			this.getCellContent = getCellContent;

			int columns = this.ColumnTypes.Count ();

			this.table.SetArraySize (columns, lineCount);

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

			for (int row=0; row<lineCount; row++)
			{
				this.TableFillRow (row);
				this.TableUpdateRow (row);
			}

			sel = System.Math.Min (sel.Value, lineCount-1);
			if (sel != -1)
			{
				this.table.SelectRow (sel.Value, true);
				this.table.ShowSelect ();
			}
		}


		public bool HasSelection
		{
			get
			{
				return this.Selection.Count != 0;
			}
		}

		public int? LastSelection
		{
			get
			{
				var selection = this.Selection;

				if (selection.Count == 0)
				{
					return null;
				}
				else
				{
					return selection.Last ();
				}
			}
		}

		public List<int> Selection
		{
			get
			{
				var list = new List<int> ();

				for (int i = 0; i < this.table.Rows; i++)
				{
					if (this.table.IsCellSelected (i, 0))
					{
						list.Add (i);
					}
				}

				return list;
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
				if (this.showAllColumns)
				{
					yield return ColumnType.Group;

					yield return ColumnType.ArticleId;
					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.Type;
					yield return ColumnType.Date;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.Discount;
					yield return ColumnType.LinePrice;
					yield return ColumnType.Vat;
					yield return ColumnType.Total;
				}
				else
				{
					yield return ColumnType.Group;

					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.Type;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.LinePrice;
					yield return ColumnType.Total;
				}
			}
		}

		private int GetColumnWidth(ColumnType columnType)
		{
			switch (columnType)
			{
				case ColumnType.Group:
					return 30;

				case ColumnType.QuantityAndUnit:
					return 60;

				case ColumnType.Type:
					return 60;

				case ColumnType.Date:
					return 70;

				case ColumnType.ArticleId:
					return 50;

				case ColumnType.ArticleDescription:
					return 180;

				case ColumnType.Discount:
				case ColumnType.UnitPrice:
				case ColumnType.LinePrice:
				case ColumnType.Vat:
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
				case ColumnType.QuantityAndUnit:
					return "Quantité";

				case ColumnType.Type:
					return "Type";

				case ColumnType.Date:
					return "Date";

				case ColumnType.ArticleId:
					return "Article";

				case ColumnType.ArticleDescription:
					return "Désignation";

				case ColumnType.Discount:
					return "Rabais";

				case ColumnType.UnitPrice:
					return "p.u. HT";

				case ColumnType.LinePrice:
					return "Prix HT";

				case ColumnType.Vat:
					return "TVA";

				case ColumnType.Total:
					return "Prix TTC";

				default:
					return null;
			}
		}

		private ContentAlignment GetRowColumnContentAlignment(int row, ColumnType columnType)
		{
			if (columnType == ColumnType.QuantityAndUnit ||
				columnType == ColumnType.Discount ||
				columnType == ColumnType.UnitPrice ||
				columnType == ColumnType.LinePrice ||
				columnType == ColumnType.Vat ||
				columnType == ColumnType.Total)
			{
				return ContentAlignment.MiddleRight;
			}

			return ContentAlignment.MiddleLeft;
		}

	
		private static readonly double lineHeight = 17;

		private readonly DocumentMetadataEntity					documentMetadataEntity;
		private readonly BusinessDocumentEntity					businessDocumentEntity;

		private CellTable										table;
		private System.Func<int, ColumnType, FormattedText>		getCellContent;
		private bool											showAllColumns;
	}
}
