namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookPlastic implémente le décorateur qui imite le plastic.
	/// </summary>
	public class LookPlastic : IAdorner
	{
		public LookPlastic()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources.LookPlastic.png", this.GetType().Assembly);
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
			this.colorCaption           = Drawing.Color.FromName("ActiveCaption");
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

			this.colorCaption = Drawing.Color.FromRGB( 58.0/255.0, 167.0/255.0, 233.0/255.0);
			this.colorHilite  = Drawing.Color.FromRGB(250.0/255.0, 196.0/255.0,  89.0/255.0);
			this.colorBorder  = Drawing.Color.FromRGB( 23.0/255.0, 132.0/255.0, 198.0/255.0);
			this.colorWindow  = Drawing.Color.FromRGB(198.0/255.0, 226.0/255.0, 234.0/255.0);
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

			double zoom = 1.0;
			if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
			{
				zoom = 1.3;
			}
			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			Drawing.Path path = new Drawing.Path();
			double spikeShift = 0.25*zoom;
			double baseShiftH = 0.20*zoom;
			double baseShiftV = 0.20*zoom;
			switch ( type )
			{
				case GlyphShape.ArrowUp:
					path.MoveTo(center.X, center.Y+rect.Height*spikeShift);
					path.LineTo(center.X-rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					path.LineTo(center.X+rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					break;

				case GlyphShape.ArrowDown:
					path.MoveTo(center.X, center.Y-rect.Height*spikeShift);
					path.LineTo(center.X-rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					path.LineTo(center.X+rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					break;

				case GlyphShape.ArrowRight:
					path.MoveTo(center.X+rect.Width*spikeShift, center.Y);
					path.LineTo(center.X-rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo(center.X-rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
					break;

				case GlyphShape.ArrowLeft:
					path.MoveTo(center.X-rect.Width*spikeShift, center.Y);
					path.LineTo(center.X+rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo(center.X+rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
					break;

				case GlyphShape.Menu:
					path.MoveTo(center.X+rect.Width*0.00*zoom, center.Y-rect.Height*0.25*zoom);
					path.LineTo(center.X-rect.Width*0.30*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.30*zoom, center.Y+rect.Height*0.15*zoom);
					break;

				case GlyphShape.Close:
				case GlyphShape.Reject:
					path.MoveTo(center.X-rect.Width*0.20*zoom, center.Y-rect.Height*0.30*zoom);
					path.LineTo(center.X-rect.Width*0.30*zoom, center.Y-rect.Height*0.20*zoom);
					path.LineTo(center.X-rect.Width*0.10*zoom, center.Y+rect.Height*0.00*zoom);
					path.LineTo(center.X-rect.Width*0.30*zoom, center.Y+rect.Height*0.20*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y+rect.Height*0.30*zoom);
					path.LineTo(center.X-rect.Width*0.00*zoom, center.Y+rect.Height*0.10*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y+rect.Height*0.30*zoom);
					path.LineTo(center.X+rect.Width*0.30*zoom, center.Y+rect.Height*0.20*zoom);
					path.LineTo(center.X+rect.Width*0.10*zoom, center.Y+rect.Height*0.00*zoom);
					path.LineTo(center.X+rect.Width*0.30*zoom, center.Y-rect.Height*0.20*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.30*zoom);
					path.LineTo(center.X+rect.Width*0.00*zoom, center.Y-rect.Height*0.10*zoom);
					break;

				case GlyphShape.Dots:
					path.MoveTo(center.X-rect.Width*0.30*zoom, center.Y+rect.Height*0.06*zoom);
					path.LineTo(center.X-rect.Width*0.18*zoom, center.Y+rect.Height*0.06*zoom);
					path.LineTo(center.X-rect.Width*0.18*zoom, center.Y-rect.Height*0.06*zoom);
					path.LineTo(center.X-rect.Width*0.30*zoom, center.Y-rect.Height*0.06*zoom);
					path.Close();
					path.MoveTo(center.X-rect.Width*0.06*zoom, center.Y+rect.Height*0.06*zoom);
					path.LineTo(center.X+rect.Width*0.06*zoom, center.Y+rect.Height*0.06*zoom);
					path.LineTo(center.X+rect.Width*0.06*zoom, center.Y-rect.Height*0.06*zoom);
					path.LineTo(center.X-rect.Width*0.06*zoom, center.Y-rect.Height*0.06*zoom);
					path.Close();
					path.MoveTo(center.X+rect.Width*0.18*zoom, center.Y+rect.Height*0.06*zoom);
					path.LineTo(center.X+rect.Width*0.30*zoom, center.Y+rect.Height*0.06*zoom);
					path.LineTo(center.X+rect.Width*0.30*zoom, center.Y-rect.Height*0.06*zoom);
					path.LineTo(center.X+rect.Width*0.18*zoom, center.Y-rect.Height*0.06*zoom);
					break;

				case GlyphShape.Accept:
					path.MoveTo(center.X-rect.Width*0.30*zoom, center.Y+rect.Height*0.00*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y+rect.Height*0.10*zoom);
					path.LineTo(center.X-rect.Width*0.10*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y+rect.Height*0.30*zoom);
					path.LineTo(center.X+rect.Width*0.30*zoom, center.Y+rect.Height*0.20*zoom);
					path.LineTo(center.X-rect.Width*0.10*zoom, center.Y-rect.Height*0.30*zoom);
					break;
			}
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			Drawing.Color color = this.colorBlack;
			if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRGB(0.5, 0.0, 0.0);  // rouge foncé
			if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRGB(0.0, 0.5, 0.0);  // vert foncé
			graphics.RenderSolid(color);
		}

		// Dessine un bouton à cocher sans texte.
		public void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetState.ActiveYes) != 0 )  // coché ?
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 52);
					}
					else if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 51);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 51);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 53);
				}
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 49);
					}
					else if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 51);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 48);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 50);
				}
			}
		}

		// Dessine un bouton radio sans texte.
		public void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetState.ActiveYes) != 0 )  // coché ?
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 44);
					}
					else if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 43);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 43);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 45);
				}
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 41);
					}
					else if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 43);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 40);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 42);
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
			rFocus.Deflate(2.5, 2.5);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1, 1);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
					else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					else
					{
						if ( style == ButtonStyle.DefaultAccept )
						{
							this.PaintImageButton(graphics, rect, 2);
						}
						else
						{
							this.PaintImageButton(graphics, rect, 0);
						}
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 3);
				}
			}
			else if ( style == ButtonStyle.Scroller )
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
						this.PaintImageButton(graphics, rInside, 0);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 3);
				}
			}
			else if ( style == ButtonStyle.Combo        ||
					  style == ButtonStyle.ExListLeft   ||
					  style == ButtonStyle.ExListMiddle ||
					  style == ButtonStyle.ExListRight  ||
					  style == ButtonStyle.Icon         ||
					  style == ButtonStyle.HeaderSlider )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
					else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 0);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 3);
				}
				rFocus.Inflate(1);
			}
			else if ( style == ButtonStyle.UpDown )
			{
				if ( (state&WidgetState.Enabled) == 0 )
				{
					graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
					graphics.RenderSolid(this.colorBorder);
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
					else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 0);
					}
				}
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 1);
				}
				if ( (state&WidgetState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 4);
				}
				if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rect, 2);
				}
				rFocus.Inflate(2);
			}
			else if ( style == ButtonStyle.ListItem )
			{
				this.PaintImageButton(graphics, rect, 0);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 0);
			}
			
			if ( (state&WidgetState.Focused) != 0 )
			{
				Drawing.Path pInside = this.PathRoundRectangle(rFocus, this.RetRadiusFrame(rFocus));
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
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, readOnly?28:26);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 28);
				}

				double radius = this.RetRadiusFrame(rect);
				Drawing.Path path = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else if ( style == TextFieldStyle.UpDown )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, readOnly?28:26);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 28);
				}

				double radius = this.RetRadiusFrame(rect);
				Drawing.Path path = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorControlLightLight);
				}

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(0.5);
				graphics.AddRectangle(rInside);
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
				this.PaintImageButton(graphics, frameRect, 25);
			}
			else
			{
				this.PaintImageButton(graphics, frameRect, 27);
			}

			if ( !tabRect.IsSurfaceZero && (state&WidgetState.Engaged) != 0 )
			{
				this.PaintImageButton(graphics, tabRect, 34);
			}
		}

		// Dessine la cabine d'un ascenseur.
		public void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetState state,
										Widgets.Direction dir)
		{
			if ( dir == Direction.Up )
			{
				thumbRect.Bottom -= 1;
				thumbRect.Top    += 1;
			}
			if ( dir == Direction.Left )
			{
				thumbRect.Left  -= 1;
				thumbRect.Right += 1;
			}
			state &= ~WidgetState.Engaged;
			this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

			Drawing.Rectangle	rect = new Drawing.Rectangle();
			Drawing.Point		center;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				switch ( dir )
				{
					case Direction.Up:
					case Direction.Down:
						if ( thumbRect.Width >= 10 && thumbRect.Height >= 20 )
						{
							center = new Drawing.Point((thumbRect.Left+thumbRect.Right)/2, (thumbRect.Bottom+thumbRect.Top)/2);
							rect.Left   = center.X-thumbRect.Width*0.2;
							rect.Right  = center.X+thumbRect.Width*0.2;
							rect.Bottom = center.Y-thumbRect.Width*0.4;
							rect.Top    = center.Y+thumbRect.Width*0.4;
							graphics.Align(ref rect);
							this.PaintImageButton(graphics, rect, 36);
						}
						break;

					case Direction.Left:
					case Direction.Right:
						if ( thumbRect.Height >= 10 && thumbRect.Width >= 20 )
						{
							center = new Drawing.Point((thumbRect.Left+thumbRect.Right)/2, (thumbRect.Bottom+thumbRect.Top)/2);
							rect.Left   = center.X-thumbRect.Height*0.4;
							rect.Right  = center.X+thumbRect.Height*0.4;
							rect.Bottom = center.Y-thumbRect.Height*0.2;
							rect.Top    = center.Y+thumbRect.Height*0.2;
							graphics.Align(ref rect);
							this.PaintImageButton(graphics, rect, 37);
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
			Drawing.Path path = this.PathRoundRectangleGroupBox(frameRect, titleRect.Left, titleRect.Right);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.colorBorder);
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
			this.PaintImageButton(graphics, rect, 32);

			rect.Deflate(0.5);
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
				this.PaintImageButton(graphics, titleRect, 9);
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 8);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 11);
				}
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
				this.PaintImageButton(graphics, titleRect, 9);
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 12);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 11);
				}
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
				this.PaintImageButton(graphics, rect, 26);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 28);
			}

			double radius = this.RetRadiusFrame(rect);
			Drawing.Path path = this.PathRoundRectangle(rect, radius);
			graphics.Rasterizer.AddOutline(path, 1);
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
				if ( (state&WidgetState.Focused) != 0 )
				{
					this.PaintImageButton(graphics, rect, 33);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 38);
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
				if ( dir == Direction.Up )
				{
					this.PaintImageButton(graphics, rect, 9);
				}
				if ( dir == Direction.Left )
				{
					this.PaintImageButton(graphics, rect, 17);
				}
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( dir == Direction.Up )
				{
					this.PaintImageButton(graphics, rect, 8);
				}
				if ( dir == Direction.Left )
				{
					this.PaintImageButton(graphics, rect, 16);
				}
			}
			else
			{
				if ( dir == Direction.Up )
				{
					this.PaintImageButton(graphics, rect, 11);
				}
				if ( dir == Direction.Left )
				{
					this.PaintImageButton(graphics, rect, 19);
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
			this.PaintImageButton(graphics, rect, 0);
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
			this.PaintImageButton(graphics, rect, 32);

#if false
			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Left += 1;
				band.Width = iconWidth-1;
				band.Top -= 1;
				band.Bottom += 1;
				this.PaintImageButton(graphics, band, 32);
			}
#endif

			rect.Deflate(0.5);
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
			if ( type == MenuType.Horizontal )
			{
				if ( itemType == MenuItemType.Select )
				{
					this.PaintImageButton(graphics, rect, 0);
				}
				if ( itemType == MenuItemType.Parent )
				{
					this.PaintImageButton(graphics, rect, 8);
				}
			}

			if ( type == MenuType.Vertical )
			{
				if ( itemType != MenuItemType.Deselect )
				{
					this.PaintImageButton(graphics, rect, 0);
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

			graphics.RenderSolid(this.colorBorder);
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
			double radius = this.RetRadiusFrame(rect);
			Drawing.Path pInside = this.PathRoundRectangle(rect, radius);
			graphics.Rasterizer.AddOutline(pInside);
			graphics.RenderSolid(this.colorBorder);
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
			graphics.RenderSolid(this.colorBorder);
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
			graphics.RenderSolid(this.colorControlLightLight);
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
				if ( (state&WidgetState.Focused) != 0 )
				{
					this.PaintImageButton(graphics, areas[i].Rect, 33);

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
					this.PaintImageButton(graphics, areas[i].Rect, 38);
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
					text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorCaptionText, Drawing.GlyphPaintStyle.Selected);
				}
				else
				{
					text.Paint(pos, graphics);
				}
			}
			else
			{
				text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorControlDarkDark, Drawing.GlyphPaintStyle.Disabled);
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

			double radius = this.RetRadiusFrame(rect);
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

		// Crée le chemin d'un rectangle à coins arrondis en forme de "U" inversé.
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

		// Crée le chemin d'un rectangle à coins arrondis en forme de "D" inversé.
		protected Drawing.Path PathLeftRoundRectangle(Drawing.Rectangle rect, double radius, bool closed)
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
			path.MoveTo (ox+dx-0.5, oy+0.5);
			path.LineTo (ox+radius+0.5, oy+0.5);
			path.CurveTo(ox+0.5, oy+0.5, ox+0.5, oy+radius+0.5);
			path.LineTo (ox+0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+radius+0.5, oy+dy-0.5);
			path.LineTo (ox+dx-0.5, oy+dy-0.5);
			if ( closed )  path.Close();

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
		protected double RetRadiusButton(Drawing.Rectangle rect)
		{
			double dim = System.Math.Min(rect.Width, rect.Height);
			double radius = System.Math.Min(dim/2, 10);
			double middle = System.Math.Max(dim-radius*2, 2);
			return System.Math.Floor((dim-middle)/2);
		}

		// Retourne le rayon à utiliser pour une zone rectangulaire.
		protected double RetRadiusFrame(Drawing.Rectangle rect)
		{
			double dim = System.Math.Min(rect.Width, rect.Height);
			double radius = System.Math.Min(dim/2, 5);
			double middle = System.Math.Max(dim-radius*2, 2);
			return System.Math.Floor((dim-middle)/2);
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

			if ( rank < 24 )
			{
				this.PaintImageButton9(graphics, rect, this.RetRadiusButton(rect), icon, 14);
			}
			else if ( rank < 32 )
			{
				this.PaintImageButton9(graphics, rect, this.RetRadiusFrame(rect), icon, 14);
			}
			else
			{
				this.PaintImageButton1(graphics, rect, icon);
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
				intensity = System.Math.Min(intensity+0.2, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromBrightness(intensity);
				color.G = System.Math.Min(color.G*1.4, 1.0);
				color.B = System.Math.Min(color.B*1.4, 1.0);  // bleuté
				color.A = alpha;
			}
		}

		public Drawing.Color ColorDisabled
		{
			get { return Drawing.Color.Empty; }
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

		public Drawing.Color ColorBorder
		{
			get { return this.colorBorder; }
		}

		public Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.colorBorder;
		}

		public Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.colorBorder;
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
		public double GeometryUpDownRightMargin { get { return 2; } }
		public double GeometryUpDownBottomMargin { get { return 2; } }
		public double GeometryUpDownTopMargin { get { return 2; } }
		public double GeometryScrollerRightMargin { get { return 2; } }
		public double GeometryScrollerBottomMargin { get { return 2; } }
		public double GeometryScrollerTopMargin { get { return 2; } }
		public double GeometryScrollListXMargin { get { return 2; } }
		public double GeometryScrollListYMargin { get { return 2; } }
		public double GeometrySliderLeftMargin { get { return 5; } }
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
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorWindow;
	}
}
