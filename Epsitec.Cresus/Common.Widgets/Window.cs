using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Window représente une fenêtre du système d'exploitation. Ce
	/// n'est pas un widget en tant que tel: Window.Root définit le widget à la
	/// racine de la fenêtre.
	/// </summary>
	public class Window : System.IDisposable, IBundleSupport, IContainer
	{
		public Window()
		{
			this.cmd_dispatcher = Support.CommandDispatcher.Default;
			this.components = new ComponentCollection (this);
			
			this.root   = new WindowRoot (this);
			this.window = new Platform.Window (this);
			this.timer  = new Timer ();
			
			this.root.Size = new Drawing.Size (this.window.ClientSize);
			this.root.Name = "Root";
			this.root.MinSizeChanged += new EventHandler (HandleRootMinSizeChanged);
			
			this.timer.TimeElapsed += new EventHandler(HandleTimeElapsed);
			this.timer.AutoRepeat = 0.050;
			
			Window.windows.Add (new System.WeakReference (this));
		}
		
		public void Run()
		{
			System.Windows.Forms.Application.Run (this.window);
		}
		
		public void Quit()
		{
			System.Windows.Forms.Application.Exit ();
		}
		
		
		public static void InvalidateAll()
		{
			for (int i = 0; i < Window.windows.Count; )
			{
				System.WeakReference weak_ref = Window.windows[i] as System.WeakReference;
				
				Window target = weak_ref.Target as Window;
				
				if ((target == null) || (target.IsDisposed))
				{
					Window.windows.RemoveAt (i);
				}
				else
				{
					target.Root.Invalidate ();
					i++;
				}
			}
		}
		
		
		public void MakeFramelessWindow()
		{
			this.window.MakeFramelessWindow ();
		}
		
		public void MakeLayeredWindow()
		{
			this.window.IsLayered = true;
		}
		
		public void MakeActive()
		{
			if (! this.IsDisposed)
			{
				this.window.Activate ();
			}
		}
		
		public void DisableMouseActivation()
		{
			this.window.IsMouseActivationEnabled = false;
		}
		
		
		public void Show()
		{
			this.window.Show ();
		}
		
		public void Hide()
		{
			this.window.Hide ();
		}
		
		
		public void AnimateShow(Animation animation)
		{
			this.window.AnimateShow (animation, this.WindowBounds);
		}
		
		public void AnimateShow(Animation animation, Drawing.Rectangle bounds)
		{
			this.window.AnimateShow (animation, bounds);
		}

		
		
		public WindowRoot					Root
		{
			get { return this.root; }
		}
		
		public Window						Owner
		{
			get { return this.owner; }
			set
			{
				if (this.owner != value)
				{
					this.owner = value;
					
					if (value == null)
					{
						this.window.Owner = null;
					}
					else
					{
						this.window.Owner = this.owner.window;
					}
				}
			}
		}
		
		
		public Widget						FocusedWidget
		{
			get { return this.focused_widget; }
			set
			{
				if (this.focused_widget != value)
				{
					Widget old_focus = this.focused_widget;
					Widget new_focus = value;
					
					this.focused_widget = null;
					
					if (old_focus != null)
					{
						old_focus.SetFocused (false);
					}
					
					this.focused_widget = new_focus;
					
					if (new_focus != null)
					{
						new_focus.SetFocused (true);
					}
				}
			}
		}
		
		public Widget						EngagedWidget
		{
			get { return this.engaged_widget; }
			set
			{
				if (this.engaged_widget != value)
				{
					Widget old_engage = this.engaged_widget;
					Widget new_engage = value;
					
					this.engaged_widget = null;
					
					if (old_engage != null)
					{
						old_engage.SetEngaged (false);
						this.timer.Stop ();
					}
					
					this.engaged_widget = new_engage;
					
					if (new_engage != null)
					{
						new_engage.SetEngaged (true);
						
						if (new_engage.AutoRepeatEngaged)
						{
							this.timer.Stop ();
							this.timer.AutoRepeat = SystemInformation.InitialKeyboardDelay;
							this.timer.Start ();
						}
					}
				}
			}
		}
		
		public MouseCursor					MouseCursor
		{
			set
			{
				this.window.Cursor = value == null ? null : value.GetPlatformCursor ();
			}
		}
		
		
		public bool							IsFrozen
		{
			get { return (this.window == null) || this.window.IsFrozen; }
		}
		
		public bool							IsDisposed
		{
			get { return (this.window == null); }
		}
		
		public bool							IsFocused
		{
			get { return this.window.Focused; }
		}
		
		public double						Alpha
		{
			get { return this.window.Alpha; }
			set { this.window.Alpha = value; }
		}
		
		public Drawing.Rectangle			WindowBounds
		{
			get { return this.window.WindowBounds; }
			set { this.window.WindowBounds = value; }
		}
		
		public Drawing.Image				Icon
		{
			get { return this.window.Icon; }
			set { this.window.Icon = value; }
		}
		
		
		public Support.CommandDispatcher	CommandDispatcher
		{
			get { return this.cmd_dispatcher; }
			set { this.cmd_dispatcher = value; }
		}
		
		public bool							PreventAutoClose
		{
			get { return this.window.PreventAutoClose; }
			set { this.window.PreventAutoClose = value; }
		}
		
		
		public static bool					IsApplicationActive
		{
			get { return Platform.Window.IsApplicationActive; }
		}
		
		
		public static int					DebugAliveWindowsCount
		{
			get
			{
				int n = 0;
				
				foreach (System.WeakReference weak_ref in Window.windows)
				{
					if (weak_ref.IsAlive)
					{
						n++;
					}
				}
				
				return n;
			}
		}
		
		public static Window[]				DebugAliveWindows
		{
			get
			{
				Window[] windows = new Window[Window.DebugAliveWindowsCount];
				
				int i = 0;
				
				foreach (System.WeakReference weak_ref in Window.windows)
				{
					if (weak_ref.IsAlive)
					{
						windows[i++] = weak_ref.Target as Window;
					}
				}
				
				return windows;
			}
		}
		
		
		public Drawing.Rectangle			PlatformBounds
		{
			get { return new Drawing.Rectangle (this.window.Bounds); }
		}
		
		public Drawing.Point				PlatformLocation
		{
			get { return new Drawing.Point (this.window.Location); }
			set { this.window.Location = new System.Drawing.Point ((int)(value.X + 0.5), (int)(value.Y + 0.5)); }
		}
		
		public Drawing.Size					PlatformSize
		{
			get { return new Drawing.Size (this.window.Size); }
			set { this.window.Size = new System.Drawing.Size ((int)(value.Width + 0.5), (int)(value.Height + 0.5)); }
		}
		
		
		public Drawing.Point				WindowLocation
		{
			get { return this.window.WindowLocation; }
			set { this.window.WindowLocation = value; }
		}
		
		public Drawing.Size					WindowSize
		{
			get { return this.window.WindowSize; }
			set { this.window.WindowSize = value; }
		}
		
		
		
		[Bundle ("size")]	public Drawing.Size		ClientSize
		{
			get { return new Drawing.Size (this.window.ClientSize); }
			set { this.window.ClientSize = new System.Drawing.Size ((int)(value.Width + 0.5), (int)(value.Height + 0.5)); }
		}
		
		[Bundle ("text")]	public string			Text
		{
			get { return this.text; }
			set { this.window.Text = this.text = value; }
		}

		[Bundle ("name")]	public string			Name
		{
			get { return this.name; }
			set { this.window.Name = this.name = value; }
		}
		
		
		#region IBundleSupport Members
		public virtual string				PublicClassName
		{
			get { return "Window"; }
		}
		
		public virtual void RestoreFromBundle(ObjectBundler bundler, ResourceBundle bundle)
		{
			//	Il faut tricher un petit peu ici, car la classe WindowFrame ne fait pas
			//	partie de la hiérarchie dérivée de Widget. Cependant, l'utilisateur ne
			//	doit pas en avoir conscience. On laisse simplement "Root" gérer toute
			//	l'initialisation.
			
			this.Root.Name = this.Name;
			this.Root.RestoreFromBundle (bundler, bundle);
			
			if (bundle.GetFieldType ("icon") == Support.ResourceFieldType.String)
			{
				this.Icon = Support.ImageProvider.Default.GetImage ("res:" + bundle.GetFieldString ("icon"));
			}
		}
		#endregion
		
		#region IContainer Members
		public void NotifyComponentInsertion(ComponentCollection collection, IComponent component)
		{
		}

		public void NotifyComponentRemoval(ComponentCollection collection, IComponent component)
		{
		}

		public ComponentCollection			Components
		{
			get
			{
				return this.components;
			}
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.root != null)
				{
					this.root.MinSizeChanged -= new EventHandler (HandleRootMinSizeChanged);
					this.root.Dispose ();
				}
				
				if (this.window != null)
				{
					this.window.ResetHostingWidgetWindow ();
					this.window.Dispose ();
				}
				
				this.timer.TimeElapsed -= new EventHandler(HandleTimeElapsed);
				this.timer.Dispose ();
				
				this.root   = null;
				this.window = null;
				this.owner  = null;
				
				this.last_in_widget   = null;
				this.capturing_widget = null;
				this.focused_widget   = null;
				this.engaged_widget   = null;;
				
				if (this.components.Count > 0)
				{
					IComponent[] components = new IComponent[this.components.Count];
					this.components.CopyTo (components, 0);
					
					//	S'il y a des composants attachés, on les détruit aussi. Si l'utilisateur
					//	ne désire pas que ses composants soient détruits, il doit les détacher
					//	avant de faire le Dispose de la fenêtre !
					
					for (int i = 0; i < components.Length; i++)
					{
						IComponent component = components[i];
						this.components.Remove (component);
						component.Dispose ();
					}
				}
				
				this.components.Dispose ();
				this.components = null;
				
				if (Message.State.LastWindow == this)
				{
					Message.ClearLastWindow ();
				}
			}
		}
		
		internal void ResetWindow()
		{
			this.window = null;
		}
		
		
		internal void OnWindowActivated()
		{
			if (this.WindowActivated != null)
			{
				this.WindowActivated (this);
			}
		}
		
		internal void OnWindowDeactivated()
		{
			if (this.WindowDeactivated != null)
			{
				this.WindowDeactivated (this);
			}
		}
		
		internal void OnWindowAnimationEnded()
		{
			if (this.WindowAnimationEnded != null)
			{
				this.WindowAnimationEnded (this);
			}
		}
		
		internal void OnWindowShown()
		{
			if (this.WindowShown != null)
			{
				this.WindowShown (this);
			}
		}
		
		internal void OnWindowHidden()
		{
			if (this.WindowHidden != null)
			{
				this.WindowHidden (this);
			}
		}

		internal void OnWindowClosed()
		{
			if (this.WindowClosed != null)
			{
				this.WindowClosed (this);
			}
		}

		internal void OnApplicationActivated()
		{
			if (Window.ApplicationActivated != null)
			{
				Window.ApplicationActivated (this);
			}
		}
		
		internal void OnApplicationDeactivated()
		{
			if (Window.ApplicationDeactivated != null)
			{
				Window.ApplicationDeactivated (this);
			}
		}
		
		
		internal bool FilterMessage(Message message)
		{
			if (Window.MessageFilter != null)
			{
				Window.MessageFilter (this, message);
					
				//	Si le message a été "absorbé" par le filtre, il ne faut en aucun
				//	cas le transmettre plus loin.
					
				if (message.Handled)
				{
					if (message.Swallowed && !this.IsFrozen)
					{
						//	Le message a été mangé. Il faut donc aussi manger le message
						//	correspondant si les messages viennent par paire.
							
						switch (message.Type)
						{
							case MessageType.MouseDown:
								this.window.FilterMouseMessages = true;
								break;
								
							case MessageType.KeyDown:
								//	TODO: prend note qu'il faut manger l'événement
								break;
						}
					}
						
					return true;
				}
			}
			
			return false;
		}
		
		internal void QueueCommand(Widget source)
		{
			System.Diagnostics.Debug.Assert (this.cmd_names.Contains (source) == false, "Cannot queue same command twice");
			
			this.cmd_queue.Enqueue (source);
			this.cmd_names[source] = source.CommandName;
			
			if (this.cmd_queue.Count == 1)
			{
				this.window.SendQueueCommand ();
			}
		}
		
		internal void DispatchQueuedCommands()
		{
			while (this.cmd_queue.Count > 0)
			{
				Widget widget = this.cmd_queue.Dequeue () as Widget;
				string name   = this.cmd_names[widget] as string;
				
				this.cmd_names.Remove (widget);
				
				this.cmd_dispatcher.Dispatch (name, widget);
			}
		}
		
		internal void DispatchMessage(Message message)
		{
			if (this.IsFrozen || (message == null))
			{
				return;
			}
			
			if (message.IsMouseType)
			{
				if (this.capturing_widget == null)
				{
					//	C'est un message souris. Nous allons commencer par vérifier si tous les widgets
					//	encore marqués comme IsEntered contiennent effectivement encore la souris. Si non,
					//	on les retire de la liste en leur signalant qu'ils viennent de perdre la souris.
					
					Widget.UpdateEntered (this, message);
				}
				else
				{
					//	Un widget a capturé les événements souris. Il ne faut donc gérer l'état Entered
					//	uniquement pour ce widget-là.
					
					Widget.UpdateEntered (this, this.capturing_widget, message);
				}
				
				this.last_in_widget = this.DetectWidget (message.Cursor);
			}
			
			message.InWidget = this.last_in_widget;
			message.Consumer = null;
			
			if (this.capturing_widget == null)
			{
				//	La capture des événements souris n'est pas active. Si un widget a le focus, il va
				//	recevoir les événements clavier en priorité (message.FilterOnlyFocused = true).
				//	Dans les autres cas, les événements sont simplement acheminés de widget en widget,
				//	en utilisant une approche en profondeur d'abord.
				
				this.root.MessageHandler (message, message.Cursor);
				
				if (this.IsDisposed) return;
				
				this.window.Capture = false;
			}
			else
			{
				message.FilterNoChildren = true;
				message.Captured         = true;
				
				this.capturing_widget.MessageHandler (message);
			}
			
			if (this.IsDisposed) return;
			
			this.PostProcessMessage (message);
		}
		
		internal void PostProcessMessage(Message message)
		{
			Widget consumer = message.Consumer;
			
			if (message.Type == MessageType.KeyUp)
			{
				if (this.EngagedWidget != null)
				{
					this.EngagedWidget.SimulateReleased ();
					this.EngagedWidget.SimulateClicked ();
					this.EngagedWidget = null;
				}
			}
			
			if (this.capturing_widget != null)
			{
				if (this.capturing_widget.IsVisible == false)
				{
					//	Il faut terminer la capture si le widget n'est plus visible,
					//	sinon on risque de ne plus jamais recevoir d'événements pour
					//	les autres widgets.
					
					this.capturing_widget = null;
					this.window.Capture = false;
				}
			}
			
			if (consumer != null)
			{
				switch (message.Type)
				{
					case MessageType.MouseDown:
						if ((consumer.AutoCapture) ||
							(message.ForceCapture))
						{
							this.capturing_widget = consumer;
							this.window.Capture = true;
						}
						else
						{
							this.window.Capture = false;
						}
						
						if ((consumer.AutoFocus) &&
							(consumer.IsFocused == false) &&
							(consumer.CanFocus))
						{
							this.FocusedWidget = consumer;
						}
						
						if (message.IsLeftButton)
						{
							if ((consumer.AutoEngage) &&
								(consumer.IsEngaged == false) &&
								(consumer.CanEngage))
							{
								this.EngagedWidget = consumer;
							}
						}
						break;
					
					case MessageType.MouseUp:
						this.capturing_widget = null;
						this.window.Capture = false;
						
						if (message.IsLeftButton)
						{
							if ((consumer.AutoToggle) &&
								(consumer.IsEnabled) &&
								(consumer.IsEntered) &&
								(!consumer.IsFrozen))
							{
								//	Change l'état du bouton...
								
								consumer.Toggle ();
							}
							
							this.EngagedWidget = null;
						}
						break;
					
					case MessageType.MouseLeave:
						if (consumer.IsEngaged)
						{
							this.EngagedWidget = null;
						}
						break;
					
					case MessageType.MouseEnter:
						if ((Message.State.IsLeftButton) &&
							(consumer.AutoEngage) &&
							(consumer.IsEngaged == false) &&
							(consumer.CanEngage))
						{
							this.EngagedWidget = consumer;
						}
						break;
				}
			}
			else
			{
				Shortcut shortcut = null;
				
				if (message.Type == MessageType.KeyDown)
				{
					//	Gère les raccourcis clavier générés avec la touche Alt. Ils ne génèrent pas d'événement
					//	KeyPress
				
					KeyCode key_code = message.KeyCodeOnly;
				
					if ((key_code != KeyCode.None) && (message.IsAltPressed))
					{
						shortcut = new Shortcut ();
						shortcut.KeyCode = message.KeyCode | KeyCode.ModifierAlt;
					}
				}
				else if (message.Type == MessageType.KeyPress)
				{
					shortcut = new Shortcut ();
					shortcut.KeyCode = message.KeyCode;
				}
				
				if (shortcut != null)
				{
					message.Handled = this.root.ShortcutHandler (shortcut);
				}
			}
		}
		
		internal void RefreshGraphics(Drawing.Graphics graphics, Drawing.Rectangle repaint)
		{
			graphics.Transform = new Drawing.Transform ();
			graphics.ResetClippingRectangle ();
			graphics.SetClippingRectangle (repaint);
				
			this.Root.PaintHandler (graphics, repaint);
			
			while (this.post_paint_queue.Count > 0)
			{
				Window.PostPaintRecord record = this.post_paint_queue.Dequeue () as Window.PostPaintRecord;
				record.Paint (graphics);
			}
		}
		
		
		protected Widget DetectWidget(Drawing.Point pos)
		{
			Widget child = this.root.FindChild (pos);
			return (child == null) ? this.root : child;
		}
		
		
		public void MarkForRepaint()
		{
			if (this.window != null)
			{
				this.window.MarkForRepaint ();
			}
		}
		
		public void MarkForRepaint(Drawing.Rectangle rect)
		{
			if (this.window != null)
			{
				this.window.MarkForRepaint (rect);
			}
		}
		
		
		public void QueuePostPaintHandler(Window.IPostPaintHandler handler, Drawing.Graphics graphics, Drawing.Rectangle repaint)
		{
			this.post_paint_queue.Enqueue (new Window.PostPaintRecord (handler, graphics, repaint));
		}
		
		
		public Drawing.Point MapWindowToScreen(Drawing.Point point)
		{
			int x = (int)(point.X + 0.5);
			int y = this.window.ClientSize.Height-1 - (int)(point.Y + 0.5);
			
			System.Drawing.Point pt = this.window.PointToScreen (new System.Drawing.Point (x, y));
			
			double xx = this.window.MapFromWinFormsX (pt.X);
			double yy = this.window.MapFromWinFormsY (pt.Y)-1;
			
			return new Drawing.Point (xx, yy);
		}
		
		public Drawing.Point MapScreenToWindow(Drawing.Point point)
		{
			int x = this.window.MapToWinFormsX (point.X);
			int y = this.window.MapToWinFormsY (point.Y)-1;
			
			System.Drawing.Point pt = this.window.PointToClient (new System.Drawing.Point (x, y));
			
			double xx = pt.X;
			double yy = this.window.ClientSize.Height-1 - pt.Y;
			
			return new Drawing.Point (xx, yy);
		}
		
		
		protected void HandleTimeElapsed(object sender)
		{
			if (this.engaged_widget != null)
			{
				this.timer.AutoRepeat = SystemInformation.KeyboardRepeatPeriod;
				
				if (this.engaged_widget.IsEngaged)
				{
					this.engaged_widget.FireStillEngaged ();
					return;
				}
			}
			
			this.timer.Stop ();
		}

		protected void HandleRootMinSizeChanged(object sender)
		{
			int width  = (int) (this.root.MinSize.Width + 0.5);
			int height = (int) (this.root.MinSize.Height + 0.5);
			
			width  += this.window.Size.Width  - this.window.ClientSize.Width;
			height += this.window.Size.Height - this.window.ClientSize.Height;
			
			this.window.MinimumSize = new System.Drawing.Size (width, height);
		}

		
		#region PostPaint related definitions
		public interface IPostPaintHandler
		{
			void Paint(Drawing.Graphics graphics, Drawing.Rectangle repaint);
		}
		
		protected class PostPaintRecord
		{
			public PostPaintRecord(Window.IPostPaintHandler handler, Drawing.Graphics graphics, Drawing.Rectangle repaint)
			{
				this.handler   = handler;
				this.repaint   = repaint;
				this.clipping  = graphics.SaveClippingRectangle ();
				this.transform = graphics.SaveTransform ();
			}
			
			public void Paint(Drawing.Graphics graphics)
			{
				graphics.RestoreClippingRectangle (this.clipping);
				graphics.RestoreTransform (this.transform);
				
				handler.Paint (graphics, this.repaint);
			}
			
			Window.IPostPaintHandler		handler;
			Drawing.Rectangle				repaint;
			Drawing.Rectangle				clipping;
			Drawing.Transform				transform;
		}
		#endregion
		
		public event EventHandler			WindowActivated;
		public event EventHandler			WindowDeactivated;
		public event EventHandler			WindowShown;
		public event EventHandler			WindowHidden;
		public event EventHandler			WindowClosed;
		public event EventHandler			WindowAnimationEnded;
		
		public static event MessageHandler	MessageFilter;
		public static event EventHandler	ApplicationActivated;
		public static event EventHandler	ApplicationDeactivated;
		
		
		private string						name;
		private string						text;
		
		private Platform.Window				window;
		private Window						owner;
		private WindowRoot					root;
		
		private Widget						last_in_widget;
		private Widget						capturing_widget;
		private Widget						focused_widget;
		private Widget						engaged_widget;
		private Timer						timer;
		
		private Support.CommandDispatcher	 cmd_dispatcher;
		private System.Collections.Queue	 cmd_queue = new System.Collections.Queue ();
		private System.Collections.Hashtable cmd_names = new System.Collections.Hashtable ();
		
		private System.Collections.Queue	post_paint_queue = new System.Collections.Queue ();
		
		private ComponentCollection			components;
		
		static System.Collections.ArrayList	windows = new System.Collections.ArrayList ();
	}
}
