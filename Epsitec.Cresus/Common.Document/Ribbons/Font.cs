using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Font permet de choisir la fonte.
	/// </summary>
	[SuppressBundleSupport]
	public class Font : Abstract
	{
		public Font() : base()
		{
			this.title.Text = Res.Strings.Property.Abstract.TextFont;

			this.fontFace  = this.CreateFieldFontFace(Res.Strings.Panel.Font.Tooltip.Name);
			this.fontStyle = this.CreateFieldFontStyle("Style de la police");
			this.fontSize  = this.CreateFieldFontSize(Res.Strings.Panel.Font.Tooltip.Size);

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleSource = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fontColor.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fontColor.TabIndex = this.tabIndex++;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, Res.Strings.Panel.Font.Tooltip.Color);

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

			this.buttonBullet1 = new IconButton(this);
			this.buttonBullet1.AutoFocus = false;
			this.buttonBullet1.Text = "a)";
			this.buttonBullet1.Clicked += new MessageEventHandler(this.HandleButtonBullet1Clicked);
			ToolTip.Default.SetToolTip(this.buttonBullet1, "Puces");

			this.buttonBullet2 = new IconButton(this);
			this.buttonBullet2.AutoFocus = false;
			this.buttonBullet2.Text = "1.";
			this.buttonBullet2.Clicked += new MessageEventHandler(this.HandleButtonBullet2Clicked);
			ToolTip.Default.SetToolTip(this.buttonBullet2, "Numérotation");
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
				return 8+180+5+50;
			}
		}


		// Effectue la mise à jour du contenu d'édition.
		protected override void DoUpdateText()
		{
			this.AdaptFieldFontFace(this.fontFace);
			this.AdaptFieldFontStyle(this.fontStyle);
			this.AdaptFieldFontSize(this.fontSize);

			Objects.Abstract editObject = this.EditObject;

			this.fontColor.SetEnabled(editObject != null);
			this.buttonBold.SetEnabled(editObject != null);
			this.buttonItalic.SetEnabled(editObject != null);
			this.buttonUnderlined.SetEnabled(editObject != null);
			this.buttonBullet1.SetEnabled(editObject != null);
			this.buttonBullet2.SetEnabled(editObject != null);

			bool bold = false;
			bool italic = false;
			bool underlined = false;
			bool bullet1 = false;
			bool bullet2 = false;

			if ( editObject != null )
			{
				bold = editObject.TextBold;
				italic = editObject.TextItalic;
				underlined = editObject.TextUnderlined;
				bullet1 = editObject.TextBullet1;
				bullet2 = editObject.TextBullet2;
			}

			this.buttonBold.ActiveState = bold ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonItalic.ActiveState = italic ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonUnderlined.ActiveState = underlined ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonBullet1.ActiveState = bullet1 ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonBullet2.ActiveState = bullet2 ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonBold == null )  return;

			double dx = this.buttonBold.DefaultHeight;
			double dy = this.buttonBold.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			rect.Width  = 180;
			this.fontFace.Bounds = rect;
			rect.Offset(rect.Width+5, 0);
			rect.Width  = 50;
			this.fontSize.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width  = 98;
			this.fontStyle.Bounds = rect;
			rect.Offset(rect.Width+5, 0);
			rect.Width  = dx;
			this.buttonBold.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonItalic.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUnderlined.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBullet1.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBullet2.Bounds = rect;
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
				editObject.GetTextFont(out face, out style);

				if ( field.Items.Count == 0 )
				{
					field.Items.Add("");  // case vide = "par défaut" TODO: faire mieux
					foreach( string f in this.document.TextContext.GetAvailableFontFaces() )
					{
						field.Items.Add(f);
					}
				}

				field.Text = face;

				this.ignoreChange = false;
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
				editObject.GetTextFont(out face, out style);

				field.Items.Clear();
				foreach ( OpenType.FontIdentity id in this.document.TextContext.GetAvailableFontIdentities(face) )
				{
					field.Items.Add(id.InvariantStyleName);
				}

				field.Text = style;

				this.ignoreChange = false;
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
				string style = this.SearchDefaultFontStyle(face);
				editObject.SetTextFont(face, style);
			}

			if ( field == this.fontStyle )
			{
				string face, style;
				editObject.GetTextFont(out face, out style);
				editObject.SetTextFont(face, field.Text);
			}
		}

		// Cherche le FontStyle par défaut pour un FontName donné.
		protected string SearchDefaultFontStyle(string face)
		{
			foreach ( OpenType.FontIdentity id in this.document.TextContext.GetAvailableFontIdentities(face) )
			{
				if ( id.FontWeight == OpenType.FontWeight.Normal &&
					 id.FontStyle  == OpenType.FontStyle.Normal  )
				{
					return id.InvariantStyleName;
				}
			}

			foreach ( OpenType.FontIdentity id in this.document.TextContext.GetAvailableFontIdentities(face) )
			{
				if ( id.FontWeight == OpenType.FontWeight.Normal )
				{
					return id.InvariantStyleName;
				}
			}

			foreach ( OpenType.FontIdentity id in this.document.TextContext.GetAvailableFontIdentities(face) )
			{
				if ( id.FontStyle == OpenType.FontStyle.Normal )
				{
					return id.InvariantStyleName;
				}
			}

			foreach ( OpenType.FontIdentity id in this.document.TextContext.GetAvailableFontIdentities(face) )
			{
				return id.InvariantStyleName;
			}

			return "";
		}


		// Crée un champ éditable pour la taille de la police.
		protected TextFieldReal CreateFieldFontSize(string tooltip)
		{
			TextFieldReal field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}

		// Adapte un champ éditable pour la taille de la police.
		protected void AdaptFieldFontSize(TextFieldReal field)
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
				this.document.Modifier.AdaptTextFieldRealFontSize(field);

				double size = editObject.TextFontSize;
				if ( size == 0 )
				{
					field.Text = "";
				}
				else
				{
					field.InternalValue = (decimal) size;
				}

				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;

			if ( field == this.fontSize )
			{
				if ( field.Text == "" )
				{
					editObject.TextFontSize = 0;
				}
				else
				{
					editObject.TextFontSize = (double) field.InternalValue;
				}
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
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;
			editObject.TextBold = !editObject.TextBold;
		}

		private void HandleButtonItalicClicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;
			editObject.TextItalic = !editObject.TextItalic;
		}

		private void HandleButtonUnderlinedClicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;
			editObject.TextUnderlined = !editObject.TextUnderlined;
		}

		private void HandleButtonBullet1Clicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;
			editObject.TextBullet1 = !editObject.TextBullet1;
		}

		private void HandleButtonBullet2Clicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;
			editObject.TextBullet2 = !editObject.TextBullet2;
		}


		// Donne l'objet en cours d'édition.
		protected Objects.Abstract EditObject
		{
			get
			{
				if ( this.document == null )  return null;
				return this.document.Modifier.RetEditObject();
			}
		}


		protected TextFieldCombo			fontFace;
		protected TextFieldCombo			fontStyle;
		protected TextFieldReal				fontSize;
		protected ColorSample				fontColor;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonBullet1;
		protected IconButton				buttonBullet2;
	}
}
