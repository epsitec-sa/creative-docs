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
			this.label.Text = "Police";

			this.fontFace = new TextFieldCombo(this);
			this.fontFace.IsReadOnly = true;
			this.fontFace.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontFace.TabIndex = this.tabIndex++;
			this.fontFace.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontFace, Res.Strings.Action.Text.Font.Face);

			this.fontStyle = new TextFieldCombo(this);
			this.fontStyle.IsReadOnly = true;
			this.fontStyle.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontStyle.TabIndex = this.tabIndex++;
			this.fontStyle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontStyle, Res.Strings.Action.Text.Font.Style);

			this.fontFeatures = this.CreateIconButton(Misc.Icon("FontFeatures"), Res.Strings.Action.Text.Font.Features, new MessageEventHandler(this.HandleFeaturesClicked));

			this.fontSize = new TextFieldCombo(this);
			this.fontSize.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontSize.TabIndex = this.tabIndex++;
			this.fontSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontSize, Res.Strings.Action.Text.Font.Size);

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleSource = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fontColor.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fontColor.TabIndex = this.tabIndex++;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, Res.Strings.Action.Text.Font.Color);

			this.buttonSizeMinus   = this.CreateIconButton(Misc.Icon("FontSizeMinus"),   Res.Strings.Action.Text.Font.SizeMinus,   new MessageEventHandler(this.HandleButtonSizeMinusClicked), false);
			this.buttonSizePlus    = this.CreateIconButton(Misc.Icon("FontSizePlus"),    Res.Strings.Action.Text.Font.SizePlus,    new MessageEventHandler(this.HandleButtonSizePlusClicked), false);
			this.buttonBold        = this.CreateIconButton(Misc.IconL("FontBold"),       Res.Strings.Action.Text.Font.Bold,        new MessageEventHandler(this.HandleButtonBoldClicked));
			this.buttonItalic      = this.CreateIconButton(Misc.IconL("FontItalic"),     Res.Strings.Action.Text.Font.Italic,      new MessageEventHandler(this.HandleButtonItalicClicked));
			this.buttonUnderlined  = this.CreateIconButton(Misc.IconL("FontUnderlined"), Res.Strings.Action.Text.Font.Underlined,  new MessageEventHandler(this.HandleButtonClicked));
			this.buttonStrike      = this.CreateIconButton(Misc.Icon("FontStrike"),      Res.Strings.Action.Text.Font.Strike,      new MessageEventHandler(this.HandleButtonClicked));
			this.buttonFrame       = this.CreateIconButton(Misc.Icon("FontFrame"),       Res.Strings.Action.Text.Font.Frame,       new MessageEventHandler(this.HandleButtonClicked));
			this.buttonSubscript   = this.CreateIconButton(Misc.Icon("FontSubscript"),   Res.Strings.Action.Text.Font.Subscript,   new MessageEventHandler(this.HandleButtonClicked));
			this.buttonSuperscript = this.CreateIconButton(Misc.Icon("FontSuperscript"), Res.Strings.Action.Text.Font.Superscript, new MessageEventHandler(this.HandleButtonClicked));

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
						h += 105;
					}
					else	// étendu/compact ?
					{
						h += 80;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			string face = this.document.FontWrapper.Defined.FontFace;
			string baseFace = face;
			if ( face == null )
			{
				face = this.document.FontWrapper.Active.FontFace;
				baseFace = face;
				if ( face == null )
				{
					face = Res.Strings.Action.Text.Font.Default;
					baseFace = null;
				}
				face = Misc.Italic(face);
			}

			string style = this.document.FontWrapper.Defined.FontStyle;
			if ( style == null )
			{
				style = this.document.FontWrapper.Active.FontStyle;
				if ( style == null )
				{
					style = Res.Strings.Action.Text.Font.Default;
				}
				style = Misc.Italic(style);
			}

			string textSize = "";
			double size = this.document.FontWrapper.Defined.FontSize;
			Text.Properties.SizeUnits units = this.document.FontWrapper.Defined.Units;
			if ( double.IsNaN(size) )
			{
				size = this.document.FontWrapper.Active.FontSize;
				units = this.document.FontWrapper.Active.Units;
				if ( double.IsNaN(size) )
				{
					textSize = "*";
				}
				else
				{
					if ( units == Common.Text.Properties.SizeUnits.Points )
					{
						size /= Modifier.fontSizeScale;
					}
					textSize = Misc.Italic(Misc.ConvertDoubleToString(size, units, 0));
				}
			}
			else
			{
				if ( units == Common.Text.Properties.SizeUnits.Points )
				{
					size /= Modifier.fontSizeScale;
				}
				textSize = Misc.ConvertDoubleToString(size, units, 0);
			}

			this.ignoreChanged = true;

			this.UpdateComboFaceList();
			this.UpdateComboStyleList(baseFace);
			this.UpdateComboSizeList();

			this.fontFace.Text = face;
			this.fontStyle.Text = style;
			this.fontSize.Text = textSize;

			this.UpdateButtonBold();
			this.UpdateButtonItalic();
			
			this.ignoreChanged = false;
		}

		protected void UpdateButtonBold()
		{
			bool enabled = false;
			bool state   = false;

			if ( this.document != null && this.document.FontWrapper.IsAttached )
			{
				string face  = this.document.FontWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.document.FontWrapper.Active.FontFace;
				}

				string style = this.document.FontWrapper.Defined.FontStyle;
				if ( style == null )
				{
					style = this.document.FontWrapper.Active.FontStyle;
				}

				OpenType.FontWeight weight = OpenType.FontWeight.Medium;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);
					weight = font.FontIdentity.FontWeight;
				}

				enabled = true;
				state   = ((int)weight > (int)OpenType.FontWeight.Medium);
				state  ^= this.document.FontWrapper.Defined.InvertBold;
			}

			this.buttonBold.SetEnabled(enabled);
			this.buttonBold.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdateButtonItalic()
		{
			bool enabled = false;
			bool state   = false;

			if ( this.document != null && this.document.FontWrapper.IsAttached )
			{
				string face  = this.document.FontWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.document.FontWrapper.Active.FontFace;
				}

				string style = this.document.FontWrapper.Defined.FontStyle;
				if ( style == null )
				{
					style = this.document.FontWrapper.Active.FontStyle;
				}

				OpenType.FontStyle italic = OpenType.FontStyle.Normal;
				if ( face != null && style != null )
				{
					OpenType.Font font = TextContext.GetFont(face, style);
					italic = font.FontIdentity.FontStyle;
				}

				enabled = true;
				state   = italic != OpenType.FontStyle.Normal;
				state  ^= this.document.FontWrapper.Defined.InvertItalic;
			}

			this.buttonItalic.SetEnabled(enabled);
			this.buttonItalic.ActiveState = state ? ActiveState.Yes : ActiveState.No;
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
			double size = this.document.FontWrapper.Defined.FontSize;
			Text.Properties.SizeUnits units = this.document.FontWrapper.Defined.Units;
			if ( double.IsNaN(size) )
			{
				size = this.document.FontWrapper.Active.FontSize;
				units = this.document.FontWrapper.Active.Units;
			}

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				size *= percents/100;
			}
			else
			{
				size += add*Modifier.fontSizeScale;
			}

			this.document.FontWrapper.SuspendSynchronisations();
			this.document.FontWrapper.Defined.FontSize = size;
			this.document.FontWrapper.Defined.Units = units;
			this.document.FontWrapper.ResumeSynchronisations();
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.fontColor.ActiveState = ActiveState.No;
		}

		// Sélectionne l'origine de couleur.
		public override void OriginColorSelect(int rank)
		{
			this.fontColor.ActiveState = ActiveState.Yes;
		}

		// Modifie la couleur d'origine.
		public override void OriginColorChange(Drawing.RichColor color)
		{
			if ( this.fontColor.Color != color )
			{
				this.fontColor.Color = color;
			}
		}

		// Donne la couleur d'origine.
		public override Drawing.RichColor OriginColorGet()
		{
			return this.fontColor.Color;
		}

		
		// Le wrapper associé a changé.
		protected override void HandleWrapperChanged(object sender)
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
				this.fontFace.SetVisible(true);
				r.Offset(0, -25);

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Right-(100+20);
					r.Width = 100;
					this.fontStyle.Bounds = r;
					this.fontStyle.SetVisible(true);
					r.Left = r.Right-1;
					r.Width = 20;
					this.fontFeatures.Bounds = r;
					this.fontFeatures.SetVisible(true);

					r.Offset(0, -25);
					r.Left = rect.Right-(50+20+20);
					r.Width = 50;
					this.fontSize.Bounds = r;
					this.fontSize.SetVisible(true);
					r.Offset(50, 0);
					r.Width = 20;
					this.buttonSizeMinus.Bounds = r;
					this.buttonSizeMinus.SetVisible(true);
					r.Offset(20, 0);
					this.buttonSizePlus.Bounds = r;
					this.buttonSizePlus.SetVisible(true);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonBold.Bounds = r;
					this.buttonBold.SetVisible(true);
					r.Offset(20, 0);
					this.buttonItalic.Bounds = r;
					this.buttonItalic.SetVisible(true);
					r.Offset(20, 0);
					this.buttonUnderlined.Bounds = r;
					this.buttonUnderlined.SetVisible(true);
					r.Offset(20, 0);
					this.buttonStrike.Bounds = r;
					this.buttonStrike.SetVisible(true);
					r.Offset(20, 0);
					this.buttonFrame.Bounds = r;
					this.buttonFrame.SetVisible(true);
					r.Offset(20+5, 0);
					this.buttonSubscript.Bounds = r;
					this.buttonSubscript.SetVisible(true);
					r.Offset(20, 0);
					this.buttonSuperscript.Bounds = r;
					this.buttonSuperscript.SetVisible(true);
					r.Offset(20+5, 0);
					r.Width = 30;
					this.fontColor.Bounds = r;
					this.fontColor.SetVisible(true);
				}
				else
				{
					r.Left = rect.Left;
					r.Right = rect.Right-(20+5+50+20+20);
					this.fontStyle.Bounds = r;
					this.fontStyle.SetVisible(true);
					r.Left = r.Right-1;
					r.Width = 20;
					this.fontFeatures.Bounds = r;
					this.fontFeatures.SetVisible(true);
					
					r.Left = rect.Right-(50+20+20);
					r.Width = 50;
					this.fontSize.Bounds = r;
					this.fontSize.SetVisible(true);
					r.Offset(50, 0);
					r.Width = 20;
					this.buttonSizeMinus.Bounds = r;
					this.buttonSizeMinus.SetVisible(true);
					r.Offset(20, 0);
					this.buttonSizePlus.Bounds = r;
					this.buttonSizePlus.SetVisible(true);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonBold.Bounds = r;
					this.buttonBold.SetVisible(true);
					r.Offset(20, 0);
					this.buttonItalic.Bounds = r;
					this.buttonItalic.SetVisible(true);
					r.Offset(20, 0);
					this.buttonUnderlined.Bounds = r;
					this.buttonUnderlined.SetVisible(true);
					r.Offset(20, 0);
					this.buttonStrike.Bounds = r;
					this.buttonStrike.SetVisible(true);
					r.Offset(20, 0);
					this.buttonFrame.Bounds = r;
					this.buttonFrame.SetVisible(true);
					r.Offset(20+5, 0);
					this.buttonSubscript.Bounds = r;
					this.buttonSubscript.SetVisible(true);
					r.Offset(20, 0);
					this.buttonSuperscript.Bounds = r;
					this.buttonSuperscript.SetVisible(true);
					r.Offset(20+5, 0);
					r.Width = 30;
					this.fontColor.Bounds = r;
					this.fontColor.SetVisible(true);
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				r.Right = rect.Right-55;
				this.fontFace.Bounds = r;
				this.fontFace.SetVisible(true);

				r.Left = rect.Right-50;
				r.Width = 50;
				this.fontSize.Bounds = r;
				this.fontSize.SetVisible(true);

				this.fontStyle.SetVisible(false);
				this.fontFeatures.SetVisible(false);
				this.fontColor.SetVisible(false);
				this.buttonSizeMinus.SetVisible(false);
				this.buttonSizePlus.SetVisible(false);
				this.buttonBold.SetVisible(false);
				this.buttonItalic.SetVisible(false);
				this.buttonUnderlined.SetVisible(false);
				this.buttonStrike.SetVisible(false);
				this.buttonFrame.SetVisible(false);
				this.buttonSubscript.SetVisible(false);
				this.buttonSuperscript.SetVisible(false);
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
			editObject.GetTextFont(true, out face, out style, out features);

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
			editObject.GetTextFont(false, out face, out style, out features);

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

			editObject.SetTextFont(face, style, newFeatures);
		}
		#endregion

		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			if ( sender == this.fontFace )
			{
				string face = this.fontFace.Text;
				this.document.FontWrapper.SuspendSynchronisations();
				this.document.FontWrapper.Defined.FontFace = face;
				this.document.FontWrapper.Defined.FontStyle = Misc.DefaultFontStyle(face);
				this.document.FontWrapper.ResumeSynchronisations();
			}

			if ( sender == this.fontStyle )
			{
				string style = this.fontStyle.Text;
				this.document.FontWrapper.Defined.FontStyle = style;
			}

			if ( sender == this.fontSize )
			{
				double size = 0;
				Text.Properties.SizeUnits units = Common.Text.Properties.SizeUnits.None;
				if ( this.fontSize.Text != Res.Strings.Action.Text.Font.Default )
				{

					Misc.ConvertStringToDouble(out size, out units, this.fontSize.Text, 0, 1000, 0);
					if ( units == Common.Text.Properties.SizeUnits.Points )
					{
						size *= Modifier.fontSizeScale;
					}
				}
				this.document.FontWrapper.SuspendSynchronisations();
				this.document.FontWrapper.Defined.FontSize = size;
				this.document.FontWrapper.Defined.Units = units;
				this.document.FontWrapper.ResumeSynchronisations();
			}
		}

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
		}

		private void HandleFieldColorChanged(object sender)
		{
			ColorSample cs = sender as ColorSample;
			if ( cs.ActiveState == ActiveState.Yes )
			{
			}
		}

		private void HandleButtonSizeMinusClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
			this.ChangeFontSize(-1, 80);
		}

		private void HandleButtonSizePlusClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
			this.ChangeFontSize(1, 125);
		}

		private void HandleButtonBoldClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
			this.document.FontWrapper.Defined.InvertBold = !this.document.FontWrapper.Defined.InvertBold;
		}

		private void HandleButtonItalicClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
			this.document.FontWrapper.Defined.InvertItalic = !this.document.FontWrapper.Defined.InvertItalic;
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
		}


		protected TextFieldCombo			fontFace;
		protected TextFieldCombo			fontStyle;
		protected IconButton				fontFeatures;
		protected TextFieldCombo			fontSize;
		protected ColorSample				fontColor;
		protected IconButton				buttonSizeMinus;
		protected IconButton				buttonSizePlus;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonStrike;
		protected IconButton				buttonFrame;
		protected IconButton				buttonSubscript;
		protected IconButton				buttonSuperscript;
	}
}
