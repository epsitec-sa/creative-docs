using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractSlider impl�mente la classe de base des potentiom�tres lin�aires
	/// HSlider et VSlider.
	/// </summary>
	public abstract class AbstractSlider : Widget, Behaviors.IDragBehaviorHost, Support.Data.INumValue
	{
		protected AbstractSlider(bool vertical, bool hasButtons)
		{
			this.isVertical   = vertical;
			this.dragBehavior = this.CreateDragBehavior();
			
			this.AutoEngage = true;
			this.AutoRepeat = true;
			
			this.InternalState |= InternalState.Engageable;

			if (hasButtons)
			{
				this.arrowUp = new GlyphButton(this);
				this.arrowUp.GlyphShape = GlyphShape.Plus;
				this.arrowUp.ButtonStyle = ButtonStyle.Slider;
				this.arrowUp.Engaged += new EventHandler(this.HandleButton);
				this.arrowUp.StillEngaged += new EventHandler(this.HandleButton);
				this.arrowUp.AutoRepeat = true;

				this.arrowDown = new GlyphButton(this);
				this.arrowDown.GlyphShape = GlyphShape.Minus;
				this.arrowDown.ButtonStyle = ButtonStyle.Slider;
				this.arrowDown.Engaged += new EventHandler(this.HandleButton);
				this.arrowDown.StillEngaged += new EventHandler(this.HandleButton);
				this.arrowDown.AutoRepeat = true;

				this.arrowMax = new GlyphButton(this);
				this.arrowMax.ButtonStyle = ButtonStyle.Icon;
				this.arrowMax.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				this.arrowMin = new GlyphButton(this);
				this.arrowMin.ButtonStyle = ButtonStyle.Icon;
				this.arrowMin.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			}
		}

		protected AbstractSlider(Widget embedder, bool vertical, bool hasButtons)
			: this (vertical, hasButtons)
		{
			this.SetEmbedder(embedder);
		}

		protected virtual Behaviors.DragBehavior CreateDragBehavior()
		{
			return new Behaviors.DragBehavior(this, true, false);
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if (this.arrowUp != null)
				{
					this.arrowUp.Engaged -= new EventHandler(this.HandleButton);
					this.arrowUp.StillEngaged -= new EventHandler(this.HandleButton);
				}

				if (this.arrowDown != null)
				{
					this.arrowDown.Engaged -= new EventHandler(this.HandleButton);
					this.arrowDown.StillEngaged -= new EventHandler(this.HandleButton);
				}

				if (this.arrowMax != null)
				{
					this.arrowMax.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				}

				if (this.arrowMin != null)
				{
					this.arrowMin.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				}
			}
			
			base.Dispose(disposing);
		}


		public bool							IsInverted
		{
			//	Inversion du fonctionnement.
			//	Ascenseur vertical:   false -> z�ro en bas
			//	Ascenseur vertical:   true  -> z�ro en haut
			//	Ascenseur horizontal: false -> z�ro � gauche
			//	Ascenseur horizontal: true  -> z�ro � droite
			
			get
			{
				return this.isInverted;
			}

			set
			{
				if (this.isInverted != value)
				{
					this.isInverted = value;
					this.Invalidate();
				}
			}
		}

		public bool							IsMinMaxButtons
		{
			//	Boutons min/max aux extr�mit�s du slider.
			get
			{
				return this.isMinMaxButtons;
			}
			set
			{
				if (this.isMinMaxButtons != value)
				{
					this.isMinMaxButtons = value;
					this.Invalidate();
				}
			}
		}
		
		
		public decimal						SmallChange
		{
			//	Valeur avanc�e par les boutons.
			get
			{
				return this.buttonStep;
			}

			set
			{
				this.buttonStep = value;
			}
		}
		
		public decimal						LargeChange
		{
			//	Valeur avanc�e en cliquant hors de la cabine.
			get
			{
				return this.pageStep;
			}

			set
			{
				this.pageStep = value;
			}
		}
		
		
		public double						DoubleValue
		{
			get { return (double) this.Value; }
		}
		
		public double						DoubleRange
		{
			get { return (double) this.Range; }
		}
		
		protected Zone						HiliteZone
		{
			get
			{
				return this.hiliteZone;
			}
			set
			{
				if (this.hiliteZone != value)
				{
					this.hiliteZone = value;
					this.UpdateInternalGeometry();
					this.Invalidate();
				}
			}
		}
		
		protected GlyphButton				ArrowUp
		{
			get { return this.arrowUp; }
		}
		
		protected GlyphButton				ArrowDown
		{
			get { return this.arrowDown; }
		}

		protected GlyphButton				ArrowMax
		{
			get { return this.arrowMax; }
		}

		protected GlyphButton				ArrowMin
		{
			get { return this.arrowMin; }
		}


		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry();
		}
		
		protected virtual void UpdateGeometry()
		{
			Rectangle rect = this.Client.Bounds;

			if (this.arrowDown == null)
			{
				this.sliderRect = rect;
//				this.UpdateInternalGeometry();
				return;
			}

			double totalLength = this.isVertical ? rect.Height : rect.Width;
			double arrowLength = this.isVertical ? rect.Width : rect.Height;

			if (arrowLength*2 > totalLength-AbstractSlider.minimalThumb)
			{
				//	Les boutons occupent trop de place. Il faut donc les comprimer.
				arrowLength = System.Math.Floor((totalLength-AbstractSlider.minimalThumb)/2);

				if (arrowLength < AbstractSlider.minimalArrow)
				{
					//	S'il n'y a plus assez de place pour afficher un bouton visible,
					//	autant les cacher compl�tement !
					arrowLength = 0;
				}
			}

			double arrowLength1 = this.isMinMaxButtons ? arrowLength : 0;
			double arrowLength2 = this.isMinMaxButtons ? arrowLength*2 : arrowLength;

			this.arrowMax.Visibility = this.isMinMaxButtons;
			this.arrowMin.Visibility = this.isMinMaxButtons;

			if (this.isVertical)
			{
				Rectangle bounds;

				if (this.isMinMaxButtons)
				{
					bounds = new Rectangle(0, rect.Height-arrowLength+2, rect.Width, arrowLength-2);
					this.arrowMax.Visibility = (arrowLength > 0);
					this.arrowMax.SetManualBounds(bounds);

					bounds = new Rectangle(0, 0, rect.Width, arrowLength-2);
					this.arrowMin.Visibility = (arrowLength > 0);
					this.arrowMin.SetManualBounds(bounds);
				}

				bounds = new Rectangle(0, rect.Height-arrowLength2, rect.Width, arrowLength);
				this.arrowUp.Visibility = (arrowLength > 0);
				this.arrowUp.SetManualBounds(bounds);

				bounds = new Rectangle(0, arrowLength1, rect.Width, arrowLength);
				this.arrowDown.Visibility = (arrowLength > 0);
				this.arrowDown.SetManualBounds(bounds);

				rect.Bottom += arrowLength;
				rect.Top    -= arrowLength;
			}
			else
			{
				Rectangle bounds;

				if (this.isMinMaxButtons)
				{
					bounds = new Rectangle(rect.Width-arrowLength+2, 0, arrowLength-2, rect.Height);
					this.arrowMax.Visibility = (arrowLength > 0);
					this.arrowMax.SetManualBounds(bounds);

					bounds = new Rectangle(0, 0, arrowLength-2, rect.Height);
					this.arrowMin.Visibility = (arrowLength > 0);
					this.arrowMin.SetManualBounds(bounds);
				}

				bounds = new Rectangle(rect.Width-arrowLength2, 0, arrowLength, rect.Height);
				this.arrowUp.Visibility = (arrowLength > 0);
				this.arrowUp.SetManualBounds(bounds);

				bounds = new Rectangle(arrowLength1, 0, arrowLength, rect.Height);
				this.arrowDown.Visibility = (arrowLength > 0);
				this.arrowDown.SetManualBounds(bounds);

				rect.Left  += arrowLength2;
				rect.Right -= arrowLength2;
			}
			
			this.sliderRect = rect;
			this.UpdateInternalGeometry();
		}
		
		protected virtual void UpdateInternalGeometry()
		{
			Rectangle sliderRect = this.sliderRect;
			Rectangle tabRect    = Rectangle.Empty;
			Rectangle thumbRect  = Rectangle.Empty;
			
			if (this.Range > 0)
			{
				decimal lp = this.LogarithmicValue - this.MinValue;
				double pos   = (double) (this.isInverted ? this.Range - lp : lp);
				double range = (double) (this.Range);

				if (this.isVertical)
				{
					
					double h = AbstractSlider.handleBreadth;
					double p = (pos/range) * (sliderRect.Height-h);

					thumbRect = sliderRect;
					thumbRect.Bottom += p;
					thumbRect.Height  = h;
					
					switch (this.HiliteZone)
					{
						case Zone.PageDown:
							tabRect     = sliderRect;
							tabRect.Top = thumbRect.Bottom;
							break;
						case Zone.PageUp:
							tabRect        = sliderRect;
							tabRect.Bottom = thumbRect.Top;
							break;
					}
				}
				else
				{
					double h = AbstractSlider.handleBreadth;
					double p = (pos/range) * (sliderRect.Width-h);

					thumbRect = sliderRect;
					thumbRect.Left += p;
					thumbRect.Width = h;
					
					switch (this.HiliteZone)
					{
						case Zone.PageDown:
							tabRect       = sliderRect;
							tabRect.Right = thumbRect.Left;
							break;
						case Zone.PageUp:
							tabRect      = sliderRect;
							tabRect.Left = thumbRect.Right;
							break;
					}
				}
			}

			this.thumbRect = thumbRect;
			this.tabRect   = tabRect;
		}

		protected void UpdateMinMaxButtons()
		{
			if (this.isMinMaxButtons)
			{
				this.arrowMax.Enable = (this.Value < this.MaxValue);
				this.arrowMin.Enable = (this.Value > this.MinValue);
			}
		}

		
		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.IsEnabled == false)
			{
				return;
			}
			
			if (message.IsMouseType)
			{
				if (this.isScrolling)
				{
					if (message.Type == MessageType.MouseUp)
					{
						this.isScrolling = false;
						message.Consumer = this;
					}
					
					return;
				}
				
				if (! this.isDragging && (message.Type != MessageType.MouseLeave))
				{
					this.HiliteZone = this.DetectZone(pos);
					
					if (this.HiliteZone == Zone.PageDown || this.HiliteZone == Zone.PageUp)
					{
						if (message.Type == MessageType.MouseDown && message.Button == MouseButtons.Left)
						{
							this.isScrolling = true;
							this.ScrollByPages(true);
							message.Consumer = this;
							return;
						}
					}
				}
			}

			if (!this.isScrolling)
			{
				if (!this.dragBehavior.ProcessMessage(message, pos))
				{
					base.ProcessMessage(message, pos);
				}
			}
		}
		
		
		protected virtual Zone DetectZone(Point pos)
		{
			if (this.arrowUp != null && this.arrowDown != null)
			{
				if (this.IsEnabled)
				{
					if (this.isVertical)
					{
						if (pos.Y < this.thumbRect.Bottom)
						{
							return Zone.PageDown;
						}
						else if (pos.Y > this.thumbRect.Top)
						{
							return Zone.PageUp;
						}
						else
						{
							return Zone.Thumb;
						}
					}
					else
					{
						if (pos.X < this.thumbRect.Left)
						{
							return Zone.PageDown;
						}
						else if (pos.X > this.thumbRect.Right)
						{
							return Zone.PageUp;
						}
						else
						{
							return Zone.Thumb;
						}
					}
				}
			}
			
			return Zone.None;
		}

		protected void ScrollByDragging(double pos, bool isInitialChange)
		{
			double offset;
			double length;

			if (this.isVertical)
			{
				offset = pos-this.sliderRect.Bottom;
				length = this.sliderRect.Height-this.thumbRect.Height;
			}
			else
			{
				offset = pos-this.sliderRect.Left;
				length = this.sliderRect.Width-this.thumbRect.Width;
			}
			
			if (length > 0)
			{
				double newPos = offset / length;

				if (this.isInverted)
				{
					newPos = 1.0 - newPos;
				}

				decimal value = (decimal) newPos;
				
				this.isInitialChange = isInitialChange;
				this.LogarithmicValue = value * this.Range + this.MinValue;
			}
		}

		protected void ScrollByPages(bool isInitialChange)
		{
			switch (this.HiliteZone)
			{
				case Zone.PageUp:
					this.isInitialChange = isInitialChange;
					this.Value += (this.isInverted) ? -this.pageStep : this.pageStep;
					break;
				case Zone.PageDown:
					this.isInitialChange = isInitialChange;
					this.Value += (this.isInverted) ? this.pageStep : -this.pageStep;
					break;
			}
		}


		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;

			if (button == this.arrowMax)
			{
				this.Value = this.MaxValue;
				this.Invalidate();
			}

			if (button == this.arrowMin)
			{
				this.Value = this.MinValue;
				this.Invalidate();
			}
		}

		private void HandleButton(object sender)
		{
			GlyphButton button = sender as GlyphButton;

			if ( button == this.arrowUp )
			{
				this.isInitialChange = true;
				if ( this.isInverted )  this.Value -= this.buttonStep;
				else                    this.Value += this.buttonStep;
				this.Invalidate();
			}
			else if ( button == this.arrowDown )
			{
				this.isInitialChange = true;
				if ( this.isInverted )  this.Value += this.buttonStep;
				else                    this.Value -= this.buttonStep;
				this.Invalidate();
			}
		}

		private void HandleRangeChanged()
		{
			this.UpdateEnable();
			this.UpdateInternalGeometry();
			this.Invalidate();
		}
		
		protected virtual void UpdateEnable()
		{
			this.Enable = (this.range.IsEmpty) ? false : true;
		}

		protected virtual  void OnValueChanged()
		{
			this.UpdateInternalGeometry();

			EventHandler handler = (EventHandler) this.GetUserEventHandler("ValueChanged");

			if (handler != null)
			{
				handler(this);
			}
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered(e);
			this.Invalidate();
		}
		
		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited(e);
			this.HiliteZone = Zone.None;
			this.Invalidate();
		}

		protected override void OnStillEngaged()
		{
			base.OnStillEngaged();
			this.ScrollByPages(false);
		}

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Direction dir = this.isVertical ? Direction.Up : Direction.Left;
			WidgetPaintState state = this.PaintState;
			
			//	Dessine le fond.
			adorner.PaintSliderBackground(graphics, this.Client.Bounds, this.thumbRect, this.tabRect, state & ~WidgetPaintState.Entered, dir);
			
			//	Dessine la cabine.
			if (this.thumbRect.IsValid && this.IsEnabled)
			{
				Rectangle rect = this.thumbRect;
				graphics.Align(ref rect);
				
				if (this.HiliteZone != Zone.Thumb)
				{
					state &= ~ WidgetPaintState.Entered;
					state &= ~ WidgetPaintState.Engaged;
				}
				if (this.isDragging)
				{
					state |= WidgetPaintState.Engaged;
					state |= WidgetPaintState.Entered;
				}
				
				adorner.PaintSliderHandle(graphics, rect, this.tabRect, state, dir);
			}
		}


		#region IDragBehaviorHost Members

		Point Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.DragLocation
		{
			get
			{
				return this.DragLocation;
			}
		}

		bool Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragBegin(Point cursor)
		{
			return this.OnDragBegin(cursor);
		}

		void Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			this.OnDragging(e);
		}

		void Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragEnd()
		{
			this.OnDragEnd();
		}

		#endregion

		protected virtual Point						DragLocation
		{
			get
			{
				return this.thumbRect.Location;
			}
		}

		protected virtual bool OnDragBegin(Point cursor)
		{
			this.isDragging = true;
			this.isFirstDragging = true;
			this.Invalidate();
			return true;
		}

		protected virtual void OnDragging(DragEventArgs e)
		{
			this.ScrollByDragging(this.isVertical ? e.ToPoint.Y : e.ToPoint.X, this.isFirstDragging);
			this.isFirstDragging = false;
		}

		protected virtual void OnDragEnd()
		{
			this.isDragging = false;
			this.Invalidate();
		}
		
		#region INumValue Members
		public decimal						Value
		{
			get
			{
				return (decimal) this.GetValue(AbstractSlider.ValueProperty);
			}
			set
			{
				this.SetValue(AbstractSlider.ValueProperty, value);
				this.UpdateMinMaxButtons();
			}
		}

		public decimal						MinValue
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

					this.range = new Types.DecimalRange(min, max, res);
					this.HandleRangeChanged();
				}
			}
		}

		public decimal						MaxValue
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

					this.range = new Types.DecimalRange(min, max, res);
					this.HandleRangeChanged();
				}
			}
		}

		public decimal						Resolution
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

					this.range = new Types.DecimalRange(min, max, res);
					this.HandleRangeChanged();
				}
			}
		}

		public decimal						LogarithmicDivisor
		{
			get
			{
				return this.logarithmicDivisor;
			}

			set
			{
				this.logarithmicDivisor = value;
			}
		}

		public decimal						LogarithmicValue
		{
			get
			{
				if ( this.MaxValue == this.MinValue )
				{
					return this.Value;
				}
				else
				{
					decimal norm = (this.Value-this.MinValue)/(this.MaxValue-this.MinValue);
					norm = (decimal)System.Math.Pow((double)norm, (double)(1.0M/this.logarithmicDivisor));
					return norm*(this.MaxValue-this.MinValue)+this.MinValue;
				}
			}
			set
			{
				if ( this.MaxValue != this.MinValue )
				{
					decimal norm = (value-this.MinValue)/(this.MaxValue-this.MinValue);
					norm = (decimal)System.Math.Pow((double)norm, (double)this.logarithmicDivisor);
					value = norm*(this.MaxValue-this.MinValue)+this.MinValue;
				}
				this.Value = value;
			}
		}

		public decimal						Range
		{
			get
			{
				return this.MaxValue - this.MinValue;
			}
		}

		public bool							IsInitialChange
		{
			get
			{
				return this.isInitialChange;
			}
		}
		
		
		public event EventHandler			ValueChanged
		{
			add
			{
				this.AddUserEventHandler("ValueChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ValueChanged", value);
			}
		}
		#endregion
		
		#region Zone enumeration
		protected enum Zone
		{
			None,
			Thumb,
			PageUp,
			PageDown
		}
		#endregion

		private static void NotifyValueChanged(DependencyObject o, object oldValue, object newValue)
		{
			AbstractSlider that = o as AbstractSlider;
			that.OnValueChanged();
		}

		private static object CoerceValue(DependencyObject o, DependencyProperty property, object value)
		{
			AbstractSlider that = o as AbstractSlider;
			decimal num = (decimal) value;

			num = that.range.Constrain(num);

			return num;
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(AbstractSlider), new Helpers.VisualPropertyMetadata(0M, AbstractSlider.NotifyValueChanged, AbstractSlider.CoerceValue, Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		
		protected static readonly double	defaultBreadth = 16;
		protected static readonly double	handleBreadth = 7;
		protected static readonly double	minimalThumb = 8;
		protected static readonly double	minimalArrow = 6;
		
		private Behaviors.DragBehavior		dragBehavior;
		
		private bool						isVertical;
		private bool						isInverted;
		private bool						isMinMaxButtons = false;
		private decimal						buttonStep = 0.1M;
		private decimal						pageStep   = 0.2M;
		private GlyphButton					arrowMax;
		private GlyphButton					arrowUp;
		private GlyphButton					arrowDown;
		private GlyphButton					arrowMin;
		private bool						isScrolling;
		private bool						isDragging;
		private bool						isFirstDragging;
		private Rectangle					sliderRect;
		private Rectangle					thumbRect;
		private Rectangle					tabRect;
		
		private Zone						hiliteZone;
		protected Types.DecimalRange		range = new Types.DecimalRange(0, 1, 0.000001M);
		private decimal						logarithmicDivisor = 1;
		private bool						isInitialChange = true;
	}
}
