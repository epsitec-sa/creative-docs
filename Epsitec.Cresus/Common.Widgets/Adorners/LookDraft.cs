namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookDraft implémente le décorateur qui imite un brouillon.
	/// </summary>
	public class LookDraft : AbstractAdorner
	{
		public LookDraft()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookDraft.png", typeof (IAdorner));
			this.paper  = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "paper1.png", typeof (IAdorner));
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs en fonction des réglages de Windows.
			this.colorBlack             = Drawing.Color.FromRgb (  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorControl           = Drawing.Color.FromRgb ( 53.0/255.0, 146.0/255.0, 255.0/255.0);
			this.colorCaption           = Drawing.Color.FromRgb (255.0/255.0, 210.0/255.0,   0.0/255.0);
			this.colorCaptionNF         = Drawing.Color.FromRgb (255.0/255.0, 242.0/255.0, 183.0/255.0);
			this.colorInfo              = Drawing.Color.FromRgb (255.0/255.0, 210.0/255.0,   0.0/255.0);
			this.colorBorder            = Drawing.Color.FromAlphaRgb(0.6, 128.0/255.0, 128.0/255.0, 128.0/255.0);
			this.colorDisabled          = Drawing.Color.FromRgb (200.0/255.0, 200.0/255.0, 200.0/255.0);
			this.colorError             = Drawing.Color.FromRgb (255.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorUndefinedLanguage = Drawing.Color.FromRgb (  0.0/255.0, 162.0/255.0, 255.0/255.0);
			this.colorTextBackground    = Drawing.Color.FromRgb (255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorWindow            = Drawing.Color.FromRgb (247.0/255.0, 247.0/255.0, 247.0/255.0);
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
			Drawing.Color color = this.colorBlack;

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

				graphics.AddLine(p.X-12.5, p.Y+1.5, p.X-1.5, p.Y+12.5);
				graphics.AddLine(p.X- 8.5, p.Y+1.5, p.X-1.5, p.Y+ 8.5);
				graphics.AddLine(p.X- 4.5, p.Y+1.5, p.X-1.5, p.Y+ 4.5);
				graphics.RenderSolid(this.colorBorder);
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

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(rect.Height*0.2);
			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
			{
				rInside.Inflate(rect.Height*0.1);
			}
			bool enabled = ( (state&WidgetPaintState.Enabled) != 0 );
			switch ( type )
			{
				case GlyphShape.Lock:
					this.PaintImageButton(graphics, rInside, enabled?78:79);
					break;

				case GlyphShape.ArrowUp:
				case GlyphShape.TriangleUp:
					this.PaintImageButton(graphics, rInside, enabled?64:65);
					return;

				case GlyphShape.ArrowDown:
				case GlyphShape.TriangleDown:
					this.PaintImageButton(graphics, rInside, enabled?66:67);
					return;

				case GlyphShape.ArrowRight:
				case GlyphShape.TriangleRight:
					this.PaintImageButton(graphics, rInside, enabled?68:69);
					return;

				case GlyphShape.ArrowLeft:
				case GlyphShape.TriangleLeft:
					this.PaintImageButton(graphics, rInside, enabled?70:71);
					return;

				case GlyphShape.HorizontalMove:
					rInside.Deflate(rInside.Width*0.1);
					rInside.Offset(-rInside.Width*0.4, 0);
					this.PaintImageButton(graphics, rInside, enabled?70:71);
					rInside.Offset(rInside.Width*0.8, 0);
					this.PaintImageButton(graphics, rInside, enabled?68:69);
					break;

				case GlyphShape.VerticalMove:
					rInside.Deflate(rInside.Width*0.1);
					rInside.Offset(0, -rInside.Width*0.4);
					this.PaintImageButton(graphics, rInside, enabled?66:67);
					rInside.Offset(0, rInside.Width*0.8);
					this.PaintImageButton(graphics, rInside, enabled?64:65);
					break;

				case GlyphShape.Menu:
					this.PaintImageButton(graphics, rInside, enabled?66:67);
					return;

				case GlyphShape.Close:
				case GlyphShape.Reject:
					this.PaintImageButton(graphics, rInside, enabled?72:73);
					return;

				case GlyphShape.Accept:
					this.PaintImageButton(graphics, rInside, enabled?74:75);
					return;
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
				this.PaintImageButton(graphics, rect, 44);

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
				this.PaintImageButton(graphics, rect, 45);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 ||  // coché ?
				 (state&WidgetPaintState.Engaged) != 0   )
			{
				Drawing.Rectangle rInside = rect;
				rInside.Deflate(rect.Height*0.1);
				rInside.Offset(1.0, 1.0);
				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					rInside.Inflate(rect.Height*0.1);
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rInside, 74);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 75);
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
				this.PaintImageButton(graphics, rect, 40);

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
				this.PaintImageButton(graphics, rect, 41);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 ||  // coché ?
				 (state&WidgetPaintState.Engaged) != 0   )
			{
				Drawing.Rectangle rInside = rect;
				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					rInside.Inflate(rInside.Height*0.1);
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rInside, 76);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 77);
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
			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel ||
				 style == ButtonStyle.DefaultAcceptAndCancel )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 2);
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

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
					{
						this.PaintImageButton(graphics, rect, 0);
					}
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
					this.PaintImageButton(graphics, rect, 41);
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
					this.PaintImageButton(graphics, rect, 41);
				}
			}
			else if ( style == ButtonStyle.Combo       ||
					  style == ButtonStyle.ExListRight )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 36);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 45);
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
					this.PaintImageButton(graphics, rect, 36);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 45);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetPaintState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else if ( style == ButtonStyle.UpDown )
			{
				if ( dir == Direction.Up )
				{
					if ( (state&WidgetPaintState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 36);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 45);
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
						this.PaintImageButton(graphics, rect, 36);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 45);
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
					this.PaintImageButton(graphics, rect, large?4:41);
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
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(1.5);
					}
					else
					{
						rect.Deflate(2.5);
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
						this.PaintImageButton(graphics, rect, 47);
					}
					else  // groupe d'un combo ?
					{
						this.PaintImageButton(graphics, rect, 44);
					}
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(1.5);
					}
					else
					{
						rect.Deflate(2.5);
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

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					this.PaintImageButton(graphics, rect, 22);
				}

				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(1.5);
					}
					else
					{
						rect.Deflate(2.5);
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
					this.PaintImageButton(graphics, rect, 30);
				}
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 28);
				}
			}
			else if ( style == ButtonStyle.ListItem )
			{
				this.PaintImageButton(graphics, rect, 2);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 2);
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
					this.PaintImageButton(graphics, rect, 32);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 34);
				}
			}
			else
			{
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
			Drawing.Rectangle rect = frameRect;
			rect.Deflate(0.5);

			if ( dir == Direction.Up )
			{
				frameRect.Bottom += frameRect.Width;
				frameRect.Top    -= frameRect.Width;
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, frameRect, 52);
				}
				else
				{
					this.PaintImageButton(graphics, frameRect, 53);
				}
			}
			if ( dir == Direction.Left )
			{
				frameRect.Left  += frameRect.Height;
				frameRect.Right -= frameRect.Height;
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, frameRect, 48);
				}
				else
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
				thumbRect.Deflate(2.0);

				bool little = (thumbRect.Height < thumbRect.Width);
				if ( little )
				{
					thumbRect.Bottom = thumbRect.Center.Y-thumbRect.Width/2.0;
					thumbRect.Height = thumbRect.Width;
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?43:55);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?41:53);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?42:54);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?42:54);
				}

				if ( thumbRect.Width >= 10 && thumbRect.Height >= 30 && !little )
				{
					Drawing.Rectangle rect = thumbRect;
					rect.Deflate(2.0, 0.0);
					rect.Bottom = rect.Center.Y-4.0*2;
					rect.Height = 4.0;
					for ( int i=0 ; i<4 ; i++ )
					{
						this.PaintSeparatorBackground(graphics, rect, state, Direction.Up, false);
						rect.Offset(0.0, 4.0);
					}
				}
			}

			if ( dir == Direction.Left )
			{
				thumbRect.Deflate(2.0);

				bool little = (thumbRect.Width < thumbRect.Height);
				if ( little )
				{
					thumbRect.Left = thumbRect.Center.X-thumbRect.Height/2.0;
					thumbRect.Width = thumbRect.Height;
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?43:58);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?41:50);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?42:56);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?42:56);
				}

				if ( thumbRect.Height >= 10 && thumbRect.Width >= 30 && !little )
				{
					Drawing.Rectangle rect = thumbRect;
					rect.Deflate(0.0, 2.0);
					rect.Left = rect.Center.X-4.0*2;
					rect.Width = 4.0;
					for ( int i=0 ; i<4 ; i++ )
					{
						this.PaintSeparatorBackground(graphics, rect, state, Direction.Right, false);
						rect.Offset(4.0, 0.0);
					}
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
				frameRect = sliderRect;
				frameRect.Left  += m;
				frameRect.Right -= m;
				this.PaintSeparatorBackground(graphics, sliderRect, state, Direction.Up, false);
			}
			else
			{
				double m = frameRect.Width*0.2;
				frameRect = sliderRect;
				frameRect.Bottom += m;
				frameRect.Top    -= m;
				this.PaintSeparatorBackground(graphics, sliderRect, state, Direction.Right, false);
			}
		}

		public override void PaintSliderHandle(Drawing.Graphics graphics,
									  Drawing.Rectangle thumbRect,
									  Drawing.Rectangle tabRect,
									  Widgets.WidgetPaintState state,
									  Widgets.Direction dir)
		{
			//	Dessine la cabine d'un potentiomètre linéaire.
			if ( dir == Direction.Left )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, 55);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 53);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, 54);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, 54);
				}
			}

			if ( dir == Direction.Up )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, 58);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 50);
				}

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, 56);
				}
				else if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, 56);
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
				rInside.Deflate(3);
				rInside.Left = (rInside.Width-rInside.Height)*progress;
				rInside.Width = rInside.Height;
				this.PaintImageButton(graphics, rInside, 42);
			}
			else
			{
				rInside.Deflate(4);
				if (progress != 0)
				{
					rInside.Width *= progress;
					this.PaintImageButton(graphics, rInside, 6);
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

			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
			{
				this.PaintImageButton(graphics, frameRect, 38);
			}
			else
			{
				this.PaintImageButton(graphics, frameRect, 39);
			}
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
			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
			{
				this.PaintImageButton(graphics, rect, 38);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 39);
			}
		}

		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine l'onglet devant les autres.
			titleRect.Left += 1.0;

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
			titleRect.Left += 1.0;

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
			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				rect.Deflate(1.0);
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
					this.PaintImageButton(graphics, rect, 28);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 30);
				}
			}

			if ( (state&WidgetPaintState.Entered) != 0 )
			{
				this.PaintImageButton(graphics, rect, 30);
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

			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
			{
				this.PaintImageButton(graphics, rect, 38);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 39);
			}
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
											MenuItemType itemType)
		{
			//	Dessine le fond d'une case de menu.
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == MenuOrientation.Horizontal )
				{
					if ( itemType != MenuItemType.Default )
					{
						this.PaintImageButton(graphics, rect, 28);
					}
				}

				if ( type == MenuOrientation.Vertical )
				{
					if ( itemType != MenuItemType.Default )
					{
						this.PaintImageButton(graphics, rect, 28);
					}
				}
			}
			else
			{
				if ( itemType != MenuItemType.Default )
				{
					this.PaintImageButton(graphics, rect, 28);
				}
			}
		}

		public override void PaintMenuItemTextLayout(Drawing.Graphics graphics,
											Drawing.Point pos,
											TextLayout text,
											WidgetPaintState state,
											Direction dir,
											MenuOrientation type,
											MenuItemType itemType)
		{
			//	Dessine le texte d'un menu.
			if ( text == null )  return;
			state &= ~WidgetPaintState.Focused;
			if ( itemType == MenuItemType.Default )
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
											MenuItemType itemType)
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
			double width = 2.0;

			if ( dir == Direction.Right )
			{
				if ( rect.Width > width )
				{
					rect.Left = rect.Center.X-width/2.0;
					rect.Width = width;
				}
				this.PaintImageButton(graphics, rect, 20);
			}
			else
			{
				if ( rect.Height > width )
				{
					rect.Bottom = rect.Center.Y-width/2.0;
					rect.Height = width;
				}
				this.PaintImageButton(graphics, rect, 21);
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
			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
			{
				this.PaintImageButton(graphics, rect, 38);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 39);
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

			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
			{
				this.PaintImageButton(graphics, rect, 38);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 39);
			}
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
			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-2;
			this.PaintSeparatorBackground(graphics, r, state, Direction.Up, false);

			r = rect;
			r.Top = r.Bottom+2;
			this.PaintSeparatorBackground(graphics, r, state, Direction.Up, false);
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
			fullRect.Left = fullRect.Right-2;
			this.PaintSeparatorBackground(graphics, fullRect, state, Direction.Right, false);

			if (text != null)
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);
				Drawing.Point pos = new Drawing.Point(textRect.Left+3, textRect.Bottom);
				text.LayoutSize = new Drawing.Size(textRect.Width-4, textRect.Height);
				text.Alignment = Drawing.ContentAlignment.MiddleCenter;
				text.Paint(pos, graphics, Drawing.Rectangle.MaxValue, Drawing.Color.FromBrightness(0), Drawing.GlyphPaintStyle.Normal);
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
				this.PaintImageButton(graphics, rect, 40);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 41);
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
			this.PaintImageButton(graphics, rect, 47);
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
			AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorBlack);
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
				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					this.PaintImageButton(graphics, areas[i].Rect, 28);
				}
				else
				{
					this.PaintImageButton(graphics, areas[i].Rect, 30);
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
					text.Paint(pos, graphics, clipRect, this.colorBlack, Drawing.GlyphPaintStyle.Selected);
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
			if ( rank >= 80 )  return;
			if ( rect.IsSurfaceZero )  return;

			Drawing.Rectangle icon = new Drawing.Rectangle();
			icon.Left   = 32*(rank%8);
			icon.Right  = icon.Left+32;
			icon.Top    = 320-32*(rank/8);
			icon.Bottom = icon.Top-32;
			icon.Inflate(margins);

			if ( (rank >=  0 && rank < 16) ||
				 (rank >= 28 && rank < 32) ||
				 (rank >= 48 && rank < 52) ||
				 (rank >= 56 && rank < 60) )
			{
				icon.Width *= 2;
				this.PaintImageButton3h(graphics, rect, icon);
			}
			else if ( (rank >= 16 && rank < 20) ||
					  (rank >= 52 && rank < 56) )
			{
				icon.Bottom -= icon.Height;
				this.PaintImageButton3v(graphics, rect, icon);
			}
			else if ( (rank >= 32 && rank < 36) )
			{
				icon.Width *= 2;
				this.PaintImageButton9(graphics, rect, 4, icon, 7);
			}
			else if ( (rank >= 36 && rank < 38) ||
					  (rank >= 44 && rank < 48) ||
					  rank == 22 )
			{
				this.PaintImageButton9(graphics, rect, 4, icon, 7);
			}
			else if ( (rank >= 38 && rank < 40) )  // cadre ?
			{
				this.PaintImageButton9(graphics, rect, 5, icon, 7);
			}
			else if ( rank == 20 )  // trait vertical ?
			{
				icon.Right = icon.Left+5;
				this.PaintImageButton1(graphics, rect, icon);
			}
			else if ( rank == 21 )  // trait horizontal ?
			{
				icon.Bottom = icon.Top-5;
				this.PaintImageButton1(graphics, rect, icon);
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
				icon.Deflate(1.0);
				graphics.PaintImage(this.bitmap, rect, icon);
			}
		}

		protected void PaintImageButton3h(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon)
		{
			//	Dessine un bouton composé de 3 morceaux d'image horizontaux.
			double rectMargin = System.Math.Min(rect.Height/2, rect.Width/2);
			double iconMargin = icon.Width/4;

			Drawing.Rectangle prect = rect;
			Drawing.Rectangle picon = icon;
			picon.Deflate(0.0, 0.5);

			prect.Left  = rect.Left;
			prect.Right = rect.Left+rectMargin;
			picon.Left  = icon.Left+0.5;
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
			picon.Right = icon.Right-0.5;
			graphics.Align(ref prect);
			if ( !prect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, prect, picon);
			}
		}

		protected void PaintImageButton3v(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon)
		{
			//	Dessine un bouton composé de 3 morceaux d'image verticaux.
			double rectMargin = System.Math.Min(rect.Width/2, rect.Height/2);
			double iconMargin = icon.Height/4;

			Drawing.Rectangle prect = rect;
			Drawing.Rectangle picon = icon;
			picon.Deflate(0.5, 0.0);

			prect.Bottom = rect.Bottom;
			prect.Top    = rect.Bottom+rectMargin;
			picon.Bottom = icon.Bottom+0.5;
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
			picon.Top    = icon.Top-0.5;
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
						graphics.PaintImage(this.paper, rect, new Drawing.Rectangle(0,0,dx,dy));
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
				intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.3, 1.0);  // augmente l'intensité
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
					return this.colorBlack;
				}
				else
				{
					return this.colorBlack;
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
				case TextFieldDisplayMode.OverriddenValue:   return Drawing.Color.FromRgb(255.0/255.0, 210.0/255.0,   0.0/255.0);
				case TextFieldDisplayMode.InheritedValue:  return Drawing.Color.FromRgb(255.0/255.0, 255.0/255.0, 223.0/255.0);
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRadioShapeMargins { get { return new Drawing.Margins(0,0,4,0); } }
		public override Drawing.Margins GeometryGroupShapeMargins { get { return new Drawing.Margins(0,0,0,1); } }
		public override Drawing.Margins GeometryToolShapeMargins { get { return new Drawing.Margins(0,1,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeMargins { get { return new Drawing.Margins(0,1,2,0); } }
		public override Drawing.Margins GeometryButtonShapeMargins { get { return new Drawing.Margins(2,2,0,5); } }
		public override Drawing.Margins GeometryRibbonShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryTextFieldShapeMargins { get { return new Drawing.Margins(1,1,1,1); } }
		public override Drawing.Margins GeometryListShapeMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override double GeometryComboRightMargin { get { return 2; } }
		public override double GeometryComboBottomMargin { get { return 2; } }
		public override double GeometryComboTopMargin { get { return 2; } }
		public override double GeometryUpDownWidthFactor { get { return 0.6; } }
		public override double GeometryUpDownRightMargin { get { return 0; } }
		public override double GeometryUpDownBottomMargin { get { return 0; } }
		public override double GeometryUpDownTopMargin { get { return 0; } }
		public override double GeometryScrollerRightMargin { get { return 0; } }
		public override double GeometryScrollerBottomMargin { get { return 0; } }
		public override double GeometryScrollerTopMargin { get { return 0; } }
		public override double GeometryScrollListXMargin { get { return 3; } }
		public override double GeometryScrollListYMargin { get { return 3; } }
		public override double GeometrySliderLeftMargin { get { return 1; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 1; } }

		protected Drawing.Image		bitmap;
		protected Drawing.Image		paper;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorDisabled;
		protected Drawing.Color		colorUndefinedLanguage;
		protected Drawing.Color		colorTextBackground;
		protected Drawing.Color		colorWindow;
	}
}
