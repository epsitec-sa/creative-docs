//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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
	public class BusinessDocumentLinesController
	{
		public BusinessDocumentLinesController(DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;
		}


		public void CreateUI(Widget parent)
		{
			int tabIndex = 0;

			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				TabIndex = tabIndex++,
			};

			{
				//	Crée la toolbar.
				double buttonSize = Library.UI.TinyButtonSize;

				var toolbar = UIBuilder.CreateMiniToolbar (frame, buttonSize);
				toolbar.Dock = DockStyle.Top;
				toolbar.Margins = new Margins (0, 0, 0, -1);
				toolbar.TabIndex = tabIndex++;

				this.addButton = new GlyphButton
				{
					Parent = toolbar,
					PreferredSize = new Size (buttonSize*2+1, buttonSize),
					GlyphShape = GlyphShape.Plus,
					Margins = new Margins (0, 0, 0, 0),
					Dock = DockStyle.Left,
					TabIndex = tabIndex++,
				};

				this.removeButton = new GlyphButton
				{
					Parent = toolbar,
					PreferredSize = new Size (buttonSize, buttonSize),
					GlyphShape = GlyphShape.Minus,
					Margins = new Margins (1, 0, 0, 0),
					Dock = DockStyle.Left,
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.addButton, "Ajoute une ligne");
				ToolTip.Default.SetToolTip (this.removeButton, "Supprime la ligne sélectionnée");
			}

			{
				//	Crée la liste.
				var tile = new FrameBox
				{
					Parent = frame,
					Dock = DockStyle.Fill,
					DrawFullFrame = true,
					TabIndex = tabIndex++,
				};

				this.table = new CellTable
				{
					Parent = tile,
					StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.Header,
					StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
					//DefHeight = 24,
					Margins = new Margins (2),
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};
			}
		}

		public void UpdateUI(int? sel = null)
		{
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
						text.FormattedText = this.GetRowColumnText (row, columnType);
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
				yield return ColumnType.Quantity;
				yield return ColumnType.Unit;
				yield return ColumnType.Type;
				yield return ColumnType.Description;
				yield return ColumnType.Price;
			}
		}

		private enum ColumnType
		{
			Group,
			Quantity,
			Unit,
			Type,
			Description,
			Price,
		}

		private int GetColumnWidth(ColumnType columnType)
		{
			switch (columnType)
			{
				case ColumnType.Group:
					return 40;

				case ColumnType.Quantity:
					return 50;

				case ColumnType.Unit:
					return 50;

				case ColumnType.Type:
					return 60;

				case ColumnType.Description:
					return 234;

				case ColumnType.Price:
					return 90;

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

				case ColumnType.Description:
					return "Désignation";

				case ColumnType.Price:
					return "Prix";

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

		private FormattedText GetRowColumnText(int row, ColumnType columnType)
		{
			var line = this.businessDocumentEntity.Lines[row];

			if (columnType == ColumnType.Quantity)
			{
				var quantity = BusinessDocumentLinesController.GetArticleQuantity (line as ArticleDocumentItemEntity);

				if (quantity != null)
				{
					return quantity.ToString ();
				}
			}

			if (columnType == ColumnType.Description)
			{
				return BusinessDocumentLinesController.GetArticleDescription (line);
			}

			if (columnType == ColumnType.Price)
			{
				var price = BusinessDocumentLinesController.GetArticlePrice (line as ArticleDocumentItemEntity);

				if (price != null)
				{
					return  Misc.PriceToString (price);
				}
			}

			return null;
		}


		#region ArticleDocumentItemEntity extensions
		private static decimal? GetArticleQuantity(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;

				decimal quantity = 0;

				foreach (var articleQuantity in article.ArticleQuantities)
				{
					quantity += articleQuantity.Quantity;
				}

				return quantity;
			}

			return null;
		}

		private static FormattedText GetArticleDescription(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;
				return Helpers.ArticleDocumentItemHelper.GetArticleDescription (article, replaceTags: true, shortDescription: true);
			}

			if (line is TextDocumentItemEntity)
			{
				var text = line as TextDocumentItemEntity;
				return text.Text;
			}

			if (line is TaxDocumentItemEntity)
			{
				var tax = line as TaxDocumentItemEntity;

				if (tax.Text.IsNullOrEmpty)
				{
					return "TVA";
				}
				else
				{
					return tax.Text;
				}
			}

			if (line is SubTotalDocumentItemEntity)
			{
				return "Sous-total";
			}

			if (line is EndTotalDocumentItemEntity)
			{
				return "Grand total";
			}

			return null;
		}

		private static decimal? GetArticlePrice(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;
				return article.PrimaryLinePriceBeforeTax;
			}

			if (line is TaxDocumentItemEntity)
			{
				var tax = line as TaxDocumentItemEntity;
				return tax.ResultingTax;
			}

			if (line is SubTotalDocumentItemEntity)
			{
				var total = line as SubTotalDocumentItemEntity;
				return total.FinalPriceBeforeTax;
			}

			if (line is EndTotalDocumentItemEntity)
			{
				var total = line as EndTotalDocumentItemEntity;
				return total.PriceAfterTax;
			}

			return null;
		}
		#endregion


		private readonly DocumentMetadataEntity documentMetadataEntity;
		private readonly BusinessDocumentEntity businessDocumentEntity;

		private GlyphButton								addButton;
		private GlyphButton								removeButton;
		private CellTable								table;
	}
}
