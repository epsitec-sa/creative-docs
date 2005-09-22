namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookMetal implémente le décorateur métalique rigolo.
	/// </summary>
	public class LookMetal : IAdorner
	{
		public LookMetal()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookMetal.png", typeof (IAdorner));
			this.RefreshColors();
		}

		// Initialise les couleurs en fonction des réglages de Windows.
		public void RefreshColors()
		{
			double r,g,b;

			this.colorBlack             = Drawing.Color.FromName("WindowFrame");
			this.colorControl           = Drawing.Color.FromName("Control");
			this.colorControlLight      = Drawing.Color.FromName("ControlLight");
			this.colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			this.colorControlDark       = Drawing.Color.FromName("ControlDark");
			this.colorControlDarkDark   = Drawing.Color.FromName("ControlDarkDark");
			this.colorCaptionText       = Drawing.Color.FromName("ActiveCaptionText");
			this.colorInfo              = Drawing.Color.FromName("Info");

			r = 1-(1-this.colorControlLight.R)*0.5;
			g = 1-(1-this.colorControlLight.G)*0.5;
			b = 1-(1-this.colorControlLight.B)*0.5;
			this.colorScrollerBack = Drawing.Color.FromRGB(r,g,b);

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorButton = Drawing.Color.FromRGB(r,g,b);

			this.colorCaption        = Drawing.Color.FromRGB(170.0/255.0, 195.0/255.0, 249.0/255.0);
			this.colorCaptionNF      = Drawing.Color.FromRGB(210.0/255.0, 210.0/255.0, 210.0/255.0);
			this.colorHilite         = Drawing.Color.FromRGB(147.0/255.0, 172.0/255.0, 245.0/255.0);
			this.colorBorder         = Drawing.Color.FromRGB(102.0/255.0, 102.0/255.0, 126.0/255.0);
			this.colorDisabled       = Drawing.Color.FromRGB(176.0/255.0, 176.0/255.0, 188.0/255.0);
			this.colorError          = Drawing.Color.FromRGB(255.0/255.0, 177.0/255.0, 177.0/255.0);
			this.colorTextBackground = Drawing.Color.FromRGB(247.0/255.0, 247.0/255.0, 248.0/255.0);
			this.colorWindow         = Drawing.Color.FromRGB(210.0/255.0, 213.0/255.0, 223.0/255.0);
		}
		

		// Dessine le fond d'une fenêtre.
		public void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetState state)
		{
			graphics.AddFilledRectangle(paintRect);
			graphics.RenderSolid(this.colorWindow);
		}

		// Dessine une icône simple (dans un bouton d'ascenseur par exemple).
		public void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state,
							   GlyphShape type,
							   PaintTextStyle style)
		{
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
				graphics.RenderSolid(Drawing.Color.FromRGB(this.colorWindow.R-0.1, this.colorWindow.G-0.1, this.colorWindow.B-0.1));
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

				case GlyphShape.TabRight:
					path.MoveTo(center.X-rect.Width*0.10, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.00, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo(center.X-rect.Width*0.10, center.Y-rect.Height*0.15);
					break;

				case GlyphShape.TabLeft:
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
			}
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				Drawing.Color color = this.colorBlack;
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRGB(0.5, 0.0, 0.0);  // rouge foncé
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRGB(0.0, 0.5, 0.0);  // vert foncé
				graphics.RenderSolid(color);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		// Dessine un bouton à cocher sans texte.
		public void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					graphics.RenderSolid(this.colorControl);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
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
				graphics.RenderSolid(Drawing.Color.FromRGB(221.0/255.0, 224.0/255.0, 227.0/255.0));
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			rInside = rect;
			rInside.Deflate(0.5);
			graphics.AddRectangle(rInside);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}

			if ( (state&WidgetState.ActiveYes) != 0 )  // coché ?
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
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}

			if ( (state&WidgetState.ActiveMaybe) != 0 )  // 3ème état ?
			{
				rect.Deflate(3);
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}
		}

		// Dessine un bouton radio sans texte.
		public void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintCircle(graphics, rect, this.colorBorder);
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				this.PaintCircle(graphics, rect, this.colorDisabled);
			}

			rInside = rect;
			rInside.Deflate(1);

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
			}

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintCircle(graphics, rInside, this.colorControl);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorControlLightLight);
				}
			}
			else
			{
				this.PaintCircle(graphics, rInside, Drawing.Color.FromRGB(221.0/255.0, 224.0/255.0, 227.0/255.0));
			}

			if ( (state&WidgetState.ActiveYes) != 0 )  // coché ?
			{
				rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorBlack);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorDisabled);
				}
			}
		}

		public void PaintIcon(Drawing.Graphics graphics,
							  Drawing.Rectangle rect,
							  Widgets.WidgetState state,
							  string icon)
		{
		}

		// Dessine le fond d'un bouton rectangulaire.
		public void PaintButtonBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetState state,
										  Widgets.Direction dir,
										  Widgets.ButtonStyle style)
		{
			Drawing.Rectangle rFocus = rect;
			rFocus.Deflate(3.5);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( style == ButtonStyle.DefaultAccept )
					{
						rFocus.Deflate(1);
						rInside.Deflate(1);
					}

					if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 4);
					}
					else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 1);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 0);
					}

					if ( style == ButtonStyle.DefaultAccept )
					{
						rect.Deflate(0.5);
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 2);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 1);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 3);

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
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 12);
					}
					else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 9);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 8);
					}

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 11);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}

				if ( style == ButtonStyle.Icon )  rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rInside, 8);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				if ( (state&WidgetState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 12);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rInside, 10);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
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

			if ( (state&WidgetState.Focused) != 0 )
			{
				this.PaintFocusBox(graphics, rFocus);
			}
		}

		// Dessine le texte d'un bouton.
		public void PaintButtonTextLayout(Drawing.Graphics graphics,
										  Drawing.Point pos,
										  TextLayout text,
										  WidgetState state,
										  ButtonStyle style)
		{
			if ( text == null )  return;

			if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
			{
				pos.X ++;
				pos.Y --;
			}
			if ( style != ButtonStyle.Tab )
			{
				state &= ~WidgetState.Focused;
			}
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, PaintTextStyle.Button, Drawing.Color.Empty);
		}

		public void PaintButtonForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetState state,
										  Widgets.Direction dir,
										  Widgets.ButtonStyle style)
		{
		}

		// Dessine le fond d'une ligne éditable.
		public void PaintTextFieldBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetState state,
											 Widgets.TextFieldStyle style,
											 bool readOnly)
		{
			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Multi  ||
				 style == TextFieldStyle.Combo  ||
				 style == TextFieldStyle.UpDown )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					if ( (state&WidgetState.Error) != 0 )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorError);
					}
					else
					{
						this.PaintImageButton(graphics, rect, readOnly?20:16);
					}

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 19);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorControlLightLight);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControlLightLight);
			}
		}

		public void PaintTextFieldForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetState state,
											 Widgets.TextFieldStyle style,
											 bool readOnly)
		{
		}

		// Dessine le fond d'un ascenseur.
		public void PaintScrollerBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle thumbRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, frameRect, 22);
			}
			else
			{
				this.PaintImageButton(graphics, frameRect, 19);
			}

			if ( !tabRect.IsSurfaceZero && (state&WidgetState.Engaged) != 0 )
			{
				this.PaintImageButton(graphics, tabRect, 18);
			}
		}

		// Dessine la cabine d'un ascenseur.
		public void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetState state,
										Widgets.Direction dir)
		{
			state &= ~WidgetState.Engaged;
			this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

			Drawing.Rectangle	rect;
			Drawing.Point		center;

			if ( (state&WidgetState.Enabled) != 0 )
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
							graphics.RenderSolid(this.colorControlDark);

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
							graphics.RenderSolid(this.colorControlDark);

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

		public void PaintScrollerForeground(Drawing.Graphics graphics,
											Drawing.Rectangle thumbRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
		}

		// Dessine le cadre d'un GroupBox.
		public void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetState state)
		{
			Drawing.Rectangle rect = frameRect;
			rect.Deflate(0.5);
			graphics.LineWidth = 1;
			this.RectangleGroupBox(graphics, rect, titleRect.Left, titleRect.Right);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintSepLine(Drawing.Graphics graphics,
								 Drawing.Rectangle frameRect,
								 Drawing.Rectangle titleRect,
								 Widgets.WidgetState state,
								 Widgets.Direction dir)
		{
		}

		public void PaintFrameTitleBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
											  Widgets.WidgetState state,
											  Widgets.Direction dir)
		{
		}

		public void PaintFrameTitleForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
											  Widgets.WidgetState state,
											  Widgets.Direction dir)
		{
		}

		public void PaintFrameBody(Drawing.Graphics graphics,
								   Drawing.Rectangle rect,
								   Widgets.WidgetState state,
								   Widgets.Direction dir)
		{
		}

		// Dessine toute la bande sous les onglets.
		public void PaintTabBand(Drawing.Graphics graphics,
								 Drawing.Rectangle rect,
								 Widgets.WidgetState state,
								 Widgets.Direction dir)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);
		}

		// Dessine la zone principale sous les onglets.
		public void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetState state,
								  Widgets.Direction dir)
		{
			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 28);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 11);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		// Dessine l'onglet devant les autres.
		public void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
			titleRect.Bottom += 1;
			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 26);
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
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
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintTabAboveForeground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
		}

		// Dessine un onglet derrière (non sélectionné).
		public void PaintTabSunkenBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetState state,
											 Widgets.Direction dir)
		{
			titleRect.Left  += 1;
			titleRect.Right -= 1;
			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 26);
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
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
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintTabSunkenForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetState state,
											 Widgets.Direction dir)
		{
		}

		// Dessine le fond d'un tableau.
		public void PaintArrayBackground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetState state)
		{
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
			{
				this.PaintImageButton(graphics, rect, 16);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 19);
			}
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintArrayForeground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetState state)
		{
		}

		// Dessine le fond d'une cellule.
		public void PaintCellBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state)
		{
			if ( (state&WidgetState.Selected) != 0 )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Focused) != 0 )
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorCaptionNF);
				}
			}
		}

		// Dessine le fond d'un bouton d'en-tête de tableau.
		public void PaintHeaderBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction dir)
		{
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
			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, rect, 9);
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 8);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 11);
			}

			if ( dir == Direction.Up )
			{
				Drawing.Path pTitle = this.PathTopRectangle(rect);
				graphics.Rasterizer.AddOutline(pTitle, 1);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			if ( dir == Direction.Left )
			{
				Drawing.Path pTitle = this.PathLeftRectangle(rect);
				graphics.Rasterizer.AddOutline(pTitle, 1);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}
		}

		public void PaintHeaderForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction dir)
		{
		}

		// Dessine le fond d'une barre d'outil.
		public void PaintToolBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction dir)
		{
			this.PaintImageButton(graphics, rect, (dir == Direction.Up) ? 23 : 31);
		}

		public void PaintToolForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction dir)
		{
		}

		// Dessine le fond d'un menu.
		public void PaintMenuBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction dir,
										Drawing.Rectangle parentRect,
										double iconWidth)
		{
			this.PaintImageButton(graphics, rect, 16);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Left += 1;
				band.Width = iconWidth-1;
				band.Top -= 1;
				band.Bottom += 1;
				this.PaintImageButton(graphics, band, 21);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintMenuForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction dir,
										Drawing.Rectangle parentRect,
										double iconWidth)
		{
		}

		// Dessine le fond d'une case de menu.
		public void PaintMenuItemBackground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetState state,
											Direction dir,
											MenuType type,
											MenuItemType itemType)
		{
			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( type == MenuType.Horizontal )
				{
					if ( itemType == MenuItemType.Select )
					{
						this.PaintImageButton(graphics, rect, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					if ( itemType == MenuItemType.Parent )
					{
						this.PaintImageButton(graphics, rect, 8);

						Drawing.Rectangle rInside;
						rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddLine(rInside.Left, rInside.Bottom-0.5, rInside.Left, rInside.Top);
						graphics.AddLine(rInside.Left, rInside.Top, rInside.Right, rInside.Top);
						graphics.AddLine(rInside.Right, rInside.Top, rInside.Right, rInside.Bottom-0.5);
						graphics.RenderSolid(this.colorBorder);
					}
				}

				if ( type == MenuType.Vertical )
				{
					if ( itemType != MenuItemType.Deselect )
					{
						this.PaintImageButton(graphics, rect, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
				}
			}
			else
			{
				if ( itemType != MenuItemType.Deselect )
				{
					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
			}
		}

		// Dessine le texte d'un menu.
		public void PaintMenuItemTextLayout(Drawing.Graphics graphics,
											Drawing.Point pos,
											TextLayout text,
											WidgetState state,
											Direction dir,
											MenuType type,
											MenuItemType itemType)
		{
			if ( text == null )  return;
			state &= ~WidgetState.Selected;
			state &= ~WidgetState.Focused;
			PaintTextStyle style = ( type == MenuType.Horizontal ) ? PaintTextStyle.HMenu : PaintTextStyle.VMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, style, Drawing.Color.Empty);
		}

		// Dessine le devant d'une case de menu.
		public void PaintMenuItemForeground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetState state,
											Direction dir,
											MenuType type,
											MenuItemType itemType)
		{
		}

		// Dessine un séparateur horizontal ou vertical.
		public void PaintSeparatorBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetState state,
											 Direction dir,
											 bool optional)
		{
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

			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintSeparatorForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetState state,
											 Direction dir,
											 bool optional)
		{
		}

		// Dessine un bouton séparateur de panneaux.
		public void PaintPaneButtonBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state,
											  Direction dir)
		{
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

		public void PaintPaneButtonForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state,
											  Direction dir)
		{
		}

		// Dessine une ligne de statuts.
		public void PaintStatusBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state)
		{
			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintStatusForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state)
		{
		}

		// Dessine une case de statuts.
		public void PaintStatusItemBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state)
		{
			rect.Width -= 1;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintStatusItemForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state)
		{
		}

		// Dessine un tag.
		public void PaintTagBackground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetState state,
									   Drawing.Color color,
									   Direction dir)
		{
			Drawing.Path path;
			
			path = new Drawing.Path();
			path.AppendCircle(rect.Center, rect.Width/2, rect.Height/2);
			graphics.Rasterizer.AddSurface(path);
			if ( (state&WidgetState.Enabled) == 0 )
			{
				Drawing.Color topColor    = Drawing.Color.FromRGB(221.0/255.0, 224.0/255.0, 227.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRGB(214.0/255.0, 216.0/255.0, 219.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}
			else if ( color.IsEmpty )
			{
				Drawing.Color topColor    = Drawing.Color.FromRGB(253.0/255.0, 253.0/255.0, 253.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRGB(205.0/255.0, 204.0/255.0, 223.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}
			else
			{
				Drawing.Color topColor    = Drawing.Color.FromColor(color, 0.0);
				Drawing.Color bottomColor = Drawing.Color.FromColor(color, 0.5);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}

			Drawing.Rectangle rInside;

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
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
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public void PaintTagForeground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetState state,
									   Drawing.Color color,
									   Direction dir)
		{
		}

		// Dessine le fond d'une bulle d'aide.
		public void PaintTooltipBackground(Drawing.Graphics graphics,
										   Drawing.Rectangle rect)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorInfo);  // fond jaune pale
			
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBlack);  // cadre noir
		}

		// Dessine le texte d'une bulle d'aide.
		public void PaintTooltipTextLayout(Drawing.Graphics graphics,
										   Drawing.Point pos,
										   TextLayout text)
		{
			text.Paint(pos, graphics);
		}


		// Dessine le rectangle pour indiquer le focus.
		public void PaintFocusBox(Drawing.Graphics graphics,
								  Drawing.Rectangle rect)
		{
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDark);
		}

		// Dessine le curseur du texte.
		public void PaintTextCursor(Drawing.Graphics graphics,
									Drawing.Point p1, Drawing.Point p2,
									bool cursorOn)
		{
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
		
		// Dessine les zones rectanglaires correspondant aux caractères sélectionnés.
		public void PaintTextSelectionBackground(Drawing.Graphics graphics,
												 TextLayout.SelectedArea[] areas,
												 WidgetState state)
		{
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				graphics.AddFilledRectangle(areas[i].Rect);
				if ( (state&WidgetState.Focused) != 0 )
				{
					graphics.RenderSolid(this.colorCaption);

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

		public void PaintTextSelectionForeground(Drawing.Graphics graphics,
												 TextLayout.SelectedArea[] areas,
												 WidgetState state)
		{
		}

		// Dessine le texte d'un widget.
		public void PaintGeneralTextLayout(Drawing.Graphics graphics,
										   Drawing.Rectangle clipRect,
										   Drawing.Point pos,
										   TextLayout text,
										   WidgetState state,
										   PaintTextStyle style,
										   Drawing.Color backColor)
		{
			if ( text == null )  return;

			Drawing.TextStyle.DefineDefaultColor(this.colorBlack);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
				{
					text.Paint(pos, graphics, clipRect, this.colorBlack, Drawing.GlyphPaintStyle.Selected);
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

			if ( (state&WidgetState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = text.StandardRectangle;
				rFocus.Offset(pos);
				graphics.Align(ref rFocus);
				rFocus.Inflate(2.5, -0.5);
				this.PaintFocusBox(graphics, rFocus);
			}
		}


		// Dessine un rectangle
		protected void RectangleGroupBox(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 double startX, double endX)
		{
			graphics.AddLine(rect.Left, rect.Top, startX, rect.Top);
			graphics.AddLine(endX, rect.Top, rect.Right, rect.Top);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
			graphics.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
		}

		// Crée le chemin d'un rectangle à coins arrondis.
		protected Drawing.Path PathRoundRectangle(Drawing.Rectangle rect, double radius)
		{
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

		// Crée le chemin d'un rectangle en forme de "U" inversé.
		protected Drawing.Path PathTopRectangle(Drawing.Rectangle rect)
		{
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

		// Crée le chemin d'un rectangle en forme de "D" inversé.
		protected Drawing.Path PathLeftRectangle(Drawing.Rectangle rect)
		{
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

		// Crée le chemin d'un rectangle "corné" en forme de "U" inversé.
		protected Drawing.Path PathTopCornerRectangle(Drawing.Rectangle rect)
		{
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

		// Crée le chemin d'un rectangle "corné" en forme de "D" inversé.
		protected Drawing.Path PathLeftCornerRectangle(Drawing.Rectangle rect)
		{
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

		// Dessine un cercle complet.
		protected void PaintCircle(Drawing.Graphics graphics,
								   Drawing.Rectangle rect,
								   Drawing.Color color)
		{
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

		// Retourne le rayon à utiliser pour une zone rectangulaire.
		protected double RetRadiusImage(Drawing.Rectangle rect)
		{
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Min(3, dim/4));
		}

		// Retourne le rayon à utiliser pour une zone rectangulaire.
		protected double RetRadius(Drawing.Rectangle rect)
		{
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Min(4.5, dim/4));
		}

		// Dessine un bouton composé de 9 morceaux d'image.
		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank)
		{
			Drawing.Rectangle icon = new Drawing.Rectangle();
			icon.Left   = 32*(rank%8);
			icon.Right  = icon.Left+32;
			icon.Top    = 256-32*(rank/8);
			icon.Bottom = icon.Top-32;

			if ( rank == 16 || rank == 17 || rank == 19 || rank == 21 || rank == 28 || rank == 44 )
			{
				this.PaintImageButton1(graphics, rect, icon);
			}
			else if ( rank == 24 || rank == 25 || rank == 26 || rank == 27 || rank == 32 || rank == 33 || rank == 34 || rank == 35 )
			{
				this.PaintImageButton9(graphics, rect, 5, icon, 5);
			}
			else
			{
				this.PaintImageButton9(graphics, rect, this.RetRadiusImage(rect), icon, 3);
			}
		}

		// Dessine un bouton composé d'un seul morceau d'image.
		protected void PaintImageButton1(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 Drawing.Rectangle icon)
		{
			icon.Deflate(0.5);
			graphics.PaintImage(this.bitmap, rect, icon);
		}

		// Dessine un bouton composé de 9 morceaux d'image.
		protected void PaintImageButton9(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 double rectMargin,
										 Drawing.Rectangle icon,
										 double iconMargin)
		{
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


		public void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
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

		public Drawing.Color ColorCaption
		{
			get { return this.colorCaption; }
		}

		public Drawing.Color ColorControl
		{
			get { return this.colorControl; }
		}

		public Drawing.Color ColorWindow
		{
			get { return this.colorWindow; }
		}

		public Drawing.Color ColorDisabled
		{
			get { return Drawing.Color.Empty; }
		}

		public Drawing.Color ColorBorder
		{
			get { return this.colorBorder; }
		}

		public Drawing.Color ColorTextBackground
		{
			get { return this.colorTextBackground; }
		}

		public Drawing.Color ColorText(WidgetState state)
		{
			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
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
				return this.colorDisabled;
			}
		}

		public Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return (enabled ? this.colorBorder : this.colorDisabled);
		}

		public Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return (enabled ? this.colorBorder : this.colorDisabled);
		}

		public double AlphaVMenu { get { return 1.0; } }

		public Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(3,3,3,3); } }
		public Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public Drawing.Margins GeometryToolShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryButtonShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryTextFieldShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryListShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public double GeometryComboRightMargin { get { return 2; } }
		public double GeometryComboBottomMargin { get { return 2; } }
		public double GeometryComboTopMargin { get { return 2; } }
		public double GeometryUpDownWidthFactor { get { return 0.6; } }
		public double GeometryUpDownRightMargin { get { return 0; } }
		public double GeometryUpDownBottomMargin { get { return 0; } }
		public double GeometryUpDownTopMargin { get { return 0; } }
		public double GeometryScrollerRightMargin { get { return 2; } }
		public double GeometryScrollerBottomMargin { get { return 2; } }
		public double GeometryScrollerTopMargin { get { return 2; } }
		public double GeometryScrollListXMargin { get { return 2; } }
		public double GeometryScrollListYMargin { get { return 2; } }
		public double GeometrySliderLeftMargin { get { return 0; } }
		public double GeometrySliderRightMargin { get { return 0; } }
		public double GeometrySliderBottomMargin { get { return 0; } }


		protected Drawing.Image		bitmap;
		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorControl;
		protected Drawing.Color		colorControlLight;
		protected Drawing.Color		colorControlLightLight;
		protected Drawing.Color		colorControlDark;
		protected Drawing.Color		colorControlDarkDark;
		protected Drawing.Color		colorScrollerBack;
		protected Drawing.Color		colorCaption;
		protected Drawing.Color		colorCaptionNF;  // NF = no focused
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorDisabled;
		protected Drawing.Color		colorError;
		protected Drawing.Color		colorTextBackground;
		protected Drawing.Color		colorWindow;
	}
}
