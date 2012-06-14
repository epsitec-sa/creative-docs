//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

[assembly:DependencyClass (typeof (SlimField))]

namespace Epsitec.Common.Widgets
{
	public class SlimField : Widget, IReadOnly
	{
		public SlimField()
		{
			this.InternalState |= WidgetInternalState.Focusable;
			
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
			get
			{
				return this.GetValue<SlimFieldDisplayMode> (SlimField.DisplayModeProperty);
			}
			set
			{
				if (value == SlimFieldDisplayMode.Label)
				{
					this.ClearValue (SlimField.DisplayModeProperty);
				}
				else
				{
					this.SetValue (SlimField.DisplayModeProperty, value);
				}
			}
		}

		public bool								IsReadOnly
		{
			get
			{
				return this.GetValue<bool> (SlimField.IsReadOnlyProperty);
			}
			set
			{
				if (value)
				{
					this.SetValue (SlimField.IsReadOnlyProperty, value);
				}
				else
				{
					this.ClearValue (SlimField.IsReadOnlyProperty);
				}
			}
		}


		#region IReadOnly Members

		bool Types.IReadOnly.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		#endregion

		
		public override Size GetBestFitSize()
		{
			var width  = System.Math.Ceiling (this.MeasureWidth (this.GetActiveDisplayMode ()));
			var height = System.Math.Ceiling (Font.DefaultFontSize * 1.2 + 2 * SlimField.MarginY);

			return new Size (width, height);
		}

		public Rectangle GetTextSurface()
		{
			return Rectangle.Deflate (this.Client.Bounds, SlimField.MarginX, SlimField.MarginY);
		}

		public SlimFieldDisplayMode GetActiveDisplayMode()
		{
			var mode = this.DisplayMode;

			switch (mode)
			{
				case SlimFieldDisplayMode.Label:
					return string.IsNullOrEmpty (this.FieldText) ? SlimFieldDisplayMode.Label : SlimFieldDisplayMode.Text;

				case SlimFieldDisplayMode.LabelEdition:
					return string.IsNullOrEmpty (this.FieldText) ? SlimFieldDisplayMode.Label : SlimFieldDisplayMode.Text;

				case SlimFieldDisplayMode.TextEdition:
					return SlimFieldDisplayMode.Text;

				default:
					return mode;
			}
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

			switch (this.GetActiveDisplayMode ())
			{
				case SlimFieldDisplayMode.Label:
				case SlimFieldDisplayMode.LabelEdition:
					this.PaintLabel (graphics);
					break;

				case SlimFieldDisplayMode.Text:
				case SlimFieldDisplayMode.TextEdition:
					this.PaintText (graphics);
					break;

				case SlimFieldDisplayMode.Menu:
					this.PaintMenu (graphics);
					break;
			}
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var bounds = this.Client.Bounds;

			this.PaintFocusRectangle (graphics, bounds);
		}


		private static int GetMenuItemsTextVariantCount(IList<MenuItem> menuItems)
		{
			int num = 0;

			foreach (var item in menuItems)
			{
				num = System.Math.Max (num, item.Item3.Texts.Count);
			}

			return num;
		}

		private int SelectMenuItemsTextVariant(double maxWidth)
		{
			IList<MenuItem> menuItems = this.GetMenuItems ().ToList ();

			int count = SlimField.GetMenuItemsTextVariantCount (menuItems);

			for (int i = 0; i < count; i++)
			{
				double valueWidth = SlimField.MeasureMenuItems (i, menuItems.Where (x => x.Item3.Style == SlimFieldMenuItemStyle.Value));
				double extraWidth = SlimField.MeasureMenuItems (i, menuItems.Where (x => x.Item3.Style == SlimFieldMenuItemStyle.Extra));
				
				if (valueWidth + extraWidth < maxWidth)
				{
					return i;
				}
			}

			return count-1;
		}

		private static double MeasureMenuItems(int variant, IEnumerable<MenuItem> menuItems)
		{
			double width = 0;

			foreach (var item in menuItems)
			{
				width += item.Item1.GetTextAdvance (item.GetText (variant));
			}

			return width * Font.DefaultFontSize;
		}

		private void PaintFocusRectangle(Graphics graphics, Rectangle bounds)
		{
			if ((this.IsFocused) ||
				(this.ContainsKeyboardFocus))
			{
				graphics.LineJoin = JoinStyle.Miter;
				graphics.LineWidth = 1.0;
				graphics.AddRectangle (Rectangle.Deflate (bounds, 0.5, 0.5));
				graphics.RenderSolid (SlimField.Colors.FocusColor);
			}
		}

		private void PaintBackgroundSurface(Graphics graphics, Rectangle bounds)
		{
			if (this.IsReadOnly)
			{
				return;
			}

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

			var geomPrefix = new TextGeometry (x, y, width, height, this.FieldPrefix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize, ContentAlignment.MiddleLeft, graphics, GridSnapping.Both);
			var geomText   = new TextGeometry (geomPrefix.Origin.X + geomPrefix.Width, geomPrefix.Origin.Y, width, height, this.FieldText, SlimField.Fonts.TextFont, Font.DefaultFontSize, ContentAlignment.BaselineLeft);
			var geomSuffix = new TextGeometry (geomText.Origin.X + geomText.Width, geomText.Origin.Y, width, height, this.FieldSuffix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize, ContentAlignment.BaselineLeft);

			graphics.Color = SlimField.Colors.TextColor;
			graphics.PaintText (geomPrefix, this.FieldPrefix, SlimField.Fonts.DescriptionFont, Font.DefaultFontSize);

			if (this.GetActiveDisplayMode () == SlimFieldDisplayMode.Text)
			{
				graphics.PaintText (geomText, this.FieldText, SlimField.Fonts.TextFont, Font.DefaultFontSize);
			}

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

			foreach (var tuple in this.GetMenuItems ())
			{
				var font    = tuple.Item1;
				var text    = tuple.Item2;
				var item    = tuple.Item3;
				var hilite  = item == null ? SlimFieldMenuItemHilite.None : item.Hilite;
				var advance = font.GetTextAdvance (text) * Font.DefaultFontSize;
				var color   = item == null ? SlimField.Colors.TextColor : item.Enable == EnableState.Enabled ? SlimField.Colors.TextColor : SlimField.Colors.DisabledColor;

				graphics.Color = color;
				graphics.PaintText (x, y, width, height, text, font, Font.DefaultFontSize, Drawing.ContentAlignment.BaselineLeft);

				if ((hilite == SlimFieldMenuItemHilite.Underline) &&
					(item.Style != SlimFieldMenuItemStyle.Symbol))
				{
					graphics.LineCap = CapStyle.Butt;
					graphics.LineWidth = 1.0;
					graphics.AddLine (x, y - 1.5, x + advance, y - 1.5);
					graphics.RenderSolid (color);
				}

				x     += advance;
				width -= advance;
			}
		}

		public double MeasureWidth(SlimFieldDisplayMode displayMode)
		{
			double width = 2 * SlimField.MarginX;

			switch (displayMode)
			{
				case SlimFieldDisplayMode.Label:
				case SlimFieldDisplayMode.LabelEdition:
					width += 6;
					width += SlimField.Fonts.LabelFont.GetTextAdvance (this.FieldLabel) * Font.DefaultFontSize;
					break;

				case SlimFieldDisplayMode.MeasureTextOnly:
					width += SlimField.Fonts.TextFont.GetTextAdvance (this.FieldText) * Font.DefaultFontSize;
					break;

				case SlimFieldDisplayMode.Text:
				case SlimFieldDisplayMode.TextEdition:
					width += SlimField.Fonts.DescriptionFont.GetTextAdvance (this.FieldPrefix) * Font.DefaultFontSize;
					width += SlimField.Fonts.TextFont.GetTextAdvance (this.FieldText) * Font.DefaultFontSize;
					width += SlimField.Fonts.DescriptionFont.GetTextAdvance (this.FieldSuffix) * Font.DefaultFontSize;
					break;

				case SlimFieldDisplayMode.MeasureTextPrefix:
					width = SlimField.Fonts.DescriptionFont.GetTextAdvance (this.FieldPrefix) * Font.DefaultFontSize;
					break;

				case SlimFieldDisplayMode.MeasureTextSuffix:
					width = SlimField.Fonts.DescriptionFont.GetTextAdvance (this.FieldSuffix) * Font.DefaultFontSize;
					break;

				case SlimFieldDisplayMode.Menu:
					foreach (var tuple in this.GetMenuItems ())
					{
						var font = tuple.Item1;
						var text = tuple.Item2;

						width += font.GetTextAdvance (text) * Font.DefaultFontSize;
					}
					break;
			}

			return width;
		}

		private IEnumerable<MenuItem> GetMenuItems()
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
					yield return new MenuItem (SlimField.Fonts.MenuFont, SlimField.Strings.MenuSeparator);
				}

				yield return new MenuItem (item);
			}
		}





		private SlimFieldMenuItem DetectMenuItem(double advance)
		{
			foreach (var tuple in this.GetMenuItems ())
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

		
		static SlimField()
		{
			Visual.PreferredWidthProperty.OverrideMetadataDefaultValue<SlimField> (18.0);
			Visual.MarginsProperty.OverrideMetadataDefaultValue<SlimField> (new Margins (0, 0, 0, 1));
			Visual.HorizontalAlignmentProperty.OverrideMetadataDefaultValue<SlimField> (HorizontalAlignment.Left);
		}


		private class MenuItem : System.Tuple<Font, string, SlimFieldMenuItem>
		{
			public MenuItem(Font font, string text)
				: base (font, text, null)
			{
			}
			
			public MenuItem(SlimFieldMenuItem item)
				: base (SlimField.GetMenuItemFont (item), item.Texts.FirstOrDefault (), item)
			{
			}

			public string GetText(int variant)
			{
				var texts = this.Item3.Texts;
				var count = texts.Count;

				if (variant < count)
				{
					return texts[variant];
				}
				else
				{
					return texts[count-1];
				}
			}
		}

		private static class Strings
		{
			public static readonly string		MenuSeparator = "  ";
		}

		private static class Colors
		{
			public static readonly Color		BackColor     = Color.FromHexa ("ffffff");
			public static readonly Color		LabelColor    = Color.FromHexa ("3399ff");
			public static readonly Color		FocusColor    = Color.FromHexa ("3399ff");
			public static readonly Color		TextColor     = Color.FromHexa ("000000");
			public static readonly Color		DisabledColor = Color.FromHexa ("cccccc");
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


		public event EventHandler<DependencyPropertyChangedEventArgs> DisplayModeChanged
		{
			add
			{
				this.AddEventHandler (SlimField.DisplayModeProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (SlimField.DisplayModeProperty, value);
			}
		}


		private const int						MarginX = 3;
		private const int						MarginY = 2;


		public static DependencyProperty DisplayModeProperty = DependencyProperty<SlimField>.Register<SlimFieldDisplayMode> (x => x.DisplayMode);
		public static DependencyProperty IsReadOnlyProperty = DependencyProperty<SlimField>.Register<bool> (x => x.IsReadOnly);


		private readonly List<SlimFieldMenuItem> menuItems;
	}
}
