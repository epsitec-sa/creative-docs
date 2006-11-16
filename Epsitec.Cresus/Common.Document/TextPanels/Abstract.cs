using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Abstract est la classe de base pour tous les panneaux des textes.
	/// </summary>
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract(Document document, bool isStyle, StyleCategory styleCategory)
		{
			this.document = document;
			this.isStyle = isStyle;
			this.styleCategory = styleCategory;

			this.PreferredHeight = this.DefaultHeight;
			
			this.label = new StaticText (this);
			this.fixIcon = new StaticText(this);

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.AutoFocus = false;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigation = TabNavigationMode.ActivateOnTab;
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


		public static Abstract Create(string name, Document document, bool isStyle)
		{
			return Abstract.Create(name, document, isStyle, StyleCategory.None);
		}
		
		public static Abstract Create(string name, Document document, bool isStyle, StyleCategory styleCategory)
		{
			//	Cr�e un nouveau panneau.
			if ( name == "Justif"    )  return new Justif(document, isStyle, styleCategory);
			if ( name == "Leading"   )  return new Leading(document, isStyle, styleCategory);
			if ( name == "Margins"   )  return new Margins(document, isStyle, styleCategory);
			if ( name == "Spaces"    )  return new Spaces(document, isStyle, styleCategory);
			if ( name == "Keep"      )  return new Keep(document, isStyle, styleCategory);
			if ( name == "Tabs"      )  return new Tabs(document, isStyle, styleCategory);
			if ( name == "Generator" )  return new Generator(document, isStyle, styleCategory);
			if ( name == "Numerator" )  return new Numerator(document, isStyle, styleCategory);
			if ( name == "Font"      )  return new Font(document, isStyle, styleCategory);
			if ( name == "Xline"     )  return new Xline(document, isStyle, styleCategory);
			if ( name == "Xscript"   )  return new Xscript(document, isStyle, styleCategory);
			if ( name == "Box"       )  return new Box(document, isStyle, styleCategory);
			if ( name == "Language"  )  return new Language(document, isStyle, styleCategory);
			return null;
		}
		
		public static bool IsFilterShow(string panel, string filter)
		{
			//	Indique si ce panneau est visible pour un filtre donn�.
			if ( panel == "Box" )  return false;  // provisoire...
			if ( filter == "All" )  return true;

			bool F = filter == "Frequently";
			bool U = filter == "Usual";
			bool P = filter == "Paragraph";
			bool C = filter == "Character";
			bool o = false;

			switch ( panel )
			{
				//                          F    U    P    C
				case "Justif":     return ( F || U || P || o );
				case "Leading":    return ( F || o || P || o );
				case "Margins":    return ( o || o || P || o );
				case "Spaces":     return ( o || o || P || o );
				case "Keep":       return ( o || o || P || o );
				case "Tabs":       return ( o || o || P || o );
				case "Generator":  return ( o || o || o || o );
				case "Numerator":  return ( o || o || P || o );
				case "Font":       return ( F || U || o || C );
				case "Xline":      return ( F || o || o || C );
				case "Xscript":    return ( F || o || o || C );
				case "Box":        return ( o || o || o || C );
				case "Language":   return ( o || o || o || C );
			}
			return true;
		}

		
		public virtual double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight+30;
			}
		}

		public virtual double TopMargin
		{
			//	Retourne la marge sup�rieure.
			get
			{
				return 0;
			}
		}

		protected double LabelHeight
		{
			//	Retourne la hauteur pour le label sup�rieur.
			get
			{
				return this.IsLabelProperties ? 14 : 0;
			}
		}

		public bool IsLabelProperties
		{
			//	Indique le mode des propri�t�s.
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
			//	Indique si ce panneau poss�de 2 hauteurs diff�rentes.
			return this.isNormalAndExtended;
		}

		public bool IsExtendedSize
		{
			//	Indique si le panneau est r�duit (petite hauteur) ou �tendu (grande hauteur).
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

		public void HeightChanged()
		{
			//	Indique que la hauteur du panneau a chang�.
			this.PreferredHeight = this.DefaultHeight;
		}

		protected void ForceHeightChanged()
		{
			//	Force la mise � jour de la hauteur du panneau.
			//	Il faut modifier la hauteur du parent (normalement Containers.Styles.panelContainer)
			//	qui contient ce panneau en mode DockStyle.Fill !
			this.Parent.PreferredHeight = this.DefaultHeight;
		}


		protected virtual void UpdateAfterChanging()
		{
			//	Met � jour apr�s un changement du wrapper.
			this.UpdateButtons();
		}


		public virtual bool DefaultFocus()
		{
			//	Met le focus par d�faut dans ce panneau.
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
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.extendedButton == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth+5;
			rect.Right -= this.extendedZoneWidth+5;
			rect.Top -= 1;
			rect.Bottom = rect.Top-this.LabelHeight;
			this.label.SetManualBounds(rect);
			this.label.Visibility = this.IsLabelProperties;

			rect = this.Client.Bounds;
			rect.Left += 1;
			rect.Width = this.extendedZoneWidth;
			rect.Top -= this.IsLabelProperties ? 3 : 9;
			rect.Bottom = rect.Top-13;
			this.fixIcon.SetManualBounds(rect);

			rect.Left = this.Client.Bounds.Right-this.extendedZoneWidth+1;
			rect.Width = this.extendedZoneWidth-3;
			this.extendedButton.SetManualBounds(rect);
		}

		protected void UpdateButtons()
		{
			//	Met � jour les boutons.
			this.extendedButton.Visibility = (this.isNormalAndExtended && !this.isStyle);
			this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
		}


		public virtual void OriginColorDeselect()
		{
			//	D�s�lectionne toutes les origines de couleurs possibles.
		}

		public virtual void OriginColorSelect(int rank)
		{
			//	S�lectionne l'origine de couleur.
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

		public void ActionMade()
		{
			//	Indique qu'une action a �t� effectu�e.
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.IsDirtySerialize = true;
		}


		
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton pour �tendre/r�duire le panneau a �t� cliqu�.
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
			color = Drawing.Color.FromRgb(color.R*this.backgroundIntensity, color.G*this.backgroundIntensity, color.B*this.backgroundIntensity);
#else
			Drawing.Color cap = adorner.ColorCaption;
			Drawing.Color color = Drawing.Color.FromAlphaRgb(1.0-this.backgroundIntensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);
#endif
			graphics.RenderSolid(color);

			if ( this.isStyle )
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

			rect.Deflate(0.5, 0.5);
			graphics.AddLine(rect.Left, rect.Bottom-0.5, rect.Left, rect.Top-0.5);
			graphics.AddLine(rect.Left+this.extendedZoneWidth, rect.Bottom-0.5, rect.Left+this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right-this.extendedZoneWidth, rect.Bottom-0.5, rect.Right-this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left-0.5, rect.Top, rect.Right+0.5, rect.Top);
			graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected IconButton CreateIconButton(string command)
		{
			//	Cr�e un bouton pour une commande.
			return this.CreateIconButton(command, "Normal");
		}

		protected IconButton CreateIconButton(string command, string iconSize)
		{
			//	Cr�e un bouton pour une commande, en pr�cisant la taille pr�f�r�e pour l'ic�ne.
			Command c = Common.Widgets.Command.Get (command);
			IconButton button = new IconButton(this);

			button.CommandObject = c;
			button.PreferredIconSize = Misc.IconPreferredSize(iconSize);
			button.AutoFocus = false;

			if ( c.Statefull )
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
			}

			button.TabIndex = this.tabIndex++;
			button.TabNavigation = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(c));
			return button;
		}

		
		protected IconButton CreateIconButton(string icon, string tooltip, MessageEventHandler handler)
		{
			//	Cr�e un bouton.
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
			button.TabNavigation = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, tooltip);
			return button;
		}

		protected void ActiveIconButton(IconButton button, bool active, bool defined)
		{
			//	Modifie l'�tat d'un bouton � 3 �tats.
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

		protected IconButton CreateClearButton(MessageEventHandler handler)
		{
			//	Cr�e un bouton "x" pour effacer une propri�t�.
			IconButton button = new IconButton(this);

			button.AutoFocus = false;
			button.Clicked += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = TabNavigationMode.ActivateOnTab;
			button.IconName = Misc.Icon("Nothing");
			ToolTip.Default.SetToolTip(button, Res.Strings.TextPanel.Clear);
			
			return button;
		}

		protected GlyphButton CreateComboButton(string command, string tooltip, MessageEventHandler handler)
		{
			//	Cr�e un bouton "v" pour un menu.
			GlyphButton button = new GlyphButton(this);

			button.CommandObject = Epsitec.Common.Widgets.Command.Get (command);
			button.ButtonStyle = ButtonStyle.Combo;
			button.GlyphShape = GlyphShape.Menu;
			button.AutoFocus = false;
			button.Clicked += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, tooltip);

			return button;
		}

		protected Widgets.TextFieldLabel CreateTextFieldLabel(string tooltip, string shortText, string longText, double minRange, double maxRange, double defRange, double step, Widgets.TextFieldLabel.Type type, EventHandler handler)
		{
			//	Cr�e un TextFieldLabel.
			Widgets.TextFieldLabel field = new Widgets.TextFieldLabel(this, type);

			field.LabelShortText = shortText;
			field.LabelLongText  = longText;
			
			if ( type == Widgets.TextFieldLabel.Type.TextField )
			{
				field.TextField.EditionAccepted += handler;
			}
			
			if ( type == Widgets.TextFieldLabel.Type.TextFieldReal )
			{
				field.SetRangeDimension(this.document, minRange, maxRange, defRange, step);
				field.TextFieldReal.EditionAccepted += handler;
			}

			if ( type == Widgets.TextFieldLabel.Type.TextFieldUnit )
			{
				field.TextFieldReal1.EditionAccepted += handler;
				field.TextFieldReal2.EditionAccepted += handler;
			}

			field.TabIndex = this.tabIndex++;
			field.TabNavigation = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);

			return field;
		}

		protected Widgets.TextFieldLabel CreateTextFieldLabelPercent(string tooltip, string shortText, string longText, double minRange, double maxRange, double defRange, double step, EventHandler handler)
		{
			//	Cr�e un TextFieldLabel en %.
			Widgets.TextFieldLabel field = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);

			field.LabelShortText = shortText;
			field.LabelLongText  = longText;
			field.SetRangePercents(this.document, minRange, maxRange, defRange, step);
			
			field.TextFieldReal.EditionAccepted += handler;
			
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			
			return field;
		}

		protected void SetTextFieldRealValue(TextFieldReal field, double value, Common.Text.Properties.SizeUnits units, bool isDefined, bool disabledIfUndefined)
		{
			//	Modifie la valeur d'un TextFieldReal.
			if ( double.IsNaN(value) )
			{
				field.ClearText();
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

		protected void GetTextFieldRealValue(TextFieldReal field, out double value, out Common.Text.Properties.SizeUnits units, out bool isDefined)
		{
			//	Donne la valeur d'un TextFieldReal.
			if ( field.IsTextEmpty )
			{
				value = double.NaN;
			}
			else
			{
				value = (double) field.InternalValue;
			}

			if ( field.UnitType == RealUnitType.Percent )
			{
				units = Common.Text.Properties.SizeUnits.Percent;
			}
			else
			{
				units = Common.Text.Properties.SizeUnits.Points;
			}

			isDefined = (field.Text != "" && !field.IsTextEmpty);
		}

		protected void SetTextFieldRealPercent(TextFieldReal field, double value, bool isDefined, bool disabledIfUndefined)
		{
			//	Modifie la valeur d'un TextFieldReal en %.
			if ( double.IsNaN(value) )
			{
				field.ClearText();
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

		protected void GetTextFieldRealPercent(TextFieldReal field, out double value, out bool isDefined)
		{
			//	Donne la valeur d'un TextFieldReal en %.
			if ( field.IsTextEmpty )
			{
				value = double.NaN;
			}
			else
			{
				value = (double) field.InternalValue;
			}

			isDefined = (field.Text != "" && !field.IsTextEmpty);
		}

		protected void ProposalTextFieldLabel(Widgets.TextFieldLabel field, bool proposal)
		{
			//	Modifie le mode d'un TextFieldLabel.
			field.TextFieldReal.TextDisplayMode = proposal ? TextDisplayMode.Proposal : TextDisplayMode.Defined;
		}

		protected void ProposalTextFieldCombo(TextFieldCombo field, bool proposal)
		{
			//	Modifie le mode d'un TextFieldCombo.
			field.TextDisplayMode = proposal ? TextDisplayMode.Proposal : TextDisplayMode.Defined;
		}

		protected void ProposalFontFaceCombo(Widgets.FontFaceCombo field, bool proposal)
		{
			//	Modifie le mode d'un FontFaceCombo.
			field.TextDisplayMode = proposal ? TextDisplayMode.Proposal : TextDisplayMode.Defined;
		}


		protected ColorSample CreateColorSample(string tooltip, MessageEventHandler handlerClicked, EventHandler handlerChanged)
		{
			//	Cr�e un �chantilon de couleur.
			ColorSample sample = new ColorSample(this);

			sample.PossibleSource = true;
			sample.Clicked += handlerClicked;
			sample.Changed += handlerChanged;
			sample.TabIndex = this.tabIndex++;
			sample.TabNavigation = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(sample, tooltip);

			return sample;
		}

		protected void SetColorSample(ColorSample sample, string color, bool isDefined, bool disabledIfUndefined)
		{
			//	Donne la couleur d'un �chantillon.
			RichColor rc = (color == null) ? RichColor.Empty : RichColor.Parse(color);
			sample.Color = rc;

			if ( disabledIfUndefined )
			{
				sample.Enable = isDefined;
			}
		}

		protected string GetColorSample(ColorSample sample)
		{
			//	Donne la couleur d'un �chantillon.
			if ( sample.Color.IsEmpty || sample.Color.A == 0.0 )
			{
				return null;
			}
			else
			{
				return RichColor.ToString(sample.Color);
			}
		}

		
		public static void SetText(Widget widget, string text)
		{
			//	Met un texte dans un widget quelconque.
			if ( widget.Text != text )
			{
				widget.Text = text;
			}
		}


		public virtual void UpdateAfterAttach()
		{
			//	Mise � jour apr�s avoir attach� le wrappers.
		}

		protected Text.Wrappers.TextWrapper TextWrapper
		{
			//	Donne le wrapper pour un texte.
			get
			{
				if ( this.isStyle )  return this.document.Wrappers.StyleTextWrapper;
				return this.document.Wrappers.TextWrapper;
			}
		}

		protected Text.Wrappers.ParagraphWrapper ParagraphWrapper
		{
			//	Donne le wrapper pour un paragraphe.
			get
			{
				if ( this.isStyle )  return this.document.Wrappers.StyleParagraphWrapper;
				return this.document.Wrappers.ParagraphWrapper;
			}
		}

		protected bool IsWrappersAttached
		{
			//	Indique si les wrappers sont attach�s.
			get
			{
				if ( this.isStyle )  return this.document.Wrappers.StyleParagraphWrapper.IsAttached;
				return this.document.Wrappers.ParagraphWrapper.IsAttached;
			}
		}


		protected virtual void OnOriginColorChanged()
		{
			//	G�n�re un �v�nement pour dire que la couleur d'origine a chang�.
			if ( this.OriginColorChanged != null )  // qq'un �coute ?
			{
				this.OriginColorChanged(this);
			}
		}

		protected virtual void OnOriginColorClosed()
		{
			//	G�n�re un �v�nement pour dire que la couleur d'origine doit �tre ferm�e.
			if ( this.OriginColorClosed != null )  // qq'un �coute ?
			{
				this.OriginColorClosed(this);
			}
		}

		public event EventHandler OriginColorChanged;
		public event EventHandler OriginColorClosed;

		
		//	ATTENTION: Ceci n'est pas propre, mais je ne sais pas comment faire mieux.
		//	Le constructeur de Common.Widget appelle DefaultHeight, qui doit
		//	conna�tre le document pour d�terminer la hauteur (avec LabelHeight).
		//	Comme ce constructeur est appel� avant l'initialisation de this.document,
		//	je n'ai pas trouv� d'autre moyen pour conna�tre le document que de le
		//	mettre au pr�alable dans une variable statique !!!
		public static Document				StaticDocument;

		protected Document					document;
		protected bool						isStyle;
		protected double					backgroundIntensity = 1.0;
		protected Text.TextStyle			textStyle;
		protected bool						isExtendedSize;
		protected bool						isNormalAndExtended;
		protected double					extendedZoneWidth = 16;
		protected StaticText				label;
		protected StaticText				fixIcon;
		protected GlyphButton				extendedButton;
		protected bool						ignoreChanged;
		protected int						tabIndex;
		protected StyleCategory				styleCategory;
	}
}
