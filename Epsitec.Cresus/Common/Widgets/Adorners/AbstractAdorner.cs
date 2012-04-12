//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe AbstractAdorner définit les méthodes et propriétés communes
	/// aux divers adorners. Ceci permet d'éviter la multiplication du code
	/// pour les réglages partagés par beaucoup d'adorners.
	/// </summary>
	public abstract class AbstractAdorner : IAdorner
	{
		protected AbstractAdorner()
		{
			this.RefreshColors();
		}


		protected static void PaintGlyphDefaultImplementation(Drawing.Rectangle rect, GlyphShape type, Drawing.Point center, Drawing.Path path)
		{
			switch (type)
			{
				case GlyphShape.Lock:
					AbstractAdorner.DrawGlyphShapeLook (path, rect, center);
					break;
				case GlyphShape.ArrowUp:
					path.MoveTo (center.X + 0.0 * rect.Width, center.Y + 0.2 * rect.Height);
					path.LineTo (center.X - 0.3 * rect.Width, center.Y - 0.1 * rect.Height);
					path.LineTo (center.X - 0.2 * rect.Width, center.Y - 0.2 * rect.Height);
					path.LineTo (center.X + 0.0 * rect.Width, center.Y + 0.0 * rect.Height);
					path.LineTo (center.X + 0.2 * rect.Width, center.Y - 0.2 * rect.Height);
					path.LineTo (center.X + 0.3 * rect.Width, center.Y - 0.1 * rect.Height);
					break;
				case GlyphShape.ArrowDown:
					path.MoveTo (center.X + 0.0 * rect.Width, center.Y - 0.2 * rect.Height);
					path.LineTo (center.X - 0.3 * rect.Width, center.Y + 0.1 * rect.Height);
					path.LineTo (center.X - 0.2 * rect.Width, center.Y + 0.2 * rect.Height);
					path.LineTo (center.X + 0.0 * rect.Width, center.Y - 0.0 * rect.Height);
					path.LineTo (center.X + 0.2 * rect.Width, center.Y + 0.2 * rect.Height);
					path.LineTo (center.X + 0.3 * rect.Width, center.Y + 0.1 * rect.Height);
					break;
				case GlyphShape.ArrowRight:
					path.MoveTo (center.X + 0.2 * rect.Width, center.Y + 0.0 * rect.Height);
					path.LineTo (center.X - 0.1 * rect.Width, center.Y - 0.3 * rect.Height);
					path.LineTo (center.X - 0.2 * rect.Width, center.Y - 0.2 * rect.Height);
					path.LineTo (center.X + 0.0 * rect.Width, center.Y + 0.0 * rect.Height);
					path.LineTo (center.X - 0.2 * rect.Width, center.Y + 0.2 * rect.Height);
					path.LineTo (center.X - 0.1 * rect.Width, center.Y + 0.3 * rect.Height);
					break;
				case GlyphShape.ArrowLeft:
					path.MoveTo (center.X - 0.2 * rect.Width, center.Y + 0.0 * rect.Height);
					path.LineTo (center.X + 0.1 * rect.Width, center.Y - 0.3 * rect.Height);
					path.LineTo (center.X + 0.2 * rect.Width, center.Y - 0.2 * rect.Height);
					path.LineTo (center.X - 0.0 * rect.Width, center.Y + 0.0 * rect.Height);
					path.LineTo (center.X + 0.2 * rect.Width, center.Y + 0.2 * rect.Height);
					path.LineTo (center.X + 0.1 * rect.Width, center.Y + 0.3 * rect.Height);
					break;
				case GlyphShape.TriangleUp:
					path.MoveTo (center.X, center.Y + rect.Height * 0.2);
					path.LineTo (center.X - rect.Width * 0.3, center.Y - rect.Height * 0.1);
					path.LineTo (center.X + rect.Width * 0.3, center.Y - rect.Height * 0.1);
					break;
				case GlyphShape.TriangleDown:
					path.MoveTo (center.X, center.Y - rect.Height * 0.2);
					path.LineTo (center.X - rect.Width * 0.3, center.Y + rect.Height * 0.1);
					path.LineTo (center.X + rect.Width * 0.3, center.Y + rect.Height * 0.1);
					break;
				case GlyphShape.TriangleRight:
					path.MoveTo (center.X + rect.Width * 0.2, center.Y);
					path.LineTo (center.X - rect.Width * 0.1, center.Y + rect.Height * 0.3);
					path.LineTo (center.X - rect.Width * 0.1, center.Y - rect.Height * 0.3);
					break;
				case GlyphShape.TriangleLeft:
					path.MoveTo (center.X - rect.Width * 0.2, center.Y);
					path.LineTo (center.X + rect.Width * 0.1, center.Y + rect.Height * 0.3);
					path.LineTo (center.X + rect.Width * 0.1, center.Y - rect.Height * 0.3);
					break;
				case GlyphShape.HorizontalMove:
					path.MoveTo (center.X - rect.Width * 0.3, center.Y);
					path.LineTo (center.X - rect.Width * 0.05, center.Y + rect.Height * 0.3);
					path.LineTo (center.X - rect.Width * 0.05, center.Y - rect.Height * 0.3);
					path.Close ();
					path.MoveTo (center.X + rect.Width * 0.3, center.Y);
					path.LineTo (center.X + rect.Width * 0.05, center.Y + rect.Height * 0.3);
					path.LineTo (center.X + rect.Width * 0.05, center.Y - rect.Height * 0.3);
					break;
				case GlyphShape.VerticalMove:
					path.MoveTo (center.X, center.Y - rect.Height * 0.3);
					path.LineTo (center.X - rect.Width * 0.3, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.3, center.Y - rect.Height * 0.05);
					path.Close ();
					path.MoveTo (center.X, center.Y + rect.Height * 0.3);
					path.LineTo (center.X - rect.Width * 0.3, center.Y + rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.3, center.Y + rect.Height * 0.05);
					break;
				case GlyphShape.Menu:
					path.MoveTo (center.X + rect.Width * 0.00, center.Y - rect.Height * 0.25);
					path.LineTo (center.X - rect.Width * 0.30, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.30, center.Y + rect.Height * 0.15);
					break;
				case GlyphShape.Close:
				case GlyphShape.Reject:
					path.MoveTo (center.X - rect.Width * 0.20, center.Y - rect.Height * 0.30);
					path.LineTo (center.X - rect.Width * 0.30, center.Y - rect.Height * 0.20);
					path.LineTo (center.X - rect.Width * 0.10, center.Y + rect.Height * 0.00);
					path.LineTo (center.X - rect.Width * 0.30, center.Y + rect.Height * 0.20);
					path.LineTo (center.X - rect.Width * 0.20, center.Y + rect.Height * 0.30);
					path.LineTo (center.X - rect.Width * 0.00, center.Y + rect.Height * 0.10);
					path.LineTo (center.X + rect.Width * 0.20, center.Y + rect.Height * 0.30);
					path.LineTo (center.X + rect.Width * 0.30, center.Y + rect.Height * 0.20);
					path.LineTo (center.X + rect.Width * 0.10, center.Y + rect.Height * 0.00);
					path.LineTo (center.X + rect.Width * 0.30, center.Y - rect.Height * 0.20);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.30);
					path.LineTo (center.X + rect.Width * 0.00, center.Y - rect.Height * 0.10);
					break;
				case GlyphShape.Dots:
					path.MoveTo (center.X - rect.Width * 0.30, center.Y + rect.Height * 0.06);
					path.LineTo (center.X - rect.Width * 0.18, center.Y + rect.Height * 0.06);
					path.LineTo (center.X - rect.Width * 0.18, center.Y - rect.Height * 0.06);
					path.LineTo (center.X - rect.Width * 0.30, center.Y - rect.Height * 0.06);
					path.Close ();
					path.MoveTo (center.X - rect.Width * 0.06, center.Y + rect.Height * 0.06);
					path.LineTo (center.X + rect.Width * 0.06, center.Y + rect.Height * 0.06);
					path.LineTo (center.X + rect.Width * 0.06, center.Y - rect.Height * 0.06);
					path.LineTo (center.X - rect.Width * 0.06, center.Y - rect.Height * 0.06);
					path.Close ();
					path.MoveTo (center.X + rect.Width * 0.18, center.Y + rect.Height * 0.06);
					path.LineTo (center.X + rect.Width * 0.30, center.Y + rect.Height * 0.06);
					path.LineTo (center.X + rect.Width * 0.30, center.Y - rect.Height * 0.06);
					path.LineTo (center.X + rect.Width * 0.18, center.Y - rect.Height * 0.06);
					break;
				case GlyphShape.Accept:
					path.MoveTo (center.X - rect.Width * 0.30, center.Y + rect.Height * 0.00);
					path.LineTo (center.X - rect.Width * 0.20, center.Y + rect.Height * 0.10);
					path.LineTo (center.X - rect.Width * 0.10, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.20, center.Y + rect.Height * 0.30);
					path.LineTo (center.X + rect.Width * 0.30, center.Y + rect.Height * 0.20);
					path.LineTo (center.X - rect.Width * 0.10, center.Y - rect.Height * 0.30);
					break;
				case GlyphShape.TabLeft:
					path.MoveTo (center.X - rect.Width * 0.10, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.00, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.00, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.15);
					path.LineTo (center.X - rect.Width * 0.10, center.Y - rect.Height * 0.15);
					break;
				case GlyphShape.TabRight:
					path.MoveTo (center.X + rect.Width * 0.00, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.10, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.10, center.Y - rect.Height * 0.15);
					path.LineTo (center.X - rect.Width * 0.20, center.Y - rect.Height * 0.15);
					path.LineTo (center.X - rect.Width * 0.20, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.00, center.Y - rect.Height * 0.05);
					break;
				case GlyphShape.TabCenter:
					path.MoveTo (center.X - rect.Width * 0.05, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.05, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.05, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.15);
					path.LineTo (center.X - rect.Width * 0.20, center.Y - rect.Height * 0.15);
					path.LineTo (center.X - rect.Width * 0.20, center.Y - rect.Height * 0.05);
					path.LineTo (center.X - rect.Width * 0.05, center.Y - rect.Height * 0.05);
					break;
				case GlyphShape.TabDecimal:
					path.MoveTo (center.X - rect.Width * 0.05, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.05, center.Y + rect.Height * 0.15);
					path.LineTo (center.X + rect.Width * 0.05, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.05);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.15);
					path.LineTo (center.X - rect.Width * 0.20, center.Y - rect.Height * 0.15);
					path.LineTo (center.X - rect.Width * 0.20, center.Y - rect.Height * 0.05);
					path.LineTo (center.X - rect.Width * 0.05, center.Y - rect.Height * 0.05);
					path.Close ();
					path.MoveTo (center.X + rect.Width * 0.10, center.Y + rect.Height * 0.10);
					path.LineTo (center.X + rect.Width * 0.20, center.Y + rect.Height * 0.10);
					path.LineTo (center.X + rect.Width * 0.20, center.Y + rect.Height * 0.00);
					path.LineTo (center.X + rect.Width * 0.10, center.Y + rect.Height * 0.00);
					break;
				case GlyphShape.TabIndent:
					path.MoveTo (center.X - rect.Width * 0.10, center.Y + rect.Height * 0.20);
					path.LineTo (center.X + rect.Width * 0.20, center.Y - rect.Height * 0.00);
					path.LineTo (center.X - rect.Width * 0.10, center.Y - rect.Height * 0.20);
					break;
				case GlyphShape.Plus:
					path.MoveTo (center.X - rect.Width * 0.29, center.Y + rect.Height * 0.07);
					path.LineTo (center.X - rect.Width * 0.07, center.Y + rect.Height * 0.07);
					path.LineTo (center.X - rect.Width * 0.07, center.Y + rect.Height * 0.29);
					path.LineTo (center.X + rect.Width * 0.07, center.Y + rect.Height * 0.29);
					path.LineTo (center.X + rect.Width * 0.07, center.Y + rect.Height * 0.07);
					path.LineTo (center.X + rect.Width * 0.29, center.Y + rect.Height * 0.07);
					path.LineTo (center.X + rect.Width * 0.29, center.Y - rect.Height * 0.07);
					path.LineTo (center.X + rect.Width * 0.07, center.Y - rect.Height * 0.07);
					path.LineTo (center.X + rect.Width * 0.07, center.Y - rect.Height * 0.29);
					path.LineTo (center.X - rect.Width * 0.07, center.Y - rect.Height * 0.29);
					path.LineTo (center.X - rect.Width * 0.07, center.Y - rect.Height * 0.07);
					path.LineTo (center.X - rect.Width * 0.29, center.Y - rect.Height * 0.07);
					break;
				case GlyphShape.Minus:
					path.MoveTo (center.X - rect.Width * 0.29, center.Y + rect.Height * 0.07);
					path.LineTo (center.X + rect.Width * 0.29, center.Y + rect.Height * 0.07);
					path.LineTo (center.X + rect.Width * 0.29, center.Y - rect.Height * 0.07);
					path.LineTo (center.X - rect.Width * 0.29, center.Y - rect.Height * 0.07);
					break;
			}
		}

		protected virtual void RefreshColors()
		{
			this.colorBlack             = Drawing.Color.FromBrightness(0);
			this.colorWhite             = Drawing.Color.FromBrightness(1);
			this.colorControl           = Drawing.Color.FromName("Control");
			this.colorControlLight      = Drawing.Color.FromName("ControlLight");
			this.colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			this.colorControlDark       = Drawing.Color.FromName("ControlDark");
			this.colorControlDarkDark   = Drawing.Color.FromName("ControlDarkDark");
			this.colorCaption           = Drawing.Color.FromName("ActiveCaption");
			this.colorCaptionText       = Drawing.Color.FromName("ActiveCaptionText");
			this.colorInfo              = Drawing.Color.FromName("Info");
		}


		protected static void DrawFocusedRectangle(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Color color)
		{
			//	Dessine un rectangle pointillé correspondant à un widget ayant le focus.
			rect.Deflate(0.5);
			using (Drawing.Path path = new Epsitec.Common.Drawing.Path (rect))
			{
				AbstractAdorner.DrawPathDash (graphics, path, 1, 0, 2, color);
			}
		}

		protected static void DrawFocusedPath(Drawing.Graphics graphics, Drawing.Path path, Drawing.Color color)
		{
			//	Dessine un chemin pointillé correspondant à un widget ayant le focus.
			AbstractAdorner.DrawPathDash(graphics, path, 1, 0, 2, color);
		}

		protected static void DrawPathDash(Drawing.Graphics graphics, Drawing.Path path, double width, double dash, double gap, Drawing.Color color)
		{
			graphics.PaintDashedOutline (path, width, dash, gap, Drawing.CapStyle.Square, color);
		}

		protected static bool IsThreeState2(Widgets.WidgetPaintState state)
		{
			//	Indique si un IconButton en mode ThreeState est 2 pixels plus haut.
			if ( (state&WidgetPaintState.ThreeState) == 0 )  return false;

			return ( (state&WidgetPaintState.ActiveYes)   != 0 ||
					 (state&WidgetPaintState.ActiveMaybe) != 0 );
		}

		protected static Drawing.Path PathThreeState2Frame(Drawing.Rectangle rect, Widgets.WidgetPaintState state)
		{
			//	Donne le chemin d'un IconButton en mode ThreeState.
			Drawing.Path path = new Drawing.Path();

			if ( AbstractAdorner.IsThreeState2(state) )
			{
				path.MoveTo(rect.Left, rect.Bottom);
				path.LineTo(rect.Left, rect.Top-2);
				path.LineTo(rect.Left+2, rect.Top);
				path.LineTo(rect.Right-2, rect.Top);
				path.LineTo(rect.Right, rect.Top-2);
				path.LineTo(rect.Right, rect.Bottom);
				path.Close();
			}
			else
			{
				path.MoveTo(rect.Left, rect.Bottom);
				path.LineTo(rect.Left, rect.Top);
				path.LineTo(rect.Right, rect.Top);
				path.LineTo(rect.Right, rect.Bottom);
				path.Close();
			}

			return path;
		}

		protected static TextLayout AdaptTextLayout(TextLayout text, TextFieldDisplayMode mode)
		{
			return text;
		}


		protected static void DrawGlyphShapeLook(Drawing.Path path, Drawing.Rectangle rect, Drawing.Point center)
		{
			path.MoveTo (center.X-0.2*rect.Width, center.Y+0.1*rect.Height);
			path.LineTo (center.X-0.2*rect.Width, center.Y+0.2*rect.Height);
			path.CurveTo (center.X-0.2*rect.Width, center.Y+0.33*rect.Height, center.X-0.13*rect.Width, center.Y+0.4*rect.Height, center.X+0.0*rect.Width, center.Y+0.4*rect.Height);
			path.CurveTo (center.X+0.13*rect.Width, center.Y+0.4*rect.Height, center.X+0.2*rect.Width, center.Y+0.33*rect.Height, center.X+0.2*rect.Width, center.Y+0.2*rect.Height);
			path.LineTo (center.X+0.2*rect.Width, center.Y+0.1*rect.Height);
			path.LineTo (center.X+0.3*rect.Width, center.Y+0.1*rect.Height);
			path.LineTo (center.X+0.3*rect.Width, center.Y-0.3*rect.Height);
			path.LineTo (center.X-0.3*rect.Width, center.Y-0.3*rect.Height);
			path.LineTo (center.X-0.3*rect.Width, center.Y+0.1*rect.Height);
			path.Close ();

			path.LineTo (center.X+0.1*rect.Width, center.Y+0.1*rect.Height);
			path.LineTo (center.X+0.1*rect.Width, center.Y+0.2*rect.Height);
			path.CurveTo (center.X+0.1*rect.Width, center.Y+0.27*rect.Height, center.X+0.07*rect.Width, center.Y+0.3*rect.Height, center.X+0.0*rect.Width, center.Y+0.3*rect.Height);
			path.CurveTo (center.X-0.07*rect.Width, center.Y+0.3*rect.Height, center.X-0.1*rect.Width, center.Y+0.27*rect.Height, center.X-0.1*rect.Width, center.Y+0.2*rect.Height);
			path.LineTo (center.X-0.1*rect.Width, center.Y+0.1*rect.Height);
			path.Close ();
		}

		protected static Drawing.Path GetMultilingualFrame(Drawing.Rectangle bounds, bool isMultilingual)
		{
			var path = new Drawing.Path();
			bounds.Deflate (0.5);

			if (isMultilingual)
			{
#if true
				double d = 5;

				path.MoveTo (bounds.BottomLeft);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.TopRight);
				path.LineTo (bounds.Left+d, bounds.Top);
				path.LineTo (bounds.Left, bounds.Top-d);
				path.Close ();
#endif
#if false
				double d = 4;

				path.MoveTo (bounds.BottomLeft);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.TopRight);
				path.LineTo (bounds.TopLeft);
				path.LineTo (bounds.Left, bounds.Center.Y+d);
				path.LineTo (bounds.Left+d, bounds.Center.Y);
				path.LineTo (bounds.Left, bounds.Center.Y-d);
				path.Close ();
#endif
#if false
				path.MoveTo (bounds.Left+3, bounds.Bottom);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.TopRight);
				path.LineTo (bounds.Left+3, bounds.Top);
				path.LineTo (bounds.Left+3, bounds.Top-2);
				path.LineTo (bounds.Left, bounds.Top-2);
				path.LineTo (bounds.Left, bounds.Top-7);
				path.LineTo (bounds.Left+3, bounds.Top-7);
				path.LineTo (bounds.Left+3, bounds.Bottom+7);
				path.LineTo (bounds.Left, bounds.Bottom+7);
				path.LineTo (bounds.Left, bounds.Bottom+2);
				path.LineTo (bounds.Left+3, bounds.Bottom+2);
				path.Close ();

				path.MoveTo (bounds.Left+1, bounds.Top+0.5);
				path.LineTo (bounds.Left+1, bounds.Top-2);

				path.MoveTo (bounds.Left+1, bounds.Bottom+7);
				path.LineTo (bounds.Left+1, bounds.Top-7);

				path.MoveTo (bounds.Left+1, bounds.Bottom-0.5);
				path.LineTo (bounds.Left+1, bounds.Bottom+2);
#endif
			}
			else
			{
				path = Drawing.Path.FromRectangle (bounds);
			}

			return path;
		}

		public static double MultilingualLeftPadding = 0;
		//?public static double MultilingualLeftPadding = 3;
		
		
		#region IAdorner Members
		public abstract void PaintWindowBackground(Drawing.Graphics graphics, Drawing.Rectangle windowRect, Drawing.Rectangle paintRect, Widgets.WidgetPaintState state);
		public abstract void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.GlyphShape type, Widgets.PaintTextStyle style);
		public abstract void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Drawing.Color color, Widgets.GlyphShape type, Widgets.PaintTextStyle style);
		public abstract void PaintCheck(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintRadio(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintIcon(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, string icon);
		public abstract void PaintButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.ButtonStyle style);
		public abstract void PaintButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state, Widgets.ButtonStyle style);
		public abstract void PaintButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.ButtonStyle style);
		
		public virtual void PaintButtonBullet(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Drawing.Color color)
		{
			if (!color.IsEmpty)
			{
				bool enable = ((state & WidgetPaintState.Enabled) != 0);

				Drawing.Rectangle r = rect;
				r.Deflate (3.5);
				r.Width = r.Height;

				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (color);

				graphics.AddRectangle (r);
				graphics.RenderSolid (this.ColorTextFieldBorder (enable));
			}
		}

		public virtual void PaintButtonMark(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, ButtonMarkDisposition markDisposition, double markLength)
		{
			using (Drawing.Path path = new Drawing.Path ())
			{
				bool enable = ((state & WidgetPaintState.Enabled) != 0);

				double middle;
				double factor = 1.0;

				switch (markDisposition)
				{
					case ButtonMarkDisposition.Below:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo (middle, rect.Bottom);
						path.LineTo (middle-markLength*factor, rect.Bottom+markLength);
						path.LineTo (middle+markLength*factor, rect.Bottom+markLength);
						break;

					case ButtonMarkDisposition.Above:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo (middle, rect.Top);
						path.LineTo (middle-markLength*factor, rect.Top-markLength);
						path.LineTo (middle+markLength*factor, rect.Top-markLength);
						break;

					case ButtonMarkDisposition.Left:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo (rect.Left, middle);
						path.LineTo (rect.Left+markLength, middle-markLength*factor);
						path.LineTo (rect.Left+markLength, middle+markLength*factor);
						break;

					case ButtonMarkDisposition.Right:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo (rect.Right, middle);
						path.LineTo (rect.Right-markLength, middle-markLength*factor);
						path.LineTo (rect.Right-markLength, middle+markLength*factor);
						break;
				}
				path.Close ();

				graphics.Color = this.ColorTextFieldBorder (enable);
				graphics.PaintSurface (path);
			}
		}

		public abstract void PaintTextFieldBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.TextFieldStyle style, TextFieldDisplayMode mode, bool readOnly, bool isMultilingual);
		public abstract void PaintTextFieldForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.TextFieldStyle style, TextFieldDisplayMode mode, bool readOnly, bool isMultilingual);
		public abstract void PaintScrollerBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintScrollerHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintScrollerForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintSliderBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle sliderRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintSliderHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintSliderForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintProgressIndicator(Drawing.Graphics graphics, Drawing.Rectangle rect, ProgressIndicatorStyle style, double progress);
		public abstract void PaintGroupBox(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state);
		public abstract void PaintSepLine(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintFrameTitleBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintFrameTitleForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintFrameBody(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabBand(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabFrame(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabAboveBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabAboveForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabSunkenBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabSunkenForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintArrayBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintArrayForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintCellBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintHeaderBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintHeaderForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintToolBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintToolForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintMenuBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		public abstract void PaintMenuForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		public abstract void PaintMenuItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.MenuOrientation type, Widgets.MenuItemState itemType);
		public abstract void PaintMenuItemTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.MenuOrientation type, Widgets.MenuItemState itemType);
		public abstract void PaintMenuItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.MenuOrientation type, Widgets.MenuItemState itemType);
		public abstract void PaintSeparatorBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, bool optional);
		public abstract void PaintSeparatorForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, bool optional);
		public abstract void PaintPaneButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintPaneButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintStatusBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintStatusForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintStatusItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintStatusItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonTabBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		public abstract void PaintRibbonTabForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		public abstract void PaintRibbonPageBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		public abstract void PaintRibbonPageForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		public abstract void PaintRibbonButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, ActiveState active);
		public abstract void PaintRibbonButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, ActiveState active);
		public abstract void PaintRibbonButtonTextLayout(Drawing.Graphics graphics, Drawing.Rectangle rect, TextLayout text, WidgetPaintState state, ActiveState active);
		public abstract void PaintRibbonSectionBackground(Drawing.Graphics graphics, Drawing.Rectangle fullRect, Drawing.Rectangle userRect, Drawing.Rectangle textRect, TextLayout text, WidgetPaintState state);
		public abstract void PaintRibbonSectionForeground(Drawing.Graphics graphics, Drawing.Rectangle fullRect, Drawing.Rectangle userRect, Drawing.Rectangle textRect, TextLayout text, WidgetPaintState state);
		public abstract void PaintTagBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Drawing.Color color, Widgets.Direction dir);
		public abstract void PaintTagForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Drawing.Color color, Widgets.Direction dir);
		public abstract void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Color defaultColor);
		public abstract void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text);
		public abstract void PaintFocusBox(Drawing.Graphics graphics, Drawing.Rectangle rect);
		public abstract void PaintTextCursor(Drawing.Graphics graphics, Drawing.Point p1, Drawing.Point p2, bool cursorOn);
		public abstract void PaintTextSelectionBackground(Drawing.Graphics graphics, Widgets.TextLayout.SelectedArea[] areas, Widgets.WidgetPaintState state, PaintTextStyle style, TextFieldDisplayMode mode);
		public abstract void PaintTextSelectionForeground(Drawing.Graphics graphics, Widgets.TextLayout.SelectedArea[] areas, Widgets.WidgetPaintState state, PaintTextStyle style, TextFieldDisplayMode mode);
		public abstract void PaintGeneralTextLayout(Drawing.Graphics graphics, Drawing.Rectangle clipRect, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state, Widgets.PaintTextStyle style, TextFieldDisplayMode mode, Drawing.Color backColor);
		public abstract Drawing.Color AdaptPictogramColor(Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor);
		
		public virtual Drawing.Color ColorCaption
		{
			get
			{
				return this.colorCaption;
			}
		}

		public virtual Drawing.Color ColorControl
		{
			get
			{
				return this.colorControl;
			}
		}

		public abstract Drawing.Color ColorWindow
		{
			get;
		}

		public virtual Drawing.Color ColorDisabled
		{
			get
			{
				return Drawing.Color.Empty;
			}
		}

		public virtual Drawing.Color ColorBorder
		{
			get
			{
				return this.colorControlDarkDark;
			}
		}

		public virtual Drawing.Color ColorTextBackground
		{
			get
			{
				return this.colorControlLightLight;
			}
		}

		public virtual Drawing.Color ColorTabBackground
		{
			get
			{
				return this.colorControl;
			}
		}

		public virtual Drawing.Color ColorError
		{
			get
			{
				return this.colorError;
			}
		}

		public abstract Drawing.Color ColorText(Widgets.WidgetPaintState state);

		public abstract Drawing.Color ColorTextSliderBorder(bool enabled);

		public abstract Drawing.Color ColorTextFieldBorder(bool enabled);

		public abstract Drawing.Color ColorTextDisplayMode(TextFieldDisplayMode mode);


		public virtual double AlphaMenu
		{
			get
			{
				return 1.0;
			}
		}

		public abstract Drawing.Margins GeometryMenuShadow
		{
			get;
		}

		public abstract Drawing.Margins GeometryMenuMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryArrayMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryRadioShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryGroupShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryToolShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryThreeStateShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryButtonShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryRibbonShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryTextFieldShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryListShapeMargins
		{
			get;
		}

		public abstract double GeometryComboRightMargin
		{
			get;
		}

		public abstract double GeometryComboBottomMargin
		{
			get;
		}

		public abstract double GeometryComboTopMargin
		{
			get;
		}

		public abstract double GeometryUpDownWidthFactor
		{
			get;
		}

		public abstract double GeometryUpDownRightMargin
		{
			get;
		}

		public abstract double GeometryUpDownBottomMargin
		{
			get;
		}

		public abstract double GeometryUpDownTopMargin
		{
			get;
		}

		public abstract double GeometryScrollerRightMargin
		{
			get;
		}

		public abstract double GeometryScrollerBottomMargin
		{
			get;
		}

		public abstract double GeometryScrollerTopMargin
		{
			get;
		}

		public abstract double GeometryScrollListXMargin
		{
			get;
		}

		public abstract double GeometryScrollListYMargin
		{
			get;
		}

		public abstract double GeometrySliderLeftMargin
		{
			get;
		}

		public abstract double GeometrySliderRightMargin
		{
			get;
		}

		public abstract double GeometrySliderBottomMargin
		{
			get;
		}
		#endregion
		
		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorWhite;
		protected Drawing.Color		colorControl;
		protected Drawing.Color		colorControlLight;
		protected Drawing.Color		colorControlLightLight;
		protected Drawing.Color		colorControlDark;
		protected Drawing.Color		colorControlDarkDark;
		protected Drawing.Color		colorCaption;
		protected Drawing.Color		colorCaptionNF;  // NF = no focused
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorError;
	}
}
