namespace Epsitec.Common.Widgets.Design
{
	using System.Collections;
	
	/// <summary>
	/// La classe WidgetWrapper encapsule un Widget de manière à le rendre
	/// éditable.
	/// </summary>
	public class WidgetWrapper
	{
		
		public WidgetWrapper()
		{
		}
		
		public void Attach(Widget widget)
		{
			if (this.widget == widget)
			{
				return;
			}
			
			if (this.widget != null)
			{
				this.widget.Invalidate ();
				
				this.widget.PaintForeground     -= new PaintEventHandler (HandleWidgetPaintForeground);
				this.widget.PreProcessing       -= new MessageEventHandler (this.HandleWidgetPreProcessing);
				this.widget.PaintBoundsCallback -= new PaintBoundsCallback (HandlePaintBoundsCallback);
				
				foreach (Widget parent in this.ancestors)
				{
					parent.PaintBoundsCallback -= new PaintBoundsCallback (HandlePaintBoundsCallback);
				}
				
				this.ancestors.Clear ();
			}
			
			this.widget = widget;
			
			if (this.widget != null)
			{
				this.widget.PaintForeground     += new PaintEventHandler (HandleWidgetPaintForeground);
				this.widget.PreProcessing       += new MessageEventHandler (this.HandleWidgetPreProcessing);
				this.widget.PaintBoundsCallback += new PaintBoundsCallback (HandlePaintBoundsCallback);
				
				Widget parent = widget.Parent;
				
				while (parent != null)
				{
					this.ancestors.Add (parent);
					parent.PaintBoundsCallback += new PaintBoundsCallback (HandlePaintBoundsCallback);
					parent = parent.Parent;
				}
				
				this.widget.Invalidate ();
			}
		}
		
		
		public Widget					Widget
		{
			get { return this.widget; }
		}
		
		public bool						GripsEnabled
		{
			get { return this.grips_enabled; }
			set { this.grips_enabled = value; }
		}
		
		
		private void HandleWidgetPreProcessing(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.widget == sender);
			
			e.Suppress |= this.ProcessMessage (e.Message);
		}
		
		
		internal bool ProcessMessage(Message message)
		{
			if (message.IsMouseType)
			{
				Drawing.Point mouse = this.widget.MapRootToClient (message.Cursor);
				
				switch (message.Type)
				{
					case MessageType.MouseEnter:
						this.mouse_cursor = MouseCursor.AsSizeAll;
						break;
					case MessageType.MouseLeave:
						this.mouse_cursor = MouseCursor.Default;
						break;
					
					case MessageType.MouseDown:
						this.is_dragging = true;
						break;
					
					case MessageType.MouseUp:
						this.is_dragging = false;
						break;
					
					case MessageType.MouseMove:
						break;
				}
				
				this.widget.Window.MouseCursor = this.mouse_cursor;
			}
			
			message.ForceCapture = this.is_dragging;
			message.Consumer = this.widget;
			
			return true;
		}
		
		
		private void HandleWidgetPaintForeground(object sender, PaintEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.widget == sender);
			
			if (this.grips_enabled)
			{
				Drawing.Graphics  graphics = e.Graphics;
				Drawing.Rectangle bounds   = this.widget.Client.Bounds;
				
				bounds.Inflate (0, 0);
				
				graphics.AddRectangle (bounds);
				graphics.AddFilledRectangle (bounds.Left  - 2, bounds.Bottom - 2, 4, 4);
				graphics.AddFilledRectangle (bounds.Left  - 2, bounds.Top    - 2, 4, 4);
				graphics.AddFilledRectangle (bounds.Right - 2, bounds.Bottom - 2, 4, 4);
				graphics.AddFilledRectangle (bounds.Right - 2, bounds.Top    - 2, 4, 4);
				
				graphics.RenderSolid (Drawing.Color.FromARGB (0.5, 1, 0, 0));
			}
		}
		
		private void HandlePaintBoundsCallback(Widget widget, ref Drawing.Rectangle bounds)
		{
			bounds.Inflate (3, 3);
		}
		
		
		
		protected bool					is_dragging;
		protected bool					grips_enabled;
		
		protected MouseCursor			mouse_cursor = MouseCursor.Default;
		
		protected Widget				widget;
		protected ArrayList				ancestors = new ArrayList ();
	}
}
