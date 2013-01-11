//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Array
{
	public class Array
	{
		public Array()
		{
			this.columnWidths = new List<double> ();
			this.rowHeights   = new List<double> ();
			this.rowPages     = new List<int> ();
		}

		public PdfExportException GeneratePdf(string path, int rowCount, List<ColumnDefinition> columnDefinitions, Func<int, int, FormattedText> accessor, ExportPdfInfo info, ArraySetup setup)
		{
			this.rowCount          = rowCount;
			this.columnDefinitions = columnDefinitions;
			this.accessor          = accessor;
			this.info              = info;
			this.setup             = setup;

			this.font = Font.GetFont (this.setup.FontFace, this.setup.FontStyle);

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
			double y = this.info.PageSize.Height - this.setup.PageMargins.Top;

			//	Imprime le header au sommet de la première page.
			if (page == 1 && this.headerHeight > 0)
			{
				var box = new Rectangle (this.setup.PageMargins.Left, y-this.headerHeight, this.UsableWidth, this.headerHeight);
				box.Deflate (this.setup.HeaderMargins);
				port.PaintText (box, this.setup.HeaderText, this.font, this.setup.FontSize);

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
				var box = new Rectangle (this.setup.PageMargins.Left, this.setup.PageMargins.Bottom, this.UsableWidth, this.footerHeight);
				box.Deflate (this.setup.FooterMargins);
				port.PaintText (box, this.setup.FooterText, this.font, this.setup.FontSize);
			}
		}

		private void RenderRow(Port port, int row, double y)
		{
			//	Effectue le rendu d'une ligne complète.
			double x = this.setup.PageMargins.Left;

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
			var def = this.columnDefinitions[column];
			var h   = (row == -1) ? this.labelHeight : this.rowHeights[row];
			var box = new Rectangle (x, y-h, this.columnWidths[column], h);

			var path = new Path ();
			path.AppendRectangle (box);

			var color = this.GetColor (row, column);
			if (!color.IsEmpty)
			{
				port.Color = color;
				port.PaintSurface (path);
			}

			if (this.setup.BorderThickness > 0)
			{
				port.LineWidth = this.setup.BorderThickness;
				port.Color = Color.FromBrightness (0);  // noir
				port.PaintOutline (path);
			}

			box.Deflate (this.setup.CellMargins);
			var text = this.GetText (row, column);
			if (!text.IsNullOrEmpty ())
			{
				var style = new TextStyle()
				{
					Alignment = def.Alignment,
				};

				port.PaintText (box, text, this.font, this.setup.FontSize, style);
			}
		}


		private void ConstantJustification()
		{
			//	Calcule les hauteurs de tous les éléments fixes.
			this.headerHeight = Port.GetTextHeight (this.UsableWidth, this.setup.HeaderText, this.font, this.setup.FontSize);
			this.footerHeight = Port.GetTextHeight (this.UsableWidth, this.setup.FooterText, this.font, this.setup.FontSize);

			if (this.headerHeight > 0)
			{
				this.headerHeight += this.setup.HeaderMargins.Height;
			}

			if (this.footerHeight > 0)
			{
				this.footerHeight += this.setup.FooterMargins.Height;
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
					this.columnWidths[column] = residual / def.StretchFactor;
				}
			}
		}

		private double ComputeColomnWidth(int column)
		{
			//	Calcule la largeur nécessaire pour une colonne, selon le contenu
			//	le plus large.
			double width = 0;

			for (int row = -1; row < this.rowCount; row++)  // -1 -> tient compte de la ligne des labels
			{
				var text = this.GetText (row, column);

				if (!text.IsNullOrEmpty ())
				{
					double w = Port.GetTextSingleLineSize (text, this.font, this.setup.FontSize).Width;
					w += this.setup.CellMargins.Width;
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
			double height = 0;

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				var width = this.columnWidths[column] - this.setup.CellMargins.Width;
				var text = this.GetText (row, column);

				if (!text.IsNullOrEmpty ())
				{
					double h = Port.GetTextHeight (width, text, this.font, this.setup.FontSize);
					height = System.Math.Max (height, h);
				}
			}

			return height + this.setup.CellMargins.Height;
		}

		private double UsableWidth
		{
			get
			{
				return this.info.PageSize.Width - this.setup.PageMargins.Width;
			}
		}

		private double UsableHeight
		{
			get
			{
				return this.info.PageSize.Height - this.setup.PageMargins.Height;
			}
		}

		private Color GetColor(int row, int column)
		{
			if (row == -1)  // label ?
			{
				return this.setup.LabelBackgroundColor;
			}
			else
			{
				if (row % 2 == 0)
				{
					return this.setup.EvenBackgroundColor;
				}
				else
				{
					return this.setup.OddBackgroundColor;
				}
			}
		}

		private FormattedText GetText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			//	La ligne -1 correspond aux labels des colonnes.
			if (row == -1)  // label ?
			{
				return this.columnDefinitions[column].Title;
			}
			else
			{
				return this.accessor(row, column);
			}
		}


		private readonly List<double> columnWidths;
		private readonly List<double> rowHeights;
		private readonly List<int> rowPages;

		private int rowCount;
		private List<ColumnDefinition> columnDefinitions;
		private Func<int, int, FormattedText> accessor;
		private ArraySetup setup;
		private ExportPdfInfo info;
		private Font font;
		private double labelHeight;
		private double headerHeight;
		private double footerHeight;
		private int pageCount;
	}
}
