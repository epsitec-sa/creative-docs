//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// La classe MenuBehavior implémente la logique permettant de gérer un menu
	/// (affichage d'une ou plusieurs fenêtres flottantes, fermeture, etc.)
	/// </summary>
	public sealed class MenuBehavior : Types.Object
	{
		public MenuBehavior()
		{
			//	En principe, on associe à toute une hiérarchie (menu + sous-menus)
			//	un unique MenuBehavior. Dans la pratique, ce n'est pas nécessaire
			//	et seul le MenuBehavior associé à la racine des menus sera utilisé
			//	pour gérer l'ensemble des (sous-)menus.
		}
		
		
		#region Animate Enumeration
		public enum Animate
		{
			Yes, No, Automatic
		}
		#endregion
		
		public static void OpenItemSubmenu(Widget item, Animate animate)
		{
			//	Ouvre le sous-menu d'un MenuItem donné.
			
			if (item == null)
			{
				return;
			}
			
			Widget submenu = MenuItem.GetSubmenu (item);
			
			if (submenu == null)
			{
				//	Il n'y a pas de sous-menu ! Il faut en tout cas montrer le
				//	menu qui contient cet item :
				
				MenuWindow menu = MenuItem.GetMenuWindow (item) as MenuWindow;
				
				if ((menu != null) &&
					(menu.ParentWidget != null))
				{
					MenuBehavior.OpenItemSubmenu (menu.ParentWidget, animate);
				}
				
				return;
			}
			
			MenuBehavior behavior = MenuItem.GetMenuBehavior (submenu);
			MenuWindow   window   = MenuItem.GetMenuWindow (submenu) as MenuWindow;
			
			behavior.OpenSubmenu (window, animate);
			behavior.UpdateItems ();
		}
		
		public static void CloseItemMenu(Widget item)
		{
			MenuBehavior behavior = MenuItem.GetMenuBehavior (item);
			
			if (behavior != null)
			{
				behavior.HideAll ();
			}
		}
		
		
		public void OpenSubmenu(MenuWindow window, Animate animate)
		{
			//	Ouvre le sous-menu spécifié par sa fenêtre. Le sous-menu doit
			//	impérativement appartenir à notre "menu" :
			
			if (window == null)
			{
				//	Il faut cacher toutes les fenêtres encore ouvertes, car ce
				//	cas (window == null) apparaît quand on désire montrer un
				//	menu horizontal déjà visible (racine d'un menu dynamique).
				
				this.HideAll ();
				
				return;
			}
			
			System.Diagnostics.Debug.Assert (window.Behavior == this);
			
			MenuWindow[] live  = (MenuWindow[]) this.live_menu_windows.ToArray (typeof (MenuWindow));
			int          index = this.live_menu_windows.IndexOf (window);
			
			if (index >= 0)
			{
				//	Ce sous-menu est déjà visible : il faut s'assurer que, s'il a
				//	lui-même des sous-menus, on ferme ceux qui doivent être cachés :
				
				for (int i = live.Length-1; i > index; i--)
				{
					live[i].Hide ();
				}
			}
			else
			{
				//	Le sous-menu n'est pas encore visible. Il faut s'assurer que
				//	le parent immédiat est visible avant de montrer le sous-menu :
				
				this.OpenSubmenu (MenuItem.GetMenuWindow (window.ParentWidget) as MenuWindow, animate);
				this.ShowSubmenu (window, animate);
			}
		}
		
		
		public void HideAll()
		{
			while (this.live_menu_windows.Count > 0)
			{
				MenuWindow window = this.live_menu_windows[this.live_menu_windows.Count-1] as MenuWindow;
				window.Hide ();
			}
			
			this.is_open = false;
			this.keyboard_menu_active = false;
		}
		
		public void Reject()
		{
			this.HideAll ();
		}
		
		
		public void Attach(Widget root_menu)
		{
			System.Diagnostics.Debug.Assert (this.root_menu == null);
			
			this.root_menu = root_menu;
			
			if (this.root_menu.IsVisible)
			{
				this.AttachRootWindow ();
			}
			
			this.root_menu.IsVisibleChanged += new PropertyChangedEventHandler (this.HandleRootMenuIsVisibleChanged);
		}
		
		public void Detach(Widget root_menu)
		{
			System.Diagnostics.Debug.Assert (this.root_menu == root_menu);
			
			this.root_menu.IsVisibleChanged -= new PropertyChangedEventHandler (this.HandleRootMenuIsVisibleChanged);
			
			if (this.root_menu.IsVisible)
			{
				this.DetachRootWindow ();
			}
			
			this.root_menu = null;
		}
		
		
		private void ShowSubmenu(MenuWindow window, Animate animate)
		{
			//	Montre un sous-menu en le positionnant au préalable par rapport
			//	à son parent, de la manière la plus judicieuse possible.
			
			Widget       parent = window.ParentWidget;
			Drawing.Size size   = window.Root.GetBestFitSize ();
			
			//	Détermine la position où afficher le menu...
			
			IMenuHost host = parent as IMenuHost;
			
			if (host == null)
			{
				host = MenuItem.GetMenuHost (parent);
			}
			
			Drawing.Point location  = window.WindowLocation;
			Animation     animation = Animation.FadeIn;
			
			if (host != null)
			{
				host.GetMenuDisposition (parent, size, out location, out animation);
			}
			
			switch (animate)
			{
				case Animate.No:
					animation = Animation.None;
					break;
				
				case Animate.Automatic:
					if (this.live_menu_windows.Count > 0)
					{
						animation = Animation.None;
					}
					break;
			}
			
			window.WindowSize     = size;
			window.WindowLocation = location;
			window.AnimateShow (animation);
			
			this.is_open = true;
		}
		
		
		internal void HandleAboutToShowMenuWindow(MenuWindow window)
		{
			//	Appelé avant qu'une fenêtre du menu ne devienne visible. A
			//	partir de ce moment, il faut considérer la fenêtre comme en
			//	cours d'affichage.
			
			MenuBehavior.AttachMenu ();
			
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_list.Remove (this);
				MenuBehavior.menu_list.Add (this);
			}
			
			System.Diagnostics.Debug.Assert (this.live_menu_windows.Contains (window) == false);
			
			this.live_menu_windows.Add (window);
			this.UpdateItems ();
		}
		
		internal void HandleAboutToHideMenuWindow(MenuWindow window)
		{
			//	Appelé avant qu'une fenêtre du menu ne soit cachée.
			
			System.Diagnostics.Debug.Assert (this.live_menu_windows.Contains (window) == true);
			System.Diagnostics.Debug.Assert (this.live_menu_windows.IndexOf (window) == this.live_menu_windows.Count-1);
			
			this.live_menu_windows.Remove (window);
			this.UpdateItems ();
			
			//	Si une fenêtre possède le focus, il faut avant de la cacher activer
			//	la fenêtre parent, pour éviter des clignotements de fenêtres :
			
			if (window.IsFocused)
			{
				Window owner = window.Owner;
				
				if (owner != null)
				{
					owner.MakeActive ();
				}
			}
			
			MenuBehavior.DetachMenu ();
			
			if (this.live_menu_windows.Count == 0)
			{
				lock (MenuBehavior.sync_object)
				{
					MenuBehavior.menu_list.Remove (this);
				}
			}
		}
		
		
		static MenuBehavior()
		{
			MenuBehavior.timer = new Timer ();
			MenuBehavior.timer.TimeElapsed += new Support.EventHandler (MenuBehavior.HandleTimerTimeElapsed);
		}
		
		
		private static void AttachMenu()
		{
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_count++;
				
				if (MenuBehavior.menu_count == 1)
				{
					MenuBehavior.RegisterFilter ();
				}
			}
		}
		
		private static void DetachMenu()
		{
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_count--;
				
				if (MenuBehavior.menu_count == 0)
				{
					MenuBehavior.UnregisterFilter ();
				}
			}
		}
		
		
		private void AttachRootWindow()
		{
			MenuBehavior.AttachMenu ();
			
			this.root_window = this.root_menu.Window;
			
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_root_list.Add (this);
			}
		}
		
		private void DetachRootWindow()
		{
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_root_list.Remove (this);
			}
			
			this.root_window = null;
			
			MenuBehavior.DetachMenu ();
		}
		
		
		private void HandleRootMenuIsVisibleChanged(object sender, PropertyChangedEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.root_menu == sender);
			
			if ((bool) e.NewValue)
			{
				this.AttachRootWindow ();
			}
			else
			{
				this.DetachRootWindow ();
			}
		}
		
		
		private void NotifyEnteredItem(MenuItem item)
		{
			this.keyboard_menu_item   = item;
			this.keyboard_menu_window = item.Window;
			
			if (this.is_open == false)
			{
				this.UpdateItems ();
			}
			else if (MenuItem.GetZeroDelay (item))
			{
				MenuBehavior.timer.Suspend ();
				MenuBehavior.timer_item = null;
				
				MenuBehavior.OpenItemSubmenu (item, Animate.No);
			}
			else
			{
				MenuBehavior.timer_item = item;
				MenuBehavior.timer.Suspend ();
				MenuBehavior.timer.Delay = SystemInformation.MenuShowDelay;
				MenuBehavior.timer.Start ();
				
				this.UpdateItems ();
			}
		}
		
		private void NotifyExitedItem(MenuItem item)
		{
			this.keyboard_menu_item = null;
			
			this.UpdateItems ();
		}
		

		private void KeyboardCloseLatestMenu()
		{
			int n = this.live_menu_windows.Count;
			
			if (n > 0)
			{
				MenuWindow window = this.live_menu_windows[n-1] as MenuWindow;
				
				this.keyboard_menu_item   = window.ParentWidget as MenuItem;
				this.keyboard_menu_window = this.keyboard_menu_item == null ? null : this.keyboard_menu_item.Window;
				this.keyboard_menu_active = true;
				
				window.Hide ();
			}
			else
			{
				this.keyboard_menu_item   = null;
				this.keyboard_menu_window = null;
				this.keyboard_menu_active = false;
				
				this.is_open = false;
				
				MenuBehavior.keyboard_navigation_active = false;
			}
			
			MenuBehavior.menu_last_item     = this.keyboard_menu_item;
			MenuBehavior.menu_last_behavior = this.keyboard_menu_item == null ? null : this;
			
			System.Diagnostics.Debug.WriteLine (this.keyboard_menu_item == null ? "No selected keyboard item" : "Selected " + this.keyboard_menu_item.ToString ());
			
			this.UpdateItems ();
		}
		
		private void KeyboardSelectItem(int direction)
		{
			int    n    = this.live_menu_windows.Count;
			Widget root = this.keyboard_menu_window == null ? null : this.keyboard_menu_window.Root;
			
			if (n > 0)
			{
				MenuItem item = null;
				
				if (direction < 0)
				{
					item = this.FindPrevItem (root, this.keyboard_menu_item);
					
					if (item == null)
					{
						item = this.FindLastItem (root);
					}
				}
				else if (direction > 0)
				{
					item = this.FindNextItem (root, this.keyboard_menu_item);
					
					if (item == null)
					{
						item = this.FindFirstItem (root);
					}
				}
				
				if (item == null)
				{
					n = 0;
				}
				else
				{
					this.keyboard_menu_item = item;
					
					MenuWindow[] live = (MenuWindow[]) this.live_menu_windows.ToArray (typeof (MenuWindow));
					
					for (int i = live.Length-1; i >= 0; i--)
					{
						if (live[i] == this.keyboard_menu_window)
						{
							break;
						}
						
						live[i].Hide ();
					}
				}
			}
			
			if (n == 0)
			{
				//	Aucun (sous-)menu n'est actuellement visible. On va donc
				//	ouvrir celui correspondant à la case sélectionnée :
				
				MenuBehavior.OpenItemSubmenu (this.keyboard_menu_item, Animate.No);
				
				n = this.live_menu_windows.Count;
				
				if (n > 0)
				{
					this.keyboard_menu_window = this.live_menu_windows[n-1] as MenuWindow;
					this.keyboard_menu_item   = this.FindFirstItem (this.keyboard_menu_window.Root);
				}
			}
			
			MenuBehavior.menu_last_item     = this.keyboard_menu_item;
			MenuBehavior.menu_last_behavior = this.keyboard_menu_item == null ? null : this;
			
			System.Diagnostics.Debug.WriteLine (this.keyboard_menu_item == null ? "No selected keyboard item" : "Selected " + this.keyboard_menu_item.ToString ());
			
			this.keyboard_menu_active = true;
			
			this.UpdateItems ();
		}
		
		private void KeyboardSelectMenu(int direction)
		{
			//	Navigation avec les touches gauche/droite, pour passer de menu
			//	en menu, dans le cas d'un menu hiérarchique.
			
			int      n      = this.live_menu_windows.Count;
			MenuItem item   = null;
			Window   window = null;
			
			if ((n > 0) &&
				((this.root_window == this.keyboard_menu_window) || (MenuBehavior.menu_last_item == null)))
			{
				//	Aucune case sélectionnée dans le menu, ou alors c'est une case
				//	du menu principal. Dans ce cas, il faut commencer par sélec-
				//	tionner la première case du premier sous-menu :
				
				MenuWindow menu = this.live_menu_windows[0] as MenuWindow;
				
				item = this.FindFirstItem (menu.Root);
				
				if (item != null)
				{
					this.keyboard_menu_item   = item;
					this.keyboard_menu_window = item.Window;
				}
			}
			
			if (n == 0)
			{
				//	Déplacement horizontal dans le menu de base. Aucun sous-menu
				//	n'est actuellement ouvert. On ne va donc pas ouvrir de menu,
				//	mais juste changer de MenuItem actif :
				
				if (direction < 0)
				{
					item = this.FindPrevItem (this.root_menu, this.keyboard_menu_item);
					
					if (item == null)
					{
						item = this.FindLastItem (this.root_menu);
					}
				}
				else if (direction > 0)
				{
					item = this.FindNextItem (this.root_menu, this.keyboard_menu_item);
					
					if (item == null)
					{
						item = this.FindFirstItem (this.root_menu);
					}
				}
				
				if (item != null)
				{
					window = this.root_window;
				}
			}
			else
			{
				//	Déplacement horizontal avec un ou plusieurs sous-menus ouverts.
				//	En fonction du niveau, il faudra soit ouvrir un sous-menu, soit
				//	passer au menu suivant/précédent :
				
				item = this.keyboard_menu_item;
				
				if (item == null)
				{
					Widget root = this.keyboard_menu_window == null ? null : this.keyboard_menu_window.Root;
					
					item   = this.FindFirstItem (root);
					window = item == null ? null : item.Window;
				}
				else
				{
					if (direction > 0)
					{
						//	Tente d'ouvrir un sous-menu pour l'élément en cours.
						
						Widget submenu = MenuItem.GetSubmenu (item);
						
						if (submenu != null)
						{
							MenuBehavior.OpenItemSubmenu (item, Animate.No);
							
							if (this.live_menu_windows.Count == n+1)
							{
								window = this.live_menu_windows[n] as MenuWindow;
								item   = this.FindFirstItem (window.Root);
							}
						}
						else if (this.root_menu != null)
						{
							//	Il n'y a pas de sous-menu pour l'élément en cours.
							//	Il faut donc fermer ce menu et ouvrir le menu de
							//	base suivant (s'il y en a).
							
							Widget root = this.FindRootItem (item);
							
							if (root != null)
							{
								item = this.FindNextItem (this.root_menu, root);
								
								if (item == null)
								{
									item = this.FindFirstItem (this.root_menu);
								}
								
								if (item != null)
								{
									MenuBehavior.OpenItemSubmenu (item, Animate.No);
									
									if (this.live_menu_windows.Count == 1)
									{
										window = this.live_menu_windows[0] as MenuWindow;
										item   = this.FindFirstItem (window.Root);
									}
								}
							}
						}
					}
					else if (direction < 0)
					{
						//	Si nous sommes actuellement dans un sous-menu, il faut le
						//	fermer et revenir au menu parent, si c'est possible :
						
						Widget parent = this.FindParentItem (item);
						
						if ((parent == null) ||
							(parent.Window == this.root_window))
						{
							//	Il n'y a plus de parent sur lequel on puisse se
							//	rabattre. Montre le menu précédent :
							
							Widget root = this.FindRootItem (item);
							
							if (root != null)
							{
								item = this.FindPrevItem (this.root_menu, root);
								
								if (item == null)
								{
									item = this.FindLastItem (this.root_menu);
								}
								
								if (item != null)
								{
									MenuBehavior.OpenItemSubmenu (item, Animate.No);
									
									if (this.live_menu_windows.Count == 1)
									{
										window = this.live_menu_windows[0] as MenuWindow;
										item   = this.FindFirstItem (window.Root);
									}
								}
							}
						}
						else
						{
							//	Ferme le sous-menu actuellement ouvert et change
							//	le "focus" de place :
							
							window = this.live_menu_windows[n-1] as MenuWindow;
							window.Hide ();
							
							item   = parent as MenuItem;
							window = parent.Window as MenuWindow;
						}
					}
				}
			}
			
			if ((item != null) &&
				(window != null))
			{
				this.keyboard_menu_item   = item;
				this.keyboard_menu_window = window;
				
				MenuBehavior.menu_last_item     = item;
				MenuBehavior.menu_last_behavior = item == null ? null : this;
				
				this.keyboard_menu_active = true;
				
				this.UpdateItems ();
			}
		}
		
		
		private Widget FindRootItem(Widget item)
		{
			MenuWindow window = MenuItem.GetMenuWindow (item) as MenuWindow;
			
			if (window == null)
			{
				return item;
			}
			
			Widget widget = window.ParentWidget;
			
			if (widget == null)
			{
				return item;
			}
			
			return this.FindRootItem (widget);
		}
		
		private Widget FindParentItem(Widget item)
		{
			MenuWindow window = MenuItem.GetMenuWindow (item) as MenuWindow;
			
			if (window == null)
			{
				return null;
			}
			
			return window.ParentWidget;
		}
		
		
		private MenuItem FindFirstItem(Widget root)
		{
			if (root != null)
			{
				Widget[] children = root.FindAllChildren ();
				
				for (int i = 0; i < children.Length; i++)
				{
					if ((children[i] is MenuItem) &&
						(! (children[i] is MenuSeparator)))
					{
						return children[i] as MenuItem;
					}
				}
			}
			
			return null;
		}
		
		private MenuItem FindLastItem(Widget root)
		{
			if (root != null)
			{
				Widget[] children = root.FindAllChildren ();
				
				for (int i = children.Length-1; i >= 0; i--)
				{
					if ((children[i] is MenuItem) &&
						(! (children[i] is MenuSeparator)))
					{
						return children[i] as MenuItem;
					}
				}
			}
			
			return null;
		}
		
		private MenuItem FindNextItem(Widget root, Widget item)
		{
			if (root != null)
			{
				Widget[] children = root.FindAllChildren ();
				bool     found    = false;
				
				for (int i = 0; i < children.Length; i++)
				{
					if (children[i] == item)
					{
						found = true;
					}
					else if ((children[i] is MenuItem) &&
						/**/ (! (children[i] is MenuSeparator)))
					{
						if (found)
						{
							return children[i] as MenuItem;
						}
					}
				}
			}
			
			return null;
		}
		
		private MenuItem FindPrevItem(Widget root, Widget item)
		{
			if (root != null)
			{
				Widget[] children = root.FindAllChildren ();
				bool     found    = false;
				
				for (int i = children.Length-1; i >= 0; i--)
				{
					if (children[i] == item)
					{
						found = true;
					}
					else if ((children[i] is MenuItem) &&
						/**/ (! (children[i] is MenuSeparator)))
					{
						if (found)
						{
							return children[i] as MenuItem;
						}
					}
				}
			}
			
			return null;
		}
		
		
		private void SuspendUpdates()
		{
			this.suspend_updates++;
		}
		
		private void ResumeUpdates()
		{
			System.Diagnostics.Debug.Assert (this.suspend_updates > 0);
			
			this.suspend_updates--;
			
			if (this.suspend_updates == 0)
			{
				if (this.update_requested > 0)
				{
					this.UpdateItems ();
				}
			}
		}
		
		private void UpdateItems()
		{
			if (this.suspend_updates > 0)
			{
				this.update_requested++;
				return;
			}
			
			this.suspend_updates++;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			MenuWindow[] windows = (MenuWindow[]) this.live_menu_windows.ToArray (typeof (MenuWindow));
			int hilite_below_i = -1;
			
			for (int i = 0; i < windows.Length; i++)
			{
				foreach (Widget widget in windows[i].Root.FindAllChildren ())
				{
					if (widget is MenuItem)
					{
						if (widget == MenuBehavior.menu_last_item)
						{
							MenuItem.SetItemType (widget, MenuItemType.Select);
							hilite_below_i = i + 1;
						}
						else
						{
							list.Add (widget);
						}
					}
				}
			}
			
			if (this.root_menu != null)
			{
				foreach (Widget widget in this.root_menu.FindAllChildren ())
				{
					if (widget is MenuItem)
					{
						if (widget == MenuBehavior.menu_last_item)
						{
							MenuItem.SetItemType (widget, MenuItemType.Select);
						}
						else
						{
							list.Add (widget);
						}
					}
				}
			}
			
			if (MenuBehavior.menu_last_item == null)
			{
				hilite_below_i = windows.Length;
			}
			
			for (int i = 0; i < hilite_below_i; i++)
			{
				MenuWindow menu = windows[i];
				
				Widget widget = menu.ParentWidget;
				MenuItem.SetItemType (widget, MenuItemType.Parent);
				list.Remove (widget);
			}
			
			foreach (Widget widget in list)
			{
				MenuItem.SetItemType (widget, MenuItemType.Deselect);
			}
			
			this.suspend_updates--;
			this.update_requested = 0;
		}
		
		
		private static void RegisterFilter()
		{
			Window.ApplicationDeactivated += new Support.EventHandler (MenuBehavior.HandleApplicationDeactivated);
			Window.MessageFilter          += new MessageHandler (MenuBehavior.MessageFilter);
		}
		
		private static void UnregisterFilter()
		{
			Window.ApplicationDeactivated -= new Support.EventHandler (MenuBehavior.HandleApplicationDeactivated);
			Window.MessageFilter          -= new MessageHandler (MenuBehavior.MessageFilter);
		}
		
		
		private static MenuBehavior[] GetActiveMenuBehaviors()
		{
			lock (MenuBehavior.sync_object)
			{
				return (MenuBehavior[]) MenuBehavior.menu_list.ToArray (typeof (MenuBehavior));
			}
		}
		
		private static MenuBehavior[] GetRootMenuBehaviors()
		{
			lock (MenuBehavior.sync_object)
			{
				return (MenuBehavior[]) MenuBehavior.menu_root_list.ToArray (typeof (MenuBehavior));
			}
		}
		
		
		private static long GetNextId()
		{
			lock (MenuBehavior.sync_object)
			{
				return MenuBehavior.next_id++;
			}
		}
		
		
		private static Window DetectWindow(Drawing.Point pos)
		{
			MenuBehavior[] behaviors = MenuBehavior.GetActiveMenuBehaviors ();
			
			for (int i = behaviors.Length-1; i >= 0; i--)
			{
				MenuBehavior behavior = behaviors[i];
				MenuWindow[] windows  = (MenuWindow[]) behavior.live_menu_windows.ToArray (typeof (MenuWindow));
				
				//	Analyse la chaîne des fenêtres correspondant au menu déroulé,
				//	en commençant par la fin (dernier élément affiché = premier à
				//	être considéré) :
				
				for (int j = windows.Length-1; j >= 0; j--)
				{
					MenuWindow window = windows[j];
					
					if ((window != null) &&
						(window.WindowBounds.Contains (pos)))
					{
						return window;
					}
				}
			}
			
			return null;
		}
		
		private static Window DetectRootWindow(Drawing.Point pos)
		{
			MenuBehavior[] behaviors = MenuBehavior.GetRootMenuBehaviors ();
			
			//	Peut-être y a-t-il une fenêtre contenant un menu racine toujours
			//	visible à l'écran ?
			
			for (int i = behaviors.Length-1; i >= 0; i--)
			{
				MenuBehavior behavior = behaviors[i];
				
				if ((behavior.root_window != null) &&
					(behavior.root_window.WindowBounds.Contains (pos)) &&
					(behavior.root_menu.HitTest (behavior.root_menu.MapScreenToParent (pos))))
				{
					return behavior.root_window;
				}
			}
			
			return null;
		}
		
		private static MenuItem DetectMenuItem(Window window, Drawing.Point pos)
		{
			Widget widget = window.Root.FindChild (pos, Widget.ChildFindMode.SkipHidden | Widget.ChildFindMode.Deep | Widget.ChildFindMode.SkipDisabled);
			
			//	Retrouve parmi les widgets parents si l'un d'entre eux est une
			//	instance de MenuItem :
			
			while (widget != null)
			{
				if (widget is MenuItem)
				{
					return widget as MenuItem;
				}
				
				widget = widget.Parent;
			}
			
			return null;
		}
		
		
		private static void RejectAll()
		{
			MenuBehavior[] behaviors = MenuBehavior.GetActiveMenuBehaviors ();
			
			foreach (MenuBehavior behavior in behaviors)
			{
				behavior.Reject ();
			}
		}
		
		private static void NotifyCursorInItem(MenuItem item)
		{
			if (MenuBehavior.menu_last_item == item)
			{
				return;
			}
			
			if ((MenuBehavior.keyboard_navigation_active) &&
				(item == null))
			{
				return;
			}
			
			MenuBehavior.keyboard_navigation_active = false;
			
			if (MenuBehavior.menu_last_item != null)
			{
				MenuItem     last_item = MenuBehavior.menu_last_item;
				MenuBehavior behavior  = MenuBehavior.menu_last_behavior;
				
				MenuBehavior.menu_last_behavior = null;
				MenuBehavior.menu_last_item     = null;
				
				behavior.SuspendUpdates ();
				behavior.NotifyExitedItem (last_item);
				behavior.ResumeUpdates ();
			}
			
			if (item != null)
			{
				MenuBehavior behavior = MenuItem.GetMenuBehavior (item);
				
				MenuBehavior.menu_last_behavior = behavior;
				MenuBehavior.menu_last_item     = item;
				
				behavior.SuspendUpdates ();
				behavior.NotifyEnteredItem (MenuBehavior.menu_last_item);
				behavior.ResumeUpdates ();
			}
		}
		
		
		private static void MessageFilter(object sender, Message message)
		{
			if ((MenuBehavior.menu_list.Count == 0) &&
				(MenuBehavior.menu_root_list.Count == 0))
			{
				//	Rien à filtrer si aucun menu n'est actuellement visible. Il
				//	est primordial de faire le test immédiatement, pour éviter
				//	de griller de cycles de CPU inutilement.
				
				return;
			}
			
			Window window = sender as Window;
			
			if (message.IsMouseType)
			{
				MenuBehavior.ProcessMouseEvent (window, message);
			}
			else if (message.IsKeyType)
			{
				MenuBehavior.ProcessKeyboardEvent (window, message);
			}
		}
		
		private static void ProcessMouseEvent(Window window, Message message)
		{
			Drawing.Point mouse = window.MapWindowToScreen (message.Cursor);
			
			MenuBehavior.last_mouse_pos = mouse;
			
			Window menu = MenuBehavior.DetectWindow (mouse);
			Window root = menu == null ? MenuBehavior.DetectRootWindow (mouse) : null;
			
			switch (message.Type)
			{
				case MessageType.MouseDown:
					if ((menu == null) &&
						(root == null))
					{
						//	L'utilisateur a cliqué hors de tout menu actuellement
						//	ouvert.
						
						if (MenuBehavior.menu_list.Count > 0)
						{
							//	Referme le(s) menu(s) et mange l'événement si cela
							//	est nécessaire.
							
							MenuBehavior.RejectAll ();
							
							if (message.NonClient == false)
							{
								message.Swallowed = true;
							}
						}
					}
					if (root != null)
					{
						if (MenuBehavior.menu_list.Count > 0)
						{
							MenuBehavior.RejectAll ();
							
							message.Swallowed = true;
						}
					}
					break;
				
				case MessageType.MouseUp:
					break;
				
				case MessageType.MouseEnter:
				case MessageType.MouseMove:
					if (MenuBehavior.keyboard_navigation_active)
					{
						//	En cas de navigation au clavier, on ne tient pas compte
						//	des petits déplacements de souris :
						
						if (Drawing.Point.Distance (mouse, MenuBehavior.keyboard_navigation_mouse_pos) < 4)
						{
							return;
						}
					}
					
					if ((menu != null) ||
						(root != null))
					{
						MenuBehavior.NotifyCursorInItem (MenuBehavior.DetectMenuItem (window, message.Cursor));
					}
					else
					{
						MenuBehavior.NotifyCursorInItem (null);
						
						if (menu != null)
						{
							message.Swallowed = true;
						}
					}
					break;
			}
		}
		
		private static void ProcessKeyboardEvent(Window window, Message message)
		{
			MenuBehavior that = MenuBehavior.menu_last_behavior;
			
			if ((that == null) &&
				(MenuBehavior.menu_list.Count > 0))
			{
				that = MenuBehavior.menu_list[MenuBehavior.menu_list.Count-1] as MenuBehavior;
			}
			
			if ((that == null) ||
				(that.keyboard_menu_window == null))
			{
				return;
			}
			
			//	S'il n'y a aucun menu ouvert, on ne traite les événements du
			//	clavier uniquement si une navigation avait été activée au
			//	préalable :
			
			if ((that.is_open == false) &&
				(that.keyboard_menu_active == false))
			{
				return;
			}
			
			MenuBehavior.ActivateKeyboardNavigation ();
			
			System.Diagnostics.Debug.WriteLine ("Keyboard event routed to menu");
			
			that.SuspendUpdates ();
			
			bool swallowed_event = true;
			
			if (message.Type == MessageType.KeyDown)
			{
				IFeel feel = Feel.Factory.Active;
				
				switch (message.KeyCode)
				{
					case KeyCode.ArrowUp:
						that.KeyboardSelectItem (-1);
						break;
					
					case KeyCode.ArrowDown:
						that.KeyboardSelectItem (1);
						break;
						
					case KeyCode.ArrowRight:
						that.KeyboardSelectMenu (1);
						break;
						
					case KeyCode.ArrowLeft:
						that.KeyboardSelectMenu (-1);
						break;
					
					default:
						if ((feel.TestSelectItemKey (message)) &&
							(that.keyboard_menu_item != null))
						{
							that.keyboard_menu_item.SimulatePressed ();
						}
						else if (feel.TestCancelKey (message))
						{
							that.KeyboardCloseLatestMenu ();
						}
						break;
				}
			}
			
			that.ResumeUpdates ();
			
			message.Swallowed = swallowed_event;
		}
		
		
		private static void ActivateKeyboardNavigation()
		{
			MenuBehavior.keyboard_navigation_active    = true;
			MenuBehavior.keyboard_navigation_mouse_pos = MenuBehavior.last_mouse_pos;
		}
		
		private static void HandleApplicationDeactivated(object sender)
		{
			MenuBehavior.RejectAll ();
		}
		
		private static void HandleTimerTimeElapsed(object sender)
		{
			if (MenuBehavior.timer_item != null)
			{
				MenuBehavior.OpenItemSubmenu (MenuBehavior.timer_item, Animate.No);
			}
		}
		
		
		static object							sync_object = new object ();
		static long								next_id = 1;
		static int								menu_count;
		static System.Collections.ArrayList		menu_list = new System.Collections.ArrayList ();		//	liste de MenuBehavior
		static System.Collections.ArrayList		menu_root_list = new System.Collections.ArrayList ();	//	liste de MenuBehavior
		static MenuItem							menu_last_item;
		static MenuBehavior						menu_last_behavior;
		static Timer							timer;
		static MenuItem							timer_item;
		static Drawing.Point					last_mouse_pos;
		static bool								keyboard_navigation_active;
		static Drawing.Point					keyboard_navigation_mouse_pos;
		
		private long							id = MenuBehavior.GetNextId ();
		private System.Collections.ArrayList	live_menu_windows = new System.Collections.ArrayList ();
		private Window							root_window;
		private Widget							root_menu;
		private bool							is_open;
		
		private bool							keyboard_menu_active;
		private Window							keyboard_menu_window;
		private MenuItem						keyboard_menu_item;
		
		private int								suspend_updates;
		private int								update_requested;
	}
}
