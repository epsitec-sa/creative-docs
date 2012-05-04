//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass (typeof (OptionTab))]


namespace Epsitec.Common.Widgets
{
	public class OptionTab : Widget
	{
		public OptionTab()
		{
			this.AutoCapture = true;
			this.AutoFocus   = false;
			this.Padding     = new Margins (6, 22, 6, 4);
		}

		
		private bool							HiliteCloseGlyph
		{
			get
			{
				return this.hiliteCloseGlyph;
			}
			set
			{
				if (this.hiliteCloseGlyph != value)
				{
					this.hiliteCloseGlyph = value;
					this.Invalidate ();
				}
			}
		}

		
		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.IsMouseType)
			{
				var closeBounds = this.GetCloseGlyphBounds ();

				if ((message.MessageType != MessageType.MouseLeave) &
					(closeBounds.Contains (pos)))
				{
					this.HiliteCloseGlyph = true;
				}
				else
				{
					this.HiliteCloseGlyph = false;
				}
			}

			base.ProcessMessage (message, pos);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintBody (graphics);
			this.PaintCloseGlyph (graphics);
		}

		
		private void PaintBody(Graphics graphics)
		{
			var rect = this.Client.Bounds;

			graphics.LineCap = CapStyle.Butt;
			graphics.LineJoin = JoinStyle.MiterRound;
			graphics.FillMode = Drawing.FillMode.NonZero;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));

			using (var path = OptionTab.CreateOutlinePath (rect, 1.0, close: true))
			{
				path.Close ();
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromBrightness (0.92));
			}

			using (var path = OptionTab.CreateOutlinePath (rect, 1.0, close: false))
			{
				graphics.LineWidth = 1.0;
				graphics.AddPath (path);
				graphics.RenderGradient (rect, Color.FromBrightness (0.7), Color.FromBrightness (0.5), GradientFill.X);
			}
		}
		
		private void PaintCloseGlyph(Graphics graphics)
		{
			using (var path = OptionTab.CreateCloseGlyphPath (this.GetCloseGlyphBounds ()))
			{
				if (this.HiliteCloseGlyph)
				{
					graphics.LineWidth = 2.8;
					graphics.LineCap = CapStyle.Butt;
					graphics.AddPath (path);
					graphics.RenderSolid (Color.FromBrightness (0.08));
				}
				else
				{
					graphics.LineWidth = 2.5;
					graphics.LineCap = CapStyle.Butt;
					graphics.AddPath (path);
					graphics.RenderSolid (Color.FromBrightness (0.16));
				}
			}
		}
		
		private Rectangle GetCloseGlyphBounds()
		{
			var frame  = this.Client.Bounds;
			var center = new Point (frame.Right - 12, System.Math.Max (System.Math.Floor (frame.Center.Y), frame.Top - 16));

			return new Rectangle (center.X - 6, center.Y - 6, 12, 12);
		}

		private static Path CreateCloseGlyphPath(Rectangle bounds)
		{
			var path = new Path ();

			path.MoveTo (bounds.Center);

			path.MoveToRelative (-4,  4);
			path.LineToRelative ( 8, -8);
			path.MoveToRelative (-8,  0);
			path.LineToRelative ( 8,  8);
			
			return path;
		}

		private static Path CreateOutlinePath(Rectangle frame, double lineWidth, bool close)
		{
			var path = new Path ();
			
			var hw = lineWidth / 2;
			var dy = frame.Height;
			var dx = frame.Width;

			var rx = lineWidth * 3;
			var ry = lineWidth * 3;

			path.MoveTo (frame.Left, frame.Top - hw);
			
			path.MoveToRelative  (hw,           0);
			path.CurveToRelative (rx,           0,
				/**/			   0,           -ry);
			path.LineToRelative  ( 0,           -dy+2*ry+2*hw);
			path.CurveToRelative ( 0,           -ry,
				/**/			  rx,           0);
			path.LineToRelative  (dx-4*rx-2*hw, 0);
			path.CurveToRelative (rx,           0,
				/**/			   0,           ry);
			path.LineToRelative  ( 0,           dy-2*ry-2*hw);
			path.CurveToRelative ( 0,           ry,
				/**/			  rx,           0);
			path.MoveToRelative  (hw,           0);

			if (close)
			{
				path.LineToRelative (  0,  hw);
				path.LineToRelative (-dx,   0);
				path.LineToRelative (  0, -hw);
				path.Close ();
			}

			return path;
		}


		private bool							hiliteCloseGlyph;
	}
}
