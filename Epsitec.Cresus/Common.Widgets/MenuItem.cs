//	Copyright � 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Helpers;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Le MenuType d�termine la disposition d'un menu.
	/// </summary>
	public enum MenuType
	{
		Invalid,
		Vertical,
		Horizontal
	}
	
	
	/// <summary>
	/// Le MenuItemType d�termine l'�tat d'une case d'un menu.
	/// </summary>
	public enum MenuItemType
	{
		Default,					//	case de menu inactive
		Selected,					//	case de menu s�lectionn�e
		SubmenuOpen					//	case de menu s�lectionn�e, sous-menu affich�
	}
	
	
	/// <summary>
	/// La classe MenuItem repr�sente une case dans un menu.
	/// </summary>
	public class MenuItem : AbstractButton
	{
		public MenuItem()
		{
			this.AutoCapture = false;
			this.AutoFocus   = false;
			this.AutoEngage  = false;
			
			this.InternalState &= ~InternalState.Focusable;
			this.InternalState &= ~InternalState.Engageable;
			
			this.Alignment = Drawing.ContentAlignment.MiddleLeft;
			
			this.icon     = new TextLayout();
			this.shortcut = new TextLayout();
			
			this.icon.Alignment     = Drawing.ContentAlignment.MiddleLeft;
			this.shortcut.Alignment = Drawing.ContentAlignment.MiddleLeft;
			
			this.submenu_mark_width = this.DefaultFontHeight;
		}
		
		public MenuItem(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public MenuItem(string command, string text) : this ()
		{
			this.Command  = command;
			this.Text     = text;
			this.onlyText = true;
		}
		
		public MenuItem(string command, string icon, string text, string shortcut) : this ()
		{
			this.Command  = command;
			this.IconName = icon;
			this.Text     = text;
			this.ShortKey = shortcut;	//#fix
			this.onlyText = false;
		}
		
		public MenuItem(string command, string icon, string text, string shortcut, string name) : this (command, icon, text, shortcut)
		{
			this.Name = name;
		}
		
		public MenuItem(Widget submenu, string icon, string text, string shortcut) : this ()
		{
			this.IconName = icon;
			this.Text     = text;
			this.ShortKey = shortcut;	//#fix
			this.onlyText = false;
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

		
		public MenuType							MenuType
		{
			get
			{
				return (MenuType) this.GetValue (MenuItem.MenuTypeProperty);
			}
		}
		
		public MenuItemType						ItemType
		{
			get
			{
				return (MenuItemType) this.GetValue (MenuItem.ItemTypeProperty);
			}

			set
			{
				this.SetValue (MenuItem.ItemTypeProperty, value);
			}
		}

		
		public string							IconName
		{
			get
			{
				return this.iconName;
			}
			set
			{
				if (this.iconName != value)
				{
					this.iconName = value;
					this.UpdateIconSize ();
				}
			}
		}

		public string							IconNameActiveNo
		{
			get
			{
				return this.iconNameActiveNo;
			}

			set
			{
				if (this.iconNameActiveNo != value)
				{
					this.iconNameActiveNo = value;
					this.UpdateIconSize ();
				}
			}
		}
		
		public string							IconNameActiveYes
		{
			get
			{
				return this.iconNameActiveYes;
			}

			set
			{
				if (this.iconNameActiveYes != value)
				{
					this.iconNameActiveYes = value;
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
				return this.icon_size.Width;
			}
			set
			{
				if (this.icon_size.Width != value)
				{
					this.icon_size.Width = value;
					this.OnIconSizeChanged ();
				}
			}
		}
		
		public Drawing.Size						IconSize
		{
			get
			{
				return this.icon_size;
			}
			set
			{
				if (this.icon_size != value)
				{
					this.icon_size = value;
					this.OnIconSizeChanged ();
				}
			}
		}
		
		
		internal void DefineMenuType(MenuType value)
		{
			this.SetValue (MenuItem.MenuTypeProperty, value);
		}
		
		
		
		public static MenuItem CreateYesNo(string command, string text, string shortcut, string name)
		{
			MenuItem item = new MenuItem (command, "", text, shortcut, name);
			
			item.IconNameActiveNo  = "manifest:Epsitec.Common.Widgets.Images.ActiveNo.icon";
			item.IconNameActiveYes = "manifest:Epsitec.Common.Widgets.Images.ActiveYes.icon";
			
			return item;
		}
		
		
		
		private void UpdateIconSize()
		{
			double dx = 0;
			double dy = 0;
			
			if ((this.iconNameActiveNo != null) &&
				(this.iconNameActiveNo.Length > 0))
			{
				TextLayout layout = this.GetIconTextLayout (this.iconNameActiveNo);
				Drawing.Size size = layout.SingleLineSize;
				
				dx = System.Math.Max (dx, size.Width);
				dy = System.Math.Max (dy, size.Height);
			}
			
			if ((this.iconNameActiveYes != null) &&
				(this.iconNameActiveYes.Length > 0))
			{
				TextLayout layout = this.GetIconTextLayout (this.iconNameActiveYes);
				Drawing.Size size = layout.SingleLineSize;
				
				dx = System.Math.Max (dx, size.Width);
				dy = System.Math.Max (dy, size.Height);
			}
			
			if ((this.iconName != null) &&
				(this.iconName.Length > 0))
			{
				TextLayout layout = this.GetIconTextLayout (this.iconName);
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

			if ( !this.onlyText )
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

			if (this.onlyText)
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
				dx += this.icon_size.Width;
				dx += this.mainTextSize.Width;
				dx += MenuItem.MarginSpace;
				dx += this.shortcutSize.Width;
				dx += this.submenu_mark_width;

				dy = System.Math.Max(dy, this.icon_size.Height);
				dy = System.Math.Max(dy, this.mainTextSize.Height);
				dy = System.Math.Max(dy, this.shortcutSize.Height);
			}
			
			return new Drawing.Size (dx, dy);
		}

		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			this.mainTextSize = this.AdjustSize (this.TextLayout.SingleLineSize);
		}

		protected override void OnResourceManagerChanged()
		{
			base.OnResourceManagerChanged ();
			
			Support.ResourceManager resource_manager = this.ResourceManager;
			
			this.icon.ResourceManager          = resource_manager;
			this.shortcut.ResourceManager      = resource_manager;

			this.Invalidate ();
		}
		
		
		protected override void OnPressed(MessageEventArgs e)
		{
			Widget submenu = this.Submenu;
			
			if (submenu == null)
			{
				Behaviors.MenuBehavior behavior = Behaviors.MenuBehavior.CloseItemMenu (this);
				
				this.ExecuteCommand ();
				
				if (behavior != null)
				{
					behavior.HandleMenuItemPressed (this);
				}
			}
			else
			{
				Behaviors.MenuBehavior.OpenItemSubmenu (this, Behaviors.MenuBehavior.Animate.Automatic);
			}
			
			base.OnPressed (e);
		}
		
		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);
		}
		
		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);
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

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.onlyText )
			{
				if ( this.TextLayout != null )  this.TextLayout.LayoutSize = this.mainTextSize;
			}
			else if ( this.IsSeparator )
			{
			}
			else
			{
				if ( this.icon != null )  this.icon.LayoutSize = this.icon_size;
				if ( this.TextLayout != null )  this.TextLayout.LayoutSize = this.mainTextSize;
				if ( this.shortcut != null )  this.shortcut.LayoutSize = this.shortcutSize;
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			MenuItemType      iType = this.ItemType;
			Drawing.Point     pos   = new Drawing.Point();
			
			if ( this.IsSeparator )
			{
				iType = MenuItemType.Default;
			}
			adorner.PaintMenuItemBackground(graphics, rect, state, Direction.Up, this.MenuType, iType);

			if ( this.onlyText || this.MenuType == MenuType.Horizontal )
			{
				pos.X = (rect.Width-this.mainTextSize.Width)/2;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.TextLayout, state, Direction.Up, this.MenuType, iType);
			}
			else if ( this.IsSeparator )
			{
				Drawing.Rectangle inside = rect;
				inside.Left  = MenuItem.MarginItem*2+this.icon_size.Width;
				inside.Right = rect.Width-MenuItem.MarginItem;
				adorner.PaintSeparatorBackground(graphics, inside, state, Direction.Up, false);
			}
			else
			{
				string icon = this.iconName;
				
				switch (this.ActiveState)
				{
					case WidgetState.ActiveNo:
						if ((this.iconNameActiveNo != null) &&
							(this.iconNameActiveNo.Length > 0))
						{
							icon = this.iconNameActiveNo;
						}
						break;
					
					case WidgetState.ActiveYes:
						if ((this.iconNameActiveYes != null) &&
							(this.iconNameActiveYes.Length > 0))
						{
							icon = this.iconNameActiveYes;
						}
						break;
				}
				
				if ((icon != null) &&
					(icon.Length > 0))
				{
					if ((this.ActiveState == WidgetState.ActiveYes) &&
						(icon != this.iconNameActiveYes))
					{
						Drawing.Rectangle iRect = rect;
						iRect.Width = this.icon_size.Width;
						iRect.Inflate(-2, -2);
						adorner.PaintButtonBackground(graphics, iRect, state, Direction.Up, ButtonStyle.ToolItem);
					}
					
					pos.X = MenuItem.MarginItem;
					pos.Y = (rect.Height-this.icon_size.Height)/2;
					
					icon = this.GetIconText (icon);
					
					if (this.icon.Text != icon)
					{
						this.icon.Text = icon;
					}
					
					adorner.PaintMenuItemTextLayout(graphics, pos, this.icon, state, Direction.Up, this.MenuType, iType);
				}

				pos.X = MenuItem.MarginItem*2+this.icon_size.Width;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.TextLayout, state, Direction.Up, this.MenuType, iType);

				pos.X = rect.Width-this.submenu_mark_width-this.shortcutSize.Width+MenuItem.MarginItem;
				pos.Y = (rect.Height-this.shortcutSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.shortcut, state, Direction.Up, this.MenuType, iType);

				if ( this.Submenu != null )  // triangle ">" ?
				{
					Drawing.Rectangle aRect = rect;
					aRect.Left = aRect.Right-this.submenu_mark_width;
					aRect.Bottom = (rect.Height-this.submenu_mark_width)/2;
					aRect.Top = aRect.Bottom+this.submenu_mark_width;
					adorner.PaintGlyph(graphics, aRect, state, GlyphShape.ArrowRight, PaintTextStyle.VMenu);
				}
			}
		}
		
		
		protected virtual void OnIconSizeChanged()
		{
		}
		
		
		public static Widget GetMenuRoot(Widget widget)
		{
			//	Trouve le widget � la racine de la hi�rarchie d'un menu. C'est
			//	soit l'anc�tre de l'arbre des widgets, soit le premier parent
			//	qui poss�de une d�finition de "menu" :
			
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
			//	Trouve le "menu" associ� avec un widget.
			
			Widget root = MenuItem.GetMenuRoot (widget);
			Widget parent = MenuItem.GetParentMenuItem (root);
			
			if (parent == null)
			{
				return root.GetValue (MenuItem.MenuBehaviorProperty) as Behaviors.MenuBehavior;
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
			//	Retourne la fen�tre utilis�e par le menu. Si le menu n'avait
			//	pas encore de fen�tre associ�e, un MenuWindow est cr�� pour
			//	le menu :
			
			Window window = widget.Window;
			
			if (window == null)
			{
				window = new MenuWindow (MenuItem.GetMenuBehavior (widget), MenuItem.GetParentMenuItem (widget));
				
				Drawing.Size size = widget.GetBestFitSize ();
				
				widget.Dock = DockStyle.Fill;
				
				window.Root.Size = size;
				window.Root.Children.Add (widget);
			}
			
			return window;
		}
		
		public static MenuItem GetParentMenuItem(Widget widget)
		{
			return widget.GetValue (MenuItem.ParentMenuItemProperty) as MenuItem;
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
		
		
		public static MenuItemType GetItemType(Widget widget)
		{
			return (MenuItemType) widget.GetValue (MenuItem.ItemTypeProperty);
		}
		
		public static void SetItemType(Widget widget, MenuItemType value)
		{
			if ((MenuItemType) widget.GetValue (MenuItem.ItemTypeProperty) != value)
			{
				widget.SetValue (MenuItem.ItemTypeProperty, value);
			}
		}
		
		
		private static void NotifySubmenuChanged(Object o, object old_value, object new_value)
		{
			//	Quand la propri�t� "Submenu" est appliqu�e � un item d'un menu,
			//	on r�alise un lien entre le menu et l'item en question. Ce lien
			//	est stock� dans une propri�t� nomm�e "ParentMenuItem" :
			
			MenuItem that = o as MenuItem;
			
			Widget old_submenu = old_value as Widget;
			Widget new_submenu = new_value as Widget;
			
			if (new_submenu != null)
			{
				//	Un MenuItem qui pointe sur un sous-menu ne peut pas avoir de
				//	commande associ�e :
				
				that.Command = null;
			}
			
			if (old_submenu != null)
			{
				System.Diagnostics.Debug.Assert (old_submenu == MenuItem.GetMenuRoot (old_submenu));
				System.Diagnostics.Debug.Assert (MenuItem.GetParentMenuItem (old_submenu) == that);
				
				old_submenu.SetValue (MenuItem.ParentMenuItemProperty, null);
			}
			
			if (new_submenu != null)
			{
				System.Diagnostics.Debug.Assert (new_submenu == MenuItem.GetMenuRoot (new_submenu));
				
				new_submenu.SetValue (MenuItem.ParentMenuItemProperty, that);
			}
		}
		
		
		public static readonly Property			SubmenuProperty			= Property.Register ("Submenu", typeof (Widget), typeof (MenuItem), new PropertyMetadata (null, new PropertyInvalidatedCallback (MenuItem.NotifySubmenuChanged)));
		public static readonly Property			ItemTypeProperty		= Property.Register ("ItemType", typeof (MenuItemType), typeof (MenuItem), new VisualPropertyMetadata (MenuItemType.Default, VisualPropertyFlags.AffectsDisplay));
		public static readonly Property			MenuTypeProperty		= Property.Register ("MenuType", typeof (MenuType), typeof (MenuItem), new VisualPropertyMetadata (MenuType.Invalid, VisualPropertyFlags.AffectsDisplay));
		
		public static readonly Property			MenuBehaviorProperty	= Property.RegisterAttached ("MenuBehavior", typeof (Behaviors.MenuBehavior), typeof (MenuItem));
		public static readonly Property			ParentMenuItemProperty	= Property.RegisterAttached ("ParentMenuItem", typeof (MenuItem), typeof (MenuItem));
		public static readonly Property			MenuHostProperty		= Property.RegisterAttached ("MenuHost", typeof (IMenuHost), typeof (MenuItem));
		public static readonly Property			ZeroDelayProperty		= Property.RegisterAttached ("ZeroDelay", typeof (bool), typeof (MenuItem), new PropertyMetadata (false));
		
		
		private const double					MarginHeader	= 6;
		private const double					MarginItem		= 2;
		private const double					MarginSpace		= 8;
		private const double					SeparatorHeight = 5;
		
		protected bool							onlyText = false;
		
		protected double						submenu_mark_width;
		protected string						iconName;
		protected string						iconNameActiveNo;
		protected string						iconNameActiveYes;
		protected TextLayout					icon;
		protected TextLayout					shortcut;
		protected Drawing.Size					icon_size;
		protected Drawing.Size					mainTextSize;
		protected Drawing.Size					shortcutSize;
		protected Widget						submenu;
	}
}
