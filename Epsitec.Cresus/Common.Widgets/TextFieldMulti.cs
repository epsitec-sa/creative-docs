namespace Epsitec.Common.Widgets
{
	using Keys = System.Windows.Forms.Keys;
	
	/// <summary>
	/// La classe TextFieldMulti implémente la ligne éditable multiple.
	/// </summary>
	public class TextFieldMulti : AbstractTextField
	{
		public TextFieldMulti()
		{
			this.scroller = new VScroller();
			this.scroller.SetEnabled(false);
			this.scroller.Moved += new EventHandler(this.HandleScrollerMoved);
			this.scroller.Parent = this;
			this.scroller.Dock = DockStyle.Right;
			
			this.rightMargin = this.scroller.Width;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.Moved -= new EventHandler(this.HandleScrollerMoved);
				this.scroller.Dispose ();
				this.scroller = null;
			}
			
			base.Dispose (disposing);
		}
		
		protected override void CursorScrollTextEnd(Drawing.Point end, Drawing.Rectangle cursor)
		{
			double offset = cursor.Bottom;
			offset -= this.realSize.Height/2;
			offset  = System.Math.Max (offset, end.Y);
			offset += this.realSize.Height;
			offset  = System.Math.Min (offset, AbstractTextField.Infinity);
			this.scrollOffset.Y = offset-this.realSize.Height;

			if ( this.scroller != null )
			{
				double h = AbstractTextField.Infinity-end.Y;  // hauteur de tout le texte
				if ( h <= this.realSize.Height )
				{
					this.scroller.SetEnabled(false);
					this.scroller.Range = 1;
					this.scroller.Display = 1;
					this.scroller.Value = 0;
				}
				else
				{
					this.scroller.SetEnabled(true);
					this.scroller.Range = h-this.realSize.Height;
					this.scroller.Display = this.realSize.Height/h * this.scroller.Range;
					this.scroller.Value = this.scroller.Range - (AbstractTextField.Infinity-offset);
					this.scroller.SmallChange = 20;
					this.scroller.LargeChange = this.realSize.Height/2;
				}
			}
		}
		
		protected override void ProcessKeyDown(Keys key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch ( key )
			{
				case Keys.Enter:
					this.InsertCharacter('\n');
					break;

				case Keys.Home:
					this.MoveExtremity(-1, isShiftPressed, isCtrlPressed);
					break;

				case Keys.End:
					this.MoveExtremity(1, isShiftPressed, isCtrlPressed);
					break;

				case Keys.Up:
					this.MoveLine(-1, isShiftPressed, isCtrlPressed);
					break;

				case Keys.Down:
					this.MoveLine(1, isShiftPressed, isCtrlPressed);
					break;
				
				default:
					base.ProcessKeyDown (key, isShiftPressed, isCtrlPressed);
					break;
			}
		}
		
		protected override void UpdateLayoutSize()
		{
			if ( this.textLayout != null )
			{
				double dx = this.Client.Width - AbstractTextField.Margin*2 - this.rightMargin - this.leftMargin;
				double dy = this.Client.Height - AbstractTextField.Margin*2;
				this.realSize = new Drawing.Size(dx, dy);
				this.textLayout.Alignment = this.Alignment;
				this.textLayout.LayoutSize = new Drawing.Size(dx, AbstractTextField.Infinity);

				if ( this.textLayout.Text != null )
				{
					this.CursorScroll();
				}
			}
		}

		private void HandleScrollerMoved(object sender)
		{
			this.scrollOffset.Y = this.scroller.Value-this.scroller.Range+AbstractTextField.Infinity-this.realSize.Height;
			this.Invalidate();
		}
		
		protected VScroller						scroller;
	}
}
