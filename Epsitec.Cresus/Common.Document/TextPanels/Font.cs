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

			this.fontFace = new Widgets.TextFieldFontFace(this);
			this.fontFace.IsReadOnly = true;
			this.fontFace.OpeningCombo += new CancelEventHandler(this.HandleFontFaceOpeningCombo);
			this.fontFace.ClosedCombo += new EventHandler(this.HandleFontFaceTextChanged);
			this.fontFace.TabIndex = this.tabIndex++;
			this.fontFace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontFace, Res.Strings.TextPanel.Font.Tooltip.Face);

			this.buttonFilter = this.CreateIconButton("TextFontFilter");

			this.fontStyle = new TextFieldCombo(this);
			this.fontStyle.IsReadOnly = true;
			this.fontStyle.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontStyle.TabIndex = this.tabIndex++;
			this.fontStyle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontStyle, Res.Strings.TextPanel.Font.Tooltip.Style);

			this.fontFeatures = this.CreateIconButton(Misc.Icon("FontFeatures"), Res.Strings.TextPanel.Font.Tooltip.Features, new MessageEventHandler(this.HandleFeaturesClicked));
			this.buttonSettings = this.CreateIconButton(Misc.Icon("Settings"), Res.Strings.Action.Settings, new MessageEventHandler(this.HandleButtonSettingsClicked), false);

			this.fontSize = this.CreateTextFieldLabel(Res.Strings.TextPanel.Font.Tooltip.Size, Res.Strings.TextPanel.Font.Short.Size, Res.Strings.TextPanel.Font.Long.Size, 0,0,0, Widgets.TextFieldLabel.Type.TextFieldUnit, new EventHandler(this.HandleSizeChanged));
			this.fontSize.SetRangeFontSize();
			this.fontSize.SetRangePercents(this.document, 25.0, 400.0, 10.0);
			this.fontSize.IsUnitPercent = true;
			this.fontSize.ButtonUnit.Clicked += new MessageEventHandler(this.HandleButtonUnitClicked);

			this.buttonSizeMenu = this.CreateComboButton(null, Res.Strings.TextPanel.Font.Tooltip.Size, new MessageEventHandler(this.HandleButtonSizeMenuClicked));

			this.fontColor = this.CreateColorSample(Res.Strings.Action.FontColor, new MessageEventHandler(this.HandleFieldColorClicked), new EventHandler(this.HandleFieldColorChanged));

			this.buttonSizeMinus = this.CreateIconButton(Misc.Icon("FontSizeMinus"), Res.Strings.Action.FontSizeMinus, new MessageEventHandler(this.HandleButtonSizeMinusClicked), false);
			this.buttonSizePlus  = this.CreateIconButton(Misc.Icon("FontSizePlus"),  Res.Strings.Action.FontSizePlus,  new MessageEventHandler(this.HandleButtonSizePlusClicked), false);

			this.fontGlue = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Font.Tooltip.Glue, Res.Strings.TextPanel.Font.Short.Glue, Res.Strings.TextPanel.Font.Long.Glue, -50.0, 200.0, 5.0, new EventHandler(this.HandleGlueValueChanged));

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
				this.fontFace.OpeningCombo -= new CancelEventHandler(this.HandleFontFaceOpeningCombo);
				this.fontFace.ClosedCombo -= new EventHandler(this.HandleFontFaceTextChanged);
				this.fontStyle.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontSize.ButtonUnit.Clicked += new MessageEventHandler(this.HandleButtonUnitClicked);
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
						h += 30+25+25+25+40;
					}
					else	// étendu/compact ?
					{
						h += 30+25+25+25+40;
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
			string color = this.GetColorSample(sample);

			if ( color == null )
			{
				this.document.TextWrapper.Defined.ClearColor();
			}
			else
			{
				this.document.TextWrapper.Defined.Color = color;
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
			bool isSize = this.document.TextWrapper.Defined.IsFontSizeDefined;
			if ( isSize )
			{
				size = this.document.TextWrapper.Defined.FontSize;
				units = this.document.TextWrapper.Defined.Units;
			}

			double glue = this.document.TextWrapper.Active.FontGlue;
			if ( double.IsNaN(glue) )  glue = 0.0;  // TODO: devrait être inutile, non ?
			bool isGlue = this.document.TextWrapper.Defined.IsFontGlueDefined;

			string color = this.document.TextWrapper.Defined.Color;
			bool isColor = this.document.TextWrapper.Defined.IsColorDefined;

			bool bold   = this.document.TextWrapper.Defined.InvertBold;
			bool italic = this.document.TextWrapper.Defined.InvertItalic;

			this.ignoreChanged = true;

			this.UpdateComboStyleList(face);

			this.fontFace.Text  = face;
			this.fontStyle.Text = style;
			this.ProposalTextFieldFontFace(this.fontFace, !isFace);
			this.ProposalTextFieldCombo(this.fontStyle, !isStyle);

			this.fontSize.IsUnitPercent = (units == Common.Text.Properties.SizeUnits.Percent);
			this.SetTextFieldRealValue(this.fontSize.TextFieldReal, size, units, isSize, false);

			this.SetTextFieldRealPercent(this.fontGlue.TextFieldReal, glue, isGlue, false);

			this.SetColorSample(this.fontColor, color, isColor, false);

			if ( this.fontColor.ActiveState == ActiveState.Yes )
			{
				this.OnOriginColorChanged();  // change la couleur dans le ColorSelector
			}

			this.checkBold.ActiveState   = bold   ? ActiveState.Yes : ActiveState.No;
			this.checkItalic.ActiveState = italic ? ActiveState.Yes : ActiveState.No;
			
			this.ignoreChanged = false;
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

				r.Left = rect.Left;
				r.Right = rect.Right-20;
				this.fontFace.Bounds = r;
				r.Left = rect.Right-20;
				r.Right = rect.Right;
				this.buttonFilter.Bounds = r;

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 129;
				this.fontStyle.Bounds = r;
				this.fontStyle.Visibility = true;
				r.Offset(129+5, 0);
				r.Width = 20;
				this.fontFeatures.Bounds = r;
				this.fontFeatures.Visibility = true;
				r.Left = rect.Right-20;
				r.Right = rect.Right;
				this.buttonSettings.Bounds = r;
				this.buttonSettings.Visibility = true;

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 69;
				this.fontSize.Bounds = r;
				this.fontSize.Visibility = true;
				r.Offset(69, 0);
				r.Width = 20;
				this.buttonSizeMenu.Bounds = r;
				this.buttonSizeMenu.Visibility = true;
				r.Offset(20, 0);
				this.buttonSizeMinus.Bounds = r;
				this.buttonSizeMinus.Visibility = true;
				r.Offset(20, 0);
				this.buttonSizePlus.Bounds = r;
				this.buttonSizePlus.Visibility = true;
				r.Left = rect.Right-40;
				r.Width = 40;
				this.fontColor.Bounds = r;
				this.fontColor.Visibility = true;

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right-25;
				this.fontGlue.Bounds = r;
				this.fontGlue.Visibility = true;
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
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Right = rect.Right-20;
				this.fontFace.Bounds = r;
				r.Left = rect.Right-20;
				r.Right = rect.Right;
				this.buttonFilter.Bounds = r;

				this.fontSize.Visibility = false;
				this.fontStyle.Visibility = false;
				this.fontFeatures.Visibility = false;
				this.buttonSettings.Visibility = false;
				this.fontGlue.Visibility = false;
				this.buttonSizeMenu.Visibility = false;
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

			string face = this.document.TextWrapper.Active.FontFace;
			string style = this.document.TextWrapper.Active.FontStyle;
			string[] features = this.document.TextWrapper.Active.FontFeatures;

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

			string[] features = this.document.TextWrapper.Active.FontFeatures;
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

			this.document.TextWrapper.Defined.FontFeatures = newFeatures;
		}
		#endregion

		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			this.document.TextWrapper.SuspendSynchronisations();

			if ( sender == this.fontStyle )
			{
				string style = this.fontStyle.Text;
				if ( style != "" )
				{
					string face = this.document.TextWrapper.Active.FontFace;
					this.document.TextWrapper.Defined.FontFace = face;
					this.document.TextWrapper.Defined.FontStyle = style;
				}
				else
				{
					this.document.TextWrapper.Defined.ClearFontStyle();
				}
			}

			this.document.TextWrapper.ResumeSynchronisations();
		}

		// Le combo pour les polices va être ouvert.
		private void HandleFontFaceOpeningCombo(object sender, CancelEventArgs e)
		{
			bool quickOnly = this.document.Modifier.ActiveViewer.DrawingContext.TextFontFilter;
			string selectedFontFace = this.document.TextWrapper.Active.FontFace;
			int quickCount;
			System.Collections.ArrayList fontList = Misc.MergeFontList(Misc.GetFontList(false), this.document.Settings.QuickFonts, quickOnly, selectedFontFace, out quickCount);

			this.fontFace.FontList = fontList;
			this.fontFace.SampleHeight = this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleHeight;
			this.fontFace.QuickCount = quickCount;
		}

		private void HandleFontFaceTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			string face = this.fontFace.Text;

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.FontFace = face;
			this.document.TextWrapper.Defined.FontStyle = Misc.DefaultFontStyle(face);
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleButtonSettingsClicked(object sender, MessageEventArgs e)
		{
			this.document.Notifier.NotifySettingsShowPage("BookDocument", "Fonts");
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

		private void HandleButtonUnitClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.fontSize.IsUnitPercent = !this.fontSize.IsUnitPercent;

			double value;
			Common.Text.Properties.SizeUnits units;

			if ( this.fontSize.IsUnitPercent )
			{
				value = 1.0;  // 100%
				units = Common.Text.Properties.SizeUnits.Percent;
			}
			else
			{
				value = 12.0 * Modifier.fontSizeScale;
				units = Common.Text.Properties.SizeUnits.Points;
			}

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.FontSize = value;
			this.document.TextWrapper.Defined.Units = units;
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleSizeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value;
			Common.Text.Properties.SizeUnits units;
			bool isDefined;
			this.GetTextFieldRealValue(field, out value, out units, out isDefined);

			this.document.TextWrapper.SuspendSynchronisations();

			if ( isDefined )
			{
				this.document.TextWrapper.Defined.FontSize = value;
				this.document.TextWrapper.Defined.Units = units;
			}
			else
			{
				this.document.TextWrapper.Defined.ClearFontSize();
				this.document.TextWrapper.Defined.ClearUnits();
			}

			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleButtonSizeMenuClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(button.Width, 0));
			VMenu menu = this.CreateMenu();
			pos.X -= menu.Width;
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleButtonSizeMinusClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			this.document.Wrappers.IncrementFontSize(-1);
		}

		private void HandleButtonSizePlusClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			this.document.Wrappers.IncrementFontSize(1);
		}

		private void HandleGlueValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			double value;
			bool isDefined;
			this.GetTextFieldRealPercent(this.fontGlue.TextFieldReal, out value, out isDefined);

			if ( isDefined )
			{
				this.document.TextWrapper.Defined.FontGlue = value;
			}
			else
			{
				this.document.TextWrapper.Defined.ClearFontGlue();
			}
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
			this.document.TextWrapper.Defined.ClearFontGlue();
			this.document.TextWrapper.Defined.ClearUnits();
			this.document.TextWrapper.Defined.ClearInvertBold();
			this.document.TextWrapper.Defined.ClearInvertItalic();
			this.document.TextWrapper.Defined.ClearColor();
			this.document.TextWrapper.ResumeSynchronisations();
		}

		
		#region Menu
		// Construit le menu pour choisir une taille.
		protected VMenu CreateMenu()
		{
			double size = this.document.TextWrapper.Active.FontSize;
			Text.Properties.SizeUnits units = this.document.TextWrapper.Active.Units;
			if ( this.document.TextWrapper.Defined.IsFontSizeDefined )
			{
				size = this.document.TextWrapper.Defined.FontSize;
				units = this.document.TextWrapper.Defined.Units;
			}
			bool percent = (units == Common.Text.Properties.SizeUnits.Percent);

			return Menus.FontSizeMenu.CreateFontSizeMenu(size, percent?"%":"", new MessageEventHandler(this.HandleMenuPressed));
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			string text = item.Name;

			double size;
			Common.Text.Properties.SizeUnits units;

			if ( text.EndsWith("%") )
			{
				text = text.Substring(0, text.Length-1);
				size = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
				units = Common.Text.Properties.SizeUnits.Percent;
			}
			else
			{
				size = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
				units = Common.Text.Properties.SizeUnits.Points;
			}

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.FontSize = size;
			this.document.TextWrapper.Defined.Units = units;
			this.document.TextWrapper.ResumeSynchronisations();
		}
		#endregion

		
		protected Widgets.TextFieldFontFace fontFace;
		protected IconButton				buttonFilter;
		protected TextFieldCombo			fontStyle;
		protected IconButton				fontFeatures;
		protected IconButton				buttonSettings;
		protected Widgets.TextFieldLabel	fontSize;
		protected ColorSample				fontColor;
		protected Widgets.TextFieldLabel	fontGlue;
		protected GlyphButton				buttonSizeMenu;
		protected IconButton				buttonSizeMinus;
		protected IconButton				buttonSizePlus;
		protected IconButton				buttonClear;
		protected CheckButton				checkBold;
		protected CheckButton				checkItalic;

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}

