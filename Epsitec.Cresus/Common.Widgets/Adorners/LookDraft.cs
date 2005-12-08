namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookDraft impl�mente le d�corateur qui imite un brouillon.
	/// </summary>
	public class LookDraft : AbstractAdorner
	{
		public LookDraft()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookDraft.png", typeof (IAdorner));
			this.paper  = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "paper1.png", typeof (IAdorner));
		}

		// Initialise les couleurs en fonction des r�glages de Windows.
		protected override void RefreshColors()
		{
			this.colorBlack          = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorControl        = Drawing.Color.FromRGB( 53.0/255.0, 146.0/255.0, 255.0/255.0);
			this.colorCaption        = Drawing.Color.FromRGB(255.0/255.0, 210.0/255.0,   0.0/255.0);
			this.colorCaptionNF      = Drawing.Color.FromRGB(255.0/255.0, 242.0/255.0, 183.0/255.0);
			this.colorInfo           = Drawing.Color.FromRGB(255.0/255.0, 210.0/255.0,   0.0/255.0);
			this.colorBorder         = Drawing.Color.FromRGB(128.0/255.0, 128.0/255.0, 128.0/255.0);
			this.colorDisabled       = Drawing.Color.FromRGB(200.0/255.0, 200.0/255.0, 200.0/255.0);
			this.colorError          = Drawing.Color.FromRGB(255.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorTextBackground = Drawing.Color.FromRGB(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorWindow         = Drawing.Color.FromRGB(247.0/255.0, 247.0/255.0, 247.0/255.0);

			this.colorBorder.A = 0.6;
		}
		

		// Dessine le fond d'une fen�tre.
		public override void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetState state)
		{
			this.PaintBackground(graphics, windowRect, paintRect);
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
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRGB(0.5, 0.0, 0.0);  // rouge fonc�
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRGB(0.0, 0.3, 0.0);  // vert fonc�
			}
			else
			{
				color = this.colorDisabled;
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
			if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
			{
				rInside.Inflate(rect.Height*0.1);
			}
			bool enabled = ( (state&WidgetState.Enabled) != 0 );
			switch ( type )
			{
				case GlyphShape.ArrowUp:
					this.PaintImageButton(graphics, rInside, enabled?64:65);
					return;

				case GlyphShape.ArrowDown:
					this.PaintImageButton(graphics, rInside, enabled?66:67);
					return;

				case GlyphShape.ArrowRight:
					this.PaintImageButton(graphics, rInside, enabled?68:69);
					return;

				case GlyphShape.ArrowLeft:
					this.PaintImageButton(graphics, rInside, enabled?70:71);
					return;

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
			if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
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

		// Dessine un bouton � cocher sans texte.
		public override void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 44);

				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				else if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else
			{
				this.PaintImageButton(graphics, rect, 45);
			}

			if ( (state&WidgetState.ActiveYes) != 0 ||  // coch� ?
				 (state&WidgetState.Engaged) != 0   )
			{
				Drawing.Rectangle rInside = rect;
				rInside.Deflate(rect.Height*0.1);
				rInside.Offset(1.0, 1.0);
				if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					rInside.Inflate(rect.Height*0.1);
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rInside, 74);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 75);
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
					graphics.RenderSolid(this.colorDisabled);
				}
			}
		}

		// Dessine un bouton radio sans texte.
		public override void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 40);

				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 42);
				}
				else if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					this.PaintImageButton(graphics, rect, 42);
				}
			}
			else
			{
				this.PaintImageButton(graphics, rect, 41);
			}

			if ( (state&WidgetState.ActiveYes) != 0 ||  // coch� ?
				 (state&WidgetState.Engaged) != 0   )
			{
				Drawing.Rectangle rInside = rect;
				if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					rInside.Inflate(rInside.Height*0.1);
				}

				if ( (state&WidgetState.Enabled) != 0 )
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
			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 2);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 4);
				}

				if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					this.PaintImageButton(graphics, rect, 6);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 6);
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( style == ButtonStyle.DefaultAccept )
					{
						this.PaintImageButton(graphics, rect, 0);
					}
				}

				if ( (state&WidgetState.Focused) != 0 )
				{
					rect.Deflate(3.0);
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.Scroller )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
					{
						this.PaintImageButton(graphics, rect, 42);
					}
					else if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
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
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
					{
						this.PaintImageButton(graphics, rect, 42);
					}
					else if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
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
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 36);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 45);
				}

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else if ( style == ButtonStyle.ExListMiddle ||
					  style == ButtonStyle.ExListLeft   )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 36);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 45);
				}

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else if ( style == ButtonStyle.UpDown )
			{
				if ( dir == Direction.Up )
				{
					if ( (state&WidgetState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 36);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 45);
					}

					if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
						 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
					{
						this.PaintImageButton(graphics, rect, 46);
					}
				}
				if ( dir == Direction.Down )
				{
					if ( (state&WidgetState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 36);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 45);
					}

					if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
						 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
					{
						this.PaintImageButton(graphics, rect, 46);
					}
				}
			}
			else if ( style == ButtonStyle.Icon )
			{
				bool large = (rect.Width > rect.Height*1.5);
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, large?2:40);
				}
				else
				{
					this.PaintImageButton(graphics, rect, large?4:41);
				}

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, large?6:42);
				}

				if ( (state&WidgetState.Focused) != 0 )
				{
					rect.Deflate(3.0);
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetState.Engaged) != 0 )   // bouton press� ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activ� ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}

				if ( (state&WidgetState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(1);
					}
					else
					{
						rect.Deflate(2);
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

				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetState.Engaged) != 0 )   // bouton press� ?
				{
					this.PaintImageButton(graphics, rect, 47);
				}
				else if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activ� ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				else if ( (state&WidgetState.ActiveMaybe) != 0 )
				{
					this.PaintImageButton(graphics, rect, 22);
				}

				if ( (state&WidgetState.Focused) != 0 )
				{
					if ( System.Math.Min(rect.Width, rect.Height) < 16 )
					{
						rect.Deflate(1);
					}
					else
					{
						rect.Deflate(2);
					}
					this.PaintFocusBox(graphics, rect);
				}
			}
			else if ( style == ButtonStyle.HeaderSlider )
			{
				if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
					 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 44);
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

		// Dessine le texte d'un bouton.
		public override void PaintButtonTextLayout(Drawing.Graphics graphics,
										  Drawing.Point pos,
										  TextLayout text,
										  WidgetState state,
										  ButtonStyle style)
		{
			if ( text == null )  return;

			if ( AbstractAdorner.IsThreeState2(state) )
			{
				pos.Y ++;
			}
			if ( style == ButtonStyle.Tab )
			{
				state |=  WidgetState.Selected;
				pos.Y -= 1.0;
			}
			else
			{
				state &= ~WidgetState.Focused;
			}
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, PaintTextStyle.Button, TextDisplayMode.Default, Drawing.Color.Empty);
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
											 TextDisplayMode mode,
											 bool readOnly)
		{
			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Combo  )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, 32);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 34);
				}
			}
			else if ( style == TextFieldStyle.Multi  )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
											 Widgets.WidgetState state,
											 Widgets.TextFieldStyle style,
											 TextDisplayMode mode,
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
			Drawing.Rectangle rect = frameRect;
			rect.Deflate(0.5);

			if ( dir == Direction.Up )
			{
				frameRect.Bottom += frameRect.Width;
				frameRect.Top    -= frameRect.Width;
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, frameRect, 48);
				}
				else
				{
					this.PaintImageButton(graphics, frameRect, 50);
				}
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
				thumbRect.Deflate(2.0);

				bool little = (thumbRect.Height < thumbRect.Width);
				if ( little )
				{
					thumbRect.Bottom = thumbRect.Center.Y-thumbRect.Width/2.0;
					thumbRect.Height = thumbRect.Width;
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?43:55);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?41:53);
				}

				if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					this.PaintImageButton(graphics, thumbRect, little?42:54);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
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

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?43:58);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?41:50);
				}

				if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					this.PaintImageButton(graphics, thumbRect, little?42:56);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
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
			if ( dir == Widgets.Direction.Left )
			{
				double m = frameRect.Height*1.2;
				frameRect.Left  += m;
				frameRect.Right -= m;
				this.PaintSeparatorBackground(graphics, frameRect, state, Direction.Up, false);
			}
			else
			{
				double m = frameRect.Width*1.2;
				frameRect.Bottom += m;
				frameRect.Top    -= m;
				this.PaintSeparatorBackground(graphics, frameRect, state, Direction.Right, false);
			}
		}

		// Dessine la cabine d'un potentiom�tre lin�aire.
		public override void PaintSliderHandle(Drawing.Graphics graphics,
									  Drawing.Rectangle thumbRect,
									  Drawing.Rectangle tabRect,
									  Widgets.WidgetState state,
									  Widgets.Direction dir)
		{
			if ( dir == Direction.Left )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, 55);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 53);
				}

				if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					this.PaintImageButton(graphics, thumbRect, 54);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, thumbRect, 54);
				}
			}

			if ( dir == Direction.Up )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, 58);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, 50);
				}

				if ( (state&WidgetState.Engaged) != 0 )  // bouton press� ?
				{
					this.PaintImageButton(graphics, thumbRect, 56);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, thumbRect, 56);
				}
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
			frameRect.Top -= titleRect.Height/2;

			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
		}

		// Dessine la zone principale sous les onglets.
		public override void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetState state,
								  Widgets.Direction dir)
		{
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
			{
				this.PaintImageButton(graphics, rect, 38);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 39);
			}
		}

		// Dessine l'onglet devant les autres.
		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetState state,
											Widgets.Direction dir)
		{
			titleRect.Left += 1.0;

			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				this.PaintImageButton(graphics, titleRect, 14);
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
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
			titleRect.Left += 1.0;

			if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
			{
				this.PaintImageButton(graphics, titleRect, 14);
			}
			else
			{
				if ( (state&WidgetState.Enabled) != 0 )
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
											 Widgets.WidgetState state,
											 Widgets.Direction dir)
		{
		}

		// Dessine le fond d'un tableau.
		public override void PaintArrayBackground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetState state)
		{
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
				if ( (state&WidgetState.Focused) != 0 )
				{
					this.PaintImageButton(graphics, rect, 28);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 30);
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
				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 8);
				}
				else if ( (state&WidgetState.Enabled) != 0 )
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
				if ( (state&WidgetState.Entered) != 0 )  // bouton survol� ?
				{
					this.PaintImageButton(graphics, rect, 16);
				}
				else if ( (state&WidgetState.Enabled) != 0 )
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
			rect.Inflate(-this.GeometryMenuShadow);
			this.PaintBackground(graphics, rect, rect);

			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
											MenuOrientation type,
											MenuItemType itemType)
		{
			if ( (state&WidgetState.Enabled) != 0 )
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

		// Dessine le texte d'un menu.
		public override void PaintMenuItemTextLayout(Drawing.Graphics graphics,
											Drawing.Point pos,
											TextLayout text,
											WidgetState state,
											Direction dir,
											MenuOrientation type,
											MenuItemType itemType)
		{
			if ( text == null )  return;
			state &= ~WidgetState.Focused;
			if ( itemType == MenuItemType.Default )
			{
				state &= ~WidgetState.Selected;
			}
			else
			{
				state |= WidgetState.Selected;
			}
			PaintTextStyle style = ( type == MenuOrientation.Horizontal ) ? PaintTextStyle.HMenu : PaintTextStyle.VMenu;
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, style, TextDisplayMode.Default, Drawing.Color.Empty);
		}

		// Dessine le devant d'une case de menu.
		public override void PaintMenuItemForeground(Drawing.Graphics graphics,
											Drawing.Rectangle rect,
											WidgetState state,
											Direction dir,
											MenuOrientation type,
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
			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
											  WidgetState state,
											  Direction dir)
		{
		}

		// Dessine une ligne de statuts.
		public override void PaintStatusBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetState state)
		{
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

			if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
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
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, style, TextDisplayMode.Default, Drawing.Color.Empty);
		}

		// Dessine la bande principale d'un ruban.
		public override void PaintRibbonTabBackground(Drawing.Graphics graphics,
											 Drawing.Rectangle rect,
											 double titleHeight,
											 WidgetState state)
		{
			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-2;
			this.PaintSeparatorBackground(graphics, r, state, Direction.Up, false);

			r = rect;
			r.Top = r.Bottom+2;
			this.PaintSeparatorBackground(graphics, r, state, Direction.Up, false);
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
			rect.Left = rect.Right-2;
			this.PaintSeparatorBackground(graphics, rect, state, Direction.Right, false);
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
			text.Alignment = Drawing.ContentAlignment.MiddleCenter;
			text.Paint(pos, graphics, Drawing.Rectangle.Infinite, Drawing.Color.FromBrightness(0), Drawing.GlyphPaintStyle.Normal);
		}

		// Dessine un tag.
		public override void PaintTagBackground(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   WidgetState state,
									   Drawing.Color color,
									   Direction dir)
		{
			if ( !color.IsEmpty && (state&WidgetState.Enabled) != 0 )
			{
				Drawing.Path path = new Drawing.Path();
				path.AppendCircle(rect.Center, rect.Width/2, rect.Height/2);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(color);
			}

			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 40);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 41);
			}

			if ( (state&WidgetState.Engaged) != 0 ||  // bouton press� ?
				 (state&WidgetState.Entered) != 0 )   // bouton survol� ?
			{
				this.PaintImageButton(graphics, rect, 42);
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
			this.PaintImageButton(graphics, rect, 47);
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
			graphics.RenderSolid(this.colorCaption);
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
												 WidgetState state, PaintTextStyle style, TextDisplayMode mode)
		{
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				if ( (state&WidgetState.Focused) != 0 )
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
												 WidgetState state, PaintTextStyle style, TextDisplayMode mode)
		{
		}

		// Dessine le texte d'un widget.
		public override void PaintGeneralTextLayout(Drawing.Graphics graphics,
										   Drawing.Rectangle clipRect,
										   Drawing.Point pos,
										   TextLayout text,
										   WidgetState state,
										   PaintTextStyle style,
										   TextDisplayMode mode,
										   Drawing.Color backColor)
		{
			if ( text == null )  return;

			string iText = "";
			if ( mode == TextDisplayMode.Proposal )
			{
				iText = text.Text;
				text.Text = string.Format("<i>{0}</i>", text.Text);
			}

			Drawing.TextStyle.DefineDefaultColor(this.colorBlack);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
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

			if ( mode == TextDisplayMode.Proposal )
			{
				text.Text = iText;
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

		// Cr�e le chemin d'un rectangle � coins arrondis en forme de "U" invers�.
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

		// Cr�e le chemin d'un rectangle � coins arrondis en forme de "D" invers�.
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

		// Cr�e le chemin d'un rectangle � coins arrondis en forme de "D".
		protected Drawing.Path PathRightRoundRectangle(Drawing.Rectangle rect, double radius)
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
			path.MoveTo (ox+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (ox+0.5, oy+dy-0.5);
			path.Close();

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

		// Retourne le rayon � utiliser pour une zone rectangulaire.
		protected double RetRadiusButton(Drawing.Rectangle rect)
		{
			return System.Math.Min(rect.Width, rect.Height)/2;
		}

		// Retourne le rayon � utiliser pour une zone rectangulaire.
		protected double RetRadiusFrame(Drawing.Rectangle rect)
		{
			double dim = System.Math.Min(rect.Width, rect.Height);
			double radius = System.Math.Min(dim/2, 5);
			double middle = System.Math.Max(dim-radius*2, 2);
			return System.Math.Floor((dim-middle)/2);
		}


		// Dessine un bouton compos� plusieurs morceaux d'image.
		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank)
		{
			this.PaintImageButton(graphics, rect, rank, new Drawing.Margins(0,0,0,0));
		}

		// Dessine un bouton compos� plusieurs morceaux d'image.
		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank,
										Drawing.Margins margins)
		{
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

		// Dessine un bouton compos� d'un seul morceau d'image.
		protected void PaintImageButton1(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 Drawing.Rectangle icon)
		{
			if ( !rect.IsSurfaceZero )
			{
				icon.Deflate(1.0);
				graphics.PaintImage(this.bitmap, rect, icon);
			}
		}

		// Dessine un bouton compos� de 3 morceaux d'image horizontaux.
		protected void PaintImageButton3h(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon)
		{
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

		// Dessine un bouton compos� de 3 morceaux d'image verticaux.
		protected void PaintImageButton3v(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon)
		{
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

		// Dessine un bouton compos� de 9 morceaux d'image.
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

		// Dessine un fond de fen�tre hachur� horizontalement.
		protected void PaintBackground(Drawing.Graphics graphics,
									   Drawing.Rectangle windowRect,
									   Drawing.Rectangle paintRect)
		{
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

		// Dessine une ombre autour d'un rectangle arrondi.
		protected void PaintRoundShadow(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int deep,
										double alphaTop, double alphaBottom,
										bool hole)
		{
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
				if ( hole )  graphics.RenderSolid(Drawing.Color.FromARGB(alphaBottom, 1,1,1));
				else         graphics.RenderSolid(Drawing.Color.FromARGB(alphaBottom, 0,0,0));

				path = new Drawing.Path();
				path.MoveTo (ox+dx-0.5, oy+dy-radius-0.5);
				path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
				path.LineTo (ox+radius+0.5, oy+dy-0.5);
				path.CurveTo(ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
				graphics.Rasterizer.AddOutline(path, 1);
				Drawing.Rectangle up = rect;
				up.Bottom = up.Top-radius;
				Drawing.Color bottomColor, topColor;
				if ( hole )  bottomColor = Drawing.Color.FromARGB(alphaBottom, 1,1,1);
				else         bottomColor = Drawing.Color.FromARGB(alphaBottom, 0,0,0);
				if ( hole )  topColor    = Drawing.Color.FromARGB(alphaTop, 0,0,0);
				else         topColor    = Drawing.Color.FromARGB(alphaTop, 1,1,1);
				this.Gradient(graphics, up, bottomColor, topColor);

				if ( !hole )  rect.Deflate(1);
				alphaTop -= incTop;
				alphaBottom -= incBottom;
			}
		}

		// Dessine une ombre en haut d'un rectangle arrondi.
		protected void PaintRoundTopShadow(Drawing.Graphics graphics,
										   Drawing.Rectangle rect)
		{
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
			this.Gradient(graphics, up, Drawing.Color.FromARGB(0.0, 0,0,0), Drawing.Color.FromARGB(0.2, 0,0,0));
		}

		// Dessine une ombre en haut d'un rectangle.
		protected void PaintRectTopShadow(Drawing.Graphics graphics,
										  Drawing.Rectangle rect)
		{
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
			this.Gradient(graphics, up, Drawing.Color.FromARGB(0.0, 0,0,0), Drawing.Color.FromARGB(0.2, 0,0,0));
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
			Drawing.Transform t = new Drawing.Transform();
			Drawing.Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			//-			t.RotateDeg(0, center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}


		public override void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
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

		public override Drawing.Color ColorText(WidgetState state)
		{
			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.Selected) != 0 )
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

		public override Drawing.Color ColorTextDisplayMode(TextDisplayMode mode)
		{
			switch ( mode )
			{
				case TextDisplayMode.Default:   return Drawing.Color.Empty;
				case TextDisplayMode.Defined:   return Drawing.Color.FromRGB(255.0/255.0, 210.0/255.0,   0.0/255.0);
				case TextDisplayMode.Proposal:  return Drawing.Color.FromRGB(255.0/255.0, 255.0/255.0, 223.0/255.0);
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,4,0); } }
		public override Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,0,1); } }
		public override Drawing.Margins GeometryToolShapeBounds { get { return new Drawing.Margins(0,1,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeBounds { get { return new Drawing.Margins(0,1,2,0); } }
		public override Drawing.Margins GeometryButtonShapeBounds { get { return new Drawing.Margins(2,2,0,5); } }
		public override Drawing.Margins GeometryRibbonShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryTextFieldShapeBounds { get { return new Drawing.Margins(1,1,1,1); } }
		public override Drawing.Margins GeometryListShapeBounds { get { return new Drawing.Margins(2,2,2,2); } }
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
		protected Drawing.Color		colorError;
		protected Drawing.Color		colorTextBackground;
		protected Drawing.Color		colorWindow;
	}
}
