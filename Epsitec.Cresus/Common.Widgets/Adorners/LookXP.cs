namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookXP impl�mente le d�corateur "presque comme Windows XP".
	/// </summary>
	public class LookXP : AbstractAdorner
	{
		public LookXP()
		{
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs en fonction des r�glages de Windows.
			double r,g,b;

			this.colorBlack             = Drawing.Color.FromBrightness(0);
			this.colorWhite             = Drawing.Color.FromName("Window");
			this.colorWindow            = Drawing.Color.FromName("Control");
			this.colorControl           = Drawing.Color.FromName("Control");
			this.colorControlLight      = Drawing.Color.FromName("ControlLight");
			this.colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			this.colorControlDark       = Drawing.Color.FromName("ControlDark");
			this.colorControlDarkDark   = Drawing.Color.FromName("ControlDarkDark");
			this.colorCaption           = Drawing.Color.FromName("ActiveCaption");
			this.colorCaptionNF         = Drawing.Color.FromName("ControlDark");
			this.colorCaptionText       = Drawing.Color.FromName("ActiveCaptionText");
			this.colorInfo              = Drawing.Color.FromName("Info");

			r = 1-(1-this.colorControlLight.R)/2;
			g = 1-(1-this.colorControlLight.G)/2;
			b = 1-(1-this.colorControlLight.B)/2;
			this.colorScrollerBack = Drawing.Color.FromRgb(r,g,b);

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorControlReadOnly = Drawing.Color.FromRgb(r,g,b);

			r = 1-(1-this.colorCaption.R)*0.25;
			g = 1-(1-this.colorCaption.G)*0.25;
			b = 1-(1-this.colorCaption.B)*0.25;
			this.colorCaptionLight = Drawing.Color.FromRgb(r,g,b);

			r = 1-(1-this.colorCaption.R)*0.5;
			g = 1-(1-this.colorCaption.G)*0.5;
			b = 1-(1-this.colorCaption.B)*0.5;
			this.colorCaptionMiddle = Drawing.Color.FromRgb(r,g,b);

			this.colorButton          = Drawing.Color.FromRgb(243.0/255.0, 243.0/255.0, 238.0/255.0);
			this.colorFocus           = Drawing.Color.FromRgb(157.0/255.0, 188.0/255.0, 235.0/255.0);
			this.colorHilite          = Drawing.Color.FromRgb(250.0/255.0, 196.0/255.0,  89.0/255.0);
			this.colorError           = Drawing.Color.FromRgb(255.0/255.0, 177.0/255.0, 177.0/255.0);
			this.colorThreeState      = Drawing.Color.FromRgb(211.0/255.0, 187.0/255.0, 153.0/255.0);
			this.colorCaptionProposal = Drawing.Color.FromRgb(154.0/255.0, 119.0/255.0,  74.0/255.0);
		}
		

		public override void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetPaintState state)
		{
			//	Dessine le fond d'une fen�tre.
			graphics.AddFilledRectangle(paintRect);
			graphics.RenderSolid(this.colorWindow);
		}

		public override void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state,
							   GlyphShape type,
							   PaintTextStyle style)
		{
			//	Dessine une ic�ne simple (dans un bouton d'ascenseur par exemple).
			Drawing.Color color = this.colorBlack;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRgb(0.5, 0.0, 0.0);  // rouge fonc�
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRgb(0.0, 0.4, 0.0);  // vert fonc�
			}
			else
			{
				color = this.colorControlDark;
			}

			this.PaintGlyph(graphics, rect, state, color, type, style);
		}
		
		public override void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state,
							   Drawing.Color color,
							   GlyphShape type,
							   PaintTextStyle style)
		{
			//	Dessine une ic�ne simple (dans un bouton d'ascenseur par exemple).
			if ( type == GlyphShape.ResizeKnob )
			{
				Drawing.Point p = rect.BottomRight;

				graphics.AddFilledRectangle(p.X-1, p.Y+1, -2, 2);
				graphics.AddFilledRectangle(p.X-5, p.Y+1, -2, 2);
				graphics.AddFilledRectangle(p.X-9, p.Y+1, -2, 2);
				graphics.AddFilledRectangle(p.X-1, p.Y+5, -2, 2);
				graphics.AddFilledRectangle(p.X-5, p.Y+5, -2, 2);
				graphics.AddFilledRectangle(p.X-1, p.Y+9, -2, 2);
				graphics.RenderSolid(Drawing.Color.FromBrightness(1.0));

				p.X -= 1.0;
				p.Y += 1.0;
				graphics.AddFilledRectangle(p.X-1, p.Y+1, -2, 2);
				graphics.AddFilledRectangle(p.X-5, p.Y+1, -2, 2);
				graphics.AddFilledRectangle(p.X-9, p.Y+1, -2, 2);
				graphics.AddFilledRectangle(p.X-1, p.Y+5, -2, 2);
				graphics.AddFilledRectangle(p.X-5, p.Y+5, -2, 2);
				graphics.AddFilledRectangle(p.X-1, p.Y+9, -2, 2);
				graphics.RenderSolid(Drawing.Color.FromRgb(this.colorWindow.R-0.1, this.colorWindow.G-0.1, this.colorWindow.B-0.1));
				return;
			}

			if ( rect.Width > rect.Height )
			{
				rect.Left += (rect.Width-rect.Height)/2;
				rect.Width = rect.Height;
			}

			if ( rect.Height > rect.Width )
			{
				rect.Bottom += (rect.Height-rect.Width)/2;
				rect.Height = rect.Width;
			}

			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			Drawing.Path path = new Drawing.Path();
			switch ( type )
			{
				case GlyphShape.ArrowUp:
					path.MoveTo(center.X+0.0*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X-0.3*rect.Width, center.Y-0.1*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.3*rect.Width, center.Y-0.1*rect.Height);
					break;

				case GlyphShape.ArrowDown:
					path.MoveTo(center.X+0.0*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X-0.3*rect.Width, center.Y+0.1*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y-0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.3*rect.Width, center.Y+0.1*rect.Height);
					break;

				case GlyphShape.ArrowRight:
					path.MoveTo(center.X+0.2*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X-0.1*rect.Width, center.Y-0.3*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X-0.1*rect.Width, center.Y+0.3*rect.Height);
					break;

				case GlyphShape.ArrowLeft:
					path.MoveTo(center.X-0.2*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.1*rect.Width, center.Y-0.3*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X-0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.1*rect.Width, center.Y+0.3*rect.Height);
					break;

				case GlyphShape.TriangleUp:
					path.MoveTo(center.X, center.Y+rect.Height*0.2);
					path.LineTo(center.X-rect.Width*0.3, center.Y-rect.Height*0.1);
					path.LineTo(center.X+rect.Width*0.3, center.Y-rect.Height*0.1);
					break;

				case GlyphShape.TriangleDown:
					path.MoveTo(center.X, center.Y-rect.Height*0.2);
					path.LineTo(center.X-rect.Width*0.3, center.Y+rect.Height*0.1);
					path.LineTo(center.X+rect.Width*0.3, center.Y+rect.Height*0.1);
					break;

				case GlyphShape.TriangleRight:
					path.MoveTo(center.X+rect.Width*0.2, center.Y);
					path.LineTo(center.X-rect.Width*0.1, center.Y+rect.Height*0.3);
					path.LineTo(center.X-rect.Width*0.1, center.Y-rect.Height*0.3);
					break;

				case GlyphShape.TriangleLeft:
					path.MoveTo(center.X-rect.Width*0.2, center.Y);
					path.LineTo(center.X+rect.Width*0.1, center.Y+rect.Height*0.3);
					path.LineTo(center.X+rect.Width*0.1, center.Y-rect.Height*0.3);
					break;

				case GlyphShape.HorizontalMove:
					path.MoveTo(center.X-rect.Width*0.3, center.Y);
					path.LineTo(center.X-rect.Width*0.05, center.Y+rect.Height*0.3);
					path.LineTo(center.X-rect.Width*0.05, center.Y-rect.Height*0.3);
					path.Close();
					path.MoveTo(center.X+rect.Width*0.3, center.Y);
					path.LineTo(center.X+rect.Width*0.05, center.Y+rect.Height*0.3);
					path.LineTo(center.X+rect.Width*0.05, center.Y-rect.Height*0.3);
					break;

				case GlyphShape.VerticalMove:
					path.MoveTo(center.X, center.Y-rect.Height*0.3);
					path.LineTo(center.X-rect.Width*0.3, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.3, center.Y-rect.Height*0.05);
					path.Close();
					path.MoveTo(center.X, center.Y+rect.Height*0.3);
					path.LineTo(center.X-rect.Width*0.3, center.Y+rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.3, center.Y+rect.Height*0.05);
					break;

				case GlyphShape.Menu:
					path.MoveTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.25);
					path.LineTo(center.X-rect.Width*0.30, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.15);
					break;

				case GlyphShape.Close:
				case GlyphShape.Reject:
					path.MoveTo(center.X-rect.Width*0.20, center.Y-rect.Height*0.30);
					path.LineTo(center.X-rect.Width*0.30, center.Y-rect.Height*0.20);
					path.LineTo(center.X-rect.Width*0.10, center.Y+rect.Height*0.00);
					path.LineTo(center.X-rect.Width*0.30, center.Y+rect.Height*0.20);
					path.LineTo(center.X-rect.Width*0.20, center.Y+rect.Height*0.30);
					path.LineTo(center.X-rect.Width*0.00, center.Y+rect.Height*0.10);
					path.LineTo(center.X+rect.Width*0.20, center.Y+rect.Height*0.30);
					path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.20);
					path.LineTo(center.X+rect.Width*0.10, center.Y+rect.Height*0.00);
					path.LineTo(center.X+rect.Width*0.30, center.Y-rect.Height*0.20);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.30);
					path.LineTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.10);
					break;

				case GlyphShape.Dots:
					path.MoveTo(center.X-rect.Width*0.30, center.Y+rect.Height*0.06);
					path.LineTo(center.X-rect.Width*0.18, center.Y+rect.Height*0.06);
					path.LineTo(center.X-rect.Width*0.18, center.Y-rect.Height*0.06);
					path.LineTo(center.X-rect.Width*0.30, center.Y-rect.Height*0.06);
					path.Close();
					path.MoveTo(center.X-rect.Width*0.06, center.Y+rect.Height*0.06);
					path.LineTo(center.X+rect.Width*0.06, center.Y+rect.Height*0.06);
					path.LineTo(center.X+rect.Width*0.06, center.Y-rect.Height*0.06);
					path.LineTo(center.X-rect.Width*0.06, center.Y-rect.Height*0.06);
					path.Close();
					path.MoveTo(center.X+rect.Width*0.18, center.Y+rect.Height*0.06);
					path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.06);
					path.LineTo(center.X+rect.Width*0.30, center.Y-rect.Height*0.06);
					path.LineTo(center.X+rect.Width*0.18, center.Y-rect.Height*0.06);
					break;

				case GlyphShape.Accept:
					path.MoveTo(center.X-rect.Width*0.30, center.Y+rect.Height*0.00);
					path.LineTo(center.X-rect.Width*0.20, center.Y+rect.Height*0.10);
					path.LineTo(center.X-rect.Width*0.10, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y+rect.Height*0.30);
					path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.20);
					path.LineTo(center.X-rect.Width*0.10, center.Y-rect.Height*0.30);
					break;

				case GlyphShape.TabLeft:
					path.MoveTo(center.X-rect.Width*0.10, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.00, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.10, center.Y-rect.Height*0.15);
					break;

				case GlyphShape.TabRight:
					path.MoveTo(center.X+rect.Width*0.00, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.10, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.10, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.05);
					break;

				case GlyphShape.TabCenter:
					path.MoveTo(center.X-rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.05, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo(center.X-rect.Width*0.05, center.Y-rect.Height*0.05);
					break;

				case GlyphShape.TabDecimal:
					path.MoveTo(center.X-rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.05, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo(center.X-rect.Width*0.05, center.Y-rect.Height*0.05);
					path.Close();
					path.MoveTo(center.X+rect.Width*0.10, center.Y+rect.Height*0.10);
					path.LineTo(center.X+rect.Width*0.20, center.Y+rect.Height*0.10);
					path.LineTo(center.X+rect.Width*0.20, center.Y+rect.Height*0.00);
					path.LineTo(center.X+rect.Width*0.10, center.Y+rect.Height*0.00);
					break;

				case GlyphShape.TabIndent:
					path.MoveTo(center.X-rect.Width*0.10, center.Y+rect.Height*0.20);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.00);
					path.LineTo(center.X-rect.Width*0.10, center.Y-rect.Height*0.20);
					break;

				case GlyphShape.Plus:
					path.MoveTo(center.X-rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo(center.X-rect.Width*0.07, center.Y+rect.Height*0.07);
					path.LineTo(center.X-rect.Width*0.07, center.Y+rect.Height*0.29);
					path.LineTo(center.X+rect.Width*0.07, center.Y+rect.Height*0.29);
					path.LineTo(center.X+rect.Width*0.07, center.Y+rect.Height*0.07);
					path.LineTo(center.X+rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo(center.X+rect.Width*0.29, center.Y-rect.Height*0.07);
					path.LineTo(center.X+rect.Width*0.07, center.Y-rect.Height*0.07);
					path.LineTo(center.X+rect.Width*0.07, center.Y-rect.Height*0.29);
					path.LineTo(center.X-rect.Width*0.07, center.Y-rect.Height*0.29);
					path.LineTo(center.X-rect.Width*0.07, center.Y-rect.Height*0.07);
					path.LineTo(center.X-rect.Width*0.29, center.Y-rect.Height*0.07);
					break;

				case GlyphShape.Minus:
					path.MoveTo(center.X-rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo(center.X+rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo(center.X+rect.Width*0.29, center.Y-rect.Height*0.07);
					path.LineTo(center.X-rect.Width*0.29, center.Y-rect.Height*0.07);
					break;
			}
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			path.Dispose();
			graphics.RenderSolid(color);
		}

		public override void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton � cocher sans texte.
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton press� ?
			{
				graphics.RenderSolid(this.colorControl);
			}
			else
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
			{
				rInside = rect;
				rInside.Deflate(1.5);
				graphics.LineWidth = 2;
				graphics.AddRectangle(rInside);
				graphics.RenderSolid(this.colorHilite);
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			rInside = rect;
			rInside.Deflate(0.5);
			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorControlDarkDark);

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coch� ?
			{
				Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
				Drawing.Path path = new Drawing.Path();
				path.MoveTo(center.X-rect.Width*0.1, center.Y-rect.Height*0.1);
				path.LineTo(center.X+rect.Width*0.3, center.Y+rect.Height*0.3);
				path.LineTo(center.X+rect.Width*0.3, center.Y+rect.Height*0.1);
				path.LineTo(center.X-rect.Width*0.1, center.Y-rect.Height*0.3);
				path.LineTo(center.X-rect.Width*0.3, center.Y-rect.Height*0.1);
				path.LineTo(center.X-rect.Width*0.3, center.Y+rect.Height*0.1);
				path.Close();
				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}

			if ( (state&WidgetPaintState.ActiveMaybe) != 0 )  // 3�me �tat ?
			{
				rect.Deflate(3);
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}
		}

		public override void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton radio sans texte.
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			this.PaintCircle(graphics, rect, this.colorControlDarkDark);

			rInside = rect;
			rInside.Deflate(1);

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
			}

			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton press� ?
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorControlLightLight);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coch� ?
			{
				rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorCaption);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorControlDark);
				}
			}
		}

		public override void PaintIcon(Drawing.Graphics graphics,
							  Drawing.Rectangle rect,
							  Widgets.WidgetPaintState state,
							  string icon)
		{
		}

		public override void PaintButtonBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetPaintState state,
										  Widgets.Direction dir,
										  Widgets.ButtonStyle style)
		{
			//	Dessine le fond d'un bouton rectangulaire.
			Drawing.Rectangle rFocus = rect;
			if ( System.Math.Min(rect.Width, rect.Height) < 16 )
			{
				rFocus.Deflate(1);
			}
			else
			{
				rFocus.Deflate(2);
			}
			double radFocus = 0;

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel ||
				 style == ButtonStyle.DefaultAcceptAndCancel )
			{
				Drawing.Path path = this.PathRoundRectangle(rect, 0, 0);
			
				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton press� ?
					{
						graphics.RenderSolid(this.colorControl);
					}
					else
					{
						graphics.RenderSolid(this.colorButton);
					}
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}
			
				if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
				{
					Drawing.Path pInside = this.PathRoundRectangle(rect, -1.5, 0);
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid(this.colorFocus);
				}

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
				{
					Drawing.Path pInside = this.PathRoundRectangle(rect, -1.5, 0);
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid(this.colorHilite);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}
			else if ( style == ButtonStyle.Scroller     ||
					  style == ButtonStyle.Combo        ||
					  style == ButtonStyle.ExListLeft   ||
					  style == ButtonStyle.ExListMiddle ||
					  style == ButtonStyle.ExListRight  ||
					  style == ButtonStyle.UpDown       ||
					  style == ButtonStyle.Icon         ||
					  style == ButtonStyle.HeaderSlider )
			{
				Drawing.Path path = this.PathRoundRectangle(rect, 0.5, 0);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.colorFocus);

				Drawing.Rectangle rInside = rect;
				rInside.Right  -= 1;
				rInside.Bottom += 1;
				Drawing.Path pInside = this.PathRoundRectangle(rInside, 0, 0);
				graphics.Rasterizer.AddOutline(pInside, 1);
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else if ( style == ButtonStyle.Slider )
			{
				Drawing.Path path = this.PathRoundRectangle(rect, 0.5, 0);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.colorFocus);

				Drawing.Rectangle rInside = rect;
				rInside.Right  -= 1;
				rInside.Bottom += 1;
				Drawing.Path pInside = this.PathRoundRectangle(rInside, 0, 0);
				graphics.Rasterizer.AddOutline(pInside, 1);
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetPaintState.Entered)   != 0 ||  // bouton survol� ?
					 (state&WidgetPaintState.Engaged)   != 0 ||  // bouton press� ?
					 (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activ� ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionLight);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Deflate(0.5);
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.colorCaption);
				}
				radFocus = -1;
			}
			else if ( style == ButtonStyle.ComboItem )
			{
				if ( (state&WidgetPaintState.Entered)   != 0 ||  // bouton survol� ?
					 (state&WidgetPaintState.Engaged)   != 0 ||  // bouton press� ?
					 (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activ� ?
				{
					if ((state&WidgetPaintState.InheritedEnter) == 0)
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaptionLight);
					}

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Deflate(0.5);
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.colorCaption);
				}
				radFocus = -1;
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				if ( AbstractAdorner.IsThreeState2(state) )
				{
					rect.Top += 2;
					rFocus.Top += 2;
				}

				rect.Right += 1;
				rFocus.Right += 1;

				if ( (state&WidgetPaintState.Entered)   != 0 ||  // bouton survol� ?
					 (state&WidgetPaintState.Engaged)   != 0 )   // bouton press� ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionLight);
				}
				else if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activ� ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionMiddle);
				}
				else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorThreeState);
				}

				Drawing.Rectangle rInside;
				rInside = rect;
				rInside.Deflate(0.5);
				graphics.AddRectangle(rInside);
				graphics.RenderSolid(this.ColorBorder);

				radFocus = -1;
			}
			else if ( style == ButtonStyle.Confirmation )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionLight);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Deflate(0.5);
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.ColorBorder);
				}
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton press� ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Deflate(0.5);
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.ColorBorder);
				}

				radFocus = -1;
			}
			else if ( style == ButtonStyle.ListItem )
			{
				if ( (state&WidgetPaintState.Selected) != 0 )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);
				}
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControl);
			}
			
			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				Drawing.Path pInside = this.PathRoundRectangle(rFocus, 0, radFocus);
				AbstractAdorner.DrawFocusedPath(graphics, pInside, this.colorControlDark);
			}
		}

		public override void PaintButtonTextLayout(Drawing.Graphics graphics,
										  Drawing.Point pos,
										  TextLayout text,
										  WidgetPaintState state,
										  ButtonStyle style)
		{
			//	Dessine le texte d'un bouton.
			if ( text == null )  return;

			if ( (state&WidgetPaintState.Engaged) != 0 &&  // bouton press� ?
				 style == ButtonStyle.ToolItem )
			{
				pos.X ++;
				pos.Y --;
			}
			if ( AbstractAdorner.IsThreeState2(state) )
			{
				pos.Y ++;
			}
			if ( style != ButtonStyle.Tab )
			{
				state &= ~WidgetPaintState.Focused;
			}
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, PaintTextStyle.Button, TextDisplayMode.Default, Drawing.Color.Empty);
		}

		public override void PaintButtonForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetPaintState state,
										  Widgets.Direction dir,
										  Widgets.ButtonStyle style)
		{
		}

		public override void PaintTextFieldBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetPaintState state,
											 Widgets.TextFieldStyle style,
											 TextDisplayMode mode,
											 bool readOnly)
		{
			//	Dessine le fond d'une ligne �ditable.
			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Multi  ||
				 style == TextFieldStyle.Combo  ||
				 style == TextFieldStyle.UpDown )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					Drawing.Color color = this.ColorTextDisplayMode(mode);
					if ( (state&WidgetPaintState.Error) != 0 )
					{
						graphics.RenderSolid(this.colorError);
					}
					else if ( !color.IsEmpty )
					{
						graphics.RenderSolid(color);
					}
					else
					{
						if ( readOnly )
						{
							graphics.RenderSolid(this.colorControlReadOnly);
						}
						else
						{
							graphics.RenderSolid(this.colorControlLightLight);
						}
					}
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(0.5);

				graphics.AddRectangle(rInside);
				graphics.RenderSolid(this.colorCaption);
			}
			else if ( style == TextFieldStyle.Simple )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(0.5);

				graphics.AddRectangle(rInside);
				graphics.RenderSolid(this.colorCaption);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControlLightLight);
			}
		}

		public override void PaintTextFieldForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetPaintState state,
											 Widgets.TextFieldStyle style,
											 TextDisplayMode mode,
											 bool readOnly)
		{
		}

		public override void PaintScrollerBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle thumbRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine le fond d'un ascenseur.
			graphics.AddFilledRectangle(frameRect);
			graphics.RenderSolid(this.colorScrollerBack);

			if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
			{
				graphics.AddFilledRectangle(tabRect);
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		public override void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetPaintState state,
										Widgets.Direction dir)
		{
			//	Dessine la cabine d'un ascenseur.
			this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

			Drawing.Rectangle	rect;
			Drawing.Point		center;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				switch ( dir )
				{
					case Direction.Up:
					case Direction.Down:
						rect = thumbRect;
						if ( rect.Width >= 10 && rect.Height >= 20 )
						{
							center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
							center.Y = System.Math.Floor(center.Y)+0.5;
							double y = center.Y-4;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(center.X-rect.Width*0.25, y, center.X+rect.Width*0.25, y);
								y += 2;
							}
							graphics.RenderSolid(this.colorCaption);

							y = center.Y-4+1;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(center.X-rect.Width*0.25-1, y, center.X+rect.Width*0.25-1, y);
								y += 2;
							}
							graphics.RenderSolid(this.colorControlLightLight);
						}
						break;

					case Direction.Left:
					case Direction.Right:
						rect = thumbRect;
						if ( rect.Height >= 10 && rect.Width >= 20 )
						{
							center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
							center.X = System.Math.Floor(center.X)-0.5;
							double x = center.X-4+1;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(x, center.Y-rect.Height*0.25, x, center.Y+rect.Height*0.25);
								x += 2;
							}
							graphics.RenderSolid(this.colorCaption);

							x = center.X-4;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(x, center.Y-rect.Height*0.25+1, x, center.Y+rect.Height*0.25+1);
								x += 2;
							}
							graphics.RenderSolid(this.colorControlLightLight);
						}
						break;
				}
			}
		}

		public override void PaintScrollerForeground(Drawing.Graphics graphics,
											Drawing.Rectangle thumbRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
		}

		public override void PaintSliderBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle frameRect, Drawing.Rectangle sliderRect,
										  Drawing.Rectangle thumbRect,
										  Drawing.Rectangle tabRect,
										  Widgets.WidgetPaintState state,
										  Widgets.Direction dir)
		{
			//	Dessine le fond d'un potentiom�tre lin�aire.
			if ( dir == Widgets.Direction.Left )
			{
				Drawing.Point p1 = new Drawing.Point (sliderRect.Left +frameRect.Height*0.2, frameRect.Center.Y);
				Drawing.Point p2 = new Drawing.Point (sliderRect.Right-frameRect.Height*0.2, frameRect.Center.Y);
				graphics.Align (ref p1);
				graphics.Align(ref p2);

				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X-0.5, p2.Y+0.5);
				graphics.RenderSolid(this.colorControlDark);
				graphics.AddLine(p1.X+0.5, p1.Y-0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorControlLightLight);

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					graphics.AddLine(tabRect.Left, p1.Y+0.5, tabRect.Right, p2.Y+0.5);
					graphics.AddLine(tabRect.Left, p1.Y-0.5, tabRect.Right, p2.Y-0.5);
					graphics.RenderSolid(this.colorCaption);
				}
			}
			else
			{
				Drawing.Point p1 = new Drawing.Point (frameRect.Center.X, sliderRect.Bottom+frameRect.Width*0.2);
				Drawing.Point p2 = new Drawing.Point (frameRect.Center.X, sliderRect.Top   -frameRect.Width*0.2);
				graphics.Align (ref p1);
				graphics.Align(ref p2);

				graphics.AddLine(p1.X-0.5, p1.Y+0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorControlDark);
				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X+0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorControlLightLight);

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					graphics.AddLine(p1.X-0.5, tabRect.Bottom, p2.X-0.5, tabRect.Top);
					graphics.AddLine(p1.X+0.5, tabRect.Bottom, p2.X+0.5, tabRect.Top);
					graphics.RenderSolid(this.colorCaption);
				}
			}
		}

		public override void PaintSliderHandle(Drawing.Graphics graphics,
									  Drawing.Rectangle thumbRect,
									  Drawing.Rectangle tabRect,
									  Widgets.WidgetPaintState state,
									  Widgets.Direction dir)
		{
			//	Dessine la cabine d'un potentiom�tre lin�aire.
			if ( dir == Widgets.Direction.Left )
			{
				thumbRect.Deflate(0.5);
				double d = thumbRect.Width/2;
				double r = 0.5;
				Drawing.Path path = new Drawing.Path();
				path.MoveTo(thumbRect.Center.X, thumbRect.Bottom);
				path.LineTo(thumbRect.Left, thumbRect.Bottom+d);
				path.LineTo(thumbRect.Left, thumbRect.Top-r);
				path.LineTo(thumbRect.Left+r, thumbRect.Top);
				path.LineTo(thumbRect.Right-r, thumbRect.Top);
				path.LineTo(thumbRect.Right, thumbRect.Top-r);
				path.LineTo(thumbRect.Right, thumbRect.Bottom+d);
				path.Close();

				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survol� ?
				{
					graphics.RenderSolid(this.colorHilite);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLight);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorControlDarkDark);

				graphics.AddLine(thumbRect.Center.X, thumbRect.Bottom+d+1, thumbRect.Center.X, thumbRect.Top-d);
				graphics.RenderSolid(this.colorControlDark);
				graphics.AddLine(thumbRect.Center.X+1, thumbRect.Bottom+d+1, thumbRect.Center.X+1, thumbRect.Top-d);
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				thumbRect.Deflate(0.5);
				double d = thumbRect.Height/2;
				double r = 0.5;
				Drawing.Path path = new Drawing.Path();
				path.MoveTo(thumbRect.Right, thumbRect.Center.Y);
				path.LineTo(thumbRect.Right-d, thumbRect.Bottom);
				path.LineTo(thumbRect.Left+r, thumbRect.Bottom);
				path.LineTo(thumbRect.Left, thumbRect.Bottom+r);
				path.LineTo(thumbRect.Left, thumbRect.Top-r);
				path.LineTo(thumbRect.Left+r, thumbRect.Top);
				path.LineTo(thumbRect.Right-d, thumbRect.Top);
				path.Close();

				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survol� ?
				{
					graphics.RenderSolid(this.colorHilite);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLight);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorControlDarkDark);

				graphics.AddLine(thumbRect.Left+d, thumbRect.Center.Y, thumbRect.Right-d-1, thumbRect.Center.Y);
				graphics.RenderSolid(this.colorControlDark);
				graphics.AddLine(thumbRect.Left+d, thumbRect.Center.Y-1, thumbRect.Right-d-1, thumbRect.Center.Y-1);
				graphics.RenderSolid(this.colorControlLightLight);
			}
		}

		public override void PaintSliderForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle thumbRect,
										  Drawing.Rectangle tabRect,
										  Widgets.WidgetPaintState state,
										  Widgets.Direction dir)
		{
		}

		public override void PaintProgressIndicator(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												ProgressIndicatorStyle style,
												double progress)
		{
			Drawing.Path path = this.PathRoundRectangle(rect, 0, 0);
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.colorControlLight);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.colorBlack);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(3);
			graphics.AddFilledRectangle(rInside);
			graphics.RenderSolid(this.colorControl);

			if (style == ProgressIndicatorStyle.UnknownDuration)
			{
				double x = rInside.Width*progress;
				double w = rInside.Width*0.2;

				this.PaintProgressUnknow(graphics, rInside, w, x-w);
				this.PaintProgressUnknow(graphics, rInside, w, x-w+rInside.Width);
			}
			else
			{
				if (progress != 0)
				{
					rInside.Width *= progress;
					graphics.AddFilledRectangle(rInside);
					graphics.RenderSolid(this.colorCaption);
				}
			}

			rInside = rect;
			rInside.Deflate(3.5);
			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorBlack);
		}

		protected void PaintProgressUnknow(Drawing.Graphics graphics, Drawing.Rectangle rect, double w, double x)
		{
			Drawing.Rectangle fill = new Drawing.Rectangle(rect.Left+x, rect.Bottom, w, rect.Height);

			if (fill.Left < rect.Left)
			{
				fill.Left = rect.Left;
			}

			if (fill.Right > rect.Right)
			{
				fill.Right = rect.Right;
			}

			if (fill.Width > 0)
			{
				graphics.AddFilledRectangle(fill);
				graphics.RenderSolid(this.colorCaption);
			}
		}

		public override void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetPaintState state)
		{
			//	Dessine le cadre d'un GroupBox.
			Drawing.Rectangle rect = frameRect;
			rect.Deflate(0.5);
			graphics.LineWidth = 1;
			this.RectangleGroupBox(graphics, rect, titleRect.Left, titleRect.Right);
			graphics.RenderSolid(this.colorControlDark);
		}

		public override void PaintSepLine(Drawing.Graphics graphics,
								 Drawing.Rectangle frameRect,
								 Drawing.Rectangle titleRect,
								 Widgets.WidgetPaintState state,
								 Widgets.Direction dir)
		{
		}

		public override void PaintFrameTitleBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
											  Widgets.WidgetPaintState state,
											  Widgets.Direction dir)
		{
		}

		public override void PaintFrameTitleForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
											  Widgets.WidgetPaintState state,
											  Widgets.Direction dir)
		{
		}

		public override void PaintFrameBody(Drawing.Graphics graphics,
								   Drawing.Rectangle rect,
								   Widgets.WidgetPaintState state,
								   Widgets.Direction dir)
		{
		}

		public override void PaintTabBand(Drawing.Graphics graphics,
								 Drawing.Rectangle rect,
								 Widgets.WidgetPaintState state,
								 Widgets.Direction dir)
		{
			//	Dessine toute la bande sous les onglets.
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);
		}

		public override void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetPaintState state,
								  Widgets.Direction dir)
		{
			//	Dessine la zone principale sous les onglets.
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDarkDark);

			rect.Deflate(0.5);
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorButton);
			}
			else
			{
				graphics.RenderSolid(this.colorControl);
			}
		}

		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine l'onglet devant les autres.
			titleRect.Bottom += 1;

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			graphics.Rasterizer.AddSurface(pTitle);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorButton);

				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}
			else
			{
				graphics.RenderSolid(this.colorControl);
			}

			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.colorControlDarkDark);
		}

		public override void PaintTabAboveForeground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
		}

		public override void PaintTabSunkenBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetPaintState state,
											 Widgets.Direction dir)
		{
			//	Dessine un onglet derri�re (non s�lectionn�).
			titleRect.Left  += 1;
			titleRect.Right -= 1;

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			graphics.Rasterizer.AddSurface(pTitle);
			graphics.RenderSolid(this.colorControl);

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
			{
				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}

			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.colorControlDarkDark);
		}

		public override void PaintTabSunkenForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetPaintState state,
											 Widgets.Direction dir)
		{
		}

		public override void PaintArrayBackground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetPaintState state)
		{
			//	Dessine le fond d'un tableau.
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(this.colorControl);
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(0.5);

			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorCaption);

			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(1.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorCaption);
			}
		}

		public override void PaintArrayForeground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetPaintState state)
		{
		}

		public override void PaintCellBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetPaintState state)
		{
			//	Dessine le fond d'une cellule.
			if ( (state&WidgetPaintState.Selected) != 0 )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Focused) != 0 ||
					 (state&WidgetPaintState.InheritedFocus) != 0 )
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorCaptionNF);
				}
			}

			if ( (state&WidgetPaintState.Entered) != 0 )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.2, this.colorCaption.R, this.colorCaption.G, this.colorCaption.B));
			}

			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(1);
				AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorControlDarkDark);
			}
		}

		public override void PaintHeaderBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetPaintState state,
										  Direction dir)
		{
			//	Dessine le fond d'un bouton d'en-t�te de tableau.
			if ( dir == Direction.Up )
			{
				rect.Left  += 1;
				rect.Right -= 0;
				rect.Top   -= 1;
			}
			if ( dir == Direction.Left )
			{
				rect.Bottom += 0;
				rect.Top    -= 1;
				rect.Left   += 1;
			}
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);

			if ( dir == Direction.Up )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
				{
					Drawing.Rectangle rTitle = rect;
					rTitle.Top = rTitle.Bottom+2;
					rTitle.Left += 2;
					rTitle.Right -= 2;
					graphics.AddFilledRectangle(rTitle);
					graphics.RenderSolid(this.colorHilite);
				}

				graphics.AddLine(rect.Left+1.5, rect.Bottom+0.5, rect.Left+1.5, rect.Top-0.5);
				graphics.AddLine(rect.Right-1.5, rect.Bottom+0.5, rect.Right-1.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+1.5, rect.Top-0.5, rect.Right-1.5, rect.Top-0.5);
				graphics.RenderSolid(this.colorControlDark);
			}

			if ( dir == Direction.Left )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
				{
					Drawing.Rectangle rTitle = rect;
					rTitle.Left = rTitle.Right-2;
					rTitle.Bottom += 2;
					rTitle.Top -= 2;
					graphics.AddFilledRectangle(rTitle);
					graphics.RenderSolid(this.colorHilite);
				}

				graphics.AddLine(rect.Left+0.5, rect.Top-0.5, rect.Right-0.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Right-0.5, rect.Bottom+0.5);
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Left+0.5, rect.Top-0.5);
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		public override void PaintHeaderForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetPaintState state,
										  Direction dir)
		{
		}

		public override void PaintToolBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetPaintState state,
										Direction dir)
		{
			//	Dessine le fond d'une barre d'outil.
			if ( dir == Direction.Up )
			{
				graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
				graphics.RenderSolid(this.colorCaption);
			}

			if ( dir == Direction.Left )
			{
				graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.RenderSolid(this.colorCaption);
			}
		}

		public override void PaintToolForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetPaintState state,
										Direction dir)
		{
		}

		public override void PaintMenuBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetPaintState state,
										Direction dir,
										Drawing.Rectangle parentRect,
										double iconWidth)
		{
			//	Dessine le fond d'un menu.
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlLightLight);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Width = iconWidth;
				band.Top -= 1;
				band.Bottom += 1;
				graphics.AddFilledRectangle(band);
				graphics.RenderSolid(this.colorControl);
			}

			rect.Deflate(0.5);
			if ( parentRect.IsSurfaceZero )
			{
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorControlDark);
			}
			else
			{
				graphics.AddLine(rect.Left, rect.Top+0.5, rect.Left, rect.Bottom-0.5);
				graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
				graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top+0.5);
				graphics.AddLine(parentRect.Right-0.5, rect.Top, rect.Right+0.5, rect.Top);
				graphics.RenderSolid(this.colorControlDark);

				graphics.AddLine(rect.Left+1, rect.Top, parentRect.Right-1.5, rect.Top);
				graphics.RenderSolid(this.colorControl);
			}
		}

		public override void PaintMenuForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetPaintState state,
										Direction dir,
										Drawing.Rectangle parentRect,
										double iconWidth)
		{
		}

		public override void PaintMenuItemBackground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetPaintState state,
											Direction dir,
											MenuOrientation type,
											MenuItemType itemType)
		{
			//	Dessine le fond d'une case de menu.
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == MenuOrientation.Horizontal )
				{
					if ( itemType == MenuItemType.Selected )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaptionLight);

						Drawing.Rectangle rInside;
						rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddRectangle(rInside);
						graphics.RenderSolid(this.colorCaption);
					}
					if ( itemType == MenuItemType.SubmenuOpen )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorControl);

						Drawing.Rectangle rInside;
						rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddLine(rInside.Left, rInside.Bottom-0.5, rInside.Left, rInside.Top);
						graphics.AddLine(rInside.Left, rInside.Top, rInside.Right, rInside.Top);
						graphics.AddLine(rInside.Right, rInside.Top, rInside.Right, rInside.Bottom-0.5);
						graphics.RenderSolid(this.colorControlDark);
					}
				}

				if ( type == MenuOrientation.Vertical )
				{
					if ( itemType != MenuItemType.Default )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaptionLight);

						Drawing.Rectangle rInside;
						rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddRectangle(rInside);
						graphics.RenderSolid(this.colorCaption);
					}
				}
			}
			else
			{
				if ( itemType != MenuItemType.Default )
				{
					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.ColorBorder);
				}
			}
		}

		public override void PaintMenuItemTextLayout(Drawing.Graphics graphics,
											Drawing.Point pos,
											TextLayout text,
											WidgetPaintState state,
											Direction dir,
											MenuOrientation type,
											MenuItemType itemType)
		{
			//	Dessine le texte d'un menu.
			if ( text == null )  return;
			state &= ~WidgetPaintState.Selected;
			state &= ~WidgetPaintState.Focused;
			PaintTextStyle style = ( type == MenuOrientation.Horizontal ) ? PaintTextStyle.HMenu : PaintTextStyle.VMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, style, TextDisplayMode.Default, Drawing.Color.Empty);
		}

		public override void PaintMenuItemForeground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetPaintState state,
											Direction dir,
											MenuOrientation type,
											MenuItemType itemType)
		{
			//	Dessine le devant d'une case de menu.
		}

		public override void PaintSeparatorBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetPaintState state,
											 Direction dir,
											 bool optional)
		{
			//	Dessine un s�parateur horizontal ou vertical.
			if ( dir == Direction.Right )
			{
				Drawing.Point p1 = new Drawing.Point(rect.Left+rect.Width/2, rect.Bottom);
				Drawing.Point p2 = new Drawing.Point(rect.Left+rect.Width/2, rect.Top);
				graphics.Align(ref p1);
				graphics.Align(ref p2);
				p1.X -= 0.5;
				p2.X -= 0.5;
				graphics.AddLine(p1, p2);
			}
			else
			{
				Drawing.Point p1 = new Drawing.Point(rect.Left, rect.Bottom+rect.Height/2);
				Drawing.Point p2 = new Drawing.Point(rect.Right, rect.Bottom+rect.Height/2);
				graphics.Align(ref p1);
				graphics.Align(ref p2);
				p1.Y -= 0.5;
				p2.Y -= 0.5;
				graphics.AddLine(p1, p2);
			}

			graphics.RenderSolid(this.colorCaption);
		}

		public override void PaintSeparatorForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetPaintState state,
											 Direction dir,
											 bool optional)
		{
		}

		public override void PaintPaneButtonBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetPaintState state,
											  Direction dir)
		{
			//	Dessine un bouton s�parateur de panneaux.
			double x, y;
			if ( dir == Direction.Down || dir == Direction.Up )
			{
				x = rect.Left+0.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(this.colorControlLightLight);

				x = rect.Left+1.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(this.colorControlLight);

				x = rect.Right-1.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(this.colorControlDark);

				x = rect.Right-0.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				y = rect.Top-0.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(this.colorControlLightLight);

				y = rect.Top-1.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(this.colorControlLight);

				y = rect.Bottom+1.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(this.colorControlDark);

				y = rect.Bottom+0.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(this.colorControlDarkDark);
			}
		}

		public override void PaintPaneButtonForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetPaintState state,
											  Direction dir)
		{
		}

		public override void PaintStatusBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetPaintState state)
		{
			//	Dessine une ligne de statuts.
			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.RenderSolid(this.colorCaption);
		}

		public override void PaintStatusForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetPaintState state)
		{
		}

		public override void PaintStatusItemBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetPaintState state)
		{
			//	Dessine une case de statuts.
			rect.Width -= 1;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
		}

		public override void PaintStatusItemForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetPaintState state)
		{
		}

		public override void PaintRibbonTabBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetPaintState state)
		{
			//	Dessine la bande principale d'un ruban.
			Drawing.Rectangle header = rect;
			graphics.AddFilledRectangle(header);
			graphics.RenderSolid(this.colorControlLight);
		}

		public override void PaintRibbonTabForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetPaintState state)
		{
			//	Dessine la bande principale d'un ruban.
		}

		public override void PaintRibbonPageBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetPaintState state)
		{
			//	Dessine la bande principale d'un ruban.
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlLight);

			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
			graphics.RenderSolid(this.colorControlDarkDark);
		}

		public override void PaintRibbonPageForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetPaintState state)
		{
			//	Dessine la bande principale d'un ruban.
		}

		public override void PaintRibbonButtonBackground(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												WidgetPaintState state,
												ActiveState active)
		{
			//	Dessine le bouton pour un ruban.
			if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activ� ?
			{
				double radius = System.Math.Min(rect.Width, rect.Height)/8;
				Drawing.Path pTitle = this.PathTopRoundRectangle(rect, radius);

				graphics.Rasterizer.AddSurface(pTitle);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorControlLight);
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
				{
					Drawing.Rectangle rHilite = rect;
					rHilite.Bottom = rHilite.Top-3;
					Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
					graphics.Rasterizer.AddSurface(pHilite);
					graphics.RenderSolid(this.colorHilite);
				}

				graphics.Rasterizer.AddOutline(pTitle, 1);
				pTitle.Dispose();
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorControlDarkDark);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}
			else
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
				{
					rect.Top    -= 2;
					rect.Bottom += 1;
					rect.Left   += 1;
					rect.Right  -= 1;

					double radius = System.Math.Min(rect.Width, rect.Height)/8;
					Drawing.Path pTitle = this.PathTopRoundRectangle(rect, radius);

					graphics.Rasterizer.AddSurface(pTitle);
					graphics.RenderSolid(this.colorControl);

					Drawing.Rectangle rHilite = rect;
					rHilite.Bottom = rHilite.Top-3;
					Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
					graphics.Rasterizer.AddSurface(pHilite);
					graphics.RenderSolid(this.colorHilite);

					graphics.Rasterizer.AddOutline(pTitle, 1);
					pTitle.Dispose();
					if ( (state&WidgetPaintState.Enabled) != 0 )
					{
						graphics.RenderSolid(this.colorControlDarkDark);
					}
					else
					{
						graphics.RenderSolid(this.colorControlDark);
					}
				}
			}
		}

		public override void PaintRibbonButtonForeground(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												WidgetPaintState state,
												ActiveState active)
		{
			//	Dessine le bouton pour un ruban.
		}

		public override void PaintRibbonButtonTextLayout(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												TextLayout text,
												WidgetPaintState state,
												ActiveState active)
		{
			//	Dessine le texte d'un bouton du ruban.
			if ( text == null )  return;

			Drawing.Point pos = new Drawing.Point();
			pos.X = (rect.Width-text.LayoutSize.Width)/2;
			pos.Y = (rect.Height-text.LayoutSize.Height)/2;
			if ( (state&WidgetPaintState.ActiveYes) == 0 )   // bouton d�sactiv� ?
			{
				pos.Y -= 2;
			}
			state &= ~WidgetPaintState.Focused;
			PaintTextStyle style = PaintTextStyle.HMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, style, TextDisplayMode.Default, Drawing.Color.Empty);
		}

		public override void PaintRibbonSectionBackground(Drawing.Graphics graphics,
												 Drawing.Rectangle fullRect,
												 Drawing.Rectangle userRect,
												 Drawing.Rectangle textRect,
												 TextLayout text,
												 WidgetPaintState state)
		{
			//	Dessine une section d'un ruban.
			fullRect.Deflate(0.5);
			graphics.AddLine(fullRect.Right, fullRect.Top, fullRect.Right, fullRect.Bottom);
			graphics.RenderSolid(this.colorControlDarkDark);

			if (text != null)
			{
				Drawing.TextStyle.DefineDefaultColor(this.colorBlack);
				Drawing.Point pos = new Drawing.Point(textRect.Left+3, textRect.Bottom);
				text.LayoutSize = new Drawing.Size(textRect.Width-4, textRect.Height);
				text.Alignment = Drawing.ContentAlignment.MiddleCenter;
				text.Paint(pos, graphics, Drawing.Rectangle.MaxValue, Drawing.Color.FromBrightness(0), Drawing.GlyphPaintStyle.Normal);
			}
		}

		public override void PaintRibbonSectionForeground(Drawing.Graphics graphics,
												 Drawing.Rectangle fullRect,
												 Drawing.Rectangle userRect,
												 Drawing.Rectangle textRect,
												 TextLayout text,
												 WidgetPaintState state)
		{
			//	Dessine une section d'un ruban.
		}

		public override void PaintTagBackground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetPaintState state,
									   Drawing.Color color,
									   Direction dir)
		{
			//	Dessine un tag.
			Drawing.Path path;
			
			path = new Drawing.Path();
			path.AppendCircle(rect.Center, rect.Width/2, rect.Height/2);
			graphics.Rasterizer.AddSurface(path);
			if ( color.IsEmpty || (state&WidgetPaintState.Enabled) == 0 )
			{
				graphics.RenderSolid(this.colorControl);
			}
			else
			{
				Drawing.Color topColor = Drawing.Color.FromColor(color, 0.2);
				Drawing.Color bottomColor = Drawing.Color.FromColor(color, 0.6);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}

			Drawing.Rectangle rInside;

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survol� ?
			{
				rInside = rect;
				rInside.Deflate(1.5);
				path = new Drawing.Path();
				path.AppendCircle(rInside.Center, rInside.Width/2, rInside.Height/2);
				graphics.Rasterizer.AddOutline(path, 2);
				graphics.RenderSolid(this.colorHilite);
			}

			rInside = rect;
			rInside.Deflate(0.5);
			path = new Drawing.Path();
			path.AppendCircle(rInside.Center, rInside.Width/2, rInside.Height/2);
			graphics.Rasterizer.AddOutline(path, 1);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		public override void PaintTagForeground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetPaintState state,
									   Drawing.Color color,
									   Direction dir)
		{
		}

		public override void PaintTooltipBackground(Drawing.Graphics graphics,
										   Drawing.Rectangle rect)
		{
			//	Dessine le fond d'une bulle d'aide.
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorInfo);  // fond jaune pale
			
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBlack);  // cadre noir
		}

		public override void PaintTooltipTextLayout(Drawing.Graphics graphics,
										   Drawing.Point pos,
										   TextLayout text)
		{
			//	Dessine le texte d'une bulle d'aide.
			text.Paint(pos, graphics);
		}


		public override void PaintFocusBox(Drawing.Graphics graphics,
								  Drawing.Rectangle rect)
		{
			//	Dessine le rectangle pour indiquer le focus.
			rect.Inflate(0.5);
			AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorControlDark);
		}

		public override void PaintTextCursor(Drawing.Graphics graphics,
									Drawing.Point p1, Drawing.Point p2,
									bool cursorOn)
		{
			//	Dessine le curseur du texte.
			if ( cursorOn )
			{
				double original = graphics.LineWidth;
				graphics.LineWidth = 1;
				graphics.Align(ref p1);
				graphics.Align(ref p2);
				p1.X -= 0.5;
				p2.X -= 0.5;
				p1.Y -= 0.5;
				p2.Y -= 0.5;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorBlack);
				graphics.LineWidth = original;
			}
		}
		
		public override void PaintTextSelectionBackground(Drawing.Graphics graphics,
												 TextLayout.SelectedArea[] areas,
												 WidgetPaintState state, PaintTextStyle style, TextDisplayMode mode)
		{
			//	Dessine les zones rectanglaires correspondant aux caract�res s�lectionn�s.
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				graphics.AddFilledRectangle(areas[i].Rect);
				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( mode == TextDisplayMode.Proposal )
					{
						graphics.RenderSolid(this.colorCaptionProposal);
					}
					else
					{
						graphics.RenderSolid(this.colorCaption);
					}

					if ( areas[i].Color != Drawing.Color.FromBrightness(0) )
					{
						Drawing.Rectangle rect = areas[i].Rect;
						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(areas[i].Color);
					}
				}
				else
				{
					graphics.RenderSolid(this.colorCaptionNF);
				}
			}
		}

		public override void PaintTextSelectionForeground(Drawing.Graphics graphics,
												 TextLayout.SelectedArea[] areas,
												 WidgetPaintState state, PaintTextStyle style, TextDisplayMode mode)
		{
		}

		public override void PaintGeneralTextLayout(Drawing.Graphics graphics,
										   Drawing.Rectangle clipRect,
										   Drawing.Point pos,
										   TextLayout text,
										   WidgetPaintState state,
										   PaintTextStyle style,
										   TextDisplayMode mode,
										   Drawing.Color backColor)
		{
			//	Dessine le texte d'un widget.
			if ( text == null )  return;

			text = AbstractAdorner.AdaptTextLayout (text, mode);
			
			Drawing.TextStyle.DefineDefaultColor(this.colorBlack);

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Selected) != 0 )
				{
					text.Paint(pos, graphics, clipRect, this.colorCaptionText, Drawing.GlyphPaintStyle.Selected);
				}
				else
				{
					text.Paint(pos, graphics, clipRect, Drawing.Color.Empty, Drawing.GlyphPaintStyle.Normal);
				}
			}
			else
			{
				text.Paint(pos, graphics, clipRect, this.colorControlDark, Drawing.GlyphPaintStyle.Disabled);
			}

			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = text.StandardRectangle;
				rFocus.Offset(pos);
				graphics.Align(ref rFocus);
				rFocus.Inflate(2.5, -0.5);
				this.PaintFocusBox(graphics, rFocus);
			}
		}


		protected void RectangleGroupBox(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 double startX, double endX)
		{
			//	Dessine un rectangle
			graphics.AddLine(rect.Left, rect.Top, startX, rect.Top);
			graphics.AddLine(endX, rect.Top, rect.Right, rect.Top);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
			graphics.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
		}

		protected Drawing.Path PathRoundRectangle(Drawing.Rectangle rect, double inflate, double radius)
		{
			//	Cr�e le chemin d'un rectangle � coins arrondis.
			rect.Inflate(inflate);
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius == 0 )
			{
				radius = System.Math.Min(dx, dy)/8;
			}
			if ( radius == -1 )
			{
				radius = 0;
			}

			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+radius+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (ox+radius+0.5, oy+dy-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
			path.LineTo (ox+0.5, oy+radius+0.5);
			path.CurveTo(ox+0.5, oy+0.5, ox+radius+0.5, oy+0.5);
			path.Close();

			return path;
		}

		protected Drawing.Path PathTopRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			//	Cr�e le chemin d'un rectangle � coins arrondis en forme de "u" invers�.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius == 0 )
			{
				radius = System.Math.Min(dx, dy)/8;
			}
			
			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+0.5, oy);
			path.LineTo (ox+0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+radius+0.5, oy+dy-0.5);
			path.LineTo (ox+dx-radius-0.5, oy+dy-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-0.5, oy+dy-radius-0.5);
			path.LineTo (ox+dx-0.5, oy);

			return path;
		}

		protected void PaintCircle(Drawing.Graphics graphics,
								   Drawing.Rectangle rect,
								   Drawing.Color color)
		{
			//	Dessine un cercle complet.
			Drawing.Point c = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			graphics.AddFilledCircle(c.X, c.Y, rx, ry);
			graphics.RenderSolid(color);
		}

		protected void Gradient(Drawing.Graphics graphics,
								Drawing.Rectangle rect,
								Drawing.Color bottomColor,
								Drawing.Color topColor)
		{
			graphics.FillMode = Drawing.FillMode.NonZero;
			graphics.GradientRenderer.Fill = Drawing.GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Drawing.Transform ot = graphics.GradientRenderer.Transform;
			Drawing.Transform t = new Drawing.Transform();
			Drawing.Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
//			t.Rotate(0, center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}


		public override void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.3, 1.0);  // augmente l'intensit�
				color = Drawing.Color.FromBrightness(intensity);
				color.A = alpha;
			}
		}

		public override Drawing.Color ColorCaption
		{
			get { return this.colorCaption; }
		}

		public override Drawing.Color ColorControl
		{
			get { return this.colorControl; }
		}

		public override Drawing.Color ColorWindow
		{
			get { return this.colorWindow; }
		}

		public override Drawing.Color ColorDisabled
		{
			get { return Drawing.Color.Empty; }
		}

		public override Drawing.Color ColorBorder
		{
			get { return this.colorCaption; }
		}

		public override Drawing.Color ColorTextBackground
		{
			get { return this.colorControlLightLight; }
		}

		public override Drawing.Color ColorText(WidgetPaintState state)
		{
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Selected) != 0 )
				{
					return this.colorCaptionText;
				}
				else
				{
					return this.colorBlack;
				}
			}
			else
			{
				return this.colorControlDark;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.colorCaption;
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.colorCaption;
		}

		public override Drawing.Color ColorTextDisplayMode(TextDisplayMode mode)
		{
			switch ( mode )
			{
				case TextDisplayMode.Default:   return Drawing.Color.Empty;
				case TextDisplayMode.Defined:   return this.colorCaptionMiddle;
				case TextDisplayMode.Proposal:  return this.colorThreeState;
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(3,3,3,3); } }
		public override Drawing.Margins GeometryRadioShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryGroupShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryToolShapeMargins { get { return new Drawing.Margins(0,1,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeMargins { get { return new Drawing.Margins(0,1,2,0); } }
		public override Drawing.Margins GeometryButtonShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRibbonShapeMargins { get { return new Drawing.Margins(0,0,0,2); } }
		public override Drawing.Margins GeometryTextFieldShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryListShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override double GeometryComboRightMargin { get { return 2; } }
		public override double GeometryComboBottomMargin { get { return 2; } }
		public override double GeometryComboTopMargin { get { return 2; } }
		public override double GeometryUpDownWidthFactor { get { return 0.7; } }
		public override double GeometryUpDownRightMargin { get { return 1; } }
		public override double GeometryUpDownBottomMargin { get { return 1; } }
		public override double GeometryUpDownTopMargin { get { return 1; } }
		public override double GeometryScrollerRightMargin { get { return 2; } }
		public override double GeometryScrollerBottomMargin { get { return 2; } }
		public override double GeometryScrollerTopMargin { get { return 2; } }
		public override double GeometryScrollListXMargin { get { return 2; } }
		public override double GeometryScrollListYMargin { get { return 2; } }
		public override double GeometrySliderLeftMargin { get { return 0; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 0; } }


		//	Variables membres de TextLayout.
		protected Drawing.Color		colorControlReadOnly;
		protected Drawing.Color		colorScrollerBack;
		protected Drawing.Color		colorCaptionLight;
		protected Drawing.Color		colorCaptionMiddle;
		protected Drawing.Color		colorCaptionProposal;
		protected Drawing.Color		colorThreeState;
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorFocus;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorError;
		protected Drawing.Color		colorWindow;
	}
}
