namespace Epsitec.Common.Widgets
{
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
			this.InternalState |= InternalState.Command;
			
			this.InternalState &= ~InternalState.AutoCapture;
			this.InternalState &= ~InternalState.AutoFocus;
			this.InternalState &= ~InternalState.AutoEngage;
			this.InternalState &= ~InternalState.Focusable;
			this.InternalState &= ~InternalState.Engageable;

			this.iconName          = "";
			this.iconNameActiveNo  = "";
			this.iconNameActiveYes = "";
			this.icon          = new TextLayout();
			this.iconActiveNo  = new TextLayout();
			this.iconActiveYes = new TextLayout();
			this.mainText      = new TextLayout();
			this.shortKey      = new TextLayout();
			this.icon.Alignment          = Drawing.ContentAlignment.MiddleLeft;
			this.iconActiveNo.Alignment  = Drawing.ContentAlignment.MiddleLeft;
			this.iconActiveYes.Alignment = Drawing.ContentAlignment.MiddleLeft;
			this.mainText.Alignment      = Drawing.ContentAlignment.MiddleLeft;
			this.shortKey.Alignment      = Drawing.ContentAlignment.MiddleLeft;

			this.subIndicatorWidth = this.DefaultFontHeight;
			this.colorControlDark = Drawing.Color.FromName("ControlDark");
		}
		
		public MenuItem(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public MenuItem(string name, string text) : this()
		{
			this.Name     = name;
			this.MainText = text;
			this.onlyText = true;
		}
		
		public MenuItem(string name, string icon, string text, string shortcut) : this()
		{
			this.Name     = name;
			this.IconName = icon;
			this.MainText = text;
			this.ShortKey = shortcut;
			this.onlyText = false;
		}
		
		public MenuItem(string name, string icon, string text, string shortcut, AbstractMenu submenu) : this()
		{
			this.Name     = name;
			this.IconName = icon;
			this.MainText = text;
			this.ShortKey = shortcut;
			this.onlyText = false;
			this.Submenu  = submenu;
		}
		
		
		internal void SetMenuType(MenuType type)
		{
			this.type = type;
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.submenu != null)
				{
					this.submenu.Dispose ();
					this.submenu = null;
				}
			}
			
			base.Dispose (disposing);
		}


		// Type de la case.
		public MenuItemType ItemType
		{
			get
			{
				return this.itemType;
			}

			set
			{
				if ( this.itemType != value )
				{
					this.itemType = value;
					this.Invalidate();
				}
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
		[ Support.Bundle ("icon") ] public string IconName
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
		[ Support.Bundle ("iconNo") ] public string IconNameActiveNo
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
		[ Support.Bundle ("iconYes") ] public string IconNameActiveYes
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

		// Nom du texte principal affiché à droite de l'icône.
		[ Support.Bundle ("text") ] public string MainText
		{
			get
			{
				return this.mainText.Text;
			}

			set
			{
				this.mainText.Text = value;
				this.separator = false;
				this.mainTextSize = this.mainText.SingleLineSize;
				this.AdjustSize(ref this.mainTextSize);
			}
		}

		// Nom du raccourci clavier affiché à droite.
		[ Support.Bundle ("key") ] public string ShortKey
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

		// Sous-menu éventuel associé à la case.
		[ Support.Bundle ("menu") ] public AbstractMenu Submenu
		{
			get
			{
				return this.submenu;
			}

			set
			{
				if (value == null)
				{
					this.InternalState |= InternalState.Command;
				}
				else
				{
					this.InternalState &= ~InternalState.Command;
				}
				
				this.separator = false;
				this.submenu = value;
			}
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
				if ( this.mainText != null )  this.mainText.LayoutSize = this.mainTextSize;
			}
			else if ( this.separator )
			{
			}
			else
			{
				if ( this.icon != null )  this.icon.LayoutSize = this.iconSize;
				if ( this.iconActiveNo != null )  this.iconActiveNo.LayoutSize = this.iconSize;
				if ( this.iconActiveYes != null )  this.iconActiveYes.LayoutSize = this.iconSize;
				if ( this.mainText != null )  this.mainText.LayoutSize = this.mainTextSize;
				if ( this.shortKey != null )  this.shortKey.LayoutSize = this.shortKeySize;
			}
		}

		// Dessine la case.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			MenuItemType      iType = this.itemType;
			Drawing.Point     pos   = new Drawing.Point(0, 0);

			if ( (state & WidgetState.Enabled) == 0 || this.separator )
			{
				iType = MenuItemType.Deselect;
			}
			adorner.PaintMenuItemBackground(graphics, rect, state, Direction.Up, this.type, iType);

			if ( this.onlyText || this.type == MenuType.Horizontal )
			{
				pos.X = (rect.Width-this.mainTextSize.Width)/2;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.mainText, state, Direction.Up, this.type, iType);
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
				adorner.PaintMenuItemTextLayout(graphics, pos, this.mainText, state, Direction.Up, this.type, iType);

				pos.X = rect.Width-this.subIndicatorWidth-this.shortKeySize.Width+this.marginItem;
				pos.Y = (rect.Height-this.shortKeySize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.shortKey, state, Direction.Up, this.type, iType);

				if ( this.submenu != null )  // triangle ">" ?
				{
					Drawing.Rectangle aRect = rect;
					aRect.Left = aRect.Right-this.subIndicatorWidth;
					aRect.Bottom = (rect.Height-this.subIndicatorWidth)/2;
					aRect.Top = aRect.Bottom+this.subIndicatorWidth;
					adorner.PaintArrow(graphics, aRect, state, Direction.Right, PaintTextStyle.VMenu);
				}
			}
		}


		protected bool				onlyText = false;
		protected bool				separator = false;
		protected MenuType			type = MenuType.Invalid;
		protected MenuItemType		itemType = MenuItemType.Deselect;
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
		protected TextLayout		mainText;
		protected TextLayout		shortKey;
		protected Drawing.Size		iconSize;
		protected Drawing.Size		mainTextSize;
		protected Drawing.Size		shortKeySize;
		protected AbstractMenu		submenu;
		protected Drawing.Color		colorControlDark;
	}
}
