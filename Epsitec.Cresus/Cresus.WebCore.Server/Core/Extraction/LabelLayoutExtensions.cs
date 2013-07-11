﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Labels;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{
	internal static class LabelLayoutExtensions
	{
		public static LabelPageLayout GetLabelPageLayout(this LabelLayout layout)
		{
			var labelSetup = new LabelPageLayout ()
			{
				TextStyle = new TextStyle ()
				{
					Font = Font.GetFont ("Arial", "Regular"),
					FontSize = 35.0,
				}
			};

			switch (layout)
			{
				case LabelLayout.Avery_3475:

					// The 0 margin width for the left and right sides is wrong. In reality, it is
					// about 1 mm and the labels on both sides are 69mm instead of 70. We can't
					// express that in the PDF library of Daniel, so we hack our way through by
					// pretending that there is no margin on the left and right and that the
					// labels are all 70mm wide.

					labelSetup = new LabelPageLayout
					{
						PageMargins = new Margins (0, 0, 45, 45),
						LabelGap = new Size (0, 0),
						LabelSize = new Size (700, 360),
						LabelMargins = new Margins (100, 100, 50, 50),
					};
					break;

				case LabelLayout.Sheet_A5_Simple:
				case LabelLayout.Sheet_A5_SimplePP:
				case LabelLayout.Sheet_A5_SimplePPPriority:

					labelSetup = new LabelPageLayout
					{
						PageMargins = new Margins (0, 0, 5, 5),
						LabelGap = new Size (0, 0),
						LabelSize = new Size (2100, 1480),
						LabelMargins = new Margins (0, 0, 0, 0),
					};

					break;

				default:
					throw new System.NotImplementedException ();
			}

			return labelSetup;
		}

		public static ExportPdfInfo GetExportPdfInfo(this LabelLayout layout)
		{
			switch (layout)
			{
				case LabelLayout.Avery_3475:
				case LabelLayout.Sheet_A5_Simple:
				case LabelLayout.Sheet_A5_SimplePP:
				case LabelLayout.Sheet_A5_SimplePPPriority:
					return new ExportPdfInfo ()
					{
						PageSize = PaperSize.A4,
					};

				default:
					throw new System.NotImplementedException ();
			}
		}

		public static LabelRenderer GetLabelRenderer(this LabelLayout layout)
		{
			switch (layout)
			{
				case LabelLayout.Avery_3475:
					return new LabelRenderer ();

				case LabelLayout.Sheet_A5_Simple:
					return new LabelRendererSheetA5 (); //.DefineLogo (@"S:\eerv.png", new Size (2100, 1480));

				case LabelLayout.Sheet_A5_SimplePP:
				case LabelLayout.Sheet_A5_SimplePPPriority:
					return new LabelRendererSheetA5
					{
						EmitterZipCode = 1002,
						EmitterPostOffice = "Lausanne",
						IncludesPrioritySymbol = layout == LabelLayout.Sheet_A5_SimplePPPriority
					};

				default:
					throw new System.NotImplementedException ();
			}
		}
	}
}
