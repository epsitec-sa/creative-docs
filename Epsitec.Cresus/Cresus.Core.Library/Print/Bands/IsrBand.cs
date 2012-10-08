//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Print.Bands
{
	/// <summary>
	/// Dessine un BVR orange.
	/// </summary>
	public class IsrBand : AbstractIsrBand
	{
		public IsrBand()
			: base ()
		{
		}



		protected override Color LightPinkColor(PreviewMode previewMode)
		{
			if (previewMode != PreviewMode.Print)
			{
				return Color.FromHexa ("ffe2d9");  // orange très pâle
			}
			else
			{
				return Color.FromHexa ("ffc6b2");  // orange pâle
			}
		}

		protected override Color DarkPinkColor(PreviewMode previewMode)
		{
			return Color.FromHexa ("ff8547");  // orange
		}


		protected override void PaintFix(IPaintPort port, PreviewMode previewMode, Point topLeft)
		{
			//	Dessine tous éléments fixes, pour simuler un BV sur un fond blanc.
			base.PaintFix (port, previewMode, topLeft);

			//	Dessine la référence.
			port.Color = this.DarkPinkColor (previewMode);
			port.LineWidth = 0.1;
			port.PaintOutline (Path.FromRectangle (topLeft.X+123, topLeft.Y-39, 83, 6));
			port.PaintOutline (Path.FromLine (topLeft.X+123, topLeft.Y-33, topLeft.X+123, topLeft.Y-32));
			port.PaintOutline (Path.FromLine (topLeft.X+123, topLeft.Y-32, topLeft.X+140, topLeft.Y-32));
			port.PaintOutline (Path.FromLine (topLeft.X+206, topLeft.Y-33, topLeft.X+206, topLeft.Y-32));
			port.PaintOutline (Path.FromLine (topLeft.X+206, topLeft.Y-32, topLeft.X+190, topLeft.Y-32));

			//	Dessine les cases pour les montants.
			port.Color = this.DarkPinkColor (previewMode);
			port.LineWidth = 0.75;
			port.PaintOutline (Path.FromRectangle (topLeft.X+1, topLeft.Y-56, 40, 6));
			port.PaintOutline (Path.FromRectangle (topLeft.X+47, topLeft.Y-56, 10, 6));
			port.PaintOutline (Path.FromRectangle (topLeft.X+62, topLeft.Y-56, 40, 6));
			port.PaintOutline (Path.FromRectangle (topLeft.X+108, topLeft.Y-56, 10, 6));

			port.Color = Color.FromBrightness (0);
			port.PaintSurface (Path.FromCircle (topLeft.X+44.5, topLeft.Y-55.5, 0.4));
			port.PaintSurface (Path.FromCircle (topLeft.X+105.5, topLeft.Y-55.5, 0.4));

			//	Dessine les textes.
			port.Color = Color.FromBrightness (0);
			port.PaintText (topLeft.X+70, topLeft.Y-76, "609", AbstractIsrBand.ocrFont, 4.2);

			port.Color = this.DarkPinkColor (previewMode);
			port.PaintText (topLeft.X+144, topLeft.Y-32, "Referenz-Nr. / N° de référence / N° di riferimento", AbstractIsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+124, topLeft.Y-14, "Keine Mitteilungen anbringen", AbstractIsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+124, topLeft.Y-18, "Pas de communications", AbstractIsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+124, topLeft.Y-22, "Non aggiungete comunicazioni", AbstractIsrBand.fixFontRegular, 2.0);
		}

		protected override void PaintContent(IPaintPort port, Point topLeft)
		{
			//	Dessine tous les textes variables.
			base.PaintContent (port, topLeft);

			port.Color = Color.FromBrightness (0);

			var isr = this.Slip;

			string codingZone                = this.NotForUse ? isr.GetInvalidCodingZone () : isr.GetCodingZone ();
			string formattedRefNumber        = this.NotForUse ? "" : isr.GetFormattedReferenceNumber ();
			string formattedSubscriberNumber = isr.GetFormattedSubscriberNumber ();

			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+8, topLeft.Y-40, 50, 28), ContentAlignment.TopLeft, fixFontBold, 3.5, this.To);
			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+68, topLeft.Y-40, 50, 28), ContentAlignment.TopLeft, fixFontBold, 3.5, this.To);

			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+7, topLeft.Y-85, 50, 19), ContentAlignment.TopLeft, fixFontRegular, 2.4, this.From);
			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+129, topLeft.Y-75, 70, 24), ContentAlignment.TopLeft, fixFontRegular, 3.0, this.From);

			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+34, topLeft.Y-47, 25, 5), ContentAlignment.TopLeft, fixFontRegular, 3.0, formattedSubscriberNumber);
			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+94, topLeft.Y-47, 25, 5), ContentAlignment.TopLeft, fixFontRegular, 3.0, formattedSubscriberNumber);

			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+7, topLeft.Y-66, 50, 5), ContentAlignment.TopLeft, fixFontRegular, 3.0, formattedRefNumber);
			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+123, topLeft.Y-39, 83, 6), ContentAlignment.MiddleCenter, ocrFont, 4.0, formattedRefNumber);
			AbstractIsrBand.PaintText (port, new Rectangle (topLeft.X+60, topLeft.Y-90, 143, 5), ContentAlignment.TopRight, ocrFont, 4.2, codingZone);

			IsrBand.PaintPrice (port, new Rectangle (topLeft.X+1, topLeft.Y-56, 40, 6), new Rectangle (topLeft.X+47, topLeft.Y-56, 10, 6), this.Slip.Amount, this.NotForUse);
			IsrBand.PaintPrice (port, new Rectangle (topLeft.X+62, topLeft.Y-56, 40, 6), new Rectangle (topLeft.X+108, topLeft.Y-56, 10, 6), this.Slip.Amount, this.NotForUse);
		}


		private static void PaintPrice(IPaintPort port, Rectangle francRect, Rectangle centRect, decimal price, bool notForUse)
		{
			if (price == 0.0M && !notForUse)
			{
				return;
			}

			string franc = IsrBand.PriceToStringFranc (price);
			string cent  = IsrBand.PriceToStringCent (price);

			if (notForUse)
			{
				franc = "XXXXXXXXXX";
				cent  = "XX";
			}

			francRect.Deflate (0, 2, 0, 0);

			AbstractIsrBand.PaintText (port, francRect, ContentAlignment.MiddleRight, fixFontBold, 4.5, franc);
			AbstractIsrBand.PaintText (port, centRect, ContentAlignment.MiddleCenter, fixFontBold, 4.5, cent);
		}

		private static string PriceToStringFranc(decimal price)
		{
			//	Extrait les francs (la partie entière) d'un prix.
			int franc = decimal.ToInt32 (price);
			return franc.ToString ();
		}

		private static string PriceToStringCent(decimal price)
		{
			//	Extrait les centimes (la partie fractionnaire) d'un prix.
			int cent = decimal.ToInt32 (price * 100) - decimal.ToInt32 (price) * 100;
			return cent.ToString ("D2");
		}

	}
}
