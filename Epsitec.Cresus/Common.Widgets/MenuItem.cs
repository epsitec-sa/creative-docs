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
		public MenuItem(MenuType type)
		{
			this.type = type;

			this.internalState &= ~InternalState.AutoCapture;
			this.internalState &= ~InternalState.AutoFocus;
			this.internalState &= ~InternalState.AutoEngage;
			this.internalState &= ~InternalState.Focusable;
			this.internalState &= ~InternalState.Engageable;

			this.icon     = new TextLayout();
			this.mainText = new TextLayout();
			this.shortKey = new TextLayout();
			this.icon.Alignment     = Drawing.ContentAlignment.MiddleLeft;
			this.mainText.Alignment = Drawing.ContentAlignment.MiddleLeft;
			this.shortKey.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.subIndicatorWidth = this.DefaultFontHeight;
			this.colorControlDark = Drawing.Color.FromName("ControlDark");
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

			set
			{
				this.onlyText = value;
			}
		}

		// Indique s'il s'agit d'une ligne de séparation horizontale.
		public bool Separator
		{
			get
			{
				return this.separator;
			}

			set
			{
				this.separator = value;
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
					this.icon.Text = "<img src=\"..\\..\\" + this.iconName + ".png\"/>";
				}
				this.iconSize = this.icon.SingleLineSize();
				AdjustSize(ref this.iconSize);
			}
		}

		// Nom du texte principal affiché à droite de l'icône.
		public string MainText
		{
			get
			{
				return this.mainText.Text;
			}

			set
			{
				this.mainText.Text = value;
				this.mainTextSize = this.mainText.SingleLineSize();
				AdjustSize(ref this.mainTextSize);
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
				this.shortKeySize = this.shortKey.SingleLineSize();
				AdjustSize(ref this.shortKeySize);
			}
		}

		// Sous-menu éventuel associé à la case.
		public Menu Submenu
		{
			get
			{
				return this.submenu;
			}

			set
			{
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

			if ( this.icon == null )  return;

			if ( this.onlyText )
			{
				this.mainText.LayoutSize = this.mainTextSize;
			}
			else if ( this.separator )
			{
			}
			else
			{
				this.icon.LayoutSize     = this.iconSize;
				this.mainText.LayoutSize = this.mainTextSize;
				this.shortKey.LayoutSize = this.shortKeySize;
			}
		}

		// Dessine la case.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			MenuItemType      iType = this.itemType;
			Direction         dir   = this.RootDirection;
			Drawing.Point     pos   = new Drawing.Point(0, 0);

			if ( (state & WidgetState.Enabled) == 0 || this.separator )
			{
				iType = MenuItemType.Deselect;
			}
			adorner.PaintMenuItemBackground(graphics, rect, state, dir, this.type, iType);

			if ( this.onlyText )
			{
				pos.X = (rect.Width-this.mainTextSize.Width)/2;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.mainText, state, dir, this.type, iType);
			}
			else if ( this.separator )
			{
				double x1 = this.marginItem*2+this.iconSize.Width;
				double x2 = rect.Width-this.marginItem;
				double y = System.Math.Floor(rect.Height/2)+0.5;
				graphics.AddLine(x1, y, x2, y);
				graphics.RenderSolid(this.colorControlDark);
			}
			else
			{
				if ( this.iconName != "" )  // icône existe ?
				{
					pos.X = this.marginItem;
					pos.Y = (rect.Height-this.iconSize.Height)/2;
					adorner.PaintMenuItemTextLayout(graphics, pos, this.icon, state, dir, this.type, iType);
				}

				pos.X = this.marginItem*2+this.iconSize.Width;
				pos.Y = (rect.Height-this.mainTextSize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.mainText, state, dir, this.type, iType);

				pos.X = rect.Width-this.subIndicatorWidth-this.shortKeySize.Width+this.marginItem;
				pos.Y = (rect.Height-this.shortKeySize.Height)/2;
				adorner.PaintMenuItemTextLayout(graphics, pos, this.shortKey, state, dir, this.type, iType);

				if ( this.submenu != null )  // triangle ">" ?
				{
					Drawing.Rectangle aRect = rect;
					aRect.Left = aRect.Right-this.subIndicatorWidth;
					aRect.Bottom = (rect.Height-this.subIndicatorWidth)/2;
					aRect.Top = aRect.Bottom+this.subIndicatorWidth;
					adorner.PaintArrow(graphics, aRect, state, dir, Direction.Right);
				}
			}
		}


		protected bool				onlyText = false;
		protected bool				separator = false;
		protected MenuType			type;
		protected MenuItemType		itemType = MenuItemType.Deselect;
		protected double			marginHeader = 6;
		protected double			marginItem = 2;
		protected double			marginSpace = 8;
		protected double			separatorHeight = 5;
		protected double			subIndicatorWidth;
		protected string			iconName;
		protected TextLayout		icon;
		protected TextLayout		mainText;
		protected TextLayout		shortKey;
		protected Drawing.Size		iconSize;
		protected Drawing.Size		mainTextSize;
		protected Drawing.Size		shortKeySize;
		protected Menu				submenu;
		protected Drawing.Color		colorControlDark;
	}
}
