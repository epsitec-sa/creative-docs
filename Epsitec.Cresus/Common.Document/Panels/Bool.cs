using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Bool permet de choisir une valeur booléenne.
	/// </summary>
	[SuppressBundleSupport]
	public class Bool : Abstract
	{
		public Bool(Document document) : base(document)
		{
			this.button = new CheckButton(this);
			this.button.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.button.TabIndex = 1;
			this.button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.button.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.button = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Bool p = this.property as Properties.Bool;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.button.Text = p.TextStyle;
			this.button.ActiveState = p.BoolValue ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Bool p = this.property as Properties.Bool;
			if ( p == null )  return;

			p.BoolValue = ( this.button.ActiveState == WidgetState.ActiveYes );
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.button == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);
			this.button.Bounds = rect;
		}
		
		// Une valeur a été changée.
		private void HandleButtonActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected CheckButton				button;
	}
}
