namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.LookAqua implémente le décorateur qui imite le Mac OS X.
	/// </summary>
	public class LookAqua : IAdorner
	{
		public LookAqua()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "LookAqua.png", typeof (IAdorner));
			this.metal1 = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "metal1.png", typeof (IAdorner));
			this.metal2 = Drawing.Bitmap.FromManifestResource("Epsitec.Common.Widgets.Adorners.Resources", "metal2.png", typeof (IAdorner));
			this.metalRenderer = false;
			this.dynamicReflect = false;
			this.RefreshColors();
		}

		// Initialise les couleurs en fonction des réglages de Windows.
		public void RefreshColors()
		{
			this.colorBlack       = Drawing.Color.FromRGB(  0.0/255.0,   0.0/255.0,   0.0/255.0);
			this.colorControl     = Drawing.Color.FromRGB( 53.0/255.0, 146.0/255.0, 255.0/255.0);
			this.colorCaption     = Drawing.Color.FromRGB(  0.0/255.0, 115.0/255.0, 244.0/255.0);
			this.colorCaptionNF   = Drawing.Color.FromRGB(190.0/255.0, 190.0/255.0, 190.0/255.0);
			this.colorCaptionText = Drawing.Color.FromRGB(255.0/255.0, 255.0/255.0, 255.0/255.0);
			this.colorInfo        = Drawing.Color.FromRGB(213.0/255.0, 233.0/255.0, 255.0/255.0);
			this.colorBorder      = Drawing.Color.FromRGB(170.0/255.0, 170.0/255.0, 170.0/255.0);
			this.colorDisabled    = Drawing.Color.FromRGB(140.0/255.0, 140.0/255.0, 140.0/255.0);
			this.colorWindow      = Drawing.Color.FromRGB(255.0/255.0, 255.0/255.0, 255.0/255.0);

			this.colorCaption.A = 0.7;
		}
		

		// Dessine le fond d'une fenêtre.
		public void PaintWindowBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle windowRect,
										  Drawing.Rectangle paintRect,
										  WidgetState state)
		{
			this.PaintBackground(graphics, windowRect, paintRect, 1.0, 20.0, true);
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
			double spikeShift = 0.20*zoom;
			double baseShiftH = 0.20*zoom;
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

				case GlyphShape.TabRight:
					path.MoveTo(center.X-rect.Width*0.10*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.00*zoom, center.Y+rect.Height*0.15*zoom);
					path.LineTo(center.X+rect.Width*0.00*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.05*zoom);
					path.LineTo(center.X+rect.Width*0.20*zoom, center.Y-rect.Height*0.15*zoom);
					path.LineTo(center.X-rect.Width*0.10*zoom, center.Y-rect.Height*0.15*zoom);
					break;

				case GlyphShape.TabLeft:
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
			}
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			if ( (state&WidgetState.Enabled) != 0 )
			{
				Drawing.Color color = this.colorBlack;
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRGB(0.5, 0.0, 0.0);  // rouge foncé
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRGB(0.0, 0.3, 0.0);  // vert foncé
				graphics.RenderSolid(color);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		// Dessine un bouton à cocher sans texte.
		public void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.ActiveYes) != 0 )  // coché ?
				{
					this.PaintImageButton(graphics, rect, 44);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 45);
				}

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				else if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
			}
			else
			{
				this.PaintImageButton(graphics, rect, 47);
			}

			if ( (state&WidgetState.ActiveYes) != 0 ||  // coché ?
				 (state&WidgetState.Engaged) != 0   )
			{
				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
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
		public void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.Align(ref rect);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				if ( (state&WidgetState.ActiveYes) != 0 )  // coché ?
				{
					this.PaintImageButton(graphics, rect, 40);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 41);
				}

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 42);
				}
				else if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 42);
				}
			}
			else
			{
				this.PaintImageButton(graphics, rect, 43);
			}

			if ( (state&WidgetState.ActiveYes) != 0 ||  // coché ?
				 (state&WidgetState.Engaged) != 0   )
			{
				Drawing.Rectangle rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					rInside.Inflate(rInside.Height*0.1);
				}
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorBlack);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorDisabled);
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
			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					Drawing.Rectangle shadow = rect;
					shadow.Left   -= 2;
					shadow.Right  += 2;
					shadow.Bottom -= 5;
					this.PaintImageButton(graphics, shadow, 72);

					if ( (state&WidgetState.Focused) != 0 )
					{
						Drawing.Path path = this.PathRoundRectangle(rect, this.RetRadiusButton(rect));
						graphics.Rasterizer.AddSurface(path);
						if ( style == ButtonStyle.DefaultAccept )
						{
							graphics.RenderSolid(Drawing.Color.FromRGB(1.0, 0.0, 1.0));
						}
						else
						{
							graphics.RenderSolid(Drawing.Color.FromRGB(0.0, 0.7, 1.0));
						}
					}

					if ( style == ButtonStyle.DefaultAccept )
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

				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 6);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 6);
				}
			}
			else if ( style == ButtonStyle.Scroller )
			{
				Drawing.Margins margins = new Drawing.Margins(0,0,0,0);
				if ( dir == Direction.Left  )  margins.Right  = -10;
				if ( dir == Direction.Right )  margins.Left   = -10;
				if ( dir == Direction.Down  )  margins.Top    = -10;
				if ( dir == Direction.Up    )  margins.Bottom = -10;

				if ( dir == Direction.Up || dir == Direction.Down )
				{
					if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
						 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 37, margins);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 35, margins);
					}
				}
				else
				{
					if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
						 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 36, margins);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 34, margins);
					}
				}
			}
			else if ( style == ButtonStyle.Combo       ||
					  style == ButtonStyle.ExListRight )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 54);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 63);
				}

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 62);
				}
			}
			else if ( style == ButtonStyle.ExListMiddle ||
					  style == ButtonStyle.ExListLeft   )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 32);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 38);
				}

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 36);
				}
			}
			else if ( style == ButtonStyle.UpDown )
			{
				if ( (state&WidgetState.Enabled) == 0 )
				{
					graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
					graphics.RenderSolid(this.ColorBorder);
				}

				if ( dir == Direction.Up )
				{
					if ( (state&WidgetState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 64);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 68);
					}

					if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
						 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 70);
					}
				}
				if ( dir == Direction.Down )
				{
					if ( (state&WidgetState.Enabled) != 0 )
					{
						this.PaintImageButton(graphics, rect, 65);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 69);
					}

					if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
						 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
					{
						this.PaintImageButton(graphics, rect, 71);
					}
				}
			}
			else if ( style == ButtonStyle.Icon )
			{
				if ( (state&WidgetState.Enabled) != 0 )
				{
					if ( (state&WidgetState.Focused) != 0 )
					{
						Drawing.Path path = this.PathRoundRectangle(rect, this.RetRadiusButton(rect));
						graphics.Rasterizer.AddSurface(path);
						graphics.RenderSolid(Drawing.Color.FromRGB(1.0, 0.0, 1.0));
					}

					this.PaintImageButton(graphics, rect, 40);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 43);
				}

				if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 42);
				}
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetState.Focused) != 0 )
				{
					this.PaintFocusBox(graphics, rect);
				}

				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 45);
				}
				if ( (state&WidgetState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 46);
				}
				if ( (state&WidgetState.ActiveYes) != 0 )   // bouton activé ?
				{
					this.PaintImageButton(graphics, rect, 44);
				}
			}
			else if ( style == ButtonStyle.HeaderSlider )
			{
				if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
					 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 36);
				}
				else
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
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, PaintTextStyle.Button, Drawing.Color.Empty);
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
				 style == TextFieldStyle.Combo  )
			{
				double radius = this.RetRadiusFrame(rect);
				Drawing.Path path = this.PathRoundRectangle(rect, radius);

				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(Drawing.Color.FromBrightness(readOnly?0.9:1.0));
				}
				else
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(Drawing.Color.FromBrightness(0.9));
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintRoundTopShadow(graphics, rect);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.ColorBorder);

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintRoundShadow(graphics, rect, 1, 0.3, 0.7, true);
				}
			}
			else if ( style == TextFieldStyle.UpDown )
			{
				double radius = this.RetRadiusFrame(rect);
				Drawing.Path path = this.PathRoundRectangle(rect, radius);

				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(Drawing.Color.FromBrightness(readOnly?0.9:1.0));
				}
				else
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(Drawing.Color.FromBrightness(0.9));
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintRoundTopShadow(graphics, rect);
				}

				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(this.ColorBorder);

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintRoundShadow(graphics, rect, 1, 0.3, 0.7, true);
				}
			}
			else if ( style == TextFieldStyle.Multi  )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(Drawing.Color.FromBrightness(readOnly?0.9:1.0));
				}
				else
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(Drawing.Color.FromBrightness(0.9));
				}

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintRectTopShadow(graphics, rect);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.ColorBorder);

				if ( (state&WidgetState.Enabled) != 0 )
				{
					rect.Inflate(0.5);
					this.PaintRectShadow(graphics, rect, 1, 0.3, 0.7, true);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(Drawing.Color.FromBrightness(readOnly?0.9:1.0));
				}
				else
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(Drawing.Color.FromBrightness(0.9));
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.ColorBorder);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromBrightness(1.0));
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
			Drawing.Rectangle rect = frameRect;
			rect.Deflate(0.5);

			if ( dir == Direction.Up )
			{
				bool little = (thumbRect.Height < thumbRect.Width);
				frameRect.Bottom += frameRect.Width;
				frameRect.Top    -= frameRect.Width;
				this.PaintImageButton(graphics, frameRect, little?53:52);
			}
			if ( dir == Direction.Left )
			{
				bool little = (thumbRect.Width < thumbRect.Height);
				frameRect.Left  += frameRect.Height;
				frameRect.Right -= frameRect.Height;
				this.PaintImageButton(graphics, frameRect, little?50:48);
			}

			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorBorder);
		}

		// Dessine la cabine d'un ascenseur.
		public void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetState state,
										Widgets.Direction dir)
		{
			thumbRect.Deflate(1);

			if ( dir == Direction.Up )
			{
				bool little = (thumbRect.Height < thumbRect.Width);

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?33:16);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?1000:18);
				}

				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?37:19);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?37:19);
				}
			}
			if ( dir == Direction.Left )
			{
				bool little = (thumbRect.Width < thumbRect.Height);

				if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, thumbRect, little?32:0);
				}
				else
				{
					this.PaintImageButton(graphics, thumbRect, little?1000:4);
				}

				if ( (state&WidgetState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?36:6);
				}
				else if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, thumbRect, little?36:6);
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
			frameRect.Top -= titleRect.Height/2;
			double radius = this.RetRadiusFrame(frameRect);
			Drawing.Path path;

			path = this.PathRoundRectangle(frameRect, radius);
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Drawing.Color.FromARGB(0.05, 0,0,0));

			if ( this.metalRenderer )
			{
				frameRect.Offset(0, -1);
				path = this.PathRoundRectangle(frameRect, radius);
				graphics.Rasterizer.AddOutline(path, 1);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.7, 1,1,1));
				frameRect.Offset(0, 1);
			}

			path = this.PathRoundRectangle(frameRect, radius);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.ColorBorder);
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
		}

		// Dessine la zone principale sous les onglets.
		public void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetState state,
								  Widgets.Direction dir)
		{
			this.PaintBackground(graphics, rect, rect, 0.95, 0.0, false);

			Drawing.Rectangle top = rect;
			Drawing.Rectangle full = rect;

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorBorder);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				top.Bottom = top.Top-10;
				top.Deflate(1, 0);
				this.PaintImageButton(graphics, top, 56);
			}

			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintRectShadow(graphics, full, 2, 0.3, 0.7, true);
			}
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
			if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
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
			this.PaintBackground(graphics, rect, rect, 0.95, 10.0, false);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorBorder);

			if ( (state&WidgetState.Enabled) != 0 )
			{
				rect.Inflate(0.5);
				this.PaintRectShadow(graphics, rect, 2, 0.3, 0.7, true);
			}
		}

		public void PaintArrayForeground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetState state)
		{
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorBorder);
		}

		// Dessine le fond d'une cellule.
		public void PaintCellBackground(Drawing.Graphics graphics,
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
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromBrightness(1.0));
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
				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 36, new Drawing.Margins(-3,-3,0,-1));
				}
				else if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 34, new Drawing.Margins(-10,-10,0,-1));
				}
				else
				{
					this.PaintImageButton(graphics, rect, 38, new Drawing.Margins(-10,-10,0,-1));
				}

				graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top);
				graphics.RenderSolid(this.ColorBorder);
			}

			if ( dir == Direction.Left )
			{
				if ( (state&WidgetState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rect, 37, new Drawing.Margins(0,-1,-3,-3));
				}
				else if ( (state&WidgetState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 35, new Drawing.Margins(0,-1,-10,-10));
				}
				else
				{
					this.PaintImageButton(graphics, rect, 39, new Drawing.Margins(0,-1,-10,-10));
				}

				graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
				graphics.RenderSolid(this.ColorBorder);
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
			this.PaintBackground(graphics, rect, rect, 0.95, 16.0, false);

			if ( dir == Direction.Up )
			{
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
				graphics.RenderSolid(this.ColorBorder);

				if ( this.metalRenderer )
				{
					rect.Bottom -= 10;
					rect.Top    += 10;
					this.PaintRectShadow(graphics, rect, 3, 0.4, 0.3, false);
				}
			}

			if ( dir == Direction.Left )
			{
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.RenderSolid(this.ColorBorder);
			}
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
			this.PaintMenuShadow(graphics, rect);

			rect.Inflate(-this.GeometryMenuShadow);
			this.PaintBackground(graphics, rect, rect, 1.05, 30.0, false);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.ColorBorder);
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
				if ( itemType != MenuItemType.Deselect )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);
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
			this.PaintGeneralTextLayout(graphics, Drawing.Rectangle.Infinite, pos, text, state, style, Drawing.Color.Empty);
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

			graphics.RenderSolid(this.ColorBorder);
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
			if ( dir == Direction.Down || dir == Direction.Up )
			{
				this.PaintImageButton(graphics, rect, 35, new Drawing.Margins(-1,-10,-14,-14));
			}
			else
			{
				this.PaintImageButton(graphics, rect, 34, new Drawing.Margins(-14,-14,-1,-10));
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
			if ( this.metalRenderer )
			{
				this.PaintRectTopShadow(graphics, rect);
			}
			else
			{
				this.PaintBackground(graphics, rect, rect, 0.95, 12.0, false);
			}

			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.RenderSolid(this.ColorBorder);
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

			if ( this.metalRenderer )
			{
				rect.Deflate(1);

				double radius = this.RetRadiusFrame(rect);
				Drawing.Path pInside = this.PathRoundRectangle(rect, radius);

				graphics.Rasterizer.AddSurface(pInside);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.6, 1,1,1));

				this.PaintRoundTopShadow(graphics, rect);

				graphics.Rasterizer.AddOutline(pInside);
				graphics.RenderSolid(this.ColorBorder);

				this.PaintRoundShadow(graphics, rect, 1, 0.3, 0.7, true);
			}
			else
			{
				double radius = this.RetRadiusFrame(rect);
				Drawing.Path pInside = this.PathRoundRectangle(rect, radius);

				graphics.Rasterizer.AddSurface(pInside);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.2, 1,1,1));

				graphics.Rasterizer.AddOutline(pInside);
				graphics.RenderSolid(this.ColorBorder);
			}
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
			if ( !color.IsEmpty && (state&WidgetState.Enabled) != 0 )
			{
				Drawing.Path path = new Drawing.Path();
				path.AppendCircle(rect.Center, rect.Width/2, rect.Height/2);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(color);
			}

			if ( (state&WidgetState.Enabled) != 0 )
			{
				this.PaintImageButton(graphics, rect, 41);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 43);
			}

			if ( (state&WidgetState.Engaged) != 0 ||  // bouton pressé ?
				 (state&WidgetState.Entered) != 0 )   // bouton survolé ?
			{
				this.PaintImageButton(graphics, rect, 42);
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
			graphics.RenderSolid(this.colorCaption);
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
				else
				{
					if ( this.metalRenderer &&
						 (style == PaintTextStyle.StaticText  ||
						  style == PaintTextStyle.CheckButton ||
						  style == PaintTextStyle.RadioButton ||
						  style == PaintTextStyle.Group       ||
						  style == PaintTextStyle.HMenu       ) )
					{
						pos.Y --;
						text.Paint(pos, graphics, clipRect, Drawing.Color.FromBrightness(1), Drawing.GlyphPaintStyle.Disabled);
						pos.Y ++;
					}
					text.Paint(pos, graphics, clipRect, Drawing.Color.Empty, Drawing.GlyphPaintStyle.Normal);
				}
			}
			else
			{
				text.Paint(pos, graphics, clipRect, this.colorDisabled, Drawing.GlyphPaintStyle.Disabled);
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

		// Crée le chemin d'un rectangle à coins arrondis en forme de "D".
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
			return System.Math.Min(rect.Width, rect.Height)/2;
		}

		// Retourne le rayon à utiliser pour une zone rectangulaire.
		protected double RetRadiusFrame(Drawing.Rectangle rect)
		{
			double dim = System.Math.Min(rect.Width, rect.Height);
			double radius = System.Math.Min(dim/2, 5);
			double middle = System.Math.Max(dim-radius*2, 2);
			return System.Math.Floor((dim-middle)/2);
		}


		// Dessine un bouton composé plusieurs morceaux d'image.
		protected void PaintImageButton(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int rank)
		{
			this.PaintImageButton(graphics, rect, rank, new Drawing.Margins(0,0,0,0));
		}

		// Dessine un bouton composé plusieurs morceaux d'image.
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

			if ( rank < 16 || rank == 48 || rank == 50 || rank == 72 )
			{
				icon.Width *= 2;
				this.PaintImageButton3h(graphics, rect, icon);
			}
			else if ( rank < 32 || rank == 52 || rank == 53 )
			{
				icon.Bottom -= icon.Height;
				this.PaintImageButton3v(graphics, rect, icon);
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
			if ( !rect.IsSurfaceZero )
			{
				graphics.PaintImage(this.bitmap, rect, icon);
			}
		}

		// Dessine un bouton composé de 3 morceaux d'image horizontaux.
		protected void PaintImageButton3h(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon)
		{
			double rectMargin = System.Math.Min(rect.Height/2, rect.Width/2);
			double iconMargin = icon.Width/4;
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

		// Dessine un bouton composé de 3 morceaux d'image verticaux.
		protected void PaintImageButton3v(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  Drawing.Rectangle icon)
		{
			double rectMargin = System.Math.Min(rect.Width/2, rect.Height/2);
			double iconMargin = icon.Height/4;
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

		// Dessine l'ombre sous un menu.
		protected void PaintMenuShadow(Drawing.Graphics graphics, Drawing.Rectangle rect)
		{
			Drawing.Margins margins = this.GeometryMenuShadow;
			Drawing.Rectangle part = new Drawing.Rectangle();

			// Dessine le coin sup/gauche.
			part.Left   = rect.Left;
			part.Width  = margins.Left;
			part.Bottom = rect.Top-margins.Bottom;
			part.Height = margins.Bottom;
			this.PaintImageButton(graphics, part, 74, new Drawing.Margins(0,-26,0,-26));

			// Dessine le coin sup/droite.
			part.Left   = rect.Right-margins.Right;
			part.Right  = rect.Right;
			this.PaintImageButton(graphics, part, 74, new Drawing.Margins(-26,0,0,-26));

			// Dessine la bande gauche.
			part.Left   = rect.Left;
			part.Width  = margins.Left;
			part.Bottom = rect.Bottom+margins.Bottom;
			part.Top    = rect.Top-margins.Bottom;
			this.PaintImageButton(graphics, part, 74, new Drawing.Margins(0,-26,-6,-6));

			// Dessine la bande droite.
			part.Left   = rect.Right-margins.Right;
			part.Right  = rect.Right;
			this.PaintImageButton(graphics, part, 74, new Drawing.Margins(-26,0,-6,-6));

			// Dessine le coin inf/gauche.
			part.Left   = rect.Left;
			part.Width  = margins.Left;
			part.Bottom = rect.Bottom;
			part.Height = margins.Bottom;
			this.PaintImageButton(graphics, part, 74, new Drawing.Margins(0,-26,-26,0));

			// Dessine la bande inférieure.
			part.Left   = rect.Left+margins.Left;
			part.Right  = rect.Right-margins.Right;
			this.PaintImageButton(graphics, part, 74, new Drawing.Margins(-6,-6,-26,0));

			// Dessine le coin inf/droite.
			part.Left   = rect.Right-margins.Right;
			part.Right  = rect.Right;
			this.PaintImageButton(graphics, part, 74, new Drawing.Margins(-26,0,-26,0));
		}

		// Dessine un fond de fenêtre hachuré horizontalement.
		protected void PaintBackground(Drawing.Graphics graphics,
									   Drawing.Rectangle windowRect,
									   Drawing.Rectangle paintRect,
									   double lightning,
									   double topShadow,
									   bool mainWindow)
		{
			if ( this.metalRenderer )
			{
				if ( mainWindow )
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
								graphics.PaintImage(this.dynamicReflect ? this.metal2 : this.metal1, rect, new Drawing.Rectangle(0,0,dx,dy));
							}
						}
					}
					if ( this.dynamicReflect )
					{
						this.PaintImageButton(graphics, windowRect, 58);
					}
					this.PaintRectShadow(graphics, windowRect, 3, 0.4, 0.3, false);
					topShadow = 0;
				}
				else
				{
					graphics.AddFilledRectangle(paintRect);
					graphics.RenderSolid(Drawing.Color.FromBrightness(0.95));
				}
			}
			else
			{
				double l1 = System.Math.Min((245.0/255.0)*lightning, 1.0);
				double l2 = System.Math.Min((238.0/255.0)*lightning, 1.0);

				graphics.AddFilledRectangle(paintRect);
				graphics.RenderSolid(Drawing.Color.FromBrightness(l1));

				double h=2;
				double offset = (paintRect.Bottom-windowRect.Bottom)%4;
				for ( double y=paintRect.Bottom ; y<paintRect.Top+h*2 ; y+=h*2 )
				{
					double y1 = y-offset;
					double y2 = y1+h;

					y1 = System.Math.Max(y1, paintRect.Bottom);
					y2 = System.Math.Min(y2, paintRect.Top);

					if ( y1 < y2 )
					{
						graphics.AddFilledRectangle(new Drawing.Rectangle(paintRect.Left, y1, paintRect.Width, y2-y1));
					}
				}
				graphics.RenderSolid(Drawing.Color.FromBrightness(l2));
			}

			if ( topShadow > 0.0 )
			{
				Drawing.Rectangle rect = windowRect;
				rect.Bottom = rect.Top-topShadow;
				if ( rect.IntersectsWith(paintRect) )
				{
					this.PaintImageButton(graphics, rect, 57);
				}
			}
		}

		// Dessine une ombre autour d'un rectangle.
		protected void PaintRectShadow(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   int deep,
									   double alphaTop, double alphaBottom,
									   bool hole)
		{
			if ( !this.metalRenderer )  return;

			double incTop = alphaTop/(double)deep;
			double incBottom = alphaBottom/(double)deep;

			for ( int i=0 ; i<deep ; i++ )
			{
				if ( hole )  rect.Inflate(1);

				graphics.AddLine(rect.Left+0.5, rect.Bottom, rect.Left+0.5, rect.Top-1.0);
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top-1.0);
				graphics.AddLine(rect.Left+1.0, rect.Bottom+0.5, rect.Right-1.0, rect.Bottom+0.5);
				if ( hole )  graphics.RenderSolid(Drawing.Color.FromARGB(alphaBottom, 1,1,1));
				else         graphics.RenderSolid(Drawing.Color.FromARGB(alphaBottom, 0,0,0));

				graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
				if ( hole )  graphics.RenderSolid(Drawing.Color.FromARGB(alphaTop, 0,0,0));
				else         graphics.RenderSolid(Drawing.Color.FromARGB(alphaTop, 1,1,1));

				if ( !hole )  rect.Deflate(1);
				alphaTop -= incTop;
				alphaBottom -= incBottom;
			}
		}

		// Dessine une ombre autour d'un rectangle arrondi.
		protected void PaintRoundShadow(Drawing.Graphics graphics,
										Drawing.Rectangle rect,
										int deep,
										double alphaTop, double alphaBottom,
										bool hole)
		{
			if ( !this.metalRenderer )  return;

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
			graphics.Rasterizer.FillMode = Drawing.FillMode.NonZero;
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


		public void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled )
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.25;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.4, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromBrightness(intensity);
				color.A = alpha;
			}
		}

		public Drawing.Color ColorDisabled
		{
			get { return this.colorDisabled; }
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
			get
			{
				if ( this.metalRenderer )  return Drawing.Color.FromBrightness(0.55);
				return this.colorBorder;
			}
		}

		public Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.ColorBorder;
		}

		public Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.ColorBorder;
		}

		public double AlphaVMenu { get { return 0.9; } }

		public Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(5,5,0,7); } }
		public Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(1,1,6,6); } }
		public Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryRadioShapeBounds { get { return new Drawing.Margins(0,0,4,0); } }
		public Drawing.Margins GeometryGroupShapeBounds { get { return new Drawing.Margins(0,0,0,1); } }
		public Drawing.Margins GeometryToolShapeBounds { get { return new Drawing.Margins(0,0,0,0); } }
		public Drawing.Margins GeometryButtonShapeBounds { get { return new Drawing.Margins(2,2,0,5); } }
		public Drawing.Margins GeometryTextFieldShapeBounds { get { return new Drawing.Margins(1,1,1,1); } }
		public Drawing.Margins GeometryListShapeBounds { get { return new Drawing.Margins(2,2,2,2); } }
		public double GeometryComboRightMargin { get { return 0; } }
		public double GeometryComboBottomMargin { get { return 0; } }
		public double GeometryComboTopMargin { get { return 0; } }
		public double GeometryUpDownRightMargin { get { return 0; } }
		public double GeometryUpDownBottomMargin { get { return 0; } }
		public double GeometryUpDownTopMargin { get { return 0; } }
		public double GeometryScrollerRightMargin { get { return 0; } }
		public double GeometryScrollerBottomMargin { get { return 0; } }
		public double GeometryScrollerTopMargin { get { return 0; } }
		public double GeometryScrollListXMargin { get { return 1; } }
		public double GeometryScrollListYMargin { get { return 1; } }
		public double GeometrySliderLeftMargin { get { return 4; } }
		public double GeometrySliderRightMargin { get { return 0; } }
		public double GeometrySliderBottomMargin { get { return 0; } }

		protected Drawing.Image		bitmap;
		protected Drawing.Image		metal1;
		protected Drawing.Image		metal2;
		protected bool				metalRenderer;
		protected bool				dynamicReflect;
		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorControl;
		protected Drawing.Color		colorCaption;
		protected Drawing.Color		colorCaptionNF;  // NF = no focused
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorInfo;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorDisabled;
		protected Drawing.Color		colorWindow;
	}

	public class LookAquaMetal : LookAqua
	{
		public LookAquaMetal()
		{
			this.metalRenderer = true;
			this.dynamicReflect = false;
			this.RefreshColors();
		}
	}

	public class LookAquaDyna : LookAqua
	{
		public LookAquaDyna()
		{
			this.metalRenderer = true;
			this.dynamicReflect = true;
			this.RefreshColors();
		}
	}
}
