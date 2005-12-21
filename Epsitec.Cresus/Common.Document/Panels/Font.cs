using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Font permet de choisir une police de caract�res.
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
				Misc.AddFontList(this.fontName, false);
			}
			//?this.fontName.SelectedIndexChanged += new EventHandler(this.HandleFieldChanged);
			this.fontName.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fontName.TabIndex = 1;
			this.fontName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontName, Res.Strings.Panel.Font.Tooltip.Name);

			this.fontSize = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fontSize.LabelShortText = Res.Strings.Panel.Font.Short.Size;
			this.fontSize.LabelLongText  = Res.Strings.Panel.Font.Long.Size;
			this.document.Modifier.AdaptTextFieldRealFontSize(this.fontSize.TextFieldReal);
			this.fontSize.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
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
				this.fontSize.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fontColor.Clicked -= new MessageEventHandler(this.HandleFieldColorClicked);
				this.fontColor.Changed -= new EventHandler(this.HandleFieldColorChanged);

				this.labelColor = null;
				this.fontName = null;
				this.fontSize = null;
				this.fontColor = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau �tendu ?
				{
					if ( this.IsLabelProperties )  // �tendu/d�tails ?
					{
						h += 80;
					}
					else	// �tendu/compact ?
					{
						h += 55;
					}
				}
				else	// panneau r�duit ?
				{
					h += 30;
				}

				return h;
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propri�t� -> widgets.
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

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propri�t�.
			Properties.Font p = this.property as Properties.Font;
			if ( p == null )  return;

			//?string name = this.fontName.SelectedName;
			string name = this.ComboSelectedName(this.fontName);
			if ( name == null )  p.FontName = "";
			else                 p.FontName = name;

			p.FontSize = (double) this.fontSize.TextFieldReal.InternalValue;
			p.FontColor = this.fontColor.Color;
		}


		protected void ComboSelectedName(TextFieldCombo combo, string name)
		{
			//	TODO: TextFieldCombo.SelectedName ne marche pas !!!
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

		protected string ComboSelectedName(TextFieldCombo combo)
		{
			//	TODO: TextFieldCombo.SelectedName ne marche pas !!!
			if ( combo.SelectedIndex == -1 )  return "";
			return combo.Items[combo.SelectedIndex] as string;
		}


		public override void OriginColorDeselect()
		{
			//	D�s�lectionne toutes les origines de couleurs possibles.
			this.fontColor.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	S�lectionne l'origine de couleur.
			this.fontColor.ActiveState = ActiveState.Yes;
		}

		public override void OriginColorChange(Drawing.RichColor color)
		{
			//	Modifie la couleur d'origine.
			if ( this.fontColor.Color != color )
			{
				this.fontColor.Color = color;
				this.OnChanged();
			}
		}

		public override Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return this.fontColor.Color;
		}

		
		protected void UpdateShortLongText()
		{
			//	Adapte les textes courts ou longs.
			if ( this.IsLabelProperties )
			{
				this.labelColor.Text = Res.Strings.Panel.Font.Long.Color + " ";
			}
			else
			{
				this.labelColor.Text = Res.Strings.Panel.Font.Short.Color;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.fontName == null )  return;

			this.UpdateShortLongText();

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				this.fontName.Bounds = r;
				this.fontName.Visibility = true;

				if ( this.IsLabelProperties )
				{
					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fontSize.LabelVisibility = true;
					this.fontSize.Bounds = r;
					this.fontSize.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-Widgets.TextFieldLabel.DefaultMarginWidth;
					this.labelColor.Bounds = r;
					this.labelColor.Visibility = true;
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					r.Right = rect.Right;
					this.fontColor.Bounds = r;
					this.fontColor.Visibility = true;
				}
				else
				{
					r.Offset(0, -25);
					r.Left = rect.Right-this.fontSize.Width-Widgets.TextFieldLabel.ShortWidth;
					r.Width = this.fontSize.Width;
					this.fontSize.LabelVisibility = true;
					this.fontSize.Bounds = r;
					this.fontSize.Visibility = true;
					r.Left = r.Right;
					r.Width = Widgets.TextFieldLabel.DefaultLabelWidth;
					this.labelColor.Bounds = r;
					this.labelColor.Visibility = true;
					r.Left = r.Right+Widgets.TextFieldLabel.DefaultMarginWidth;
					r.Width = Widgets.TextFieldLabel.DefaultTextWidth;
					this.fontColor.Bounds = r;
					this.fontColor.Visibility = true;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-5;
				this.fontName.Bounds = r;
				this.fontName.Visibility = true;

				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fontSize.LabelVisibility = false;
				this.fontSize.Bounds = r;
				this.fontSize.Visibility = true;

				this.labelColor.Visibility = false;
				this.fontColor.Visibility = false;
			}
		}


		private void HandleFieldChanged(object sender)
		{
			//	Un champ a �t� chang�.
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
			if ( cs.ActiveState == ActiveState.Yes )
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
