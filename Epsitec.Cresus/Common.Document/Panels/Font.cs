using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Font permet de choisir une police de caractères.
	/// </summary>
	[SuppressBundleSupport]
	public class Font : Abstract
	{
		public Font(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.fontName = new TextFieldCombo(this);
			this.fontName.IsReadOnly = true;
			this.fontName.Items.Add("Tahoma");
			this.fontName.Items.Add("Arial");
			this.fontName.Items.Add("Courier New");
			this.fontName.Items.Add("Times New Roman");
			//?this.fontName.SelectedIndexChanged += new EventHandler(this.HandleFieldChanged);
			this.fontName.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontName.TabIndex = 1;
			this.fontName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontName, "Police du texte par défaut");

			this.fontSize = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fontSize);
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.fontSize.InternalMinValue =  0.1M;
				this.fontSize.InternalMaxValue = 20.0M;
				this.fontSize.Step = 0.1M;
				this.fontSize.Resolution = 0.01M;
			}
			else
			{
				this.fontSize.InternalMinValue =  1.0M;
				this.fontSize.InternalMaxValue = 200.0M;
				this.fontSize.Step = 5.0M;
				this.fontSize.Resolution = 0.1M;
			}
			this.fontSize.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontSize.TabIndex = 3;
			this.fontSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontSize, "Taille du texte par défaut");

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleOrigin = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleColorClicked);
			this.fontColor.TabIndex = 4;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, "Couleur du texte par défaut");

			this.labelSize = new StaticText(this);
			this.labelSize.Text = "Taille";
			this.labelSize.Alignment = ContentAlignment.MiddleLeft;

			this.labelColor = new StaticText(this);
			this.labelColor.Text = "Couleur";
			this.labelColor.Alignment = ContentAlignment.MiddleLeft;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				//?this.fontName.SelectedIndexChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontName.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontSize.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontColor.Clicked -= new MessageEventHandler(this.HandleColorClicked);

				this.label = null;
				this.labelSize = null;
				this.labelColor = null;
				this.fontName = null;
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
				return ( this.isExtendedSize ? 80 : 30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Font p = this.property as Properties.Font;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			//?this.fontName.SelectedName = p.FontName;
			this.ComboSelectedName(this.fontName, p.FontName);
			this.fontSize.InternalValue = (decimal) p.FontSize;
			this.fontColor.Color = p.FontColor;

			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Font p = this.property as Properties.Font;
			if ( p == null )  return;

			//?string name = this.fontName.SelectedName;
			string name = this.ComboSelectedName(this.fontName);
			if ( name == null )  p.FontName = "";
			else                 p.FontName = name;

			p.FontSize = (double) this.fontSize.InternalValue;
			p.FontColor = this.fontColor.Color;
		}


		// TODO: TextFieldCombo.SelectedName ne marche pas !!!
		protected void ComboSelectedName(TextFieldCombo combo, string name)
		{
			int total = combo.Items.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				string s = combo.Items[i];
				if ( s == name )
				{
					combo.SelectedIndex = i;
					return;
				}
			}
		}

		// TODO: TextFieldCombo.SelectedName ne marche pas !!!
		protected string ComboSelectedName(TextFieldCombo combo)
		{
			if ( combo.SelectedIndex == -1 )  return "";
			return combo.Items[combo.SelectedIndex] as string;
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.fontColor.ActiveState = WidgetState.ActiveNo;
		}

		// Sélectionne l'origine de couleur.
		public override void OriginColorSelect(int rank)
		{
			this.fontColor.ActiveState = WidgetState.ActiveYes;
		}

		// Modifie la couleur d'origine.
		public override void OriginColorChange(Drawing.Color color)
		{
			this.fontColor.Color = color;
		}

		// Donne la couleur d'origine.
		public override Drawing.Color OriginColorGet()
		{
			return this.fontColor.Color;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fontName == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right-110;
			this.label.Bounds = r;
			r.Left = rect.Right-110;
			r.Right = rect.Right;
			this.fontName.Bounds = r;

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Right = rect.Right-50;
			this.labelSize.Bounds = r;
			this.labelSize.SetVisible(this.isExtendedSize);
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fontSize.Bounds = r;
			this.fontSize.SetVisible(this.isExtendedSize);

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Right = rect.Right-50;
			this.labelColor.Bounds = r;
			this.labelColor.SetVisible(this.isExtendedSize);
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fontColor.Bounds = r;
			this.fontColor.SetVisible(this.isExtendedSize);
		}


		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleColorClicked(object sender, MessageEventArgs e)
		{
			this.OnOriginColorChanged();
		}


		protected StaticText				label;
		protected StaticText				labelSize;
		protected StaticText				labelColor;
		protected TextFieldCombo			fontName;
		protected TextFieldReal				fontSize;
		protected ColorSample				fontColor;
	}
}
