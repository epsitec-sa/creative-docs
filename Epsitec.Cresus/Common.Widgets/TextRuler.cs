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

			this.fontName = "Toto";
			this.fontSize = 10;

			this.fieldFontName = new TextFieldCombo(this);
			this.fieldFontName.AutoFocus = false;
			this.fieldFontName.IsReadOnly = true;
			this.fieldFontName.Items.Add("Tahoma");
			this.fieldFontName.Items.Add("Arial");
			this.fieldFontName.Items.Add("Courier New");
			this.fieldFontName.Items.Add("Times New Roman");
			this.fieldFontName.SelectedIndexChanged += new Support.EventHandler(this.HandleFieldFontNameChanged);

			this.fieldFontSize = new TextFieldUpDown(this);
			this.fieldFontSize.AutoFocus = false;
			this.fieldFontSize.MinValue =  1.0M;
			this.fieldFontSize.MaxValue = 20.0M;
			this.fieldFontSize.Step = 0.5M;
			this.fieldFontSize.Resolution = 0.1M;
			this.fieldFontSize.TextChanged += new Support.EventHandler(this.HandleFieldFontSizeChanged);

			this.buttonBold = new IconButton(this);
			this.buttonBold.AutoFocus = false;
			this.buttonBold.Text = "<b>G</b>";
			this.buttonBold.Clicked += new MessageEventHandler(this.HandleButtonBoldClicked);

			this.buttonItalic = new IconButton(this);
			this.buttonItalic.AutoFocus = false;
			this.buttonItalic.Text = "<i>I</i>";
			this.buttonItalic.Clicked += new MessageEventHandler(this.HandleButtonItalicClicked);

			this.buttonUnderlined = new IconButton(this);
			this.buttonUnderlined.AutoFocus = false;
			this.buttonUnderlined.Text = "<u>S</u>";
			this.buttonUnderlined.Clicked += new MessageEventHandler(this.HandleButtonUnderlinedClicked);
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
				this.fieldFontSize.TextChanged -= new Support.EventHandler(this.HandleFieldFontSizeChanged);
				this.buttonBold.Clicked -= new MessageEventHandler(this.HandleButtonBoldClicked);
				this.buttonItalic.Clicked -= new MessageEventHandler(this.HandleButtonItalicClicked);
				this.buttonUnderlined.Clicked -= new MessageEventHandler(this.HandleButtonUnderlinedClicked);
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
					   TextRuler.buttonFontSizeWidth +
					   TextRuler.buttonWidth*3 + 5;
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

		public double							FontSize
		{
			get
			{
				return this.fontSize;
			}

			set
			{
				if ( this.fontSize != value )
				{
					this.fontSize = value;
					this.UpdateButtons();

					if ( this.textLayout != null && this.textNavigator != null )
					{
						this.textLayout.SetSelectionFontSize(this.textNavigator.Context, this.fontSize);
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
			}

			this.textLayout    = textLayout;
			this.textNavigator = textNavigator;

			if ( this.textNavigator != null )
			{
				this.textNavigator.CursorChanged += new Epsitec.Common.Support.EventHandler(this.HandleCursorChanged);
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

		private void HandleFieldFontSizeChanged(object sender)
		{
			if ( this.silent )  return;
			this.FontSize = (double) this.fieldFontSize.Value;
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

		private void HandleCursorChanged(object sender)
		{
			this.fontName   = this.textLayout.GetSelectionFontName(this.textNavigator.Context);
			this.fontSize   = this.textLayout.GetSelectionFontSize(this.textNavigator.Context);
			this.bold       = this.textLayout.IsSelectionBold(this.textNavigator.Context);
			this.italic     = this.textLayout.IsSelectionItalic(this.textNavigator.Context);
			this.underlined = this.textLayout.IsSelectionUnderlined(this.textNavigator.Context);
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

			double widthName = rect.Width-(TextRuler.buttonFontSizeWidth+TextRuler.buttonWidth*3+5);
			widthName = System.Math.Max(widthName, TextRuler.buttonFontNameMinWidth);
			widthName = System.Math.Min(widthName, TextRuler.buttonFontNameMaxWidth);
			rect.Width = widthName;
			this.fieldFontName.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonFontSizeWidth;
			this.fieldFontSize.Bounds = rect;

			rect.Offset(rect.Width+3, 0);
			rect.Width = TextRuler.buttonWidth;
			this.buttonBold.Bounds = rect;

			rect.Offset(rect.Width, 0);
			this.buttonItalic.Bounds = rect;

			rect.Offset(rect.Width, 0);
			this.buttonUnderlined.Bounds = rect;
		}

		protected void UpdateButtons()
		{
			this.silent = true;
			this.ComboSelectedName(this.fieldFontName, this.fontName);
			this.fieldFontSize.Value = (decimal) this.fontSize;
			this.ButtonActive(this.buttonBold,       this.bold      );
			this.ButtonActive(this.buttonItalic,     this.italic    );
			this.ButtonActive(this.buttonUnderlined, this.underlined);
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
		protected double					fontSize = 10;
		protected bool						bold = false;
		protected bool						italic = false;
		protected bool						underlined = false;

		protected TextFieldCombo			fieldFontName;
		protected TextFieldUpDown			fieldFontSize;
		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected bool						silent = false;

		protected static readonly double	buttonMargin = 3;
		protected static readonly double	buttonWidth = 20;
		protected static readonly double	buttonFontNameMinWidth = 60;
		protected static readonly double	buttonFontNameMaxWidth = 150;
		protected static readonly double	buttonFontSizeWidth = 45;
		protected static readonly double	zoneSupHeight = TextRuler.buttonMargin*2+TextRuler.buttonWidth;
		protected static readonly double	zoneInfHeight = 15;
	}
}
