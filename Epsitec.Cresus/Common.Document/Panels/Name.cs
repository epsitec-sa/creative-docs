using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Name permet de choisir une chaîne de caractères.
	/// </summary>
	[SuppressBundleSupport]
	public class Name : Abstract
	{
		public Name(Document document) : base(document)
		{
			this.field = new TextField(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
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


		// Propriété -> widget.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Name p = this.property as Properties.Name;
			if ( p == null )  return;

			this.ignoreChanged = true;

			if ( this.property.IsMulti )
			{
				this.field.Text = "...";
				this.field.SetEnabled(false);
			}
			else
			{
				this.field.Text = p.String;
				this.field.SetEnabled(true);
			}

			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Name p = this.property as Properties.Name;
			if ( p == null )  return;

			p.String = this.field.Text;
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			this.field.Bounds = r;
		}
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected TextField					field;
	}
}
