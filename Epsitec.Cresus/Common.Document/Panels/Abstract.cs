using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Abstract est la classe de base pour tous les panels.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class Abstract : Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;

			this.Entered += new MessageEventHandler(this.HandleMouseEntered);
			this.Exited += new MessageEventHandler(this.HandleMouseExited);

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, "Etend ou réduit le panneau");

			this.stylesButton = new GlyphButton(this);
			this.stylesButton.ButtonStyle = ButtonStyle.Icon;
			this.stylesButton.GlyphShape = GlyphShape.Dots;
			this.stylesButton.Clicked += new MessageEventHandler(this.StylesButtonClicked);
			this.stylesButton.TabIndex = 1000;
			this.stylesButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.stylesButton, "Styles");

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
				this.stylesButton.Clicked -= new MessageEventHandler(this.StylesButtonClicked);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return 30;
			}
		}

		// Indique si ce panneau possède 2 hauteurs différentes.
		public virtual bool IsNormalAndExtended()
		{
			return this.isNormalAndExtended;
		}

		// Indique si le panneau est réduit (petite hauteur) ou étendu (grande hauteur).
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
					this.Height = this.DefaultHeight;
					this.ForceLayout();
				}
			}
		}

		// Indique si le panneau édite directement un style.
		public bool IsStyleDirect
		{
			get { return this.isStyleDirect; }
			set { this.isStyleDirect = value; }
		}

		// Indique si le panneau édite directement une propriété de calque.
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

		// Etat survolé du panneau.
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


		// La souris est entrée dans le panneau.
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


		// Choix de la propriété éditée par le panneau.
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
			}
		}

		// Met à jour toutes les valeurs du panneau.
		public void UpdateValues()
		{
			this.PropertyToWidgets();
		}

		// Propriété -> widgets.
		protected virtual void PropertyToWidgets()
		{
			this.UpdateButtons();
		}

		// Widgets -> propriété.
		protected virtual void WidgetsToProperty()
		{
		}


		// Met le focus par défaut dans ce panneau.
		public virtual bool DefaultFocus()
		{
			return false;
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.extendedButton == null )  return;

			Rectangle rTop = this.Client.Bounds;
			rTop.Left += 2;
			rTop.Width = this.extendedZoneWidth-3;
			rTop.Top -= 8;
			rTop.Bottom = rTop.Top-13;
			this.extendedButton.Bounds = rTop;

			rTop.Offset(this.Client.Width-this.extendedZoneWidth-1, 0);
			this.stylesButton.Bounds = rTop;

			this.UpdateButtons();
		}

		// Met à jour les boutons.
		protected void UpdateButtons()
		{
			if ( this.isObjectHilite )
			{
				this.extendedButton.ButtonStyle = ButtonStyle.None;
				this.extendedButton.GlyphShape = GlyphShape.ArrowRight;
				this.extendedButton.SetVisible(this.isHilite);
			}
			else
			{
				this.extendedButton.ButtonStyle = ButtonStyle.Icon;
				this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
				this.extendedButton.SetVisible(this.isNormalAndExtended && !this.isStyleDirect && !this.isLayoutDirect);
			}

			this.stylesButton.SetVisible(!this.isStyleDirect && !this.isLayoutDirect);
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public virtual void OriginColorDeselect()
		{
		}

		// Sélectionne l'origine de couleur.
		public virtual void OriginColorSelect(int rank)
		{
		}

		// Retourne le rang de la couleur d'origine.
		public virtual int OriginColorRank()
		{
			return -1;
		}

		// Modifie la couleur d'origine.
		public virtual void OriginColorChange(Drawing.Color color)
		{
		}

		// Donne la couleur d'origine.
		public virtual Drawing.Color OriginColorGet()
		{
			return Drawing.Color.FromBrightness(0);
		}


		// Génère un événement pour dire que ça a changé.
		protected virtual void OnChanged()
		{
			if ( this.ignoreChanged )  return;

			int id = (int) this.property.Type;
			string name = string.Format("Changement de l'attribut \"{0}\"", Properties.Abstract.Text(this.property.Type));
			this.document.Modifier.OpletQueueBeginAction(name, "ChangeProperty", id);
			this.WidgetsToProperty();
			this.document.Modifier.OpletQueueValidateAction();

			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		// Génère un événement pour dire que la couleur d'origine a changé.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un écoute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;


		// Le bouton pour étendre/réduire le panneau a été cliqué.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.IsExtendedSize = !this.isExtendedSize;
			this.property.IsExtendedSize = this.isExtendedSize;
			this.document.Modifier.IsPropertiesExtended(this.property.Type, this.isExtendedSize);
		}

		// Le bouton des styles a été cliqué.
		private void StylesButtonClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;
			Point pos = button.MapClientToScreen(new Point(0, button.Height));
			VMenu menu = this.document.Modifier.CreateStyleMenu(this.property);
			menu.Host = this;
			pos.X -= menu.Width;
			menu.ShowAsContextMenu(this.Window, pos);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
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
				part.Right = rect.Right-this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(DrawingContext.ColorMultiBack);
			}

			if ( (this.property != null && this.property.IsStyle) || this.isStyleDirect )
			{
				Rectangle part = rect;
				part.Left = part.Right-this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(DrawingContext.ColorStyle);

				part.Left = rect.Left+this.extendedZoneWidth;
				part.Right = rect.Right-this.extendedZoneWidth;
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


		protected Document					document;
		protected Drawing.Color				colorBlack;
		protected double					backgroundIntensity = 1.0;
		protected Properties.Abstract		property;
		protected bool						isExtendedSize = false;
		protected bool						isNormalAndExtended = false;
		protected double					extendedZoneWidth = 16;
		protected GlyphButton				extendedButton;
		protected GlyphButton				stylesButton;
		protected bool						isStyleDirect = false;
		protected bool						isLayoutDirect = false;
		protected bool						isHilite = false;
		protected bool						isObjectHilite = false;
		protected bool						ignoreChanged = false;
	}
}
