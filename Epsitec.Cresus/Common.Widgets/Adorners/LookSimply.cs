namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookSimply implémente un décorateur minimaliste.
	/// </summary>
	public class LookSimply : IAdorner
	{
		public LookSimply()
		{
			this.RefreshColors();
		}

		// Initialise les couleurs en fonction des réglages de Windows.
		public void RefreshColors()
		{
			double r,g,b;

			this.colorBlack             = Drawing.Color.FromName("WindowFrame");
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

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorButton = Drawing.Color.FromRGB(r,g,b);

			this.colorHilite = Drawing.Color.FromRGB(250.0/255.0, 196.0/255.0,  89.0/255.0);
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
							   GlyphType type,
							   PaintTextStyle style)
		{
			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			Drawing.Path path = new Drawing.Path();
			switch ( type )
			{
				case GlyphType.ArrowUp:
					path.MoveTo(center.X+0.0*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X-0.3*rect.Width, center.Y-0.1*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.3*rect.Width, center.Y-0.1*rect.Height);
					break;

				case GlyphType.ArrowDown:
					path.MoveTo(center.X+0.0*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X-0.3*rect.Width, center.Y+0.1*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y-0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.3*rect.Width, center.Y+0.1*rect.Height);
					break;

				case GlyphType.ArrowRight:
					path.MoveTo(center.X+0.2*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X-0.1*rect.Width, center.Y-0.3*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X-0.1*rect.Width, center.Y+0.3*rect.Height);
					break;

				case GlyphType.ArrowLeft:
					path.MoveTo(center.X-0.2*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.1*rect.Width, center.Y-0.3*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X-0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.1*rect.Width, center.Y+0.3*rect.Height);
					break;

				case GlyphType.Menu:
					path.MoveTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.25);
					path.LineTo(center.X-rect.Width*0.30, center.Y+rect.Height*0.15);
					path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.15);
					break;

				case GlyphType.Close:
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
			}
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
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(this.colorControl);
			}

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				rInside = rect;
				rInside.Inflate(-1.5, -1.5);
				graphics.LineWidth = 2;
				graphics.AddRectangle(rInside);
				graphics.RenderSolid(this.colorHilite);
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			rInside = rect;
			rInside.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rInside);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
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

			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintCircle(graphics, rect, this.colorControlDarkDark);
			}
			else
			{
				this.PaintCircle(graphics, rect, this.colorControlDark);
			}

			rInside = rect;
			rInside.Inflate(-1, -1);

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Inflate(-1, -1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Inflate(-1, -1);
			}

			if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintCircle(graphics, rInside, this.colorControlLightLight);
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}

			if ( (state&WidgetState.ActiveYes) != 0 )  // coché ?
			{
				rInside = rect;
				rInside.Inflate(-rect.Height*0.3, -rect.Height*0.3);
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
			rFocus.Deflate(2, 2);
			double radFocus = 0;

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultActive )
			{
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
						if ( style == ButtonStyle.DefaultActive )
						{
							graphics.RenderSolid(this.colorControlLightLight);
						}
						else
						{
							graphics.RenderSolid(this.colorButton);
						}
					}
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rInside = rect;
					rInside.Inflate(-1.5, -1.5);
					Drawing.Path pInside = this.PathRoundRectangle(rInside, 0);
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid(this.colorHilite);
				}

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
			else if ( style == ButtonStyle.Scroller ||
					  style == ButtonStyle.Combo    ||
					  style == ButtonStyle.UpDown   ||
					  style == ButtonStyle.Icon     )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControl);

				Drawing.Rectangle rInside;

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					rInside = rect;
					rInside.Inflate(-1.5, -1.5);
					Drawing.Path pInside = this.PathRoundRectangle(rInside, -1);
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid(this.colorHilite);
				}

				rInside = rect;
				rInside.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rInside);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorControlDarkDark);
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
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionLight);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Inflate(-0.5, -0.5);
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.colorCaption);
				}
				rFocus.Inflate(2, 2);
				rFocus.Right ++;
				radFocus = -1;
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
				Drawing.Path pInside = this.PathRoundRectangle(rFocus, radFocus);
				graphics.Rasterizer.AddOutline(pInside, 1);
				graphics.RenderSolid(this.colorControlDark);
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
				 style == TextFieldStyle.Combo  ||
				 style == TextFieldStyle.UpDown )
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
				rInside.Inflate(-0.5, -0.5);

				graphics.AddRectangle(rInside);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorControlDarkDark);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorControlLightLight);
				}

				Drawing.Rectangle rInside = rect;
				rInside.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rInside);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
			graphics.AddFilledRectangle(frameRect);
			graphics.RenderSolid(this.colorScrollerBack);

			Drawing.Rectangle rInside = new Drawing.Rectangle();
			rInside = frameRect;
			rInside.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rInside);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
			}

			if ( !tabRect.IsSurfaceZero )
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
			Drawing.Rectangle rect = new Drawing.Rectangle();

			rect = frameRect;
			rect.Inflate(-0.5, -0.5);
			graphics.LineWidth = 1;
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDark);

			graphics.AddFilledRectangle(titleRect);
			graphics.RenderSolid(this.colorControl);
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
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
			}

			rect.Inflate(-0.5, -0.5);
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

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

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
				Drawing.Rectangle rHilite = new Drawing.Rectangle();
				rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}

			graphics.Rasterizer.AddOutline(pTitle, 1);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
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

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			graphics.Rasterizer.AddSurface(pTitle);
			graphics.RenderSolid(this.colorControl);

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				Drawing.Rectangle rHilite = new Drawing.Rectangle();
				rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}

			graphics.Rasterizer.AddOutline(pTitle, 1);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
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
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
			rInside.Inflate(-0.5, -0.5);

			graphics.AddRectangle(rInside);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
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
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDark);
			}
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
			graphics.AddFilledRectangle(rect);
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
			if ( dir == Direction.Up )
			{
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
			}

			if ( dir == Direction.Left )
			{
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
			}
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
				graphics.RenderSolid(this.colorControl);
			}

			rect.Inflate(-0.5, -0.5);
			if ( parentRect.IsSurfaceZero )
			{
				graphics.AddRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )
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
				graphics.AddLine(rect.Left, rect.Top+0.5, rect.Left, rect.Bottom-0.5);
				graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
				graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top+0.5);
				graphics.AddLine(parentRect.Right-0.5, rect.Top, rect.Right+0.5, rect.Top);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorControlDarkDark);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
				}
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
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionLight);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Inflate(-0.5, -0.5);
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.colorCaption);
				}
				if ( itemType == MenuItemType.Parent )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorControl);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Inflate(-0.5, -0.5);
					graphics.AddLine(rInside.Left, rInside.Bottom-0.5, rInside.Left, rInside.Top);
					graphics.AddLine(rInside.Left, rInside.Top, rInside.Right, rInside.Top);
					graphics.AddLine(rInside.Right, rInside.Top, rInside.Right, rInside.Bottom-0.5);
					graphics.RenderSolid(this.colorControlDarkDark);
				}
			}

			if ( type == MenuType.Vertical )
			{
				if ( itemType != MenuItemType.Deselect )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionLight);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Inflate(-0.5, -0.5);
					graphics.AddRectangle(rInside);
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

			graphics.RenderSolid(Drawing.Color.FromBrightness(0.75));
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
			graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
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
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
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
									   Drawing.Color color)
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
				color.A = 0.3;
				graphics.RenderSolid(color);
			}

			Drawing.Rectangle rInside;

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				rInside = rect;
				rInside.Inflate(-1.5, -1.5);
				path = new Drawing.Path();
				path.AppendCircle(rInside.Center, rInside.Width/2, rInside.Height/2);
				graphics.Rasterizer.AddOutline(path, 2);
				graphics.RenderSolid(this.colorHilite);
			}

			rInside = rect;
			rInside.Inflate(-0.5, -0.5);
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

			this.PaintGlyph(graphics, rect, state, GlyphType.Menu, PaintTextStyle.Button);
		}

		public void PaintTagForeground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetState state,
									   Drawing.Color color)
		{
		}

		// Dessine le fond d'une bulle d'aide.
		public void PaintTooltipBackground(Drawing.Graphics graphics,
										   Drawing.Rectangle rect)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorInfo);  // fond jaune pale
			
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDarkDark);  // cadre noir
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
									Drawing.Rectangle rect,
									bool cursorOn)
		{
			if ( cursorOn )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorBlack);
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
		protected Drawing.Path PathRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius ==  0 )  radius = System.Math.Min(dx, dy)/8;
			if ( radius == -1 )  radius = 0;
			
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

		// Crée le chemin d'un rectangle à coins arrondis en forme de "u" inversé.
		protected Drawing.Path PathTopRoundRectangle(Drawing.Rectangle rect, double radius)
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
			path.MoveTo (ox+0.5, oy);
			path.LineTo (ox+0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+radius+0.5, oy+dy-0.5);
			path.LineTo (ox+dx-radius-0.5, oy+dy-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-0.5, oy+dy-radius-0.5);
			path.LineTo (ox+dx-0.5, oy);

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
				intensity = System.Math.Min(intensity+0.3, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromBrightness(intensity);
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
			get { return Drawing.Color.FromBrightness(0.5); }
		}

		public Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return enabled ? this.colorControlDarkDark : this.colorControlDark;
		}

		public Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return enabled ? this.colorControlDarkDark : this.colorControlDark;
		}

		public double AlphaVMenu { get { return 1.0; } }

		public Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public Drawing.Margins GeometryToolShapeBounds { get { return new Drawing.Margins(0,1,0,0); } }
		public Drawing.Margins GeometryButtonShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryTextFieldShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryListShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public double GeometryComboRightMargin { get { return 0; } }
		public double GeometryComboBottomMargin { get { return 0; } }
		public double GeometryComboTopMargin { get { return 0; } }
		public double GeometryUpDownRightMargin { get { return 0; } }
		public double GeometryUpDownBottomMargin { get { return 0; } }
		public double GeometryUpDownTopMargin { get { return 0; } }
		public double GeometryScrollerRightMargin { get { return 0; } }
		public double GeometryScrollerBottomMargin { get { return 0; } }
		public double GeometryScrollerTopMargin { get { return 0; } }
		public double GeometrySelectedLeftMargin { get { return -2; } }
		public double GeometrySelectedRightMargin { get { return 2; } }
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
