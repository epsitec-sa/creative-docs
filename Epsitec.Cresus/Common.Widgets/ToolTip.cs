namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ToolTip implémente les "info bulles".
	/// </summary>
	public class ToolTip : Widget
	{
		public ToolTip()
		{
			this.colorBack  = Drawing.Color.FromName("Info");
			this.colorFrame = Drawing.Color.FromName("Black");

			this.window = new Window();
			this.window.MakeFramelessWindow();
			this.window.DisableMouseActivation();
			this.window.Root.Children.Add(this);
			this.window.Hide();

			this.timer = new Timer();
			this.timer.TimeElapsed += new EventHandler(this.HandleTimerTimeElapsed);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.timer.TimeElapsed -= new EventHandler(this.HandleTimerTimeElapsed);
				this.timer.Dispose();
				this.timer = null;
			}
			
			base.Dispose(disposing);
		}

		// Utilisé par un widget pour spécifier son texte.
		public void SetToolTip(Widget widget, string text)
		{
			if ( this.hash.Contains(widget) == false )
			{
				if ( text == null )
				{
					this.hash.Remove(widget);
					widget.Entered -= new MessageEventHandler(this.HandleWidgetEntered);
					widget.Exited  -= new MessageEventHandler(this.HandleWidgetExited);
					return;
				}
				widget.Entered += new MessageEventHandler(this.HandleWidgetEntered);
				widget.Exited  += new MessageEventHandler(this.HandleWidgetExited);
			}

			this.hash[widget] = text;
		}

		// La souris entre dans un widget.
		private void HandleWidgetEntered(object sender, MessageEventArgs e)
		{
			this.widget = sender as Widget;
			this.timer.Suspend();
			this.timer.Delay = this.openDelay;
			this.timer.Start();
		}

		// La souris sort d'un widget.
		private void HandleWidgetExited(object sender, MessageEventArgs e)
		{
			this.timer.Stop();
			this.HideToolTip();
			this.widget = null;
		}

		// Le timer arrive à zéro.
		private void HandleTimerTimeElapsed(object sender)
		{
			if ( !this.isVisible )  // pas encore visible ?
			{
				this.ShowToolTip();
				this.timer.Suspend();
				this.timer.Delay = this.closeDelay;
				this.timer.Start();
			}
			else	// déjà visible ?
			{
				this.HideToolTip();
			}
		}

		// Montre le tooltip en fonction du widget visé.
		protected void ShowToolTip()
		{
			if ( this.widget == null )  return;

			this.Text = (string)this.hash[this.widget];
			Drawing.Size size = this.textLayout.SingleLineSize();
			size.Width  += this.margin.X*2;
			size.Height += this.margin.Y*2;

			Drawing.Point mouse = Message.State.LastWindow.MapWindowToScreen(Message.State.LastPosition);
			mouse += this.offset;
			mouse.Y -= size.Height;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Size = size;
			rect.Location = mouse;
			this.window.WindowBounds = rect;

			this.Location = new Drawing.Point(0, 0);
			this.Size = size;
			this.textLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
			this.Invalidate();

			this.window.Show();
			this.isVisible = true;
		}

		// Cache le tooltip.
		protected void HideToolTip()
		{
			this.window.Hide();
			this.isVisible = false;
		}

		// Dessine le tooltip.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			Drawing.Point     pos   = new Drawing.Point(0, 0);
			
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.colorBack);  // fond jaune pale
			
			Drawing.Rectangle rInside = rect;
			rInside.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rInside);
			graphics.RenderSolid(this.colorFrame);  // cadre noir

			pos.X += this.margin.X;  // à cause du Drawing.ContentAlignment.MiddleLeft
			adorner.PaintGeneralTextLayout(graphics, pos, this.textLayout, state, dir);
		}


		protected Window				window;
		protected bool					isVisible = false;
		protected Timer					timer;
		protected double				openDelay = 1;  // délai d'ouverture en secondes
		protected double				closeDelay = 5;  // délai de fermeture en secondes
		protected Widget				widget;
		protected Drawing.Point			mouse;
		protected Drawing.Point			margin = new Drawing.Point(3, 2);
		protected Drawing.Point			offset = new Drawing.Point(8, -16);
		protected Drawing.Color			colorBack;
		protected Drawing.Color			colorFrame;
		protected System.Collections.Hashtable	hash = new System.Collections.Hashtable();
	}
}
