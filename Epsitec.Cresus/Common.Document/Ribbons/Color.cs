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
			this.title.Text = Res.Strings.Action.ColorMain;

			this.buttonColorToRGB  = this.CreateIconButton("ColorToRGB",  Misc.Icon("OperColorToRGB"),  Res.Strings.Action.ColorToRGB);
			this.buttonColorToCMYK = this.CreateIconButton("ColorToCMYK", Misc.Icon("OperColorToCMYK"), Res.Strings.Action.ColorToCMYK);
			this.buttonColorToGray = this.CreateIconButton("ColorToGray", Misc.Icon("OperColorToGray"), Res.Strings.Action.ColorToGray);
			this.separator = new IconSeparator(this);
			this.buttonColorStrokeDark  = this.CreateIconButton("ColorStrokeDark",  Misc.Icon("OperColorStrokeDark"),  Res.Strings.Action.ColorStrokeDark);
			this.buttonColorStrokeLight = this.CreateIconButton("ColorStrokeLight", Misc.Icon("OperColorStrokeLight"), Res.Strings.Action.ColorStrokeLight);
			this.buttonColorFillDark    = this.CreateIconButton("ColorFillDark",    Misc.Icon("OperColorFillDark"),    Res.Strings.Action.ColorFillDark);
			this.buttonColorFillLight   = this.CreateIconButton("ColorFillLight",   Misc.Icon("OperColorFillLight"),   Res.Strings.Action.ColorFillLight);
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
				return 8 + 22*2 + this.separatorWidth + 22*2 + 50;
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

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonColorToGray.Bounds = rect;
			rect.Offset(dx*2+this.separatorWidth, 0);
			this.buttonColorFillDark.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonColorFillLight.Bounds = rect;
			rect.Offset(dx, dy*0.5);
			rect.Width = 50;
			this.fieldColor.Bounds = rect;
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
