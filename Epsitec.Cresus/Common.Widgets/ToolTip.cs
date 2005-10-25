//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public enum ToolTipBehaviour
	{
		Normal,							//	presque comme Windows
		FollowMouse,					//	suit la souris
		Manual,							//	position définie manuellement
	}

	/// <summary>
	/// La classe ToolTip implémente les "info bulles".
	/// </summary>
	public class ToolTip : Support.Data.IComponent
	{
		public ToolTip()
		{
			this.window = new Window ();
			this.window.MakeFramelessWindow ();
			this.window.MakeFloatingWindow ();
			this.window.Name = "ToolTip";
			this.window.DisableMouseActivation ();
			this.window.WindowBounds = new Drawing.Rectangle (0, 0, 8, 8);
			
			this.timer = new Timer ();
			this.timer.TimeElapsed += new Support.EventHandler (this.HandleTimerTimeElapsed);
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
				return this.initial_pos;
			}
			set
			{
				this.initial_pos = value;
			}
		}
		
		
		public static ToolTip					Default
		{
			get
			{
				return ToolTip.default_tool_tip;
			}
		}


		public void ShowToolTipForWidget(Widget widget)
		{
			if (this.hash.Contains (widget))
			{
				this.timer.Stop ();
				this.HideToolTip ();
				this.AttachToWidget (widget);
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
		
		
		public void UpdateManualToolTip(Drawing.Point mouse, string caption)
		{
			if (this.behaviour == ToolTipBehaviour.Manual)
			{
				this.ShowToolTip (mouse, caption);
			}
		}
		
		public void UpdateManualToolTip(Drawing.Point mouse, Widget caption)
		{
			if (this.behaviour == ToolTipBehaviour.Manual)
			{
				this.ShowToolTip (mouse, caption);
			}
		}
		
		
		public void SetToolTip(Widget widget, string caption)
		{
			this.DefineToolTip (widget, caption);
		}
		
		public void SetToolTip(Widget widget, Widget caption)
		{
			this.DefineToolTip (widget, caption);
		}
		
		
		private void DefineToolTip(Widget widget, object caption)
		{
			if (this.hash == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (widget != null);
			
			if ((this.hash.Contains (widget)) &&
				(caption == null))
			{
				if (this.widget == widget)
				{
					this.DetachFromWidget (this.widget);
				}
				
				this.hash.Remove (widget);
				
				widget.Entered  -= new MessageEventHandler (this.HandleWidgetEntered);
				widget.Exited   -= new MessageEventHandler (this.HandleWidgetExited);
				widget.Disposed -= new Support.EventHandler (this.HandleWidgetDisposed);
			}
			
			if (caption == null)
			{
				return;
			}
			
			if (this.hash.Contains (widget) == false)
			{
				widget.Entered   += new MessageEventHandler (this.HandleWidgetEntered);
				widget.Exited    += new MessageEventHandler (this.HandleWidgetExited);
				widget.Disposed  += new Support.EventHandler (this.HandleWidgetDisposed);
			}
			
			if (this != ToolTip.default_tool_tip)
			{
				if (this.owner == null)
				{
					this.owner = widget.Window;
					
					if (this.owner != null)
					{
						//	C'est la première fois que le widget auquel nous nous attachons
						//	possède une fenêtre valide. On va donc s'enregistrer auprès de
						//	la fenêtre en tant que IComponent; ça permet de garantir que
						//	lorsque la fenêtre est détruite, le ToolTip l'est aussi...
						
						this.owner.Components.Add (this);
					}
				}
			}
			
			this.hash[widget] = caption;
			
			if ((this.widget == widget) &&
				(this.is_displayed))
			{
				this.caption = caption;
				this.ShowToolTip (this.birth_pos, this.caption);
			}
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Widget[] widgets = new Widget[this.hash.Count];
				this.hash.Keys.CopyTo (widgets, 0);
				
				foreach (Widget widget in widgets)
				{
					this.DefineToolTip (widget, null);
				}
				
				System.Diagnostics.Debug.Assert (this.hash.Count == 0);
				
				if (this.widget != null)
				{
					this.DetachFromWidget (this.widget);
				}
				
				this.hash   = null;
				this.widget = null;
				this.owner  = null;
				
				if (this.window != null)
				{
					this.window.Dispose ();
					this.window = null;
				}
				
				if (this.timer != null)
				{
					this.timer.TimeElapsed -= new Support.EventHandler (this.HandleTimerTimeElapsed);
					this.timer.Dispose();
					this.timer = null;
				}
				
				if (this.Disposed != null)
				{
					this.Disposed (this);
					this.Disposed = null;
				}
			}
		}
		
		
		private void AttachToWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (this.widget == null);
			System.Diagnostics.Debug.Assert (widget != null);
			
			this.widget  = widget;
			this.caption = this.hash[this.widget];
			this.host_provided_caption = null;
			
			this.widget.PreProcessing += new MessageEventHandler (this.HandleWidgetPreProcessing);
		}
		
		private void DetachFromWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (this.widget == widget);
			System.Diagnostics.Debug.Assert (this.widget != null);
			
			this.widget.PreProcessing -= new MessageEventHandler (this.HandleWidgetPreProcessing);
			
			this.widget  = null;
			this.caption = null;
		}
		
		
		private void RestartTimer(double delay)
		{
			this.timer.Suspend ();
			this.timer.Delay = delay;
			this.timer.Start();
		}
		
		
		private bool ProcessToolTipHost(Helpers.IToolTipHost host, Drawing.Point pos)
		{
			if (host != null)
			{
				object caption = host.GetToolTipCaption (pos);
				
				if (caption == this.host_provided_caption)
				{
					return true;
				}
				
				if (this.host_provided_caption == null)
				{
					this.RestartTimer (SystemInformation.ToolTipShowDelay);
				}
				else if (caption == null)
				{
					this.timer.Stop ();
					this.HideToolTip ();
				}
				
				this.host_provided_caption = caption;
				this.caption = caption;
				
				if ((caption != null) &&
					(this.is_displayed))
				{
					Drawing.Point mouse = Helpers.VisualTree.MapVisualToScreen (this.widget, pos);
					this.ShowToolTip (mouse, caption);
				}
				
				return true;
			}
			
			return false;
		}
		
		
		private void HandleWidgetEntered(object sender, MessageEventArgs e)
		{
			this.AttachToWidget (sender as Widget);
			
			System.Diagnostics.Debug.Assert (this.widget != null);
			
			if (this.ProcessToolTipHost (this.widget as Helpers.IToolTipHost, e.Point))
			{
				return;
			}
			
			if (this.behaviour != ToolTipBehaviour.Manual)
			{
				this.RestartTimer (SystemInformation.ToolTipShowDelay);
			}
		}

		private void HandleWidgetExited(object sender, MessageEventArgs e)
		{
			if ( this.behaviour != ToolTipBehaviour.Manual )
			{
				this.timer.Stop();
				this.HideToolTip();
				this.DetachFromWidget (this.widget);
			}
		}
		
		private void HandleWidgetPreProcessing(object sender, MessageEventArgs e)
		{
			if ((e.Message.Type == MessageType.MouseMove) &&
				(this.ProcessToolTipHost (this.widget as Helpers.IToolTipHost, e.Point)))
			{
				return;
			}

			if ((this.is_displayed) &&
				(e.Message.Type == MessageType.MouseMove))
			{
				Drawing.Point mouse = Helpers.VisualTree.MapVisualToScreen (this.widget, e.Point);
				
				switch (this.behaviour)
				{
					case ToolTipBehaviour.Normal:
						if (Drawing.Point.Distance (mouse, this.birth_pos) > ToolTip.hide_distance)
						{
							this.timer.Stop ();
							this.HideToolTip ();
							this.RestartTimer (SystemInformation.ToolTipShowDelay);
						}
						break;

					case ToolTipBehaviour.FollowMouse:
						mouse   += ToolTip.offset;
						mouse.Y -= this.window.Root.Height;
						this.window.WindowLocation = mouse;
						break;
						
					case ToolTipBehaviour.Manual:
						break;
				}
			}
		}
		
		private void HandleWidgetDisposed(object sender)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.Assert(this.widget != widget);
			System.Diagnostics.Debug.Assert(this.hash.Contains(widget));
			
			this.DefineToolTip (widget, null);
		}
		
		private void HandleTimerTimeElapsed(object sender)
		{
			if (this.is_displayed)
			{
				this.HideToolTip ();
				System.Diagnostics.Debug.Assert (this.is_displayed == false);
			}
			else
			{
				this.ShowToolTip ();
				this.RestartTimer (SystemInformation.ToolTipAutoCloseDelay);
			}
		}
		
		
		private void ShowToolTip()
		{
			if (this.widget != null)
			{
				this.birth_pos = (this.behaviour == ToolTipBehaviour.Manual)
					/**/	   ? this.initial_pos
					/**/	   : Message.State.LastWindow.MapWindowToScreen (Message.State.LastPosition);
				
				this.ShowToolTip (this.birth_pos, this.caption);
			}
		}
		
		private void ShowToolTip(Drawing.Point mouse, object caption)
		{
			Widget tip = null;
			
			if (caption is string)
			{
				tip = new Contents ();
				
				tip.Text                 = caption as string;
				tip.TextLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
				
				Drawing.Size size = tip.TextLayout.SingleLineSize;
				
				double dx = size.Width + ToolTip.margin.X * 2;
				double dy = size.Height + ToolTip.margin.Y * 2;
				
				tip.Bounds = new Drawing.Rectangle (0, 0, dx, dy);
			}
			else if (caption is Widget)
			{
				tip = caption as Widget;
			}
			else
			{
				throw new System.InvalidOperationException ("Caption neither a string nor a widget");
			}
			
			if (this.behaviour != ToolTipBehaviour.Manual)
			{
				mouse += ToolTip.offset;
			}
			
			mouse.Y -= tip.Height;
			
			//	Modifie la position du tool-tip pour qu'il ne dépasse pas de l'écran.
			
			Drawing.Rectangle wa = ScreenInfo.Find (mouse).WorkingArea;
			
			if (mouse.Y < wa.Bottom)
			{
				mouse.Y = wa.Bottom;
			}
			if (mouse.X + tip.Width > wa.Right)  // dépasse à droite ?
			{
				mouse.X = wa.Right - tip.Width;
			}
			
			this.window.WindowBounds = new Drawing.Rectangle (mouse, tip.Size);
			this.window.Owner        = this.widget.Window;
			
			if (tip.Parent != this.window.Root)
			{
				this.window.Root.Children.Clear ();
				this.window.Root.Children.Add (tip);
			}
			
			if (this.is_displayed == false)
			{
				this.window.Show ();
				this.is_displayed = true;
			}
		}

		private void HideToolTip()
		{
			if (this.is_displayed)
			{
				this.window.Hide ();
				this.is_displayed = false;
			}
		}

		
		#region Contents Class
		public class Contents : Widget
		{
			public Contents()
			{
			}
			
			
			protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
			{
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				
				Drawing.Rectangle rect  = this.Client.Bounds;
				WidgetState       state = this.PaintState;
				Drawing.Point     pos   = new Drawing.Point();
				
				pos.X += ToolTip.margin.X;  // à cause du Drawing.ContentAlignment.MiddleLeft
				
				adorner.PaintTooltipBackground (graphics, rect);
				
				if (this.TextLayout != null)
				{
					adorner.PaintTooltipTextLayout (graphics, pos, this.TextLayout);
				}
				
				base.PaintBackgroundImplementation (graphics, clipRect);
			}
		}
		#endregion
		
		public event Support.EventHandler		Disposed;
		
		protected ToolTipBehaviour				behaviour = ToolTipBehaviour.Normal;
		
		protected Window						owner;
		protected Window						window;
		protected bool							is_displayed;
		protected Timer							timer;
		
		private Widget							widget;
		private object							caption;
		private object							host_provided_caption;
		
		private Drawing.Point					birth_pos;
		private Drawing.Point					initial_pos;
		private System.Collections.Hashtable	hash = new System.Collections.Hashtable();
		
		private static readonly double			hide_distance = 12;
		private static readonly Drawing.Point	margin = new Drawing.Point(3, 2);
		private static readonly Drawing.Point	offset = new Drawing.Point(8, -16);
		
		private static ToolTip					default_tool_tip = new ToolTip ();
	}
}
