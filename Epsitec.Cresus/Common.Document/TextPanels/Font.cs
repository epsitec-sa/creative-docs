using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Font permet de choisir une police de caractères.
	/// </summary>
	[SuppressBundleSupport]
	public class Font : Abstract
	{
		public Font(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Font.Title;

			this.fixIcon.Text = Misc.Image("TextFont");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Font.Title);

			this.fontFace = new TextFieldCombo(this);
			this.fontFace.IsReadOnly = true;
			this.fontFace.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontFace.TabIndex = this.tabIndex++;
			this.fontFace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontFace, Res.Strings.TextPanel.Font.Tooltip.Face);

			this.fontStyle = new TextFieldCombo(this);
			this.fontStyle.IsReadOnly = true;
			this.fontStyle.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontStyle.TabIndex = this.tabIndex++;
			this.fontStyle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontStyle, Res.Strings.TextPanel.Font.Tooltip.Style);

			this.fontFeatures = this.CreateIconButton(Misc.Icon("FontFeatures"), Res.Strings.TextPanel.Font.Tooltip.Features, new MessageEventHandler(this.HandleFeaturesClicked));

			this.fontSize = new TextFieldCombo(this);
			this.fontSize.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontSize.TabIndex = this.tabIndex++;
			this.fontSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontSize, Res.Strings.TextPanel.Font.Tooltip.Size);

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleSource = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fontColor.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fontColor.TabIndex = this.tabIndex++;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, Res.Strings.Action.Text.Font.Color);

			this.buttonSizeMinus = this.CreateIconButton(Misc.Icon("FontSizeMinus"), Res.Strings.Action.Text.Font.SizeMinus, new MessageEventHandler(this.HandleButtonSizeMinusClicked), false);
			this.buttonSizePlus  = this.CreateIconButton(Misc.Icon("FontSizePlus"),  Res.Strings.Action.Text.Font.SizePlus,  new MessageEventHandler(this.HandleButtonSizePlusClicked), false);

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.checkBold = new CheckButton(this);
			this.checkBold.Text = "Inverser le gras";
			this.checkBold.TabIndex = this.tabIndex++;
			this.checkBold.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.checkBold.ActiveStateChanged += new EventHandler(this.HandleCheckBoldActiveStateChanged);

			this.checkItalic = new CheckButton(this);
			this.checkItalic.Text = "Inverser l'italique";
			this.checkItalic.TabIndex = this.tabIndex++;
			this.checkItalic.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.checkItalic.ActiveStateChanged += new EventHandler(this.HandleCheckItalicActiveStateChanged);

			this.document.TextWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.TextWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fontFace.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontStyle.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontSize.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontColor.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fontColor.Changed -= new EventHandler(this.HandleFieldColorChanged);

				this.document.TextWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.document.TextWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);

				this.fontFace = null;
				this.fontSize = null;
				this.fontColor = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 120;
					}
					else	// étendu/compact ?
					{
						h += 120;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		// Retourne la marge supérieure.
		public override double TopMargin
		{
			get
			{
				return 5;
			}
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.fontColor.ActiveState = ActiveState.No;
		}

		// Sélectionne l'origine de couleur.
		public override void OriginColorSelect(int rank)
		{
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				if ( rank == 0 )  this.originFieldColor = this.fontColor;
			}
			if ( this.originFieldColor == null )  return;

			this.OriginColorDeselect();
			this.originFieldColor.ActiveState = ActiveState.Yes;
		}

		// Retourne le rang de la couleur d'origine.
		public override int OriginColorRank()
		{
			return this.originFieldRank;
		}

		// Modifie la couleur d'origine.
		public override void OriginColorChange(Drawing.RichColor color)
		{
			if ( this.originFieldColor == null )  return;
			
			if ( this.originFieldColor.Color != color )
			{
				this.originFieldColor.Color = color;
				this.ColorToWrapper(this.originFieldColor);
			}
		}

		// Donne la couleur d'origine.
		public override Drawing.RichColor OriginColorGet()
		{
			if ( this.originFieldColor == null )  return Drawing.RichColor.FromBrightness(0.0);
			return this.originFieldColor.Color;
		}

		// Donne la couleur au wrapper.
		protected void ColorToWrapper(ColorSample sample)
		{
			if ( sample.Color.IsEmpty )
			{
				this.document.TextWrapper.Defined.ClearColor();
			}
			else
			{
				string sc = RichColor.ToString(sample.Color);
				this.document.TextWrapper.Defined.Color = sc;
			}
		}

		
		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			string face = this.document.TextWrapper.Active.FontFace;
			bool isFace = this.document.TextWrapper.Defined.IsFontFaceDefined;

			string style = this.document.TextWrapper.Active.FontStyle;
			bool isStyle = this.document.TextWrapper.Defined.IsFontStyleDefined;

			double size = this.document.TextWrapper.Active.FontSize;
			Text.Properties.SizeUnits units = this.document.TextWrapper.Active.Units;
			if ( units == Common.Text.Properties.SizeUnits.Points )
			{
				size /= Modifier.fontSizeScale;
			}
			bool isSize = this.document.TextWrapper.Defined.IsFontSizeDefined;
			string textSize = Misc.ConvertDoubleToString(size, units, 0);

			string sc = this.document.TextWrapper.Defined.Color;
			RichColor color = (sc == null) ? RichColor.Empty : RichColor.Parse(sc);

			bool bold   = this.document.TextWrapper.Defined.InvertBold;
			bool italic = this.document.TextWrapper.Defined.InvertItalic;

			this.ignoreChanged = true;

			this.UpdateComboFaceList();
			this.UpdateComboStyleList(face);
			this.UpdateComboSizeList();

			this.fontFace.Text  = face;
			this.fontStyle.Text = style;
			this.fontSize.Text  = textSize;

			this.ProposalTextFieldCombo(this.fontFace,  !isFace);
			this.ProposalTextFieldCombo(this.fontStyle, !isStyle);
			this.ProposalTextFieldCombo(this.fontSize,  !isSize);

			this.fontColor.Color = color;

			if ( this.fontColor.ActiveState == ActiveState.Yes )
			{
				this.OnOriginColorChanged();  // change la couleur dans le ColorSelector
			}

			this.checkBold.ActiveState   = bold   ? ActiveState.Yes : ActiveState.No;
			this.checkItalic.ActiveState = italic ? ActiveState.Yes : ActiveState.No;
			
			this.ignoreChanged = false;
		}

		// Met à jour la liste d'un champ éditable pour le nom de la police.
		protected void UpdateComboFaceList()
		{
			if ( this.fontFace.Items.Count == 0 )
			{
				this.fontFace.Items.Add(Res.Strings.Action.Text.Font.Default);  // par défaut
				Misc.AddFontList(this.fontFace, false);
			}
		}

		// Met à jour la liste d'un champ éditable pour le style de la police.
		protected void UpdateComboStyleList(string face)
		{
			this.fontStyle.Items.Clear();  // vide la liste
			if ( face == null )  return;

			OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(face);
			foreach ( OpenType.FontIdentity id in list )
			{
				this.fontStyle.Items.Add(id.InvariantStyleName);
			}
		}

		// Met à jour la liste d'un champ éditable pour la taille de la police.
		protected void UpdateComboSizeList()
		{
			if ( this.fontSize.Items.Count == 0 )
			{
				this.fontSize.Items.Add(Res.Strings.Action.Text.Font.Default);  // par défaut
				this.fontSize.Items.Add("\u2015\u2015\u2015\u2015");
				this.fontSize.Items.Add("50%");
				this.fontSize.Items.Add("75%");
				this.fontSize.Items.Add("150%");
				this.fontSize.Items.Add("200%");
				this.fontSize.Items.Add("\u2015\u2015\u2015\u2015");
				this.fontSize.Items.Add("8");
				this.fontSize.Items.Add("9");
				this.fontSize.Items.Add("10");
				this.fontSize.Items.Add("11");
				this.fontSize.Items.Add("12");
				this.fontSize.Items.Add("14");
				this.fontSize.Items.Add("16");
				this.fontSize.Items.Add("18");
				this.fontSize.Items.Add("20");
				this.fontSize.Items.Add("24");
				this.fontSize.Items.Add("36");
				this.fontSize.Items.Add("48");
				this.fontSize.Items.Add("72");
			}
		}

		protected void ChangeFontSize(double add, double percents)
		{
			double size = this.document.TextWrapper.Defined.FontSize;
			Text.Properties.SizeUnits units = this.document.TextWrapper.Defined.Units;
			if ( double.IsNaN(size) )
			{
				size = this.document.TextWrapper.Active.FontSize;
				units = this.document.TextWrapper.Active.Units;
			}

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				size *= percents/100;
			}
			else
			{
				size += add*Modifier.fontSizeScale;
			}

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.FontSize = size;
			this.document.TextWrapper.Defined.Units = units;
			this.document.TextWrapper.ResumeSynchronisations();
		}


		// Le wrapper associé a changé.
		protected void HandleWrapperChanged(object sender)
		{
			this.UpdateAfterChanging();
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fontFace == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				this.fontFace.Bounds = r;
				this.fontFace.Visibility = true;
				r.Offset(0, -25);

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Right-(120+20);
					r.Width = 120;
					this.fontStyle.Bounds = r;
					this.fontStyle.Visibility = true;
					r.Left = r.Right-1;
					r.Width = 20;
					this.fontFeatures.Bounds = r;
					this.fontFeatures.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 60;
					this.fontSize.Bounds = r;
					this.fontSize.Visibility = true;
					r.Offset(60, 0);
					r.Width = 20;
					this.buttonSizeMinus.Bounds = r;
					this.buttonSizeMinus.Visibility = true;
					r.Offset(20, 0);
					this.buttonSizePlus.Bounds = r;
					this.buttonSizePlus.Visibility = true;
					r.Offset(20+10, 0);
					r.Width = 40;
					this.fontColor.Bounds = r;
					this.fontColor.Visibility = true;
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
					this.buttonClear.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.checkBold.Bounds = r;
					this.checkBold.Visibility = true;
					r.Offset(0, -18);
					this.checkItalic.Bounds = r;
					this.checkItalic.Visibility = true;
				}
				else
				{
					r.Left = rect.Right-(120+20);
					r.Width = 120;
					this.fontStyle.Bounds = r;
					this.fontStyle.Visibility = true;
					r.Left = r.Right-1;
					r.Width = 20;
					this.fontFeatures.Bounds = r;
					this.fontFeatures.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 60;
					this.fontSize.Bounds = r;
					this.fontSize.Visibility = true;
					r.Offset(60, 0);
					r.Width = 20;
					this.buttonSizeMinus.Bounds = r;
					this.buttonSizeMinus.Visibility = true;
					r.Offset(20, 0);
					this.buttonSizePlus.Bounds = r;
					this.buttonSizePlus.Visibility = true;
					r.Offset(20+10, 0);
					r.Width = 40;
					this.fontColor.Bounds = r;
					this.fontColor.Visibility = true;
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;
					this.buttonClear.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.checkBold.Bounds = r;
					this.checkBold.Visibility = true;
					r.Offset(0, -18);
					this.checkItalic.Bounds = r;
					this.checkItalic.Visibility = true;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				r.Right = rect.Right-55;
				this.fontFace.Bounds = r;
				this.fontFace.Visibility = true;

				r.Left = rect.Right-50;
				r.Width = 50;
				this.fontSize.Bounds = r;
				this.fontSize.Visibility = true;

				this.fontStyle.Visibility = false;
				this.fontFeatures.Visibility = false;
				this.buttonSizeMinus.Visibility = false;
				this.buttonSizePlus.Visibility = false;
				this.fontColor.Visibility = false;
				this.buttonClear.Visibility = false;
				this.checkBold.Visibility = false;
				this.checkItalic.Visibility = false;
			}
		}


		#region FeaturesMenu
		// Appelé lors du clic sur le bouton "OpenType" pour ouvrir le menu.
		private void HandleFeaturesClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			if ( button == null )  return;

			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			string face, style;
			string[] features;
#if false
			editObject.GetTextFont(true, out face, out style, out features);
#else
			face = "";
			style = "";
			features = null;
#endif

			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = this.BuildFeaturesMenu(face, style, features);
			if ( menu == null )  return;
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		// Construit le menu des variantes OpenType (features).
		protected VMenu BuildFeaturesMenu(string face, string style, string[] features)
		{
			OpenType.Font font = TextContext.GetFont(face, style);
			if ( font == null )  return null;
			string[] supported = font.GetSupportedFeatures();

			VMenu menu = new VMenu();
			MessageEventHandler message = new MessageEventHandler(this.HandleFeaturesMenu);

			string[] defs = Misc.DefaultFeatures();
			for ( int i=0 ; i<defs.Length ; i++ )
			{
				string text = Misc.GetFeatureText(defs[i]);
				bool active = Misc.IsInsideList(features,  defs[i]);
				bool valid  = Misc.IsInsideList(supported, defs[i]);
				this.BuildFeaturesMenu(menu, font, text, defs[i], active, valid, message);
			}

			for ( int i=0 ; i<supported.Length ; i++ )
			{
				if ( Misc.IsInsideList(defs, supported[i]) )  continue;
				string text = Misc.GetFeatureText(supported[i]);
				bool active = Misc.IsInsideList(features, supported[i]);
				bool valid  = true;
				this.BuildFeaturesMenu(menu, font, text, supported[i], active, valid, message);
			}

			menu.AdjustSize();
			return menu;
		}

		// Crée une case du menu des variantes OpenType (features).
		protected void BuildFeaturesMenu(VMenu menu, OpenType.Font font, string text, string feature, bool active, bool valid, MessageEventHandler message)
		{
			OpenType.LookupTable[] tables = font.GetLookupTables(feature);
			foreach ( OpenType.LookupTable table in tables )
			{
				if ( table.LookupType == OpenType.LookupType.Alternate )  return;
			}

			string icon = Misc.Icon(active ? "ActiveYes" : "ActiveNo");

			if ( !valid )  text = Misc.Italic(text);

			MenuItem item = new MenuItem("", icon, text, "", feature);
			item.Pressed += message;

			menu.Items.Add(item);
		}

		// Appelé lors du choix dans le menu.
		private void HandleFeaturesMenu(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if ( item == null )  return;

			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;

			string cmd = item.Name;

			string face, style;
			string[] features;
#if false
			editObject.GetTextFont(false, out face, out style, out features);
#else
			face = "";
			style = "";
			features = null;
#endif

			if ( features == null )  features = new string[0];
			
			string[] newFeatures;
			if ( Misc.IsInsideList(features, cmd) )
			{
				newFeatures = new string[features.Length-1];
				int j=0;
				for ( int i=0 ; i<features.Length ; i++ )
				{
					if ( features[i] != cmd )
					{
						newFeatures[j++] = features[i];
					}
				}
			}
			else
			{
				newFeatures = new string[features.Length+1];
				for ( int i=0 ; i<features.Length ; i++ )
				{
					newFeatures[i] = features[i];
				}
				newFeatures[features.Length] = cmd;
			}

#if false
			editObject.SetTextFont(face, style, newFeatures);
#endif
		}
		#endregion

		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			this.document.TextWrapper.SuspendSynchronisations();

			if ( sender == this.fontFace )
			{
				string face = this.fontFace.Text;
				if ( face != "" )
				{
					this.document.TextWrapper.Defined.FontFace = face;
					this.document.TextWrapper.Defined.FontStyle = Misc.DefaultFontStyle(face);
				}
				else
				{
					this.document.TextWrapper.Defined.ClearFontFace();
					this.document.TextWrapper.Defined.ClearFontStyle();
				}
			}

			if ( sender == this.fontStyle )
			{
				string style = this.fontStyle.Text;
				if ( style != "" )
				{
					this.document.TextWrapper.Defined.FontStyle = style;
				}
				else
				{
					this.document.TextWrapper.Defined.ClearFontStyle();
				}
			}

			if ( sender == this.fontSize )
			{
				if ( this.fontSize.Text != "" )
				{
					double size;
					Text.Properties.SizeUnits units;
					Misc.ConvertStringToDouble(out size, out units, this.fontSize.Text, 0, 1000, 0);
					if ( units == Common.Text.Properties.SizeUnits.Points )
					{
						size *= Modifier.fontSizeScale;
					}
					this.document.TextWrapper.Defined.FontSize = size;
					this.document.TextWrapper.Defined.Units = units;
				}
				else
				{
					this.document.TextWrapper.Defined.ClearFontSize();
					this.document.TextWrapper.Defined.ClearUnits();
				}
			}

			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.originFieldColor = sender as ColorSample;

			this.originFieldRank = -1;
			if ( this.originFieldColor == this.fontColor )  this.originFieldRank = 0;

			this.OnOriginColorChanged();
		}

		private void HandleFieldColorChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			ColorSample cs = sender as ColorSample;
			if ( cs.ActiveState == ActiveState.Yes )
			{
				this.OnOriginColorChanged();
			}

			this.ColorToWrapper(cs);
		}

		private void HandleButtonSizeMinusClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			this.ChangeFontSize(-1, 80);
		}

		private void HandleButtonSizePlusClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			this.ChangeFontSize(1, 125);
		}

		private void HandleCheckBoldActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			bool value = this.checkBold.ActiveState == ActiveState.Yes;
			
			if ( this.document.TextWrapper.Defined.InvertBold != value ||
				 this.document.TextWrapper.Defined.IsInvertBoldDefined == false )
			{
				this.document.TextWrapper.Defined.InvertBold = value;
			}
		}

		private void HandleCheckItalicActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			bool value = this.checkItalic.ActiveState == ActiveState.Yes;
			
			if ( this.document.TextWrapper.Defined.InvertItalic != value ||
				 this.document.TextWrapper.Defined.IsInvertItalicDefined == false )
			{
				this.document.TextWrapper.Defined.InvertItalic = value;
			}
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.ClearFontFace();
			this.document.TextWrapper.Defined.ClearFontStyle();
			this.document.TextWrapper.Defined.ClearFontFeatures();
			this.document.TextWrapper.Defined.ClearFontSize();
			this.document.TextWrapper.Defined.ClearUnits();
			this.document.TextWrapper.Defined.ClearInvertBold();
			this.document.TextWrapper.Defined.ClearInvertItalic();
			this.document.TextWrapper.Defined.ClearColor();
			this.document.TextWrapper.ResumeSynchronisations();
		}

		
		protected TextFieldCombo			fontFace;
		protected TextFieldCombo			fontStyle;
		protected IconButton				fontFeatures;
		protected TextFieldCombo			fontSize;
		protected ColorSample				fontColor;
		protected IconButton				buttonSizeMinus;
		protected IconButton				buttonSizePlus;
		protected IconButton				buttonClear;
		protected CheckButton				checkBold;
		protected CheckButton				checkItalic;

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}
