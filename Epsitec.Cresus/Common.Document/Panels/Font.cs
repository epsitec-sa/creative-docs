using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Font permet de choisir une police de caractères.
	/// </summary>
	public class Font : Abstract
	{
		public Font(Document document) : base(document)
		{
			this.fontFace = new Widgets.FontFaceCombo(this);
			this.fontFace.IsReadOnly = true;
			this.fontFace.ComboOpening += new EventHandler<CancelEventArgs> (this.HandleFontFaceComboOpening);
			this.fontFace.ComboClosed += this.HandleFontFaceTextChanged;
			this.fontFace.TabIndex = 1;
			this.fontFace.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontFace, Res.Strings.TextPanel.Font.Tooltip.Face);

			if (Command.IsDefined ("TextFontFilter"))
			{
				this.buttonFilter = new IconButton (this);
				this.buttonFilter.CommandObject = Command.Get ("TextFontFilter");
				this.buttonFilter.IconUri = Misc.Icon ("TextFontFilter");
				this.buttonFilter.PreferredIconSize = Misc.IconPreferredSize ("Normal");
				this.buttonFilter.AutoFocus = false;
				this.buttonFilter.ButtonStyle = ButtonStyle.ActivableIcon;
				this.buttonFilter.TabIndex = 2;
				this.buttonFilter.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip (this.buttonFilter, Res.Strings.Action.TextFontFilter);
			}

			this.fontSize = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fontSize.LabelShortText = Res.Strings.Panel.Font.Short.Size;
			this.fontSize.LabelLongText  = Res.Strings.Panel.Font.Long.Size;
			this.document.Modifier.AdaptTextFieldRealFontSize(this.fontSize.TextFieldReal);
			this.fontSize.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fontSize.TabIndex = 3;
			this.fontSize.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontSize, Res.Strings.Panel.Font.Tooltip.Size);

			this.fontColor = new ColorSample(this);
			this.fontColor.DragSourceFrame = true;
			this.fontColor.Clicked += this.HandleFieldColorClicked;
			this.fontColor.ColorChanged += this.HandleFieldColorChanged;
			this.fontColor.TabIndex = 4;
			this.fontColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontColor, Res.Strings.Panel.Font.Tooltip.Color);

			this.labelColor = new StaticText(this);
			this.labelColor.ContentAlignment = ContentAlignment.MiddleRight;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fontFace.ComboOpening -= new EventHandler<CancelEventArgs> (this.HandleFontFaceComboOpening);
				this.fontFace.ComboClosed -= this.HandleFontFaceTextChanged;
				this.fontSize.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fontColor.Clicked -= this.HandleFieldColorClicked;
				this.fontColor.ColorChanged -= this.HandleFieldColorChanged;

				this.labelColor = null;
				this.fontFace = null;
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

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Font p = this.property as Properties.Font;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.fontFace.Text = p.FontName;
			this.fontSize.TextFieldReal.InternalValue = (decimal) p.FontSize;
			this.fontColor.Color = p.FontColor;

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Font p = this.property as Properties.Font;
			if ( p == null )  return;

			p.FontName = this.fontFace.Text;

			if (this.fontSize.TextFieldReal.IsValid)
			{
				p.FontSize = (double) this.fontSize.TextFieldReal.InternalValue;
			}

			p.FontColor = this.fontColor.Color;
		}


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.fontColor.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
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
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fontFace == null )  return;

			this.UpdateShortLongText();

			Rectangle rect = this.UsefulZone;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right-20;
				this.fontFace.SetManualBounds(r);
				this.fontFace.Visibility = true;

				r.Left = rect.Right-20;
				r.Right = rect.Right;
				
				if (this.buttonFilter != null)
				{
					this.buttonFilter.SetManualBounds (r);
					this.buttonFilter.Visibility = true;
				}

				if ( this.IsLabelProperties )
				{
					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right;
					this.fontSize.LabelVisibility = true;
					this.fontSize.SetManualBounds(r);
					this.fontSize.Visibility = true;

					r.Offset(0, -25);
					r.Left = rect.Left;
					r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-Widgets.TextFieldLabel.DefaultMarginWidth;
					this.labelColor.SetManualBounds(r);
					this.labelColor.Visibility = true;
					r.Left = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth;
					r.Right = rect.Right;
					this.fontColor.SetManualBounds(r);
					this.fontColor.Visibility = true;
				}
				else
				{
					r.Offset(0, -25);
					r.Left = rect.Right-this.fontSize.ActualWidth-Widgets.TextFieldLabel.ShortWidth;
					r.Width = this.fontSize.ActualWidth;
					this.fontSize.LabelVisibility = true;
					this.fontSize.SetManualBounds(r);
					this.fontSize.Visibility = true;
					r.Left = r.Right;
					r.Width = Widgets.TextFieldLabel.DefaultLabelWidth;
					this.labelColor.SetManualBounds(r);
					this.labelColor.Visibility = true;
					r.Left = r.Right+Widgets.TextFieldLabel.DefaultMarginWidth;
					r.Width = Widgets.TextFieldLabel.DefaultTextWidth;
					this.fontColor.SetManualBounds(r);
					this.fontColor.Visibility = true;
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;
				r.Right = rect.Right-Widgets.TextFieldLabel.DefaultTextWidth-5;
				this.fontFace.SetManualBounds(r);
				this.fontFace.Visibility = true;

				if (this.buttonFilter != null)
				{
					this.buttonFilter.Visibility = false;
				}

				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fontSize.LabelVisibility = false;
				this.fontSize.SetManualBounds(r);
				this.fontSize.Visibility = true;

				this.labelColor.Visibility = false;
				this.fontColor.Visibility = false;
			}
		}


		private void HandleFontFaceComboOpening(object sender, CancelEventArgs e)
		{
			//	Le combo pour les polices va être ouvert.
			bool quickOnly = this.document.Modifier.ActiveViewer.DrawingContext.TextFontFilter;
			string selectedFontFace = this.fontFace.Text;
			int quickCount;
			System.Collections.ArrayList fontList = Misc.MergeFontList(Misc.GetFontList(false), this.document.Settings.QuickFonts, quickOnly, selectedFontFace, out quickCount);

			this.fontFace.FontList     = fontList;
			this.fontFace.QuickCount   = quickCount;
			this.fontFace.SampleHeight = this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleHeight;
			this.fontFace.SampleAbc    = this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleAbc;
		}

		private void HandleFontFaceTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
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
		protected Widgets.FontFaceCombo		fontFace;
		protected IconButton				buttonFilter;
		protected Widgets.TextFieldLabel	fontSize;
		protected ColorSample				fontColor;
	}
}
