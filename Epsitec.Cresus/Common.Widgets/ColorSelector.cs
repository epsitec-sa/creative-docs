namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe ColorSelector permet de choisir une couleur rgb.
	/// </summary>
	public class ColorSelector : Widget
	{
		public ColorSelector()
		{
			this.colorBlack = Drawing.Color.FromName("WindowFrame");

			this.nbField = 4+3;
			this.labels = new StaticText[this.nbField];
			this.fields = new TextFieldSlider[this.nbField];
			for ( int i=0 ; i<this.nbField ; i++ )
			{
				this.labels[i] = new StaticText(this);
				this.fields[i] = new TextFieldSlider(this);

				this.fields[i].Value = 0;
				if ( i < 4 )
				{
					this.fields[i].MinRange = 0;
					this.fields[i].MaxRange = 255;
					this.fields[i].Step = 10;
					this.fields[i].TextChanged += new Support.EventHandler(this.HandleTextRGBChanged);
				}
				else if ( i == 4 )
				{
					this.fields[i].MinRange = 0;
					this.fields[i].MaxRange = 360;
					this.fields[i].Step = 15;
					this.fields[i].TextChanged += new Support.EventHandler(this.HandleTextHSVChanged);
				}
				else
				{
					this.fields[i].MinRange = 0;
					this.fields[i].MaxRange = 100;
					this.fields[i].Step = 5;
					this.fields[i].TextChanged += new Support.EventHandler(this.HandleTextHSVChanged);
				}
			}
			this.fields[0].Color = Drawing.Color.FromRGB(1,0,0);
			this.fields[1].Color = Drawing.Color.FromRGB(0,1,0);
			this.fields[2].Color = Drawing.Color.FromRGB(0,0,1);
			this.fields[3].Color = Drawing.Color.FromRGB(0.5,0.5,0.5);

			this.fields[4].Color = Drawing.Color.FromRGB(0,0,0);
			this.fields[4].BackColor = Drawing.Color.FromRGB(0.5,0.5,0.5);

			this.fields[5].Color = Drawing.Color.FromRGB(0,0,0);
			this.fields[5].BackColor = Drawing.Color.FromRGB(1,1,1);
			
			this.fields[6].Color = Drawing.Color.FromRGB(1,1,1);
			this.fields[6].BackColor = Drawing.Color.FromRGB(0,0,0);

			this.labels[0].Text = "R";
			this.labels[1].Text = "V";
			this.labels[2].Text = "B";
			this.labels[3].Text = "A";
			this.labels[4].Text = "T";
			this.labels[5].Text = "S";
			this.labels[6].Text = "L";

			this.circle = new ColorWheel(this);
			this.circle.Changed += new Support.EventHandler(this.HandleCircleChanged);

			this.nbPalette = 16;
			this.palette = new ColorSample[this.nbPalette];
			for ( int i=0 ; i<this.nbPalette ; i++ )
			{
				this.palette[i] = new ColorSample(this);
				this.palette[i].Clicked += new MessageEventHandler(this.ColorSelectorClicked);
			}

			this.palette[ 0].Color = Drawing.Color.FromARGB(0.0, 1.0, 1.0, 1.0);
			this.palette[ 1].Color = Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0);
			this.palette[ 2].Color = Drawing.Color.FromARGB(1.0, 1.0, 1.0, 0.0);
			this.palette[ 3].Color = Drawing.Color.FromARGB(1.0, 0.0, 1.0, 0.0);
			this.palette[ 4].Color = Drawing.Color.FromARGB(1.0, 0.0, 1.0, 1.0);
			this.palette[ 5].Color = Drawing.Color.FromARGB(1.0, 0.0, 0.0, 1.0);
			this.palette[ 6].Color = Drawing.Color.FromARGB(1.0, 1.0, 0.0, 1.0);
			this.palette[ 7].Color = Drawing.Color.FromARGB(0.5, 0.5, 0.5, 0.5);

			this.palette[ 8].Color = Drawing.Color.FromARGB(1.0, 1.0, 1.0, 1.0);
			this.palette[ 9].Color = Drawing.Color.FromARGB(1.0, 0.9, 0.9, 0.9);
			this.palette[10].Color = Drawing.Color.FromARGB(1.0, 0.8, 0.8, 0.8);
			this.palette[11].Color = Drawing.Color.FromARGB(1.0, 0.7, 0.7, 0.7);
			this.palette[12].Color = Drawing.Color.FromARGB(1.0, 0.6, 0.6, 0.6);
			this.palette[13].Color = Drawing.Color.FromARGB(1.0, 0.5, 0.5, 0.5);
			this.palette[14].Color = Drawing.Color.FromARGB(1.0, 0.4, 0.4, 0.4);
			this.palette[15].Color = Drawing.Color.FromARGB(1.0, 0.0, 0.0, 0.0);
		}
		
		public ColorSelector(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
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

				foreach ( ColorSample cs in this.palette )
				{
					cs.Clicked -= new MessageEventHandler(this.ColorSelectorClicked);
				}
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return 240;
			}
		}

		// Couleur.
		public Drawing.Color Color
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
					this.UpdateColors ();
					this.suspendColorEvents = false;
				}
			}
		}
		
		protected void UpdateColors()
		{
			this.ColorToFieldsRGB ();
			this.ColorToFieldsHSV ();
			this.OnChanged ();
			this.Invalidate ();
		}

		// Couleur -> textes éditables.
		protected void ColorToFieldsRGB()
		{
			double a,r,g,b;
			this.Color.GetARGB(out a, out r, out g, out b);
			
			this.fields[0].Value = System.Math.Floor(r*255+0.5);
			this.fields[1].Value = System.Math.Floor(g*255+0.5);
			this.fields[2].Value = System.Math.Floor(b*255+0.5);
			this.fields[3].Value = System.Math.Floor(a*255+0.5);
		}

		// Couleur -> textes éditables.
		protected void ColorToFieldsHSV()
		{
			double h,s,v;
			this.circle.GetHSV(out h, out s, out v);
			
			this.fields[4].Value = System.Math.Floor(h);
			this.fields[5].Value = System.Math.Floor(s*100+0.5);
			this.fields[6].Value = System.Math.Floor(v*100+0.5);
			
			this.ColoriseSliders();
		}

		// Textes éditables RGB -> couleur.
		protected void FieldsRGBToColor()
		{
			double r = this.fields[0].Value/255;
			double g = this.fields[1].Value/255;
			double b = this.fields[2].Value/255;
			double a = this.fields[3].Value/255;
			
			this.Color = Drawing.Color.FromARGB(a,r,g,b);
		}

		// Textes éditables HSV -> couleur.
		protected void FieldsHSVToColor()
		{
			double h = this.fields[4].Value;
			double s = this.fields[5].Value/100;
			double v = this.fields[6].Value/100;
			
			System.Diagnostics.Debug.Assert(this.suspendColorEvents == false);
			
			this.suspendColorEvents = true;
			this.circle.SetHSV (h,s,v);
			this.UpdateColors ();
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
			rect.Inflate(-5, -5);
			double hCircle = rect.Height-5-20*4;
			hCircle = System.Math.Min(hCircle, rect.Width);
			Drawing.Rectangle r = new Drawing.Rectangle();

			bool visibleCircle = ( rect.Height > 160 );
			bool visibleFields = ( rect.Height >  80 );

			r.Left   = rect.Left;
			r.Right  = rect.Left + hCircle;
			r.Bottom = rect.Top - hCircle;
			r.Top    = rect.Top;
			this.circle.Bounds = r;
			this.circle.SetVisible(visibleCircle);

			double dx = System.Math.Floor((rect.Width-hCircle-10)/2);
			if ( dx > 4 )
			{
				double dy = System.Math.Floor(hCircle/(this.nbPalette/2));
				dx = System.Math.Min(dx, dy);
				Drawing.Point pos = new Drawing.Point();
				pos.X = rect.Right-dx*2;
				int i = 0;
				for ( int x=0 ; x<2 ; x++ )
				{
					pos.Y = rect.Top;
					for ( int y=0 ; y<this.nbPalette/2 ; y++ )
					{
						r.Left   = pos.X;
						r.Right  = pos.X+dx+1;
						r.Top    = pos.Y;
						r.Bottom = pos.Y-dy-1;
						this.palette[i].Bounds = r;
						this.palette[i].SetVisible(visibleCircle);
						i ++;
						pos.Y -= dy;
					}
					pos.X += dx;
				}
			}
			else
			{
				for ( int i=0 ; i<this.nbPalette ; i++ )
				{
					this.palette[i].Hide();
				}
			}

			r.Top    = rect.Bottom+4*19;
			r.Bottom = r.Top-20;
			for ( int i=0 ; i<4 ; i++ )
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

			r.Top    = rect.Bottom+4*19;
			r.Bottom = r.Top-20;
			for ( int i=4 ; i<7 ; i++ )
			{
				r.Left  = 80;
				r.Width = 12;
				this.labels[i].Bounds = r;
				this.labels[i].SetVisible(visibleFields);

				r.Left  = r.Right;
				r.Width = 50;
				this.fields[i].Bounds = r;
				this.fields[i].SetVisible(visibleFields);

				r.Offset(0, -19);
			}
		}
		
		// Une valeur RGB a été changée.
		private void HandleTextRGBChanged(object sender)
		{
			if ( ! this.suspendColorEvents )
			{
				this.FieldsRGBToColor();
			}
		}

		// Une valeur HSV a été changée.
		private void HandleTextHSVChanged(object sender)
		{
			if ( ! this.suspendColorEvents )
			{
				this.FieldsHSVToColor();
			}
		}

		// Couleur dans le cercle changée.
		private void HandleCircleChanged(object sender)
		{
			if ( ! this.suspendColorEvents )
			{
				this.suspendColorEvents = true;
				this.UpdateColors();
				this.suspendColorEvents = false;
			}
		}

		// Couleur dans palette cliquée.
		private void ColorSelectorClicked(object sender, MessageEventArgs e)
		{
			ColorSample cs = sender as ColorSample;
			if ( e.Message.IsShiftPressed || e.Message.IsCtrlPressed )
			{
				cs.Color = this.Color;
			}
			else
			{
				this.Color = cs.Color;
				this.OnChanged();
			}
		}


		// Génère un événement pour dire ça a changé.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}

		public event Support.EventHandler	Changed;
		

		protected Drawing.Color				colorBlack;
		protected ColorWheel				circle;
		protected int						nbPalette;
		protected ColorSample[]				palette;
		protected int						nbField;
		protected StaticText[]				labels;
		protected TextFieldSlider[]			fields;
		protected bool						suspendColorEvents = false;
	}
}
