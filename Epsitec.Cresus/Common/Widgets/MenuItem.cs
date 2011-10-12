//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Helpers;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MenuItem représente une case dans un menu.
	/// </summary>
	public class MenuItem : AbstractButton
	{
		public MenuItem()
		{
			this.AutoCapture = false;
			this.AutoFocus   = false;
			this.AutoEngage  = false;
			
			this.InternalState &= ~WidgetInternalState.Focusable;
			this.InternalState &= ~WidgetInternalState.Engageable;
			
			this.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
			
			this.icon     = new TextLayout();
			this.shortcut = new TextLayout();
			
			this.icon.Alignment     = Drawing.ContentAlignment.MiddleLeft;
			this.shortcut.Alignment = Drawing.ContentAlignment.MiddleLeft;
			
			this.submenuMarkWidth = Widget.DefaultFontHeight;
		}
		
		public MenuItem(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}

		public MenuItem(Command command)
			: this ()
		{
			this.CommandObject = command;
		}
		
		public MenuItem(string command, string text) : this ()
		{
			this.CommandObject = Command.Get (command);
			this.Text     = text;
			this.textOnly = true;
		}
		
		public MenuItem(string command, string icon, string text, string shortcut) : this ()
		{
			this.CommandObject = Command.Get (command);
			this.IconUri  = icon;
			this.Text     = text;
			this.ShortKey = shortcut;	//#fix
			this.textOnly = false;
		}
		
		public MenuItem(string command, string icon, string text, string shortcut, string name) : this (command, icon, text, shortcut)
		{
			this.Name = name;
		}
		
		public MenuItem(Widget submenu, string icon, string text, string shortcut) : this ()
		{
			this.IconUri  = icon;
			this.Text     = text;
			this.ShortKey = shortcut;	//#fix
			this.textOnly = false;
			this.Submenu  = submenu;
		}
		
		public MenuItem(Widget submenu, string icon, string text, string shortcut, string name) : this (submenu, icon, text, shortcut)
		{
			this.Name = name;
		}

		
		public virtual bool						IsSeparator
		{
			get
			{
				return false;
			}
		}

		
		public MenuOrientation					MenuOrientation
		{
			get
			{
				return (MenuOrientation) this.GetValue (MenuItem.MenuOrientationProperty);
			}
		}
		
		public MenuItemState					ItemState
		{
			get
			{
				return (MenuItemState) this.GetValue (MenuItem.ItemStateProperty);
			}

			set
			{
				this.SetValue (MenuItem.ItemStateProperty, value);
			}
		}

		
		public string							IconUriActiveNo
		{
			get
			{
				return this.iconUriActiveNo;
			}

			set
			{
				if (this.iconUriActiveNo != value)
				{
					this.iconUriActiveNo = value;
					this.UpdateIconSize ();
				}
			}
		}
		
		public string							IconUriActiveYes
		{
			get
			{
				return this.iconUriActiveYes;
			}

			set
			{
				if (this.iconUriActiveYes != value)
				{
					this.iconUriActiveYes = value;
					this.UpdateIconSize ();
				}
			}
		}

		
		public string							ShortKey
		{
			get
			{
				return this.shortcut.Text;
			}

			set
			{
				this.shortcut.Text = value;
				this.shortcutSize = this.AdjustSize (this.shortcut.SingleLineSize);
			}
		}

		public Widget							Submenu
		{
			get
			{
				return this.GetValue (MenuItem.SubmenuProperty) as Widget;
			}

			set
			{
				this.SetValue (MenuItem.SubmenuProperty, value);
			}
		}

		
		public double							IconWidth
		{
			get
			{
				return this.iconSize.Width;
			}
			set
			{
				if (this.iconSize.Width != value)
				{
					this.iconSize.Width = value;
					this.OnIconSizeChanged ();
				}
			}
		}
		
		public Drawing.Size						IconSize
		{
			get
			{
				return this.iconSize;
			}
			set
			{
				if (this.iconSize != value)
				{
					this.iconSize = value;
					this.OnIconSizeChanged ();
				}
			}
		}
		
		
		internal void DefineMenuOrientation(MenuOrientation value)
		{
			this.SetValue (MenuItem.MenuOrientationProperty, value);
		}
		
		
		
		public static MenuItem CreateYesNo(string command, string text, string shortcut, string name)
		{
			MenuItem item = new MenuItem (command, "", text, shortcut, name);
			
			item.IconUriActiveNo  = "manifest:Epsitec.Common.Widgets.Images.ActiveNo.icon";
			item.IconUriActiveYes = "manifest:Epsitec.Common.Widgets.Images.ActiveYes.icon";
			
			return item;
		}
		
		
		
		private void UpdateIconSize()
		{
			double dx = 0;
			double dy = 0;
			
			if ((this.iconUriActiveNo != null) &&
				(this.iconUriActiveNo.Length > 0))
			{
				TextLayout layout = this.GetIconTextLayout (this.iconUriActiveNo);
				Drawing.Size size = layout.SingleLineSize;
				
				dx = System.Math.Max (dx, size.Width);
				dy = System.Math.Max (dy, size.Height);
			}
			
			if ((this.iconUriActiveYes != null) &&
				(this.iconUriActiveYes.Length > 0))
			{
				TextLayout layout = this.GetIconTextLayout (this.iconUriActiveYes);
				Drawing.Size size = layout.SingleLineSize;
				
				dx = System.Math.Max (dx, size.Width);
				dy = System.Math.Max (dy, size.Height);
			}
			
			if (this.ContainsValue (Widget.IconUriProperty))
			{
				TextLayout layout = this.GetIconTextLayout (this.IconUri);
				Drawing.Size size = layout.SingleLineSize;
				
				dx = System.Math.Max (dx, size.Width);
				dy = System.Math.Max (dy, size.Height);
			}
			
			this.IconSize = this.AdjustSize (new Drawing.Size (dx, dy));
		}
		
		private string GetIconText(string icon)
		{
			return string.Concat (@"<img src=""", icon, @"""/>");
		}
		
		private TextLayout GetIconTextLayout(string icon)
		{
			TextLayout layout = new TextLayout ();
			
			layout.Alignment = Drawing.ContentAlignment.MiddleLeft;
			layout.Text      = this.GetIconText (icon);
			
			return layout;
		}

		private Drawing.Size AdjustSize(Drawing.Size size)
		{
			size.Width  = System.Math.Ceiling(size.Width);
			size.Height = System.Math.Ceiling(size.Height);

			if ( !this.textOnly )
			{
				size.Width  += MenuItem.MarginItem*2;
				size.Height += MenuItem.MarginItem*2;
			}
			
			return size;
		}

		
		public override Drawing.Size GetBestFitSize()
		{
			double dx = 0;
			double dy = 0;

			if (this.textOnly)
			{
				dx = MenuItem.MarginHeader*2 + this.mainTextSize.Width;
				dy = this.mainTextSize.Height;
			}
			else if (this.IsSeparator)
			{
				dy = MenuItem.SeparatorHeight;
			}
			else
			{
				dx += this.iconSize.Width;
				dx += this.mainTextSize.Width;
				dx += MenuItem.MarginSpace;
				dx += this.shortcutSize.Width;
				dx += this.submenuMarkWidth;

				dy = System.Math.Max(dy, this.iconSize.Height);
				dy = System.Math.Max(dy, this.mainTextSize.Height);
				dy = System.Math.Max(dy, this.shortcutSize.Height);
			}
			
			return new Drawing.Size (dx, dy);
		}


		protected override void QueueCommandForExecution(Command command, CommandState state)
		{
			state.AdvancedState = this.Name;
			base.QueueCommandForExecution (command, state);
		}
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			this.mainTextSize = this.AdjustSize (this.TextLayout.SingleLineSize);
		}

		protected override void OnClicked(MessageEventArgs e)
		{
			this.OnUserAction (e);
			base.OnClicked (e);
		}

		protected override void OnPressed(MessageEventArgs e)
		{
			base.OnPressed (e);
		}

		protected override void OnReleased(MessageEventArgs e)
		{
			base.OnReleased (e);
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);
		}
		
		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);
		}

		protected override void OnIconUriChanged(string oldIconUri, string newIconUri)
		{
			base.OnIconUriChanged (oldIconUri, newIconUri);
			
			this.UpdateIconSize ();
		}

		
		protected virtual void OnUserAction(MessageEventArgs e)
		{
			Widget submenu = this.Submenu;
			
			if (submenu == null)
			{
				Behaviors.MenuBehavior behavior = Behaviors.MenuBehavior.CloseItemMenu (this);
				
//				this.ExecuteCommand ();
				
				if (behavior != null)
				{
					behavior.HandleMenuItemPressed (this);
				}
			}
			else
			{
				Behaviors.MenuBehavior.OpenItemSubmenu (this, Behaviors.MenuBehavior.Animate.Automatic);
			}
		}
		
		protected virtual MenuItemState GetPaintItemState()
		{
			return this.ItemState;
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				System.IDisposable submenu = this.Submenu as System.IDisposable;
				
				if (submenu != null)
				{
					submenu.Dispose ();
				}
				
				this.Submenu = null;
			}
			
			base.Dispose (disposing);
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			if ( this.textOnly )
			{
				if ( this.TextLayout != null )  this.TextLayout.LayoutSize = this.mainTextSize;
			}
			else if ( this.IsSeparator )
			{
			}
			else
			{
				if ( this.icon != null )  this.icon.LayoutSize = this.iconSize;
				if ( this.TextLayout != null )  this.TextLayout.LayoutSize = this.mainTextSize;
				if ( this.shortcut != null )  this.shortcut.LayoutSize = this.shortcutSize;
			}
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.GetPaintState ();
			MenuItemState      iType = this.GetPaintItemState ();
			Drawing.Point     pos   = new Drawing.Point();
			
			if ( this.IsSeparator )
			{
				iType = MenuItemState.Default;
			}
			adorner.PaintMenuItemBackground(graphics, rect, state, Direction.Up, this.MenuOrientation, iType);

			if ( this.textOnly || this.MenuOrientation == MenuOrientation.Horizontal )
			{
				pos.X = (rect.Width-this.mainTextSize.Width)/2;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.TextLayout, state, Direction.Up, this.MenuOrientation, iType);
			}
			else if ( this.IsSeparator )
			{
				Drawing.Rectangle inside = rect;
				inside.Left  = MenuItem.MarginItem*2+this.iconSize.Width;
				inside.Right = rect.Width-MenuItem.MarginItem;
				adorner.PaintSeparatorBackground(graphics, inside, state, Direction.Up, false);
			}
			else
			{
				string icon = this.IconUri;
				
				switch (this.ActiveState)
				{
					case ActiveState.No:
						if ((this.iconUriActiveNo != null) &&
							(this.iconUriActiveNo.Length > 0))
						{
							icon = this.iconUriActiveNo;
						}
						break;
					
					case ActiveState.Yes:
						if ((this.iconUriActiveYes != null) &&
							(this.iconUriActiveYes.Length > 0))
						{
							icon = this.iconUriActiveYes;
						}
						break;
				}
				
				if ((icon != null) &&
					(icon.Length > 0))
				{
					if ((this.ActiveState == ActiveState.Yes) &&
						(icon != this.iconUriActiveYes))
					{
						Drawing.Rectangle iRect = rect;
						iRect.Width = this.iconSize.Width;
						iRect.Inflate(-2, -2);
						adorner.PaintButtonBackground(graphics, iRect, state, Direction.Up, ButtonStyle.ToolItem);
					}
					
					pos.X = MenuItem.MarginItem;
					pos.Y = (rect.Height-this.iconSize.Height)/2;
					
					icon = this.GetIconText (icon);
					
					if (this.icon.Text != icon)
					{
						this.icon.Text = icon;
					}
					
					adorner.PaintMenuItemTextLayout(graphics, pos, this.icon, state, Direction.Up, this.MenuOrientation, iType);
				}

				pos.X = MenuItem.MarginItem*2+this.iconSize.Width;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.TextLayout, state, Direction.Up, this.MenuOrientation, iType);

				pos.X = rect.Width-this.submenuMarkWidth-this.shortcutSize.Width+MenuItem.MarginItem;
				pos.Y = (rect.Height-this.shortcutSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.shortcut, state, Direction.Up, this.MenuOrientation, iType);

				if ( this.Submenu != null )  // triangle ">" ?
				{
					Drawing.Rectangle aRect = rect;
					aRect.Left = aRect.Right-this.submenuMarkWidth;
					aRect.Bottom = (rect.Height-this.submenuMarkWidth)/2;
					aRect.Top = aRect.Bottom+this.submenuMarkWidth;
					adorner.PaintGlyph(graphics, aRect, state, GlyphShape.ArrowRight, PaintTextStyle.VMenu);
				}
			}
		}
		
		
		protected virtual void OnIconSizeChanged()
		{
		}
		
		
		public static Widget GetMenuRoot(Widget widget)
		{
			//	Trouve le widget à la racine de la hiérarchie d'un menu. C'est
			//	soit l'ancêtre de l'arbre des widgets, soit le premier parent
			//	qui possède une définition de "menu" :
			
			if (widget == null)
			{
				return widget;
			}
			
			Widget parent = widget.Parent;
			
			while (parent != null)
			{
				if (widget.ContainsLocalValue (MenuItem.MenuBehaviorProperty))
				{
					return widget;
				}
				
				widget = parent;
				parent = widget.Parent;
			}
			
			return widget;
		}
		
		
		public static Behaviors.MenuBehavior GetMenuBehavior(Widget widget)
		{
			//	Trouve le "menu" associé à un widget.
			
			Widget root   = MenuItem.GetMenuRoot (widget);
			Widget parent = MenuItem.GetParentMenuItem (root);
			
			if (parent == null)
			{
				return (root == null) ? null : root.GetValue (MenuItem.MenuBehaviorProperty) as Behaviors.MenuBehavior;
			}
			else
			{
				return MenuItem.GetMenuBehavior (parent);
			}
		}
		
		public static void SetMenuBehavior(Widget widget, Behaviors.MenuBehavior value)
		{
			//	Associe un "menu" avec un widget.
			
			Widget root = MenuItem.GetMenuRoot (widget);
			root.SetValue (MenuItem.MenuBehaviorProperty, value);
		}
		
		
		public static Widget GetSubmenu(Widget widget)
		{
			return widget.GetValue (MenuItem.SubmenuProperty) as Widget;
		}
		
		public static Window GetMenuWindow(Widget widget)
		{
			//	Retourne la fenêtre utilisée par le menu. Si le menu n'avait
			//	pas encore (ou plus) de fenêtre associée, un MenuWindow est
			//	(re)créé pour le menu :
			
			Window window = widget.Window;
			
			if (window == null)
			{
				MenuWindow menu = new MenuWindow (MenuItem.GetMenuBehavior (widget), MenuItem.GetParentMenuItem (widget));
				
				Drawing.Size size = widget.GetBestFitSize ();
				
				widget.Dock = DockStyle.Fill;
				
				menu.Root.SetManualBounds (new Drawing.Rectangle (Drawing.Point.Zero, size));
				menu.Root.Children.Add (widget);
				menu.MenuType = MenuItem.GetParentMenuItem (widget) == null ? MenuType.Undefined : MenuType.Submenu;
				
//-				System.Diagnostics.Debug.WriteLine ("Menu defaulting to MenuType." + menu.MenuType);
				
				window = menu;
			}
			
			return window;
		}
		
		public static MenuItem GetParentMenuItem(Widget widget)
		{
			return (widget == null) ? null : widget.GetValue (MenuItem.ParentMenuItemProperty) as MenuItem;
		}
		
		
		public static IMenuHost GetMenuHost(Widget widget)
		{
			IMenuHost host = widget.GetValue (MenuItem.MenuHostProperty) as IMenuHost;
			
			if (host == null)
			{
				Widget root = MenuItem.GetMenuRoot (widget);
				host = root.GetValue (MenuItem.MenuHostProperty) as IMenuHost;
			}
			
			return host;
		}
		
		public static void SetMenuHost(Widget widget, IMenuHost value)
		{
			widget.SetValue (MenuItem.MenuHostProperty, value);
		}
		
		
		public static bool GetZeroDelay(Widget widget)
		{
			if (widget.ContainsLocalValue (MenuItem.ZeroDelayProperty))
			{
				return (bool) widget.GetValue (MenuItem.ZeroDelayProperty);
			}
			else
			{
				Widget root = MenuItem.GetMenuRoot (widget);
				return (bool) root.GetValue (MenuItem.ZeroDelayProperty);
			}
		}
		
		public static void SetZeroDelay(Widget widget, bool value)
		{
			widget.SetValue (MenuItem.ZeroDelayProperty, value);
		}
		
		
		public static MenuItemState GetItemState(Widget widget)
		{
			return (MenuItemState) widget.GetValue (MenuItem.ItemStateProperty);
		}
		
		public static void SetItemState(Widget widget, MenuItemState value)
		{
			if ((MenuItemState) widget.GetValue (MenuItem.ItemStateProperty) != value)
			{
				widget.SetValue (MenuItem.ItemStateProperty, value);
			}
		}
		
		
		private static void NotifySubmenuChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Quand la propriété "Submenu" est appliquée à un item d'un menu,
			//	on réalise un lien entre le menu et l'item en question. Ce lien
			//	est stocké dans une propriété nommée "ParentMenuItem" :
			
			MenuItem that = o as MenuItem;
			
			Widget oldSubmenu = oldValue as Widget;
			Widget newSubmenu = newValue as Widget;
			
			if (newSubmenu != null)
			{
				//	Un MenuItem qui pointe sur un sous-menu ne peut pas avoir de
				//	commande associée :
				
				that.CommandObject = null;
			}
			
			if (oldSubmenu != null)
			{
				System.Diagnostics.Debug.Assert (oldSubmenu == MenuItem.GetMenuRoot (oldSubmenu));
				System.Diagnostics.Debug.Assert (MenuItem.GetParentMenuItem (oldSubmenu) == that);
				
				oldSubmenu.ClearValue (MenuItem.ParentMenuItemProperty);
			}
			
			if (newSubmenu != null)
			{
				System.Diagnostics.Debug.Assert (newSubmenu == MenuItem.GetMenuRoot (newSubmenu));
				
				newSubmenu.SetValue (MenuItem.ParentMenuItemProperty, that);
			}
		}
		
		
		public static readonly DependencyProperty			SubmenuProperty			= DependencyProperty.Register ("Submenu", typeof (Widget), typeof (MenuItem), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (MenuItem.NotifySubmenuChanged)));
		public static readonly DependencyProperty			ItemStateProperty		= DependencyProperty.Register ("ItemState", typeof (MenuItemState), typeof (MenuItem), new VisualPropertyMetadata (MenuItemState.Default, VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty			MenuOrientationProperty	= DependencyProperty.Register ("MenuOrientation", typeof (MenuOrientation), typeof (MenuItem), new VisualPropertyMetadata (MenuOrientation.Undefined, VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty			MenuBehaviorProperty	= DependencyProperty.RegisterAttached ("MenuBehavior", typeof (Behaviors.MenuBehavior), typeof (MenuItem));
		public static readonly DependencyProperty			ParentMenuItemProperty	= DependencyProperty.RegisterAttached ("ParentMenuItem", typeof (MenuItem), typeof (MenuItem));
		public static readonly DependencyProperty			MenuHostProperty		= DependencyProperty.RegisterAttached ("MenuHost", typeof (IMenuHost), typeof (MenuItem));
		public static readonly DependencyProperty			ZeroDelayProperty		= DependencyProperty.RegisterAttached ("ZeroDelay", typeof (bool), typeof (MenuItem), new DependencyPropertyMetadata (false));
		
		
		private const double					MarginHeader	= 6;
		private const double					MarginItem		= 2;
		private const double					MarginSpace		= 8;
		private const double					SeparatorHeight = 5;
		
		protected bool							textOnly = false;
		
		protected double						submenuMarkWidth;
		protected string						iconUriActiveNo;
		protected string						iconUriActiveYes;
		protected TextLayout					icon;
		protected TextLayout					shortcut;
		protected Drawing.Size					iconSize;
		protected Drawing.Size					mainTextSize;
		protected Drawing.Size					shortcutSize;
		protected Widget						submenu;
	}
}
