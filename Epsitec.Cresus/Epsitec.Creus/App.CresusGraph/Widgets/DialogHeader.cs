//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class DialogHeader : FrameBox
	{
		public DialogHeader()
		{
			this.PreferredHeight = 92;
			this.Dock = DockStyle.Top;
			this.BackColor = Color.FromBrightness (1);
			this.image = new StaticImage ()
			{
				Parent = this,
				Anchor = AnchorStyles.TopRight,
				Margins = new Margins (24, 24, 20, 20),
				PreferredSize = new Size (80, 56),
				ImageSize = new Size (80, 56),
				ImageName = "manifest:Epsitec.Cresus.Graph.Images.HeaderLogo.png",
			};
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
			var size = 28.0;

			graphics.Color = adorner.ColorCaption;
			graphics.PaintText (24, 32, this.Text, font, size);
		}

		private readonly StaticImage image;
	}
}
