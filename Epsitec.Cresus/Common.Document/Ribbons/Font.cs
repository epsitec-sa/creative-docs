using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Font permet de choisir la fonte du texte.
	/// </summary>
	[SuppressBundleSupport]
	public class Font : Abstract
	{
		public Font() : base()
		{
			this.title.Text = Res.Strings.Action.Text.Font.Main;

			this.fontFace  = this.CreateFieldFontFace(Res.Strings.Action.Text.Font.Face);
			this.fontStyle = this.CreateFieldFontStyle(Res.Strings.Action.Text.Font.Style);
			this.fontFeatures = this.CreateIconButton(Misc.Icon("FontFeatures"), Res.Strings.Action.Text.Font.Features, new MessageEventHandler(this.HandleFeaturesClicked));
			this.fontSize  = this.CreateFieldFontSize(Res.Strings.Action.Text.Font.Size);

			this.buttonSizePlus  = this.CreateIconButton(Misc.Icon("FontSizePlus"),  Res.Strings.Action.Text.Font.SizePlus,  new MessageEventHandler(this.HandleButtonClicked), false);
			this.buttonSizeMinus = this.CreateIconButton(Misc.Icon("FontSizeMinus"), Res.Strings.Action.Text.Font.SizeMinus, new MessageEventHandler(this.HandleButtonClicked), false);

			this.buttonBold        = this.CreateIconButton(Res.Strings.Text.ButtonBold,       Res.Strings.Action.Text.Font.Bold,        new MessageEventHandler(this.HandleButtonBoldClicked));
			this.buttonItalic      = this.CreateIconButton(Res.Strings.Text.ButtonItalic,     Res.Strings.Action.Text.Font.Italic,      new MessageEventHandler(this.HandleButtonItalicClicked));
			this.buttonUnderlined  = this.CreateIconButton(Res.Strings.Text.ButtonUnderlined, Res.Strings.Action.Text.Font.Underlined,  new MessageEventHandler(this.HandleButtonClicked));
			this.buttonSubscript   = this.CreateIconButton(Misc.Icon("FontSubscript"),        Res.Strings.Action.Text.Font.Subscript,   new MessageEventHandler(this.HandleButtonClicked));
			this.buttonSuperscript = this.CreateIconButton(Misc.Icon("FontSuperscript"),      Res.Strings.Action.Text.Font.Superscript, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonUserX       = this.CreateIconButton(Res.Strings.Text.ButtonUserX,      Res.Strings.Action.Text.Font.UserX,       new MessageEventHandler(this.HandleButtonClicked));
			this.buttonUserY       = this.CreateIconButton(Res.Strings.Text.ButtonUserY,      Res.Strings.Action.Text.Font.UserY,       new MessageEventHandler(this.HandleButtonClicked));
			this.buttonUserZ       = this.CreateIconButton(Res.Strings.Text.ButtonUserZ,      Res.Strings.Action.Text.Font.UserZ,       new MessageEventHandler(this.HandleButtonClicked));

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleSource = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fontColor.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fontColor.TabIndex = this.tabIndex++;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, Res.Strings.Action.Text.Font.Color);

			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, Settings.GlobalSettings gs, Document document)
		{
			if ( this.document != null )
			{
				this.document.FontWrapper.Active.Changed -= new EventHandler(this.HandleWrapperChanged);
				this.document.FontWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}

			base.SetDocument(type, install, gs, document);

			if ( this.document != null )
			{
				this.document.FontWrapper.Active.Changed += new EventHandler(this.HandleWrapperChanged);
				this.document.FontWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);
			}

			this.HandleWrapperChanged(null);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8+180+5+50+22*2+25;
			}
		}


		protected void UpdateButtonBold()
		{
			bool enabled = false;
			bool state   = false;

			if ( this.document.FontWrapper.IsAttached )
			{
				enabled = true;
				state   = this.document.FontWrapper.Defined.InvertBold;
			}

			this.buttonBold.SetEnabled(enabled);
			this.buttonBold.ActiveState = state ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		protected void UpdateButtonItalic()
		{
			bool enabled = false;
			bool state   = false;

			if ( this.document.FontWrapper.IsAttached )
			{
				enabled = true;
				state   = this.document.FontWrapper.Defined.InvertItalic;
			}

			this.buttonItalic.SetEnabled(enabled);
			this.buttonItalic.ActiveState = state ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonBold == null )  return;

			double dx = this.buttonBold.DefaultWidth;
			double dy = this.buttonBold.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			rect.Width = 180;
			this.fontFace.Bounds = rect;
			rect.Offset(rect.Width+5, 0);
			rect.Width = 50;
			this.fontSize.Bounds = rect;
			rect.Offset(rect.Width, 0);
			rect.Width = dx;
			this.buttonSizePlus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSizeMinus.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			this.buttonBold.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonItalic.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUnderlined.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSubscript.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSuperscript.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUserX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUserY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUserZ.Bounds = rect;
			rect.Offset(dx, 0);
			this.fontColor.Bounds = rect;
			rect.Offset(dx+5, 0);
			rect.Width = 76;
			this.fontStyle.Bounds = rect;
			rect.Offset(rect.Width, 0);
			rect.Width = dx;
			this.fontFeatures.Bounds = rect;
		}


		// Crée un champ éditable pour le nom de la police.
		protected TextFieldCombo CreateFieldFontFace(string tooltip)
		{
			TextFieldCombo field = new TextFieldCombo(this);
			field.IsReadOnly = true;
			field.TextChanged += new EventHandler(this.HandleFieldComboChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}
		
		// Adapte un champ éditable pour le nom de la police.
		protected void AdaptFieldFontFace(TextFieldCombo field)
		{
			if ( this.document.FontWrapper.IsAttached )
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				
				this.UpdateFieldFontFaceList(field);

				string face = this.document.FontWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.document.FontWrapper.Active.FontFace;
					if ( face == null )
					{
						face = Res.Strings.Action.Text.Font.Default;
					}
					face = Misc.Italic(face);
				}
				field.Text = face;

				this.ignoreChange = false;
			}
			else
			{
				field.SetEnabled(false);
			}
		}

		// Met à jour la liste d'un champ éditable pour le nom de la police.
		protected void UpdateFieldFontFaceList(TextFieldCombo field)
		{
			if ( field.Items.Count == 0 )
			{
				field.Items.Add(Res.Strings.Action.Text.Font.Default);  // par défaut
				Misc.AddFontList(field, false);
			}
		}

		// Crée un champ éditable pour le style de la police.
		protected TextFieldCombo CreateFieldFontStyle(string tooltip)
		{
			TextFieldCombo field = new TextFieldCombo(this);
			field.IsReadOnly = true;
			field.TextChanged += new EventHandler(this.HandleFieldComboChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}
		
		// Adapte un champ éditable pour le style de la police.
		protected void AdaptFieldFontStyle(TextFieldCombo field)
		{
			if ( this.document.FontWrapper.IsAttached )
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				
				string face = this.document.FontWrapper.Defined.FontFace;
				if ( face == null )
				{
					face = this.document.FontWrapper.Active.FontFace;
				}
				this.UpdateFieldFontStyleList(field, face);

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
				field.Text = style;

				this.ignoreChange = false;
			}
			else
			{
				field.SetEnabled(false);
			}
		}

		// Met à jour la liste d'un champ éditable pour le style de la police.
		protected void UpdateFieldFontStyleList(TextFieldCombo field, string face)
		{
			field.Items.Clear();  // vide la liste
			if ( face == null )  return;

			OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(face);
			foreach ( OpenType.FontIdentity id in list )
			{
				field.Items.Add(id.InvariantStyleName);
			}
		}

		// Crée un champ éditable pour la taille de la police.
		protected TextFieldCombo CreateFieldFontSize(string tooltip)
		{
			TextFieldCombo field = new TextFieldCombo(this);
			field.TextChanged += new EventHandler(this.HandleFieldComboChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}

		// Adapte un champ éditable pour la taille de la police.
		protected void AdaptFieldFontSize(TextFieldCombo field)
		{
			if ( this.document.FontWrapper.IsAttached )
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				
				this.UpdateFieldFontSizeList(field);

				double size = this.document.FontWrapper.Defined.FontSize;
				Text.Properties.SizeUnits units = this.document.FontWrapper.Defined.Units;
				if ( size == double.NaN )
				{
					size = this.document.FontWrapper.Active.FontSize;
					units = this.document.FontWrapper.Active.Units;
					if ( size == double.NaN )
					{
						size = 0;
						units = Common.Text.Properties.SizeUnits.Points;
					}
				}

				if ( units == Common.Text.Properties.SizeUnits.Points )
				{
					size /= Modifier.fontSizeScale;
				}
				field.Text = Misc.ConvertDoubleToString(size, units, 0);

				this.ignoreChange = false;
			}
			else
			{
				field.SetEnabled(false);
			}
		}

		// Met à jour la liste d'un champ éditable pour la taille de la police.
		protected void UpdateFieldFontSizeList(TextFieldCombo field)
		{
			if ( field.Items.Count == 0 )
			{
				field.Items.Add(Res.Strings.Action.Text.Font.Default);  // par défaut
				field.Items.Add("\u2015\u2015\u2015\u2015");
				field.Items.Add("50%");
				field.Items.Add("75%");
				field.Items.Add("150%");
				field.Items.Add("200%");
				field.Items.Add("\u2015\u2015\u2015\u2015");
				field.Items.Add("8");
				field.Items.Add("9");
				field.Items.Add("10");
				field.Items.Add("11");
				field.Items.Add("12");
				field.Items.Add("14");
				field.Items.Add("16");
				field.Items.Add("18");
				field.Items.Add("20");
				field.Items.Add("24");
				field.Items.Add("36");
				field.Items.Add("48");
				field.Items.Add("72");
			}
		}

		// Le wrapper associé a changé.
		private void HandleWrapperChanged(object sender)
		{
			this.AdaptFieldFontFace(this.fontFace);
			this.AdaptFieldFontStyle(this.fontStyle);
			this.AdaptFieldFontSize(this.fontSize);
			this.UpdateButtonBold();
			this.UpdateButtonItalic();
		}

		// Un champ combo a été changé.
		private void HandleFieldComboChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextFieldCombo field = sender as TextFieldCombo;
			if ( field == null )  return;

			if ( !this.document.FontWrapper.IsAttached )  return;

			if ( field == this.fontFace )
			{
				string face = field.Text;
				this.document.FontWrapper.Defined.FontFace = face;
			}

			if ( field == this.fontStyle )
			{
				string style = field.Text;
				this.document.FontWrapper.Defined.FontStyle = style;
			}

			if ( field == this.fontSize )
			{
				if ( field.Text.StartsWith("\u2015") )  // sur un "séparateur" ?
				{
					this.ignoreChange = true;
					field.Text = "";
					this.ignoreChange = false;
					return;
				}

				double size = 0;
				Text.Properties.SizeUnits units = Common.Text.Properties.SizeUnits.None;
				if ( field.Text != Res.Strings.Action.Text.Font.Default )
				{

					Misc.ConvertStringToDouble(out size, out units, field.Text, 0, 1000, 0);
					if ( units == Common.Text.Properties.SizeUnits.Points )
					{
						size *= Modifier.fontSizeScale;
					}
				}
				this.document.FontWrapper.Defined.FontSize = size;
				this.document.FontWrapper.Defined.Units = units;
			}
		}


		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}

		private void HandleFieldColorChanged(object sender)
		{
			ColorSample cs = sender as ColorSample;
			if ( cs.ActiveState == WidgetState.ActiveYes )
			{
				this.OnOriginColorChanged();
			}
		}

		private void HandleButtonBoldClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
			bool state = (this.buttonBold.ActiveState == WidgetState.ActiveYes);
			this.document.FontWrapper.Defined.InvertBold = !state;
		}

		private void HandleButtonItalicClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
			bool state = (this.buttonItalic.ActiveState == WidgetState.ActiveYes);
			this.document.FontWrapper.Defined.InvertItalic = !state;
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if ( !this.document.FontWrapper.IsAttached )  return;
		}


		#region FeaturesMenu
		// Appelé lors du clic sur le bouton "OpenType" pour ouvrir le menu.
		private void HandleFeaturesClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			if ( button == null )  return;

			Objects.Abstract editObject = this.EditObject;
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
			int total = this.globalSettings.LastFilenameCount;
			if ( total == 0 )  return null;

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

			Objects.Abstract editObject = this.EditObject;
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


		protected TextFieldCombo			fontFace;
		protected TextFieldCombo			fontStyle;
		protected IconButton				fontFeatures;
		protected TextFieldCombo			fontSize;
		protected IconButton				buttonSizePlus;
		protected IconButton				buttonSizeMinus;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonSubscript;
		protected IconButton				buttonSuperscript;
		protected IconButton				buttonUserX;
		protected IconButton				buttonUserY;
		protected IconButton				buttonUserZ;
		protected ColorSample				fontColor;
	}
}
