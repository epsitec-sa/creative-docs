//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Samuel LOUP

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Common.Pdf.LetterDocument
{
	public class LetterDocument : CommonPdf
	{
		public LetterDocument(ExportPdfInfo info, CommonSetup setup)
			: base (info, setup)
		{
			this.lineHeights = new List<double> ();
			this.linePages   = new List<int> ();
			this.Setup.TextStyle.Alignment = ContentAlignment.TopLeft;
			this.Setup.TextStyle.FontSize = 42.0;
		}

		public void GeneratePdf(string path, FormattedText topReference, FormattedText recipientAddress, FormattedText content)
		{
			using (var stream = File.Open (path, FileMode.Create))
			{
				this.GeneratePdf (stream, topReference, recipientAddress, content);
			}
		}

		public void GeneratePdf(Stream stream, FormattedText topReference, FormattedText recipientAddress, FormattedText content)
		{
			this.content = content;

			this.ConstantJustification ();
			this.VerticalJustification ();


			this.AddTopLeftLayer (topReference, 200, 200, this.Setup.TextStyle);
			this.AddCustomLayer (recipientAddress, new Margins (1400.0, 0.0, 500.0, 0.0), this.Setup.TextStyle);
	
			var export = new Export (this.info);
			export.ExportToFile (stream, this.pageCount, this.RenderPage);
		}

		private void RenderPage(Port port, int page)
		{
			this.RenderLayers (port, page);


			double topMargin = 200.0;

			if (page == 1)
			{
				topMargin = 1000.0;
			}

			var clipRect = new Rectangle (Point.Zero, this.info.PageSize);
			clipRect.Deflate (this.setup.PageMargins.Left, this.setup.PageMargins.Right, topMargin, this.setup.PageMargins.Bottom);

			double h = 0;
			for (int i=0; i<this.linePages.Count; i++)
			{
				if (this.linePages[i] >= page)
				{
					break;
				}

				h += this.lineHeights[i];
			}

			var bounds = new Rectangle (clipRect.Left, clipRect.Bottom, clipRect.Width, clipRect.Height + h);

			port.PaintText (bounds, clipRect, this.content, this.Setup.TextStyle);
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
			this.lineHeights.AddRange (Port.GetTextLineHeights (this.UsableWidth, this.content, this.Setup.TextStyle));

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


		private LetterDocumentSetup Setup
		{
			get
			{
				return this.setup as LetterDocumentSetup;
			}
		}


		private readonly List<double> lineHeights;
		private readonly List<int> linePages;

		private FormattedText		content;

		private double headerHeight = 200;
		private double footerHeight;
		private int pageCount;
	}
}
