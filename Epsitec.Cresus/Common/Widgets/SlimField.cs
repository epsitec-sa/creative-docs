//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	public class SlimField : Widget
	{
		public SlimField()
		{
			this.menuItems = new List<SlimFieldMenuItem> ();
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

		public string							FieldOther
		{
			get;
			set;
		}

		public IList<SlimFieldMenuItem>			MenuItems
		{
			get
			{
				return this.menuItems;
			}
		}

		public SlimFieldDisplayMode				DisplayMode
		{
			get;
			set;
		}

		
		public override Size GetBestFitSize()
		{
			var width  = System.Math.Ceiling (this.MeasureWidth ());
			var height = System.Math.Ceiling (Font.DefaultFontSize * 1.2 + 2 * SlimField.MarginY);

			return new Size (width, height);
		}

		public Rectangle GetTextSurface()
		{
			return Rectangle.Deflate (this.Client.Bounds, SlimField.MarginX, SlimField.MarginY);
		}

		public SlimFieldMenuItem DetectMenuItem(Point pos)
		{
			double advance = pos.X - this.GetTextSurface ().X;
			return this.DetectMenuItem (advance);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var bounds = this.Client.Bounds;

			this.PaintBackgroundSurface (graphics, bounds);

			switch (this.DisplayMode)
			{
				case SlimFieldDisplayMode.Label:
					this.PaintLabel (graphics);
					break;

				case SlimFieldDisplayMode.Text:
					this.PaintText (graphics);
					break;

				case SlimFieldDisplayMode.Menu:
					this.PaintMenu (graphics);
					break;
			}
		}

		private void PaintBackgroundSurface(Graphics graphics, Rectangle bounds)
		{
			graphics.AddFilledRectangle (bounds);
			graphics.RenderSolid (SlimField.Colors.BackColor);
		}

		private void PaintLabel(Graphics graphics)
		{
			var surface = this.GetTextSurface ();

			using (var path = Path.CreateRoundedRectangle (surface, 2, 2))
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (SlimField.Colors.LabelColor);

				graphics.Color = SlimField.Colors.BackColor;
				graphics.PaintText (surface, this.FieldLabel, SlimField.Fonts.LabelFont, Font.DefaultFontSize, Drawing.ContentAlignment.MiddleCenter);
			}
		}

		private void PaintText(Graphics graphics)
		{
			var surface = this.GetTextSurface ();

			var x       = surface.X;
			var y       = surface.Y;
			var width   = surface.Width;
			var height  = surface.Height;

			var geomPrefix = new TextGeometry (x, y, width, height, this.FieldPrefix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize, ContentAlignment.MiddleLeft);
			var geomText   = new TextGeometry (geomPrefix.Origin.X + geomPrefix.Width, geomPrefix.Origin.Y, width, height, this.FieldText, SlimField.Fonts.TextFont, Font.DefaultFontSize, ContentAlignment.BaselineLeft);
			var geomSuffix = new TextGeometry (geomText.Origin.X + geomText.Width, geomText.Origin.Y, width, height, this.FieldSuffix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize, ContentAlignment.BaselineLeft);

			graphics.Color = SlimField.Colors.TextColor;
			graphics.PaintText (geomPrefix, this.FieldPrefix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize);
			graphics.PaintText (geomText, this.FieldText, SlimField.Fonts.TextFont, Font.DefaultFontSize);
			graphics.PaintText (geomSuffix, this.FieldSuffix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize);
		}

		private void PaintMenu(Graphics graphics)
		{
			var surface = this.GetTextSurface ();

			var x       = surface.X;
			var y       = surface.Y;
			var width   = surface.Width;
			var height  = surface.Height;
			var geom    = new TextGeometry (x, y, width, height, "", SlimField.Fonts.TextFont, Font.DefaultFontSize, ContentAlignment.MiddleLeft);

			x = geom.Origin.X;
			y = geom.Origin.Y;

			graphics.Color = SlimField.Colors.TextColor;

			foreach (var tuple in this.GetMenuItemFontTextTuples ())
			{
				var font    = tuple.Item1;
				var text    = tuple.Item2;
				var item    = tuple.Item3;
				var hilite  = item == null ? SlimFieldMenuItemHilite.None : item.Hilite;
				var advance = font.GetTextAdvance (text) * Font.DefaultFontSize;

				graphics.PaintText (x, y, width, height, text, font, Font.DefaultFontSize, Drawing.ContentAlignment.BaselineLeft);

				if ((hilite == SlimFieldMenuItemHilite.Underline) &&
					(item.Style != SlimFieldMenuItemStyle.Symbol))
				{
					graphics.LineCap = CapStyle.Butt;
					graphics.LineWidth = 1.0;
					graphics.AddLine (x, y - 1.5, x + advance, y - 1.5);
					graphics.RenderSolid (SlimField.Colors.TextColor);
				}

				x     += advance;
				width -= advance;
			}
		}

		private double MeasureWidth()
		{
			double width = 2 * SlimField.MarginX;

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
					foreach (var tuple in this.GetMenuItemFontTextTuples ())
					{
						var font = tuple.Item1;
						var text = tuple.Item2;

						width += font.GetTextAdvance (text) * Font.DefaultFontSize;
					}
					break;
			}

			return width;
		}

		private IEnumerable<System.Tuple<Font, string, SlimFieldMenuItem>> GetMenuItemFontTextTuples()
		{
			bool first = true;

			foreach (var item in this.menuItems)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					yield return new System.Tuple<Font, string, SlimFieldMenuItem> (SlimField.Fonts.MenuFont, SlimField.Strings.MenuSeparator, null);
				}

				yield return new System.Tuple<Font, string, SlimFieldMenuItem> (SlimField.GetMenuItemFont (item), item.Text, item);
			}
		}


		private SlimFieldMenuItem DetectMenuItem(double advance)
		{
			foreach (var tuple in this.GetMenuItemFontTextTuples ())
			{
				var font = tuple.Item1;
				var text = tuple.Item2;
				var item = tuple.Item3;

				double width = font.GetTextAdvance (text) * Font.DefaultFontSize;

				if (advance <= width)
				{
					return item;
				}

				advance -= width;
			}

			return null;
		}

		private static Font GetMenuItemFont(SlimFieldMenuItem item)
		{
			switch (item.Style)
			{
				case SlimFieldMenuItemStyle.Value:
				case SlimFieldMenuItemStyle.Option:
					return item.Active == Widgets.ActiveState.Yes ? SlimField.Fonts.SelectedTextFont : SlimField.Fonts.TextFont;
				
				case SlimFieldMenuItemStyle.Extra:
					return item.Active == Widgets.ActiveState.Yes ? SlimField.Fonts.SelectedExtraFont : SlimField.Fonts.ExtraFont;

				case SlimFieldMenuItemStyle.Symbol:
					return SlimField.Fonts.SymbolFont;

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", item.Style.GetQualifiedName ()));
			}
		}




		private static class Strings
		{
			public static readonly string		MenuSeparator = "  ";
		}

		private static class Colors
		{
			public static readonly Color		BackColor  = Color.FromHexa ("ffffff");
			public static readonly Color		LabelColor = Color.FromHexa ("3399ff");
			public static readonly Color		TextColor  = Color.FromHexa ("000000");
		}

		private static class Fonts
		{
			public static readonly Font			LabelFont         = Font.GetFont ("Segoe UI", "Bold");
			public static readonly Font			DescriptionFont   = Font.GetFont ("Segoe UI", "Light Regular");
			public static readonly Font			TextFont          = Font.GetFont ("Segoe UI", "Regular");
			public static readonly Font			ExtraFont         = Font.GetFont ("Segoe UI", "Italic");
			public static readonly Font			MenuFont          = Font.GetFont ("Segoe UI", "Regular");
			public static readonly Font			SymbolFont        = Font.GetFont ("Segoe UI Symbol", "Regular");
			public static readonly Font			SelectedTextFont  = Font.GetFont ("Segoe UI", "Bold");
			public static readonly Font			SelectedExtraFont = Font.GetFont ("Segoe UI", "Bold Italic");
		}

		private const int						MarginX = 3;
		private const int						MarginY = 2;

		private readonly List<SlimFieldMenuItem> menuItems;
	}
}