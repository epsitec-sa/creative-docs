namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.Default implémente le décorateur par défaut.
	/// </summary>
	public class Default : IAdorner
	{
		public Default()
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

			r = 1-(1-this.colorControlLight.R)/2;
			g = 1-(1-this.colorControlLight.G)/2;
			b = 1-(1-this.colorControlLight.B)/2;
			this.colorScrollerBack = Drawing.Color.FromRGB(r,g,b);

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorControlReadOnly = Drawing.Color.FromRGB(r,g,b);
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

			if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
			{
				rect.Offset(1, -1);
			}
			Drawing.Point center = rect.Center;
			Drawing.Path path = new Drawing.Path();
			double spikeShift = 0.15;
			double baseShiftH = 0.30;
			double baseShiftV = 0.15;
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
				if ( type == GlyphShape.Validate )  color = Drawing.Color.FromRGB(0.0, 0.5, 0.0);  // vert foncé
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
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
			{
				graphics.RenderSolid(this.colorControl);
			}
			else
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			// Ombre claire en bas à droite.
			Direction shadow = Direction.Up;
			this.PaintL(graphics, rect, this.colorControlLightLight, shadow);
			this.PaintL(graphics, rInside, this.colorControlLight, shadow);

			// Ombre foncée en haut à droite.
			this.PaintL(graphics, rect, this.colorControlDarkDark, Opposite(shadow));
			this.PaintL(graphics, rInside, this.colorControlDark, Opposite(shadow));

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
			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			// Ombre claire en bas à droite.
			Direction shadow = Direction.Up;
			this.PaintHalfCircle(graphics, rect, this.colorControlLightLight, shadow);
			this.PaintHalfCircle(graphics, rInside, this.colorControlLight, shadow);

			// Ombre foncée en haut à droite.
			this.PaintHalfCircle(graphics, rect, this.colorControlDarkDark, Opposite(shadow));
			this.PaintHalfCircle(graphics, rInside, this.colorControlDark, Opposite(shadow));

			rInside = rect;
			rInside.Deflate(2);
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
			Direction shadow = Direction.Up;

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControl);

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(1);

				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					// Rectangle noir autour.
					Drawing.Rectangle rOut = rect;
					rOut.Deflate(0.5);
					graphics.AddRectangle(rOut);
					graphics.RenderSolid(this.colorBlack);
					rOut.Deflate(1);
					graphics.AddRectangle(rOut);
					graphics.RenderSolid(this.colorControlDark);
				}
				else
				{
					if ( style == ButtonStyle.DefaultAccept )
					{
						// Rectangle noir autour.
						Drawing.Rectangle rOut = rect;
						rOut.Deflate(0.5);
						graphics.AddRectangle(rOut);
						graphics.RenderSolid(this.colorBlack);

						rect.Deflate(1);
						rInside.Deflate(1);
					}

					// Ombre claire en haut à gauche.
					PaintL(graphics, rect, this.colorControlLightLight, Opposite(shadow));
					PaintL(graphics, rInside, this.colorControlLight, Opposite(shadow));

					// Ombre foncée en bas à droite.
					PaintL(graphics, rect, this.colorControlDarkDark, shadow);
					PaintL(graphics, rInside, this.colorControlDark, shadow);
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
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControl);

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(1);

				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					shadow = Opposite(shadow);
				}

				// Ombre claire en haut à gauche.
				PaintL(graphics, rect, this.colorControlLight, Opposite(shadow));
				PaintL(graphics, rInside, this.colorControlLightLight, Opposite(shadow));

				// Ombre foncée en bas à droite.
				PaintL(graphics, rect, this.colorControlDarkDark, shadow);
				PaintL(graphics, rInside, this.colorControlDark, shadow);
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activé ?
				{
					graphics.RenderSolid(this.colorControlLight);
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					shadow = Opposite(shadow);
				}
				if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activé ?
				{
					shadow = Opposite(shadow);
				}

				// Ombre claire en haut à gauche.
				PaintL(graphics, rect, this.colorControlLightLight, Opposite(shadow));

				// Ombre foncée en bas à droite.
				PaintL(graphics, rect, this.colorControlDarkDark, shadow);
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
				Drawing.Rectangle rFocus = rect;
				rFocus.Deflate(3.5);
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
				rInside.Deflate(1);

				// Ombre foncée en haut à gauche.
				Direction shadow = Direction.Up;
				PaintL(graphics, rect, this.colorControlDark, Opposite(shadow));
				PaintL(graphics, rInside, this.colorControlDarkDark, Opposite(shadow));

				// Ombre claire en bas à droite.
				PaintL(graphics, rect, this.colorControlLightLight, shadow);
				PaintL(graphics, rInside, this.colorControlLight, shadow);
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
			graphics.AddFilledRectangle(frameRect);
			graphics.RenderSolid(this.colorScrollerBack);

			if ( !tabRect.IsSurfaceZero && (state&WidgetState.Engaged) != 0 )
			{
				graphics.AddFilledRectangle(tabRect);
				graphics.RenderSolid(this.colorControlDarkDark);
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
			this.PaintButtonBackground(graphics, thumbRect, state, Direction.Up, ButtonStyle.Scroller);
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
			rect.Deflate(1);
			graphics.LineWidth = 2;
			this.RectangleGroupBox(graphics, rect, titleRect.Left, titleRect.Right);
			graphics.RenderSolid(this.colorControlLightLight);

			rect = frameRect;
			rect.Deflate(0.5);
			rect.Right --;
			rect.Bottom ++;
			graphics.LineWidth = 1;
			this.RectangleGroupBox(graphics, rect, titleRect.Left, titleRect.Right);
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
			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			// Ombre claire en haut à gauche.
			Direction shadow = Direction.Up;
			PaintL(graphics, rect, this.colorControlLightLight, Opposite(shadow));
			PaintL(graphics, rInside, this.colorControlLight, Opposite(shadow));

			// Ombre foncée en bas à droite.
			PaintL(graphics, rect, this.colorControlDarkDark, shadow);
			PaintL(graphics, rInside, this.colorControlDark, shadow);
		}

		// Dessine l'onglet devant les autres.
		public void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
			Drawing.Rectangle rBack = titleRect;
			rBack.Right  -= 2;
			rBack.Bottom -= 1;
			rBack.Top    -= 2;
			graphics.AddFilledRectangle(rBack);
			graphics.RenderSolid(this.colorControl);

			titleRect.Left   -= 2;
			titleRect.Bottom += 1;
			this.PaintTabBackground(graphics, frameRect, titleRect, state, dir);
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
			this.PaintTabBackground(graphics, frameRect, titleRect, state, dir);
		}

		// Dessine un onglet quelconque.
		protected void PaintTabBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle frameRect,
										  Drawing.Rectangle titleRect,
										  Widgets.WidgetState state,
										  Widgets.Direction dir)
		{
			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			Drawing.Point p1 = new Drawing.Point();
			Drawing.Point p2 = new Drawing.Point();

			p1.X = titleRect.Left+0.5;
			p1.Y = titleRect.Bottom;
			p2.X = titleRect.Left+0.5;
			p2.Y = titleRect.Top-2;
			graphics.AddLine(p1, p2);
			p1.X = titleRect.Left+0.5;
			p1.Y = titleRect.Top-2.5;
			p2.X = titleRect.Left+2.5;
			p2.Y = titleRect.Top-0.5;
			graphics.AddLine(p1, p2);
			p1.X = titleRect.Left+2;
			p1.Y = titleRect.Top-0.5;
			p2.X = titleRect.Right-2;
			p2.Y = titleRect.Top-0.5;
			graphics.AddLine(p1, p2);
			graphics.RenderSolid(this.colorControlLightLight);

			p1.X = titleRect.Left+1.5;
			p1.Y = titleRect.Bottom;
			p2.X = titleRect.Left+1.5;
			p2.Y = titleRect.Top-2;
			graphics.AddLine(p1, p2);
			p1.X = titleRect.Left+2;
			p1.Y = titleRect.Top-1.5;
			p2.X = titleRect.Right-1;
			p2.Y = titleRect.Top-1.5;
			graphics.AddLine(p1, p2);
			graphics.RenderSolid(this.colorControlLight);

			p1.X = titleRect.Right-0.5;
			p1.Y = titleRect.Bottom;
			p2.X = titleRect.Right-0.5;
			p2.Y = titleRect.Top-2;
			graphics.AddLine(p1, p2);
			graphics.RenderSolid(this.colorControlDarkDark);

			p1.X = titleRect.Right-1.5;
			p1.Y = titleRect.Bottom;
			p2.X = titleRect.Right-1.5;
			p2.Y = titleRect.Top-1;
			graphics.AddLine(p1, p2);
			graphics.RenderSolid(this.colorControlDark);
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
			rInside.Deflate(1);

			// Ombre foncée en haut à gauche.
			Direction shadow = Direction.Up;
			PaintL(graphics, rect, this.colorControlDark, Opposite(shadow));
			PaintL(graphics, rInside, this.colorControlDarkDark, Opposite(shadow));

			// Ombre claire en bas à droite.
			PaintL(graphics, rect, this.colorControlLightLight, shadow);
			PaintL(graphics, rInside, this.colorControlLight, shadow);
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
			this.PaintButtonBackground(graphics, rect, state, dir, ButtonStyle.Scroller);
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
			rect.Deflate(0.5);

			graphics.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
			graphics.RenderSolid(this.colorControlLightLight);

			graphics.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			graphics.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
			graphics.RenderSolid(this.colorControlDark);
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
#if true
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlLightLight);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDark);
#else
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			// Ombre claire en haut à gauche.
			PaintL(graphics, rect, this.colorControlLightLight, Opposite(shadow));
			PaintL(graphics, rInside, this.colorControlLight, Opposite(shadow));

			// Ombre foncée en bas à droite.
			PaintL(graphics, rect, this.colorControlDarkDark, shadow);
			PaintL(graphics, rInside, this.colorControlDark, shadow);
#endif
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
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorCaption);
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
			state &= ~WidgetState.Focused;
			if ( itemType == MenuItemType.Deselect )
			{
				state &= ~WidgetState.Selected;
			}
			else
			{
				state |= WidgetState.Selected;
			}
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
			if ( optional )  return;

			if ( dir == Direction.Right )
			{
				Drawing.Point p1 = new Drawing.Point(rect.Left+rect.Width/2, rect.Bottom);
				Drawing.Point p2 = new Drawing.Point(rect.Left+rect.Width/2, rect.Top);
				graphics.Align(ref p1);
				graphics.Align(ref p2);
				p1.X -= 0.5;
				p2.X -= 0.5;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorControlDark);

				p1.X += 1.0;
				p2.X += 1.0;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorControlLightLight);
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
				graphics.RenderSolid(this.colorControlLightLight);

				p1.Y += 1.0;
				p2.Y += 1.0;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorControlDark);
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
			graphics.RenderSolid(this.colorControlDarkDark);
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
									   Drawing.Color color,
									   Direction dir)
		{
			graphics.Align(ref rect);
			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			// Ombre foncée en bas à droite.
			Direction shadow = Direction.Up;
			this.PaintHalfCircle(graphics, rect, this.colorControlLightLight, Opposite(shadow));
			this.PaintHalfCircle(graphics, rInside, this.colorControlLight, Opposite(shadow));

			// Ombre claire en haut à droite.
			this.PaintHalfCircle(graphics, rect, this.colorControlDarkDark, shadow);
			this.PaintHalfCircle(graphics, rInside, this.colorControlDark, shadow);

			rInside = rect;
			rInside.Deflate(2);
			if ( color.IsEmpty || (state&WidgetState.Enabled) == 0 )
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else
			{
				color.R = 1.0-(1.0-color.R)*0.5;
				color.G = 1.0-(1.0-color.G)*0.5;
				color.B = 1.0-(1.0-color.B)*0.5;
				this.PaintCircle(graphics, rInside, color);
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
										   Drawing.Point pos,
										   TextLayout text,
										   WidgetState state,
										   PaintTextStyle style,
										   Drawing.Color backColor)
		{
			if ( text == null )  return;

			TextLayout.DefaultColor = this.colorBlack;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
				{
					text.Paint(pos, graphics, graphics.ClipBounds, this.colorCaptionText, Drawing.GlyphPaintStyle.Selected);
				}
				else
				{
					text.Paint(pos, graphics);
				}
			}
			else
			{
				double gamma = graphics.Rasterizer.Gamma;
				graphics.Rasterizer.Gamma = 0.5;
				pos.X ++;
				pos.Y --;
				text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorControlLightLight, Drawing.GlyphPaintStyle.Disabled);
				graphics.Rasterizer.Gamma = gamma;  // remet gamma initial
				pos.X --;
				pos.Y ++;
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

		// Dessine un "L" pour simuler une ombre.
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
					p1.Y = rect.Bottom+1.0;
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
					p1.X = rect.Left+1.0;
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
					p1.X = rect.Left+1.0;
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
					p1.Y = rect.Top+1.0;
					p2.X = p1.X;
					p2.Y = rect.Bottom;
					graphics.AddLine(p1, p2);
					break;
			}
			graphics.RenderSolid(color);
		}

		// Dessine un demi-cercle en bas à droite si dir=Up.
		protected void PaintHalfCircle(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   Drawing.Color color,
									   Widgets.Direction dir)
		{
			double angle = 0;
			switch ( dir )
			{
				case Direction.Up:		angle = 180+45;	break;  // en bas à droite
				case Direction.Down:	angle =     45;	break;  // en haut à gauche
				case Direction.Left:	angle =  90+45;	break;  // en bas à gauche
				case Direction.Right:	angle = 270+45;	break;  // en haut à droite
			}
			PaintHalfCircle(graphics, rect, color, angle);
		}

		// Dessine un demi-cercle. Si angle=0, le demi-cercle est en haut.
		// L'angle est donné en degrés.
		protected void PaintHalfCircle(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   Drawing.Color color,
									   double angle)
		{
			Drawing.Point c = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			Drawing.Path path = new Drawing.Path();
			angle = angle*System.Math.PI/180;  // angle en radians
			double c1x, c1y, c2x, c2y, px, py;
			px  = -rx;       py  = 0;        this.RotatePoint(angle, ref px,  ref py);
			path.MoveTo(c.X+px, c.Y+py);
			c1x = -rx;       c1y = ry*0.56;  this.RotatePoint(angle, ref c1x, ref c1y);
			c2x = -rx*0.56;  c2y = ry;       this.RotatePoint(angle, ref c2x, ref c2y);
			px  = 0;         py  = ry;       this.RotatePoint(angle, ref px,  ref py);
			path.CurveTo(c.X+c1x, c.Y+c1y, c.X+c2x, c.Y+c2y, c.X+px, c.Y+py);
			c1x = rx*0.56;   c1y = ry;       this.RotatePoint(angle, ref c1x, ref c1y);
			c2x = rx;        c2y = ry*0.56;  this.RotatePoint(angle, ref c2x, ref c2y);
			px  = rx;        py  = 0;        this.RotatePoint(angle, ref px,  ref py);
			path.CurveTo(c.X+c1x, c.Y+c1y, c.X+c2x, c.Y+c2y, c.X+px, c.Y+py);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(color);
		}

		// Fait tourner un point autour de l'origine.
		// L'angle est exprimé en radians.
		// Un angle positif est anti-horaire (CCW).
		protected void RotatePoint(double angle, ref double x, ref double y)
		{
			double xx = x*System.Math.Cos(angle) - y*System.Math.Sin(angle);
			double yy = x*System.Math.Sin(angle) + y*System.Math.Cos(angle);
			x = xx;
			y = yy;
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
		

		public void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				color.R = uniqueColor.R;
				color.G = uniqueColor.G;
				color.B = uniqueColor.B;
			}
		}

		public Drawing.Color ColorDisabled
		{
			get { return this.colorControlDark; }
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
			get { return this.colorControlDarkDark; }
		}

		public Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.colorControlDarkDark;
		}

		public Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.colorControlDarkDark;
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
		public double GeometrySelectedLeftMargin { get { return 0; } }
		public double GeometrySelectedRightMargin { get { return 0; } }
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
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorWindow;
	}
}
