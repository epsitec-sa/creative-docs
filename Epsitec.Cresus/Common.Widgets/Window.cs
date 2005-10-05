//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	using Win32Api = Epsitec.Common.Widgets.Platform.Win32Api;
	
	/// <summary>
	/// La classe Window représente une fenêtre du système d'exploitation. Ce
	/// n'est pas un widget en tant que tel: Window.Root définit le widget à la
	/// racine de la fenêtre.
	/// </summary>
	public class Window : System.IDisposable, Support.Data.IContainer, Support.ICommandDispatcherHost, Support.Data.IPropertyProvider
	{
		public Window()
		{
			this.Initialise (new WindowRoot (this));
		}
		
		internal Window(WindowRoot root)
		{
			this.Initialise (root);
		}
		
		
		private void Initialise(WindowRoot root)
		{
			this.CommandDispatcher = Support.CommandDispatcher.Default;
			this.components        = new Support.Data.ComponentCollection (this);
			
			this.root   = root;
			this.window = new Platform.Window (this);
			this.timer  = new Timer ();
			
			this.root.Size = new Drawing.Size (this.window.ClientSize);
			this.root.Name = "Root";
			this.root.MinSizeChanged += new EventHandler (this.HandleRootMinSizeChanged);
			
			this.timer.TimeElapsed += new EventHandler (this.HandleTimeElapsed);
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
		
		
		public static void InvalidateAll(Window.InvalidateReason reason)
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
					WindowRoot root = target.Root;
					
					switch (reason)
					{
						case InvalidateReason.Generic:
							root.Invalidate ();
							break;
						
						case InvalidateReason.AdornerChanged:
							root.NotifyAdornerChanged ();
							root.Invalidate ();
							break;
						
						case InvalidateReason.CultureChanged:
							root.NotifyCultureChanged ();
							break;
					}
					
					i++;
				}
			}
		}
		
		
		public static void GrabScreen(Drawing.Image bitmap, int x, int y)
		{
			Win32Api.GrabScreen (bitmap, x, y);
		}
		
		public static Window FindFirstLiveWindow()
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
					return target;
				}
			}
			
			return null;
		}
		
		public static Window FindFromText(string text)
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
					if (target.Text == text)
					{
						return target;
					}
					
					i++;
				}
			}
			
			return null;
		}
		
		public static Window FindFromHandle(System.IntPtr handle)
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
					if (target.window.Handle == handle)
					{
						return target;
					}
					
					i++;
				}
			}
			
			return null;
		}
		
		public static Window FindFromName(string name)
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
					if (target.Name == name)
					{
						return target;
					}
					
					i++;
				}
			}
			
			return null;
		}
		
		
		public static void PumpEvents()
		{
			System.Windows.Forms.Application.DoEvents ();
		}
		
		
		public void MakeTopLevelWindow()
		{
			this.window.MakeTopLevelWindow ();
		}
		
		public void MakeFramelessWindow()
		{
			this.window.MakeFramelessWindow ();
		}
		
		public void MakeLayeredWindow()
		{
			this.window.IsLayered = true;
		}
		
		public void MakeFixedSizeWindow()
		{
			this.window.MakeFixedSizeWindow ();
		}
		
		public void MakeButtonlessWindow()
		{
			this.window.MakeButtonlessWindow ();
		}
		
		public void MakeSecondaryWindow()
		{
			this.window.MakeSecondaryWindow ();
		}
		
		public void MakeToolWindow()
		{
			this.window.MakeToolWindow ();
		}
		
		public void MakeFloatingWindow()
		{
			this.window.MakeFloatingWindow ();
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
			this.AsyncValidation ();
			
			if (this.show_count == 0)
			{
				this.show_count++;
				this.root.InternalUpdateGeometry ();
				this.root.Invalidate ();
			}
			
			this.window.ShowWindow ();
		}
		
		public void ShowDialog()
		{
			this.AsyncValidation ();
			
			if (this.show_count == 0)
			{
				this.show_count++;
				this.root.InternalUpdateGeometry ();
			}
			
			this.window.ShowDialogWindow ();
		}
		
		public void Hide()
		{
			this.window.Hide ();
		}
		
		public void Close()
		{
			this.window.Close ();
		}
		
		public void SynchronousRepaint()
		{
			if (this.window != null)
			{
				this.window.SynchronousRepaint ();
			}
		}
		
		public void AnimateShow(Animation animation)
		{
			this.window.AnimateShow (animation, this.WindowBounds);
		}
		
		public void AnimateShow(Animation animation, Drawing.Rectangle bounds)
		{
			this.window.AnimateShow (animation, bounds);
		}

		public void AnimateHide(Animation animation)
		{
			this.window.AnimateHide (animation, this.WindowBounds);
		}
		
		
		
		public WindowRoot						Root
		{
			get { return this.root; }
		}
		
		public Window							Owner
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
						this.window.Owner = null;
						this.window.Owner = this.owner.window;
					}
				}
			}
		}
		
		
		public Widget							FocusedWidget
		{
			get
			{
				return this.focused_widget;
			}
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
					
					this.OnFocusedWidgetChanged ();
				}
			}
		}
		
		public Widget							EngagedWidget
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
						
						if (new_engage.AutoRepeat)
						{
							this.timer.Stop ();
							this.timer.AutoRepeat = SystemInformation.InitialKeyboardDelay;
							this.timer.Start ();
						}
					}
				}
			}
		}
		
		public Widget							CapturingWidget
		{
			get
			{
				return this.capturing_widget;
			}
		}
		
		public IPaintFilter						PaintFilter
		{
			get
			{
				return this.paint_filter;
			}
			set
			{
				this.paint_filter = value;
			}
		}
		
		public MouseCursor						MouseCursor
		{
			set
			{
				this.window.Cursor = (value == null) ? null : value.GetPlatformCursor ();
				this.window_cursor = value;
			}
		}
		
		
		public bool								IsSyncPaintDisabled
		{
			get
			{
				return (this.window == null) || (this.window.PreventSyncPaint);
			}
		}

		public bool								IsVisible
		{
			get
			{
				if ((this.window != null) &&
					(this.window.Visible))
				{
					return true;
				}
				
				return false;
			}
		}
		
		public bool								IsActive
		{
			get
			{
				return this.window.IsActive;
			}
		}
		
		public bool								IsFrozen
		{
			get { return (this.window == null) || this.window.IsFrozen; }
		}
		
		public bool								IsDisposed
		{
			get { return (this.window == null); }
		}
		
		public bool								IsFocused
		{
			get { return this.window.Focused; }
		}
		
		public bool								IsOwned
		{
			get
			{
				return (this.owner != null);
			}
		}
		
		public bool								IsFullScreen
		{
			get
			{
				return (this.window != null) && (this.window.WindowState == System.Windows.Forms.FormWindowState.Maximized);
			}
			set
			{
				if (this.window != null)
				{
					this.window.WindowState = value ? System.Windows.Forms.FormWindowState.Maximized : System.Windows.Forms.FormWindowState.Normal;
				}
			}
		}
		
		public bool								IsMinimized
		{
			get
			{
				return (this.window != null) && (this.window.WindowState == System.Windows.Forms.FormWindowState.Minimized);
			}
		}
		
		public bool								IsToolWindow
		{
			get
			{
				return this.window.IsToolWindow;
			}
		}
		
		public bool								IsSizeMoveInProgress
		{
			get
			{
				return (this.window != null) && (this.window.IsSizeMoveInProgress);
			}
		}
		
		public double							Alpha
		{
			get { return this.window.Alpha; }
			set { this.window.Alpha = value; }
		}
		
		public Drawing.Rectangle				WindowBounds
		{
			get { return this.window.WindowBounds; }
			set { this.window.WindowBounds = value; }
		}
		
		public Drawing.Image					Icon
		{
			get { return this.window.Icon; }
			set { this.window.Icon = value; }
		}
		
		
		internal WindowStyles					WindowStyles
		{
			get
			{
				return this.window.WindowStyles;
			}
			set
			{
				this.window.WindowStyles = value;
			}
		}
		
		internal WindowType						WindowType
		{
			get
			{
				return this.window.WindowType;
			}
			set
			{
				this.window.WindowType = value;
			}
		}
		
		
		#region ICommandDispatcherHost Members
		public Support.CommandDispatcher		CommandDispatcher
		{
			get
			{
				return this.cmd_dispatcher;
			}
			set
			{
				if (this.cmd_dispatcher != value)
				{
					if (this.cmd_dispatcher != null)
					{
						this.cmd_dispatcher.ValidationRuleBecameDirty -= new Support.EventHandler (this.HandleValidationRuleBecameDirty);
					}
					
					this.cmd_dispatcher = value;
					
					if (this.cmd_dispatcher != null)
					{
						this.cmd_dispatcher.ValidationRuleBecameDirty += new Support.EventHandler (this.HandleValidationRuleBecameDirty);
					}
				}
			}
		}
		#endregion
		
		public bool								PreventAutoClose
		{
			get
			{
				return this.window.PreventAutoClose;
			}
			set
			{
				this.window.PreventAutoClose = value;
			}
		}
		
		public bool								PreventAutoQuit
		{
			get
			{
				return this.window.PreventAutoQuit;
			}
			set
			{
				this.window.PreventAutoQuit = value;
			}
		}
		
		
		public bool								IsValidDropTarget
		{
			get
			{
				return this.window.AllowDrop;
			}
			set
			{
				this.window.AllowDrop = value;
			}
		}
		
		
		public static bool						IsApplicationActive
		{
			get { return Platform.Window.IsApplicationActive; }
		}
		
		
		public static int						DebugAliveWindowsCount
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
		
		public static Window[]					DebugAliveWindows
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
		
		public string							DebugWindowHandle
		{
			get
			{
				return this.window == null ? "<null>" : this.window.Handle.ToInt64 ().ToString ("X");
			}
		}
		
		
		public Drawing.Rectangle				PlatformBounds
		{
			get { return new Drawing.Rectangle (this.window.Bounds); }
		}
		
		public Drawing.Point					PlatformLocation
		{
			get { return new Drawing.Point (this.window.Location); }
			set { this.window.Location = new System.Drawing.Point ((int)(value.X + 0.5), (int)(value.Y + 0.5)); }
		}
		
		public Drawing.Size						PlatformSize
		{
			get { return new Drawing.Size (this.window.Size); }
			set { this.window.Size = new System.Drawing.Size ((int)(value.Width + 0.5), (int)(value.Height + 0.5)); }
		}
		
		internal Platform.Window				PlatformWindow
		{
			get { return this.window; }
		}
		
		public object							PlatformWindowObject
		{
			get
			{
				return this.window;
			}
		}
		
		
		public Drawing.Point					WindowLocation
		{
			get { return this.window.WindowLocation; }
			set { this.window.WindowLocation = value; }
		}
		
		public Drawing.Size						WindowSize
		{
			get { return this.window.WindowSize; }
			set { this.window.WindowSize = value; }
		}
		
		public Drawing.Rectangle				WindowPlacementNormalBounds
		{
			get
			{
				if (this.window == null)
				{
					return Drawing.Rectangle.Empty;
				}
				
				return this.window.WindowPlacementNormalBounds;
			}
		}

		[Bundle ("Size")]	public Drawing.Size	ClientSize
		{
			get { return new Drawing.Size (this.window.ClientSize); }
			set
			{
				if (this.window != null)
				{
					Drawing.Size window_size = this.window.WindowSize;
					Drawing.Size client_size = this.ClientSize;
					
					this.window.WindowSize = new Drawing.Size (value.Width - client_size.Width + window_size.Width, value.Height - client_size.Height + window_size.Height);
				}
			}
		}
		
		[Bundle]			public string		Text
		{
			get { return this.text; }
			set { this.window.Text = this.text = value; }
		}

		[Bundle]			public string		Name
		{
			get { return this.name; }
			set { this.window.Name = this.name = value; }
		}
		
		
		#region IPropertyProvider Members
		public string[] GetPropertyNames()
		{
			if (this.property_hash == null)
			{
				return new string[0];
			}
			
			string[] names = new string[this.property_hash.Count];
			this.property_hash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			
			return names;
		}
		
		public void SetProperty(string key, object value)
		{
			if (this.property_hash == null)
			{
				this.property_hash = new System.Collections.Hashtable ();
			}
			
			this.property_hash[key] = value;
		}
		
		public object GetProperty(string key)
		{
			if (this.property_hash != null)
			{
				return this.property_hash[key];
			}
			
			return null;
		}
		
		public bool IsPropertyDefined(string key)
		{
			if (this.property_hash != null)
			{
				return this.property_hash.Contains (key);
			}
			
			return false;
		}
		
		public void ClearProperty(string key)
		{
			if (this.property_hash != null)
			{
				this.property_hash.Remove (key);
			}
		}
		#endregion
		
		#region IContainer Members
		public void NotifyComponentInsertion(Support.Data.ComponentCollection collection, Support.Data.IComponent component)
		{
		}

		public void NotifyComponentRemoval(Support.Data.ComponentCollection collection, Support.Data.IComponent component)
		{
		}

		
		public Support.Data.ComponentCollection	Components
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
			if (!this.is_disposed)
			{
				this.is_disposed = true;
				
				this.Dispose (true);
				System.GC.SuppressFinalize (this);
			}
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine ("Window.Dispose: " + this.Name);
			}
			
			if (disposing)
			{
				if (this.cmd_queue.Count > 0)
				{
					//	Il y a encore des commandes dans la queue d'exécution. Il faut soit les transmettre
					//	à une autre fenêtre encore en vie, soit les exécuter tout de suite.
					
					Window helper = this.Owner;
					
					if (helper == null)
					{
						helper = Window.FindFirstLiveWindow ();
					}
					
					if (helper == null)
					{
						this.DispatchQueuedCommands ();
					}
					else
					{
						while (this.cmd_queue.Count > 0)
						{
							QueueItem item = this.cmd_queue.Dequeue () as QueueItem;
							helper.QueueCommand (item);
						}
					}
				}
				
				this.CommandDispatcher = null;
				
				if (this.root != null)
				{
					this.root.MinSizeChanged -= new EventHandler (this.HandleRootMinSizeChanged);
					this.root.Dispose ();
				}
				
				if (this.window != null)
				{
					Platform.Window[] owned = this.window.FindOwnedWindows ();
					
					for (int i = 0; i < owned.Length; i++)
					{
						owned[i].HostingWidgetWindow.Dispose ();
					}
					
					if (this.window.IsActive && Window.IsApplicationActive)
					{
						//	Si la fenêtre est active au moment de sa destruction, Windows a tendance
						//	à se comporter de manière étrange. On va donc se dépêcher d'activer une
						//	autre fenêtre (le propriétaire fera l'affaire) :
						
						Platform.Window owner = this.window.Owner as Platform.Window;
						
						if (owner != null)
						{
							System.Diagnostics.Debug.WriteLine ("Disposing active window.");
							owner.Activate ();
						}
					}
					
					this.window.ResetHostingWidgetWindow ();
					this.window.Dispose ();
				}
				
				this.timer.TimeElapsed -= new EventHandler (this.HandleTimeElapsed);
				this.timer.Dispose ();
				
				this.timer  = null;
				this.root   = null;
				this.window = null;
				this.owner  = null;
				
				this.last_in_widget   = null;
				this.capturing_widget = null;
				this.focused_widget   = null;
				this.engaged_widget   = null;;
				
				if (this.components.Count > 0)
				{
					Support.Data.IComponent[] components = new Support.Data.IComponent[this.components.Count];
					this.components.CopyTo (components, 0);
					
					//	S'il y a des composants attachés, on les détruit aussi. Si l'utilisateur
					//	ne désire pas que ses composants soient détruits, il doit les détacher
					//	avant de faire le Dispose de la fenêtre !
					
					for (int i = 0; i < components.Length; i++)
					{
						Support.Data.IComponent component = components[i];
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
				
				if (this.WindowDisposed != null)
				{
					this.WindowDisposed (this);
				}
			}
		}
		
		
		internal void ResetWindow()
		{
			this.window = null;
		}
		
		
		protected void OnAsyncNotification()
		{
			if (this.AsyncNotification != null)
			{
				this.AsyncNotification (this);
			}
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

		internal void OnWindowCloseClicked()
		{
			if (this.WindowCloseClicked != null)
			{
				this.WindowCloseClicked (this);
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
		
		internal void OnWindowFocused()
		{
			if (this.WindowFocused != null)
			{
				this.WindowFocused (this);
			}
			if (this.focused_widget != null)
			{
				this.focused_widget.SimulateFocused ();
			}
		}
		
		internal void OnWindowDefocused()
		{
			if (this.WindowDefocused != null)
			{
				this.WindowDefocused (this);
			}
			if (this.focused_widget != null)
			{
				this.focused_widget.SimulateDefocused ();
			}
		}
		
		internal void OnWindowDragEntered(WindowDragEventArgs e)
		{
			if (this.WindowDragEntered != null)
			{
				this.WindowDragEntered (this, e);
			}
		}
		
		internal void OnWindowDragLeft()
		{
			if (this.WindowDragLeft != null)
			{
				this.WindowDragLeft (this);
			}
		}
		
		internal void OnWindowDragDropped(WindowDragEventArgs e)
		{
			if (this.WindowDragDropped != null)
			{
				this.WindowDragDropped (this, e);
			}
		}
		
		internal void OnWindowSizeMoveStatusChanged()
		{
			if (this.WindowSizeMoveStatusChanged != null)
			{
				this.WindowSizeMoveStatusChanged (this);
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
								this.window.FilterKeyMessages = true;
								break;
						}
					}
						
					return true;
				}
			}
			
			return false;
		}
		
		
		protected virtual void OnFocusedWidgetChanged()
		{
			if (this.FocusedWidgetChanged != null)
			{
				this.FocusedWidgetChanged (this);
			}
		}
		
		
		#region QueueItem class
		protected class QueueItem
		{
			public QueueItem(object source, string command, Support.CommandDispatcher dispatcher)
			{
				this.source     = source;
				this.command    = command;
				this.dispatcher = dispatcher;
			}
			
			public QueueItem(Widget source)
			{
				this.source     = source;
				this.command    = source.Command;
				this.dispatcher = source.CommandDispatcher;
			}
			
			
			public object						Source
			{
				get { return this.source; }
			}
			
			public string						Command
			{
				get { return this.command; }
			}
			
			public Support.CommandDispatcher	CommandDispatcher
			{
				get { return this.dispatcher; }
			}
			
			
			protected object					source;
			protected string					command;
			protected Support.CommandDispatcher	dispatcher;
		}
		#endregion
		
		public void QueueCommand(Widget source)
		{
			this.QueueCommand (new QueueItem (source));
		}
		
		public void QueueCommand(object source, string command)
		{
			this.QueueCommand (source, command, this.CommandDispatcher);
		}
		
		public void QueueCommand(object source, string command, Support.CommandDispatcher dispatcher)
		{
			this.QueueCommand (new QueueItem (source, command, dispatcher));
		}
		
		
		protected void QueueCommand(QueueItem item)
		{
			this.cmd_queue.Enqueue (item);
			
			if (this.cmd_queue.Count == 1)
			{
				if (this.window != null)
				{
					this.window.SendQueueCommand ();
				}
			}
			
			if (this.window == null)
			{
				this.DispatchQueuedCommands ();
			}
		}
		
		
		public void AsyncDispose()
		{
			this.is_dispose_queued = true;
			this.window.SendQueueCommand ();
		}
		
		public void AsyncNotify()
		{
			if (this.is_async_notification_queued == false)
			{
				this.is_async_notification_queued = true;
				this.window.SendQueueCommand ();
			}
		}
		
		public void AsyncValidation()
		{
			if (this.pending_validation == false)
			{
				this.pending_validation = true;
				this.window.SendValidation ();
			}
		}
		
		
		private void HandleValidationRuleBecameDirty(object sender)
		{
			this.AsyncValidation ();
		}
		
		
		internal void DispatchQueuedCommands()
		{
			while (this.cmd_queue.Count > 0)
			{
				QueueItem item    = this.cmd_queue.Dequeue () as QueueItem;
				object    source  = item.Source;
				string    command = item.Command;
				
				Support.CommandDispatcher dispatcher = item.CommandDispatcher;
				
				if (dispatcher == null)
				{
					dispatcher = this.CommandDispatcher;
				}
				
				dispatcher.Dispatch (command, source);
			}
			
			if (this.is_dispose_queued)
			{
				this.Dispose ();
			}
			if (this.is_async_notification_queued)
			{
				this.is_async_notification_queued = false;
				this.OnAsyncNotification ();
			}
		}
		
		internal void DispatchValidation()
		{
			this.pending_validation = false;
			Support.CommandDispatcher.SyncAllValidationRules ();
		}
		
		internal void DispatchMessage(Message message)
		{
			this.DispatchMessage (message, null);
		}
		
		internal void DispatchMessage(Message message, Widget root)
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
				
				Drawing.Point pos = message.Cursor;
				
				if (root == null)
				{
					root = this.root;
				}
				else
				{
					pos = root.MapRootToClient (pos);
					pos = root.MapClientToParent (pos);
				}
				
				root.MessageHandler (message, pos);
				
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
		
		
		internal void FocusWidget(Widget consumer)
		{
			if ((consumer != null) &&
				(consumer.IsFocused == false) &&
				(consumer.CanFocus) &&
				(consumer != this.focused_widget))
			{
				//	On va réaliser un changement de focus. Mais pour cela, il faut que le widget
				//	ayant le focus actuellement, ainsi que le widget candidat pour l'obtention du
				//	focus soient d'accord...
				
				if ((this.focused_widget == null) ||
					(this.focused_widget.InternalAboutToLoseFocus (Widget.TabNavigationDir.None, Widget.TabNavigationMode.Passive)))
				{
					Widget focus;
					
					if (consumer.InternalAboutToGetFocus (Widget.TabNavigationDir.None, Widget.TabNavigationMode.Passive, out focus))
					{
						this.FocusedWidget = focus;
					}
				}
			}
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
						
						if (consumer.AutoFocus)
						{
							this.FocusWidget (consumer);
						}
						
						if (message.IsLeftButton)
						{
							if ((consumer.AutoEngage) &&
								(consumer.IsEngaged == false) &&
								(consumer.CanEngage))
							{
								this.EngagedWidget = consumer;
								this.initially_engaged_widget = consumer;
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
							this.initially_engaged_widget = null;
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
							(Message.State.IsSameWindowAsButtonDown) &&
							(this.initially_engaged_widget == consumer) &&
							(consumer.AutoEngage) &&
							(consumer.IsEngaged == false) &&
							(consumer.CanEngage))
						{
							this.EngagedWidget = consumer;
						}
						break;
				}
			}
			else if (! message.Handled)
			{
				Shortcut shortcut = Shortcut.FromMessage (message);
				
				if (shortcut != null)
				{
					if (this.root.ShortcutHandler (shortcut))
					{
						message.Handled   = true;
						message.Swallowed = true;
					}
				}
				
				if ((! message.Handled) &&
					(this.owner != null))
				{
					//	Le message n'a pas été traité. Peut-être qu'il pourrait intéresser la fenêtre
					//	parent; pour l'instant, on poubellise simplement l'événement, car le faire
					//	remonter m'a causé passablement de surprises...
				}
			}
			
			if (message.Swallowed)
			{
				//	Le message a été mangé. Il faut donc aussi manger le message
				//	correspondant si les messages viennent par paire.
						
				switch (message.Type)
				{
					case MessageType.MouseDown:
						this.window.FilterMouseMessages = true;
						this.capturing_widget = null;
						break;
							
					case MessageType.KeyDown:
						this.window.FilterKeyMessages = true;
						break;
				}
			}
		}
		
		internal void RefreshGraphics(Drawing.Graphics graphics, Drawing.Rectangle repaint, Drawing.Rectangle[] strips)
		{
			if (strips.Length > 1)
			{
				//	On doit repeindre toute une série de rectangles :
				
				for (int i = 0; i < strips.Length; i++)
				{
					graphics.Transform = new Drawing.Transform ();
					graphics.ResetClippingRectangle ();
					graphics.SetClippingRectangle (strips[i]);
					
//-					System.Diagnostics.Debug.WriteLine (string.Format ("Strip {0} : {1}", i, strips[i].ToString ()));
					
					this.Root.PaintHandler (graphics, strips[i], this.paint_filter);
				}
				
//-				System.Diagnostics.Debug.WriteLine ("Done");
				
				graphics.Transform = new Drawing.Transform ();
				graphics.ResetClippingRectangle ();
				graphics.SetClippingRectangle (repaint);
			}
			else
			{
				graphics.Transform = new Drawing.Transform ();
				graphics.ResetClippingRectangle ();
				graphics.SetClippingRectangle (repaint);
				
				this.Root.PaintHandler (graphics, repaint, this.paint_filter);
			}
			
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
			int width  = (int) (this.root.RealMinSize.Width + 0.5);
			int height = (int) (this.root.RealMinSize.Height + 0.5);
			
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
				this.transform = graphics.Transform;
			}
			
			public void Paint(Drawing.Graphics graphics)
			{
				graphics.RestoreClippingRectangle (this.clipping);
				graphics.Transform = this.transform;
				
				handler.Paint (graphics, this.repaint);
			}
			
			Window.IPostPaintHandler		handler;
			Drawing.Rectangle				repaint;
			Drawing.Rectangle				clipping;
			Drawing.Transform				transform;
		}
		#endregion
		
		public event EventHandler				AsyncNotification;
		
		public event EventHandler				WindowActivated;
		public event EventHandler				WindowDeactivated;
		public event EventHandler				WindowShown;
		public event EventHandler				WindowHidden;
		public event EventHandler				WindowClosed;
		public event EventHandler				WindowCloseClicked;
		public event EventHandler				WindowAnimationEnded;
		public event EventHandler				WindowFocused;
		public event EventHandler				WindowDefocused;
		public event EventHandler				WindowDisposed;
		public event EventHandler				WindowSizeMoveStatusChanged;
		
		public event EventHandler				FocusedWidgetChanged;
		
		public event WindowDragEventHandler		WindowDragEntered;
		public event EventHandler				WindowDragLeft;
		public event WindowDragEventHandler		WindowDragDropped;
		
		public static event MessageHandler		MessageFilter;
		public static event EventHandler		ApplicationActivated;
		public static event EventHandler		ApplicationDeactivated;
		
		public enum InvalidateReason
		{
			Generic,
			AdornerChanged,
			CultureChanged
		}
		
		private string							name;
		private string							text;
		
		private Platform.Window					window;
		private Window							owner;
		private WindowRoot						root;
		
		private int								show_count;
		private Widget							last_in_widget;
		private Widget							capturing_widget;
		private Widget							focused_widget;
		private Widget							engaged_widget;
		private Widget							initially_engaged_widget;
		private Timer							timer;
		private MouseCursor						window_cursor;
		
		private Support.CommandDispatcher		cmd_dispatcher;
		private System.Collections.Queue		cmd_queue = new System.Collections.Queue ();
		private bool							is_dispose_queued;
		private bool							is_async_notification_queued;
		private bool							is_disposed;
		private bool							pending_validation;
		private IPaintFilter					paint_filter;
		
		private System.Collections.Queue		post_paint_queue = new System.Collections.Queue ();
		private System.Collections.Hashtable	property_hash;
		
		private Support.Data.ComponentCollection components;
		
		static System.Collections.ArrayList		windows = new System.Collections.ArrayList ();
	}
}
