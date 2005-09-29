using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Abstract est la classe de base pour tous les rubans.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;

			this.title = new TextLayout();
			this.title.DefaultFont     = this.DefaultFont;
			this.title.DefaultFontSize = this.DefaultFontSize;

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowRight;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = this.tabIndex++;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Panel.Abstract.Extend);

			this.UpdateButtons();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.extendedButton.Clicked -= new MessageEventHandler(this.ExtendedButtonClicked);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return this.isExtendedSize ? this.ExtendWidth : this.CompactWidth;
			}
		}

		// Retourne la largeur compacte.
		public virtual double CompactWidth
		{
			get
			{
				return 8+22+22;
			}
		}

		// Retourne la largeur �tendue.
		public virtual double ExtendWidth
		{
			get
			{
				return 8+22+22+22+22;
			}
		}

		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.LabelHeight+8+22+22;
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

		// Indique qu'il faudra mettre � jour tout le contenu.
		public void SetDirtyContent()
		{
			this.isDirtyContent = true;
			this.Update();  // m�j imm�diate si le ruban est visible
		}

		// Indique si ce panneau poss�de 2 hauteurs diff�rentes.
		public virtual bool IsNormalAndExtended()
		{
			return this.isNormalAndExtended;
		}

		// Indique si le panneau est r�duit (petite largeur) ou �tendu (grande largeur).
		public virtual bool IsExtendedSize
		{
			get
			{
				return this.isExtendedSize;
			}

			set
			{
				if ( this.isExtendedSize != value )
				{
					this.isExtendedSize = value;
					this.UpdateButtons();
					this.WidthChanged();
				}
			}
		}

		// Met � jour le contenu, si n�cessaire.
		protected void Update()
		{
			if ( !this.IsVisible )  return;

			if ( this.isDirtyContent )
			{
				this.DoUpdateContent();
				this.isDirtyContent = false;  // propre
			}
		}
		
		// Effectue la mise � jour du contenu.
		protected virtual void DoUpdateContent()
		{
		}
		
		// Appel� par Widget lorsque la visibilit� change.
		protected override void OnVisibleChanged()
		{
			base.OnVisibleChanged();
			this.Update();  // m�j si visible et sale
		}

		// Indique que la largeur du panneau a chang�.
		protected void WidthChanged()
		{
			double w = this.DefaultWidth;
			if ( this.Width != w )
			{
				this.Width = w;
				this.ForceLayout();
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

			if ( this.extendedButton == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Right -= 1;
			rect.Left = rect.Right-(this.LabelHeight-2);
			rect.Top -= 1;
			rect.Bottom = rect.Top-(this.LabelHeight-2);
			this.extendedButton.Bounds = rect;

			this.UpdateButtons();
		}

		// Met � jour les boutons.
		protected virtual void UpdateButtons()
		{
			this.extendedButton.SetVisible(this.isNormalAndExtended);
			this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowLeft : GlyphShape.ArrowRight;
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


		// G�n�re un �v�nement pour dire que �a a chang�.
		protected virtual void OnChanged()
		{
			if ( this.ignoreChanged )  return;

			if ( this.Changed != null )  // qq'un �coute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		// G�n�re un �v�nement pour dire que la couleur d'origine a chang�.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un �coute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;


		// Le bouton pour �tendre/r�duire le panneau a �t� cliqu�.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.IsExtendedSize = !this.isExtendedSize;

			if ( this.IsExtendedSize )
			{
				this.CompactOthers(this);
			}
		}

		// Compacte tous les rubans sauf un.
		protected void CompactOthers(Abstract activeRibbon)
		{
			RibbonContainer container = this.Parent as RibbonContainer;
			if ( container == null )  return;

			foreach ( Abstract ribbon in container.Children )
			{
				if ( ribbon == null || ribbon == activeRibbon )  continue;
				ribbon.IsExtendedSize = false;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetState state = this.PaintState;
			adorner.PaintRibbonSectionBackground(graphics, rect, this.LabelHeight, state);

			Point pos = new Point(rect.Left+3, rect.Top-this.LabelHeight);
			this.title.LayoutSize = new Size(rect.Width-this.LabelHeight, this.LabelHeight);
			adorner.PaintRibbonSectionTextLayout(graphics, pos, this.title, state);
		}


		// Cr�e un s�parateur.
		protected void CreateSeparator(ref IconSeparator sep)
		{
			sep = new IconSeparator(this);
		}

		// Cr�e un IconButton.
		protected void CreateButton(ref IconButton button, string icon, string tooltip, MessageEventHandler handler)
		{
			button = new IconButton(Misc.Icon(icon));
			button.Parent = this;
			button.Clicked += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, tooltip);
		}

		// 
		protected IconButton CreateIconButton(string command, string icon, string tooltip)
		{
			IconButton button = new IconButton(command, icon, command);
			button.Parent = this;
			ToolTip.Default.SetToolTip(button, tooltip);
			return button;
		}


		protected Document					document;
		protected TextLayout				title;
		protected double					backgroundIntensity = 1.0;
		protected bool						isExtendedSize = false;
		protected bool						isNormalAndExtended = true;
		protected GlyphButton				extendedButton;
		protected bool						ignoreChanged = false;
		protected int						tabIndex = 0;
		protected bool						isDirtyContent;
		protected double					separatorWidth = 8;
	}
}
