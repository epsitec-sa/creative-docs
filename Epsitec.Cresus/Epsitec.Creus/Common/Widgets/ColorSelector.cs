//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ColorSelector</c> class provides a user inteface to select a
	/// color (mixing a color wheel, a palette and a set of text fields).
	/// </summary>
	public class ColorSelector : Widget
	{
		public ColorSelector()
		{
			this.CreateUserInterface ();
		}

		public ColorSelector(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public Drawing.RichColor				Color
		{
			get
			{
				if ((this.blackReplacement.IsValid) &&
					(this.color.Basic == Drawing.Color.FromBrightness (0)))
				{
					return this.blackReplacement;
				}
				else
				{
					return this.color;
				}
			}
			set
			{
				if (value.IsEmpty)
				{
					value = Drawing.RichColor.FromAlphaRgb (0, 1, 1, 1);
				}

				if (this.color != value)
				{
					this.suspendColorEvents++;
					this.color = value;
					this.wheel.Color = value;
					this.UpdateColors ();
					this.suspendColorEvents--;
				}
			}
		}

		public ColorPalette						ColorPalette
		{
			get
			{
				return this.palette;
			}
		}

		public bool								CloseButtonVisibility
		{
			get
			{
				return this.closeButton.Visibility;
			}
			set
			{
				if (this.CloseButtonVisibility != value)
				{
					this.closeButton.Visibility = value;
					this.UpdateGeometry ();
				}
			}
		}

		public Drawing.RichColor				BlackReplacement
		{
			get
			{
				return this.blackReplacement;
			}
			set
			{
				this.blackReplacement = value;
			}
		}



		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.UpdateGeometry ();
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (this.BackColor.IsValid)
			{
				graphics.AddFilledRectangle (this.Client.Bounds);
				graphics.RenderSolid (this.BackColor);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.fields[0].EditionAccepted   -= this.HandleTextRgbChanged;
				this.fields[1].EditionAccepted   -= this.HandleTextRgbChanged;
				this.fields[2].EditionAccepted   -= this.HandleTextRgbChanged;
				this.fields[3].EditionAccepted   -= this.HandleTextAlphaChanged;
				this.fields[4].EditionAccepted   -= this.HandleTextHsvChanged;
				this.fields[5].EditionAccepted   -= this.HandleTextHsvChanged;
				this.fields[6].EditionAccepted   -= this.HandleTextHsvChanged;
				this.fields[7].EditionAccepted   -= this.HandleTextCmykChanged;
				this.fields[8].EditionAccepted   -= this.HandleTextCmykChanged;
				this.fields[9].EditionAccepted   -= this.HandleTextCmykChanged;
				this.fields[10].EditionAccepted  -= this.HandleTextCmykChanged;
				this.fields[11].EditionAccepted  -= this.HandleTextGrayChanged;
				this.fieldHexa.EditionAccepted   -= this.HandleTextHexaChanged;
				this.wheel.Changed               -= this.HandleWheelChanged;
				this.palette.ExportSelectedColor -= this.HandlePaletteExport;
				this.palette.ImportSelectedColor -= this.HandlePaletteImport;
				this.picker.HotColorChanged      -= this.HandlePickerHotColorChanged;
			}

			base.Dispose (disposing);
		}

		
		private void CreateUserInterface()
		{
			this.labels = new StaticText[ColorSelector.FieldCount];
			this.fields = new TextFieldSlider[ColorSelector.FieldCount];
			for (int i=0; i<ColorSelector.FieldCount; i++)
			{
				this.labels[i] = new StaticText (this);
				this.fields[i] = new TextFieldSlider (this);

				this.fields[i].TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.fields[i].Value = 0;

				this.fields[i].DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.fields[i].AutoSelectOnFocus = true;
				this.fields[i].SwallowEscapeOnRejectEdition = true;

				if (i < 3)  // r,g,b ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 255;
					this.fields[i].Step = 10;
					this.fields[i].EditionAccepted += this.HandleTextRgbChanged;
				}
				else if (i == 3)  // a ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 255;
					this.fields[i].Step = 10;
					this.fields[i].EditionAccepted += this.HandleTextAlphaChanged;
				}
				else if (i == 4)  // t ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 360;
					this.fields[i].Step = 10;
					this.fields[i].EditionAccepted += this.HandleTextHsvChanged;
				}
				else if (i < 7)  // s,i ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 100;
					this.fields[i].Step = 5;
					this.fields[i].EditionAccepted += this.HandleTextHsvChanged;
				}
				else if (i< 11)  // c,m,y,k ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 100;
					this.fields[i].Step = 5;
					this.fields[i].EditionAccepted += this.HandleTextCmykChanged;
				}
				else	// g ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 100;
					this.fields[i].Step = 5;
					this.fields[i].EditionAccepted += this.HandleTextGrayChanged;
				}
			}

			int index = 100;
			this.fields[0].TabIndex = index++;
			this.fields[1].TabIndex = index++;
			this.fields[2].TabIndex = index++;
			this.fields[4].TabIndex = index++;
			this.fields[5].TabIndex = index++;
			this.fields[6].TabIndex = index++;
			this.fields[7].TabIndex = index++;
			this.fields[8].TabIndex = index++;
			this.fields[9].TabIndex = index++;
			this.fields[10].TabIndex = index++;
			this.fields[11].TabIndex = index++;
			this.fields[3].TabIndex = index++;

			this.fields[0].Color = Drawing.Color.FromRgb (1, 0, 0);  // r
			this.fields[1].Color = Drawing.Color.FromRgb (0, 1, 0);  // v
			this.fields[2].Color = Drawing.Color.FromRgb (0, 0, 1);  // b
			this.fields[3].Color = Drawing.Color.FromRgb (0.5, 0.5, 0.5);
			ToolTip.Default.SetToolTip (this.fields[0], Res.Strings.ColorSelector.LongRed);
			ToolTip.Default.SetToolTip (this.fields[1], Res.Strings.ColorSelector.LongGreen);
			ToolTip.Default.SetToolTip (this.fields[2], Res.Strings.ColorSelector.LongBlue);
			ToolTip.Default.SetToolTip (this.fields[3], Res.Strings.ColorSelector.LongAlpha);

			this.fields[4].TextSuffix = "\u00B0";  // symbole unicode "degré" (#176)
			this.fields[4].Color = Drawing.Color.FromRgb (0, 0, 0);
			this.fields[4].BackColor = Drawing.Color.FromRgb (0.5, 0.5, 0.5);
			ToolTip.Default.SetToolTip (this.fields[4], Res.Strings.ColorSelector.LongHue);

			this.fields[5].TextSuffix = "%";
			this.fields[5].Color = Drawing.Color.FromRgb (0, 0, 0);
			this.fields[5].BackColor = Drawing.Color.FromRgb (1, 1, 1);
			ToolTip.Default.SetToolTip (this.fields[5], Res.Strings.ColorSelector.LongSaturation);

			this.fields[6].TextSuffix = "%";
			this.fields[6].Color = Drawing.Color.FromRgb (1, 1, 1);
			this.fields[6].BackColor = Drawing.Color.FromRgb (0, 0, 0);
			ToolTip.Default.SetToolTip (this.fields[6], Res.Strings.ColorSelector.LongValue);

			this.fields[7].Color = Drawing.Color.FromRgb (0, 1, 1);  // c
			this.fields[8].Color = Drawing.Color.FromRgb (1, 0, 1);  // m
			this.fields[9].Color = Drawing.Color.FromRgb (1, 1, 0);  // y
			this.fields[10].Color = Drawing.Color.FromRgb (0, 0, 0);  // k
			this.fields[10].BackColor = Drawing.Color.FromRgb (1, 1, 1);
			ToolTip.Default.SetToolTip (this.fields[7], Res.Strings.ColorSelector.LongCyan);
			ToolTip.Default.SetToolTip (this.fields[8], Res.Strings.ColorSelector.LongMagenta);
			ToolTip.Default.SetToolTip (this.fields[9], Res.Strings.ColorSelector.LongYellow);
			ToolTip.Default.SetToolTip (this.fields[10], Res.Strings.ColorSelector.LongBlack);

			this.fields[11].Color = Drawing.Color.FromRgb (1, 1, 1);  // g
			this.fields[11].BackColor = Drawing.Color.FromRgb (0, 0, 0);
			ToolTip.Default.SetToolTip (this.fields[11], Res.Strings.ColorSelector.LongGray);

			this.labels[ 0].FormattedText = Res.Strings.ColorSelector.ShortRed;
			this.labels[ 1].FormattedText = Res.Strings.ColorSelector.ShortGreen;
			this.labels[ 2].FormattedText = Res.Strings.ColorSelector.ShortBlue;
			this.labels[ 3].FormattedText = Res.Strings.ColorSelector.ShortAlpha;
			this.labels[ 4].FormattedText = Res.Strings.ColorSelector.ShortHue;
			this.labels[ 5].FormattedText = Res.Strings.ColorSelector.ShortSaturation;
			this.labels[ 6].FormattedText = Res.Strings.ColorSelector.ShortValue;
			this.labels[ 7].FormattedText = Res.Strings.ColorSelector.ShortCyan;
			this.labels[ 8].FormattedText = Res.Strings.ColorSelector.ShortMagenta;
			this.labels[ 9].FormattedText = Res.Strings.ColorSelector.ShortYellow;
			this.labels[10].FormattedText = Res.Strings.ColorSelector.ShortBlack;
			this.labels[11].FormattedText = Res.Strings.ColorSelector.ShortGray;

			this.labelHexa = new StaticText (this);
			this.labelHexa.FormattedText = Res.Strings.ColorSelector.ShortHexa;

			this.fieldHexa = new TextField (this);
			this.fieldHexa.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldHexa.AutoSelectOnFocus = true;
			this.fieldHexa.SwallowEscapeOnRejectEdition = true;
			this.fieldHexa.EditionAccepted += this.HandleTextHexaChanged;
			this.fieldHexa.TabIndex = 200;
			this.fieldHexa.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldHexa, Res.Strings.ColorSelector.LongHexa);

			this.wheel = new ColorWheel (this);
			this.wheel.Changed += this.HandleWheelChanged;

			this.palette = new ColorPalette (this);
			this.palette.OptionButtonVisibility = true;
			this.palette.ExportSelectedColor += this.HandlePaletteExport;
			this.palette.ImportSelectedColor += this.HandlePaletteImport;
			this.palette.TabIndex = 10;
			this.palette.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.picker = new Tools.MagnifierDragSource (this);
			this.picker.HotColorChanged += this.HandlePickerHotColorChanged;
			ToolTip.Default.SetToolTip (this.picker, Res.Strings.ColorSelector.Picker);

			this.closeButton = new GlyphButton (this);
			this.closeButton.GlyphShape = GlyphShape.Close;
			this.closeButton.ButtonStyle = ButtonStyle.Normal;
			this.closeButton.Clicked += this.HandleButtonCloseClicked;
			this.closeButton.TabIndex = 1;
			this.closeButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.closeButton, Res.Strings.ColorSelector.Close);

			this.colorSpaceController = Helpers.GroupController.GetGroupController (this, "ColorSpace");
			this.colorSpaceController.Changed += this.HandleColorSpaceChanged;

			this.buttonRgb = new IconButton (this);
			this.buttonRgb.AutoRadio = true;
			this.buttonRgb.AutoToggle = true;
			this.buttonRgb.Group = this.colorSpaceController.Group;
			this.buttonRgb.Index = 1;
			this.buttonRgb.IconUri = "manifest:Epsitec.Common.Widgets.Images.ColorSpaceRGB.icon";
			this.buttonRgb.TabIndex = 21;
			this.buttonRgb.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.buttonRgb, Res.Strings.ColorSelector.ColorSpace.Rgb);

			this.buttonCmyk = new IconButton (this);
			this.buttonCmyk.AutoRadio = true;
			this.buttonCmyk.AutoToggle = true;
			this.buttonCmyk.Group = this.colorSpaceController.Group;
			this.buttonCmyk.Index = 2;
			this.buttonCmyk.IconUri = "manifest:Epsitec.Common.Widgets.Images.ColorSpaceCMYK.icon";
			this.buttonCmyk.TabIndex = 21;
			this.buttonCmyk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.buttonCmyk, Res.Strings.ColorSelector.ColorSpace.Cmyk);

			this.buttonGray = new IconButton (this);
			this.buttonGray.AutoRadio = true;
			this.buttonGray.AutoToggle = true;
			this.buttonGray.Group = this.colorSpaceController.Group;
			this.buttonGray.Index = 3;
			this.buttonGray.IconUri = "manifest:Epsitec.Common.Widgets.Images.ColorSpaceGray.icon";
			this.buttonGray.TabIndex = 21;
			this.buttonGray.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.buttonGray, Res.Strings.ColorSelector.ColorSpace.Gray);
		}

		private void UpdateColors()
		{
			//	Met tout à jour après un changement de couleur.
			System.Diagnostics.Debug.Assert (this.suspendColorEvents > 0);
			this.ColorToFieldsRgb ();
			this.ColorToFieldsHsv ();
			this.ColorToFieldsCmyk ();
			this.ColorToFieldsGray ();
			this.ColorToFieldsHexa ();
			this.UpdateColorSpace ();
			this.UpdateGeometry ();
			this.Invalidate ();
			this.OnColorChanged ();
		}

		private void UpdateColorSpace()
		{
			//	Met à jour les boutons pour l'espace de couleur.
			Drawing.ColorSpace cs = this.color.ColorSpace;

			this.buttonRgb.ActiveState = (cs == Drawing.ColorSpace.Rgb) ? ActiveState.Yes : ActiveState.No;
			this.buttonCmyk.ActiveState = (cs == Drawing.ColorSpace.Cmyk) ? ActiveState.Yes : ActiveState.No;
			this.buttonGray.ActiveState = (cs == Drawing.ColorSpace.Gray) ? ActiveState.Yes : ActiveState.No;
		}

		private void ColorToFieldsRgb()
		{
			//	Couleur -> textes éditables.
			double a, r, g, b;
			this.color.Basic.GetAlphaRgb (out a, out r, out g, out b);

			this.fields[0].Value = (decimal) System.Math.Floor (r*255+0.5);
			this.fields[1].Value = (decimal) System.Math.Floor (g*255+0.5);
			this.fields[2].Value = (decimal) System.Math.Floor (b*255+0.5);
			this.fields[3].Value = (decimal) System.Math.Floor (a*255+0.5);
		}

		private void ColorToFieldsHsv()
		{
			//	Couleur -> textes éditables.
			double h, s, v;
			this.wheel.GetHsv (out h, out s, out v);

			this.fields[4].Value = (decimal) System.Math.Floor (System.Math.Floor (h+0.5));
			this.fields[5].Value = (decimal) System.Math.Floor (s*100+0.5);
			this.fields[6].Value = (decimal) System.Math.Floor (v*100+0.5);

			this.ColorizeSliders ();
		}

		private void ColorToFieldsCmyk()
		{
			//	Couleur -> textes éditables.
			double a = this.color.A;
			double c = this.color.C;
			double m = this.color.M;
			double y = this.color.Y;
			double k = this.color.K;

			this.fields[3].Value = (decimal) System.Math.Floor (a*255+0.5);
			this.fields[7].Value = (decimal) System.Math.Floor (c*100+0.5);
			this.fields[8].Value = (decimal) System.Math.Floor (m*100+0.5);
			this.fields[9].Value = (decimal) System.Math.Floor (y*100+0.5);
			this.fields[10].Value = (decimal) System.Math.Floor (k*100+0.5);
		}

		private void ColorToFieldsGray()
		{
			//	Couleur -> textes éditables.
			double a = this.color.A;
			double g = this.color.Gray;

			this.fields[3].Value = (decimal) System.Math.Floor (a*255+0.5);
			this.fields[11].Value = (decimal) System.Math.Floor (g*100+0.5);
		}

		private void ColorToFieldsHexa()
		{
			//	Couleur -> textes éditables.
			double a, r, g, b;
			this.color.Basic.GetAlphaRgb (out a, out r, out g, out b);

			r = Epsitec.Common.Math.Clip (r);
			g = Epsitec.Common.Math.Clip (g);
			b = Epsitec.Common.Math.Clip (b);

			int rr = (int) System.Math.Floor (r*255+0.5);
			int gg = (int) System.Math.Floor (g*255+0.5);
			int bb = (int) System.Math.Floor (b*255+0.5);

			string text = string.Format ("#{0}{1}{2}", rr.ToString ("x2"), gg.ToString ("x2"), bb.ToString ("x2"));
			this.fieldHexa.Text = text;
			this.fieldHexa.SelectAll ();
		}

		private void FieldsRgbToColor()
		{
			//	Textes éditables RGB -> couleur.
			double r = (double) this.fields[0].Value/255;
			double g = (double) this.fields[1].Value/255;
			double b = (double) this.fields[2].Value/255;
			double a = (double) this.fields[3].Value/255;

			this.suspendColorEvents++;
			this.color = Drawing.RichColor.FromAlphaRgb (a, r, g, b);
			this.wheel.Color = this.color;
			this.ColorToFieldsHsv ();
			this.ColorToFieldsCmyk ();
			this.ColorToFieldsGray ();
			this.ColorToFieldsHexa ();
			this.ColorizeSliders ();
			this.Invalidate ();
			this.OnColorChanged ();
			this.suspendColorEvents--;
		}

		private void FieldsHsvToColor()
		{
			//	Textes éditables HSV -> couleur.
			double h = (double) this.fields[4].Value;
			double s = (double) this.fields[5].Value/100;
			double v = (double) this.fields[6].Value/100;

			this.suspendColorEvents++;
			this.wheel.SetHsv (h, s, v);
			this.color = Drawing.RichColor.FromAlphaHsv (this.color.A, h, s, v);
			this.ColorToFieldsRgb ();
			this.ColorToFieldsCmyk ();
			this.ColorToFieldsGray ();
			this.ColorToFieldsHexa ();
			this.ColorizeSliders ();
			this.Invalidate ();
			this.OnColorChanged ();
			this.suspendColorEvents--;
		}

		private void FieldsCmykToColor()
		{
			//	Textes éditables CMYK -> couleur.
			double a = (double) this.fields[3].Value/255;
			double c = (double) this.fields[7].Value/100;
			double m = (double) this.fields[8].Value/100;
			double y = (double) this.fields[9].Value/100;
			double k = (double) this.fields[10].Value/100;

			this.suspendColorEvents++;
			this.color = Drawing.RichColor.FromAlphaCmyk (a, c, m, y, k);
			this.wheel.Color = this.color;
			this.ColorToFieldsRgb ();
			this.ColorToFieldsHsv ();
			this.ColorToFieldsGray ();
			this.ColorToFieldsHexa ();
			this.ColorizeSliders ();
			this.Invalidate ();
			this.OnColorChanged ();
			this.suspendColorEvents--;
		}

		private void FieldsGrayToColor()
		{
			//	Textes éditables Gray -> couleur.
			double a = (double) this.fields[3].Value/255;
			double g = (double) this.fields[11].Value/100;

			this.suspendColorEvents++;
			this.color = Drawing.RichColor.FromAGray (a, g);
			this.wheel.SetAGray (a, g);
			this.ColorToFieldsRgb ();
			this.ColorToFieldsHsv ();
			this.ColorToFieldsCmyk ();
			this.ColorToFieldsHexa ();
			this.ColorizeSliders ();
			this.Invalidate ();
			this.OnColorChanged ();
			this.suspendColorEvents--;
		}

		private void FieldsHexaToColor()
		{
			//	Textes éditables Hexa -> couleur.
			double r = 0;
			double g = 0;
			double b = 0;
			double a = (double) this.fields[3].Value/255;

			string text = this.fieldHexa.Text;
			if (text.Length >= 1 && text[0] == '#')
			{
				text = text.Substring (1);  // supprime le #
			}

			if (text.Length == 3)
			{
				int rr = ColorSelector.ParseHexa (text.Substring (0, 1));
				int gg = ColorSelector.ParseHexa (text.Substring (1, 1));
				int bb = ColorSelector.ParseHexa (text.Substring (2, 1));

				r = (rr * 16 + rr) / 255.0;
				g = (gg * 16 + gg) / 255.0;
				b = (bb * 16 + bb) / 255.0;
			}
			else
			{
				if (text.Length >= 2)
				{
					r = ColorSelector.ParseHexa (text.Substring (0, 2))/255.0;
				}

				if (text.Length >= 4)
				{
					g = ColorSelector.ParseHexa (text.Substring (2, 2))/255.0;
				}

				if (text.Length >= 6)
				{
					b = ColorSelector.ParseHexa (text.Substring (4, 2))/255.0;
				}
			}

			this.suspendColorEvents++;
			this.color = Drawing.RichColor.FromAlphaRgb (a, r, g, b);
			this.wheel.Color = this.color;
			this.ColorToFieldsRgb ();
			this.ColorToFieldsHsv ();
			this.ColorToFieldsCmyk ();
			this.ColorToFieldsGray ();
			this.ColorizeSliders ();
			this.Invalidate ();
			this.OnColorChanged ();
			this.suspendColorEvents--;
		}

		private void ColorizeSliders()
		{
			//	Colorise certains sliders en fonction de la couleur définie.
			double h, s, v;
			this.wheel.GetHsv (out h, out s, out v);
			Drawing.Color saturated = Drawing.Color.FromHsv (h, 1, 1);

			this.fields[4].Color = saturated;

			if (s == 0.0)  // couleur grise ?
			{
				saturated = Drawing.Color.FromBrightness (1.0);  // blanc
			}

			this.fields[5].Color = saturated;
			this.fields[6].Color = saturated;
		}

		private void UpdateGeometry()
		{
			//	Met à jour la géométrie.

			if (this.fields == null)
			{
				return;
			}

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate (1);
			double hWheel = rect.Height-5-20*3;
			hWheel = System.Math.Min (hWheel, rect.Width);
			Drawing.Rectangle r = new Drawing.Rectangle ();

			bool visibleWheel  = (rect.Height > 160);
			bool visibleFields = (rect.Height > 3*20);

			bool visibleFieldsRgb  = visibleFields;
			bool visibleFieldsCmyk = visibleFields;
			bool visibleFieldsGray = visibleFields;

			switch (this.Color.ColorSpace)
			{
				case Drawing.ColorSpace.Rgb:
					visibleFieldsCmyk = false;
					visibleFieldsGray = false;
					break;

				case Drawing.ColorSpace.Cmyk:
					visibleFieldsRgb  = false;
					visibleFieldsGray = false;
					break;

				case Drawing.ColorSpace.Gray:
					visibleFieldsRgb  = false;
					visibleFieldsCmyk = false;
					break;

				default:
					visibleFieldsRgb  = false;
					visibleFieldsCmyk = false;
					visibleFieldsGray = false;
					break;
			}

			r.Left   = rect.Left;
			r.Right  = rect.Left + hWheel;
			r.Bottom = rect.Top - hWheel;
			r.Top    = rect.Top;
			this.wheel.SetManualBounds (r);
			this.wheel.Visibility = (visibleWheel);

			double dx = System.Math.Floor ((rect.Width-hWheel-10)/this.palette.ColumnCount);
			if (dx > 4)
			{
				double dy = System.Math.Floor (hWheel/(this.palette.RowCount));
				dx = System.Math.Min (dx, dy)+1;

				r.Left = rect.Right-(dx*this.palette.ColumnCount-1)+1-15;
				r.Right = rect.Right+1;
				r.Top = rect.Top;
				r.Bottom = rect.Top-(dx*this.palette.RowCount-1);
				this.palette.SetManualBounds (r);
				this.palette.Show ();
			}
			else
			{
				this.palette.Hide ();
			}

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for (int i=0; i<=2; i++)  // r,g,b
			{
				r.Left  = 10;
				r.Width = 12;
				this.labels[i].SetManualBounds (r);
				this.labels[i].Visibility = (visibleFieldsRgb);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].SetManualBounds (r);
				this.fields[i].Visibility = (visibleFieldsRgb);

				r.Offset (0, -19);
			}

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for (int i=4; i<=6; i++)  // t,s,i
			{
				r.Left  = 10+70;
				r.Width = 12;
				this.labels[i].SetManualBounds (r);
				this.labels[i].Visibility = (visibleFieldsRgb);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].SetManualBounds (r);
				this.fields[i].Visibility = (visibleFieldsRgb);

				r.Offset (0, -19);
			}

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for (int i=7; i<=9; i++)  // c,m,y
			{
				r.Left  = 10;
				r.Width = 12;
				this.labels[i].SetManualBounds (r);
				this.labels[i].Visibility = (visibleFieldsCmyk);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].SetManualBounds (r);
				this.fields[i].Visibility = (visibleFieldsCmyk);

				r.Offset (0, -19);
			}

			r.Top    = rect.Bottom+1*19;
			r.Bottom = r.Top-20;
			r.Left  = 10+70;
			r.Width = 12;
			this.labels[10].SetManualBounds (r);
			this.labels[10].Visibility = (visibleFieldsCmyk);

			r.Left  = r.Right;
			r.Width = 50;
			this.fields[10].SetManualBounds (r);
			this.fields[10].Visibility = (visibleFieldsCmyk);

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			r.Left  = 10;
			r.Width = 12;
			this.labels[11].SetManualBounds (r);
			this.labels[11].Visibility = (visibleFieldsGray);

			r.Left  = r.Right;
			r.Width = 50;
			this.fields[11].SetManualBounds (r);
			this.fields[11].Visibility = (visibleFieldsGray);

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for (int i=3; i<=3; i++)  // a
			{
				r.Left  = 10+70+70;
				r.Width = 12;
				this.labels[i].SetManualBounds (r);
				this.labels[i].Visibility = (visibleFields);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].SetManualBounds (r);
				this.fields[i].Visibility = (visibleFields);

				r.Offset (0, -19);
			}

			r.Left  = 10+70+70;
			r.Width = 12;
			this.labelHexa.SetManualBounds (r);

			r.Left  = r.Right;
			r.Width = 50;
			this.fieldHexa.SetManualBounds (r);

			r.Offset (0, -19);

			r.Top    = r.Top-2;
			r.Bottom = r.Bottom+1;
			r.Right = this.fields[3].ActualBounds.Right;
			r.Left  = r.Right - r.Height;
			this.picker.SetManualBounds (r);
			this.picker.Visibility = (visibleFields);

			if (this.CloseButtonVisibility)
			{
				r.Left = rect.Left;
				r.Width = 14;
				r.Bottom = rect.Top-14;
				r.Top = rect.Top;

				this.closeButton.SetManualBounds (r);
			}

			r.Left = rect.Left;
			r.Width = 14;
			r.Bottom = rect.Top-hWheel;
			r.Height = 14;
			this.buttonRgb.SetManualBounds (r);
			r.Offset (14, 0);
			this.buttonCmyk.SetManualBounds (r);
			r.Offset (14, 0);
			this.buttonGray.SetManualBounds (r);
		}


		private void HandlePickerHotColorChanged(object sender)
		{
			this.Color = new Drawing.RichColor (this.picker.HotColor);
		}

		private void HandleTextAlphaChanged(object sender)
		{
			//	La valeur alpha a été changée.
			if (this.suspendColorEvents == 0)
			{
				switch (this.Color.ColorSpace)
				{
					case Drawing.ColorSpace.Rgb:
						this.FieldsRgbToColor ();
						break;

					case Drawing.ColorSpace.Cmyk:
						this.FieldsCmykToColor ();
						break;
					case Drawing.ColorSpace.Gray:
						this.FieldsGrayToColor ();
						break;
				}
			}
		}

		private void HandleTextRgbChanged(object sender)
		{
			//	Une valeur RGB a été changée.
			if (this.suspendColorEvents == 0)
			{
				this.FieldsRgbToColor ();
			}
		}

		private void HandleTextHsvChanged(object sender)
		{
			//	Une valeur HSV a été changée.
			if (this.suspendColorEvents == 0)
			{
				this.FieldsHsvToColor ();
			}
		}

		private void HandleTextCmykChanged(object sender)
		{
			//	Une valeur CMYK a été changée.
			if (this.suspendColorEvents == 0)
			{
				this.FieldsCmykToColor ();
			}
		}

		private void HandleTextGrayChanged(object sender)
		{
			//	Une valeur Gray a été changée.
			if (this.suspendColorEvents == 0)
			{
				this.FieldsGrayToColor ();
			}
		}

		private void HandleTextHexaChanged(object sender)
		{
			//	Une valeur Hexa a été changée.
			if (this.suspendColorEvents == 0)
			{
				this.FieldsHexaToColor ();
			}
		}

		private void HandleWheelChanged(object sender)
		{
			//	Couleur dans le cercle changée.
			if (this.suspendColorEvents == 0)
			{
				this.suspendColorEvents++;
				this.color = this.wheel.Color;
				this.ColorToFieldsRgb ();
				this.ColorToFieldsHsv ();
				this.ColorToFieldsCmyk ();
				this.ColorToFieldsGray ();
				this.ColorToFieldsHexa ();
				this.ColorizeSliders ();
				this.Invalidate ();
				this.OnColorChanged ();
				this.suspendColorEvents--;
			}
		}

		private void HandlePaletteExport(object sender)
		{
			//	Couleur dans palette cliquée.
			this.Color = this.palette.SelectedColor;
			this.OnColorChanged ();
		}

		private void HandlePaletteImport(object sender)
		{
			//	Couleur dans palette cliquée.
			this.palette.WriteSelectedColor (this.Color);
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.OnCloseClicked ();
		}

		private void HandleColorSpaceChanged(object sender)
		{
			switch (this.colorSpaceController.ActiveIndex)
			{
				case 1:
					this.color.ColorSpace = Drawing.ColorSpace.Rgb;
					break;
				case 2:
					this.color.ColorSpace = Drawing.ColorSpace.Cmyk;
					break;
				case 3:
					this.color.ColorSpace = Drawing.ColorSpace.Gray;
					break;
			}

			this.wheel.Color = this.color;

			this.suspendColorEvents++;
			this.UpdateColors ();
			this.suspendColorEvents--;
		}


		private void OnColorChanged()
		{
			//	Generate the event only if the color really changed since the
			//	last time (avoiding useless notifications).

			if (this.color == this.colorBeforeChange)
			{
				return;
			}

			var e = new DependencyPropertyChangedEventArgs ("Color", this.colorBeforeChange, this.color);
			var handler = this.GetUserEventHandler<DependencyPropertyChangedEventArgs> (ColorSelector.ColorChangedEvent);

			this.colorBeforeChange = this.color;
			
			if (handler != null)
			{
				handler (this, e);
			}
		}

		private void OnCloseClicked()
		{
			var handler = this.GetUserEventHandler (ColorSelector.CloseClickedEvent);
			
			if (handler != null)
			{
				handler (this);
			}
		}


		static private int ParseHexa(string hexa)
		{
			int result;

			if (System.Int32.TryParse (hexa, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out result))
			{
				return result;
			}
			else
			{
				return 0;
			}
		}


		public event EventHandler<DependencyPropertyChangedEventArgs> ColorChanged
		{
			add
			{
				this.AddUserEventHandler (ColorSelector.ColorChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ColorSelector.ColorChangedEvent, value);
			}
		}

		public event EventHandler				CloseClicked
		{
			add
			{
				this.AddUserEventHandler (ColorSelector.CloseClickedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ColorSelector.CloseClickedEvent, value);
			}
		}

		
		static ColorSelector()
		{
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (221.0);

			Visual.PreferredHeightProperty.OverrideMetadata (typeof (ColorSelector), metadataDy);
		}

		
		private const int						FieldCount = 4+3+4+1;

		private const string					ColorChangedEvent = "ColorChanged";
		private const string					CloseClickedEvent = "CloseClicked";

		private Drawing.RichColor				colorBeforeChange;
		private Drawing.RichColor				color;
		private ColorWheel						wheel;
		private ColorPalette					palette;
		private StaticText[]					labels;
		private TextFieldSlider[]				fields;
		private StaticText						labelHexa;
		private TextField						fieldHexa;
		private int								suspendColorEvents;
		private GlyphButton						closeButton;

		private Helpers.GroupController			colorSpaceController;
		private IconButton						buttonRgb;
		private IconButton						buttonCmyk;
		private IconButton						buttonGray;
		private Tools.MagnifierDragSource		picker;
		private Drawing.RichColor				blackReplacement;
	}
}
