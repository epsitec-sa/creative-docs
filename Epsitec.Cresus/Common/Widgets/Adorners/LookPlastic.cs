namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookPlastic implémente le décorateur qui imite le plastic.
	/// </summary>
	public class LookPlastic : AbstractAdorner
	{
		public LookPlastic()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookPlastic.png", typeof (IAdorner));
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs en fonction des réglages de Windows.
			double r,g,b;

			this.colorBlack             = Drawing.Color.FromBrightness(0);
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
			this.colorScrollerBack = Drawing.Color.FromRgb(r,g,b);

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorButton = Drawing.Color.FromRgb(r,g,b);

			this.colorCaption           = Drawing.Color.FromRgb ( 58.0/255.0, 167.0/255.0, 233.0/255.0);
			this.colorHilite            = Drawing.Color.FromRgb (250.0/255.0, 196.0/255.0,  89.0/255.0);
			this.colorBorder            = Drawing.Color.FromRgb ( 23.0/255.0, 132.0/255.0, 198.0/255.0);
			this.colorError             = Drawing.Color.FromHexa ("ffb1b1");  // rouge pâle
			this.colorUndefinedLanguage = Drawing.Color.FromHexa ("b1e3ff");  // bleu pâle
			this.colorTextBackground    = Drawing.Color.FromRgb (250.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorWindow            = Drawing.Color.FromRgb (198.0/255.0, 226.0/255.0, 234.0/255.0);
		}
		

		public override void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetPaintState state)
		{
			//	Dessine le fond d'une fenêtre.
			graphics.AddFilledRectangle(paintRect);
			graphics.RenderSolid(this.colorWindow);
		}

		public override void PaintGlyph(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state,
							   GlyphShape type,
							   PaintTextStyle style)
		{
			//	Dessine une icône simple (dans un bouton d'ascenseur par exemple).
			Drawing.Color color = this.colorBlack;
			if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRgb(0.5, 0.0, 0.0);  // rouge foncé
			if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRgb(0.0, 0.5, 0.0);  // vert foncé

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
				graphics.RenderSolid(Drawing.Color.FromRgb(this.colorWindow.R-0.1, this.colorWindow.G-0.1, this.colorWindow.B-0.1));
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
			double spikeShift = 0.25*zoom;
			double baseShiftH = 0.20*zoom;
			double baseShiftV = 0.20*zoom;
			switch ( type )
			{
				default:
					AbstractAdorner.GenerateGlyphShape (rect, type, center, path);
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
			rect = graphics.Align (rect);

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 52);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
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
			else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )  // 3ème état ?
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 55);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 51);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 54);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 53);
				}
			}
			else
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 49);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
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

		public override void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton radio sans texte.
			rect.Deflate(0.5);
			rect = graphics.Align (rect);

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 44);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
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
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 41);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
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
			Drawing.Rectangle rFocus = rect;
			rFocus.Deflate(2.5);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel ||
				 style == ButtonStyle.DefaultAcceptAndCancel )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					else
					{
						if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
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
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 4);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
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
			else if ( style == ButtonStyle.Slider )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 43);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 41);
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
			else if ( style == ButtonStyle.Combo        ||
					  style == ButtonStyle.ExListLeft   ||
					  style == ButtonStyle.ExListMiddle ||
					  style == ButtonStyle.ExListRight  ||
					  style == ButtonStyle.Icon         ||
					  style == ButtonStyle.HeaderSlider )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
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
				if ( (state&WidgetPaintState.Enabled) == 0 )
				{
					graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
					graphics.RenderSolid(this.colorBorder);
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
					else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
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
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 5);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 5);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 2);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
				}
				rFocus.Inflate(1);
			}
			else if ( style == ButtonStyle.ComboItem )
			{
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 5);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 5);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 2);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						if ((state&WidgetPaintState.InheritedEnter) == 0)
						{
							this.PaintImageButton(graphics, rect, 1);
						}
						else  // groupe d'un combo ?
						{
							this.PaintImageButton(graphics, rect, 26);
						}
					}
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
				}
				rFocus.Inflate(1);
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				if ( AbstractAdorner.IsThreeState2(state) )
				{
					rect.Top += 2;
					rFocus.Top += 2;
				}

				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 5);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 5);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 2);
					}
				}
				else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 7);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 7);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 6);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 4);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 0);
					}
				}
				rFocus.Inflate(1);
			}
			else if ( style == ButtonStyle.Confirmation )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rInside, 1);
				}
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 2);
				}
				rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.ListItem )
			{
				this.PaintImageButton(graphics, rect, 0);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 0);
			}
			
			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				rFocus.Inflate(0.5);
				Drawing.Path pInside = this.PathRoundRectangle(rFocus, this.RetRadiusFrame(rFocus));
				AbstractAdorner.DrawFocusedPath(graphics, pInside, this.colorControlDark);
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
			if ( style != ButtonStyle.Tab )
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
			double radius;
			Drawing.Path path;

			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Multiline  ||
				 style == TextFieldStyle.Combo  )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					Drawing.Color color = this.ColorTextDisplayMode(mode);
					if ((state&WidgetPaintState.Error) != 0)
					{
						radius = this.RetRadiusFrame (rect);
						path = this.PathRoundRectangle (rect, radius);
						graphics.Rasterizer.AddSurface (path);
						graphics.RenderSolid (this.colorError);
					}
					else if ((state&WidgetPaintState.UndefinedLanguage) != 0)
					{
						radius = this.RetRadiusFrame (rect);
						path = this.PathRoundRectangle (rect, radius);
						graphics.Rasterizer.AddSurface (path);
						graphics.RenderSolid (this.colorUndefinedLanguage);
					}
					else if (!color.IsEmpty)
					{
						radius = this.RetRadiusFrame(rect);
						path = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddSurface(path);
						graphics.RenderSolid(color);
					}
					else
					{
						this.PaintImageButton(graphics, rect, readOnly?28:26);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 28);
				}

				radius = this.RetRadiusFrame(rect);
				path = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else if ( style == TextFieldStyle.UpDown )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					Drawing.Color color = this.ColorTextDisplayMode(mode);
					if ((state&WidgetPaintState.Error) != 0)
					{
						radius = this.RetRadiusFrame (rect);
						path = this.PathRoundRectangle (rect, radius);
						graphics.Rasterizer.AddSurface (path);
						graphics.RenderSolid (this.colorError);
					}
					else if ((state&WidgetPaintState.UndefinedLanguage) != 0)
					{
						radius = this.RetRadiusFrame (rect);
						path = this.PathRoundRectangle (rect, radius);
						graphics.Rasterizer.AddSurface (path);
						graphics.RenderSolid (this.colorUndefinedLanguage);
					}
					else if (!color.IsEmpty)
					{
						radius = this.RetRadiusFrame(rect);
						path = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddSurface(path);
						graphics.RenderSolid(color);
					}
					else
					{
						this.PaintImageButton(graphics, rect, readOnly?28:26);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 28);
				}

				radius = this.RetRadiusFrame(rect);
				path = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, 26);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 28);
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
				this.PaintImageButton(graphics, frameRect, 25);
			}
			else
			{
				this.PaintImageButton(graphics, frameRect, 27);
			}

			if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
			{
				this.PaintImageButton(graphics, tabRect, 34);
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
				thumbRect.Bottom -= 1;
				thumbRect.Top    += 1;
			}
			if ( dir == Direction.Left )
			{
				thumbRect.Left  -= 1;
				thumbRect.Right += 1;
			}
			state &= ~WidgetPaintState.Engaged;
			this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

			Drawing.Rectangle	rect = new Drawing.Rectangle();
			Drawing.Point		center;

			if ( (state&WidgetPaintState.Enabled) != 0 )
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
							rect = graphics.Align (rect);
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
							rect = graphics.Align (rect);
							this.PaintImageButton(graphics, rect, 37);
						}
						break;
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
				frameRect.Bottom = p-3.5;
				frameRect.Top    = p+3.5;

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, frameRect, 25);
				}
				else
				{
					this.PaintImageButton(graphics, frameRect, 27);
				}

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					tabRect.Bottom = p-2.5;
					tabRect.Top    = p+2.5;
					this.PaintImageButton(graphics, tabRect, 34);
				}
			}
			else
			{
				double m = frameRect.Width*0.2;
				double p = frameRect.Center.X;
				frameRect = sliderRect;
				frameRect.Bottom += m;
				frameRect.Top    -= m;
				frameRect.Left   = p-3.5;
				frameRect.Right  = p+3.5;

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, frameRect, 25);
				}
				else
				{
					this.PaintImageButton(graphics, frameRect, 27);
				}

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					tabRect.Left  = p-2.5;
					tabRect.Right = p+2.5;
					this.PaintImageButton(graphics, tabRect, 34);
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
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, thumbRect, 1);
					}
					else
					{
						this.PaintImageButton(graphics, thumbRect, 0);
					}
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 3);
				}

				graphics.AddLine(thumbRect.Center.X, thumbRect.Bottom+1, thumbRect.Center.X, thumbRect.Top-1);
				graphics.RenderSolid(this.colorControlDarkDark);
			}
			else
			{
				thumbRect.Inflate(0, 1);

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, thumbRect, 1);
					}
					else
					{
						this.PaintImageButton(graphics, thumbRect, 0);
					}
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 3);
				}

				graphics.AddLine(thumbRect.Left+1, thumbRect.Center.Y, thumbRect.Right-1, thumbRect.Center.Y);
				graphics.RenderSolid(this.colorControlDarkDark);
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
			this.PaintImageButton(graphics, rect, 0);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(3);
			if (style == ProgressIndicatorStyle.UnknownDuration)
			{
				rInside.Left = (rInside.Width-rInside.Height)*progress;
				rInside.Width = rInside.Height;
				this.PaintImageButton(graphics, rInside, 2);
			}
			else
			{
				if (progress != 0)
				{
					rInside.Width *= progress;
					this.PaintImageButton(graphics, rInside, 2);
				}
			}
		}

		public override void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetPaintState state)
		{
			//	Dessine le cadre d'un GroupBox.
			Drawing.Path path = this.PathRoundRectangleGroupBox(frameRect, titleRect.Left, titleRect.Right);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.colorBorder);
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);
		}

		public override void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetPaintState state,
								  Widgets.Direction dir)
		{
			//	Dessine la zone principale sous les onglets.
			this.PaintImageButton(graphics, rect, 32);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBorder);
		}

		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine l'onglet devant les autres.
			titleRect.Bottom += 1;
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 9);
			}
			else
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 8);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 11);
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
			titleRect.Left  += 1;
			titleRect.Right -= 1;
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintImageButton(graphics, titleRect, 9);
			}
			else
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, titleRect, 12);
				}
				else
				{
					this.PaintImageButton(graphics, titleRect, 11);
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
			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
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

			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(1.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorCaption);
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
				if ( (state&WidgetPaintState.Focused) != 0 ||
					 (state&WidgetPaintState.InheritedFocus) != 0 )
				{
					this.PaintImageButton(graphics, rect, 33);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 38);
				}
			}

			if ( (state&WidgetPaintState.Entered) != 0 )
			{
				this.PaintImageButton(graphics, rect, 32);
			}

			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(1);
				AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorBorder);
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
			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
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
			else if ( (state&WidgetPaintState.Enabled) != 0 )
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
			this.PaintImageButton(graphics, rect, 0);
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
					if ( itemType == MenuItemState.Selected )
					{
						this.PaintImageButton(graphics, rect, 0);
					}
					if ( itemType == MenuItemState.SubmenuOpen )
					{
						this.PaintImageButton(graphics, rect, 8);
					}
				}

				if ( type == MenuOrientation.Vertical )
				{
					if ( itemType != MenuItemState.Default )
					{
						this.PaintImageButton(graphics, rect, 0);
					}
				}
			}
			else
			{
				if ( itemType != MenuItemState.Default )
				{
					this.PaintImageButton(graphics, rect, 3);
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
			state &= ~WidgetPaintState.Selected;
			state &= ~WidgetPaintState.Focused;
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
				Drawing.Point p1 = graphics.Align(new Drawing.Point(rect.Left+rect.Width/2, rect.Bottom));
				Drawing.Point p2 = graphics.Align(new Drawing.Point(rect.Left+rect.Width/2, rect.Top));
				p1.X -= 0.5;
				p2.X -= 0.5;
				graphics.AddLine(p1, p2);
			}
			else
			{
				Drawing.Point p1 = graphics.Align(new Drawing.Point(rect.Left, rect.Bottom+rect.Height/2));
				Drawing.Point p2 = graphics.Align(new Drawing.Point(rect.Right, rect.Bottom+rect.Height/2));
				p1.Y -= 0.5;
				p2.Y -= 0.5;
				graphics.AddLine(p1, p2);
			}

			graphics.RenderSolid(this.colorBorder);
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
			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.RenderSolid(this.colorBorder);
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
			graphics.Rasterizer.AddOutline(pInside);
			graphics.RenderSolid(this.colorBorder);
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
			this.PaintImageButton(graphics, rect, 2);
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
			rect.Bottom -= 8;  // pour cacher la partie inférieure
			this.PaintButtonBackground(graphics, rect, state, Widgets.Direction.None, ButtonStyle.ToolItem);
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
			state &= ~WidgetPaintState.Focused;
			PaintTextStyle style = PaintTextStyle.HMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.MaxValue, pos, text, state, style, TextFieldDisplayMode.Default, Drawing.Color.Empty);
		}

		public override void PaintRibbonSectionBackground(Drawing.Graphics graphics,
												 Drawing.Rectangle fullRect,
												 Drawing.Rectangle userRect,
												 Drawing.Rectangle textRect,
												 TextLayout text,
												 WidgetPaintState state)
		{
			//	Dessine une section d'un ruban.
			this.PaintImageButton(graphics, fullRect, 0);

			if (text != null)
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);
				Drawing.Point pos = new Drawing.Point(textRect.Left+3, textRect.Bottom+2);
				text.LayoutSize = new Drawing.Size(textRect.Width-4, textRect.Height);
				text.Alignment = Drawing.ContentAlignment.MiddleCenter;
				text.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.Split | Drawing.TextBreakMode.SingleLine;
				text.Paint (pos, graphics, Drawing.Rectangle.MaxValue, Drawing.Color.FromBrightness (0), Drawing.GlyphPaintStyle.Normal);
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
			Drawing.Path path;
			
			path = new Drawing.Path();
			path.AppendCircle(rect.Center, rect.Width/2, rect.Height/2);
			graphics.Rasterizer.AddSurface(path);
			if ( color.IsEmpty || (state&WidgetPaintState.Enabled) == 0 )
			{
				graphics.RenderSolid(this.colorControl);
			}
			else
			{
				graphics.RenderSolid(Drawing.Color.FromAlphaColor(0.5, color));
			}

			Drawing.Rectangle rInside;

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
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
			graphics.RenderSolid(backColor.ColorOrDefault (this.colorInfo));  // fond jaune pale
			
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
			AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorControl);
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
				p1 = graphics.Align(p1);
				p2 = graphics.Align(p2);
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
				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( mode == TextFieldDisplayMode.OverriddenValue )
					{
						this.PaintImageButton(graphics, areas[i].Rect, 39);
					}
					else
					{
						this.PaintImageButton(graphics, areas[i].Rect, 33);
					}

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
			
			Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);

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
				text.Paint(pos, graphics, clipRect, this.colorControlDarkDark, Drawing.GlyphPaintStyle.Disabled);
			}

			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = text.StandardRectangle;
				rFocus.Offset(pos);
				rFocus = graphics.Align (rFocus);
				rFocus.Inflate(2.5, -0.5);
				this.PaintFocusBox(graphics, rFocus);
			}
		}


		protected Drawing.Path PathRoundRectangleGroupBox(Drawing.Rectangle rect,
														  double startX, double endX)
		{
			//	Crée le chemin d'un rectangle à coins arrondis.
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
			double dim = System.Math.Min(rect.Width, rect.Height);
			double radius = System.Math.Min(dim/2, 10);
			double middle = System.Math.Max(dim-radius*2, 2);
			return System.Math.Floor((dim-middle)/2);
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
			//	Dessine un bouton composé de 9 morceaux d'image.
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

		protected void PaintImageButton1(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 Drawing.Rectangle icon)
		{
			//	Dessine un bouton composé d'un seul morceau d'image.
			icon.Deflate(0.5);
			graphics.PaintImage(this.bitmap, rect, icon);
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

			rect = graphics.Align (rect);

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


		public override Drawing.Color AdaptPictogramColor(Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.2, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromAlphaRgb(alpha, intensity, System.Math.Min (intensity*1.4, 1.0), System.Math.Min (intensity*1.4, 1.0));  // bleuté
			}
			return color;
		}

		public override Drawing.Color ColorDisabled
		{
			get { return Drawing.Color.Empty; }
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
			get { return this.colorBorder; }
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
					return this.colorBlack;
				}
			}
			else
			{
				return this.colorControlDarkDark;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.colorBorder;
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.colorBorder;
		}

		public override Drawing.Color ColorTextDisplayMode(TextFieldDisplayMode mode)
		{
			switch ( mode )
			{
				case TextFieldDisplayMode.Default:   return Drawing.Color.Empty;
				case TextFieldDisplayMode.OverriddenValue:   return Drawing.Color.FromRgb(255.0/255.0, 236.0/255.0, 171.0/255.0);
				case TextFieldDisplayMode.InheritedValue:  return Drawing.Color.FromRgb(186.0/255.0, 220.0/255.0, 255.0/255.0);
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(3,3,3,3); } }
		public override Drawing.Margins GeometryRadioShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryGroupShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryToolShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeMargins { get { return new Drawing.Margins(0,0,2,0); } }
		public override Drawing.Margins GeometryButtonShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRibbonShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryTextFieldShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryListShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override double GeometryComboRightMargin { get { return 2; } }
		public override double GeometryComboBottomMargin { get { return 2; } }
		public override double GeometryComboTopMargin { get { return 2; } }
		public override double GeometryUpDownWidthFactor { get { return 0.7; } }
		public override double GeometryUpDownRightMargin { get { return 1; } }
		public override double GeometryUpDownBottomMargin { get { return 1; } }
		public override double GeometryUpDownTopMargin { get { return 1; } }
		public override double GeometryScrollerRightMargin { get { return 2; } }
		public override double GeometryScrollerBottomMargin { get { return 2; } }
		public override double GeometryScrollerTopMargin { get { return 2; } }
		public override double GeometryScrollListXMargin { get { return 2; } }
		public override double GeometryScrollListYMargin { get { return 2; } }
		public override double GeometrySliderLeftMargin { get { return 5; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 0; } }


		protected Drawing.Image		bitmap;
		protected Drawing.Color		colorScrollerBack;
		protected Drawing.Color		colorButton;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorUndefinedLanguage;
		protected Drawing.Color		colorTextBackground;
		protected Drawing.Color		colorWindow;
	}
}
