namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookBlue implémente le décorateur bleu & rigolo.
	/// </summary>
	public class LookBlue : IAdorner
	{
		public LookBlue()
		{
			this.RefreshColors();
		}

		// Initialise les couleurs en fonction des réglages de Windows.
		public void RefreshColors()
		{
			double r,g,b;

			this.colorBlack             = Drawing.Color.FromName("WindowFrame");
			this.colorWindow            = Drawing.Color.FromRGB(107.0/255.0, 144.0/255.0, 189.0/255.0);
			this.colorControl           = Drawing.Color.FromRGB(107.0/255.0, 144.0/255.0, 189.0/255.0);
			this.colorButton            = Drawing.Color.FromRGB(107.0/255.0, 144.0/255.0, 189.0/255.0);
			this.colorControlLight      = Drawing.Color.FromRGB(178.0/255.0, 198.0/255.0, 221.0/255.0);
			this.colorControlLightLight = Drawing.Color.FromRGB(218.0/255.0, 238.0/255.0, 255.0/255.0);
			this.colorControlDark       = Drawing.Color.FromRGB( 34.0/255.0,  78.0/255.0, 123.0/255.0);
			this.colorControlDarkDark   = Drawing.Color.FromRGB(  0.0/255.0,  51.0/255.0, 102.0/255.0);
			this.colorCaption           = Drawing.Color.FromRGB(255.0/255.0, 186.0/255.0,   1.0/255.0);
			this.colorCaptionNF         = Drawing.Color.FromRGB(178.0/255.0, 198.0/255.0, 221.0/255.0);
			this.colorCaptionText       = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorHilite            = Drawing.Color.FromRGB(255.0/255.0, 186.0/255.0,   1.0/255.0);
			this.colorInfo              = Drawing.Color.FromName("Info");

			r = 1-(1-this.colorControlLight.R)*0.5;
			g = 1-(1-this.colorControlLight.G)*0.5;
			b = 1-(1-this.colorControlLight.B)*0.5;
			this.colorScrollerBack = Drawing.Color.FromRGB(r,g,b);

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorControlReadOnly = Drawing.Color.FromRGB(r,g,b);

			r = 1-(1-this.colorCaption.R)*0.25;
			g = 1-(1-this.colorCaption.G)*0.25;
			b = 1-(1-this.colorCaption.B)*0.25;
			this.colorCaptionLight = Drawing.Color.FromRGB(r,g,b);
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
				case GlyphShape.Cancel:
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

				case GlyphShape.Validate:
					path.MoveTo(center.X-rect.Width*0.30, center.Y+rect.Height*0.00);
					path.LineTo(center.X-rect.Width*0.20, center.Y+rect.Height*0.10);
					path.LineTo(center.X-rect.Width*0.10, center.Y-rect.Height*0.05);
					path.LineTo(center.X+rect.Width*0.20, center.Y+rect.Height*0.30);
					path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.20);
					path.LineTo(center.X-rect.Width*0.10, center.Y-rect.Height*0.30);
					break;
			}
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				Drawing.Color color = this.colorBlack;
				if ( type == GlyphShape.Cancel   )  color = Drawing.Color.FromRGB(0.5, 0.0, 0.0);  // rouge foncé
				if ( type == GlyphShape.Validate )  color = Drawing.Color.FromRGB(0.0, 0.3, 0.0);  // vert foncé
				graphics.RenderSolid(color);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		// Dessine un bouton à cocher sans texte.
		public void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

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

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			rInside = rect;
			rInside.Deflate(0.5);
			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorControlDarkDark);

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
					graphics.RenderSolid(this.colorControlDark);
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

			this.PaintCircle(graphics, rect, this.colorControlDarkDark);

			rInside = rect;
			rInside.Deflate(1);

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
			}

			if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorControlLightLight);
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
					this.PaintCircle(graphics, rInside, this.colorControlDark);
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
			rFocus.Deflate(3.5, 3.5);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				if ( (state&WidgetState.Enabled) != 0 &&
					 style == ButtonStyle.DefaultAccept )
				{
					rect.Deflate(1, 1);
				}

				Drawing.Path path = this.PathRoundRectangle(rect, 0);

				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
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
			
				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rInside = rect;
					rInside.Deflate(1.5);
					Drawing.Path pInside = this.PathRoundRectangle(rInside, 0);
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid(this.colorHilite);
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					Drawing.Rectangle lightRect = rect;
					lightRect.Left += 1;
					lightRect.Top  -= 1;
					Drawing.Path lightPath = this.PathRoundRectangle(lightRect, 0);
					graphics.Rasterizer.AddOutline(lightPath, 1);
					graphics.RenderSolid(this.colorControlLightLight);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}

				if ( (state&WidgetState.Enabled) != 0 &&
					 style == ButtonStyle.DefaultAccept )
				{
					rect.Inflate(1);
					path = this.PathRoundRectangle(rect, 0);
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorBlack);
					rect.Deflate(1);
				}
			}
			else if ( style == ButtonStyle.Scroller     ||
					  style == ButtonStyle.Combo        ||
					  style == ButtonStyle.UpDown       ||
					  style == ButtonStyle.Icon         ||
					  style == ButtonStyle.HeaderSlider )
			{
				double radius = (style == ButtonStyle.UpDown) ? -1 : 0;
				Drawing.Path path = this.PathRoundRectangle(rect, radius);
			
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.colorControl);
			
				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rInside = rect;
					rInside.Deflate(1.5);
					Drawing.Path pInside = this.PathRoundRectangle(rInside, radius);
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid(this.colorHilite);
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					Drawing.Rectangle lightRect = rect;
					lightRect.Left += 1;
					lightRect.Top  -= 1;
					Drawing.Path lightPath = this.PathRoundRectangle(lightRect, radius);
					graphics.Rasterizer.AddOutline(lightPath, 1);
					graphics.RenderSolid(this.colorControlLightLight);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				rect.Right += 1;

				if ( (state&WidgetState.Entered)   != 0 ||  // bouton survolé ?
					 (state&WidgetState.Engaged)   != 0 ||  // bouton pressé ?
					 (state&WidgetState.ActiveYes) != 0 )   // bouton activé ?
				{
					Drawing.Path path = this.PathRoundRectangle(rect, 0);

					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorCaptionLight);

					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorCaption);
				}
				rFocus.Inflate(3.0);
				rFocus.Right ++;
			}
			else if ( style == ButtonStyle.ListItem )
			{
				if ( (state&WidgetState.Selected) != 0 )
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

			if ( (state&WidgetState.Engaged) != 0 &&  // bouton pressé ?
				 style == ButtonStyle.ToolItem )
			{
				pos.X ++;
				pos.Y --;
			}
			if ( style != ButtonStyle.Tab )
			{
				state &= ~WidgetState.Focused;
			}
			this.PaintGeneralTextLayout(graphics, pos, text, state, PaintTextStyle.Button, Drawing.Color.Empty);
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
				 style == TextFieldStyle.Combo  )
			{
				Drawing.Path path = this.PathRoundRectangle(rect, 0);

				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}
			else if ( style == TextFieldStyle.UpDown )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;
				Drawing.Rectangle rInside = rect;
				rInside.Deflate(0.5);
				graphics.AddRectangle(rInside);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
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
			Drawing.Path path = this.PathRoundRectangle(frameRect, 0);

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.colorScrollerBack);

			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.colorControlDark);

			if ( !tabRect.IsSurfaceZero && (state&WidgetState.Engaged) != 0 )
			{
				graphics.AddFilledRectangle(tabRect);
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		// Dessine la cabine d'un ascenseur.
		public void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetState state,
										Widgets.Direction dir)
		{
			this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);
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
			Drawing.Path path = this.PathRoundRectangleGroupBox(frameRect, titleRect.Left, titleRect.Right);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.colorControlDark);
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
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDarkDark);

			rect.Deflate(0.5);
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlLight);
			}
			else
			{
				graphics.RenderSolid(this.colorControl);
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

			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, 0);

			graphics.Rasterizer.AddSurface(pTitle);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlLight);
			}
			else
			{
				graphics.RenderSolid(this.colorControl);
			}

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, 0);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}

			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.colorControlDarkDark);
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

			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, 0);

			graphics.Rasterizer.AddSurface(pTitle);
			graphics.RenderSolid(this.colorControl);

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, 0);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}

			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.colorControlDarkDark);
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
			Drawing.Path path = this.PathRoundRectangle(rect, 0);

			graphics.Rasterizer.AddSurface(path);
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(this.colorControl);
			}

			graphics.Rasterizer.AddOutline(path, 1);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBlack);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
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

			Drawing.Path path;
			if ( dir == Direction.Up )
			{
				path = this.PathTopRoundRectangle(rect, 0);
			}
			else
			{
				path = this.PathLeftRoundRectangle(rect, 0);
			}
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.colorControl);

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				if ( dir == Direction.Up )
				{
					rect.Top = rect.Bottom+2;
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorHilite);
				}
				if ( dir == Direction.Left )
				{
					rect.Left = rect.Right-2;
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorHilite);
				}
			}

			graphics.Rasterizer.AddOutline(path, 1);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBlack);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlLight);
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlLightLight);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Width = iconWidth;
				band.Top -= 1;
				band.Bottom += 1;
				graphics.AddFilledRectangle(band);
				graphics.RenderSolid(this.colorControlLight);
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
			if ( type == MenuType.Horizontal )
			{
				if ( itemType == MenuItemType.Select )
				{
					Drawing.Path path = this.PathRoundRectangle(rect, 0);

					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorCaptionLight);

					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorCaption);
				}
				if ( itemType == MenuItemType.Parent )
				{
					Drawing.Path path = this.PathTopRoundRectangle(rect, 0);

					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorCaptionLight);

					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorControlDark);
				}
			}

			if ( type == MenuType.Vertical )
			{
				if ( itemType != MenuItemType.Deselect )
				{
					Drawing.Path path = this.PathRoundRectangle(rect, 0);

					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorCaptionLight);

					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorCaption);
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
			this.PaintGeneralTextLayout(graphics, pos, text, state, style, Drawing.Color.Empty);
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
				p1.X += 0.5;
				p2.X += 0.5;
				graphics.AddLine(p1, p2);
			}
			else
			{
				Drawing.Point p1 = new Drawing.Point(rect.Left, rect.Bottom+rect.Height/2);
				Drawing.Point p2 = new Drawing.Point(rect.Right, rect.Bottom+rect.Height/2);
				graphics.Align(ref p1);
				graphics.Align(ref p2);
				p1.Y += 0.5;
				p2.Y += 0.5;
				graphics.AddLine(p1, p2);
			}

			graphics.RenderSolid(this.colorControlDark);
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
			graphics.RenderSolid(this.colorControlDark);
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
			Drawing.Path pInside = this.PathRoundRectangle(rect, 0);
			graphics.Rasterizer.AddOutline(pInside);
			graphics.RenderSolid(this.colorControlDark);
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
			if ( color.IsEmpty || (state&WidgetState.Enabled) == 0 )
			{
				graphics.RenderSolid(this.colorControl);
			}
			else
			{
				color.A = 0.5;
				graphics.RenderSolid(color);
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
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
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
												 Drawing.Rectangle[] rects,
												 WidgetState state)
		{
			for ( int i=0 ; i<rects.Length ; i++ )
			{
				graphics.AddFilledRectangle(rects[i]);
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

		public void PaintTextSelectionForeground(Drawing.Graphics graphics,
												 Drawing.Rectangle[] rects,
												 WidgetState state)
		{
		}

		// Dessine le texte d'un widget.
		public void PaintGeneralTextLayout(Drawing.Graphics graphics,
										   Drawing.Point pos,
										   TextLayout text,
										   WidgetState state,
										   PaintTextStyle style,
										   Drawing.Color backColor)
		{
			if ( text == null )  return;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
				{
					text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorCaptionText, Drawing.GlyphPaintStyle.Selected);
				}
				else
				{
					text.Paint(pos, graphics);
				}
			}
			else
			{
				text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorControlDark, Drawing.GlyphPaintStyle.Disabled);
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


		// Crée le chemin d'un rectangle à coins arrondis.
		protected Drawing.Path PathRoundRectangleGroupBox(Drawing.Rectangle rect,
														  double startX, double endX)
		{
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			double radius = System.Math.Min(dx, dy);
			if ( radius < 12 )  radius = 0;
			else                radius = 3;
			radius = System.Math.Min(radius, startX);
			
			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+radius+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (endX, oy+dy-0.5);
			path.MoveTo (startX, oy+dy-0.5);
			path.LineTo (ox+radius+0.5, oy+dy-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
			path.LineTo (ox+0.5, oy+radius+0.5);
			path.CurveTo(ox+0.5, oy+0.5, ox+radius+0.5, oy+0.5);

			return path;
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
				radius = System.Math.Min(dx, dy);
				if ( radius < 12 )  radius = 0;
				else                radius = 3;
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

		// Crée le chemin d'un rectangle à coins arrondis en forme de "U" inversé.
		protected Drawing.Path PathTopRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius == 0 )
			{
				radius = System.Math.Min(3, System.Math.Min(dx, dy)/8);
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

		// Crée le chemin d'un rectangle à coins arrondis en forme de "D" inversé.
		protected Drawing.Path PathLeftRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius == 0 )
			{
				radius = System.Math.Min(3, System.Math.Min(dx, dy)/8);
			}
			
			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+dx-0.5, oy+0.5);
			path.LineTo (ox+radius+0.5, oy+0.5);
			path.CurveTo(ox+0.5, oy+0.5, ox+0.5, oy+radius+0.5);
			path.LineTo (ox+0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+radius+0.5, oy+dy-0.5);
			path.LineTo (ox+dx-0.5, oy+dy-0.5);

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
			Drawing.Path path = new Drawing.Path();
			path.MoveTo(c.X-rx, c.Y);
			path.CurveTo(c.X-rx, c.Y+ry*0.56, c.X-rx*0.56, c.Y+ry, c.X, c.Y+ry);
			path.CurveTo(c.X+rx*0.56, c.Y+ry, c.X+rx, c.Y+ry*0.56, c.X+rx, c.Y);
			path.CurveTo(c.X+rx, c.Y-ry*0.56, c.X+rx*0.56, c.Y-ry, c.X, c.Y-ry);
			path.CurveTo(c.X-rx*0.56, c.Y-ry, c.X-rx, c.Y-ry*0.56, c.X-rx, c.Y);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(color);
		}


		public void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.1, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromBrightness(intensity);
				color.G = System.Math.Min(color.G*1.1, 1.0);
				color.B = System.Math.Min(color.B*1.2, 1.0);  // bleuté
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
			get { return this.colorControlDark; }
		}

		public Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return enabled ? this.colorBlack : this.colorControlDark;
		}

		public Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return enabled ? this.colorBlack : this.colorControlDark;
		}

		public double AlphaVMenu { get { return 1.0; } }

		public Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(3,3,3,3); } }
		public Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public Drawing.Margins GeometryToolShapeBounds { get { return new Drawing.Margins(0,1,0,0); } }
		public Drawing.Margins GeometryButtonShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryTextFieldShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryListShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public double GeometryComboRightMargin { get { return 2; } }
		public double GeometryComboBottomMargin { get { return 2; } }
		public double GeometryComboTopMargin { get { return 2; } }
		public double GeometryUpDownRightMargin { get { return 2; } }
		public double GeometryUpDownBottomMargin { get { return 2; } }
		public double GeometryUpDownTopMargin { get { return 2; } }
		public double GeometryScrollerRightMargin { get { return 2; } }
		public double GeometryScrollerBottomMargin { get { return 2; } }
		public double GeometryScrollerTopMargin { get { return 2; } }
		public double GeometrySelectedLeftMargin { get { return -1; } }
		public double GeometrySelectedRightMargin { get { return 1; } }
		public double GeometrySliderLeftMargin { get { return 0; } }
		public double GeometrySliderRightMargin { get { return 0; } }
		public double GeometrySliderBottomMargin { get { return 0; } }


		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorControl;
		protected Drawing.Color		colorControlLight;
		protected Drawing.Color		colorControlLightLight;
		protected Drawing.Color		colorControlDark;
		protected Drawing.Color		colorControlDarkDark;
		protected Drawing.Color		colorControlReadOnly;
		protected Drawing.Color		colorScrollerBack;
		protected Drawing.Color		colorCaption;
		protected Drawing.Color		colorCaptionNF;  // NF = no focused
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorCaptionLight;
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorWindow;
	}
}
