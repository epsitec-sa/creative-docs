namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookFlat impl�mente un d�corateur minimaliste.
	/// </summary>
	public class LookFlat : AbstractAdorner
	{
		public LookFlat()
		{
		}

		// Initialise les couleurs en fonction des r�glages de Windows.
		protected override void RefreshColors()
		{
			this.colorBlack             = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorControlField      = Drawing.Color.FromRGB(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorControlReadOnly   = Drawing.Color.FromRGB(240.0/255.0, 240.0/255.0, 240.0/255.0);
			this.colorWindow            = Drawing.Color.FromRGB(220.0/255.0, 220.0/255.0, 220.0/255.0);
			this.colorCaptionNF         = Drawing.Color.FromRGB(210.0/255.0, 210.0/255.0, 210.0/255.0);
			this.colorControlBar        = Drawing.Color.FromRGB(200.0/255.0, 200.0/255.0, 200.0/255.0);
			this.colorBackDisabled      = Drawing.Color.FromRGB(200.0/255.0, 200.0/255.0, 200.0/255.0);
			this.colorControlLightLight = Drawing.Color.FromRGB(180.0/255.0, 180.0/255.0, 180.0/255.0);
			this.colorFrontDisabled     = Drawing.Color.FromRGB(170.0/255.0, 170.0/255.0, 170.0/255.0);
			this.colorControlLight      = Drawing.Color.FromRGB(160.0/255.0, 160.0/255.0, 160.0/255.0);
			this.colorControl           = Drawing.Color.FromRGB(140.0/255.0, 140.0/255.0, 140.0/255.0);
			this.colorControlDark       = Drawing.Color.FromRGB(120.0/255.0, 120.0/255.0, 120.0/255.0);
			this.colorControlDarkDark   = Drawing.Color.FromRGB(100.0/255.0, 100.0/255.0, 100.0/255.0);
			this.colorCaption           = Drawing.Color.FromRGB(255.0/255.0, 210.0/255.0,   0.0/255.0);
			this.colorCaptionText       = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorInfo              = Drawing.Color.FromRGB(250.0/255.0, 250.0/255.0, 250.0/255.0);
			this.colorError             = Drawing.Color.FromRGB(255.0/255.0, 177.0/255.0, 177.0/255.0);
		}
		

		// Dessine le fond d'une fen�tre.
		public override void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetState state)
		{
			graphics.AddFilledRectangle(paintRect);
			graphics.RenderSolid(this.colorWindow);
		}

		// Dessine une ic�ne simple (dans un bouton d'ascenseur par exemple).
		public override void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state,
							   GlyphShape type,
							   PaintTextStyle style)
		{
			Drawing.Color color = this.colorBlack;

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRGB(0.7, 0.0, 0.0);  // rouge fonc�
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRGB(0.0, 0.6, 0.0);  // vert fonc�
			}
			else
			{
				color = this.colorFrontDisabled;
			}

			this.PaintGlyph(graphics, rect, state, color, type, style);
		}
		
		// Dessine une ic�ne simple (dans un bouton d'ascenseur par exemple).
		public override void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state,
							   Drawing.Color color,
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
				graphics.RenderSolid(this.colorControlDark);
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

			double zoom = 1.0;
			if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
			{
				zoom = 1.2;
			}
			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			Drawing.Path path = new Drawing.Path();
			double spikeShift = 0.20*zoom;
			double baseShiftH = 0.25*zoom;
			double baseShiftV = 0.15*zoom;
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

				case GlyphShape.TabLeft:
					path.MoveTo(center.X-rect.Width*0.10*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.00*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.00*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.10*zoom, center.Y-rect.Height*0.15*zoom);
					break;

				case GlyphShape.TabRight:
					path.MoveTo(center.X+rect.Width*0.00*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.10*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.10*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.00*zoom, center.Y-rect.Height*0.05*zoom);
					break;

				case GlyphShape.TabCenter:
					path.MoveTo(center.X-rect.Width*0.05*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.05*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.05*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X-rect.Width*0.05*zoom, center.Y-rect.Height*0.05*zoom);
					break;

				case GlyphShape.TabDecimal:
					path.MoveTo(center.X-rect.Width*0.05*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.05*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.05*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.20*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X-rect.Width*0.05*zoom, center.Y-rect.Height*0.05*zoom);
					path.Close();
					path.MoveTo(center.X+rect.Width*0.10*zoom, center.Y+rect.Height*0.10*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y+rect.Height*0.10*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y+rect.Height*0.00*zoom);
					path.LineTo(center.X+rect.Width*0.10*zoom, center.Y+rect.Height*0.00*zoom);
					break;

				case GlyphShape.TabIndent:
					path.MoveTo(center.X-rect.Width*0.10*zoom, center.Y+rect.Height*0.20*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.00*zoom);
					path.LineTo(center.X-rect.Width*0.10*zoom, center.Y-rect.Height*0.20*zoom);
					break;

				case GlyphShape.Plus:
					path.MoveTo(center.X-rect.Width*0.29*zoom, center.Y+rect.Height*0.07*zoom);
					path.LineTo(center.X-rect.Width*0.07*zoom, center.Y+rect.Height*0.07*zoom);
					path.LineTo(center.X-rect.Width*0.07*zoom, center.Y+rect.Height*0.29*zoom);
					path.LineTo(center.X+rect.Width*0.07*zoom, center.Y+rect.Height*0.29*zoom);
					path.LineTo(center.X+rect.Width*0.07*zoom, center.Y+rect.Height*0.07*zoom);
					path.LineTo(center.X+rect.Width*0.29*zoom, center.Y+rect.Height*0.07*zoom);
					path.LineTo(center.X+rect.Width*0.29*zoom, center.Y-rect.Height*0.07*zoom);
					path.LineTo(center.X+rect.Width*0.07*zoom, center.Y-rect.Height*0.07*zoom);
					path.LineTo(center.X+rect.Width*0.07*zoom, center.Y-rect.Height*0.29*zoom);
					path.LineTo(center.X-rect.Width*0.07*zoom, center.Y-rect.Height*0.29*zoom);
					path.LineTo(center.X-rect.Width*0.07*zoom, center.Y-rect.Height*0.07*zoom);
					path.LineTo(center.X-rect.Width*0.29*zoom, center.Y-rect.Height*0.07*zoom);
					break;

				case GlyphShape.Minus:
					path.MoveTo(center.X-rect.Width*0.29*zoom, center.Y+rect.Height*0.07*zoom);
					path.LineTo(center.X+rect.Width*0.29*zoom, center.Y+rect.Height*0.07*zoom);
					path.LineTo(center.X+rect.Width*0.29*zoom, center.Y-rect.Height*0.07*zoom);
					path.LineTo(center.X-rect.Width*0.29*zoom, center.Y-rect.Height*0.07*zoom);
					break;
			}
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			path.Dispose();
			graphics.RenderSolid(color);
		}

		// Dessine un bouton � cocher sans texte.
		public override void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			graphics.Align(ref rect);
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
				 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
			{
				graphics.RenderSolid(this.colorCaption);
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(this.colorBackDisabled);
			}

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
					graphics.RenderSolid(this.colorFrontDisabled);
				}
			}

			if ( (state&WidgetState.ActiveMaybe) != 0 )  // 3�me �tat ?
			{
				rect.Deflate(3);
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorFrontDisabled);
				}
			}
		}

		// Dessine un bouton radio sans texte.
		public override void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			graphics.Align(ref rect);

			if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
				 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
			{
				this.PaintCircle(graphics, rect, this.colorCaption);
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintCircle(graphics, rect, this.colorControlLightLight);
			}
			else
			{
				this.PaintCircle(graphics, rect, this.colorBackDisabled);
			}

			if ( (state&WidgetState.ActiveYes) != 0 )  // coch� ?
			{
				Drawing.Rectangle rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorBlack);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorFrontDisabled);
				}
			}
		}

		public override void PaintIcon(Drawing.Graphics graphics,
							  Drawing.Rectangle rect,
							  Widgets.WidgetState state,
							  string icon)
		{
		}

		// Dessine le fond d'un bouton rectangulaire.
		public override void PaintButtonBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetState state,
										  Widgets.Direction dir,
										  Widgets.ButtonStyle style)
		{
			Drawing.Rectangle rFocus = rect;
			rFocus.Deflate(1.0);
			double radFocus = 0;

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				Drawing.Path path = this.PathRoundRectangle(rect, 0);
			
				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					graphics.RenderSolid(this.colorControl);
				}
				else if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( style == ButtonStyle.DefaultAccept )
					{
						graphics.RenderSolid(this.colorControlLight);
					}
					else
					{
						graphics.RenderSolid(this.colorControlLightLight);
					}
				}
				else
				{
					graphics.RenderSolid(this.colorBackDisabled);
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
				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}
				else
				{
					graphics.RenderSolid(this.colorBackDisabled);
				}
				rFocus.Inflate(1);
				radFocus = -1;
			}
			else if ( style == ButtonStyle.Slider )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else if ( (state&WidgetState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}
				else
				{
					graphics.RenderSolid(this.colorBackDisabled);
				}
				rFocus.Inflate(1);
				radFocus = -1;
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
				}
				rFocus.Inflate(1);
				rFocus.Right ++;
				radFocus = -1;
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				rect.Right += 1;

				if ( (state&WidgetState.Entered)   != 0 ||  // bouton survol� ?
					 (state&WidgetState.Engaged)   != 0 ||  // bouton press� ?
					 (state&WidgetState.ActiveYes) != 0 )   // bouton activ� ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorControlLight);
				}
				rFocus.Inflate(1);
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
				graphics.Rasterizer.AddOutline(pInside, 2);
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		// Dessine le texte d'un bouton.
		public override void PaintButtonTextLayout(Drawing.Graphics graphics,
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
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, PaintTextStyle.Button, Drawing.Color.Empty);
		}

		public override void PaintButtonForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Widgets.WidgetState state,
										  Widgets.Direction dir,
										  Widgets.ButtonStyle style)
		{
		}

		// Dessine le fond d'une ligne �ditable.
		public override void PaintTextFieldBackground(Drawing.Graphics graphics,
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
					if ( (state&WidgetState.Error) != 0 )
					{
						graphics.RenderSolid(this.colorError);
					}
					else
					{
						graphics.RenderSolid(readOnly ? this.colorControlReadOnly : this.colorControlField);
					}
				}
				else
				{
					graphics.RenderSolid(this.colorBackDisabled);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorControlField);
				}
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControlField);
			}
		}

		public override void PaintTextFieldForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 Widgets.WidgetState state,
											 Widgets.TextFieldStyle style,
											 bool readOnly)
		{
		}

		// Dessine le fond d'un ascenseur.
		public override void PaintScrollerBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle thumbRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
			graphics.AddFilledRectangle(frameRect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(this.colorBackDisabled);
			}

			if ( !tabRect.IsSurfaceZero && (state&WidgetState.Engaged) != 0 )
			{
				if ( dir == Direction.Up )
				{
					tabRect.Left  += 2;
					tabRect.Right -= 2;
				}
				if ( dir == Direction.Left )
				{
					tabRect.Bottom += 2;
					tabRect.Top    -= 2;
				}
				graphics.AddFilledRectangle(tabRect);
				graphics.RenderSolid(this.colorCaption);
			}
		}

		// Dessine la cabine d'un ascenseur.
		public override void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetState state,
										Widgets.Direction dir)
		{
			if ( dir == Direction.Up )
			{
				thumbRect.Left  += 2;
				thumbRect.Right -= 2;
			}
			if ( dir == Direction.Left )
			{
				thumbRect.Bottom += 2;
				thumbRect.Top    -= 2;
			}
			graphics.AddFilledRectangle(thumbRect);
			graphics.RenderSolid(this.colorControlDark);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				switch ( dir )
				{
					case Direction.Up:
					case Direction.Down:
						if ( thumbRect.Width >= 10 && thumbRect.Height >= 20 )
						{
							Drawing.Point p1 = new Drawing.Point(thumbRect.Left+3, (thumbRect.Bottom+thumbRect.Top)/2);
							Drawing.Point p2 = new Drawing.Point(thumbRect.Right-3, (thumbRect.Bottom+thumbRect.Top)/2);
							graphics.Align(ref p1);
							graphics.Align(ref p2);
							p1.Y -= 5.5;
							p2.Y -= 5.5;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(p1, p2);
								p1.Y += 3;
								p2.Y += 3;
							}
							graphics.RenderSolid(this.colorControlLightLight);
						}
						break;

					case Direction.Left:
					case Direction.Right:
						if ( thumbRect.Height >= 10 && thumbRect.Width >= 20 )
						{
							Drawing.Point p1 = new Drawing.Point((thumbRect.Left+thumbRect.Right)/2, thumbRect.Bottom+3);
							Drawing.Point p2 = new Drawing.Point((thumbRect.Left+thumbRect.Right)/2, thumbRect.Top-3);
							graphics.Align(ref p1);
							graphics.Align(ref p2);
							p1.X -= 5.5;
							p2.X -= 5.5;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(p1, p2);
								p1.X += 3;
								p2.X += 3;
							}
							graphics.RenderSolid(this.colorControlLightLight);
						}
						break;
				}
			}
		}

		public override void PaintScrollerForeground(Drawing.Graphics graphics,
											Drawing.Rectangle thumbRect,
											Drawing.Rectangle tabRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
		}

		// Dessine le fond d'un potentiom�tre lin�aire.
		public override void PaintSliderBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle frameRect,
										  Drawing.Rectangle thumbRect,
										  Drawing.Rectangle tabRect,
										  Widgets.WidgetState state,
										  Widgets.Direction dir)
		{
			bool enabled = ( (state&WidgetState.Enabled) != 0 );

			if ( dir == Widgets.Direction.Left )
			{
				Drawing.Point p1 = new Drawing.Point(frameRect.Left +frameRect.Height*1.2, frameRect.Center.Y);
				Drawing.Point p2 = new Drawing.Point(frameRect.Right-frameRect.Height*1.2, frameRect.Center.Y);
				graphics.Align(ref p1);
				graphics.Align(ref p2);

				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X-0.5, p2.Y+0.5);
				graphics.AddLine(p1.X+0.5, p1.Y-0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(enabled ? this.colorControlDark : this.colorControlLight);

				if ( !tabRect.IsSurfaceZero && (state&WidgetState.Engaged) != 0 )
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
				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X+0.5, p2.Y-0.5);
				graphics.RenderSolid(enabled ? this.colorControlDark : this.colorControlLight);

				if ( !tabRect.IsSurfaceZero && (state&WidgetState.Engaged) != 0 )
				{
					graphics.AddLine(p1.X-0.5, tabRect.Bottom, p2.X-0.5, tabRect.Top);
					graphics.AddLine(p1.X+0.5, tabRect.Bottom, p2.X+0.5, tabRect.Top);
					graphics.RenderSolid(this.colorCaption);
				}
			}
		}

		// Dessine la cabine d'un potentiom�tre lin�aire.
		public override void PaintSliderHandle(Drawing.Graphics graphics,
									  Drawing.Rectangle thumbRect,
									  Drawing.Rectangle tabRect,
									  Widgets.WidgetState state,
									  Widgets.Direction dir)
		{
			if ( dir == Widgets.Direction.Left )
			{
				graphics.AddFilledRectangle(thumbRect);

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}

				graphics.AddLine(thumbRect.Center.X, thumbRect.Bottom+2, thumbRect.Center.X, thumbRect.Top-2);
				graphics.RenderSolid(this.colorControlDark);
			}
			else
			{
				graphics.AddFilledRectangle(thumbRect);

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
				{
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}

				graphics.AddLine(thumbRect.Left+2, thumbRect.Center.Y, thumbRect.Right-2, thumbRect.Center.Y);
				graphics.RenderSolid(this.colorControlDark);
			}
		}

		public override void PaintSliderForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle thumbRect,
										  Drawing.Rectangle tabRect,
										  Widgets.WidgetState state,
										  Widgets.Direction dir)
		{
		}

		// Dessine le cadre d'un GroupBox.
		public override void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetState state)
		{
			frameRect.IntersectsWith(titleRect);
			graphics.AddFilledRectangle(frameRect);
			graphics.RenderSolid(this.colorControlBar);
		}

		public override void PaintSepLine(Drawing.Graphics graphics,
								 Drawing.Rectangle frameRect,
								 Drawing.Rectangle titleRect,
								 Widgets.WidgetState state,
								 Widgets.Direction dir)
		{
		}

		public override void PaintFrameTitleBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
											  Widgets.WidgetState state,
											  Widgets.Direction dir)
		{
		}

		public override void PaintFrameTitleForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  Drawing.Rectangle titleRect,
											  Widgets.WidgetState state,
											  Widgets.Direction dir)
		{
		}

		public override void PaintFrameBody(Drawing.Graphics graphics,
								   Drawing.Rectangle rect,
								   Widgets.WidgetState state,
								   Widgets.Direction dir)
		{
		}

		// Dessine toute la bande sous les onglets.
		public override void PaintTabBand(Drawing.Graphics graphics,
								 Drawing.Rectangle rect,
								 Widgets.WidgetState state,
								 Widgets.Direction dir)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);
		}

		// Dessine la zone principale sous les onglets.
		public override void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetState state,
								  Widgets.Direction dir)
		{
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlBar);
			}
			else
			{
				graphics.RenderSolid(this.colorBackDisabled);
			}
		}

		// Dessine l'onglet devant les autres.
		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
			titleRect.Bottom += 1;

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			graphics.Rasterizer.AddSurface(pTitle);
			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				graphics.RenderSolid(this.colorCaption);
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlBar);
			}
			else
			{
				graphics.RenderSolid(this.colorBackDisabled);
			}
		}

		public override void PaintTabAboveForeground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
		}

		// Dessine un onglet derri�re (non s�lectionn�).
		public override void PaintTabSunkenBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetState state,
											 Widgets.Direction dir)
		{
			titleRect.Bottom -= 1;
			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			graphics.Rasterizer.AddSurface(pTitle);
			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				graphics.RenderSolid(this.colorCaption);
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(this.colorBackDisabled);
			}
		}

		public override void PaintTabSunkenForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle frameRect,
											 Drawing.Rectangle titleRect,
											 Widgets.WidgetState state,
											 Widgets.Direction dir)
		{
		}

		// Dessine le fond d'un tableau.
		public override void PaintArrayBackground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetState state)
		{
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
			{
				graphics.RenderSolid(this.colorControlBar);
			}
			else
			{
				graphics.RenderSolid(this.colorBackDisabled);
			}
		}

		public override void PaintArrayForeground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetState state)
		{
		}

		// Dessine le fond d'une cellule.
		public override void PaintCellBackground(Drawing.Graphics graphics,
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

		// Dessine le fond d'un bouton d'en-t�te de tableau.
		public override void PaintHeaderBackground(Drawing.Graphics graphics,
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
			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				graphics.RenderSolid(this.colorCaption);
			}
			else if ( (state&WidgetState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(this.colorBackDisabled);
			}
		}

		public override void PaintHeaderForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state,
										  Direction dir)
		{
		}

		// Dessine le fond d'une barre d'outil.
		public override void PaintToolBackground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction dir)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlBar);
		}

		public override void PaintToolForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction dir)
		{
		}

		// Dessine le fond d'un menu.
		public override void PaintMenuBackground(Drawing.Graphics graphics,
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
				band.Deflate(2);
				graphics.AddFilledRectangle(band);
				graphics.RenderSolid(this.colorControlBar);
			}
		}

		public override void PaintMenuForeground(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										WidgetState state,
										Direction dir,
										Drawing.Rectangle parentRect,
										double iconWidth)
		{
		}

		// Dessine le fond d'une case de menu.
		public override void PaintMenuItemBackground(Drawing.Graphics graphics,
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
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaption);
					}
					if ( itemType == MenuItemType.Parent )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorControlLightLight);
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
			else
			{
				if ( itemType != MenuItemType.Deselect )
				{
					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.ColorBorder);
				}
			}
		}

		// Dessine le texte d'un menu.
		public override void PaintMenuItemTextLayout(Drawing.Graphics graphics,
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
		public override void PaintMenuItemForeground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetState state,
											Direction dir,
											MenuType type,
											MenuItemType itemType)
		{
		}

		// Dessine un s�parateur horizontal ou vertical.
		public override void PaintSeparatorBackground(Drawing.Graphics graphics,
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

			graphics.RenderSolid(this.colorControlBar);
		}

		public override void PaintSeparatorForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 WidgetState state,
											 Direction dir,
											 bool optional)
		{
		}

		// Dessine un bouton s�parateur de panneaux.
		public override void PaintPaneButtonBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state,
											  Direction dir)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlBar);
		}

		public override void PaintPaneButtonForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state,
											  Direction dir)
		{
		}

		// Dessine une ligne de statuts.
		public override void PaintStatusBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlBar);
		}

		public override void PaintStatusForeground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state)
		{
		}

		// Dessine une case de statuts.
		public override void PaintStatusItemBackground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state)
		{
			rect.Width -= 1;
			rect.Deflate(1);
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorWindow);
		}

		public override void PaintStatusItemForeground(Drawing.Graphics graphics,
											  Drawing.Rectangle rect,
											  WidgetState state)
		{
		}

		// Dessine le bouton pour un ruban.
		public override void PaintRibbonButtonBackground(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												WidgetState state)
		{
			this.PaintButtonBackground(graphics, rect, state, Widgets.Direction.None, ButtonStyle.ToolItem);
		}

		// Dessine le bouton pour un ruban.
		public override void PaintRibbonButtonForeground(Drawing.Graphics graphics,
												Drawing.Rectangle rect,
												WidgetState state)
		{
		}

		// Dessine le texte d'un bouton du ruban.
		public override void PaintRibbonButtonTextLayout(Drawing.Graphics graphics,
												Drawing.Point pos,
												TextLayout text,
												WidgetState state)
		{
			if ( text == null )  return;
			state &= ~WidgetState.Focused;
			PaintTextStyle style = PaintTextStyle.HMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, style, Drawing.Color.Empty);
		}

		// Dessine la bande principale d'un ruban.
		public override void PaintRibbonTabBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 double titleHeight,
											 WidgetState state)
		{
			rect.Top -= titleHeight;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControlBar);
		}

		// Dessine la bande principale d'un ruban.
		public override void PaintRibbonTabForeground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 double titleHeight,
											 WidgetState state)
		{
		}

		// Dessine une section d'un ruban.
		public override void PaintRibbonSectionBackground(Drawing.Graphics graphics,
												 Drawing.Rectangle rect,
												 double titleHeight,
												 WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
			graphics.RenderSolid(this.ColorBorder);
		}

		// Dessine une section d'un ruban.
		public override void PaintRibbonSectionForeground(Drawing.Graphics graphics,
												 Drawing.Rectangle rect,
												 double titleHeight,
												 WidgetState state)
		{
		}

		// Dessine le texte du titre d'une section d'un ruban.
		public override void PaintRibbonSectionTextLayout(Drawing.Graphics graphics,
												 Drawing.Point pos,
												 TextLayout text,
												 WidgetState state)
		{
			if ( text == null )  return;

			Drawing.TextStyle.DefineDefaultColor(this.colorBlack);
			text.Alignment = Drawing.ContentAlignment.MiddleLeft;
			text.Paint(pos, graphics, Drawing.Rectangle.Infinite, Drawing.Color.FromBrightness(0), Drawing.GlyphPaintStyle.Normal);
		}

		// Dessine un tag.
		public override void PaintTagBackground(Drawing.Graphics graphics,
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
				graphics.RenderSolid(this.colorBackDisabled);
			}
			else if ( color.IsEmpty )
			{
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				graphics.RenderSolid(color);
			}
		}

		public override void PaintTagForeground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetState state,
									   Drawing.Color color,
									   Direction dir)
		{
		}

		// Dessine le fond d'une bulle d'aide.
		public override void PaintTooltipBackground(Drawing.Graphics graphics,
										   Drawing.Rectangle rect)
		{
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorInfo);
		}

		// Dessine le texte d'une bulle d'aide.
		public override void PaintTooltipTextLayout(Drawing.Graphics graphics,
										   Drawing.Point pos,
										   TextLayout text)
		{
			text.Paint(pos, graphics);
		}


		// Dessine le rectangle pour indiquer le focus.
		public override void PaintFocusBox(Drawing.Graphics graphics,
								  Drawing.Rectangle rect)
		{
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDark);
		}

		// Dessine le curseur du texte.
		public override void PaintTextCursor(Drawing.Graphics graphics,
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
		
		// Dessine les zones rectanglaires correspondant aux caract�res s�lectionn�s.
		public override void PaintTextSelectionBackground(Drawing.Graphics graphics,
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

		public override void PaintTextSelectionForeground(Drawing.Graphics graphics,
												 TextLayout.SelectedArea[] areas,
												 WidgetState state)
		{
		}

		// Dessine le texte d'un widget.
		public override void PaintGeneralTextLayout(Drawing.Graphics graphics,
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
					text.Paint(pos, graphics, clipRect, this.colorCaptionText, Drawing.GlyphPaintStyle.Selected);
				}
				else if ( (state&WidgetState.Entered) != 0 )
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
				if ( style == PaintTextStyle.HMenu ||
					 style == PaintTextStyle.VMenu )
				{
					text.Paint(pos, graphics, clipRect, this.colorControlField, Drawing.GlyphPaintStyle.Disabled);
				}
				else
				{
					text.Paint(pos, graphics, clipRect, this.colorFrontDisabled, Drawing.GlyphPaintStyle.Disabled);
				}
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


		// Cr�e le chemin d'un rectangle � coins arrondis.
		protected Drawing.Path PathRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			rect.Deflate(0.5);
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius ==  0 )  radius = System.Math.Min(dx, dy)/4;
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

		// Cr�e le chemin d'un rectangle � coins arrondis en forme de "u" invers�.
		protected Drawing.Path PathTopRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			rect.Deflate(0.5);
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
			graphics.AddFilledCircle(c.X, c.Y, rx, ry);
			graphics.RenderSolid(color);
		}


		public override void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Normal )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				color = Drawing.Color.FromBrightness(intensity);
				color.A = alpha;
			}

			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.3, 1.0);  // augmente l'intensit�
				color = Drawing.Color.FromBrightness(intensity);
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
			get { return this.colorControlLight; }
		}

		public override Drawing.Color ColorTextBackground
		{
			get { return this.colorControlField; }
		}

		public override Drawing.Color ColorText(WidgetState state)
		{
			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
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
				return this.colorFrontDisabled;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return Drawing.Color.Empty;
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.colorControlLight;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryToolShapeBounds { get { return new Drawing.Margins(0,1,0,0); } }
		public override Drawing.Margins GeometryButtonShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRibbonShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryTextFieldShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryListShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override double GeometryComboRightMargin { get { return 0; } }
		public override double GeometryComboBottomMargin { get { return 0; } }
		public override double GeometryComboTopMargin { get { return 0; } }
		public override double GeometryUpDownWidthFactor { get { return 0.6; } }
		public override double GeometryUpDownRightMargin { get { return 0; } }
		public override double GeometryUpDownBottomMargin { get { return 0; } }
		public override double GeometryUpDownTopMargin { get { return 0; } }
		public override double GeometryScrollerRightMargin { get { return 0; } }
		public override double GeometryScrollerBottomMargin { get { return 0; } }
		public override double GeometryScrollerTopMargin { get { return 0; } }
		public override double GeometryScrollListXMargin { get { return 1; } }
		public override double GeometryScrollListYMargin { get { return 1; } }
		public override double GeometrySliderLeftMargin { get { return -1; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return -1; } }


		protected Drawing.Color		colorControlField;
		protected Drawing.Color		colorControlReadOnly;
		protected Drawing.Color		colorControlBar;
		protected Drawing.Color		colorBackDisabled;
		protected Drawing.Color		colorFrontDisabled;
		protected Drawing.Color		colorError;
		protected Drawing.Color		colorWindow;
	}
}
