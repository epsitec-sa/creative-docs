using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelBool permet de choisir une valeur booléenne.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelBool : AbstractPanel
	{
		public PanelBool(Drawer drawer) : base(drawer)
		{
			this.button = new CheckButton(this);
			this.button.ActiveStateChanged += new EventHandler(this.ButtonActiveStateChanged);
			this.button.TabIndex = 1;
			this.button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.button.ActiveStateChanged -= new EventHandler(this.ButtonActiveStateChanged);
				this.button = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override void SetProperty(AbstractProperty property)
		{
			//	Propriété -> widget.
			base.SetProperty(property);
			this.button.Text = this.textStyle;

			PropertyBool p = property as PropertyBool;
			if ( p == null )  return;

			this.button.ActiveState = p.Bool ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
			PropertyBool p = new PropertyBool();
			base.GetProperty(p);

			p.Bool = ( this.button.ActiveState == WidgetState.ActiveYes );
			return p;
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.button == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);
			this.button.Bounds = rect;
		}
		
		private void ButtonActiveStateChanged(object sender)
		{
			//	Une valeur a été changée.
			this.OnChanged();
		}


		protected CheckButton				button;
	}
}
