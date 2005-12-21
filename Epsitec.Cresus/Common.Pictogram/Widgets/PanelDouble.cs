using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelDouble permet de choisir un nombre réel.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelDouble : AbstractPanel
	{
		public PanelDouble(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new TextFieldSlider(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.field = null;
				this.label = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override void SetProperty(AbstractProperty property)
		{
			//	Propriété -> widget.
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyDouble p = property as PropertyDouble;
			if ( p == null )  return;

			this.field.Value    = (decimal) p.Value;
			this.field.MinValue = (decimal) p.MinRange;
			this.field.MaxValue = (decimal) p.MaxRange;
			this.field.Step     = (decimal) p.Step;
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
			PropertyDouble p = new PropertyDouble();
			base.GetProperty(p);

			p.Value    = (double) this.field.Value;
			p.MinRange = (double) this.field.MinValue;
			p.MaxRange = (double) this.field.MaxValue;
			p.Step     = (double) this.field.Step;
			return p;
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Right = rect.Right-50;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-50;
			this.field.Bounds = r;
		}
		
		private void HandleTextChanged(object sender)
		{
			//	Une valeur a été changée.
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldSlider			field;
	}
}
