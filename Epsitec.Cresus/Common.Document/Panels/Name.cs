using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Name permet de choisir une cha�ne de caract�res.
	/// </summary>
	public class Name : Abstract
	{
		public Name(Document document) : base(document)
		{
			this.field = new TextField(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, Res.Strings.Panel.Name.Tooltip.Title);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.field = null;
			}
			
			base.Dispose(disposing);
		}


		protected override void PropertyToWidgets()
		{
			//	Propri�t� -> widget.
			base.PropertyToWidgets();

			Properties.Name p = this.property as Properties.Name;
			if ( p == null )  return;

			this.ignoreChanged = true;

			if ( this.property.IsMulti )
			{
				this.field.Text = "...";
				this.field.Enable = false;
			}
			else
			{
				this.field.Text = p.String;
				this.field.Enable = true;
			}

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propri�t�.
			Properties.Name p = this.property as Properties.Name;
			if ( p == null )  return;

			p.String = this.field.Text;
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			this.field.SetManualBounds(r);
		}
		
		private void HandleTextChanged(object sender)
		{
			//	Une valeur a �t� chang�e.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected TextField					field;
	}
}
