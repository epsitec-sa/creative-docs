using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelString permet de choisir une chaîne de caractères.
	/// </summary>
	public class PanelString : AbstractPanel
	{
		public PanelString()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new TextField(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}
		
		public PanelString(Widget embedder) : this()
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

			PropertyString p = property as PropertyString;
			if ( p == null )  return;

			this.field.Text = p.String;
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyString p = new PropertyString();
			base.GetProperty(p);

			p.String = this.field.Text;
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
			r.Right = rect.Right-100;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-100;
			this.field.Bounds = r;
		}
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextField					field;
	}
}
