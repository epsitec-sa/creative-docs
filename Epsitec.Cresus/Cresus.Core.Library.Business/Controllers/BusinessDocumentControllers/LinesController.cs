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
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	/// <summary>
	/// Liste de lignes d'articles (AbstractDocumentItemEntity).
	/// </summary>
	public class LinesController
	{
		public LinesController(AccessData accessData)
		{
			this.accessData = accessData;

			this.colorIndexes = new List<int> ();
		}


		public void CreateUI(Widget parent, System.Func<bool> selectionChanged)
		{
			this.selectionChanged = selectionChanged;

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
				IsCompactStyle = true,
				DefHeight = LinesController.lineHeight,
				Margins = new Margins (2),
				Dock = DockStyle.Fill,
			};

			this.table.SelectionChanged += delegate
			{
				this.selectionChanged();
			};
		}

		public void UpdateUI(ViewMode viewMode, EditMode editMode, int lineCount, System.Func<int, LineInformations> getLineInformations, System.Func<int, ColumnType, FormattedText> getCellContent, int? sel = null)
		{
			this.viewMode            = viewMode;
			this.editMode            = editMode;
			this.getLineInformations = getLineInformations;
			this.getCellContent      = getCellContent;

			int columns = this.ColumnTypes.Count ();

			this.table.SetArraySize (columns, lineCount);

			int column = 0;
			foreach (var columnType in this.ColumnTypes)
			{
				this.table.SetWidthColumn (column, this.GetColumnWidth (columnType));
				this.table.SetHeaderTextH (column, this.GetColumnName (columnType));

				column++;
			}

			this.table[0, 0].HasRightSeparator = false;

			if (sel == null)
			{
				sel = this.table.SelectedRow;
			}

			this.lastDocumentItemEntity = null;
			this.documentItemIndex = -1;

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

			this.selectionChanged ();
		}


		public bool HasSelection
		{
			get
			{
				return this.Selection.Count != 0;
			}
		}

		public bool HasSingleSelection
		{
			get
			{
				return this.Selection.Count == 1;
			}
		}

		public bool HasMultiSelection
		{
			get
			{
				return this.Selection.Count > 1;
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
					if (this.table[column, row].IsEmpty)
					{
						var displayer = new IndexDisplayerWidget
						{
							Dock = DockStyle.Fill,
						};

						this.table[column, row].Insert (displayer);
					}
				}
				else
				{
					if (this.table[column, row].IsEmpty)
					{
						var text = new StaticText
						{
							PreferredHeight = LinesController.lineHeight,
							ContentAlignment = this.GetRowColumnContentAlignment (row, columnType),
							Dock = DockStyle.Fill,
							Margins = new Margins (4, 4, 0, 0),
						};

						this.table[column, row].Insert (text);
					}
					else
					{
						var text = this.table[column, row].Children[0] as StaticText;
						text.ContentAlignment = this.GetRowColumnContentAlignment (row, columnType);
					}
				}

				column++;
			}
		}

		private void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			var info = this.getLineInformations (row);

			if (this.lastDocumentItemEntity != info.AbstractDocumentItemEntity)
			{
				this.lastDocumentItemEntity = info.AbstractDocumentItemEntity;
				this.documentItemIndex++;
			}

			//	Détermine s'il faut dessiner la ligne horizontale de séparation.
			bool separator = true;

			if (row < this.table.Rows-1)
			{
				var nextInfo = this.getLineInformations (row+1);

				if (info.SublineIndex == nextInfo.SublineIndex-1)  // est-ce que la ligne suivante fait partie de la même entité ?
				{
					separator = false;
				}
			}

			this.table[0, row].HasBottomSeparator = separator;

			//	Met à jour les contenus des cellules.
			int column = 0;
			foreach (var columnType in this.ColumnTypes)
			{
				if (columnType == ColumnType.Group)
				{
					if (this.table[column, row].Children.Count != 0)
					{
						var displayer = this.table[column, row].Children[0] as IndexDisplayerWidget;

						if (displayer != null)
						{
							this.UpdateIndexDisplayerWidget (displayer, info.AbstractDocumentItemEntity.GroupIndex);
						}
					}
				}
				else
				{
					if (this.table[column, row].Children.Count != 0)
					{
						var text = this.table[column, row].Children[0] as StaticText;

						if (text != null)
						{
							text.FormattedText = this.getCellContent (row, columnType).ToSimpleText ();
						}
					}

#if false
					if (info.AbstractDocumentItemEntity.Attributes.HasFlag (DocumentItemAttributes.ProFormaOnly))
					{
						this.table[column, row].BackColor = Color.FromBrightness (0.70);
					}
					else if (info.AbstractDocumentItemEntity.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated))
					{
						this.table[column, row].BackColor = Color.FromBrightness (0.95);
					}
					else
					{
						var color = this.GetNiceBackgroundColor (info.AbstractDocumentItemEntity.GroupIndex);
						this.table[column, row].BackColor = color;
					}
#else
					var color = this.GetNiceBackgroundColor (info.AbstractDocumentItemEntity.GroupIndex);
					this.table[column, row].BackColor = color;
#endif
				}

				column++;
			}

			this.table.SelectRow (row, false);
		}

		private void UpdateIndexDisplayerWidget(IndexDisplayerWidget displayer, int groupIndex)
		{
			displayer.Colors.Clear ();

			var list = BusinessDocumentLinesController.GroupIndexSplit (groupIndex);

			if (list.Count == 0)
			{
				var color = this.GetNiceBackgroundColor (0);
				displayer.Colors.Add (color);
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
                {
					var partialList = new List<int> ();

					for (int j = 0; j <= i; j++)
					{
						partialList.Add (list[j]);
					}

					int groupLevel = BusinessDocumentLinesController.GroupIndexCombine (partialList);
					var color = this.GetNiceBackgroundColor (groupLevel);
					displayer.Colors.Add (color);
				}
			}

			displayer.Invalidate ();
		}


		private IEnumerable<ColumnType> ColumnTypes
		{
			//	Retourne les colonnes visibles dans la table, de gauche à droite.
			get
			{
				if (this.viewMode == ViewMode.Compact)
				{
					yield return ColumnType.Group;

					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.Type;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.Total;
				}
				else if (this.viewMode == ViewMode.Default)
				{
					yield return ColumnType.Group;

					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.Type;
					yield return ColumnType.Date;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.Discount;
					yield return ColumnType.LinePrice;
					yield return ColumnType.Vat;
					yield return ColumnType.Total;

					yield return ColumnType.GroupIndex;  // TODO: provisoire, pour le debug
				}
				else
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
			}
		}

		private int GetColumnWidth(ColumnType columnType)
		{
			if (this.viewMode == ViewMode.Compact)
			{
				switch (columnType)
				{
					case ColumnType.ArticleDescription:
						return 416;

					case ColumnType.QuantityAndUnit:
						return 80;
				}
			}
			else if (this.viewMode == ViewMode.Default)
			{
				switch (columnType)
				{
					case ColumnType.ArticleId:
						return 50;

					case ColumnType.ArticleDescription:
						return 186;

					case ColumnType.QuantityAndUnit:
						return 60;
				}
			}
			else
			{
				switch (columnType)
				{
					case ColumnType.ArticleId:
						return 80;

					case ColumnType.ArticleDescription:
						return 250;

					case ColumnType.QuantityAndUnit:
						return 80;
				}
			}

			switch (columnType)
			{
				case ColumnType.Group:
					return 40;

				case ColumnType.GroupIndex:
					return 60;

				case ColumnType.Type:
					return 65;

				case ColumnType.Date:
					return 70;

				case ColumnType.UnitPrice:
				case ColumnType.Discount:
				case ColumnType.LinePrice:
				case ColumnType.Vat:
				case ColumnType.Total:
					return 60;

				default:
					return 100;
			}
		}

		private string GetColumnName(ColumnType columnType)
		{
			switch (columnType)
			{
				case ColumnType.GroupIndex:
					return "Debug";

				case ColumnType.QuantityAndUnit:
					return "Quantité";

				case ColumnType.Type:
					return "Type";

				case ColumnType.Date:
					return "Date";

				case ColumnType.ArticleId:
					return "Article";

				case ColumnType.ArticleDescription:
					return (this.editMode == EditMode.InternalDescription) ? "Désignation interne" : "Désignation pour le client";

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
			if (columnType == ColumnType.GroupIndex ||
				columnType == ColumnType.QuantityAndUnit ||
				columnType == ColumnType.Discount ||
				columnType == ColumnType.UnitPrice ||
				columnType == ColumnType.LinePrice ||
				columnType == ColumnType.Vat ||
				columnType == ColumnType.Total)
			{
				return ContentAlignment.MiddleRight;  // les chiffres sont alignés à droite
			}

			return ContentAlignment.MiddleLeft;
		}


		private Color GetNiceBackgroundColor(int groupIndex)
		{
			//	Attribue une couleur spécifique unique à chaque groupe, dans la mesure
			//	du possible (8 couleurs pastels sont disponibles). Une fois l'attribution
			//	effectuée, ne change plus, afin d'éviter qu'un groupe existant change
			//	brusquement parce qu'on a modifié un autre groupe ailleurs.
			if (groupIndex == 0)
			{
				return Color.FromBrightness (0.95);  // gris très clair
			}

			int index = this.colorIndexes.IndexOf (groupIndex);

			if (index == -1)  // couleur pas encore attribuée ?
			{
				this.colorIndexes.Add (groupIndex);
				index = this.colorIndexes.Count-1;
			}

			return LinesController.niceBackgroundColors[index % LinesController.niceBackgroundColors.Length];
		}

		private static readonly Color[] niceBackgroundColors =
		{
#if false
			//	Arc-en-ciel de couleurs pastels, bleu-violet-rouge-jaune-vert.
			Color.FromHsv (180.0, 0.12, 1.0),
			Color.FromHsv (216.0, 0.12, 1.0),
			Color.FromHsv (252.0, 0.12, 1.0),
			Color.FromHsv (288.0, 0.12, 1.0),
			Color.FromHsv (  0.0, 0.12, 1.0),
			Color.FromHsv ( 36.0, 0.12, 1.0),
			Color.FromHsv ( 80.0, 0.12, 1.0),
			Color.FromHsv (120.0, 0.12, 1.0),
#else
			//	Couleurs pastels disjointes.
			Color.FromHsv (180.0, 0.12, 1.0),
			Color.FromHsv (  0.0, 0.12, 1.0),
			Color.FromHsv (120.0, 0.12, 1.0),
			Color.FromHsv (216.0, 0.12, 1.0),
			Color.FromHsv ( 36.0, 0.12, 1.0),
			Color.FromHsv (252.0, 0.12, 1.0),
			Color.FromHsv ( 80.0, 0.12, 1.0),
			Color.FromHsv (288.0, 0.12, 1.0),
#endif
		};

	
		private static readonly double lineHeight = 17;

		private readonly AccessData								accessData;
		private readonly List<int>								colorIndexes;

		private CellTable										table;
		private System.Func<bool>								selectionChanged;
		private System.Func<int, LineInformations>				getLineInformations;
		private System.Func<int, ColumnType, FormattedText>		getCellContent;
		private ViewMode										viewMode;
		private EditMode										editMode;

		private AbstractDocumentItemEntity						lastDocumentItemEntity;
		private int												documentItemIndex;
	}
}
