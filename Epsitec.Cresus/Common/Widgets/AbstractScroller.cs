//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			this.dragBehavior = new Behaviors.DragBehavior (this, true, false);
			
			this.isVertical = vertical;
			
			this.AutoEngage = true;
			this.AutoRepeat = true;
			
			this.InternalState |= WidgetInternalState.Engageable;

			this.arrowUp = new GlyphButton(this);
			this.arrowDown = new GlyphButton(this);
			this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
			this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
			this.arrowUp.ButtonStyle = ButtonStyle.Scroller;
			this.arrowDown.ButtonStyle = ButtonStyle.Scroller;
			this.arrowUp.Engaged += this.HandleButton;
			this.arrowDown.Engaged += this.HandleButton;
			this.arrowUp.StillEngaged += this.HandleButton;
			this.arrowDown.StillEngaged += this.HandleButton;
			this.arrowUp.AutoRepeat = true;
			this.arrowDown.AutoRepeat = true;
		}
		
		protected AbstractScroller(Widget embedder, bool vertical) : this(vertical)
		{
			this.SetEmbedder(embedder);
		}


		static AbstractScroller()
		{
			Types.DependencyPropertyMetadata metadata = Widget.AutoCaptureProperty.DefaultMetadata.Clone ();
			
			metadata.DefineDefaultValue (true);
			
			Widget.AutoCaptureProperty.OverrideMetadata (typeof (AbstractScroller), metadata);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.arrowUp.Engaged -= this.HandleButton;
				this.arrowDown.Engaged -= this.HandleButton;
				this.arrowUp.StillEngaged -= this.HandleButton;
				this.arrowDown.StillEngaged -= this.HandleButton;
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
				return this.smallChange;
			}

			set
			{
				this.smallChange = value;
			}
		}
		
		public decimal						LargeChange
		{
			//	Valeur avancée en cliquant hors de la cabine.
			get
			{
				return this.largeChange;
			}

			set
			{
				this.largeChange = value;
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


		public void SimulateArrowEngaged(int direction)
		{
			if (direction == 0)
			{
				this.Window.EngagedWidget = null;
			}
			else if (direction < 0)
			{
				this.Window.EngagedWidget = this.ArrowDown;
			}
			else if (direction > 0)
			{
				this.Window.EngagedWidget = this.ArrowUp;
			}
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
			
			double arrowLength = this.isVertical ? rect.Width : rect.Height;
			double totalLength = this.isVertical ? rect.Height : rect.Width;
			
			if (arrowLength * 2 > totalLength - AbstractScroller.minimalThumb)
			{
				//	Les boutons occupent trop de place. Il faut donc les comprimer.
				
				arrowLength = System.Math.Floor ((totalLength - AbstractScroller.minimalThumb) / 2);
				
				if (arrowLength < AbstractScroller.minimalArrow)
				{
					//	S'il n'y a plus assez de place pour afficher un bouton visible,
					//	autant les cacher complètement !
					
					arrowLength = 0;
				}
			}
			
			if (this.isVertical)
			{
				Drawing.Rectangle bounds;
				
				bounds = new Drawing.Rectangle (0, rect.Height - arrowLength, rect.Width, arrowLength);
				
				this.arrowUp.Visibility = (arrowLength > 0);
				this.arrowUp.SetManualBounds(bounds);
				this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
				
				bounds = new Drawing.Rectangle (0, 0, rect.Width, arrowLength);
				
				this.arrowDown.Visibility = (arrowLength > 0);
				this.arrowDown.SetManualBounds(bounds);
				this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
				
				rect.Bottom += arrowLength;
				rect.Top    -= arrowLength;
			}
			else
			{
				Drawing.Rectangle bounds;
				
				bounds = new Drawing.Rectangle (rect.Width - arrowLength, 0, arrowLength, rect.Height);
				
				this.arrowUp.Visibility = (arrowLength > 0);
				this.arrowUp.SetManualBounds(bounds);
				this.arrowUp.GlyphShape = GlyphShape.ArrowRight;
				
				bounds = new Drawing.Rectangle (0, 0, arrowLength, rect.Height);
				
				this.arrowDown.Visibility = (arrowLength > 0);
				this.arrowDown.SetManualBounds(bounds);
				this.arrowDown.GlyphShape = GlyphShape.ArrowLeft;
				
				rect.Left  += arrowLength;
				rect.Right -= arrowLength;
			}
			
			this.sliderRect = rect;
			this.UpdateInternalGeometry ();
		}
		
		protected virtual void UpdateInternalGeometry()
		{
			Drawing.Rectangle sliderRect = this.sliderRect;
			Drawing.Rectangle tabRect    = Drawing.Rectangle.Empty;
			Drawing.Rectangle thumbRect  = Drawing.Rectangle.Empty;

			decimal value = System.Math.Max (0, System.Math.Min (this.Range, this.position));
			
			if ((this.Range > 0) && (this.VisibleRangeRatio > 0))
			{
				double pos   = (double) (this.isInverted ? this.Range - value : value);
				double range = (double) (this.Range);
				double ratio = (double) (this.VisibleRangeRatio);
				
				if (this.isVertical)
				{
					double h = sliderRect.Height * ratio;
					h = System.Math.Max (h, AbstractScroller.minimalThumb);
					h = System.Math.Min (h, sliderRect.Height);
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
					double h = sliderRect.Width * ratio;
					h = System.Math.Max (h, AbstractScroller.minimalThumb);
					h = System.Math.Min (h, sliderRect.Width);
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
			
			this.thumbRect  = thumbRect;
			this.tabRect    = tabRect;
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
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
					this.HiliteZone = this.DetectZone (pos);
					
					if ((this.HiliteZone == Zone.PageDown) ||
						(this.HiliteZone == Zone.PageUp))
					{
						if ((message.MessageType == MessageType.MouseDown) &&
							(message.Button == MouseButtons.Left))
						{
							this.isScrolling = true;
							this.ScrollByPages ();
							message.Consumer = this;
							return;
						}
					}
				}
			}
			
			if (! this.isScrolling)
			{
				if (! this.dragBehavior.ProcessMessage (message, pos))
				{
					base.ProcessMessage (message, pos);
				}
			}
		}
		
		
		protected Zone DetectZone(Drawing.Point pos)
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
			
			return Zone.None;
		}

		protected void ScrollByDragging(double pos)
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
				
				this.Value = value * this.Range + this.MinValue;
			}
		}

		protected void ScrollByPages()
		{
			switch (this.HiliteZone)
			{
				case Zone.PageUp:
					this.Value += (this.isInverted) ? -this.largeChange : this.largeChange;
					break;
				case Zone.PageDown:
					this.Value += (this.isInverted) ? this.largeChange : -this.largeChange;
					break;
			}
		}

		
		private void HandleButton(object sender)
		{
			GlyphButton button = sender as GlyphButton;

			if ( button == this.arrowUp )
			{
				if ( this.isInverted )  this.Value -= this.smallChange;
				else                     this.Value += this.smallChange;
				this.Invalidate();
			}
			else if ( button == this.arrowDown )
			{
				if ( this.isInverted )  this.Value += this.smallChange;
				else                     this.Value -= this.smallChange;
				this.Invalidate();
			}
		}

		private void HandleRangeChanged()
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

			var handler = this.GetUserEventHandler("ValueChanged");

			if (handler != null)
			{
				handler(this);
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
			
			Widgets.Direction dir   = this.isVertical ? Direction.Up : Direction.Left;
			WidgetPaintState       state = this.GetPaintState ();

			//	Dessine le fond.
			adorner.PaintScrollerBackground (graphics, this.Client.Bounds, this.thumbRect, this.tabRect, state & ~WidgetPaintState.Entered, dir);
			
			//	Dessine la cabine.
			if (this.thumbRect.IsValid && this.IsEnabled)
			{
				Drawing.Rectangle rect = this.thumbRect;
				rect = graphics.Align (rect);
				
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
			this.isDragging = true;
			this.Invalidate ();
			return true;
		}
		
		void Behaviors.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			this.ScrollByDragging (this.isVertical ? e.ToPoint.Y : e.ToPoint.X);
		}
		
		void Behaviors.IDragBehaviorHost.OnDragEnd()
		{
			this.isDragging = false;
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
					decimal min = value;
					decimal max = this.range.Maximum;
					decimal res = this.range.Resolution;

					this.range = new Types.DecimalRange (min, max, res);
					this.HandleRangeChanged ();
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

					this.range = new Types.DecimalRange (min, max, res);
					this.HandleRangeChanged ();
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

					this.range = new Types.DecimalRange (min, max, res);
					this.HandleRangeChanged ();
				}
			}
		}

		public decimal						Range
		{
			get
			{
				return this.MaxValue - this.MinValue;
			}
		}
		
		
		public event Support.EventHandler	ValueChanged
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
		
		public static readonly double		DefaultBreadth = 17;
		protected static readonly double	minimalThumb = 8;
		protected static readonly double	minimalArrow = 6;
		
		private Behaviors.DragBehavior		dragBehavior;
		
		private bool						isVertical;
		private bool						isInverted;
		private decimal						display    = 0.5M;
		private decimal						position   = 0.0M;
		private decimal						smallChange = 0.1M;
		private decimal						largeChange   = 0.2M;
		private GlyphButton					arrowUp;
		private GlyphButton					arrowDown;
		private bool						isScrolling;
		private bool						isDragging;
		private Drawing.Rectangle			sliderRect;
		private Drawing.Rectangle			thumbRect;
		private Drawing.Rectangle			tabRect;
		
		private Zone						hiliteZone;
		private Types.DecimalRange			range = new Types.DecimalRange (0, 1, 0.000001M);
	}
}
