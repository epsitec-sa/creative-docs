namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldMulti implémente la ligne éditable multiple.
	/// </summary>
	public class TextFieldMulti : AbstractTextField
	{
		public TextFieldMulti()
		{
			this.textStyle = TextFieldStyle.Multi;

			this.scroller = new VScroller(this);
			this.scroller.SetEnabled(false);
			this.scroller.ValueChanged += new Support.EventHandler(this.HandleScrollerValueChanged);
			//this.scroller.Dock = DockStyle.Right;
			
			this.margins.Right = this.scroller.Width;
		}
		
		public TextFieldMulti(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.scroller.ValueChanged -= new Support.EventHandler(this.HandleScrollerValueChanged);
				this.scroller.Dispose();
				this.scroller = null;
			}
			
			base.Dispose(disposing);
		}
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.scroller != null )
			{
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = this.Bounds.Width-this.margins.Right-adorner.GeometryScrollerRightMargin;
				rect.Right  = this.Bounds.Width-adorner.GeometryScrollerRightMargin;
				rect.Bottom = adorner.GeometryScrollerBottomMargin;
				rect.Top    = this.Bounds.Height-adorner.GeometryScrollerTopMargin;
				this.scroller.Bounds = rect;
			}
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry();
			base.OnAdornerChanged();
		}

		protected override void CursorScrollTextEnd(Drawing.Point end, Drawing.Rectangle cursor)
		{
			double offset = cursor.Bottom;
			offset -= this.realSize.Height/2;
			offset  = System.Math.Max(offset, end.Y);
			offset += this.realSize.Height;
			offset  = System.Math.Min(offset, AbstractTextField.Infinity);
			this.scrollOffset.Y = offset-this.realSize.Height;

			if ( this.scroller != null )
			{
				double h = AbstractTextField.Infinity-end.Y;  // hauteur de tout le texte
				if ( h <= this.realSize.Height || this.realSize.Height < 0 )
				{
					this.scroller.SetEnabled(false);
					this.scroller.MaxValue          = 1;
					this.scroller.VisibleRangeRatio = 1;
					this.scroller.Value             = 0;
				}
				else
				{
					this.scroller.SetEnabled(true);
					this.scroller.MaxValue          = (decimal) (h-this.realSize.Height);
					this.scroller.VisibleRangeRatio = (decimal) (this.realSize.Height/h);
					
					decimal value = this.scroller.Range - (decimal) (AbstractTextField.Infinity-offset);
					
					if (value > this.scroller.Range) value = this.scroller.Range;
					if (value < 0)                   value = 0;
					
					this.scroller.Value       = value;
					this.scroller.SmallChange = 20;
					this.scroller.LargeChange = (decimal) (this.realSize.Height/2.0);
				}
			}
		}
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseWheel:
					decimal v = this.scroller.Value;
					if (message.Wheel > 0)  v = System.Math.Min(v+this.scroller.SmallChange, this.scroller.Range);
					if (message.Wheel < 0)  v = System.Math.Max(v-this.scroller.SmallChange, 0);
					this.scroller.Value = v;
					message.Consumer = this;
					return;
			}

			base.ProcessMessage(message, pos);
		}

		protected override bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch ( key )
			{
				case KeyCode.Return:
					this.InsertCharacter('\n');
					break;

				case KeyCode.Home:
					this.MoveExtremity(-1, isShiftPressed, isCtrlPressed);
					break;

				case KeyCode.End:
					this.MoveExtremity(1, isShiftPressed, isCtrlPressed);
					break;

				case KeyCode.ArrowUp:
					this.MoveLine(-1, isShiftPressed, isCtrlPressed);
					break;

				case KeyCode.ArrowDown:
					this.MoveLine(1, isShiftPressed, isCtrlPressed);
					break;
				
				case KeyCode.PageUp:
				case KeyCode.PageDown:
					//	TODO: gérer PageUp et PageDown...
				
				default:
					return base.ProcessKeyDown(key, isShiftPressed, isCtrlPressed);
			}
			
			return true;
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
