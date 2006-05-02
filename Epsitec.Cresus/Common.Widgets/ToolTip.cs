//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.window.Root.SyncPaint = true;
			
			this.timer = new Timer ();
			this.timer.TimeElapsed += new Support.EventHandler (this.HandleTimerTimeElapsed);
			
			lock (ToolTip.global_tool_tips)
			{
				ToolTip.global_tool_tips.Add (this);
			}
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


		public static void HideAllToolTips()
		{
			ToolTip[] tips = null;
			
			lock (ToolTip.global_tool_tips)
			{
				tips = (ToolTip[]) ToolTip.global_tool_tips.ToArray (typeof (ToolTip));
			}
			
			foreach (ToolTip tip in tips)
			{
				tip.HideToolTip ();
			}
		}
		
		
		public void ShowToolTipForWidget(Widget widget)
		{
			if (this.hash.Contains (widget))
			{
				this.HideToolTip ();
				this.AttachToWidget (widget);
				this.ShowToolTip ();
			}
		}
		
		public void HideToolTipForWidget(Widget widget)
		{
			if ((this.widget == widget) &&
				(widget != null))
			{
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
				
				lock (ToolTip.global_tool_tips)
				{
					ToolTip.global_tool_tips.Remove (this);
				}
			}
		}
		
		
		private void AttachToWidget(Widget widget)
		{
			if (this.widget != null)
			{
				this.DetachFromWidget (this.widget);
			}
			
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
		
		private void DelayShow()
		{
			System.TimeSpan delta = System.DateTime.Now.Subtract (this.last_change_time);
			
			long   delta_ticks   = delta.Ticks;
			double delta_seconds = (delta_ticks / System.TimeSpan.TicksPerMillisecond) / 1000.0;
			
			if (delta_seconds < SystemInformation.ToolTipShowDelay)
			{
				this.RestartTimer (SystemInformation.ToolTipShowDelay / 10.0);
			}
			else
			{
				this.RestartTimer (SystemInformation.ToolTipShowDelay);
			}
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
					this.DelayShow ();
				}
				else if (caption == null)
				{
					this.HideToolTip ();
				}
				
				this.host_provided_caption = caption;
				this.caption = caption;
				
				if ((caption != null) &&
					(this.is_displayed))
				{
					Drawing.Point mouse = Helpers.VisualTree.MapVisualToScreen (this.widget, pos);
					this.ShowToolTip (mouse, caption);
					this.RestartTimer (SystemInformation.ToolTipAutoCloseDelay);
				}
				
				return true;
			}
			
			return false;
		}
		
		
		private void HandleWidgetEntered(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			
//-			System.Diagnostics.Debug.WriteLine ("HandleWidgetEntered: " + widget.ToString ());
			
			this.AttachToWidget (widget);
			
			System.Diagnostics.Debug.Assert (this.widget != null);
			
			if (this.ProcessToolTipHost (this.widget as Helpers.IToolTipHost, e.Point))
			{
				return;
			}
			
			if (this.behaviour != ToolTipBehaviour.Manual)
			{
				this.DelayShow ();
			}
		}

		private void HandleWidgetExited(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			
			if (this.behaviour != ToolTipBehaviour.Manual)
			{
//-				System.Diagnostics.Debug.WriteLine ("HandleWidgetExited: " + widget.ToString ());
				if (this.widget == widget)
				{
					this.HideToolTip ();
					this.DetachFromWidget (widget);
				}
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
							this.HideToolTip ();
							this.RestartTimer (SystemInformation.ToolTipShowDelay);
						}
						break;

					case ToolTipBehaviour.FollowMouse:
						mouse   += ToolTip.offset;
						mouse.Y -= this.window.Root.ActualHeight;
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
			
			if (this.widget == widget)
			{
				this.DetachFromWidget (widget);
			}
			
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
					/**/	   : Message.CurrentState.LastWindow.MapWindowToScreen (Message.CurrentState.LastPosition);
				
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
				
				tip.PreferredWidth = dx;
				tip.PreferredHeight = dy;
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
			
			mouse.Y -= tip.ActualHeight;
			
			//	Modifie la position du tool-tip pour qu'il ne dépasse pas de l'écran.
			
			Drawing.Rectangle wa = ScreenInfo.Find (mouse).WorkingArea;
			
			if (mouse.Y < wa.Bottom)
			{
				mouse.Y = wa.Bottom;
			}
			if (mouse.X + tip.ActualWidth > wa.Right)  // dépasse à droite ?
			{
				mouse.X = wa.Right - tip.ActualWidth;
			}
			
			this.window.WindowBounds = new Drawing.Rectangle (mouse, tip.ActualSize);
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
			
			this.last_change_time = System.DateTime.Now;
		}

		private void HideToolTip()
		{
			this.timer.Stop ();
			
			if (this.is_displayed)
			{
				this.window.Hide ();
				this.is_displayed = false;
				this.last_change_time = System.DateTime.Now;
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
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				
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
		protected System.DateTime				last_change_time;
		
		private Widget							widget;
		private object							caption;
		private object							host_provided_caption;
		
		private Drawing.Point					birth_pos;
		private Drawing.Point					initial_pos;
		private System.Collections.Hashtable	hash = new System.Collections.Hashtable();
		
		private static readonly double			hide_distance = 24;
		private static readonly Drawing.Point	margin = new Drawing.Point(3, 2);
		private static readonly Drawing.Point	offset = new Drawing.Point(8, -16);
		
		static System.Collections.ArrayList		global_tool_tips = new System.Collections.ArrayList ();
		static ToolTip							default_tool_tip = new ToolTip ();
	}
}
