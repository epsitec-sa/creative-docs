//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Widgets
{
	public class WindowTitle : FrameBox
	{
		public WindowTitle()
		{
			this.PreferredHeight = 92;
			this.Dock = DockStyle.Top;
			this.BackColor = Color.FromBrightness (1);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					this.Window.StartWindowManagerOperation (Epsitec.Common.Widgets.Platform.WindowManagerOperation.MoveWindow);
					break;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var adorner  = Epsitec.Common.Widgets.Adorners.Factory.Active;

			graphics.Color = this.BackColor;
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.RenderSolid ();

			graphics.Color = adorner.ColorBorder;
			graphics.LineWidth = 1.0;
			graphics.LineCap = CapStyle.Butt;
			graphics.AddLine (0, 0.5, this.Client.Size.Width, 0.5);
			graphics.RenderSolid ();

			var font = Font.DefaultFont;
			var size = 14.0;

			graphics.Color = adorner.ColorCaption;
			graphics.PaintText (24, 32, this.Text, font, size);
		}
	}
}
