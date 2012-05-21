using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Tension permet de choisir la tension d'un trait à main levée.
	/// </summary>
	public class Tension : Abstract
	{
		public Tension(Document document) : base(document)
		{
			this.fieldTension = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldTension.LabelShortText = Res.Strings.Panel.Tension.Short.Value;
			this.fieldTension.LabelLongText  = Res.Strings.Panel.Tension.Long.Value;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldTension.TextFieldReal);
			this.fieldTension.TextFieldReal.InternalMinValue = 0;
			this.fieldTension.TextFieldReal.InternalMaxValue = 100;
			this.fieldTension.TextFieldReal.Step = 5;
			this.fieldTension.TextFieldReal.TextSuffix = "%";
			this.fieldTension.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldTension.TabIndex = 3;
			this.fieldTension.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldTension, Res.Strings.Panel.Tension.Tooltip.Value);

			this.isNormalAndExtended = false;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldTension.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldTension = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight+30;
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Tension p = this.property as Properties.Tension;
			if ( p == null )  return;

			this.ignoreChanged = true;
			this.fieldTension.TextFieldReal.InternalValue = (decimal) p.TensionValue*100;
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Tension p = this.property as Properties.Tension;
			if ( p == null )  return;

			p.TensionValue = (double) this.fieldTension.TextFieldReal.InternalValue/100;
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fieldTension == null )  return;

			Rectangle r = this.UsefulZone;
			r.Bottom = r.Top-20;
			this.fieldTension.SetManualBounds(r);
		}
		
		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected Widgets.TextFieldLabel	fieldTension;
	}
}
