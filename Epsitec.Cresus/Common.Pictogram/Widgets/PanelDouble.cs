using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelDouble permet de choisir un nombre réel.
	/// </summary>
	public class PanelDouble : AbstractPanel
	{
		public PanelDouble()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new TextFieldSlider(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}
		
		public PanelDouble(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.TextChanged -= new EventHandler(this.HandleTextChanged);
			}
			
			base.Dispose(disposing);
		}

		
		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.text;

			PropertyDouble p = property as PropertyDouble;
			if ( p == null )  return;

			this.field.Value    = (decimal) p.Value;
			this.field.MinValue = (decimal) p.MinRange;
			this.field.MaxValue = (decimal) p.MaxRange;
			this.field.Step     = (decimal) p.Step;
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyDouble p = new PropertyDouble();
			base.GetProperty(p);

			p.Value    = (double) this.field.Value;
			p.MinRange = (double) this.field.MinValue;
			p.MaxRange = (double) this.field.MaxValue;
			p.Step     = (double) this.field.Step;
			return p;
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth;
			rect.Inflate(-5, -5);

			Drawing.Rectangle r = rect;
			r.Right = rect.Right-50;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-50;
			this.field.Bounds = r;
		}
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldSlider			field;
	}
}
