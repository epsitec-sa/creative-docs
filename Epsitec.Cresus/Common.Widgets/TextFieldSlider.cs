namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldSlider implémente la ligne éditable numérique avec slider.
	/// </summary>
	public class TextFieldSlider : TextFieldUpDown
	{
		public TextFieldSlider()
		{
			this.slider = new Slider (this);
			this.slider.HasFrame = false;
			this.slider.ValueChanged += this.HandleSliderValueChanged;

			this.UpdateSliderRange ();
		}

		public TextFieldSlider(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public Drawing.Color Color
		{
			//	Couleur du slider.
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
			//	Toute modification de la propriété BackColor doit être répercutée
			//	sur le slider. Le plus simple est d'utiliser un override callback
			//	sur la propriété BackColor :

			Types.DependencyPropertyMetadata metadata = Visual.BackColorProperty.DefaultMetadata.Clone ();
			
			metadata.DefineDefaultValue (Drawing.Color.Empty);
			metadata.SetValueOverride = new Epsitec.Common.Types.SetValueOverrideCallback (TextFieldSlider.SetBackColorValue);

			Visual.BackColorProperty.OverrideMetadata (typeof (TextFieldSlider), metadata);
		}


		private static void SetBackColorValue(Types.DependencyObject o, object value)
		{
			TextFieldSlider that = o as TextFieldSlider;
			that.slider.BackColor = (Drawing.Color) value;
			that.SetValueBase (Visual.BackColorProperty, value);
		}

		public Types.DecimalRange PreferredRange
		{
			get
			{
				return this.preferredRange;
			}
			set
			{
				if (this.preferredRange != value)
				{
					this.preferredRange = value;

					if (!value.IsEmpty)
					{
						this.Step = value.Resolution;
					}

					this.UpdateSliderRange ();
				}
			}
		}

		public decimal LogarithmicDivisor
		{
			get
			{
				return this.slider.LogarithmicDivisor;
			}

			set
			{
				this.slider.LogarithmicDivisor = value;
			}
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.slider != null)
				{
					this.slider.ValueChanged -= this.HandleSliderValueChanged;
					this.slider.Dispose ();
				}
				this.slider = null;
			}

			base.Dispose (disposing);
		}

		protected override void UpdateGeometry()
		{
			base.UpdateGeometry ();

			if (this.arrowUp == null)
				return;

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Width -= System.Math.Floor (rect.Height*adorner.GeometryUpDownWidthFactor)-1;
			rect.Height = TextFieldSlider.SliderHeight;
			this.slider.SetManualBounds (rect);
		}

		protected override void OnValueChanged()
		{
			//	Valeur numérique éditée.
			base.OnValueChanged ();

			if (this.Text != "")
			{
				this.ignoreSliderChanges++;
				this.slider.Value = this.Value;
				this.ignoreSliderChanges--;
			}
		}

		protected override void InitializeMargins()
		{
			base.InitializeMargins ();
			this.margins.Bottom = TextFieldSlider.SliderHeight-AbstractTextField.FrameMargin;
		}


		private void HandleSliderValueChanged(object sender)
		{
			if (this.ignoreSliderChanges == 0)
			{
				this.SetValue (this.slider.Value);
			}
		}

		protected override void OnRangeChanged()
		{
			base.OnRangeChanged ();
			this.UpdateSliderRange ();
		}


		protected virtual void UpdateSliderRange()
		{
			if (this.PreferredRange.IsEmpty)
			{
				this.slider.MinValue   = this.MinValue;
				this.slider.MaxValue   = this.MaxValue;
				this.slider.Resolution = this.Resolution;
			}
			else
			{
				this.slider.MinValue   = this.PreferredRange.Minimum;
				this.slider.MaxValue   = this.PreferredRange.Maximum;
				this.slider.Resolution = this.Resolution;
			}
		}

		private const double SliderHeight = 5;

		private int ignoreSliderChanges;
		protected Slider slider;
		protected Types.DecimalRange preferredRange;
	}
}
