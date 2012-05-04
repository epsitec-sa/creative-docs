namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookPastel implémente le décorateur pastel rigolo.
	/// </summary>
	public class LookPastel : AbstractAdorner
	{
		public LookPastel()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookPastel.png", typeof (IAdorner));
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs en fonction des réglages de Windows.
			this.colorBlack             = Drawing.Color.FromBrightness(0);
			this.colorControl           = Drawing.Color.FromName("Control");
			this.colorControlLight      = Drawing.Color.FromName("ControlLight");
			this.colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			this.colorControlDark       = Drawing.Color.FromName("ControlDark");
			this.colorControlDarkDark   = Drawing.Color.FromName("ControlDarkDark");
			this.colorInfo              = Drawing.Color.FromName("Info");

			this.colorGlyph             = Drawing.Color.FromRgb ( 68.0/255.0, 106.0/255.0, 140.0/255.0);
			this.colorCaption           = Drawing.Color.FromRgb (111.0/255.0, 189.0/255.0, 249.0/255.0);
			this.colorCaptionNF         = Drawing.Color.FromRgb (180.0/255.0, 230.0/255.0, 255.0/255.0);
			this.colorCaptionProposal   = Drawing.Color.FromRgb (240.0/255.0, 203.0/255.0,   0.0/255.0);
			this.colorHilite            = Drawing.Color.FromRgb (250.0/255.0, 196.0/255.0,  89.0/255.0);
			this.colorBorder            = Drawing.Color.FromRgb (122.0/255.0, 148.0/255.0, 170.0/255.0);
			this.colorError             = Drawing.Color.FromHexa ("ffb1b1");  // rouge pâle
			this.colorUndefinedLanguage = Drawing.Color.FromHexa ("b1e3ff");  // bleu pâle
			this.colorTextBackground    = Drawing.Color.FromRgb (250.0/255.0, 252.0/255.0, 252.0/255.0);
			this.colorWindow            = Drawing.Color.FromRgb (255.0/255.0, 255.0/255.0, 255.0/255.0);
		}
		

		public override void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetPaintState state)
		{
			//	Dessine le fond d'une fenêtre.
#if false
			Drawing.Color topColor    = Drawing.Color.FromRgb(162.0/255.0, 212.0/255.0, 252.0/255.0);
			Drawing.Color bottomColor = Drawing.Color.FromRgb(249.0/255.0, 252.0/255.0, 255.0/255.0);
			this.GradientRect(graphics, windowRect, bottomColor, topColor, 0);
#else
			Drawing.Rectangle rect;
			Drawing.Color topColor;
			Drawing.Color bottomColor;

			rect = windowRect;
			rect.Height = windowRect.Height*0.2;
			graphics.Align(ref rect);
			if ( rect.IntersectsWith(paintRect) )
			{
				topColor    = Drawing.Color.FromRgb(205.0/255.0, 232.0/255.0, 253.0/255.0);
				bottomColor = Drawing.Color.FromRgb(249.0/255.0, 252.0/255.0, 255.0/255.0);
				this.GradientRect(graphics, rect, bottomColor, topColor, 0);
			}

			rect.Bottom = rect.Top;
			rect.Height = windowRect.Height*0.5;
			graphics.Align(ref rect);
			if ( rect.IntersectsWith(paintRect) )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRgb(205.0/255.0, 232.0/255.0, 253.0/255.0));
			}

			rect.Bottom = rect.Top;
			rect.Top    = windowRect.Top;
			graphics.Align(ref rect);
			if ( rect.IntersectsWith(paintRect) )
			{
				topColor    = Drawing.Color.FromRgb(162.0/255.0, 212.0/255.0, 252.0/255.0);
				bottomColor = Drawing.Color.FromRgb(205.0/255.0, 232.0/255.0, 253.0/255.0);
				this.GradientRect(graphics, rect, bottomColor, topColor, 0);
			}
#endif
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
				color = this.colorControlLightLight;
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

				graphics.AddLine(p.X-11.5, p.Y+1.5, p.X-1.5, p.Y+11.5);
				graphics.AddLine(p.X-10.5, p.Y+1.5, p.X-1.5, p.Y+10.5);
				graphics.AddLine(p.X- 7.5, p.Y+1.5, p.X-1.5, p.Y+ 7.5);
				graphics.AddLine(p.X- 6.5, p.Y+1.5, p.X-1.5, p.Y+ 6.5);
				graphics.AddLine(p.X- 3.5, p.Y+1.5, p.X-1.5, p.Y+ 3.5);
				graphics.AddLine(p.X- 2.5, p.Y+1.5, p.X-1.5, p.Y+ 2.5);
				graphics.RenderSolid(this.colorBorder);

				graphics.AddLine(p.X-12.5, p.Y+1.5, p.X-1.5, p.Y+12.5);
				graphics.AddLine(p.X- 8.5, p.Y+1.5, p.X-1.5, p.Y+ 8.5);
				graphics.AddLine(p.X- 4.5, p.Y+1.5, p.X-1.5, p.Y+ 4.5);
				graphics.RenderSolid(Drawing.Color.FromBrightness(1.0));
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
				default:
					AbstractAdorner.GenerateGlyphShape (rect, type, center, path);
					break;

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

				case GlyphShape.Menu:
					path.MoveTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.25);
					path.LineTo(center.X-rect.Width*0.30, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.15);
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

			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
			{
				graphics.RenderSolid(this.colorControl);
			}
			else
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
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
			graphics.RenderSolid(this.ColorOutline(state));

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
					graphics.RenderSolid(this.colorGlyph);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}

			if ( (state&WidgetPaintState.ActiveMaybe) != 0 )  // 3ème état ?
			{
				rect.Deflate(3);
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorGlyph);
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

			this.PaintCircle(graphics, rect, this.ColorOutline(state));

			rInside = rect;
			rInside.Deflate(1);

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
			}

			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorControlLightLight);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorGlyph);
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
				rFocus.Deflate(2.5);
			}
			else
			{
				rFocus.Deflate(3.5);
			}

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel ||
				 style == ButtonStyle.DefaultAcceptAndCancel )
			{
				Drawing.Rectangle shadow = rect;
				shadow.Right  += 4;
				shadow.Bottom -= 4;
				this.PaintImageButton(graphics, shadow, 6);

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
					{
						rFocus.Deflate(1);
						rInside.Deflate(1);
					}

					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 3);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 1);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 0);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 2);
				}

				if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
				{
					rect.Deflate(0.5);
					double radius = this.RetRadius(rect);
					Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
					graphics.Rasterizer.AddOutline(pTitle, 2);
					graphics.RenderSolid(this.ColorOutline(state));
				}
				else
				{
					double radius = this.RetRadius(rect);
					Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.ColorOutline(state));
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
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 11);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 9);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 8);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 10);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.ColorOutline(state));

				if ( style == ButtonStyle.Icon )  rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.Slider )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 3);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 1);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 0);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 2);
				}

				double radius = this.RetRadius(rect);
				Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.ColorOutline(state));
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 11);
				}
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rInside, 8);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.ColorOutline(state));
				}
				rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.ComboItem )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 11);
				}
				if ( (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					if ((state&WidgetPaintState.InheritedEnter) == 0)
					{
						this.PaintImageButton(graphics, rInside, 11);
					}
					else  // groupe d'un combo ?
					{
						this.PaintImageButton(graphics, rInside, 16);
					}
				}
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rInside, 8);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.ColorOutline(state));
				}
				rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				if ( AbstractAdorner.IsThreeState2(state) )
				{
					rect.Top += 2;
					rInside.Top += 2;
				}

				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 12);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 9);

						rect.Deflate(0.5);
						Drawing.Path path = AbstractAdorner.PathThreeState2Frame(rect, state);
						graphics.Rasterizer.AddOutline(path, 1);
						graphics.RenderSolid(this.ColorOutline(state));
					}
				}
				else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 14);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 13);

						rect.Deflate(0.5);
						Drawing.Path path = AbstractAdorner.PathThreeState2Frame(rect, state);
						graphics.Rasterizer.AddOutline(path, 1);
						graphics.RenderSolid(this.ColorOutline(state));
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 11);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 8);
					}
				}
				rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.Confirmation )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rInside, 8);
				}
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 9);
				}
				rFocus.Inflate(1.0);
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
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, PaintTextStyle.Button, TextFieldDisplayMode.Default, Drawing.Color.Empty);
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
											 TextFieldDisplayMode mode,
											 bool readOnly, bool isMultilingual)
		{
			//	Dessine le fond d'une ligne éditable.
			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Multiline  ||
				 style == TextFieldStyle.Combo  ||
				 style == TextFieldStyle.UpDown )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					Drawing.Color color = this.ColorTextDisplayMode(mode);
					if ((state&WidgetPaintState.Error) != 0)
					{
						graphics.RenderSolid (this.colorError);
					}
					else if ((state&WidgetPaintState.UndefinedLanguage) != 0)
					{
						graphics.RenderSolid (this.colorUndefinedLanguage);
					}
					else if (!color.IsEmpty)
					{
						graphics.RenderSolid(color);
					}
					else
					{
						this.PaintImageButton(graphics, rect, readOnly?19:16);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 18);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.ColorOutline(state));
			}
			else if ( style == TextFieldStyle.Simple )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.ColorOutline(state));
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
											 TextFieldDisplayMode mode,
											 bool readOnly, bool isMultilingual)
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, frameRect, 20);
			}
			else
			{
				this.PaintImageButton(graphics, frameRect, 18);
			}

			if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
			{
				this.PaintImageButton(graphics, tabRect, 17);
			}
		}

		public override void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetPaintState state,
										Widgets.Direction dir)
		{
			//	Dessine la cabine d'un ascenseur.
			state &= ~WidgetPaintState.Engaged;
			this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

			Drawing.Rectangle	rect = new Drawing.Rectangle();
			Drawing.Point		center;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				switch ( dir )
				{
					case Direction.Up:
					case Direction.Down:
						if ( thumbRect.Width >= 10 && thumbRect.Height >= 20 )
						{
							center = new Drawing.Point((thumbRect.Left+thumbRect.Right)/2, (thumbRect.Bottom+thumbRect.Top)/2);
							rect.Left   = center.X-thumbRect.Width*0.3;
							rect.Right  = center.X+thumbRect.Width*0.3;
							rect.Bottom = center.Y-thumbRect.Width*0.3;
							rect.Top    = center.Y+thumbRect.Width*0.3;
							graphics.Align(ref rect);
							this.PaintImageButton(graphics, rect, 7);
						}
						break;

					case Direction.Left:
					case Direction.Right:
						if ( thumbRect.Height >= 10 && thumbRect.Width >= 20 )
						{
							center = new Drawing.Point((thumbRect.Left+thumbRect.Right)/2, (thumbRect.Bottom+thumbRect.Top)/2);
							rect.Left   = center.X-thumbRect.Height*0.3;
							rect.Right  = center.X+thumbRect.Height*0.3;
							rect.Bottom = center.Y-thumbRect.Height*0.3;
							rect.Top    = center.Y+thumbRect.Height*0.3;
							graphics.Align(ref rect);
							this.PaintImageButton(graphics, rect, 7);
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
			//	Dessine le fond d'un potentiomètre linéaire.
			if ( dir == Widgets.Direction.Left )
			{
				Drawing.Point p1 = new Drawing.Point (sliderRect.Left +frameRect.Height*0.2, frameRect.Center.Y);
				Drawing.Point p2 = new Drawing.Point (sliderRect.Right-frameRect.Height*0.2, frameRect.Center.Y);
				graphics.Align (ref p1);
				graphics.Align(ref p2);

				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X-0.5, p2.Y+0.5);
				graphics.RenderSolid(this.colorBorder);
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
				graphics.RenderSolid(this.colorBorder);
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
			//	Dessine la cabine d'un potentiomètre linéaire.
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
				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorBorder);

				graphics.AddLine(thumbRect.Center.X, thumbRect.Bottom+d+1, thumbRect.Center.X, thumbRect.Top-d);
				graphics.RenderSolid(this.colorBorder);
				graphics.AddLine(thumbRect.Center.X+1, thumbRect.Bottom+d+1, thumbRect.Center.X+1, thumbRect.Top-d);
				graphics.RenderSolid(this.colorControlLight);
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
				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorBorder);

				graphics.AddLine(thumbRect.Left+d, thumbRect.Center.Y, thumbRect.Right-d-1, thumbRect.Center.Y);
				graphics.RenderSolid(this.colorBorder);
				graphics.AddLine(thumbRect.Left+d, thumbRect.Center.Y-1, thumbRect.Right-d-1, thumbRect.Center.Y-1);
				graphics.RenderSolid(this.colorControlLight);
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
			this.PaintImageButton(graphics, rect, 3);

			double radius = this.RetRadius(rect);
			Drawing.Path path = this.PathRoundRectangle(rect, radius);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.colorBorder);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(3);
			if (style == ProgressIndicatorStyle.UnknownDuration)
			{
				double x = rInside.Width*progress;
				double w = rInside.Width*0.4;

				this.PaintProgressUnknow(graphics, rInside, w, x-w);
				this.PaintProgressUnknow(graphics, rInside, w, x-w+rInside.Width);
			}
			else
			{
				if (progress != 0)
				{
					rInside.Width *= progress;
					this.PaintImageButton(graphics, rInside, 1);
				}
			}

			rInside = rect;
			rInside.Deflate(3.5);
			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorBorder);
		}

		protected void PaintProgressUnknow(Drawing.Graphics graphics, Drawing.Rectangle rect, double w, double x)
		{
			Drawing.Rectangle fill = new Drawing.Rectangle(rect.Left+x, rect.Bottom, w, rect.Height);
			Drawing.Rectangle icon = new Drawing.Rectangle(128, 0, 32*4, 32);

			if (fill.Left < rect.Left)
			{
				double over = (rect.Left-fill.Left)/w;
				icon.Left += icon.Width*over;
				fill.Left = rect.Left;
			}

			if (fill.Right > rect.Right)
			{
				double over = (fill.Right-rect.Right)/w;
				icon.Width -= icon.Width*over;
				fill.Right = rect.Right;
			}

			if (fill.Width > 0)
			{
				graphics.PaintImage(this.bitmap, fill, icon);
			}
		}

		public override void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetPaintState state)
		{
			//	Dessine le cadre d'un GroupBox.
			Drawing.Rectangle rect = frameRect;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.2, 1,1,1));

			rect.Deflate(0.5);
			graphics.LineWidth = 1;
			this.RectangleGroupBox(graphics, rect, titleRect.Left, titleRect.Right);
			graphics.RenderSolid(this.ColorOutline(state));
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 5);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 10);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorOutline(state));
		}

		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine l'onglet devant les autres.
			titleRect.Bottom += 1;
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 26);
			}
			else
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 24);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 27);
				}
			}

			Drawing.Path pTitle = this.PathTopCornerRectangle(titleRect);
			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.ColorOutline(state));
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
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 26);
			}
			else
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 25);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 27);
				}
			}

			Drawing.Path pTitle = this.PathTopCornerRectangle(titleRect);
			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.ColorOutline(state));
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
				this.PaintImageButton(graphics, rect, 16);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 18);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorOutline(state));

			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(1.0);
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
				AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorBorder);
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
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, rect, 9);
			}
			else if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 8);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 10);
			}

			if ( dir == Direction.Up )
			{
				Drawing.Path pTitle = this.PathTopRectangle(rect);
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.ColorOutline(state));
			}
			if ( dir == Direction.Left )
			{
				Drawing.Path pTitle = this.PathLeftRectangle(rect);
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.ColorOutline(state));
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
			Drawing.Color topColor    = Drawing.Color.FromRgb(198.0/255.0, 225.0/255.0, 255.0/255.0);
			Drawing.Color bottomColor = Drawing.Color.FromRgb(249.0/255.0, 252.0/255.0, 255.0/255.0);
			this.GradientRect(graphics, rect, bottomColor, topColor, 90);

			if ( dir == Direction.Up )
			{
				graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
				graphics.RenderSolid(this.colorBorder);
			}

			if ( dir == Direction.Left )
			{
				graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.RenderSolid(this.colorBorder);
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
			this.PaintImageButton(graphics, rect, 16);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Left += 1;
				band.Width = iconWidth-1;
				band.Top -= 1;
				band.Bottom += 1;
				Drawing.Color topColor    = Drawing.Color.FromRgb(198.0/255.0, 225.0/255.0, 255.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRgb(223.0/255.0, 238.0/255.0, 255.0/255.0);
				this.GradientRect(graphics, band, bottomColor, topColor, 90);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorOutline(state));
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
											MenuItemState itemType)
		{
			//	Dessine le fond d'une case de menu.
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == MenuOrientation.Horizontal )
				{
					if ( itemType == MenuItemState.Selected )
					{
						this.PaintImageButton(graphics, rect, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.ColorOutline(state));
					}
					if ( itemType == MenuItemState.SubmenuOpen )
					{
						this.PaintImageButton(graphics, rect, 8);

						Drawing.Rectangle rInside;
						rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddLine(rInside.Left, rInside.Bottom-0.5, rInside.Left, rInside.Top);
						graphics.AddLine(rInside.Left, rInside.Top, rInside.Right, rInside.Top);
						graphics.AddLine(rInside.Right, rInside.Top, rInside.Right, rInside.Bottom-0.5);
						graphics.RenderSolid(this.ColorOutline(state));
					}
				}

				if ( type == MenuOrientation.Vertical )
				{
					if ( itemType != MenuItemState.Default )
					{
						this.PaintImageButton(graphics, rect, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.ColorOutline(state));
					}
				}
			}
			else
			{
				if ( itemType != MenuItemState.Default )
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
											MenuItemState itemType)
		{
			//	Dessine le texte d'un menu.
			if ( text == null )  return;
			state &= ~WidgetPaintState.Selected;
			state &= ~WidgetPaintState.Focused;
			PaintTextStyle style = ( type == MenuOrientation.Horizontal ) ? PaintTextStyle.HMenu : PaintTextStyle.VMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, style, TextFieldDisplayMode.Default, Drawing.Color.Empty);
		}

		public override void PaintMenuItemForeground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetPaintState state,
											Direction dir,
											MenuOrientation type,
											MenuItemState itemType)
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

			graphics.RenderSolid(Drawing.Color.FromBrightness(0.75));
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
			double x, y;
			if ( dir == Direction.Down || dir == Direction.Up )
			{
				x = rect.Left+0.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(Drawing.Color.FromRgb(255.0/255.0, 255.0/255.0, 255.0/255.0));

				x = rect.Left+1.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(Drawing.Color.FromRgb(209.0/255.0, 230.0/255.0, 251.0/255.0));

				x = rect.Right-1.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(Drawing.Color.FromRgb(157.0/255.0, 182.0/255.0, 202.0/255.0));

				x = rect.Right-0.5;
				graphics.AddLine(x, rect.Bottom, x, rect.Top);
				graphics.RenderSolid(Drawing.Color.FromRgb(122.0/255.0, 148.0/255.0, 170.0/255.0));
			}
			else
			{
				y = rect.Top-0.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(Drawing.Color.FromRgb(255.0/255.0, 255.0/255.0, 255.0/255.0));

				y = rect.Top-1.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(Drawing.Color.FromRgb(209.0/255.0, 230.0/255.0, 251.0/255.0));

				y = rect.Bottom+1.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(Drawing.Color.FromRgb(157.0/255.0, 182.0/255.0, 202.0/255.0));

				y = rect.Bottom+0.5;
				graphics.AddLine(rect.Left, y, rect.Right, y);
				graphics.RenderSolid(Drawing.Color.FromRgb(122.0/255.0, 148.0/255.0, 170.0/255.0));
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromRgb(212.0/255.0, 234.0/255.0, 252.0/255.0));

			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.RenderSolid(this.colorBorder);
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
			Drawing.Color topColor    = Drawing.Color.FromRgb(185.0/255.0, 221.0/255.0, 253.0/255.0);
			Drawing.Color bottomColor = Drawing.Color.FromRgb(212.0/255.0, 234.0/255.0, 252.0/255.0);
			this.GradientRect(graphics, rect, bottomColor, topColor, 45);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBorder);
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
			Drawing.Color topColor    = Drawing.Color.FromRgb(138.0/255.0, 178.0/255.0, 231.0/255.0);
			Drawing.Color bottomColor = Drawing.Color.FromRgb(168.0/255.0, 215.0/255.0, 252.0/255.0);
			this.GradientRect(graphics, header, bottomColor, topColor, 0);
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
			Drawing.Color topColor    = Drawing.Color.FromRgb(198.0/255.0, 225.0/255.0, 255.0/255.0);
			Drawing.Color bottomColor = Drawing.Color.FromRgb(249.0/255.0, 252.0/255.0, 255.0/255.0);
			this.GradientRect(graphics, rect, bottomColor, topColor, 90);

			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
			graphics.RenderSolid(this.colorBorder);
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
			if ((state&WidgetPaintState.ActiveYes) != 0)   // bouton activé ?
			{
				rect.Bottom -= 4;  // pour cacher la partie inférieure

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(1);
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rInside, 13);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 8);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.ColorOutline(state));
			}
			else
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rInside = rect;
					rInside.Deflate(1);
					this.PaintImageButton(graphics, rInside, 13);
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
			state &= ~WidgetPaintState.Focused;
			PaintTextStyle style = PaintTextStyle.HMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, style, TextFieldDisplayMode.Default, Drawing.Color.Empty);
		}

		public override void PaintRibbonSectionBackground(Drawing.Graphics graphics,
												 Drawing.Rectangle fullRect,
												 Drawing.Rectangle userRect,
												 Drawing.Rectangle textRect,
												 TextLayout text,
												 WidgetPaintState state)
		{
			//	Dessine une section d'un ruban.
			Drawing.Color topColor    = Drawing.Color.FromRgb(138.0/255.0, 178.0/255.0, 231.0/255.0);
			Drawing.Color bottomColor = Drawing.Color.FromRgb(168.0/255.0, 215.0/255.0, 252.0/255.0);
			this.GradientRect(graphics, textRect, bottomColor, topColor, 90);

			fullRect.Deflate(0.5);
			graphics.AddRectangle(fullRect);
			graphics.RenderSolid(this.ColorBorder);

			if (text != null)
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);
				Drawing.Point pos = new Drawing.Point(textRect.Left+3, textRect.Bottom+1);
				text.LayoutSize = new Drawing.Size(textRect.Width-4, textRect.Height);
				text.Alignment = Drawing.ContentAlignment.MiddleLeft;
				text.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.Split | Drawing.TextBreakMode.SingleLine;
				text.Paint (pos, graphics, Drawing.Rectangle.MaxValue, Drawing.Color.FromBrightness (0), Drawing.GlyphPaintStyle.Normal);
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
			if ( (state&WidgetPaintState.Enabled) == 0 )
			{
				Drawing.Color topColor    = Drawing.Color.FromRgb(155.0/255.0, 172.0/255.0, 189.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRgb( 93.0/255.0, 102.0/255.0, 133.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor, 0);
			}
			else if ( color.IsEmpty )
			{
				Drawing.Color topColor    = Drawing.Color.FromRgb(253.0/255.0, 253.0/255.0, 253.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRgb(205.0/255.0, 204.0/255.0, 223.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor, 0);
			}
			else
			{
				Drawing.Color topColor    = Drawing.Color.FromColor(color, 0.0);
				Drawing.Color bottomColor = Drawing.Color.FromColor(color, 0.5);
				this.Gradient(graphics, rect, bottomColor, topColor, 0);
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
			graphics.RenderSolid(this.ColorOutline(state));
		}

		public override void PaintTagForeground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetPaintState state,
									   Drawing.Color color,
									   Direction dir)
		{
		}

		public override void PaintTooltipBackground(Drawing.Graphics graphics,
										   Drawing.Rectangle rect, Drawing.Color backColor)
		{
			//	Dessine le fond d'une bulle d'aide.
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid (backColor.ColorOrDefault (this.colorInfo));  // fond jaune pale
			
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorGlyph);  // cadre noir
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
			AbstractAdorner.DrawFocusedRectangle(graphics, rect, Drawing.Color.FromRgb(143.0/255.0, 201.0/255.0, 255.0/255.0));
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
												 WidgetPaintState state, PaintTextStyle style, TextFieldDisplayMode mode)
		{
			//	Dessine les zones rectanglaires correspondant aux caractères sélectionnés.
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				graphics.AddFilledRectangle(areas[i].Rect);
				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( mode == TextFieldDisplayMode.InheritedValue )
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
												 WidgetPaintState state, PaintTextStyle style, TextFieldDisplayMode mode)
		{
		}

		public override void PaintGeneralTextLayout(Drawing.Graphics graphics,
										   Drawing.Rectangle clipRect,
										   Drawing.Point pos,
										   TextLayout text,
										   WidgetPaintState state,
										   PaintTextStyle style,
										   TextFieldDisplayMode mode,
										   Drawing.Color backColor)
		{
			//	Dessine le texte d'un widget.
			if ( text == null )  return;

			text = AbstractAdorner.AdaptTextLayout (text, mode);

			Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Selected) != 0 )
				{
					text.Paint(pos, graphics, clipRect, this.colorBlack, Drawing.GlyphPaintStyle.Selected);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )
				{
					text.Paint(pos, graphics, clipRect, Drawing.Color.Empty, Drawing.GlyphPaintStyle.Entered);
				}
				else
				{
					text.Paint(pos, graphics, clipRect, Drawing.Color.Empty, Drawing.GlyphPaintStyle.Normal);
				}
			}
			else
			{
				if ( style == PaintTextStyle.StaticText  ||
					 style == PaintTextStyle.Group       ||
					 style == PaintTextStyle.CheckButton ||
					 style == PaintTextStyle.RadioButton ||
					 style == PaintTextStyle.HMenu       )
				{
					text.Paint(pos, graphics, clipRect, this.colorControlDarkDark, Drawing.GlyphPaintStyle.Disabled);
				}
				else if ( style == PaintTextStyle.VMenu )
				{
					text.Paint(pos, graphics, clipRect, this.colorControlDark, Drawing.GlyphPaintStyle.Disabled);
				}
				else
				{
					text.Paint(pos, graphics, clipRect, this.colorControlLightLight, Drawing.GlyphPaintStyle.Disabled);
				}
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

		protected void GradientRect(Drawing.Graphics graphics,
									Drawing.Rectangle rect,
									Drawing.Color bottomColor,
									Drawing.Color topColor,
									double angle)
		{
			Drawing.Path path = new Drawing.Path();
			path.MoveTo(rect.BottomLeft);
			path.LineTo(rect.TopLeft);
			path.LineTo(rect.TopRight);
			path.LineTo(rect.BottomRight);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			this.Gradient(graphics, rect, bottomColor, topColor, angle);
		}

		protected void Gradient(Drawing.Graphics graphics,
								Drawing.Rectangle rect,
								Drawing.Color bottomColor,
								Drawing.Color topColor,
								double angle)
		{
			graphics.FillMode = Drawing.FillMode.NonZero;
			graphics.GradientRenderer.Fill = Drawing.GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Drawing.Transform ot = graphics.GradientRenderer.Transform;
			Drawing.Transform t = Drawing.Transform.Identity;
			Drawing.Point center = rect.Center;
			if ( angle == 0 )  t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			else               t = t.Scale(rect.Height/100/2, rect.Width/100/2);
			t = t.Translate (center);
			t = t.RotateDeg (angle, center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected double RetRadiusImage(Drawing.Rectangle rect)
		{
			//	Retourne le rayon à utiliser pour une zone rectangulaire.
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Min(6, dim/4));
		}

		protected double RetRadius(Drawing.Rectangle rect)
		{
			//	Retourne le rayon à utiliser pour une zone rectangulaire.
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Min(4.5, dim/4));
		}

		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank)
		{
			//	Dessine un bouton composé de 9 morceaux d'image.
			Drawing.Rectangle icon = new Drawing.Rectangle();
			icon.Left   = 32*(rank%8);
			icon.Right  = icon.Left+32;
			icon.Top    = 128-32*(rank/8);
			icon.Bottom = icon.Top-32;

			if ( rank == 5 || rank == 7 || rank == 16 || rank == 18 || rank == 20 )
			{
				this.PaintImageButton1(graphics, rect, icon);
			}
			else if ( rank == 24 || rank == 25 || rank == 26 || rank == 27 )
			{
				this.PaintImageButton9(graphics, rect, 5, icon, 5);
			}
			else
			{
				this.PaintImageButton9(graphics, rect, this.RetRadiusImage(rect), icon, 10);
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


		protected Drawing.Color ColorOutline(Widgets.WidgetPaintState state)
		{
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				return this.colorBorder;
			}
			else
			{
				return Drawing.Color.FromRgb(131.0/255.0, 156.0/255.0, 176.0/255.0);
			}
		}


		public override Drawing.Color AdaptPictogramColor(Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Normal )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = intensity*1.4-0.4;  // augmente le contraste
				color = Drawing.Color.FromAlphaRgb(alpha, 0.7*intensity, 0.9*intensity, 1.4*intensity);  // bleuté
				color = color.ClipToRange();
			}

			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.1;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.35, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromAlphaRgb(alpha, 0.9*intensity, intensity, 1.1*intensity);  // bleuté
				color = color.ClipToRange();
			}

			return color;
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
					return this.colorBlack;
				}
				else
				{
					return this.colorBlack;
				}
			}
			else
			{
				return this.colorControlLightLight;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.ColorOutline(enabled ? WidgetPaintState.Enabled : WidgetPaintState.None);
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.ColorOutline(enabled ? WidgetPaintState.Enabled : WidgetPaintState.None);
		}

		public override Drawing.Color ColorTextDisplayMode(TextFieldDisplayMode mode)
		{
			switch ( mode )
			{
				case TextFieldDisplayMode.Default:   return Drawing.Color.Empty;
				case TextFieldDisplayMode.OverriddenValue:   return Drawing.Color.FromRgb(198.0/255.0, 230.0/255.0, 255.0/255.0);
				case TextFieldDisplayMode.InheritedValue:  return Drawing.Color.FromRgb(255.0/255.0, 246.0/255.0, 200.0/255.0);
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(3,3,3,3); } }
		public override Drawing.Margins GeometryRadioShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryGroupShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryToolShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeMargins { get { return new Drawing.Margins(0,0,2,0); } }
		public override Drawing.Margins GeometryButtonShapeMargins { get { return new Drawing.Margins(0,4,0,4); } }
		public override Drawing.Margins GeometryRibbonShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryTextFieldShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryListShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override double GeometryComboRightMargin { get { return 2; } }
		public override double GeometryComboBottomMargin { get { return 2; } }
		public override double GeometryComboTopMargin { get { return 2; } }
		public override double GeometryUpDownWidthFactor { get { return 0.6; } }
		public override double GeometryUpDownRightMargin { get { return 0; } }
		public override double GeometryUpDownBottomMargin { get { return 0; } }
		public override double GeometryUpDownTopMargin { get { return 0; } }
		public override double GeometryScrollerRightMargin { get { return 2; } }
		public override double GeometryScrollerBottomMargin { get { return 2; } }
		public override double GeometryScrollerTopMargin { get { return 2; } }
		public override double GeometryScrollListXMargin { get { return 2; } }
		public override double GeometryScrollListYMargin { get { return 2; } }
		public override double GeometrySliderLeftMargin { get { return 0; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 0; } }


		protected Drawing.Image		bitmap;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorUndefinedLanguage;
		protected Drawing.Color		colorWindow;
		protected Drawing.Color		colorTextBackground;
		protected Drawing.Color		colorCaptionProposal;
		protected Drawing.Color		colorGlyph;
	}
}
