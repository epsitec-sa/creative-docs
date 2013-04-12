//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Types;

using System;
using System.IO;

namespace Epsitec.Common.Pdf.Stickers
{
	using Path = Epsitec.Common.Drawing.Path;

	public class Stickers : CommonPdf
	{
		public Stickers(ExportPdfInfo info, CommonSetup setup)
			: base (info, setup)
		{
		}

		public void GeneratePdf(string path, int count, Func<int, FormattedText> dataAccessor)
		{
			using (var stream = File.Open (path, FileMode.Create))
			{
				this.GeneratePdf (stream, count, dataAccessor);
			}
		}

		public void GeneratePdf(Stream stream, int count, Func<int, FormattedText> dataAccessor)
		{
			this.count        = count;
			this.dataAccessor = dataAccessor;

			this.stickersPerPage = this.StickersPerPage;

			if (this.stickersPerPage < 1)
			{
				throw new PdfExportException("Etiquettes trop grandes par rapport au papier");
			}

			int pageCount = (this.count + this.stickersPerPage - 1) / this.stickersPerPage;

			var export = new Export (this.info);
			export.ExportToFile (stream, pageCount, this.RenderPage);
		}

		private void RenderPage(Port port, int page)
		{
			this.RenderLayers (port, page);

			for (int y = 0; y < this.VerticalStickersCount; y++)
			{
				for (int x = 0; x < this.HorizontalStickersCount; x++)
				{
					int rankIntoPage = y * this.HorizontalStickersCount + x;
					int rank = (page-1) * this.stickersPerPage + rankIntoPage;

					if (rank < this.count)
					{
						var text = this.dataAccessor (rank);

						var bounds = this.GetStickerBounds (rankIntoPage);

						if (this.Setup.PaintFrame)
						{
							var path = new Path ();
							path.AppendRectangle (bounds);

							port.LineWidth = 1.0;  // épaisseur de 0.1mm
							port.Color = Color.FromBrightness (0.8);  // gris clair
							port.PaintOutline (path);
						}

						bounds.Deflate (this.Setup.StickerMargins);
						port.PaintText (bounds, text, this.Setup.TextStyle);
					}
				}
			}
		}


		private Rectangle GetStickerBounds(int rankIntoPage)
		{
			int x = rankIntoPage % this.HorizontalStickersCount;
			int y = rankIntoPage / this.HorizontalStickersCount;

			double ox = this.Setup.PageMargins.Left + (this.Setup.StickerSize.Width  + this.Setup.StickerGap.Width) * x;
			double oy = this.Setup.PageMargins.Top  + (this.Setup.StickerSize.Height + this.Setup.StickerGap.Height) * y;

			return new Rectangle (ox, this.info.PageSize.Height - this.Setup.StickerSize.Height - oy, this.Setup.StickerSize.Width, this.Setup.StickerSize.Height);
		}

		private int StickersPerPage
		{
			get
			{
				return this.HorizontalStickersCount * this.VerticalStickersCount;
			}
		}

		private int HorizontalStickersCount
		{
			get
			{
				return (int) ((this.info.PageSize.Width - this.Setup.PageMargins.Width + this.Setup.StickerGap.Width) / (this.Setup.StickerSize.Width + this.Setup.StickerGap.Width));
			}
		}

		private int VerticalStickersCount
		{
			get
			{
				return (int) ((this.info.PageSize.Height - this.Setup.PageMargins.Height + this.Setup.StickerGap.Height) / (this.Setup.StickerSize.Height + this.Setup.StickerGap.Height));
			}
		}


		private StickersSetup Setup
		{
			get
			{
				return this.setup as StickersSetup;
			}
		}


		private Func<int, FormattedText> dataAccessor;
		private int stickersPerPage;
		private int count;
	}
}
