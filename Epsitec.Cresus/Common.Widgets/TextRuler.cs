namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class TextRuler représente une règle pour un pavé de texte.
	/// </summary>
	public class TextRuler : Widget
	{
		public TextRuler()
		{
			this.AutoFocus = false;

			this.fieldFontName = new TextFieldCombo(this);
			this.fieldFontName.AutoFocus = false;
			this.fieldFontName.IsReadOnly = true;
			this.fieldFontName.Items.Add("Par défaut...");
			this.fieldFontName.Items.Add("Tahoma");
			this.fieldFontName.Items.Add("Arial");
			this.fieldFontName.Items.Add("Courier New");
			this.fieldFontName.Items.Add("Times New Roman");
			this.fieldFontName.SelectedIndexChanged += new Support.EventHandler(this.HandleFieldFontNameChanged);
			ToolTip.Default.SetToolTip(this.fieldFontName, "Police de caractère");

			this.fieldFontScale = new TextFieldUpDown(this);
			this.fieldFontScale.AutoFocus = false;
			this.fieldFontScale.MinValue =   10.0M;
			this.fieldFontScale.MaxValue = 1000.0M;
			this.fieldFontScale.Step = 10.0M;
			this.fieldFontScale.Resolution = 1.0M;
			this.fieldFontScale.TextChanged += new Support.EventHandler(this.HandleFieldFontScaleChanged);
			ToolTip.Default.SetToolTip(this.fieldFontScale, "Taille en pour-cent");

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

			this.fieldFontColor = new TextFieldCombo(this);
			this.fieldFontColor.AutoFocus = false;
			this.fieldFontColor.IsReadOnly = true;
			this.fieldFontColor.Items.Add("Par défaut...");
			this.fieldFontColor.Items.Add("Noir");
			this.fieldFontColor.Items.Add("Rouge");
			this.fieldFontColor.Items.Add("Bleu");
			this.fieldFontColor.Items.Add("Magenta");
			this.fieldFontColor.Items.Add("Vert");
			this.fieldFontColor.Items.Add("Cyan");
			this.fieldFontColor.Items.Add("Jaune");
			this.fieldFontColor.Items.Add("Blanc");
			this.fieldFontColor.SelectedIndexChanged += new Support.EventHandler(this.HandleFieldFontColorChanged);
			ToolTip.Default.SetToolTip(this.fieldFontColor, "Couleur");
		}
		
		public TextRuler(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.DetachFromText();
				this.fieldFontName.SelectedIndexChanged -= new Support.EventHandler(this.HandleFieldFontNameChanged);
				this.fieldFontScale.TextChanged -= new Support.EventHandler(this.HandleFieldFontScaleChanged);
				this.buttonBold.Clicked -= new MessageEventHandler(this.HandleButtonBoldClicked);
				this.buttonItalic.Clicked -= new MessageEventHandler(this.HandleButtonItalicClicked);
				this.buttonUnderlined.Clicked -= new MessageEventHandler(this.HandleButtonUnderlinedClicked);
				this.fieldFontColor.SelectedIndexChanged -= new Support.EventHandler(this.HandleFieldFontColorChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public double							MinimalWidth
		{
			// Retourne la largeur minimale d'une règle.
			get
			{
				return TextRuler.buttonMargin*2 +
					   TextRuler.buttonFontNameMinWidth +
					   TextRuler.buttonFontScaleWidth +
					   TextRuler.buttonWidth*3 +
					   TextRuler.buttonFontColorWidth + 8;
			}
		}

		public override double					DefaultWidth
		{
			// Retourne la largeur standard d'une règle.
			get
			{
				return 200;
			}
		}

		public override double					DefaultHeight
		{
			// Retourne la hauteur standard d'une règle.
			get
			{
				return TextRuler.zoneSupHeight + TextRuler.zoneInfHeight;
			}
		}

		public string							FontName
		{
			get
			{
				return this.fontName;
			}

			set
			{
				if ( this.fontName != value )
				{
					this.fontName = value;
					this.UpdateButtons();

					if ( this.textLayout != null && this.textNavigator != null )
					{
						this.textLayout.SetSelectionFontName(this.textNavigator.Context, this.fontName);
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public double							FontScale
		{
			get
			{
				return this.fontScale;
			}

			set
			{
				if ( this.fontScale != value )
				{
					this.fontScale = value;
					this.UpdateButtons();

					if ( this.textLayout != null && this.textNavigator != null )
					{
						this.textLayout.SetSelectionFontScale(this.textNavigator.Context, this.fontScale);
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public bool								Bold
		{
			// Attribut typographique "gras".
			get
			{
				return this.bold;
			}

			set
			{
				if ( this.bold != value )
				{
					this.bold = value;
					this.UpdateButtons();

					if ( this.textLayout != null && this.textNavigator != null )
					{
						this.textLayout.SetSelectionBold(this.textNavigator.Context, this.bold);
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public bool								Italic
		{
			// Attribut typographique "italique".
			get
			{
				return this.italic;
			}

			set
			{
				if ( this.italic != value )
				{
					this.italic = value;
					this.UpdateButtons();

					if ( this.textLayout != null && this.textNavigator != null )
					{
						this.textLayout.SetSelectionItalic(this.textNavigator.Context, this.italic);
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public bool								Underlined
		{
			// Attribut typographique "souligné".
			get
			{
				return this.underlined;
			}

			set
			{
				if ( this.underlined != value )
				{
					this.underlined = value;
					this.UpdateButtons();

					if ( this.textLayout != null && this.textNavigator != null )
					{
						this.textLayout.SetSelectionUnderlined(this.textNavigator.Context, this.underlined);
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}

		public Drawing.Color					FontColor
		{
			get
			{
				return this.fontColor;
			}

			set
			{
				if ( this.fontColor != value )
				{
					this.fontColor = value;
					this.UpdateButtons();

					if ( this.textLayout != null && this.textNavigator != null )
					{
						this.textLayout.SetSelectionFontColor(this.textNavigator.Context, this.fontColor);
					}

					if ( this.textField != null )
					{
						this.textField.Invalidate();
					}
				}
			}
		}


		public void AttachToText(AbstractTextField text)
		{
			this.textField = text;
			this.AttachToText(text.TextLayout, text.TextNavigator);
		}

		public void AttachToText(TextLayout textLayout, TextNavigator textNavigator)
		{
			// Lie la règle à un texte éditable.
			if ( this.textLayout    == textLayout    &&
				 this.textNavigator == textNavigator )  return;

			if ( this.textNavigator != null )
			{
				this.textNavigator.CursorChanged -= new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextInserted -= new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextDeleted -= new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
			}

			this.textLayout    = textLayout;
			this.textNavigator = textNavigator;

			if ( this.textNavigator != null )
			{
				this.textNavigator.CursorChanged += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextInserted += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.textNavigator.TextDeleted += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
				this.HandleCursorChanged(null);
			}
		}
		
		public void DetachFromText()
		{
			this.textField = null;
			this.AttachToText(null, null);
		}
		
		
		private void HandleFieldFontNameChanged(object sender)
		{
			if ( this.silent )  return;
			this.FontName = this.ComboSelectedName(this.fieldFontName);
			this.OnChanged();
		}

		private void HandleFieldFontScaleChanged(object sender)
		{
			if ( this.silent )  return;
			this.FontScale = (double) this.fieldFontScale.Value / 100;
			this.OnChanged();
		}

		private void HandleButtonBoldClicked(object sender, MessageEventArgs e)
		{
			if ( this.silent )  return;
			this.Bold = !this.Bold;
			this.OnChanged();
		}

		private void HandleButtonItalicClicked(object sender, MessageEventArgs e)
		{
			if ( this.silent )  return;
			this.Italic = !this.Italic;
			this.OnChanged();
		}

		private void HandleButtonUnderlinedClicked(object sender, MessageEventArgs e)
		{
			if ( this.silent )  return;
			this.Underlined = !this.Underlined;
			this.OnChanged();
		}

		private void HandleFieldFontColorChanged(object sender)
		{
			if ( this.silent )  return;
			this.FontColor = this.ComboSelectedColor(this.fieldFontColor);
			this.OnChanged();
		}

		private void HandleCursorChanged(object sender)
		{
			this.fontName   = this.textLayout.GetSelectionFontName(this.textNavigator.Context);
			this.fontScale  = this.textLayout.GetSelectionFontScale(this.textNavigator.Context);
			this.bold       = this.textLayout.IsSelectionBold(this.textNavigator.Context);
			this.italic     = this.textLayout.IsSelectionItalic(this.textNavigator.Context);
			this.underlined = this.textLayout.IsSelectionUnderlined(this.textNavigator.Context);
			this.fontColor  = this.textLayout.GetSelectionFontColor(this.textNavigator.Context);
			this.UpdateButtons();
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateGeometry();
		}

		protected void UpdateGeometry()
		{
			if ( this.buttonBold == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Bottom += TextRuler.zoneInfHeight;
			rect.Deflate(TextRuler.buttonMargin);

			double widthName = rect.Width-(TextRuler.buttonFontScaleWidth+TextRuler.buttonWidth*3+TextRuler.buttonFontColorWidth+8);
			widthName = System.Math.Max(widthName, TextRuler.buttonFontNameMinWidth);
			widthName = System.Math.Min(widthName, TextRuler.buttonFontNameMaxWidth);
			rect.Width = widthName;
			this.fieldFontName.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonFontScaleWidth;
			this.fieldFontScale.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonWidth;
			this.buttonBold.Bounds = rect;

			rect.Offset(rect.Width, 0);
			this.buttonItalic.Bounds = rect;

			rect.Offset(rect.Width, 0);
			this.buttonUnderlined.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonFontColorWidth;
			this.fieldFontColor.Bounds = rect;
		}

		protected void UpdateButtons()
		{
			this.silent = true;
			this.ComboSelectedName(this.fieldFontName, this.fontName);
			this.fieldFontScale.Value = (decimal) this.fontScale * 100;
			this.ButtonActive(this.buttonBold,       this.bold      );
			this.ButtonActive(this.buttonItalic,     this.italic    );
			this.ButtonActive(this.buttonUnderlined, this.underlined);
			this.ComboSelectedColor(this.fieldFontColor, this.fontColor);
			this.silent = false;
		}

		protected void ButtonActive(Button button, bool active)
		{
			button.ActiveState = active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine la règle.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(adorner.ColorWindow);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.AddLine(rect.Left, rect.Bottom+TextRuler.zoneInfHeight, rect.Right, rect.Bottom+TextRuler.zoneInfHeight);
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}


		protected void ComboSelectedName(TextFieldCombo combo, string name)
		{
			if ( name == "" )
			{
				combo.SelectedIndex = 0;
			}
			else
			{
				int total = combo.Items.Count;
				for ( int i=1 ; i<total ; i++ )
				{
					string s = combo.Items[i];
					if ( s == name )
					{
						combo.SelectedIndex = i;
						return;
					}
				}
			}
		}

		protected string ComboSelectedName(TextFieldCombo combo)
		{
			if ( combo.SelectedIndex <= 0 )  return "";
			return combo.Items[combo.SelectedIndex] as string;
		}


		protected Drawing.Color ComboSelectedColor(TextFieldCombo combo)
		{
			if ( combo.SelectedIndex == 1 )  return Drawing.Color.FromRGB(0,0,0);
			if ( combo.SelectedIndex == 2 )  return Drawing.Color.FromRGB(1,0,0);
			if ( combo.SelectedIndex == 3 )  return Drawing.Color.FromRGB(0,0,1);
			if ( combo.SelectedIndex == 4 )  return Drawing.Color.FromRGB(1,0,1);
			if ( combo.SelectedIndex == 5 )  return Drawing.Color.FromRGB(0,1,0);
			if ( combo.SelectedIndex == 6 )  return Drawing.Color.FromRGB(0,1,1);
			if ( combo.SelectedIndex == 7 )  return Drawing.Color.FromRGB(1,1,0);
			if ( combo.SelectedIndex == 8 )  return Drawing.Color.FromRGB(1,1,1);
			return Drawing.Color.Empty;
		}

		protected void ComboSelectedColor(TextFieldCombo combo, Drawing.Color color)
		{
			combo.SelectedIndex = 0;
			if ( color == Drawing.Color.FromRGB(0,0,0) )  combo.SelectedIndex = 1;
			if ( color == Drawing.Color.FromRGB(1,0,0) )  combo.SelectedIndex = 2;
			if ( color == Drawing.Color.FromRGB(0,0,1) )  combo.SelectedIndex = 3;
			if ( color == Drawing.Color.FromRGB(1,0,1) )  combo.SelectedIndex = 4;
			if ( color == Drawing.Color.FromRGB(0,1,0) )  combo.SelectedIndex = 5;
			if ( color == Drawing.Color.FromRGB(0,1,1) )  combo.SelectedIndex = 6;
			if ( color == Drawing.Color.FromRGB(1,1,0) )  combo.SelectedIndex = 7;
			if ( color == Drawing.Color.FromRGB(1,1,1) )  combo.SelectedIndex = 8;
		}


		// Génère un événement pour dire que la règle a changé.
		protected void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}

		
		
		public event Support.EventHandler	Changed;


		protected AbstractTextField			textField;
		protected TextLayout				textLayout;
		protected TextNavigator				textNavigator;
		protected string					fontName = "";
		protected double					fontScale = 1;
		protected bool						bold = false;
		protected bool						italic = false;
		protected bool						underlined = false;
		protected Drawing.Color				fontColor = Drawing.Color.Empty;

		protected TextFieldCombo			fieldFontName;
		protected TextFieldUpDown			fieldFontScale;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected TextFieldCombo			fieldFontColor;
		protected bool						silent = false;

		protected static readonly double	buttonMargin = 3;
		protected static readonly double	buttonWidth = 20;
		protected static readonly double	buttonFontNameMinWidth = 60;
		protected static readonly double	buttonFontNameMaxWidth = 150;
		protected static readonly double	buttonFontScaleWidth = 45;
		protected static readonly double	buttonFontColorWidth = 90;
		protected static readonly double	zoneSupHeight = TextRuler.buttonMargin*2+TextRuler.buttonWidth;
		protected static readonly double	zoneInfHeight = 15;
	}
}
