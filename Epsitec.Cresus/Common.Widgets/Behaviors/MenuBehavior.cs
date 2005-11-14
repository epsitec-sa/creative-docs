//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// La classe MenuBehavior implémente la logique permettant de gérer un menu
	/// (affichage d'une ou plusieurs fenêtres flottantes, fermeture, etc.)
	/// </summary>
	public sealed class MenuBehavior
	{
		public MenuBehavior()
		{
		}
		
		
		
		public void HideAll()
		{
			while (this.live_menu_windows.Count > 0)
			{
				MenuWindow window = this.live_menu_windows[this.live_menu_windows.Count-1] as MenuWindow;
				
				window.Hide ();
			}
		}
		
		public void Reject()
		{
			this.HideAll ();
		}
		
		
		public void AttachMenuWindow(MenuWindow window)
		{
			//	Appelé avant qu'une fenêtre du menu ne devienne visible. A
			//	partir de ce moment, il faut considérer la fenêtre comme en
			//	cours d'affichage.
			
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_window_count++;
				
				if (MenuBehavior.menu_window_count == 1)
				{
					MenuBehavior.RegisterFilter ();
				}
				
				MenuBehavior.menu_list.Remove (this);
				MenuBehavior.menu_list.Add (this);
			}
			
			System.Diagnostics.Debug.Assert (this.live_menu_windows.Contains (window) == false);
			
			this.live_menu_windows.Add (window);
		}
		
		public void DetachMenuWindow(MenuWindow window)
		{
			//	Appelé avant qu'une fenêtre du menu ne soit cachée.
			
			System.Diagnostics.Debug.Assert (this.live_menu_windows.Contains (window) == true);
			System.Diagnostics.Debug.Assert (this.live_menu_windows.IndexOf (window) == this.live_menu_windows.Count-1);
			
			this.live_menu_windows.Remove (window);
			
			Window owner = window.Owner;
			
			if (owner != null)
			{
				owner.MakeActive ();
			}
			
			lock (MenuBehavior.sync_object)
			{
				MenuBehavior.menu_window_count--;
				
				if (MenuBehavior.menu_window_count == 0)
				{
					MenuBehavior.UnregisterFilter ();
				}
				
				if (this.live_menu_windows.Count == 0)
				{
					MenuBehavior.menu_list.Remove (this);
				}
			}
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
		
		private static long GetNextId()
		{
			lock (MenuBehavior.sync_object)
			{
				return MenuBehavior.next_id++;
			}
		}
		
		
		private static Window Detect(Drawing.Point pos)
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
					
					if (window.WindowBounds.Contains (pos))
					{
						return window;
					}
				}
				
				//	Si aucune des fenêtres considérées ne contient le curseur, il
				//	faut encore vérifier si le "owner" de la racine contient un
				//	widget lié au menu behavior :
				
				Widget root = windows[0].ParentWidget;
				
				if ((root != null) &&
					(root.ContainsLocalValue (MenuBehavior.MenuBehaviorProperty)) &&
					(root.Bounds.Contains (root.MapScreenToParent (pos))))
				{
					return root.Window;
				}
			}
			
			return null;
		}
		
		
		private static void MessageFilter(object sender, Message message)
		{
			Window window = sender as Window;
			
			if (message.IsMouseType == false)
			{
				return;
			}
			
			Drawing.Point mouse = window.MapWindowToScreen (message.Cursor);
			Window        menu  = MenuBehavior.Detect (mouse);
			
			switch (message.Type)
			{
				case MessageType.MouseDown:
					if (menu == null)
					{
						MenuBehavior.RejectAll ();
						
						if (message.NonClient == false)
						{
							message.Swallowed = true;
						}
					}
					else
					{
//						MenuItem cell = AbstractMenu.SearchItem(mouse, menu);
//						if ( cell == null )
//						{
//							AbstractMenu.Reject();
//							message.Swallowed = true;
//						}
					}
					break;

				case MessageType.MouseUp:
					if (menu != null)
					{
//						MenuItem cell = AbstractMenu.SearchItem(mouse, menu);
//						if ( cell != null )
//						{
//							if ( cell.Submenu == null && !cell.Separator )
//							{
//								AbstractMenu.ValidateAndExecuteCommand();
//								message.Swallowed = true;
//							}
//						}
					}
					break;
				
				case MessageType.MouseEnter:
				case MessageType.MouseMove:
					if (menu == null)
					{
						message.Swallowed = true;
					}
					break;
			}
		}
		
		private static void HandleApplicationDeactivated(object sender)
		{
			MenuBehavior.RejectAll ();
		}
		
		private static void RejectAll()
		{
			MenuBehavior[] behaviors = MenuBehavior.GetActiveMenuBehaviors ();
			
			foreach (MenuBehavior behavior in behaviors)
			{
				behavior.Reject ();
			}
		}
		
		
		public static readonly Property			MenuBehaviorProperty = Property.RegisterAttached ("MenuBehavior", typeof (MenuBehavior), typeof (MenuBehavior));
		
		static object							sync_object = new object ();
		static long								next_id = 1;
		static int								menu_window_count;
		static System.Collections.ArrayList		menu_list = new System.Collections.ArrayList ();
		
		private long							id = MenuBehavior.GetNextId ();
		private System.Collections.ArrayList	live_menu_windows = new System.Collections.ArrayList ();
	}
}
