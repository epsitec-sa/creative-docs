namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldSlider impl�mente la ligne �ditable num�rique avec slider.
	/// </summary>
	public class TextFieldSlider : TextFieldUpDown
	{
		public TextFieldSlider()
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	mani�re, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.slider = new Slider(this);
			this.slider.HasFrame = false;
			this.slider.ValueChanged += new Support.EventHandler(this.HandleSliderValueChanged);
			
			this.range.Changed += new Support.EventHandler(this.HandleRangeChanged);
			this.UpdateSliderRange();
		}
		
		public TextFieldSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Drawing.Color				Color
		{
			// Couleur du slider.
			get
			{
				return this.slider.Color;
			}

			set
			{
				this.slider.Color = value;
			}
		}
		
		static TextFieldSlider()
		{
			//	Toute modification de la propri�t� BackColor doit �tre r�percut�e
			//	sur le slider. Le plus simple est d'utiliser un override callback
			//	sur la propri�t� BackColor :
			
			Helpers.VisualPropertyMetadata metadata = new Helpers.VisualPropertyMetadata (Drawing.Color.Empty, Helpers.VisualPropertyFlags.AffectsDisplay);
			
			metadata.SetValueOverride = new Epsitec.Common.Types.SetValueOverrideCallback (TextFieldSlider.SetBackColorValue);
			
			Visual.BackColorProperty.OverrideMetadata (typeof (TextFieldSlider), metadata);
		}
		
		
		private static void SetBackColorValue(Types.Object o, object value)
		{
			TextFieldSlider that = o as TextFieldSlider;
			that.slider.BackColor = (Drawing.Color) value;
			that.SetValueBase (Visual.BackColorProperty, value);
		}

		
		public decimal						Logarithmic
		{
			get
			{
				return this.slider.Logarithmic;
			}

			set
			{
				this.slider.Logarithmic = value;
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.slider != null )
				{
					this.slider.ValueChanged -= new Support.EventHandler(this.HandleSliderValueChanged);
					this.slider.Dispose();
				}
				this.slider = null;
			}
			
			base.Dispose(disposing);
		}
		
		protected override void UpdateClientGeometry()
		{
			this.margins.Bottom = TextFieldSlider.sliderHeight-AbstractTextField.FrameMargin;
			
			base.UpdateClientGeometry();

			if ( this.arrowUp == null )  return;

			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Width -= System.Math.Floor(rect.Height*adorner.GeometryUpDownWidthFactor)-1;
			rect.Height = TextFieldSlider.sliderHeight;
			this.slider.Bounds = rect;
		}

		protected override void OnValueChanged()
		{
			// Valeur num�rique �dit�e.
			base.OnValueChanged ();
			
			if (this.Text != "")
			{
				this.slider.Value = this.Value;
			}
		}

		
		private void HandleSliderValueChanged(object sender)
		{
			this.Value = this.slider.Value;
		}
		
		private void HandleRangeChanged(object sender)
		{
			this.UpdateSliderRange();
		}
		
		
		protected virtual void UpdateSliderRange()
		{
			this.slider.MinValue   = this.MinValue;
			this.slider.MaxValue   = this.MaxValue;
			this.slider.Resolution = this.Resolution;
		}

		
		protected Slider					slider;
		protected static readonly double	sliderHeight = 5;
	}
}
