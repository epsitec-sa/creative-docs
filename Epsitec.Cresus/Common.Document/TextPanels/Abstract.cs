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
			this.extendedButton.AutoFocus = false;
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


		// Indique si ce panneau est visible pour un filtre donn�.
		public static bool IsFilterShow(string panel, string filter)
		{
			if ( panel == "Box" )  return false;  // provisoire...
			if ( filter == "All" )  return true;

			bool F = filter == "Frequently";
			bool U = filter == "Usual";
			bool P = filter == "Paragraph";
			bool C = filter == "Character";
			bool o = false;

			switch ( panel )
			{
				//                         F    U    P    C
				case "Justif":    return ( F || U || P || o );
				case "Leading":   return ( F || o || P || o );
				case "Margins":   return ( o || o || P || o );
				case "Spaces":    return ( o || o || P || o );
				case "Keep":      return ( o || o || P || o );
				case "Font":      return ( F || U || o || C );
				case "Xline":     return ( F || o || o || C );
				case "Xscript":   return ( F || o || o || C );
				case "Box":       return ( o || o || o || C );
				case "Language":  return ( o || o || o || C );
			}
			return true;
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.LabelHeight+30;
			}
		}

		// Retourne la marge sup�rieure.
		public virtual double TopMargin
		{
			get
			{
				return 0;
			}
		}

		// Retourne la hauteur pour le label sup�rieur.
		protected double LabelHeight
		{
			get
			{
				return this.IsLabelProperties ? 14 : 0;
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
					this.document.Modifier.IsTextPanelExtended(this, this.isExtendedSize);
					this.UpdateAfterChanging();
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


		// Met � jour apr�s un changement du wrapper.
		protected virtual void UpdateAfterChanging()
		{
			this.UpdateButtons();
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

		// Met � jour les boutons.
		protected void UpdateButtons()
		{
			this.extendedButton.Visibility = (this.isNormalAndExtended);
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


		
		// Le bouton pour �tendre/r�duire le panneau a �t� cliqu�.
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


		// Cr�e un bouton.
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

		// Modifie l'�tat d'un bouton � 3 �tats.
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

		// Cr�e un bouton "x" pour effacer une propri�t�.
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

		// Cr�e un TextFieldLabel.
		protected Widgets.TextFieldLabel CreateTextFieldLabel(string tooltip, string shortText, string longText, double minRange, double maxRange, double step, Widgets.TextFieldLabel.Type type, EventHandler handler)
		{
			Widgets.TextFieldLabel field = new Widgets.TextFieldLabel(this, type);

			field.LabelShortText = shortText;
			field.LabelLongText  = longText;
			
			if ( type == Widgets.TextFieldLabel.Type.TextField )
			{
				field.TextField.EditionAccepted += handler;
			}
			
			if ( type == Widgets.TextFieldLabel.Type.TextFieldReal )
			{
				field.SetRangeDimension(this.document, minRange, maxRange, step);
				field.TextFieldReal.EditionAccepted += handler;
			}

			if ( type == Widgets.TextFieldLabel.Type.TextFieldUnit )
			{
				field.TextFieldReal1.EditionAccepted += handler;
				field.TextFieldReal2.EditionAccepted += handler;
			}

			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);

			return field;
		}

		// Cr�e un TextFieldLabel en %.
		protected Widgets.TextFieldLabel CreateTextFieldLabelPercent(string tooltip, string shortText, string longText, double minRange, double maxRange, double step, EventHandler handler)
		{
			Widgets.TextFieldLabel field = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);

			field.LabelShortText = shortText;
			field.LabelLongText  = longText;
			field.SetRangePercents(this.document, minRange, maxRange, step);
			
			field.TextFieldReal.EditionAccepted += handler;
			
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			
			return field;
		}

		// Modifie la valeur d'un TextFieldReal.
		protected void SetTextFieldRealValue(TextFieldReal field, double value, Common.Text.Properties.SizeUnits units, bool isDefined, bool disabledIfUndefined)
		{
			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				field.InternalValue = (decimal) value;
			}
			else
			{
				field.InternalValue = (decimal) value;
			}

			if ( disabledIfUndefined )
			{
				field.TextDisplayMode = isDefined ? TextDisplayMode.Defined : TextDisplayMode.Default;
				field.Enable = isDefined;
			}
			else
			{
				field.TextDisplayMode = isDefined ? TextDisplayMode.Defined : TextDisplayMode.Proposal;
			}
		}

		// Donne la valeur d'un TextFieldReal.
		protected void GetTextFieldRealValue(TextFieldReal field, out double value, out Common.Text.Properties.SizeUnits units, out bool isDefined)
		{
			if ( field.UnitType == RealUnitType.Percent )
			{
				value = (double) field.InternalValue;
				units = Common.Text.Properties.SizeUnits.Percent;
			}
			else
			{
				value = (double) field.InternalValue;
				units = Common.Text.Properties.SizeUnits.Points;
			}

			isDefined = (field.Text != "");
		}

		// Modifie la valeur d'un TextFieldReal en %.
		protected void SetTextFieldRealPercent(TextFieldReal field, double value, bool isDefined, bool disabledIfUndefined)
		{
			field.InternalValue = (decimal) value;

			if ( disabledIfUndefined )
			{
				field.TextDisplayMode = isDefined ? TextDisplayMode.Defined : TextDisplayMode.Default;
				field.Enable = isDefined;
			}
			else
			{
				field.TextDisplayMode = isDefined ? TextDisplayMode.Defined : TextDisplayMode.Proposal;
			}
		}

		// Donne la valeur d'un TextFieldReal en %.
		protected void GetTextFieldRealPercent(TextFieldReal field, out double value, out bool isDefined)
		{
			value = (double) field.InternalValue;
			isDefined = (field.Text != "");
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


		// Cr�e un �chantilon de couleur.
		protected ColorSample CreateColorSample(string tooltip, MessageEventHandler handlerClicked, EventHandler handlerChanged)
		{
			ColorSample sample = new ColorSample(this);

			sample.PossibleSource = true;
			sample.Clicked += handlerClicked;
			sample.Changed += handlerChanged;
			sample.TabIndex = this.tabIndex++;
			sample.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(sample, tooltip);

			return sample;
		}

		// Donne la couleur d'un �chantillon.
		protected void SetColorSample(ColorSample sample, string color, bool isDefined, bool disabledIfUndefined)
		{
			RichColor rc = (color == null) ? RichColor.Empty : RichColor.Parse(color);
			sample.Color = rc;

			if ( disabledIfUndefined )
			{
				sample.Enable = isDefined;
			}
		}

		// Donne la couleur d'un �chantillon.
		protected string GetColorSample(ColorSample sample)
		{
			if ( sample.Color.IsEmpty )
			{
				return null;
			}
			else
			{
				if ( sample.Color.A == 0.0 )  return null;
				return RichColor.ToString(sample.Color);
			}
		}

		
		// Met un texte dans un widget quelconque.
		public static void SetText(Widget widget, string text)
		{
			if ( widget.Text != text )
			{
				widget.Text = text;
			}
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

		
		// ATTENTION: Ceci n'est pas propre, mais je ne sais pas comment faire mieux.
		// Le constructeur de Common.Widget appelle DefaultHeight, qui doit
		// conna�tre le document pour d�terminer la hauteur (avec LabelHeight).
		// Comme ce constructeur est appel� avant l'initialisation de this.document,
		// je n'ai pas trouv� d'autre moyen pour conna�tre le document que de le
		// mettre au pr�alable dans une variable statique !!!
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
