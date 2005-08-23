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
			this.grid = new Widgets.RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 0;
			this.grid.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(false);
			this.AddRadioIcon(true);
		}
		
		protected void AddRadioIcon(bool type)
		{
			this.grid.AddRadioIcon(Properties.Bool.GetIconText(type), Properties.Bool.GetName(type), type?1:0, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.grid = null;
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
			this.grid.SelectedValue = p.BoolValue ? 1 : 0;
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Bool p = this.property as Properties.Bool;
			if ( p == null )  return;

			p.BoolValue = (this.grid.SelectedValue == 1);
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			Rectangle rect = this.UsefulZone;
			rect.Inflate(1);
			this.grid.Bounds = rect;
		}
		
		// Le type a été changé.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected Widgets.RadioIconGrid		grid;
	}
}
