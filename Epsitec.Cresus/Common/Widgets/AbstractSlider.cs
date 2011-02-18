using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractSlider implémente la classe de base des potentiomètres linéaires
	/// HSlider et VSlider.
	/// </summary>
	public abstract class AbstractSlider : Widget, Behaviors.IDragBehaviorHost, Support.Data.INumValue
	{
		protected AbstractSlider(bool vertical, bool hasButtons)
		{
			this.isVertical   = vertical;
			this.dragBehavior = this.CreateDragBehavior();

			this.ContainerLayoutMode = this.isVertical ? ContainerLayoutMode.VerticalFlow : ContainerLayoutMode.HorizontalFlow;
			
			this.AutoEngage = true;
			this.AutoRepeat = true;
			
			this.showScrollButtons = hasButtons;
			this.showMinMaxButtons = false;
			
			this.InternalState |= WidgetInternalState.Engageable;

			if (hasButtons)
			{
				this.arrowMax = new GlyphButton (this);
				this.arrowMax.ButtonStyle = ButtonStyle.Icon;
				this.arrowMax.Clicked += this.HandleButtonClicked;
				this.arrowMax.Dock = this.isVertical ? DockStyle.Top : DockStyle.Right;

				this.arrowMin = new GlyphButton (this);
				this.arrowMin.ButtonStyle = ButtonStyle.Icon;
				this.arrowMin.Clicked += this.HandleButtonClicked;
				this.arrowMin.Dock = this.isVertical ? DockStyle.Bottom : DockStyle.Left;
				
				this.arrowUp = new GlyphButton (this);
				this.arrowUp.GlyphShape = GlyphShape.Plus;
				this.arrowUp.ButtonStyle = ButtonStyle.Slider;
				this.arrowUp.Engaged += this.HandleButton;
				this.arrowUp.StillEngaged += this.HandleButton;
				this.arrowUp.AutoRepeat = true;
				this.arrowUp.Dock = this.isVertical ? DockStyle.Top : DockStyle.Right;

				this.arrowDown = new GlyphButton(this);
				this.arrowDown.GlyphShape = GlyphShape.Minus;
				this.arrowDown.ButtonStyle = ButtonStyle.Slider;
				this.arrowDown.Engaged += this.HandleButton;
				this.arrowDown.StillEngaged += this.HandleButton;
				this.arrowDown.AutoRepeat = true;
				this.arrowDown.Dock = this.isVertical ? DockStyle.Bottom : DockStyle.Left;

				this.UpdateGlyphs ();
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
					this.arrowUp.Engaged -= this.HandleButton;
					this.arrowUp.StillEngaged -= this.HandleButton;
				}

				if (this.arrowDown != null)
				{
					this.arrowDown.Engaged -= this.HandleButton;
					this.arrowDown.StillEngaged -= this.HandleButton;
				}

				if (this.arrowMax != null)
				{
					this.arrowMax.Clicked -= this.HandleButtonClicked;
				}

				if (this.arrowMin != null)
				{
					this.arrowMin.Clicked -= this.HandleButtonClicked;
				}
			}
			
			base.Dispose(disposing);
		}


		public bool							IsInverted
		{
			//	Inversion du fonctionnement.
			//	Ascenseur vertical:   false -> zéro en bas
			//	Ascenseur vertical:   true  -> zéro en haut
			//	Ascenseur horizontal: false -> zéro à gauche
			//	Ascenseur horizontal: true  -> zéro à droite
			
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

		public bool UseArrowGlyphs
		{
			get
			{
				return this.useArrowGlyphs;
			}
			set
			{
				if (this.useArrowGlyphs != value)
				{
					this.useArrowGlyphs = value;
					this.UpdateGlyphs ();
					this.Invalidate ();
				}
			}
		}

		public bool ShowMinMaxButtons
		{
			//	Boutons min/max aux extrémités du slider.
			get
			{
				return this.showMinMaxButtons;
			}
			set
			{
				if (this.showMinMaxButtons != value)
				{
					this.showMinMaxButtons = value;
					this.Invalidate ();
				}
			}
		}

		public bool ShowScrollButtons
		{
			//	Boutons pour incrémenter/décrémenter
			get
			{
				return this.showScrollButtons;
			}
			set
			{
				if (this.showScrollButtons != value)
				{
					this.showScrollButtons = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public decimal						SmallChange
		{
			//	Valeur avancée par les boutons.
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
			//	Valeur avancée en cliquant hors de la cabine.
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
			get
			{
				return this.arrowUp;
			}
		}
		
		protected GlyphButton				ArrowDown
		{
			get
			{
				return this.arrowDown;
			}
		}

		protected GlyphButton				ArrowMax
		{
			get
			{
				return this.arrowMax;
			}
		}

		protected GlyphButton				ArrowMin
		{
			get
			{
				return this.arrowMin;
			}
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
			}
			else
			{
				double totalLength = this.isVertical ? rect.Height : rect.Width;
				double arrowLength = this.isVertical ? rect.Width : rect.Height;

				int buttonCount = 0;

				if (this.showScrollButtons)
				{
					buttonCount += 2;
				}
				if (this.showMinMaxButtons)
				{
					buttonCount += 2;
				}

				if ((buttonCount > 0) &&
					((arrowLength * buttonCount) > (totalLength - AbstractSlider.minimalThumb)))
				{
					//	Les boutons occupent trop de place. Il faut donc les comprimer.
					arrowLength = System.Math.Floor ((totalLength-AbstractSlider.minimalThumb)/buttonCount);

					if (arrowLength < AbstractSlider.minimalArrow)
					{
						//	S'il n'y a plus assez de place pour afficher un bouton visible,
						//	autant les cacher complètement !
						arrowLength = 0;
					}
				}

				double arrowLength1 = this.showMinMaxButtons ? arrowLength : 0;
				double arrowLength2 = this.showScrollButtons ? arrowLength : 0;

				Drawing.Size buttonSize = this.isVertical ? new Size (0, arrowLength) : new Size (arrowLength, 0);

				this.arrowMax.Visibility  = arrowLength1 > 0;
				this.arrowMin.Visibility  = arrowLength1 > 0;
				this.arrowUp.Visibility   = arrowLength2 > 0;
				this.arrowDown.Visibility = arrowLength2 > 0;

				this.arrowMax.PreferredSize  = buttonSize;
				this.arrowMin.PreferredSize  = buttonSize;
				this.arrowUp.PreferredSize   = buttonSize;
				this.arrowDown.PreferredSize = buttonSize;

				if (this.isVertical)
				{
					rect.Top    -= arrowLength1;
					rect.Top    -= arrowLength2;
					rect.Bottom += arrowLength1;
					rect.Bottom += arrowLength2;
				}
				else
				{
					rect.Right -= arrowLength1;
					rect.Right -= arrowLength2;
					rect.Left  += arrowLength1;
					rect.Left  += arrowLength2;
				}

				this.sliderRect = rect;
			}
			
			this.UpdateInternalGeometry ();
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
			if (this.arrowUp != null)
			{
				decimal min = this.isInverted ? this.MaxValue : this.MinValue;
				decimal max = this.isInverted ? this.MinValue : this.MaxValue;

				this.arrowUp.Enable   = (this.Value < max);
				this.arrowDown.Enable = (this.Value > min);
				this.arrowMax.Enable  = (this.Value < max);
				this.arrowMin.Enable  = (this.Value > min);
			}
		}

		private void UpdateGlyphs()
		{
			if (this.arrowUp == null)
			{
				return;
			}

			if (this.isVertical)
			{
				this.arrowMax.GlyphShape = GlyphShape.ArrowUp;
				this.arrowMin.GlyphShape = GlyphShape.ArrowDown;
			}
			else
			{
				this.arrowMax.GlyphShape = GlyphShape.ArrowRight;
				this.arrowMin.GlyphShape = GlyphShape.ArrowLeft;
			}

			if (this.useArrowGlyphs)
			{
				if (this.isVertical)
				{
					this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
					this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
				}
				else
				{
					this.arrowUp.GlyphShape = GlyphShape.ArrowRight;
					this.arrowDown.GlyphShape = GlyphShape.ArrowLeft;
				}
			}
			else
			{
				this.arrowUp.GlyphShape = GlyphShape.Plus;
				this.arrowDown.GlyphShape = GlyphShape.Minus;
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
					if (message.MessageType == MessageType.MouseUp)
					{
						this.isScrolling = false;
						message.Consumer = this;
					}
					
					return;
				}
				
				if (! this.isDragging && (message.MessageType != MessageType.MouseLeave))
				{
					this.HiliteZone = this.DetectZone(pos);
					
					if (this.HiliteZone == Zone.PageDown || this.HiliteZone == Zone.PageUp)
					{
						if (message.MessageType == MessageType.MouseDown && message.Button == MouseButtons.Left)
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
				this.Value = this.isInverted ? this.MinValue : this.MaxValue;
				this.Invalidate();
			}

			if (button == this.arrowMin)
			{
				this.Value = this.isInverted ? this.MaxValue : this.MinValue;
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
			WidgetPaintState state = this.GetPaintState ();
			
			//	Dessine le fond.
			adorner.PaintSliderBackground(graphics, this.Client.Bounds, this.sliderRect, this.thumbRect, this.tabRect, state & ~WidgetPaintState.Entered, dir);
			
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
		private bool						showMinMaxButtons;
		private bool						showScrollButtons;
		private bool						useArrowGlyphs;
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
