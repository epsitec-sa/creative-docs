using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Abstract est la classe de base pour tous les panels.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;

			this.Entered += new MessageEventHandler(this.HandleMouseEntered);
			this.Exited += new MessageEventHandler(this.HandleMouseExited);

			this.label = new StaticText(this);
			this.fixIcon = new StaticText(this);

			this.hiliteButton = new GlyphButton(this);
			this.hiliteButton.ButtonStyle = ButtonStyle.None;
			this.hiliteButton.GlyphShape = GlyphShape.ArrowRight;

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Panel.Abstract.Extend);

			this.colorBlack = Drawing.Color.FromName("WindowFrame");
			this.UpdateButtons();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.Entered -= new MessageEventHandler(this.HandleMouseEntered);
				this.Exited -= new MessageEventHandler(this.HandleMouseExited);
				this.extendedButton.Clicked -= new MessageEventHandler(this.ExtendedButtonClicked);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.LabelHeight+30;
			}
		}

		// Retourne la hauteur pour le label sup�rieur.
		protected double LabelHeight
		{
			get
			{
				return (this.IsLabelProperties || this is ModColor) ? 14 : 0;
			}
		}

		// Indique le mode des propri�t�s.
		public bool IsLabelProperties
		{
			get
			{
				if ( this.document != null )
				{
					return this.document.GlobalSettings.LabelProperties;
				}

				System.Diagnostics.Debug.Assert(Abstract.StaticDocument != null);
				return Abstract.StaticDocument.GlobalSettings.LabelProperties;
			}
		}

		// Indique si ce panneau poss�de 2 hauteurs diff�rentes.
		protected virtual bool IsNormalAndExtended()
		{
			return this.isNormalAndExtended;
		}

		// Indique si le panneau est r�duit (petite hauteur) ou �tendu (grande hauteur).
		public bool IsExtendedSize
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
					this.HeightChanged();
				}
			}
		}

		// Indique que la hauteur du panneau a chang�.
		public void HeightChanged()
		{
			double h = this.DefaultHeight;
			if ( this.Height != h )
			{
				this.Height = h;
				this.ForceLayout();
			}
		}

		// Indique si le panneau �dite directement un style.
		public bool IsStyleDirect
		{
			get { return this.isStyleDirect; }
			set { this.isStyleDirect = value; }
		}

		// Indique si le panneau �dite directement une propri�t� de calque.
		public bool IsLayoutDirect
		{
			get { return this.isLayoutDirect; }
			set { this.isLayoutDirect = value; }
		}

		// Indique si la souris montre un objet.
		public bool IsObjectHilite
		{
			get
			{
				return this.isObjectHilite;
			}

			set
			{
				if ( this.isObjectHilite != value )
				{
					this.isObjectHilite = value;
					this.UpdateButtons();
				}
			}
		}

		// Etat survol� du panneau.
		public bool IsHilite
		{
			get
			{
				return this.isHilite;
			}

			set
			{
				if ( this.isHilite != value )
				{
					this.isHilite = value;
					this.UpdateButtons();
					this.Invalidate();
				}
			}
		}


		// La souris est entr�e dans le panneau.
		private void HandleMouseEntered(object sender, MessageEventArgs e)
		{
			if ( !this.document.Modifier.PropertiesDetailMany )  return;
			if ( this.property == null )  return;

			int total = this.property.Owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = this.property.Owners[i] as Objects.Abstract;
				if ( obj == null )  continue;
				obj.IsHilite = true;
			}
		}

		// La souris est sortie du panneau.
		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			if ( !this.document.Modifier.PropertiesDetailMany )  return;
			if ( this.property == null )  return;

			int total = this.property.Owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = this.property.Owners[i] as Objects.Abstract;
				if ( obj == null )  continue;
				obj.IsHilite = false;
			}
		}


		// Choix de la propri�t� �dit�e par le panneau.
		public Properties.Abstract Property
		{
			get
			{
				return this.property;
			}

			set
			{
				this.property = value;
				this.backgroundIntensity = Properties.Abstract.BackgroundIntensity(this.property.Type);
				this.PropertyToWidgets();

				this.label.Text = Properties.Abstract.Text(this.property.Type);

				this.fixIcon.Text = Misc.Image(Properties.Abstract.IconText(this.property.Type));
				ToolTip.Default.SetToolTip(this.fixIcon, Properties.Abstract.Text(this.property.Type));
			}
		}

		// Met � jour toutes les valeurs du panneau.
		public void UpdateValues()
		{
			this.PropertyToWidgets();
		}

		// Propri�t� -> widgets.
		protected virtual void PropertyToWidgets()
		{
			this.UpdateButtons();
		}

		// Widgets -> propri�t�.
		protected virtual void WidgetsToProperty()
		{
		}


		// Met le focus par d�faut dans ce panneau.
		public virtual bool DefaultFocus()
		{
			return false;
		}


		// Retourne la zone rectangulaire utile pour les widgets.
		protected Rectangle UsefulZone
		{
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Top -= this.LabelHeight;
				rect.Left += this.extendedZoneWidth;
				rect.Right -= this.extendedZoneWidth;
				rect.Deflate(5);
				return rect;
			}
		}

		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.extendedButton == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth+5;
			rect.Right -= this.extendedZoneWidth+5;
			rect.Top -= 1;
			rect.Bottom = rect.Top-this.LabelHeight;
			this.label.Bounds = rect;
			this.label.Visibility = (this.IsLabelProperties || this is ModColor);

			rect = this.Client.Bounds;
			rect.Left += 1;
			rect.Width = this.extendedZoneWidth;
			rect.Top -= (this.IsLabelProperties || this is ModColor) ? 2 : 8;
			rect.Bottom = rect.Top-13;
			this.fixIcon.Bounds = rect;
			this.hiliteButton.Bounds = rect;

			rect.Left = this.Client.Bounds.Right-this.extendedZoneWidth+1;
			rect.Width = this.extendedZoneWidth-3;
			this.extendedButton.Bounds = rect;

			this.UpdateButtons();
		}

		// Met � jour les boutons.
		protected void UpdateButtons()
		{
			if ( this.isObjectHilite )
			{
				this.fixIcon.Visibility = false;
				this.hiliteButton.Visibility = (this.isHilite);
			}
			else
			{
				this.fixIcon.Visibility = true;
				this.hiliteButton.Visibility = false;
			}

			this.extendedButton.Visibility = (this.isNormalAndExtended && !this.isStyleDirect && !this.isLayoutDirect);
			this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
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

			if ( this.property != null )
			{
				int id = Properties.Aggregate.UniqueId(this.document.Aggregates, this.property);
				string name = string.Format(Res.Strings.Action.PropertyChange, Properties.Abstract.Text(this.property.Type));
				this.document.Modifier.OpletQueueBeginAction(name, "ChangeProperty", id);
				this.WidgetsToProperty();
				this.document.Modifier.OpletQueueValidateAction();
			}

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
			this.property.IsExtendedSize = this.isExtendedSize;
			this.document.Modifier.IsPropertiesExtended(this.property.Type, this.isExtendedSize);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;

			Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
#if false
			Drawing.Color color = adorner.ColorWindow;
			color = Drawing.Color.FromRGB(color.R*this.backgroundIntensity, color.G*this.backgroundIntensity, color.B*this.backgroundIntensity);
#else
			Drawing.Color cap = adorner.ColorCaption;
			Drawing.Color color = Drawing.Color.FromARGB(1.0-this.backgroundIntensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);
#endif
			graphics.RenderSolid(color);

			if ( this.property != null && this.property.IsMulti )
			{
				Rectangle part = rect;
				part.Width = this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(DrawingContext.ColorMulti);

				part.Left = rect.Left+this.extendedZoneWidth;
				part.Right = rect.Right;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(DrawingContext.ColorMultiBack);
			}

			if ( (this.property != null && this.property.IsStyle) || this.isStyleDirect )
			{
				Rectangle part = rect;
				part.Width = this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(DrawingContext.ColorStyle);

				part.Left = rect.Left+this.extendedZoneWidth;
				part.Right = rect.Right;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(DrawingContext.ColorStyleBack);
			}

			if ( this.isHilite )
			{
				Rectangle part = rect;
				part.Width = this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(context.HiliteSurfaceColor);
			}

			rect.Deflate(0.5, 0.5);
			graphics.AddLine(rect.Left, rect.Bottom-0.5, rect.Left, rect.Top-0.5);
			graphics.AddLine(rect.Left+this.extendedZoneWidth, rect.Bottom-0.5, rect.Left+this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right-this.extendedZoneWidth, rect.Bottom-0.5, rect.Right-this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left-0.5, rect.Top, rect.Right+0.5, rect.Top);
			graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		// Met un texte dans un widget quelconque.
		public static void SetText(Widget widget, string text)
		{
			if ( widget.Text != text )
			{
				widget.Text = text;
			}
		}


		// ATTENTION: Ceci n'est pas propre, mais je ne sais pas comment faire mieux.
		// Le constructeur de Common.Widget appelle DefaultHeight, qui doit
		// conna�tre le document pour d�terminer la hauteur (avec LabelHeight).
		// Comme ce constructeur est appel� avant l'initialisation de this.document,
		// je n'ai pas trouv� d'autre moyen pour conna�tre le document que de le
		// mettre au pr�alable dans une variable statique !!!
		public static Document				StaticDocument;

		protected Document					document;
		protected Drawing.Color				colorBlack;
		protected double					backgroundIntensity = 1.0;
		protected Properties.Abstract		property;
		protected bool						isExtendedSize = false;
		protected bool						isNormalAndExtended = false;
		protected double					extendedZoneWidth = 16;
		protected StaticText				label;
		protected StaticText				fixIcon;
		protected GlyphButton				hiliteButton;
		protected GlyphButton				extendedButton;
		protected bool						isStyleDirect = false;
		protected bool						isLayoutDirect = false;
		protected bool						isHilite = false;
		protected bool						isObjectHilite = false;
		protected bool						ignoreChanged = false;
	}
}
