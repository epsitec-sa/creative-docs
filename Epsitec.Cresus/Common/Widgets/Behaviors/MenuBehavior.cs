//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Behaviors
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// La classe MenuBehavior implémente la logique permettant de gérer un menu
	/// (affichage d'une ou plusieurs fenêtres flottantes, fermeture, etc.)
	/// </summary>
	public sealed class MenuBehavior : Types.DependencyObject
	{
		public MenuBehavior()
		{
			//	En principe, on associe à toute une hiérarchie (menu + sous-menus)
			//	un unique MenuBehavior. Dans la pratique, ce n'est pas nécessaire
			//	et seul le MenuBehavior associé à la racine des menus sera utilisé
			//	pour gérer l'ensemble des (sous-)menus.
		}


		public static bool						HasOpenMenus
		{
			get
			{
				lock (MenuBehavior.syncObject)
				{
					if (MenuBehavior.menuList.Count > 0)
					{
						return true;
					}
					if (MenuBehavior.stack.Count > 0)
					{
						return true;
					}
				}

				return false;
			}
		}

		private bool							IsFrozen
		{
			get
			{
				return this.isFrozen;
			}
			set
			{
				if (this.isFrozen != value)
				{
					this.isFrozen = value;
					
					if (value)
					{
						System.Diagnostics.Debug.Assert (MenuBehavior.menuLastBehavior == this);
						
						//	Quand un menu est "gelé", il ne réagit plus aux
						//	événements souris. Il faut mémoriser l'état courant
						//	pour pouvoir le restaurer au moment où le menu est
						//	dégelé :
						
						this.frozenMenuLastItem = MenuBehavior.menuLastItem;
						
						MenuBehavior.menuLastBehavior = null;
						MenuBehavior.menuLastItem     = null;
						
						Behaviors.MenuBehavior.DisableKeyboardFilter ();
					}
					else
					{
						if (this.frozenMenuLastItem != null)
						{
							MenuBehavior.menuLastBehavior = this;
							MenuBehavior.menuLastItem     = this.frozenMenuLastItem;
							
							this.frozenMenuLastItem = null;
						}
						
						Behaviors.MenuBehavior.EnableKeyboardFilter ();
					}
				}
			}
		}
		
		
		#region Animate Enumeration
		public enum Animate
		{
			Yes, No, Automatic
		}
		#endregion
		
		public static MenuBehavior OpenItemSubmenu(Widget item, Animate animate)
		{
			//	Ouvre le sous-menu d'un MenuItem donné. Retourne le MenuBehavior
			//	associé au sous-menu en question.
			
			//	Si l'item spécifié ne possède pas de sous-menu, on garantit au
			//	moins que le (sous-)menu contenant l'item lui-même est visible.
			
			if (item == null)
			{
				return null;
			}
			
			Widget submenu = MenuItem.GetSubmenu (item);
			
			MenuBehavior behavior = null;
			MenuWindow   window   = null;
			
			if (submenu == null)
			{
				//	Il n'y a pas de sous-menu ! Il faut en tout cas montrer le
				//	menu qui contient cet item :
				
				window   = MenuItem.GetMenuWindow (item) as MenuWindow;
				behavior = MenuItem.GetMenuBehavior (item);
				
				if (window != null)
				{
					if ((window.ParentWidget == null) ||
						(window.MenuType != MenuType.Submenu))
					{
						//	Le menu n'a pas de parent. C'est donc le menu racine
						//	dans le cas d'un menu flottant. Il faut fermer tous
						//	les sous-menus :
						
						behavior.OpenSubmenu (window, Animate.No);
						behavior.UpdateItems ();
					}
					else
					{
						MenuBehavior.OpenItemSubmenu (window.ParentWidget, animate);
					}
				}
			}
			else
			{
				behavior = MenuItem.GetMenuBehavior (submenu);
				window   = MenuItem.GetMenuWindow (submenu) as MenuWindow;
				
				behavior.OpenSubmenu (window, animate);
				behavior.UpdateItems ();
			}
			
			return behavior;
		}
		
		public static MenuBehavior CloseItemMenu(Widget item)
		{
			//	Ferme complètement le menu associé à l'item spécifié. Les fenêtres
			//	correspondant aux (sous-)menus sont simplement cachées.
			
			MenuBehavior behavior = MenuItem.GetMenuBehavior (item);
			
			if (behavior != null)
			{
				behavior.HideAll ();
			}
			
			return behavior;
		}

		
		private static void DisableKeyboardFilter()
		{
			MenuBehavior.filterKeyboardOff++;
		}
		
		private static void EnableKeyboardFilter()
		{
			if (MenuBehavior.filterKeyboardOff > 0)
			{
				MenuBehavior.filterKeyboardOff--;
			}
		}
		
		
		public void OpenPopup(MenuWindow window, Animate animate)
		{
			//	Montre le menu en animant l'ouverture à la façon d'un "pop-up".
			
			this.OpenGenericMenu (window, animate, MenuType.Popup);
		}
		
		public void OpenSubmenu(MenuWindow window, Animate animate)
		{
			//	Montre le menu en animant l'ouverture à la façon d'un sous-menu.
			this.OpenGenericMenu (window, animate, MenuType.Submenu);
		}

		public void OpenCombo(MenuWindow window, Animate animate)
		{
			//	Montre le menu en animant l'ouverture à la façon d'une combo.
			this.OpenGenericMenu (window, animate, MenuType.ComboList);
		}

		
		private void OpenGenericMenu(MenuWindow window, Animate animate, MenuType type)
		{
			//	Ouvre le sous-menu spécifié par sa fenêtre. Le sous-menu doit
			//	impérativement appartenir à notre "menu".
			
			if (window == null)
			{
				//	Il faut cacher toutes les fenêtres encore ouvertes, car ce
				//	cas (window == null) apparaît quand on désire montrer un
				//	menu horizontal déjà visible (racine d'un menu dynamique).
				
				this.HideAll ();
				
				return;
			}
			
			System.Diagnostics.Debug.Assert (window.Behavior == this);
			
			MenuWindow[] live  = (MenuWindow[]) this.liveMenuWindows.ToArray (typeof (MenuWindow));
			int          index = this.liveMenuWindows.IndexOf (window);
			
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
				//	Le sous-menu n'est pas encore visible.
				
				if (type != MenuType.Undefined)
				{
					window.MenuType = type;
				}
				
				//	Il faut s'assurer que le parent immédiat est visible avant
				//	de montrer le sous-menu (si c'en est un) :
				
				if (window.MenuType == MenuType.Submenu)
				{
					Widget parentWidget = window.ParentWidget;
					Window parentWindow = parentWidget == null ? null : MenuItem.GetMenuWindow (parentWidget);
					
					if (parentWidget != null)
					{
						this.OpenSubmenu (parentWindow as MenuWindow, animate);
					}
				}
				
				this.ShowSubmenu (window, animate);
			}
		}
		
		
		public void HideAll()
		{
			//	Ferme tous les menus associés à ce gestionnaire.
			
			MenuBehavior.DisableKeyboardNavigation ();
			
			while (this.liveMenuWindows.Count > 0)
			{
				MenuWindow window = this.liveMenuWindows[this.liveMenuWindows.Count-1] as MenuWindow;
				window.Hide ();
			}
			
			this.CleanupAfterClose ();
		}
		
		public void Accept()
		{
			if (this.isOpen)
			{
				this.HideAll ();
				this.OnAccepted ();
			}
		}
		
		public void Reject()
		{
			if (this.isOpen)
			{
				this.HideAll ();
				this.OnRejected ();
			}
		}
		
		
		internal void Attach(Widget rootMenu)
		{
			//	Attache un menu racine au gestionnaire de menus. Un seul menu
			//	racine peut être attaché à la fois. Attache aussi la fenêtre
			//	contenant le menu racine si le menu est est visible.
			
			System.Diagnostics.Debug.Assert (this.rootMenu == null);
			
			this.rootMenu = rootMenu;
			
			if (this.rootMenu.IsVisible)
			{
				this.AttachRootWindow ();
			}
			
			this.rootMenu.IsVisibleChanged += this.HandleRootMenuIsVisibleChanged;
		}
		
		internal void Detach(Widget rootMenu)
		{
			//	Cf. Attach
			
			System.Diagnostics.Debug.Assert (this.rootMenu == rootMenu);
			
			this.rootMenu.IsVisibleChanged -= this.HandleRootMenuIsVisibleChanged;
			
			if (this.rootMenu.IsVisible)
			{
				this.DetachRootWindow ();
			}
			
			this.rootMenu = null;
		}
		
		
		internal void AttachMenuItemContainer(MenuItemContainer item)
		{
			this.IsFrozen = true;
			this.UpdateItems ();
			
			System.Diagnostics.Debug.Assert (this.frozenMenuLastItem == item);
		}
		
		internal void DetachMenuItemContainer(MenuItemContainer item)
		{
			System.Diagnostics.Debug.Assert (this.frozenMenuLastItem == item);
			
			this.IsFrozen = false;
			this.UpdateItems ();
		}
		
		
		private void ShowSubmenu(MenuWindow window, Animate animate)
		{
			//	Montre un sous-menu en le positionnant au préalable par rapport
			//	à son parent, de la manière la plus judicieuse possible.
			
			Widget       parent = window.ParentWidget;
			Drawing.Size size   = window.Root.GetBestFitSize ();
			
			//	Détermine la position où afficher le menu...
			
			IMenuHost host = parent as IMenuHost;
			
			if ((host == null) &&
				(parent != null) &&
				(window.MenuType != MenuType.Popup))
			{
				host = MenuItem.GetMenuHost (parent);
			}
			
			Drawing.Point location  = window.WindowLocation;
			Animation     animation = Animation.FadeIn;
			
			if (host != null)
			{
				host.GetMenuDisposition (parent, ref size, out location, out animation);
			}
			
			switch (animate)
			{
				case Animate.No:
					animation = Animation.None;
					break;
				
				case Animate.Automatic:
					if (this.liveMenuWindows.Count > 0)
					{
						animation = Animation.None;
					}
					break;
			}
			
			ToolTip.HideAllToolTips ();
			
			window.WindowSize     = size;
			window.WindowLocation = location;
			window.AnimateShow (animation);
			
			this.isOpen = true;
			
			if (this.keyboardMenuWindow == null)
			{
				this.keyboardMenuWindow = window;
			}
		}
		
		private void CleanupAfterClose()
		{
			//	Après la fermeture complète d'un menu, stoppe le timer et remet
			//	le gestionnaire de menus dans un état "neutre".
			
			MenuBehavior.timer.Stop ();
			MenuBehavior.timerItem      = null;
			MenuBehavior.timerKeepMenu  = null;
			MenuBehavior.timerBehaviour = null;
			
			if (this.IsFrozen)
			{
				this.ChangeFocusedItem (null);
			}
			
			System.Diagnostics.Debug.Assert (this.isFrozen == false);
			System.Diagnostics.Debug.Assert (this.frozenMenuLastItem == null);
			
			this.isOpen = false;
			this.keyboardMenuActive = false;
		}
		
		
		internal void HandleMenuItemPressed(MenuItem item)
		{
			MenuBehavior.DisableKeyboardNavigation ();
			
			this.CleanupAfterClose ();
			this.OnAccepted ();
		}
		
		internal void HandleAboutToShowMenuWindow(MenuWindow window)
		{
			//	Appelé avant qu'une fenêtre du menu ne devienne visible. A
			//	partir de ce moment, il faut considérer la fenêtre comme en
			//	cours d'affichage.
			
			this.AttachMenu ();
			
			lock (MenuBehavior.syncObject)
			{
				MenuBehavior.menuList.Remove (this);
				MenuBehavior.menuList.Add (this);
			}
			
			MenuBehavior.NotifyMenuListChanged ();
			
			System.Diagnostics.Debug.Assert (this.liveMenuWindows.Contains (window) == false);
			
			this.liveMenuWindows.Add (window);
			this.UpdateItems ();
		}
		
		internal void HandleAboutToHideMenuWindow(MenuWindow window)
		{
			//	Appelé avant qu'une fenêtre du menu ne soit cachée.
			
			System.Diagnostics.Debug.Assert (this.liveMenuWindows.Contains (window) == true);
			System.Diagnostics.Debug.Assert (this.liveMenuWindows.IndexOf (window) == this.liveMenuWindows.Count-1);
			
			this.liveMenuWindows.Remove (window);
			this.UpdateItems ();
			
			//	Si une fenêtre possède le focus, il faut avant de la cacher activer
			//	la fenêtre parent, pour éviter des clignotements de fenêtres :
			
			if ((window.IsFocused) &&
				(Window.IsApplicationActive))
			{
				Window owner = window.Owner;
				
				if (owner != null)
				{
					owner.MakeFocused ();
				}
			}
			
			this.DetachMenu ();
			
			if (this.liveMenuWindows.Count == 0)
			{
				lock (MenuBehavior.syncObject)
				{
					MenuBehavior.menuList.Remove (this);
				}
				
				MenuBehavior.NotifyMenuListChanged ();
			}
		}
		
		internal void HandleKeyboardEvent(Window window, Message message)
		{
			MenuBehavior.ProcessKeyboardEvent (window, message);
		}
		
		
		static MenuBehavior()
		{
			MenuBehavior.timer = new Timer ();
			MenuBehavior.stack = new Stack<State> ();

			MenuBehavior.menuList      = new List<MenuBehavior> ();
			MenuBehavior.menuRootList = new List<MenuBehavior> ();
			
			MenuBehavior.timer.TimeElapsed += MenuBehavior.HandleTimerTimeElapsed;
		}
		
		
		private void AttachMenu()
		{
			this.localMenuCount++;
			
			if (this.localMenuCount == 1)
			{
				lock (MenuBehavior.syncObject)
				{
					MenuBehavior.menuCount++;
				
					if (MenuBehavior.menuCount == 1)
					{
						MenuBehavior.RegisterFilter ();
					}
				}
				
				MenuBehavior.Push (this);
			}
		}
		
		private void DetachMenu()
		{
			this.localMenuCount--;
			
			if (this.localMenuCount == 0)
			{
				MenuBehavior.Pop (this);
				
				lock (MenuBehavior.syncObject)
				{
					MenuBehavior.menuCount--;
				
					if (MenuBehavior.menuCount == 0)
					{
						MenuBehavior.UnregisterFilter ();
						MenuBehavior.GenerateDummyMouseMoveEvents ();
					}
				}
			}
		}
		
		
		private void AttachRootWindow()
		{
			//	Quand la fenêtre contenant le menu racine devient visible, on
			//	active le filtre, etc. comme si on avait ouvert un menu.
			
			this.AttachMenu ();
			
			this.rootWindow = this.rootMenu.Window;
			
			lock (MenuBehavior.syncObject)
			{
				MenuBehavior.menuRootList.Add (this);
			}
		}
		
		private void DetachRootWindow()
		{
			//	Cf. AttachRootWindow
			
			lock (MenuBehavior.syncObject)
			{
				MenuBehavior.menuRootList.Remove (this);
			}
			
			this.rootWindow = null;
			
			this.DetachMenu ();
		}
		
		
		private void HandleRootMenuIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Quand le menu racine devient visible, on s'attache à sa fenêtre
			//	via AttachRootWindow, et vice versa.
			
			System.Diagnostics.Debug.Assert (this.rootMenu == sender);
			
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
			if (item == null)
			{
				return;
			}
			
			if (this.keyboardMenuItem != item)
			{
				this.keyboardMenuItem   = item;
				this.keyboardMenuWindow = item.Window;
			}
			
			if (this.isOpen == false)
			{
				this.UpdateItems ();
			}
			else if (MenuItem.GetZeroDelay (item))
			{
				MenuBehavior.timer.Suspend ();
				
				MenuBehavior.timerItem      = null;
				MenuBehavior.timerKeepMenu = null;
				MenuBehavior.timerBehaviour = null;
				
				MenuBehavior.OpenItemSubmenu (item, Animate.No);
			}
			else
			{
				MenuBehavior.timer.Suspend ();
				
				MenuBehavior.timerItem      = item;
				MenuBehavior.timerKeepMenu = null;
				MenuBehavior.timerBehaviour = null;

				MenuBehavior.timer.Delay = SystemInformation.MenuShowDelay;
				MenuBehavior.timer.Start ();
				
				this.UpdateItems ();
			}
		}
		
		private void NotifyExitedItem(MenuItem item)
		{
			this.keyboardMenuItem   = null;
			this.keyboardMenuWindow = null;
			
			if (this.liveMenuWindows.Count > 0)
			{
				//	Il reste probablement des fenêtres de sous-menu à
				//	refermer :
				
				MenuBehavior.timer.Suspend ();
				
				MenuBehavior.timerItem      = null;
				MenuBehavior.timerKeepMenu = MenuItem.GetMenuWindow (item) as MenuWindow;
				MenuBehavior.timerBehaviour = this;
				
				MenuBehavior.timer.Delay = SystemInformation.MenuShowDelay;
				MenuBehavior.timer.Start ();
			}
			
			this.UpdateItems ();
		}
		

		private void KeyboardCloseLatestMenu()
		{
			int n = this.liveMenuWindows.Count;
			
			if (n > 0)
			{
				MenuWindow window = this.liveMenuWindows[n-1] as MenuWindow;
				
				this.keyboardMenuItem   = ((window.ParentWidget != null) && (window.ParentWidget.RootParent is WindowRoot)) ? window.ParentWidget as MenuItem : null;
				this.keyboardMenuWindow = (this.keyboardMenuItem == null) ? null : this.keyboardMenuItem.Window;
				this.keyboardMenuActive = true;
				
				if (this.keyboardMenuWindow != null)
				{
					window.Hide ();
				}
				else
				{
					this.Reject ();
				}
			}
			else
			{
				this.keyboardMenuItem   = null;
				this.keyboardMenuWindow = null;
				this.keyboardMenuActive = false;
				
				this.isOpen = false;
				
				MenuBehavior.DisableKeyboardNavigation ();
			}
			
			MenuBehavior.menuLastItem     = this.keyboardMenuItem;
			MenuBehavior.menuLastBehavior = this.keyboardMenuItem == null ? null : this;
			
			this.UpdateItems ();
		}
		
		private void KeyboardSelectItem(int direction)
		{
			//	Sélectionne un élément du même menu à la suite d'un déplacement
			//	avec les touches haut/bas.
			
			int    n    = this.liveMenuWindows.Count;
			Widget root = this.keyboardMenuWindow == null ? null : this.keyboardMenuWindow.Root;
			
			if (n > 0)
			{
				MenuItem item = null;
				
				if (direction < 0)
				{
					item = this.FindPrevItem (root, this.keyboardMenuItem);
					
					if (item == null)
					{
						item = this.FindLastItem (root);
					}
				}
				else if (direction > 0)
				{
					item = this.FindNextItem (root, this.keyboardMenuItem);
					
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
					this.keyboardMenuItem = item;
					
					MenuWindow[] live = (MenuWindow[]) this.liveMenuWindows.ToArray (typeof (MenuWindow));
					
					for (int i = live.Length-1; i >= 0; i--)
					{
						if (live[i] == this.keyboardMenuWindow)
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
				
				MenuBehavior.OpenItemSubmenu (this.keyboardMenuItem, Animate.No);
				MenuBehavior.EnableKeyboardNavigation ();
				
				n = this.liveMenuWindows.Count;
				
				if (n > 0)
				{
					this.keyboardMenuWindow = this.liveMenuWindows[n-1] as MenuWindow;
					this.keyboardMenuItem   = this.FindFirstItem (this.keyboardMenuWindow.Root);
				}
			}
			
			this.UpdateItemsAfterKeyboardAction (this.keyboardMenuItem);
		}
		
		private void KeyboardSelectMenu(int direction)
		{
			//	Navigation avec les touches gauche/droite, pour passer de menu
			//	en menu, dans le cas d'un menu hiérarchique.
			
			int      n      = this.liveMenuWindows.Count;
			MenuItem item   = null;
			Window   window = null;
			
			if ((n > 0) &&
				((this.rootWindow == this.keyboardMenuWindow) || (MenuBehavior.menuLastItem == null)))
			{
				//	Aucune case sélectionnée dans le menu, ou alors c'est une case
				//	du menu principal. Dans ce cas, il faut commencer par sélec-
				//	tionner la première case du premier sous-menu :
				
				MenuWindow menu = this.liveMenuWindows[0] as MenuWindow;
				
				item = this.FindFirstItem (menu.Root);
				
				if (item != null)
				{
					this.keyboardMenuItem   = item;
					this.keyboardMenuWindow = item.Window;
				}
			}
			
			if (n == 0)
			{
				//	Déplacement horizontal dans le menu de base. Aucun sous-menu
				//	n'est actuellement ouvert. On ne va donc pas ouvrir de menu,
				//	mais juste changer de MenuItem actif :
				
				if (direction < 0)
				{
					item = this.FindPrevItem (this.rootMenu, this.keyboardMenuItem);
					
					if (item == null)
					{
						item = this.FindLastItem (this.rootMenu);
					}
				}
				else if (direction > 0)
				{
					item = this.FindNextItem (this.rootMenu, this.keyboardMenuItem);
					
					if (item == null)
					{
						item = this.FindFirstItem (this.rootMenu);
					}
				}
				
				if (item != null)
				{
					window = this.rootWindow;
				}
			}
			else
			{
				//	Déplacement horizontal avec un ou plusieurs sous-menus ouverts.
				//	En fonction du niveau, il faudra soit ouvrir un sous-menu, soit
				//	passer au menu suivant/précédent :
				
				item = null;
				
				if ((this.keyboardMenuItem != null) &&
					(this.keyboardMenuItem.RootParent is WindowRoot))
				{
					item = this.keyboardMenuItem;
				}
				
				if (item == null)
				{
					Widget root = this.keyboardMenuWindow == null ? null : this.keyboardMenuWindow.Root;
					
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
							MenuBehavior.EnableKeyboardNavigation ();
							
							n = this.liveMenuWindows.Count - 1;
							
							if (n > 0)
							{
								window = this.liveMenuWindows[n] as MenuWindow;
								item   = this.FindFirstItem (window.Root);
							}
						}
						else if (this.rootMenu != null)
						{
							//	Il n'y a pas de sous-menu pour l'élément en cours.
							//	Il faut donc fermer ce menu et ouvrir le menu de
							//	base suivant (s'il y en a).
							
							Widget root = this.FindRootItem (item);
							
							if (root != null)
							{
								item = this.FindNextItem (this.rootMenu, root);
								
								if (item == null)
								{
									item = this.FindFirstItem (this.rootMenu);
								}
								
								if (item != null)
								{
									MenuBehavior.OpenItemSubmenu (item, Animate.No);
									MenuBehavior.EnableKeyboardNavigation ();
									
									if (this.liveMenuWindows.Count == 1)
									{
										window = this.liveMenuWindows[0] as MenuWindow;
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
							(parent.Window == this.rootWindow))
						{
							//	Il n'y a plus de parent sur lequel on puisse se
							//	rabattre. Montre le menu précédent :
							
							Widget root = this.FindRootItem (item);
							
							if (root != null)
							{
								item = this.FindPrevItem (this.rootMenu, root);
								
								if (item == null)
								{
									item = this.FindLastItem (this.rootMenu);
								}
								
								if (item != null)
								{
									MenuBehavior.OpenItemSubmenu (item, Animate.No);
									MenuBehavior.EnableKeyboardNavigation ();
									
									if (this.liveMenuWindows.Count == 1)
									{
										window = this.liveMenuWindows[0] as MenuWindow;
										item   = this.FindFirstItem (window.Root);
									}
								}
							}
						}
						else
						{
							//	Ferme le sous-menu actuellement ouvert et change
							//	le "focus" de place :
							
							window = this.liveMenuWindows[n-1] as MenuWindow;
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
				this.keyboardMenuItem   = item;
				this.keyboardMenuWindow = window;
				
				this.UpdateItemsAfterKeyboardAction (item);
			}
		}
		
		private void UpdateItemsAfterKeyboardAction(MenuItem item)
		{
			this.SuspendUpdates ();
			
			MenuItem oldItem = MenuBehavior.menuLastItem;
			MenuItem newItem = item;
			
			MenuBehavior.menuLastItem     = item;
			MenuBehavior.menuLastBehavior = (item == null) ? null : this;
			
			if (oldItem != newItem)
			{
				this.ChangeFocusedItem (newItem);
			}
			
			this.keyboardMenuActive = true;
			
			this.UpdateItems ();
			this.ResumeUpdates ();
		}
		
		private bool ChangeFocusedItem(MenuItem newItem)
		{
			//	Change le focus d'un item à un autre, tout en tenant compte des
			//	éventuels items "riches" qui nécessitent un traitement spécial.
			
			MenuItemContainer oldContainer = this.frozenMenuLastItem as MenuItemContainer;
			MenuItemContainer newContainer = newItem as MenuItemContainer;
			
			if (oldContainer != null)
			{
				//	Un item "riche" a le focus et il faut donc lui signaler qu'il va le
				//	perdre (ce qui va mettre IsFrozen à 'false' et procéder à diverses
				//	modifications) :
				
				oldContainer.DefocusFromMenu ();
				
				//	Les réglages de menuLast_... peuvent avoir été modifiés depuis
				//	notre appel. On les applique à nouveau ici pour être sûr :
				
				MenuBehavior.menuLastItem     = newItem;
				MenuBehavior.menuLastBehavior = (newItem == null) ? null : this;
			}
			
			System.Diagnostics.Debug.Assert (this.isFrozen == false);
			System.Diagnostics.Debug.Assert (this.frozenMenuLastItem == null);
			
			if (newContainer != null)
			{
				//	Un item "riche" va recevoir le focus :
				
				return newContainer.FocusFromMenu ();
			}
			else
			{
				return false;
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
				foreach (Widget item in root.FindAllChildren ())
				{
					if ((item is MenuItem) && (! (item is MenuSeparator)))
					{
						return item as MenuItem;
					}
				}
			}
			
			return null;
		}
		
		private MenuItem FindLastItem(Widget root)
		{
			if (root != null)
			{
				foreach (Widget item in Types.Collection.Reverse<Widget> (root.FindAllChildren ()))
				{
					if ((item is MenuItem) && (!(item is MenuSeparator)))
					{
						return item as MenuItem;
					}
				}
			}
			
			return null;
		}
		
		private MenuItem FindNextItem(Widget root, Widget item)
		{
			if (root != null)
			{
				bool found = false;
				
				foreach (Widget child in root.FindAllChildren ())
				{
					if (child == item)
					{
						found = true;
					}
					else if ((found) && (child is MenuItem) && (! (child is MenuSeparator)))
					{
						return child as MenuItem;
					}
				}
			}
			
			return null;
		}
		
		private MenuItem FindPrevItem(Widget root, Widget item)
		{
			if (root != null)
			{
				bool found = false;

				foreach (Widget child in Types.Collection.Reverse<Widget> (root.FindAllChildren ()))
				{
					if (child == item)
					{
						found = true;
					}
					else if ((found) && (child is MenuItem) && (!(child is MenuSeparator)))
					{
						return child as MenuItem;
					}
				}
			}
			
			return null;
		}
		
		
		private void SuspendUpdates()
		{
			this.suspendUpdates++;
		}
		
		private void ResumeUpdates()
		{
			System.Diagnostics.Debug.Assert (this.suspendUpdates > 0);
			
			this.suspendUpdates--;
			
			if (this.suspendUpdates == 0)
			{
				if (this.updateRequested > 0)
				{
					this.UpdateItems ();
				}
			}
		}
		
		private void UpdateItems()
		{
			if (this.suspendUpdates > 0)
			{
				this.updateRequested++;
				return;
			}
			
			this.suspendUpdates++;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			MenuWindow[] windows = (MenuWindow[]) this.liveMenuWindows.ToArray (typeof (MenuWindow));
			int hiliteBelowI = -1;
			
			for (int i = 0; i < windows.Length; i++)
			{
				foreach (Widget widget in windows[i].Root.FindAllChildren ())
				{
					if (widget is MenuItem)
					{
						if (widget == MenuBehavior.menuLastItem)
						{
							MenuItem.SetItemState (widget, MenuItemState.Selected);
							hiliteBelowI = i + 1;
						}
						else
						{
							list.Add (widget);
						}
					}
				}
			}
			
			if (this.rootMenu != null)
			{
				foreach (Widget widget in this.rootMenu.FindAllChildren ())
				{
					if (widget is MenuItem)
					{
						if (widget == MenuBehavior.menuLastItem)
						{
							MenuItem.SetItemState (widget, MenuItemState.Selected);
						}
						else
						{
							list.Add (widget);
						}
					}
				}
			}
			
			if (MenuBehavior.menuLastItem == null)
			{
				hiliteBelowI = windows.Length;
			}
			
			if ((hiliteBelowI > 0) &&
				(MenuBehavior.timerKeepMenu != null))
			{
				//	La souris est sortie du menu spécifié par timerKeepMenu et
				//	on ne doit donc pas peindre la case mise en évidence pour le
				//	sous-menu ouvert, s'il y en a un.
				
				for (int i = 0; i < hiliteBelowI; i++)
				{
					if (windows[i] == MenuBehavior.timerKeepMenu)
					{
						hiliteBelowI = i+1;
						break;
					}
				}
			}
			
			for (int i = 0; i < hiliteBelowI; i++)
			{
				MenuWindow menu = windows[i];
				
				Widget widget = menu.ParentWidget;
				
				if (widget != null)
				{
					MenuItem.SetItemState (widget, MenuItemState.SubmenuOpen);
				}
				
				list.Remove (widget);
			}
			
			foreach (Widget widget in list)
			{
				MenuItem.SetItemState (widget, MenuItemState.Default);
			}
			
			this.suspendUpdates--;
			this.updateRequested = 0;
		}
		
		
		private static void RegisterFilter()
		{
			Window.ApplicationDeactivated += MenuBehavior.HandleApplicationDeactivated;
			Window.MessageFilter          += MenuBehavior.MessageFilter;
		}
		
		private static void UnregisterFilter()
		{
			Window.ApplicationDeactivated -= MenuBehavior.HandleApplicationDeactivated;
			Window.MessageFilter          -= MenuBehavior.MessageFilter;
		}
		
		private static void GenerateDummyMouseMoveEvents()
		{
			Drawing.Point pos = Message.CurrentState.LastScreenPosition;
			Window[]  windows = Window.FindFromPosition (pos);
			
			foreach (Window window in windows)
			{
				window.DispatchMessage (Message.CreateDummyMouseMoveEvent (window.MapScreenToWindow (pos)));
			}
		}
		
		
		private static MenuBehavior[] GetActiveMenuBehaviors()
		{
			lock (MenuBehavior.syncObject)
			{
				return MenuBehavior.menuList.ToArray ();
			}
		}
		
		private static MenuBehavior[] GetRootMenuBehaviors()
		{
			lock (MenuBehavior.syncObject)
			{
				return MenuBehavior.menuRootList.ToArray ();
			}
		}
		
		
		private static long GetNextId()
		{
			lock (MenuBehavior.syncObject)
			{
				return MenuBehavior.nextId++;
			}
		}
		
		
		private static Window DetectWindow(Drawing.Point pos)
		{
			MenuBehavior[] behaviors = MenuBehavior.GetActiveMenuBehaviors ();
			
			for (int i = behaviors.Length-1; i >= 0; i--)
			{
				MenuBehavior behavior = behaviors[i];
				MenuWindow[] windows  = (MenuWindow[]) behavior.liveMenuWindows.ToArray (typeof (MenuWindow));
				
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
				
				if ((behavior.rootWindow != null) &&
					(behavior.rootWindow.WindowBounds.Contains (pos)) &&
					(behavior.rootMenu.HitTest (behavior.rootMenu.MapScreenToParent (pos))))
				{
					return behavior.rootWindow;
				}
			}
			
			return null;
		}
		
		private static MenuItem DetectMenuItem(Window window, Drawing.Point pos)
		{
			Widget widget = window.Root.FindChild (pos, WidgetChildFindMode.SkipHidden | WidgetChildFindMode.Deep | WidgetChildFindMode.SkipDisabled);
			
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
		
		private static void RejectActive()
		{
			MenuBehavior[] behaviors = MenuBehavior.GetActiveMenuBehaviors ();
			
			if (behaviors.Length > 0)
			{
				behaviors[behaviors.Length-1].Reject ();
			}
		}
		
		
		private static void MessageFilter(object sender, Message message)
		{
			if ((MenuBehavior.menuList.Count == 0) &&
				(MenuBehavior.menuRootList.Count == 0))
			{
				//	Rien à filtrer si aucun menu n'est actuellement visible. Il
				//	est primordial de faire le test immédiatement, pour éviter
				//	de griller inutilement des cycles CPU.
				
				return;
			}
			
			Window window = sender as Window;
			
			if ((window == null) ||
				(window.IsDisposed))
			{
				return;
			}
			
			if (message.IsMouseType)
			{
				if ((MenuBehavior.menuList.Count > 0) &&
					(window.CapturingWidget != null))
				{
					//	Quelqu'un a capturé la souris... On désactive cette capture,
					//	à moins que la fenêtre responsable de la capture soit elle-
					//	même une fenêtre appartenant au menu actif :
					
					int  n       = MenuBehavior.menuList.Count;
					bool release = true;
					
					if (n > 0)
					{
						MenuBehavior that = MenuBehavior.menuList[n-1];
						
						foreach (Window menu in that.liveMenuWindows)
						{
							if (menu == window)
							{
								release = false;
								break;
							}
						}
					}
					
					if (release)
					{
						System.Diagnostics.Debug.WriteLine ("Released Window Capture because a menu is visible.");
						window.ReleaseCapture ();
					}
				}
				
				MenuBehavior.ProcessMouseEvent (window, message);
			}
			else if (message.IsKeyType)
			{
				if (MenuBehavior.filterKeyboardOff > 0)
				{
					return;
				}
				
				MenuWindow menuWindow = window as MenuWindow;
				
				if ((menuWindow != null) &&
					(menuWindow.MenuType == MenuType.ComboList))
				{
					return;
				}
				
				MenuBehavior.ProcessKeyboardEvent (window, message);
			}
		}
		
		private static void ProcessMouseEvent(Window window, Message message)
		{
			Drawing.Point mouse = window.MapWindowToScreen (message.Cursor);
			
			MenuBehavior.lastMousePos = mouse;
			
			//	Détermine dans quelle fenêtre appartenant à un menu la souris se
			//	trouve ('menu' ou 'root' si la souris se trouve dans la fenêtre
			//	racine d'un menu hiérarchique).
			
			Window   menu = MenuBehavior.DetectWindow (mouse);
			Window   root = menu == null ? MenuBehavior.DetectRootWindow (mouse) : null;
			MenuItem item = null;
			
			//	Détermine dans quel MenuItem la souris se trouve actuellement :
			
			if ((menu != null) ||
				(root != null))
			{
				item = MenuBehavior.DetectMenuItem (window, message.Cursor);
			}
			
			//	Par défaut, on consomme l'événement lorsque toutes les conditions
			//	suivantes sont remplies :
			//
			//	- Un menu est affiché
			//	- La souris se trouve dans une partie "client" d'une fenêtre
			//	- La souris se trouve hors d'une fenêtre appartenant à un menu

			bool mouseInMenu   = (item != null) || (menu != null);
			bool swallowMessage = (MenuBehavior.menuList.Count > 0) && (message.NonClient == false) && !mouseInMenu;
			
			
			//	Pour résumer : un clic hors du menu ferme le menu; un clic dans
			//	le menu est traité par le menu item lui-même. Un relâchement de
			//	bouton dans un menu item est considéré comme un clic.
			
			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					if (!mouseInMenu)
					{
						//	L'utilisateur a cliqué hors de tout menu actuellement
						//	ouvert. Si un menu est actuellement ouvert, il faut le
						//	refermer :
						
						if (MenuBehavior.menuList.Count > 0)
						{
							MenuBehavior.RejectActive ();
						}
					}
					else if (mouseInMenu && (root == null))
					{
						//	L'utilisateur a cliqué dans une fenêtre appartenant à
						//	un menu (autre que celui de la racine); on va changer
						//	le focus si un item "riche" a été cliqué :
						
						MenuBehavior that = MenuItem.GetMenuBehavior (item);
						
						if (that != null)
						{
							if (that.ChangeFocusedItem (item))
							{
								message.Swallowed = true;
							}
						}
					}
					else
					{
						//	L'utilisateur a cliqué dans la fenêtre racine, dans la
						//	surface du menu racine. S'il y a des menus ouverts, il
						//	faut les fermer :
						
						if (MenuBehavior.menuList.Count > 0)
						{
							MenuBehavior.RejectActive ();
							swallowMessage = true;
						}
					}
					break;
				
				case MessageType.MouseUp:
					if (item != null)
					{
						if ((item is MenuItemContainer) ||
							(item is MenuSeparator))
						{
							//	Rien à faire (on transmet le message plus loin) si
							//	la souris se trouve dans un séparateur ou dans un
							//	item "riche".
						}
						else
						{
							//	Sinon, simule une pression du MenuItem et consomme
							//	l'événement.
							
							item.SimulateClicked ();
							
							swallowMessage = true;
						}
					}
					break;
				
				case MessageType.MouseEnter:
				case MessageType.MouseLeave:
				case MessageType.MouseMove:
					if ((MenuBehavior.keyboardNavigationActive) &&
						(Drawing.Point.Distance (mouse, MenuBehavior.keyboardNavigationMousePos) < 4))
					{
						//	En cas de navigation au clavier, on ne tient pas compte
						//	des petits déplacements de souris.
						
						swallowMessage = true;
					}
					else
					{
						//	...sinon, on traite l'éventuel changement d'item actif :
						
						MenuBehavior.ProcessMouseMoveInItem (item);
					}
					break;
			}
			
			if (swallowMessage)
			{
				message.Swallowed = true;
			}
		}
		
		private static void ProcessMouseMoveInItem(MenuItem item)
		{
			//	Signale que la souris est dans le menu item spécifié. S'il y a
			//	eu un changement depuis la dernière fois, il faut mettre à jour
			//	les menus correspondants.
			
			if (MenuBehavior.menuLastItem == item)
			{
				return;
			}
			
			//	Si la souris est sortie du menu, mais qu'une navigation au clavier
			//	est active, on ignore le changement :
			
			if ((item == null) &&
				(MenuBehavior.keyboardNavigationActive))
			{
				return;
			}
			
			//	Du moment qu'il y a eu un déplacement de souris suffisamment
			//	important, on désactive la navigation au clavier :
			
			MenuBehavior.DisableKeyboardNavigation ();
			
			//	Nous allons mettre à jour (peut-être en plusieurs étapes) les
			//	états de sélection des menu items. Pour éviter de clignoter, on
			//	suspend (éventuellement) le redessin temporairement.
			
			MenuBehavior suspendedBehavior = null;
			
			//	S'il y avait un menu item sélectionné, on commence par signaler
			//	que la souris a quitté cet item.
			
			if (MenuBehavior.menuLastItem != null)
			{
				MenuItem     lastItem = MenuBehavior.menuLastItem;
				MenuBehavior behavior  = MenuBehavior.menuLastBehavior;
				
				MenuBehavior.menuLastBehavior = null;
				MenuBehavior.menuLastItem     = null;
				
				System.Diagnostics.Debug.Assert (behavior.IsFrozen == false);
				
				behavior.SuspendUpdates ();
				behavior.NotifyExitedItem (lastItem);
				
				suspendedBehavior = behavior;
			}
			
			//	S'il y a un nouvel item qui contient la souris, on signale qu'il
			//	faut le sélectionner.
			
			if (item != null)
			{
				MenuBehavior behavior = MenuItem.GetMenuBehavior (item);
				
				if (behavior.IsFrozen == false)
				{
					MenuBehavior.menuLastBehavior = behavior;
					MenuBehavior.menuLastItem     = item;
					
					behavior.SuspendUpdates ();
					behavior.NotifyEnteredItem (MenuBehavior.menuLastItem);
					behavior.ResumeUpdates ();
				}
			}
			
			if (suspendedBehavior != null)
			{
				suspendedBehavior.ResumeUpdates ();
			}
		}
		
		
		private static void ProcessKeyboardEvent(Window window, Message message)
		{
			MenuBehavior that = MenuBehavior.menuLastBehavior;
			
			if ((that == null) &&
				(MenuBehavior.menuList.Count > 0))
			{
				that = MenuBehavior.menuList[MenuBehavior.menuList.Count-1];
			}
			
			if (that == null)
			{
				return;
			}
			
			//	S'il n'y a aucun menu ouvert, on ne traite les événements du
			//	clavier uniquement si une navigation avait été activée au
			//	préalable :
			
			if ((that.isOpen == false) &&
				(that.keyboardMenuActive == false))
			{
				return;
			}
			
			MenuBehavior.EnableKeyboardNavigation ();
			
			that.SuspendUpdates ();
			
			bool swallowMessage = true;
			
			if (message.MessageType == MessageType.KeyDown)
			{
				IFeel feel = Feel.Factory.Active;
				
				switch (message.KeyCode)
				{
					case KeyCode.ArrowUp:		that.KeyboardSelectItem (-1);	break;
					case KeyCode.ArrowDown:		that.KeyboardSelectItem (1);	break;
					case KeyCode.ArrowRight:	that.KeyboardSelectMenu (1);	break;
					case KeyCode.ArrowLeft:		that.KeyboardSelectMenu (-1);	break;
					
					default:
						if ((feel.TestSelectItemKey (message)) &&
							(that.keyboardMenuItem != null))
						{
							that.keyboardMenuItem.SimulatePressed ();
						}
						else if (feel.TestCancelKey (message))
						{
							that.KeyboardCloseLatestMenu ();
						}
						break;
				}
			}
			
			that.ResumeUpdates ();
			
			if (swallowMessage)
			{
				message.Swallowed = true;
			}
		}
		
		
		private static void EnableKeyboardNavigation()
		{
			MenuBehavior.keyboardNavigationActive    = true;
			MenuBehavior.keyboardNavigationMousePos = MenuBehavior.lastMousePos;
		}
		
		private static void DisableKeyboardNavigation()
		{
			if (MenuBehavior.keyboardNavigationActive)
			{
				MenuBehavior.keyboardNavigationActive = false;
			}
		}
		
		private static void HandleApplicationDeactivated(object sender)
		{
			while (MenuBehavior.stack.Count > 1)
			{
				MenuBehavior.RejectAll ();
			}
			
			MenuBehavior.RejectAll ();
		}
		
		private static void HandleTimerTimeElapsed(object sender)
		{
			if (MenuBehavior.timerItem != null)
			{
				MenuBehavior.OpenItemSubmenu (MenuBehavior.timerItem, Animate.No);
			}
			if (MenuBehavior.timerBehaviour != null)
			{
				MenuWindow   menu     = MenuBehavior.timerKeepMenu;
				MenuBehavior behavior = MenuBehavior.timerBehaviour;
				
				MenuBehavior.timerKeepMenu = null;
				MenuBehavior.timerBehaviour = null;
				
				if (menu != null)
				{
					behavior.SuspendUpdates ();
					behavior.OpenSubmenu (menu, Animate.No);
					behavior.UpdateItems ();
					behavior.ResumeUpdates ();
				}
			}
		}

		private static void NotifyMenuListChanged()
		{
			if (MenuBehavior.MenuListChanged != null)
			{
				MenuBehavior.MenuListChanged ();
			}
		}
		
		
		private void OnAccepted()
		{
			if (this.Accepted != null)
			{
				this.Accepted (this);
			}
		}
		
		private void OnRejected()
		{
			if (this.Rejected != null)
			{
				this.Rejected (this);
			}
		}
		
		
		private static void Push(MenuBehavior behavior)
		{
			State state = new State (behavior);
			MenuBehavior.stack.Push (state);
		}
		
		private static void Pop(MenuBehavior behavior)
		{
			State state = MenuBehavior.stack.Peek ();
			
			System.Diagnostics.Debug.Assert (state.Behavior == behavior);
			
			MenuBehavior.stack.Pop ();
			
			state.Restore ();
		}
		
		
		#region State Class
		private class State
		{
			public State(MenuBehavior behavior)
			{
				this.behavior = behavior;
				
				this.Save ();
			}
			
			
			public MenuBehavior					Behavior
			{
				get
				{
					return this.behavior;
				}
			}
			
			
			public void Save()
			{
				this.menuList      = new List<MenuBehavior> ();
				this.menuRootList  = new List<MenuBehavior> ();
				
				this.menuList.AddRange (MenuBehavior.menuList);
				this.menuRootList.AddRange (MenuBehavior.menuRootList);
				
				this.menuLastItem     = MenuBehavior.menuLastItem;
				this.menuLastBehavior = MenuBehavior.menuLastBehavior;
				
				this.filterKeyboardOff = MenuBehavior.filterKeyboardOff;
				
//-				System.Diagnostics.Debug.WriteLine (string.Format ("Saved; menu list: {0} items, root list: {1} items", this.menuList.Count, this.menuRootList.Count));
				
				MenuBehavior.menuLastItem = null;
				MenuBehavior.menuLastBehavior = null;
				MenuBehavior.menuList.Clear ();
				MenuBehavior.menuRootList.Clear ();
				MenuBehavior.filterKeyboardOff = 0;

				MenuBehavior.NotifyMenuListChanged ();
			}
			
			public void Restore()
			{
				MenuBehavior.menuList.Clear ();
				MenuBehavior.menuRootList.Clear ();
				
				MenuBehavior.menuList.AddRange (this.menuList);
				MenuBehavior.menuRootList.AddRange (this.menuRootList);
				
				MenuBehavior.menuLastItem     = this.menuLastItem;
				MenuBehavior.menuLastBehavior = this.menuLastBehavior;
				
				MenuBehavior.filterKeyboardOff = this.filterKeyboardOff;
				
//-				System.Diagnostics.Debug.WriteLine (string.Format ("Restored; menu list: {0} items, root list: {1} items", this.menuList.Count, this.menuRootList.Count));

				MenuBehavior.NotifyMenuListChanged ();
			}
			
			
			MenuBehavior						behavior;
			
			List<MenuBehavior>					menuList;
			List<MenuBehavior>					menuRootList;
			MenuItem							menuLastItem;
			MenuBehavior						menuLastBehavior;
			int									filterKeyboardOff;
		}
		#endregion
		
		public event Support.EventHandler		Accepted;
		public event Support.EventHandler		Rejected;

		public static event Support.SimpleCallback MenuListChanged;
		
		static readonly object					syncObject = new object ();
		static long								nextId = 1;
		static int								menuCount;
		static List<MenuBehavior>				menuList;
		static List<MenuBehavior>				menuRootList;
		static MenuItem							menuLastItem;
		static MenuBehavior						menuLastBehavior;
		
		static Stack<State>						stack;
		static Timer							timer;
		
		static MenuItem							timerItem;
		static MenuWindow						timerKeepMenu;
		static MenuBehavior						timerBehaviour;
		
		static int								filterKeyboardOff;
		static Drawing.Point					lastMousePos;
		static bool								keyboardNavigationActive;
		static Drawing.Point					keyboardNavigationMousePos;
		
		private long							id = MenuBehavior.GetNextId ();
		private System.Collections.ArrayList	liveMenuWindows = new System.Collections.ArrayList ();
		private Window							rootWindow;
		private Widget							rootMenu;
		private bool							isOpen;
		private bool							isFrozen;
		
		private bool							keyboardMenuActive;
		private Window							keyboardMenuWindow;
		private MenuItem						keyboardMenuItem;
		
		private MenuItem						frozenMenuLastItem;
		
		private int								localMenuCount;
		private int								suspendUpdates;
		private int								updateRequested;
	}
}
