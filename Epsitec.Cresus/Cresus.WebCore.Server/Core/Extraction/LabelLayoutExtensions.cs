using Epsitec.Common.Drawing;

using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Labels;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	internal static class LabelLayoutExtensions
	{


		public static LabelsSetup GetLabelsSetup(this LabelLayout layout)
		{
			switch (layout)
			{
				case LabelLayout.Label_3475_70_X_36:

					// The 0 margin width for the left and right sides is wrong. In reality, it is
					// about 1 mm and the labels on both sides are 69mm instead of 70. We can't
					// express that in the PDF library of Daniel, so we hack our way through by
					// pretending that there is no margin on the left and right and that the
					// labels are all 70mm wide.

					return new LabelsSetup ()
					{
						PageMargins = new Margins (0, 0, 45, 45),
						LabelGap = new Size (0, 0),
						LabelSize = new Size (700, 360),
						LabelMargins = new Margins (50, 50, 25, 25),
					};

				default:
					throw new NotImplementedException ();
			}
		}


		public static ExportPdfInfo GetExportPdfInfo(this LabelLayout layout)
		{
			switch (layout)
			{
				case LabelLayout.Label_3475_70_X_36:
					return new ExportPdfInfo ()
					{
						PageSize = PaperSize.A4,
					};

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
