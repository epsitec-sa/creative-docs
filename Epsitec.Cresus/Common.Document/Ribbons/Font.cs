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
			this.fontSize  = this.CreateFieldFontSize(Res.Strings.Action.Text.Font.Size);

			this.buttonSizePlus  = this.CreateIconButton(Misc.Icon("FontSizePlus"),  Res.Strings.Action.Text.Font.SizePlus,  new MessageEventHandler(this.HandleButtonClicked), false);
			this.buttonSizeMinus = this.CreateIconButton(Misc.Icon("FontSizeMinus"), Res.Strings.Action.Text.Font.SizeMinus, new MessageEventHandler(this.HandleButtonClicked), false);

			this.buttonBold       = this.CreateIconButton(Res.Strings.Text.ButtonBold,       Res.Strings.Action.Text.Font.Bold,       new MessageEventHandler(this.HandleButtonClicked));
			this.buttonItalic     = this.CreateIconButton(Res.Strings.Text.ButtonItalic,     Res.Strings.Action.Text.Font.Italic,     new MessageEventHandler(this.HandleButtonClicked));
			this.buttonUnderlined = this.CreateIconButton(Res.Strings.Text.ButtonUnderlined, Res.Strings.Action.Text.Font.Underlined, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonIndice     = this.CreateIconButton(Misc.Icon("FontIndice"),           Res.Strings.Action.Text.Font.Indice,     new MessageEventHandler(this.HandleButtonClicked));
			this.buttonExposant   = this.CreateIconButton(Misc.Icon("FontExposant"),         Res.Strings.Action.Text.Font.Exposant,   new MessageEventHandler(this.HandleButtonClicked));
			this.buttonUserX      = this.CreateIconButton(Res.Strings.Text.ButtonUserX,      Res.Strings.Action.Text.Font.UserX,      new MessageEventHandler(this.HandleButtonClicked));
			this.buttonUserY      = this.CreateIconButton(Res.Strings.Text.ButtonUserY,      Res.Strings.Action.Text.Font.UserY,      new MessageEventHandler(this.HandleButtonClicked));
			this.buttonUserZ      = this.CreateIconButton(Res.Strings.Text.ButtonUserZ,      Res.Strings.Action.Text.Font.UserZ,      new MessageEventHandler(this.HandleButtonClicked));

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleSource = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fontColor.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fontColor.TabIndex = this.tabIndex++;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, Res.Strings.Action.Text.Font.Color);
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
			base.SetDocument(type, install, gs, document);

			this.AdaptFieldFontFace(this.fontFace);
			this.AdaptFieldFontStyle(this.fontStyle);
			this.AdaptFieldFontSize(this.fontSize);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8+180+5+50+22*2;
			}
		}


		// Effectue la mise à jour du contenu d'édition.
		protected override void DoUpdateText()
		{
			this.AdaptFieldFontFace(this.fontFace);
			this.AdaptFieldFontStyle(this.fontStyle);
			this.AdaptFieldFontSize(this.fontSize);

			Objects.Abstract editObject = this.EditObject;

			this.buttonSizePlus.SetEnabled(editObject != null);
			this.buttonSizeMinus.SetEnabled(editObject != null);
			this.fontColor.SetEnabled(editObject != null);

			this.UpdateButton(this.buttonBold, editObject, "Bold");
			this.UpdateButton(this.buttonItalic, editObject, "Italic");
			this.UpdateButton(this.buttonUnderlined, editObject, "Underlined");
			this.UpdateButton(this.buttonIndice, editObject, "Indice");
			this.UpdateButton(this.buttonExposant, editObject, "Exposant");
			this.UpdateButton(this.buttonUserX, editObject, "UserX");
			this.UpdateButton(this.buttonUserY, editObject, "UserY");
			this.UpdateButton(this.buttonUserZ, editObject, "UserZ");
		}

		protected void UpdateButton(IconButton button, Objects.Abstract editObject, string name)
		{
			button.SetEnabled(editObject != null);

			bool state = false;

			if ( editObject != null )
			{
				state = editObject.GetTextStyle(name);;
			}

			button.ActiveState = state ? WidgetState.ActiveYes : WidgetState.ActiveNo;
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
			rect.Width  = 180;
			this.fontFace.Bounds = rect;
			rect.Offset(rect.Width+5, 0);
			rect.Width  = 50;
			this.fontSize.Bounds = rect;
			rect.Offset(rect.Width, 0);
			rect.Width  = dx;
			this.buttonSizePlus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSizeMinus.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width  = 76;
			this.fontStyle.Bounds = rect;
			rect.Offset(rect.Width+5, 0);
			rect.Width  = dx;
			this.buttonBold.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonItalic.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUnderlined.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonIndice.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonExposant.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUserX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUserY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUserZ.Bounds = rect;
			rect.Offset(dx, 0);
			this.fontColor.Bounds = rect;
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
			Objects.Abstract editObject = this.EditObject;

			if ( editObject == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				
				string face, style;
				editObject.GetTextFont(false, out face, out style);
				this.UpdateFieldFontFaceList(field);
				if ( face == "" )  face = Res.Strings.Action.Text.Font.Default;
				field.Text = face;

				this.ignoreChange = false;
			}
		}

		// Met à jour la liste d'un champ éditable pour le nom de la police.
		protected void UpdateFieldFontFaceList(TextFieldCombo field)
		{
			if ( field.Items.Count == 0 )
			{
				field.Items.Add(Res.Strings.Action.Text.Font.Default);  // par défaut

				foreach( string face in this.document.TextContext.GetAvailableFontFaces() )
				{
					field.Items.Add(face);
				}
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
			Objects.Abstract editObject = this.EditObject;

			if ( editObject == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;

				string face, style;
				editObject.GetTextFont(false, out face, out style);
				this.UpdateFieldFontStyleList(field, face);
				if ( style == "" )  style = Res.Strings.Action.Text.Font.Default;
				field.Text = style;

				this.ignoreChange = false;
			}
		}

		// Met à jour la liste d'un champ éditable pour le style de la police.
		protected void UpdateFieldFontStyleList(TextFieldCombo field, string face)
		{
			field.Items.Clear();  // vide la liste

			OpenType.FontIdentity[] list = this.document.TextContext.GetAvailableFontIdentities(face);
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
			Objects.Abstract editObject = this.EditObject;

			if ( editObject == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				
				this.UpdateFieldFontSizeList(field);

				double size;
				Text.Properties.SizeUnits units;
				editObject.GetTextFontSize(out size, out units, false);
				if ( units == Common.Text.Properties.SizeUnits.None )
				{
					field.Text = Res.Strings.Action.Text.Font.Default;
				}
				else
				{
					if ( units == Common.Text.Properties.SizeUnits.Points )
					{
						size /= Modifier.fontSizeScale;
					}
					field.Text = Misc.ConvertDoubleToString(size, units, 0);
				}

				this.ignoreChange = false;
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

		// Un champ combo a été changé.
		private void HandleFieldComboChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextFieldCombo field = sender as TextFieldCombo;
			if ( field == null )  return;

			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;

			if ( field == this.fontFace )
			{
				string face = field.Text;
				string style = "";
				if ( face == Res.Strings.Action.Text.Font.Default )
				{
					face = "";
				}
				else
				{
					style = this.document.SearchDefaultFontStyle(face);
				}
				editObject.SetTextFont(face, style);
			}

			if ( field == this.fontStyle )
			{
				string face, style;
				editObject.GetTextFont(false, out face, out style);
				style = "";
				if ( field.Text != Res.Strings.Action.Text.Font.Default )
				{
					style = field.Text;
				}
				editObject.SetTextFont(face, style);
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
				editObject.SetTextFontSize(size, units, false);
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

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;

			if ( sender == this.buttonBold       )  this.InvertStyle(editObject, "Bold");
			if ( sender == this.buttonItalic     )  this.InvertStyle(editObject, "Italic");
			if ( sender == this.buttonUnderlined )  this.InvertStyle(editObject, "Underlined");
			if ( sender == this.buttonIndice     )  this.InvertStyle(editObject, "Indice");
			if ( sender == this.buttonExposant   )  this.InvertStyle(editObject, "Exposant");
			if ( sender == this.buttonUserX      )  this.InvertStyle(editObject, "UserX");
			if ( sender == this.buttonUserY      )  this.InvertStyle(editObject, "UserY");
			if ( sender == this.buttonUserZ      )  this.InvertStyle(editObject, "UserZ");
			if ( sender == this.buttonSizePlus   )  this.ChangeFontSize(editObject,  1, 125);
			if ( sender == this.buttonSizeMinus  )  this.ChangeFontSize(editObject, -1,  80);
		}

		protected void InvertStyle(Objects.Abstract editObject, string name)
		{
			bool state = editObject.GetTextStyle(name);
			editObject.SetTextStyle(name, !state);
		}

		protected void ChangeFontSize(Objects.Abstract editObject, double add, double percents)
		{
			double size;
			Text.Properties.SizeUnits units;
			editObject.GetTextFontSize(out size, out units, false);

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				editObject.SetTextFontSize(percents, Common.Text.Properties.SizeUnits.Percent, true);
			}
			else
			{
				editObject.GetTextFontSize(out size, out units, true);
				size += add*Modifier.fontSizeScale;
				editObject.SetTextFontSize(size, Common.Text.Properties.SizeUnits.Points, false);
			}
		}


		protected TextFieldCombo			fontFace;
		protected TextFieldCombo			fontStyle;
		protected TextFieldCombo			fontSize;
		protected IconButton				buttonSizePlus;
		protected IconButton				buttonSizeMinus;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonIndice;
		protected IconButton				buttonExposant;
		protected IconButton				buttonUserX;
		protected IconButton				buttonUserY;
		protected IconButton				buttonUserZ;
		protected ColorSample				fontColor;
	}
}
