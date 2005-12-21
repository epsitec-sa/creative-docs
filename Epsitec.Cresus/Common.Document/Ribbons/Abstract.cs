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

		public virtual void NotifyChanged(string changed)
		{
		}

		
		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*2;
			}
		}

		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight + 8 + 22 + 5 + 22;
			}
		}

		protected double LabelHeight
		{
			//	Retourne la hauteur pour le label supérieur.
			get
			{
				return 14;
			}
		}


		protected Rectangle UsefulZone
		{
			//	Retourne la zone rectangulaire utile pour les widgets.
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Top -= this.LabelHeight;
				rect.Deflate(4);
				return rect;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();
		}


		public virtual void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
		}

		public virtual void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
		}

		public virtual int OriginColorRank()
		{
			//	Retourne le rang de la couleur d'origine.
			return -1;
		}

		public virtual void OriginColorChange(Drawing.RichColor color)
		{
			//	Modifie la couleur d'origine.
		}

		public virtual Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return Drawing.RichColor.FromBrightness(0);
		}


		protected virtual void OnOriginColorChanged()
		{
			//	Génère un événement pour dire que la couleur d'origine a changé.
			if ( this.OriginColorChanged != null )  // qq'un écoute ?
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


		protected IconButton CreateIconButton(string command)
		{
			//	Crée un bouton pour une commande.
			return this.CreateIconButton(command, "0");
		}

		protected IconButton CreateIconButton(string command, string iconSize)
		{
			//	Crée un bouton pour une commande, en précisant la taille préférée pour l'icône.
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

		protected GlyphButton CreateMenuButton(string command, string tooltip, MessageEventHandler handler)
		{
			//	Crée un bouton "v" pour un menu.
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


		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut)
		{
			//	Ajoute une icône.
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


		protected Objects.Abstract EditObject
		{
			//	Donne l'objet en cours d'édition, s'il existe.
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
