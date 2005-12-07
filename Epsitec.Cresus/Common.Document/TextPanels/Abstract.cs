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

		// Retourne la marge supérieure.
		public virtual double TopMargin
		{
			get
			{
				return 0;
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
					this.document.Modifier.IsTextPanelExtended(this, this.isExtendedSize);
					this.UpdateAfterChanging();
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


		// Met à jour après un changement du wrapper.
		protected virtual void UpdateAfterChanging()
		{
			this.UpdateButtons();
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
			this.label.Visibility = (this.IsLabelProperties);

			rect = this.Client.Bounds;
			rect.Left += 1;
			rect.Width = this.extendedZoneWidth;
			rect.Top -= this.IsLabelProperties ? 2 : 8;
			rect.Bottom = rect.Top-13;
			this.fixIcon.Bounds = rect;

			rect.Left = this.Client.Bounds.Right-this.extendedZoneWidth+1;
			rect.Width = this.extendedZoneWidth-3;
			this.extendedButton.Bounds = rect;
		}

		// Met à jour les boutons.
		protected void UpdateButtons()
		{
			this.extendedButton.Visibility = (this.isNormalAndExtended);
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


		
		// Le bouton pour étendre/réduire le panneau a été cliqué.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.IsExtendedSize = !this.isExtendedSize;
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

			rect.Deflate(0.5, 0.5);
			graphics.AddLine(rect.Left, rect.Bottom-0.5, rect.Left, rect.Top-0.5);
			graphics.AddLine(rect.Left+this.extendedZoneWidth, rect.Bottom-0.5, rect.Left+this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right-this.extendedZoneWidth, rect.Bottom-0.5, rect.Right-this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left-0.5, rect.Top, rect.Right+0.5, rect.Top);
			graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		// Crée un bouton.
		protected IconButton CreateIconButton(string icon, string tooltip, MessageEventHandler handler)
		{
			return this.CreateIconButton(icon, tooltip, handler, true);
		}
		
		protected IconButton CreateIconButton(string icon, string tooltip, MessageEventHandler handler, bool activable)
		{
			IconButton button = new IconButton(this);

			if ( icon.StartsWith("manifest:") )
			{
				button.IconName = icon;
			}
			else
			{
				button.Text = icon;
			}

			if ( activable )
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
				button.AcceptThreeState = true;
			}

			button.AutoFocus = false;
			button.Clicked += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, tooltip);
			return button;
		}

		// Modifie l'état d'un bouton à 3 états.
		protected void ActiveIconButton(IconButton button, bool active, bool defined)
		{
			if ( active && defined )
			{
				button.ActiveState = ActiveState.Yes;
			}
			else if ( active )
			{
				button.ActiveState = ActiveState.Maybe;
			}
			else
			{
				button.ActiveState = ActiveState.No;
			}
		}

		// Crée un bouton "x" pour effacer une propriété.
		protected IconButton CreateClearButton(MessageEventHandler handler)
		{
			IconButton button = new IconButton(this);

			button.AutoFocus = false;
			button.Clicked += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button.IconName = Misc.Icon("Nothing");
			ToolTip.Default.SetToolTip(button, Res.Strings.TextPanel.Clear);
			
			return button;
		}

		// Crée un TextFieldLabel.
		protected Widgets.TextFieldLabel CreateTextFieldLabel(string tooltip, string shortText, string longText, double minRange, double maxRange, double step, bool simply, EventHandler handler)
		{
			Widgets.TextFieldLabel field = new Widgets.TextFieldLabel(this, simply);

			field.LabelShortText = shortText;
			field.LabelLongText  = longText;
			
			field.TextFieldReal.FactorMinRange = (decimal) minRange;
			field.TextFieldReal.FactorMaxRange = (decimal) maxRange;
			field.TextFieldReal.FactorStep     = (decimal) step;
			
			if ( !simply )
			{
				this.document.Modifier.AdaptTextFieldRealDimension(field.TextFieldReal);
			}
			
			field.TextFieldReal.DefocusAction     = DefocusAction.AutoAcceptOrRejectEdition;
			field.TextFieldReal.AutoSelectOnFocus = true;
			field.TextFieldReal.SwallowEscape     = true;
			field.TextFieldReal.EditionAccepted  += handler;
//-			field.TextFieldReal.ValueChanged += handler;
			
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			
			return field;
		}

		// Modifie le mode d'un TextFieldLabel.
		protected void ProposalTextFieldLabel(Widgets.TextFieldLabel field, bool proposal)
		{
			field.TextFieldReal.TextDisplayMode = proposal ? TextDisplayMode.Proposal : TextDisplayMode.Defined;
		}

		// Modifie le mode d'un TextFieldCombo.
		protected void ProposalTextFieldCombo(TextFieldCombo field, bool proposal)
		{
			field.TextDisplayMode = proposal ? TextDisplayMode.Proposal : TextDisplayMode.Defined;
		}

		
		// Met un texte dans un widget quelconque.
		public static void SetText(Widget widget, string text)
		{
			if ( widget.Text != text )
			{
				widget.Text = text;
			}
		}


		// Génère un événement pour dire que la couleur d'origine a changé.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un écoute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;

		
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
		protected int						tabIndex = 0;
	}
}
