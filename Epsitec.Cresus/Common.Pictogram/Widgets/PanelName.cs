using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelName permet de choisir une chaîne de caractères.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelName : AbstractPanel
	{
		public PanelName(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.field = new TextField(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, "Nom de l'objet");
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

			PropertyName p = property as PropertyName;
			if ( p == null )  return;

			if ( this.multi )
			{
				this.field.Text = "...";
				this.field.SetEnabled(false);
			}
			else
			{
				this.field.Text = p.String;
				this.field.SetEnabled(true);
			}
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
			PropertyName p = new PropertyName();
			base.GetProperty(p);

			p.String = this.field.Text;
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
			r.Bottom = r.Top-20;
			r.Right = rect.Right-110;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-110;
			this.field.Bounds = r;
		}
		
		private void HandleTextChanged(object sender)
		{
			//	Une valeur a été changée.
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextField					field;
	}
}
