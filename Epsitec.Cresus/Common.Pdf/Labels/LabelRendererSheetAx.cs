//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Pdf.Labels
{
	/// <summary>
	/// The <c>LabelRendererSheetAx</c> class renders an Ax-sheet to be used as
	/// a label, with the P.P. franking information, when requested. This is the
	/// base class used for A4/A5 output.
	/// </summary>
	public abstract class LabelRendererSheetAx<T> : LabelRenderer
		where T : LabelRendererSheetAx<T>
	{
		protected LabelRendererSheetAx(Point pageOffset)
		{
			this.pageOffset = pageOffset;
		}


		public bool								UsesPP
		{
			get
			{
				return (this.EmitterZipCode >= 1000)
					&& (string.IsNullOrEmpty (this.EmitterPostOffice) == false);
			}
		}

		public bool								IncludesPrioritySymbol
		{
			get;
			set;
		}

		public int								EmitterZipCode
		{
			get;
			set;
		}

		public string							EmitterPostOffice
		{
			get;
			set;
		}


		public T DefineLogo(string logoPath, Size logoSize)
		{
			this.logoPath = logoPath;
			this.logoSize = logoSize;
			this.logoImage = Bitmap.FromNativeBitmap (System.IO.File.ReadAllBytes (logoPath));

			return this as T;
		}


		protected override void RenderLabel(Port port, FormattedText text, Rectangle bounds, LabelPageLayout layout)
		{
			bounds = Rectangle.Offset (bounds, this.pageOffset);

			if (this.logoImage != null)
			{
				port.PaintImage (this.logoImage, bounds);
			}

			var textRect = new Rectangle (bounds.X + 1200, bounds.Y + 700, 800, 300);

			port.PaintText (textRect, text, layout.TextStyle);

			if (this.UsesPP)
			{
				this.RenderPP (port, bounds, layout, textRect);
			}
		}

		
		private void RenderPP(Port port, Rectangle bounds, LabelPageLayout layout, Rectangle textRect)
		{
			//	Renders "P.P. CH-1400 Yverdon-les-Bains  A  Poste CH SA" with the proper
			//	layout.

			//	http://www.poste.ch/post-startseite/post-geschaeftskunden/post-briefe/post-briefe-frankieren/post-briefe-ppfrankieren.htm
			//	http://www.poste.ch/post-startseite/post-geschaeftskunden/post-briefe/post-briefe-frankieren/post-briefe-ppfrankieren/post-briefe-pp-frankieren-factsheet.pdf
			//	http://www.poste.ch/post-startseite/post-geschaeftskunden/post-briefe/post-briefe-gestalten/post-briefe-gut-zum-druck/post-briefe-gut-zu-druck-region-west.htm

			using (var path = new Path ())
			{
				var markRect = new Rectangle (bounds.X + 1200, bounds.Y + 1030, 400, 70);
				path.AppendRectangle (markRect);

				path.MoveTo (textRect.Left, markRect.Bottom - 7.5);
				path.LineTo (textRect.Right, markRect.Bottom - 7.5);

				port.Color = Color.FromBrightness (1);
				port.PaintSurface (path);

				port.LineWidth = 0.5;
				port.Color = Color.FromBrightness (0);
				port.PaintOutline (path);

				var text = string.Format ("CH-{0:0000}<br/>{1}", this.EmitterZipCode, this.EmitterPostOffice.Truncate (18));

				LabelRendererSheetAx<T>.RenderPPSymbol (port, layout, ref markRect);
				LabelRendererSheetAx<T>.RenderPPEmitter (port, layout, text, ref markRect);

				if (this.IncludesPrioritySymbol)
				{
					LabelRendererSheetAx<T>.RenderPrioritySymbol (port, layout, ref markRect);
				}

				LabelRendererSheetAx<T>.RenderSwissPostLogo (port, layout, ref markRect);
			}
		}

		
		private static void RenderPPSymbol(Port port, LabelPageLayout layout, ref Rectangle rect)
		{
			var markText1 = new FormattedText (@"<font face=""Arial"" size=""60""><b>P</b></font>");
			var markText2 = new FormattedText (@"<font face=""Arial"" size=""60""><b>.P</b></font>");
			var markText3 = new FormattedText (@"<font face=""Arial"" size=""60""><b>.</b></font>");

			rect = Rectangle.Offset (rect, 20, 0);
			port.PaintText (rect, markText1, layout.TextStyle);
			rect = Rectangle.Offset (rect, 24, 0);
			port.PaintText (rect, markText2, layout.TextStyle);
			rect = Rectangle.Offset (rect, 40, 0);
			port.PaintText (rect, markText3, layout.TextStyle);
			rect = Rectangle.Offset (rect, 40, 0);
		}
		
		private static void RenderPPEmitter(Port port, LabelPageLayout layout, string text, ref Rectangle rect)
		{
			var markText4 = new FormattedText (@"<font face=""Arial"" size=""30"">"+ text + "</font>");
			port.PaintText (rect, markText4, layout.TextStyle);
			rect = Rectangle.Offset (rect, 300, 12);
		}

		private static void RenderPrioritySymbol(Port port, LabelPageLayout layout, ref Rectangle rect)
		{
			var markText5 = new FormattedText (@"<font face=""Arial"" size=""90""><b>A</b></font>");
			port.PaintText (rect, markText5, layout.TextStyle);
		}
		
		private static void RenderSwissPostLogo(Port port, LabelPageLayout layout, ref Rectangle rect)
		{
			var markText6 = new FormattedText (@"<font face=""Arial"" size=""25"">Poste CH SA</font>");
			rect = Rectangle.Offset (rect, 200, -9);
			port.PaintText (rect, markText6, layout.TextStyle);
		}


		private readonly Point					pageOffset;
		
		private Image							logoImage;
		private string							logoPath;
		private Size							logoSize;
	}
}

