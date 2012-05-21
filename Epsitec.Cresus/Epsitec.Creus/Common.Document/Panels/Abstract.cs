using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Abstract est la classe de base pour tous les panels.
	/// </summary>
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;

			this.Entered += this.HandleMouseEntered;
			this.Exited += this.HandleMouseExited;

			this.PreferredHeight = this.DefaultHeight;
			
			this.label = new StaticText (this);
			this.fixIcon = new StaticText(this);

			this.hiliteButton = new GlyphButton(this);
			this.hiliteButton.ButtonStyle = ButtonStyle.None;
			this.hiliteButton.GlyphShape = GlyphShape.ArrowRight;
			this.hiliteButton.AutoFocus = false;

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.AutoFocus = false;
			this.extendedButton.Clicked += this.ExtendedButtonClicked;
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Panel.Abstract.Extend);

			this.colorBlack = Drawing.Color.FromName("WindowFrame");
			this.UpdateButtons();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.Entered -= this.HandleMouseEntered;
				this.Exited -= this.HandleMouseExited;
				this.extendedButton.Clicked -= this.ExtendedButtonClicked;
			}
			
			base.Dispose(disposing);
		}

		
		public virtual double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight+30;
			}
		}

		protected double LabelHeight
		{
			//	Retourne la hauteur pour le label supérieur.
			get
			{
				return (this.IsLabelProperties || this is ModColor) ? 14 : 0;
			}
		}

		public bool IsLabelProperties
		{
			//	Indique le mode des propriétés.
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

		protected virtual bool IsNormalAndExtended()
		{
			//	Indique si ce panneau possède 2 hauteurs différentes.
			return this.isNormalAndExtended;
		}

		public bool IsExtendedSize
		{
			//	Indique si le panneau est réduit (petite hauteur) ou étendu (grande hauteur).
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

		public void HeightChanged()
		{
			//	Indique que la hauteur du panneau a changé.
			this.PreferredHeight = this.DefaultHeight;
		}

		public bool IsLayoutDirect
		{
			//	Indique si le panneau édite directement une propriété de calque.
			get { return this.isLayoutDirect; }
			set { this.isLayoutDirect = value; }
		}

		public bool IsObjectHilite
		{
			//	Indique si la souris montre un objet.
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

		public bool IsHilite
		{
			//	Etat survolé du panneau.
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


		private void HandleMouseEntered(object sender, MessageEventArgs e)
		{
			//	La souris est entrée dans le panneau.
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

		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			//	La souris est sortie du panneau.
			if ( this.document.Modifier == null )  return;
			if ( this.document.Modifier.PropertiesDetailMany == false )  return;
			if ( this.property == null )  return;

			int total = this.property.Owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = this.property.Owners[i] as Objects.Abstract;
				if ( obj == null )  continue;
				obj.IsHilite = false;
			}
		}


		public Properties.Abstract Property
		{
			//	Choix de la propriété éditée par le panneau.
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

		public void UpdateValues()
		{
			//	Met à jour toutes les valeurs du panneau.
			this.PropertyToWidgets();
		}

		public virtual void UpdateGeometry()
		{
			//	Met à jour après un changement de géométrie.
		}

		protected virtual void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			this.UpdateButtons();
		}

		protected virtual void WidgetsToProperty()
		{
			//	Widgets -> propriété.
		}


		public virtual bool DefaultFocus()
		{
			//	Met le focus par défaut dans ce panneau.
			return false;
		}


		protected Rectangle UsefulZone
		{
			//	Retourne la zone rectangulaire utile pour les widgets.
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

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.extendedButton == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth+5;
			rect.Right -= this.extendedZoneWidth+5;
			rect.Top -= 1;
			rect.Bottom = rect.Top-this.LabelHeight;
			this.label.SetManualBounds(rect);
			this.label.Visibility = (this.IsLabelProperties || this is ModColor);

			rect = this.Client.Bounds;
			rect.Left += 1;
			rect.Width = this.extendedZoneWidth;
			rect.Top -= (this.IsLabelProperties || this is ModColor) ? 3 : 9;
			rect.Bottom = rect.Top-13;
			this.fixIcon.SetManualBounds(rect);
			this.hiliteButton.SetManualBounds(rect);

			rect.Left = this.Client.Bounds.Right-this.extendedZoneWidth+1;
			rect.Width = this.extendedZoneWidth-3;
			this.extendedButton.SetManualBounds(rect);

			this.UpdateButtons();
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons.
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

			this.extendedButton.Visibility = (this.isNormalAndExtended && !this.isLayoutDirect);
			this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
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


		protected virtual void OnChanged()
		{
			//	Génère un événement pour dire que ça a changé.
			if ( this.ignoreChanged )  return;

			if ( this.property != null )
			{
				int id = Properties.Aggregate.UniqueId(this.document.Aggregates, this.property);
				string name = string.Format(Res.Strings.Action.PropertyChange, Properties.Abstract.Text(this.property.Type));
				this.document.Modifier.OpletQueueBeginAction(name, "ChangeProperty", id);
				this.WidgetsToProperty();
				this.document.Modifier.OpletQueueValidateAction();
			}

			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		//	Génère un événement pour dire que la couleur d'origine a changé.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un écoute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;


		//	Le bouton pour étendre/réduire le panneau a été cliqué.
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
			color = Drawing.Color.FromRgb(color.R*this.backgroundIntensity, color.G*this.backgroundIntensity, color.B*this.backgroundIntensity);
#else
			Drawing.Color cap = adorner.ColorCaption;
			Drawing.Color color = Drawing.Color.FromAlphaRgb(1.0-this.backgroundIntensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);
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

			if ( this.property != null && this.property.IsStyle )
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


		public static void SetText(Widget widget, string text)
		{
			//	Met un texte dans un widget quelconque.
			if ( widget.Text != text )
			{
				widget.Text = text;
			}
		}


		//	ATTENTION: Ceci n'est pas propre, mais je ne sais pas comment faire mieux.
		//	Le constructeur de Common.Widget appelle DefaultHeight, qui doit
		//	connaître le document pour déterminer la hauteur (avec LabelHeight).
		//	Comme ce constructeur est appelé avant l'initialisation de this.document,
		//	je n'ai pas trouvé d'autre moyen pour connaître le document que de le
		//	mettre au préalable dans une variable statique !!!
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
		protected bool						isLayoutDirect = false;
		protected bool						isHilite = false;
		protected bool						isObjectHilite = false;
		protected bool						ignoreChanged = false;
	}
}
