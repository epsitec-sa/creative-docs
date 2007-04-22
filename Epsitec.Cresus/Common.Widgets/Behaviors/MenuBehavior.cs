//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Behaviors
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// La classe MenuBehavior impl�mente la logique permettant de g�rer un menu
	/// (affichage d'une ou plusieurs fen�tres flottantes, fermeture, etc.)
	/// </summary>
	public sealed class MenuBehavior : Types.DependencyObject
	{
		public MenuBehavior()
		{
			//	En principe, on associe � toute une hi�rarchie (menu + sous-menus)
			//	un unique MenuBehavior. Dans la pratique, ce n'est pas n�cessaire
			//	et seul le MenuBehavior associ� � la racine des menus sera utilis�
			//	pour g�rer l'ensemble des (sous-)menus.
		}


		public static bool						HasOpenMenus
		{
			get
			{
				lock (MenuBehavior.sync_object)
				{
					if (MenuBehavior.menu_list.Count > 0)
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
				return this.is_frozen;
			}
			set
			{
				if (this.is_frozen != value)
				{
					this.is_frozen = value;
					
					if (value)
					{
						System.Diagnostics.Debug.Assert (MenuBehavior.menu_last_behavior == this);
						
						//	Quand un menu est "gel�", il ne r�agit plus aux
						//	�v�nements souris. Il faut m�moriser l'�tat courant
						//	pour pouvoir le restaurer au moment o� le menu est
						//	d�gel� :
						
						this.frozen_menu_last_item = MenuBehavior.menu_last_item;
						
						MenuBehavior.menu_last_behavior = null;
						MenuBehavior.menu_last_item     = null;
						
						Behaviors.MenuBehavior.DisableKeyboardFilter ();
					}
					else
					{
						if (this.frozen_menu_last_item != null)
						{
							MenuBehavior.menu_last_behavior = this;
							MenuBehavior.menu_last_item     = this.frozen_menu_last_item;
							
							this.frozen_menu_last_item = null;
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
			//	Ouvre le sous-menu d'un MenuItem donn�. Retourne le MenuBehavior
			//	associ� au sous-menu en question.
			
			//	Si l'item sp�cifi� ne poss�de pas de sous-menu, on garantit au
			//	moins que le (sous-)menu contenant l'item lui-m�me est visible.
			
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
			//	Ferme compl�tement le menu associ� � l'item sp�cifi�. Les fen�tres
			//	correspondant aux (sous-)menus sont simplement cach�es.
			
			MenuBehavior behavior = MenuItem.GetMenuBehavior (item);
			
			if (behavior != null)
			{
				behavior.HideAll ();
			}
			
			return behavior;
		}

		
		private static void DisableKeyboardFilter()
		{
			MenuBehavior.filter_keyboard_off++;
		}
		
		private static void EnableKeyboardFilter()
		{
			if (MenuBehavior.filter_keyboard_off > 0)
			{
				MenuBehavior.filter_keyboard_off--;
			}
		}
		
		
		public void OpenPopup(MenuWindow window, Animate animate)
		{
			//	Montre le menu en animant l'ouverture � la fa�on d'un "pop-up".
			
			this.OpenGenericMenu (window, animate, MenuType.Popup);
		}
		
		public void OpenSubmenu(MenuWindow window, Animate animate)
		{
			//	Montre le menu en animant l'ouverture � la fa�on d'un sous-menu.
			this.OpenGenericMenu (window, animate, MenuType.Submenu);
		}

		public void OpenCombo(MenuWindow window, Animate animate)
		{
			//	Montre le menu en animant l'ouverture � la fa�on d'une combo.
			this.OpenGenericMenu (window, animate, MenuType.ComboList);
		}

		
		private void OpenGenericMenu(MenuWindow window, Animate animate, MenuType type)
		{
			//	Ouvre le sous-menu sp�cifi� par sa fen�tre. Le sous-menu doit
			//	imp�rativement appartenir � notre "menu".
			
			if (window == null)
			{
				//	Il faut cacher toutes les fen�tres encore ouvertes, car ce
				//	cas (window == null) appara�t quand on d�sire montrer un
				//	menu horizontal d�j� visible (racine d'un menu dynamique).
				
				this.HideAll ();
				
				return;
			}
			
			System.Diagnostics.Debug.Assert (window.Behavior == this);
			
			MenuWindow[] live  = (MenuWindow[]) this.live_menu_windows.ToArray (typeof (MenuWindow));
			int          index = this.live_menu_windows.IndexOf (window);
			
			if (index >= 0)
			{
				//	Ce sous-menu est d�j� visible : il faut s'assurer que, s'il a
				//	lui-m�me des sous-menus, on ferme ceux qui doivent �tre cach�s :
				
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
				
				//	Il faut s'assurer que le parent imm�diat est visible avant
				//	de montrer le sous-menu (si c'en est un) :
				
				if (window.MenuType == MenuType.Submenu)
				{
					Widget parent_widget = window.ParentWidget;
					Window parent_window = parent_widget == null ? null : MenuItem.GetMenuWindow (parent_widget);
					
					if (parent_widget != null)
					{
						this.OpenSubmenu (parent_window as MenuWindow, animate);
					}
				}
				
				this.ShowSubmenu (window, animate);
			}
		}
		
		
		public void HideAll()
		{
			//	Ferme tous les menus associ�s � ce gestionnaire.
			
			MenuBehavior.DisableKeyboardNavigation ();
			
			while (this.live_menu_windows.Count > 0)
			{
				MenuWindow window = this.live_menu_windows[this.live_menu_windows.Count-1] as MenuWindow;
				window.Hide ();
			}
			
			this.CleanupAfterClose ();
		}
		
		public void Accept()
		{
			this.HideAll ();
			
			this.OnAccepted ();
		}
		
		public void Reject()
		{
			this.HideAll ();
			
			this.OnRejected ();
		}
		
		
		internal void Attach(Widget root_menu)
		{
			//	Attache un menu racine au gestionnaire de menus. Un seul menu
			//	racine peut �tre attach� � la fois. Attache aussi la fen�tre
			//	contenant le menu racine si le menu est est visible.
			
			System.Diagnostics.Debug.Assert (this.root_menu == null);
			
			this.root_menu = root_menu;
			
			if (this.root_menu.IsVisible)
			{
				this.AttachRootWindow ();
			}
			
			this.root_menu.IsVisibleChanged += this.HandleRootMenuIsVisibleChanged;
		}
		
		internal void Detach(Widget root_menu)
		{
			//	Cf. Attach
			
			System.Diagnostics.Debug.Assert (this.root_menu == root_menu);
			
			this.root_menu.IsVisibleChanged -= this.HandleRootMenuIsVisibleChanged;
			
			if (this.root_menu.IsVisible)
			{
				this.DetachRootWindow ();
			}
			
			this.root_menu = null;
		}
		
		
		internal void AttachMenuItemContainer(MenuItemContainer item)
		{
			this.IsFrozen = true;
			this.UpdateItems ();
			
			System.Diagnostics.Debug.Assert (this.frozen_menu_last_item == item);
		}
		
		internal void DetachMenuItemContainer(MenuItemContainer item)
		{
			System.Diagnostics.Debug.Assert (this.frozen_menu_last_item == item);
			
			this.IsFrozen = false;
			this.UpdateItems ();
		}
		
		
		private void ShowSubmenu(MenuWindow window, Animate animate)
		{
			//	Montre un sous-menu en le positionnant au pr�alable par rapport
			//	� son parent, de la mani�re la plus judicieuse possible.
			
			Widget       parent = window.ParentWidget;
			Drawing.Size size   = window.Root.GetBestFitSize ();
			
			//	D�termine la position o� afficher le menu...
			
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
					if (this.live_menu_windows.Count > 0)
					{
						animation = Animation.None;
					}
					break;
			}
			
			ToolTip.HideAllToolTips ();
			
			window.WindowSize     = size;
			window.WindowLocation = location;
			window.AnimateShow (animation);
			
			this.is_open = true;
			
			if (this.keyboard_menu_window == null)
			{
				this.keyboard_menu_window = window;
			}
		}
		
		private void CleanupAfterClose()
		{
			//	Apr�s la fermeture compl�te d'un menu, stoppe le timer et remet
			//	le gestionnaire de menus dans un �tat "neutre".
			
			MenuBehavior.timer.Stop ();
			MenuBehavior.timer_item      = null;
			MenuBehavior.timer_keep_menu = null;
			MenuBehavior.timer_behaviour = null;
			
			if (this.IsFrozen)
			{
				this.ChangeFocusedItem (null);
			}
			
			System.Diagnostics.Debug.Assert (this.is_frozen == false);
			System.Diagnostics.Debug.Assert (this.frozen_menu_last_item == null);
			
			this.is_open = false;
			this.keyboard_menu_active = false;
		}
		
		
		internal void HandleMenuItemPressed(MenuItem item)
		{
			MenuBehavior.DisableKeyboardNavigation ();
			
			this.CleanupAfterClose ();
			this.OnAccepted ();
		}
		
		internal void HandleAboutToShowMenuWindow(MenuWindow window)
		{
			//	Appel� avant qu'une fen�tre du menu ne devienne visible. A
			//	partir de ce moment, il faut consid�rer la fen�tre comme en
			//	cours d'affichage.
			
			this.AttachMenu ();
			
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_list.Remove (this);
				MenuBehavior.menu_list.Add (this);
			}
			
			MenuBehavior.NotifyMenuListChanged ();
			
			System.Diagnostics.Debug.Assert (this.live_menu_windows.Contains (window) == false);
			
			this.live_menu_windows.Add (window);
			this.UpdateItems ();
		}
		
		internal void HandleAboutToHideMenuWindow(MenuWindow window)
		{
			//	Appel� avant qu'une fen�tre du menu ne soit cach�e.
			
			System.Diagnostics.Debug.Assert (this.live_menu_windows.Contains (window) == true);
			System.Diagnostics.Debug.Assert (this.live_menu_windows.IndexOf (window) == this.live_menu_windows.Count-1);
			
			this.live_menu_windows.Remove (window);
			this.UpdateItems ();
			
			//	Si une fen�tre poss�de le focus, il faut avant de la cacher activer
			//	la fen�tre parent, pour �viter des clignotements de fen�tres :
			
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
			
			if (this.live_menu_windows.Count == 0)
			{
				lock (MenuBehavior.sync_object)
				{
					MenuBehavior.menu_list.Remove (this);
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

			MenuBehavior.menu_list      = new List<MenuBehavior> ();
			MenuBehavior.menu_root_list = new List<MenuBehavior> ();
			
			MenuBehavior.timer.TimeElapsed += MenuBehavior.HandleTimerTimeElapsed;
		}
		
		
		private void AttachMenu()
		{
			this.local_menu_count++;
			
			if (this.local_menu_count == 1)
			{
				lock (MenuBehavior.sync_object)
				{
					MenuBehavior.menu_count++;
				
					if (MenuBehavior.menu_count == 1)
					{
						MenuBehavior.RegisterFilter ();
					}
				}
				
				MenuBehavior.Push (this);
			}
		}
		
		private void DetachMenu()
		{
			this.local_menu_count--;
			
			if (this.local_menu_count == 0)
			{
				MenuBehavior.Pop (this);
				
				lock (MenuBehavior.sync_object)
				{
					MenuBehavior.menu_count--;
				
					if (MenuBehavior.menu_count == 0)
					{
						MenuBehavior.UnregisterFilter ();
						MenuBehavior.GenerateDummyMouseMoveEvent ();
					}
				}
			}
		}
		
		
		private void AttachRootWindow()
		{
			//	Quand la fen�tre contenant le menu racine devient visible, on
			//	active le filtre, etc. comme si on avait ouvert un menu.
			
			this.AttachMenu ();
			
			this.root_window = this.root_menu.Window;
			
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_root_list.Add (this);
			}
		}
		
		private void DetachRootWindow()
		{
			//	Cf. AttachRootWindow
			
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_root_list.Remove (this);
			}
			
			this.root_window = null;
			
			this.DetachMenu ();
		}
		
		
		private void HandleRootMenuIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Quand le menu racine devient visible, on s'attache � sa fen�tre
			//	via AttachRootWindow, et vice versa.
			
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
			if (item == null)
			{
				return;
			}
			
			if (this.keyboard_menu_item != item)
			{
				this.keyboard_menu_item   = item;
				this.keyboard_menu_window = item.Window;
			}
			
			if (this.is_open == false)
			{
				this.UpdateItems ();
			}
			else if (MenuItem.GetZeroDelay (item))
			{
				MenuBehavior.timer.Suspend ();
				
				MenuBehavior.timer_item      = null;
				MenuBehavior.timer_keep_menu = null;
				MenuBehavior.timer_behaviour = null;
				
				MenuBehavior.OpenItemSubmenu (item, Animate.No);
			}
			else
			{
				MenuBehavior.timer.Suspend ();
				
				MenuBehavior.timer_item      = item;
				MenuBehavior.timer_keep_menu = null;
				MenuBehavior.timer_behaviour = null;

				MenuBehavior.timer.Delay = SystemInformation.MenuShowDelay;
				MenuBehavior.timer.Start ();
				
				this.UpdateItems ();
			}
		}
		
		private void NotifyExitedItem(MenuItem item)
		{
			this.keyboard_menu_item   = null;
			this.keyboard_menu_window = null;
			
			if (this.live_menu_windows.Count > 0)
			{
				//	Il reste probablement des fen�tres de sous-menu �
				//	refermer :
				
				MenuBehavior.timer.Suspend ();
				
				MenuBehavior.timer_item      = null;
				MenuBehavior.timer_keep_menu = MenuItem.GetMenuWindow (item) as MenuWindow;
				MenuBehavior.timer_behaviour = this;
				
				MenuBehavior.timer.Delay = SystemInformation.MenuShowDelay;
				MenuBehavior.timer.Start ();
			}
			
			this.UpdateItems ();
		}
		

		private void KeyboardCloseLatestMenu()
		{
			int n = this.live_menu_windows.Count;
			
			if (n > 0)
			{
				MenuWindow window = this.live_menu_windows[n-1] as MenuWindow;
				
				this.keyboard_menu_item   = ((window.ParentWidget != null) && (window.ParentWidget.RootParent is WindowRoot)) ? window.ParentWidget as MenuItem : null;
				this.keyboard_menu_window = (this.keyboard_menu_item == null) ? null : this.keyboard_menu_item.Window;
				this.keyboard_menu_active = true;
				
				if (this.keyboard_menu_window != null)
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
				this.keyboard_menu_item   = null;
				this.keyboard_menu_window = null;
				this.keyboard_menu_active = false;
				
				this.is_open = false;
				
				MenuBehavior.DisableKeyboardNavigation ();
			}
			
			MenuBehavior.menu_last_item     = this.keyboard_menu_item;
			MenuBehavior.menu_last_behavior = this.keyboard_menu_item == null ? null : this;
			
			this.UpdateItems ();
		}
		
		private void KeyboardSelectItem(int direction)
		{
			//	S�lectionne un �l�ment du m�me menu � la suite d'un d�placement
			//	avec les touches haut/bas.
			
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
				//	ouvrir celui correspondant � la case s�lectionn�e :
				
				MenuBehavior.OpenItemSubmenu (this.keyboard_menu_item, Animate.No);
				MenuBehavior.EnableKeyboardNavigation ();
				
				n = this.live_menu_windows.Count;
				
				if (n > 0)
				{
					this.keyboard_menu_window = this.live_menu_windows[n-1] as MenuWindow;
					this.keyboard_menu_item   = this.FindFirstItem (this.keyboard_menu_window.Root);
				}
			}
			
			this.UpdateItemsAfterKeyboardAction (this.keyboard_menu_item);
		}
		
		private void KeyboardSelectMenu(int direction)
		{
			//	Navigation avec les touches gauche/droite, pour passer de menu
			//	en menu, dans le cas d'un menu hi�rarchique.
			
			int      n      = this.live_menu_windows.Count;
			MenuItem item   = null;
			Window   window = null;
			
			if ((n > 0) &&
				((this.root_window == this.keyboard_menu_window) || (MenuBehavior.menu_last_item == null)))
			{
				//	Aucune case s�lectionn�e dans le menu, ou alors c'est une case
				//	du menu principal. Dans ce cas, il faut commencer par s�lec-
				//	tionner la premi�re case du premier sous-menu :
				
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
				//	D�placement horizontal dans le menu de base. Aucun sous-menu
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
				//	D�placement horizontal avec un ou plusieurs sous-menus ouverts.
				//	En fonction du niveau, il faudra soit ouvrir un sous-menu, soit
				//	passer au menu suivant/pr�c�dent :
				
				item = null;
				
				if ((this.keyboard_menu_item != null) &&
					(this.keyboard_menu_item.RootParent is WindowRoot))
				{
					item = this.keyboard_menu_item;
				}
				
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
						//	Tente d'ouvrir un sous-menu pour l'�l�ment en cours.
						
						Widget submenu = MenuItem.GetSubmenu (item);
						
						if (submenu != null)
						{
							MenuBehavior.OpenItemSubmenu (item, Animate.No);
							MenuBehavior.EnableKeyboardNavigation ();
							
							n = this.live_menu_windows.Count - 1;
							
							if (n > 0)
							{
								window = this.live_menu_windows[n] as MenuWindow;
								item   = this.FindFirstItem (window.Root);
							}
						}
						else if (this.root_menu != null)
						{
							//	Il n'y a pas de sous-menu pour l'�l�ment en cours.
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
									MenuBehavior.EnableKeyboardNavigation ();
									
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
							//	rabattre. Montre le menu pr�c�dent :
							
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
									MenuBehavior.EnableKeyboardNavigation ();
									
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
				
				this.UpdateItemsAfterKeyboardAction (item);
			}
		}
		
		private void UpdateItemsAfterKeyboardAction(MenuItem item)
		{
			this.SuspendUpdates ();
			
			MenuItem old_item = MenuBehavior.menu_last_item;
			MenuItem new_item = item;
			
			MenuBehavior.menu_last_item     = item;
			MenuBehavior.menu_last_behavior = (item == null) ? null : this;
			
			if (old_item != new_item)
			{
				this.ChangeFocusedItem (new_item);
			}
			
			this.keyboard_menu_active = true;
			
			this.UpdateItems ();
			this.ResumeUpdates ();
		}
		
		private bool ChangeFocusedItem(MenuItem new_item)
		{
			//	Change le focus d'un item � un autre, tout en tenant compte des
			//	�ventuels items "riches" qui n�cessitent un traitement sp�cial.
			
			MenuItemContainer old_container = this.frozen_menu_last_item as MenuItemContainer;
			MenuItemContainer new_container = new_item as MenuItemContainer;
			
			if (old_container != null)
			{
				//	Un item "riche" a le focus et il faut donc lui signaler qu'il va le
				//	perdre (ce qui va mettre IsFrozen � 'false' et proc�der � diverses
				//	modifications) :
				
				old_container.DefocusFromMenu ();
				
				//	Les r�glages de menu_last_... peuvent avoir �t� modifi�s depuis
				//	notre appel. On les applique � nouveau ici pour �tre s�r :
				
				MenuBehavior.menu_last_item     = new_item;
				MenuBehavior.menu_last_behavior = (new_item == null) ? null : this;
			}
			
			System.Diagnostics.Debug.Assert (this.is_frozen == false);
			System.Diagnostics.Debug.Assert (this.frozen_menu_last_item == null);
			
			if (new_container != null)
			{
				//	Un item "riche" va recevoir le focus :
				
				return new_container.FocusFromMenu ();
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
							MenuItem.SetItemType (widget, MenuItemType.Selected);
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
							MenuItem.SetItemType (widget, MenuItemType.Selected);
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
			
			if ((hilite_below_i > 0) &&
				(MenuBehavior.timer_keep_menu != null))
			{
				//	La souris est sortie du menu sp�cifi� par timer_keep_menu et
				//	on ne doit donc pas peindre la case mise en �vidence pour le
				//	sous-menu ouvert, s'il y en a un.
				
				for (int i = 0; i < hilite_below_i; i++)
				{
					if (windows[i] == MenuBehavior.timer_keep_menu)
					{
						hilite_below_i = i+1;
						break;
					}
				}
			}
			
			for (int i = 0; i < hilite_below_i; i++)
			{
				MenuWindow menu = windows[i];
				
				Widget widget = menu.ParentWidget;
				
				if (widget != null)
				{
					MenuItem.SetItemType (widget, MenuItemType.SubmenuOpen);
				}
				
				list.Remove (widget);
			}
			
			foreach (Widget widget in list)
			{
				MenuItem.SetItemType (widget, MenuItemType.Default);
			}
			
			this.suspend_updates--;
			this.update_requested = 0;
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
		
		private static void GenerateDummyMouseMoveEvent()
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
			lock (MenuBehavior.sync_object)
			{
				return MenuBehavior.menu_list.ToArray ();
			}
		}
		
		private static MenuBehavior[] GetRootMenuBehaviors()
		{
			lock (MenuBehavior.sync_object)
			{
				return MenuBehavior.menu_root_list.ToArray ();
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
				
				//	Analyse la cha�ne des fen�tres correspondant au menu d�roul�,
				//	en commen�ant par la fin (dernier �l�ment affich� = premier �
				//	�tre consid�r�) :
				
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
			
			//	Peut-�tre y a-t-il une fen�tre contenant un menu racine toujours
			//	visible � l'�cran ?
			
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
			if ((MenuBehavior.menu_list.Count == 0) &&
				(MenuBehavior.menu_root_list.Count == 0))
			{
				//	Rien � filtrer si aucun menu n'est actuellement visible. Il
				//	est primordial de faire le test imm�diatement, pour �viter
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
				if ((MenuBehavior.menu_list.Count > 0) &&
					(window.CapturingWidget != null))
				{
					//	Quelqu'un a captur� la souris... On d�sactive cette capture,
					//	� moins que la fen�tre responsable de la capture soit elle-
					//	m�me une fen�tre appartenant au menu actif :
					
					int  n       = MenuBehavior.menu_list.Count;
					bool release = true;
					
					if (n > 0)
					{
						MenuBehavior that = MenuBehavior.menu_list[n-1];
						
						foreach (Window menu in that.live_menu_windows)
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
				if (MenuBehavior.filter_keyboard_off > 0)
				{
					return;
				}
				
				MenuWindow menu_window = window as MenuWindow;
				
				if ((menu_window != null) &&
					(menu_window.MenuType == MenuType.ComboList))
				{
					return;
				}
				
				MenuBehavior.ProcessKeyboardEvent (window, message);
			}
		}
		
		private static void ProcessMouseEvent(Window window, Message message)
		{
			Drawing.Point mouse = window.MapWindowToScreen (message.Cursor);
			
			MenuBehavior.last_mouse_pos = mouse;
			
			//	D�termine dans quelle fen�tre appartenant � un menu la souris se
			//	trouve ('menu' ou 'root' si la souris se trouve dans la fen�tre
			//	racine d'un menu hi�rarchique).
			
			Window   menu = MenuBehavior.DetectWindow (mouse);
			Window   root = menu == null ? MenuBehavior.DetectRootWindow (mouse) : null;
			MenuItem item = null;
			
			//	D�termine dans quel MenuItem la souris se trouve actuellement :
			
			if ((menu != null) ||
				(root != null))
			{
				item = MenuBehavior.DetectMenuItem (window, message.Cursor);
			}
			
			//	Par d�faut, on consomme l'�v�nement lorsque toutes les conditions
			//	suivantes sont remplies :
			//
			//	- Un menu est affich�
			//	- La souris se trouve dans une partie "client" d'une fen�tre
			//	- La souris se trouve hors d'une fen�tre appartenant � un menu

			bool mouse_in_menu   = (item != null) || (menu != null);
			bool swallow_message = (MenuBehavior.menu_list.Count > 0) && (message.NonClient == false) && !mouse_in_menu;
			
			
			//	Pour r�sumer : un clic hors du menu ferme le menu; un clic dans
			//	le menu est trait� par le menu item lui-m�me. Un rel�chement de
			//	bouton dans un menu item est consid�r� comme un clic.
			
			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					if (!mouse_in_menu)
					{
						//	L'utilisateur a cliqu� hors de tout menu actuellement
						//	ouvert. Si un menu est actuellement ouvert, il faut le
						//	refermer :
						
						if (MenuBehavior.menu_list.Count > 0)
						{
							MenuBehavior.RejectActive ();
						}
					}
					else if (mouse_in_menu && (root == null))
					{
						//	L'utilisateur a cliqu� dans une fen�tre appartenant �
						//	un menu (autre que celui de la racine); on va changer
						//	le focus si un item "riche" a �t� cliqu� :
						
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
						//	L'utilisateur a cliqu� dans la fen�tre racine, dans la
						//	surface du menu racine. S'il y a des menus ouverts, il
						//	faut les fermer :
						
						if (MenuBehavior.menu_list.Count > 0)
						{
							MenuBehavior.RejectActive ();
							swallow_message = true;
						}
					}
					break;
				
				case MessageType.MouseUp:
					if (item != null)
					{
						if ((item is MenuItemContainer) ||
							(item is MenuSeparator))
						{
							//	Rien � faire (on transmet le message plus loin) si
							//	la souris se trouve dans un s�parateur ou dans un
							//	item "riche".
						}
						else
						{
							//	Sinon, simule une pression du MenuItem et consomme
							//	l'�v�nement.
							
							item.SimulateClicked ();
							
							swallow_message = true;
						}
					}
					break;
				
				case MessageType.MouseEnter:
				case MessageType.MouseLeave:
				case MessageType.MouseMove:
					if ((MenuBehavior.keyboard_navigation_active) &&
						(Drawing.Point.Distance (mouse, MenuBehavior.keyboard_navigation_mouse_pos) < 4))
					{
						//	En cas de navigation au clavier, on ne tient pas compte
						//	des petits d�placements de souris.
						
						swallow_message = true;
					}
					else
					{
						//	...sinon, on traite l'�ventuel changement d'item actif :
						
						MenuBehavior.ProcessMouseMoveInItem (item);
					}
					break;
			}
			
			if (swallow_message)
			{
				message.Swallowed = true;
			}
		}
		
		private static void ProcessMouseMoveInItem(MenuItem item)
		{
			//	Signale que la souris est dans le menu item sp�cifi�. S'il y a
			//	eu un changement depuis la derni�re fois, il faut mettre � jour
			//	les menus correspondants.
			
			if (MenuBehavior.menu_last_item == item)
			{
				return;
			}
			
			//	Si la souris est sortie du menu, mais qu'une navigation au clavier
			//	est active, on ignore le changement :
			
			if ((item == null) &&
				(MenuBehavior.keyboard_navigation_active))
			{
				return;
			}
			
			//	Du moment qu'il y a eu un d�placement de souris suffisamment
			//	important, on d�sactive la navigation au clavier :
			
			MenuBehavior.DisableKeyboardNavigation ();
			
			//	Nous allons mettre � jour (peut-�tre en plusieurs �tapes) les
			//	�tats de s�lection des menu items. Pour �viter de clignoter, on
			//	suspend (�ventuellement) le redessin temporairement.
			
			MenuBehavior suspended_behavior = null;
			
			//	S'il y avait un menu item s�lectionn�, on commence par signaler
			//	que la souris a quitt� cet item.
			
			if (MenuBehavior.menu_last_item != null)
			{
				MenuItem     last_item = MenuBehavior.menu_last_item;
				MenuBehavior behavior  = MenuBehavior.menu_last_behavior;
				
				MenuBehavior.menu_last_behavior = null;
				MenuBehavior.menu_last_item     = null;
				
				System.Diagnostics.Debug.Assert (behavior.IsFrozen == false);
				
				behavior.SuspendUpdates ();
				behavior.NotifyExitedItem (last_item);
				
				suspended_behavior = behavior;
			}
			
			//	S'il y a un nouvel item qui contient la souris, on signale qu'il
			//	faut le s�lectionner.
			
			if (item != null)
			{
				MenuBehavior behavior = MenuItem.GetMenuBehavior (item);
				
				if (behavior.IsFrozen == false)
				{
					MenuBehavior.menu_last_behavior = behavior;
					MenuBehavior.menu_last_item     = item;
					
					behavior.SuspendUpdates ();
					behavior.NotifyEnteredItem (MenuBehavior.menu_last_item);
					behavior.ResumeUpdates ();
				}
			}
			
			if (suspended_behavior != null)
			{
				suspended_behavior.ResumeUpdates ();
			}
		}
		
		
		private static void ProcessKeyboardEvent(Window window, Message message)
		{
			MenuBehavior that = MenuBehavior.menu_last_behavior;
			
			if ((that == null) &&
				(MenuBehavior.menu_list.Count > 0))
			{
				that = MenuBehavior.menu_list[MenuBehavior.menu_list.Count-1];
			}
			
			if (that == null)
			{
				return;
			}
			
			//	S'il n'y a aucun menu ouvert, on ne traite les �v�nements du
			//	clavier uniquement si une navigation avait �t� activ�e au
			//	pr�alable :
			
			if ((that.is_open == false) &&
				(that.keyboard_menu_active == false))
			{
				return;
			}
			
			MenuBehavior.EnableKeyboardNavigation ();
			
			that.SuspendUpdates ();
			
			bool swallow_message = true;
			
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
			
			if (swallow_message)
			{
				message.Swallowed = true;
			}
		}
		
		
		private static void EnableKeyboardNavigation()
		{
			MenuBehavior.keyboard_navigation_active    = true;
			MenuBehavior.keyboard_navigation_mouse_pos = MenuBehavior.last_mouse_pos;
		}
		
		private static void DisableKeyboardNavigation()
		{
			if (MenuBehavior.keyboard_navigation_active)
			{
				MenuBehavior.keyboard_navigation_active = false;
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
			if (MenuBehavior.timer_item != null)
			{
				MenuBehavior.OpenItemSubmenu (MenuBehavior.timer_item, Animate.No);
			}
			if (MenuBehavior.timer_behaviour != null)
			{
				MenuWindow   menu     = MenuBehavior.timer_keep_menu;
				MenuBehavior behavior = MenuBehavior.timer_behaviour;
				
				MenuBehavior.timer_keep_menu = null;
				MenuBehavior.timer_behaviour = null;
				
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
				this.menu_list      = new List<MenuBehavior> ();
				this.menu_root_list = new List<MenuBehavior> ();
				
				this.menu_list.AddRange (MenuBehavior.menu_list);
				this.menu_root_list.AddRange (MenuBehavior.menu_root_list);
				
				this.menu_last_item     = MenuBehavior.menu_last_item;
				this.menu_last_behavior = MenuBehavior.menu_last_behavior;
				
				this.filter_keyboard_off = MenuBehavior.filter_keyboard_off;
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Saved; menu list: {0} items, root list: {1} items", this.menu_list.Count, this.menu_root_list.Count));
				
				MenuBehavior.menu_last_item = null;
				MenuBehavior.menu_last_behavior = null;
				MenuBehavior.menu_list.Clear ();
				MenuBehavior.menu_root_list.Clear ();
				MenuBehavior.filter_keyboard_off = 0;

				MenuBehavior.NotifyMenuListChanged ();
			}
			
			public void Restore()
			{
				MenuBehavior.menu_list.Clear ();
				MenuBehavior.menu_root_list.Clear ();
				
				MenuBehavior.menu_list.AddRange (this.menu_list);
				MenuBehavior.menu_root_list.AddRange (this.menu_root_list);
				
				MenuBehavior.menu_last_item     = this.menu_last_item;
				MenuBehavior.menu_last_behavior = this.menu_last_behavior;
				
				MenuBehavior.filter_keyboard_off = this.filter_keyboard_off;
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Restored; menu list: {0} items, root list: {1} items", this.menu_list.Count, this.menu_root_list.Count));

				MenuBehavior.NotifyMenuListChanged ();
			}
			
			
			MenuBehavior						behavior;
			
			List<MenuBehavior>					menu_list;
			List<MenuBehavior>					menu_root_list;
			MenuItem							menu_last_item;
			MenuBehavior						menu_last_behavior;
			int									filter_keyboard_off;
		}
		#endregion
		
		public event Support.EventHandler		Accepted;
		public event Support.EventHandler		Rejected;

		public static event Support.SimpleCallback MenuListChanged;
		
		static readonly object					sync_object = new object ();
		static long								next_id = 1;
		static int								menu_count;
		static List<MenuBehavior>				menu_list;
		static List<MenuBehavior>				menu_root_list;
		static MenuItem							menu_last_item;
		static MenuBehavior						menu_last_behavior;
		
		static Stack<State>						stack;
		static Timer							timer;
		
		static MenuItem							timer_item;
		static MenuWindow						timer_keep_menu;
		static MenuBehavior						timer_behaviour;
		
		static int								filter_keyboard_off;
		static Drawing.Point					last_mouse_pos;
		static bool								keyboard_navigation_active;
		static Drawing.Point					keyboard_navigation_mouse_pos;
		
		private long							id = MenuBehavior.GetNextId ();
		private System.Collections.ArrayList	live_menu_windows = new System.Collections.ArrayList ();
		private Window							root_window;
		private Widget							root_menu;
		private bool							is_open;
		private bool							is_frozen;
		
		private bool							keyboard_menu_active;
		private Window							keyboard_menu_window;
		private MenuItem						keyboard_menu_item;
		
		private MenuItem						frozen_menu_last_item;
		
		private int								local_menu_count;
		private int								suspend_updates;
		private int								update_requested;
	}
}
