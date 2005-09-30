using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Color permet de mettre à l'échelle la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Color : Abstract
	{
		public Color() : base()
		{
			this.title.Text = "Color";

			this.CreateButton(ref this.buttonColorToRGB,  "OperColorToRGB",  Res.Strings.Action.ColorToRGB,  new MessageEventHandler(this.HandleButtonColorToRGB));
			this.CreateButton(ref this.buttonColorToCMYK, "OperColorToCMYK", Res.Strings.Action.ColorToCMYK, new MessageEventHandler(this.HandleButtonColorToCMYK));
			this.CreateButton(ref this.buttonColorToGray, "OperColorToGray", Res.Strings.Action.ColorToGray, new MessageEventHandler(this.HandleButtonColorToGray));
			this.CreateSeparator(ref this.separator);
			this.CreateButton(ref this.buttonColorStrokeDark,  "OperColorStrokeDark",  Res.Strings.Action.ColorStrokeDark,  new MessageEventHandler(this.HandleButtonColorStrokeDark));
			this.CreateButton(ref this.buttonColorStrokeLight, "OperColorStrokeLight", Res.Strings.Action.ColorStrokeLight, new MessageEventHandler(this.HandleButtonColorStrokeLight));
			this.CreateButton(ref this.buttonColorFillDark,    "OperColorFillDark",    Res.Strings.Action.ColorFillDark,    new MessageEventHandler(this.HandleButtonColorFillDark));
			this.CreateButton(ref this.buttonColorFillLight,   "OperColorFillLight",   Res.Strings.Action.ColorFillLight,   new MessageEventHandler(this.HandleButtonColorFillLight));
			this.CreateFieldColor(ref this.fieldColor, Res.Strings.Action.ColorValue);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, gs, document);

			this.AdaptFieldColor(this.fieldColor);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*2 + this.separatorWidth + 22*4;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonColorToRGB == null )  return;

			double dx = this.buttonColorToRGB.DefaultWidth;
			double dy = this.buttonColorToRGB.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonColorToRGB.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonColorToCMYK.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonColorStrokeDark.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonColorStrokeLight.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonColorFillDark.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonColorFillLight.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonColorToGray.Bounds = rect;
			rect.Offset(dx*2+this.separatorWidth, 0);
			rect.Width = 50;
			this.fieldColor.Bounds = rect;
		}


		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			bool enabled = false;

			if ( this.document != null )
			{
				enabled = (this.document.Modifier.TotalSelected > 0);

				if ( this.document.Modifier.Tool == "Edit" )
				{
					enabled = false;
				}
			}

			this.buttonColorToRGB.SetEnabled(enabled);
			this.buttonColorToCMYK.SetEnabled(enabled);
			this.buttonColorToGray.SetEnabled(enabled);
			this.buttonColorStrokeDark.SetEnabled(enabled);
			this.buttonColorStrokeLight.SetEnabled(enabled);
			this.buttonColorFillDark.SetEnabled(enabled);
			this.buttonColorFillLight.SetEnabled(enabled);
		}
		
		// Crée un champ éditable pour une couleur.
		protected void CreateFieldColor(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		// Adapte un champ éditable pour une couleur.
		protected void AdaptFieldColor(TextFieldReal field)
		{
			if ( this.document == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				this.document.Modifier.AdaptTextFieldRealPercent(field);
				field.InternalMinValue = 0.0M;
				field.InternalMaxValue = 1.0M;
				field.DefaultValue = 0.1M;
				field.InternalValue = (decimal) this.document.Modifier.ColorAdjust;
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == this.fieldColor )
			{
				this.document.Modifier.ColorAdjust = (double) field.InternalValue;
			}
		}

		private void HandleButtonColorToRGB(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ColorSelection(ColorSpace.RGB);
		}

		private void HandleButtonColorToCMYK(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ColorSelection(ColorSpace.CMYK);
		}

		private void HandleButtonColorToGray(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ColorSelection(ColorSpace.Gray);
		}

		private void HandleButtonColorStrokeDark(object sender, MessageEventArgs e)
		{
			double adjust = this.document.Modifier.ColorAdjust;
			this.document.Modifier.ColorSelection(-adjust, true);
		}

		private void HandleButtonColorStrokeLight(object sender, MessageEventArgs e)
		{
			double adjust = this.document.Modifier.ColorAdjust;
			this.document.Modifier.ColorSelection(adjust, true);
		}

		private void HandleButtonColorFillDark(object sender, MessageEventArgs e)
		{
			double adjust = this.document.Modifier.ColorAdjust;
			this.document.Modifier.ColorSelection(-adjust, false);
		}

		private void HandleButtonColorFillLight(object sender, MessageEventArgs e)
		{
			double adjust = this.document.Modifier.ColorAdjust;
			this.document.Modifier.ColorSelection(adjust, false);
		}


		protected IconButton				buttonColorToRGB;
		protected IconButton				buttonColorToCMYK;
		protected IconButton				buttonColorToGray;
		protected IconSeparator				separator;
		protected IconButton				buttonColorStrokeDark;
		protected IconButton				buttonColorStrokeLight;
		protected IconButton				buttonColorFillDark;
		protected IconButton				buttonColorFillLight;
		protected TextFieldReal				fieldColor;
	}
}
