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
			this.rowHeights = new List<double> ();
			this.rowPages   = new List<int> ();
		}

		public PdfExportException GeneratePdf(string path, int rowCount, List<ColumnDefinition> columnDefinitions, Func<int, int, FormattedText> accessor, ExportPdfInfo info, ArraySetup setup)
		{
			this.rowCount          = rowCount;
			this.columnDefinitions = columnDefinitions;
			this.accessor          = accessor;
			this.info              = info;
			this.setup             = setup;

			this.font = Font.GetFont (this.setup.FontFace, this.setup.FontStyle);

			this.VerticalJustification ();

			int pageCount = this.rowPages.Last ();

			var export = new Export (this.info);
			return export.ExportToFile (path, pageCount, this.Renderer);
		}

		private void Renderer(Port port, int page)
		{
			double y = this.info.PageSize.Height - this.setup.PageMargins.Top;

			for (int row = 0; row < this.rowCount; row++)
			{
				if (this.rowPages[row] == page)
				{
					this.RenderRow (port, row, y);
					y -= this.rowHeights[row];
				}
			}
		}

		private void RenderRow(Port port, int row, double y)
		{
			double x = this.setup.PageMargins.Left;

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				this.RenderCell (port, row, column, x, y);
				x += this.GetColumnWidth (column);
			}
		}

		private void RenderCell(Port port, int row, int column, double x, double y)
		{
			var h = this.rowHeights[row];
			var box = new Rectangle (x, y-h, this.GetColumnWidth (column), h);

			var path = new Path ();
			path.AppendRectangle (box);

			port.LineWidth = 1.0;  // épaisseur de 0.1mm
			port.Color = Color.FromBrightness (0);  // noir
			port.PaintOutline (path);

			box.Deflate (this.setup.CellMargins);
			var text = this.accessor (row, column);
			port.PaintText (box, text, this.font, this.setup.FontSize);
		}


		private void VerticalJustification()
		{
			//	Calcule les hauteurs pour toutes les lignes.
			this.rowHeights.Clear ();

			for (int row = 0; row < this.rowCount; row++)
			{
				this.rowHeights.Add (this.ComputeRowHeight (row));
			}

			//	Détermine dans quelles pages vont aller les lignes.
			this.rowPages.Clear ();

			int page = 1;
			double dispo = this.info.PageSize.Height - this.setup.PageMargins.Height;

			for (int row = 0; row < this.rowCount; row++)
			{
				dispo -= this.rowHeights[row];

				if (dispo < 0)  // plus assez de place dans cette page ?
				{
					page++;
					dispo = this.info.PageSize.Height - this.setup.PageMargins.Height;
				}

				this.rowPages.Add (page);
			}

		}

		private double ComputeRowHeight(int row)
		{
			double height = 0;

			for (int column = 0; column < this.columnDefinitions.Count; column++)
			{
				var width = this.GetColumnWidth (column) - this.setup.CellMargins.Width;
				var text = this.accessor(row, column);

				if (!text.IsNullOrEmpty ())
				{
					double h = Port.GetTextHeight (width, text, this.font, this.setup.FontSize);
					height = System.Math.Max (height, h);
				}
			}

			return height + this.setup.CellMargins.Height;
		}

		private double GetColumnWidth(int column)
		{
			var def = this.columnDefinitions[column];

			if (def.Width.HasValue)
			{
				return def.Width.Value;
			}
			else
			{
				double total = 0;
				int undefined = 0;

				foreach (var cd in this.columnDefinitions)
				{
					if (cd.Width.HasValue)
					{
						total += cd.Width.Value;
					}
					else
					{
						undefined++;
					}
				}

				//	Réparti de façon uniforme l'espace restant entre toutes les colonnes élastiques.
				return (this.info.PageSize.Width - this.setup.PageMargins.Width - total) / undefined;
			}
		}


		private readonly List<double> rowHeights;
		private readonly List<int> rowPages;

		private int rowCount;
		private List<ColumnDefinition> columnDefinitions;
		private Func<int, int, FormattedText> accessor;
		private ArraySetup setup;
		private ExportPdfInfo info;
		private Font font;
	}
}
