using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldUpDown implémente la ligne éditable numérique.
	/// </summary>
	public class TextFieldUpDown : AbstractTextField, Support.Data.INumValue
	{
		public TextFieldUpDown()
			: base (TextFieldStyle.UpDown)
		{
			this.TextNavigator.IsNumeric = true;
			this.range = new Types.DecimalRange (0, 100, 1);

			this.arrowUp = new GlyphButton (this);
			this.arrowDown = new GlyphButton (this);
			this.arrowUp.Name = "Up";
			this.arrowDown.Name = "Down";
			this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
			this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
			this.arrowUp.ButtonStyle = ButtonStyle.UpDown;
			this.arrowDown.ButtonStyle = ButtonStyle.UpDown;
			this.arrowUp.Engaged += this.HandleButton;
			this.arrowDown.Engaged += this.HandleButton;
			this.arrowUp.StillEngaged += this.HandleButton;
			this.arrowDown.StillEngaged += this.HandleButton;
			this.arrowUp.AutoRepeat = true;
			this.arrowDown.AutoRepeat = true;

			this.UpdateValidator ();
		}

		public TextFieldUpDown(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		#region INumValue Members
		public virtual decimal Value
		{
			get
			{
				string text  = TextLayout.ConvertToSimpleText (this.Text);
				decimal value = this.DefaultValue;

				if (text != "")
				{
					string dec = Types.InvariantConverter.ExtractDecimal (ref text);

					try
					{
						value = decimal.Parse (dec, System.Globalization.CultureInfo.CurrentUICulture);
					}
					catch
					{
					}
				}

				return value;
			}
			set
			{
				if (this.Value == value && this.Text != "")
				{
					return;
				}

				value = this.range.Constrain (value);

				if (this.Text == "" || this.Value != value)
				{
					this.Text = this.range.ConvertToString (value);
					this.SelectAll ();
				}
			}
		}

		public virtual decimal MinValue
		{
			get
			{
				return this.range.Minimum;
			}
			set
			{
				if (this.range.Minimum != value)
				{
					decimal min = value;
					decimal max = this.range.Maximum;
					decimal res = this.range.Resolution;

					this.range = new Types.DecimalRange (min, max, res);
					this.OnRangeChanged ();
				}
			}
		}

		public virtual decimal MaxValue
		{
			get
			{
				return this.range.Maximum;
			}
			set
			{
				if (this.range.Maximum != value)
				{
					decimal min = this.range.Minimum;
					decimal max = value;
					decimal res = this.range.Resolution;

					this.range = new Types.DecimalRange (min, max, res);
					this.OnRangeChanged ();
				}
			}
		}

		public virtual decimal Resolution
		{
			get
			{
				return this.range.Resolution;
			}
			set
			{
				if (this.range.Resolution != value)
				{
					decimal min = this.range.Minimum;
					decimal max = this.range.Maximum;
					decimal res = value;

					this.range = new Types.DecimalRange (min, max, res);
					this.OnRangeChanged ();

					if (this.Text != "")
					{
						this.Text = this.range.Constrain (this.Value).ToString ();
						this.SelectAll ();
					}
				}
			}
		}

		public virtual decimal Range
		{
			get
			{
				return this.MaxValue-this.MinValue;
			}
		}

		public event EventHandler ValueChanged
		{
			add
			{
				this.AddUserEventHandler ("ValueChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("ValueChanged", value);
			}
		}
		#endregion

		public virtual bool IsValueInRange
		{
			get
			{
				return this.range.CheckInRange (this.Value);
			}
		}

		public virtual bool IsDefaultValueDefined
		{
			get
			{
				return this.isDefaultValueDefined;
			}
		}

		public virtual decimal DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				if (this.defaultValue != value || this.isDefaultValueDefined == false)
				{
					this.defaultValue = value;
					this.isDefaultValueDefined = true;
					this.UpdateValidator ();

					if (this.Validator != null)
					{
						this.Validator.MakeDirty (true);
					}
				}
			}
		}

		public virtual decimal Step
		{
			get
			{
				return this.step;
			}
			set
			{
				this.step = value;
			}
		}

		public virtual string TextSuffix
		{
			get
			{
				return this.textSuffix;
			}
			set
			{
				if (value == "")
				{
					value = null;
				}

				if (this.textSuffix != value)
				{
					this.textSuffix = value;
					this.OnTextSuffixChanged ();
				}
			}
		}

		protected override TextLayout GetTextLayout()
		{
			if (string.IsNullOrEmpty (this.textSuffix) || this.IsTextEmpty)
			{
				return base.GetTextLayout ();
			}

			TextLayout layout = new TextLayout (base.GetTextLayout ());
			layout.Text = string.Concat (this.Text, this.textSuffix);
			return layout;
		}

		protected override bool CanStartEdition
		{
			get
			{
				return true;
			}
		}


		public void ClearDefaultValue()
		{
			if (this.isDefaultValueDefined)
			{
				this.isDefaultValueDefined = false;
				this.UpdateValidator ();

				if (this.Validator != null)
				{
					this.Validator.MakeDirty (true);
				}
			}
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.arrowUp != null)
				{
					this.arrowUp.Engaged -= this.HandleButton;
					this.arrowUp.StillEngaged -= this.HandleButton;
					this.arrowUp.Dispose ();
				}
				if (this.arrowDown != null)
				{
					this.arrowDown.Engaged -= this.HandleButton;
					this.arrowDown.StillEngaged -= this.HandleButton;
					this.arrowDown.Dispose ();
				}

				this.arrowUp = null;
				this.arrowDown = null;
			}

			base.Dispose (disposing);
		}

		protected override void InitializeMargins()
		{
			base.InitializeMargins ();

			if (this.IsActualGeometryValid)
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				Drawing.Rectangle rect = this.ActualBounds;
				double width = System.Math.Floor (rect.Height*adorner.GeometryUpDownWidthFactor);
				this.margins.Right = width - AbstractTextField.FrameMargin;
			}
		}

		public override Drawing.Margins GetInternalPadding()
		{
			return base.GetInternalPadding ();
		}

		protected override void UpdateGeometry()
		{
			base.UpdateGeometry ();

			this.InitializeMargins ();

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.ActualBounds;

			double width = this.margins.Right + AbstractTextField.FrameMargin;

			if (this.arrowUp   != null &&
				this.arrowDown != null)
			{
				Drawing.Rectangle aRect = new Drawing.Rectangle ();

				aRect.Left  = rect.Width-width;
				aRect.Width = width-adorner.GeometryUpDownRightMargin;

				double h = System.Math.Ceiling ((rect.Height-adorner.GeometryUpDownBottomMargin-adorner.GeometryUpDownTopMargin)/2);

				aRect.Bottom = adorner.GeometryUpDownBottomMargin;
				aRect.Height = h;
				this.arrowDown.SetManualBounds (aRect);

				aRect.Bottom = rect.Height-adorner.GeometryUpDownTopMargin-h;
				aRect.Height = h;
				this.arrowUp.SetManualBounds (aRect);
			}
		}

		protected override void UpdateTextLayout()
		{
			if (base.TextLayout != null)
			{
				this.realSize = this.InnerTextBounds.Size;

				base.TextLayout.Alignment  = this.ContentAlignment;
				base.TextLayout.LayoutSize = this.GetTextLayoutSize ();

				if (this.TextLayout.Text != null)
				{
					this.CursorScroll (true);
				}
			}
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseWheel:
					if (message.Wheel > 0)
						this.IncrementValue (1);
					if (message.Wheel < 0)
						this.IncrementValue (-1);
					message.Consumer = this;
					return;
			}

			base.ProcessMessage (message, pos);
		}

		protected override bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			switch (message.KeyCode)
			{
				case KeyCode.ArrowUp:
					this.IncrementValue (1);
					break;

				case KeyCode.ArrowDown:
					this.IncrementValue (-1);
					break;

				default:
					return base.ProcessKeyDown (message, pos);
			}

			return true;
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateGeometry ();
			base.OnAdornerChanged ();
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();

			if (this.IsValid)
			{
				this.SetError (false);
				this.OnValueChanged ();
			}
			else
			{
				this.SetError (true);
			}
		}


		protected virtual void OnValueChanged()
		{
			var handler = this.GetUserEventHandler ("ValueChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnRangeChanged()
		{
			this.UpdateValidator ();

			var handler = this.GetUserEventHandler ("RangeChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnTextSuffixChanged()
		{
			var handler = this.GetUserEventHandler ("TextSuffixChanged");
			if (handler != null)
			{
				handler (this);
			}
		}


		protected virtual void IncrementValue(decimal delta)
		{
			Types.DecimalRange range = new Types.DecimalRange (this.MinValue, this.MaxValue, this.Step);

			decimal orgValue   = this.Value;
			decimal roundValue = range.ConstrainToZero (orgValue);

			if (orgValue == roundValue)
			{
				//	La valeur d'origine était déjà parfaitement alignée sur une frontière (step),
				//	on peut donc simplement passer au pas suivant :
				roundValue += delta * this.Step;
			}
			else
			{
				//	L'arrondi vers zéro suffit dans les cas suivants :
				//	o  13 =>  10,  13 - 10 =>  10   orgValue > 0, delta < 0
				//	o -13 => -10, -13 + 10 => -10   orgValue < 0, delta > 0
				//	en supposant un pas de 10.

				if ((orgValue < 0 && delta > 0) ||
					 (orgValue > 0 && delta < 0))
				{
					//	La valeur arrondie fait l'affaire.
				}
				else
				{
					//	Il faut encore ajouter l'incrément.
					roundValue += delta * this.Step;
				}
			}

			this.SetValue (roundValue);
		}

		protected void SetValue(decimal value)
		{
			//	Modifie une valeur en envoyant l'événement AcceptEdition si nécessaire.
			if (this.Value != value)
			{
				this.StartEdition ();
				this.Value = value;
				this.AcceptEdition ();
			}
		}

		protected virtual void UpdateValidator()
		{
			if (this.validator1 != null)
			{
				this.validator1.Dispose ();
			}

			this.validator1 = new Validators.RegexValidator (this, RegexFactory.LocalizedDecimalNum, this.IsDefaultValueDefined);

			if (this.validator2 != null)
			{
				this.validator2.Dispose ();
			}

			this.validator2 = new Validators.NumRangeValidator (this);
		}


		private void HandleButton(object sender)
		{
			if (sender == this.arrowUp)
			{
				this.IncrementValue (1);
			}
			else if (sender == this.arrowDown)
			{
				this.IncrementValue (-1);
			}
		}



		public event EventHandler RangeChanged
		{
			add
			{
				this.AddUserEventHandler ("RangeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("RangeChanged", value);
			}
		}

		public event EventHandler TextSuffixChanged
		{
			add
			{
				this.AddUserEventHandler ("TextSuffixChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("TextSuffixChanged", value);
			}
		}

		protected Types.DecimalRange range;
		protected string textSuffix;
		protected GlyphButton arrowUp;
		protected GlyphButton arrowDown;
		protected decimal defaultValue = 0;
		protected bool isDefaultValueDefined = false;
		protected decimal step = 1;
		protected Validators.RegexValidator validator1;
		protected Validators.NumRangeValidator validator2;
	}
}
