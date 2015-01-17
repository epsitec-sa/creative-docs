//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Types;

using System.IO;

namespace Epsitec.Common.Pdf.Labels
{
	using Path = Epsitec.Common.Drawing.Path;

	public class LabelGenerator : CommonPdf
	{
		public LabelGenerator(ExportPdfInfo info, LabelPageLayout setup, LabelRenderer renderer)
			: base (info, setup)
		{
			this.renderer = renderer;
		}


		private int								LabelsPerPage
		{
			get
			{
				return this.HorizontalLabelsCount * this.VerticalLabelsCount;
			}
		}

		private int								HorizontalLabelsCount
		{
			get
			{
				return (int) ((this.info.PageSize.Width - this.Setup.PageMargins.Width + this.Setup.LabelGap.Width) / (this.Setup.LabelSize.Width + this.Setup.LabelGap.Width));
			}
		}

		private int								VerticalLabelsCount
		{
			get
			{
				return (int) ((this.info.PageSize.Height - this.Setup.PageMargins.Height + this.Setup.LabelGap.Height) / (this.Setup.LabelSize.Height + this.Setup.LabelGap.Height));
			}
		}

		private LabelPageLayout					Setup
		{
			get
			{
				return this.setup as LabelPageLayout;
			}
		}
		
		
		public void GeneratePdf(string path, int count, System.Func<int, FormattedText> dataAccessor)
		{
			using (var stream = File.Open (path, FileMode.Create))
			{
				this.GeneratePdf (stream, count, dataAccessor);
			}
		}

		public void GeneratePdf(Stream stream, int count, System.Func<int, FormattedText> dataAccessor)
		{
			this.totalLabelCount = count;
			this.textGetter    = dataAccessor;

			this.labelsPerPage = this.LabelsPerPage;

			if (this.labelsPerPage < 1)
			{
				throw new PdfExportException ("Etiquettes trop grandes par rapport au papier");
			}

			int pageCount = (this.totalLabelCount + this.labelsPerPage - 1) / this.labelsPerPage;

			var export = new Export (this.info);
			export.ExportToFile (stream, pageCount, this.RenderPage);
		}

		
		private void RenderPage(Port port, int page)
		{
			this.RenderLayers (port, page);

			for (int y = 0; y < this.VerticalLabelsCount; y++)
			{
				for (int x = 0; x < this.HorizontalLabelsCount; x++)
				{
					int rankIntoPage = y * this.HorizontalLabelsCount + x;
					int rank         = (page-1) * this.labelsPerPage + rankIntoPage;

					if (rank < this.totalLabelCount)
					{
						var text   = this.textGetter (rank);
						var bounds = this.GetLabelBounds (rankIntoPage);

						this.renderer.Render (port, text, bounds, this.Setup);
					}
				}
			}
		}

		private Rectangle GetLabelBounds(int rankIntoPage)
		{
			int x = rankIntoPage % this.HorizontalLabelsCount;
			int y = rankIntoPage / this.HorizontalLabelsCount;

			double ox = this.Setup.PageMargins.Left + (this.Setup.LabelSize.Width  + this.Setup.LabelGap.Width) * x;
			double oy = this.Setup.PageMargins.Top  + (this.Setup.LabelSize.Height + this.Setup.LabelGap.Height) * y;

			return new Rectangle (ox, this.info.PageSize.Height - this.Setup.LabelSize.Height - oy, this.Setup.LabelSize.Width, this.Setup.LabelSize.Height);
		}


		private readonly LabelRenderer			renderer;
		
		private System.Func<int, FormattedText>	textGetter;
		private int								labelsPerPage;
		private int								totalLabelCount;
	}
}
