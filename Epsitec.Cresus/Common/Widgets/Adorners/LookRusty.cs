namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookRusty implémente le décorateur vieux et rouillé.
	/// </summary>
	public class LookRusty : AbstractAdorner
	{
		public LookRusty()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookRusty.png", typeof (IAdorner));
			this.metal3 = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "metal3.png", typeof (IAdorner));
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs en fonction des réglages de Windows.
			this.colorBlack             = Drawing.Color.FromRgb(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorWhite             = Drawing.Color.FromRgb(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorControl           = Drawing.Color.FromRgb( 53.0/255.0, 146.0/255.0, 255.0/255.0);
			this.colorCaption           = Drawing.Color.FromRgb(187.0/255.0, 119.0/255.0,  36.0/255.0);
			this.colorCaptionNF         = Drawing.Color.FromRgb(240.0/255.0, 204.0/255.0, 134.0/255.0);
			this.colorCaptionText       = Drawing.Color.FromRgb(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorInfo              = Drawing.Color.FromRgb(213.0/255.0, 233.0/255.0, 255.0/255.0);
			this.colorBorder            = Drawing.Color.FromAlphaRgb(0.6, 31.0/255.0,   7.0/255.0,   8.0/255.0);
			this.colorDisabled          = Drawing.Color.FromRgb(140.0/255.0, 140.0/255.0, 140.0/255.0);
			this.colorError             = Drawing.Color.FromHexa ("ffb1b1");  // rouge pâle
			this.colorUndefinedLanguage = Drawing.Color.FromHexa ("b1e3ff");  // bleu pâle
			this.colorTextBackground    = Drawing.Color.FromRgb(63.0/255.0, 45.0/255.0, 15.0/255.0);
			this.colorThreeState        = Drawing.Color.FromRgb(206.0/255.0, 182.0/255.0, 154.0/255.0);
			this.colorActivableIcon     = Drawing.Color.FromRgb( 96.0/255.0,  70.0/255.0,  27.0/255.0);
			this.colorWindow            = Drawing.Color.FromRgb( 79.0/255.0,  74.0/255.0,  66.0/255.0);
		}
		

		public override void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetPaintState state)
		{
			//	Dessine le fond d'une fenêtre.
			this.PaintBackground(graphics, windowRect, paintRect);
		}

		public override void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state,
							   GlyphShape type,
							   PaintTextStyle style)
		{
			//	Dessine une icône simple (dans un bouton d'ascenseur par exemple).
			Drawing.Color color = Drawing.Color.FromBrightness(1.0);

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRgb(0.5, 0.0, 0.0);  // rouge foncé
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRgb(0.0, 0.3, 0.0);  // vert foncé
			}
			else
			{
				color = this.colorDisabled;
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
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
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
			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
			{
				zoom = 1.3;
			}
			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			Drawing.Path path = new Drawing.Path();
			double spikeShift = 0.20*zoom;
			double baseShiftH = 0.20*zoom;
			double baseShiftV = 0.15*zoom;
			switch ( type )
			{
				case GlyphShape.Lock:
					AbstractAdorner.DrawGlyphShapeLook (path, rect, center);
					break;

				case GlyphShape.ArrowUp:
				case GlyphShape.TriangleUp:
					path.MoveTo(center.X, center.Y+rect.Height*spikeShift);
					path.LineTo(center.X-rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					path.LineTo(center.X+rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					break;

				case GlyphShape.ArrowDown:
				case GlyphShape.TriangleDown:
					path.MoveTo(center.X, center.Y-rect.Height*spikeShift);
					path.LineTo(center.X-rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					path.LineTo(center.X+rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					break;

				case GlyphShape.ArrowRight:
				case GlyphShape.TriangleRight:
					path.MoveTo(center.X+rect.Width*spikeShift, center.Y);
					path.LineTo(center.X-rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo(center.X-rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
					break;

				case GlyphShape.ArrowLeft:
				case GlyphShape.TriangleLeft:
					path.MoveTo(center.X-rect.Width*spikeShift, center.Y);
					path.LineTo(center.X+rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo(center.X+rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
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

		public override void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton à cocher sans texte.
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
				{
					this.PaintImageButton(graphics, rect, 44);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 45);
				}

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else
			{
				this.PaintImageButton(graphics, rect, 47);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 ||  // coché ?
				 (state&WidgetPaintState.Engaged) != 0   )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					rect.Inflate(rect.Height*0.1);
				}
				Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
				Drawing.Path path = new Drawing.Path();
				path.MoveTo(center.X+rect.Width*0.00, center.Y+rect.Height*0.10);
				path.LineTo(center.X+rect.Width*0.30, center.Y+rect.Height*0.70);
				path.LineTo(center.X+rect.Width*0.50, center.Y+rect.Height*0.70);
				path.LineTo(center.X+rect.Width*0.00, center.Y-rect.Height*0.30);
				path.LineTo(center.X-rect.Width*0.35, center.Y+rect.Height*0.20);
				path.LineTo(center.X-rect.Width*0.15, center.Y+rect.Height*0.20);
				path.Close();
				graphics.Rasterizer.AddSurface(path);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}

			if ( (state&WidgetPaintState.ActiveMaybe) != 0 )  // 3ème état ?
			{
				rect.Deflate(3);
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}
		}

		public override void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton radio sans texte.
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
				{
					this.PaintImageButton(graphics, rect, 40);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 41);
				}

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 42);
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 42);
				}
			}
			else
			{
				this.PaintImageButton(graphics, rect, 43);
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
			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel ||
				 style == ButtonStyle.DefaultAcceptAndCancel )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					Drawing.Rectangle shadow = rect;
					shadow.Left   -= 2;
					shadow.Right  += 2;
					shadow.Bottom -= 5;
					this.PaintImageButton(graphics, shadow, 64);

					if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
					{
						this.PaintImageButton(graphics, rect, 0);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 2);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 4);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 6);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 6);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					rect.Deflate(3.5);
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.Scroller )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 42);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 42);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 40);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 43);
				}
			}
			else if ( style == ButtonStyle.Slider )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 42);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 42);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 40);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 43);
				}
			}
			else if ( style == ButtonStyle.Combo       ||
					  style == ButtonStyle.ExListRight )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 44);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 47);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else if ( style == ButtonStyle.ExListMiddle ||
					  style == ButtonStyle.ExListLeft   )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 44);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 47);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else if ( style == ButtonStyle.UpDown )
			{
				if ( (state&WidgetPaintState.Enabled) == 0 )
				{
					graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
					graphics.RenderSolid(this.ColorBorder);
				}

				if ( dir == Direction.Up )
				{
					if ( (state&WidgetPaintState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 44);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 47);
					}

					if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
						 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 46);
					}
				}
				if ( dir == Direction.Down )
				{
					if ( (state&WidgetPaintState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 44);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 47);
					}

					if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
						 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 46);
					}
				}
			}
			else if ( style == ButtonStyle.Icon )
			{
				bool large = (rect.Width > rect.Height*1.5);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, large?2:40);
				}
				else
				{
					this.PaintImageButton(graphics, rect, large?4:43);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, large?6:42);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					rect.Deflate(3.5);
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionNF);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionNF);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(2);
					}
					else
					{
						rect.Deflate(3);
					}
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.ComboItem )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					if ((state&WidgetPaintState.InheritedEnter) == 0)
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaptionNF);
					}

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaptionNF);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				else if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(2);
					}
					else
					{
						rect.Deflate(3);
					}
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				if ( AbstractAdorner.IsThreeState2(state) )
				{
					rect.Top += 2;
				}

				rect.Right += 1;
				Drawing.Path path;

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorCaptionNF);

					rect.Deflate(0.5);
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorCaptionNF);

					rect.Deflate(0.5);
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorCaption);

					rect.Deflate(0.5);
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorThreeState);

					rect.Deflate(0.5);
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(this.colorActivableIcon);

					rect.Deflate(0.5);
					path = AbstractAdorner.PathThreeState2Frame(rect, state);
					graphics.Rasterizer.AddOutline(path, 1);
					graphics.RenderSolid(this.colorBorder);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(2);
					}
					else
					{
						rect.Deflate(3);
					}
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.HeaderSlider )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 44);
				}
			}
			else if ( style == ButtonStyle.Confirmation )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 32);
				}
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 34);
				}
			}
			else if ( style == ButtonStyle.ListItem )
			{
				this.PaintImageButton(graphics, rect, 0);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 0);
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

			if ( AbstractAdorner.IsThreeState2(state) )
			{
				pos.Y ++;
			}
			if ( style == ButtonStyle.Tab )
			{
				state |=  WidgetPaintState.Selected;
				pos.Y -= 1.0;
			}
			else
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
				 style == TextFieldStyle.Combo  )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, 32);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 34);
				}
			}
			else if ( style == TextFieldStyle.UpDown )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, 32);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 34);
				}
			}
			else if ( style == TextFieldStyle.Multiline  )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, 32);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 34);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorWindow);
				}
				else
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorWindow);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.ColorBorder);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorDisabled);
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
				if ( dir == Direction.Up )
				{
					this.PaintImageButton(graphics, frameRect, 52);
				}
				if ( dir == Direction.Left )
				{
					this.PaintImageButton(graphics, frameRect, 48);
				}
			}
			else
			{
				if ( dir == Direction.Up )
				{
					this.PaintImageButton(graphics, frameRect, 53);
				}
				if ( dir == Direction.Left )
				{
					this.PaintImageButton(graphics, frameRect, 50);
				}
			}
		}

		public override void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetPaintState state,
										Widgets.Direction dir)
		{
			//	Dessine la cabine d'un ascenseur.
			if ( dir == Direction.Up )
			{
				thumbRect.Left  += 1.0;
				thumbRect.Right -= 2.0;

				bool little = (thumbRect.Height < thumbRect.Width);

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?44:54);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?1000:53);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?46:55);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?46:55);
				}
			}
			if ( dir == Direction.Left )
			{
				thumbRect.Top    -= 1.0;
				thumbRect.Bottom += 2.0;

				bool little = (thumbRect.Width < thumbRect.Height);

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?44:56);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?1000:50);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?46:58);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?46:58);
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
				double m = frameRect.Height*0.2;
				double p = frameRect.Center.Y;
				frameRect = sliderRect;
				frameRect.Left   += m;
				frameRect.Right  -= m;
				frameRect.Bottom = p-2;
				frameRect.Top    = p+2;
				this.PaintImageButton(graphics, frameRect, 48);

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					tabRect.Bottom = p-2;
					tabRect.Top    = p+2;
					this.PaintImageButton(graphics, tabRect, 58);
				}
			}
			else
			{
				double m = frameRect.Width*0.2;
				double p = frameRect.Center.X;
				frameRect = sliderRect;
				frameRect.Bottom += m;
				frameRect.Top    -= m;
				frameRect.Left   = p-2;
				frameRect.Right  = p+2;
				this.PaintImageButton(graphics, frameRect, 52);

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					tabRect.Left  = p-2;
					tabRect.Right = p+2;
					this.PaintImageButton(graphics, tabRect, 55);
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
				thumbRect.Inflate(1, 0);

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, 54+1000);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 53+1000);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, 55+1000);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, 55+1000);
				}
			}
			else
			{
				thumbRect.Inflate(0, 1);

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, 56+1000);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 50+1000);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, 58+1000);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, 58+1000);
				}
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
			this.PaintImageButton(graphics, rect, 48);

			Drawing.Rectangle rInside = rect;
			if (style == ProgressIndicatorStyle.UnknownDuration)
			{
				rInside.Deflate(1);
				rInside.Left = (rInside.Width-rInside.Height)*progress;
				rInside.Width = rInside.Height;
				this.PaintImageButton(graphics, rInside, 40);
			}
			else
			{
				rInside.Deflate(3);
				if (progress != 0)
				{
					rInside.Width *= progress;
					this.PaintImageButton(graphics, rInside, 56);
				}
			}
		}

		public override void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetPaintState state)
		{
			//	Dessine le cadre d'un GroupBox.
			frameRect.Top -= titleRect.Height/2;
			double radius = this.RetRadiusFrame(frameRect);
			Drawing.Path path;

			path = this.PathRoundRectangle(frameRect, radius);
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.05, 0,0,0));

			path = this.PathRoundRectangle(frameRect, radius);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.ColorBorder);
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
		}

		public override void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetPaintState state,
								  Widgets.Direction dir)
		{
			//	Dessine la zone principale sous les onglets.
			this.PaintBackground(graphics, rect, rect);

			Drawing.Rectangle top = rect;
			Drawing.Rectangle full = rect;

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorBorder);

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				top.Bottom = top.Top-10;
				top.Deflate(1, 0);
				this.PaintImageButton(graphics, top, 67);
			}
		}

		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine l'onglet devant les autres.
			titleRect.Bottom += 2.0;
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 14);
			}
			else
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 8);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 12);
				}
			}
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
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 14);
			}
			else
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 10);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 12);
				}
			}
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
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ((state&WidgetPaintState.Focused) != 0)
			{
				graphics.RenderSolid(this.colorCaption);
			}
			else
			{
				graphics.RenderSolid(this.ColorBorder);
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
				AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorWhite);
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
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 8);
				}
				else if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 10);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 12);
				}
			}

			if ( dir == Direction.Left )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 16);
				}
				else if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 17);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 18);
				}
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.4, 1.0, 1.0, 1.0));

			if ( dir == Direction.Up )
			{
				graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
				graphics.RenderSolid(this.ColorBorder);
			}

			if ( dir == Direction.Left )
			{
				graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.RenderSolid(this.ColorBorder);
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
			rect.Inflate(-this.GeometryMenuShadow);
			this.PaintBackground(graphics, rect, rect);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Width = iconWidth;
				band.Top -= 1;
				band.Bottom += 1;
				graphics.AddFilledRectangle(band);
				graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.4, 1.0, 1.0, 1.0));
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorBorder);
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
					if ( itemType != MenuItemState.Default )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaption);
					}
				}

				if ( type == MenuOrientation.Vertical )
				{
					if ( itemType != MenuItemState.Default )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaption);
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
			state &= ~WidgetPaintState.Focused;
			if ( itemType == MenuItemState.Default )
			{
				state &= ~WidgetPaintState.Selected;
			}
			else
			{
				state |= WidgetPaintState.Selected;
			}
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

			graphics.RenderSolid(this.ColorBorder);
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
			this.PaintImageButton(graphics, rect, 44);
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
			this.PaintBackground(graphics, rect, rect);

			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.RenderSolid(this.ColorBorder);
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

			double radius = this.RetRadiusFrame(rect);
			Drawing.Path pInside = this.PathRoundRectangle(rect, radius);

			graphics.Rasterizer.AddSurface(pInside);
			graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.2, 1,1,1));

			graphics.Rasterizer.AddOutline(pInside);
			graphics.RenderSolid(this.ColorBorder);
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.3, 1.0, 1.0, 1.0));

			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
			graphics.RenderSolid(this.colorBlack);
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
			if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
			{
				if ((state&WidgetPaintState.Entered) != 0)  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 14);
				}
				else
				{
					if ( (state&WidgetPaintState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 10);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 12);
					}
				}
			}
			else
			{
				rect.Top    -= 2;
				rect.Bottom += 1;

				if ((state&WidgetPaintState.Entered) != 0)  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 8);
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

			Drawing.Color color;
			if ((state&WidgetPaintState.ActiveYes) != 0)   // bouton activé ?
			{
				color = this.colorBlack;
			}
			else
			{
				color = this.colorWhite;
			}

			text.Paint(pos, graphics, Drawing.Rectangle.MaxValue, color, Drawing.GlyphPaintStyle.Normal);
		}

		public override void PaintRibbonSectionBackground(Drawing.Graphics graphics,
												 Drawing.Rectangle fullRect,
												 Drawing.Rectangle userRect,
												 Drawing.Rectangle textRect,
												 TextLayout text,
												 WidgetPaintState state)
		{
			//	Dessine une section d'un ruban.
			fullRect.Deflate(0.5);
			graphics.AddLine(fullRect.Right, fullRect.Top, fullRect.Right, fullRect.Bottom);
			graphics.RenderSolid(this.colorBorder);

			if (text != null)
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);
				Drawing.Point pos = new Drawing.Point(textRect.Left+3, textRect.Bottom);
				text.LayoutSize = new Drawing.Size(textRect.Width-4, textRect.Height);
				text.Alignment = Drawing.ContentAlignment.MiddleLeft;
				text.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.Split | Drawing.TextBreakMode.SingleLine;
				text.Paint (pos, graphics, Drawing.Rectangle.MaxValue, this.colorBlack, Drawing.GlyphPaintStyle.Normal);
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
			if ( !color.IsEmpty && (state&WidgetPaintState.Enabled) != 0 )
			{
				Drawing.Path path = new Drawing.Path();
				path.AppendCircle(rect.Center, rect.Width/2, rect.Height/2);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(color);
			}

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 41);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 43);
			}

			if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
				 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
			{
				this.PaintImageButton(graphics, rect, 42);
			}
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
			graphics.RenderSolid(this.colorCaption);  // fond jaune pale
			
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBlack);  // cadre noir
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
			AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorWhite);
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
					graphics.RenderSolid(this.colorCaption);

					if ( areas[i].Color != Drawing.Color.FromBrightness(1.0) )
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
			
			Drawing.TextStyle.DefineDefaultFontColor(Drawing.Color.FromBrightness(1.0));

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Selected) != 0 )
				{
					text.Paint(pos, graphics, clipRect, this.colorCaptionText, Drawing.GlyphPaintStyle.Selected);
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

			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = text.StandardRectangle;
				rFocus.Offset(pos);
				graphics.Align(ref rFocus);
				rFocus.Inflate(2.5, -0.5);
				this.PaintFocusBox(graphics, rFocus);
			}
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

		protected Drawing.Path PathTopRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			//	Crée le chemin d'un rectangle à coins arrondis en forme de "U" inversé.
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

		protected Drawing.Path PathLeftRoundRectangle(Drawing.Rectangle rect, double radius, bool closed)
		{
			//	Crée le chemin d'un rectangle à coins arrondis en forme de "D" inversé.
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

		protected Drawing.Path PathRightRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			//	Crée le chemin d'un rectangle à coins arrondis en forme de "D".
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius == 0 )
			{
				radius = System.Math.Min(dx, dy)/8;
			}
			
			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (ox+0.5, oy+dy-0.5);
			path.Close();

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

		protected double RetRadiusButton(Drawing.Rectangle rect)
		{
			//	Retourne le rayon à utiliser pour une zone rectangulaire.
			return System.Math.Min(rect.Width, rect.Height)/2;
		}

		protected double RetRadiusFrame(Drawing.Rectangle rect)
		{
			//	Retourne le rayon à utiliser pour une zone rectangulaire.
			double dim = System.Math.Min(rect.Width, rect.Height);
			double radius = System.Math.Min(dim/2, 5);
			double middle = System.Math.Max(dim-radius*2, 2);
			return System.Math.Floor((dim-middle)/2);
		}


		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank)
		{
			//	Dessine un bouton composé plusieurs morceaux d'image.
			this.PaintImageButton(graphics, rect, rank, new Drawing.Margins(0,0,0,0));
		}

		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank,
										Drawing.Margins margins)
		{
			//	Dessine un bouton composé plusieurs morceaux d'image.
			bool simply = false;
			if ( rank >= 1000 )
			{
				rank -= 1000;
				simply = true;
			}

			if ( rank >= 72 )  return;
			if ( rect.IsSurfaceZero )  return;

			Drawing.Rectangle icon = new Drawing.Rectangle();
			icon.Left   = 32*(rank%8);
			icon.Right  = icon.Left+32;
			icon.Top    = 288-32*(rank/8);
			icon.Bottom = icon.Top-32;
			icon.Inflate(margins);

			if ( rank < 8 )
			{
				icon.Width *= 2;
				if ( simply )
				{
					this.PaintImageButton1(graphics, rect, icon);
				}
				else
				{
					this.PaintImageButton3h(graphics, rect, icon, false);
				}
			}
			else if ( rank < 16 || rank == 48 || rank == 50 || rank == 56 || rank == 58 || rank == 64 )
			{
				icon.Width *= 2;
				if ( simply )
				{
					this.PaintImageButton1(graphics, rect, icon);
				}
				else
				{
					this.PaintImageButton3h(graphics, rect, icon, true);
				}
			}
			else if ( rank < 32 || rank == 52 || rank == 53 || rank == 54 || rank == 55 )
			{
				icon.Bottom -= icon.Height;
				if ( simply )
				{
					this.PaintImageButton1(graphics, rect, icon);
				}
				else
				{
					this.PaintImageButton3v(graphics, rect, icon, true);
				}
			}
			else if ( rank == 32 || rank == 34 )
			{
				icon.Width *= 2;
				if ( simply )
				{
					this.PaintImageButton1(graphics, rect, icon);
				}
				else
				{
					this.PaintImageButton9(graphics, rect, 3, icon, 3);
				}
			}
			else
			{
				this.PaintImageButton1(graphics, rect, icon);
			}
		}

		protected void PaintImageButton1(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 Drawing.Rectangle icon)
		{
			//	Dessine un bouton composé d'un seul morceau d'image.
			if ( !rect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, rect, icon);
			}
		}

		protected void PaintImageButton3h(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon,
										  bool big)
		{
			//	Dessine un bouton composé de 3 morceaux d'image horizontaux.
			double rectMargin = 8;
			double iconMargin = 8;

			if ( big )
			{
				rectMargin = System.Math.Min(rect.Height/2, rect.Width/2);
				iconMargin = icon.Width/4;
			}

			Drawing.Rectangle prect = rect;
			Drawing.Rectangle picon = icon;

			prect.Left  = rect.Left;
			prect.Right = rect.Left+rectMargin;
			picon.Left  = icon.Left;
			picon.Right = icon.Left+iconMargin;
			graphics.Align(ref prect);
			if ( !prect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, prect, picon);
			}

			prect.Left  = rect.Left+rectMargin;
			prect.Right = rect.Right-rectMargin;
			picon.Left  = icon.Left+iconMargin;
			picon.Right = icon.Right-iconMargin;
			graphics.Align(ref prect);
			if ( !prect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, prect, picon);
			}

			prect.Left  = rect.Right-rectMargin;
			prect.Right = rect.Right;
			picon.Left  = icon.Right-iconMargin;
			picon.Right = icon.Right;
			graphics.Align(ref prect);
			if ( !prect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, prect, picon);
			}
		}

		protected void PaintImageButton3v(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon,
										  bool big)
		{
			//	Dessine un bouton composé de 3 morceaux d'image verticaux.
			double rectMargin = 8;
			double iconMargin = 8;

			if ( big )
			{
				rectMargin = System.Math.Min(rect.Width/2, rect.Height/2);
				iconMargin = icon.Height/4;
			}

			Drawing.Rectangle prect = rect;
			Drawing.Rectangle picon = icon;

			prect.Bottom = rect.Bottom;
			prect.Top    = rect.Bottom+rectMargin;
			picon.Bottom = icon.Bottom;
			picon.Top    = icon.Bottom+iconMargin;
			graphics.Align(ref prect);
			if ( !prect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, prect, picon);
			}

			prect.Bottom = rect.Bottom+rectMargin;
			prect.Top    = rect.Top-rectMargin;
			picon.Bottom = icon.Bottom+iconMargin;
			picon.Top    = icon.Top-iconMargin;
			graphics.Align(ref prect);
			if ( !prect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, prect, picon);
			}

			prect.Bottom = rect.Top-rectMargin;
			prect.Top    = rect.Top;
			picon.Bottom = icon.Top-iconMargin;
			picon.Top    = icon.Top;
			graphics.Align(ref prect);
			if ( !prect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, prect, picon);
			}
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

		protected void PaintBackground(Drawing.Graphics graphics,
									   Drawing.Rectangle windowRect,
									   Drawing.Rectangle paintRect)
		{
			//	Dessine un fond de fenêtre hachuré horizontalement.
			double dx = 512;
			double dy = 512;
			for ( double y=windowRect.Bottom ; y<windowRect.Top ; y+=dy )
			{
				for ( double x=windowRect.Left ; x<windowRect.Right ; x+=dx )
				{
					Drawing.Rectangle rect = new Drawing.Rectangle(x, y, dx, dy);
					if ( rect.IntersectsWith(paintRect) )
					{
						graphics.PaintImage(this.metal3, rect, new Drawing.Rectangle(0,0,dx,dy));
					}
				}
			}
		}

		protected void PaintRoundShadow(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int deep,
										double alphaTop, double alphaBottom,
										bool hole)
		{
			//	Dessine une ombre autour d'un rectangle arrondi.
			Drawing.Path path;

			double incTop = alphaTop/(double)deep;
			double incBottom = alphaBottom/(double)deep;

			for ( int i=0 ; i<deep ; i++ )
			{
				if ( hole )  rect.Inflate(1);
				double radius = this.RetRadiusFrame(rect);

				double ox = rect.Left;
				double oy = rect.Bottom;
				double dx = rect.Width;
				double dy = rect.Height;

				path = new Drawing.Path();
				path.MoveTo (ox+0.5, oy+dy-radius-0.5);
				path.LineTo (ox+0.5, oy+radius+0.5);
				path.CurveTo(ox+0.5, oy+0.5, ox+radius+0.5, oy+0.5);
				path.LineTo (ox+dx-radius-0.5, oy+0.5);
				path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
				path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
				graphics.Rasterizer.AddOutline(path, 1);
				if ( hole )  graphics.RenderSolid(Drawing.Color.FromAlphaRgb(alphaBottom, 1,1,1));
				else         graphics.RenderSolid(Drawing.Color.FromAlphaRgb(alphaBottom, 0,0,0));

				path = new Drawing.Path();
				path.MoveTo (ox+dx-0.5, oy+dy-radius-0.5);
				path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
				path.LineTo (ox+radius+0.5, oy+dy-0.5);
				path.CurveTo(ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
				graphics.Rasterizer.AddOutline(path, 1);
				Drawing.Rectangle up = rect;
				up.Bottom = up.Top-radius;
				Drawing.Color bottomColor, topColor;
				if ( hole )  bottomColor = Drawing.Color.FromAlphaRgb(alphaBottom, 1,1,1);
				else         bottomColor = Drawing.Color.FromAlphaRgb(alphaBottom, 0,0,0);
				if ( hole )  topColor    = Drawing.Color.FromAlphaRgb(alphaTop, 0,0,0);
				else         topColor    = Drawing.Color.FromAlphaRgb(alphaTop, 1,1,1);
				this.Gradient(graphics, up, bottomColor, topColor);

				if ( !hole )  rect.Deflate(1);
				alphaTop -= incTop;
				alphaBottom -= incBottom;
			}
		}

		protected void PaintRoundTopShadow(Drawing.Graphics graphics,
										   Drawing.Rectangle rect)
		{
			//	Dessine une ombre en haut d'un rectangle arrondi.
			double radius = this.RetRadiusFrame(rect);
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (ox+radius+0.5, oy+dy-0.5);
			path.CurveTo(ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			Drawing.Rectangle up = rect;
			up.Bottom = up.Top-radius;
			this.Gradient(graphics, up, Drawing.Color.FromAlphaRgb(0.0, 0,0,0), Drawing.Color.FromAlphaRgb(0.2, 0,0,0));
		}

		protected void PaintRectTopShadow(Drawing.Graphics graphics,
										  Drawing.Rectangle rect)
		{
			//	Dessine une ombre en haut d'un rectangle.
			double radius = this.RetRadiusFrame(rect);
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(ox+dx-0.5, oy+dy-radius-0.5);
			path.LineTo(ox+dx-0.5, oy+dy-0.5);
			path.LineTo(ox+0.5, oy+dy-0.5);
			path.LineTo(ox+0.5, oy+dy-radius-0.5);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			Drawing.Rectangle up = rect;
			up.Bottom = up.Top-radius;
			this.Gradient(graphics, up, Drawing.Color.FromAlphaRgb(0.0, 0,0,0), Drawing.Color.FromAlphaRgb(0.2, 0,0,0));
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
			Drawing.Transform t = Drawing.Transform.Identity;
			Drawing.Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
//-			t = t.RotateDeg(0, center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}


		public override Drawing.Color AdaptPictogramColor(Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.7)*0.25;  // diminue le contraste
				color = Drawing.Color.FromAlphaRgb(alpha, intensity, intensity, intensity);
			}
			
			return color;
		}

		public override Drawing.Color ColorDisabled
		{
			get { return this.colorDisabled; }
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

		public override Drawing.Color ColorBorder
		{
			get
			{
				return this.colorBorder;
			}
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
					return this.colorCaptionText;
				}
				else
				{
					return Drawing.Color.FromBrightness(1.0);
				}
			}
			else
			{
				return this.colorDisabled;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.ColorBorder;
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.ColorBorder;
		}

		public override Drawing.Color ColorTextDisplayMode(TextFieldDisplayMode mode)
		{
			switch ( mode )
			{
				case TextFieldDisplayMode.Default:   return Drawing.Color.Empty;
				case TextFieldDisplayMode.OverriddenValue:   return this.colorCaption;
				case TextFieldDisplayMode.InheritedValue:  return this.colorThreeState;
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(1,1,1,1); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRadioShapeMargins { get { return new Drawing.Margins(0,0,4,0); } }
		public override Drawing.Margins GeometryGroupShapeMargins { get { return new Drawing.Margins(0,0,0,1); } }
		public override Drawing.Margins GeometryToolShapeMargins { get { return new Drawing.Margins(0,1,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeMargins { get { return new Drawing.Margins(0,1,2,0); } }
		public override Drawing.Margins GeometryButtonShapeMargins { get { return new Drawing.Margins(2,2,0,5); } }
		public override Drawing.Margins GeometryRibbonShapeMargins { get { return new Drawing.Margins(0,0,0,2); } }
		public override Drawing.Margins GeometryTextFieldShapeMargins { get { return new Drawing.Margins(1,1,1,1); } }
		public override Drawing.Margins GeometryListShapeMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override double GeometryComboRightMargin { get { return 1; } }
		public override double GeometryComboBottomMargin { get { return 2; } }
		public override double GeometryComboTopMargin { get { return 2; } }
		public override double GeometryUpDownWidthFactor { get { return 0.6; } }
		public override double GeometryUpDownRightMargin { get { return 0; } }
		public override double GeometryUpDownBottomMargin { get { return 0; } }
		public override double GeometryUpDownTopMargin { get { return 0; } }
		public override double GeometryScrollerRightMargin { get { return 0; } }
		public override double GeometryScrollerBottomMargin { get { return 0; } }
		public override double GeometryScrollerTopMargin { get { return 0; } }
		public override double GeometryScrollListXMargin { get { return 1; } }
		public override double GeometryScrollListYMargin { get { return 1; } }
		public override double GeometrySliderLeftMargin { get { return 0; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 1; } }

		protected Drawing.Image		bitmap;
		protected Drawing.Image		metal3;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorDisabled;
		protected Drawing.Color		colorUndefinedLanguage;
		protected Drawing.Color		colorTextBackground;
		protected Drawing.Color		colorThreeState;
		protected Drawing.Color		colorActivableIcon;
		protected Drawing.Color		colorWindow;
	}
}
