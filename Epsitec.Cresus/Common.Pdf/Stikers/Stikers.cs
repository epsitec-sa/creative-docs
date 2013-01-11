using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Stikers
{
	public class Stikers
	{
		public Stikers()
		{
		}

		public PdfExportException GeneratePdf(string path, int count, Func<int, FormattedText> accessor, StikersSetup setup)
		{
			this.count = count;
			this.accessor = accessor;
			this.setup = setup;
			this.info = new ExportPdfInfo ();

			this.stikersPerPage = this.StikersPerPage;
			this.font = Font.GetFont (this.setup.FontFace, this.setup.FontStyle);

			if (this.stikersPerPage < 1)
			{
				return new PdfExportException("Etiquettes trop grandes par rapport au papier");
			}

			int pageCount = (this.count + this.stikersPerPage - 1) / this.stikersPerPage;

			var export = new Export (this.info);
			return export.ExportToFile (path, pageCount, this.Renderer);
		}

		private void Renderer(Port port, int page)
		{
			for (int y = 0; y < this.VerticalStikersCount; y++)
			{
				for (int x = 0; x < this.HorizontalStikersCount; x++)
				{
					int rankIntoPage = y * this.HorizontalStikersCount + x;
					int rank = (page-1) * this.stikersPerPage + rankIntoPage;

					if (rank < this.count)
					{
						var text = this.accessor (rank);

						var bounds = this.GetStikerBounds (rankIntoPage);

						if (this.setup.PaintFrame)
						{
							var path = new Path ();
							path.AppendRectangle (bounds);

							port.LineWidth = 1.0;  // épaisseur de 0.1mm
							port.Color = Color.FromBrightness (0.8);  // gris clair
							port.PaintOutline (path);
						}

						bounds.Deflate (this.setup.StikerMargins);
						port.PaintText (bounds.Left, bounds.Bottom, bounds.Size, text, this.font, this.setup.FontSize);
					}
				}
			}
		}


		private Rectangle GetStikerBounds(int rankIntoPage)
		{
			int x = rankIntoPage % this.HorizontalStikersCount;
			int y = rankIntoPage / this.HorizontalStikersCount;

			double ox = this.setup.PageMargins.Left + (this.setup.StikerSize.Width  + this.setup.StikerGap.Width ) * x;
			double oy = this.setup.PageMargins.Top  + (this.setup.StikerSize.Height + this.setup.StikerGap.Height) * y;

			return new Rectangle (ox, this.info.PageSize.Height - this.setup.StikerSize.Height - oy, this.setup.StikerSize.Width, this.setup.StikerSize.Height);
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
				return (int) ((this.info.PageSize.Width - this.setup.PageMargins.Width + this.setup.StikerGap.Width) / (this.setup.StikerSize.Width + this.setup.StikerGap.Width));
			}
		}

		private int VerticalStikersCount
		{
			get
			{
				return (int) ((this.info.PageSize.Height - this.setup.PageMargins.Height + this.setup.StikerGap.Height) / (this.setup.StikerSize.Height + this.setup.StikerGap.Height));
			}
		}


		private Func<int, FormattedText> accessor;
		private StikersSetup setup;
		private ExportPdfInfo info;
		private int stikersPerPage;
		private Font font;
		private int count;
	}
}
