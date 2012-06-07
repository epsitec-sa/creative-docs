//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorner.Default implémente le décorateur par défaut.
	/// </summary>
	public class Default : AbstractAdorner
	{
		public Default()
		{
			this.RefreshColors();
		}

		protected override void RefreshColors()
		{
			//	Initialise les couleurs en fonction des réglages de Windows.
			double r,g,b;

			this.colorBlack             = Drawing.Color.FromBrightness(0);
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
			this.colorError             = Drawing.Color.FromHexa ("ffb1b1");  // rouge pâle
			this.colorUndefinedLanguage = Drawing.Color.FromHexa ("b1e3ff");  // bleu pâle

			r = 1-(1-this.colorControlLight.R)/2;
			g = 1-(1-this.colorControlLight.G)/2;
			b = 1-(1-this.colorControlLight.B)/2;
			this.colorScrollerBack = Drawing.Color.FromRgb(r,g,b);

			r = 1-(1-this.colorControlLight.R)*0.7;
			g = 1-(1-this.colorControlLight.G)*0.7;
			b = 1-(1-this.colorControlLight.B)*0.7;
			this.colorControlReadOnly = Drawing.Color.FromRgb(r,g,b);
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

			if ( (state&WidgetPaintState.Enabled) != 0 )
			{
				if ( type == GlyphShape.Reject )  color = Drawing.Color.FromRgb(0.5, 0.0, 0.0);  // rouge foncé
				if ( type == GlyphShape.Accept )  color = Drawing.Color.FromRgb(0.0, 0.5, 0.0);  // vert foncé
			}
			else
			{
				color = this.colorControlDark;
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
				graphics.RenderSolid(Drawing.Color.FromRgb(this.colorWindow.R-0.2, this.colorWindow.G-0.2, this.colorWindow.B-0.2));

				graphics.AddLine(p.X-12.5, p.Y+1.5, p.X-1.5, p.Y+12.5);
				graphics.AddLine(p.X- 8.5, p.Y+1.5, p.X-1.5, p.Y+ 8.5);
				graphics.AddLine(p.X- 4.5, p.Y+1.5, p.X-1.5, p.Y+ 4.5);
				graphics.RenderSolid(Drawing.Color.FromBrightness(1.0));
				return;
			}

			Path path = Default.GetGlyphPath (rect, state, type);

			graphics.Rasterizer.AddSurface (path);
			path.Dispose();
			graphics.RenderSolid(color);
		}

		public static Path GetGlyphPath(Rectangle rect, WidgetPaintState state, GlyphShape glyphShape)
		{
			if (rect.Width > rect.Height)
			{
				rect.Left += (rect.Width-rect.Height)/2;
				rect.Width = rect.Height;
			}

			if (rect.Height > rect.Width)
			{
				rect.Bottom += (rect.Height-rect.Width)/2;
				rect.Height = rect.Width;
			}

			if (state.HasFlag (WidgetPaintState.Engaged))  // bouton pressé ?
			{
				rect.Offset (1, -1);
			}

			var center = rect.Center;
			var path   = new Drawing.Path ();

			const double spikeShift = 0.15;
			const double baseShiftH = 0.30;
			const double baseShiftV = 0.15;

			switch (glyphShape)
			{
				case GlyphShape.ArrowUp:
				case GlyphShape.TriangleUp:
					path.MoveTo (center.X, center.Y+rect.Height*spikeShift);
					path.LineTo (center.X-rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					path.LineTo (center.X+rect.Width*baseShiftH, center.Y-rect.Height*baseShiftV);
					break;

				case GlyphShape.ArrowDown:
				case GlyphShape.TriangleDown:
					path.MoveTo (center.X, center.Y-rect.Height*spikeShift);
					path.LineTo (center.X-rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					path.LineTo (center.X+rect.Width*baseShiftH, center.Y+rect.Height*baseShiftV);
					break;

				case GlyphShape.ArrowRight:
				case GlyphShape.TriangleRight:
					path.MoveTo (center.X+rect.Width*spikeShift, center.Y);
					path.LineTo (center.X-rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo (center.X-rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
					break;

				case GlyphShape.ArrowLeft:
				case GlyphShape.TriangleLeft:
					path.MoveTo (center.X-rect.Width*spikeShift, center.Y);
					path.LineTo (center.X+rect.Width*baseShiftV, center.Y+rect.Height*baseShiftH);
					path.LineTo (center.X+rect.Width*baseShiftV, center.Y-rect.Height*baseShiftH);
					break;

				case GlyphShape.HorizontalMove:
					path.MoveTo (center.X-rect.Width*0.3, center.Y);
					path.LineTo (center.X-rect.Width*0.05, center.Y+rect.Height*0.3);
					path.LineTo (center.X-rect.Width*0.05, center.Y-rect.Height*0.3);
					path.Close ();
					path.MoveTo (center.X+rect.Width*0.3, center.Y);
					path.LineTo (center.X+rect.Width*0.05, center.Y+rect.Height*0.3);
					path.LineTo (center.X+rect.Width*0.05, center.Y-rect.Height*0.3);
					break;

				case GlyphShape.VerticalMove:
					path.MoveTo (center.X, center.Y-rect.Height*0.3);
					path.LineTo (center.X-rect.Width*0.3, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.3, center.Y-rect.Height*0.05);
					path.Close ();
					path.MoveTo (center.X, center.Y+rect.Height*0.3);
					path.LineTo (center.X-rect.Width*0.3, center.Y+rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.3, center.Y+rect.Height*0.05);
					break;

				case GlyphShape.Menu:
					path.MoveTo (center.X+rect.Width*0.00, center.Y-rect.Height*0.25);
					path.LineTo (center.X-rect.Width*0.30, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.30, center.Y+rect.Height*0.15);
					break;

				case GlyphShape.Close:
				case GlyphShape.Reject:
					path.MoveTo (center.X-rect.Width*0.20, center.Y-rect.Height*0.30);
					path.LineTo (center.X-rect.Width*0.30, center.Y-rect.Height*0.20);
					path.LineTo (center.X-rect.Width*0.10, center.Y+rect.Height*0.00);
					path.LineTo (center.X-rect.Width*0.30, center.Y+rect.Height*0.20);
					path.LineTo (center.X-rect.Width*0.20, center.Y+rect.Height*0.30);
					path.LineTo (center.X-rect.Width*0.00, center.Y+rect.Height*0.10);
					path.LineTo (center.X+rect.Width*0.20, center.Y+rect.Height*0.30);
					path.LineTo (center.X+rect.Width*0.30, center.Y+rect.Height*0.20);
					path.LineTo (center.X+rect.Width*0.10, center.Y+rect.Height*0.00);
					path.LineTo (center.X+rect.Width*0.30, center.Y-rect.Height*0.20);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.30);
					path.LineTo (center.X+rect.Width*0.00, center.Y-rect.Height*0.10);
					break;

				case GlyphShape.Dots:
					path.MoveTo (center.X-rect.Width*0.30, center.Y+rect.Height*0.06);
					path.LineTo (center.X-rect.Width*0.18, center.Y+rect.Height*0.06);
					path.LineTo (center.X-rect.Width*0.18, center.Y-rect.Height*0.06);
					path.LineTo (center.X-rect.Width*0.30, center.Y-rect.Height*0.06);
					path.Close ();
					path.MoveTo (center.X-rect.Width*0.06, center.Y+rect.Height*0.06);
					path.LineTo (center.X+rect.Width*0.06, center.Y+rect.Height*0.06);
					path.LineTo (center.X+rect.Width*0.06, center.Y-rect.Height*0.06);
					path.LineTo (center.X-rect.Width*0.06, center.Y-rect.Height*0.06);
					path.Close ();
					path.MoveTo (center.X+rect.Width*0.18, center.Y+rect.Height*0.06);
					path.LineTo (center.X+rect.Width*0.30, center.Y+rect.Height*0.06);
					path.LineTo (center.X+rect.Width*0.30, center.Y-rect.Height*0.06);
					path.LineTo (center.X+rect.Width*0.18, center.Y-rect.Height*0.06);
					break;

				case GlyphShape.Accept:
					path.MoveTo (center.X-rect.Width*0.30, center.Y+rect.Height*0.00);
					path.LineTo (center.X-rect.Width*0.20, center.Y+rect.Height*0.10);
					path.LineTo (center.X-rect.Width*0.10, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.20, center.Y+rect.Height*0.30);
					path.LineTo (center.X+rect.Width*0.30, center.Y+rect.Height*0.20);
					path.LineTo (center.X-rect.Width*0.10, center.Y-rect.Height*0.30);
					break;

				case GlyphShape.TabLeft:
					path.MoveTo (center.X-rect.Width*0.10, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.00, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.00, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo (center.X-rect.Width*0.10, center.Y-rect.Height*0.15);
					break;

				case GlyphShape.TabRight:
					path.MoveTo (center.X+rect.Width*0.00, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.10, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.10, center.Y-rect.Height*0.15);
					path.LineTo (center.X-rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo (center.X-rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.00, center.Y-rect.Height*0.05);
					break;

				case GlyphShape.TabCenter:
					path.MoveTo (center.X-rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.05, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo (center.X-rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo (center.X-rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo (center.X-rect.Width*0.05, center.Y-rect.Height*0.05);
					break;

				case GlyphShape.TabDecimal:
					path.MoveTo (center.X-rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.05, center.Y+rect.Height*0.15);
					path.LineTo (center.X+rect.Width*0.05, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo (center.X-rect.Width*0.20, center.Y-rect.Height*0.15);
					path.LineTo (center.X-rect.Width*0.20, center.Y-rect.Height*0.05);
					path.LineTo (center.X-rect.Width*0.05, center.Y-rect.Height*0.05);
					path.Close ();
					path.MoveTo (center.X+rect.Width*0.10, center.Y+rect.Height*0.10);
					path.LineTo (center.X+rect.Width*0.20, center.Y+rect.Height*0.10);
					path.LineTo (center.X+rect.Width*0.20, center.Y+rect.Height*0.00);
					path.LineTo (center.X+rect.Width*0.10, center.Y+rect.Height*0.00);
					break;

				case GlyphShape.TabIndent:
					path.MoveTo (center.X-rect.Width*0.10, center.Y+rect.Height*0.20);
					path.LineTo (center.X+rect.Width*0.20, center.Y-rect.Height*0.00);
					path.LineTo (center.X-rect.Width*0.10, center.Y-rect.Height*0.20);
					break;

				case GlyphShape.Plus:
					path.MoveTo (center.X-rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo (center.X-rect.Width*0.07, center.Y+rect.Height*0.07);
					path.LineTo (center.X-rect.Width*0.07, center.Y+rect.Height*0.29);
					path.LineTo (center.X+rect.Width*0.07, center.Y+rect.Height*0.29);
					path.LineTo (center.X+rect.Width*0.07, center.Y+rect.Height*0.07);
					path.LineTo (center.X+rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo (center.X+rect.Width*0.29, center.Y-rect.Height*0.07);
					path.LineTo (center.X+rect.Width*0.07, center.Y-rect.Height*0.07);
					path.LineTo (center.X+rect.Width*0.07, center.Y-rect.Height*0.29);
					path.LineTo (center.X-rect.Width*0.07, center.Y-rect.Height*0.29);
					path.LineTo (center.X-rect.Width*0.07, center.Y-rect.Height*0.07);
					path.LineTo (center.X-rect.Width*0.29, center.Y-rect.Height*0.07);
					break;

				case GlyphShape.Minus:
					path.MoveTo (center.X-rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo (center.X+rect.Width*0.29, center.Y+rect.Height*0.07);
					path.LineTo (center.X+rect.Width*0.29, center.Y-rect.Height*0.07);
					path.LineTo (center.X-rect.Width*0.29, center.Y-rect.Height*0.07);
					break;

				default:
					path.Dispose ();
					return null;
			}
			
			path.Close ();
			
			return path;
		}

		public override void PaintCheck(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton à cocher sans texte.
			rect = graphics.Align (rect);
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
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

			//	Ombre claire en bas à droite.
			Direction shadow = Direction.Up;
			this.PaintL(graphics, rect, this.colorControlLightLight, shadow);
			this.PaintL(graphics, rInside, this.colorControlLight, shadow);

			//	Ombre foncée en haut à droite.
			this.PaintL(graphics, rect, this.colorControlDarkDark, this.Opposite(shadow));
			this.PaintL(graphics, rInside, this.colorControlDark, this.Opposite(shadow));

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
				using (Drawing.Path path = new Drawing.Path())
				{
					path.MoveTo(center.X-rect.Width*0.1, center.Y-rect.Height*0.1);
					path.LineTo(center.X+rect.Width*0.3, center.Y+rect.Height*0.3);
					path.LineTo(center.X+rect.Width*0.3, center.Y+rect.Height*0.1);
					path.LineTo(center.X-rect.Width*0.1, center.Y-rect.Height*0.3);
					path.LineTo(center.X-rect.Width*0.3, center.Y-rect.Height*0.1);
					path.LineTo(center.X-rect.Width*0.3, center.Y+rect.Height*0.1);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
				}
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.RenderSolid(this.colorBlack);
				}
				else
				{
					graphics.RenderSolid(this.colorControlDark);
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
					graphics.RenderSolid(this.colorControlDark);
				}
			}
		}

		public override void PaintRadio(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Widgets.WidgetPaintState state)
		{
			//	Dessine un bouton radio sans texte.
			rect = graphics.Align (rect);
			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			//	Ombre claire en bas à droite.
			Direction shadow = Direction.Up;
			this.PaintHalfCircle(graphics, rect, this.colorControlLightLight, shadow);
			this.PaintHalfCircle(graphics, rInside, this.colorControlLight, shadow);

			//	Ombre foncée en haut à droite.
			this.PaintHalfCircle(graphics, rect, this.colorControlDarkDark, this.Opposite(shadow));
			this.PaintHalfCircle(graphics, rInside, this.colorControlDark, this.Opposite(shadow));

			rInside = rect;
			rInside.Deflate(2);
			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else
			{
				this.PaintCircle(graphics, rInside, this.colorControlLightLight);
			}

			if ( (state&WidgetPaintState.ActiveYes) != 0 )  // coché ?
			{
				rInside = rect;
				rInside.Deflate(rect.Height*0.3);
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					this.PaintCircle(graphics, rInside, this.colorBlack);
				}
				else
				{
					this.PaintCircle(graphics, rInside, this.colorControlDark);
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
			Direction shadow = Direction.Up;

			if ( style == ButtonStyle.Normal        ||
				 style == ButtonStyle.DefaultAccept ||
				 style == ButtonStyle.DefaultCancel ||
				 style == ButtonStyle.DefaultAcceptAndCancel )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControl);

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(1);

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					//	Rectangle noir autour.
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
					if ( style == ButtonStyle.DefaultAccept || style == ButtonStyle.DefaultAcceptAndCancel )
					{
						//	Rectangle noir autour.
						Drawing.Rectangle rOut = rect;
						rOut.Deflate(0.5);
						graphics.AddRectangle(rOut);
						graphics.RenderSolid(this.colorBlack);

						rect.Deflate(1);
						rInside.Deflate(1);
					}

					//	Ombre claire en haut à gauche.
					this.PaintL(graphics, rect, this.colorControlLightLight, this.Opposite(shadow));
					this.PaintL(graphics, rInside, this.colorControlLight, this.Opposite(shadow));

					//	Ombre foncée en bas à droite.
					this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
					this.PaintL(graphics, rInside, this.colorControlDark, shadow);
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

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					shadow = this.Opposite(shadow);
				}

				//	Ombre claire en haut à gauche.
				this.PaintL(graphics, rect, this.colorControlLight, this.Opposite(shadow));
				this.PaintL(graphics, rInside, this.colorControlLightLight, this.Opposite(shadow));

				//	Ombre foncée en bas à droite.
				this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
				this.PaintL(graphics, rInside, this.colorControlDark, shadow);
			}
			else if ( style == ButtonStyle.Slider )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorControl);

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(1);

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					shadow = this.Opposite(shadow);
				}

				//	Ombre claire en haut à gauche.
				this.PaintL(graphics, rect, this.colorControlLight, this.Opposite(shadow));
				this.PaintL(graphics, rInside, this.colorControlLightLight, this.Opposite(shadow));

				//	Ombre foncée en bas à droite.
				this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
				this.PaintL(graphics, rInside, this.colorControlDark, shadow);
			}
			else if ( style == ButtonStyle.ToolItem  ||
					  style == ButtonStyle.ComboItem )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					graphics.RenderSolid(this.colorControlLight);
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					shadow = this.Opposite(shadow);
				}
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					shadow = this.Opposite(shadow);
				}

				//	Ombre claire en haut à gauche.
				this.PaintL(graphics, rect, this.colorControlLightLight, this.Opposite(shadow));

				//	Ombre foncée en bas à droite.
				this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
			}
			else if ( style == ButtonStyle.ActivableIcon )
			{
				if ( AbstractAdorner.IsThreeState2(state) )
				{
					rect.Top += 2;
				}

				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.ActiveYes)   != 0 ||   // bouton activé ?
					 (state&WidgetPaintState.ActiveMaybe) != 0 )
				{
					graphics.RenderSolid(this.colorControlLight);
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
				{
					shadow = this.Opposite(shadow);
				}
				if ( (state&WidgetPaintState.ActiveYes) != 0 )   // bouton activé ?
				{
					shadow = this.Opposite(shadow);
				}

				if ( AbstractAdorner.IsThreeState2(state) )
				{
					//	Ombre claire en haut à gauche.
					this.PaintL2(graphics, rect, this.colorControlLightLight, this.Opposite(shadow));

					//	Ombre foncée en bas à droite.
					this.PaintL2(graphics, rect, this.colorControlDarkDark, shadow);
				}
				else
				{
					//	Ombre claire en haut à gauche.
					this.PaintL(graphics, rect, this.colorControlLightLight, this.Opposite(shadow));

					//	Ombre foncée en bas à droite.
					this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
				}
			}
			else if ( style == ButtonStyle.Confirmation )
			{
				if ( (state&WidgetPaintState.Entered) != 0 )  // bouton survolé ?
				{
					//	Ombre claire en haut à gauche.
					this.PaintL(graphics, rect, this.colorControlLight, this.Opposite(shadow));

					//	Ombre foncée en bas à droite.
					this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
				}
				if ( (state&WidgetPaintState.Engaged) != 0 )   // bouton pressé ?
				{
					//	Ombre claire en haut à gauche.
					this.PaintL(graphics, rect, this.colorControlLight, this.Opposite(shadow));

					//	Ombre foncée en bas à droite.
					this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
				}
			}
			else if ( style == ButtonStyle.ListItem )
			{
				if ( (state&WidgetPaintState.Selected) != 0 )
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

			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = rect;
				if ( System.Math.Min(rect.Width, rect.Height) < 16 )
				{
					rFocus.Deflate(1.5);
				}
				else
				{
					rFocus.Deflate(2.5);
				}
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
			if ( style == TextFieldStyle.Normal ||
				 style == TextFieldStyle.Multiline  ||
				 style == TextFieldStyle.Combo  ||
				 style == TextFieldStyle.UpDown )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					if ( (state&WidgetPaintState.Error) != 0 )
					{
						graphics.RenderSolid(this.colorError);
					}
					else if ((state&WidgetPaintState.UndefinedLanguage) != 0)
					{
						graphics.RenderSolid (this.colorUndefinedLanguage);
					}
					else
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
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				graphics.LineWidth = 1;
				graphics.LineCap = Drawing.CapStyle.Butt;

				Drawing.Rectangle rInside = rect;
				rInside.Deflate(1);

				//	Ombre foncée en haut à gauche.
				Direction shadow = Direction.Up;
				this.PaintL(graphics, rect, this.colorControlDark, this.Opposite(shadow));
				this.PaintL(graphics, rInside, this.colorControlDarkDark, this.Opposite(shadow));

				//	Ombre claire en bas à droite.
				this.PaintL(graphics, rect, this.colorControlLightLight, shadow);
				this.PaintL(graphics, rInside, this.colorControlLight, shadow);
			}
			else if ( style == TextFieldStyle.Simple )
			{
				graphics.AddFilledRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
				{
					graphics.RenderSolid(this.colorControlLightLight);
				}
				else
				{
					graphics.RenderSolid(this.colorControl);
				}

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
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
			graphics.AddFilledRectangle(frameRect);
			graphics.RenderSolid(this.colorScrollerBack);

			if ( !tabRect.IsSurfaceZero && (state&WidgetPaintState.Engaged) != 0 )
			{
				graphics.AddFilledRectangle(tabRect);
				graphics.RenderSolid(this.colorControlDarkDark);
			}
		}

		public override void PaintScrollerHandle(Drawing.Graphics graphics,
										Drawing.Rectangle thumbRect,
										Drawing.Rectangle tabRect,
										Widgets.WidgetPaintState state,
										Widgets.Direction dir)
		{
			//	Dessine la cabine d'un ascenseur.
			state &= ~WidgetPaintState.Engaged;
			this.PaintButtonBackground(graphics, thumbRect, state, Direction.Up, ButtonStyle.Scroller);
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
				Drawing.Point p1 = graphics.Align(new Drawing.Point (sliderRect.Left +frameRect.Height*0.2, frameRect.Center.Y));
				Drawing.Point p2 = graphics.Align(new Drawing.Point (sliderRect.Right-frameRect.Height*0.2, frameRect.Center.Y));
				
				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X-0.5, p2.Y+0.5);
				graphics.RenderSolid(this.colorControlDark);
				graphics.AddLine(p1.X+0.5, p1.Y-0.5, p2.X-0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorControlLightLight);

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
				graphics.RenderSolid(this.colorControlDark);
				graphics.AddLine(p1.X+0.5, p1.Y+0.5, p2.X+0.5, p2.Y-0.5);
				graphics.RenderSolid(this.colorControlLightLight);

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
			graphics.AddFilledRectangle(thumbRect);
			graphics.RenderSolid(this.colorControl);

			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			Drawing.Rectangle rInside = thumbRect;
			rInside.Deflate(1);

			Direction shadow = Direction.Up;
			if ( (state&WidgetPaintState.Engaged) != 0 )  // bouton pressé ?
			{
				shadow = this.Opposite(shadow);
			}

			//	Ombre claire en haut à gauche.
			this.PaintL(graphics, thumbRect, this.colorControlLight, this.Opposite(shadow));
			this.PaintL(graphics, rInside, this.colorControlLightLight, this.Opposite(shadow));

			//	Ombre foncée en bas à droite.
			this.PaintL(graphics, thumbRect, this.colorControlDarkDark, shadow);
			this.PaintL(graphics, rInside, this.colorControlDark, shadow);
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
			this.PaintL(graphics, rect, this.colorControlDarkDark, this.Opposite(Direction.Up));
			this.PaintL(graphics, rect, this.colorControlLightLight, Direction.Up);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(2);
			if (style == ProgressIndicatorStyle.UnknownDuration)
			{
				double x = rInside.Width*progress;
				double w = rInside.Width*0.2;

				this.PaintProgressUnknow(graphics, rInside, w, x-w);
				this.PaintProgressUnknow(graphics, rInside, w, x-w+rInside.Width);
			}
			else
			{
				if (progress != 0)
				{
					rInside.Width *= progress;
					graphics.AddFilledRectangle(rInside);
					graphics.RenderSolid(this.colorCaption);
				}
			}
		}

		protected void PaintProgressUnknow(Drawing.Graphics graphics, Drawing.Rectangle rect, double w, double x)
		{
			Drawing.Rectangle fill = new Drawing.Rectangle(rect.Left+x, rect.Bottom, w, rect.Height);

			if (fill.Left < rect.Left)
			{
				fill.Left = rect.Left;
			}

			if (fill.Right > rect.Right)
			{
				fill.Right = rect.Right;
			}

			if (fill.Width > 0)
			{
				graphics.AddFilledRectangle(fill);
				graphics.RenderSolid(this.colorCaption);
			}
		}

		public override void PaintGroupBox(Drawing.Graphics graphics,
								  Drawing.Rectangle frameRect,
								  Drawing.Rectangle titleRect,
								  Widgets.WidgetPaintState state)
		{
			//	Dessine le cadre d'un GroupBox.
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
			graphics.LineWidth = 1;
			graphics.LineCap = Drawing.CapStyle.Butt;

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			//	Ombre claire en haut à gauche.
			Direction shadow = Direction.Up;
			this.PaintL(graphics, rect, this.colorControlLightLight, this.Opposite(shadow));
			this.PaintL(graphics, rInside, this.colorControlLight, this.Opposite(shadow));

			//	Ombre foncée en bas à droite.
			this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
			this.PaintL(graphics, rInside, this.colorControlDark, shadow);
		}

		public override void PaintTabAboveBackground(Drawing.Graphics graphics,
											Drawing.Rectangle frameRect,
											Drawing.Rectangle titleRect,
											Widgets.WidgetPaintState state,
											Widgets.Direction dir)
		{
			//	Dessine l'onglet devant les autres.
			Drawing.Rectangle rBack = titleRect;
			rBack.Right  -= 2;
			rBack.Bottom -= 1;
			rBack.Top    -= 2;
			graphics.AddFilledRectangle(rBack);
			graphics.RenderSolid(this.colorControl);

			titleRect.Bottom += 1;
			this.PaintTabBackground(graphics, frameRect, titleRect, state, dir);
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
			titleRect.Right += 1;
			this.PaintTabBackground(graphics, frameRect, titleRect, state, dir);
		}

		protected void PaintTabBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle frameRect,
										  Drawing.Rectangle titleRect,
										  Widgets.WidgetPaintState state,
										  Widgets.Direction dir)
		{
			//	Dessine un onglet quelconque.
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
			graphics.AddFilledRectangle(rect);
			if ( (state&WidgetPaintState.Enabled) != 0 )  // bouton enable ?
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

			//	Ombre foncée en haut à gauche.
			Direction shadow = Direction.Up;
			this.PaintL(graphics, rect, this.colorControlDark, this.Opposite(shadow));
			this.PaintL(graphics, rInside, this.colorControlDarkDark, this.Opposite(shadow));

			//	Ombre claire en bas à droite.
			this.PaintL(graphics, rect, this.colorControlLightLight, shadow);
			this.PaintL(graphics, rInside, this.colorControlLight, shadow);

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
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorCaption);
			}

			if ( (state&WidgetPaintState.Entered) != 0 )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.2, this.colorCaption.R, this.colorCaption.G, this.colorCaption.B));
			}

			if ((state&WidgetPaintState.Focused) != 0)
			{
				rect.Deflate(1);
				AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorControlDarkDark);
			}
		}

		public override void PaintHeaderBackground(Drawing.Graphics graphics,
										  Drawing.Rectangle rect,
										  WidgetPaintState state,
										  Direction dir)
		{
			//	Dessine le fond d'un bouton d'en-tête de tableau.
			this.PaintButtonBackground(graphics, rect, state, dir, ButtonStyle.Scroller);
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
			rect.Deflate(0.5);

			graphics.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
			graphics.RenderSolid(this.colorControlLightLight);

			graphics.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			graphics.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
			graphics.RenderSolid(this.colorControlDark);
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
#if true
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorControlDark);
#else
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorControl);

			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			//	Ombre claire en haut à gauche.
			this.PaintL(graphics, rect, this.colorControlLightLight, this.Opposite(shadow));
			this.PaintL(graphics, rInside, this.colorControlLight, this.Opposite(shadow));

			//	Ombre foncée en bas à droite.
			this.PaintL(graphics, rect, this.colorControlDarkDark, shadow);
			this.PaintL(graphics, rInside, this.colorControlDark, shadow);
#endif
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
			if ( itemType != MenuItemState.Default )
			{
				if ( (state&WidgetPaintState.Enabled) != 0 )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.colorCaption);
				}
				else
				{
					rect.Deflate(0.5);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.ColorBorder);
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
			if ( optional )  return;

			if ( dir == Direction.Right )
			{
				var p1 = graphics.Align (new Drawing.Point (rect.Left+rect.Width/2, rect.Bottom));
				var p2 = graphics.Align (new Drawing.Point (rect.Left+rect.Width/2, rect.Top));
				p1.X -= 1.5;
				p2.X -= 1.5;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorControlDark);

				p1.X += 1.0;
				p2.X += 1.0;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorControlLightLight);
			}
			else
			{
				var p1 = graphics.Align (new Drawing.Point (rect.Left, rect.Bottom+rect.Height/2));
				var p2 = graphics.Align (new Drawing.Point (rect.Right, rect.Bottom+rect.Height/2));
				p1.Y -= 1.5;
				p2.Y -= 1.5;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorControlLightLight);

				p1.Y += 1.0;
				p2.Y += 1.0;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(this.colorControlDark);
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
			graphics.RenderSolid(this.colorControlDarkDark);
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
			graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
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
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Drawing.Color.FromBrightness(this.colorControl.GetBrightness()-0.1));
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
			rect.Deflate(0.5);

			graphics.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
			graphics.RenderSolid(this.colorControlLightLight);

			graphics.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			graphics.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
			graphics.RenderSolid(this.colorControlDark);
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
			if ( (state&WidgetPaintState.ActiveYes) == 0 )  return;  // bouton activé ?

			rect.Bottom -= 4;  // pour cacher la partie inférieure !
			state &= ~WidgetPaintState.ActiveYes;
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
			fullRect.Deflate(0.5);
			graphics.AddLine(fullRect.Right, fullRect.Bottom, fullRect.Right, fullRect.Top);
			graphics.RenderSolid(this.colorControlDark);

			if (text != null)
			{
				Drawing.TextStyle.DefineDefaultFontColor(this.colorBlack);
				Drawing.Point pos = new Drawing.Point(textRect.Left+3, textRect.Bottom+2);
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
			rect = graphics.Align (rect);
			Drawing.Rectangle rInside = rect;
			rInside.Deflate(1);

			//	Ombre foncée en bas à droite.
			Direction shadow = Direction.Up;
			this.PaintHalfCircle(graphics, rect, this.colorControlLightLight, this.Opposite(shadow));
			this.PaintHalfCircle(graphics, rInside, this.colorControlLight, this.Opposite(shadow));

			//	Ombre claire en haut à droite.
			this.PaintHalfCircle(graphics, rect, this.colorControlDarkDark, shadow);
			this.PaintHalfCircle(graphics, rInside, this.colorControlDark, shadow);

			rInside = rect;
			rInside.Deflate(2);
			if ( color.IsEmpty || (state&WidgetPaintState.Enabled) == 0 )
			{
				this.PaintCircle(graphics, rInside, this.colorControl);
			}
			else
			{
				double r = 1.0-(1.0-color.R)*0.5;
				double g = 1.0-(1.0-color.G)*0.5;
				double b = 1.0-(1.0-color.B)*0.5;
				this.PaintCircle(graphics, rInside, Drawing.Color.FromRgb(r, g, b));
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
			AbstractAdorner.DrawFocusedRectangle(graphics, rect, this.colorControlDark);
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
				p1 = graphics.Align(p1) - new Point (0.5, 0.5);
				p2 = graphics.Align(p2) - new Point (0.5, 0.5);
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
				double gamma = graphics.Rasterizer.Gamma;
				graphics.Rasterizer.Gamma = 0.5;
				pos.X ++;
				pos.Y --;
				text.Paint(pos, graphics, clipRect, this.colorControlLightLight, Drawing.GlyphPaintStyle.Shadow);
				graphics.Rasterizer.Gamma = gamma;  // remet gamma initial
				pos.X --;
				pos.Y ++;
				text.Paint(pos, graphics, clipRect, this.colorControlDark, Drawing.GlyphPaintStyle.Disabled);
			}

			if ( (state&WidgetPaintState.Focused) != 0 )
			{
				Drawing.Rectangle rFocus = text.StandardRectangle;
				rFocus.Offset(pos);
				rFocus = graphics.Align(rFocus);
				rFocus.Inflate(2.5, -0.5);
				this.PaintFocusBox(graphics, rFocus);
			}
		}


		protected void RectangleGroupBox(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 double startX, double endX)
		{
			//	Dessine un rectangle
			graphics.AddLine(rect.Left, rect.Top, startX, rect.Top);
			graphics.AddLine(endX, rect.Top, rect.Right, rect.Top);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			graphics.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
			graphics.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
		}

		protected void PaintL(Drawing.Graphics graphics,
							  Drawing.Rectangle rect,
							  Drawing.Color color,
							  Widgets.Direction dir)
		{
			//	Dessine un "L" pour simuler une ombre.
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

		protected void PaintL2(Drawing.Graphics graphics,
							   Drawing.Rectangle rect,
							   Drawing.Color color,
							   Widgets.Direction dir)
		{
			//	Dessine un "L" pour simuler une ombre sur un bouton ThreeState 2 pixels plus haut.
			Drawing.Point p1 = new Drawing.Point();
			Drawing.Point p2 = new Drawing.Point();

			switch ( dir )
			{
				case Direction.Up:	// en bas à droite
					p1.X = rect.Left;
					p1.Y = rect.Bottom+0.5;
					p2.X = rect.Right-0.5;
					p2.Y = rect.Bottom+0.5;
					graphics.AddLine(p1, p2);
					p1 = p2;
					p2.X = rect.Right-0.5;
					p2.Y = rect.Top-2.5;
					graphics.AddLine(p1, p2);
					p1 = p2;
					p2.X = rect.Right-2.5;
					p2.Y = rect.Top-0.5;
					graphics.AddLine(p1, p2);
					break;

				case Direction.Down:	// en haut à gauche
					p1.X = rect.Left+0.5;
					p1.Y = rect.Bottom;
					p2.X = rect.Left+0.5;
					p2.Y = rect.Top-2.5;
					graphics.AddLine(p1, p2);
					p1 = p2;
					p2.X = rect.Left+2.5;
					p2.Y = rect.Top-0.5;
					graphics.AddLine(p1, p2);
					p1 = p2;
					p2.X = rect.Right-2.5;
					p2.Y = rect.Top-0.5;
					graphics.AddLine(p1, p2);
					break;
			}
			graphics.RenderSolid(color);
		}

		protected void PaintHalfCircle(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   Drawing.Color color,
									   Widgets.Direction dir)
		{
			//	Dessine un demi-cercle en bas à droite si dir=Up.
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

		protected void PaintHalfCircle(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   Drawing.Color color,
									   double angle)
		{
			//	Dessine un demi-cercle. Si angle=0, le demi-cercle est en haut.
			//	L'angle est donné en degrés.
			Drawing.Point c = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			
			using (Drawing.Path path = new Drawing.Path())
			{
				angle = angle*System.Math.PI/180;  // angle en radians
				double c1x, c1y, c2x, c2y, px, py;
				
				px  = -rx;						py  = 0;						this.RotatePoint(angle, ref px,  ref py);
				path.MoveTo(c.X+px, c.Y+py);
				
				c1x = -rx;						c1y = ry*Drawing.Path.Kappa;	this.RotatePoint(angle, ref c1x, ref c1y);
				c2x = -rx*Drawing.Path.Kappa;	c2y = ry;						this.RotatePoint(angle, ref c2x, ref c2y);
				px  = 0;						py  = ry;						this.RotatePoint(angle, ref px,  ref py);
				path.CurveTo(c.X+c1x, c.Y+c1y, c.X+c2x, c.Y+c2y, c.X+px, c.Y+py);
				
				c1x = rx*Drawing.Path.Kappa;	c1y = ry;						this.RotatePoint(angle, ref c1x, ref c1y);
				c2x = rx;						c2y = ry*Drawing.Path.Kappa;	this.RotatePoint(angle, ref c2x, ref c2y);
				px  = rx;						py  = 0;						this.RotatePoint(angle, ref px,  ref py);
				path.CurveTo(c.X+c1x, c.Y+c1y, c.X+c2x, c.Y+c2y, c.X+px, c.Y+py);
				
				path.Close();
				
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(color);
			}
		}

		protected void RotatePoint(double angle, ref double x, ref double y)
		{
			//	Fait tourner un point autour de l'origine.
			//	L'angle est exprimé en radians.
			//	Un angle positif est anti-horaire (CCW).
			double xx = x*System.Math.Cos(angle) - y*System.Math.Sin(angle);
			double yy = x*System.Math.Sin(angle) + y*System.Math.Cos(angle);
			x = xx;
			y = yy;
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

		protected Direction Opposite(Direction dir)
		{
			//	Retourne la direction opposée.
			switch ( dir )
			{
				case Direction.Up:     return Direction.Down;
				case Direction.Down:   return Direction.Up;
				case Direction.Left:   return Direction.Right;
				case Direction.Right:  return Direction.Left;
			}
			return Direction.Up;
		}


		public override Drawing.Color AdaptPictogramColor(Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor)
		{
			if ( paintStyle == Drawing.GlyphPaintStyle.Disabled ||
				 paintStyle == Drawing.GlyphPaintStyle.Shadow   )
			{
				color = Drawing.Color.FromAlphaRgb(color.A, uniqueColor.R, uniqueColor.G, uniqueColor.B);
			}

			return color;
		}

		public override Drawing.Color ColorDisabled
		{
			get { return this.colorControlDark; }
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
			get { return this.colorControlDarkDark; }
		}

		public override Drawing.Color ColorTextBackground
		{
			get { return this.colorControlLightLight; }
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
				return this.colorControlDark;
			}
		}

		public override Drawing.Color ColorTextSliderBorder(bool enabled)
		{
			return this.colorControlDarkDark;
		}

		public override Drawing.Color ColorTextFieldBorder(bool enabled)
		{
			return this.colorControlDarkDark;
		}

		public override Drawing.Color ColorTextDisplayMode(TextFieldDisplayMode mode)
		{
			switch ( mode )
			{
				case TextFieldDisplayMode.Default:   return Drawing.Color.Empty;
				case TextFieldDisplayMode.OverriddenValue:   return Drawing.Color.Empty;
				case TextFieldDisplayMode.InheritedValue:  return Drawing.Color.Empty;
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
		public override double GeometryScrollListXMargin { get { return 3; } }
		public override double GeometryScrollListYMargin { get { return 3; } }
		public override double GeometrySliderLeftMargin { get { return 0; } }
		public override double GeometrySliderRightMargin { get { return 0; } }
		public override double GeometrySliderBottomMargin { get { return 0; } }


		protected Drawing.Color		colorControlReadOnly;
		protected Drawing.Color		colorScrollerBack;
		protected Drawing.Color		colorUndefinedLanguage;
		protected Drawing.Color		colorWindow;
	}
}
