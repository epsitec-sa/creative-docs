namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookXP implémente le décorateur au look Windows-XP.
	/// </summary>
	public class LookXP : IAdorner
	{
		public LookXP()
		{
			RefreshColors();
		}

		// Initialise les couleurs en fonction des réglages de Windows.
		public void RefreshColors()
		{
			colorControl           = Drawing.Color.FromName("Control");
			colorControlLight      = Drawing.Color.FromName("ControlLight");
			colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			colorControlDark       = Drawing.Color.FromName("ControlDark");
			colorControlDarkDark   = Drawing.Color.FromName("ControlDark");
		}
		

		// Dessine une flèche (dans un bouton d'ascenseur par exemple).
		public void PaintArrow(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow,
			Widgets.Direction dir)
		{
			graphics.AddFilledRectangle(rect.Left, rect.Bottom, rect.Width, rect.Height);
			graphics.RenderSolid(Drawing.Color.FromBrightness(1));
		}

		public void PaintCheck(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintRadio(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintIcon(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow,
			string icon)
		{
		}

		// Dessine un "L".
		protected void PaintL(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Drawing.Color color,
			Widgets.Direction dir)
		{
			Drawing.Point p1 = new Drawing.Point();
			Drawing.Point p2 = new Drawing.Point();

			switch ( dir )
			{
				case Direction.Up:	// en bas à droite
					p1.X = rect.Left;
					p1.Y = rect.Bottom+0.5;
					p2.X = rect.Right;
					p2.Y = p1.Y;
					graphics.AddLine(p1, p2);
					p1.X = rect.Right-0.5;
					p1.Y = rect.Bottom;
					p2.X = p1.X;
					p2.Y = rect.Top;
					graphics.AddLine(p1, p2);
					break;

				case Direction.Down:	// en haut à gauche
					p1.X = rect.Left+0.5;
					p1.Y = rect.Bottom;
					p2.X = p1.X;
					p2.Y = rect.Top;
					graphics.AddLine(p1, p2);
					p1.X = rect.Left;
					p1.Y = rect.Top-0.5;
					p2.X = rect.Right;
					p2.Y = p1.Y;
					graphics.AddLine(p1, p2);
					break;

				case Direction.Left:	// en bas à gauche
					p1.X = rect.Left+0.5;
					p1.Y = rect.Top;
					p2.X = p1.X;
					p2.Y = rect.Bottom;
					graphics.AddLine(p1, p2);
					p1.X = rect.Left;
					p1.Y = rect.Bottom+0.5;
					p2.X = rect.Right;
					p2.Y = p1.Y;
					graphics.AddLine(p1, p2);
					break;

				case Direction.Right:	// en haut à droite
					p1.X = rect.Left;
					p1.Y = rect.Top-0.5;
					p2.X = rect.Right;
					p2.Y = p1.Y;
					graphics.AddLine(p1, p2);
					p1.X = rect.Right-0.5;
					p1.Y = rect.Top;
					p2.X = p1.X;
					p2.Y = rect.Bottom;
					graphics.AddLine(p1, p2);
					break;
			}
			graphics.RenderSolid(color);
		}

		// Dessine le fond d'un bouton rectangulaire.
		public void PaintButtonBackground(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow,
			Widgets.ButtonStyle style)
		{
			if ( style == ButtonStyle.Normal )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(colorControl);

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Point p1 = new Drawing.Point();
				Drawing.Point p2 = new Drawing.Point();

				Drawing.Rectangle rInside = rect;
				rInside.Inflate(-1, -1);

				// Ombre en haut à droite.
				PaintL(graphics, rect, colorControlLightLight, Opposite(shadow));
				PaintL(graphics, rInside, colorControlLight, Opposite(shadow));

				// Ombre en bas à droite.
				PaintL(graphics, rect, colorControlDarkDark, shadow);
				PaintL(graphics, rInside, colorControlDark, shadow);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(colorControl);
			}
		}

		public void PaintButtonForeground(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow,
			Widgets.ButtonStyle style)
		{
		}

		public void PaintTextFieldBackground(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow,
			Widgets.TextFieldStyle style)
		{
		}

		public void PaintTextFieldForeground(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow,
			Widgets.TextFieldStyle style)
		{
		}

		public void PaintScrollerBackground(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle tab_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintScrollerHandle(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle tab_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintScrollerForeground(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle tab_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintGroupBox(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintSepLine(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintFrameTitleBackground(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintFrameTitleForeground(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintFrameBody(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintTabBand(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintTabFrame(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintTabAboveBackground(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintTabAboveForeground(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintTabSunkenBackground(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintTabSunkenForeground(Drawing.Graphics graphics,
			Drawing.Rectangle frame_rect,
			Drawing.Rectangle title_rect,
			Widgets.WidgetState state,
			Widgets.Direction shadow)
		{
		}

		public void PaintFocusBox(Drawing.Graphics graphics,
			Drawing.Rectangle rect)
		{
		}

		public void PaintTextCursor(Drawing.Graphics graphics,
			Drawing.Rectangle rect,
			bool cursor_on)
		{
		}
		
		public void PaintTextSelectionBackground(Drawing.Graphics graphics,
			Drawing.Rectangle[] rect)
		{
		}

		public void PaintTextSelectionForeground(Drawing.Graphics graphics,
			Drawing.Rectangle[] rect)
		{
		}



		// Retourne la direction opposée.
		protected Direction Opposite(Direction dir)
		{
			switch ( dir )
			{
				case Direction.Up:     return Direction.Down;
				case Direction.Down:   return Direction.Up;
				case Direction.Left:   return Direction.Right;
				case Direction.Right:  return Direction.Left;
			}
			return Direction.Up;
		}
		

		// Variables membres de TextLayout.
		protected Drawing.Color		colorControl           = new Drawing.Color(1,1,1);
		protected Drawing.Color		colorControlLight      = new Drawing.Color(1,1,1);
		protected Drawing.Color		colorControlLightLight = new Drawing.Color(1,1,1);
		protected Drawing.Color		colorControlDark       = new Drawing.Color(1,1,1);
		protected Drawing.Color		colorControlDarkDark   = new Drawing.Color(1,1,1);
	}
}
