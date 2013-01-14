//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Stikers
{
	public class Stikers : CommonPdf
	{
		public Stikers(ExportPdfInfo info, CommonSetup setup)
			: base (info, setup)
		{
		}

		public PdfExportException GeneratePdf(string path, int count, Func<int, FormattedText> dataAccessor)
		{
			this.count        = count;
			this.dataAccessor = dataAccessor;

			this.stikersPerPage = this.StikersPerPage;

			if (this.stikersPerPage < 1)
			{
				return new PdfExportException("Etiquettes trop grandes par rapport au papier");
			}

			int pageCount = (this.count + this.stikersPerPage - 1) / this.stikersPerPage;

			var export = new Export (this.info);
			return export.ExportToFile (path, pageCount, this.RenderPage);
		}

		private void RenderPage(Port port, int page)
		{
			this.RenderLayers (port, page);

			for (int y = 0; y < this.VerticalStikersCount; y++)
			{
				for (int x = 0; x < this.HorizontalStikersCount; x++)
				{
					int rankIntoPage = y * this.HorizontalStikersCount + x;
					int rank = (page-1) * this.stikersPerPage + rankIntoPage;

					if (rank < this.count)
					{
						var text = this.dataAccessor (rank);

						var bounds = this.GetStikerBounds (rankIntoPage);

						if (this.Setup.PaintFrame)
						{
							var path = new Path ();
							path.AppendRectangle (bounds);

							port.LineWidth = 1.0;  // épaisseur de 0.1mm
							port.Color = Color.FromBrightness (0.8);  // gris clair
							port.PaintOutline (path);
						}

						bounds.Deflate (this.Setup.StikerMargins);
						port.PaintText (bounds, text, this.Setup.TextStyle);
					}
				}
			}
		}


		private Rectangle GetStikerBounds(int rankIntoPage)
		{
			int x = rankIntoPage % this.HorizontalStikersCount;
			int y = rankIntoPage / this.HorizontalStikersCount;

			double ox = this.Setup.PageMargins.Left + (this.Setup.StikerSize.Width  + this.Setup.StikerGap.Width) * x;
			double oy = this.Setup.PageMargins.Top  + (this.Setup.StikerSize.Height + this.Setup.StikerGap.Height) * y;

			return new Rectangle (ox, this.info.PageSize.Height - this.Setup.StikerSize.Height - oy, this.Setup.StikerSize.Width, this.Setup.StikerSize.Height);
		}

		private int StikersPerPage
		{
			get
			{
				return this.HorizontalStikersCount * this.VerticalStikersCount;
			}
		}

		private int HorizontalStikersCount
		{
			get
			{
				return (int) ((this.info.PageSize.Width - this.Setup.PageMargins.Width + this.Setup.StikerGap.Width) / (this.Setup.StikerSize.Width + this.Setup.StikerGap.Width));
			}
		}

		private int VerticalStikersCount
		{
			get
			{
				return (int) ((this.info.PageSize.Height - this.Setup.PageMargins.Height + this.Setup.StikerGap.Height) / (this.Setup.StikerSize.Height + this.Setup.StikerGap.Height));
			}
		}


		private StikersSetup Setup
		{
			get
			{
				return this.setup as StikersSetup;
			}
		}


		private Func<int, FormattedText> dataAccessor;
		private int stikersPerPage;
		private int count;
	}
}
