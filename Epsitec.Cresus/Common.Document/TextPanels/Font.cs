using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Font permet de choisir une police de caractères.
	/// </summary>
	public class Font : Abstract
	{
		public Font(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Font.Title;

			this.fixIcon.Text = Misc.Image("TextFont");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Font.Title);

			this.fontFace = new Widgets.FontFaceCombo(this);
			this.fontFace.IsReadOnly = true;
			this.fontFace.AutoFocus = false;
			this.fontFace.ComboOpening += new EventHandler<CancelEventArgs> (this.HandleFontFaceComboOpening);
			this.fontFace.ComboClosed += this.HandleFontFaceTextChanged;
			this.fontFace.TabIndex = this.tabIndex++;
			this.fontFace.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontFace, Res.Strings.TextPanel.Font.Tooltip.Face);

			if (Command.IsDefined ("TextFontFilter"))
			{
				this.buttonFilter = this.CreateIconButton ("TextFontFilter");
			}

			this.fontStyle = new TextFieldCombo(this);
			this.fontStyle.IsReadOnly = true;
			this.fontStyle.AutoFocus = false;
			this.fontStyle.ComboClosed += this.HandleFontStyleTextChanged;
			this.fontStyle.TabIndex = this.tabIndex++;
			this.fontStyle.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontStyle, Res.Strings.TextPanel.Font.Tooltip.Style);

			this.fontFeatures = this.CreateIconButton(Misc.Icon("FontFeatures"), Res.Strings.TextPanel.Font.Tooltip.Features, this.HandleFeaturesClicked);

			if (Command.IsDefined ("Settings"))
			{
				this.buttonSettings = this.CreateIconButton (Misc.Icon ("Settings"), Res.Strings.Action.Settings, this.HandleButtonSettingsClicked, false);
			}

			this.fontSize = this.CreateTextFieldLabel(Res.Strings.TextPanel.Font.Tooltip.Size, Res.Strings.TextPanel.Font.Short.Size, Res.Strings.TextPanel.Font.Long.Size, 0,0,0,0, Widgets.TextFieldLabel.Type.TextFieldUnit, this.HandleSizeChanged);
			this.fontSize.SetRangeFontSize(this.document);
			this.fontSize.SetRangePercents(this.document, 25.0, 400.0, 100.0, 10.0);
			this.fontSize.IsUnitPercent = true;
			this.fontSize.ButtonUnit.Clicked += this.HandleButtonUnitClicked;
			this.fontSize.Name = "FontSize Field";

			this.buttonSizeMenu = this.CreateComboButton(null, Res.Strings.TextPanel.Font.Tooltip.Size, this.HandleButtonSizeMenuClicked);
			this.buttonSizeMenu.Name = "FontSize Menu";

			this.fontColor = this.CreateColorSample(Res.Strings.Action.FontColor, this.HandleFieldColorClicked, this.HandleFieldColorChanged);

			this.fontGlue = this.CreateTextFieldLabelPercent(Res.Strings.TextPanel.Font.Tooltip.Glue, Res.Strings.TextPanel.Font.Short.Glue, Res.Strings.TextPanel.Font.Long.Glue, -50.0, 200.0, 0.0, 5.0, this.HandleGlueValueChanged);

			this.buttonClear = this.CreateClearButton(this.HandleClearClicked);
			this.buttonClear.Name = "Clear";

			this.checkBold = new CheckButton(this);
			this.checkBold.Text = Res.Strings.Action.FontInvertBold;
			this.checkBold.TabIndex = this.tabIndex++;
			this.checkBold.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.checkBold.ActiveStateChanged += this.HandleCheckBoldActiveStateChanged;

			this.checkItalic = new CheckButton(this);
			this.checkItalic.Text = Res.Strings.Action.FontInvertItalic;
			this.checkItalic.TabIndex = this.tabIndex++;
			this.checkItalic.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.checkItalic.ActiveStateChanged += this.HandleCheckItalicActiveStateChanged;

			this.TextWrapper.Active.Changed  += this.HandleWrapperChanged;
			this.TextWrapper.Defined.Changed += this.HandleWrapperChanged;

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fontFace.ComboOpening -= new EventHandler<CancelEventArgs> (this.HandleFontFaceComboOpening);
				this.fontFace.ComboClosed -= this.HandleFontFaceTextChanged;
				this.fontStyle.ComboClosed -= this.HandleFontStyleTextChanged;
				this.fontSize.ButtonUnit.Clicked += this.HandleButtonUnitClicked;
				this.fontColor.Clicked -= this.HandleFieldColorClicked;
				this.fontColor.ColorChanged -= this.HandleFieldColorChanged;

				this.TextWrapper.Active.Changed  -= this.HandleWrapperChanged;
				this.TextWrapper.Defined.Changed -= this.HandleWrapperChanged;

				this.fontFace = null;
				this.fontSize = null;
				this.fontColor = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.fontSize.ButtonUnitEnable = !this.TextWrapper.IsAttachedToDefaultParagraphStyle;
			this.buttonClear.Visibility = !this.TextWrapper.IsAttachedToDefaultParagraphStyle;
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
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

					if ( this.styleCategory == StyleCategory.Paragraph )
					{
						h -= 40;
					}
				}
				else	// panneau réduit ?
				{
					h += 30+25+25;
				}

				return h;
			}
		}

		public override double TopMargin
		{
			//	Retourne la marge supérieure.
			get
			{
				return 5;
			}
		}


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.fontColor.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				if ( rank == 0 )  this.originFieldColor = this.fontColor;
			}
			if ( this.originFieldColor == null )  return;

			this.OriginColorDeselect();
			this.originFieldColor.ActiveState = ActiveState.Yes;
		}

		public override int OriginColorRank()
		{
			//	Retourne le rang de la couleur d'origine.
			return this.originFieldRank;
		}

		public override void OriginColorChange(Drawing.RichColor color)
		{
			//	Modifie la couleur d'origine.
			if ( this.originFieldColor == null )  return;
			
			if ( this.originFieldColor.Color != color )
			{
				this.originFieldColor.Color = color;
				this.ColorToWrapper(this.originFieldColor);
			}
		}

		public override Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			if ( this.originFieldColor == null )  return Drawing.RichColor.FromBrightness(0.0);
			return this.originFieldColor.Color;
		}

		protected void ColorToWrapper(ColorSample sample)
		{
			//	Donne la couleur au wrapper.
			string color = this.GetColorSample(sample);

			this.TextWrapper.SuspendSynchronizations();

			if ( color == null )
			{
				this.TextWrapper.Defined.ClearColor();
			}
			else
			{
				this.TextWrapper.Defined.Color = color;
			}
			
			this.TextWrapper.DefineOperationName("FontColor", Res.Strings.Action.FontColor);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		
		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.TextWrapper.IsAttached == false )  return;

			string face = this.TextWrapper.Active.FontFace;
			bool isFace = this.TextWrapper.Defined.IsFontFaceDefined;

			string style = this.TextWrapper.Active.FontStyle;
			bool isStyle = this.TextWrapper.Defined.IsFontStyleDefined;

			double size = this.TextWrapper.Active.FontSize;
			Text.Properties.SizeUnits units = this.TextWrapper.Active.Units;
			bool isSize = this.TextWrapper.Defined.IsFontSizeDefined;
			if ( isSize )
			{
				size = this.TextWrapper.Defined.FontSize;
				units = this.TextWrapper.Defined.Units;
			}

			double glue = this.TextWrapper.Active.FontGlue;
			bool isGlue = this.TextWrapper.Defined.IsFontGlueDefined;

			string color = this.TextWrapper.Defined.Color;
			bool isColor = this.TextWrapper.Defined.IsColorDefined;

			bool bold   = this.TextWrapper.Defined.InvertBold;
			bool italic = this.TextWrapper.Defined.InvertItalic;

			this.ignoreChanged = true;

			this.UpdateComboStyleList(face, style);

			this.fontFace.Text  = TextLayout.ConvertToTaggedText(Misc.FaceInvariantToInvariant(face, style));
			this.fontStyle.Text = TextLayout.ConvertToTaggedText(Misc.StyleInvariantToLocale(face, style));
			this.fontStyle.Enable = !string.IsNullOrEmpty(face);
			this.ProposalFontFaceCombo(this.fontFace, !isFace);
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

		protected void UpdateComboStyleList(string face, string style)
		{
			//	Met à jour la liste d'un champ éditable pour le style de la police.
			this.fontStyle.Items.Clear();  // vide la liste
			face = Misc.FaceInvariantToInvariant(face, style);
			
			if ( face != null && style != null )
			{
				OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(face);
				this.stylesInvariant = new List<OpenType.FontIdentity>();
				this.stylesLocal = new List<string>();

				foreach ( OpenType.FontIdentity id in list )
				{
					this.fontStyle.Items.Add(id.LocaleStyleName);

					this.stylesInvariant.Add(id);
					this.stylesLocal.Add(id.LocaleStyleName);
				}
			}
		}


		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fontFace == null )  return;

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Right = rect.Right-20;
				this.fontFace.SetManualBounds(r);
				r.Left = rect.Right-20;
				r.Right = rect.Right;

				if (this.buttonFilter != null)
				{
					this.buttonFilter.SetManualBounds (r);
				}

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 129;
				this.fontStyle.SetManualBounds(r);
				r.Offset(129+5, 0);
				r.Width = 20;
				this.fontFeatures.SetManualBounds(r);
				this.fontFeatures.Visibility = true;
				r.Left = rect.Right-20;
				r.Right = rect.Right;
				if (this.buttonSettings != null)
				{
					this.buttonSettings.SetManualBounds (r);
					this.buttonSettings.Visibility = true;
				}

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 69;
				this.fontSize.SetManualBounds(r);
				r.Offset(69, 0);
				r.Width = 20;
				this.buttonSizeMenu.SetManualBounds(r);
				r.Left = rect.Right-40;
				r.Width = 40;
				this.fontColor.SetManualBounds(r);

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right-25;
				this.fontGlue.SetManualBounds(r);
				this.fontGlue.Visibility = true;
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.checkBold.SetManualBounds(r);
				this.checkBold.Visibility = (this.styleCategory == StyleCategory.Paragraph) ? false : true;
				r.Offset(0, -18);
				this.checkItalic.SetManualBounds(r);
				this.checkItalic.Visibility = (this.styleCategory == StyleCategory.Paragraph) ? false : true;
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Right = rect.Right-20;
				this.fontFace.SetManualBounds(r);
				r.Left = rect.Right-20;
				r.Right = rect.Right;

				if (this.buttonFilter != null)
				{
					this.buttonFilter.SetManualBounds (r);
				}

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right-25;
				this.fontStyle.SetManualBounds(r);
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 69;
				this.fontSize.SetManualBounds(r);
				r.Offset(69, 0);
				r.Width = 20;
				this.buttonSizeMenu.SetManualBounds(r);
				r.Left = rect.Right-40;
				r.Width = 40;
				this.fontColor.SetManualBounds(r);

				this.fontFeatures.Visibility = false;
				if (this.buttonSettings != null)
				{
					this.buttonSettings.Visibility = false;
				}
				this.fontGlue.Visibility = false;
				this.checkBold.Visibility = false;
				this.checkItalic.Visibility = false;
			}
		}


		#region FeaturesMenu
		private void HandleFeaturesClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lors du clic sur le bouton "OpenType" pour ouvrir le menu.
			IconButton button = sender as IconButton;
			if ( button == null )  return;

			string face = this.TextWrapper.Active.FontFace;
			string style = this.TextWrapper.Active.FontStyle;
			string[] features = this.TextWrapper.Active.FontFeatures;

			VMenu menu = this.BuildFeaturesMenu(face, style, features);
			if ( menu == null )  return;

			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu, true);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		protected VMenu BuildFeaturesMenu(string face, string style, string[] features)
		{
			//	Construit le menu des variantes OpenType (features).
			OpenType.Font font = TextContext.GetFont(face, style);
			if ( font == null )  return null;
			string[] supported = font.GetSupportedFeatures();

			VMenu menu = new VMenu();
			
			string[] defs = Misc.DefaultFeatures();
			for ( int i=0 ; i<defs.Length ; i++ )
			{
				string text = Misc.GetFeatureText(defs[i]);
				bool active = Misc.IsInsideList(features,  defs[i]);
				bool valid  = Misc.IsInsideList(supported, defs[i]);
				this.BuildFeaturesMenu(menu, font, text, defs[i], active, valid, this.HandleFeaturesMenu);
			}

			for ( int i=0 ; i<supported.Length ; i++ )
			{
				if ( Misc.IsInsideList(defs, supported[i]) )  continue;
				string text = Misc.GetFeatureText(supported[i]);
				bool active = Misc.IsInsideList(features, supported[i]);
				bool valid  = true;
				this.BuildFeaturesMenu(menu, font, text, supported[i], active, valid, this.HandleFeaturesMenu);
			}

			menu.AdjustSize();
			return menu;
		}

		protected void BuildFeaturesMenu(VMenu menu, OpenType.Font font, string text, string feature, bool active, bool valid, Support.EventHandler<MessageEventArgs> message)
		{
			//	Crée une case du menu des variantes OpenType (features).
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

		private void HandleFeaturesMenu(object sender, MessageEventArgs e)
		{
			//	Appelé lors du choix dans le menu.
			MenuItem item = sender as MenuItem;
			if ( item == null )  return;

			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;

			string cmd = item.Name;

			string[] features = this.TextWrapper.Active.FontFeatures;
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

			this.TextWrapper.SuspendSynchronizations();
			this.TextWrapper.Defined.FontFeatures = newFeatures;
			this.TextWrapper.DefineOperationName("FontFeatures", Res.Strings.Action.FontFeatures);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		#endregion


		private void HandleFontFaceComboOpening(object sender, CancelEventArgs e)
		{
			//	Le combo pour les polices va être ouvert.
			bool quickOnly = this.document.Modifier.ActiveViewer.DrawingContext.TextFontFilter;
			string selectedFontFace = this.TextWrapper.Active.FontFace;
			int quickCount;
			System.Collections.ArrayList fontList = Misc.MergeFontList(Misc.GetFontList(false), this.document.Settings.QuickFonts, quickOnly, selectedFontFace, out quickCount);

			this.fontFace.FontList     = fontList;
			this.fontFace.QuickCount   = quickCount;
			this.fontFace.SampleHeight = this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleHeight;
			this.fontFace.SampleAbc    = this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleAbc;
		}

		private void HandleFontFaceTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			string face  = TextLayout.ConvertToSimpleText(this.fontFace.Text);
			string style = TextLayout.ConvertToSimpleText(this.fontStyle.Text);

			this.TextWrapper.SuspendSynchronizations();

			if (string.IsNullOrEmpty(face))
			{
				this.TextWrapper.Defined.ClearFontFace();
				this.TextWrapper.Defined.ClearFontStyle();
			}
			else
			{
				if ( !Misc.IsExistingFontStyle(face, style) )
				{
					style = Misc.DefaultFontStyle(face);
				}

				this.TextWrapper.Defined.FontFace = face;
				this.TextWrapper.Defined.FontStyle = style;
				this.TextWrapper.DefineOperationName("FontFace", Res.Strings.Action.FontFace);
			}

			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleFontStyleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			string style = TextLayout.ConvertToSimpleText(this.fontStyle.Text);
			string face = this.TextWrapper.Active.FontFace;

			for (int i=0; i<this.stylesLocal.Count; i++)
			{
				if (this.stylesLocal[i] == style)
				{
					//?style = this.stylesInvariant[i].InvariantSimpleStyleName;
					//?face  = this.stylesInvariant[i].InvariantSimpleFaceName;
					style = this.stylesInvariant[i].InvariantStyleName;
					face  = this.stylesInvariant[i].InvariantFaceName;
					break;
				}
			}

			this.TextWrapper.SuspendSynchronizations();

			if (string.IsNullOrEmpty(style))
			{
				this.TextWrapper.Defined.ClearFontStyle();
			}
			else
			{
				this.TextWrapper.Defined.FontFace = face;
				this.TextWrapper.Defined.FontStyle = style;
			}

			this.TextWrapper.DefineOperationName("FontStyle", Res.Strings.Action.FontStyle);

			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
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

			ColorSample c = sender as ColorSample;
			if ( c.ActiveState == ActiveState.Yes )
			{
				this.OnOriginColorChanged();
			}

			this.ColorToWrapper(c);
		}

		private void HandleButtonUnitClicked(object sender, MessageEventArgs e)
		{
			if ( !this.TextWrapper.IsAttached )  return;

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
				value = 12.0 * Modifier.FontSizeScale;
				units = Common.Text.Properties.SizeUnits.Points;
			}

			this.TextWrapper.SuspendSynchronizations();
			this.TextWrapper.Defined.FontSize = value;
			this.TextWrapper.Defined.Units = units;
			this.TextWrapper.DefineOperationName("FontSize", Res.Strings.Action.FontSize);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleSizeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			double value;
			Common.Text.Properties.SizeUnits units;
			bool isDefined;
			this.GetTextFieldRealValue(field, out value, out units, out isDefined);

			this.TextWrapper.SuspendSynchronizations();

			if ( isDefined )
			{
				this.TextWrapper.Defined.FontSize = value;
				this.TextWrapper.Defined.Units = units;
			}
			else
			{
				this.TextWrapper.Defined.ClearFontSize();
				this.TextWrapper.Defined.ClearUnits();
			}

			this.TextWrapper.DefineOperationName("FontSize", Res.Strings.Action.FontSize);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleButtonSizeMenuClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			VMenu menu = this.CreateMenu();
			menu.Host = this;
			menu.MinWidth = this.fontSize.ActualWidth+button.ActualWidth;
			TextFieldCombo.AdjustComboSize(this.fontSize, menu, false);
			menu.ShowAsComboList(this.fontSize, Point.Zero, button);
		}

		private void HandleGlueValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			double value;
			bool isDefined;
			this.GetTextFieldRealPercent(this.fontGlue.TextFieldReal, out value, out isDefined);

			this.TextWrapper.SuspendSynchronizations();

			if ( isDefined )
			{
				this.TextWrapper.Defined.FontGlue = value;
			}
			else
			{
				this.TextWrapper.Defined.ClearFontGlue();
			}

			this.TextWrapper.DefineOperationName("FontGlue", Res.Strings.TextPanel.Font.Tooltip.Glue);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleCheckBoldActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;
			
			bool value = this.checkBold.ActiveState == ActiveState.Yes;
			
			if ( this.TextWrapper.Defined.InvertBold != value ||
				 this.TextWrapper.Defined.IsInvertBoldDefined == false )
			{
				this.TextWrapper.SuspendSynchronizations();
				this.TextWrapper.Defined.InvertBold = value;
				this.TextWrapper.DefineOperationName(Res.Commands.FontBold.CommandId, Res.Strings.Action.FontBold);
				this.TextWrapper.ResumeSynchronizations();
				this.ActionMade();
			}
		}

		private void HandleCheckItalicActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;
			
			bool value = this.checkItalic.ActiveState == ActiveState.Yes;
			
			if ( this.TextWrapper.Defined.InvertItalic != value ||
				 this.TextWrapper.Defined.IsInvertItalicDefined == false )
			{
				this.TextWrapper.SuspendSynchronizations();
				this.TextWrapper.Defined.InvertItalic = value;
				this.TextWrapper.DefineOperationName(Res.Commands.FontItalic.CommandId, Res.Strings.Action.FontItalic);
				this.TextWrapper.ResumeSynchronizations();
				this.ActionMade();
			}
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			this.TextWrapper.Defined.ClearFontFace();
			this.TextWrapper.Defined.ClearFontStyle();
			this.TextWrapper.Defined.ClearFontFeatures();
			this.TextWrapper.Defined.ClearFontSize();
			this.TextWrapper.Defined.ClearFontGlue();
			this.TextWrapper.Defined.ClearUnits();
			this.TextWrapper.Defined.ClearInvertBold();
			this.TextWrapper.Defined.ClearInvertItalic();
			this.TextWrapper.Defined.ClearColor();
			this.TextWrapper.DefineOperationName("FontFaceClear", Res.Strings.TextPanel.Clear);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		
		#region Menu
		protected VMenu CreateMenu()
		{
			//	Construit le menu pour choisir une taille.
			double size = this.TextWrapper.Active.FontSize;
			Text.Properties.SizeUnits units = this.TextWrapper.Active.Units;
			if ( this.TextWrapper.Defined.IsFontSizeDefined )
			{
				size = this.TextWrapper.Defined.FontSize;
				units = this.TextWrapper.Defined.Units;
			}
			bool percent = (units == Common.Text.Properties.SizeUnits.Percent);

			double factor = (this.document.Type == DocumentType.Pictogram) ? 0.1 : 1.0;
			bool isPercent = !this.TextWrapper.IsAttachedToDefaultParagraphStyle;
			bool isDefault = !this.TextWrapper.IsAttachedToDefaultParagraphStyle;
			return Menus.FontSizeMenu.CreateFontSizeMenu(size, percent?"%":"", factor, isPercent, isDefault, this.HandleMenuPressed);
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			string text = item.Name;

			double size;
			Common.Text.Properties.SizeUnits units;

			if (string.IsNullOrEmpty(text))
			{
				size = double.NaN;
				units = Common.Text.Properties.SizeUnits.Points;
			}
			else
			{
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
			}

			this.TextWrapper.SuspendSynchronizations();

			if ( double.IsNaN(size) )
			{
				this.TextWrapper.Defined.ClearFontSize();
				this.TextWrapper.Defined.ClearUnits();
			}
			else
			{
				this.TextWrapper.Defined.FontSize = size;
				this.TextWrapper.Defined.Units = units;
				this.TextWrapper.DefineOperationName("FontFace", Res.Strings.Action.FontFace);
			}

			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}
		#endregion

		
		protected Widgets.FontFaceCombo		fontFace;
		protected IconButton				buttonFilter;
		protected TextFieldCombo			fontStyle;
		protected IconButton				fontFeatures;
		protected IconButton				buttonSettings;
		protected Widgets.TextFieldLabel	fontSize;
		protected ColorSample				fontColor;
		protected Widgets.TextFieldLabel	fontGlue;
		protected GlyphButton				buttonSizeMenu;
		protected IconButton				buttonClear;
		protected CheckButton				checkBold;
		protected CheckButton				checkItalic;

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
		protected List<OpenType.FontIdentity> stylesInvariant;
		protected List<string>				stylesLocal;
	}
}

