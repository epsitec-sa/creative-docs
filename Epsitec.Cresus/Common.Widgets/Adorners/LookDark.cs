namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookDark impl�mente le d�corateur technique sombre.
	/// </summary>
	public class LookDark : IAdorner
	{
		public LookDark()
		{
			this.RefreshColors();
		}

		// Initialise les couleurs en fonction des r�glages de Windows.
		public void RefreshColors()
		{
			double r,g,b;

#if false
			this.colorBlack             = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorWhite             = Drawing.Color.FromRGB(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorWindow            = Drawing.Color.FromRGB( 80.0/255.0,  80.0/255.0, 100.0/255.0);
			this.colorControl           = Drawing.Color.FromRGB( 80.0/255.0,  80.0/255.0, 100.0/255.0);
			this.colorControlLight      = Drawing.Color.FromRGB(100.0/255.0, 100.0/255.0, 110.0/255.0);
			this.colorControlLightLight = Drawing.Color.FromRGB(128.0/255.0, 128.0/255.0, 138.0/255.0);
			this.colorControlDark       = Drawing.Color.FromRGB( 70.0/255.0,  70.0/255.0,  90.0/255.0);
			this.colorControlDarkDark   = Drawing.Color.FromRGB( 60.0/255.0,  60.0/255.0,  80.0/255.0);
			this.colorButton            = Drawing.Color.FromRGB( 50.0/255.0,  50.0/255.0,  70.0/255.0);
			this.colorScrollerBack      = Drawing.Color.FromRGB(128.0/255.0, 128.0/255.0, 138.0/255.0);
			this.colorCaption           = Drawing.Color.FromRGB(255.0/255.0, 215.0/255.0,  89.0/255.0);
			this.colorCaptionText       = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
#else
			this.colorBlack             = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorWhite             = Drawing.Color.FromRGB(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorWindow            = Drawing.Color.FromRGB( 80.0/255.0,  80.0/255.0,  90.0/255.0);
			this.colorControl           = Drawing.Color.FromRGB( 80.0/255.0,  80.0/255.0,  90.0/255.0);
			this.colorControlLight      = Drawing.Color.FromRGB(100.0/255.0, 100.0/255.0, 110.0/255.0);
			this.colorControlLightLight = Drawing.Color.FromRGB(128.0/255.0, 128.0/255.0, 138.0/255.0);
			this.colorControlDark       = Drawing.Color.FromRGB( 70.0/255.0,  70.0/255.0,  80.0/255.0);
			this.colorControlDarkDark   = Drawing.Color.FromRGB( 60.0/255.0,  60.0/255.0,  70.0/255.0);
			this.colorButton            = Drawing.Color.FromRGB( 50.0/255.0,  50.0/255.0,  60.0/255.0);
			this.colorScrollerBack      = Drawing.Color.FromRGB(128.0/255.0, 128.0/255.0, 138.0/255.0);
			this.colorCaption           = Drawing.Color.FromRGB(255.0/255.0, 215.0/255.0,  89.0/255.0);
			this.colorCaptionText       = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
#endif
			this.colorInfo              = Drawing.Color.FromName("Info");

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorControlReadOnly = Drawing.Color.FromRGB(r,g,b);

			r = 1-(1-this.colorCaption.R)*0.25;
			g = 1-(1-this.colorCaption.G)*0.25;
			b = 1-(1-this.colorCaption.B)*0.25;
			this.colorCaptionLight = Drawing.Color.FromRGB(r,g,b);

			this.colorHilite = this.colorCaption;
		}
		

		// Dessine le fond d'une fen�tre.
		public void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction shadow)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorWindow);
		}

		// Dessine une fl�che (dans un bouton d'ascenseur par exemple).
		public void PaintArrow(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state,
							   Widgets.Direction shadow,
							   Widgets.Direction dir)
		{
			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			Drawing.Path path = new Drawing.Path();
			switch ( dir )
			{
				case Direction.Up:
					path.MoveTo(center.X+0.0*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X-0.3*rect.Width, center.Y-0.1*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.3*rect.Width, center.Y-0.1*rect.Height);
					break;

				case Direction.Down:
					path.MoveTo(center.X+0.0*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X-0.3*rect.Width, center.Y+0.1*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y-0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.3*rect.Width, center.Y+0.1*rect.Height);
					break;

				case Direction.Right:
					path.MoveTo(center.X+0.2*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X-0.1*rect.Width, center.Y-0.3*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X+0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X-0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X-0.1*rect.Width, center.Y+0.3*rect.Height);
					break;

				case Direction.Left:
					path.MoveTo(center.X-0.2*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.1*rect.Width, center.Y-0.3*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y-0.2*rect.Height);
					path.LineTo(center.X-0.0*rect.Width, center.Y+0.0*rect.Height);
					path.LineTo(center.X+0.2*rect.Width, center.Y+0.2*rect.Height);
					path.LineTo(center.X+0.1*rect.Width, center.Y+0.3*rect.Height);
					break;
			}
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorWhite);
			}
			else
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
		}

		// Dessine un bouton � cocher sans texte.
		public void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state,
							   Widgets.Direction shadow)
		{
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Enabled) == 0 )  // bouton disabled ?
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
			{
				graphics.RenderSolid(this.colorControl);
			}
			else
			{
				graphics.RenderSolid(this.colorWhite);
			}

			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
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
			graphics.RenderSolid(this.colorControlDarkDark);

			if ( (state&WidgetState.ActiveYes) != 0 )  // coch� ?
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
							   Widgets.WidgetState state,
							   Widgets.Direction shadow)
		{
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			this.PaintCircle(graphics, rect, this.colorControlDarkDark);

			rInside = rect;
			rInside.Inflate(-1, -1);

			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Inflate(-1, -1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Inflate(-1, -1);
			}

			if ( (state&WidgetState.Enabled) == 0 )  // bouton disabled ?
			{
				this.PaintCircle(graphics, rInside, this.colorControlLightLight);
			}
			else if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorWhite);
			}

			if ( (state&WidgetState.ActiveYes) != 0 )  // coch� ?
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
							  Widgets.Direction shadow,
							  string icon)
		{
		}

		// Dessine le fond d'un bouton rectangulaire.
		public void PaintButtonBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetState state,
										  Widgets.Direction shadow,
										  Widgets.ButtonStyle style)
		{
			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultActive )
			{
				Drawing.Path path = PathRoundRectangle(rect, 0);
			
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
					{
						graphics.SolidRenderer.Color = this.colorControl;
					}
					else
					{
						graphics.SolidRenderer.Color = this.colorButton;
					}
				}
				else
				{
					graphics.SolidRenderer.Color = this.colorControl;
				}
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid();
			
				if ( (state&WidgetState.Focused) != 0 )
				{
					Drawing.Rectangle rInside = rect;
					rInside.Inflate(-1.5, -1.5);
					Drawing.Path pInside = PathRoundRectangle(rInside, 0);
					graphics.SolidRenderer.Color = this.colorControlDark;
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid();
				}

				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					Drawing.Rectangle rInside = rect;
					rInside.Inflate(-1.5, -1.5);
					Drawing.Path pInside = PathRoundRectangle(rInside, 0);
					graphics.SolidRenderer.Color = this.colorHilite;
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid();
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.SolidRenderer.Color = this.colorBlack;
				}
				else
				{
					graphics.SolidRenderer.Color = this.colorControlDarkDark;
				}
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid();
			}
			else if ( style == ButtonStyle.Scroller )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorButton);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLight);
				}

				Drawing.Rectangle rInside;

				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					rInside = rect;
					rInside.Inflate(-1.5, -1.5);
					Drawing.Path pInside = PathRoundRectangle(rInside, 0);
					graphics.SolidRenderer.Color = this.colorHilite;
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid();
				}

				rInside = rect;
				rInside.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rInside);
				graphics.RenderSolid(this.colorBlack);
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				rect.Right += 1;

				if ( (state&WidgetState.Entered)   != 0 ||  // bouton survol� ?
					 (state&WidgetState.Engaged)   != 0 ||  // bouton press� ?
					 (state&WidgetState.ActiveYes) != 0 )   // bouton activ� ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);

					Drawing.Rectangle rInside;
					rInside = rect;
					rInside.Inflate(-0.5, -0.5);
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.colorBlack);
				}

				state &= ~WidgetState.Focused;
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
		}

		// Dessine le texte d'un bouton.
		public void PaintButtonTextLayout(Drawing.Graphics graphics,
										  Drawing.Point pos,
										  TextLayout text,
										  WidgetState state,
										  Direction shadow,
										  ButtonStyle style)
		{
			if ( text == null )  return;

			if ( (state&WidgetState.Engaged) != 0 &&  // bouton press� ?
				 style == ButtonStyle.ToolItem )
			{
				pos.X ++;
				pos.Y --;
			}
			state &= ~WidgetState.Focused;
			this.PaintGeneralTextLayout(graphics, pos, text, state, shadow);
		}

		public void PaintButtonForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetState state,
										  Widgets.Direction shadow,
										  Widgets.ButtonStyle style)
		{
		}

		// Dessine le fond d'une ligne �ditable.
		public void PaintTextFieldBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetState state,
											 Widgets.Direction shadow,
											 Widgets.TextFieldStyle style,
											 bool readOnly)
		{
			if ( style == TextFieldStyle.Normal || style == TextFieldStyle.UpDown )
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
						graphics.RenderSolid(this.colorControlDark);
					}
				}
				else
				{
					graphics.RenderSolid(this.colorControlLight);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Inflate(-0.5, -0.5);

				graphics.AddRectangle(rInside);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDarkDark);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControlDark);

				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBlack);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		public void PaintTextFieldForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetState state,
											 Widgets.Direction shadow,
											 Widgets.TextFieldStyle style,
											 bool readOnly)
		{
		}

		// Dessine le fond d'un ascenseur.
		public void PaintScrollerBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetState state,
											Widgets.Direction shadow)
		{
			graphics.AddFilledRectangle(frameRect);
			graphics.RenderSolid(this.colorScrollerBack);

			Drawing.Rectangle rInside = new Drawing.Rectangle();
			rInside = frameRect;
			rInside.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorBlack);

			if ( !tabRect.IsSurfaceZero )
			{
				graphics.AddFilledRectangle(tabRect);
				graphics.RenderSolid(this.colorWhite);
			}
		}

		// Dessine la cabine d'un ascenseur.
		public void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle frameRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetState state,
										Widgets.Direction shadow)
		{
			Drawing.Rectangle	rect;
			Drawing.Point		center;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.AddFilledRectangle(frameRect);
				graphics.RenderSolid(this.colorControlDark);

				rect = frameRect;
				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBlack);

				switch ( shadow )
				{
					case Direction.Up:
					case Direction.Down:
						rect = frameRect;
						if ( rect.Width >= 10 && rect.Height >= 20 )
						{
							center = new Drawing.Point((rect.Left+rect.Right)/2+1, (rect.Bottom+rect.Top)/2);
							center.Y = System.Math.Floor(center.Y)+0.5;
							double y = center.Y-6;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(center.X-rect.Width*0.25, y, center.X+rect.Width*0.25, y);
								y += 3;
							}
							graphics.RenderSolid(this.colorBlack);

							y = center.Y-6+1;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(center.X-rect.Width*0.25-1, y, center.X+rect.Width*0.25-1, y);
								y += 3;
							}
							graphics.RenderSolid(this.colorControlLightLight);
						}
						break;

					case Direction.Left:
					case Direction.Right:
						rect = frameRect;
						if ( rect.Height >= 10 && rect.Width >= 20 )
						{
							center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2-1);
							center.X = System.Math.Floor(center.X)-0.5;
							double x = center.X-6+1;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(x, center.Y-rect.Height*0.25, x, center.Y+rect.Height*0.25);
								x += 3;
							}
							graphics.RenderSolid(this.colorBlack);

							x = center.X-6;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(x, center.Y-rect.Height*0.25+1, x, center.Y+rect.Height*0.25+1);
								x += 3;
							}
							graphics.RenderSolid(this.colorControlLightLight);
						}
						break;
				}
			}
		}

		public void PaintScrollerForeground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetState state,
											Widgets.Direction shadow)
		{
		}

		// Dessine le cadre d'un GroupBox.
		public void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetState state,
								  Widgets.Direction shadow)
		{
			Drawing.Rectangle rect = new Drawing.Rectangle();

			rect = frameRect;
			rect.Inflate(-0.5, -0.5);
			graphics.LineWidth = 1;
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBlack);

			graphics.AddFilledRectangle(titleRect);
			graphics.RenderSolid(this.colorControl);
		}

		public void PaintSepLine(Drawing.Graphics graphics,
								 Drawing.Rectangle frameRect,
								 Drawing.Rectangle titleRect,
								 Widgets.WidgetState state,
								 Widgets.Direction shadow)
		{
		}

		public void PaintFrameTitleBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
											  Widgets.WidgetState state,
											  Widgets.Direction shadow)
		{
		}

		public void PaintFrameTitleForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
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

		// Dessine toute la bande sous les onglets.
		public void PaintTabBand(Drawing.Graphics graphics,
								 Drawing.Rectangle rect,
								 Widgets.WidgetState state,
								 Widgets.Direction shadow)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);
		}

		// Dessine la zone principale sous les onglets.
		public void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetState state,
								  Widgets.Direction shadow)
		{
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDarkDark);

			rect.Inflate(-0.5, -0.5);
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlLight);
		}

		// Dessine l'onglet devant les autres.
		public void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction shadow)
		{
			titleRect.Bottom += 1;
			Drawing.Path pTitle = PathTopCornerRectangle(titleRect);

			graphics.SolidRenderer.Color = this.colorControlLight;
			graphics.Rasterizer.AddSurface(pTitle);
			graphics.RenderSolid();

			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				Drawing.Rectangle rHilite = new Drawing.Rectangle();
				rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = PathTopCornerRectangle(rHilite);
				graphics.SolidRenderer.Color = this.colorHilite;
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid();
			}

			graphics.SolidRenderer.Color = this.colorControlDarkDark;
			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid();
		}

		public void PaintTabAboveForeground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction shadow)
		{
		}

		// Dessine un onglet derri�re (non s�lectionn�).
		public void PaintTabSunkenBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetState state,
											 Widgets.Direction shadow)
		{
			titleRect.Left  += 1;
			titleRect.Right -= 1;
			Drawing.Path pTitle = PathTopCornerRectangle(titleRect);

			graphics.SolidRenderer.Color = this.colorButton;
			graphics.Rasterizer.AddSurface(pTitle);
			graphics.RenderSolid();

			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				Drawing.Rectangle rHilite = new Drawing.Rectangle();
				rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = PathTopCornerRectangle(rHilite);
				graphics.SolidRenderer.Color = this.colorHilite;
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid();
			}

			graphics.SolidRenderer.Color = this.colorBlack;
			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid();
		}

		public void PaintTabSunkenForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetState state,
											 Widgets.Direction shadow)
		{
		}

		// Dessine le fond d'un tableau.
		public void PaintArrayBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow)
		{
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
			{
				graphics.RenderSolid(this.colorControlLight);
			}
			else
			{
				graphics.RenderSolid(this.colorWindow);
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			Drawing.Rectangle rInside = rect;
			rInside.Inflate(-0.5, -0.5);

			graphics.AddRectangle(rInside);
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
			{
				graphics.RenderSolid(this.colorBlack);
			}
			else
			{
				graphics.RenderSolid(this.colorControlDarkDark);
			}
		}

		// Dessine le fond d'une cellule.
		public void PaintCellBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow)
		{
			if ( (state&WidgetState.Selected) != 0 )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorCaption);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWindow);
			}
		}

		// Dessine le fond d'un bouton d'en-t�te de tableau.
		public void PaintHeaderBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction shadow,
										  Direction type)
		{
			if ( type == Direction.Up )
			{
				rect.Left  += 1;
				rect.Right -= 0;
				rect.Top   -= 1;
			}
			if ( type == Direction.Left )
			{
				rect.Bottom += 0;
				rect.Top    -= 1;
				rect.Left   += 1;
			}

			Drawing.Path path;
			if ( type == Direction.Up )
			{
				path = this.PathTopCornerRectangle(rect);
			}
			else
			{
				path = this.PathLeftCornerRectangle(rect);
			}
			graphics.SolidRenderer.Color = this.colorButton;
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid();

			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				if ( type == Direction.Up )
				{
					rect.Top = rect.Bottom+2;
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorHilite);
				}
				if ( type == Direction.Left )
				{
					rect.Left = rect.Right-2;
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorHilite);
				}
			}

			graphics.SolidRenderer.Color = this.colorBlack;
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid();
		}

		public void PaintHeaderForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction shadow,
										  Direction type)
		{
		}

		// Dessine le fond d'une barre d'outil.
		public void PaintToolBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction shadow,
										Direction type)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlLightLight);
		}

		public void PaintToolForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction shadow,
										Direction type)
		{
		}

		// Dessine le fond d'un menu.
		public void PaintMenuBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction shadow,
										Drawing.Rectangle parentRect,
										double iconWidth)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlDark);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Width = iconWidth;
				band.Top -= 1;
				band.Bottom += 1;
				graphics.AddFilledRectangle(band);
				graphics.RenderSolid(this.colorControlLightLight);
			}

			rect.Inflate(-0.5, -0.5);
			if ( parentRect.IsSurfaceZero )
			{
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBlack);
			}
			else
			{
				graphics.AddLine(rect.Left, rect.Top+0.5, rect.Left, rect.Bottom-0.5);
				graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
				graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top+0.5);
				graphics.AddLine(parentRect.Right-0.5, rect.Top, rect.Right+0.5, rect.Top);
				graphics.RenderSolid(this.colorBlack);
			}
		}

		public void PaintMenuForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction shadow,
										Drawing.Rectangle parentRect,
										double iconWidth)
		{
		}

		// Dessine le fond d'une case de menu.
		public void PaintMenuItemBackground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetState state,
											Direction shadow,
											MenuType type,
											MenuItemType itemType)
		{
			if ( type == MenuType.Horizontal )
			{
				if ( itemType == MenuItemType.Select )
				{
					graphics.AddFilledRectangle(rect);
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
					graphics.RenderSolid(this.colorControlDark);
				}
			}

			if ( type == MenuType.Vertical )
			{
				if ( itemType != MenuItemType.Deselect )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);
				}
			}
		}

		// Dessine le texte d'un menu.
		public void PaintMenuItemTextLayout(Drawing.Graphics graphics,
											Drawing.Point pos,
											TextLayout text,
											WidgetState state,
											Direction shadow,
											MenuType type,
											MenuItemType itemType)
		{
			if ( text == null )  return;
			state &= ~WidgetState.Focused;
			if ( itemType == MenuItemType.Deselect )
			{
				state &= ~WidgetState.Selected;
			}
			else
			{
				state |= WidgetState.Selected;
			}
			this.PaintGeneralTextLayout(graphics, pos, text, state, shadow);
		}

		// Dessine le devant d'une case de menu.
		public void PaintMenuItemForeground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetState state,
											Direction shadow,
											MenuType type,
											MenuItemType itemType)
		{
		}

		// Dessine un s�parateur horizontal ou vertical.
		public void PaintSeparatorBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetState state,
											 Direction shadow,
											 Direction type,
											 bool optional)
		{
			if ( type == Direction.Right )
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
											 Direction shadow,
											 Direction type,
											 bool optional)
		{
		}

		// Dessine une case de statuts.
		public void PaintStatusBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction shadow)
		{
			rect.Width -= 1;
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBlack);
		}

		public void PaintStatusForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction shadow)
		{
		}

		// Dessine le fond d'une bulle d'aide.
		public void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Direction shadow)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorInfo);  // fond jaune pale
			
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBlack);  // cadre noir
		}

		// Dessine le texte d'une bulle d'aide.
		public void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Direction shadow)
		{
			text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorBlack);
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
				graphics.RenderSolid(this.colorWhite);
			}
		}
		
		// Dessine les zones rectanglaires correspondant aux caract�res s�lectionn�s.
		public void PaintTextSelectionBackground(Drawing.Graphics graphics,
												 Drawing.Rectangle[] rects)
		{
			for (int i = 0; i < rects.Length; i++)
			{
				graphics.AddFilledRectangle(rects[i]);
				graphics.RenderSolid(this.colorCaption);
			}
		}

		public void PaintTextSelectionForeground(Drawing.Graphics graphics,
												 Drawing.Rectangle[] rects)
		{
		}

		// Dessine le texte d'un widget.
		public void PaintGeneralTextLayout(Drawing.Graphics graphics,
										   Drawing.Point pos,
										   TextLayout text,
										   WidgetState state,
										   Direction shadow)
		{
			if ( text == null )  return;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
				{
					text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorCaptionText);
				}
				else
				{
					text.Paint(pos, graphics);
				}
			}
			else
			{
				text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorControlLightLight);
			}

			if ( (state&WidgetState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = text.StandardRectangle;
				rFocus.Offset(pos);
				graphics.Align(ref rFocus);
				rFocus.Inflate(2.5, -0.5);
				PaintFocusBox(graphics, rFocus);
			}
		}


		// Cr�e le chemin d'un rectangle � coins arrondis.
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

		// Cr�e le chemin d'un rectangle "corn�" en forme de "U" invers�.
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

		// Cr�e le chemin d'un rectangle "corn�" en forme de "D" invers�.
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


		public void AdaptEnabledTextColor(ref Drawing.Color color)
		{
			color.R = 1.0-color.R;
			color.G = 1.0-color.G;
			color.B = 1.0-color.B;
		}

		public void AdaptDisabledTextColor(ref Drawing.Color color, Drawing.Color uniqueColor)
		{
			double alpha = color.A;
			double intensity = color.GetBrightness ();
			intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
			//intensity = System.Math.Min(intensity+0.0, 1.0);  // augmente l'intensit�
			color = Drawing.Color.FromBrightness(intensity);
			color.A = alpha;
		}

		public Drawing.Color GetColorCaption()
		{
			return this.colorCaption;
		}

		public Drawing.Color GetColorControl()
		{
			return this.colorControl;
		}

		public Drawing.Color GetColorWindow()
		{
			return this.colorWindow;
		}

		public Drawing.Color GetColorBorder()
		{
			return Drawing.Color.FromBrightness(0.0);
		}


		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorWhite;
		protected Drawing.Color		colorControl;
		protected Drawing.Color		colorControlLight;
		protected Drawing.Color		colorControlLightLight;
		protected Drawing.Color		colorControlDark;
		protected Drawing.Color		colorControlDarkDark;
		protected Drawing.Color		colorControlReadOnly;
		protected Drawing.Color		colorScrollerBack;
		protected Drawing.Color		colorCaption;
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorCaptionLight;
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorWindow;
	}
}
