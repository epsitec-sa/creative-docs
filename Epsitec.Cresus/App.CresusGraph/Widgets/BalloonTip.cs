//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class BalloonTip : FrameBox
	{
		public BalloonTip()
		{
		}

		public ButtonMarkDisposition Disposition
		{
			get;
			set;
		}

		public Point TipAttachment
		{
			get;
			set;
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			var bounds  = this.Client.Bounds;
			var textBox = BalloonTip.GetTextBox (bounds, this.Disposition);

			using (Path path = BalloonTip.GetBalloonPath (bounds, this.TipAttachment, this.Disposition))
			{
				graphics.Color = this.BackColor;
				graphics.PaintSurface (path);
			}

			bounds = Rectangle.Deflate (bounds, new Margins (0.5, 0.5, 0.5, 0.5));

			using (Path path = BalloonTip.GetBalloonPath (bounds, this.TipAttachment, this.Disposition))
			{
				graphics.Color = adorner.ColorBorder;
				graphics.PaintOutline (path);
			}

			if (!string.IsNullOrEmpty (this.Text))
			{
				this.TextLayout.Paint (textBox.Location, graphics);
			}
		}

		protected override Size GetTextLayoutSize()
		{
			if (this.IsActualGeometryValid)
			{
				return BalloonTip.GetTextBox (this.Client.Bounds, this.Disposition).Size;
			}
			else
			{
				return BalloonTip.GetTextBox (new Rectangle (0, 0, this.PreferredWidth, this.PreferredHeight), this.Disposition).Size;
			}
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			base.ProcessMessage (message, pos);

			message.Handled = true;
		}
		
		private static Path GetBalloonPath(Rectangle bounds, Point attachment, ButtonMarkDisposition disposition)
		{
			Path path = new Path ();

			var r = BalloonTip.Radius;
			var d = BalloonTip.AttachmentLength;
			var m = new Margins ();
			var x = attachment.X;
			var y = attachment.Y;

			switch (disposition)
			{
				case ButtonMarkDisposition.Below:
					m = new Margins (0, 0, d, 0);
					x = System.Math.Max (2*r, System.Math.Min (x, bounds.Width-2*r));
					break;

				case ButtonMarkDisposition.Above:
					m = new Margins (0, 0, 0, d);
					x = System.Math.Max (2*r, System.Math.Min (x, bounds.Width-2*r));
					break;

				case ButtonMarkDisposition.Left:
				case ButtonMarkDisposition.Right:
					throw new System.NotImplementedException ();
			}

			bounds = Rectangle.Deflate (bounds, m);

			path.MoveTo (bounds.Left, bounds.Top-r);
			path.LineTo (bounds.Left, bounds.Bottom+r);
			path.ArcTo (bounds.Left, bounds.Bottom, bounds.Left+r, bounds.Bottom);
			
			if (disposition == ButtonMarkDisposition.Above)
			{
				path.LineTo (x-d/3, bounds.Bottom);
				path.LineTo (x, bounds.Bottom-d);
				path.LineTo (x+d/3, bounds.Bottom);
			}

			path.LineTo (bounds.Right-r, bounds.Bottom);
			path.ArcTo (bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom+r);
			path.LineTo (bounds.Right, bounds.Top-r);
			path.ArcTo (bounds.Right, bounds.Top, bounds.Right-r, bounds.Top);

			if (disposition == ButtonMarkDisposition.Below)
			{
				path.LineTo (x+d/3, bounds.Top);
				path.LineTo (x, bounds.Top+d);
				path.LineTo (x-d/3, bounds.Top);
			}

			path.LineTo (bounds.Left+r, bounds.Top);
			path.ArcTo (bounds.Left, bounds.Top, bounds.Left, bounds.Top-r);
			path.Close ();
			
			return path;
		}
		
		private static Rectangle GetTextBox(Rectangle bounds, ButtonMarkDisposition disposition)
		{
			var r = BalloonTip.Radius;
			var d = BalloonTip.AttachmentLength;
			var m = new Margins ();
			
			switch (disposition)
			{
				case ButtonMarkDisposition.Below:
					m = new Margins (r, r, d+r, r);
					break;

				case ButtonMarkDisposition.Above:
					m = new Margins (r, r, r, d+r);
					break;

				case ButtonMarkDisposition.Left:
				case ButtonMarkDisposition.Right:
					throw new System.NotImplementedException ();
			}

			return Rectangle.Deflate (bounds, m);
		}

		public static Rectangle GetBestPosition(Size expectedSize, Rectangle bounds, Size containerSize, ref ButtonMarkDisposition disposition)
		{
			double cx;
			double cy;

			switch (disposition)
			{
				case ButtonMarkDisposition.Below:
				case ButtonMarkDisposition.None:
					disposition = (bounds.Bottom < expectedSize.Height) ? ButtonMarkDisposition.Above : ButtonMarkDisposition.Below;
					break;

				case ButtonMarkDisposition.Above:
					disposition = (bounds.Top+expectedSize.Height < containerSize.Height) ?	ButtonMarkDisposition.Above : ButtonMarkDisposition.Below;
					break;

				case ButtonMarkDisposition.Left:
				case ButtonMarkDisposition.Right:
					throw new System.NotImplementedException ();
			}

			switch (disposition)
			{
				case ButtonMarkDisposition.Above:
					cx = bounds.Center.X;
					cx = (cx+expectedSize.Width/2 > containerSize.Width) ? containerSize.Width-expectedSize.Width/2 : cx;
					cx = (cx-expectedSize.Width/2 < 0) ? 0 : cx-expectedSize.Width/2;
					cy = bounds.Top;
					return new Rectangle (cx, cy, expectedSize.Width, expectedSize.Height);

				case ButtonMarkDisposition.Below:
					cx = bounds.Center.X;
					cx = (cx+expectedSize.Width/2 > containerSize.Width) ? containerSize.Width-expectedSize.Width/2 : cx;
					cx = (cx-expectedSize.Width/2 < 0) ? 0 : cx-expectedSize.Width/2;
					cy = bounds.Bottom-expectedSize.Height;
					return new Rectangle (cx, cy, expectedSize.Width, expectedSize.Height);
			}

			throw new System.NotImplementedException ();
		}

		private const double Radius = 5.0;
		private const double AttachmentLength = 9.0;
	}
}
