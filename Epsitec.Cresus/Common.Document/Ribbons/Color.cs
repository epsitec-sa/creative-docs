using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Color permet de modifier les couleurs de la sélection.
	/// </summary>
	public class Color : Abstract
	{
		public Color() : base()
		{
			this.Title = Res.Strings.Action.ColorMain;
			this.PreferredWidth = 8 + 22*2 + this.separatorWidth + 22*2 + 5 + 50;

			this.buttonColorToRgb  = this.CreateIconButton("ColorToRgb");
			this.buttonColorToCmyk = this.CreateIconButton("ColorToCmyk");
			this.buttonColorToGray = this.CreateIconButton("ColorToGray");
			this.separator = new IconSeparator(this);
			this.buttonColorStrokeDark  = this.CreateIconButton("ColorStrokeDark");
			this.buttonColorStrokeLight = this.CreateIconButton("ColorStrokeLight");
			this.buttonColorFillDark    = this.CreateIconButton("ColorFillDark");
			this.buttonColorFillLight   = this.CreateIconButton("ColorFillLight");
			this.CreateFieldColor(ref this.fieldColor, Res.Strings.Action.ColorValue);
			
//			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, DebugMode debug, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, install, debug, gs, document);

			this.AdaptFieldColor(this.fieldColor);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonColorToRgb == null )  return;

			double dx = this.buttonColorToRgb.PreferredWidth;
			double dy = this.buttonColorToRgb.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonColorToRgb.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonColorToCmyk.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonColorStrokeDark.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonColorStrokeLight.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonColorToGray.SetManualBounds(rect);
			rect.Offset(dx*2+this.separatorWidth, 0);
			this.buttonColorFillDark.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonColorFillLight.SetManualBounds(rect);
			rect.Offset(dx+5, dy*0.5);
			rect.Width = 50;
			this.fieldColor.SetManualBounds(rect);
		}


		protected void CreateFieldColor(ref TextFieldReal field, string tooltip)
		{
			//	Crée un champ éditable pour une couleur.
			field = new TextFieldReal(this);
			field.PreferredWidth = 50;
			field.TabIndex = tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		protected void AdaptFieldColor(TextFieldReal field)
		{
			//	Adapte un champ éditable pour une couleur.
			if ( this.document == null )
			{
				field.Enable = false;
			}
			else
			{
				field.Enable = true;

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


		protected IconButton				buttonColorToRgb;
		protected IconButton				buttonColorToCmyk;
		protected IconButton				buttonColorToGray;
		protected IconSeparator				separator;
		protected IconButton				buttonColorStrokeDark;
		protected IconButton				buttonColorStrokeLight;
		protected IconButton				buttonColorFillDark;
		protected IconButton				buttonColorFillLight;
		protected TextFieldReal				fieldColor;
	}
}
