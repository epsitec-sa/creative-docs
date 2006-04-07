//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs.Helpers
{
	/// <summary>
	/// Summary description for MessageBuilder.
	/// </summary>
	public class MessageBuilder
	{
		public static Widgets.Widget CreateAlert(string message)
		{
			return null;
		}
		
		public static Widgets.Widget CreateIconAndText(string icon_name, string message)
		{
			Widgets.Widget container = new Widgets.Widget ();
			Widgets.StaticImage widget_icon = new Widgets.StaticImage (container, icon_name);
			Widgets.StaticText  widget_text = new Widgets.StaticText (container, message);
			
			widget_icon.Size          = new Drawing.Size (48, 48);
			widget_icon.Anchor        = Widgets.AnchorStyles.TopLeft;
			widget_icon.Margins = new Drawing.Margins (0, 0, 0, 0);
			
			double min_width = System.Math.Min (400, widget_text.TextLayout.SingleLineSize.Width+4);
			
			widget_text.TextLayout.LayoutSize = new Drawing.Size (min_width, Widgets.TextLayout.Infinite);
			
			double width  = widget_text.TextLayout.LayoutSize.Width;
			double height = System.Math.Ceiling (widget_text.TextLayout.TotalRectangle.Height + 4);
			
			widget_text.TextBreakMode = Drawing.TextBreakMode.Hyphenate;
			widget_text.Anchor        = Widgets.AnchorStyles.All;
			widget_text.Margins = new Drawing.Margins (widget_icon.Width + 8, 0, 0, 0);
			
			container.Size = new Drawing.Size (widget_icon.Width + 8 + width, System.Math.Max (widget_icon.Height, height));
			
			return container;
		}
	}
}
