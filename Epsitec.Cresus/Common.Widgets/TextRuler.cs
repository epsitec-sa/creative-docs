namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class TextRuler représente une règle pour un pavé de texte.
	/// </summary>
	public class TextRuler : Widget
	{
		public TextRuler()
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	manière, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.AutoFocus = false;
			this.MouseCursor = MouseCursor.AsArrow;

			this.buttonTab = new GlyphButton(this);
			this.buttonTab.AutoFocus = false;
			this.buttonTab.Clicked += new MessageEventHandler(this.HandleButtonTabClicked);
			ToolTip.Default.SetToolTip(this.buttonTab, "Choix du type de tabulateur");

			this.fieldFontName = new TextFieldCombo(this);
			this.fieldFontName.AutoFocus = false;
			this.fieldFontName.IsReadOnly = true;
			this.UpdateFieldFontName();
			this.fieldFontName.SelectedIndexChanged += new Support.EventHandler(this.HandleFieldFontNameChanged);
			ToolTip.Default.SetToolTip(this.fieldFontName, "Police de caractères");

			this.fieldFontScale = new TextFieldUpDown(this);
			this.fieldFontScale.AutoFocus = false;
			this.fieldFontScale.MinValue =   10.0M;
			this.fieldFontScale.MaxValue = 1000.0M;
			this.fieldFontScale.Step = 10.0M;
			this.fieldFontScale.Resolution = 1.0M;
			this.fieldFontScale.ValueChanged += new Support.EventHandler(this.HandleFieldFontScaleChanged);
			ToolTip.Default.SetToolTip(this.fieldFontScale, "Taille en pourcents");

			this.buttonBold = new IconButton(this);
			this.buttonBold.AutoFocus = false;
			this.buttonBold.Text = "<b>G</b>";
			this.buttonBold.Clicked += new MessageEventHandler(this.HandleButtonBoldClicked);
			ToolTip.Default.SetToolTip(this.buttonBold, "Gras");

			this.buttonItalic = new IconButton(this);
			this.buttonItalic.AutoFocus = false;
			this.buttonItalic.Text = "<i>I</i>";
			this.buttonItalic.Clicked += new MessageEventHandler(this.HandleButtonItalicClicked);
			ToolTip.Default.SetToolTip(this.buttonItalic, "Italique");

			this.buttonUnderlined = new IconButton(this);
			this.buttonUnderlined.AutoFocus = false;
			this.buttonUnderlined.Text = "<u>S</u>";
			this.buttonUnderlined.Clicked += new MessageEventHandler(this.HandleButtonUnderlinedClicked);
			ToolTip.Default.SetToolTip(this.buttonUnderlined, "Souligné");

			this.fieldFontColor = new TextFieldCombo(this);
			this.fieldFontColor.AutoFocus = false;
			this.fieldFontColor.IsReadOnly = true;
			this.fieldFontColor.Items.Add("Par défaut...");
			this.fieldFontColor.Items.Add("Noir");
			this.fieldFontColor.Items.Add("Rouge");
			this.fieldFontColor.Items.Add("Bleu");
			this.fieldFontColor.Items.Add("Magenta");
			this.fieldFontColor.Items.Add("Vert");
			this.fieldFontColor.Items.Add("Cyan");
			this.fieldFontColor.Items.Add("Jaune");
			this.fieldFontColor.Items.Add("Blanc");
			this.fieldFontColor.SelectedIndexChanged += new Support.EventHandler(this.HandleFieldFontColorChanged);
			ToolTip.Default.SetToolTip(this.fieldFontColor, "Couleur");

			this.buttonListNum = new IconButton(this);
			this.buttonListNum.AutoFocus = false;
			this.buttonListNum.IconName = @"manifest:Epsitec.Common.Widgets.Images.RulerListNum.icon";
			this.buttonListNum.Clicked += new MessageEventHandler(this.HandleButtonListNumClicked);
			ToolTip.Default.SetToolTip(this.buttonListNum, "Numérotation");

			this.buttonListFix = new IconButton(this);
			this.buttonListFix.AutoFocus = false;
			this.buttonListFix.IconName = @"manifest:Epsitec.Common.Widgets.Images.RulerListFix.icon";
			this.buttonListFix.Clicked += new MessageEventHandler(this.HandleButtonListFixClicked);
			ToolTip.Default.SetToolTip(this.buttonListFix, "Puces");

			this.manualToolTip = new ToolTip();
			this.manualToolTip.Behaviour = ToolTipBehaviour.Manual;
		}
		
		public TextRuler(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.DetachFromText();
				
				if ( this.buttonTab != null )
				{
					this.buttonTab.Clicked -= new MessageEventHandler(this.HandleButtonTabClicked);
				}
				if ( this.fieldFontName != null )
				{
					this.fieldFontName.SelectedIndexChanged -= new Support.EventHandler(this.HandleFieldFontNameChanged);
				}
				if ( this.fieldFontScale != null )
				{
					this.fieldFontScale.ValueChanged -= new Support.EventHandler(this.HandleFieldFontScaleChanged);
				}
				if ( this.buttonBold != null )
				{
					this.buttonBold.Clicked -= new MessageEventHandler(this.HandleButtonBoldClicked);
				}
				if ( this.buttonItalic != null )
				{
					this.buttonItalic.Clicked -= new MessageEventHandler(this.HandleButtonItalicClicked);
				}
				if ( this.buttonUnderlined != null )
				{
					this.buttonUnderlined.Clicked -= new MessageEventHandler(this.HandleButtonUnderlinedClicked);
				}
				if ( this.fieldFontColor != null )
				{
					this.fieldFontColor.SelectedIndexChanged -= new Support.EventHandler(this.HandleFieldFontColorChanged);
				}
				if ( this.buttonListNum != null )
				{
					this.buttonListNum.Clicked -= new MessageEventHandler(this.HandleButtonListNumClicked);
				}
				if ( this.buttonListFix != null )
				{
					this.buttonListFix.Clicked -= new MessageEventHandler(this.HandleButtonListFixClicked);
				}
			}
			
			base.Dispose(disposing);
		}

		
		public double							MinimalWidth
		{
			// Retourne la largeur minimale d'une règle.
			get
			{
				return TextRuler.buttonMargin*2 +
					   TextRuler.buttonFontNameMinWidth +
					   this.FixWidth;
			}
		}

		protected double						FixWidth
		{
			// Retourne la largeur minimale pour les widgets fixes.
			get
			{
				if ( this.listCapability )
				{
					return TextRuler.buttonFontScaleWidth +
						   TextRuler.buttonWidth*6 +
						   TextRuler.buttonFontColorWidth + 15;
				}
				else
				{
					return TextRuler.buttonFontScaleWidth +
						   TextRuler.buttonWidth*4 +
						   TextRuler.buttonFontColorWidth + 12;
				}
			}
		}

		public override double					DefaultWidth
		{
			// Retourne la largeur standard d'une règle.
			get
			{
				return 200;
			}
		}

		public override double					DefaultHeight
		{
			// Retourne la hauteur standard d'une règle.
			get
			{
				return TextRuler.zoneSupHeight + (this.tabCapability ? TextRuler.zoneInfHeight : 0.0);
			}
		}

		public bool								ListCapability
		{
			// Indique si la règle permet l'utilisation des puces.
			get
			{
				return this.listCapability;
			}

			set
			{
				if ( this.listCapability != value )
				{
					this.listCapability = value;
					this.UpdateButtons();  // màj. les widgets
					this.UpdateGeometry();
					this.Invalidate();
				}
			}
		}

		public bool								TabCapability
		{
			// Indique si la règle permet l'édition des tabulateurs.
			get
			{
				return this.tabCapability;
			}

			set
			{
				if ( this.tabCapability != value )
				{
					this.tabCapability = value;
					this.UpdateGeometry();
					this.Invalidate();
				}
			}
		}

		public bool								AllFonts
		{
			// Indique si la règle permet l'accès à toutes les fontes.
			get
			{
				return this.allFonts;
			}

			set
			{
				if ( this.allFonts != value )
				{
					this.allFonts = value;
					this.UpdateFieldFontName();
				}
			}
		}

		public double							LeftMargin
		{
			// Marge de gauche pour la graduation.
			get
			{
				return this.leftMargin;
			}

			set
			{
				if ( this.leftMargin != value )
				{
					this.leftMargin = value;
					this.Invalidate();
				}
			}
		}

		public double							RightMargin
		{
			// Marge de droite pour la graduation.
			get
			{
				return this.rightMargin;
			}

			set
			{
				if ( this.rightMargin != value )
				{
					this.rightMargin = value;
					this.Invalidate();
				}
			}
		}

		public double							PPM
		{
			// Nombre de points/millimètres pour la graduation.
			get
			{
				return this.ppm;
			}

			set
			{
				if ( this.ppm != value )
				{
					this.ppm = value;
					this.Invalidate();
				}
			}
		}

		public double							Scale
		{
			// Echelle de la graduation.
			get
			{
				return this.scale;
			}

			set
			{
				if ( this.scale != value )
				{
					this.scale = value;
					this.Invalidate();
				}
			}
		}

		public string							FontName
		{
			// Nom de la fonte.
			get
			{
				return this.fontName;
			}

			set
			{
				if ( this.fontName != value )
				{
					this.fontName = value;
					this.UpdateButtons();  // màj. les widgets

					if ( this.textNavigator != null )
					{
						this.textNavigator.SelectionFontName = this.fontName;
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public double							FontScale
		{
			// Echelle de la fonte.
			get
			{
				return this.fontScale;
			}

			set
			{
				if ( this.fontScale != value )
				{
					this.fontScale = value;
					this.UpdateButtons();  // màj. les widgets

					if ( this.textNavigator != null )
					{
						this.textNavigator.SelectionFontScale = this.fontScale;
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public Drawing.Color					FontColor
		{
			// Couleur de la fonte.
			get
			{
				return this.fontColor;
			}

			set
			{
				if ( this.fontColor != value )
				{
					this.fontColor = value;
					this.UpdateButtons();  // màj. les widgets

					if ( this.textNavigator != null )
					{
						this.textNavigator.SelectionFontColor = this.fontColor;
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public bool								Bold
		{
			// Attribut typographique "gras".
			get
			{
				return this.bold;
			}

			set
			{
				if ( this.bold != value )
				{
					this.bold = value;
					this.UpdateButtons();  // màj. les widgets

					if ( this.textNavigator != null )
					{
						this.textNavigator.SelectionBold = this.bold;
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public bool								Italic
		{
			// Attribut typographique "italique".
			get
			{
				return this.italic;
			}

			set
			{
				if ( this.italic != value )
				{
					this.italic = value;
					this.UpdateButtons();  // màj. les widgets

					if ( this.textNavigator != null )
					{
						this.textNavigator.SelectionItalic = this.italic;
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public bool								Underlined
		{
			// Attribut typographique "souligné".
			get
			{
				return this.underlined;
			}

			set
			{
				if ( this.underlined != value )
				{
					this.underlined = value;
					this.UpdateButtons();  // màj. les widgets

					if ( this.textNavigator != null )
					{
						this.textNavigator.SelectionUnderlined = this.underlined;
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public Drawing.TextListType				List
		{
			// Attribut typographique "puces".
			get
			{
				return this.list;
			}

			set
			{
				if ( this.list != value )
				{
					this.list = value;
					this.UpdateButtons();  // màj. les widgets

					if ( this.textNavigator != null )
					{
						this.textNavigator.SelectionList = this.list;
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}


		public void AttachToTextField(AbstractTextField text)
		{
			// Lie la règle à un objet TextField quelconque.
			this.textField = text;

			Drawing.Rectangle cb = text.Client.Bounds;
			Drawing.Rectangle tb = text.InnerTextBounds;
			this.leftMargin  = tb.Left-cb.Left;
			this.rightMargin = cb.Right-tb.Right;

			this.AttachToText(text.TextNavigator);
		}

		public void AttachToText(TextNavigator textNavigator)
		{
			// Lie la règle à un texte éditable.
			if ( this.textNavigator == textNavigator )  return;

			if ( this.textNavigator != null )
			{
				this.textNavigator.CursorChanged -= new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextInserted -= new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextDeleted -= new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.StyleChanged -= new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
			}

			this.textNavigator = textNavigator;

			if ( this.textNavigator != null )
			{
				this.textNavigator.CursorChanged += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextInserted += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextDeleted += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.StyleChanged += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.HandleCursorChanged(null);
				this.Invalidate();
			}
		}
		
		public void DetachFromText()
		{
			// Délie la règle du texte.
			this.textField = null;
			this.AttachToText(null);
		}
		
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					if ( this.TabMouseDown(pos) )
					{
						message.Consumer = this;
					}
					break;

				case MessageType.MouseMove:
					this.TabMouseMove(pos);
					this.MouseCursor = MouseCursor.AsArrow;
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					if ( this.TabMouseUp(pos) )
					{
						message.Consumer = this;
					}
					break;
			}
		}

		protected bool TabMouseDown(Drawing.Point pos)
		{
			if ( !this.tabCapability )  return false;
			if ( !this.DetectTabZone(pos) )  return false;

			this.textNavigator.TabUndoMemorise();

			this.mouseTabRank = this.DetectTabRank(pos);
			if ( this.mouseTabRank == -1 )  // crée un nouveau tabulateur ?
			{
				Drawing.TextStyle.Tab tab = new Drawing.TextStyle.Tab();
				tab.Pos  = this.ScreenToTab(pos.X);
				tab.Type = this.GlyphType(this.buttonTab);
				tab.Line = Drawing.TextTabLine.None;
				this.mouseTabRank = this.textNavigator.TabInsert(tab);
				this.mouseMove = false;
			}
			else	// déplace un tabulateur existant ?
			{
				this.mouseMove = true;
			}

			this.VerticalMarkShow(pos);
			//-this.ShowManualToolTip(pos);
			//-this.UpdateManualToolTip(pos);

			this.mouseDown = true;
			this.TabChanged();
			return true;
		}

		protected bool TabMouseMove(Drawing.Point pos)
		{
			if ( !this.tabCapability )  return false;
			if ( !this.mouseDown )  return false;

			if ( this.DetectTabZone(pos) )
			{
				this.textNavigator.SetTabPosition(this.mouseTabRank, this.ScreenToTab(pos.X));
			}
			else
			{
				this.textNavigator.SetTabPosition(this.mouseTabRank, -1.0);  // pseudo-destruction
			}

			this.VerticalMarkShow(pos);
			//-this.UpdateManualToolTip(pos);

			this.TabChanged();
			return true;
		}

		protected bool TabMouseUp(Drawing.Point pos)
		{
			if ( !this.tabCapability )  return false;
			if ( !this.mouseDown )  return false;

			Drawing.TextStyle.Tab tab = this.textNavigator.GetTab(this.mouseTabRank);
			if ( tab.Pos < 0.0 )  // supprime définitivement le tabulateur ?
			{
				this.textNavigator.TabRemoveAt(this.mouseTabRank);
				this.TabChanged();
			}

			this.VerticalMarkHide();
			//-this.HideManualToolTip(pos);

			this.mouseDown = false;
			return true;
		}


		protected void ShowManualToolTip(Drawing.Point pos)
		{
			this.manualToolTip.SetToolTip(this, this.TextManualToolTip(pos));
			this.manualToolTip.ShowToolTipForWidget(this);
			this.manualToolTip.ManualUpdate(this.MapClientToScreen(pos), this.TextManualToolTip(pos));
		}

		protected void UpdateManualToolTip(Drawing.Point pos)
		{
			this.manualToolTip.ManualUpdate(this.MapClientToScreen(pos), this.TextManualToolTip(pos));
		}

		protected void HideManualToolTip(Drawing.Point pos)
		{
			this.manualToolTip.HideToolTipForWidget(this);
		}

		protected string TextManualToolTip(Drawing.Point pos)
		{
			// Retourne le texte du tooltip en fonction de la position de la souris.
			string text = "";
			if ( this.DetectTabZone(pos) )
			{
				double value = this.ScreenToTab(pos.X);
				text = string.Format("{0} {1}", (value/this.ppm).ToString("F2"), "mm");
			}
			else
			{
				text = "Supprimé";
			}
			return text;
		}


		protected void VerticalMarkShow(Drawing.Point pos)
		{
			if ( this.textField == null )  return;

			if ( this.DetectTabZone(pos) )
			{
				this.VerticalMarkPos(pos.X-this.Client.Bounds.Left-this.leftMargin);
			}
			else
			{
				this.VerticalMarkHide();
			}
		}

		protected void VerticalMarkHide()
		{
			if ( this.textField == null )  return;
			this.VerticalMarkPos(double.NaN);
		}

		protected void VerticalMarkPos(double pos)
		{
			if ( this.textField == null )  return;
			if ( this.textField.TextLayout.VerticalMark == pos )  return;
			this.textField.TextLayout.VerticalMark = pos;
			this.textField.Invalidate();
		}


		protected void TabChanged()
		{
			// Informe d'un changement de tabulateur.
			this.Invalidate();
			if ( this.textField != null )
			{
				this.textField.Invalidate();
			}
			this.OnChanged();
		}

		protected int DetectTabRank(Drawing.Point pos)
		{
			// Détecte le rang du tabulateur visé par la souris.
			if ( !this.tabCapability )  return -1;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Height = TextRuler.zoneInfHeight;

			int total = this.textNavigator.TabCount;
			for ( int i=0 ; i<total ; i++ )
			{
				Drawing.TextStyle.Tab tab = this.textNavigator.GetTab(i);
				double tabPos = this.TabToScreen(tab.Pos);
				rect.Left  = tabPos-rect.Height/2;
				rect.Right = tabPos+rect.Height/2;

				if ( rect.Contains(pos) )  return i;
			}
			return -1;
		}

		protected bool DetectTabZone(Drawing.Point pos)
		{
			// Détecte si la souris est dans la zone des tabulateurs.
			if ( !this.tabCapability )  return false;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Left  += this.leftMargin;
			rect.Right -= this.rightMargin;
			rect.Height = TextRuler.zoneInfHeight;
			rect.Deflate(1);
			return rect.Contains(pos);
		}

		protected double ScreenToTab(double pos)
		{
			// Retourne la position du tabulateur en fonction de la position dans l'interface.
			return (pos-this.Client.Bounds.Left-this.leftMargin)/this.scale;
		}

		protected double TabToScreen(double pos)
		{
			// Retourne la position en X dans l'interface en fonction de la position du tabulateur.
			return (pos*this.scale)+this.Client.Bounds.Left+this.leftMargin;
		}


		private void HandleButtonTabClicked(object sender, MessageEventArgs e)
		{
			// Bouton pour choisir le type de tabulateur cliqué.
			if ( this.silent )  return;

			switch ( this.tabType )
			{
				case Drawing.TextTabType.Right:    this.tabType = Drawing.TextTabType.Left;     break;
				case Drawing.TextTabType.Left:     this.tabType = Drawing.TextTabType.Center;   break;
				case Drawing.TextTabType.Center:   this.tabType = Drawing.TextTabType.Decimal;  break;
				case Drawing.TextTabType.Decimal:  this.tabType = Drawing.TextTabType.Indent;   break;
				case Drawing.TextTabType.Indent:   this.tabType = Drawing.TextTabType.Right;    break;
			}
			this.GlyphType(this.buttonTab, this.tabType);
		}

		private void HandleFieldFontNameChanged(object sender)
		{
			// Combo pour le nom de la fonte changé.
			if ( this.silent )  return;
			this.FontName = this.ComboSelectedName(this.fieldFontName);
			this.OnChanged();
		}

		private void HandleFieldFontScaleChanged(object sender)
		{
			// Valeur de l'échelle de la fonte changée.
			if ( this.silent )  return;
			this.FontScale = (double) this.fieldFontScale.Value / 100;
			this.OnChanged();
		}

		private void HandleButtonBoldClicked(object sender, MessageEventArgs e)
		{
			// Bouton "bold" cliqué.
			if ( this.silent )  return;
			this.Bold = !this.Bold;
			this.OnChanged();
		}

		private void HandleButtonItalicClicked(object sender, MessageEventArgs e)
		{
			// Bouton "italique" cliqué.
			if ( this.silent )  return;
			this.Italic = !this.Italic;
			this.OnChanged();
		}

		private void HandleButtonUnderlinedClicked(object sender, MessageEventArgs e)
		{
			// Bouton "souligné" cliqué.
			if ( this.silent )  return;
			this.Underlined = !this.Underlined;
			this.OnChanged();
		}

		private void HandleFieldFontColorChanged(object sender)
		{
			// Combo pour la couleur de la fonte changé.
			if ( this.silent )  return;
			this.FontColor = this.ComboSelectedColor(this.fieldFontColor);
			this.OnChanged();
		}

		private void HandleButtonListNumClicked(object sender, MessageEventArgs e)
		{
			// Bouton pour la numnérotation cliqué.
			if ( this.silent )  return;
			if ( this.list == Drawing.TextListType.Num )  this.List = Drawing.TextListType.None;
			else                                          this.List = Drawing.TextListType.Num;
			this.OnChanged();
		}

		private void HandleButtonListFixClicked(object sender, MessageEventArgs e)
		{
			// Bouton pour les puces cliqué.
			if ( this.silent )  return;
			if ( this.list == Drawing.TextListType.Fix )  this.List = Drawing.TextListType.None;
			else                                          this.List = Drawing.TextListType.Fix;
			this.OnChanged();
		}

		private void HandleCursorChanged(object sender)
		{
			// Met à jour les widgets de la règle lorsque le texte a changé.
			this.fontName   = this.textNavigator.SelectionFontName;
			this.fontScale  = this.textNavigator.SelectionFontScale;
			this.bold       = this.textNavigator.SelectionBold;
			this.italic     = this.textNavigator.SelectionItalic;
			this.underlined = this.textNavigator.SelectionUnderlined;
			this.fontColor  = this.textNavigator.SelectionFontColor;
			this.list       = this.textNavigator.SelectionList;

			this.UpdateButtons();  // màj. les widgets
		}

		protected void UpdateButtons()
		{
			// Met à jour les widgets de la règle en fonction des données.
			this.silent = true;
			this.GlyphType(this.buttonTab, this.tabType);
			this.ComboSelectedName(this.fieldFontName, this.fontName);
			this.fieldFontScale.Value = (decimal) this.fontScale * 100;
			if ( !this.fieldFontScale.Text.EndsWith("%") )
			{
				this.fieldFontScale.Text = string.Concat(this.fieldFontScale.Text, "%");
			}
			this.ButtonActive(this.buttonBold,       this.bold      );
			this.ButtonActive(this.buttonItalic,     this.italic    );
			this.ButtonActive(this.buttonUnderlined, this.underlined);
			this.ComboSelectedColor(this.fieldFontColor, this.fontColor);
			this.ButtonActive(this.buttonListNum, this.list == Drawing.TextListType.Num);
			this.ButtonActive(this.buttonListFix, this.list == Drawing.TextListType.Fix);
			this.buttonListNum.SetVisible(this.listCapability);
			this.buttonListFix.SetVisible(this.listCapability);
			this.silent = false;
		}

		protected void ButtonActive(Button button, bool active)
		{
			// Indique si un bouton est actif.
			button.ActiveState = active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		protected void GlyphType(GlyphButton button, Drawing.TextTabType type)
		{
			// Modifie le glyph d'un bouton.
			switch ( type )
			{
				case Drawing.TextTabType.Right:
					button.GlyphShape = Widgets.GlyphShape.TabRight;
					break;

				case Drawing.TextTabType.Left:
					button.GlyphShape = Widgets.GlyphShape.TabLeft;
					break;

				case Drawing.TextTabType.Center:
					button.GlyphShape = Widgets.GlyphShape.TabCenter;
					break;

				case Drawing.TextTabType.Decimal:
					button.GlyphShape = Widgets.GlyphShape.TabDecimal;
					break;

				case Drawing.TextTabType.Indent:
					button.GlyphShape = Widgets.GlyphShape.TabIndent;
					break;
			}
		}

		protected Drawing.TextTabType GlyphType(GlyphButton button)
		{
			// Retourne le glyph d'un bouton.
			switch ( button.GlyphShape )
			{
				case Widgets.GlyphShape.TabRight:    return Drawing.TextTabType.Right;
				case Widgets.GlyphShape.TabLeft:     return Drawing.TextTabType.Left;
				case Widgets.GlyphShape.TabCenter:   return Drawing.TextTabType.Center;
				case Widgets.GlyphShape.TabDecimal:  return Drawing.TextTabType.Decimal;
				case Widgets.GlyphShape.TabIndent:   return Drawing.TextTabType.Indent;
			}
			return Drawing.TextTabType.None;
		}

		protected void ComboSelectedName(TextFieldCombo combo, string name)
		{
			// Modifie le texte d'un combo.
			if ( name == "" )
			{
				combo.SelectedIndex = 0;
			}
			else
			{
				int total = combo.Items.Count;
				for ( int i=1 ; i<total ; i++ )
				{
					string s = combo.Items[i];
					if ( s == name )
					{
						combo.SelectedIndex = i;
						return;
					}
				}
			}
		}

		protected string ComboSelectedName(TextFieldCombo combo)
		{
			// Retourne le texte d'un combo.
			if ( combo.SelectedIndex <= 0 )  return "";
			return combo.Items[combo.SelectedIndex] as string;
		}


		protected void ComboSelectedColor(TextFieldCombo combo, Drawing.Color color)
		{
			// Modifie la couleur d'un combo.
			combo.SelectedIndex = 0;
			if ( color == Drawing.Color.FromRGB(0,0,0) )  combo.SelectedIndex = 1;
			if ( color == Drawing.Color.FromRGB(1,0,0) )  combo.SelectedIndex = 2;
			if ( color == Drawing.Color.FromRGB(0,0,1) )  combo.SelectedIndex = 3;
			if ( color == Drawing.Color.FromRGB(1,0,1) )  combo.SelectedIndex = 4;
			if ( color == Drawing.Color.FromRGB(0,1,0) )  combo.SelectedIndex = 5;
			if ( color == Drawing.Color.FromRGB(0,1,1) )  combo.SelectedIndex = 6;
			if ( color == Drawing.Color.FromRGB(1,1,0) )  combo.SelectedIndex = 7;
			if ( color == Drawing.Color.FromRGB(1,1,1) )  combo.SelectedIndex = 8;
		}

		protected Drawing.Color ComboSelectedColor(TextFieldCombo combo)
		{
			// Retourne la couleur d'un combo.
			if ( combo.SelectedIndex == 1 )  return Drawing.Color.FromRGB(0,0,0);
			if ( combo.SelectedIndex == 2 )  return Drawing.Color.FromRGB(1,0,0);
			if ( combo.SelectedIndex == 3 )  return Drawing.Color.FromRGB(0,0,1);
			if ( combo.SelectedIndex == 4 )  return Drawing.Color.FromRGB(1,0,1);
			if ( combo.SelectedIndex == 5 )  return Drawing.Color.FromRGB(0,1,0);
			if ( combo.SelectedIndex == 6 )  return Drawing.Color.FromRGB(0,1,1);
			if ( combo.SelectedIndex == 7 )  return Drawing.Color.FromRGB(1,1,0);
			if ( combo.SelectedIndex == 8 )  return Drawing.Color.FromRGB(1,1,1);
			return Drawing.Color.Empty;
		}

		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateGeometry();
		}

		protected void UpdateGeometry()
		{
			// Met à jour la géométrie des widgets de la règle.
			if ( this.buttonBold == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Bottom += this.tabCapability ? TextRuler.zoneInfHeight : 0.0;
			rect.Deflate(TextRuler.buttonMargin);

			double widthName = rect.Width-this.FixWidth;
			widthName = System.Math.Max(widthName, TextRuler.buttonFontNameMinWidth);
			widthName = System.Math.Min(widthName, TextRuler.buttonFontNameMaxWidth);

			rect.Width = TextRuler.buttonWidth;
			this.buttonTab.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = widthName;
			this.fieldFontName.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonFontScaleWidth;
			this.fieldFontScale.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonWidth;
			this.buttonBold.Bounds = rect;

			rect.Offset(rect.Width, 0);
			this.buttonItalic.Bounds = rect;

			rect.Offset(rect.Width, 0);
			this.buttonUnderlined.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonFontColorWidth;
			this.fieldFontColor.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonWidth;
			this.buttonListNum.Bounds = rect;

			rect.Offset(rect.Width, 0);
			this.buttonListFix.Bounds = rect;
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine la règle et sa graduation.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(adorner.ColorWindow);  // dessine le fond

			if ( this.tabCapability )
			{
				Drawing.Rectangle part = rect;
				part.Height = TextRuler.zoneInfHeight;
				Drawing.Color color = adorner.ColorTextFieldBorder(this.IsEnabled);
				color.A = 0.2;
				if ( this.leftMargin > 0.0 )
				{
					part.Width = this.leftMargin;
					graphics.AddFilledRectangle(part);
					graphics.RenderSolid(color);  // grise la partie à gauche de la graduation
				}
				if ( this.rightMargin > 0.0 )
				{
					part.Left = rect.Right-this.rightMargin;
					part.Width = this.rightMargin;
					graphics.AddFilledRectangle(part);
					graphics.RenderSolid(color);  // grise la partie à droite de la graduation
				}

				this.PaintGrad(graphics);  // dessine la graduation
				this.PaintTab(graphics);  // dessine les tabulateurs par-dessus
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);  // dessine le cadre

			if ( this.tabCapability )
			{
				graphics.AddLine(rect.Left, rect.Bottom+TextRuler.zoneInfHeight, rect.Right, rect.Bottom+TextRuler.zoneInfHeight);
				graphics.AddLine(rect.Left+this.leftMargin, rect.Bottom, rect.Left+this.leftMargin, rect.Bottom+TextRuler.zoneInfHeight);
				graphics.AddLine(rect.Right-this.rightMargin, rect.Bottom, rect.Right-this.rightMargin, rect.Bottom+TextRuler.zoneInfHeight);
			}

			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}

		protected void PaintGrad(Drawing.Graphics graphics)
		{
			// Dessine la graduation dans la règle.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Height = TextRuler.zoneInfHeight;
			graphics.Align(ref rect);

			graphics.Color = adorner.ColorTextFieldBorder(this.IsEnabled);
			Drawing.Font font = Drawing.Font.GetFont("Tahoma", "Regular");

			double step = 3.0;  // espace (en pixels) espéré entre 2 graduations
			double mm = step/(this.scale*this.ppm);
			mm = 1.0/System.Math.Pow(10.0, System.Math.Floor(System.Math.Log10(1.0/mm)));
			step = mm*(this.scale*this.ppm);

			double posx = rect.Left+this.leftMargin;
			int rank = 0;

			graphics.SolidRenderer.Color = adorner.ColorText(this.PaintState);
			while ( posx+0.5 < rect.Right-this.rightMargin )
			{
				double h = rect.Height;
				     if ( rank%10 == 0 )  h *= 1.0;
				else if ( rank% 5 == 0 )  h *= 0.4;
				else                      h *= 0.2;
				graphics.AddLine(posx+0.5, rect.Bottom, posx+0.5, rect.Bottom+h);

				if ( rank%10 == 0 )
				{
					double value = (posx-rect.Left-this.leftMargin)/this.scale/this.ppm;
					value *= 1000000.0;
					value = System.Math.Floor(value+0.5);  // arrondi à la 6ème décimale
					value /= 1000000.0;
					string text = value.ToString();

					double size = rect.Height*0.5;
					Drawing.Rectangle bounds = font.GetTextBounds(text);
					bounds.Scale(size);

					if ( posx+4+bounds.Width < rect.Right-this.rightMargin )
					{
						graphics.PaintText(posx+2, rect.Bottom+rect.Height*0.5, text, font, size);
					}
				}

				posx += step;
				rank ++;
			}
			graphics.RenderSolid(adorner.ColorText(this.PaintState));
		}

		protected void PaintTab(Drawing.Graphics graphics)
		{
			// Dessine les tabulateurs dans la règle.
			if ( this.textNavigator == null )  return;

			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Height = TextRuler.zoneInfHeight;
			rect.Inflate(2);
			graphics.Align(ref rect);
			rect.Offset(0, 0.5);

			int total = this.textNavigator.TabCount;
			for ( int i=0 ; i<total ; i++ )
			{
				Drawing.TextStyle.Tab tab = this.textNavigator.GetTab(i);
				if ( tab.Pos < 0.0 )  continue;  // pseudo-destruction ?

				double pos = this.TabToScreen(tab.Pos);
				if ( pos < this.Client.Bounds.Left +this.leftMargin  )  continue;
				if ( pos > this.Client.Bounds.Right-this.rightMargin )  continue;

				rect.Left  = pos-rect.Height/2;
				rect.Right = pos+rect.Height/2;

				Widgets.GlyphShape glyph = Widgets.GlyphShape.TabRight;
				switch ( tab.Type )
				{
					case Drawing.TextTabType.Right:    glyph = Widgets.GlyphShape.TabRight;    break;
					case Drawing.TextTabType.Left:     glyph = Widgets.GlyphShape.TabLeft;     break;
					case Drawing.TextTabType.Center:   glyph = Widgets.GlyphShape.TabCenter;   break;
					case Drawing.TextTabType.Decimal:  glyph = Widgets.GlyphShape.TabDecimal;  break;
					case Drawing.TextTabType.Indent:   glyph = Widgets.GlyphShape.TabIndent;   break;
				}
				adorner.PaintGlyph(graphics, rect, Widgets.WidgetState.Enabled, glyph, Widgets.PaintTextStyle.Button);
			}
		}


		protected void UpdateFieldFontName()
		{
			this.fieldFontName.Items.Clear();
			this.fieldFontName.Items.Add("Par défaut...");

			if ( this.allFonts )
			{
				Drawing.Font.FaceInfo[] list = Drawing.Font.Faces;
				foreach ( Drawing.Font.FaceInfo info in list )
				{
					if ( info.IsLatin )
					{
						this.fieldFontName.Items.Add(info.Name);
					}
				}
			}
			else
			{
				this.fieldFontName.Items.Add("Tahoma");
				this.fieldFontName.Items.Add("Arial");
				this.fieldFontName.Items.Add("Courier New");
				this.fieldFontName.Items.Add("Times New Roman");
			}
		}


		// Génère un événement pour dire que la règle a changé.
		protected void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}
		
		public event Support.EventHandler	Changed;


		protected AbstractTextField			textField;
		protected TextNavigator				textNavigator;
		protected Drawing.TextTabType		tabType = Drawing.TextTabType.Right;
		protected string					fontName = "";
		protected double					fontScale = 1;
		protected Drawing.Color				fontColor = Drawing.Color.Empty;
		protected bool						bold = false;
		protected bool						italic = false;
		protected bool						underlined = false;
		protected Drawing.TextListType		list = Drawing.TextListType.None;
		protected bool						mouseDown = false;
		protected bool						mouseMove;
		protected int						mouseTabRank;
		protected double					leftMargin = 0.0;
		protected double					rightMargin = 0.0;
		protected double					ppm = 10.0;
		protected double					scale = 1.0;
		protected bool						listCapability = true;
		protected bool						tabCapability = true;
		protected bool						allFonts = true;

		protected GlyphButton				buttonTab;
		protected TextFieldCombo			fieldFontName;
		protected TextFieldUpDown			fieldFontScale;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected TextFieldCombo			fieldFontColor;
		protected IconButton				buttonListNum;
		protected IconButton				buttonListFix;
		protected ToolTip					manualToolTip;
		protected bool						silent = false;

		protected static readonly double	buttonMargin = 3;
		protected static readonly double	buttonWidth = 20;
		protected static readonly double	buttonFontNameMinWidth = 100;
		protected static readonly double	buttonFontNameMaxWidth = 150;
		protected static readonly double	buttonFontScaleWidth = 45;
		protected static readonly double	buttonFontColorWidth = 90;
		protected static readonly double	zoneSupHeight = TextRuler.buttonMargin*2+TextRuler.buttonWidth;
		protected static readonly double	zoneInfHeight = 15;
	}
}
