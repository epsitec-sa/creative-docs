namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractScroller impl�mente la classe de base des ascenseurs
	/// HScroller et VScroller.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractScroller : Widget, Helpers.IDragBehaviorHost, Support.INumValue
	{
		protected AbstractScroller(bool vertical)
		{
			this.drag_behavior = new Helpers.DragBehavior (this, true, false);
			
			this.is_vertical = vertical;
			
			this.InternalState |= InternalState.AutoEngage;
			this.InternalState |= InternalState.Engageable;
			this.InternalState |= InternalState.AutoRepeatEngaged;

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
			this.range.Changed += new System.EventHandler(this.HandleRangeChanged);
			this.arrowUp.AutoRepeatEngaged = true;
			this.arrowDown.AutoRepeatEngaged = true;
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
				this.range.Changed -= new System.EventHandler(this.HandleRangeChanged);
			}
			
			base.Dispose(disposing);
		}


		public bool							IsInverted
		{
			// Inversion du fonctionnement.
			// Ascenseur vertical:   false -> z�ro en bas
			// Ascenseur vertical:   true  -> z�ro en haut
			// Ascenseur horizontal: false -> z�ro � gauche
			// Ascenseur horizontal: true  -> z�ro � droite
			
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
			//	Hauteur visible repr�sent�e par l'ascenseur (de 0 � 1).
			
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
					this.UpdateInternalGeometry ();
					this.Invalidate();
				}
			}
		}

		
		public decimal						SmallChange
		{
			// Valeur avanc�e par les boutons.
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
			// Valeur avanc�e en cliquant hors de la cabine.
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
		
		
		public override bool				IsEnabled
		{
			get
			{
				if ((this.range.IsEmpty) ||
					(this.display == 1.0M))
				{
					return false;
				}
				
				return base.IsEnabled;
			}
		}
		
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateGeometry ();
		}
		
		protected virtual void UpdateGeometry()
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
					//	autant les cacher compl�tement !
					
					arrow_length = 0;
				}
			}
			
			if (this.is_vertical)
			{
				Drawing.Rectangle bounds;
				
				bounds = new Drawing.Rectangle (0, rect.Height - arrow_length, rect.Width, arrow_length);
				
				this.arrowUp.SetVisible (arrow_length > 0);
				this.arrowUp.Bounds    = bounds;
				this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
				
				bounds = new Drawing.Rectangle (0, 0, rect.Width, arrow_length);
				
				this.arrowDown.SetVisible (arrow_length > 0);
				this.arrowDown.Bounds    = bounds;
				this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
				
				rect.Bottom += arrow_length;
				rect.Top    -= arrow_length;
			}
			else
			{
				Drawing.Rectangle bounds;
				
				bounds = new Drawing.Rectangle (rect.Width - arrow_length, 0, arrow_length, rect.Height);
				
				this.arrowUp.SetVisible (arrow_length > 0);
				this.arrowUp.Bounds    = bounds;
				this.arrowUp.GlyphShape = GlyphShape.ArrowRight;
				
				bounds = new Drawing.Rectangle (0, 0, arrow_length, rect.Height);
				
				this.arrowDown.SetVisible (arrow_length > 0);
				this.arrowDown.Bounds    = bounds;
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

		private void HandleRangeChanged(object sender, System.EventArgs e)
		{
			this.UpdateInternalGeometry ();
			this.Invalidate ();
		}

		protected virtual  void OnValueChanged()
		{
			this.UpdateInternalGeometry ();
			if ( this.ValueChanged != null )  // qq'un �coute ?
			{
				this.ValueChanged(this);
			}
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);
			this.HiliteZone = Zone.None;
		}

		protected override void OnStillEngaged()
		{
			base.OnStillEngaged ();
			this.ScrollByPages ();
		}

		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			Widgets.Direction dir   = this.is_vertical ? Direction.Up : Direction.Left;
			WidgetState       state = this.PaintState;
			
			// Dessine le fond.
			adorner.PaintScrollerBackground (graphics, this.Client.Bounds, this.thumbRect, this.tabRect, state & ~WidgetState.Entered, dir);
			
			// Dessine la cabine.
			if (this.thumbRect.IsValid && this.IsEnabled)
			{
				Drawing.Rectangle rect = this.thumbRect;
				graphics.Align(ref rect);
				
				if (this.HiliteZone != Zone.Thumb)
				{
					state &= ~ WidgetState.Entered;
					state &= ~ WidgetState.Engaged;
				}
				if (this.is_dragging)
				{
					state |= WidgetState.Engaged;
					state |= WidgetState.Entered;
				}
				
				adorner.PaintScrollerHandle(graphics, rect, this.tabRect, state, dir);
			}
		}


		#region IDragBehaviorHost Members
		Drawing.Point						Helpers.IDragBehaviorHost.DragLocation
		{
			get
			{
				return this.thumbRect.Location;
			}
		}
		
		
		void Helpers.IDragBehaviorHost.OnDragBegin(Drawing.Point cursor)
		{
			this.is_dragging = true;
			this.Invalidate ();
		}
		
		void Helpers.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			this.ScrollByDragging (this.is_vertical ? e.ToPoint.Y : e.ToPoint.X);
		}
		
		void Helpers.IDragBehaviorHost.OnDragEnd()
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
				value = this.range.Constrain (value);
				
				if (this.Value != value)
				{
					this.position = value - this.range.Minimum;
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
				this.range.Minimum = value;
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
				this.range.Maximum = value;
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
		
		private Helpers.DragBehavior		drag_behavior;
		
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
