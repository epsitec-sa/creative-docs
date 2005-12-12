namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe ColorSelector permet de choisir une couleur.
	/// </summary>
	public class ColorSelector : Widget
	{
		public ColorSelector()
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	mani�re, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.nbField = 4+3+4+1;
			this.labels = new StaticText[this.nbField];
			this.fields = new TextFieldSlider[this.nbField];
			for ( int i=0 ; i<this.nbField ; i++ )
			{
				this.labels[i] = new StaticText(this);
				this.fields[i] = new TextFieldSlider(this);

				this.fields[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.fields[i].Value = 0;

				this.fields[i].DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.fields[i].AutoSelectOnFocus = true;
				this.fields[i].SwallowEscape = true;

				if ( i < 3 )  // r,g,b ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 255;
					this.fields[i].Step = 10;
					this.fields[i].EditionAccepted += new Support.EventHandler(this.HandleTextRGBChanged);
				}
				else if ( i == 3 )  // a ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 255;
					this.fields[i].Step = 10;
					this.fields[i].EditionAccepted += new Support.EventHandler(this.HandleTextAlphaChanged);
				}
				else if ( i == 4 )  // t ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 360;
					this.fields[i].Step = 10;
					this.fields[i].EditionAccepted += new Support.EventHandler(this.HandleTextHSVChanged);
				}
				else if ( i < 7 )  // s,i ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 100;
					this.fields[i].Step = 5;
					this.fields[i].EditionAccepted += new Support.EventHandler(this.HandleTextHSVChanged);
				}
				else if ( i< 11 )  // c,m,y,k ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 100;
					this.fields[i].Step = 5;
					this.fields[i].EditionAccepted += new Support.EventHandler(this.HandleTextCMYKChanged);
				}
				else	// g ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 100;
					this.fields[i].Step = 5;
					this.fields[i].EditionAccepted += new Support.EventHandler(this.HandleTextGrayChanged);
				}
			}

			int index = 100;
			this.fields[ 0].TabIndex = index++;
			this.fields[ 1].TabIndex = index++;
			this.fields[ 2].TabIndex = index++;
			this.fields[ 4].TabIndex = index++;
			this.fields[ 5].TabIndex = index++;
			this.fields[ 6].TabIndex = index++;
			this.fields[ 7].TabIndex = index++;
			this.fields[ 8].TabIndex = index++;
			this.fields[ 9].TabIndex = index++;
			this.fields[10].TabIndex = index++;
			this.fields[11].TabIndex = index++;
			this.fields[ 3].TabIndex = index++;

			this.fields[0].Color = Drawing.Color.FromRGB(1,0,0);  // r
			this.fields[1].Color = Drawing.Color.FromRGB(0,1,0);  // v
			this.fields[2].Color = Drawing.Color.FromRGB(0,0,1);  // b
			this.fields[3].Color = Drawing.Color.FromRGB(0.5,0.5,0.5);
			ToolTip.Default.SetToolTip(this.fields[0], Res.Strings.ColorSelector.LongRed);
			ToolTip.Default.SetToolTip(this.fields[1], Res.Strings.ColorSelector.LongGreen);
			ToolTip.Default.SetToolTip(this.fields[2], Res.Strings.ColorSelector.LongBlue);
			ToolTip.Default.SetToolTip(this.fields[3], Res.Strings.ColorSelector.LongAlpha);

			this.fields[4].TextSuffix = "\u00B0";  // symbole unicode "degr�" (#176)
			this.fields[4].Color = Drawing.Color.FromRGB(0,0,0);
			this.fields[4].BackColor = Drawing.Color.FromRGB(0.5,0.5,0.5);
			ToolTip.Default.SetToolTip(this.fields[4], Res.Strings.ColorSelector.LongHue);

			this.fields[5].TextSuffix = "%";
			this.fields[5].Color = Drawing.Color.FromRGB(0,0,0);
			this.fields[5].BackColor = Drawing.Color.FromRGB(1,1,1);
			ToolTip.Default.SetToolTip(this.fields[5], Res.Strings.ColorSelector.LongSaturation);
			
			this.fields[6].TextSuffix = "%";
			this.fields[6].Color = Drawing.Color.FromRGB(1,1,1);
			this.fields[6].BackColor = Drawing.Color.FromRGB(0,0,0);
			ToolTip.Default.SetToolTip(this.fields[6], Res.Strings.ColorSelector.LongValue);

			this.fields[ 7].Color = Drawing.Color.FromRGB(0,1,1);  // c
			this.fields[ 8].Color = Drawing.Color.FromRGB(1,0,1);  // m
			this.fields[ 9].Color = Drawing.Color.FromRGB(1,1,0);  // y
			this.fields[10].Color = Drawing.Color.FromRGB(0,0,0);  // k
			this.fields[10].BackColor = Drawing.Color.FromRGB(1,1,1);
			ToolTip.Default.SetToolTip(this.fields[ 7], Res.Strings.ColorSelector.LongCyan);
			ToolTip.Default.SetToolTip(this.fields[ 8], Res.Strings.ColorSelector.LongMagenta);
			ToolTip.Default.SetToolTip(this.fields[ 9], Res.Strings.ColorSelector.LongYellow);
			ToolTip.Default.SetToolTip(this.fields[10], Res.Strings.ColorSelector.LongBlack);

			this.fields[11].Color = Drawing.Color.FromRGB(1,1,1);  // g
			this.fields[11].BackColor = Drawing.Color.FromRGB(0,0,0);
			ToolTip.Default.SetToolTip(this.fields[11], Res.Strings.ColorSelector.LongGray);

			this.labels[ 0].Text = Res.Strings.ColorSelector.ShortRed;
			this.labels[ 1].Text = Res.Strings.ColorSelector.ShortGreen;
			this.labels[ 2].Text = Res.Strings.ColorSelector.ShortBlue;
			this.labels[ 3].Text = Res.Strings.ColorSelector.ShortAlpha;
			this.labels[ 4].Text = Res.Strings.ColorSelector.ShortHue;
			this.labels[ 5].Text = Res.Strings.ColorSelector.ShortSaturation;
			this.labels[ 6].Text = Res.Strings.ColorSelector.ShortValue;
			this.labels[ 7].Text = Res.Strings.ColorSelector.ShortCyan;
			this.labels[ 8].Text = Res.Strings.ColorSelector.ShortMagenta;
			this.labels[ 9].Text = Res.Strings.ColorSelector.ShortYellow;
			this.labels[10].Text = Res.Strings.ColorSelector.ShortBlack;
			this.labels[11].Text = Res.Strings.ColorSelector.ShortGray;

			this.labelHexa = new StaticText(this);
			this.labelHexa.Text = Res.Strings.ColorSelector.ShortHexa;

			this.fieldHexa = new TextField(this);
			this.fieldHexa.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.fieldHexa.AutoSelectOnFocus = true;
			this.fieldHexa.SwallowEscape = true;
			this.fieldHexa.EditionAccepted += new Support.EventHandler(this.HandleTextHexaChanged);
			this.fieldHexa.TabIndex = 200;
			this.fieldHexa.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldHexa, Res.Strings.ColorSelector.LongHexa);

			this.circle = new ColorWheel(this);
			this.circle.Changed += new Support.EventHandler(this.HandleCircleChanged);

			this.palette = new ColorPalette(this);
			this.palette.HasOptionButton = true;
			this.palette.Export += new Support.EventHandler(this.HandlePaletteExport);
			this.palette.Import += new Support.EventHandler(this.HandlePaletteImport);
			this.palette.TabIndex = 10;
			this.palette.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.picker = new Tools.Magnifier.DragSource(this);
			this.picker.HotColorChanged += new Support.EventHandler(this.HandlePickerHotColorChanged);
			ToolTip.Default.SetToolTip(this.picker, Res.Strings.ColorSelector.Picker);

			this.buttonClose = new GlyphButton(this);
			this.buttonClose.GlyphShape = GlyphShape.Close;
			this.buttonClose.ButtonStyle = ButtonStyle.Normal;
			this.buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonClose, Res.Strings.ColorSelector.Close);
			
			this.colorSpaceController = Helpers.GroupController.GetGroupController(this, "ColorSpace");
			this.colorSpaceController.Changed += new Support.EventHandler(this.HandleColorSpaceChanged);
			
			this.buttonRGB = new IconButton(this);
			this.buttonRGB.AutoRadio = true;
			this.buttonRGB.AutoToggle = true;
			this.buttonRGB.Group = this.colorSpaceController.Group;
			this.buttonRGB.Index = 1;
			this.buttonRGB.IconName = "manifest:Epsitec.Common.Widgets.Images.ColorSpaceRGB.icon";
			this.buttonRGB.TabIndex = 21;
			this.buttonRGB.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonRGB, Res.Strings.ColorSelector.ColorSpace.RGB);

			this.buttonCMYK = new IconButton(this);
			this.buttonCMYK.AutoRadio = true;
			this.buttonCMYK.AutoToggle = true;
			this.buttonCMYK.Group = this.colorSpaceController.Group;
			this.buttonCMYK.Index = 2;
			this.buttonCMYK.IconName = "manifest:Epsitec.Common.Widgets.Images.ColorSpaceCMYK.icon";
			this.buttonCMYK.TabIndex = 21;
			this.buttonCMYK.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonCMYK, Res.Strings.ColorSelector.ColorSpace.CMYK);

			this.buttonGray = new IconButton(this);
			this.buttonGray.AutoRadio = true;
			this.buttonGray.AutoToggle = true;
			this.buttonGray.Group = this.colorSpaceController.Group;
			this.buttonGray.Index = 3;
			this.buttonGray.IconName = "manifest:Epsitec.Common.Widgets.Images.ColorSpaceGray.icon";
			this.buttonGray.TabIndex = 21;
			this.buttonGray.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonGray, Res.Strings.ColorSelector.ColorSpace.Gray);
		}
		
		public ColorSelector(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double					DefaultHeight
		{
			get
			{
				return 221;
			}
		}

		public Drawing.RichColor				Color
		{
			get
			{
				return this.color;
			}

			set
			{
				if ( value.IsEmpty )
				{
					value = Drawing.RichColor.FromARGB(0, 1,1,1);
				}

				if ( this.color != value )
				{
					bool isc = this.suspendColorEvents;
					this.suspendColorEvents = true;
					this.color = value;
					this.circle.Color = value;
					this.UpdateColors();
					this.suspendColorEvents = isc;
				}
			}
		}

		public ColorPalette						ColorPalette
		{
			get
			{
				return this.palette;
			}

			set
			{
				if ( this.palette != value )
				{
					if ( this.palette != null )
					{
						this.palette.Dispose();
					}

					this.palette = value;
				}
			}
		}
		
		public bool								HasCloseButton
		{
			get
			{
				return this.hasCloseButton;
			}

			set
			{
				if ( this.hasCloseButton != value )
				{
					this.hasCloseButton = value;
					this.UpdateClientGeometry();
				}
			}
		}


		// Met tout � jour apr�s un changement de couleur.
		protected void UpdateColors()
		{
			System.Diagnostics.Debug.Assert(this.suspendColorEvents == true);
			this.ColorToFieldsRGB();
			this.ColorToFieldsHSV();
			this.ColorToFieldsCMYK();
			this.ColorToFieldsGray();
			this.ColorToFieldsHexa();
			this.UpdateColorSpace();
			this.UpdateClientGeometry();
			this.Invalidate();
			this.OnChanged();
		}

		// Met � jour les boutons pour l'espace de couleur.
		protected void UpdateColorSpace()
		{
			Drawing.ColorSpace cs = this.color.ColorSpace;

			this.buttonRGB .ActiveState = (cs == Drawing.ColorSpace.RGB ) ? ActiveState.Yes : ActiveState.No;
			this.buttonCMYK.ActiveState = (cs == Drawing.ColorSpace.CMYK) ? ActiveState.Yes : ActiveState.No;
			this.buttonGray.ActiveState = (cs == Drawing.ColorSpace.Gray) ? ActiveState.Yes : ActiveState.No;
		}

		// Couleur -> textes �ditables.
		protected void ColorToFieldsRGB()
		{
			double a,r,g,b;
			this.color.Basic.GetARGB(out a, out r, out g, out b);
		
			this.fields[0].Value = (decimal) System.Math.Floor(r*255+0.5);
			this.fields[1].Value = (decimal) System.Math.Floor(g*255+0.5);
			this.fields[2].Value = (decimal) System.Math.Floor(b*255+0.5);
			this.fields[3].Value = (decimal) System.Math.Floor(a*255+0.5);
		}

		// Couleur -> textes �ditables.
		protected void ColorToFieldsHSV()
		{
			double h,s,v;
			this.circle.GetHSV(out h, out s, out v);
		
			this.fields[4].Value = (decimal) System.Math.Floor(System.Math.Floor(h+0.5));
			this.fields[5].Value = (decimal) System.Math.Floor(s*100+0.5);
			this.fields[6].Value = (decimal) System.Math.Floor(v*100+0.5);
		
			this.ColoriseSliders();
		}

		// Couleur -> textes �ditables.
		protected void ColorToFieldsCMYK()
		{
			double a = this.color.A;
			double c = this.color.C;
			double m = this.color.M;
			double y = this.color.Y;
			double k = this.color.K;
		
			this.fields[ 3].Value = (decimal) System.Math.Floor(a*255+0.5);
			this.fields[ 7].Value = (decimal) System.Math.Floor(c*100+0.5);
			this.fields[ 8].Value = (decimal) System.Math.Floor(m*100+0.5);
			this.fields[ 9].Value = (decimal) System.Math.Floor(y*100+0.5);
			this.fields[10].Value = (decimal) System.Math.Floor(k*100+0.5);
		}

		// Couleur -> textes �ditables.
		protected void ColorToFieldsGray()
		{
			double a = this.color.A;
			double g = this.color.Gray;
		
			this.fields[ 3].Value = (decimal) System.Math.Floor(a*255+0.5);
			this.fields[11].Value = (decimal) System.Math.Floor(g*100+0.5);
		}

		// Couleur -> textes �ditables.
		protected void ColorToFieldsHexa()
		{
			double a,r,g,b;
			this.color.Basic.GetARGB(out a, out r, out g, out b);

			r = Epsitec.Common.Math.Clip(r);
			g = Epsitec.Common.Math.Clip(g);
			b = Epsitec.Common.Math.Clip(b);

			int rr = (int) System.Math.Floor(r*255+0.5);
			int gg = (int) System.Math.Floor(g*255+0.5);
			int bb = (int) System.Math.Floor(b*255+0.5);

			string text = string.Format("#{0}{1}{2}", rr.ToString("x2"), gg.ToString("x2"), bb.ToString("x2"));
			this.fieldHexa.Text = text;
			this.fieldHexa.SelectAll();
		}

		// Textes �ditables RGB -> couleur.
		protected void FieldsRGBToColor()
		{
			double r = (double) this.fields[0].Value/255;
			double g = (double) this.fields[1].Value/255;
			double b = (double) this.fields[2].Value/255;
			double a = (double) this.fields[3].Value/255;

			bool isc = this.suspendColorEvents;
			this.suspendColorEvents = true;
			this.color = Drawing.RichColor.FromARGB(a,r,g,b);
			this.circle.Color = this.color;
			this.ColorToFieldsHSV();
			this.ColorToFieldsCMYK();
			this.ColorToFieldsGray();
			this.ColorToFieldsHexa();
			this.ColoriseSliders();
			this.Invalidate();
			this.OnChanged();
			this.suspendColorEvents = isc;
		}

		// Textes �ditables HSV -> couleur.
		protected void FieldsHSVToColor()
		{
			double h = (double) this.fields[4].Value;
			double s = (double) this.fields[5].Value/100;
			double v = (double) this.fields[6].Value/100;

			bool isc = this.suspendColorEvents;
			this.suspendColorEvents = true;
			this.circle.SetHSV(h,s,v);
			this.color = Drawing.RichColor.FromAHSV(this.color.A, h,s,v);
			this.ColorToFieldsRGB();
			this.ColorToFieldsCMYK();
			this.ColorToFieldsGray();
			this.ColorToFieldsHexa();
			this.ColoriseSliders();
			this.Invalidate();
			this.OnChanged();
			this.suspendColorEvents = isc;
		}

		// Textes �ditables CMYK -> couleur.
		protected void FieldsCMYKToColor()
		{
			double a = (double) this.fields[ 3].Value/255;
			double c = (double) this.fields[ 7].Value/100;
			double m = (double) this.fields[ 8].Value/100;
			double y = (double) this.fields[ 9].Value/100;
			double k = (double) this.fields[10].Value/100;

			bool isc = this.suspendColorEvents;
			this.suspendColorEvents = true;
			this.color = Drawing.RichColor.FromACMYK(a,c,m,y,k);
			this.circle.Color = this.color;
			this.ColorToFieldsRGB();
			this.ColorToFieldsHSV();
			this.ColorToFieldsGray();
			this.ColorToFieldsHexa();
			this.ColoriseSliders();
			this.Invalidate();
			this.OnChanged();
			this.suspendColorEvents = isc;
		}

		// Textes �ditables Gray -> couleur.
		protected void FieldsGrayToColor()
		{
			double a = (double) this.fields[ 3].Value/255;
			double g = (double) this.fields[11].Value/100;

			bool isc = this.suspendColorEvents;
			this.suspendColorEvents = true;
			this.color = Drawing.RichColor.FromAGray(a,g);
			this.circle.SetAGray(a,g);
			this.ColorToFieldsRGB();
			this.ColorToFieldsHSV();
			this.ColorToFieldsCMYK();
			this.ColorToFieldsHexa();
			this.ColoriseSliders();
			this.Invalidate();
			this.OnChanged();
			this.suspendColorEvents = isc;
		}

		// Textes �ditables Hexa -> couleur.
		protected void FieldsHexaToColor()
		{
			double r = 0;
			double g = 0;
			double b = 0;
			double a = (double) this.fields[3].Value/255;

			string text = this.fieldHexa.Text;
			if ( text.Length >= 1 && text[0] == '#' )
			{
				text = text.Substring(1);  // supprime le #
			}

			if ( text.Length == 3 )
			{
				int rr = ColorSelector.ParseHexa(text.Substring(0, 1));
				int gg = ColorSelector.ParseHexa(text.Substring(1, 1));
				int bb = ColorSelector.ParseHexa(text.Substring(2, 1));
				
				r = (rr * 16 + rr) / 255.0;
				g = (gg * 16 + gg) / 255.0;
				b = (bb * 16 + bb) / 255.0;
			}
			else
			{
				if ( text.Length >= 2 )
				{
					r = ColorSelector.ParseHexa(text.Substring(0, 2))/255.0;
				}

				if ( text.Length >= 4 )
				{
					g = ColorSelector.ParseHexa(text.Substring(2, 2))/255.0;
				}
				
				if ( text.Length >= 6 )
				{
					b = ColorSelector.ParseHexa(text.Substring(4, 2))/255.0;
				}
			}
			
			bool isc = this.suspendColorEvents;
			this.suspendColorEvents = true;
			this.color = Drawing.RichColor.FromARGB(a,r,g,b);
			this.circle.Color = this.color;
			this.ColorToFieldsRGB();
			this.ColorToFieldsHSV();
			this.ColorToFieldsCMYK();
			this.ColorToFieldsGray();
			this.ColoriseSliders();
			this.Invalidate();
			this.OnChanged();
			this.suspendColorEvents = isc;
		}

		// Colorise certains sliders en fonction de la couleur d�finie.
		protected void ColoriseSliders()
		{
			double h,s,v;
			this.circle.GetHSV(out h, out s, out v);
			Drawing.Color saturated = Drawing.Color.FromHSV(h,1,1);

			this.fields[4].Color = saturated;

			if ( s == 0.0 )  // couleur grise ?
			{
				saturated = Drawing.Color.FromBrightness(1.0);  // blanc
			}

			this.fields[5].Color = saturated;
			this.fields[6].Color = saturated;
		}


		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fields == null )  return;
			
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(1);
			double hCircle = rect.Height-5-20*3;
			hCircle = System.Math.Min(hCircle, rect.Width);
			Drawing.Rectangle r = new Drawing.Rectangle();

			bool visibleCircle = ( rect.Height > 160 );
			bool visibleFields = ( rect.Height > 3*20 );

			bool visibleFieldsRGB  = visibleFields;
			bool visibleFieldsCMYK = visibleFields;
			bool visibleFieldsGray = visibleFields;

			switch ( this.Color.ColorSpace )
			{
				case Drawing.ColorSpace.RGB:
					visibleFieldsCMYK = false;
					visibleFieldsGray = false;
					break;

				case Drawing.ColorSpace.CMYK:
					visibleFieldsRGB  = false;
					visibleFieldsGray = false;
					break;
				
				case Drawing.ColorSpace.Gray:
					visibleFieldsRGB  = false;
					visibleFieldsCMYK = false;
					break;
				
				default:
					visibleFieldsRGB  = false;
					visibleFieldsCMYK = false;
					visibleFieldsGray = false;
					break;
			}

			r.Left   = rect.Left;
			r.Right  = rect.Left + hCircle;
			r.Bottom = rect.Top - hCircle;
			r.Top    = rect.Top;
			this.circle.Bounds = r;
			this.circle.Visibility = (visibleCircle);

			double dx = System.Math.Floor((rect.Width-hCircle-10)/this.palette.Columns);
			if ( dx > 4 )
			{
				double dy = System.Math.Floor(hCircle/(this.palette.Rows));
				dx = System.Math.Min(dx, dy)+1;

				r.Left = rect.Right-(dx*this.palette.Columns-1)+1-15;
				r.Right = rect.Right+1;
				r.Top = rect.Top;
				r.Bottom = rect.Top-(dx*this.palette.Rows-1);
				this.palette.Bounds = r;
				this.palette.Show();
			}
			else
			{
				this.palette.Hide();
			}

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for ( int i=0 ; i<=2 ; i++ )  // r,g,b
			{
				r.Left  = 10;
				r.Width = 12;
				this.labels[i].Bounds = r;
				this.labels[i].Visibility = (visibleFieldsRGB);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].Visibility = (visibleFieldsRGB);

				r.Offset(0, -19);
			}

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for ( int i=4 ; i<=6 ; i++ )  // t,s,i
			{
				r.Left  = 10+70;
				r.Width = 12;
				this.labels[i].Bounds = r;
				this.labels[i].Visibility = (visibleFieldsRGB);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].Visibility = (visibleFieldsRGB);

				r.Offset(0, -19);
			}
			
			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for ( int i=7 ; i<=9 ; i++ )  // c,m,y
			{
				r.Left  = 10;
				r.Width = 12;
				this.labels[i].Bounds = r;
				this.labels[i].Visibility = (visibleFieldsCMYK);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].Visibility = (visibleFieldsCMYK);

				r.Offset(0, -19);
			}

			r.Top    = rect.Bottom+1*19;
			r.Bottom = r.Top-20;
			r.Left  = 10+70;
			r.Width = 12;
			this.labels[10].Bounds = r;
			this.labels[10].Visibility = (visibleFieldsCMYK);

			r.Left  = r.Right;
			r.Width = 50;
			this.fields[10].Bounds = r;
			this.fields[10].Visibility = (visibleFieldsCMYK);

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			r.Left  = 10;
			r.Width = 12;
			this.labels[11].Bounds = r;
			this.labels[11].Visibility = (visibleFieldsGray);

			r.Left  = r.Right;
			r.Width = 50;
			this.fields[11].Bounds = r;
			this.fields[11].Visibility = (visibleFieldsGray);

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for ( int i=3 ; i<=3 ; i++ )  // a
			{
				r.Left  = 10+70+70;
				r.Width = 12;
				this.labels[i].Bounds = r;
				this.labels[i].Visibility = (visibleFields);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].Visibility = (visibleFields);

				r.Offset(0, -19);
			}
			
			r.Left  = 10+70+70;
			r.Width = 12;
			this.labelHexa.Bounds = r;

			r.Left  = r.Right;
			r.Width = 50;
			this.fieldHexa.Bounds = r;

			r.Offset(0, -19);

			r.Top    = r.Top-2;
			r.Bottom = r.Bottom+1;
			r.Right = this.fields[3].Right;
			r.Left  = r.Right - r.Height;
			this.picker.Bounds = r;
			this.picker.Visibility = (visibleFields);

			if ( this.hasCloseButton )
			{
				r.Left = rect.Left;
				r.Width = 14;
				r.Bottom = rect.Top-14;
				r.Top = rect.Top;
				
				System.Diagnostics.Debug.WriteLine ("Setting close button bounds to " + r.ToString());
				
				this.buttonClose.Bounds = r;
				this.buttonClose.Visibility = true;
			}
			else
			{
				this.buttonClose.Visibility = false;
			}

			r.Left = rect.Left;
			r.Width = 14;
			r.Bottom = rect.Top-hCircle;
			r.Height = 14;
			this.buttonRGB.Bounds = r;
			r.Offset(14, 0);
			this.buttonCMYK.Bounds = r;
			r.Offset(14, 0);
			this.buttonGray.Bounds = r;
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if ( !this.BackColor.IsEmpty )
			{
				graphics.AddFilledRectangle(this.Client.Bounds);
				graphics.RenderSolid(this.BackColor);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<this.nbField ; i++ )
				{
					if ( i < 3 )
					{
						this.fields[i].EditionAccepted -= new Support.EventHandler(this.HandleTextRGBChanged);
					}
					else if ( i == 3 )
					{
						this.fields[i].EditionAccepted -= new Support.EventHandler(this.HandleTextAlphaChanged);
					}
					else if ( i == 4 )
					{
						this.fields[i].EditionAccepted -= new Support.EventHandler(this.HandleTextHSVChanged);
					}
					else if ( i < 7 )
					{
						this.fields[i].EditionAccepted -= new Support.EventHandler(this.HandleTextHSVChanged);
					}
					else if ( i < 11 )
					{
						this.fields[i].EditionAccepted -= new Support.EventHandler(this.HandleTextCMYKChanged);
					}
					else
					{
						this.fields[i].EditionAccepted -= new Support.EventHandler(this.HandleTextGrayChanged);
					}
				}
				
				if ( this.fieldHexa != null )
				{
					this.fieldHexa.EditionAccepted -= new Support.EventHandler(this.HandleTextHexaChanged);
				}

				if ( this.circle != null )
				{
					this.circle.Changed -= new Support.EventHandler(this.HandleCircleChanged);
				}

				if ( this.palette != null )
				{
					this.palette.Export -= new Support.EventHandler(this.HandlePaletteExport);
					this.palette.Import -= new Support.EventHandler(this.HandlePaletteImport);
				}

				if ( this.picker != null )
				{
					this.picker.HotColorChanged -= new Support.EventHandler(this.HandlePickerHotColorChanged);
				}
			}
			
			base.Dispose(disposing);
		}

		
		private void HandlePickerHotColorChanged(object sender)
		{
			this.Color = new Drawing.RichColor(this.picker.HotColor);
		}
		
		// La valeur alpha a �t� chang�e.
		private void HandleTextAlphaChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				if ( this.Color.ColorSpace == Drawing.ColorSpace.RGB  )
				{
					this.FieldsRGBToColor();
				}
			
				if ( this.Color.ColorSpace == Drawing.ColorSpace.CMYK )
				{
					this.FieldsCMYKToColor();
				}
			
				if ( this.Color.ColorSpace == Drawing.ColorSpace.Gray )
				{
					this.FieldsGrayToColor();
				}
			}
		}

		// Une valeur RGB a �t� chang�e.
		private void HandleTextRGBChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.FieldsRGBToColor();
			}
		}

		// Une valeur HSV a �t� chang�e.
		private void HandleTextHSVChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.FieldsHSVToColor();
			}
		}

		// Une valeur CMYK a �t� chang�e.
		private void HandleTextCMYKChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.FieldsCMYKToColor();
			}
		}

		// Une valeur Gray a �t� chang�e.
		private void HandleTextGrayChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.FieldsGrayToColor();
			}
		}

		// Une valeur Hexa a �t� chang�e.
		private void HandleTextHexaChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.FieldsHexaToColor();
			}
		}

		// Couleur dans le cercle chang�e.
		private void HandleCircleChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				bool isc = this.suspendColorEvents;
				this.suspendColorEvents = true;
				this.color = this.circle.Color;
				this.ColorToFieldsRGB();
				this.ColorToFieldsHSV();
				this.ColorToFieldsCMYK();
				this.ColorToFieldsGray();
				this.ColorToFieldsHexa();
				this.ColoriseSliders();
				this.Invalidate();
				this.OnChanged();
				this.suspendColorEvents = isc;
			}
		}

		// Couleur dans palette cliqu�e.
		private void HandlePaletteExport(object sender)
		{
			this.Color = this.palette.Color;
			this.OnChanged();
		}

		// Couleur dans palette cliqu�e.
		private void HandlePaletteImport(object sender)
		{
			this.palette.Color = this.Color;
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.OnCloseClicked();
		}

		private void HandleColorSpaceChanged(object sender)
		{
			switch( this.colorSpaceController.ActiveIndex )
			{
				case 1: this.color.ColorSpace = Drawing.ColorSpace.RGB;		break;
				case 2: this.color.ColorSpace = Drawing.ColorSpace.CMYK;	break;
				case 3: this.color.ColorSpace = Drawing.ColorSpace.Gray;	break;
			}
			
			this.circle.Color = this.color;

			bool isc = this.suspendColorEvents;
			this.suspendColorEvents = true;
			this.UpdateColors();
			this.suspendColorEvents = isc;
		}
		
		
		// G�n�re un �v�nement pour dire �a a chang�.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un �coute ?
			{
				this.Changed(this);
			}
		}

		protected virtual void OnCloseClicked()
		{
			if ( this.CloseClicked != null )
			{
				this.CloseClicked(this);
			}
		}
		
		
		// Conversion d'une cha�ne hexad�cimale en un entier.
		static protected int ParseHexa(string hexa)
		{
			int i = 0;

			try
			{
				i = System.Int32.Parse(hexa, System.Globalization.NumberStyles.AllowHexSpecifier);
			}
			catch
			{
				i = 0;
			}

			return i;
		}


		public event Support.EventHandler		Changed;
		public event Support.EventHandler		CloseClicked;

		protected Drawing.RichColor				color = Drawing.RichColor.Empty;
		protected ColorWheel					circle;
		protected ColorPalette					palette;
		protected int							nbField;
		protected StaticText[]					labels;
		protected TextFieldSlider[]				fields;
		protected StaticText					labelHexa;
		protected TextField						fieldHexa;
		protected bool							suspendColorEvents = false;
		protected bool							hasCloseButton = false;
		protected GlyphButton					buttonClose;
		
		protected Helpers.GroupController		colorSpaceController;
		protected IconButton					buttonRGB;
		protected IconButton					buttonCMYK;
		protected IconButton					buttonGray;
		private Tools.Magnifier.DragSource		picker;
	}
}
