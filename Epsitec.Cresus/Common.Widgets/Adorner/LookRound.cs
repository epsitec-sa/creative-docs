namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookRound implémente le décorateur rond & rigolo.
	/// </summary>
	public class LookRound : IAdorner
	{
		public LookRound()
		{
			RefreshColors();
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
			this.colorCaptionText       = Drawing.Color.FromName("ActiveCaptionText");

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

			this.colorHilite = Drawing.Color.FromRGB(250.0/255.0, 196.0/255.0,  89.0/255.0);
		}
		

		// Dessine le fond d'une fenêtre.
		public void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction shadow)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorWindow);
		}

		// Dessine une flèche (dans un bouton d'ascenseur par exemple).
		public void PaintArrow(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state,
							   Widgets.Direction shadow,
							   Widgets.Direction dir)
		{
			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			Drawing.Path path = new Drawing.Path();
			double spikeShift = 0.15;
			double baseShiftH = 0.30;
			double baseShiftV = 0.15;
			switch ( dir )
			{
				case Direction.Up:
					path.MoveTo(center.X, center.Y+rect.Height*spikeShift);
					path.LineTo(center.X-rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					path.LineTo(center.X+rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					break;

				case Direction.Down:
					path.MoveTo(center.X, center.Y-rect.Height*spikeShift);
					path.LineTo(center.X-rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					path.LineTo(center.X+rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					break;

				case Direction.Right:
					path.MoveTo(center.X+rect.Width*spikeShift, center.Y);
					path.LineTo(center.X-rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo(center.X-rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
					break;

				case Direction.Left:
					path.MoveTo(center.X-rect.Width*spikeShift, center.Y);
					path.LineTo(center.X+rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo(center.X+rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
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
							   Widgets.WidgetState state,
							   Widgets.Direction shadow)
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
							   Widgets.WidgetState state,
							   Widgets.Direction shadow)
		{
			graphics.Align(ref rect);
			Drawing.Rectangle rInside;

			this.PaintCircle(graphics, rect, this.colorControlDarkDark);

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
					if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
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

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
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
					graphics.SolidRenderer.Color = this.colorControlDark;
				}
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid();
			}
			else if ( style == ButtonStyle.Scroller )
			{
				double radius = System.Math.Min(3, System.Math.Min(rect.Width, rect.Height));
				Drawing.Path path = PathRoundRectangle(rect, radius);
			
				graphics.SolidRenderer.Color = this.colorControl;
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid();
			
				if ( (state&WidgetState.Focused) != 0 )
				{
					Drawing.Rectangle rInside = rect;
					rInside.Inflate(-1.5, -1.5);
					Drawing.Path pInside = PathRoundRectangle(rInside, radius);
					graphics.SolidRenderer.Color = this.colorControlDark;
					graphics.Rasterizer.AddOutline(pInside, 2);
					graphics.RenderSolid();
				}

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rInside = rect;
					rInside.Inflate(-1.5, -1.5);
					Drawing.Path pInside = PathRoundRectangle(rInside, radius);
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
					graphics.SolidRenderer.Color = this.colorControlDark;
				}
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid();
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				rect.Right += 1;

				if ( (state&WidgetState.Entered) != 0 ||  // bouton survolé ?
					 (state&WidgetState.Engaged) != 0 )   // bouton pressé ?
				{
					double radius = System.Math.Min(3, System.Math.Min(rect.Width, rect.Height));
					Drawing.Path path = PathRoundRectangle(rect, radius);

					graphics.SolidRenderer.Color = this.colorCaptionLight;
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid();

					graphics.SolidRenderer.Color = this.colorCaption;
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid();
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

			if ( (state&WidgetState.Engaged) != 0 &&  // bouton pressé ?
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

		// Dessine le fond d'une ligne éditable.
		public void PaintTextFieldBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetState state,
											 Widgets.Direction shadow,
											 Widgets.TextFieldStyle style,
											 bool readOnly)
		{
			if ( style == TextFieldStyle.Normal )
			{
				double radius = System.Math.Min(3, System.Math.Min(rect.Width, rect.Height));
				Drawing.Path path = PathRoundRectangle(rect, radius);

				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					if ( readOnly )
					{
						graphics.RenderSolid(this.colorControl);
					}
					else
					{
						graphics.RenderSolid(this.colorControlLightLight);
					}
				}
				else
				{
					graphics.SolidRenderer.Color = this.colorControl;
				}
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid();

				graphics.SolidRenderer.Color = this.colorBlack;
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid();
			}
			else if ( style == TextFieldStyle.Simple )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControlLightLight);

				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBlack);
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
			double radius = System.Math.Min(3, System.Math.Min(frameRect.Width, frameRect.Height));
			Drawing.Path path = PathRoundRectangle(frameRect, radius);

			graphics.SolidRenderer.Color = this.colorScrollerBack;
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid();

			graphics.SolidRenderer.Color = this.colorControlDark;
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid();

			if ( !tabRect.IsEmpty )
			{
				graphics.AddFilledRectangle(tabRect);
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		// Dessine la cabine d'un ascenseur.
		public void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle frameRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetState state,
										Widgets.Direction shadow)
		{
			this.PaintButtonBackground(graphics, frameRect, state, shadow, ButtonStyle.Scroller);
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
			double radius = System.Math.Min(7, System.Math.Min(frameRect.Width, frameRect.Height));
			Drawing.Path path = PathRoundRectangle(frameRect, radius);
			graphics.SolidRenderer.Color = this.colorControlDark;
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid();

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

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/3;
			Drawing.Path pTitle = PathTopRoundRectangle(titleRect, radius);

			graphics.SolidRenderer.Color = this.colorControlLight;
			graphics.Rasterizer.AddSurface(pTitle);
			graphics.RenderSolid();

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				Drawing.Rectangle rHilite = new Drawing.Rectangle();
				rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = PathTopRoundRectangle(rHilite, radius);
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

		// Dessine un onglet derrière (non sélectionné).
		public void PaintTabSunkenBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetState state,
											 Widgets.Direction shadow)
		{
			titleRect.Left  += 1;
			titleRect.Right -= 1;

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/3;
			Drawing.Path pTitle = PathTopRoundRectangle(titleRect, radius);

			graphics.SolidRenderer.Color = this.colorControl;
			graphics.Rasterizer.AddSurface(pTitle);
			graphics.RenderSolid();

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
			{
				Drawing.Rectangle rHilite = new Drawing.Rectangle();
				rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = PathTopRoundRectangle(rHilite, radius);
				graphics.SolidRenderer.Color = this.colorHilite;
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid();
			}

			graphics.SolidRenderer.Color = this.colorControlDarkDark;
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

		// Dessine le fond d'une cellule.
		public void PaintCellBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow)
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

			double radius = System.Math.Min(rect.Width, rect.Height)/3;
			Drawing.Path path;
			if ( type == Direction.Up )
			{
				path = this.PathTopRoundRectangle(rect, radius);
			}
			else
			{
				path = this.PathLeftRoundRectangle(rect, radius);
			}
			graphics.SolidRenderer.Color = this.colorControl;
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid();

			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
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
			rect.Inflate(-0.5, -0.5);

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
			if ( parentRect.IsEmpty )
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
					double radius = System.Math.Min(3, System.Math.Min(rect.Width, rect.Height));
					Drawing.Path path = PathRoundRectangle(rect, radius);

					graphics.SolidRenderer.Color = this.colorCaptionLight;
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid();

					graphics.SolidRenderer.Color = this.colorCaption;
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid();
				}
				if ( itemType == MenuItemType.Parent )
				{
					double radius = System.Math.Min(3, System.Math.Min(rect.Width, rect.Height));
					Drawing.Path path = PathTopRoundRectangle(rect, radius);

					graphics.SolidRenderer.Color = this.colorControl;
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid();

					graphics.SolidRenderer.Color = this.colorControlDark;
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid();
				}
			}

			if ( type == MenuType.Vertical )
			{
				if ( itemType != MenuItemType.Deselect )
				{
					double radius = System.Math.Min(3, System.Math.Min(rect.Width, rect.Height));
					Drawing.Path path = PathRoundRectangle(rect, radius);

					graphics.SolidRenderer.Color = this.colorCaptionLight;
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid();

					graphics.SolidRenderer.Color = this.colorCaption;
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid();
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
			state &= ~WidgetState.Selected;
			state &= ~WidgetState.Focused;
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

		// Dessine le rectangle pour indiquer le focus.
		public void PaintFocusBox(Drawing.Graphics graphics,
								  Drawing.Rectangle rect)
		{
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDark);
		}

		// Dessine le curseur du texte.
		public void PaintTextCursor(Drawing.Graphics graphics,
									Drawing.Point pos,
									Drawing.Rectangle rect,
									bool cursorOn)
		{
			if ( cursorOn )
			{
				rect.Offset(pos);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBlack);
			}
		}
		
		// Dessine les zones rectanglaires correspondant aux caractères sélectionnés.
		public void PaintTextSelectionBackground(Drawing.Graphics graphics,
												 Drawing.Point pos,
												 Drawing.Rectangle[] rects)
		{
			foreach ( Drawing.Rectangle rect in rects )
			{
				rect.Offset(pos);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorCaption);
			}
		}

		public void PaintTextSelectionForeground(Drawing.Graphics graphics,
												 Drawing.Point pos,
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
				text.Paint(pos, graphics, Drawing.Rectangle.Infinite, this.colorControlDark);
			}

			if ( (state&WidgetState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = text.StandardRectangle;
				rFocus.Offset(pos);
				graphics.Align (ref rFocus);
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
				radius = System.Math.Min(dx, dy)/3;
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
				radius = System.Math.Min(dx, dy)/3;
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
				radius = System.Math.Min(dx, dy)/3;
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


		// Variables membres de TextLayout.
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
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorWindow;
	}
}
