using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelFont permet de choisir une police de caractères.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelFont : AbstractPanel
	{
		public PanelFont(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.fontName = new TextFieldCombo(this);
			this.fontName.IsReadOnly = true;
			this.fontName.Items.Add("Tahoma");
			this.fontName.Items.Add("Arial");
			this.fontName.Items.Add("Courier New");
			this.fontName.Items.Add("Times New Roman");
			this.fontName.SelectedIndexChanged += new EventHandler(this.HandleFieldChanged);
			this.fontName.TabIndex = 1;
			this.fontName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontName, "Police du texte par défaut");

			this.fontSize = new TextFieldSlider(this);
			this.fontSize.MinValue =  0.1M;
			this.fontSize.MaxValue = 20.0M;
			this.fontSize.Step = 0.1M;
			this.fontSize.Resolution = 0.01M;
			this.fontSize.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontSize.TabIndex = 3;
			this.fontSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontSize, "Taille du texte par défaut");

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleSource = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleColorClicked);
			this.fontColor.TabIndex = 4;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, "Couleur du texte par défaut");

			this.labelSize = new StaticText(this);
			this.labelSize.Text = "Taille";
			this.labelSize.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.labelColor = new StaticText(this);
			this.labelColor.Text = "Couleur";
			this.labelColor.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fontName.SelectedIndexChanged -= new EventHandler(this.HandleFieldChanged);
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
				return ( this.extendedSize ? 80 : 30 );
			}
		}

		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyFont p = property as PropertyFont;
			if ( p == null )  return;

			//?this.fontName.SelectedName = p.FontName;
			this.ComboSelectedName(this.fontName, p.FontName);
			this.fontSize.Value = (decimal) p.FontSize;
			this.fontColor.Color = p.FontColor;
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyFont p = new PropertyFont();
			base.GetProperty(p);

			//?string name = this.fontName.SelectedName;
			string name = this.ComboSelectedName(this.fontName);
			if ( name == null )  p.FontName = "";
			else                 p.FontName = name;

			p.FontSize = (double) this.fontSize.Value;
			p.FontColor = this.fontColor.Color;

			return p;
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

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
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
			this.labelSize.SetVisible(this.extendedSize);
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fontSize.Bounds = r;
			this.fontSize.SetVisible(this.extendedSize);

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Right = rect.Right-50;
			this.labelColor.Bounds = r;
			this.labelColor.SetVisible(this.extendedSize);
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fontColor.Bounds = r;
			this.fontColor.SetVisible(this.extendedSize);
		}


		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
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
		protected TextFieldSlider			fontSize;
		protected ColorSample				fontColor;
	}
}
