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
			this.fontName = new TextFieldCombo(this);
			this.fontName.IsReadOnly = true;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.fontName.Items.Add("Tahoma");
				this.fontName.Items.Add("Arial");
				this.fontName.Items.Add("Courier New");
				this.fontName.Items.Add("Times New Roman");
			}
			else
			{
				Misc.AddFontList(this.document, this.fontName);
			}
			//?this.fontName.SelectedIndexChanged += new EventHandler(this.HandleFieldChanged);
			this.fontName.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontName.TabIndex = 1;
			this.fontName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontName, Res.Strings.Panel.Font.Tooltip.Name);

			this.fontSize = new Widgets.TextFieldLabel(this, false);
			this.fontSize.LabelShortText = Res.Strings.Panel.Font.Short.Size;
			this.fontSize.LabelLongText  = Res.Strings.Panel.Font.Long.Size;
			this.document.Modifier.AdaptTextFieldRealFontSize(this.fontSize.TextFieldReal);
			this.fontSize.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fontSize.TabIndex = 3;
			this.fontSize.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontSize, Res.Strings.Panel.Font.Tooltip.Size);

			this.fontColor = new ColorSample(this);
			this.fontColor.PossibleSource = true;
			this.fontColor.Clicked += new MessageEventHandler(this.HandleFieldColorClicked);
			this.fontColor.Changed += new EventHandler(this.HandleFieldColorChanged);
			this.fontColor.TabIndex = 4;
			this.fontColor.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, Res.Strings.Panel.Font.Tooltip.Color);

			this.labelColor = new StaticText(this);
			this.labelColor.Alignment = ContentAlignment.MiddleRight;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				//?this.fontName.SelectedIndexChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontName.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontSize.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontColor.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fontColor.Changed -= new EventHandler(this.HandleFieldColorChanged);

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
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 80;
					}
					else	// étendu/compact ?
					{
						h += 55;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Font p = this.property as Properties.Font;
			if ( p == null )  return;

			this.ignoreChanged = true;

			//?this.fontName.SelectedName = p.FontName;
			this.ComboSelectedName(this.fontName, p.FontName);
			this.fontSize.TextFieldReal.InternalValue = (decimal) p.FontSize;
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

			p.FontSize = (double) this.fontSize.TextFieldReal.InternalValue;
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
		public override void OriginColorChange(Drawing.RichColor color)
		{
			if ( this.fontColor.Color != color )
			{
				this.fontColor.Color = color;
				this.OnChanged();
			}
		}

		// Donne la couleur d'origine.
		public override Drawing.RichColor OriginColorGet()
		{
			return this.fontColor.Color;
		}

		
		// Adapte les textes courts ou longs.
		protected void UpdateShortLongText()
		{
			if ( this.IsLabelProperties )
			{
				this.labelColor.Text = Res.Strings.Panel.Font.Long.Color + " ";
			}
			else
			{
				this.labelColor.Text = Res.Strings.Panel.Font.Short.Color;
			}
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fontName == null )  return;

			this.UpdateShortLongText();

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				this.fontName.Bounds = r;
				this.fontName.SetVisible(true);

				if ( this.IsLabelProperties )
				{
					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fontSize.LabelVisibility = true;
					this.fontSize.Bounds = r;
					this.fontSize.SetVisible(true);

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-Widgets.TextFieldLabel.DefaultMarginWidth;
					this.labelColor.Bounds = r;
					this.labelColor.SetVisible(true);
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					r.Right = rect.Right;
					this.fontColor.Bounds = r;
					this.fontColor.SetVisible(true);
				}
				else
				{
					r.Offset(0, -25);
					r.Left = rect.Right-this.fontSize.Width-Widgets.TextFieldLabel.ShortWidth;
					r.Width = this.fontSize.Width;
					this.fontSize.LabelVisibility = true;
					this.fontSize.Bounds = r;
					this.fontSize.SetVisible(true);
					r.Left = r.Right;
					r.Width = Widgets.TextFieldLabel.DefaultLabelWidth;
					this.labelColor.Bounds = r;
					this.labelColor.SetVisible(true);
					r.Left = r.Right+Widgets.TextFieldLabel.DefaultMarginWidth;
					r.Width = Widgets.TextFieldLabel.DefaultTextWidth;
					this.fontColor.Bounds = r;
					this.fontColor.SetVisible(true);
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-5;
				this.fontName.Bounds = r;
				this.fontName.SetVisible(true);

				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fontSize.LabelVisibility = false;
				this.fontSize.Bounds = r;
				this.fontSize.SetVisible(true);

				this.labelColor.SetVisible(false);
				this.fontColor.SetVisible(false);
			}
		}


		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
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

			this.OnChanged();
		}


		protected StaticText				labelColor;
		protected TextFieldCombo			fontName;
		protected Widgets.TextFieldLabel	fontSize;
		protected ColorSample				fontColor;
	}
}
