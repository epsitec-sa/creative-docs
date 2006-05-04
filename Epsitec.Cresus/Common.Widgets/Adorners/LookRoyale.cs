namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookRoyale implémente le décorateur Energy Blue Royale.
	/// </summary>
	public class LookRoyale : AbstractAdorner
	{
		public LookRoyale()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookRoyale.png", typeof (IAdorner));
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs en fonction des réglages de Windows.
			double r,g,b;

			this.colorBlack           = Drawing.Color.FromRgb(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorGlyph           = Drawing.Color.FromRgb( 91.0/255.0, 100.0/255.0, 115.0/255.0);
			this.colorWindow          = Drawing.Color.FromRgb(235.0/255.0, 233.0/255.0, 237.0/255.0);
			this.colorFrame           = Drawing.Color.FromRgb(250.0/255.0, 251.0/255.0, 255.0/255.0);
			this.colorControl         = Drawing.Color.FromRgb(107.0/255.0, 144.0/255.0, 189.0/255.0);
			this.colorGreen           = Drawing.Color.FromRgb( 33.0/255.0, 161.0/255.0,  33.0/255.0);
			this.colorBorder          = Drawing.Color.FromRgb(133.0/255.0, 153.0/255.0, 177.0/255.0);
			this.colorBorderLight     = Drawing.Color.FromRgb(210.0/255.0, 210.0/255.0, 212.0/255.0);
			this.colorBorderButton    = Drawing.Color.FromRgb( 43.0/255.0,  79.0/255.0, 130.0/255.0);
			this.colorEntered         = Drawing.Color.FromRgb(255.0/255.0, 200.0/255.0,  60.0/255.0);
			this.colorDefault         = Drawing.Color.FromRgb(101.0/255.0, 151.0/255.0, 210.0/255.0);
			this.colorDisabled        = Drawing.Color.FromRgb(198.0/255.0, 197.0/255.0, 201.0/255.0);
			this.colorCaption         = Drawing.Color.FromRgb( 51.0/255.0,  94.0/255.0, 168.0/255.0);
			this.colorCaptionText     = Drawing.Color.FromRgb(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorCaptionProposal = Drawing.Color.FromRgb(154.0/255.0, 119.0/255.0,  74.0/255.0);
			this.colorHilite          = Drawing.Color.FromRgb(255.0/255.0, 186.0/255.0,   1.0/255.0);
			this.colorWhite           = Drawing.Color.FromRgb(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorError           = Drawing.Color.FromRgb(255.0/255.0, 177.0/255.0, 177.0/255.0);
			this.colorTextBackground  = Drawing.Color.FromRgb(247.0/255.0, 246.0/255.0, 248.0/255.0);
			this.colorInfo            = Drawing.Color.FromName("Info");

			r = 1-(1-this.colorCaption.R)*0.25;
			g = 1-(1-this.colorCaption.G)*0.25;
			b = 1-(1-this.colorCaption.B)*0.25;
			this.colorCaptionLight = Drawing.Color.FromRgb(r,g,b);

			r = 1-(1-this.colorCaption.R)*0.5;
			g = 1-(1-this.colorCaption.G)*0.5;
			b = 1-(1-this.colorCaption.B)*0.5;
			this.colorCaptionNF = Drawing.Color.FromRgb(r,g,b);
		}
		

		public override void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetPaintState state)
		{
			//	Dessine le fond d'une fenêtre.
			graphics.AddFilledRectangle(paintRect);
			graphics.RenderSolid(this.colorWindow);
		}

		public override void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state,
							   GlyphShape type,
							   PaintTextStyle style)
		{
			//	Dessine une icône simple (dans un bouton d'ascenseur par exemple).
			Drawing.Color color = this.colorGlyph;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRgb(0.5, 0.0, 0.0);  // rouge foncé
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRgb(0.0, 0.5, 0.0);  // vert foncé
			}
			else
			{
				color = this.colorDisabled;
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
			//	Dessine une icône simple (dans un bouton d'ascenseur par exemple).
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

			if ( rect.Width < 10 )
			{
				rect.Width  *= 1.5;
				rect.Height *= 1.5;
			}

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
			//	Dessine un bouton à cocher sans texte.
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 13);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 21);
				}

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					rInside = rect;
					rInside.Deflate(1.5);
					graphics.LineWidth = 2;
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.colorHilite);
				}
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWhite);
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			rInside = rect;
			rInside.Deflate(0.5);
			graphics.AddRectangle(rInside);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorderButton);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
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
					graphics.RenderSolid(this.colorGreen);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}

			if ( (state&WidgetPaintState.ActiveMaybe) != 0 )  // 3ème état ?
			{
				rect.Deflate(3);
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorGreen);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
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

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				this.PaintCircle(graphics, rect, this.colorBorderButton);
			}
			else
			{
				this.PaintCircle(graphics, rect, this.colorDisabled);
			}

			rInside = rect;
			rInside.Deflate(1);

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
			}

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 14);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 22);
				}
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorWhite);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorGreen);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorDisabled);
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
				rFocus.Deflate(0.5);
			}
			else
			{
				rFocus.Deflate(1.5);
			}

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( style == ButtonStyle.DefaultAccept )
					{
						rFocus.Deflate(1);
						rInside.Deflate(1);
					}

					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 0);
					}

					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						rect.Deflate(1.0);
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 3.0);
						graphics.RenderSolid(this.colorEntered);
						rect.Inflate(1.0);
					}
					else if ( style == ButtonStyle.DefaultAccept )
					{
						rect.Deflate(1.0);
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 3.0);
						graphics.RenderSolid(this.colorDefault);
						rect.Inflate(1.0);
					}

					{
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 1.0);
						graphics.RenderSolid(this.colorBorderButton);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 2);

					double radius = this.RetRadius(rect);
					Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorDisabled);
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
				int add = 1;
				if ( style == ButtonStyle.Scroller )
				{
					rect.Deflate(1.0);
					if ( dir == Direction.Up || dir == Direction.Down )  add = 0;
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 28+add);

					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						double radius = this.RetRadiusScroller(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 1.0);
						graphics.RenderSolid(this.colorBorderButton);
					}
					else
					{
						double radius = this.RetRadiusScroller(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 1.0);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 30+add);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}

				if ( style == ButtonStyle.Icon )  rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.Slider )
			{
				int add = 1;
				if ( dir == Direction.Up || dir == Direction.Down )  add = 0;

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 28+add);

					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						double radius = this.RetRadiusScroller(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 1.0);
						graphics.RenderSolid(this.colorBorderButton);
					}
					else
					{
						double radius = this.RetRadiusScroller(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 1.0);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 30+add);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}

				if ( style == ButtonStyle.Icon )  rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 10);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 9);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				rFocus.Deflate(1.0);
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				if ( AbstractAdorner.IsThreeState2(state) )
				{
					rect.Top += 2;
					rInside.Top += 2;
					rFocus.Top +=2;
				}

				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 10);
					}
				}
				else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 15);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 15);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 12);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 9);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 8);
					}
				}
				rFocus.Deflate(1.0);
			}
			else if ( style == ButtonStyle.ListItem )
			{
				this.PaintImageButton(graphics, rect, 8);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 8);
			}

			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				this.PaintFocusBox(graphics, rFocus);
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

			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
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
			//	Dessine le fond d'une ligne éditable.
			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Multi  ||
				 style == TextFieldStyle.Combo  ||
				 style == TextFieldStyle.UpDown )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					Drawing.Color color = this.ColorTextDisplayMode(mode);
					if ( (state&WidgetPaintState.Error) != 0 )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorError);
					}
					else if ( !color.IsEmpty )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(color);
					}
					else
					{
						this.PaintImageButton(graphics, rect, readOnly?17:16);
					}

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorWhite);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, 16);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorWhite);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWhite);
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
			if ( dir == Direction.Up || dir == Direction.Down )  // ascenseur vertical ?
			{
				this.PaintImageButton(graphics, frameRect, 24);
			}
			else	// ascenseur horizontal ?
			{
				this.PaintImageButton(graphics, frameRect, 25);
			}

			if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
			{
				if ( dir == Direction.Up || dir == Direction.Down )  // ascenseur vertical ?
				{
					tabRect.Left   += 1.0;
					tabRect.Right  -= 1.0;
					tabRect.Top    += 2.0;
					tabRect.Bottom -= 2.0;
					this.PaintImageButton(graphics, tabRect, 26);
				}
				else	// ascenseur horizontal ?
				{
					tabRect.Top    -= 1.0;
					tabRect.Bottom += 1.0;
					tabRect.Left   -= 2.0;
					tabRect.Right  += 2.0;
					this.PaintImageButton(graphics, tabRect, 27);
				}
			}
		}

		public override void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetPaintState state,
										Widgets.Direction dir)
		{
			//	Dessine la cabine d'un ascenseur.
			if ( dir == Direction.Up || dir == Direction.Down )
			{
				thumbRect.Top += 2.0;
				thumbRect.Bottom -= 2.0;
			}
			else
			{
				thumbRect.Left -= 2.0;
				thumbRect.Right += 2.0;
			}
			state &= ~WidgetPaintState.Engaged;
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
								graphics.AddLine(center.X-rect.Width*0.15, y, center.X+rect.Width*0.15, y);
								y += 2;
							}
							graphics.RenderSolid(this.colorBorder);

							y = center.Y-4+1;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(center.X-rect.Width*0.15-1, y, center.X+rect.Width*0.15-1, y);
								y += 2;
							}
							graphics.RenderSolid(this.colorWhite);
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
								graphics.AddLine(x, center.Y-rect.Height*0.15, x, center.Y+rect.Height*0.15);
								x += 2;
							}
							graphics.RenderSolid(this.colorBorder);

							x = center.X-4;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(x, center.Y-rect.Height*0.15+1, x, center.Y+rect.Height*0.15+1);
								x += 2;
							}
							graphics.RenderSolid(this.colorWhite);
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
										  Drawing.Rectangle frameRect,
										  Drawing.Rectangle thumbRect,
										  Drawing.Rectangle tabRect,
										  Widgets.WidgetPaintState state,
										  Widgets.Direction dir)
		{
			//	Dessine le fond d'un potentiomètre linéaire.
			bool enabled = ( (state&WidgetPaintState.Enabled) != 0 );

			if ( dir == Widgets.Direction.Left )
			{
				Drawing.Point p1 = new Drawing.Point(frameRect.Left +frameRect.Height*1.2, frameRect.Center.Y);
				Drawing.Point p2 = new Drawing.Point(frameRect.Right-frameRect.Height*1.2, frameRect.Center.Y);
				graphics.Align(ref p1);
				graphics.Align(ref p2);

				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X-0.5, p2.Y+0.5);
				graphics.RenderSolid(enabled ? this.colorBorder : this.colorDisabled);
				graphics.AddLine(p1.X+0.5, p1.Y-0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorWhite);

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					graphics.AddLine(tabRect.Left, p1.Y+0.5, tabRect.Right, p2.Y+0.5);
					graphics.AddLine(tabRect.Left, p1.Y-0.5, tabRect.Right, p2.Y-0.5);
					graphics.RenderSolid(this.colorCaption);
				}
			}
			else
			{
				Drawing.Point p1 = new Drawing.Point(frameRect.Center.X, frameRect.Bottom+frameRect.Width*1.2);
				Drawing.Point p2 = new Drawing.Point(frameRect.Center.X, frameRect.Top   -frameRect.Width*1.2);
				graphics.Align(ref p1);
				graphics.Align(ref p2);

				graphics.AddLine(p1.X-0.5, p1.Y+0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(enabled ? this.colorBorder : this.colorDisabled);
				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X+0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorWhite);

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
			//	Dessine la cabine d'un potentiomètre linéaire.
			if ( dir == Widgets.Direction.Left )
			{
				state &= ~WidgetPaintState.Engaged;
				this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Slider);

				double d = thumbRect.Width/2+1;
				graphics.AddLine(thumbRect.Center.X, thumbRect.Bottom+d, thumbRect.Center.X, thumbRect.Top-d);
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				state &= ~WidgetPaintState.Engaged;
				this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Slider);

				double d = thumbRect.Height/2+1;
				graphics.AddLine(thumbRect.Left+d, thumbRect.Center.Y, thumbRect.Right-d, thumbRect.Center.Y);
				graphics.RenderSolid(this.colorBorder);
			}
		}

		public override void PaintSliderForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle thumbRect,
										  Drawing.Rectangle tabRect,
										  Widgets.WidgetPaintState state,
										  Widgets.Direction dir)
		{
		}

		public override void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetPaintState state)
		{
			//	Dessine le cadre d'un GroupBox.
			Drawing.Path path = this.PathRoundRectangleGroupBox(frameRect, titleRect.Left, titleRect.Right);
			graphics.Rasterizer.AddOutline(path, 1);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorderLight);
			}
			else
			{
				graphics.RenderSolid(this.colorWhite);
			}
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}

			rect.Deflate(0.5);
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorFrame);
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
				graphics.RenderSolid(this.colorFrame);

				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);

				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorWhite);

				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorDisabled);
			}
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
			//	Dessine un onglet derrière (non sélectionné).
			titleRect.Left  += 1;
			titleRect.Right -= 1;

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			this.PaintImageButton(graphics, titleRect, 18);

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorDisabled);
			}
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
			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWhite);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWhite);
			}
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorderLight);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
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
				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorCaptionNF);
				}
			}
		}

		public override void PaintHeaderBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetPaintState state,
										  Direction dir)
		{
			//	Dessine le fond d'un bouton d'en-tête de tableau.
			if ( dir == Direction.Up )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rTitle = rect;
					rTitle.Height = 3;
					graphics.AddFilledRectangle(rTitle);
					graphics.RenderSolid(this.colorHilite);
				}

				graphics.AddLine(rect.Left+0.5, rect.Bottom+2.0, rect.Left+0.5, rect.Top-3.0);
				graphics.RenderSolid(this.colorBorderLight);
			}

			if ( dir == Direction.Left )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rTitle = rect;
					rTitle.Left = rTitle.Right-3;
					graphics.AddFilledRectangle(rTitle);
					graphics.RenderSolid(this.colorHilite);
				}

				graphics.AddLine(rect.Left+3.0, rect.Top-0.5, rect.Right-2.0, rect.Top-0.5);
				graphics.RenderSolid(this.colorBorderLight);
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
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
				graphics.RenderSolid(this.colorWhite);
			}

			if ( dir == Direction.Left )
			{
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.RenderSolid(this.colorWhite);
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
			graphics.RenderSolid(this.colorWhite);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Width = iconWidth;
				band.Top -= 1;
				band.Bottom += 1;
				graphics.AddFilledRectangle(band);
				graphics.RenderSolid(this.colorWindow);
			}

			rect.Deflate(0.5);
			if ( parentRect.IsSurfaceZero )
			{
				graphics.AddRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else
			{
				graphics.AddLine(rect.Left, rect.Top+0.5, rect.Left, rect.Bottom-0.5);
				graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
				graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top+0.5);
				graphics.AddLine(parentRect.Right-0.5, rect.Top, rect.Right+0.5, rect.Top);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}

				graphics.AddLine(rect.Left+1, rect.Top, parentRect.Right-1.5, rect.Top);
				graphics.RenderSolid(this.colorWindow);
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

						Drawing.Rectangle rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddRectangle(rInside);
						graphics.RenderSolid(this.colorCaption);
					}
					if ( itemType == MenuItemType.SubmenuOpen )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorWindow);

						Drawing.Rectangle rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddLine(rInside.Left, rInside.Bottom-0.5, rInside.Left, rInside.Top);
						graphics.AddLine(rInside.Left, rInside.Top, rInside.Right, rInside.Top);
						graphics.AddLine(rInside.Right, rInside.Top, rInside.Right, rInside.Bottom-0.5);
						graphics.RenderSolid(this.colorBorder);
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
					graphics.RenderSolid(this.colorBorder);
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
			//	Dessine un séparateur horizontal ou vertical.
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

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
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
			//	Dessine un bouton séparateur de panneaux.
			if ( dir == Direction.Down || dir == Direction.Up )
			{
				this.PaintImageButton(graphics, rect, 20);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 19);
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorWhite);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public override void PaintStatusItemForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetPaintState state)
		{
		}

		public override void PaintRibbonButtonBackground(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												WidgetPaintState state)
		{
			//	Dessine le bouton pour un ruban.
			rect.Bottom -= 2;

			if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
			{
				double radius = System.Math.Min(rect.Width, rect.Height)/8;
				Drawing.Path pTitle = this.PathTopRoundRectangle(rect, radius);

				graphics.Rasterizer.AddSurface(pTitle);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorFrame);

					Drawing.Rectangle rHilite = rect;
					rHilite.Bottom = rHilite.Top-3;
					Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
					graphics.Rasterizer.AddSurface(pHilite);
					graphics.RenderSolid(this.colorHilite);

					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorWhite);

					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else
			{
				rect.Top -= 2;
				rect.Left  += 1;
				rect.Right -= 1;

				double radius = System.Math.Min(rect.Width, rect.Height)/8;
				Drawing.Path pTitle = this.PathTopRoundRectangle(rect, radius);

				this.PaintImageButton(graphics, rect, 18);

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rHilite = rect;
					rHilite.Bottom = rHilite.Top-3;
					Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
					graphics.Rasterizer.AddSurface(pHilite);
					graphics.RenderSolid(this.colorHilite);
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorDisabled);
				}
			}
		}

		public override void PaintRibbonButtonForeground(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												WidgetPaintState state)
		{
			//	Dessine le bouton pour un ruban.
		}

		public override void PaintRibbonButtonTextLayout(Drawing.Graphics graphics,
												Drawing.Point pos,
												TextLayout text,
												WidgetPaintState state)
		{
			//	Dessine le texte d'un bouton du ruban.
			if ( text == null )  return;

			if ( (state&WidgetPaintState.ActiveYes) == 0 )   // bouton désactivé ?
			{
				pos.Y -= 2;
			}
			state &= ~WidgetPaintState.Focused;
			PaintTextStyle style = PaintTextStyle.HMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, style, TextDisplayMode.Default, Drawing.Color.Empty);
		}

		public override void PaintRibbonTabBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 double titleHeight,
											 WidgetPaintState state)
		{
			//	Dessine la bande principale d'un ruban.
			Drawing.Rectangle header = rect;
			header.Bottom = header.Top-titleHeight;
			this.PaintImageButton(graphics, header, 8);

			rect.Top -= titleHeight;
			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.RenderSolid(this.colorBorder);
			graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
			graphics.RenderSolid(this.colorWhite);
		}

		public override void PaintRibbonTabForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 double titleHeight,
											 WidgetPaintState state)
		{
			//	Dessine la bande principale d'un ruban.
		}

		public override void PaintRibbonSectionBackground(Drawing.Graphics graphics,
												 Drawing.Rectangle rect,
												 double titleHeight,
												 WidgetPaintState state)
		{
			//	Dessine une section d'un ruban.
			rect.Deflate(0.5);

			graphics.AddLine(rect.Right, rect.Top, rect.Right, rect.Top-titleHeight);
			graphics.RenderSolid(Drawing.Color.FromRgb(167.0/255.0, 185.0/255.0, 208.0/255.0));

			graphics.AddLine(rect.Right, rect.Top-titleHeight, rect.Right, rect.Bottom);
			graphics.RenderSolid(this.colorBorder);
		}

		public override void PaintRibbonSectionForeground(Drawing.Graphics graphics,
												 Drawing.Rectangle rect,
												 double titleHeight,
												 WidgetPaintState state)
		{
			//	Dessine une section d'un ruban.
		}

		public override void PaintRibbonSectionTextLayout(Drawing.Graphics graphics,
												 Drawing.Point pos,
												 TextLayout text,
												 WidgetPaintState state)
		{
			//	Dessine le texte du titre d'une section d'un ruban.
			if ( text == null )  return;

			Drawing.TextStyle.DefineDefaultColor(this.colorBlack);
			text.Alignment = Drawing.ContentAlignment.MiddleLeft;
			text.Paint(pos, graphics, Drawing.Rectangle.MaxValue, Drawing.Color.FromBrightness(0), Drawing.GlyphPaintStyle.Normal);
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
			if ( (state&WidgetPaintState.Enabled) == 0 )
			{
				Drawing.Color topColor    = Drawing.Color.FromRgb(221.0/255.0, 224.0/255.0, 227.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRgb(214.0/255.0, 216.0/255.0, 219.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}
			else if ( color.IsEmpty )
			{
				Drawing.Color topColor    = Drawing.Color.FromRgb(253.0/255.0, 253.0/255.0, 253.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRgb(205.0/255.0, 204.0/255.0, 223.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}
			else
			{
				Drawing.Color topColor    = Drawing.Color.FromColor(color, 0.0);
				Drawing.Color bottomColor = Drawing.Color.FromColor(color, 0.5);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}

			Drawing.Rectangle rInside;

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
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
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
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
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorCaption);
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
			//	Dessine les zones rectanglaires correspondant aux caractères sélectionnés.
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

			string iText = "";
			if ( mode == TextDisplayMode.Proposal )
			{
				iText = text.Text;
				text.Text = string.Format("<i>{0}</i>", text.Text);
			}

			if ( style == PaintTextStyle.Group )
			{
				Drawing.TextStyle.DefineDefaultColor(this.colorBorderButton);
			}
			else
			{
				Drawing.TextStyle.DefineDefaultColor(this.colorBlack);
			}

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
				text.Paint(pos, graphics, clipRect, this.colorDisabled, Drawing.GlyphPaintStyle.Disabled);
			}

			if ( mode == TextDisplayMode.Proposal )
			{
				text.Text = iText;
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


		protected Drawing.Path PathRoundRectangleGroupBox(Drawing.Rectangle rect,
														  double startX, double endX)
		{
			//	Crée le chemin d'un rectangle à coins arrondis.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			double radius = System.Math.Min(7, System.Math.Min(rect.Width, rect.Height));
			radius = System.Math.Min(radius, startX);
			
			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+radius+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (endX, oy+dy-0.5);
			path.MoveTo (ox+radius+0.5, oy+dy-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
			path.LineTo (ox+0.5, oy+radius+0.5);
			path.CurveTo(ox+0.5, oy+0.5, ox+radius+0.5, oy+0.5);

			return path;
		}

		protected Drawing.Path PathRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			//	Crée le chemin d'un rectangle à coins arrondis.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius == 0 )
			{
				radius = System.Math.Min(dx, dy)/8;
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
			//	Crée le chemin d'un rectangle à coins arrondis en forme de "u" inversé.
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

		protected Drawing.Path PathTopRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle en forme de "U" inversé.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(ox+0.5, oy);
			path.LineTo(ox+0.5, oy+dy-0.5);
			path.LineTo(ox+dx-0.5, oy+dy-0.5);
			path.LineTo(ox+dx-0.5, oy);

			return path;
		}

		protected Drawing.Path PathLeftRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle en forme de "D" inversé.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(ox+dx-0.5, oy+0.5);
			path.LineTo(ox+0.5, oy+0.5);
			path.LineTo(ox+0.5, oy+dy-0.5);
			path.LineTo(ox+dx-0.5, oy+dy-0.5);

			return path;
		}

		protected Drawing.Path PathTopCornerRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle "corné" en forme de "U" inversé.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(ox+0.5, oy);
			path.LineTo(ox+0.5, oy+dy-0.5-5);
			path.LineTo(ox+0.5+5, oy+dy-0.5);
			path.LineTo(ox+dx-0.5, oy+dy-0.5);
			path.LineTo(ox+dx-0.5, oy);

			return path;
		}

		protected Drawing.Path PathLeftCornerRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle "corné" en forme de "D" inversé.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(ox+dx-0.5, oy+0.5);
			path.LineTo(ox+0.5, oy+0.5);
			path.LineTo(ox+0.5, oy+dy-0.5-5);
			path.LineTo(ox+0.5+5, oy+dy-0.5);
			path.LineTo(ox+dx-0.5, oy+dy-0.5);

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
//			t.RotateDeg(0, center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected double RetRadius(Drawing.Rectangle rect)
		{
			//	Retourne le rayon à utiliser pour une zone rectangulaire.
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Max(3, dim/7));
		}

		protected double RetRadiusScroller(Drawing.Rectangle rect)
		{
			//	Retourne le rayon à utiliser pour une zone rectangulaire.
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Max(2, dim/7));
		}

		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank)
		{
			//	Dessine un bouton composé de 9 morceaux d'image.
			Drawing.Rectangle icon = new Drawing.Rectangle();
			icon.Left   = 32*(rank%8);
			icon.Right  = icon.Left+32;
			icon.Top    = 256-32*(rank/8);
			icon.Bottom = icon.Top-32;

			if ( rank ==  8 || rank ==  9 || rank == 10 || rank == 13 || rank == 14 ||
				 rank == 16 || rank == 17 || rank == 21 || rank == 22 )
			{
				this.PaintImageButton1(graphics, rect, icon);
			}
			else if ( rank == 24 || rank == 25 || rank == 26 || rank == 27 || rank == 28 || rank == 29 || rank == 30 || rank == 31 ||
					  rank == 19 || rank == 20 )
			{
				this.PaintImageButton9(graphics, rect, this.RetRadiusScroller(rect), icon, 2);
			}
			else
			{
				this.PaintImageButton9(graphics, rect, this.RetRadius(rect), icon, 3);
			}
		}

		protected void PaintImageButton1(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 Drawing.Rectangle icon)
		{
			//	Dessine un bouton composé d'un seul morceau d'image.
			icon.Deflate(0.5);
			graphics.PaintImage(this.bitmap, rect, icon);
		}

		protected void PaintImageButton9(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 double rectMargin,
										 Drawing.Rectangle icon,
										 double iconMargin)
		{
			//	Dessine un bouton composé de 9 morceaux d'image.
			if ( rectMargin <= 1 )
			{
				PaintImageButton1(graphics, rect, icon);
				return;
			}

			graphics.Align(ref rect);

			Drawing.Rectangle prect = new Drawing.Rectangle();
			Drawing.Rectangle picon = new Drawing.Rectangle();

			for ( int i=0 ; i<3 ; i++ )
			{
				switch ( i )
				{
					case 0:
						prect.Bottom = rect.Bottom;
						prect.Top    = rect.Bottom+rectMargin;
						picon.Bottom = icon.Bottom;
						picon.Top    = icon.Bottom+iconMargin;
						break;
					case 1:
						prect.Bottom = rect.Bottom+rectMargin;
						prect.Top    = rect.Top-rectMargin;
						picon.Bottom = icon.Bottom+iconMargin;
						picon.Top    = icon.Top-iconMargin;
						break;
					case 2:
						prect.Bottom = rect.Top-rectMargin;
						prect.Top    = rect.Top;
						picon.Bottom = icon.Top-iconMargin;
						picon.Top    = icon.Top;
						break;
				}

				prect.Left   = rect.Left;
				prect.Right  = rect.Left+rectMargin;
				picon.Left   = icon.Left;
				picon.Right  = icon.Left+iconMargin;
				graphics.PaintImage(this.bitmap, prect, picon);

				prect.Left   = rect.Left+rectMargin;
				prect.Right  = rect.Right-rectMargin;
				picon.Left   = icon.Left+iconMargin;
				picon.Right  = icon.Right-iconMargin;
				graphics.PaintImage(this.bitmap, prect, picon);

				prect.Left   = rect.Right-rectMargin;
				prect.Right  = rect.Right;
				picon.Left   = icon.Right-iconMargin;
				picon.Right  = icon.Right;
				graphics.PaintImage(this.bitmap, prect, picon);
			}
		}


		public override void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.3, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromBrightness(intensity);
				color.G *= 1.02;
				color.B *= 1.05;  // bleuté
				color = color.ClipToRange();
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
			get { return this.colorBorder; }
		}

		public override Drawing.Color ColorTextBackground
		{
			get { return this.colorTextBackground; }
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
				return this.colorDisabled;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return (enabled ? this.colorBorder : this.colorDisabled);
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return (enabled ? this.colorBorder : this.colorDisabled);
		}

		public override Drawing.Color ColorTextDisplayMode(TextDisplayMode mode)
		{
			switch ( mode )
			{
				case TextDisplayMode.Default:   return Drawing.Color.Empty;
				case TextDisplayMode.Defined:   return Drawing.Color.FromRgb(171.0/255.0, 189.0/255.0, 211.0/255.0);
				case TextDisplayMode.Proposal:  return Drawing.Color.FromRgb(221.0/255.0, 205.0/255.0, 188.0/255.0);
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryToolShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeBounds { get { return new Drawing.Margins(0,0,2,0); } }
		public override Drawing.Margins GeometryButtonShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRibbonShapeBounds { get { return new Drawing.Margins(0,0,0,2); } }
		public override Drawing.Margins GeometryTextFieldShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryListShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override double GeometryComboRightMargin { get { return 2; } }
		public override double GeometryComboBottomMargin { get { return 2; } }
		public override double GeometryComboTopMargin { get { return 2; } }
		public override double GeometryUpDownWidthFactor { get { return 0.8; } }
		public override double GeometryUpDownRightMargin { get { return 2; } }
		public override double GeometryUpDownBottomMargin { get { return 2; } }
		public override double GeometryUpDownTopMargin { get { return 2; } }
		public override double GeometryScrollerRightMargin { get { return 2; } }
		public override double GeometryScrollerBottomMargin { get { return 2; } }
		public override double GeometryScrollerTopMargin { get { return 2; } }
		public override double GeometryScrollListXMargin { get { return 2; } }
		public override double GeometryScrollListYMargin { get { return 2; } }
		public override double GeometrySliderLeftMargin { get { return 0; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 0; } }


		protected Drawing.Image		bitmap;
		protected Drawing.Color		colorGlyph;
		protected Drawing.Color		colorGreen;
		protected Drawing.Color		colorCaptionLight;
		protected Drawing.Color		colorCaptionProposal;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorBorderLight;
		protected Drawing.Color		colorBorderButton;
		protected Drawing.Color		colorEntered;
		protected Drawing.Color		colorDefault;
		protected Drawing.Color		colorDisabled;
		protected Drawing.Color		colorFrame;
		protected Drawing.Color		colorError;
		protected Drawing.Color		colorWindow;
		protected Drawing.Color		colorTextBackground;
	}
}
