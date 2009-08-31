//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public static Widgets.Widget CreateIconAndText(string iconName, string message)
		{
			Widgets.Widget container = new Widgets.Widget ();
			Widgets.StaticImage widgetIcon = new Widgets.StaticImage (container, iconName);
			Widgets.StaticText  widgetText = new Widgets.StaticText (container, message);
			
			widgetIcon.PreferredSize = new Drawing.Size (48, 48);
			widgetIcon.Anchor        = Widgets.AnchorStyles.TopLeft;
			widgetIcon.Margins = new Drawing.Margins (0, 0, 0, 0);
			
			double minWidth = System.Math.Min (400, widgetText.TextLayout.SingleLineSize.Width+4);
			
			widgetText.TextLayout.LayoutSize = new Drawing.Size (minWidth, Widgets.TextLayout.Infinite);
			
			double width  = widgetText.TextLayout.LayoutSize.Width;
			double height = System.Math.Ceiling (widgetText.TextLayout.TotalRectangle.Height + 4);
			
			widgetText.TextBreakMode = Drawing.TextBreakMode.Hyphenate;
			widgetText.Anchor        = Widgets.AnchorStyles.All;
			widgetText.Margins = new Drawing.Margins (widgetIcon.PreferredWidth + 8, 0, 0, 0);
			
			container.PreferredSize = new Drawing.Size (widgetIcon.PreferredWidth + 8 + width, System.Math.Max (widgetIcon.PreferredHeight, height));
			
			return container;
		}
	}
}
