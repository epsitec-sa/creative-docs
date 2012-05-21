//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Menu représente un menu (horizontal ou vertical).
	/// </summary>
	public abstract class AbstractMenu : Widget, Collections.IWidgetCollectionHost<MenuItem>
	{
		protected AbstractMenu()
		{
			this.UpdateAdornerInformation ();
			
			this.items = new MenuItemCollection (this);
			
			MenuItem.SetMenuBehavior (this, new Behaviors.MenuBehavior ());
			MenuItem.SetMenuHost (this, new MenuHost (this));
		}
		
		
		public MenuItemCollection				Items
		{
			get
			{
				return this.items;
			}
		}
		
		
		public abstract MenuOrientation			MenuOrientation
		{
			get;
		}
		
		public bool								IsHorizontal
		{
			get
			{
				return this.MenuOrientation == MenuOrientation.Horizontal;
			}
		}

		public bool								IsVertical
		{
			get
			{
				return this.MenuOrientation == MenuOrientation.Vertical;
			}
		}

		public bool								IsMenuOpen
		{
			get
			{
				WindowRoot root = this.RootParent as WindowRoot;
				
				if (root != null)
				{
					return root.Window.IsVisible;
				}
				
				return false;
			}
		}

		public bool								AutoDispose
		{
			get
			{
				return this.autoDisposeOnCleanup;
			}
			set
			{
				this.autoDisposeOnCleanup = value;
			}
		}
		
		public double							IconWidth
		{
			get
			{
				return this.iconWidth;
			}
		}
		
		public Behaviors.MenuBehavior			Behavior
		{
			get
			{
				return MenuItem.GetMenuBehavior (this);
			}
		}
		
		public DependencyObject					Host
		{
			get
			{
				return this.host;
			}
			set
			{
				this.host = value;
			}
		}
		
		
		public Drawing.Margins					MenuShadow
		{
			get
			{
				return this.shadow;
			}
		}
		
		public Drawing.Margins					MenuMargins
		{
			get
			{
				return this.margins;
			}
		}
		
		public virtual void AdjustSize()
		{
			//	Ajuste les dimensions du menu selon son contenu.
			//	Il faut appeler AdjustSize après avoir fini de remplir le
			//	menu.
			
			if (this.IsVertical)
			{
				this.PreferredSize = this.GetBestFitSize ();
				Layouts.LayoutContext.SyncArrange (this);
			}
		}
		
		public void ShowAsContextMenu(Window owner, Drawing.Point pos)
		{
			this.AdjustSize ();
			
			Window.ResetMouseCursor ();
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			
			Layouts.LayoutContext.SyncArrange (this);
			
			pos.Y -= this.ActualHeight;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;
			
			window.Owner          = owner;
			window.WindowLocation = pos;
			
			this.ConnectEventHandlers ();
			this.Behavior.OpenPopup (window, Behaviors.MenuBehavior.Animate.Yes);
			
			this.Focus ();
		}

		public void ShowAsContextMenu(Widget parent, Drawing.Point pos)
		{
			this.AdjustSize ();
			
			Window.ResetMouseCursor ();
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			Window     owner  = parent.Window;

			Layouts.LayoutContext.SyncArrange (this);
			
			pos.Y -= this.ActualHeight;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;
			
			window.Owner          = owner;
			window.WindowLocation = pos;
			window.ParentWidget   = parent;
			
			this.ConnectEventHandlers ();
			this.Behavior.OpenPopup (window, Behaviors.MenuBehavior.Animate.Yes);
			
			this.Focus ();
		}

		public void ShowAsComboList(Widget parent, Drawing.Point pos, Widget button, Behaviors.MenuBehavior.Animate animate=Behaviors.MenuBehavior.Animate.Yes)
		{
			this.AdjustSize ();

			Window.ResetMouseCursor ();
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			Window     owner  = parent.Window;

			Layouts.LayoutContext.SyncArrange (this);
			
			pos.Y -= this.ActualHeight;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;
			
			window.Owner          = owner;
			window.WindowLocation = pos;
			window.ParentWidget   = parent;
			
			if (button != null)
			{
				this.openButton = button;
				this.openButton.SetEngaged (true);
				this.openButton.SetFrozen (true);
			}
			
			this.ConnectEventHandlers ();
			this.Behavior.OpenCombo (window, animate);
			
			this.Focus ();
			this.Window.MakeFocused ();
		}

		
		public override Drawing.Size GetBestFitSize()
		{
			//	Retourne les dimensions nécessaires pour tout le menu,
			//	en partant de l'idée qu'il est vertical.
			
			this.UpdateAdornerInformation ();
			
			double max = 0;
			
			foreach (MenuItem item in this.items)
			{
				max = System.Math.Max (max, item.IconWidth);
			}
			
			this.iconWidth = max;
			
			double dx = 0;
			double dy = 0;
			
			foreach (MenuItem item in this.items)
			{
				item.IconWidth = this.iconWidth;
				
				Drawing.Size rs = item.GetBestFitSize ();
				
				dx  = System.Math.Max (dx, rs.Width);
				dy += rs.Height;
			}
			
			dx += this.shadow.Width  + this.margins.Width;
			dy += this.shadow.Height + this.margins.Height;

			dx = System.Math.Max (this.MinWidth, dx);
			dx = System.Math.Min (this.MaxWidth, dx);
			
			dy = System.Math.Max (this.MinHeight, dy);
			dy = System.Math.Min (this.MaxHeight, dy);

			return new Drawing.Size (dx, dy);
		}

		public override void InvalidateRectangle(Drawing.Rectangle rect, bool sync)
		{
			if (this.IsHorizontal && this.IsVisible)
			{
				Window window = this.Window;
				
				if ((window != null) &&
					(window.IsSyncPaintDisabled == false))
				{
					window.SynchronousRepaint ();
					
					this.RootParent.Invalidate (this.MapClientToRoot (rect));
					
					window.SynchronousRepaint ();
					
					return;
				}
			}
			
			base.InvalidateRectangle (rect, sync);
		}
		
		
		protected override void OnAdornerChanged()
		{
			this.AdjustSize ();
			base.OnAdornerChanged ();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.items != null)
				{
					MenuItem[] items = new MenuItem[this.items.Count];
					
					this.items.CopyTo (items, 0);
					this.items.Clear ();
					
					foreach (MenuItem item in items)
					{
						item.Dispose();
					}
				}
				
				this.DisconnectEventHandlers ();
				
				this.items = null;
				this.host  = null;
			}
			
			base.Dispose (disposing);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			System.Diagnostics.Debug.Assert (this.FindHost() != null, "No Host defined for menu.",
				/**/						 "The menu you are trying to display has no associated command dispatcher host.\n"+
				/**/						 "Use AbstractMenu.Host to define it when you setup the menu.");
		}
		
		
		protected virtual void ConnectEventHandlers()
		{
			if (this.isConnected == false)
			{
				this.Behavior.Accepted += this.HandleBehaviorAccepted;
				this.Behavior.Rejected += this.HandleBehaviorRejected;
				
				this.isConnected = true;
			}
		}
		
		protected virtual void DisconnectEventHandlers()
		{
			if (this.isConnected)
			{
				if (this.openButton != null)
				{
					this.openButton.SetFrozen (false);
					this.openButton.SetEngaged (false);
					this.openButton = null;
				}
				
				this.Behavior.Accepted -= this.HandleBehaviorAccepted;
				this.Behavior.Rejected -= this.HandleBehaviorRejected;
				
				this.isConnected = false;
			}
		}
		
		
		private void HandleBehaviorAccepted(object sender)
		{
			this.OnAccepted ();
			this.CleanUp ();
		}

		private void HandleBehaviorRejected(object sender)
		{
			this.OnRejected ();
			this.CleanUp ();
		}

		private void CleanUp()
		{
			if (this.autoDisposeOnCleanup)
			{
				Application.QueueAsyncCallback (this.Dispose);
			}
			else
			{
				this.DisconnectEventHandlers ();
			}
		}
		
		
		private void UpdateAdornerInformation()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			this.margins = adorner.GeometryMenuMargins;
			this.shadow  = adorner.GeometryMenuShadow;
			
			if (this.IsVertical)
			{
				this.Padding = this.shadow + this.margins;
			}
			else
			{
				this.Padding = Drawing.Margins.Zero;
			}
		}

		private DependencyObject FindHost()
		{
			if (this.host != null)
			{
				return this.host;
			}
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			
			if ((window != null) &&
				(window.ParentWidget != null))
			{
				Widget       parent = window.ParentWidget;
				AbstractMenu menu   = MenuItem.GetMenuRoot (parent) as AbstractMenu;
				
				if (menu != null)
				{
					DependencyObject host = menu.FindHost ();
					
					if (host != null)
					{
						return host;
					}
				}
			}
			
			if (this.Window != null)
			{
				return this.Window;
			}
			
			return null;
		}
		
		
		#region MenuHost Class
		private class MenuHost : IMenuHost
		{
			public MenuHost(AbstractMenu menu)
			{
				this.menu = menu;
			}
			
			
			#region IMenuHost Members
			public void GetMenuDisposition(Widget item, ref Drawing.Size size, out Drawing.Point location, out Animation animation)
			{
				ScreenInfo        screenInfo;
				Drawing.Rectangle workingArea;
				
				if (this.menu.IsHorizontal)
				{
					animation = Animation.RollDown;
					location  = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (0, 0));
					
					location.X -= this.menu.shadow.Left;
					location.Y -= size.Height;
				}
				else
				{
					Drawing.Point test = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (item.ActualWidth, 0));
					
					test.X += size.Width;
					
					screenInfo  = ScreenInfo.Find (test);
					workingArea = screenInfo.WorkingArea;
					
					if (test.X <= workingArea.Right)
					{
						//	Le menu a suffisamment de place pour s'afficher à droite
						//	de son parent :
						
						animation = Animation.RollRight;
						location  = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (item.ActualWidth, item.ActualHeight));
						
						location.X -= this.menu.shadow.Left;
						location.Y -= size.Height - this.menu.shadow.Top - 1;
					}
					else
					{
						//	Il n'y a plus de place à droite; on affiche par conséquent
						//	le sous-menu à gauche :
						
						animation = Animation.RollLeft;
						location  = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (0, item.ActualHeight));
						
						location.X -= size.Width + this.menu.shadow.Left;
						location.Y -= size.Height - this.menu.shadow.Top - 1;
					}
				}
				
				//	Détermine s'il faut décaler verticalement le menu pour qu'il tienne
				//	dans la partie visible de l'écran :
				
				screenInfo  = ScreenInfo.Find (new Drawing.Rectangle (location, size));
				workingArea = screenInfo.WorkingArea;
				
				if (location.Y < workingArea.Bottom)
				{
					animation  = Animation.FadeIn;
					location.Y = workingArea.Bottom;
				}
			}
			#endregion
			
			private AbstractMenu				menu;
		}
		#endregion

		#region IWidgetCollectionHost Members
		Collections.WidgetCollection<MenuItem> Collections.IWidgetCollectionHost<MenuItem>.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(MenuItem item)
		{
			item.PreferredSize = item.GetBestFitSize ();
			item.DefineMenuOrientation (this.MenuOrientation);
			
			this.Children.Add (item);
			
			switch (this.MenuOrientation)
			{
				case MenuOrientation.Horizontal:
					item.Dock    = DockStyle.Left;
					item.Margins = new Drawing.Margins (2, 0, 2, 2);
					break;
				
				case MenuOrientation.Vertical:
					item.Dock    = DockStyle.Top;
					item.Margins = Drawing.Margins.Zero;
					break;
			}
		}

		public void NotifyRemoval(MenuItem item)
		{
			item.DefineMenuOrientation (MenuOrientation.Undefined);
			
			this.Children.Remove (item);
		}

		public void NotifyPostRemoval(MenuItem item)
		{
		}
		#endregion
		
		#region MenuItemCollection Class
		
		public class MenuItemCollection : Collections.WidgetCollection<MenuItem>
		{
			public MenuItemCollection(AbstractMenu menu) : base (menu)
			{
			}

			public void AddItem(Command commandObject)
			{
				this.Add (new MenuItem (commandObject));
			}

			public void AddSeparator()
			{
				this.Add (new MenuSeparator ());
			}
		}
		
		#endregion
		
		protected virtual void OnAccepted()
		{
			var handler = this.GetUserEventHandler ("Accepted");
			if (handler != null)
			{
				handler (this);
			}
		}
		
		protected virtual void OnRejected()
		{
			var handler = this.GetUserEventHandler ("Rejected");
			if (handler != null)
			{
				handler (this);
			}
		}
		
		
		public event EventHandler				Accepted
		{
			add
			{
				this.AddUserEventHandler ("Accepted", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("Accepted", value);
			}
		}

		public event EventHandler				Rejected
		{
			add
			{
				this.AddUserEventHandler ("Rejected", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("Rejected", value);
			}
		}
		
		private Drawing.Margins					margins = new Drawing.Margins (2,2,2,2);
		private Drawing.Margins					shadow  = Drawing.Margins.Zero;
		private MenuItemCollection				items;
		private DependencyObject				host;
		private double							iconWidth;
		private Widget							openButton;
		private bool							isConnected;
		private bool							autoDisposeOnCleanup;
	}
}
