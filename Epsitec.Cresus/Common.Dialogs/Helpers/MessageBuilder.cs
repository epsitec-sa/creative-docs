//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs.Helpers
{
	/// <summary>
	/// Summary description for MessageBuilder.
	/// </summary>
	public class MessageBuilder
	{
		public static Widget CreateAlert(string message)
		{
			return null;
		}
		
		public static Widget CreateIconAndText(string iconUri, string message)
		{
			Widget container = new Widget ();
			StaticImage widgetIcon = new StaticImage (container, iconUri);
			StaticText  widgetText = new StaticText (container, message);
			
			widgetIcon.PreferredSize = new Drawing.Size (48, 48);
			widgetIcon.Anchor        = AnchorStyles.TopLeft;
			widgetIcon.Margins = new Drawing.Margins (0, 0, 0, 0);
			
			double minWidth = System.Math.Min (400, widgetText.TextLayout.SingleLineSize.Width+4);
			
			widgetText.TextLayout.LayoutSize = new Drawing.Size (minWidth, TextLayout.Infinite);
			
			double width  = widgetText.TextLayout.LayoutSize.Width;
			double height = System.Math.Ceiling (widgetText.TextLayout.TotalRectangle.Height + 4);
			
			widgetText.TextBreakMode = Drawing.TextBreakMode.Hyphenate;
			widgetText.Anchor        = AnchorStyles.All;
			widgetText.Margins = new Drawing.Margins (widgetIcon.PreferredWidth + 8, 0, 0, 0);
			
			container.PreferredSize = new Drawing.Size (widgetIcon.PreferredWidth + 8 + width, System.Math.Max (widgetIcon.PreferredHeight, height));
			
			return container;
		}
	}
}
