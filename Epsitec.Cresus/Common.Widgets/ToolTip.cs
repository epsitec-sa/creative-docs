using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	public enum ToolTipBehaviour
	{
		Normal,							// presque comme Window
		FollowMe,						// suit la souris
		Manual,							// position définie manuellement
	}

	/// <summary>
	/// La classe ToolTip implémente les "info bulles".
	/// </summary>
	[SuppressBundleSupport]
	public class ToolTip : Widget, Support.Data.IComponent
	{
		public ToolTip()
		{
			this.colorBack  = Drawing.Color.FromName("Info");
			this.colorFrame = Drawing.Color.FromName("Black");

			this.window = new Window();
			this.window.MakeFramelessWindow();
			this.window.MakeFloatingWindow();
			this.window.Name = "ToolTip";
			this.window.DisableMouseActivation();
			this.window.WindowBounds = new Drawing.Rectangle (0, 0, 8, 8);
			
			this.SetEmbedder(this.window.Root);
			
			this.timer = new Timer();
			this.timer.TimeElapsed += new EventHandler(this.HandleTimerTimeElapsed);
		}

		
		public ToolTipBehaviour					Behaviour
		{
			get
			{
				return this.behaviour;
			}

			set
			{
				this.behaviour = value;
			}
		}

		public Drawing.Point					InitialLocation
		{
			get
			{
				return this.initialPos;
			}
			set
			{
				this.initialPos = value;
			}
		}
		
		
		public void ShowToolTipForWidget(Widget widget)
		{
			if (this.hash.Contains (widget))
			{
				this.timer.Stop ();
				this.HideToolTip ();
				this.widget = widget;
				this.ShowToolTip ();
			}
		}
		
		public void HideToolTipForWidget(Widget widget)
		{
			if (this.widget == widget)
			{
				this.timer.Stop ();
				this.HideToolTip ();
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				Widget[] widgets = new Widget[this.hash.Count];
				this.hash.Keys.CopyTo (widgets, 0);
				
				foreach (Widget widget in widgets)
				{
					this.SetToolTip(widget, null);
				}
				
				System.Diagnostics.Debug.Assert(this.hash.Count == 0);
				
				this.hash   = null;
				this.widget = null;
				this.owner  = null;
				this.Parent = null;
				
				this.window.Dispose ();
				this.window = null;
				
				this.timer.TimeElapsed -= new EventHandler(this.HandleTimerTimeElapsed);
				this.timer.Dispose();
				this.timer = null;
			}
			
			// Ne pas oublier, une fois que le dispose est terminé, de signaler encore
			// que nous n'existons plus (c'est requis par IComponent). C'est fait par
			// Widget !
			
			base.Dispose(disposing);
		}

		
		// Utilisé par un widget pour spécifier son texte.
		public void SetToolTip(Widget widget, string text)
		{
			if ( this.hash == null )
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert(widget != null);
			
			if ( this.hash.Contains(widget) )
			{
				if ( text == null )
				{
					this.hash.Remove(widget);
					widget.Entered   -= new MessageEventHandler(this.HandleWidgetEntered);
					widget.Exited    -= new MessageEventHandler(this.HandleWidgetExited);
					widget.Disposed  -= new EventHandler(this.HandleWidgetDisposed);
					return;
				}
			}
			else
			{
				if ( text == null )
				{
					return;
				}
				
				widget.Entered   += new MessageEventHandler(this.HandleWidgetEntered);
				widget.Exited    += new MessageEventHandler(this.HandleWidgetExited);
				widget.Disposed  += new EventHandler(this.HandleWidgetDisposed);
			}
			
			if ( this.owner == null )
			{
				this.owner = widget.Window;
				
				if ( this.owner != null )
				{
					// C'est la première fois que le widget auquel nous nous attachons
					// possède une fenêtre valide. On va donc s'enregistrer auprès de
					// la fenêtre en tant que IComponent; ça permet de garantir que
					// lorsque la fenêtre est détruite, le ToolTip l'est aussi...
					
					this.owner.Components.Add(this);
				}
			}
			
			this.hash[widget] = text;
		}

		// La souris entre dans un widget.
		private void HandleWidgetEntered(object sender, MessageEventArgs e)
		{
			this.widget = sender as Widget;
			
			if ( this.filterRegisterCount++ == 0 )
			{
				Window.MessageFilter += new Widgets.MessageHandler(this.MessageFilter);
			}
			if ( this.behaviour != ToolTipBehaviour.Manual )
			{
				this.timer.Suspend();
				this.timer.Delay = SystemInformation.ToolTipShowDelay;
				this.timer.Start();
			}
		}

		// La souris sort d'un widget.
		private void HandleWidgetExited(object sender, MessageEventArgs e)
		{
			if ( --this.filterRegisterCount == 0 )
			{
				Window.MessageFilter -= new Widgets.MessageHandler(this.MessageFilter);
			}
			if ( this.behaviour != ToolTipBehaviour.Manual )
			{
				this.timer.Stop();
				this.HideToolTip();
				this.widget = null;
			}
		}
		
		// Un widget est supprimé -- on doit donc le retirer de notre liste interne.
		private void HandleWidgetDisposed(object sender)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.Assert(this.widget != widget);
			System.Diagnostics.Debug.Assert(this.hash.Contains(widget));
			
			this.SetToolTip(widget, null);
		}
		
		private void MessageFilter(object sender, Message message)
		{
			if ( !this.isVisible )  return;  // pas encore visible ?

			switch ( message.Type )
			{
				case MessageType.MouseMove:
					Drawing.Point mouse = Message.State.LastWindow.MapWindowToScreen(Message.State.LastPosition);

					switch ( this.behaviour )
					{
						case ToolTipBehaviour.Normal:
							double dist = System.Math.Sqrt(System.Math.Pow(mouse.X-this.birthPos.X, 2) + System.Math.Pow(mouse.Y-this.birthPos.Y, 2));
							if ( dist > this.deadDist )
							{
								this.timer.Stop();
								this.HideToolTip();

								this.timer.Delay = SystemInformation.ToolTipShowDelay;
								this.timer.Start();
							}
							break;

						case ToolTipBehaviour.FollowMe:
							mouse += this.offset;
							mouse.Y -= this.Size.Height;
							this.window.WindowLocation = mouse;
							break;
						
						case ToolTipBehaviour.Manual:
							break;
					}

					break;
			}
		}

		// Le timer arrive à zéro.
		private void HandleTimerTimeElapsed(object sender)
		{
			if ( !this.isVisible )  // pas encore visible ?
			{
				this.ShowToolTip();
				this.timer.Suspend();
				this.timer.Delay = SystemInformation.ToolTipAutoCloseDelay;
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
			Drawing.Size size = this.TextLayout.SingleLineSize;
			size.Width  += this.margin.X*2;
			size.Height += this.margin.Y*2;
			
			Drawing.Point mouse;
			
			if ( this.behaviour == ToolTipBehaviour.Manual )
			{
				mouse = this.initialPos;
			}
			else
			{
				mouse = Message.State.LastWindow.MapWindowToScreen(Message.State.LastPosition);
			}
			
			this.birthPos = mouse;
			
			if ( this.behaviour != ToolTipBehaviour.Manual )
			{
				mouse += this.offset;
			}
			
			mouse.Y -= size.Height;
			
			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Size = size;
			rect.Location = mouse;
			this.window.ClientSize = size;
			this.window.WindowBounds = rect;
			this.window.Owner = this.widget.Window;

			this.Location = new Drawing.Point(0, 0);
			this.Size = size;
			this.TextLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
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
			if ( this.TextLayout == null )  return;
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();

			pos.X += this.margin.X;  // à cause du Drawing.ContentAlignment.MiddleLeft
			adorner.PaintTooltipBackground(graphics, rect);
			adorner.PaintTooltipTextLayout(graphics, pos, this.TextLayout);
		}


		public static ToolTip					Default
		{
			get
			{
				return ToolTip.defaultToolTip;
			}
		}


		protected ToolTipBehaviour				behaviour = ToolTipBehaviour.Normal;
		protected Window						owner;
		protected Window						window;
		protected bool							isVisible = false;
		protected Timer							timer;
		protected Widget						widget;
		protected Drawing.Point					birthPos;
		protected Drawing.Point					initialPos;
		protected double						deadDist = 12;
		protected Drawing.Point					margin = new Drawing.Point(3, 2);
		protected Drawing.Point					offset = new Drawing.Point(8, -16);
		protected Drawing.Color					colorBack;
		protected Drawing.Color					colorFrame;
		protected System.Collections.Hashtable	hash = new System.Collections.Hashtable();
		protected int							filterRegisterCount;
		protected static ToolTip				defaultToolTip = new ToolTip();
	}
}
