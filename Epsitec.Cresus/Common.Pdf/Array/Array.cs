//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Array
{
	public class Array : CommonPdf
	{
		public Array(ExportPdfInfo info, CommonSetup setup)
			: base (info, setup)
		{
			this.columnWidths = new List<double> ();
			this.rowHeights   = new List<double> ();
			this.rowPages     = new List<int> ();
		}

		public PdfExportException GeneratePdf(string path, int rowCount, List<ColumnDefinition> columnDefinitions, Func<int, int, CellContent> dataAccessor)
		{
			this.rowCount          = rowCount;
			this.columnDefinitions = columnDefinitions;
			this.dataAccessor      = dataAccessor;

			this.ConstantJustification ();
			this.HorizontalJustification ();
			this.VerticalJustification ();

			var ex = this.HasException ();
			if (ex != null)
			{
				return ex;
			}

			var export = new Export (this.info);
			return export.ExportToFile (path, this.pageCount, this.RenderPage);
		}


		private PdfExportException HasException()
		{
			//	Détecte tous les cas dégénérés exceptionnels.
			if (this.rowCount == 0)
			{
				return new PdfExportException ("Aucun contenu");
			}

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				if (this.columnWidths[column] < 0)
				{
					return new PdfExportException ("Débordement d'une cellule");
				}
			}

			return null;  // ok
		}


		private void RenderPage(Port port, int page)
		{
			//	Effectue le rendu d'une page complète.
			this.RenderLayers (port, page);

			double y = this.info.PageSize.Height - this.Setup.PageMargins.Top;

			//	Imprime le header au sommet de la première page.
			if (page == 1 && this.headerHeight > 0)
			{
				var box = new Rectangle (this.Setup.PageMargins.Left, y-this.headerHeight, this.UsableWidth, this.headerHeight);
				box.Deflate (this.Setup.HeaderMargins);
				port.PaintText (box, this.Setup.HeaderText, this.Setup.TextStyle);

				y -= this.headerHeight;
			}

			//	Imprime la ligne de labels du tableau.
			this.RenderRow (port, -1, y);
			y -= this.labelHeight;

			//	Imprime le tableau.
			for (int row = 0; row < this.rowCount; row++)
			{
				if (this.rowPages[row] == page)  // ligne dans cette page ?
				{
					this.RenderRow (port, row, y);
					y -= this.rowHeights[row];
				}
			}

			//	Imprime le footer au bas de la dernière page.
			if (page == this.pageCount && this.footerHeight > 0)
			{
				var box = new Rectangle (this.Setup.PageMargins.Left, this.Setup.PageMargins.Bottom, this.UsableWidth, this.footerHeight);
				box.Deflate (this.Setup.FooterMargins);
				port.PaintText (box, this.Setup.FooterText, this.Setup.TextStyle);
			}
		}

		private void RenderRow(Port port, int row, double y)
		{
			//	Effectue le rendu d'une ligne complète.
			//	La ligne -1 correspond aux labels des colonnes.
			double x = this.Setup.PageMargins.Left;

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				if (this.columnWidths[column] > 0)
				{
					this.RenderCell (port, row, column, x, y);
					x += this.columnWidths[column];
				}
			}
		}

		private void RenderCell(Port port, int row, int column, double x, double y)
		{
			//	Effectue le rendu d'une cellule.
			//	La ligne -1 correspond aux labels des colonnes.
			var def = this.columnDefinitions[column];
			var h   = (row == -1) ? this.labelHeight : this.rowHeights[row];
			var box = new Rectangle (x, y-h, this.columnWidths[column], h);

			var path = new Path ();
			path.AppendRectangle (box);

			var content = this.GetCellContent (row, column);

			//	Dessine le fond.
			if (!content.BackgroundColor.IsEmpty)
			{
				port.Color = content.BackgroundColor;
				port.PaintSurface (path);
			}

			//	Dessine le cadre.
			if (this.Setup.BorderThickness > 0)
			{
				port.LineWidth = this.Setup.BorderThickness;
				port.Color = Color.FromBrightness (0);  // noir
				port.PaintOutline (path);
			}

			//	Dessine le texte contenu.
			if (!content.Text.IsNullOrEmpty ())
			{
				box.Deflate (this.Setup.CellMargins);

				var style = new TextStyle (this.Setup.TextStyle)
				{
					Alignment = def.Alignment,
				};

				port.PaintText (box, content.Text, style);
			}
		}


		private void ConstantJustification()
		{
			//	Calcule les hauteurs des éléments fixes (header et footer).
			this.headerHeight = Port.GetTextHeight (this.UsableWidth, this.Setup.HeaderText, this.Setup.TextStyle);
			this.footerHeight = Port.GetTextHeight (this.UsableWidth, this.Setup.FooterText, this.Setup.TextStyle);

			if (this.headerHeight > 0)
			{
				this.headerHeight += this.Setup.HeaderMargins.Height;
			}

			if (this.footerHeight > 0)
			{
				this.footerHeight += this.Setup.FooterMargins.Height;
			}
		}

		private void HorizontalJustification()
		{
			//	Calcule les largeurs des colonnes.
			this.columnWidths.Clear ();

			//	Calcule les largeurs des colonnes absolues et automatiques.
			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				var def = this.columnDefinitions[column];
				double width;

				if (def.ColumnType == ColumnType.Absolute)
				{
					width = def.AbsoluteWidth;
				}
				else if (def.ColumnType == ColumnType.Stretch)
				{
					width = 0;
				}
				else
				{
					width = this.ComputeColomnWidth(column);
				}

				this.columnWidths.Add (width);
			}

			//	Calcule les largeurs des colonnes "stretch".
			double totalWidthUsed = 0;
			double totalStretch = 0;

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				var def = this.columnDefinitions[column];

				if (def.ColumnType == ColumnType.Stretch)
				{
					totalStretch += def.StretchFactor;
				}
				else
				{
					totalWidthUsed += this.columnWidths[column];
				}
			}

			//	Réparti de façon uniforme l'espace restant entre toutes les colonnes élastiques.
			double residual = this.UsableWidth - totalWidthUsed;

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				var def = this.columnDefinitions[column];

				if (def.ColumnType == ColumnType.Stretch && def.StretchFactor != 0)
				{
					this.columnWidths[column] = residual * (def.StretchFactor / totalStretch);
				}
			}
		}

		private double ComputeColomnWidth(int column)
		{
			//	Calcule la largeur nécessaire pour une colonne, selon le contenu
			//	le plus large (mode ColumnType.Automatic).
			double width = 0;

			for (int row = -1; row < this.rowCount; row++)  // -1 -> tient compte de la ligne des labels
			{
				var text = this.GetCellContent (row, column).Text;

				if (!text.IsNullOrEmpty ())
				{
					double w = Port.GetTextSingleLineSize (text, this.Setup.TextStyle).Width;
					w += this.Setup.CellMargins.Width;
					width = System.Math.Max (width, w);
				}
			}

			return width;
		}


		private void VerticalJustification()
		{
			//	Calcule les hauteurs pour toutes les lignes.
			this.rowHeights.Clear ();

			this.labelHeight = this.ComputeRowHeight (-1);

			for (int row = 0; row < this.rowCount; row++)
			{
				this.rowHeights.Add (this.ComputeRowHeight (row));
			}

			//	Détermine dans quelles pages vont aller les lignes.
			this.rowPages.Clear ();

			int page = 1;
			double dispo = this.UsableHeight;
			dispo -= this.headerHeight;
			dispo -= this.labelHeight;

			for (int row = 0; row < this.rowCount; row++)
			{
				dispo -= this.rowHeights[row];

				if (dispo < 0)  // plus assez de place dans cette page ?
				{
					page++;
					dispo = this.UsableHeight;
					dispo -= this.labelHeight;

					dispo -= this.rowHeights[row];
				}

				this.rowPages.Add (page);
			}

			this.pageCount = this.rowPages.Last ();

			//	Si on manque de place dans la dernière page pour le footer, on génère
			//	simplement une page de plus.
			if (dispo < this.footerHeight)
			{
				this.pageCount++;
			}
		}

		private double ComputeRowHeight(int row)
		{
			//	Calcule la hauteur nécessaire pour une ligne, en fonction de la
			//	colonne la plus haute.
			//	La ligne -1 correspond aux labels des colonnes.
			double height = 0;

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				var width = this.columnWidths[column] - this.Setup.CellMargins.Width;
				var text = this.GetCellContent (row, column).Text;

				if (!text.IsNullOrEmpty ())
				{
					double h = Port.GetTextHeight (width, text, this.Setup.TextStyle);
					height = System.Math.Max (height, h);
				}
			}

			return height + this.Setup.CellMargins.Height;
		}


		private double UsableWidth
		{
			//	Retourne la largeur utilisable dans une page.
			get
			{
				return this.info.PageSize.Width - this.Setup.PageMargins.Width;
			}
		}

		private double UsableHeight
		{
			//	Retourne la hauteur utilisable dans une page.
			get
			{
				return this.info.PageSize.Height - this.Setup.PageMargins.Height;
			}
		}


		private CellContent GetCellContent(int row, int column)
		{
			//	Retourne le contenu d'une cellule.
			//	La ligne -1 correspond aux labels des colonnes.
			if (row == -1)  // label ?
			{
				return new CellContent (this.columnDefinitions[column].Title, this.Setup.LabelBackgroundColor);
			}
			else
			{
				var content = this.dataAccessor(row, column);

				if (content == null)
				{
					return new CellContent ("");
				}

				var color = content.BackgroundColor;

				if (color.IsEmpty)
				{
					if (row % 2 == 0)
					{
						color = this.Setup.EvenBackgroundColor;
					}
					else
					{
						color = this.Setup.OddBackgroundColor;
					}
				}

				return new CellContent (content.Text, color);
			}
		}


		private ArraySetup Setup
		{
			get
			{
				return this.setup as ArraySetup;
			}
		}


		private readonly List<double> columnWidths;
		private readonly List<double> rowHeights;
		private readonly List<int> rowPages;

		private int rowCount;
		private List<ColumnDefinition> columnDefinitions;
		private Func<int, int, CellContent> dataAccessor;
		private double labelHeight;
		private double headerHeight;
		private double footerHeight;
		private int pageCount;
	}
}
