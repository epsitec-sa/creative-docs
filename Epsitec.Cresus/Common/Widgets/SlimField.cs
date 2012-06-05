//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class SlimField : Widget
	{
		public SlimField()
		{

		}


		public string							FieldLabel
		{
			get;
			set;
		}

		public string							FieldPrefix
		{
			get;
			set;
		}

		public string							FieldSuffix
		{
			get;
			set;
		}

		public string							FieldText
		{
			get;
			set;
		}

		
		public SlimFieldDisplayMode				DisplayMode
		{
			get;
			set;
		}

		public override Size GetBestFitSize()
		{
			var width  = System.Math.Ceiling (this.MeasureWidth ());
			var height = System.Math.Ceiling (Font.DefaultFontSize * 1.2 + 4.0);

			return new Size (width, height);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var bounds = this.Client.Bounds;

			this.PaintBackgroundSurface (graphics, bounds);

			switch (this.DisplayMode)
			{
				case SlimFieldDisplayMode.Label:
					this.PaintLabel (graphics, bounds);
					break;
						
				case SlimFieldDisplayMode.Text:
					this.PaintText (graphics, bounds);
					break;
			}
		}

		private void PaintBackgroundSurface(Graphics graphics, Rectangle bounds)
		{
			graphics.AddFilledRectangle (bounds);
			graphics.RenderSolid (SlimField.Colors.BackColor);
		}

		private void PaintLabel(Graphics graphics, Rectangle bounds)
		{
			var surface = Rectangle.Deflate (bounds, 2, 2);

			using (var path = Path.CreateRoundedRectangle (surface, 2, 2))
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (SlimField.Colors.LabelColor);

				graphics.Color = SlimField.Colors.BackColor;
				graphics.PaintText (surface, this.FieldLabel, SlimField.Fonts.LabelFont, Font.DefaultFontSize, Drawing.ContentAlignment.MiddleCenter);
			}
		}

		private void PaintText(Graphics graphics, Rectangle bounds)
		{
			var surface = Rectangle.Deflate (bounds, 2, 2);

			var x       = surface.X;
			var y       = surface.Y;
			var width   = surface.Width;
			var height  = surface.Height;

			var geomPrefix = new TextGeometry (x, y, width, height, this.FieldPrefix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize, ContentAlignment.MiddleLeft);
			var geomText   = new TextGeometry (geomPrefix.Origin.X + geomPrefix.Width, geomPrefix.Origin.Y, width, height, this.FieldText, SlimField.Fonts.TextFont, Font.DefaultFontSize, ContentAlignment.BaselineLeft);
			var geomSuffix = new TextGeometry (geomText.Origin.X + geomText.Width, geomText.Origin.Y, width, height, this.FieldSuffix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize, ContentAlignment.BaselineLeft);

			graphics.Color = SlimField.Colors.TextColor;
			graphics.PaintText (geomPrefix, this.FieldPrefix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize);
			graphics.PaintText (geomText,  this.FieldText,  SlimField.Fonts.TextFont, Font.DefaultFontSize);
			graphics.PaintText (geomSuffix, this.FieldSuffix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize);
		}

		private double MeasureWidth()
		{
			double width = 4;

			switch (this.DisplayMode)
			{
				case SlimFieldDisplayMode.Label:
					width += 6;
					width += SlimField.Fonts.LabelFont.GetTextAdvance (this.FieldLabel) * Font.DefaultFontSize;
					break;

				case SlimFieldDisplayMode.Text:
					width += SlimField.Fonts.DescriptionFont.GetTextAdvance (this.FieldPrefix) * Font.DefaultFontSize;
					width += SlimField.Fonts.TextFont.GetTextAdvance (this.FieldText) * Font.DefaultFontSize;
					width += SlimField.Fonts.DescriptionFont.GetTextAdvance (this.FieldSuffix) * Font.DefaultFontSize;
					break;

				case SlimFieldDisplayMode.Menu:
					break;
			}

			return width;
		}

		private static class Colors
		{
			public static readonly Color		BackColor  = Color.FromHexa ("ffffff");
			public static readonly Color		LabelColor = Color.FromHexa ("3399ff");
			public static readonly Color		TextColor  = Color.FromHexa ("000000");
		}

		private static class Fonts
		{
			public static readonly Font			LabelFont = Font.GetFont ("Segoe UI", "Bold");
			public static readonly Font			DescriptionFont = Font.GetFont ("Segoe UI", "Light Regular");
			public static readonly Font			TextFont = Font.GetFont ("Segoe UI", "Regular");
		}
	}
}