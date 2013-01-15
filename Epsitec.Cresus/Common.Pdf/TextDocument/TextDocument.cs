//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.TextDocument
{
	public class TextDocument : CommonPdf
	{
		public TextDocument(ExportPdfInfo info, CommonSetup setup)
			: base (info, setup)
		{
			this.lineHeights = new List<double> ();
			this.linePages   = new List<int> ();
		}

		public PdfExportException GeneratePdf(string path, FormattedText text)
		{
			this.text = text;

			this.ConstantJustification ();
			this.VerticalJustification ();

			var export = new Export (this.info);
			return export.ExportToFile (path, this.pageCount, this.RenderPage);
		}

		private void RenderPage(Port port, int page)
		{
			this.RenderLayers (port, page);

			double topMargin = this.Setup.PageMargins.Top;

			//	Imprime le header au sommet de la première page.
			if (page == 1 && this.headerHeight > 0)
			{
				var box = new Rectangle (this.Setup.PageMargins.Left, this.info.PageSize.Height-this.Setup.PageMargins.Top-this.headerHeight, this.UsableWidth, this.headerHeight);
				box.Deflate (this.Setup.HeaderMargins);
				port.PaintText (box, this.Setup.HeaderText, this.Setup.TextStyle);

				topMargin += this.headerHeight;
			}

			//	Imprime le texte.
			var bounds = new Rectangle (Point.Zero, this.info.PageSize);
			bounds.Deflate (this.setup.PageMargins.Left, this.setup.PageMargins.Right, topMargin, this.setup.PageMargins.Bottom);

			int firstLine = this.linePages.IndexOf (page);
			int lastLine  = this.linePages.IndexOf (page+1) - 1;

			if (lastLine < 0)
			{
				lastLine = int.MaxValue;
			}

			double h = 0;
			for (int i=0; i<this.linePages.Count; i++)
			{
				if (this.linePages[i] >= page)
				{
					break;
				}

				h += this.lineHeights[i];
			}
			bounds.Offset (0, h);

			port.PaintText (bounds, firstLine, lastLine, this.text, this.Setup.TextStyle);

			//	Imprime le footer au bas de la dernière page.
			if (page == this.pageCount && this.footerHeight > 0)
			{
				var box = new Rectangle (this.Setup.PageMargins.Left, this.Setup.PageMargins.Bottom, this.UsableWidth, this.footerHeight);
				box.Deflate (this.Setup.FooterMargins);
				port.PaintText (box, this.Setup.FooterText, this.Setup.TextStyle);
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

		private void VerticalJustification()
		{
			this.lineHeights.Clear ();
			this.lineHeights.AddRange (Port.GetTextLineHeights (this.UsableWidth, this.text, this.Setup.TextStyle));

			//	Détermine dans quelles pages vont aller les lignes.
			this.linePages.Clear ();

			int page = 1;
			double dispo = this.UsableHeight;
			dispo -= this.headerHeight;

			for (int row = 0; row < this.lineHeights.Count; row++)
			{
				dispo -= this.lineHeights[row];

				if (dispo < 0)  // plus assez de place dans cette page ?
				{
					page++;
					dispo = this.UsableHeight;
					dispo -= this.lineHeights[row];
				}

				this.linePages.Add (page);
			}

			this.pageCount = this.linePages.Last ();

			//	Si on manque de place dans la dernière page pour le footer, on génère
			//	simplement une page de plus.
			if (dispo < this.footerHeight)
			{
				this.pageCount++;
			}
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


		private TextDocumentSetup Setup
		{
			get
			{
				return this.setup as TextDocumentSetup;
			}
		}


		private readonly List<double> lineHeights;
		private readonly List<int> linePages;

		private FormattedText text;
		private double headerHeight;
		private double footerHeight;
		private int pageCount;
	}
}
