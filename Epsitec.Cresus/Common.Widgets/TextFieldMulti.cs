namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldMulti implémente la ligne éditable multiple.
	/// </summary>
	public class TextFieldMulti : AbstractTextField
	{
		public TextFieldMulti()
		{
			this.TextLayout.BreakMode &= ~Drawing.TextBreakMode.SingleLine;
			this.TextLayout.BreakMode |=  Drawing.TextBreakMode.Hyphenate;
			
			this.textFieldStyle = TextFieldStyle.Multi;

			this.scroller = new VScroller(this);
			this.scroller.Enable = false;
			this.scroller.ValueChanged += new Support.EventHandler(this.HandleScrollerValueChanged);
			//this.scroller.Dock = DockStyle.Right;
			
			this.margins.Right = this.scroller.PreferredWidth;
		}
		
		public TextFieldMulti(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.scroller != null )
				{
					this.scroller.ValueChanged -= new Support.EventHandler(this.HandleScrollerValueChanged);
					this.scroller.Dispose();
				}
				this.scroller = null;
			}
			
			base.Dispose(disposing);
		}
		
		protected override void UpdateGeometry()
		{
			base.UpdateGeometry ();
			
			if ( this.scroller != null )
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = this.ActualWidth-this.margins.Right-adorner.GeometryScrollerRightMargin;
				rect.Right  = this.ActualWidth-adorner.GeometryScrollerRightMargin;
				rect.Bottom = adorner.GeometryScrollerBottomMargin;
				rect.Top    = this.ActualHeight-adorner.GeometryScrollerTopMargin;
				this.scroller.SetManualBounds(rect);
			}
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateGeometry();
			base.OnAdornerChanged();
		}

		protected override void CursorScrollText(Drawing.Rectangle cursor, bool force)
		{
			Drawing.Point end = this.TextLayout.FindTextEnd();

			if ( force )
			{
				double offset = cursor.Bottom;
				offset -= this.realSize.Height/2;
				offset  = System.Math.Max(offset, end.Y);
				offset += this.realSize.Height;
				offset  = System.Math.Min(offset, AbstractTextField.Infinity);
				this.scrollOffset.Y = offset-this.realSize.Height;
			}
			else
			{
				double ratioBottom = (cursor.Bottom-this.scrollOffset.Y)/this.realSize.Height;  // 0..1
				double ratioTop    = (cursor.Top   -this.scrollOffset.Y)/this.realSize.Height;  // 0..1
				double zone = this.scrollZone*0.5;

				double h = AbstractTextField.Infinity-end.Y;  // hauteur de tout le texte
				if ( h <= this.realSize.Height || this.realSize.Height < 0 )
				{
					this.scrollOffset.Y = AbstractTextField.Infinity-this.realSize.Height;
				}
				else
				{
					if ( ratioBottom <= zone )  // curseur trop bas ?
					{
						this.scrollOffset.Y -= (zone-ratioBottom)*this.realSize.Height;
						double min = System.Math.Min(end.Y, AbstractTextField.Infinity-this.realSize.Height);
						this.scrollOffset.Y = System.Math.Max(this.scrollOffset.Y, min);
					}

					if ( ratioTop >= 1.0-zone )  // curseur trop haut ?
					{
						this.scrollOffset.Y += (ratioTop-(1.0-zone))*this.realSize.Height;
						this.scrollOffset.Y = System.Math.Min(this.scrollOffset.Y, AbstractTextField.Infinity-this.realSize.Height);
					}
				}
			}

			this.scrollOffset.X = 0;
			this.UpdateScroller();
		}
		
		protected override void ScrollVertical(double dist)
		{
			//	Décale le texte vers le haut (+) ou le bas (-), lorsque la
			//	souris dépasse pendant une sélection.
			this.scrollOffset.Y += dist;
			Drawing.Point end = this.TextLayout.FindTextEnd();
			double min = System.Math.Min(end.Y, AbstractTextField.Infinity-this.realSize.Height);
			double max = AbstractTextField.Infinity-this.realSize.Height;
			this.scrollOffset.Y = System.Math.Max(this.scrollOffset.Y, min);
			this.scrollOffset.Y = System.Math.Min(this.scrollOffset.Y, max);
			this.Invalidate();
			this.UpdateScroller();

			Drawing.Point pos = this.lastMousePos;
			pos.X -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos.Y -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos = this.Client.Bounds.Constrain(pos);
			pos += this.scrollOffset;
			this.TextNavigator.MouseMoveMessage(pos);
		}

		protected void UpdateScroller()
		{
			//	Met à jour l'asceuseur en fonction de this.scrollOffset.
			if ( this.scroller == null )  return;

			Drawing.Point end = this.TextLayout.FindTextEnd();
			double h = AbstractTextField.Infinity-end.Y;  // hauteur de tout le texte
			if ( h <= this.realSize.Height || this.realSize.Height < 0 )
			{
				this.scroller.Enable            = false;
				this.scroller.MaxValue          = 0;
				this.scroller.VisibleRangeRatio = 0;
				this.scroller.Value             = 0;
			}
			else
			{
				this.scroller.Enable            = true;
				this.scroller.MaxValue          = (decimal) (h-this.realSize.Height);
				this.scroller.VisibleRangeRatio = (decimal) (this.realSize.Height/h);
				
				double offset = this.scrollOffset.Y+this.realSize.Height;
				decimal value = this.scroller.Range - (decimal) (AbstractTextField.Infinity-offset);
				
				if ( value > this.scroller.Range )  value = this.scroller.Range;
				if ( value < 0 )                    value = 0;
				
				this.scroller.Value       = value;
				this.scroller.SmallChange = 20;
				this.scroller.LargeChange = (decimal) (this.realSize.Height/2.0);
			}
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			decimal v;
			switch ( message.Type )
			{
				case MessageType.KeyDown:
					if ( message.KeyCode == KeyCode.ArrowUp && message.IsControlPressed )
					{
						v = this.scroller.Value;
						v = System.Math.Min(v+this.scroller.SmallChange*0.5m, this.scroller.Range);
						this.scroller.Value = v;
						message.Consumer = this;
						return;
					}
					if ( message.KeyCode == KeyCode.ArrowDown && message.IsControlPressed )
					{
						v = this.scroller.Value;
						v = System.Math.Max(v-this.scroller.SmallChange*0.5m, 0);
						this.scroller.Value = v;
						message.Consumer = this;
						return;
					}
					break;

				case MessageType.MouseWheel:
					v = this.scroller.Value;
					if ( message.Wheel > 0 )  v = System.Math.Min(v+this.scroller.SmallChange, this.scroller.Range);
					if ( message.Wheel < 0 )  v = System.Math.Max(v-this.scroller.SmallChange, 0);
					this.scroller.Value = v;
					message.Consumer = this;
					return;
			}

			base.ProcessMessage(message, pos);
		}

		protected override Drawing.Size GetTextLayoutSize()
		{
			return new Drawing.Size(this.realSize.Width, AbstractTextField.Infinity);
		}

		private void HandleScrollerValueChanged(object sender)
		{
			this.scrollOffset.Y = this.scroller.DoubleValue-this.scroller.DoubleRange+AbstractTextField.Infinity-this.realSize.Height;
			this.Invalidate();
		}
		
		
		protected VScroller						scroller;
	}
}
