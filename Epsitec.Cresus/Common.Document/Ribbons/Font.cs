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

			this.fontName = this.CreateFieldName(Res.Strings.Panel.Font.Tooltip.Name);
			this.fontSize = this.CreateFieldSize(Res.Strings.Panel.Font.Tooltip.Size);

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
			this.buttonBullet1.Text = "o";
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

			this.AdaptFieldName(this.fontName);
			this.AdaptFieldSize(this.fontSize);
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
			Objects.Abstract editObject = null;

			if ( this.document != null )
			{
				editObject = this.document.Modifier.RetEditObject();
			}

			this.fontName.SetEnabled(editObject != null);
			this.fontSize.SetEnabled(editObject != null);
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
				bold = editObject.IsEditBold;
				italic = editObject.IsEditItalic;
				underlined = editObject.IsEditUnderlined;
				bullet1 = editObject.IsEditBullet1;
				bullet2 = editObject.IsEditBullet2;
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
			this.fontName.Bounds = rect;
			rect.Offset(rect.Width+5, 0);
			rect.Width  = 50;
			this.fontSize.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
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


		// Crée un champ éditable pour la police.
		protected TextFieldCombo CreateFieldName(string tooltip)
		{
			TextFieldCombo field = new TextFieldCombo(this);
			field.IsReadOnly = true;
			field.TextChanged += new EventHandler(this.HandleFieldComboChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}
		
		// Adapte un champ éditable pour la police.
		protected void AdaptFieldName(TextFieldCombo field)
		{
			if ( this.document == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				if ( field.Items.Count == 0 )
				{
					if ( this.documentType == DocumentType.Pictogram )
					{
						field.Items.Add("Tahoma");
						field.Items.Add("Arial");
						field.Items.Add("Courier New");
						field.Items.Add("Times New Roman");
					}
					else
					{
						Misc.AddFontList(field, false);
					}
				}
				this.ignoreChange = false;
			}
		}

		// Un champ a été changé.
		private void HandleFieldComboChanged(object sender)
		{
		}


		// Crée un champ éditable pour la taille.
		protected TextFieldReal CreateFieldSize(string tooltip)
		{
			TextFieldReal field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}

		// Adapte un champ éditable pour la taille.
		protected void AdaptFieldSize(TextFieldReal field)
		{
			if ( this.document == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				this.document.Modifier.AdaptTextFieldRealFontSize(field);
				//?field.InternalValue = (decimal) this.document.Modifier.RotateAngle;
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == this.fontSize )
			{
				//?this.document.Modifier.RotateAngle = (double) field.InternalValue;
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
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			editObject.EditBold();
		}

		private void HandleButtonItalicClicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			editObject.EditItalic();
		}

		private void HandleButtonUnderlinedClicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			editObject.EditUnderlined();
		}

		private void HandleButtonBullet1Clicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			editObject.EditBullet1();
		}

		private void HandleButtonBullet2Clicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			editObject.EditBullet2();
		}


		protected TextFieldCombo			fontName;
		protected TextFieldReal				fontSize;
		protected ColorSample				fontColor;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonBullet1;
		protected IconButton				buttonBullet2;
	}
}
