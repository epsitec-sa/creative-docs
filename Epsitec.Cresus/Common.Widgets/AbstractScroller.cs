namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractScroller implémente la classe de base des ascenseurs
	/// HScroller et VScroller.
	/// </summary>
	public abstract class AbstractScroller : Widget, Behaviors.IDragBehaviorHost, Support.Data.INumValue
	{
		protected AbstractScroller(bool vertical)
		{
			this.drag_behavior = new Behaviors.DragBehavior (this, true, false);
			
			this.is_vertical = vertical;
			
			this.AutoEngage = true;
			this.AutoRepeat = true;
			
			this.InternalState |= InternalState.Engageable;

			this.arrowUp = new GlyphButton(this);
			this.arrowDown = new GlyphButton(this);
			this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
			this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
			this.arrowUp.ButtonStyle = ButtonStyle.Scroller;
			this.arrowDown.ButtonStyle = ButtonStyle.Scroller;
			this.arrowUp.Engaged += new Support.EventHandler(this.HandleButton);
			this.arrowDown.Engaged += new Support.EventHandler(this.HandleButton);
			this.arrowUp.StillEngaged += new Support.EventHandler(this.HandleButton);
			this.arrowDown.StillEngaged += new Support.EventHandler(this.HandleButton);
			this.range.Changed += new Support.EventHandler(this.HandleRangeChanged);
			this.arrowUp.AutoRepeat = true;
			this.arrowDown.AutoRepeat = true;
		}
		
		protected AbstractScroller(Widget embedder, bool vertical) : this(vertical)
		{
			this.SetEmbedder(embedder);
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.arrowUp.Engaged -= new Support.EventHandler(this.HandleButton);
				this.arrowDown.Engaged -= new Support.EventHandler(this.HandleButton);
				this.arrowUp.StillEngaged -= new Support.EventHandler(this.HandleButton);
				this.arrowDown.StillEngaged -= new Support.EventHandler(this.HandleButton);
				this.range.Changed -= new Support.EventHandler(this.HandleRangeChanged);
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
				return this.is_inverted;
			}

			set
			{
				if (this.is_inverted != value)
				{
					this.is_inverted = value;
					this.Invalidate();
				}
			}
		}
		
		public decimal						VisibleRangeRatio
		{
			//	Hauteur visible représentée par l'ascenseur (de 0 à 1).
			
			get
			{
				return this.display;
			}

			set
			{
				System.Diagnostics.Debug.Assert (value >= 0.0M);
				System.Diagnostics.Debug.Assert (value <= 1.0M);
				
				if (this.display != value)
				{
					this.display = value;
					this.UpdateEnable ();
					this.UpdateInternalGeometry ();
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
					this.UpdateInternalGeometry ();
					this.Invalidate ();
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


		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			if ((this.arrowDown == null) ||
				(this.arrowUp == null))
			{
				return;
			}
			
			Drawing.Rectangle rect = this.Client.Bounds;
			
			double arrow_length = this.is_vertical ? rect.Width : rect.Height;
			double total_length = this.is_vertical ? rect.Height : rect.Width;
			
			if (arrow_length * 2 > total_length - AbstractScroller.minimalThumb)
			{
				//	Les boutons occupent trop de place. Il faut donc les comprimer.
				
				arrow_length = System.Math.Floor ((total_length - AbstractScroller.minimalThumb) / 2);
				
				if (arrow_length < AbstractScroller.minimalArrow)
				{
					//	S'il n'y a plus assez de place pour afficher un bouton visible,
					//	autant les cacher complètement !
					
					arrow_length = 0;
				}
			}
			
			if (this.is_vertical)
			{
				Drawing.Rectangle bounds;
				
				bounds = new Drawing.Rectangle (0, rect.Height - arrow_length, rect.Width, arrow_length);
				
				this.arrowUp.Visibility = (arrow_length > 0);
				this.arrowUp.SetManualBounds(bounds);
				this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
				
				bounds = new Drawing.Rectangle (0, 0, rect.Width, arrow_length);
				
				this.arrowDown.Visibility = (arrow_length > 0);
				this.arrowDown.SetManualBounds(bounds);
				this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
				
				rect.Bottom += arrow_length;
				rect.Top    -= arrow_length;
			}
			else
			{
				Drawing.Rectangle bounds;
				
				bounds = new Drawing.Rectangle (rect.Width - arrow_length, 0, arrow_length, rect.Height);
				
				this.arrowUp.Visibility = (arrow_length > 0);
				this.arrowUp.SetManualBounds(bounds);
				this.arrowUp.GlyphShape = GlyphShape.ArrowRight;
				
				bounds = new Drawing.Rectangle (0, 0, arrow_length, rect.Height);
				
				this.arrowDown.Visibility = (arrow_length > 0);
				this.arrowDown.SetManualBounds(bounds);
				this.arrowDown.GlyphShape = GlyphShape.ArrowLeft;
				
				rect.Left  += arrow_length;
				rect.Right -= arrow_length;
			}
			
			this.sliderRect = rect;
			this.UpdateInternalGeometry ();
		}
		
		protected virtual void UpdateInternalGeometry()
		{
			Drawing.Rectangle slider_rect = this.sliderRect;
			Drawing.Rectangle tab_rect    = Drawing.Rectangle.Empty;
			Drawing.Rectangle thumb_rect  = Drawing.Rectangle.Empty;
			
			if ((this.Range > 0) && (this.VisibleRangeRatio > 0))
			{
				double pos   = (double) (this.is_inverted ? this.Range - this.position : this.position);
				double range = (double) (this.Range);
				double ratio = (double) (this.VisibleRangeRatio);
				
				if (this.is_vertical)
				{
					
					double h = slider_rect.Height * ratio;
					h = System.Math.Max (h, AbstractScroller.minimalThumb);
					h = System.Math.Min (h, slider_rect.Height);
					double p = (pos/range) * (slider_rect.Height-h);
					
					thumb_rect = slider_rect;
					thumb_rect.Bottom += p;
					thumb_rect.Height  = h;
					
					switch (this.HiliteZone)
					{
						case Zone.PageDown:
							tab_rect     = slider_rect;
							tab_rect.Top = thumb_rect.Bottom;
							break;
						case Zone.PageUp:
							tab_rect        = slider_rect;
							tab_rect.Bottom = thumb_rect.Top;
							break;
					}
				}
				else
				{
					double h = slider_rect.Width * ratio;
					h = System.Math.Max (h, AbstractScroller.minimalThumb);
					h = System.Math.Min (h, slider_rect.Width);
					double p = (pos/range) * (slider_rect.Width-h);
					
					thumb_rect = slider_rect;
					thumb_rect.Left += p;
					thumb_rect.Width = h;
					
					switch (this.HiliteZone)
					{
						case Zone.PageDown:
							tab_rect       = slider_rect;
							tab_rect.Right = thumb_rect.Left;
							break;
						case Zone.PageUp:
							tab_rect      = slider_rect;
							tab_rect.Left = thumb_rect.Right;
							break;
					}
				}
			}
			
			this.thumbRect  = thumb_rect;
			this.tabRect    = tab_rect;
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.IsEnabled == false)
			{
				return;
			}
			
			if (message.IsMouseType)
			{
				if (this.is_scrolling)
				{
					if (message.Type == MessageType.MouseUp)
					{
						this.is_scrolling = false;
						message.Consumer = this;
					}
					
					return;
				}
				
				if (! this.is_dragging && (message.Type != MessageType.MouseLeave))
				{
					this.HiliteZone = this.DetectZone (pos);
					
					if ((this.HiliteZone == Zone.PageDown) ||
						(this.HiliteZone == Zone.PageUp))
					{
						if ((message.Type == MessageType.MouseDown) &&
							(message.Button == MouseButtons.Left))
						{
							this.is_scrolling = true;
							this.ScrollByPages ();
							message.Consumer = this;
							return;
						}
					}
				}
			}
			
			if (! this.is_scrolling)
			{
				if (! this.drag_behavior.ProcessMessage (message, pos))
				{
					base.ProcessMessage (message, pos);
				}
			}
		}
		
		
		protected Zone DetectZone(Drawing.Point pos)
		{
			if (this.IsEnabled)
			{
				if (this.is_vertical)
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
			
			return Zone.None;
		}

		protected void ScrollByDragging(double pos)
		{
			double offset;
			double length;
			
			if (this.is_vertical)
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
				double new_pos = offset / length;
				
				if (this.is_inverted)
				{
					new_pos = 1.0 - new_pos;
				}
				
				decimal value = (decimal) new_pos;
				
				this.Value = value * this.Range + this.MinValue;
			}
		}

		protected void ScrollByPages()
		{
			switch (this.HiliteZone)
			{
				case Zone.PageUp:
					this.Value += (this.is_inverted) ? -this.pageStep : this.pageStep;
					break;
				case Zone.PageDown:
					this.Value += (this.is_inverted) ? this.pageStep : -this.pageStep;
					break;
			}
		}

		
		private void HandleButton(object sender)
		{
			GlyphButton button = sender as GlyphButton;

			if ( button == this.arrowUp )
			{
				if ( this.is_inverted )  this.Value -= this.buttonStep;
				else                     this.Value += this.buttonStep;
				this.Invalidate();
			}
			else if ( button == this.arrowDown )
			{
				if ( this.is_inverted )  this.Value += this.buttonStep;
				else                     this.Value -= this.buttonStep;
				this.Invalidate();
			}
		}

		private void HandleRangeChanged(object sender)
		{
			this.UpdateEnable ();
			this.UpdateInternalGeometry ();
			this.Invalidate ();
		}
		
		protected virtual void UpdateEnable()
		{
			this.Enable = (this.range.IsEmpty || this.display == 1.0M) ? false : true;
		}

		protected virtual  void OnValueChanged()
		{
			this.UpdateInternalGeometry ();
			if ( this.ValueChanged != null )  // qq'un écoute ?
			{
				this.ValueChanged(this);
			}
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);
			this.Invalidate ();
		}
		
		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);
			this.HiliteZone = Zone.None;
			this.Invalidate ();
		}

		protected override void OnStillEngaged()
		{
			base.OnStillEngaged ();
			this.ScrollByPages ();
		}

		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			Widgets.Direction dir   = this.is_vertical ? Direction.Up : Direction.Left;
			WidgetPaintState       state = this.PaintState;

			//	Dessine le fond.
			adorner.PaintScrollerBackground (graphics, this.Client.Bounds, this.thumbRect, this.tabRect, state & ~WidgetPaintState.Entered, dir);
			
			//	Dessine la cabine.
			if (this.thumbRect.IsValid && this.IsEnabled)
			{
				Drawing.Rectangle rect = this.thumbRect;
				graphics.Align(ref rect);
				
				if (this.HiliteZone != Zone.Thumb)
				{
					state &= ~ WidgetPaintState.Entered;
					state &= ~ WidgetPaintState.Engaged;
				}
				if (this.is_dragging)
				{
					state |= WidgetPaintState.Engaged;
					state |= WidgetPaintState.Entered;
				}
				
				adorner.PaintScrollerHandle(graphics, rect, this.tabRect, state, dir);
			}
		}


		#region IDragBehaviorHost Members
		Drawing.Point						Behaviors.IDragBehaviorHost.DragLocation
		{
			get
			{
				return this.thumbRect.Location;
			}
		}
		
		
		bool Behaviors.IDragBehaviorHost.OnDragBegin(Drawing.Point cursor)
		{
			this.is_dragging = true;
			this.Invalidate ();
			return true;
		}
		
		void Behaviors.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			this.ScrollByDragging (this.is_vertical ? e.ToPoint.Y : e.ToPoint.X);
		}
		
		void Behaviors.IDragBehaviorHost.OnDragEnd()
		{
			this.is_dragging = false;
			this.Invalidate ();
		}
		#endregion
		
		#region INumValue Members
		public decimal						Value
		{
			get
			{
				return this.range.Constrain (this.position + this.range.Minimum);
			}
			set
			{
				value = this.range.Constrain (value) - this.range.Minimum;
				
				if (this.position != value)
				{
					this.position = value;
					this.OnValueChanged ();
					this.Invalidate ();
				}
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
					this.range.Minimum = value;
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
					this.range.Maximum = value;
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
				this.range.Resolution = value;
			}
		}

		public decimal						Range
		{
			get
			{
				return this.MaxValue - this.MinValue;
			}
		}
		
		
		public event Support.EventHandler	ValueChanged;
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
		
		protected static readonly double	defaultBreadth = 17;
		protected static readonly double	minimalThumb = 8;
		protected static readonly double	minimalArrow = 6;
		
		private Behaviors.DragBehavior		drag_behavior;
		
		private bool						is_vertical;
		private bool						is_inverted;
		private decimal						display    = 0.5M;
		private decimal						position   = 0.0M;
		private decimal						buttonStep = 0.1M;
		private decimal						pageStep   = 0.2M;
		private GlyphButton					arrowUp;
		private GlyphButton					arrowDown;
		private bool						is_scrolling;
		private bool						is_dragging;
		private Drawing.Rectangle			sliderRect;
		private Drawing.Rectangle			thumbRect;
		private Drawing.Rectangle			tabRect;
		
		private Zone						hiliteZone;
		private Types.DecimalRange			range = new Types.DecimalRange (0, 1, 0.000001M);
	}
}
