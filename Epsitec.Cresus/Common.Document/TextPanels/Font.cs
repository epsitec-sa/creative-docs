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
			this.fontName = new TextFieldCombo(this);
			this.fontName.IsReadOnly = true;
			this.ComboFontFaceList(this.fontName);
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

			Text.Properties.FontProperty     font      = this.textStyle[Common.Text.Properties.WellKnownType.Font]     as Text.Properties.FontProperty;
			Text.Properties.FontSizeProperty fontSize  = this.textStyle[Common.Text.Properties.WellKnownType.FontSize] as Text.Properties.FontSizeProperty;
			Text.Properties.ColorProperty    fontColor = this.textStyle[Common.Text.Properties.WellKnownType.Color]    as Text.Properties.ColorProperty;

			this.ignoreChanged = true;

			if ( font == null )
			{
				this.ComboSelectedName(this.fontName, Res.Strings.Action.Text.Font.Default);
			}
			else
			{
				this.ComboSelectedName(this.fontName, font.FaceName);
			}

			if ( fontSize == null )
			{
				this.fontSize.Text = Res.Strings.Action.Text.Font.Default;
			}
			else
			{
				this.fontSize.TextFieldReal.InternalValue = (decimal) fontSize.Size;
			}

			if ( fontColor == null )
			{
				this.fontColor.Color = RichColor.Empty;
			}
			else
			{
				this.fontColor.Color = RichColor.FromName(fontColor.TextColor);
			}

			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			string fontFace = this.ComboSelectedName(this.fontName);
			if ( fontFace == "" || fontFace == Res.Strings.Action.Text.Font.Default )
			{
			}
			else
			{
				string fontStyle = this.document.SearchDefaultFontStyle(fontFace);
				Text.Properties.FontProperty font = new Text.Properties.FontProperty(fontFace, fontStyle);
			}

			if ( this.fontSize.Text == "" || this.fontSize.Text == Res.Strings.Action.Text.Font.Default )
			{
			}
			else
			{
				double size = (double) this.fontSize.TextFieldReal.InternalValue;
				Text.Properties.FontSizeProperty fontSize = new Text.Properties.FontSizeProperty(size, Common.Text.Properties.SizeUnits.Points);
			}

			if ( this.fontColor.IsEmpty )
			{
			}
			else
			{
				Color basicColor = this.fontColor.Color.Basic;
				string textColor = Color.ToHexa(basicColor);
				Text.Properties.ColorProperty fontColor = new Text.Properties.ColorProperty(textColor);
			}

			// TODO: modifier le style...
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

		// Met à jour la liste d'un champ éditable pour le nom de la police.
		protected void ComboFontFaceList(TextFieldCombo combo)
		{
			if ( combo.Items.Count == 0 )
			{
				combo.Items.Add(Res.Strings.Action.Text.Font.Default);  // par défaut
				Misc.AddFontList(this.document, combo);
			}
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


		// Donne le type.
		protected override Text.Properties.WellKnownType Type
		{
			get { return Common.Text.Properties.WellKnownType.Font; }
		}


		protected StaticText				labelColor;
		protected TextFieldCombo			fontName;
		protected Widgets.TextFieldLabel	fontSize;
		protected ColorSample				fontColor;
	}
}
