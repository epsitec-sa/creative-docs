using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Abstract est la classe de base pour tous les panneaux des textes.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;

			this.label = new StaticText(this);
			this.fixIcon = new StaticText(this);

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Panel.Abstract.Extend);

			this.UpdateButtons();
		}

		// Crée un nouveau panneau.
		public static Abstract NewPanel(Text.Properties.WellKnownType type, Document document)
		{
			Abstract.StaticDocument = document;

			switch ( type )
			{
				case Common.Text.Properties.WellKnownType.Font:  return new Font(document);
			}

			return null;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
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

		// Retourne la hauteur pour le label supérieur.
		protected double LabelHeight
		{
			get
			{
				return this.IsLabelProperties ? 14 : 0;
			}
		}

		// Indique le mode des propriétés.
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

		// Indique si ce panneau possède 2 hauteurs différentes.
		protected virtual bool IsNormalAndExtended()
		{
			return this.isNormalAndExtended;
		}

		// Indique si le panneau est réduit (petite hauteur) ou étendu (grande hauteur).
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

		// Indique que la hauteur du panneau a changé.
		public void HeightChanged()
		{
			double h = this.DefaultHeight;
			if ( this.Height != h )
			{
				this.Height = h;
				this.ForceLayout();
			}
		}


		// Choix du style édité par le panneau.
		public Text.TextStyle TextStyle
		{
			get
			{
				return this.textStyle;
			}

			set
			{
				this.textStyle = value;
				this.PropertyToWidgets();

				this.label.Text = Abstract.LabelText(this.Type);

				this.fixIcon.Text = Misc.Image(Abstract.IconText(this.Type));
				ToolTip.Default.SetToolTip(this.fixIcon, Abstract.LabelText(this.Type));
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

		// Met à jour la géométrie.
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
			this.label.SetVisible(this.IsLabelProperties);

			rect = this.Client.Bounds;
			rect.Left += 1;
			rect.Width = this.extendedZoneWidth;
			rect.Top -= this.IsLabelProperties ? 2 : 8;
			rect.Bottom = rect.Top-13;
			this.fixIcon.Bounds = rect;

			rect.Left = this.Client.Bounds.Right-this.extendedZoneWidth+1;
			rect.Width = this.extendedZoneWidth-3;
			this.extendedButton.Bounds = rect;

			this.UpdateButtons();
		}

		// Met à jour les boutons.
		protected void UpdateButtons()
		{
			this.extendedButton.SetVisible(this.isNormalAndExtended);
			this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
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
		public virtual void OriginColorChange(Drawing.RichColor color)
		{
		}

		// Donne la couleur d'origine.
		public virtual Drawing.RichColor OriginColorGet()
		{
			return Drawing.RichColor.FromBrightness(0);
		}


		// Génère un événement pour dire que ça a changé.
		protected virtual void OnChanged()
		{
			if ( this.ignoreChanged )  return;

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


		// Donne le type.
		protected virtual Text.Properties.WellKnownType Type
		{
			get { return Common.Text.Properties.WellKnownType.Other; }
		}

		// Nom d'une propriété de style de texte.
		public static string LabelText(Text.Properties.WellKnownType type)
		{
			switch ( type )
			{
				case Common.Text.Properties.WellKnownType.Font:      return Res.Strings.Property.Abstract.TextFont;
				case Common.Text.Properties.WellKnownType.Margins:   return Res.Strings.Property.Abstract.TextJustif;
				case Common.Text.Properties.WellKnownType.Leading:   return Res.Strings.Property.Abstract.TextLine;
				case Common.Text.Properties.WellKnownType.Language:  return Res.Strings.Property.Abstract.Name;
			}
			return "";
		}

		// Nom d'une icône de style de texte.
		public static string IconText(Text.Properties.WellKnownType type)
		{
			switch ( type )
			{
				case Common.Text.Properties.WellKnownType.Font:      return "PropertyTextFont";
				case Common.Text.Properties.WellKnownType.Margins:   return "PropertyTextJustif";
				case Common.Text.Properties.WellKnownType.Leading:   return "PropertyTextLine";
				case Common.Text.Properties.WellKnownType.Language:  return "PropertyName";
			}
			return "";
		}


		// ATTENTION: Ceci n'est pas propre, mais je ne sais pas comment faire mieux.
		// Le constructeur de Common.Widget appelle DefaultHeight, qui doit
		// connaître le document pour déterminer la hauteur (avec LabelHeight).
		// Comme ce constructeur est appelé avant l'initialisation de this.document,
		// je n'ai pas trouvé d'autre moyen pour connaître le document que de le
		// mettre au préalable dans une variable statique !!!
		public static Document				StaticDocument;

		protected Document					document;
		protected double					backgroundIntensity = 1.0;
		protected Text.TextStyle			textStyle;
		protected bool						isExtendedSize = false;
		protected bool						isNormalAndExtended = false;
		protected double					extendedZoneWidth = 16;
		protected StaticText				label;
		protected StaticText				fixIcon;
		protected GlyphButton				extendedButton;
		protected bool						ignoreChanged = false;
	}
}
