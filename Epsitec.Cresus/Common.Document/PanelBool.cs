using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PanelBool permet de choisir une valeur bool�enne.
	/// </summary>
	[SuppressBundleSupport]
	public class PanelBool : AbstractPanel
	{
		public PanelBool(Document document) : base(document)
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

		
		// Propri�t� -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			PropertyBool p = this.property as PropertyBool;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.button.Text = p.TextStyle;
			this.button.ActiveState = p.Bool ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.ignoreChanged = false;
		}

		// Widgets -> propri�t�.
		protected override void WidgetsToProperty()
		{
			PropertyBool p = this.property as PropertyBool;
			if ( p == null )  return;

			p.Bool = ( this.button.ActiveState == WidgetState.ActiveYes );
		}


		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.button == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);
			this.button.Bounds = rect;
		}
		
		// Une valeur a �t� chang�e.
		private void HandleButtonActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected CheckButton				button;
	}
}
