//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class LineTableController
	{
		public LineTableController(AccessData accessData)
		{
			this.accessData = accessData;

			this.colorIndexes = new List<int> ();
		}


		public bool								HasSelection
		{
			get
			{
				return this.Selection.Count != 0;
			}
		}

		public bool								HasSingleSelection
		{
			get
			{
				return this.Selection.Count == 1;
			}
		}

		public bool								HasMultiSelection
		{
			get
			{
				return this.Selection.Count > 1;
			}
		}

		public int?								LastSelection
		{
			get
			{
				return this.Selection.LastOrDefault ();
			}
		}

		public IList<int>						Selection
		{
			// Le getter retourne la liste des lignes sélectionnées.
			// Le setter sélectionne les lignes données dans la liste.
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
			set
			{
				bool existing = false;
				bool changing = false;

				for (int row = 0; row < this.table.Rows; row++)
				{
					bool newSel = (value == null) ? false : value.Contains (row);
					bool oldSel = this.table.IsCellSelected (row, 0);

					if (newSel)
					{
						existing = true;
					}

					if (newSel != oldSel)
					{
						changing = true;
					}

					this.table.SelectRow (row, newSel);
				}

				if (existing)
				{
					this.table.ShowSelect ();  // montre la sélection
				}

				if (changing)
				{
					this.OnSelectionChanged ();
				}
			}
		}

		private IEnumerable<ColumnType>			ColumnTypes
		{
			//	Retourne les colonnes visibles dans la table, de gauche à droite.
			get
			{
				if (this.viewMode == ViewMode.Compact)
				{
					yield return ColumnType.Group;

					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.AdditionalType;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.Total;
				}
				else if (this.viewMode == ViewMode.Default)
				{
					yield return ColumnType.Group;

					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.AdditionalType;
					yield return ColumnType.AdditionalDate;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.Discount;
					yield return ColumnType.LinePrice;
					yield return ColumnType.Vat;
					yield return ColumnType.Total;
				}
				else if (this.viewMode == ViewMode.Full)
				{
					yield return ColumnType.Group;
					yield return ColumnType.GroupNumber;

					yield return ColumnType.ArticleId;
					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.AdditionalType;
					yield return ColumnType.AdditionalDate;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.Discount;
					yield return ColumnType.LinePrice;
					yield return ColumnType.Vat;
					yield return ColumnType.Total;
				}
				else if (this.viewMode == ViewMode.Debug)
				{
					yield return ColumnType.Group;

					yield return ColumnType.GroupIndex;
					yield return ColumnType.GroupNumber;
					yield return ColumnType.LineNumber;
					yield return ColumnType.FullNumber;

					yield return ColumnType.ArticleId;
					yield return ColumnType.ArticleDescription;

					yield return ColumnType.QuantityAndUnit;
					yield return ColumnType.AdditionalType;
					yield return ColumnType.AdditionalDate;

					yield return ColumnType.UnitPrice;
					yield return ColumnType.Discount;
					yield return ColumnType.LinePrice;
					yield return ColumnType.Vat;
					yield return ColumnType.Total;
				}
			}
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
				ColumnsToSkipFromLeftForSeparator = 1,
				IsCompactStyle = true,
				DrawHiliteFocus = false,
				DefHeight = LineTableController.LineHeight,
				Margins = new Margins (2),
				Dock = DockStyle.Fill,
			};

			this.table.FinalSelectionChanged += delegate
			{
				this.OnSelectionChanged();
			};
		}

		public void UpdateUI(ILineProvider provider)
		{
			this.lineProvider        = provider;
			this.viewMode            = provider.CurrentViewMode;
			this.editMode            = provider.CurrentEditMode;
			this.lineCount           = provider.Count;

			int columns = this.ColumnTypes.Count ();

			this.table.SetArraySize (columns, this.lineCount);

			int column = 0;
			foreach (var columnType in this.ColumnTypes)
			{
				this.table.SetWidthColumn (column, this.GetColumnWidth (columnType));
				this.table.SetHeaderTextH (column, this.GetColumnName (columnType));

				column++;
			}

			if (this.table.Rows > 0)
			{
				this.table[0, 0].HasRightSeparator = false;
			}

			this.lastDocumentItemEntity = null;
			this.documentItemIndex = -1;

			this.colorIndexes.Clear ();

			for (int row=0; row<this.lineCount; row++)
			{
				this.TableFillRow (row);
				this.TableUpdateRow (row);
			}
		}

		public void DeselectAll()
		{
			this.Selection = null;
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
							PreferredHeight = LineTableController.LineHeight,
							ContentAlignment = this.GetRowColumnContentAlignment (row, columnType),
							TextBreakMode = Common.Drawing.TextBreakMode.SingleLine | TextBreakMode.Split | TextBreakMode.Ellipsis,
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
			var info = this.lineProvider.GetLine (row);

			if (this.lastDocumentItemEntity != info.DocumentItem)
			{
				this.lastDocumentItemEntity = info.DocumentItem;
				this.documentItemIndex++;
			}

			if (info.DocumentItem is TextDocumentItemEntity)
			{
				var t = info.DocumentItem as TextDocumentItemEntity;
				if (t.Text == "a")
				{
				}
			}


			//	Détermine s'il faut dessiner la ligne horizontale de séparation.
			bool separator = true;

			if (row < this.table.Rows-1)
			{
				var nextInfo = this.lineProvider.GetLine (row+1);

				if (info.SublineIndex == nextInfo.SublineIndex-1)  // est-ce que la ligne suivante fait partie de la même entité ?
				{
					separator = false;
				}
			}

			this.table[0, row].HasBottomSeparator = separator;

			bool selection = this.table.IsCellSelected (row, 0);

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
							this.UpdateIndexDisplayerWidget (displayer, row, info.DocumentItem.GroupIndex);
						}
					}
				}
				else
				{
					var color = this.GetNiceBackgroundColor (info.DocumentItem.GroupIndex);

					if (this.table[column, row].Children.Count != 0)
					{
						var text = this.table[column, row].Children[0] as StaticText;

						if (text != null)
						{
							var cellContent = this.lineProvider.GetCellContent (row, columnType);

							text.FormattedText = cellContent.Text.ToSimpleText ();

							if (cellContent.Error != Library.Business.ContentAccessors.DocumentItemAccessorError.None)
							{
								color = Color.FromName ("Gold");
							}
						}
					}

					this.table[column, row].BackColor = color;
				}

				column++;
			}

			this.table.SelectRow (row, selection);
		}

		private void UpdateIndexDisplayerWidget(IndexDisplayerWidget displayer, int row, int groupIndex)
		{
			displayer.DrawTopSeparator = (row == 0) ? false : this.table[0, row-1].HasBottomSeparator;

			//	Initialise les couleurs à utiliser.
			displayer.Colors.Clear ();

			int level = AbstractDocumentItemEntity.GetGroupLevel (groupIndex);

			if (level == 0)
			{
				var color = this.GetNiceBackgroundColor (0);
				displayer.Colors.Add (color);
			}
			else
			{
				for (int i = 0; i < level; i++)
				{
					int partialGroupIndex = 0;

					for (int j = 0; j <= i; j++)
					{
						int rank = AbstractDocumentItemEntity.GroupExtract (groupIndex, j);
						partialGroupIndex = AbstractDocumentItemEntity.GroupReplace (partialGroupIndex, j, rank);
					}

					var color = this.GetNiceBackgroundColor (partialGroupIndex);
					displayer.Colors.Add (color);
				}
			}

			//	Initialise les GroupIndex précédent, courant et suivant.
			displayer.CurrentGroupIndex = groupIndex;

			if (row == 0)
			{
				displayer.TopGroupIndex = 0;
			}
			else
			{
				var info = this.lineProvider.GetLine (row-1);
				displayer.TopGroupIndex = info.DocumentItem.GroupIndex;
			}

			if (row >= this.lineCount-1)
			{
				displayer.BottomGroupIndex = 0;
			}
			else
			{
				var info = this.lineProvider.GetLine (row+1);
				displayer.BottomGroupIndex = info.DocumentItem.GroupIndex;
			}

			displayer.Invalidate ();
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
			else if (this.viewMode == ViewMode.Full)
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
			else
			{
				switch (columnType)
				{
					case ColumnType.ArticleId:
						return 50;

					case ColumnType.ArticleDescription:
						return 200;

					case ColumnType.QuantityAndUnit:
						return 80;
				}
			}

			switch (columnType)
			{
				case ColumnType.Group:
					return 40;

				case ColumnType.GroupNumber:
					return 60;

				case ColumnType.LineNumber:
					return 30;

				case ColumnType.FullNumber:
					return 60;

				case ColumnType.GroupIndex:
					return 70;

				case ColumnType.AdditionalType:
					return 65;

				case ColumnType.AdditionalDate:
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

				case ColumnType.GroupNumber:
					return "Groupe";

				case ColumnType.LineNumber:
					return "N°";

				case ColumnType.FullNumber:
					return "Complet";

				case ColumnType.QuantityAndUnit:
					return "Quantité";

				case ColumnType.AdditionalType:
					return "Type";

				case ColumnType.AdditionalDate:
					return "Date";

				case ColumnType.ArticleId:
					return "Article";

				case ColumnType.ArticleDescription:
					return (this.editMode == EditMode.Name) ? "Désignation courte" : "Désignation longue";

				case ColumnType.Discount:
					return "Rabais";

				case ColumnType.UnitPrice:
					return this.accessData.IsExcludingTax ? "p.u. HT" : "p.u. TTC";

				case ColumnType.LinePrice:
					return this.accessData.IsExcludingTax ? "Prix HT" : "Prix TTC";

				case ColumnType.Vat:
					return "TVA";

				case ColumnType.Total:
					return "Total";

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
				return Color.FromBrightness (0.75);  // gris
			}

			int index = this.colorIndexes.IndexOf (groupIndex);

			if (index == -1)  // couleur pas encore attribuée ?
			{
				this.colorIndexes.Add (groupIndex);
				index = this.colorIndexes.Count-1;
			}

			return LineTableController.niceBackgroundColors[index % LineTableController.niceBackgroundColors.Length];
		}

		#region Background Colors

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
#endif
#if false
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
#if false
			//	Gris clairs en dégradés.
			Color.FromBrightness (0.95),
			Color.FromBrightness (0.90),
			Color.FromBrightness (0.85),
			Color.FromBrightness (0.80),
			Color.FromBrightness (0.85),
			Color.FromBrightness (0.90),
			Color.FromBrightness (0.95),
			Color.FromBrightness (1.00),
#endif
#if true
			//	Gris clairs en dégradés.
			Color.FromBrightness (0.94),
			Color.FromBrightness (0.89),
			Color.FromBrightness (0.84),
			Color.FromBrightness (0.80),
			Color.FromBrightness (0.86),
			Color.FromBrightness (0.91),
			Color.FromBrightness (0.96),
			Color.FromBrightness (1.00),
#endif
		};

		#endregion

		private void OnSelectionChanged()
		{
			this.SelectionChanged.Raise (this);
		}
		
		
		public event EventHandler				SelectionChanged;
	
		
		private static readonly double			LineHeight = 17;

		private readonly AccessData				accessData;
		private readonly List<int>				colorIndexes;

		private CellTable						table;
		private ILineProvider					lineProvider;
		private ViewMode						viewMode;
		private EditMode						editMode;
		private int								lineCount;

		private AbstractDocumentItemEntity		lastDocumentItemEntity;
		private int								documentItemIndex;
	}
}
