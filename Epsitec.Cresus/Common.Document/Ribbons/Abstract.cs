using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Abstract est la classe de base pour toutes les sections de rubans.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract()
		{
			this.title = new TextLayout();
			this.title.DefaultFont     = this.DefaultFont;
			this.title.DefaultFontSize = this.DefaultFontSize;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public virtual void SetDocument(DocumentType type, InstallType install, DebugMode debugMode, Settings.GlobalSettings gs, Document document)
		{
			this.documentType = type;
			this.installType = install;
			this.debugMode = debugMode;
			this.globalSettings = gs;
			this.document = document;
		}

		
		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*2;
			}
		}

		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.LabelHeight + 8 + 22 + 5 + 22;
			}
		}

		// Retourne la hauteur pour le label sup�rieur.
		protected double LabelHeight
		{
			get
			{
				return 14;
			}
		}


		// Retourne la zone rectangulaire utile pour les widgets.
		protected Rectangle UsefulZone
		{
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Top -= this.LabelHeight;
				rect.Deflate(4);
				return rect;
			}
		}

		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
		}


		// D�s�lectionne toutes les origines de couleurs possibles.
		public virtual void OriginColorDeselect()
		{
		}

		// S�lectionne l'origine de couleur.
		public virtual void OriginColorSelect(int rank)
		{
		}

		// Retourne le rang de la couleur d'origine.
		public virtual int OriginColorRank()
		{
			return -1;
		}

		// Modifie la couleur d'origine.
		public virtual void OriginColorChange(Drawing.RichColor color)
		{
		}

		// Donne la couleur d'origine.
		public virtual Drawing.RichColor OriginColorGet()
		{
			return Drawing.RichColor.FromBrightness(0);
		}


		// G�n�re un �v�nement pour dire que la couleur d'origine a chang�.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un �coute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetState state = this.PaintState;
			adorner.PaintRibbonSectionBackground(graphics, rect, this.LabelHeight, state);

			Point pos = new Point(rect.Left+3, rect.Top-this.LabelHeight);
			this.title.LayoutSize = new Size(rect.Width-4, this.LabelHeight);
			adorner.PaintRibbonSectionTextLayout(graphics, pos, this.title, state);
		}


		// Cr�e un bouton pour une commande.
		protected IconButton CreateIconButton(string command)
		{
			return this.CreateIconButton(command, "0");
		}

		// Cr�e un bouton pour une commande, en pr�cisant la taille pr�f�r�e pour l'ic�ne.
		protected IconButton CreateIconButton(string command, string iconSize)
		{
			CommandState cs = CommandDispatcher.GetFocusedPrimaryDispatcher().GetCommandState(command);
			IconButton button = new IconButton(this);

			button.Command = command;
			button.IconName = Misc.Icon(cs.IconName, iconSize);
			button.AutoFocus = false;

			if ( cs.Statefull )
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
			}

			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
			return button;
		}

		// Cr�e un bouton "v" pour un menu.
		protected GlyphButton CreateMenuButton(string command, string tooltip, MessageEventHandler handler)
		{
			GlyphButton button = new GlyphButton(this);
			button.Command = command;
			button.ButtonStyle = ButtonStyle.ToolItem;
			button.GlyphShape = GlyphShape.Menu;
			button.AutoFocus = false;
			button.Clicked += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, tooltip);
			return button;
		}


		// Ajoute une ic�ne.
		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut)
		{
			this.MenuAdd(vmenu, icon, command, text, shortcut, command);
		}
		
		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut, string name)
		{
			if ( text == "" )
			{
				vmenu.Items.Add(new MenuSeparator());
			}
			else
			{
				MenuItem item;
				
				if ( icon == "y/n" )
				{
					item = MenuItem.CreateYesNo(command, text, shortcut, name);
				}
				else
				{
					item = new MenuItem(command, icon, text, shortcut, name);
				}
				
				vmenu.Items.Add(item);
			}
		}


		// Donne l'objet en cours d'�dition, s'il existe.
		protected Objects.Abstract EditObject
		{
			get
			{
				if ( this.document == null )  return null;
				return this.document.Modifier.RetEditObject();
			}
		}


		protected DocumentType				documentType;
		protected InstallType				installType;
		protected DebugMode					debugMode;
		protected Settings.GlobalSettings	globalSettings;
		protected Document					document;
		protected TextLayout				title;
		protected int						tabIndex = 0;
		protected bool						ignoreChange = false;
		protected double					separatorWidth = 8;
	}
}
