namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.LookBusiness implémente un décorateur sobre pour des applications commerciales.
	/// Les sélections sont orangées et les hilites bleutés.
	/// </summary>
	public class LookBusiness : AbstractAdorner
	{
		public LookBusiness()
		{
			this.bitmap = Drawing.Bitmap.FromManifestResource ("Epsitec.Common.Widgets.Adorners.Resources", "LookBusiness.png", typeof (IAdorner));
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs.
			this.colorBlack             = Drawing.Color.FromHexa ("000000");  // noir
			this.colorGlyph             = Drawing.Color.FromHexa ("5b6473");  // violet foncé
			this.colorWindow            = Drawing.Color.FromHexa ("ebe9ed");  // violet très clair
			this.colorFrame             = Drawing.Color.FromHexa ("fafbff");  // violet très très clair
			this.colorControl           = Drawing.Color.FromHexa ("6b90bd");  // bleu sombre
			this.colorGreen             = Drawing.Color.FromHexa ("21a121");  // vert foncé
			this.colorBorder            = Drawing.Color.FromHexa ("8599b1");  // bleu terne
			this.colorBorderLight       = Drawing.Color.FromHexa ("d2d2d4");  // gris-bleu clair
			this.colorBorderButton      = Drawing.Color.FromHexa ("2b4f82");  // bleu foncé
			this.colorEntered           = Drawing.Color.FromHexa ("afc6e1");  // bleu léger
			this.colorDefault           = Drawing.Color.FromHexa ("ffba01");  // orange
			this.colorSelected          = Drawing.Color.FromHexa ("ffba01");  // orange
			this.colorDisabled          = Drawing.Color.FromHexa ("c6c5c9");  // gris-bleu
			this.colorCaption           = Drawing.Color.FromHexa ("ffd672");  // orange
			this.colorCaptionNF         = Drawing.Color.FromHexa ("ffba49");  // orange soutenu
			this.colorCaptionLight      = Drawing.Color.FromHexa ("ffe39d");  // orange léger
			this.colorCaptionText       = Drawing.Color.FromHexa ("000000");  // noir
			this.colorCaptionProposal   = Drawing.Color.FromHexa ("9a774a");  // brun
			this.colorHilite            = Drawing.Color.FromHexa ("afc6e1");  // bleu léger
			this.colorWhite             = Drawing.Color.FromHexa ("ffffff");  // blanc
			this.colorError             = Drawing.Color.FromHexa ("ffb1b1");  // rouge pâle
			this.colorUndefinedLanguage = Drawing.Color.FromHexa ("b1e3ff");  // bleu pâle
			this.colorTextBackground    = Drawing.Color.FromHexa ("f7f6f8");  // violet très très clair
			this.colorInfo              = Drawing.Color.FromName ("Info");  // jaune pâle
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
			Drawing.Color color = this.colorGlyph;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRgb(0.5, 0.0, 0.0);  // rouge foncé
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRgb(0.0, 0.5, 0.0);  // vert foncé
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

			Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);

			if ( rect.Width < 10 )
			{
				rect.Width  *= 1.5;
				rect.Height *= 1.5;
			}

			using (Drawing.Path path = new Drawing.Path ())
			{
				AbstractAdorner.GenerateGlyphShape (rect, type, center, path);
				
				path.Close ();
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (color);
			}
		}

		public override void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton à cocher sans texte.
			rect = graphics.Align (rect);
			Drawing.Rectangle rInside;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rect, 13);
				}
				else
				{
					this.PaintImageButton(graphics, rect, 21);
				}

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					rInside = rect;
					rInside.Deflate(1.5);
					graphics.LineWidth = 2;
					graphics.AddRectangle(rInside);
					graphics.RenderSolid(this.colorHilite);
				}
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWhite);
			}

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			rInside = rect;
			rInside.Deflate(0.5);
			graphics.AddRectangle(rInside);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorderButton);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				Drawing.Point center = rect.Center;
				using (Drawing.Path path = new Drawing.Path ())
				{
					path.MoveTo (center.X-rect.Width*0.1, center.Y-rect.Height*0.1);
					path.LineTo (center.X+rect.Width*0.3, center.Y+rect.Height*0.3);
					path.LineTo (center.X+rect.Width*0.3, center.Y+rect.Height*0.1);
					path.LineTo (center.X-rect.Width*0.1, center.Y-rect.Height*0.3);
					path.LineTo (center.X-rect.Width*0.3, center.Y-rect.Height*0.1);
					path.LineTo (center.X-rect.Width*0.3, center.Y+rect.Height*0.1);
					path.Close ();
					graphics.Rasterizer.AddSurface (path);
				}
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorGreen);
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
					graphics.RenderSolid(this.colorGreen);
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
			rect = graphics.Align (rect);
			Drawing.Rectangle rInside;

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				this.PaintCircle(graphics, rect, this.colorBorderButton);
			}
			else
			{
				this.PaintCircle(graphics, rect, this.colorDisabled);
			}

			rInside = rect;
			rInside.Deflate(1);

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
				this.PaintCircle(graphics, rInside, this.colorHilite);
				rInside.Deflate(1);
			}

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 14);
				}
				else
				{
					this.PaintImageButton(graphics, rInside, 22);
				}
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorWhite);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorGreen);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorDisabled);
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
			if ((state&WidgetPaintState.Furtive) != 0 &&
				(state&WidgetPaintState.Entered) == 0 &&
				(state&WidgetPaintState.Focused) == 0)
			{
				return;
			}

			Drawing.Rectangle rFocus = rect;
			if ( System.Math.Min(rect.Width, rect.Height) < 16 )
			{
				rFocus.Deflate(0.5);
			}
			else
			{
				rFocus.Deflate(1.5);
			}

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel ||
				 style == ButtonStyle.DefaultAcceptAndCancel )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
					{
						rFocus.Deflate(1);
						rInside.Deflate(1);
					}

					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 1);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 0);
					}

					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						rect.Deflate(1.0);
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 3.0);
						graphics.RenderSolid(this.colorEntered);
						rect.Inflate(1.0);
					}
					else if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
					{
						rect.Deflate(1.0);
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 3.0);
						graphics.RenderSolid(this.colorDefault);
						rect.Inflate(1.0);
					}

					{
						double radius = this.RetRadius(rect);
						Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
						graphics.Rasterizer.AddOutline(pTitle, 1.0);
						graphics.RenderSolid(this.colorBorderButton);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 2);

					double radius = this.RetRadius(rect);
					Drawing.Path pTitle = this.PathRoundRectangle(rect, radius);
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorDisabled);
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
				int add = 1;
				if ( style == ButtonStyle.Scroller )
				{
					rect.Deflate(1.0);
					if ( dir == Direction.Up || dir == Direction.Down )  add = 0;
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintImageButton(graphics, rect, 28+add);

					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						if (rect.Width > 10 && rect.Height > 10)
						{
							rect.Deflate (1.0);
							double radius = this.RetRadius (rect);
							Drawing.Path pTitle = this.PathRoundRectangle (rect, radius);
							graphics.Rasterizer.AddOutline (pTitle, 3.0);
							graphics.RenderSolid (this.colorEntered);
							rect.Inflate (1.0);
						}

						{
							Drawing.Path pTitle = this.PathRoundRectangle (rect, 2);
							graphics.Rasterizer.AddOutline (pTitle, 1.0);
							graphics.RenderSolid (this.colorBorderButton);
						}
					}
					else
					{
						Drawing.Path pTitle = this.PathRoundRectangle(rect, 2);
						graphics.Rasterizer.AddOutline(pTitle, 1.0);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 30+add);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorDisabled);
				}

				if ( style == ButtonStyle.Icon )  rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.Slider )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
					{
						this.PaintImageButton(graphics, rect, 33);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 32);
					}

					if ((state&WidgetPaintState.Entered) != 0)  // bouton survolé ?
					{
						graphics.LineWidth = 3.0;
						graphics.AddCircle(rect.Center, rect.Width/2-1.5, rect.Height/2-1.5);
						graphics.LineWidth = 1.0;
						graphics.RenderSolid(this.colorEntered);

						graphics.AddCircle(rect.Center, rect.Width/2-0.5, rect.Height/2-0.5);
						graphics.RenderSolid(this.colorBorderButton);
					}
					else
					{
						graphics.AddCircle(rect.Center, rect.Width/2-0.5, rect.Height/2-0.5);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					this.PaintImageButton(graphics, rect, 34);

					graphics.AddCircle(rect.Center, rect.Width/2-0.5, rect.Height/2-0.5);
					graphics.RenderSolid(this.colorDisabled);
				}

				if ( style == ButtonStyle.Icon )  rFocus.Inflate(1.0);
			}
			else if ( style == ButtonStyle.ToolItem )
			{
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 10);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 9);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				rFocus.Deflate(1.0);
			}
			else if ( style == ButtonStyle.ComboItem )
			{
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rInside, 10);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						if ((state&WidgetPaintState.InheritedEnter) == 0)
						{
							this.PaintImageButton(graphics, rInside, 8);
						}

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 9);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
				}
				rFocus.Deflate(1.0);
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				if ( AbstractAdorner.IsThreeState2(state) )
				{
					rect.Top += 2;
					rInside.Top += 2;
					rFocus.Top +=2;
				}

				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 11);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 10);
					}
				}
				else if ( (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 15);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 15);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 12);
					}
				}
				else
				{
					if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
					{
						this.PaintImageButton(graphics, rInside, 8);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
					{
						this.PaintImageButton(graphics, rInside, 9);

						rect.Deflate(0.5);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.colorBorder);
					}
					else
					{
						this.PaintImageButton(graphics, rect, 8);
					}
				}
				rFocus.Deflate(1.0);
			}
			else if ( style == ButtonStyle.Confirmation )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					this.PaintImageButton(graphics, rInside, 17);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					this.PaintImageButton(graphics, rInside, 18);

					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.colorBorder);
				}
				rFocus.Deflate(1.0);
			}
			else if (style == ButtonStyle.ListItem)
			{
				this.PaintImageButton (graphics, rect, 8);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 8);
			}

			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				this.PaintFocusBox(graphics, rFocus);
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

			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
			{
				pos.X ++;
				pos.Y --;
			}
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
			var frame = AbstractAdorner.GetMultilingualFrame (rect, isMultilingual);

			if ((state&WidgetPaintState.Furtive) != 0 &&
				(state&WidgetPaintState.Entered) == 0 &&
				(state&WidgetPaintState.Focused) == 0)
			{
				graphics.Rasterizer.AddOutline (frame);
				graphics.RenderSolid (this.colorBorder);
				return;
			}

			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Multiline  ||
				 style == TextFieldStyle.Combo  ||
				 style == TextFieldStyle.UpDown )
			{
				if ((state&WidgetPaintState.Enabled) != 0)  // bouton enable ?
				{
					Drawing.Color color = this.ColorTextDisplayMode(mode);
					if ((state&WidgetPaintState.Error) != 0)
					{
						graphics.Rasterizer.AddSurface (frame);
						graphics.RenderSolid (this.colorError);
					}
					else if ((state&WidgetPaintState.UndefinedLanguage) != 0)
					{
						graphics.Rasterizer.AddSurface (frame);
						graphics.RenderSolid (this.colorUndefinedLanguage);
					}
					else if (!color.IsEmpty)
					{
						graphics.Rasterizer.AddSurface (frame);
						graphics.RenderSolid(color);
					}
					else
					{
						graphics.Rasterizer.AddSurface (frame);
						graphics.RenderSolid (readOnly ? Drawing.Color.FromHexa ("e2eeff") : Drawing.Color.FromHexa ("f8f8f9"));
					}

					graphics.Rasterizer.AddOutline (frame);
					graphics.RenderSolid (this.colorBorder);
				}
				else
				{
					graphics.Rasterizer.AddSurface (frame);
					graphics.RenderSolid (this.colorWhite);

					graphics.Rasterizer.AddOutline (frame);
					graphics.RenderSolid (this.colorDisabled);
				}
			}
			else if ( style == TextFieldStyle.Simple )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					this.PaintImageButton(graphics, rect, 16);

					graphics.Rasterizer.AddOutline (frame);
					graphics.RenderSolid (this.colorBorder);
				}
				else
				{
					graphics.Rasterizer.AddSurface (frame);
					graphics.RenderSolid (this.colorWhite);

					graphics.Rasterizer.AddOutline (frame);
					graphics.RenderSolid (this.colorDisabled);
				}
			}
			else
			{
				graphics.Rasterizer.AddSurface (frame);
				graphics.RenderSolid (this.colorWhite);
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
			if ( dir == Direction.Up || dir == Direction.Down )  // ascenseur vertical ?
			{
				this.PaintImageButton(graphics, frameRect, 24);
			}
			else	// ascenseur horizontal ?
			{
				this.PaintImageButton(graphics, frameRect, 25);
			}

			if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
			{
				if ( dir == Direction.Up || dir == Direction.Down )  // ascenseur vertical ?
				{
					tabRect.Left   += 1.0;
					tabRect.Right  -= 1.0;
					tabRect.Top    += 2.0;
					tabRect.Bottom -= 2.0;
					this.PaintImageButton(graphics, tabRect, 26);
				}
				else	// ascenseur horizontal ?
				{
					tabRect.Top    -= 1.0;
					tabRect.Bottom += 1.0;
					tabRect.Left   -= 2.0;
					tabRect.Right  += 2.0;
					this.PaintImageButton(graphics, tabRect, 27);
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
			if ( dir == Direction.Up || dir == Direction.Down )
			{
				thumbRect.Top += 2.0;
				thumbRect.Bottom -= 2.0;
			}
			else
			{
				thumbRect.Left -= 2.0;
				thumbRect.Right += 2.0;
			}
			state &= ~WidgetPaintState.Engaged;
			this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

			Drawing.Rectangle	rect;
			Drawing.Point		center;

			if ( (state&WidgetPaintState.Enabled) != 0 )
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
								graphics.AddLine(center.X-rect.Width*0.15, y, center.X+rect.Width*0.15, y);
								y += 2;
							}
							graphics.RenderSolid(this.colorBorder);

							y = center.Y-4+1;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(center.X-rect.Width*0.15-1, y, center.X+rect.Width*0.15-1, y);
								y += 2;
							}
							graphics.RenderSolid(this.colorWhite);
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
								graphics.AddLine(x, center.Y-rect.Height*0.15, x, center.Y+rect.Height*0.15);
								x += 2;
							}
							graphics.RenderSolid(this.colorBorder);

							x = center.X-4;
							for ( int i=0 ; i<4 ; i++ )
							{
								graphics.AddLine(x, center.Y-rect.Height*0.15+1, x, center.Y+rect.Height*0.15+1);
								x += 2;
							}
							graphics.RenderSolid(this.colorWhite);
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
			bool enabled = ( (state&WidgetPaintState.Enabled) != 0 );

			if ( dir == Widgets.Direction.Left )
			{
				Drawing.Point p1 = graphics.Align(new Drawing.Point (sliderRect.Left +frameRect.Height*0.2, frameRect.Center.Y));
				Drawing.Point p2 = graphics.Align(new Drawing.Point (sliderRect.Right-frameRect.Height*0.2, frameRect.Center.Y));

				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X-0.5, p2.Y+0.5);
				graphics.RenderSolid(enabled ? this.colorBorder : this.colorDisabled);
				graphics.AddLine(p1.X+0.5, p1.Y-0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorWhite);

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					graphics.AddLine(tabRect.Left, p1.Y+0.5, tabRect.Right, p2.Y+0.5);
					graphics.AddLine(tabRect.Left, p1.Y-0.5, tabRect.Right, p2.Y-0.5);
					graphics.RenderSolid(this.colorCaption);
				}
			}
			else
			{
				Drawing.Point p1 = graphics.Align(new Drawing.Point (frameRect.Center.X, sliderRect.Bottom+frameRect.Width*0.2));
				Drawing.Point p2 = graphics.Align(new Drawing.Point (frameRect.Center.X, sliderRect.Top   -frameRect.Width*0.2));

				graphics.AddLine(p1.X-0.5, p1.Y+0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(enabled ? this.colorBorder : this.colorDisabled);
				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X+0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorWhite);

				if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
				{
					graphics.AddLine(p1.X-0.5, tabRect.Bottom, p2.X-0.5, tabRect.Top);
					graphics.AddLine(p1.X+0.5, tabRect.Bottom, p2.X+0.5, tabRect.Top);
					graphics.RenderSolid(this.colorCaption);
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
			thumbRect.Inflate(1);

			if ( dir == Widgets.Direction.Left )
			{
				state &= ~WidgetPaintState.Engaged;
				this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

				double d = thumbRect.Width/2+1;
				graphics.AddLine(thumbRect.Center.X, thumbRect.Bottom+d, thumbRect.Center.X, thumbRect.Top-d);
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				state &= ~WidgetPaintState.Engaged;
				this.PaintButtonBackground(graphics, thumbRect, state, dir, ButtonStyle.Scroller);

				double d = thumbRect.Height/2+1;
				graphics.AddLine(thumbRect.Left+d, thumbRect.Center.Y, thumbRect.Right-d, thumbRect.Center.Y);
				graphics.RenderSolid(this.colorBorder);
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
			this.PaintImageButton(graphics, rect, 1);

			double radius = this.RetRadius(rect);
			Drawing.Path path = this.PathRoundRectangle(rect, radius);
			graphics.Rasterizer.AddOutline(path, 1);
			graphics.RenderSolid(this.colorBorder);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(3);
			if (style == ProgressIndicatorStyle.UnknownDuration)
			{
				double x = rInside.Width*progress;
				double w = rInside.Width*0.4;

				this.PaintProgressUnknow(graphics, rInside, w, x-w);
				this.PaintProgressUnknow(graphics, rInside, w, x-w+rInside.Width);
			}
			else
			{
				if (progress != 0)
				{
					rInside.Width *= progress;
					this.PaintImageButton(graphics, rInside, 11);
				}
			}

			rInside = rect;
			rInside.Deflate(3.5);
			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorBorder);
		}

		protected void PaintProgressUnknow(Drawing.Graphics graphics, Drawing.Rectangle rect, double w, double x)
		{
			Drawing.Rectangle fill = new Drawing.Rectangle(rect.Left+x, rect.Bottom, w, rect.Height);
			Drawing.Rectangle icon = new Drawing.Rectangle(0, 0, 32*4, 32);

			if (fill.Left < rect.Left)
			{
				double over = (rect.Left-fill.Left)/w;
				icon.Left += icon.Width*over;
				fill.Left = rect.Left;
			}

			if (fill.Right > rect.Right)
			{
				double over = (fill.Right-rect.Right)/w;
				icon.Width -= icon.Width*over;
				fill.Right = rect.Right;
			}

			if (fill.Width > 0)
			{
				graphics.PaintImage(this.bitmap, fill, icon);
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorderLight);
			}
			else
			{
				graphics.RenderSolid(this.colorWhite);
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);
		}

		public override void PaintTabFrame(Drawing.Graphics graphics,
								  Drawing.Rectangle rect,
								  Widgets.WidgetPaintState state,
								  Widgets.Direction dir)
		{
			//	Dessine la zone principale sous les onglets.
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}

			rect.Deflate(0.5);
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorFrame);
		}


		public override Drawing.Color ColorTabBackground
		{
			get
			{
				return this.colorFrame;
			}
		}
		
		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine l'onglet devant les autres.
			titleRect.Bottom += 1;

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			graphics.Rasterizer.AddSurface(pTitle);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorFrame);

				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorSelected);

				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorWhite);

				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorDisabled);
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

			double radius = System.Math.Min(titleRect.Width, titleRect.Height)/8;
			Drawing.Path pTitle = this.PathTopRoundRectangle(titleRect, radius);

			this.PaintImageButton(graphics, titleRect, 18);

			if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
			{
				Drawing.Rectangle rHilite = titleRect;
				rHilite.Bottom = rHilite.Top-3;
				Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
				graphics.Rasterizer.AddSurface(pHilite);
				graphics.RenderSolid(this.colorHilite);
			}

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.Rasterizer.AddOutline(pTitle, 1);
				graphics.RenderSolid(this.colorDisabled);
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
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWhite);
			}
			else
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorWhite);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorderLight);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
		}

		public override void PaintArrayForeground(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 WidgetPaintState state)
		{
			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				rect.Deflate(1.0);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorCaption);
			}
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
					Drawing.Rectangle rTitle = rect;
					rTitle.Height = 3;
					rTitle.Left += 2;
					rTitle.Right -= 2;
					graphics.AddFilledRectangle(rTitle);
					graphics.RenderSolid(this.colorHilite);
				}

#if false
				graphics.AddLine(rect.Left+0.5, rect.Bottom+2.0, rect.Left+0.5, rect.Top-3.0);
#else
				graphics.AddLine(rect.Left+1.5, rect.Bottom+0.5, rect.Left+1.5, rect.Top-0.5);
				graphics.AddLine(rect.Right-1.5, rect.Bottom+0.5, rect.Right-1.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+1.5, rect.Top-0.5, rect.Right-1.5, rect.Top-0.5);
#endif
				graphics.RenderSolid(this.colorBorderLight);
			}

			if ( dir == Direction.Left )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rTitle = rect;
					rTitle.Left = rTitle.Right-3;
					rTitle.Bottom += 2;
					rTitle.Top -= 2;
					graphics.AddFilledRectangle(rTitle);
					graphics.RenderSolid(this.colorHilite);
				}

#if false
				graphics.AddLine(rect.Left+3.0, rect.Top-0.5, rect.Right-2.0, rect.Top-0.5);
#else
				graphics.AddLine(rect.Left+0.5, rect.Top-0.5, rect.Right-0.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Right-0.5, rect.Bottom+0.5);
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Left+0.5, rect.Top-0.5);
#endif
				graphics.RenderSolid(this.colorBorderLight);
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
			if ( dir == Direction.Up )
			{
				graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
				graphics.RenderSolid(this.colorWhite);
			}

			if ( dir == Direction.Left )
			{
				graphics.AddLine(rect.Right-0.5, rect.Bottom, rect.Right-0.5, rect.Top);
				graphics.RenderSolid(this.colorWhite);
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorWhite);

			if ( iconWidth > 0 )
			{
				Drawing.Rectangle band = rect;
				band.Width = iconWidth;
				band.Top -= 1;
				band.Bottom += 1;
				graphics.AddFilledRectangle(band);
				graphics.RenderSolid(this.colorWindow);
			}

			rect.Deflate(0.5);
			if ( parentRect.IsSurfaceZero )
			{
				graphics.AddRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else
			{
				graphics.AddLine(rect.Left, rect.Top+0.5, rect.Left, rect.Bottom-0.5);
				graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
				graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top+0.5);
				graphics.AddLine(parentRect.Right-0.5, rect.Top, rect.Right+0.5, rect.Top);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorDisabled);
				}

				graphics.AddLine(rect.Left+1, rect.Top, parentRect.Right-1.5, rect.Top);
				graphics.RenderSolid(this.colorWindow);
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
											MenuItemState itemType)
		{
			//	Dessine le fond d'une case de menu.
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == MenuOrientation.Horizontal )
				{
					if ( itemType == MenuItemState.Selected )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaptionLight);

						Drawing.Rectangle rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddRectangle(rInside);
						graphics.RenderSolid(this.colorCaption);
					}
					if ( itemType == MenuItemState.SubmenuOpen )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorWindow);

						Drawing.Rectangle rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddLine(rInside.Left, rInside.Bottom-0.5, rInside.Left, rInside.Top);
						graphics.AddLine(rInside.Left, rInside.Top, rInside.Right, rInside.Top);
						graphics.AddLine(rInside.Right, rInside.Top, rInside.Right, rInside.Bottom-0.5);
						graphics.RenderSolid(this.colorBorder);
					}
				}

				if ( type == MenuOrientation.Vertical )
				{
					if ( itemType != MenuItemState.Default )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.colorCaptionLight);

						Drawing.Rectangle rInside;
						rInside = rect;
						rInside.Deflate(0.5);
						graphics.AddRectangle(rInside);
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

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
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
			if ( dir == Direction.Down || dir == Direction.Up )
			{
				this.PaintImageButton(graphics, rect, 20);
			}
			else
			{
				this.PaintImageButton(graphics, rect, 19);
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorWhite);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
			}
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
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
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
			this.PaintImageButton(graphics, rect, 23);

			graphics.AddLine(rect.Left, rect.Top-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
			graphics.RenderSolid(this.colorBorder);
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
			double radius = 5.0;

			if ((state&WidgetPaintState.ActiveYes) != 0)   // bouton activé ?
			{
				Drawing.Path pTitle = this.PathTopRoundRectangle(rect, radius);

				graphics.Rasterizer.AddSurface(pTitle);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorFrame);

					Drawing.Rectangle rHilite = rect;
					rHilite.Bottom = rHilite.Top-3;
					Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
					graphics.Rasterizer.AddSurface(pHilite);
					graphics.RenderSolid (this.colorSelected);

					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.RenderSolid(this.colorWhite);

					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorDisabled);
				}
			}
			else
			{
				rect.Top    -= 2;
				rect.Bottom += 1;
				rect.Left   += 1;
				rect.Right  -= 1;

				Drawing.Path pTitle = this.PathTopRoundRectangle(rect, radius);

				this.PaintImageButton(graphics, rect, 18);

				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					Drawing.Rectangle rHilite = rect;
					rHilite.Bottom = rHilite.Top-3;
					Drawing.Path pHilite = this.PathTopRoundRectangle(rHilite, radius);
					graphics.Rasterizer.AddSurface(pHilite);
					graphics.RenderSolid(this.colorHilite);
				}

				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorBorder);
				}
				else
				{
					graphics.Rasterizer.AddOutline(pTitle, 1);
					graphics.RenderSolid(this.colorDisabled);
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
			if ( (state&WidgetPaintState.ActiveYes) == 0 )   // bouton désactivé ?
			{
				pos.Y -= 2;
			}
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
			Drawing.Path pFullRect = this.PathRoundRectangle(fullRect, 3.0);

			graphics.Rasterizer.AddSurface(pFullRect);
			graphics.RenderSolid(this.colorWindow);

			textRect.Top += 1;

			Drawing.Path pTextRect = this.PathBottomRoundRectangle(textRect, 3.0);
			Drawing.Color topColor = Drawing.Color.FromRgb(156.0/255.0, 179.0/255.0, 206.0/255.0);
			this.GradientPath(graphics, pTextRect, this.colorBorder, topColor, 0);

			if (text != null)
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);
				Drawing.Point pos = new Drawing.Point(textRect.Left+3, textRect.Bottom+1);
				text.LayoutSize = new Drawing.Size(textRect.Width-4, textRect.Height);
				text.Alignment = Drawing.ContentAlignment.MiddleCenter;
				text.BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.Split | Drawing.TextBreakMode.SingleLine;
				text.Paint (pos, graphics, Drawing.Rectangle.MaxValue, Drawing.Color.FromBrightness (1), Drawing.GlyphPaintStyle.Normal);
			}

			graphics.Rasterizer.AddOutline(pFullRect, 1.0);
			graphics.RenderSolid(this.colorBorder);
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
			if ( (state&WidgetPaintState.Enabled) == 0 )
			{
				Drawing.Color topColor    = Drawing.Color.FromRgb(221.0/255.0, 224.0/255.0, 227.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRgb(214.0/255.0, 216.0/255.0, 219.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}
			else if ( color.IsEmpty )
			{
				Drawing.Color topColor    = Drawing.Color.FromRgb(253.0/255.0, 253.0/255.0, 253.0/255.0);
				Drawing.Color bottomColor = Drawing.Color.FromRgb(205.0/255.0, 204.0/255.0, 223.0/255.0);
				this.Gradient(graphics, rect, bottomColor, topColor);
			}
			else
			{
				Drawing.Color topColor    = Drawing.Color.FromColor(color, 0.0);
				Drawing.Color bottomColor = Drawing.Color.FromColor(color, 0.5);
				this.Gradient(graphics, rect, bottomColor, topColor);
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
			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				graphics.RenderSolid(this.colorBorder);
			}
			else
			{
				graphics.RenderSolid(this.colorDisabled);
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
			graphics.RenderSolid (backColor.ColorOrDefault (this.colorInfo));  // fond jaune pale
			
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
			AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorCaption);
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
				graphics.AddFilledRectangle(areas[i].Rect);
				if ( (state&WidgetPaintState.Focused) != 0 )
				{
					if ( mode == TextFieldDisplayMode.InheritedValue )
					{
						graphics.RenderSolid(this.colorCaptionProposal);
					}
					else
					{
						graphics.RenderSolid(this.colorCaption);
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
			
			if ( style == PaintTextStyle.Group )
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBorderButton);
			}
			else
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);
			}

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

			double radius = System.Math.Min(7, System.Math.Min(rect.Width, rect.Height));
			radius = System.Math.Min(radius, startX);
			
			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+radius+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo(ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (endX, oy+dy-0.5);
			path.MoveTo (ox+radius+0.5, oy+dy-0.5);
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
			//	Crée le chemin d'un rectangle à coins arrondis en forme de "u" inversé.
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

		protected Drawing.Path PathBottomRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			//	Crée le chemin d'un rectangle à coins arrondis en forme de "u".
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			if ( radius == 0 )
			{
				radius = System.Math.Min(dx, dy)/8;
			}
			
			Drawing.Path path = new Drawing.Path();
			path.MoveTo (ox+0.5, oy+dy);
			path.LineTo (ox+0.5, oy+radius+0.5);
			path.CurveTo(ox+0.5, oy+0.5, ox+radius+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo(ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy);

			return path;
		}

		protected Drawing.Path PathTopRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle en forme de "U" inversé.
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

		protected Drawing.Path PathLeftRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle en forme de "D" inversé.
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

		protected Drawing.Path PathTopCornerRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle "corné" en forme de "U" inversé.
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

		protected Drawing.Path PathLeftCornerRectangle(Drawing.Rectangle rect)
		{
			//	Crée le chemin d'un rectangle "corné" en forme de "D" inversé.
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
//			t = t.RotateDeg(0, center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected void GradientPath(Drawing.Graphics graphics,
									Drawing.Path path,
									Drawing.Color bottomColor,
									Drawing.Color topColor,
									double angle)
		{
			graphics.Rasterizer.AddSurface(path);
			this.Gradient(graphics, path.ComputeBounds(), bottomColor, topColor, angle);
		}

		protected void GradientRect(Drawing.Graphics graphics,
									Drawing.Rectangle rect,
									Drawing.Color bottomColor,
									Drawing.Color topColor,
									double angle)
		{
			Drawing.Path path = new Drawing.Path();
			path.MoveTo(rect.BottomLeft);
			path.LineTo(rect.TopLeft);
			path.LineTo(rect.TopRight);
			path.LineTo(rect.BottomRight);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			this.Gradient(graphics, rect, bottomColor, topColor, angle);
		}

		protected void Gradient(Drawing.Graphics graphics,
								Drawing.Rectangle rect,
								Drawing.Color bottomColor,
								Drawing.Color topColor,
								double angle)
		{
			graphics.FillMode = Drawing.FillMode.NonZero;
			graphics.GradientRenderer.Fill = Drawing.GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Drawing.Transform ot = graphics.GradientRenderer.Transform;
			Drawing.Transform t = Drawing.Transform.Identity;
			Drawing.Point center = rect.Center;
			if ( angle == 0 )  t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			else               t = t.Scale(rect.Height/100/2, rect.Width/100/2);
			t = t.Translate (center);
			t = t.RotateDeg (angle, center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected double RetRadius(Drawing.Rectangle rect)
		{
			//	Retourne le rayon à utiliser pour une zone rectangulaire.
			double dim = System.Math.Min(rect.Width, rect.Height);
			return System.Math.Floor(System.Math.Max(3, dim/7));
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

			if ( rank ==  8 || rank ==  9 || rank == 10 || rank == 13 || rank == 14 ||
				 rank == 16 || rank == 17 || rank == 21 || rank == 22 || rank == 23 ||
				 rank == 32 || rank == 33 || rank == 34 )
			{
				this.PaintImageButton1(graphics, rect, icon);
			}
			else
			{
				this.PaintImageButton9 (graphics, rect, 2, icon, 2);
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
				intensity = System.Math.Min(intensity+0.3, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromAlphaRgb (alpha, intensity, intensity*1.02, intensity*1.05);  // bleuté
				color = color.ClipToRange();
			}

			return color;
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
				return this.colorDisabled;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return (enabled ? this.colorBorder : this.colorDisabled);
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return (enabled ? this.colorBorder : this.colorDisabled);
		}

		public override Drawing.Color ColorTextDisplayMode(TextFieldDisplayMode mode)
		{
			switch ( mode )
			{
				case TextFieldDisplayMode.Default:   return Drawing.Color.Empty;
				case TextFieldDisplayMode.OverriddenValue:   return Drawing.Color.FromRgb(171.0/255.0, 189.0/255.0, 211.0/255.0);
				case TextFieldDisplayMode.InheritedValue:  return Drawing.Color.FromRgb(221.0/255.0, 205.0/255.0, 188.0/255.0);
			}
			return Drawing.Color.Empty;
		}

		public override Drawing.Margins GeometryMenuMargins { get { return new Drawing.Margins(2,2,2,2); } }
		public override Drawing.Margins GeometryMenuShadow { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryArrayMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRadioShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryGroupShapeMargins { get { return new Drawing.Margins(0,0,3,0); } }
		public override Drawing.Margins GeometryToolShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryThreeStateShapeMargins { get { return new Drawing.Margins(0,0,2,0); } }
		public override Drawing.Margins GeometryButtonShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryRibbonShapeMargins { get { return new Drawing.Margins(0,0,0,2); } }
		public override Drawing.Margins GeometryTextFieldShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
		public override Drawing.Margins GeometryListShapeMargins { get { return new Drawing.Margins(0,0,0,0); } }
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
		public override double GeometryScrollListXMargin { get { return 2; } }
		public override double GeometryScrollListYMargin { get { return 2; } }
		public override double GeometrySliderLeftMargin { get { return 0; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 0; } }


		protected Drawing.Image		bitmap;
		protected Drawing.Color		colorGlyph;
		protected Drawing.Color		colorGreen;
		protected Drawing.Color		colorCaptionLight;
		protected Drawing.Color		colorCaptionProposal;
		protected Drawing.Color		colorHilite;
		protected Drawing.Color		colorBorder;
		protected Drawing.Color		colorBorderLight;
		protected Drawing.Color		colorBorderButton;
		protected Drawing.Color		colorEntered;
		protected Drawing.Color		colorDefault;
		protected Drawing.Color		colorDisabled;
		protected Drawing.Color		colorFrame;
		protected Drawing.Color		colorUndefinedLanguage;
		protected Drawing.Color		colorWindow;
		protected Drawing.Color		colorTextBackground;
		protected Drawing.Color		colorSelected;
	}
}
