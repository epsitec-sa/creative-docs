using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Helpers;

namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Support.BundleAttribute;

	public enum MenuItemType
	{
		Deselect,					// case désélectionnée
		Select,						// case sélectionnée
		Parent,						// case sélectionnée comme parent
	}
	
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
			
			this.InternalState &= ~InternalState.Focusable;
			this.InternalState &= ~InternalState.Engageable;

			this.iconName          = "";
			this.iconNameActiveNo  = "";
			this.iconNameActiveYes = "";
			this.icon          = new TextLayout();
			this.iconActiveNo  = new TextLayout();
			this.iconActiveYes = new TextLayout();
			this.shortKey      = new TextLayout();
			this.icon.Alignment          = Drawing.ContentAlignment.MiddleLeft;
			this.iconActiveNo.Alignment  = Drawing.ContentAlignment.MiddleLeft;
			this.iconActiveYes.Alignment = Drawing.ContentAlignment.MiddleLeft;
			this.Alignment               = Drawing.ContentAlignment.MiddleLeft;
			this.shortKey.Alignment      = Drawing.ContentAlignment.MiddleLeft;

			this.subIndicatorWidth = this.DefaultFontHeight;
			this.colorControlDark = Drawing.Color.FromName("ControlDark");
		}
		
		public MenuItem(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public MenuItem(string command, string text) : this()
		{
			this.Command  = command;
			this.Text     = text;
			this.onlyText = true;
		}
		
		public MenuItem(string command, string icon, string text, string shortcut) : this()
		{
			this.Command  = command;
			this.IconName = icon;
			this.Text     = text;
			this.ShortKey = shortcut;
			this.onlyText = false;
		}
		
		public MenuItem(string command, string icon, string text, string shortcut, string name) : this(command, icon, text, shortcut)
		{
			this.Name = name;
		}
		
		public MenuItem(Widget submenu, string icon, string text, string shortcut) : this()
		{
			this.IconName = icon;
			this.Text     = text;
			this.ShortKey = shortcut;
			this.onlyText = false;
			this.Submenu  = submenu;
		}
		
		public MenuItem(Widget submenu, string icon, string text, string shortcut, string name) : this(submenu, icon, text, shortcut)
		{
			this.Name = name;
		}
		
		
		internal void SetMenuType(MenuType type)
		{
			this.type = type;
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

		// Indique s'il s'agit d'une case d'un menu horizontal avec un texte seul.
		public bool OnlyText
		{
			get
			{
				return this.onlyText;
			}
		}

		// Indique s'il s'agit d'une ligne de séparation horizontale.
		public bool Separator
		{
			get
			{
				return this.separator;
			}
		}

		// Nom de l'icône affichée à gauche.
		public string IconName
		{
			get
			{
				return this.iconName;
			}

			set
			{
				this.iconName = value;
				if ( this.iconName == "" )
				{
					this.icon.Text = "";
				}
				else
				{
					this.icon.Text = @"<img src=""" + this.iconName + @"""/>";
				}
				this.iconSize = this.icon.SingleLineSize;
				this.AdjustSize(ref this.iconSize);
				this.separator = false;
			}
		}

		// Nom de l'icône affichée à gauche.
		public string IconNameActiveNo
		{
			get
			{
				return this.iconNameActiveNo;
			}

			set
			{
				this.iconNameActiveNo = value;
				if ( this.iconNameActiveNo == "" )
				{
					this.iconActiveNo.Text = "";
				}
				else
				{
					this.iconActiveNo.Text = @"<img src=""" + this.iconNameActiveNo + @"""/>";
					this.iconSize = this.iconActiveNo.SingleLineSize;
					this.AdjustSize(ref this.iconSize);
				}
				this.separator = false;
			}
		}

		// Nom de l'icône affichée à gauche.
		public string IconNameActiveYes
		{
			get
			{
				return this.iconNameActiveYes;
			}

			set
			{
				this.iconNameActiveYes = value;
				if ( this.iconNameActiveYes == "" )
				{
					this.iconActiveYes.Text = "";
				}
				else
				{
					this.iconActiveYes.Text = @"<img src=""" + this.iconNameActiveYes + @"""/>";
					this.iconSize = this.iconActiveYes.SingleLineSize;
					this.AdjustSize(ref this.iconSize);
				}
				this.separator = false;
			}
		}

		// Nom du raccourci clavier affiché à droite.
		public string ShortKey
		{
			get
			{
				return this.shortKey.Text;
			}

			set
			{
				this.shortKey.Text = value;
				this.separator = false;
				this.shortKeySize = this.shortKey.SingleLineSize;
				this.AdjustSize(ref this.shortKeySize);
			}
		}

		
		public Widget			Submenu
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

		
		public static MenuItem CreateYesNo(string command, string text, string shortcut, string name)
		{
			MenuItem item = new MenuItem (command, "", text, shortcut, name);
			
			item.IconNameActiveNo  = "manifest:Epsitec.Common.Widgets.Images.ActiveNo.icon";
			item.IconNameActiveYes = "manifest:Epsitec.Common.Widgets.Images.ActiveYes.icon";
			
			return item;
		}
		
		
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			this.separator = false;
			this.mainTextSize = this.TextLayout.SingleLineSize;
			this.AdjustSize (ref this.mainTextSize);
		}

		protected override void OnResourceManagerChanged()
		{
			base.OnResourceManagerChanged ();
			
			Support.ResourceManager resource_manager = this.ResourceManager;
			
			this.icon.ResourceManager          = resource_manager;
			this.iconActiveNo.ResourceManager  = resource_manager;
			this.iconActiveYes.ResourceManager = resource_manager;
			this.shortKey.ResourceManager      = resource_manager;

			this.Invalidate ();
		}
		
		// Ajuste des dimensions d'un TextLayout.
		protected void AdjustSize(ref Drawing.Size size)
		{
			size.Width  = System.Math.Ceiling(size.Width);
			size.Height = System.Math.Ceiling(size.Height);

			if ( !this.onlyText )
			{
				size.Width  += this.marginItem*2;
				size.Height += this.marginItem*2;
			}
		}

		// Largeur effective pour l'icône. Cette largeur doit être identique
		// dans toutes les lignes d'un menu vertical.
		public double IconWidth
		{
			get
			{
				return this.iconSize.Width;
			}

			set
			{
				this.iconSize.Width = value;
			}
		}

		// Retourne les dimensions requises en fonction du contenu.
		public Drawing.Size RequiredSize
		{
			get
			{
				Drawing.Size size = new Drawing.Size(0, 0);

				if ( this.onlyText )
				{
					size.Width = this.marginHeader*2 + this.mainTextSize.Width;
					size.Height = this.mainTextSize.Height;
				}
				else if ( this.separator )
				{
					size.Height = this.separatorHeight;
				}
				else
				{
					size.Width += this.iconSize.Width;
					size.Width += this.mainTextSize.Width;
					size.Width += this.marginSpace;
					size.Width += this.shortKeySize.Width;
					size.Width += this.subIndicatorWidth;

					size.Height = System.Math.Max(size.Height, this.iconSize.Height);
					size.Height = System.Math.Max(size.Height, this.mainTextSize.Height);
					size.Height = System.Math.Max(size.Height, this.shortKeySize.Height);
				}
				return size;
			}
		}

		// Met à jour la géométrie de la case du menu.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.onlyText )
			{
				if ( this.TextLayout != null )  this.TextLayout.LayoutSize = this.mainTextSize;
			}
			else if ( this.separator )
			{
			}
			else
			{
				if ( this.icon != null )  this.icon.LayoutSize = this.iconSize;
				if ( this.iconActiveNo != null )  this.iconActiveNo.LayoutSize = this.iconSize;
				if ( this.iconActiveYes != null )  this.iconActiveYes.LayoutSize = this.iconSize;
				if ( this.TextLayout != null )  this.TextLayout.LayoutSize = this.mainTextSize;
				if ( this.shortKey != null )  this.shortKey.LayoutSize = this.shortKeySize;
			}
		}

		// Dessine la case.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			MenuItemType      iType = this.ItemType;
			Drawing.Point     pos   = new Drawing.Point();
			
			if ( this.separator )
			{
				iType = MenuItemType.Deselect;
			}
			adorner.PaintMenuItemBackground(graphics, rect, state, Direction.Up, this.type, iType);

			if ( this.onlyText || this.type == MenuType.Horizontal )
			{
				pos.X = (rect.Width-this.mainTextSize.Width)/2;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.TextLayout, state, Direction.Up, this.type, iType);
			}
			else if ( this.separator )
			{
				Drawing.Rectangle inside = rect;
				inside.Left  = this.marginItem*2+this.iconSize.Width;
				inside.Right = rect.Width-this.marginItem;
				adorner.PaintSeparatorBackground(graphics, inside, state, Direction.Up, false);
			}
			else
			{
				TextLayout il = null;
				if ( this.iconNameActiveNo != "" && this.ActiveState == WidgetState.ActiveNo )
				{
					il = this.iconActiveNo;
				}
				if ( this.iconNameActiveYes != "" && this.ActiveState == WidgetState.ActiveYes )
				{
					il = this.iconActiveYes;
				}
				if ( il == null && this.iconName != "" )
				{
					il = this.icon;
				}

				if ( il != null )  // icône existe ?
				{
					if ( this.ActiveState == WidgetState.ActiveYes && il != this.iconActiveYes )
					{
						Drawing.Rectangle iRect = rect;
						iRect.Width = this.iconSize.Width;
						iRect.Inflate(-2, -2);
						adorner.PaintButtonBackground(graphics, iRect, state, Direction.Up, ButtonStyle.ToolItem);
					}
					pos.X = this.marginItem;
					pos.Y = (rect.Height-this.iconSize.Height)/2;
					adorner.PaintMenuItemTextLayout(graphics, pos, il, state, Direction.Up, this.type, iType);
				}

				pos.X = this.marginItem*2+this.iconSize.Width;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.TextLayout, state, Direction.Up, this.type, iType);

				pos.X = rect.Width-this.subIndicatorWidth-this.shortKeySize.Width+this.marginItem;
				pos.Y = (rect.Height-this.shortKeySize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.shortKey, state, Direction.Up, this.type, iType);

				if ( this.Submenu != null )  // triangle ">" ?
				{
					Drawing.Rectangle aRect = rect;
					aRect.Left = aRect.Right-this.subIndicatorWidth;
					aRect.Bottom = (rect.Height-this.subIndicatorWidth)/2;
					aRect.Top = aRect.Bottom+this.subIndicatorWidth;
					adorner.PaintGlyph(graphics, aRect, state, GlyphShape.ArrowRight, PaintTextStyle.VMenu);
				}
			}
		}
		
		protected override void OnPressed(MessageEventArgs e)
		{
			Widget submenu = this.Submenu;
			
			if (submenu == null)
			{
				Behaviors.MenuBehavior.CloseItemMenu (this);
				
				this.ExecuteCommand ();
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
			//	Trouve le "menu" associé avec un widget.
			
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
			//	Retourne la fenêtre utilisée par le menu. Si le menu n'avait
			//	pas encore de fenêtre associée, un MenuWindow est créé pour
			//	le menu :
			
			Window window = widget.Window;
			
			if (window == null)
			{
				window = new MenuWindow (MenuItem.GetMenuBehavior (widget), MenuItem.GetParentMenuItem (widget));
				
				Drawing.Size size = widget.GetBestFitSize ();
				
				widget.Dock = DockStyle.Fill;
				
				window.Root.Size = size;
				window.Root.Children.Add (widget);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("RootSize -> {0}", window.Root.Size.ToString ()));
				System.Diagnostics.Debug.WriteLine (string.Format ("ClientSize -> {0}", window.ClientSize.ToString ()));
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
			widget.SetValue (MenuItem.ItemTypeProperty, value);
		}
		
		
		private static void NotifySubmenuChanged(Object o, object old_value, object new_value)
		{
			//	Quand la propriété "Submenu" est appliquée à un item d'un menu,
			//	on réalise un lien entre le menu et l'item en question. Ce lien
			//	est stocké dans une propriété nommée "ParentMenuItem" :
			
			MenuItem that = o as MenuItem;
			
			Widget old_submenu = old_value as Widget;
			Widget new_submenu = new_value as Widget;
			
			if (new_submenu != null)
			{
				//	Un MenuItem qui pointe sur un sous-menu ne peut pas avoir de
				//	commande associée :
				
				that.Command = null;
			}
			
			that.separator = false;
			
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
		public static readonly Property			ItemTypeProperty		= Property.Register ("ItemType", typeof (MenuItemType), typeof (MenuItem), new VisualPropertyMetadata (MenuItemType.Deselect, VisualPropertyFlags.AffectsDisplay));
		
		public static readonly Property			MenuBehaviorProperty	= Property.RegisterAttached ("MenuBehavior", typeof (Behaviors.MenuBehavior), typeof (MenuItem));
		public static readonly Property			ParentMenuItemProperty	= Property.RegisterAttached ("ParentMenuItem", typeof (MenuItem), typeof (MenuItem));
		public static readonly Property			MenuHostProperty		= Property.RegisterAttached ("MenuHost", typeof (IMenuHost), typeof (MenuItem));
		public static readonly Property			ZeroDelayProperty		= Property.RegisterAttached ("ZeroDelay", typeof (bool), typeof (MenuItem), new PropertyMetadata (false));
		
		
		protected bool				onlyText = false;
		protected bool				separator = false;
		protected MenuType			type = MenuType.Invalid;
		protected double			marginHeader = 6;
		protected double			marginItem = 2;
		protected double			marginSpace = 8;
		protected double			separatorHeight = 5;
		protected double			subIndicatorWidth;
		protected string			iconName;
		protected string			iconNameActiveNo;
		protected string			iconNameActiveYes;
		protected TextLayout		icon;
		protected TextLayout		iconActiveNo;
		protected TextLayout		iconActiveYes;
		protected TextLayout		shortKey;
		protected Drawing.Size		iconSize;
		protected Drawing.Size		mainTextSize;
		protected Drawing.Size		shortKeySize;
		protected Object			submenu;
		protected Drawing.Color		colorControlDark;
	}
}
