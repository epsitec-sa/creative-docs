namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookMetal implémente le décorateur métalique rigolo.
	/// </summary>
	public class LookMetal : IAdorner
	{
		public LookMetal()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources.LookMetal.png", this.GetType().Assembly);
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

			r = 1-(1-this.colorCaption.R)*0.25;
			g = 1-(1-this.colorCaption.G)*0.25;
			b = 1-(1-this.colorCaption.B)*0.25;
			this.colorCaptionLight = Drawing.Color.FromRGB(r,g,b);

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorButton = Drawing.Color.FromRGB(r,g,b);

			this.colorCaption = Drawing.Color.FromRGB(170.0/255.0, 195.0/255.0, 249.0/255.0);
			this.colorHilite  = Drawing.Color.FromRGB(250.0/255.0, 196.0/255.0,  89.0/255.0);
			this.colorBorder  = Drawing.Color.FromRGB(102.0/255.0, 102.0/255.0, 126.0/255.0);
			this.colorWindow  = Drawing.Color.FromRGB(212.0/255.0, 212.0/255.0, 218.0/255.0);
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

		// Dessine une flèche (dans un bouton d'ascenseur par exemple).
		public void PaintArrow(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state,
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
				graphics.RenderSolid(this.colorBlack);
			}
			else
			{
				graphics.RenderSolid(this.colorControlLightLight);
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
			graphics.RenderSolid(this.colorBorder);

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

			this.PaintCircle(graphics, rect, this.colorBorder);

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
			else
			{
				this.PaintCircle(graphics, rInside, this.colorControlLightLight);
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
			Drawing.Rectangle rInside;
			rInside = rect;
			rInside.Inflate(-1, -1);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultActive )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
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
						if ( style == ButtonStyle.DefaultActive )
						{
							this.PaintImageButton(graphics, rInside, 1);
						}
						else
						{
							this.PaintImageButton(graphics, rInside, 0);
						}
					}

					double radius = this.RetRadius(rect);
					Drawing.Path pTitle = PathRoundRectangle(rect, radius);
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 3);
				}
			}
			else if ( style == ButtonStyle.Scroller ||
					  style == ButtonStyle.Combo    ||
					  style == ButtonStyle.UpDown   ||
					  style == ButtonStyle.Icon     )
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
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 11);
				}

				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBorder);
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rInside, 8);

					rect.Inflate(-0.5, -0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				if ( (state&WidgetState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 12);

					rect.Inflate(-0.5, -0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rInside, 10);

					rect.Inflate(-0.5, -0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
			}
			else if ( style == ButtonStyle.ListItem )
			{
				this.PaintImageButton(graphics, rect, 8);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 8);
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
			state &= ~WidgetState.Focused;
			this.PaintGeneralTextLayout(graphics, pos, text, state);
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
					this.PaintImageButton(graphics, rect, readOnly?20:16);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 18);
				}
				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBorder);
			}
			else if ( style == TextFieldStyle.Simple )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}

				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBorder);
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
				this.PaintImageButton(graphics, frameRect, 18);
			}

			if ( !tabRect.IsSurfaceZero )
			{
				this.PaintImageButton(graphics, tabRect, 19);
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
			graphics.RenderSolid(this.colorBorder);

			graphics.AddFilledRectangle(titleRect);
			graphics.RenderSolid(this.colorWindow);
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

			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBorder);
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

			Drawing.Path pTitle = PathTopCornerRectangle(titleRect);
			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.colorBorder);
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

			Drawing.Path pTitle = PathTopCornerRectangle(titleRect);
			graphics.Rasterizer.AddOutline(pTitle, 1);
			graphics.RenderSolid(this.colorBorder);
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
				this.PaintImageButton(graphics, rect, 16);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 18);
			}
			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBorder);
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
				graphics.RenderSolid(this.colorCaption);
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
				Drawing.Path pTitle = PathTopRectangle(rect);
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			if ( dir == Direction.Left )
			{
				Drawing.Path pTitle = PathLeftRectangle(rect);
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorBorder);
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

			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBorder);
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
			if ( itemType != MenuItemType.Deselect )
			{
				this.PaintImageButton(graphics, rect, 8);

				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBorder);
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
			this.PaintGeneralTextLayout(graphics, pos, text, state);
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
			graphics.RenderSolid(this.colorBorder);
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

		// Dessine le fond d'une bulle d'aide.
		public void PaintTooltipBackground(Drawing.Graphics graphics,
										   Drawing.Rectangle rect)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorInfo);  // fond jaune pale
			
			rect.Inflate(-0.5, -0.5);
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
										   WidgetState state)
		{
			if ( text == null )  return;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
				{
					text.Paint(pos, graphics, Drawing.Rectangle.Infinite, Drawing.Color.Empty, Drawing.GlyphPaintStyle.Selected);
				}
				else
				{
					text.Paint(pos, graphics);
				}
			}
			else
			{
				text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorControlLightLight, Drawing.GlyphPaintStyle.Disabled);
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

		// Retourne le rayon à utiliser pour une zone rectangulaire.
		protected double RetRadius(Drawing.Rectangle rect)
		{
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Min(3, dim/4));
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

			if ( rank == 16 || rank == 17 || rank == 18 || rank == 21 || rank == 28 || rank == 44 )
			{
				this.PaintImageButton1(graphics, rect, icon);
			}
			else if ( rank == 24 || rank == 25 || rank == 26 || rank == 27 || rank == 32 || rank == 33 || rank == 34 || rank == 35 )
			{
				this.PaintImageButton9(graphics, rect, 5, icon, 5);
			}
			else
			{
				this.PaintImageButton9(graphics, rect, this.RetRadius(rect), icon, 3);
			}
		}

		// Dessine un bouton composé d'un seul morceau d'image.
		protected void PaintImageButton1(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 Drawing.Rectangle icon)
		{
			icon.Inflate(-0.5, -0.5);
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
				double intensity = color.GetBrightness ();
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
			get { return Drawing.Color.FromBrightness(0.0); }
		}

		public Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.colorBorder;
		}

		public double AlphaVMenu { get { return 1.0; } }

		public Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(3,3,3,3); } }
		public Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public double GeometryComboRightMargin { get { return 2; } }
		public double GeometryComboBottomMargin { get { return 2; } }
		public double GeometryComboTopMargin { get { return 2; } }
		public double GeometryUpDownRightMargin { get { return 2; } }
		public double GeometryUpDownBottomMargin { get { return 2; } }
		public double GeometryUpDownTopMargin { get { return 2; } }
		public double GeometryScrollerRightMargin { get { return 2; } }
		public double GeometryScrollerBottomMargin { get { return 2; } }
		public double GeometryScrollerTopMargin { get { return 2; } }
		public double GeometrySelectedLeftMargin { get { return -2; } }
		public double GeometrySelectedRightMargin { get { return 2; } }
		public double GeometrySliderLeftMargin { get { return 0; } }
		public double GeometrySliderRightMargin { get { return 0; } }


		protected Drawing.Image		bitmap;
		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorControl;
		protected Drawing.Color		colorControlLight;
		protected Drawing.Color		colorControlLightLight;
		protected Drawing.Color		colorControlDark;
		protected Drawing.Color		colorControlDarkDark;
		protected Drawing.Color		colorScrollerBack;
		protected Drawing.Color		colorCaption;
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorCaptionLight;
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorWindow;
	}
}
