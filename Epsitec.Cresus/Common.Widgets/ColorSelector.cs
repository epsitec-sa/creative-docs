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
			this.black = Drawing.Color.FromName("WindowFrame");

			this.nbField = 4+3;
			this.labels = new StaticText[this.nbField];
			this.fields = new TextFieldSlider[this.nbField];
			for ( int i=0 ; i<this.nbField ; i++ )
			{
				this.labels[i] = new StaticText(this);
				this.fields[i] = new TextFieldSlider(this);

				this.fields[i].TabIndex = i+100;
				this.fields[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.fields[i].Value = 0;
				if ( i < 4 )  // r,g,b,a ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 255;
					this.fields[i].Step = 10;
					this.fields[i].TextChanged += new Support.EventHandler(this.HandleTextRGBChanged);
				}
				else if ( i == 4 )  // t ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 360;
					this.fields[i].Step = 10;
					this.fields[i].TextChanged += new Support.EventHandler(this.HandleTextHSVChanged);
				}
				else	// s,i ?
				{
					this.fields[i].MinValue = 0;
					this.fields[i].MaxValue = 100;
					this.fields[i].Step = 5;
					this.fields[i].TextChanged += new Support.EventHandler(this.HandleTextHSVChanged);
				}
			}
			this.fields[0].Color = Drawing.Color.FromRGB(1,0,0);
			this.fields[1].Color = Drawing.Color.FromRGB(0,1,0);
			this.fields[2].Color = Drawing.Color.FromRGB(0,0,1);
			this.fields[3].Color = Drawing.Color.FromRGB(0.5,0.5,0.5);
			ToolTip.Default.SetToolTip(this.fields[0], "Rouge");
			ToolTip.Default.SetToolTip(this.fields[1], "Vert");
			ToolTip.Default.SetToolTip(this.fields[2], "Bleu");
			ToolTip.Default.SetToolTip(this.fields[3], "Alpha (transparence)");

			this.fields[4].Color = Drawing.Color.FromRGB(0,0,0);
			this.fields[4].BackColor = Drawing.Color.FromRGB(0.5,0.5,0.5);
			ToolTip.Default.SetToolTip(this.fields[4], "Teinte");

			this.fields[5].Color = Drawing.Color.FromRGB(0,0,0);
			this.fields[5].BackColor = Drawing.Color.FromRGB(1,1,1);
			ToolTip.Default.SetToolTip(this.fields[5], "Saturation");
			
			this.fields[6].Color = Drawing.Color.FromRGB(1,1,1);
			this.fields[6].BackColor = Drawing.Color.FromRGB(0,0,0);
			ToolTip.Default.SetToolTip(this.fields[6], "Luminosité");

			this.labels[0].Text = "R";
			this.labels[1].Text = "V";
			this.labels[2].Text = "B";
			this.labels[3].Text = "A";
			this.labels[4].Text = "T";
			this.labels[5].Text = "S";
			this.labels[6].Text = "L";

			this.circle = new ColorWheel(this);
			this.circle.Changed += new Support.EventHandler(this.HandleCircleChanged);

			this.palette = new ColorPalette(this);
			this.palette.HasOptionButton = true;
			this.palette.Export += new Support.EventHandler(this.HandlePaletteExport);
			this.palette.Import += new Support.EventHandler(this.HandlePaletteImport);

			this.picker = new Tools.Magnifier.DragSource(this);
			this.picker.HotColorChanged += new Support.EventHandler(this.HandlePickerHotColorChanged);
			ToolTip.Default.SetToolTip(this.picker, "Pipette-loupe");

			this.buttonClose = new GlyphButton(this);
			this.buttonClose.GlyphShape = GlyphShape.Close;
			this.buttonClose.ButtonStyle = ButtonStyle.Normal;
			this.buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
			ToolTip.Default.SetToolTip(this.buttonClose, "Fermer ce panneau");
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

		public Drawing.Color					Color
		{
			get
			{
				return this.circle.Color;
			}

			set
			{
				if ( this.Color != value )
				{
					System.Diagnostics.Debug.Assert(this.suspendColorEvents == false);
					
					this.suspendColorEvents = true;
					this.circle.Color = value;
					this.UpdateColors();
					this.suspendColorEvents = false;
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

		
		protected void UpdateColors()
		{
			this.ColorToFieldsRGB();
			this.ColorToFieldsHSV();
			this.OnChanged();
			this.Invalidate();
		}

		// Couleur -> textes éditables.
		protected void ColorToFieldsRGB()
		{
			double a,r,g,b;
			this.Color.GetARGB(out a, out r, out g, out b);
			
			this.fields[0].Value = (decimal) System.Math.Floor(r*255+0.5);
			this.fields[1].Value = (decimal) System.Math.Floor(g*255+0.5);
			this.fields[2].Value = (decimal) System.Math.Floor(b*255+0.5);
			this.fields[3].Value = (decimal) System.Math.Floor(a*255+0.5);
		}

		// Couleur -> textes éditables.
		protected void ColorToFieldsHSV()
		{
			double h,s,v;
			this.circle.GetHSV(out h, out s, out v);
			
			this.fields[4].Value = (decimal) System.Math.Floor(h);
			this.fields[5].Value = (decimal) System.Math.Floor(s*100+0.5);
			this.fields[6].Value = (decimal) System.Math.Floor(v*100+0.5);
			
			this.ColoriseSliders();
		}

		// Textes éditables RGB -> couleur.
		protected void FieldsRGBToColor()
		{
			double r = (double) this.fields[0].Value/255;
			double g = (double) this.fields[1].Value/255;
			double b = (double) this.fields[2].Value/255;
			double a = (double) this.fields[3].Value/255;
			
			this.Color = Drawing.Color.FromARGB(a,r,g,b);
		}

		// Textes éditables HSV -> couleur.
		protected void FieldsHSVToColor()
		{
			double h = (double) this.fields[4].Value;
			double s = (double) this.fields[5].Value/100;
			double v = (double) this.fields[6].Value/100;
			
			System.Diagnostics.Debug.Assert(this.suspendColorEvents == false);
			
			this.suspendColorEvents = true;
			this.circle.SetHSV(h,s,v);
			this.UpdateColors();
			this.suspendColorEvents = false;
		}

		// Couleur -> sliders.
		protected void ColoriseSliders()
		{
			double h,s,v;
			this.circle.GetHSV(out h, out s, out v);
			
			Drawing.Color saturated = Drawing.Color.FromHSV(h,1,1);
			
			this.fields[4].Color = saturated;
			this.fields[5].Color = saturated;
			this.fields[6].Color = saturated;
		}


		// Met à jour la géométrie.
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

			r.Left   = rect.Left;
			r.Right  = rect.Left + hCircle;
			r.Bottom = rect.Top - hCircle;
			r.Top    = rect.Top;
			this.circle.Bounds = r;
			this.circle.SetVisible(visibleCircle);

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
				this.labels[i].SetVisible(visibleFields);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].SetVisible(visibleFields);

				r.Offset(0, -19);
			}

			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for ( int i=4 ; i<=6 ; i++ )  // t,s,i
			{
				r.Left  = 10+70;
				r.Width = 12;
				this.labels[i].Bounds = r;
				this.labels[i].SetVisible(visibleFields);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].SetVisible(visibleFields);

				r.Offset(0, -19);
			}
			
			r.Top    = rect.Bottom+3*19;
			r.Bottom = r.Top-20;
			for ( int i=3 ; i<=3 ; i++ )  // a
			{
				r.Left  = 10+70+70;
				r.Width = 12;
				this.labels[i].Bounds = r;
				this.labels[i].SetVisible(visibleFields);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].SetVisible(visibleFields);

				r.Offset(0, -19);
			}
			
			r.Top    = r.Top-2;
			r.Bottom = r.Bottom+1;
			r.Right = this.fields[3].Right;
			r.Left  = r.Right - r.Height;
			this.picker.Bounds = r;
			this.picker.SetVisible(visibleFields);

			if ( this.hasCloseButton )
			{
				r.Left = rect.Left;
				r.Width = 14;
				r.Bottom = rect.Top-14;
				r.Top = rect.Top;
				this.buttonClose.Bounds = r;
				this.buttonClose.SetVisible(true);
			}
			else
			{
				this.buttonClose.SetVisible(false);
			}
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
					if ( i < 4 )
					{
						this.fields[i].TextChanged -= new Support.EventHandler(this.HandleTextRGBChanged);
					}
					else if ( i == 4 )
					{
						this.fields[i].TextChanged -= new Support.EventHandler(this.HandleTextHSVChanged);
					}
					else
					{
						this.fields[i].TextChanged -= new Support.EventHandler(this.HandleTextHSVChanged);
					}
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
			this.Color = this.picker.HotColor;
		}
		
		// Une valeur RGB a été changée.
		private void HandleTextRGBChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.FieldsRGBToColor();
			}
		}

		// Une valeur HSV a été changée.
		private void HandleTextHSVChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.FieldsHSVToColor();
			}
		}

		// Couleur dans le cercle changée.
		private void HandleCircleChanged(object sender)
		{
			if ( !this.suspendColorEvents )
			{
				this.suspendColorEvents = true;
				this.UpdateColors();
				this.suspendColorEvents = false;
			}
		}

		// Couleur dans palette cliquée.
		private void HandlePaletteExport(object sender)
		{
			this.Color = this.palette.Color;
			this.OnChanged();
		}

		// Couleur dans palette cliquée.
		private void HandlePaletteImport(object sender)
		{
			this.palette.Color = this.Color;
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.OnCloseClicked();
		}

		
		// Génère un événement pour dire ça a changé.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
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
		
		
		public event Support.EventHandler		Changed;
		public event Support.EventHandler		CloseClicked;

		protected Drawing.Color					black;
		protected ColorWheel					circle;
		protected ColorPalette					palette;
		protected int							nbField;
		protected StaticText[]					labels;
		protected TextFieldSlider[]				fields;
		protected bool							suspendColorEvents = false;
		protected bool							hasCloseButton = false;
		protected GlyphButton					buttonClose;
		private Tools.Magnifier.DragSource		picker;
	}
}
