//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Samuel LOUP

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Common.Pdf.TextDocument
{
	public class ListingDocument : CommonPdf
	{
		public ListingDocument(ExportPdfInfo info, CommonSetup setup)
			: base (info, setup)
		{
			this.lineHeights = new List<double> ();
			this.linePages   = new List<int> ();
		}

		public void GeneratePdf(string path, FormattedText text)
		{
			using (var stream = File.Open (path, FileMode.Create))
			{
				this.GeneratePdf (stream, text);
			}
		}

		public void GeneratePdf(Stream stream, FormattedText text)
		{
			this.text = text;

			this.ConstantJustification ();
			this.VerticalJustification ();

			var export = new Export (this.info);
			export.ExportToFile (stream, this.pageCount, this.RenderPage);
		}

		public double HeaderHeight
		{
			get;
			set;
		}
		public double FooterHeight
		{
			get;
			set;
		}

		private void RenderPage(Port port, int page)
		{
			this.RenderLayers (port, page);

			double topMargin = this.Setup.PageMargins.Top + this.HeaderHeight;
			double botMargin = this.setup.PageMargins.Bottom + this.FooterHeight;

			var clipRect = new Rectangle (Point.Zero, this.info.PageSize);
			clipRect.Deflate (this.setup.PageMargins.Left, this.setup.PageMargins.Right, topMargin, botMargin);

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

			port.PaintText (bounds, clipRect, this.text, this.Setup.TextStyle);			
		}


		private void ConstantJustification()
		{
			this.HeaderHeight += this.Setup.HeaderMargins.Height;
			this.FooterHeight += this.Setup.FooterMargins.Height;		
		}

		private void VerticalJustification()
		{
			this.lineHeights.Clear ();
			this.lineHeights.AddRange (Port.GetTextLineHeights (this.UsableWidth, this.text, this.Setup.TextStyle));

			//	Détermine dans quelles pages vont aller les lignes.
			this.linePages.Clear ();

			int page = 1;
			double dispo = this.UsableHeight;

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
				return this.info.PageSize.Height - this.Setup.PageMargins.Height - this.HeaderHeight - this.FooterHeight;
			}
		}


		private ListingDocumentSetup Setup
		{
			get
			{
				return this.setup as ListingDocumentSetup;
			}
		}


		private readonly List<double> lineHeights;
		private readonly List<int> linePages;
		private FormattedText text;
		private int pageCount;
	}
}
