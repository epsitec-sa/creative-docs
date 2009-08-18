using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Bool permet de choisir une valeur booléenne.
	/// </summary>
	public class Bool : Abstract
	{
		public Bool(Document document) : base(document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += HandleTypeChanged;
			this.grid.TabIndex = 0;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(false);
			this.AddRadioIcon(true);
		}
		
		protected void AddRadioIcon(bool type)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Bool.GetIconText(type)), Properties.Bool.GetName(type), type?1:0, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= HandleTypeChanged;
				this.grid = null;
			}
			
			base.Dispose(disposing);
		}

		
		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Bool p = this.property as Properties.Bool;
			if ( p == null )  return;

			this.ignoreChanged = true;
			this.grid.SelectedValue = p.BoolValue ? 1 : 0;
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Bool p = this.property as Properties.Bool;
			if ( p == null )  return;

			p.BoolValue = (this.grid.SelectedValue == 1);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			Rectangle rect = this.UsefulZone;
			rect.Inflate(1);
			this.grid.SetManualBounds(rect);
		}
		
		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected RadioIconGrid				grid;
	}
}
